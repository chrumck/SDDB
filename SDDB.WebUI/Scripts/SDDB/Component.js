/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared_Views.js" />

//--------------------------------------Global Properties------------------------------------//

var recordTemplate = {
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

var extCurrRecords = [];
var extRecordTemplate = {
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

var msFilterByProject = {};
var msFilterByType = {};
var msFilterByAssy = {};

labelTextCreate = "Create Component";
labelTextEdit = "Edit Component";
urlFillForEdit = "/ComponentSrv/GetByIds";
urlEdit = "/ComponentSrv/Edit";
urlDelete = "/ComponentSrv/Delete";

var extEditFormId = "editFormExtended";
var extColumnSelectClass = ".extColumnSelect";
var extColumnSetNos = [3, 4, 5];
var extUrlTypeUpd = "/ComponentTypeSrv/GetByIds";
var extHttpTypeTypeUpd = "POST";
var extHttpTypeEdit = "POST";
var extUrlEdit = "/ComponentSrv/EditExt";

callBackAfterCreate = updateFormForSelectedType;

callBackAfterEdit = function (currRecords) {
    return updateFormForSelectedType()
        .then(function () {return fillFormForEditFromDbEntries(currentActive, currRecords, extEditFormId); });
};

callBackBeforeSubmitEdit = function () {
    if (!formIsValid(extEditFormId, currentIds.length === 0)) {
        showModalFail("Errors in Form", "Extended attributes have invalid inputs. Please correct.");
        return $.Deferred().reject();
    }
    return $.Deferred().resolve();
};

callBackAfterSubmitEdit = function (data) {
    extCurrRecords = [];
    for (var i = 0; i < currentIds.length; i++) {
        extCurrRecords[i] = $.extend(true, {}, extRecordTemplate);
        extCurrRecords[i].Id = currentIds[i];
    }
    return submitEditsGenericWrp(extEditFormId, [], extCurrRecords, extHttpTypeEdit, extUrlEdit);
};

callBackAfterCopy = function (currRecords) {
    return updateFormForSelectedType()
        .then(function () { return fillFormForCopyFromDbEntries(currRecords, extEditFormId); });
};
//-------------------------------------------------------------------------------------------//

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//

    //wire up dropdownId1 - Show Comp. Log
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = tableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/ComponentLogEntry?ComponentId=" +
            tableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
    });

    //Initialize MagicSuggest msFilterByType
    msFilterByType = $("#msFilterByType").magicSuggest({
        data: "/ComponentTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByType
    $(msFilterByType).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest msFilterByProject
    msFilterByProject = $("#msFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByProject
    $(msFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest msFilterByAssy
    msFilterByAssy = $("#msFilterByAssy").magicSuggest({
        data: "/AssemblyDbSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: msFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByAssy
    $(msFilterByAssy).on("selectionchange", function (e, m) { refreshMainView(); });


    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11, 12],
        [13, 14, 15, 16, 17],
        [18, 19, 20, 21, 22],
        [23, 24, 25, 26, 27]
    ];

    //tableMain Components
    tableMain = $("#tableMain").DataTable({
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
    showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    msAddToMsArray(magicSuggests, "ComponentType_Id", "/ComponentTypeSrv/Lookup", 1);
    msAddToMsArray(magicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    msAddToMsArray(magicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(magicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

    //Initialize MagicSuggest Array Event - ComponentType_Id
    $(magicSuggests[0]).on("selectionchange", function (e, m) {
        updateFormForSelectedType().done(function () { $("#ComponentType_Id input").focus(); });
    });

    //Enable DateTimePicker
    $("#" + editFormId + " [data-val-dbisdateiso]").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });


    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);
    switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    tableMain.clear().search("").draw();
    return modalWaitWrapper(function () {
        return updateMainViewForSelectedType()
            .then(function () {
                if (msFilterByType.getValue().length !== 0 || msFilterByProject.getValue().length !== 0 ||
                    msFilterByAssy.getValue().length !== 0) {
                    return refreshTableGeneric(tableMain, "/ComponentSrv/GetByAltIds2",
                        {
                            projectIds: msFilterByProject.getValue(),
                            typeIds: msFilterByType.getValue(),
                            assyIds: msFilterByAssy.getValue(),
                            getActive: currentActive
                        },
                        "POST");
                }
                if (ComponentIds && ComponentIds.length > 0) {
                    return refreshTableGeneric(tableMain, "/ComponentSrv/GetByIds",
                        { ids: ComponentIds, getActive: currentActive }, "POST");
                }
            });
    });
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
                msSetSelectionSilent(msFilterByAssy, [{ id: data[0].Id, name: data[0].AssyName }]);
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

    if (msFilterByType.getValue().length != 1) {
        switchMainViewForExtendedHelper(false);
        return deferred0.resolve();
    }
    updateTableForExtended(extHttpTypeTypeUpd, extUrlTypeUpd, { ids: msFilterByType.getValue()[0] }, tableMain)
        .done(function (typeHasAttrs) {
            switchMainViewForExtendedHelper(typeHasAttrs);
            return deferred0.resolve();
        });
    return deferred0.promise();

    //switchMainViewForExtendedHelper
    function switchMainViewForExtendedHelper(switchOn) {
        if (switchOn) {
            $(extColumnSelectClass).removeClass("disabled");
            return;
        }
        if ($.inArray(selectedColumnSet, extColumnSetNos) != -1) { showColumnSet(1, tableMainColumnSets); }
        $(extColumnSelectClass).addClass("disabled");
    }
}

//updateFormForSelectedType
function updateFormForSelectedType() {
    clearFormInputs(extEditFormId);
    $("#" + extEditFormId + " .modifiable").data("ismodified", true);
    $("#" + extEditFormId).addClass("hidden");
    if (magicSuggests[0].getValue().length == 1 && magicSuggests[0].getValue()[0] != "_VARIES_") {
        var deferred0 = $.Deferred();
        updateFormForExtendedWrp(extHttpTypeTypeUpd, extUrlTypeUpd, { ids: magicSuggests[0].getValue()[0] }, extEditFormId)
            .done(function (typeHasAttrs) {
                if (typeHasAttrs) { $("#" + extEditFormId).removeClass("hidden"); }
                return deferred0.resolve();
            })
            .fail(function () { return deferred0.reject(); });
        return deferred0.promise();
    }
    return $.Deferred().resolve();
}

//---------------------------------------Helper Methods--------------------------------------//


