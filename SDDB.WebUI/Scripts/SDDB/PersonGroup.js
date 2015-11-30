/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var RecordTemplate = {
    Id: "RecordTemplateId",
    PrsGroupName: null,
    PrsGroupAltName: null,
    Comments: null,
    IsActive_bl: null
},
    TableGroupManagersAdd = {},
    TableGroupManagersRemove = {},
    TableGroupPersonsAdd = {},
    TableGroupPersonsRemove = {};

LabelTextCreate = "Create Group";
LabelTextEdit = "Edit Group";
UrlFillForEdit = "/PersonGroupSrv/GetByIds";
UrlEdit = "/PersonGroupSrv/Edit";
UrlDelete = "/PersonGroupSrv/Delete";

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//
        
    //Wire Up BtnEditGroupPersons 
    $("#BtnEditGroupPersons").click(function () {
        event.preventDefault();
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current" }).data();
            $("#GroupPersonsViewPanel").text(selectedRecord.PrsGroupName);
        }
        else { $("#GroupPersonsViewPanel").text("_MULTIPLE_"); }

        showModalWait();
        fillFormForRelatedGeneric(
                TableGroupPersonsAdd, TableGroupPersonsRemove, CurrIds,
                "GET", "/PersonGroupSrv/GetGroupPersons", { id: CurrIds[0]},
                "GET", "/PersonGroupSrv/GetGroupPersonsNot", { id: CurrIds[0]},
                "GET", "/PersonSrv/GetAll",
                {getActive: true }, 1)
            .always(hideModalWait)
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "GroupPersonsView", "tdo-btngroup-grouppersons");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

    });

    //Wire Up BtnEditGroupManagers 
    $("#BtnEditGroupManagers").click(function () {
        event.preventDefault();
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current" }).data();
            $("#GroupManagersViewPanel").text(selectedRecord.PrsGroupName);
        }
        else { $("#GroupManagersViewPanel").text("_MULTIPLE_"); }

        showModalWait();
        fillFormForRelatedGeneric(
                TableGroupManagersAdd, TableGroupManagersRemove, CurrIds,
                "GET", "/PersonGroupSrv/GetGroupManagers", { id: CurrIds[0] },
                "GET", "/PersonGroupSrv/GetGroupManagersNot", { id: CurrIds[0] },
                "GET", "/PersonSrv/GetAll", { getActive: true }, 1)
            .always(hideModalWait)
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "GroupManagersView", "tdo-btngroup-groupmanagers");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

    });
            
    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3]
    ];
    
    //TableMain PersonGroups
    TableMain = $("#TableMain").DataTable({
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
    showColumnSet(TableMainColumnSets, 1);

    //---------------------------------------EditFormView----------------------------------------//

    
    //----------------------------------------GroupPersonsView----------------------------------------//

    //Wire Up GroupPersonsViewBtnCancel
    $("#GroupPersonsViewBtnCancel").click(function () {
        switchView("GroupPersonsView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up GroupPersonsViewBtnOk
    $("#GroupPersonsViewBtnOk").click(function () {
        if (TableGroupPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            TableGroupPersonsRemove.rows(".ui-selected", { page: "current" }).data().length === 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
        submitEditsForRelatedGeneric(
                CurrIds,
                TableGroupPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                TableGroupPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonGroupSrv/EditGroupPersons")
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("GroupPersonsView", "MainView", "tdo-btngroup-main", TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

    });

    //---------------------------------------DataTables------------

    //TableGroupPersonsAdd
    TableGroupPersonsAdd = $("#TableGroupPersonsAdd").DataTable({
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

    //TableGroupPersonsRemove
    TableGroupPersonsRemove = $("#TableGroupPersonsRemove").DataTable({
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
   
    //----------------------------------------GroupManagersView----------------------------------------//

    //Wire Up GroupManagersViewBtnCancel
    $("#GroupManagersViewBtnCancel").click(function () {
        switchView("GroupManagersView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up GroupManagersViewBtnOk
    $("#GroupManagersViewBtnOk").click(function () {
        if (TableGroupManagersAdd.rows(".ui-selected", { page: "current" }).data().length +
            TableGroupManagersRemove.rows(".ui-selected", { page: "current" }).data().length === 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
        submitEditsForRelatedGeneric(
                CurrIds,
                TableGroupManagersAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                TableGroupManagersRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonGroupSrv/EditGroupManagers")
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("GroupManagersView", "MainView", "tdo-btngroup-main", TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

    });

    //---------------------------------------DataTables------------

    //TableGroupManagersAdd
    TableGroupManagersAdd = $("#TableGroupManagersAdd").DataTable({
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

    //TableGroupManagersRemove
    TableGroupManagersRemove = $("#TableGroupManagersRemove").DataTable({
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

    refreshMainView();
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh Main view 
function refreshMainView() {
    var deferred0 = $.Deferred();
    refreshTblGenWrp(TableMain, "/PersonGroupSrv/Get", { getActive: GetActive }).done(deferred0.resolve);
    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//



