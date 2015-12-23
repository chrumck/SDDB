/*global sddb */
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
        LastName: null,
        FirstName: null,
        UserName: null,
        Email: null,
        LDAPAuthenticated_bl: null,
        Password: null,
        PasswordConf: null
    },

    tableMainColumnSets: [
        [1],
        [2, 3, 4, 5]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" }, //0
            { data: "LastName", name: "LastName" }, //1
            { data: "FirstName", name: "FirstName" }, //2
            { data: "UserName", name: "UserName" }, //3
            { data: "Email", name: "Email" }, //4
            { data: "LDAPAuthenticated_bl", name: "LDAPAuthenticated_bl" } //5
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 5], searchable: false },
            // - first set of columns
            { targets: [4, 5], className: "hidden-xs hidden-sm" }
        ],
        order: [[1, "asc"]],
        bAutoWidth: false,
        lengthMenu: [10, 25, 50, 100, 200],
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        }
    }),

    labelTextCreate : "Create SDDB User",
    labelTextEdit : "Edit SDDB User",
    urlFillForEdit : "/DBUserSrv/GetByIds",
    urlEdit : "/DBUserSrv/Edit",
    urlDelete: "/DBUserSrv/Delete",
    urlRefreshMainView: "/DbUserSrv/Get",
    dataRefreshMainView: function () {
        "use strict";
        return {};
    }


});

//callBackAfterCreate
sddb.callBackAfterCreate = function () {
    "use strict";
    sddb.cfg.magicSuggests[0].enable();
    return $.Deferred().resolve();
};

//callBackAfterEdit
sddb.callBackAfterEdit = function () {
    "use strict";
    //Id not handled by submitEditsGeneric, has to be set
    if (sddb.cfg.currentRecords.length == 1) {
        sddb.cfg.magicSuggests[0].addToSelection([{
            id: sddb.cfg.currentRecords[0].Id,
            name: sddb.cfg.currentRecords[0].FirstName + " " + sddb.cfg.currentRecords[0].LastName
        }], true);
    }
    else {
        sddb.cfg.magicSuggests[0].addToSelection([{ id: "_VARIES_", name: "_VARIES_" }], true);
    }
    sddb.cfg.magicSuggests[0].disable();
    return $.Deferred().resolve();
};

//callBackBeforeSubmitEdit
sddb.callBackBeforeSubmitEdit = function () {
    "use strict";
    //Id not handled by submitEditsGeneric, has to be set
    if (sddb.cfg.currentRecords.length == 1) {
        sddb.cfg.currentRecords[0].Id = sddb.cfg.magicSuggests[0].getValue()[0];
    }

    //Password and PasswordConf not returned in CurrentRecords by server, needs to be added manually
    for (var i = 0; i < sddb.cfg.currentRecords.length; i += 1) {
        sddb.cfg.currentRecords[i].Password = "";
        sddb.cfg.currentRecords[i].PasswordConf = "";
    }

    return $.Deferred().resolve();
};

//----------------------------------------------setup after page load------------------------------------------------//

$(document).ready(function () {
    "use strict";
    //--------------------------------------dbRolesCfg--------------------------------------//

    //tableDBRolesAdd
    sddb.tableDBRolesAdd = $("#tableDBRolesAdd").DataTable({
        columns: [
            { data: "Name", name: "Name" }
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

    //tableDBRolesRemove
    sddb.tableDBRolesRemove = $("#tableDBRolesRemove").DataTable({
        columns: [
            { data: "Name", name: "Name" }
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

    //dbRolesCfg
    sddb.dbRolesCfg = {
        tableAdd: sddb.tableDBRolesAdd,
        tableRemove: sddb.tableDBRolesRemove,
        url: "/DBUserSrv/GetUserRoles",
        urlNot: "/DBUserSrv/GetUserRolesNot",
        sortColumn: 0,
        relatedViewId: "dBRolesView",
        relatedViewBtnGroupClass: "tdo-btngroup-dbroles",
        relatedViewPanelId: "bBRolesViewPanel",
        relatedViewPanelText: function () {
            if (sddb.cfg.currentIds.length > 1) { return "_MULTIPLE_"; }
            var selectedRecord = sddb.cfg.tableMain.row(".ui-selected", { page: "current" }).data();
            return selectedRecord.FirstName + " " + selectedRecord.LastName;
        },
        urlEdit: "/DBUserSrv/EditRoles"
    };

    //-----------------------------------------mainView------------------------------------------//
 
    //Wire Up btnEditRoles 
    $("#btnEditRoles").click(function (event) {
        event.preventDefault();
        sddb.prepareRelatedFormForEdit(sddb.dbRolesCfg);
    });

    //---------------------------------------editFormView----------------------------------------//

    ///Initialize MagicSuggest Array
    sddb.msAddToArray("Id", "/PersonSrv/PersonsWoDBUser");
   
    //----------------------------------------dBRolesView----------------------------------------//

    //Wire Up dBRolesViewBtnCancel
    $("#dBRolesViewBtnCancel").click(function (event) {
        event.preventDefault();
        sddb.switchView(sddb.dbRolesCfg.relatedViewId, sddb.cfg.mainViewId,
            sddb.cfg.mainViewBtnGroupClass, sddb.cfg.tableMain);
    });

    //Wire Up dBRolesViewBtnOk
    $("#dBRolesViewBtnOk").click(function (event) {
        event.preventDefault();
        sddb.submitRelatedEditForm(sddb.dbRolesCfg);
    });
      
    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});
