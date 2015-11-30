/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared_Views.js" />

//--------------------------------------Global Properties------------------------------------//

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
    AssignedToProject_Id: null,
    AssignedToAssemblyDb_Id: null
};

var ExtCurrRecords = [];
var ExtRecordTemplate = {
    Id: "RecordTemplateId",
    Attr01: null,
    Attr02: null,
    Attr03: null,
    Attr04: null,
    Attr05: null,
    Attr06: null,
    Attr07: null,
    Attr08: null,
    Attr09: null,
    Attr10: null,
    Attr11: null,
    Attr12: null,
    Attr13: null,
    Attr14: null,
    Attr15: null
};

var MsFilterByProject = {};
var MsFilterByType = {};
var MsFilterByAssy = {};

var DatePickers = [];

LabelTextCreate = "Create Component";
LabelTextEdit = "Edit Component";
UrlFillForEdit = "/ComponentSrv/GetByIds";
UrlEdit = "/ComponentSrv/Edit";
UrlDelete = "/ComponentSrv/Delete";

var ExtEditFormId = "EditFormExtended";
var ExtColumnSelectClass = ".extColumnSelect";
var ExtColumnSetNos = [3, 4, 5];
var ExtTypeUrl = "/ComponentTypeSrv/GetByIds";
var ExtTypeHttpType = "POST";
var ExtHttpTypeEdit = "POST";
var ExtUrlEdit = "/ComponentSrv/EditExt";

callBackBeforeCreate = updateFormForSelectedType;

callBackBeforeEdit = function (currRecords) {
    return updateFormForSelectedType()
        .then(function () {return fillFormForEditFromDbEntries(GetActive, currRecords, ExtEditFormId); });
};

callBackBeforeSubmitEdit = function () {
    if (!formIsValid(ExtEditFormId, CurrIds.length === 0)) {
        showModalFail("Errors in Form", "Extended attributes have invalid inputs. Please correct.");
        return $.Deferred().reject();
    }
    return $.Deferred().resolve();
};

callBackAfterSubmitEdit = function (data) {
    ExtCurrRecords = [];
    for (var i = 0; i < CurrIds.length; i++) {
        ExtCurrRecords[i] = $.extend(true, {}, ExtRecordTemplate);
        ExtCurrRecords[i].Id = CurrIds[i];
    }
    return submitEditsGenericWrp(ExtEditFormId, [], ExtCurrRecords, ExtHttpTypeEdit, ExtUrlEdit);
};

