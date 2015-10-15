/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var TableGroupManagersAdd = {};
var TableGroupManagersRemove = {};
var TableGroupPersonsAdd = {};
var TableGroupPersonsRemove = {};

var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    PrsGroupName: null,
    PrsGroupAltName: null,
    Comments: null,
    IsActive_bl: null
};

var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Person Group", "MainView");
        saveViewSettings(TableMain);
        switchView("MainView", "EditFormView", "tdo-btngroup-edit");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
	    fillFormForEditGeneric(CurrIds, "POST", "/PersonGroupSrv/GetByIds",
		        GetActive, "EditForm", "Edit Person Group", MagicSuggests)
            .always(hideModalWait)
            .done(function (currRecords) {
                CurrRecords = currRecords;
                saveViewSettings(TableMain);
                switchView("MainView", "EditFormView", "tdo-btngroup-edit");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });
    
    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });
    
    //Wire Up BtnEditGroupPersons 
    $("#BtnEditGroupPersons").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current"}).data()
            $("#GroupPersonsViewPanel").text(selectedRecord.PrsGroupName);
        }
        else { $("#GroupPersonsViewPanel").text("_MULTIPLE_") }

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
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current"}).data()
            $("#GroupManagersViewPanel").text(selectedRecord.PrsGroupName);
        }
        else { $("#GroupManagersViewPanel").text("_MULTIPLE_") }

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

    //wire up BtnTableMainExport
    $("#BtnTableMainExport").click(function (event) {
        exportTableToTxt(TableMain);
    });

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) {
            GetActive = true;
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        } else {
            GetActive = false;
            $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        }
        refreshMainView();
    });

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
            { targets: [0, 4], visible: false }, // - never show
            { targets: [0, 4], searchable: false },  //"orderable": false, "visible": false
            { targets: [3], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" } // - first set of columns
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

    //---------------------------------------EditFormView----------------------------------------//

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel").click(function () {
        switchView("EditFormView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
            showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
            return;
        }
        showModalWait();
        var createMultiple = $("#CreateMultiple").val() != "" ? $("#CreateMultiple").val() : 1;
    	submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/PersonGroupSrv/Edit", createMultiple)
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("EditFormView", "MainView", "tdo-btngroup-main", TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
    });

    //----------------------------------------GroupPersonsView----------------------------------------//

    //Wire Up GroupPersonsViewBtnCancel
    $("#GroupPersonsViewBtnCancel").click(function () {
        switchView("GroupPersonsView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up GroupPersonsViewBtnOk
    $("#GroupPersonsViewBtnOk").click(function () {
        if (TableGroupPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            TableGroupPersonsRemove.rows(".ui-selected", { page: "current" }).data().length == 0) {
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
            TableGroupManagersRemove.rows(".ui-selected", { page: "current" }).data().length == 0) {
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
    
    $("#InitialView").addClass("hidden");
    $("#MainView").removeClass("hidden");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function deleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/PersonGroupSrv/Delete", refreshMainView);
}

//refresh Main view 
function refreshMainView() {
    var deferred0 = $.Deferred();
    refreshTblGenWrp(TableMain, "/PersonGroupSrv/Get", { getActive: GetActive }).done(deferred0.resolve);
    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//



