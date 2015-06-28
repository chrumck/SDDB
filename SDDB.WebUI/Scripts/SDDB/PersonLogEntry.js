﻿/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var TableLogEntryAssysAdd = {}; var TableLogEntryAssysRemove = {};
var TableLogEntryPersonsAdd = {}; var TableLogEntryPersonsRemove = {};
var MsFilterByProject = {}; var MsFilterByType = {}; var MsFilterByPerson = {};
var MagicSuggests = [];
var CurrRecord = {}; var CurrIds = [];
var GetActive = true;


$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        FillFormForCreateGeneric("EditForm", MagicSuggests, "Create Log Entry", "MainView");
        MagicSuggests[3].disable(); MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw(); TableLogEntryAssysRemove.clear().search("").draw();

        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
        FillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
            "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
            "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
            "GET", "/PersonSrv/Get", { getActive: true })
            .always(function () { $("#ModalWait").modal("hide"); })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {

        if (GetActive) $("#EditFormGroupIsActive").addClass("hide"); 
        else $("#EditFormGroupIsActive").removeClass("hide");

        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();

        if (CurrIds.length == 0) ShowModalNothingSelected();
        else {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            $.when(
                FillFormForEditGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                    GetActive, "EditForm", "Edit Person Log Entry", MagicSuggests),

                FillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                    "GET", "/PersonSrv/Get", { getActive: true })



                )
                .then(function (currRecord) {
                    CurrRecord = currRecord;
                    return FillFormForRelatedGeneric(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys", { logEntryId: CurrIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot", { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                        "GET", "AssemblyDbSrv/LookupByLocDTables", { getActive: true });

                })
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds == 0) ShowModalNothingSelected();
        else ShowModalDelete(CurrIds.length);
    });

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { RefreshMainView(); });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { RefreshMainView(); });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/PersonActivityTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByType).on('selectionchange', function (e, m) { RefreshMainView(); });

    //Initialize MagicSuggest MsFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on('selectionchange', function (e, m) { RefreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByPerson).on('selectionchange', function (e, m) { RefreshMainView(); });
   
        
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) { GetActive = true; $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary"); }
        else { GetActive = false; $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger"); }
        RefreshMainView();
    });

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
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
            { targets: [0, 9, 10, 11, 12, 13, 14], visible: false }, // - never show
            { targets: [0, 1, 4, 9, 10, 11, 12, 13, 14], searchable: false },  //"orderable": false, "visible": false
            { targets: [5, 6], className: "hidden-xs" }, // - first set of columns
            { targets: [3, 4], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7, 8], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns
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
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    AddToMSArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    AddToMSArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    
    //Initialize MagicSuggest Array Event
    $(MagicSuggests[2]).on('selectionchange', function (e, m) {
        if (this.getValue().length == 0) {
            MagicSuggests[3].disable(); MagicSuggests[3].clear(true);
            MagicSuggests[4].disable(); MagicSuggests[4].clear(true);
            TableLogEntryAssysAdd.clear().search("").draw();
            TableLogEntryAssysRemove.clear().search("").draw();
        }
        else {
            MagicSuggests[3].enable(); MagicSuggests[3].clear(true);
            MagicSuggests[4].enable(); MagicSuggests[4].clear(true);
        }
    });

    //Initialize MagicSuggest Array Event
    $(MagicSuggests[3]).on('selectionchange', function (e, m) {
        if (this.getValue().length == 0) {
            TableLogEntryAssysAdd.clear().search("").draw();
        }
        else {
            
            if (CurrIds.length == 1) {
                RefreshTblGenWrp(TableLogEntryAssysAdd, "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                    { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] }, "GET")
                    .done(function () { $("#AssignedToLocation_Id input").focus(); });
            }
            else {
                RefreshTblGenWrp(TableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
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
        MsValidate(MagicSuggests);
        if (FormIsValid("EditForm", CurrIds.length == 0) && MsIsValid(MagicSuggests)) {

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            SubmitEditsGeneric(CurrIds, "EditForm", MagicSuggests, CurrRecord, "POST", "/PersonLogEntrySrv/Edit")
                .then(function (data) {

                    var deferred0 = $.Deferred();

                    var ids = (CurrIds.length == 0) ? data.ReturnIds : CurrIds;
                    var idsAssysAdd = TableLogEntryAssysAdd.cells(".ui-selected", "Id:name").data().toArray();
                    var idsAssysRemove = TableLogEntryAssysRemove.cells(".ui-selected", "Id:name").data().toArray();
                    var idsPersonsAdd = TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name").data().toArray();
                    var idsPersonsRemove = TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name").data().toArray();

                    $.when(
                        SubmitEditsForRelatedGeneric(ids, idsAssysAdd, idsAssysRemove, "/PersonLogEntrySrv/EditPrsLogEntryAssys"),
                        SubmitEditsForRelatedGeneric(ids, idsPersonsAdd, idsPersonsRemove, "/PersonLogEntrySrv/EditPrsLogEntryPersons")
                        )
                        .done(function () { deferred0.resolve(); })
                        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

                    return deferred0.promise();
                })
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    RefreshMainView();
                    $("#MainView").removeClass("hide");
                    $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
                })
                .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
        }

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

    //--------------------------------------View Initialization------------------------------------//

    $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#FilterDateEnd").val(moment().format("YYYY-MM-DD"));

    //if (typeof assyId !== "undefined" && assyId != "") {
    //    $.ajax({
    //        type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 20000,
    //        data: { ids: [assyId], getActive: true }, dataType: "json",
    //        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    //    })
    //        .always(function () { $("#ModalWait").modal("hide"); })
    //        .done(function (data) {
    //            MsFilterByAssy.setSelection([{ id: data[0].Id, name: data[0].AssyName,  }]);
    //        })
    //        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    //}

    

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function DeleteRecords() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
    $.ajax({ type: "POST", url: "/PersonLogEntrySrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json"})
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshMainView(); })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}
//refresh view after magicsuggest update
function RefreshMainView() {
    if ($("#FilterDateStart").val() == "" || $("#FilterDateEnd").val() == "" ||
            (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0
                && MsFilterByPerson.getValue().length == 0)
        ){
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = moment($("#FilterDateEnd").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");

        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

        RefreshTableGeneric(TableMain, "/PersonLogEntrySrv/GetByFilterIds", { personIds: MsFilterByPerson.getValue(), 
                projectIds: MsFilterByProject.getValue(), typeIds: MsFilterByType.getValue(),
                startDate: $("#FilterDateStart").val(), endDate: endDate, getActive: GetActive}, "POST")
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function () {
                $("#ChBoxShowDeleted").bootstrapToggle("enable");
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }

}

function SetLogEntryFilesTable() {
    var deferred0 = $.Deferred();

    if (CurrIds.length == 1) {
        $("#LogEntryFilesHeading").text("Files"); $("#LogEntryFilesBtnGroup").removeClass("hide");
        RefreshTableGeneric(TableLogEntryFiles, "/PersonLogEntrySrv/GetFiles", { id: CurrIds[0] }, "GET")
        .done(function () { deferred0.resolve(); })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
    }
    else {
        $("#LogEntryFilesHeading").text("Files - disabled if multiple"); $("#LogEntryFilesBtnGroup").addClass("hide");
        deferred0.resolve();
    }

    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//


