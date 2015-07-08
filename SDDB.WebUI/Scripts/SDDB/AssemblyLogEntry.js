﻿/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain;
var MsFilterByProject; var MsFilterByAssembly; var MsFilterByPerson;
var MagicSuggests = [];
var CurrRecord = {
    Id: null,
    LogEntryDateTime: null,
    AssemblyDb_Id: null,
    EnteredByPerson_Id: null,
    AssemblyStatus_Id: null,
    AssignedToProject_Id: null,
    AssignedToLocation_Id: null,
    AssyGlobalX: null,
    AssyGlobalY: null,
    AssyGlobalZ: null,
    AssyLocalXDesign: null,
    AssyLocalYDesign: null,
    AssyLocalZDesign: null,
    AssyLocalXAsBuilt: null,
    AssyLocalYAsBuilt: null,
    AssyLocalZAsBuilt: null,
    AssyStationing: null,
    AssyLength: null,
    Comments: null,
    IsActive_bl: null
};
var CurrIds = [];
var GetActive = true;
var SelectedRecord;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Log Entry", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            if (GetActive) $("#EditFormGroupIsActive").addClass("hide");
            else $("#EditFormGroupIsActive").removeClass("hide");

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            fillFormForEditGeneric(CurrIds, "POST", "/AssemblyLogEntrySrv/GetByIds", GetActive, "EditForm", "Edit Log Entry", MagicSuggests)
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function (currRecord) {
                    CurrRecord = currRecord;
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

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByAssembly
    MsFilterByAssembly = $("#MsFilterByAssembly").magicSuggest({
        data: "/AssemblyDbSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByAssembly).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });
   
        
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
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            { data: "Assembly_", render: function (data, type, full, meta) { return data.AssyName }, name: "Assembly_" }, //2
            { data: "EnteredByPerson_", render: function (data, type, full, meta) { return data.LastName + " " + data.Initials }, name: "EnteredByPerson_" }, //3
            { data: "AssemblyStatus_", render: function (data, type, full, meta) { return data.AssyStatusName }, name: "AssemblyStatus_" }, //4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //5
            { data: "AssignedToLocation_", render: function (data, type, full, meta) { return data.LocName }, name: "AssignedToAssemblyDb_" }, //6
            //------------------------------------------------first set of columns
            { data: "AssyGlobalX", name: "AssyGlobalX" },//7
            { data: "AssyGlobalY", name: "AssyGlobalY" },//8
            { data: "AssyGlobalZ", name: "AssyGlobalZ" },//9
            { data: "AssyLocalXDesign", name: "AssyLocalXDesign" },//10
            { data: "AssyLocalYDesign", name: "AssyLocalYDesign" },//11
            { data: "AssyLocalZDesign", name: "AssyLocalZDesign" },//12
            //------------------------------------------------first set of columns
            { data: "AssyLocalXAsBuilt", name: "AssyLocalXAsBuilt" },//13
            { data: "AssyLocalYAsBuilt", name: "AssyLocalYAsBuilt" },//14
            { data: "AssyLocalZAsBuilt", name: "AssyLocalZAsBuilt" },//15
            { data: "AssyStationing", name: "AssyStationing" },//16
            { data: "AssyLength", name: "AssyLength" },//17
            { data: "Comments", name: "Comments" },//18
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//19
            { data: "AssemblyDb_Id", name: "AssemblyDb_Id" },//20
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//21
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//22
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//23
            { data: "AssignedToAssemblyDb_Id", name: "AssignedToAssemblyDb_Id" }//24
        ],
        columnDefs: [
            { targets: [0, 19, 20, 21, 22, 23, 24], visible: false }, // - never show
            { targets: [0, 1, 19, 20, 21, 22, 23, 24], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 4], className: "hidden-xs" }, // - first set of columns
            { targets: [5, 6], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [7, 8, 9, 10, 11, 12], visible: false }, // - second set of columns - to toggle with options
            { targets: [3, 4], className: "hidden-xs" }, // - second set of columns
            { targets: [9, 10], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [11, 12, 13], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [13, 14, 15, 16, 17, 18], visible: false }, // - third set of columns - to toggle with options
            { targets: [3, 4], className: "hidden-xs" }, // - third set of columns
            { targets: [18, 19], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [14, 15, 16], className: "hidden-xs hidden-sm hidden-md" }, // - third set of columns

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

    //Enable DateTimePicker
    $("#LastCalibrationDate").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    addToMSArray(MagicSuggests, "AssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
            submitEditsGeneric(CurrIds, "EditForm", MagicSuggests, CurrRecord, "POST", "/AssemblyLogEntrySrv/Edit")
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    refreshMainView();
                    $("#MainView").removeClass("hide");
                    $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
        }
    });

    //--------------------------------------View Initialization------------------------------------//

    if (typeof assyId !== "undefined" && assyId != "") {
        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
        $.ajax({
            type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 20000, data: { ids: [assyId], getActive: true }, dataType: "json"})
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                MsFilterByAssembly.setSelection([{ id: data[0].Id, name: data[0].AssyName, }]);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
    
    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
    $.ajax({ type: "POST", url: "/AssemblyLogEntrySrv/Delete", timeout: 20000, data: { ids: CurrIds }, dataType: "json" })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { refreshMainView(); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//refresh view after magicsuggest update
function refreshMainView() {

    if ( ($("#FilterDateStart").val() == "" && $("#FilterDateEnd").val() == "") &&
        (MsFilterByProject.getValue().length == 0 && MsFilterByAssembly.getValue().length == 0 &&
                MsFilterByPerson.getValue().length == 0) ) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = ($("#FilterDateEnd").val() == "") ? "" : moment($("#FilterDateEnd").val())
            .hour(23).minute(59).format("YYYY-MM-DD HH:mm");

        refreshTblGenWrp(TableMain, "/AssemblyLogEntrySrv/GetByAltIds",
            {projectIds: MsFilterByProject.getValue(), assyIds: MsFilterByAssembly.getValue(),
            personIds: MsFilterByPerson.getValue(), startDate: $("#FilterDateStart").val(), endDate: endDate,
            getActive: GetActive }, "POST")
            .done(function () { $("#ChBoxShowDeleted").bootstrapToggle("enable"); })
    }
}


//---------------------------------------Helper Methods--------------------------------------//




