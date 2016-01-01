/*global sddb, PersonId, UserId */
/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared_Views.js" />
/// <reference path="PersonLogEntryShared.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//

//setting up sddb
sddb.setConfig({

    tableMainColumnSets : [
        [1],
        [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            {
                data: "EnteredByPerson_",
                name: "EnteredByPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.Initials;
                }
            }, //2
            { data: "PrsLogEntryPersonsInitials", name: "PrsLogEntryPersonsInitials" },//3
            {
                data: "PersonActivityType_",
                name: "PersonActivityType_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.ActivityTypeName;
                }
            }, //4
            { data: "ManHours", name: "ManHours" },//5
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.ProjectName + " " + data.ProjectCode;
                }
            }, //6
            {
                data: "AssignedToLocation_",
                name: "AssignedToLocation_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.LocName;
                }
            }, //7
            {
                data: "AssignedToProjectEvent_",
                name: "AssignedToProjectEvent_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.EventName;
                }
            }, //8
            { data: "PrsLogEntryFilesCount", name: "PrsLogEntryFilesCount" },//9
            { data: "PrsLogEntryAssysCount", name: "PrsLogEntryAssysCount" },//10
            {
                data: "QcdByPerson_",
                name: "QcdByPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.Initials;
                }
            }, //11
            { data: "Comments", name: "Comments" },//12
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//13
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//14
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//15
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//16
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" },//17
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" }//18
        ],
        columnDefs: [
            // - never show
            { targets: [0, 13, 14, 15, 16, 17, 18], visible: false },
            //"orderable": false, "visible": false
            { targets: [0, 1, 5, 9, 10, 13, 14, 15, 16, 17, 18], searchable: false },
             // - first set of columns
            { targets: [4, 5], className: "hidden-xs" },
            { targets: [3, 12], className: "hidden-xs hidden-sm" },
            { targets: [7, 8, 9, 10, 11], className: "hidden-xs hidden-sm hidden-md" }
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

    labelTextCreate: "Create Activity",
    labelTextEdit : "Edit Activity",
    labelTextCopy : "Copy Activity"
});

