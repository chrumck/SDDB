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
var MsFilterByProject;
var MsFilterByComponent;
var MsFilterByPerson;
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    LogEntryDateTime: null,
    Component_Id: null,
    ComponentStatus_Id: null,
    AssignedToProject_Id: null,
    AssignedToAssemblyDb_Id: null,
    LastCalibrationDate: null,
    Comments: null,
    IsActive_bl: null
};
var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Log Entry", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            if (GetActive) $("#EditFormGroupIsActive").addClass("hide");
            else $("#EditFormGroupIsActive").removeClass("hide");

            showModalWait();

            fillFormForEditGeneric(CurrIds, "POST", "/ComponentLogEntrySrv/GetByIds", GetActive, "EditForm", "Edit Log Entry", MagicSuggests)
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function (currRecords) {
                    CurrRecords = currRecords;
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
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
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByComponent
    MsFilterByComponent = $("#MsFilterByComponent").magicSuggest({
        data: "/ComponentSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    $(MsFilterByComponent).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    $(MsFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });
   
        
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) {
            GetActive = true;
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        } else {
            GetActive = false;
            $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        }
        refreshMainView();
    });

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            { data: "Component_", render: function (data, type, full, meta) { return data.CompName }, name: "Component_" }, //2
            { data: "LastSavedByPerson_", render: function (data, type, full, meta) { return data.LastName + " " + data.Initials }, name: "LastSavedByPerson_" }, //3
            { data: "ComponentStatus_", render: function (data, type, full, meta) { return data.CompStatusName }, name: "ComponentStatus_" }, //4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //5
            { data: "AssignedToAssemblyDb_", render: function (data, type, full, meta) { return data.AssyName }, name: "AssignedToAssemblyDb_" }, //6
            { data: "LastCalibrationDate", name: "LastCalibrationDate" },//7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//9
            { data: "Component_Id", name: "Component_Id" },//10
            { data: "LastSavedByPerson_Id", name: "LastSavedByPerson_Id" },//11
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//12
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//13
            { data: "AssignedToAssemblyDb_Id", name: "AssignedToAssemblyDb_Id" }//14
        ],
        columnDefs: [
            { targets: [0, 9, 10, 11, 12, 13, 14], visible: false }, // - never show
            { targets: [0, 1, 7, 9, 10, 11, 12, 13, 14], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 4], className: "hidden-xs" }, // - first set of columns
            { targets: [5, 6], className: "hidden-xs hidden-sm" }, // - first set of columns
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

    //Enable DateTimePicker
    $("#LastCalibrationDate").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "Component_Id", "/ComponentSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            showModalWait();
            submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/ComponentLogEntrySrv/Edit")
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

    if (typeof ComponentId !== "undefined" && ComponentId != "") {
        showModalWait();
        $.ajax({
            type: "POST", url: "/ComponentSrv/GetByIds", timeout: 120000, data: { ids: [ComponentId], getActive: true }, dataType: "json"})
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                MsFilterByComponent.setSelection([{ id: data[0].Id, name: data[0].CompName, }]);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
    else {
        $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
        $("#FilterDateEnd").val(moment().format("YYYY-MM-DD"));
        refreshMainView();
    }
    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");
    
    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/ComponentLogEntrySrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {

    if ( ($("#FilterDateStart").val() == "" || $("#FilterDateEnd").val() == "") &&
        (MsFilterByProject.getValue().length == 0 && MsFilterByComponent.getValue().length == 0 &&
                MsFilterByPerson.getValue().length == 0) ) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = ($("#FilterDateEnd").val() == "") ? "" : moment($("#FilterDateEnd").val())
            .hour(23).minute(59).format("YYYY-MM-DD HH:mm");

        refreshTblGenWrp(TableMain, "/ComponentLogEntrySrv/GetByAltIds",
            {projectIds: MsFilterByProject.getValue(), componentIds: MsFilterByComponent.getValue(),
            personIds: MsFilterByPerson.getValue(), startDate: $("#FilterDateStart").val(), endDate: endDate,
            getActive: GetActive }, "POST")
            .done(function () { $("#ChBoxShowDeleted").bootstrapToggle("enable"); })
    }
}


//---------------------------------------Helper Methods--------------------------------------//





