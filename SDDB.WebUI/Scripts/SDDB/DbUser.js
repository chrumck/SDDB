/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableDBRolesAdd = {};
var TableDBRolesRemove = {};

var RecordTemplate = {
    Id: "RecordTemplateId",
    LastName: null,
    FirstName: null,
    UserName: null,
    Email: null,
    LDAPAuthenticated_bl: null,
    Password: null,
    PasswordConf: null
};

LabelTextCreate = "Create SDDB User";
LabelTextEdit = "Edit SDDB User";
UrlFillForEdit = "/DBUserSrv/GetByIds";
UrlEdit = "/DBUserSrv/Edit";
UrlDelete = "/DBUserSrv/Delete";

callBackBeforeCreate = function () {
    MagicSuggests[0].enable();
    return $.Deferred().resolve();
};

callBackBeforeEdit = function () {
    //Id not handled by submitEditsGeneric, has to be set
    if (CurrRecords.length == 1) {
        MagicSuggests[0].addToSelection([{
            id: CurrRecords[0].Id,
            name: CurrRecords[0].FirstName + " " + CurrRecords[0].LastName
        }], true);
    }
    else {
        MagicSuggests[0].addToSelection([{ id: "_VARIES_", name: "_VARIES_" }], true);
    }
    MagicSuggests[0].disable();
    return $.Deferred().resolve();
};

callBackBeforeSubmitEdit = function (data) {
    //Id not handled by submitEditsGeneric, has to be set
    if (CurrRecords.length == 1) { CurrRecords[0].Id = MagicSuggests[0].getValue()[0]; }

    //Password and PasswordConf not returned in CurrentRecords by server, needs to be added manually
    for (var i = 0; i < CurrRecords.length; i++) {
        CurrRecords[i].Password = "";
        CurrRecords[i].PasswordConf = "";
    }

    return $.Deferred().resolve();
};

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire Up BtnEditRoles 
    $("#BtnEditRoles").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }

        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current" }).data();
            $("#DBRolesViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else {
            $("#DBRolesViewPanel").text("_MULTIPLE_");
        }

        modalWaitWrapper(function () {
            return fillFormForRelatedGeneric(
                TableDBRolesAdd, TableDBRolesRemove, CurrIds,
                "GET", "/DBUserSrv/GetUserRoles", { id: CurrIds[0] },
                "GET", "/DBUserSrv/GetUserRolesNot", { id: CurrIds[0] },
                "GET", "/DBUserSrv/GetAllRoles",
                null, 0);
        })
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "DBRolesView", "tdo-btngroup-dbroles");
            });
    });
        
    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5]
    ];

    //TableMain DBUsers
    TableMain = $("#TableMain").DataTable({
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
    showColumnSet(TableMainColumnSets, 1);

    //---------------------------------------EditFormView----------------------------------------//

    ///Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "Id", "/PersonSrv/PersonsWoDBUser", 1);
   
    //----------------------------------------DBRolesView----------------------------------------//

    //Wire Up DBRolesViewBtnCancel
    $("#DBRolesViewBtnCancel").click(function () {
        switchView("DBRolesView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up DBRolesViewBtnOk
    $("#DBRolesViewBtnOk").click(function () {
        var idsAdd = TableDBRolesAdd.cells(".ui-selected", "Name:name", { page: "current" }).data().toArray(),
            idsRemove = TableDBRolesRemove.cells(".ui-selected", "Name:name", { page: "current" }).data().toArray();
        
        if (idsAdd.length + idsRemove.length === 0) {
            showModalNothingSelected();
            return;
        }
        modalWaitWrapper(function () {
            return submitEditsForRelatedGeneric(CurrIds, idsAdd, idsRemove, "/DBUserSrv/EditRoles");
        })
            .then(function () {
                return refreshMainView();
            })
            .done(function () {
                switchView("DBRolesView", "MainView", "tdo-btngroup-main", TableMain);
            });
    });

    //---------------------------------------DataTables------------

    //TableDBRolesAdd
    TableDBRolesAdd = $("#TableDBRolesAdd").DataTable({
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

    //TableDBRolesRemove
    TableDBRolesRemove = $("#TableDBRolesRemove").DataTable({
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

    refreshMainView();
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh Main view 
function refreshMainView() {
    var deferred0 = $.Deferred();
    refreshTblGenWrp(TableMain, "/DbUserSrv/Get").done(deferred0.resolve);
    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//



