/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableProjectsAdd = {},
    TableProjectsRemove = {},
    TablePersonGroupsAdd = {},
    TablePersonGroupsRemove = {},
    TableManagedGroupsAdd = {},
    TableManagedGroupsRemove = {};

var RecordTemplate = {
    Id: "RecordTemplateId",
    FirstName: null,
    LastName: null,
    Initials: null,
    Phone: null,
    PhoneMobile: null,
    Email: null,
    Comments: null,
    IsCurrentEmployee_bl: null,
    EmployeePosition: null,
    IsSalaried_bl: null,
    EmployeeStart: null,
    EmployeeEnd: null,
    EmployeeDetails: null,
    IsActive_bl: null
};

LabelTextCreate = "Create Person";
LabelTextEdit = "Edit Person";
UrlFillForEdit = "/PersonSrv/GetAllByIds";
UrlEdit = "/PersonSrv/Edit";
UrlDelete = "/PersonSrv/Delete";

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire Up BtnEditPrsProj 
    $("#BtnEditPrsProj").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }

        if (CurrIds.length === 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current" }).data();
            $("#PrsProjViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else { 
            $("#PrsProjViewPanel").text("_MULTIPLE_"); 
        }

        modalWaitWrapper(function () {
            return fillFormForRelatedGeneric(
                TableProjectsAdd, TableProjectsRemove, CurrIds,
                "GET", "/PersonSrv/GetPersonProjects", { id: CurrIds[0] },
                "GET", "/PersonSrv/GetPersonProjectsNot", { id: CurrIds[0] },
                "GET", "/ProjectSrv/Get", { getActive: true });
        })
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "PrsProjView", "tdo-btngroup-prsproj");
            });

    });

    //Wire Up BtnEditPersonGroups 
    $("#BtnEditPersonGroups").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }

        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current" }).data();
            $("#PersonGroupsViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else {
            $("#PersonGroupsViewPanel").text("_MULTIPLE_");
        }

        modalWaitWrapper(function () {
            return fillFormForRelatedGeneric(
                TablePersonGroupsAdd, TablePersonGroupsRemove, CurrIds,
                "GET", "/PersonSrv/GetPersonGroups", { id: CurrIds[0] },
                "GET", "/PersonSrv/GetPersonGroupsNot", { id: CurrIds[0] },
                "GET", "/PersonGroupSrv/Get", { getActive: true });
        })
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "PersonGroupsView", "tdo-btngroup-prsgroups");
            });
    });

    //Wire Up BtnEditManagedGroups 
    $("#BtnEditManagedGroups").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }

        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected", { page: "current" }).data();
            $("#ManagedGroupsViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else {
            $("#ManagedGroupsViewPanel").text("_MULTIPLE_");
        }

        modalWaitWrapper(function () {
            return fillFormForRelatedGeneric(
                TableManagedGroupsAdd, TableManagedGroupsRemove, CurrIds,
                "GET", "/PersonSrv/GetManagedGroups", { id: CurrIds[0] },
                "GET", "/PersonSrv/GetManagedGroupsNot", { id: CurrIds[0] },
                "GET", "/PersonGroupSrv/Get", { getActive: true });
        })
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "ManagedGroupsView", "tdo-btngroup-managedgroups");
            });
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) { showModalSelectOne(); }
        else {
            window.open("/PersonLogEntry?PersonId=" +
                TableMain.cell(".ui-selected", "Id:name", { page: "current"}).data());
        }
    });

    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1, 2],
        [3, 4, 5, 6, 7],
        [8, 9, 10, 11, 12, 13]
    ];
        
    //TableMain Persons
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            //------------------------------------------------first set of columns
            { data: "Initials", name: "Initials" },//3
            { data: "Phone", name: "Phone" },//4
            { data: "PhoneMobile", name: "PhoneMobile" },//5
            { data: "Email", name: "Email" },//6
            { data: "Comments", name: "Comments" },//7
            //------------------------------------------------second set of columns
            { data: "IsCurrentEmployee_bl", name: "IsCurrentEmployee_bl" },//8
            { data: "EmployeePosition", name: "EmployeePosition" },//9
            { data: "IsSalaried_bl", name: "IsSalaried_bl" },//10
            { data: "EmployeeStart", name: "EmployeeStart" },//11
            { data: "EmployeeEnd", name: "EmployeeEnd" },//12
            { data: "EmployeeDetails", name: "EmployeeDetails" },//13
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" }//14
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 8, 10, 11, 12, 14], searchable: false },
            //1st set of columns - responsive
            { targets: [4, 5, 6], className: "hidden-xs hidden-sm" },
            { targets: [7], className: "hidden-xs hidden-sm hidden-md" },
            //2nd set of columns - responsive
            { targets: [8, 9, 10, 11, 12, 13], visible: false },
            { targets: [9, 11, 12], className: "hidden-xs hidden-sm" },
            { targets: [10, 13], className: "hidden-xs hidden-sm hidden-md" }
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

    //Enable DatePicker
    $("[data-val-dbisdateiso]").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });
            
    //----------------------------------------PrsProjView----------------------------------------//

    //Wire Up PrsProjViewBtnCancel
    $("#PrsProjViewBtnCancel").click(function () {
        switchView("PrsProjView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up PrsProjViewBtnOk
    $("#PrsProjViewBtnOk").click(function () {
        var idsAdd = TableProjectsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            idsRemove = TableProjectsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();

        if (idsAdd.length + idsRemove.length === 0) {
            showModalNothingSelected();
            return;
        }
        modalWaitWrapper(function () {
            return submitEditsForRelatedGeneric(CurrIds, idsAdd, idsRemove, "/PersonSrv/EditPersonProjects");
        })
            .then(function () {
                return refreshMainView();
            })
            .done(function () {
                switchView("PrsProjView", "MainView", "tdo-btngroup-main", TableMain);
            });
    });

    //---------------------------------------DataTables------------

    //TableProjectsAdd
    TableProjectsAdd = $("#TableProjectsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectCode", name: "ProjectCode" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },
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

    //TableProjectsRemove
    TableProjectsRemove = $("#TableProjectsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectCode", name: "ProjectCode" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },
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

    //----------------------------------------PersonGroupsView----------------------------------------//

    //Wire Up PersonGroupsViewBtnCancel
    $("#PersonGroupsViewBtnCancel").click(function () {
        switchView("PersonGroupsView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up PersonGroupsViewBtnOk
    $("#PersonGroupsViewBtnOk").click(function () {
        var idsAdd = TablePersonGroupsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            idsRemove = TablePersonGroupsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();

        if (idsAdd.length + idsRemove.length === 0) {
            showModalNothingSelected();
            return;
        }

        modalWaitWrapper(function () {
            return submitEditsForRelatedGeneric(CurrIds, idsAdd, idsRemove, "/PersonSrv/EditPersonGroups");
        })
            .then(function () {
                return refreshMainView();
            })
            .done(function () {
                switchView("PersonGroupsView", "MainView", "tdo-btngroup-main", TableMain);
            });
    });
    
    //---------------------------------------DataTables------------

    //TablePersonGroupsAdd
    TablePersonGroupsAdd = $("#TablePersonGroupsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },
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

    //TablePersonGroupsRemove
    TablePersonGroupsRemove = $("#TablePersonGroupsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },
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

    //----------------------------------------ManagedGroupsView----------------------------------------//

    //Wire Up ManagedGroupsViewBtnCancel
    $("#ManagedGroupsViewBtnCancel").click(function () {
        switchView("ManagedGroupsView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up ManagedGroupsViewBtnOk
    $("#ManagedGroupsViewBtnOk").click(function () {
        var idsAdd = TableManagedGroupsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            idsRemove = TableManagedGroupsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();

        if (idsAdd.length + idsRemove.length === 0) {
            showModalNothingSelected();
            return;
        }

        modalWaitWrapper(function () {
            return submitEditsForRelatedGeneric(CurrIds, idsAdd, idsRemove, "/PersonSrv/EditManagedGroups");
        })
            .then(function () {
                return refreshMainView();
            })
            .done(function () {
                switchView("ManagedGroupsView", "MainView", "tdo-btngroup-main", TableMain);
            });
    });

    //---------------------------------------DataTables------------

    //TableManagedGroupsAdd
    TableManagedGroupsAdd = $("#TableManagedGroupsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },
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

    //TableManagedGroupsRemove
    TableManagedGroupsRemove = $("#TableManagedGroupsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },
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
    refreshTblGenWrp(TableMain, "/PersonSrv/GetAll", { getActive: GetActive }).done(deferred0.resolve);
    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//



