/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableProjectPersonsAdd = {},
    TableProjectPersonsRemove = {};

var RecordTemplate = {
    Id: "RecordTemplateId",
    ProjectName: null,
    ProjectAltName: null,
    ProjectCode: null,
    Comments: null,
    IsActive_bl: null,
    ProjectManager_Id: null
};

LabelTextCreate = "Create Project";
LabelTextEdit = "Edit Project";
UrlFillForEdit = "/ProjectSrv/GetByIds";
UrlEdit = "/ProjectSrv/Edit";
UrlDelete = "/ProjectSrv/Delete";
urlRefreshMainView = "/ProjectSrv/Get";

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//
    
    //Wire Up BtnEditProjectPersons
    $("#BtnEditProjectPersons").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current" }).data();
            $("#ProjectPersonsViewPanel").text(selectedRecord.ProjectName + " " + selectedRecord.ProjectCode);
        }
        else { $("#ProjectPersonsViewPanel").text("_MULTIPLE_"); }

        showModalWait();
        fillFormForRelatedGeneric(TableProjectPersonsAdd, TableProjectPersonsRemove, CurrIds,
                "GET", "/ProjectSrv/GetProjectPersons", { id: CurrIds[0] },
                "GET", "/ProjectSrv/GetProjectPersonsNot", { id: CurrIds[0] },
                "GET", "/PersonSrv/GetAll", { getActive: true })
            .always(hideModalWait)
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "ProjectPersonsView", "tdo-btngroup-projectpersons");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });
    
    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/Location?ProjectId=" +
            TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
    });

    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5]
    ];
    
    //TableMain Projects
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectAltName", name: "ProjectAltName" },//2
            { data: "ProjectCode", name: "ProjectCode" },//3
            {
                data: "ProjectManager_",
                name: "ProjectManager_",
                render: function (data, type, full, meta) { return data.Initials; }
            }, //4
            { data: "Comments", name: "Comments" },//5
            { data: "IsActive_bl", name: "IsActive_bl" },//6
            { data: "ProjectManager_Id", name: "ProjectManager_Id" }//7
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 6, 7], searchable: false },
            // - first set of columns
            { targets: [2, 3], className: "hidden-xs hidden-sm" }, 
            { targets: [5], className: "hidden-xs hidden-sm hidden-md" }
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

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "ProjectManager_Id", "/PersonSrv/LookupAll", 1);
    
    //----------------------------------------ProjectPersonsView----------------------------------------//

    //Wire Up ProjectPersonsViewBtnCancel
    $("#ProjectPersonsViewBtnCancel").click(function () {
        switchView("ProjectPersonsView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up ProjectPersonsViewBtnOk
    $("#ProjectPersonsViewBtnOk").click(function () {
        if (TableProjectPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            TableProjectPersonsRemove.rows(".ui-selected", { page: "current" }).data().length === 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
        submitEditsForRelatedGeneric(
                CurrIds,
                TableProjectPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                TableProjectPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/ProjectSrv/EditProjectPersons")
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("ProjectPersonsView", "MainView", "tdo-btngroup-main", TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //---------------------------------------DataTables------------

    //TableProjectPersonsAdd
    TableProjectPersonsAdd = $("#TableProjectPersonsAdd").DataTable({
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

    //TableProjectPersonsRemove
    TableProjectPersonsRemove = $("#TableProjectPersonsRemove").DataTable({
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


//---------------------------------------Helper Methods--------------------------------------//



