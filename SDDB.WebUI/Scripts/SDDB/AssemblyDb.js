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
    AssyName: null,
    AssyAltName: null,
    AssyAltName2: null,
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
    AssyReadingIntervalSecs: null,
    IsReference_bl: null,
    TechnicalDetails: null,
    PowerSupplyDetails: null,
    HSEDetails: null,
    Comments: null,
    IsActive_bl: null,
    AssemblyType_Id: null,
    AssemblyStatus_Id: null,
    AssemblyModel_Id: null,
    AssignedToProject_Id: null,
    AssignedToLocation_Id: null
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
        $("#EditFormCreateMultiple").removeClass("hide");
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Assembly", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        if (GetActive) { $("#EditFormGroupIsActive").addClass("hide"); }
        else { $("#EditFormGroupIsActive").removeClass("hide"); }

        $("#EditFormCreateMultiple").addClass("hide");

        showModalWait();

        fillFormForEditGeneric(CurrIds, "POST", "/AssemblyDbSrv/GetByIds",
                GetActive, "EditForm", "Edit Assembly", MagicSuggests)
            .always(hideModalWait)
            .done(function (currRecords) {
                CurrRecords = currRecords;
                $("#MainView").addClass("hide");
                $("#EditFormView").removeClass("hide");
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
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(false);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(true);
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(false);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(true);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(false);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(false);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(true);
    });

    //wire up dropdownId5 - Show Assy Components
    $("#dropdownId5").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/Component?AssemblyId="
            + TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data())
    });

    //wire up dropdownId6 - Show Assy Log
    $("#dropdownId6").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/AssemblyLogEntry?AssemblyId="
            + TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data())
    });

    //wire up dropdownId7 - Go to Extended
    $("#dropdownId7").click(function (event) {
        event.preventDefault();
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        var modelIds = TableMain.cells(".ui-selected", "AssemblyModel_Id:name", { page: "current" }).data().toArray();
        if (!modelIdsAreSame(modelIds)) {
            showModalFail("Error", "Selected records have no models or their models are not the same.");
            return;
        }
        var newWindowName = moment().format("YYYYDDMMHHmmss");
        window.open("about:blank", newWindowName);
        submitFormFromArray("POST", "/AssemblyExt", newWindowName, CurrIds, "AssemblyIds");
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/AssemblyTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByType
    $(MsFilterByType).on('selectionchange', function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest msFilterByProject
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

    //Initialize MagicSuggest MsFilterByLoc
    MsFilterByLoc = $("#MsFilterByLoc").magicSuggest({
        data: "/LocationSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: MsFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByLoc
    $(MsFilterByLoc).on('selectionchange', function (e, m) { refreshMainView(); });


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

    //TableMain AssemblyDbs
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" },//1
            //------------------------------------------------first set of columns
            { data: "AssyAltName", name: "AssyAltName" },//2
            { data: "AssyAltName2", name: "AssyAltName2" },//3
            { data: "AssemblyType_", render: function (data, type, full, meta) { return data.AssyTypeName }, name: "AssemblyType_" }, //4
            { data: "AssemblyStatus_", render: function (data, type, full, meta) { return data.AssyStatusName }, name: "AssemblyStatus_" }, //5
            { data: "AssemblyModel_", render: function (data, type, full, meta) { return data.AssyModelName }, name: "AssemblyModel_" }, //6
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject" }, //7
            //------------------------------------------------second set of columns
            { data: "AssignedToLocation_", render: function (data, type, full, meta) { return data.LocName + " - " + data.LocTypeName }, name: "AssignedToLocation" }, //8
            { data: "AssyGlobalX", name: "AssyGlobalX" },//9
            { data: "AssyGlobalY", name: "AssyGlobalY" },//10
            { data: "AssyGlobalZ", name: "AssyGlobalZ" },//11
            { data: "AssyLocalXDesign", name: "AssyLocalXDesign" },//12
            { data: "AssyLocalYDesign", name: "AssyLocalYDesign" },//13
            //------------------------------------------------third set of columns
            { data: "AssyLocalZDesign", name: "AssyLocalZDesign" },//14
            { data: "AssyLocalXAsBuilt", name: "AssyLocalXAsBuilt" },//15
            { data: "AssyLocalYAsBuilt", name: "AssyLocalYAsBuilt" },//16
            { data: "AssyLocalZAsBuilt", name: "AssyLocalZAsBuilt" },//17
            { data: "AssyStationing", name: "AssyStationing" },//18
            { data: "AssyLength", name: "AssyLength" },//19
            //------------------------------------------------Fourth set of columns
            { data: "AssyReadingIntervalSecs", name: "AssyReadingIntervalSecs" },//20
            { data: "IsReference_bl", name: "IsReference_bl" },//21
            { data: "TechnicalDetails", name: "TechnicalDetails" },//22
            { data: "PowerSupplyDetails", name: "PowerSupplyDetails" },//23
            { data: "HSEDetails", name: "HSEDetails" },//24
            { data: "Comments", name: "Comments" },//25
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//26
            { data: "AssemblyType_Id", name: "AssemblyType_Id" },//27
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//28
            { data: "AssemblyModel_Id", name: "AssemblyModel_Id" },//29
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//30
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" }//31
        ],
        columnDefs: [
            { targets: [0, 26, 27, 28, 29, 30, 31], visible: false }, // - never show
            { targets: [0, 21, 26, 27, 28, 29, 30, 31], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 3, 4], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6, 7], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [8, 9, 10, 11, 12, 13], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 10], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [11, 12, 13], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [14, 15, 16, 17, 18, 19], visible: false }, // - third set of columns - to toggle with options
            { targets: [18, 19], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [14, 15, 16], className: "hidden-xs hidden-sm hidden-md" }, // - third set of columns

            { targets: [20, 21, 22, 23, 24, 25], visible: false }, // - fourth set of columns - to toggle with options
            { targets: [21, 22, 23], className: "hidden-xs hidden-sm" }, // - fourth set of columns
            { targets: [24, 25], className: "hidden-xs hidden-sm hidden-md" } // - fourth set of columns
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
    msAddToMsArray(MagicSuggests, "AssemblyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssemblyModel_Id", "/AssemblyModelSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide");
        window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            showModalWait();
            var createMultiple = $("#CreateMultiple").val() != "" ? $("#CreateMultiple").val() : 1;
            submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/AssemblyDbSrv/Edit", createMultiple)
                .always(hideModalWait)
                .done(function () {
                    refreshMainView();
                    $("#MainView").removeClass("hide");
                    $("#EditFormView").addClass("hide");
                    window.scrollTo(0, 0);
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
        }
    });

    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);

    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/AssemblyDbSrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {
    TableMain.clear().search("").draw();

    if (MsFilterByType.getValue().length == 0 &&
        MsFilterByProject.getValue().length == 0 &&
        MsFilterByLoc.getValue().length == 0)
    {
        if (typeof AssemblyIds !== "undefined" && AssemblyIds != null && AssemblyIds.length > 0) {
            refreshTblGenWrp(TableMain, "/AssemblyDbSrv/GetByIds", { ids: AssemblyIds, getActive: GetActive }, "POST");
        }
        return;
    }

    refreshTblGenWrp(TableMain, "/AssemblyDbSrv/GetByAltIds2",
        {
            projectIds: MsFilterByProject.getValue(),
            typeIds: MsFilterByType.getValue(),
            locIds: MsFilterByLoc.getValue(),
            getActive: GetActive
        },
        "POST")
}

//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() {
    var deferred0 = $.Deferred();
    if (typeof LocationId !== "undefined" && LocationId != "") {
        showModalWait();
        $.ajax({
            type: "POST",
            url: "/LocationSrv/GetByIds",
            timeout: 120000,
            data: { ids: [LocationId], getActive: true },
            dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                msSetSelectionSilent(MsFilterByLoc, [{ id: data[0].Id, name: data[0].LocName }]);
                deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { deferred0.resolve(); }
    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//
