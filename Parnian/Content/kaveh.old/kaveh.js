var kaveh = (function ($) {
    $.fn.kaveh = function (userSettings) {

        "use strict"; //enabling Strict Mode.

        var defaults = {
            lang: "fa",
            //
            fireButtonSelector: "",
            fileNameInputSelector: "#imageName",
            rootUrl: "",
            //
            canSelectImage: true,
            showSelectedImage: true,
            showImageSelector: "#image",
            imageDimensionsInputSelector: "",
            //
            canSelectFile: true,
            //
            closeAfterSelect: true,
            //
            downloadAction: "/Home/download?url=~"
        },

        config = {
            controller: "/Kaveh",
            actions: {
                explore: "/Explore",
                remove: "/Remove",
                rename: "/Rename",
                download: "/Download",
                upload: "/Upload",
                thumbnail: "/Thumbnail"
            },
            //classes used to scafold the explorer
            selectors: {
                kaveh: "kaveh",
                //toolbar
                toolbar: "k-toolbar",
                barBtn: "k-bar-btn",
                close: "k-bar-btn-close",
                closeIcon: "k-icon-switch",
                refresh: "k-bar-btn-refresh",
                refreshIcon: "k-icon-loop2",
                //content
                content: "k-content",
                //ribbon
                ribbon: "k-ribbon",
                //address bar
                addressBar: "k-address-bar",
                historyToggle: "history",
                back: "k-back",
                backIcon: "k-icon-circle-left",
                forth: "k-forth",
                forthIcon: "k-icon-circle-right",
                //explorer
                explorer: "k-explorer",
                //pane
                navPane: "k-pane",
                directory: "k-directory",
                //box
                box: "k-box",
                boxMessage: "k-box-msg",
                file: "k-file",
                imageFile: "k-img-file",
                imageContainer: "k-img-ctr",
                fileName: "k-file-name",
                fileIcon: "k-file-icon",
                //
                optionList: "k-optn-list",
                optionItem: "k-optn-item",
                //
                copyURL: "k-copy-url",
                remove: "k-remove",
                rename: "k-rename",
                download: "k-download",
                //footer
                footer: "k-footer",
                upload: "k-upload",
                uploadLimit: "k-upload-limit",
                message: "k-message",
                fileInput: "k-file-input",
                progressLabel: "k-progress"
            },
            //
            attributes: {
                type: "data-type",
                url: "data-url",
                title: "title",
                length: "data-len",
                creation: "data-creation",
                lastAccess: "data-access",
                lastWrite: "data-write",
                dimensions: "data-dim"
            },
            //
            types: {
                file: "file",
                folder: "folder",
                //image
                ".jpg": "image",
                ".jpeg": "image",
                ".png": "image",
                ".gif": "image",
                ".ico": "image",
                ".svg": "image",
                //sound
                ".mp3": "sound",
                ".wma": "sound",
                //movie
                ".mp4": "movie",
                ".avi": "movie",
                ".mkv": "movie",
                ".mov": "movie",
                ".m4v": "movie",
                ".ogv": "movie",
                ".webm": "movie",
                //text
                ".txt": "text",
                ".srt": "text",
                ".html": "text",
                ".htm": "text",
                ".xml": "text",
                //javascript
                ".js": "javascript",
                ".json": "javascript",
                //font
                ".ttf": "font",
                ".fon": "font",
                ".otf": "font",
                ".ttc": "font",
                ".eot": "font",
                ".woff": "font",
                ".woff2": "font",
                //dll
                ".dll": "dll",
                //program
                ".exe": "program",
                ".apk": "program",
                //cshtml
                ".cshtml": "cshtml",
                //stylesheet
                ".css": "stylesheet",
                ".less": "stylesheet",
                ".sass": "stylesheet",
                //csharp
                ".cs": "csharp",
                //config
                ".config": "config",
                ".ini": "config",
                //pdf
                ".pdf": "pdf"
            }
        },

            opts = $.extend({}, defaults, userSettings),
            text = $.fn.kaveh.languages[opts.lang],
            //
            slr = config.selectors,
            atr = config.attributes;

        return this.each(function () {
            var $this = $(scaffold()),
                rootUrl = opts.rootUrl;

            //gets a list of directories in "Server.MapPath('~/')" and lists them in "navPane".
            //---------------------------------------------------------------------------------------------
            bekav({ path: "", getFiles: false }, true);

            //gets all files and folders in desired directory, i.e rootUrl and shows them in "box".
            //---------------------------------------------------------------------------------------------
            bekav({ path: rootUrl }, false);

            //set the position.
            //---------------------------------------------------------------------------------------------
            positioning();

            window.onresize = function () { positioning(); };

            function positioning() {
                var h = $(window).innerHeight(),
                    top = (h - $this.innerHeight()) / 2,
                    w = $(window).innerWidth(),
                    right = (w - $this.innerWidth()) / 2;

                top = (top < 0) ? 0 : top;
                right = (right < 0) ? 0 : right;

                $this.css({ top: top.toFixed(2) + "px", right: right.toFixed(2) + "px" });
            }

            //by clicking on desired directory name in "navPane",
            //gets all files and folders in that directory and shows them in "box".
            //---------------------------------------------------------------------------------------------
            $this.on("click", dot(slr.directory), function () {
                rootUrl = $(this).attr(atr.url);
                bekav({ path: rootUrl }, false);
            });

            //by dbouble-clicking on desired folder in "box",
            //gets all files and folders in that folder and shows them in "box".
            //---------------------------------------------------------------------------------------------
            $this.on("dblclick", '[' + atr.type + '=' + config.types.folder + ']', function () {
                rootUrl = $(this).attr(atr.url);
                bekav({ path: rootUrl }, false);
            });

            //by double-clicking on desired file in "box", sets the "value" of "fileNameInputSelector"
            //to that file's url, then closes the explorer window.
            //if file was a image, sets the "src" of "showImageSelector"
            //to that file's url.
            //---------------------------------------------------------------------------------------------
            $this.on("dblclick", '[' + atr.type + '=' + config.types.file + ']', function () {
                var $file = $(this);
                if ($file.is(dot(slr.imageFile))) {
                    if (opts.canSelectImage) {
                        var name = $file.find(dot(slr.fileName)).attr(atr.title),
                            dim = $file.attr(atr.dimensions);
                        $(opts.fileNameInputSelector).val(name);
                        $(opts.imageDimensionsInputSelector).val(dim);
                        if (opts.showSelectedImage) {
                            $(opts.showImageSelector).css("display", "block").attr("src", $file.attr(atr.url));
                        }
                        if (opts.closeAfterSelect) close();
                    }
                }
                else if (opts.canSelectFile) {
                    var name = $file.find(dot(slr.fileName)).attr(atr.title);
                    $(opts.fileNameInputSelector).val(name);
                    if (opts.closeAfterSelect) close();
                }
            });

            //shows a dialogue to copy the file's URL.
            //---------------------------------------------------------------------------------------------
            $this.on("click", dot(slr.copyURL), function () {
                var parent = $(this).parents(dot(slr.file)),
                    path = parent.attr(atr.url);
                window.prompt(text.copyUrlPrompt, opts.downloadAction + path);
            });

            //shows a dialogue to remove file or folder. if user confirms, removes it.
            //---------------------------------------------------------------------------------------------
            $this.on("click", dot(slr.remove), function () {
                var parent = $(this).parents(dot(slr.file)),
                    path = parent.attr(atr.url),
                    type = parent.attr(atr.type),
                    url = config.controller + config.actions.remove,
                    userConfirm = false;

                if (type == "file")
                    userConfirm = confirm(text.fileRemoveConfirm);
                else if (type == "folder")
                    userConfirm = confirm(text.folderRemoveConfirm);

                if (userConfirm) {
                    var dataString = new FormData();
                    dataString.append("path", path);
                    dataString.append("type", type);
                    $.ajax({
                        type: "Post",
                        url: url,
                        data: dataString,
                        cache: false,
                        contentType: false,
                        processData: false,

                        success: function (data) {
                            alert(data);
                            bekav({ path: rootUrl }, false);
                            return false;
                        },

                        error: function (jqXHR, textStatus, errorThrown) {
                            alert(text.removeFailedError);
                            return false;
                        },
                    });
                }

                return false;
            });

            //shows a dialogue to rename file or folder.
            //---------------------------------------------------------------------------------------------
            $this.on("click", dot(slr.rename), function () {
                var parent = $(this).parents(dot(slr.file)),
                    path = parent.attr(atr.url),
                    type = parent.attr(atr.type),
                    name = parent.find(dot(slr.fileName)).html(),
                    newName = prompt(text.newName, name);

                if (newName) {
                    var dataString = new FormData();
                    dataString.append("path", path);
                    dataString.append("type", type);
                    dataString.append("newName", newName);

                    $.ajax({
                        type: "Post",
                        url: config.controller + config.actions.rename,
                        data: dataString,
                        cache: false,
                        contentType: false,
                        processData: false,

                        success: function (data) {
                            alert(data);
                            bekav({ path: rootUrl }, false);
                            return false;
                        },

                        error: function (jqXHR, textStatus, errorThrown) {
                            alert(text.renameFailedError);
                            return false;
                        }
                    });
                }
                return false;
            });

            //closes the explorer window.
            //---------------------------------------------------------------------------------------------
            $this.on("click", dot(slr.close), function () {
                close();
                return false;
            });

            //refreshes the content of "box".
            //---------------------------------------------------------------------------------------------
            $this.on("click", dot(slr.refresh), function () {
                bekav({ path: rootUrl }, false);
                return false;
            });

            //when fired, closes the explorer window.
            //---------------------------------------------------------------------------------------------
            function close() {
                //unbind all "click", "dblclick" and "submit"(for upload form) event listeners within
                //"$this" element. if you remove these lines, all event listeners will be fired multiple
                //times when "kaveh" metheod is triggered more than once in a page.
                $this.off("click");
                $this.off("dblclick");
                $this.find(dot(slr.upload)).find("form").unbind();

                $this.remove();
                return false;
            }

            //Ajax call of "Explore" action in "Kaveh" controller.
            //---------------------------------------------------------------------------------------------
            function bekav(bekavArg, fillNavPane) {
                //bekavArg = {
                //    string path, 
                //    bool isVirtual = true, 
                //    bool goThroughSubDirectories = false, 
                //    bool getFiles = true, 
                //    bool getDirectories = true
                //}

                var dataString = new FormData();
                Object.keys(bekavArg).forEach(function (key, index) {
                    dataString.append(key, bekavArg[key]);
                    return false;
                });

                $.ajax({
                    type: "POST",
                    url: config.controller + config.actions.explore,
                    data: dataString,
                    cache: false,
                    contentType: false,
                    processData: false,

                    //if everything goes well, "respond" would be a list of files and folders.
                    success: function (respond) {
                        if (fillNavPane) FillNavPane(respond, bekavArg.path);
                        else {
                            fillBox(respond, bekavArg.path);
                            prepareUpload(bekavArg.path);
                        }
                        return false;
                    },

                    error: function (jqXHR, textStatus, errorThrown) {
                        alert(text.internalError);
                        return false;
                    },
                });
                $this.find(dot(slr.ribbon)).html(rootUrl);
                return false;
            }

            //prepares Upload.
            //---------------------------------------------------------------------------------------------
            function prepareUpload(path) {
                var form = $this.find(dot(slr.upload)).find("form");
                form.unbind();
                form.uploadMyFilePlz({ rootPath: "~" + path });
            }

            //when fired, gets a list of directories in "Server.MapPath('~/')" and shows them in "navPane".
            //---------------------------------------------------------------------------------------------
            function FillNavPane(Directories) {
                var k_nav = $this.find(dot(slr.navPane)).find("div").first();
                k_nav.html(""); //don't delete this line or else there will be duplicates after refresh.

                //adding the "Root" directory to "navPane".
                var k_directory = document.createElement("div"),
                    k_directoryDiv = document.createElement("div");

                k_directory.classList.add(slr.directory);
                k_directory.setAttribute(atr.url, "");
                k_directory.appendChild(k_directoryDiv);
                k_directoryDiv.innerHTML = "Root";
                k_directory.style.color = "red";

                k_nav.append(k_directory);

                //loops over "Directories" and adds them to "navPane".
                Directories.forEach(function (directory, index) {
                    if (directory.isDirectory) {
                        var k_directory = document.createElement("div"),
                            k_directoryDiv = document.createElement("div");

                        k_directory.classList.add(slr.directory);
                        k_directory.setAttribute(atr.url, directory.path + "/");
                        k_directory.appendChild(k_directoryDiv);
                        k_directoryDiv.innerHTML = directory.name;

                        k_nav.append(k_directory);
                    }
                    return false;
                });

                return false;
            }

            //when fired, gets all files and folders in desired directory and shows them in "box".
            //---------------------------------------------------------------------------------------------
            function fillBox(iFileSystemList, path) {
                var k_box = $this.find(dot(slr.box)).find("div").first();

                k_box.html(""); //don't delete this line or else there will be duplicates after refresh.

                //shows a message to indicate that list is empty.
                if (iFileSystemList.length == 0) {
                    var message = document.createElement("div");
                    message.classList.add(slr.boxMessage);
                    message.innerHTML = text.emptyFolderInfo;
                    k_box.append(message);
                }
                    //loops over objects in list and fills the box with files and folders.
                else {
                    sort(iFileSystemList, "isDirectory", false);

                    iFileSystemList.forEach(function (iFS, index) {
                        var name = iFS.name,
                            fullName = iFS.path;

                        var k_file = document.createElement("div");
                        k_file.classList.add(slr.file);

                        k_file.setAttribute(atr.url, fullName);
                        k_file.setAttribute(atr.creation, iFS.creation);
                        k_file.setAttribute(atr.lastAccess, iFS.lastAccess);
                        k_file.setAttribute(atr.lastWrite, iFS.lastWrite);

                        var k_fileImage = document.createElement("div");
                        k_fileImage.classList.add(slr.fileIcon);

                        if (iFS.isDirectory) {
                            k_fileImage.classList.add(config.types.folder);
                            k_file.setAttribute(atr.type, config.types.folder);
                            k_file.setAttribute(atr.url, fullName + "/");
                        }

                        else {
                            k_file.setAttribute(atr.type, config.types.file);
                            k_file.setAttribute(atr.length, iFS.length);
                            //k_file.setAttribute("data-props", iFS.properties);

                            if (config.types.hasOwnProperty(iFS.extension)) {
                                var type = config.types[iFS.extension];
                                k_fileImage.classList.add(type);

                                if (type == "image") {
                                    var k_imgContainer = document.createElement("div"),
                                        k_img = document.createElement("img");
                                    k_imgContainer.classList.add(slr.imageContainer);
                                    var src = (iFS.extension == ".ico") ? fullName : config.controller + config.actions.thumbnail + "?path=~" + fullName;
                                    k_img.src = src;
                                    k_img.alt = name;
                                    k_imgContainer.appendChild(k_img);
                                    k_fileImage.appendChild(k_imgContainer);
                                    k_file.classList.add(slr.imageFile);
                                    k_file.setAttribute(atr.dimensions, iFS.imageDimensions);
                                }
                            }

                            else k_fileImage.classList.add("other");
                        }

                        //add file name.
                        var k_fileName = document.createElement("div");
                        k_fileName.title = name;
                        k_fileName.classList.add(slr.fileName);
                        k_fileName.innerHTML = name;

                        //create option-list.
                        var k_optionList = document.createElement("div");
                        k_optionList.classList.add(slr.optionList);

                        //creates copyURL option-item and appends it to option-list.
                        var k_copyURL = document.createElement("div");
                        k_copyURL.innerHTML = text.copyUrlLabel;
                        k_copyURL.classList.add(slr.optionItem);
                        k_copyURL.classList.add(slr.copyURL);
                        k_optionList.appendChild(k_copyURL);

                        //creates remove option-item and appends it to option-list.
                        var k_remove = document.createElement("div");
                        k_remove.innerHTML = text.removeLabel;
                        k_remove.classList.add(slr.optionItem);
                        k_remove.classList.add(slr.remove);
                        k_optionList.appendChild(k_remove);

                        //creates rename option-item and appends it to option-list.
                        var k_rename = document.createElement("div");
                        k_rename.innerHTML = text.renameLabel;
                        k_rename.classList.add(slr.optionItem);
                        k_rename.classList.add(slr.rename);
                        k_optionList.appendChild(k_rename);

                        //creates download option-item for files (not folders) and appends it to option-list.
                        if (!iFS.isDirectory) {
                            var k_download = document.createElement("div"),
                            k_download_anchor = document.createElement("a");
                            k_download_anchor.innerHTML = text.downloadLabel;
                            k_download_anchor.href = config.controller + config.actions.download + "?url=~" + fullName;
                            k_download.classList.add(slr.optionItem);
                            k_download.classList.add(slr.download);

                            k_download.appendChild(k_download_anchor);

                            k_optionList.appendChild(k_download);
                        }

                        //appends 
                        k_file.appendChild(k_fileImage);
                        k_file.appendChild(k_fileName);
                        k_file.appendChild(k_optionList);

                        //appends file to "box".
                        k_box.append(k_file);

                        return false;
                    });
                }

                return false;
            }

            //adds a dot(.) at the beginning of the passed string.
            //---------------------------------------------------------------------------------------------
            function dot(string) {
                if (typeof string == "string") {
                    return "." + string;
                }
                return false;
            }

            //usage: array.sort(compare(...));
            //---------------------------------------------------------------------------------------------
            function sort(array, sortOption, isStrNum) {
                array.sort(function (a, b) {
                    var an = a[sortOption], bn = b[sortOption], type = typeof an;
                    var boolean = true, string = "string", number = 1;
                    if (!isStrNum) {
                        if (type === typeof boolean) {
                            if (an && !bn) return -1;
                            if (!an && bn) return 1;
                            return 0;
                        }
                        else if (type === typeof number || type === typeof string) {
                            if (an > bn) return -1;
                            if (an < bn) return 1;
                            return 0;
                        }
                    }
                    else {
                        an = eval(an), bn = eval(bn);
                        if (an > bn) return -1;
                        if (an < bn) return 1;
                        return 0;
                    }
                });
            }

            //Scaffolds the explorer
            //---------------------------------------------------------------------------------------------
            function scaffold() {
                var container = createElement({ classlist: [slr.kaveh], attributes: { dir: "ltr" } });
                //
                var toolbar = createElement({ classlist: [slr.toolbar] });
                var btn_close = createElement({ classlist: [slr.barBtn, slr.close], attributes: { title: text.close } });
                btn_close.appendChild(createElement({ tag: "span", classlist: [slr.closeIcon] }));
                var btn_refresh = createElement({ classlist: [slr.barBtn, slr.refresh], attributes: { title: text.refresh } });
                btn_refresh.appendChild(createElement({ tag: "span", classlist: [slr.refreshIcon] }));
                toolbar.appendChild(btn_close);
                toolbar.appendChild(btn_refresh);
                //
                var content = createElement({ classlist: [slr.content] });
                //
                var ribbon = createElement({ classlist: [slr.ribbon] });
                //
                var address_bar = createElement({ classlist: [slr.addressBar] });
                //var go_back = createElement({ classlist: [slr.historyToggle, slr.back] });
                //go_back.appendChild(createElement({ tag: "span", classlist: [slr.backIcon] }));
                //var go_forth = createElement({ classlist: [slr.historyToggle, slr.forth] });
                //go_forth.appendChild(createElement({ tag: "span", classlist: [slr.forthIcon] }));
                //address_bar.appendChild(go_back);
                //address_bar.appendChild(go_forth);
                //
                var explorer = createElement({ classlist: [slr.explorer] });
                var pane = createElement({ classlist: [slr.navPane] });
                pane.appendChild(createElement());
                var box = createElement({ classlist: [slr.box] });
                box.appendChild(createElement());
                explorer.appendChild(pane);
                explorer.appendChild(box);
                //
                var footer = createElement({ classlist: [slr.footer] });
                var upload = createElement({ classlist: [slr.upload] });
                var upload_form = createElement({
                    tag: "form", attributes: {
                        action: config.controller + config.actions.upload,
                        enctype: "multipart/form-data",
                        method: "POST",
                        "data-ajax": "true",
                        "data-ajax-method": "POST"
                    }
                });
                //
                var upload_limit = createElement({ classlist: [slr.uploadLimit] });
                upload_limit.innerHTML = "حجم فایل باید کمتر از دوازده مگابایت باشد";
                var message = createElement({ classlist: [slr.message] });
                var btn_group = createElement({classlist: ["k-btn-group"]});
                var file_input = createElement({ tag: "input", classlist: [slr.fileInput, "btn"], attributes: { type: "file", name: "file", multiple: "multiple" } });
                var upload_bottun = createElement({ tag: "input", classlist: ["btn"], attributes: { type: "submit", value: text.upload, role: "button" } });

                btn_group.appendChild(file_input);
                btn_group.appendChild(upload_bottun);

                var upload_progress_bar = createElement({ tag: "progress", attributes: { value: 0, max: 100 } });
                var upload_progress_lable = createElement({ tag: "span", classlist: [slr.progressLabel] });
                //
                upload_form.appendChild(upload_limit);
                upload_form.appendChild(message);
                //upload_form.appendChild(file_input);
                //upload_form.appendChild(upload_bottun);
                upload_form.appendChild(btn_group);
                upload_form.appendChild(upload_progress_bar);
                upload_form.appendChild(upload_progress_lable);
                //
                upload.appendChild(upload_form);
                footer.appendChild(upload);
                //
                content.appendChild(ribbon);
                content.appendChild(address_bar);
                content.appendChild(explorer);
                content.appendChild(footer);
                //
                container.appendChild(toolbar);
                container.appendChild(content);
                //
                document.body.appendChild(container);
                return container;
            }

            //Create Element
            //---------------------------------------------------------------------------------------------
            function createElement(arg) {
                var def = {
                    tag: "div",
                    classlist: [],
                    attributes: {},
                };
                var obj = $.extend({}, def, arg);
                var e = document.createElement(obj.tag);
                obj.classlist.forEach(function (cls, index) {
                    e.classList.add(cls);
                });

                Object.keys(obj.attributes).forEach(function (key, index) {
                    e.setAttribute(key.toString(), obj.attributes[key].toString());
                });
                return e;
            }
        });
    };

    $.fn.kaveh.languages = {
        fa: {
            copyUrlPrompt: "این متن را کپی کنید",
            newName: "نام جدید",
            //
            fileRemoveConfirm: "این فایل حذف شود؟",
            folderRemoveConfirm: "با حذف این پوشه، همه‌ی محتویات آن نیز حذف می‌شوند\nآیا مطمئن هستید؟",
            removeFailedError: "انجام نشد!",
            //
            renameFailedError: "انجام نشد!",
            //
            emptyFolderInfo: "این پوشه خالیست",
            //
            copyUrlLabel: "کپی آدرس",
            removeLabel: "حذف",
            renameLabel: "تغییر نام",
            downloadLabel: "دانلود",
            //
            close: "بستن این پنجره",
            refresh: "بارگذاری دوباره",
            upload: "آپلود",
            //
            internalError: "خطای سرور"
        }
    };

})(jQuery);

