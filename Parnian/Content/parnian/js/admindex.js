function GroupAction(options) {
    "use strict";
    var defaults = {
        idType: "int",
        controller: "",
        entityName: "آیتم",
        sorting: true,
        deleting: true,
        hiding: true,
        sortAction: "SetPriority",
        deleteAction: "GroupDelete",
        hideAction: "GroupdoNotShow"
    },
    opts = $.extend({}, defaults, options);

    // initializing
    //----------------------------------------------------------------------
    $(".group-action").prop("checked", false);
    $("input#select-all").prop("checked", false);
    $("#select-all").click(function () {
        if ($(this).is(":checked"))
            $(".group-action").prop("checked", true);
        else $(".group-action").prop("checked", false);
    });

    // actions
    //----------------------------------------------------------------------
    if (opts.sorting) {
        $("tbody").sortable({
            placeholder: "ui-state-highlight",
            handle: $(".handle"),
            update: function () {
                handleIndex(".handle");
                $("#set-priority").addClass("flasher");
            },
        });

        $("#set-priority").click(function () {
            setPriority();
        });
    }

    if (opts.deleting) {
        $("#group-delete").click(function () {
            groupAction(".group-action", "حذف", "/" + opts.controller + "/" + opts.deleteAction);
        });
    }

    if (opts.hiding) {
        $("#group-do-not-show").click(function () {
            groupAction(".group-action", "پنهان کردن", "/" + opts.controller + "/" + opts.hideAction);
        });
    }
    //handleIndex
    //----------------------------------------------------------------------
    function handleIndex(handle) {
        $(handle).each(function (index, element) {
            $(element).html(index + 1);
        });
    }
    //setPriority
    //----------------------------------------------------------------------
    function setPriority() {

        function model(id, priority) {
            this.i = id;
            this.p = priority;
        }

        var modelArray = new Array();

        $(".handle").each(function (index, element) {
            var id = eval($(element).attr("data-id")),
                priority = index + 1,
                m = new model(id, priority);
            modelArray.push(m);
        });

        var dataString = new FormData(),
            json = JSON.stringify(modelArray);
        dataString.append("jsonArray", json);
        Ajax("post", "/" + opts.controller + "/" + opts.sortAction, dataString);
    }

    //groupAction
    //----------------------------------------------------------------------
    function groupAction(selector, action, url) {
        var idArray = createAndFillArray(selector);
        if (idArray.length !== 0) {
            bootbox.confirm(action + " " + idArray.length + " " + opts.entityName + "؟", function (result) {
                if (result) {
                    var dataString = new FormData(),
                        json = JSON.stringify(idArray);
                    dataString.append("jsonArray", json);
                    Ajax("post", url, dataString);
                }
            });
        }
        else {
            bootbox.alert("چیزی انتخاب نشده!");
        }
    }

    //Ajax
    //----------------------------------------------------------------------
    function Ajax(type, url, data) {
        $.ajax({
            type: type,
            url: url,
            data: data,
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                bootbox.alert(response, function () {
                    window.location.reload();
                });
            }
        });
        bootbox.dialog({
            title: "شکیبا باشید!",
            message: "لطفا کمی صبر کنید..."
        });
    }

    //createAndFillArray
    //----------------------------------------------------------------------
    function createAndFillArray(selector) {
        var array = new Array();
        $(selector).each(function (index, element) {
            if ($(element).is(":checked")) {
                var id = (opts.idType === "int") ? parseInt($(this).attr("data-id")) : (opts.idType === "string") ? $(this).attr("data-id") : bootbox.alert("نوع شناسه را مشخص کنید.");
                array.push(id);
            }
        });
        return array;
    }
    //----------------------------------------------------------------------
}