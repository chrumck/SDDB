/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//
var TableLogEntryFiles = {};
var FileCurrNames = [];
var DlToken;
var DlTimer;
var DlAttempts;
var XHR = new window.XMLHttpRequest();

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnEditLogEntryFiles 
    $("#BtnEditLogEntryFiles").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length != 1) showModalSelectOne();
        else { fillLogEntryFilesForm(); }
    });

    //--------------------------------------LogEntryFilesView---------------------------------------//

    //Wire Up EditFormBtnCancel
    $("#LogEntryFilesViewBtnCancel, #LogEntryFilesViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#LogEntryFilesView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up LogEntryFilesBtnDload
    $("#LogEntryFilesBtnDload").click(function () {
        var fileIds = TableLogEntryFiles.cells(".ui-selected", "Id:name").data().toArray();
        if (fileIds.length == 0) showModalNothingSelected();
        else {
            showModalWait();

            DlToken = new Date().getTime(); DlAttempts = 60;
            DlTimer = window.setInterval(function () {
                if ((getCookie("DlToken") == DlToken) || (DlAttempts == 0)) {
                    hideModalWait();
                    window.clearInterval(DlTimer);
                    expireCookie("DlToken");
                    if (DlAttempts == 0) showModalFail("Server Error", "Server response timed out.");
                    else if ($("#LogEntryFilesIframe").contents().find("body").html() != "")
                        showModalFail("Server Error", $("#LogEntryFilesIframe").contents().find("body").html());
                }
                else DlAttempts--;
            }, 500);

            $("#LogEntryFilesIframe").contents().find("body").html("");

            var form = $('<form method="POST" action="/PersonLogEntrySrv/DownloadFiles" target="LogEntryFilesIframe">');
            form.append($('<input type="hidden" name="DlToken" value="' + DlToken + '">'));
            form.append($('<input type="hidden" name="id" value="' + CurrIds[0] + '">'));
            $.each(fileIds, function (i, name) { form.append($('<input type="hidden" name="fileIds[' + i + ']" value="' + name + '">')); });
            $("body").append(form);
            form.submit();
        }
    });

    //wire up LogEntryFilesBtnUpload
    $("#LogEntryFilesBtnUpload").on("change", function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {

                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                $("#ModalUpload").modal({ show: true, backdrop: "static", keyboard: false });
                $.ajax({
                    type: "POST", url: "/PersonLogEntrySrv/UploadFiles?logEntryId=" + CurrIds[0],
                    contentType: false, processData: false, data: data,
                    xhr: function () {
                        XHR.upload.addEventListener("progress", function (e) {
                            if (e.lengthComputable) {
                                var PROGRESS = "Progress: " + Math.round((e.loaded / e.total) * 100) + "%";
                                $("#ModalUploadBody").text(PROGRESS);
                            }
                        }, false); return XHR;
                    }
                })
                    .always(function () { $("#ModalUpload").modal("hide"); })
                    .done(function () { setTimeout(fillLogEntryFilesForm, 200); })
                    .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

            } else showModalFail("Browser Error", "This browser doesn't support HTML5 file uploads!");
        }
        $(e.target).val("");
    });


    //Wire Up LogEntryFilesBtnDelete 
    $("#LogEntryFilesBtnDelete").click(function () {
        FileCurrNames = TableLogEntryFiles.cells(".ui-selected", "Name:name").data().toArray();
        if (FileCurrNames.length == 0) showModalNothingSelected();
        else {
            $("#ModalDeleteFilesBody").text("Confirm deleting " + FileCurrNames.length + " file(s).");
            $("#ModalDeleteFiles").modal("show");
        }
    });
    //Wire Up ModalUploadBtnAbort
    $("#ModalUploadBtnAbort").click(function () { XHR.abort(); $("#ModalUpload").modal("hide"); });

    //Get focus on ModalDeleteBtnCancel
    $("#ModalDeleteFiles").on("shown.bs.modal", function () { $("#ModalDeleteFilesBtnCancel").focus(); });

    //Wire Up ModalDeleteBtnCancel 
    $("#ModalDeleteFilesBtnCancel").click(function () { $("#ModalDeleteFiles").modal("hide"); });

    //Wire Up ModalDeleteFilesBtnOk
    $("#ModalDeleteFilesBtnOk").click(function () {
        $("#ModalDeleteFiles").modal("hide");
        showModalWait();
        $.ajax({
            type: "POST", url: "/PersonLogEntrySrv/DeleteFiles", timeout: 20000,
            data: { id: CurrIds[0], names: FileCurrNames }, dataType: "json"
        })
            .always(hideModalWait)
            .done(function () { setTimeout(fillLogEntryFilesForm, 200); })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //------------------------------------DataTables - Log Entry Files ---

    //TableLogEntryFiles
    TableLogEntryFiles = $("#TableLogEntryFiles").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "FileName", name: "FileName" },//1
            { data: "FileType", name: "FileType" },//2
            { data: "FileSize", name: "FileSize" },//3
            { data: "FileDateTime", name: "FileDateTime" },//4
            { data: "LastSavedByPerson_", render: function (data, type, full, meta) { return data.Initials }, name: "LastSavedByPerson_" }, //5
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 5], className: "hidden-xs" }, // - first set of columns
            { targets: [4], className: "hidden-xs hidden-sm" }, // - first set of columns
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 10
    });
        

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Fill form showing log entry files
function fillLogEntryFilesForm(panelText) {
    var deferred0 = $.Deferred();

    var selectedRecord = TableMain.row(".ui-selected").data();
    if (typeof panelText !== "undefined") { $("#LogEntryFilesViewPanel").text(panelText); }
    else if (typeof selectedRecord !== "undefined") {
        $("#LogEntryFilesViewPanel").text(selectedRecord.EnteredByPerson_.FirstName + " " +
            selectedRecord.EnteredByPerson_.LastName + " - " + selectedRecord.LogEntryDateTime);
    }
    else {
        $("#LogEntryFilesViewPanel").text("New Log Entry");
    }

    showModalWait();

    refreshTableGeneric(TableLogEntryFiles, "/PersonLogEntrySrv/ListFiles", { logEntryId: CurrIds[0] }, "GET")
        .always(hideModalWait)
        .done(function () {
            $("#MainView").addClass("hide");
            $("#LogEntryFilesView").removeClass("hide");
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); deferred0.reject(); });

    return deferred0.promise();
}

//---------------------------------------Helper Methods--------------------------------------//

//gets cookie by name
function getCookie(name) {
    var parts = document.cookie.split(name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}

//expires cookie by name
function expireCookie(name) {
    document.cookie = encodeURIComponent(name) + "=deleted; expires=" + new Date(0).toUTCString();
}
