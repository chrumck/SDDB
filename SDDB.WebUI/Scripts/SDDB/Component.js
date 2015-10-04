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
var MsFilterByProject = {};
var MsFilterByType = {};
var MsFilterByLoc = {};
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    CompName: null,
    CompAltName: null,
    CompAltName2: null,
    PositionInAssy: null,
    ProgramAddress: null,
    CalibrationReqd_bl: null,
    LastCalibrationDate: null,
    Comments: null,
    IsActive_bl: null,
    ComponentType_Id: null,
    ComponentStatus_Id: null,
    ComponentModel_Id: null,
    AssignedToProject_Id: null,
    AssignedToAssemblyDb_Id: null
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
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Component", "MainView");
        saveViewSettings(TableMain);
        switchView("MainView", "EditFormView", "tdo-btngroup-edit");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
        fillFormForEditGeneric(CurrIds, "POST", "/ComponentSrv/GetByIds",
                GetActive, "EditForm", "Edit Component", MagicSuggests)
            .always(hideModalWait)
            .done(function (currRecords) {
                CurrRecords = currRecords;
                saveViewSettings(TableMain);
                switchView("MainView", "EditFormView", "tdo-btngroup-edit");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(true);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(true);
    });

    //wire up dropdownId3 - Show Comp. Log
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/ComponentLogEntry?ComponentId=" +
            TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data())
    });

    //wire up dropdownId4 - Go to Extended
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        var modelIds = TableMain.cells(".ui-selected", "ComponentModel_Id:name", { page: "current" }).data().toArray();
        if (!modelIdsAreSame(modelIds)) {
            showModalFail("Error", "Selected records have no models or their models are not the same.");
            return;
        }
        var newWindowName = moment().format("YYYYDDMMHHmmss");
        window.open("about:blank", newWindowName);
        submitFormFromArray("POST", "/ComponentExt", newWindowName, CurrIds, "ComponentIds");
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/ComponentTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByType
    $(MsFilterByType).on('selectionchange', function (e, m) { refreshMainView(); });

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
    //Wire up on change event for MsFilterByProject
    $(MsFilterByProject).on('selectionchange', function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByAssy
    MsFilterByAssy = $("#MsFilterByAssy").magicSuggest({
        data: "/AssemblyDbSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: MsFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByAssy
    $(MsFilterByAssy).on('selectionchange', function (e, m) { refreshMainView(); });


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

    //TableMain Components
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompName", name: "CompName" },//1
            //------------------------------------------------first set of columns
            { data: "CompAltName", name: "CompAltName" },//2
            { data: "CompAltName2", name: "CompAltName2" },//3
            { data: "ComponentType_", render: function (data, type, full, meta) { return data.CompTypeName }, name: "ComponentType_" }, //4
            { data: "ComponentStatus_", render: function (data, type, full, meta) { return data.CompStatusName }, name: "ComponentStatus_" }, //5
            { data: "ComponentModel_", render: function (data, type, full, meta) { return data.CompModelName }, name: "ComponentModel_" }, //6
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //7
            //------------------------------------------------second set of columns
            { data: "AssignedToAssemblyDb_", render: function (data, type, full, meta) { return data.AssyName }, name: "AssignedToAssemblyDb_" }, //8
            { data: "PositionInAssy", name: "PositionInAssy" },//9
            { data: "ProgramAddress", name: "ProgramAddress" },//10
            { data: "CalibrationReqd_bl", name: "CalibrationReqd_bl" },//11
            { data: "LastCalibrationDate", name: "LastCalibrationDate" },//12
            { data: "Comments", name: "Comments" },//13
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//14
            { data: "ComponentType_Id", name: "ComponentType_Id" },//15
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//16
            { data: "ComponentModel_Id", name: "ComponentModel_Id" },//17
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//18
            { data: "AssignedToAssemblyDb_Id", name: "AssignedToAssemblyDb_Id" }//19
        ],
        columnDefs: [
            { targets: [0, 14, 15, 16, 17, 18, 19], visible: false }, // - never show
            { targets: [0, 11, 12, 14, 15, 16, 17, 18, 19], searchable: false },  //"orderable": false, "visible": false
            { targets: [4, 6, 7], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [2, 3], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [8, 9, 10, 11, 12, 13], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 11, 12], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [10, 13], className: "hidden-xs hidden-sm hidden-md" } // - second set of columns
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

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "ComponentType_Id", "/ComponentTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ComponentModel_Id", "/ComponentModelSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

    //Enable DateTimePicker
    $("[data-val-dbisdateiso]").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel").click(function () {
        switchView("EditFormView","MainView", "tdo-btngroup-main", true);
    });
    
    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
            showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
            return;
        }
        showModalWait();
        var createMultiple = $("#CreateMultiple").val() != "" ? $("#CreateMultiple").val() : 1;
        submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/ComponentSrv/Edit", createMultiple)
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("EditFormView", "MainView", "tdo-btngroup-main", true, TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
    });

    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);
    
    $("#InitialView").addClass("hidden");
    $("#MainView").removeClass("hidden");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//Delete Records from DB
function deleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/ComponentSrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {
    var deferred0 = $.Deferred();

    TableMain.clear().search("").draw();

    if (MsFilterByType.getValue().length == 0 &&
        MsFilterByProject.getValue().length == 0 &&
        MsFilterByAssy.getValue().length == 0)
    {
        if (typeof ComponentIds !== "undefined" && ComponentIds != null && ComponentIds.length > 0) {
            refreshTblGenWrp(TableMain, "/ComponentSrv/GetByIds", { ids: ComponentIds, getActive: GetActive }, "POST")
                .done(deferred0.resolve);
        }
        return deferred0.promise();
    }

    refreshTblGenWrp(TableMain, "/ComponentSrv/GetByAltIds2",
        {
            projectIds: MsFilterByProject.getValue(),
            typeIds: MsFilterByType.getValue(),
            assyIds: MsFilterByAssy.getValue(),
            getActive: GetActive
        },
        "POST")
        .done(deferred0.resolve);

    return deferred0.promise();
}

//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() { 
    var deferred0 = $.Deferred();
    if (typeof AssemblyId !== "undefined" && AssemblyId != "") {
        showModalWait();
        $.ajax({
            type: "POST",
            url: "/AssemblyDbSrv/GetByIds",
            timeout: 120000,
            data: { ids: [AssemblyId], getActive: true },
            dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                msSetSelectionSilent(MsFilterByAssy, [{ id: data[0].Id, name: data[0].AssyName}]);
                return deferred0.resolve();
            })
            .fail(function(xhr, status, error) {
                showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { return deferred0.resolve(); }
    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//

