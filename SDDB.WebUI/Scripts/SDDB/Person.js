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

var tableProjectsAdd = {},
    tableProjectsRemove = {},
    tablePersonGroupsAdd = {},
    tablePersonGroupsRemove = {},
    tableManagedGroupsAdd = {},
    tableManagedGroupsRemove = {};

var recordTemplate = {
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

labelTextCreate = "Create Person";
labelTextEdit = "Edit Person";
urlFillForEdit = "/PersonSrv/GetAllByIds";
urlEdit = "/PersonSrv/Edit";
urlDelete = "/PersonSrv/Delete";
urlRefreshMainView = "/PersonSrv/GetAll";

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//

    //Wire Up btnEditPrsProj 
    $("#btnEditPrsProj").click(function () {
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }

        if (currentIds.length === 1) {
            var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
            $("#prsProjViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else { 
            $("#prsProjViewPanel").text("_MULTIPLE_"); 
        }

        sddb.modalWaitWrapper(function () {
            return sddb.fillFormForRelatedGeneric(
                tableProjectsAdd, tableProjectsRemove, currentIds,
                "GET", "/PersonSrv/GetPersonProjects", { id: currentIds[0] },
                "GET", "/PersonSrv/GetPersonProjectsNot", { id: currentIds[0] },
                "GET", "/ProjectSrv/Get", { getActive: true });
        })
            .done(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView("mainView", "prsProjView", "tdo-btngroup-prsproj");
            });

    });

    //Wire Up btnEditPersonGroups 
    $("#btnEditPersonGroups").click(function () {
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }

        if (currentIds.length == 1) {
            var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
            $("#personGroupsViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else {
            $("#personGroupsViewPanel").text("_MULTIPLE_");
        }

        sddb.modalWaitWrapper(function () {
            return sddb.fillFormForRelatedGeneric(
                tablePersonGroupsAdd, tablePersonGroupsRemove, currentIds,
                "GET", "/PersonSrv/GetPersonGroups", { id: currentIds[0] },
                "GET", "/PersonSrv/GetPersonGroupsNot", { id: currentIds[0] },
                "GET", "/PersonGroupSrv/Get", { getActive: true });
        })
            .done(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView("mainView", "personGroupsView", "tdo-btngroup-prsgroups");
            });
    });

    //Wire Up btnEditManagedGroups 
    $("#btnEditManagedGroups").click(function () {
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }

        if (currentIds.length == 1) {
            var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
            $("#managedGroupsViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else {
            $("#managedGroupsViewPanel").text("_MULTIPLE_");
        }

        sddb.modalWaitWrapper(function () {
            return sddb.fillFormForRelatedGeneric(
                tableManagedGroupsAdd, tableManagedGroupsRemove, currentIds,
                "GET", "/PersonSrv/GetManagedGroups", { id: currentIds[0] },
                "GET", "/PersonSrv/GetManagedGroupsNot", { id: currentIds[0] },
                "GET", "/PersonGroupSrv/Get", { getActive: true });
        })
            .done(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView("mainView", "managedGroupsView", "tdo-btngroup-managedgroups");
            });
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = tableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) { sddb.showModalSelectOne(); }
        else {
            window.open("/PersonLogEntry?PersonId=" +
                tableMain.cell(".ui-selected", "Id:name", { page: "current"}).data());
        }
    });

    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1, 2],
        [3, 4, 5, 6, 7],
        [8, 9, 10, 11, 12, 13]
    ];
        
    //tableMain Persons
    tableMain = $("#tableMain").DataTable({
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
    sddb.showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //Enable DatePicker
    $("[data-val-dbisdateiso]").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });
            
    //----------------------------------------prsProjView----------------------------------------//

    //Wire Up prsProjViewBtnCancel
    $("#prsProjViewBtnCancel").click(function () {
        sddb.switchView("prsProjView", "mainView", "tdo-btngroup-main", tableMain);
    });

    //Wire Up prsProjViewBtnOk
    $("#prsProjViewBtnOk").click(function () {
        var idsAdd = tableProjectsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            idsRemove = tableProjectsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();

        if (idsAdd.length + idsRemove.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        sddb.modalWaitWrapper(function () {
            return sddb.submitEditsForRelatedGeneric(currentIds, idsAdd, idsRemove, "/PersonSrv/EditPersonProjects");
        })
            .then(function () {
                return sddb.refreshMainView();
            })
            .done(function () {
                sddb.switchView("prsProjView", "mainView", "tdo-btngroup-main", tableMain);
            });
    });

    //---------------------------------------DataTables------------

    //tableProjectsAdd
    tableProjectsAdd = $("#tableProjectsAdd").DataTable({
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

    //tableProjectsRemove
    tableProjectsRemove = $("#tableProjectsRemove").DataTable({
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

    //----------------------------------------personGroupsView----------------------------------------//

    //Wire Up personGroupsViewBtnCancel
    $("#personGroupsViewBtnCancel").click(function () {
        sddb.switchView("personGroupsView", "mainView", "tdo-btngroup-main", tableMain);
    });

    //Wire Up personGroupsViewBtnOk
    $("#personGroupsViewBtnOk").click(function () {
        var idsAdd = tablePersonGroupsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            idsRemove = tablePersonGroupsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();

        if (idsAdd.length + idsRemove.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }

        sddb.modalWaitWrapper(function () {
            return sddb.submitEditsForRelatedGeneric(currentIds, idsAdd, idsRemove, "/PersonSrv/EditPersonGroups");
        })
            .then(function () {
                return sddb.refreshMainView();
            })
            .done(function () {
                sddb.switchView("personGroupsView", "mainView", "tdo-btngroup-main", tableMain);
            });
    });
    
    //---------------------------------------DataTables------------

    //tablePersonGroupsAdd
    tablePersonGroupsAdd = $("#tablePersonGroupsAdd").DataTable({
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

    //tablePersonGroupsRemove
    tablePersonGroupsRemove = $("#tablePersonGroupsRemove").DataTable({
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

    //----------------------------------------managedGroupsView----------------------------------------//

    //Wire Up managedGroupsViewBtnCancel
    $("#managedGroupsViewBtnCancel").click(function () {
        sddb.switchView("managedGroupsView", "mainView", "tdo-btngroup-main", tableMain);
    });

    //Wire Up managedGroupsViewBtnOk
    $("#managedGroupsViewBtnOk").click(function () {
        var idsAdd = tableManagedGroupsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            idsRemove = tableManagedGroupsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();

        if (idsAdd.length + idsRemove.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }

        sddb.modalWaitWrapper(function () {
            return sddb.submitEditsForRelatedGeneric(currentIds, idsAdd, idsRemove, "/PersonSrv/EditManagedGroups");
        })
            .then(function () {
                return sddb.refreshMainView();
            })
            .done(function () {
                sddb.switchView("managedGroupsView", "mainView", "tdo-btngroup-main", tableMain);
            });
    });

    //---------------------------------------DataTables------------

    //tableManagedGroupsAdd
    tableManagedGroupsAdd = $("#tableManagedGroupsAdd").DataTable({
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

    //tableManagedGroupsRemove
    tableManagedGroupsRemove = $("#tableManagedGroupsRemove").DataTable({
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

    sddb.refreshMainView();
    sddb.switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//---------------------------------------Helper Methods--------------------------------------//



