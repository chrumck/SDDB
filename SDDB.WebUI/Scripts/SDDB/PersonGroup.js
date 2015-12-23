/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

"use strict";

//--------------------------------------Global Properties------------------------------------//

var recordTemplate = {
    Id: "RecordTemplateId",
    PrsGroupName: null,
    PrsGroupAltName: null,
    Comments: null,
    IsActive_bl: null
},
    tableGroupManagersAdd = {},
    tableGroupManagersRemove = {},
    tableGroupPersonsAdd = {},
    tableGroupPersonsRemove = {};

labelTextCreate = "Create Group";
labelTextEdit = "Edit Group";
urlFillForEdit = "/PersonGroupSrv/GetByIds";
urlEdit = "/PersonGroupSrv/Edit";
urlDelete = "/PersonGroupSrv/Delete";
urlRefreshMainView = "/PersonGroupSrv/Get";

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//
        
    //Wire Up btnEditGroupPersons 
    $("#btnEditGroupPersons").click(function (event) {
        event.preventDefault();
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        if (currentIds.length == 1) {
            var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
            $("#groupPersonsViewPanel").text(selectedRecord.PrsGroupName);
        }
        else { $("#groupPersonsViewPanel").text("_MULTIPLE_"); }

        sddb.showModalWait();
        sddb.fillFormForRelatedGeneric(
                tableGroupPersonsAdd, tableGroupPersonsRemove, currentIds,
                "GET", "/PersonGroupSrv/GetGroupPersons", { ids: currentIds },
                "GET", "/PersonGroupSrv/GetGroupPersonsNot", { ids: currentIds },
                1)
            .always(sddb.hideModalWait)
            .done(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView("mainView", "groupPersonsView", "tdo-btngroup-grouppersons");
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });

    });

    //Wire Up btnEditGroupManagers 
    $("#btnEditGroupManagers").click(function (event) {
        event.preventDefault();
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        if (currentIds.length == 1) {
            var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
            $("#groupManagersViewPanel").text(selectedRecord.PrsGroupName);
        }
        else { $("#groupManagersViewPanel").text("_MULTIPLE_"); }

        sddb.showModalWait();
        sddb.fillFormForRelatedGeneric(
                tableGroupManagersAdd, tableGroupManagersRemove, currentIds,
                "GET", "/PersonGroupSrv/GetGroupManagers", { ids: currentIds },
                "GET", "/PersonGroupSrv/GetGroupManagersNot", { ids: currentIds },
                1)
            .always(sddb.hideModalWait)
            .done(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView("mainView", "groupManagersView", "tdo-btngroup-groupmanagers");
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });

    });
            
    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3]
    ];
    
    //tableMain PersonGroups
    tableMain = $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" },//2
            { data: "Comments", name: "Comments" },//3
            { data: "IsActive_bl", name: "IsActive_bl" }//4
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 4], searchable: false },
            // - first set of columns
            { targets: [3], className: "hidden-xs hidden-sm" },
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }
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
    //showing the first Set of columns on startup;
    sddb.showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    
    //----------------------------------------groupPersonsView----------------------------------------//

    //Wire Up groupPersonsViewBtnCancel
    $("#groupPersonsViewBtnCancel").click(function () {
        sddb.switchView("groupPersonsView", "mainView", "tdo-btngroup-main", tableMain);
    });

    //Wire Up groupPersonsViewBtnOk
    $("#groupPersonsViewBtnOk").click(function () {
        if (tableGroupPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            tableGroupPersonsRemove.rows(".ui-selected", { page: "current" }).data().length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        sddb.showModalWait();
        sddb.submitEditsForRelatedGeneric(
                currentIds,
                tableGroupPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                tableGroupPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonGroupSrv/EditGroupPersons")
            .always(sddb.hideModalWait)
            .done(function () {
                sddb.refreshMainView()
                    .done(function () {
                        sddb.switchView("groupPersonsView", "mainView", "tdo-btngroup-main", tableMain);
                    });
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });

    });

    //---------------------------------------DataTables------------

    //tableGroupPersonsAdd
    tableGroupPersonsAdd = $("#tableGroupPersonsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //tableGroupPersonsRemove
    tableGroupPersonsRemove = $("#tableGroupPersonsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });
   
    //----------------------------------------groupManagersView----------------------------------------//

    //Wire Up groupManagersViewBtnCancel
    $("#groupManagersViewBtnCancel").click(function () {
        sddb.switchView("groupManagersView", "mainView", "tdo-btngroup-main", tableMain);
    });

    //Wire Up groupManagersViewBtnOk
    $("#groupManagersViewBtnOk").click(function () {
        if (tableGroupManagersAdd.rows(".ui-selected", { page: "current" }).data().length +
            tableGroupManagersRemove.rows(".ui-selected", { page: "current" }).data().length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        sddb.showModalWait();
        sddb.submitEditsForRelatedGeneric(
                currentIds,
                tableGroupManagersAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                tableGroupManagersRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonGroupSrv/EditGroupManagers")
            .always(sddb.hideModalWait)
            .done(function () {
                sddb.refreshMainView()
                    .done(function () {
                        sddb.switchView("groupManagersView", "mainView", "tdo-btngroup-main", tableMain);
                    });
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });

    });

    //---------------------------------------DataTables------------

    //tableGroupManagersAdd
    tableGroupManagersAdd = $("#tableGroupManagersAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //tableGroupManagersRemove
    tableGroupManagersRemove = $("#tableGroupManagersRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });
        
    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//---------------------------------------Helper Methods--------------------------------------//



