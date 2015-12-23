/*global sddb, AssemblyId, ComponentIds */
/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared_Views.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//

//setting up sddb
sddb.setConfig({
    recordTemplate: {
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
    },

    tableMainColumnSets: [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11, 12],
        [13, 14, 15, 16, 17],
        [18, 19, 20, 21, 22],
        [23, 24, 25, 26, 27]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompName", name: "CompName" },//1
            //------------------------------------------------1st set of columns
            { data: "CompAltName", name: "CompAltName" },//2
            { data: "CompAltName2", name: "CompAltName2" },//3
            {
                data: "ComponentType_",
                name: "ComponentType_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.CompTypeName;
                }
            }, //4
            {
                data: "ComponentStatus_",
                name: "ComponentStatus_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.CompStatusName;
                }
            }, //5
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.ProjectName + " " + data.ProjectCode;
                }
            }, //6
            //------------------------------------------------2nd set of columns
            {
                data: "AssignedToAssemblyDb_",
                name: "AssignedToAssemblyDb_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.AssyName;
                }
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
    }),

    labelTextCreate: "Create Component",
    labelTextEdit: "Edit Component",
    urlFillForEdit: "/ComponentSrv/GetByIds",
    urlEdit: "/ComponentSrv/Edit",
    urlDelete: "/ComponentSrv/Delete",

    extRecordTemplate: {
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
    },
    extColumnSetNos: [3, 4, 5],
    extUrlTypeUpd: "/ComponentTypeSrv/GetByIds",
    extUrlEdit: "/ComponentSrv/EditExt"

});

