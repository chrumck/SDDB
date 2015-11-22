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
    AssignedToLocation_Id: null
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
var MsFilterByLoc = {};

var DatePickers = [];

LabelTextCreate = "Create Assembly";
LabelTextEdit = "Edit Assembly";
UrlFillForEdit = "/AssemblyDbSrv/GetByIds";
UrlEdit = "/AssemblyDbSrv/Edit";
UrlDelete = "/AssemblyDbSrv/Delete";

var ExtEditFormId = "EditFormExtended";
var ExtColumnSelectClass = ".extColumnSelect";
var ExtColumnSetNos = [5, 6, 7];
var ExtTypeUrl = "/AssemblyTypeSrv/GetByIds";
var ExtTypeHttpType = "POST";
var ExtHttpTypeEdit = "POST";
var ExtUrlEdit = "/AssemblyDbSrv/EditExt";

callBackBeforeCreate = updateFormForSelectedType;

callBackBeforeEdit = function (currRecords) {
    return updateFormForSelectedType()
        .then(function () { return fillFormForEditFromDbEntries(GetActive, currRecords, ExtEditFormId); });
};

callBackBeforeSubmitEdit = function (data) {
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
    
    //wire up dropdownId1 - Show Assy Components
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/Component?AssemblyId=" +
            TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
    });

    //wire up dropdownId2 - Show Assy Log
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/AssemblyLogEntry?AssemblyId=" +
            TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
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
    $(MsFilterByLoc).on("selectionchange", function (e, m) { refreshMainView(); });


    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11, 12],
        [13, 14, 15, 16, 17, 18],
        [19, 20, 21, 22, 23],
        [24, 25, 26, 27, 28],
        [29, 30, 31, 32, 33],
        [34, 35, 36, 37, 38]
    ];
        
    //TableMain AssemblyDbs
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" },//1
            //------------------------------------------------1st set of columns
            { data: "AssyAltName", name: "AssyAltName" },//2
            { data: "AssyAltName2", name: "AssyAltName2" },//3
            {
                data: "AssemblyType_",
                name: "AssemblyType_",
                render: function (data, type, full, meta) { return data.AssyTypeName; }
            }, //4
            {
                data: "AssemblyStatus_",
                name: "AssemblyStatus_",
                render: function (data, type, full, meta) { return data.AssyStatusName; }
            }, //5
            {
                data: "AssignedToLocation_",
                name: "AssignedToLocation",
                render: function (data, type, full, meta) { return data.LocName + " - " + data.ProjectName; }
            }, //6
            //------------------------------------------------2nd set of columns
            { data: "AssyGlobalX", name: "AssyGlobalX" },//7
            { data: "AssyGlobalY", name: "AssyGlobalY" },//8
            { data: "AssyGlobalZ", name: "AssyGlobalZ" },//9
            { data: "AssyLocalXDesign", name: "AssyLocalXDesign" },//10
            { data: "AssyLocalYDesign", name: "AssyLocalYDesign" },//11
            { data: "AssyLocalZDesign", name: "AssyLocalZDesign" },//12
            //------------------------------------------------3rd set of columns
            { data: "AssyLocalXAsBuilt", name: "AssyLocalXAsBuilt" },//13
            { data: "AssyLocalYAsBuilt", name: "AssyLocalYAsBuilt" },//14
            { data: "AssyLocalZAsBuilt", name: "AssyLocalZAsBuilt" },//15
            { data: "AssyStationing", name: "AssyStationing" },//16
            { data: "AssyLength", name: "AssyLength" },//17
            { data: "AssyReadingIntervalSecs", name: "AssyReadingIntervalSecs" },//18
            //------------------------------------------------4th set of columns
            { data: "IsReference_bl", name: "IsReference_bl" },//19
            { data: "TechnicalDetails", name: "TechnicalDetails" },//20
            { data: "PowerSupplyDetails", name: "PowerSupplyDetails" },//21
            { data: "HSEDetails", name: "HSEDetails" },//22
            { data: "Comments", name: "Comments" },//23
            //------------------------------------------------5th set of columns
            { data: "Attr01", name: "Attr01" },//24
            { data: "Attr02", name: "Attr02" },//25
            { data: "Attr03", name: "Attr03" },//26
            { data: "Attr04", name: "Attr04" },//27
            { data: "Attr05", name: "Attr05" },//28
            //------------------------------------------------6th set of columns
            { data: "Attr06", name: "Attr06" },//29
            { data: "Attr07", name: "Attr07" },//30
            { data: "Attr08", name: "Attr08" },//31
            { data: "Attr09", name: "Attr09" },//32
            { data: "Attr10", name: "Attr10" },//33
            //------------------------------------------------7th set of columns
            { data: "Attr11", name: "Attr11" },//34
            { data: "Attr12", name: "Attr12" },//35
            { data: "Attr13", name: "Attr13" },//36
            { data: "Attr14", name: "Attr14" },//37
            { data: "Attr15", name: "Attr15" },//38
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//39
            { data: "AssemblyType_Id", name: "AssemblyType_Id" },//40
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//41
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" }//42
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 19, 39, 40, 41, 42], searchable: false },  
            //1st set of columns - responsive
            { targets: [4, 6], className: "hidden-xs" },
            { targets: [3], className: "hidden-xs hidden-sm" }, 
            //2nd set of columns - responsive
            { targets: [8, 9], className: "hidden-xs" }, 
            { targets: [10, 11, 12], className: "hidden-xs hidden-sm" }, 
            //3rd set of columns - responsive
            { targets: [13, 14, 15, 17], className: "hidden-xs" }, 
            { targets: [18], className: "hidden-xs hidden-sm " }, 
            //4th set of columns - responsive
            { targets: [19, 20, 21], className: "hidden-xs" },
            { targets: [22], className: "hidden-xs hidden-sm" },
            //5th set of columns - responsive
            { targets: [25, 26], className: "hidden-xs" },
            { targets: [27, 28], className: "hidden-xs hidden-sm" },
            //6th set of columns - responsive
            { targets: [30, 31], className: "hidden-xs" },
            { targets: [32, 33], className: "hidden-xs hidden-sm" },
            //7th set of columns - responsive
            { targets: [35, 36], className: "hidden-xs" },
            { targets: [37, 38], className: "hidden-xs hidden-sm" }
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
    msAddToMsArray(MagicSuggests, "AssemblyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/Lookup", 1);

    //Initialize MagicSuggest Array Event - AssemblyType_Id
    $(MagicSuggests[0]).on("selectionchange", function (e, m) {
        updateFormForSelectedType().done(function () { $("#AssemblyType_Id input").focus(); });
    });

        
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
                    MsFilterByLoc.getValue().length !== 0) {
                refreshTblGenWrp(TableMain, "/AssemblyDbSrv/GetByAltIds2",
                    {
                        projectIds: MsFilterByProject.getValue(),
                        typeIds: MsFilterByType.getValue(),
                        locIds: MsFilterByLoc.getValue(),
                        getActive: GetActive
                    },
                    "POST")
                    .done(deferred0.resolve);
                return;
            }
            if (typeof AssemblyIds !== "undefined" && AssemblyIds !== null && AssemblyIds.length > 0) {
                refreshTblGenWrp(TableMain, "/AssemblyDbSrv/GetByIds",
                    {
                        ids: AssemblyIds,
                        getActive: GetActive
                    }, "POST")
                    .done(deferred0.resolve);
            }
        });
    return deferred0.promise();
}

//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() {
    var deferred0 = $.Deferred();
    if (typeof LocationId !== "undefined" && LocationId !== "") {
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


