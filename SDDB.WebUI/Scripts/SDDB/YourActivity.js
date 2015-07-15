/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//


var TableMain;
var TableLogEntryAssysAdd;
var TableLogEntryAssysRemove;
var TableLogEntryPersonsAdd;
var TableLogEntryPersonsRemove;
var MsFilterByPerson;
var MsFilterByType;
var MsFilterByProject;
var MsFilterByAssy;
var ModalChngStsMs;
var MagicSuggests = [];
var CurrRecord = {
    Id: null,
    LogEntryDateTime: null,
    EnteredByPerson_Id: null,
    PersonActivityType_Id: null,
    ManHours: null,
    AssignedToProject_Id: null,
    AssignedToLocation_Id: null,
    AssignedToProjectEvent_Id: null,
    Comments: null,
    IsActive_bl: null
};
var CurrIds = [];
var FileCurrNames = [];
var ChngStsAssyIds = [];
var GetActive = true;
var DlToken;
var DlTimer;
var DlAttempts;
var XHR = new window.XMLHttpRequest();

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Activity", "MainView");
        MagicSuggests[3].disable();
        MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw();
        TableLogEntryAssysRemove.clear().search("").draw();
        MagicSuggests[0].setValue([UserId]);
        $("#HoursWorkedPicker").data("DateTimePicker").date("00:00");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length != 1) showModalSelectOne();
        else {
            if (GetActive) $("#EditFormGroupIsActive").addClass("hide");
            else $("#EditFormGroupIsActive").removeClass("hide");
            
            showModalWait();

            $.when(
                fillFormForEditGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                    GetActive, "EditForm", "Edit Activity", MagicSuggests)
                )
                .then(function (currRecord) {
                    CurrRecord = currRecord;
                    return fillFormForRelatedGeneric(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys", { logEntryId: CurrIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot", { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                        "GET", "AssemblyDbSrv/LookupByLocDTables", { getActive: true });

                })
                .always(hideModalWait)
                .done(function () {
                    $("#LogEntryTime").data('DateTimePicker').date(moment($("#LogEntryDateTime").val()));
                    $("#HoursWorkedPicker").data('DateTimePicker').date(moment($("#ManHours").val(),"HH"));
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else showModalDelete(CurrIds.length);
    });

    //Wire up BtnEditLogEntryFiles 
    $("#BtnEditLogEntryFiles").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length != 1) showModalSelectOne();
        else { fillLogEntryFilesForm(); }
    });

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });
        
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) { GetActive = true; $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary"); }
        else { GetActive = false; $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger"); }
        refreshMainView();
    });

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", render: function (data, type, full, meta) { return moment(data).format("HH:mm") }, name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            { data: "EnteredByPerson_", render: function (data, type, full, meta) { return data.LastName + " " + data.Initials }, name: "EnteredByPerson_" }, //2
            { data: "PersonActivityType_", render: function (data, type, full, meta) { return data.ActivityTypeName }, name: "PersonActivityType_" }, //3
            { data: "ManHours", name: "ManHours" },//4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //5
            { data: "AssignedToLocation_", render: function (data, type, full, meta) { return data.LocName }, name: "AssignedToLocation_" }, //6
            { data: "AssignedToProjectEvent_", render: function (data, type, full, meta) { return data.EventName }, name: "AssignedToProjectEvent_" }, //7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//9
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//10
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//11
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//12
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" },//13
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" },//14
        ],
        columnDefs: [
            { targets: [0, 2, 9, 10, 11, 12, 13, 14], visible: false }, // - never show
            { targets: [0, 1, 2, 4, 9, 10, 11, 12, 13, 14], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 6], className: "hidden-xs" }, // - first set of columns
            { targets: [7, 8], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns
        ],
        order: [[1, "asc"]],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        }
    });

    //---------------------------------------EditFormView----------------------------------------//

    //Initialize DateTimePicker
    $("#LogEntryTime").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            $("#LogEntryDateTime").val(moment($("#FilterDateStart").val()).hour($(this).data('DateTimePicker').date().hour()).format("YYYY-MM-DD HH:mm"));
            $("#LogEntryDateTime").data("ismodified", true);
        });

    //Initialize DateTimePicker
    $("#HoursWorkedPicker").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            $("#ManHours").val($(this).data('DateTimePicker').date().hour());
            $("#ManHours").data("ismodified", true);
        });

    //Initialize MagicSuggest Array
    addToMSArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1, null, {},
        false, false);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1, null,
        {}, false, false);
    addToMSArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    addToMSArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue }, false, false);
    
    //Initialize MagicSuggest Array Event
    $(MagicSuggests[2]).on("selectionchange", function (e, m) {
        MagicSuggests[3].clear(true);
        MagicSuggests[3].isModified = false;
        MagicSuggests[4].clear(true);
        MagicSuggests[4].isModified = false;
        TableLogEntryAssysAdd.clear().search("").draw();
        if (this.getValue().length == 0) {
            MagicSuggests[3].disable(); 
            MagicSuggests[4].disable();
        }
        else {
            MagicSuggests[3].enable(); 
            MagicSuggests[4].enable();
        }
    });

    //Initialize MagicSuggest Array Event
    $(MagicSuggests[3]).on("selectionchange", function (e, m) {
        if (this.getValue().length == 0) {
            TableLogEntryAssysAdd.clear().search("").draw();
        }
        else {
            
            if (CurrIds.length == 1) {
                refreshTblGenWrp(TableLogEntryAssysAdd, "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                    { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] }, "GET")
                    .done(function () { $("#AssignedToLocation_Id input").focus(); });
            }
            else {
                refreshTblGenWrp(TableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
                { getActive: true, locId: MagicSuggests[3].getValue()[0] }, "GET")
                .done(function () { $("#AssignedToLocation_Id input").focus(); });
            }
        }
    });
    
    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            submitEdits();
        }
    });

    //Wire Up EditFormBtnOkFiles
    $("#EditFormBtnOkFiles").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            submitEdits().done(function () { setTimeout(fillLogEntryFilesForm, 200); });
        }
    });

    //Wire Up EditFormBtnChngSts
    $("#EditFormBtnChngSts").click(function () {
        ChngStsAssyIds = TableLogEntryAssysAdd.cells(".ui-selected", "Id:name").data().toArray();
        if (ChngStsAssyIds.length == 0) {
            showModalNothingSelected("Please select one or more rows from 'Add Assemblies' table.");
        }
        else { showModalChngSts(); }
    });

    //------------------------------------DataTables - Log Entry Assemblies ---

    //TableLogEntryAssysAdd
    TableLogEntryAssysAdd = $("#TableLogEntryAssysAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" }//1
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false }  //"orderable": false, "visible": false
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

    //TableLogEntryAssysRemove
    TableLogEntryAssysRemove = $("#TableLogEntryAssysRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" }//1
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false }  //"orderable": false, "visible": false
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
    
    //------------------------------------DataTables - Log Entry Persons ---

    //TableLogEntryPersonsAdd
    TableLogEntryPersonsAdd = $("#TableLogEntryPersonsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
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

    //TableLogEntryPersonsRemove
    TableLogEntryPersonsRemove = $("#TableLogEntryPersonsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
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

    //--------------------------------------LogEntryFilesView---------------------------------------//

    //Wire Up EditFormBtnCancel
    $("#LogEntryFilesViewBtnCancel, #LogEntryFilesViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#LogEntryFilesView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up LogEntryFilesBtnDload
    $("#LogEntryFilesBtnDload").click(function () {
        var names = TableLogEntryFiles.cells(".ui-selected", "Name:name").data().toArray();
        if (names.length == 0) showModalNothingSelected();
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
            $.each(names, function (i, name) {form.append($('<input type="hidden" name="names[' + i + ']" value="' + name + '">')); });
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
                    type: "POST", url: "/PersonLogEntrySrv/UploadFiles?id=" + CurrIds[0],
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

            } else showModalFail("Browser Error","This browser doesn't support HTML5 file uploads!"); 
        }
        $(e.target).val("");
    });

    //Wire Up ModalUploadBtnAbort
    $("#ModalUploadBtnAbort").click(function () { XHR.abort(); $("#ModalUpload").modal("hide"); });

    //Wire Up LogEntryFilesBtnDelete 
    $("#LogEntryFilesBtnDelete").click(function () {
        FileCurrNames = TableLogEntryFiles.cells(".ui-selected", "Name:name").data().toArray();
        if (FileCurrNames.length == 0) showModalNothingSelected();
        else {
            $("#ModalDeleteFilesBody").text("Confirm deleting " + FileCurrNames.length + " file(s).");
            $("#ModalDeleteFiles").modal("show");
        }
    });

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
            { data: "Name", name: "Name" },//0
            { data: "Size", name: "Size" },//1
            { data: "Modified", name: "Modified" }//2
        ],
        columnDefs: [
            { targets: [], visible: false }, // - never show
            { targets: [], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs" }
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

    //----------------------------------Modal Dialog - Change Assy Status -----------------------//

    //Wire Up ModalDeleteBtnCancel 
    $("#ModalChngStsBtnCancel").click(function () { $("#ModalChngSts").modal("hide"); });

    //Get focus on ModalChngStsMs
    $("#ModalChngSts").on("shown.bs.modal", function () { $("#ModalChngStsMs :input").focus(); });

    //Wire up MagicSuggest ModalChngStsMs
    ModalChngStsMs = $("#ModalChngStsMs").magicSuggest({
        data: "/AssemblyStatusSrv/Lookup",
        allowFreeEntries: false,
        maxSelection: 1,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });

    //Wire Up ModalChngStsBtnOk
    $("#ModalChngStsBtnOk").click(function () {
        $("#ModalChngSts").modal("hide");
        changeAssyStatus();
    });

    //--------------------------------------View Initialization------------------------------------//

    $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
    refreshMainView();
  

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    if ($("#FilterDateStart").val() == "") {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = moment($("#FilterDateStart").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");

        refreshTblGenWrp(TableMain, "/PersonLogEntrySrv/GetByAltIds",
            {
                personIds: [UserId],
                startDate: $("#FilterDateStart").val(),
                endDate: endDate,
                getActive: GetActive
            },
            "POST")
            .done(function () { $("#ChBoxShowDeleted").bootstrapToggle("enable"); })
    }
}

//Delete Records from DB
function DeleteRecords() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    showModalWait();
    $.ajax({ type: "POST", url: "/PersonLogEntrySrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json"})
        .always(hideModalWait)
        .done(function () { refreshMainView(); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//submit edits to DB
function submitEdits() {
    var deferred0 = $.Deferred();

    showModalWait();

    submitEditsGeneric(CurrIds, "EditForm", MagicSuggests, CurrRecord, "POST", "/PersonLogEntrySrv/Edit")
        .then(function (data) {

            var deferred1 = $.Deferred();

            CurrIds = (CurrIds.length == 0) ? data.ReturnIds : CurrIds;
            var idsAssysAdd = TableLogEntryAssysAdd.cells(".ui-selected", "Id:name").data().toArray();
            var idsAssysRemove = TableLogEntryAssysRemove.cells(".ui-selected", "Id:name").data().toArray();
            var idsPersonsAdd = TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name").data().toArray();
            var idsPersonsRemove = TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name").data().toArray();

            $.when(
                submitEditsForRelatedGeneric(CurrIds, idsAssysAdd, idsAssysRemove, "/PersonLogEntrySrv/EditPrsLogEntryAssys"),
                submitEditsForRelatedGeneric(CurrIds, idsPersonsAdd, idsPersonsRemove, "/PersonLogEntrySrv/EditPrsLogEntryPersons")
                )
                .done(function () { deferred1.resolve(); })
                .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });

            return deferred1.promise();
        })
        .always(hideModalWait)
        .done(function () {
            refreshMainView();
            $("#MainView").removeClass("hide");
            $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); deferred0.reject(); });

    return deferred0.promise();
}

//Fill form showing log entry files
function fillLogEntryFilesForm() {
    var deferred0 = $.Deferred();

    var selectedRecord = TableMain.row(".ui-selected").data();
    if ( typeof selectedRecord === "undefined") { $("#LogEntryFilesViewPanel").text("New Log Entry"); }
    else {
        var selectedRecord = TableMain.row(".ui-selected").data();
        $("#LogEntryFilesViewPanel").text(selectedRecord.EnteredByPerson_.FirstName + " " +
            selectedRecord.EnteredByPerson_.LastName + " - " + selectedRecord.LogEntryDateTime);
    }

    showModalWait();

    refreshTableGeneric(TableLogEntryFiles, "/PersonLogEntrySrv/GetFiles", { id: CurrIds[0] }, "GET")
        .always(hideModalWait)
        .done(function () {
            $("#MainView").addClass("hide");
            $("#LogEntryFilesView").removeClass("hide");
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); deferred0.reject(); });

    return deferred0.promise();
}

//-----------------Modal Dialog - Change Assy Status methods ------------------

//Show modal for chagning assembly(ies) status
function showModalChngSts() {
    $("#ModalChngStsBody").text("Chagning status of " + ChngStsAssyIds.length + " assembly(ies).");
    ModalChngStsMs.clear(true);
    $("#ModalChngSts").modal("show");
}

function changeAssyStatus() {
    if (ModalChngStsMs.getValue().length == 1) {
        showModalWait();
        $.ajax({
            type: "POST", url: "/AssemblyDbSrv/EditStatus",
            timeout: 20000,
            data: {
                ids: TableLogEntryAssysAdd.cells(".ui-selected", "Id:name").data().toArray(),
                statusId: ModalChngStsMs.getValue()[0]
            },
            dataType: "json"
        })
        .always(hideModalWait)
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
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
