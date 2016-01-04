/*global sddb */
/// <reference path="Shared_Views.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//


//TODO:file not finished. Needs to be refactored in order to be able serve document and other files



//--------------------------------------Global Properties------------------------------------//

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
        sddb.updateIdsResolveIfOneSelected()
            .then(function () {
                var selectedRecord = sddb.cfg.tableMain.row(".ui-selected", { page: "current" }).data();
                $("#logEntryFilesViewPanel").text(selectedRecord.EnteredByPerson_.FirstName + " " +
                    selectedRecord.EnteredByPerson_.LastName + " - " + selectedRecord.LogEntryDateTime);
                return sddb.fillLogEntryFilesForm();
            })
            .then(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView(sddb.cfg.mainViewId, "logEntryFilesView", "tdo-btngroup-logentryfiles");
            });
    });

    //---------------------------------------editFormView----------------------------------------//

    //Wire Up editFormBtnOkFiles
    $("#editFormBtnOkFiles").click(function () {
        $("#logEntryFilesViewPanel").text($("#LogEntryDateTime").val() +
            " " + magicSuggests[0].getSelection()[0].name);
        sddb.submitEditForm()
            .then(function () {
                return sddb.fillLogEntryFilesForm();
            })
            .done(function () {
                sddb.switchView(sddb.cfg.mainViewId, "logEntryFilesView", "tdo-btngroup-logentryfiles");
            });
    });

    //--------------------------------------logEntryFilesView---------------------------------------//

    //Wire Up logEntryFilesViewBtnBack
    $("#logEntryFilesViewBtnBack").click(function() {
        sddb.refreshMainView()
            .done(function () {
                sddb.switchView("logEntryFilesView", "mainView", "tdo-btngroup-main", tableMain);
            });
    });

    //Wire Up logEntryFilesBtnDload
    $("#logEntryFilesBtnDload").click(function () {
        var fileIds = tableLogEntryFiles.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (fileIds.length == 0) {
            sddb.showModalNothingSelected();
            return;
        }

        sddb.showModalWait();
        $("#logEntryFilesIframe").contents().find("body").html("");

        dlToken = new Date().getTime(); dlAttempts = 60;
        dlTimer = window.setInterval(function () {
            if ((getCookie("dlToken") == dlToken) || (dlAttempts == 0)) {
                sddb.hideModalWait();
                window.clearInterval(dlTimer);
                expireCookie("dlToken");
                if (dlAttempts == 0) {
                    sddb.showModalFail("Server Error", "Server response timed out.");
                    return;
                }
                var iFrameBodyHtml = $("#logEntryFilesIframe").contents().find("body").html();
                if (typeof iFrameBodyHtml !== "undefined" && iFrameBodyHtml != "") {
                    sddb.showModalFail("Server Error", $("#logEntryFilesIframe").contents().find("body").html());
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
                        sddb.showModalFail("Upload Too Large",
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
                    .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });

            } else sddb.showModalFail("Browser Error", "This browser doesn't support HTML5 file uploads!");
        }
        $(e.target).val("");
    });


    //Wire Up logEntryFilesBtnDelete 
    $("#logEntryFilesBtnDelete").click(function () {
        filecurrIds = tableLogEntryFiles.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (filecurrIds.length == 0) sddb.showModalNothingSelected();
        else {
            $("#modalDeleteFilesBody").text("Confirm deleting " + filecurrIds.length + " file(s).");
            $("#modalDeleteFiles").modal("show");
        }
    });

    //------------------------------------DataTables - Log Entry Files ---

    //tableLogEntryFiles
    sddb.tableLogEntryFiles = $("#tableLogEntryFiles").DataTable({
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
        sddb.showModalWait();
        $.ajax({
            type: "POST", url: "/PersonLogEntrySrv/DeleteFiles", timeout: 120000,
            data: { logEntryId: currentIds[0], ids: filecurrIds }, dataType: "json"
        })
            .always(sddb.hideModalWait)
            .done(function () { fillLogEntryFilesForm(); })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });
    });

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Fill form showing log entry files
sddb.fillLogEntryFilesForm = function () {
    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(tableLogEntryFiles,
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
