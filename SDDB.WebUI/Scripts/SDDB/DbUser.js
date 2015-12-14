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

var tableDBRolesAdd = {},
    tableDBRolesRemove = {};

var recordTemplate = {
    Id: "RecordTemplateId",
    LastName: null,
    FirstName: null,
    UserName: null,
    Email: null,
    LDAPAuthenticated_bl: null,
    Password: null,
    PasswordConf: null
};

labelTextCreate = "Create SDDB User";
labelTextEdit = "Edit SDDB User";
urlFillForEdit = "/DBUserSrv/GetByIds";
urlEdit = "/DBUserSrv/Edit";
urlDelete = "/DBUserSrv/Delete";

urlRefreshMainView = "/DbUserSrv/Get";
dataRefreshMainView = function () { return {}; };

callBackAfterCreate = function () {
    magicSuggests[0].enable();
    return $.Deferred().resolve();
};

callBackAfterEdit = function () {
    //Id not handled by submitEditsGeneric, has to be set
    if (currentRecords.length == 1) {
        magicSuggests[0].addToSelection([{
            id: currentRecords[0].Id,
            name: currentRecords[0].FirstName + " " + currentRecords[0].LastName
        }], true);
    }
    else {
        magicSuggests[0].addToSelection([{ id: "_VARIES_", name: "_VARIES_" }], true);
    }
    magicSuggests[0].disable();
    return $.Deferred().resolve();
};

callBackBeforeSubmitEdit = function () {
    //Id not handled by submitEditsGeneric, has to be set
    if (currentRecords.length == 1) { currentRecords[0].Id = magicSuggests[0].getValue()[0]; }

    //Password and PasswordConf not returned in CurrentRecords by server, needs to be added manually
    for (var i = 0; i < currentRecords.length; i++) {
        currentRecords[i].Password = "";
        currentRecords[i].PasswordConf = "";
    }

    return $.Deferred().resolve();
};

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//

    //Wire Up btnEditRoles 
    $("#btnEditRoles").click(function () {
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }

        if (currentIds.length == 1) {
            var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
            $("#bBRolesViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else {
            $("#bBRolesViewPanel").text("_MULTIPLE_");
        }

        sddb.modalWaitWrapper(function () {
            return sddb.fillFormForRelatedGeneric(
                tableDBRolesAdd, tableDBRolesRemove, currentIds,
                "GET", "/DBUserSrv/GetUserRoles", { id: currentIds[0] },
                "GET", "/DBUserSrv/GetUserRolesNot", { id: currentIds[0] },
                "GET", "/DBUserSrv/GetAllRoles",
                null, 0);
        })
            .done(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView("mainView", "dBRolesView", "tdo-btngroup-dbroles");
            });
    });
        
    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5]
    ];

    //tableMain DBUsers
    tableMain = $("#tableMain").DataTable({
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
    });

    //showing the first Set of columns on startup;
    sddb.showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    ///Initialize MagicSuggest Array
    sddb.msAddToArray("Id", "/PersonSrv/PersonsWoDBUser");
   
    //----------------------------------------dBRolesView----------------------------------------//

    //Wire Up dBRolesViewBtnCancel
    $("#dBRolesViewBtnCancel").click(function () {
        sddb.switchView("dBRolesView", "mainView", "tdo-btngroup-main", tableMain);
    });

    //Wire Up dBRolesViewBtnOk
    $("#dBRolesViewBtnOk").click(function () {
        var idsAdd = tableDBRolesAdd.cells(".ui-selected", "Name:name", { page: "current" }).data().toArray(),
            idsRemove = tableDBRolesRemove.cells(".ui-selected", "Name:name", { page: "current" }).data().toArray();
        
        if (idsAdd.length + idsRemove.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        sddb.modalWaitWrapper(function () {
            return sddb.submitEditsForRelatedGeneric(currentIds, idsAdd, idsRemove, "/DBUserSrv/EditRoles");
        })
            .then(function () {
                return sddb.refreshMainView();
            })
            .done(function () {
                sddb.switchView("dBRolesView", "mainView", "tdo-btngroup-main", tableMain);
            });
    });

    //---------------------------------------DataTables------------

    //tableDBRolesAdd
    tableDBRolesAdd = $("#tableDBRolesAdd").DataTable({
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
    tableDBRolesRemove = $("#tableDBRolesRemove").DataTable({
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
      
    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//---------------------------------------Helper Methods--------------------------------------//