//fillFiltersFromRequestParams
sddb.fillFiltersFromRequestParams = function () {
    "use strict";
    var deferred0 = $.Deferred();
    if (typeof AssemblyId !== "undefined" && AssemblyId !== "") {
        sddb.showModalWait();
        $.ajax({
            type: "POST",
            url: "/AssemblyDbSrv/GetByIds",
            timeout: 120000,
            data: { ids: [AssemblyId], getActive: true },
            dataType: "json"
        })
            .always(sddb.hideModalWait)
            .done(function (data) {
                sddb.msSetSelectionSilent(sddb.msFilterByAssy, [{ id: data[0].Id, name: data[0].AssyName }]);
                return deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                sddb.showModalFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { return deferred0.resolve(); }
    return deferred0.promise();
};

//updateMainViewForSelectedType
sddb.updateMainViewForSelectedType = function () {
    "use strict";
    //switchMainViewForExtendedHelper
    var switchMainViewForExtendedHelper = function (switchOn) {
        if (switchOn) {
            $(sddb.cfg.extColumnSelectClass).removeClass("disabled");
            return;
        }
        if ($.inArray(sddb.cfg.selectedColumnSet, sddb.cfg.extColumnSetNos) != -1) { sddb.showColumnSet(); }
        $(sddb.cfg.extColumnSelectClass).addClass("disabled");
    },
    //
    deferred0 = $.Deferred();

    //main
    if (sddb.msFilterByType.getValue().length != 1) {
        switchMainViewForExtendedHelper(false);
        return deferred0.resolve();
    }
    sddb.updateTableForExtended({ ids: sddb.msFilterByType.getValue()[0] })
        .done(function (typeHasAttrs) {
            switchMainViewForExtendedHelper(typeHasAttrs);
            return deferred0.resolve();
        });
    return deferred0.promise();
};

//updateFormForSelectedType
sddb.updateFormForSelectedType = function () {
    "use strict";
    sddb.clearFormInputs(sddb.cfg.extEditFormId);
    $("#" + sddb.cfg.extEditFormId + " .modifiable").data("ismodified", true);
    $("#" + sddb.cfg.extEditFormId).addClass("hidden");
    if (sddb.cfg.magicSuggests[0].getValue().length == 1 && sddb.cfg.magicSuggests[0].getValue()[0] != "_VARIES_") {
        return sddb.modalWaitWrapper(function () {
            return sddb.updateFormForExtended({ ids: sddb.cfg.magicSuggests[0].getValue()[0] })
                .then(function (typeHasAttrs) {
                    if (typeHasAttrs) { $("#" + sddb.cfg.extEditFormId).removeClass("hidden"); }
                });
        });
    }
    return $.Deferred().resolve();
};

//callBackAfterCreate
sddb.callBackAfterCreate = sddb.updateFormForSelectedType;

//callBackAfterEdit
sddb.callBackAfterEdit = function (dbEntries) {
    "use strict";
    return sddb.updateFormForSelectedType()
        .then(function () {
            return sddb.fillFormForEditFromDbEntries(sddb.cfg.currentActive, dbEntries, sddb.cfg.extEditFormId);
        });
};

//callBackBeforeSubmitEdit
sddb.callBackBeforeSubmitEdit = function () {
    "use strict";
    if (!sddb.formIsValid(sddb.cfg.extEditFormId, sddb.cfg.currentIds.length === 0)) {
        sddb.showModalFail("Errors in Form", "Extended attributes have invalid inputs. Please correct.");
        return $.Deferred().reject();
    }
    return $.Deferred().resolve();
};

//callBackAfterSubmitEdit
sddb.callBackAfterSubmitEdit = function (data) {
    "use strict";
    sddb.cfg.extCurrRecords = [];
    for (var i = 0; i < sddb.cfg.currentIds.length; i += 1) {
        sddb.cfg.extCurrRecords[i] = $.extend(true, {}, sddb.cfg.extRecordTemplate);
        sddb.cfg.extCurrRecords[i].Id = sddb.cfg.currentIds[i];
    }
    return sddb.modalWaitWrapper(function () {
        return sddb.submitEditsGeneric(sddb.cfg.extEditFormId, [],
            sddb.cfg.extCurrRecords, sddb.cfg.extHttpTypeEdit, sddb.cfg.extUrlEdit);
    });
};

//callBackAfterCopy
sddb.callBackAfterCopy = function (dbEntries) {
    "use strict";
    return sddb.updateFormForSelectedType()
        .then(function () { return sddb.fillFormForCopyFromDbEntries(dbEntries, sddb.cfg.extEditFormId); });
};

//refresh view after magicsuggest update
sddb.refreshMainView = function () {
    "use strict";
    sddb.cfg.tableMain.clear().search("").draw();
    return sddb.modalWaitWrapper(function () {
        return sddb.updateMainViewForSelectedType()
            .then(function () {
                if (sddb.msFilterByType.getValue().length !== 0 || sddb.msFilterByProject.getValue().length !== 0 ||
                    sddb.msFilterByAssy.getValue().length !== 0) {
                    return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/ComponentSrv/GetByAltIds2",
                        {
                            projectIds: sddb.msFilterByProject.getValue(),
                            typeIds: sddb.msFilterByType.getValue(),
                            assyIds: sddb.msFilterByAssy.getValue(),
                            getActive: sddb.cfg.currentActive
                        },
                        "POST");
                }
                if (ComponentIds && ComponentIds.length > 0) {
                    return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/ComponentSrv/GetByIds",
                        { ids: ComponentIds, getActive: sddb.cfg.currentActive }, "POST");
                }
            });
    });
};


//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//

    //wire up dropdownId1 - Show Assy Components
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        sddb.sendSingleIdToNewWindow("/ComponentLogEntry?ComponentId=");
    });

    //Initialize MagicSuggest sddb.msFilterByType
    sddb.msFilterByType = sddb.msSetFilter("msFilterByType", "/ComponentTypeSrv/Lookup");

    //Initialize MagicSuggest sddb.msFilterByProject
    sddb.msFilterByProject = sddb.msSetFilter("msFilterByProject", "/ProjectSrv/Lookup");

    //Initialize MagicSuggest sddb.msFilterByLoc
    sddb.msFilterByAssy = sddb.msSetFilter("msFilterByAssy", "/AssemblyDbSrv/LookupByProj",
        { dataUrlParams: { projectIds: sddb.msFilterByProject.getValue } });


    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("ComponentType_Id", "/ComponentTypeSrv/Lookup", {}, function () {
        sddb.updateFormForSelectedType().done(function () { $("#ComponentType_Id input").focus(); });
    });
    sddb.msAddToArray("ComponentStatus_Id", "/ComponentStatusSrv/Lookup");
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup");
    sddb.msAddToArray("AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup");

    //--------------------------------------View Initialization------------------------------------//

    sddb.fillFiltersFromRequestParams().done(sddb.refreshMainView);
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});

