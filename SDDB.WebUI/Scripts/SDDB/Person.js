/*global sddb */
/// <reference path="Shared_Views.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//

//setting up sddb
sddb.setConfig({
    recordTemplate: {
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
    },

    tableMainColumnSets: [
        [1, 2],
        [3, 4, 5, 6, 7],
        [8, 9, 10, 11, 12, 13]
    ],

    tableMain: $("#tableMain").DataTable({
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
    }),

    labelTextCreate: "Create Person",
    labelTextEdit: "Edit Person",
    urlFillForEdit: "/PersonSrv/GetAllByIds",
    urlEdit: "/PersonSrv/Edit",
    urlDelete: "/PersonSrv/Delete",
    urlRefreshMainView: "/PersonSrv/GetAll"

});

sddb.callBackBeforeCopy = function () {
    "use strict";
    return sddb.showModalConfirm("NOTE: Person groups and projects are not copied.", "Confirm Copy");
};

//----------------------------------------------setup after page load------------------------------------------------//

$(document).ready(function () {
    "use strict";
    //--------------------------------------personProjectsCfg--------------------------------------//

    //tableProjectsAdd
    sddb.tableProjectsAdd = $("#tableProjectsAdd").DataTable({
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
    sddb.tableProjectsRemove = $("#tableProjectsRemove").DataTable({
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

    //personProjectsCfg
    sddb.personProjectsCfg = {
        tableAdd: sddb.tableProjectsAdd,
        tableRemove: sddb.tableProjectsRemove,
        url: "/PersonSrv/GetPersonProjects",
        urlNot: "/PersonSrv/GetPersonProjectsNot",
        relatedViewId: "prsProjView",
        relatedViewBtnGroupClass: "tdo-btngroup-prsproj",
        relatedViewPanelId: "prsProjViewPanel",
        relatedViewPanelText: function (selectedRecord) {
            return selectedRecord.FirstName + " " + selectedRecord.LastName;
        },
        urlEdit: "/PersonSrv/EditPersonProjects",
        btnEditId: "btnEditPrsProj",
        btnCancelId: "prsProjViewBtnCancel",
        btnOkId: "prsProjViewBtnOk"
    };

    sddb.wireButtonsForRelated(sddb.personProjectsCfg);

    //--------------------------------------personGroupsCfg--------------------------------------//

    //tablePersonGroupsAdd
    sddb.tablePersonGroupsAdd = $("#tablePersonGroupsAdd").DataTable({
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
    sddb.tablePersonGroupsRemove = $("#tablePersonGroupsRemove").DataTable({
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

    //personGroupsCfg
    sddb.personGroupsCfg = {
        tableAdd: sddb.tablePersonGroupsAdd,
        tableRemove: sddb.tablePersonGroupsRemove,
        url: "/PersonSrv/GetPersonGroups",
        urlNot: "/PersonSrv/GetPersonGroupsNot",
        relatedViewId: "personGroupsView",
        relatedViewBtnGroupClass: "tdo-btngroup-prsgroups",
        relatedViewPanelId: "personGroupsViewPanel",
        relatedViewPanelText: function (selectedRecord) {
            return selectedRecord.FirstName + " " + selectedRecord.LastName;
        },
        urlEdit: "/PersonSrv/EditPersonGroups",
        btnEditId: "btnEditPersonGroups",
        btnCancelId: "personGroupsViewBtnCancel",
        btnOkId: "personGroupsViewBtnOk"
    };

    sddb.wireButtonsForRelated(sddb.personGroupsCfg);

    //--------------------------------------managedGroupsCfg--------------------------------------//

    //tableManagedGroupsAdd
    sddb.tableManagedGroupsAdd = $("#tableManagedGroupsAdd").DataTable({
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
    sddb.tableManagedGroupsRemove = $("#tableManagedGroupsRemove").DataTable({
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

    //managedGroupsCfg
    sddb.managedGroupsCfg = {
        tableAdd: sddb.tableManagedGroupsAdd,
        tableRemove: sddb.tableManagedGroupsRemove,
        url: "/PersonSrv/GetManagedGroups",
        urlNot: "/PersonSrv/GetManagedGroupsNot",
        relatedViewId: "managedGroupsView",
        relatedViewBtnGroupClass: "tdo-btngroup-managedgroups",
        relatedViewPanelId: "managedGroupsViewPanel",
        relatedViewPanelText: function (selectedRecord) {
            return selectedRecord.FirstName + " " + selectedRecord.LastName;
        },
        urlEdit: "/PersonSrv/EditManagedGroups",
        btnEditId: "btnEditManagedGroups",
        btnCancelId: "managedGroupsViewBtnCancel",
        btnOkId: "managedGroupsViewBtnOk"
    };

    sddb.wireButtonsForRelated(sddb.managedGroupsCfg);
        
    //-----------------------------------------mainView------------------------------------------//

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        sddb.sendSingleIdToNewWindow("/PersonLogEntry?PersonId=");
    });

    //---------------------------------------editFormView----------------------------------------//
        

    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});

