﻿/*global sddb, AssemblyIds, LocationId */
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
    },

    tableMainColumnSets : [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11, 12],
        [13, 14, 15, 16, 17, 18],
        [19, 20, 21, 22, 23],
        [24, 25, 26, 27, 28],
        [29, 30, 31, 32, 33],
        [34, 35, 36, 37, 38]
    ],
        
    tableMain : $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" },//1
            //------------------------------------------------1st set of columns
            { data: "AssyAltName", name: "AssyAltName" },//2
            { data: "AssyAltName2", name: "AssyAltName2" },//3
            {
                data: "AssemblyType_",
                name: "AssemblyType_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.AssyTypeName;
                }
            }, //4
            {
                data: "AssemblyStatus_",
                name: "AssemblyStatus_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.AssyStatusName;
                }
            }, //5
            {
                data: "AssignedToLocation_",
                name: "AssignedToLocation",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.LocName + " - " + data.ProjectName;
                }
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
    }),

    labelTextCreate: "Create Assembly",
    labelTextEdit: "Edit Assembly",
    urlFillForEdit: "/AssemblyDbSrv/GetByIds",
    urlEdit: "/AssemblyDbSrv/Edit",
    urlDelete: "/AssemblyDbSrv/Delete",
    
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
    extColumnSetNos: [5, 6, 7],
    extUrlTypeUpd: "/AssemblyTypeSrv/GetByIds",
    extUrlEdit: "/AssemblyDbSrv/EditExt"

});

//fillFiltersFromRequestParams
sddb.fillFiltersFromRequestParams = function () {
    "use strict";
    if (!LocationId) { return $.Deferred().resolve(); }

    return sddb.modalWaitWrapper(function () {
        return $.ajax({
            type: "POST",
            url: "/LocationSrv/GetByIds",
            timeout: 120000,
            data: { ids: [LocationId], getActive: true },
            dataType: "json"
        })
            .then(function (data) {
                if (!data || data.length === 0) { return; }
                sddb.msSetSelectionSilent(sddb.msFilterByLoc, [{ id: data[0].Id, name: data[0].LocName }]);
            });
    });
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
sddb.callBackAfterEdit =  function (dbEntries) {
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
                    sddb.msFilterByLoc.getValue().length !== 0) {
                    return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/AssemblyDbSrv/GetByAltIds2",
                        {
                            projectIds: sddb.msFilterByProject.getValue(),
                            typeIds: sddb.msFilterByType.getValue(),
                            locIds: sddb.msFilterByLoc.getValue(),
                            getActive: sddb.cfg.currentActive
                        },
                        "POST");
                }
                if (AssemblyIds && AssemblyIds.length > 0) {
                    return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/AssemblyDbSrv/GetByIds",
                        { ids: AssemblyIds, getActive: sddb.cfg.currentActive }, "POST");
                }
            });
    });
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//
    
    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        sddb.sendSingleIdToNewWindow("/Component?AssemblyId=");
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        sddb.sendSingleIdToNewWindow("/AssemblyLogEntry?AssemblyId=");
    });

    //Initialize MagicSuggest sddb.msFilterByType
    sddb.msFilterByType = sddb.msSetFilter("msFilterByType", "/AssemblyTypeSrv/Lookup");
    
    //Initialize MagicSuggest sddb.msFilterByProject
    sddb.msFilterByProject = sddb.msSetFilter("msFilterByProject", "/ProjectSrv/Lookup");
    
    //Initialize MagicSuggest sddb.msFilterByLoc
    sddb.msFilterByLoc = sddb.msSetFilter("msFilterByLoc", "/LocationSrv/LookupByProj",
        { dataUrlParams: { projectIds: sddb.msFilterByProject.getValue } });
    

    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("AssemblyType_Id", "/AssemblyTypeSrv/Lookup", {}, function () {
        sddb.updateFormForSelectedType().done(function () { $("#AssemblyType_Id input").focus(); });
    });
    sddb.msAddToArray("AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup");
    sddb.msAddToArray("AssignedToLocation_Id", "/LocationSrv/Lookup");
        
    //--------------------------------------View Initialization------------------------------------//

    sddb.fillFiltersFromRequestParams().done(sddb.refreshMainView);
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});