//callBackAfterCreate
sddb.callBackAfterCreate = function () {
    "use strict";
    $("#editFormBtnOkFiles").prop("disabled", false);
    $("#LogEntryDateTime").val(moment().format("YYYY-MM-DD HH:mm"));
    $("#ManHours").val(0);
    sddb.cfg.magicSuggests[0].setValue([UserId]);
    sddb.cfg.magicSuggests[3].disable();
    sddb.cfg.magicSuggests[4].disable();
    sddb.tableLogEntryAssysAdd.clear().search("").draw();
    sddb.tableLogEntryAssysRemove.clear().search("").draw();
    sddb.tableLogEntryPersonsAdd.clear().search("").draw();
    sddb.tableLogEntryPersonsRemove.clear().search("").draw();

    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(sddb.tableLogEntryPersonsAdd, "/PersonSrv/Get", { getActive: true }, "GET");
    });
};
//callBackAfterEdit
sddb.callBackAfterEdit = function (currRecords) {
    "use strict";
    if (sddb.cfg.currentIds.length > 1) { $("#editFormBtnOkFiles").prop("disabled", true); }
    else { $("#editFormBtnOkFiles").prop("disabled", false); }

    return sddb.modalWaitWrapper(function () {
        return $.when(
            sddb.fillFormForRelatedGeneric(sddb.tableLogEntryPersonsAdd, sddb.tableLogEntryPersonsRemove,
                sddb.cfg.currentIds,
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryPersons",
                { ids: sddb.cfg.currentIds },
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot",
                { ids: sddb.cfg.currentIds }),
            sddb.fillFormForRelatedGeneric(sddb.tableLogEntryAssysAdd, sddb.tableLogEntryAssysRemove,
                sddb.cfg.currentIds,
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                { ids: sddb.cfg.currentIds },
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                { ids: sddb.cfg.currentIds, locId: sddb.cfg.magicSuggests[3].getValue()[0] })
        );
    });
};
//callBackBeforeCopy
sddb.callBackBeforeCopy = function () {
    "use strict";
    return sddb.showModalConfirm("NOTE: Assemblies, People and Files are not copied! Continue?", "Confirm Copy");
};
//callBackAfterCopy
sddb.callBackAfterCopy = function () {
    "use strict";
    $("#editFormBtnOkFiles").prop("disabled", false);
    sddb.tableLogEntryAssysAdd.clear().search("").draw();
    sddb.tableLogEntryAssysRemove.clear().search("").draw();
    sddb.tableLogEntryPersonsAdd.clear().search("").draw();
    sddb.tableLogEntryPersonsRemove.clear().search("").draw();

    sddb.cfg.magicSuggests[5].clear();
    $("#QcdDateTime").val("");

    return sddb.modalWaitWrapper(function () {
        return $.when(
                sddb.refreshTableGeneric(sddb.tableLogEntryPersonsAdd,
                    "/PersonSrv/Get",
                    { getActive: true }),
                sddb.refreshTableGeneric(sddb.tableLogEntryAssysAdd,
                    "AssemblyDbSrv/LookupByLocDTables",
                    { getActive: true, locId: sddb.cfg.magicSuggests[3].getValue()[0] })
            );
    });
};
//fillFiltersFromRequestParams
sddb.fillFiltersFromRequestParams = function () {
    "use strict";
    $("#filterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#filterDateEnd").val(moment().format("YYYY-MM-DD"));

    if (!PersonId) { return $.Deferred().resolve(); }

    return sddb.modalWaitWrapper(function () {
        return $.ajax({
            type: "POST",
            url: "/PersonSrv/GetAllByIds",
            timeout: 120000,
            data: { ids: [PersonId], getActive: true },
            dataType: "json"
        })
            .then(function (data) {
                if (typeof data[0].Id !== undefined) {
                    sddb.msSetSelectionSilent(sddb.msFilterByPerson, [{
                        id: data[0].Id,
                        name: data[0].FirstName + " " + data[0].LastName + " " + data[0].Initials
                    }]);
                }
            });
    });
};
//refreshMainView
sddb.refreshMainView = function () {
    "use strict";
    sddb.cfg.tableMain.clear().search("").draw();
    if ($("#filterDateStart").val() === "" || $("#filterDateEnd").val() === "") { return $.Deferred().resolve(); }
    
    var endDate = moment($("#filterDateEnd").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");
    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/PersonLogEntrySrv/GetByAltIds",
        {
            personIds: sddb.msFilterByPerson.getValue(),
            typeIds: sddb.msFilterByType.getValue(),
            projectIds: sddb.msFilterByProject.getValue(),
            assyIds: sddb.msFilterByAssy.getValue(),
            startDate: $("#filterDateStart").val(),
            endDate: endDate,
            getActive: sddb.cfg.currentActive
        },
        "POST");
    });
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        sddb.updateIdsResolveIfAnySelected()
            .then(function () {
                return sddb.showModalConfirm("Confirm that you want to QC the entry(ies).", "Confirm QC");
            })
            .then(function () {
                sddb.saveViewSettings();
                return sddb.qcSelected();
            })
            .done(function () {
                sddb.loadViewSettings();
            });
    });

    //Initialize MagicSuggest msFilterByPerson
    sddb.msFilterByPerson = sddb.msSetFilter("msFilterByPerson", "/PersonSrv/LookupFromProject");

    //Initialize MagicSuggest msFilterByType
    sddb.msFilterByType = sddb.msSetFilter("msFilterByType", "/PersonActivityTypeSrv/Lookup");

    //Initialize MagicSuggest sddb.msFilterByProject
    sddb.msFilterByProject = sddb.msSetFilter("msFilterByProject", "/ProjectSrv/Lookup");
        
    //Initialize MagicSuggest msFilterByAssy
    sddb.msFilterByAssy = sddb.msSetFilter("msFilterByAssy", "/AssemblyDbSrv/LookupByProj",
        { dataUrlParams: { projectIds: sddb.msFilterByProject.getValue } });
            

    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("EnteredByPerson_Id", "/PersonSrv/Lookup");
    sddb.msAddToArray("PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup");
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup", {}, function () {
            sddb.cfg.magicSuggests[3].clear();
            sddb.cfg.magicSuggests[4].clear();
            sddb.tableLogEntryAssysAdd.clear().search("").draw();
            if (this.getValue().length === 0) {
                sddb.cfg.magicSuggests[3].disable();
                sddb.cfg.magicSuggests[4].disable();
            }
            else {
                sddb.cfg.magicSuggests[3].enable();
                sddb.cfg.magicSuggests[4].enable();
            }
        });
    sddb.msAddToArray("AssignedToLocation_Id", "/LocationSrv/LookupByProj",
        { dataUrlParams: { projectIds: sddb.cfg.magicSuggests[2].getValue } }, function () {
            sddb.tableLogEntryAssysAdd.clear().search("").draw();
            if (this.getValue().length === 0) { return; }

            sddb.modalWaitWrapper(function () {
                if (sddb.cfg.currentIds.length !== 0) {
                    return sddb.refreshTableGeneric(sddb.tableLogEntryAssysAdd,
                        "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                        { ids: sddb.cfg.currentIds, locId: sddb.cfg.magicSuggests[3].getValue()[0] }, "POST");
                }
                return sddb.refreshTableGeneric(sddb.tableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
                        { getActive: true, locId: sddb.cfg.magicSuggests[3].getValue()[0] });
            })
                .done(function () { $("#AssignedToLocation_Id input").focus(); });
        });
    sddb.msAddToArray("AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj",
        { dataUrlParams: { projectIds: sddb.cfg.magicSuggests[2].getValue } });
    sddb.msAddToArray("QcdByPerson_Id", "/PersonSrv/Lookup");

    //Wire Up editFormBtnQcSelected
    $("#editFormBtnQcSelected").click(function () {
        if (sddb.cfg.currentIds.length === 0) {
            sddb.showModalFail("QC not possible!", "You cannot QC an entry while it is being created.");
            return;
        }
        sddb.showModalConfirm("Confirm that you want to QC the entry(ies).", "Confirm QC", "no")
            .then(sddb.qcSelected)
            .done(function () {
                sddb.modalWaitWrapper(function () {
                    return sddb.fillFormForEditGeneric(sddb.cfg.currentIds,
                        "POST", "/PersonLogEntrySrv/GetByIds", sddb.cfg.currentActive,
                        sddb.cfg.editFormId, sddb.cfg.labelTextEdit, sddb.cfg.magicSuggests);
                });
            });
    });
        
    //--------------------------------------View Initialization------------------------------------//

    sddb.fillFiltersFromRequestParams().done(sddb.refreshMainView);
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});