// Upload
//========================================================================================
var myUploader = (function ($) {
    $.fn.uploadMyFilePlz = function (options) {
        var opts = $.extend({}, $.fn.uploadMyFilePlz.defaults, options);

        return this.each(function () {
            var $this = $(this),
                $file = $this.find(".k-file-input");

            fileVal();

            $file.change(function () {
                fileVal();
            });

            $this.submit(function (event) {
                event.preventDefault();

                if ($this.attr("enctype") == "multipart/form-data") {
                    //var file = $file.prop("files")[0];
                    var files = $file.prop("files");
                    if (files.length != 0) {
                        Array.prototype.forEach.call(files, function (file, index) {
                            var fileSize = file.size;
                            if (fileSize == 0) {
                                showMessage("فایل انتخاب شده خالی است");
                            }
                            else if (0 < fileSize && fileSize <= 12582912) {
                                var action = $this.attr("action"),
                                    dataString = new FormData();
                                dataString.append("rootPath", opts.rootPath);
                                dataString.append("file", file);

                                $.ajax({
                                    type: opts.type,
                                    url: action,
                                    xhr: function () {
                                        var myXhr = $.ajaxSettings.xhr();
                                        if (myXhr.upload) {
                                            myXhr.upload.addEventListener("progress", function (e) {
                                                if (e.lengthComputable) {
                                                    var total = e.total,
                                                        loaded = e.loaded,
                                                        percent = (loaded / total) * 100;
                                                    $this.find("progress").attr({ value: percent.toFixed(2) });
                                                    $this.find(".progress").html(percent.toFixed(2) + "%");
                                                }
                                            }, false);
                                        }
                                        return myXhr;
                                    },
                                    data: dataString,
                                    cache: false,
                                    contentType: false,
                                    processData: false,

                                    success: function (data) {
                                        showMessage(data);
                                    },

                                    error: function (jqXHR, textStatus, errorThrown) {
                                        alert(opts.errorMessage);
                                        showMessage(opts.errorMessage);
                                    }
                                });
                            }
                            else {
                                showMessage("آپلود نشد چون حجم فایل انتخابی بزرگتر از ۱۲ مگابایت است");
                            }
                        });
                    }
                    else {
                        showMessage("فایلی انتخاب نشده است");
                    }
                }

                function showMessage(message) {
                    var $uploadMess = $this.find(".k-message");
                    $uploadMess.text(message).addClass("k-message-show");

                    var t = setTimeout(function () {
                        $uploadMess.removeClass("k-message-show").html("");
                    }, opts.uploadMessageTimeout);
                }
            });

            function fileVal() {
                $this.find(".browseAddress").val($file.val());
            }
        });
    };

    $.fn.uploadMyFilePlz.defaults = {
        type: "POST",
        rootPath: "~/",
        uploadMessageTimeout: 7000,
        errorMessage: "اشکال در برقراری ارنباط با سرور، یا اشکال در خود سرور",
    };
})(jQuery);