callBackBeforeCopy = function (currRecords) {
    return updateFormForSelectedType()
        .then(function () { return fillFormForCopyFromDbEntries(currRecords, ExtEditFormId); });
};
//-------------------------------------------------------------------------------------------//

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //wire up dropdownId1 - Show Comp. Log
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/ComponentLogEntry?ComponentId=" +
            TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
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
    $(MsFilterByType).on("selectionchange", function (e, m) { refreshMainView(); });

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
    $(MsFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

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
    $(MsFilterByAssy).on("selectionchange", function (e, m) { refreshMainView(); });


    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11, 12],
        [13, 14, 15, 16, 17],
        [18, 19, 20, 21, 22],
        [23, 24, 25, 26, 27]
    ];

    //TableMain Components
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompName", name: "CompName" },//1
            //------------------------------------------------1st set of columns
            { data: "CompAltName", name: "CompAltName" },//2
            { data: "CompAltName2", name: "CompAltName2" },//3
            {
                data: "ComponentType_",
                name: "ComponentType_",
                render: function (data, type, full, meta) { return data.CompTypeName; }
            }, //4
            {
                data: "ComponentStatus_",
                name: "ComponentStatus_",
                render: function (data, type, full, meta) { return data.CompStatusName; }
            }, //5
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode; }
            }, //6
            //------------------------------------------------2nd set of columns
            {
                data: "AssignedToAssemblyDb_",
                name: "AssignedToAssemblyDb_",
                render: function (data, type, full, meta) { return data.AssyName; }
            }, //7
            { data: "PositionInAssy", name: "PositionInAssy" },//8
            { data: "ProgramAddress", name: "ProgramAddress" },//9
            { data: "CalibrationReqd_bl", name: "CalibrationReqd_bl" },//10
            { data: "LastCalibrationDate", name: "LastCalibrationDate" },//11
            { data: "Comments", name: "Comments" },//12
            //------------------------------------------------3rd set of columns
            { data: "Attr01", name: "Attr01" },//13
            { data: "Attr02", name: "Attr02" },//14
            { data: "Attr03", name: "Attr03" },//15
            { data: "Attr04", name: "Attr04" },//16
            { data: "Attr05", name: "Attr05" },//17
            //------------------------------------------------4th set of columns
            { data: "Attr06", name: "Attr06" },//18
            { data: "Attr07", name: "Attr07" },//19
            { data: "Attr08", name: "Attr08" },//20
            { data: "Attr09", name: "Attr09" },//21
            { data: "Attr10", name: "Attr10" },//22
            //------------------------------------------------5th set of columns
            { data: "Attr11", name: "Attr11" },//23
            { data: "Attr12", name: "Attr12" },//24
            { data: "Attr13", name: "Attr13" },//25
            { data: "Attr14", name: "Attr14" },//26
            { data: "Attr15", name: "Attr15" },//27
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//28
            { data: "ComponentType_Id", name: "ComponentType_Id" },//29
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//30
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//31
            { data: "AssignedToAssemblyDb_Id", name: "AssignedToAssemblyDb_Id" }//32
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 10, 11, 28, 29, 30, 31, 32], searchable: false },
            //1st set of columns - responsive
            { targets: [4, 6], className: "hidden-xs hidden-sm" },
            { targets: [2, 3], className: "hidden-xs hidden-sm hidden-md" },
            //2nd set of columns - responsive
            { targets: [8, 10, 11], className: "hidden-xs hidden-sm" },
            { targets: [9, 12], className: "hidden-xs hidden-sm hidden-md" },
            //3rd set of columns - responsive
            { targets: [14, 15], className: "hidden-xs" },
            { targets: [16, 17], className: "hidden-xs hidden-sm" },
            //4th set of columns - responsive
            { targets: [19, 20], className: "hidden-xs" },
            { targets: [21, 22], className: "hidden-xs hidden-sm" },
            //5th set of columns - responsive
            { targets: [24, 25], className: "hidden-xs" },
            { targets: [26, 27], className: "hidden-xs hidden-sm" }
        ],
        order: [[1, "asc"]],
        bAutoWidth: false,
        lengthMenu: [10, 25, 50, 75, 100, 500, 1000, 5000],
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        }
    });
    //showing the first Set of columns on startup;
    showColumnSet(TableMainColumnSets, 1);

    //---------------------------------------EditFormView----------------------------------------//

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "ComponentType_Id", "/ComponentTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

    //Initialize MagicSuggest Array Event - ComponentType_Id
    $(MagicSuggests[0]).on("selectionchange", function (e, m) {
        updateFormForSelectedType().done(function () { $("#ComponentType_Id input").focus(); });
    });

    //Enable DateTimePicker
    $("#" + EditFormId + " [data-val-dbisdateiso]").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });


    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    var deferred0 = $.Deferred();

    TableMain.clear().search("").draw();
    updateMainViewForSelectedType()
        .done(function () {
            if (MsFilterByType.getValue().length !== 0 || MsFilterByProject.getValue().length !== 0 ||
                    MsFilterByAssy.getValue().length !== 0) {
                refreshTblGenWrp(TableMain, "/ComponentSrv/GetByAltIds2",
                    {
                        projectIds: MsFilterByProject.getValue(),
                        typeIds: MsFilterByType.getValue(),
                        assyIds: MsFilterByAssy.getValue(),
                        getActive: GetActive
                    },
                    "POST")
                    .done(deferred0.resolve);
                return;
            }
            if (typeof ComponentIds !== "undefined" && ComponentIds !== null && ComponentIds.length > 0) {
                refreshTblGenWrp(TableMain, "/ComponentSrv/GetByIds",
                    {
                        ids: ComponentIds,
                        getActive: GetActive
                    },
                    "POST")
                    .done(deferred0.resolve);
            }
        });
    return deferred0.promise();
}


//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() {
    var deferred0 = $.Deferred();
    if (typeof AssemblyId !== "undefined" && AssemblyId !== "") {
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
                msSetSelectionSilent(MsFilterByAssy, [{ id: data[0].Id, name: data[0].AssyName }]);
                return deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { return deferred0.resolve(); }
    return deferred0.promise();
}


//updateMainViewForSelectedType
function updateMainViewForSelectedType() {
    var deferred0 = $.Deferred();

    if (MsFilterByType.getValue().length != 1) {
        switchMainViewForExtendedHelper(false);
        return deferred0.resolve();
    }
    updateTableForExtended(ExtTypeHttpType, ExtTypeUrl, { ids: MsFilterByType.getValue()[0] }, TableMain)
        .done(function (typeHasAttrs) {
            switchMainViewForExtendedHelper(typeHasAttrs);
            return deferred0.resolve();
        });
    return deferred0.promise();

    //switchMainViewForExtendedHelper
    function switchMainViewForExtendedHelper(switchOn) {
        if (switchOn) {
            $(ExtColumnSelectClass).removeClass("disabled");
            return;
        }
        if ($.inArray(SelectedColumnSet, ExtColumnSetNos) != -1) { showColumnSet(TableMainColumnSets, 1); }
        $(ExtColumnSelectClass).addClass("disabled");
    }
}

//updateFormForSelectedType
function updateFormForSelectedType() {
    clearFormInputs(ExtEditFormId);
    $("#" + ExtEditFormId + " .modifiable").data("ismodified", true);
    $("#" + ExtEditFormId).addClass("hidden");
    if (MagicSuggests[0].getValue().length == 1 && MagicSuggests[0].getValue()[0] != "_VARIES_") {
        var deferred0 = $.Deferred();
        updateFormForExtendedWrp(ExtTypeHttpType, ExtTypeUrl, { ids: MagicSuggests[0].getValue()[0] }, ExtEditFormId)
            .done(function (typeHasAttrs) {
                if (typeHasAttrs) { $("#" + ExtEditFormId).removeClass("hidden"); }
                return deferred0.resolve();
            })
            .fail(function () { return deferred0.reject(); });
        return deferred0.promise();
    }
    return $.Deferred().resolve();
}

//---------------------------------------Helper Methods--------------------------------------//


