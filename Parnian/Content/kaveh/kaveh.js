(function ($, bootbox, Dropzone) {

    /* within current scope, I use word 'KFSI' for any view box item,
     * and it stands for 'Kaveh File System Info'. */

    // enabling 'Strict Mode'.
    "use strict";

    if (typeof $ == "undefined") throw new Error("Kaveh File Manager requires jQuery");
    //if (typeof bootstrap == "undefined") throw new Error("Kaveh File Manager requires Bootstrap");
    if (typeof bootbox == "undefined") throw new Error("Kaveh File Manager requires BootBox");
    if (typeof Dropzone == "undefined") throw new Error("Kaveh File Manager requires Dropzone");

    // declare root variable - window in the browser, global on the server.
    var root = this,
        previous = root.KavehFileManager,
        KFM = function (settings) {

            // define properties

            this.options = extendObject(KFM.defaults, settings);

            this.bootstraped = false;

            this.location = {
                index: -1,
                current: this.options.home,
                parent: "",
                back: "",
                forth: ""
            };

            this.dom = {};

            this.dropzone = {};

            this.history = [];

            this.id = uid();

            this.navPaneContent = {};

            this.text = KFM.dictionary[this.options.lang];

            this.viewBoxContent = [];

            // initialize

            if (this.options.auto) { this.bootstrap(); }

            // finally

            KFM.instances[this.id] = this;

            return this;
        };

    // public methods
    //////////////////////////////////////////////////////////////////////////////

    KFM.prototype.addNewItem = function (type, name) {
        var _ = this;

        ajax({
            data: { type: type, name: name, parent: _.location.current },
            method: "POST",
            url: KFM.server.addNewItemAction,
            success: function (response) {
                if (response.status == 0) {
                    _.refreshViewBox();
                    if (type == KFM.fileTypes.folder) _.updateNavPane();
                } else if (response.status == 1 || response.status == 2) {
                    bootbox.alert(response.message);
                }
            }
        });
    }

    KFM.prototype.bootstrap = function () {
        var _ = this;

        _.initializeDOM();

        if (_.dom.hasOwnProperty("navPane")) { _.updateNavPane(); }

        if (_.dom.hasOwnProperty("viewBox")) { _.updateViewBox(_.location.current, ""); }

        _.initializeEvents();

        _.bootstraped = true;
    };

    KFM.prototype.close = function () {
        this.dom.body.style.display = "none";
    };

    KFM.prototype.createNavPane = function (object) {
        if (!this.dom.hasOwnProperty("navPane")) return;

        //this.dom.navPane.appendChild(createNavPaneItem(object, 0));
        var ul = document.createElement("ul");
        ul.className = "k-accordion k-accordion-open";
        ul.appendChild(createNavPaneItem2(object, 1));
        this.dom.navPane.appendChild(ul);
    };

    KFM.prototype.createViewBox = function (sortkey, order) {
        var _ = this,
            dom = _.dom,
            list = _.viewBoxContent;

        if (!dom.hasOwnProperty("viewBox")) return;

        if (dom.hasOwnProperty("statusBar")) dom.status.innerHTML = list.length + " مورد";

        if (list.length == 0) {
            dom.boxMessage.innerHTML = KFM.dictionary[_.options.lang].emptyFolderInfo;
            dom.boxMessage.style.display = "block";
            return;
        }

        dom.boxMessage.style.display = "none";

        if (!KFM.kfsi.hasOwnProperty(sortkey)) sortkey = KFM.kfsi.isDirectory;
        if (order != 1 && order != -1) order = 1;
        sort(list, order, function (a) { return a[sortkey]; });

        list.forEach(function (kfsi, index) {
            kfsi.isSelected = false;
            kfsi.type = "";
            dom.viewBox.appendChild(createViewBoxItem(kfsi, index));
        });
    };

    KFM.prototype.kfsiProperty = function (id, property, value) {
        var _ = this;
        if (KFM.kfsi.hasOwnProperty(property)) {
            var kfsi = _.viewBoxContent[id]
            if (typeof value != "undefined") {
                kfsi[property] = value;
                return;
            } else {
                return kfsi[property];
            }
        }
    }

    KFM.prototype.getNavPaneContent = function (settings) {
        var _ = this,
            opts = extendObject({
                deepDive: true,
                path: "~/",
                success: function (response) {
                    if (response.status == 0) {
                        _.navPaneContent = response.data;
                        _.createNavPane(response.data);
                    } else if (response.status == 1) {
                        bootbox.alert(response.message);
                    }
                }
            }, settings);

        ajax({
            method: "GET",
            url: KFM.server.getNavPaneContentAction,
            data: { path: opts.path, exclude: opts.exclude, deepDive: opts.deepDive },
            success: opts.success
        });

        //return _.navPaneContent;
    };

    KFM.prototype.getViewBoxContent = function (settings) {
        var _ = this,
            opts = extendObject({
                exclude: "",
                path: _.location.current,
                success: function (response) {
                    if (response.status == 0) {
                        _.viewBoxContent = response.data.list;
                        _.updateAddressBar(/*path, direction*/);
                        _.createViewBox();
                    } else {
                        bootbox.alert(response.message);
                        _.updateViewBox("/");
                    }
                }
            }, settings);

        ajax({
            method: "GET",
            url: KFM.server.getViewBoxContentAction,
            data: { path: opts.path, exclude: opts.exclude },
            success: opts.success
        });

        //return _.viewBoxContent;
    };

    KFM.prototype.initializeDOM = function () {
        var _ = this;

        if (_.options.scaffoldBody) { _.dom = _.scafold(); return; }

        _.dom.body = document.getElementById(_.options.id);

        if (!_.dom.body) {
            alert('پنل مدیریت فایل یافت نشد!');
            return;
        }

        var body = _.dom.body,
            cls = KFM.classes;

        // tool bar -----------------------------------------

        var toolBar = body.querySelectorAll("." + cls.toolBar)[0];

        if (toolBar) {
            _.dom.toolBar = toolBar;
        }

        // adderss bar --------------------------------------

        var addressBar = body.querySelectorAll("." + cls.addressBar)[0];

        if (addressBar) {
            _.dom.addressBar = addressBar;

            _.dom.historyBackBtn = addressBar.querySelectorAll("." + cls.historyBackBtn)[0];

            _.dom.historyForwardBtn = addressBar.querySelectorAll("." + cls.historyForwardBtn)[0];

            _.dom.parentDirBtn = addressBar.querySelectorAll("." + cls.parentDirBtn)[0];

            _.dom.breadcrumb = addressBar.querySelectorAll("." + cls.breadcrumb)[0];
        }

        // nav pane -----------------------------------------

        var navPane = body.querySelectorAll("." + cls.navPane)[0];

        if (navPane) {
            _.dom.navPane = navPane;
        }

        // view box -----------------------------------------

        var viewBox = body.querySelectorAll("." + cls.viewBox)[0];

        if (viewBox) {
            _.dom.viewBox = viewBox;

            _.dom.boxMessage = body.querySelectorAll("." + cls.boxMessage)[0];
        }

        // status bar ---------------------------------------

        var statusBar = body.querySelectorAll("." + cls.statusBar)[0];

        if (statusBar) {
            _.dom.statusBar = statusBar;

            _.dom.status = statusBar.querySelectorAll("." + cls.status)[0];
        }

        // choose bar ---------------------------------------

        var chooseBar = body.querySelectorAll("." + cls.chooseBar)[0];

        if (chooseBar) {
            _.dom.chooseBar = chooseBar;
        }

        // upload --------------------------------------------

        var upload = body.querySelectorAll("." + cls.uploadSection)[0];

        if (upload) {
            _.dom.uploadSection = upload;

            _.dom.uploadForm = upload.querySelectorAll("." + cls.uploadForm)[0];

            Dropzone.autoDiscover = false;

            _.dropzone = new Dropzone(_.dom.uploadForm, {
                url: KFM.server.uploadAction + "?parent=" + _.location.current,
                uploadMultiple: true,
                autoProcessQueue: false,
                parallelUploads: 4,
                maxFilesize: 20,
                addRemoveLinks: true,
                init: function () { },
                dictDefaultMessage: "فایلا رو بندازید اینجا",
                dictFallbackMessage: "مرورگر شما از قابلیت بگیر و بنداز پشتیبانی نمی‌کند.",
                dictFallbackText: "Please use the fallback form below to upload your files like in the olden days.",
                dictFileTooBig: "فایل خیلی بزرگه ({{filesize}}MiB). حداکثر حجم مجاز: {{maxFilesize}}MiB است.",
                dictInvalidFileType: "You can't upload files of this type.",
                dictResponseError: "پاسخ سرور:‌ کد {{statusCode}}.",
                dictCancelUpload: "لغو بارگذاری",
                dictCancelUploadConfirmation: "توقف بارگذاری؟",
                dictRemoveFile: "حذف فایل از لیست",
                dictRemoveFileConfirmation: "فایل از لیست حذف شود؟",
                dictMaxFilesExceeded: "نمی‌توانید بیشتر از این بارگذاری کنید.",
            });
        }

        // modal ---------------------------------------------

        if (typeof _.options.modalId == "string") {
            var modal = document.getElementById(_.options.modalId);
            if (modal != null) _.dom.modal = modal;
        }
    };

    KFM.prototype.initializeEvents = function () {
        var _ = this,
            dom = _.dom,
            toolBar = $(dom.toolBar),
            addressBar = $(dom.addressBar),
            navPane = $(dom.navPane),
            viewBox = $(dom.viewBox),
            statusBar = $(dom.statusBar),
            chooseBar = $(dom.chooseBar),
            uploadSection = $(dom.uploadSection),
            opts = _.options,
            cls = KFM.classes,
            atr = KFM.attrs,
            text = KFM.dictionary[opts.lang];

        // tool bar -----------------------------------------

        if (toolBar) {

            toolBar.on("click", "." + cls.removeBtn, function () {
                var selectList = _.viewBoxContent.filter(function (i) { return i[KFM.kfsi.isSelected]; }),
                    L = selectList.length;

                if (L == 0) { bootbox.alert(text.nothingSelected); return; }

                var selectedFileCount = 0,
                    selectedFolderCount = 0,
                    msg = text.removeConfirm,
                    data = new FormData();

                while (L--) {
                    var item = selectList[L],
                        type = "";
                    if (item.isDirectory) { selectedFolderCount++; type = "folder"; }
                    else { selectedFileCount++; type = "file"; }
                    selectList[L] = { id: item.id, type: type, url: item.path };
                }

                if (selectedFileCount != 0) msg += "<br/>" + selectedFileCount + " " + text.file;
                if (selectedFolderCount != 0) msg += "<br/>" + selectedFolderCount + " " + text.folder;

                data.append("json", JSON.stringify(selectList));

                bootbox.confirm(msg, function (confirmed) {
                    if (confirmed) {
                        ajax({
                            dataType: "json",
                            url: KFM.server.removeAction,
                            data: data,
                            contentType: false,
                            processData: false,
                            success: function () { _.refreshViewBox(); },
                            //timeout: 10000,
                        });
                    }
                });
            });

            toolBar.on("click", "." + cls.renameBtn, function () {
                var selectList = _.viewBoxContent.filter(function (i) { return i[KFM.kfsi.isSelected]; }),
                    L = selectList.length;

                if (L == 0) { bootbox.alert(text.nothingSelected); return; }

                bootbox.prompt({
                    title: "تغییر نام",
                    value: "new name",
                    callback: function (newName) {
                        if (newName !== null) {
                            var selectedFileCount = 0,
                                selectedFolderCount = 0,
                                msg = text.renameConfirm,
                                data = new FormData();

                            while (L--) {
                                var item = selectList[L],
                                    type = "";
                                if (item.isDirectory) { selectedFolderCount++; type = "folder"; }
                                else { selectedFileCount++; type = "file"; }
                                selectList[L] = { id: item.id, type: type, url: item.path };
                            }

                            if (selectedFileCount != 0) msg += "<br/>" + selectedFileCount + " " + text.file;
                            if (selectedFolderCount != 0) msg += "<br/>" + selectedFolderCount + " " + text.folder;

                            data.append("newName", newName);
                            data.append("json", JSON.stringify(selectList));

                            bootbox.confirm(msg, function (confirmed) {
                                if (confirmed) {
                                    ajax({
                                        dataType: "json",
                                        url: KFM.server.renameAction,
                                        data: data,
                                        contentType: false,
                                        processData: false,
                                        success: function () { _.refreshViewBox(); },
                                        //timeout: 10000,
                                    });
                                }
                            });
                        }
                    }
                });
            });

            toolBar.on("click", "." + cls.uploadBtn, function () {
                dom.uploadSection.style.display = "block";
                _.dropzone.options.url = KFM.server.uploadAction + "?parent=" + _.location.current;
            });

            toolBar.on("click", "." + cls.linkBtn, function () {
                var selectList = _.viewBoxContent.filter(function (i) { return i[KFM.kfsi.isSelected]; }),
                    L = selectList.length;

                if (L == 0) { bootbox.alert(text.nothingSelected); return; }

                prompt(text.copyUrlPrompt + "  Ctrl + C, Enter", selectList[0].path);
            });

            toolBar.on("click", "." + cls.downloadBtn, function () {
                var selectList = _.viewBoxContent.filter(function (i) { return i[KFM.kfsi.isSelected]; }),
                    L = selectList.length;

                if (L == 0) { bootbox.alert(text.nothingSelected); return; }

                bootbox.prompt({
                    title: text.enterDownloadFileName,
                    value: "download",
                    callback: function (result) {
                        while (L--) {
                            var item = selectList[L],
                                type = "";
                            if (item.isDirectory) { type = "folder"; }
                            else { type = "file"; }
                            selectList[L] = { id: item.name, type: type, url: item.path };
                            window.location = KFM.server.downloadAction + "?json=" + JSON.stringify(selectList) + "&downloadName=" + result;
                        }
                    }
                });
            });

            toolBar.on("click", "." + cls.downloadLinkBtn, function () {
                var selectList = _.viewBoxContent.filter(function (i) { return i[KFM.kfsi.isSelected]; }),
                    L = selectList.length;

                if (L == 0) { bootbox.alert(text.nothingSelected); return; }

                prompt(text.copyUrlPrompt + "  Ctrl + C, Enter", root.location.protocol + "//" + root.location.hostname + selectList[0].path);
            });

            toolBar.on("click", "." + cls.invertSelectionBtn, function () {
                viewBox.find("." + cls.selectInput).click();
            });

            toolBar.on("click", "." + cls.deselectAllBtn, function () {
                viewBox.find("." + cls.selectInput + ":checked").click();
            });

            toolBar.on("click", "." + cls.selectAllBtn, function () {
                viewBox.find("." + cls.selectInput).not(":checked").click();
            });

            toolBar.on("click", "." + cls.addFileBtn, function () {
                bootbox.prompt({
                    title: text.enterNewFileName,
                    value: "New File.txt",
                    callback: function (name) {
                        if (name) {
                            _.addNewItem(KFM.fileTypes.file, name);
                        }
                    }
                });
            });

            toolBar.on("click", "." + cls.addFolderBtn, function () {
                bootbox.prompt({
                    title: text.enterNewFolderName,
                    value: "New Folder",
                    callback: function (name) {
                        if (name) {
                            _.addNewItem(KFM.fileTypes.folder, name);
                        }
                    }
                });
            });
        }

        // address bar --------------------------------------

        if (addressBar) {
            addressBar.on("click", "." + cls.historyForwardBtn, function () {
                //_.updateViewBox($(this).attr(atr.url), "forward");
                _.updateViewBox(_.location.forth, "forward");
            });

            addressBar.on("click", "." + cls.historyBackBtn, function () {
                //_.updateViewBox($(this).attr(atr.url), "back");
                _.updateViewBox(_.location.back, "back");
            });

            addressBar.on("click", "." + cls.parentDirBtn, function () {
                //_.updateViewBox($(this).attr(atr.url));
                _.updateViewBox(_.location.parent);
            });

            addressBar.on("click", "." + cls.refreshBtn, function () {
                _.refreshViewBox();
            });

            addressBar.on("click", "." + cls.goBtn, function () {
                _.updateViewBox(_.dom.breadcrumb.value);
            });

            addressBar.on("click", "." + cls.homeBtn, function () {
                _.updateViewBox(opts.home);
            });
        }

        // nav pane -----------------------------------------

        if (navPane) {
            navPane.on("click", "." + cls.navPaneNameText, function () {
                _.updateViewBox($(this).attr(atr.url));
            });

            navPane.on("click", "." + cls.navPaneToggle, function () {
                $(this.parentElement.parentElement.nextElementSibling).toggleClass(cls.navPaneClose);
                $(this).toggleClass(cls.navPaneRotate);
            });
        }

        // view box -----------------------------------------

        if (viewBox) {
            viewBox.on("dblclick", ".k-type-" + KFM.fileTypes.folder, function (e) {
                _.updateViewBox(_.kfsiProperty(parseInt(this.getAttribute(atr.fileId)), KFM.kfsi.path));
            });

            //viewBox.on("click", "." + cls.copyURL, function () {
            //    var parent = $(this).parents("." + cls.file),
            //        path = parent.attr(atr.url);
            //    window.prompt(text.copyUrlPrompt, opts.downloadAction + path);
            //});

            viewBox.on("change", "." + cls.selectInput, function (e) {
                var file = this.parentElement,
                    id = parseInt(file.getAttribute(atr.fileId));
                // if it is unchecked
                if (!$(this).prop("checked")) {
                    file.classList.remove(cls.selected);
                    _.kfsiProperty(id, KFM.kfsi.isSelected, false);
                } else {
                    file.classList.add(cls.selected);
                    _.kfsiProperty(id, KFM.kfsi.isSelected, true);
                }
            });
        }

        // status bar ---------------------------------------

        if (statusBar) { }

        // choose bar ---------------------------------------

        if (chooseBar) {
            chooseBar.on("click", "." + cls.chooseBtn, function () {

                var selectList = _.viewBoxContent.filter(function (i) { return i[KFM.kfsi.isSelected]; });

                if (selectList.length != 0) {
                    opts.onchoose.call(_, selectList);
                }
            });

            chooseBar.on("click", "." + cls.cancelBtn, function () {
                //if (_.dom.modal) $(_.dom.modal).modal("hide");
                opts.oncancel.call(_);
            });
        }

        // upload section -----------------------------------

        if (uploadSection) {
            uploadSection.on("click", ".k-upload-cancel", function () {
                dom.uploadSection.style.display = "none";
                _.dropzone.removeAllFiles();
            });

            uploadSection.on("click", ".k-upload-submit", function () {
                //dom.uploadSection.style.display = "none";
                _.dropzone.processQueue();
            });
        }
    };

    KFM.prototype.refreshViewBox = function () {
        var _ = this;

        if (!_.dom.hasOwnProperty("viewBox")) return;

        _.viewBoxContent = [];

        while (_.dom.viewBox.firstChild) _.dom.viewBox.removeChild(_.dom.viewBox.firstChild);

        this.getViewBoxContent();
    };

    KFM.prototype.scafold = function () {

    };

    KFM.prototype.updateAddressBar = function (/*path, direction*/) {
        var _ = this,
            dom = _.dom,
            path = _.location.current;

        if (!dom.addressBar) return;

        //var indexOfTrailingSlash = path.lastIndexOf("/");

        //if (indexOfTrailingSlash == path.length - 1) { path = path.slice(0, indexOfTrailingSlash); }

        var parent = path.slice(0, path.lastIndexOf("/"));

        _.location.parent = (parent == "") ? "/" : parent;

        dom.breadcrumb.value = path;

        //if (direction == "back" && _.location.index > 0) _.location.index--;
        //else if (direction == "forward" && _.location.index < _.history.length - 1) _.location.index++;

        if (path != _.history[_.location.index]) {
            if (_.location.index < _.history.length - 1) while (_.history.length > _.location.index + 1) _.history.pop();
            _.history.push(path);
            _.location.index++;
        }

        if (_.location.parent == path) {
            dom.parentDirBtn.setAttribute("disabled", "disabled");
        } else {
            dom.parentDirBtn.removeAttribute("disabled");
            //dom.parentDirBtn.setAttribute(KFM.attrs.url, parent);
        }

        if (_.location.index == 0) {
            dom.historyBackBtn.setAttribute("disabled", "disabled");
        } else if (_.history[_.location.index - 1]) {
            dom.historyBackBtn.removeAttribute("disabled");
            _.location.back = _.history[_.location.index - 1];
            //dom.historyBackBtn.setAttribute(KFM.attrs.url, _.history[_.location.index - 1]);
        }

        if (_.location.index == _.history.length - 1) {
            dom.historyForwardBtn.setAttribute("disabled", "disabled");
        } else if (_.history[_.location.index + 1]) {
            dom.historyForwardBtn.removeAttribute("disabled");
            _.location.forth = _.history[_.location.index + 1];
            //dom.historyForwardBtn.setAttribute(KFM.attrs.url, _.history[_.location.index + 1]);
        }
    };

    KFM.prototype.updateNavPane = function () {
        var _ = this;
        if (!_.dom.navPane) return;

        while (_.dom.navPane.firstChild) _.dom.navPane.removeChild(_.dom.navPane.firstChild);
        _.getNavPaneContent();
    };

    KFM.prototype.updateViewBox = function (path, direction) {
        var _ = this;

        if (!_.dom.hasOwnProperty("viewBox")) return;

        if (path != "/" && path.lastIndexOf("/") == path.length - 1) path = path.slice(0, path.length - 1);

        _.location.current = path;

        if (direction == "back" && _.location.index > 0) _.location.index--;
        else if (direction == "forward" && _.location.index < _.history.length - 1) _.location.index++;

        _.refreshViewBox();

        //if (_.dom.hasOwnProperty("addressBar")) _.updateAddressBar(path, direction);
    };

    // private functions
    //////////////////////////////////////////////////////////////////////////////

    function ajax(settings) {
        $.ajax(extendObject({
            beforeSend: function () { },
            cache: false,
            complete: function () { },
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: null,
            dataType: "json", // text, html or json
            error: function (jqXHR, textStatus, errorThrown) { console.error(errorThrown); },
            method: "POST", // an alias for 'type'.
            processData: true,
            statusCode: {
                404: function () { alert("page not found"); },
                500: function () { alert("internal server error"); }
            },
            success: function () { },
            //timeout: 10000,
            traditional: false,
            url: KFM.server.exploreAction
        }, settings));
    }

    function createNavPaneItem(directory, rank) {

        /* directory structure:
         * name [string]
         * path [string]
         * list [array of directories] */

        var cls = KFM.classes,
            k_dir = document.createElement("div"),
            k_dir_name_outer = document.createElement("div"),
            k_dir_name_inner = document.createElement("div"),
            k_dir_name_text = document.createElement("div"),
            k_dir_name_icon = document.createElement("span");

        // dorectory object
        k_dir.classList.add(cls.navPaneItem);
        // directory name outer
        k_dir_name_outer.classList.add(cls.navPaneName);
        // directory name inner
        k_dir_name_inner.style.marginLeft = 25 + (10 * rank) + "px";
        // directory name text
        k_dir_name_text.classList.add(cls.navPaneNameText);
        k_dir_name_text.setAttribute(KFM.attrs.url, directory.path);
        k_dir_name_text.innerHTML = directory.name;

        // directory name icon
        k_dir_name_icon.className = cls.navPaneNameIcon + " glyphicon glyphicon-folder-close";
        // appending
        //container.appendChild(k_dir);
        k_dir.appendChild(k_dir_name_outer);
        k_dir_name_outer.appendChild(k_dir_name_inner);
        k_dir_name_inner.appendChild(k_dir_name_text);
        k_dir_name_text.appendChild(k_dir_name_icon);

        if (directory.list != null) {
            var k_toggle = document.createElement("span"),
                k_drawer = document.createElement("div");

            // toggle
            k_toggle.className = cls.navPaneToggle + " glyphicon glyphicon-triangle-right";
            // drawer
            k_drawer.classList.add(cls.navPaneDrawer);
            if (rank == 0) k_toggle.classList.add(cls.navPaneRotate);
            else k_drawer.classList.add(cls.navPaneClose);
            // appending
            k_dir_name_inner.appendChild(k_toggle);
            k_dir.appendChild(k_drawer);

            sort(directory.list, -1, function (a) { return a.name; });

            directory.list.forEach(function (KFSI, Index) {
                //if (KFSI.isDirectory) {
                k_drawer.appendChild(createNavPaneItem(KFSI, rank + 1));
                //}
            });
        }

        return k_dir;
    }

    function createNavPaneItem2(directory, rank) {

        /* directory structure:
         * name [string]
         * path [string]
         * list [array of directories] */

        var li = document.createElement("li"),
            div = document.createElement("div"),
            span = document.createElement("span"),
            label = document.createElement("label");

        li.setAttribute(KFM.attrs.url, directory.path);
        span.className = "glyphicon glyphicon-chevron-right";
        span.style.paddingLeft = 10 * rank + "px";
        label.innerHTML = directory.name;

        div.appendChild(span);
        div.appendChild(label);
        li.appendChild(div);

        if (directory.list.length != 0) {
            li.classList.add("k-has-children");
            var accordion = document.createElement("ul"),
                _class = (rank == 1) ? "k-accordion k-accordion-open" : "k-accordion";
            accordion.className = _class;

            sort(directory.list, -1, function (a) { return a.name; });

            directory.list.forEach(function (KFSI, Index) {
                accordion.appendChild(createNavPaneItem2(KFSI, rank + 1));
            });

            li.appendChild(accordion);
        }

        return li;
    }

    function createViewBoxItem(KFSI, index) {

        /* KFSI structure:
         * name [string]
         * extension [string: "" for folder, ".*" for files]
         * path [string]
         * creation [string: dd/mm/yyyy]
         * imageDimensions [string: "w*h", for images only]
         * length [long integer]
         * isDirectory [boolean]
         * isSelected [boolean: this is added by JS to handel selectList, i.e. is not created in server] 
         * type [string: this is added by JS] */

        var cls = KFM.classes,
            atr = KFM.attrs,
            name = KFSI.name,
            fullName = KFSI.path,
            fileWrapper = document.createElement("li"),
            //file = document.createElement("label"), // ambigiuity between 'change' and 'dblclick' events!!
            file = document.createElement("div"),
            fileImage = document.createElement("div");

        file.classList.add(cls.file);
        file.setAttribute(atr.fileId, index);
        //file.setAttribute(atr.url, fullName);
        //file.setAttribute(atr.creation, KFSI.creation);
        file.parentElement
        fileImage.classList.add(cls.fileIcon);

        if (KFSI.isDirectory) {
            //fileImage.classList.add(KFM.fileTypes.folder);
            file.classList.add("k-type-" + KFM.fileTypes.folder);
            KFSI.type = KFM.fileTypes.folder;
            //file.setAttribute(atr.type, KFM.fileTypes.folder);
        } else {
            //file.setAttribute(atr.type, KFM.fileTypes.file);
            //file.setAttribute(atr.length, KFSI.length);

            if (KFM.fileTypes.hasOwnProperty(KFSI.extension)) {
                var type = KFM.fileTypes[KFSI.extension];
                //fileImage.classList.add(type);
                file.classList.add("k-type-" + type);
                KFSI.type = type;
                if (type == "image") {
                    var imgContainer = document.createElement("div"),
                        img = document.createElement("img"),
                        src = (KFSI.extension == ".ico") ? fullName : KFM.server.thumbnailAction + "?path=~" + fullName;

                    imgContainer.classList.add(cls.imageContainer);
                    img.src = src;
                    img.alt = name;
                    imgContainer.appendChild(img);
                    fileImage.appendChild(imgContainer);
                    file.classList.add(cls.imageFile);
                    //file.setAttribute(atr.dimensions, KFSI.imageDimensions);
                }
            } else {
                //fileImage.classList.add("other");
                file.classList.add("k-type-other");
                KFSI.type = "other";
            }
        }

        //add file name.
        var fileName = document.createElement("div");
        fileName.title = name;
        fileName.classList.add(cls.fileName);
        fileName.innerHTML = name;

        var fileSelect = document.createElement("input");
        fileSelect.setAttribute("type", "checkbox");
        fileSelect.classList.add(cls.selectInput);

        //appends 
        file.appendChild(fileImage);
        file.appendChild(fileName);
        file.appendChild(fileSelect); // fileSelect must be appended last to be in front of all.

        fileWrapper.appendChild(file);

        return fileWrapper;
        //return file;
    }

    function extendObject(source, object) {
        if (typeof object == "object") {
            for (var p in object) {
                if (!object.hasOwnProperty(p)) continue;
                source[p] = object[p];
            }
        }
        return source;
    }

    function isEmpty(object) {
        for (var p in object) {
            if (object.hasOwnProperty(p)) {
                return false;
            } else {
                continue;
            }
        }
        return true;
    }

    function prepareUpload(path) {
        var form = $(this.body).find("." + KFM.classes.upload).find("form");
        form.unbind();
        //form.uploadMyFilePlz({ rootPath: "~" + path });
    }

    function sort(array, order, callbck) {
        array.sort(function (a, b) {
            var an = callbck(a),
                bn = callbck(b);

            if (typeof an == "boolean") {
                if (an && !bn) return -order;
                if (!an && bn) return order;
                return 0;
            } else if (typeof an == "number" || typeof an == "string") {
                if (an > bn) return -order;
                if (an < bn) return order;
                return 0;
            }
        });
    }

    /* this is a 'JavaScript Closure',
     * i.e. a function having access to the parent scope,
     * even after the parent function has closed. */
    /* the variable 'uid' is assigned the return value of a 'self-invoking' function.
     * the 'self-invoking' function only runs once. it sets the id to 0, and returns a function expression.
     * this way 'uid' becomes a function. The wonderful part is that it can access the 'id' in the parent scope. */

    var uid = (function () {
        var id = 0;
        return function () {
            return "kfm" + id++;
        };
    })();

    // static properties
    //////////////////////////////////////////////////////////////////////////////

    KFM.defaults = {
        auto: true,
        // initialize
        closeAfterSelect: true,
        downloadAction: "/Home/download?url=~",
        home: "/", // to add: check if this location exists.
        id: "kaveh",
        lang: "fa",
        locationHistoryLength: 20,
        multiChoose: true,
        scaffoldBody: false,
        selectFile: false,
        modalId: "",
        // events
        onchoose: function (list) { alert(list.length + " folder(s)"); /*this.close();*/ },
        oncancel: function () { },
        onstart: function () { },
        // roles
        selsectRole: {
            include: [],
            exclude: []
        },
        showRole: {
            include: [],
            exclude: []
        },
        uploadRole: {
            include: [],
            exclude: []
        },
    };

    KFM.server = {
        controller: "/Kaveh/FileManager",
        addNewItemAction: "/Kaveh/FileManager/AddNewItem",
        downloadAction: "/Kaveh/FileManager/Download",
        getNavPaneContentAction: "/Kaveh/FileManager/GetNavPaneContent",
        getViewBoxContentAction: "/Kaveh/FileManager/GetViewBoxContent",
        removeAction: "/Kaveh/FileManager/Remove",
        renameAction: "/Kaveh/FileManager/Rename",
        thumbnailAction: "/Kaveh/FileManager/Thumbnail",
        uploadAction: "/Kaveh/FileManager/Upload",
    };

    KFM.kfsi = {
        name: "name",
        extension: "extension",
        path: "path",
        creation: "creation",
        imageDimensions: "imageDimensions",
        length: "length",
        isDirectory: "isDirectory",
        // added by JS, not server
        isSelected: "isSelected",
        type: "type"
    };

    KFM.classes = {
        pluginClass: "kaveh",

        // toolbar
        //----------------------------------------------
        toolBar: "k-tool-bar",
        addFileBtn: "k-add-file",
        addFolderBtn: "k-add-folder",
        removeBtn: "k-remove",
        renameBtn: "k-rename",
        copyBtn: "k-copy",
        cutBtn: "k-cut",
        sortByNameBtn: "k-sort-name",
        sortByTypeBtn: "k-sort-type",
        sortBySizeBtn: "k-sort-size",
        filterByNameBtn: "k-filter-name",
        filterByTypeBtn: "k-filter-type",
        filterBySizeBtn: "k-filter-size",
        uploadBtn: "k-upload",
        linkBtn: "k-link",
        downloadBtn: "k-download",
        downloadLinkBtn: "k-download-link",
        viewLargeTileBtn: "k-view-tile-large",
        viewSmallTileBtn: "k-view-tile-small",
        viewDetailsBtn: "k-view-details",
        selectAllBtn: "k-select-all",
        deselectAllBtn: "k-deselect-all",
        invertSelectionBtn: "k-invert-selection",

        // addressbar
        //----------------------------------------------
        addressBar: "k-address-bar",
        searchBtn: "k-search",
        searchInput: "k-search-input",
        refreshBtn: "k-refresh",
        goBtn: "k-go",
        breadcrumb: "k-breadcrumb",
        homeBtn: "k-home-directory",
        parentDirBtn: "k-parent-directory",
        historyBackBtn: "k-history-back",
        historyForwardBtn: "k-history-forward",

        // file and folders
        //----------------------------------------------
        content: "k-content-wrapper",

        // box
        viewBox: "k-viewbox",
        boxMessage: "k-viewbox-msg",
        file: "k-file",
        imageFile: "k-img-file",
        imageContainer: "k-img-ctr",
        fileName: "k-file-name",
        fileIcon: "k-file-icon",
        //optionList: "k-optn-list",
        //optionItem: "k-optn-item",
        //copyURL: "k-copy-url",
        //remove: "k-remove",
        //rename: "k-rename",
        //download: "k-download",

        selectInput: "k-file-select",
        selected: "k-selected",

        // pane
        navPane: "k-pane",
        navPaneItem: "k-pane-item",
        navPaneName: "k-pane-name",
        navPaneNameText: "k-pane-text",
        navPaneNameIcon: "k-pane-icon",
        navPaneToggle: "k-pane-toggle",
        navPaneRotate: "k-pane-rotate",
        navPaneDrawer: "k-pane-drawer",
        navPaneClose: "k-pane-close",

        // status bar
        //----------------------------------------------
        statusBar: "k-status-bar",
        status: "k-status",

        // choose bar
        //----------------------------------------------
        chooseBar: "k-choose-bar",
        chooseBtn: "k-choose",
        cancelBtn: "k-cancel",

        // choose bar
        //----------------------------------------------
        uploadSection: "k-upload-section",
        uploadForm: "k-upload-form"
    };

    KFM.attrs = {
        creation: "data-creation",
        dimensions: "data-dim",
        fileId: "data-k-id",
        lastAccess: "data-access",
        lastWrite: "data-write",
        length: "data-len",
        title: "title",
        type: "data-k-type",
        url: "data-url"
    };

    KFM.fileTypes = {
        file: "file",
        folder: "folder",
        ".apk": "program",
        ".aspx": "aspx",
        ".avi": "movie",
        ".config": "config",
        ".cs": "csharp",
        ".cshtml": "cshtml",
        ".css": "stylesheet",
        ".dll": "dll",
        ".exe": "program",
        ".fon": "font",
        ".gif": "image",
        ".htm": "html",
        ".html": "html",
        ".ico": "image",
        ".ini": "config",
        ".jpg": "image",
        ".jpeg": "image",
        ".js": "javascript",
        ".json": "json",
        ".mp3": "sound",
        ".mp4": "movie",
        ".mkv": "movie",
        ".mov": "movie",
        ".otf": "font",
        ".pdf": "pdf",
        ".png": "image",
        ".srt": "text",
        ".ttc": "font",
        ".ttf": "font",
        ".txt": "text",
        ".wma": "sound",
        ".xml": "text"
    };

    KFM.instances = {}; // this is a 'static' property to store all instances.

    KFM.dictionary = {
        fa: {
            close: "بستن این پنجره",
            copyUrlLabel: "کپی آدرس",
            copyUrlPrompt: "این متن را کپی کنید",
            directoryNotExists: "این فولدر وجود ندارد",
            downloadLabel: "دانلود",
            emptyFolderInfo: "این پوشه خالیست",
            enterDownloadFileName: "فایل زیپ با چه نامی دریافت شود؟",
            enterNewFileName: "نام و پسوند فایل جدید را وارد کنید. مثلا new.txt",
            enterNewFolderName: "نام فولدر جدید را وارد کنید.",
            file: "فایل",
            fileRemoveConfirm: "این فایل حذف شود؟",
            folder: "فولدر",
            folderRemoveConfirm: "با حذف این پوشه، همه‌ی محتویات آن نیز حذف می‌شوند\nآیا مطمئن هستید؟",
            internalError: "خطای سرور",
            newName: "نام جدید",
            nothingSelected: "چیزی انتخاب نشده!",
            refresh: "بارگذاری دوباره",
            removeConfirm: "تایید حذف:",
            removeFailedError: "انجام نشد!",
            removeLabel: "حذف",
            renameConfirm: "تایید تغییر نام:",
            renameFailedError: "انجام نشد!",
            renameLabel: "تغییر نام",
            upload: "آپلود"
        }
    };

    // attaching the object to to root.
    // whitout this, the 'KFM' object is not accessible out of this scope.
    root.KavehFileManager = KFM;

    KFM.noConflict = function () {
        root.KavehFileManager = previous;
        return KFM;
    };

}).call(this, jQuery, bootbox, Dropzone);
// })().call(this); // this line will throw a 'RefrenceError'.

/* we are in 'strict mode', so the 'this' keyword is 'undefined' within above scope.
 * to solve THIS problem, i call 'this'.
 * now the 'this' keyword inside above scope is 'window' instead of 'undefined'. */