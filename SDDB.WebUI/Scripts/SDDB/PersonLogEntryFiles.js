/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//
var tableLogEntryFiles = {};
var filecurrIds = [];
var dlToken;
var dlTimer;
var dlAttempts;
var XHR = new window.XMLHttpRequest();
var MAX_UPLOAD_SIZE = 20971520;

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//

    //Wire up btnEditLogEntryFiles 
    $("#btnEditLogEntryFiles").click(function (event) {
        event.preventDefault();
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length != 1) {
            showModalSelectOne();
            return;
        }

        var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
        $("#logEntryFilesViewPanel").text(selectedRecord.EnteredByPerson_.FirstName +
            " " + selectedRecord.EnteredByPerson_.LastName + " - " +selectedRecord.LogEntryDateTime);
        
        fillLogEntryFilesForm()
            .done(function () {
                saveViewSettings(tableMain);
                switchView("mainView", "logEntryFilesView", "tdo-btngroup-logentryfiles");
            });
    });

    //---------------------------------------editFormView----------------------------------------//

    //Wire Up editFormBtnOkFiles
    $("#editFormBtnOkFiles").click(function () {
        $("#logEntryFilesViewPanel").text($("#LogEntryDateTime").val() +
            " " + magicSuggests[0].getSelection()[0].name);
        submitEditForm()
            .then(function () { return fillLogEntryFilesForm(); })
            .done(function () { switchView(mainViewId, "logEntryFilesView", "tdo-btngroup-logentryfiles"); });
    });

    //--------------------------------------logEntryFilesView---------------------------------------//

    //Wire Up logEntryFilesViewBtnBack
    $("#logEntryFilesViewBtnBack").click(function() {
        refreshMainView()
            .done(function () {
                switchView("logEntryFilesView", "mainView", "tdo-btngroup-main", tableMain);
            });
    });

    //Wire Up logEntryFilesBtnDload
    $("#logEntryFilesBtnDload").click(function () {
        var fileIds = tableLogEntryFiles.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (fileIds.length == 0) {
            showModalNothingSelected();
            return;
        }

        showModalWait();
        $("#logEntryFilesIframe").contents().find("body").html("");

        dlToken = new Date().getTime(); dlAttempts = 60;
        dlTimer = window.setInterval(function () {
            if ((getCookie("dlToken") == dlToken) || (dlAttempts == 0)) {
                hideModalWait();
                window.clearInterval(dlTimer);
                expireCookie("dlToken");
                if (dlAttempts == 0) {
                    showModalFail("Server Error", "Server response timed out.");
                    return;
                }
                var iFrameBodyHtml = $("#logEntryFilesIframe").contents().find("body").html();
                if (typeof iFrameBodyHtml !== "undefined" && iFrameBodyHtml != "") {
                    showModalFail("Server Error", $("#logEntryFilesIframe").contents().find("body").html());
                    return;
                }
            }
            dlAttempts--
        }, 500);

        var form = $('<form method="POST" action="/PersonLogEntrySrv/DownloadFiles" target="logEntryFilesIframe">');
        form.append($('<input type="hidden" name="dlToken" value="' + dlToken + '">'));
        form.append($('<input type="hidden" name="id" value="' + currentIds[0] + '">'));
        $.each(fileIds, function (i, name) { form.append($('<input type="hidden" name="fileIds[' + i + ']" value="' + name + '">')); });
        $("body").append(form);
        form.submit();
    });

    //wire up logEntryFilesBtnUpload
    $("#logEntryFilesBtnUpload").on("change", function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {

                var data = new FormData();
                var dataSize = 0;
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                    dataSize += files[x].size;
                    if (dataSize > MAX_UPLOAD_SIZE) {
                        showModalFail("Upload Too Large",
                            "The total single upload is limited to " + MAX_UPLOAD_SIZE / 1024 + " kB");
                        return;
                    }
                }

                $("#modalUpload").modal({ show: true, backdrop: "static", keyboard: false });
                $.ajax({
                    type: "POST", url: "/PersonLogEntrySrv/UploadFiles?logEntryId=" + currentIds[0],
                    contentType: false, processData: false, data: data,
                    xhr: function () {
                        XHR.upload.addEventListener("progress", function (e) {
                            if (e.lengthComputable) {
                                var PROGRESS = "Progress: " + Math.round((e.loaded / e.total) * 100) + "%";
                                $("#modalUploadBody").text(PROGRESS);
                            }
                        }, false); return XHR;
                    }
                })
                    .always(function () { $("#modalUpload").modal("hide"); })
                    .done(function () { fillLogEntryFilesForm(); })
                    .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

            } else showModalFail("Browser Error", "This browser doesn't support HTML5 file uploads!");
        }
        $(e.target).val("");
    });


    //Wire Up logEntryFilesBtnDelete 
    $("#logEntryFilesBtnDelete").click(function () {
        filecurrIds = tableLogEntryFiles.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (filecurrIds.length == 0) showModalNothingSelected();
        else {
            $("#modalDeleteFilesBody").text("Confirm deleting " + filecurrIds.length + " file(s).");
            $("#modalDeleteFiles").modal("show");
        }
    });

    //------------------------------------DataTables - Log Entry Files ---

    //tableLogEntryFiles
    tableLogEntryFiles = $("#tableLogEntryFiles").DataTable({
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

    //-------------------------------------modalUpload dialog---------------------------------------//

    //Wire Up modalUploadBtnAbort
    $("#modalUploadBtnAbort").click(function () { XHR.abort(); $("#modalUpload").modal("hide"); });

    //-----------------------------------modalDeleteFiles dialog------------------------------------//

    //Get focus on ModalDeleteBtnCancel
    $("#modalDeleteFiles").on("shown.bs.modal", function () { $("#modalDeleteFilesBtnCancel").focus(); });

    //Wire Up ModalDeleteBtnCancel 
    $("#modalDeleteFilesBtnCancel").click(function () { $("#modalDeleteFiles").modal("hide"); });

    //Wire Up modalDeleteFilesBtnOk
    $("#modalDeleteFilesBtnOk").click(function () {
        $("#modalDeleteFiles").modal("hide");
        showModalWait();
        $.ajax({
            type: "POST", url: "/PersonLogEntrySrv/DeleteFiles", timeout: 120000,
            data: { logEntryId: currentIds[0], ids: filecurrIds }, dataType: "json"
        })
            .always(hideModalWait)
            .done(function () { fillLogEntryFilesForm(); })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Fill form showing log entry files
function fillLogEntryFilesForm() {
    return modalWaitWrapper(function () {
        return refreshTableGeneric(tableLogEntryFiles,
            "/PersonLogEntrySrv/ListFiles", { logEntryId: currentIds[0] }, "GET");
    });
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
