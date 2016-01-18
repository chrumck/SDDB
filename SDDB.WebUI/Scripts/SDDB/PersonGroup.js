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
        PrsGroupName: null,
        PrsGroupAltName: null,
        Comments: null,
        IsActive_bl: null
    },

    tableMainColumnSets: [
        [1],
        [2, 3]
    ],

    tableMain: $("#tableMain").DataTable({
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
    }),

    labelTextCreate: "Create Group",
    labelTextEdit: "Edit Group",
    urlFillForEdit: "/PersonGroupSrv/GetByIds",
    urlEdit: "/PersonGroupSrv/Edit",
    urlDelete: "/PersonGroupSrv/Delete",
    urlRefreshMainView: "/PersonGroupSrv/Get"

});

sddb.callBackBeforeCopy = function () {
    "use strict";
    return sddb.showModalConfirm("NOTE: Group People are not copied.", "Confirm Copy");
};

//----------------------------------------------setup after page load------------------------------------------------//

$(document).ready(function () {
    "use strict";
    //--------------------------------------groupPersonsCfg--------------------------------------//

    //tableGroupPersonsAdd
    sddb.tableGroupPersonsAdd = $("#tableGroupPersonsAdd").DataTable({
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
    sddb.tableGroupPersonsRemove = $("#tableGroupPersonsRemove").DataTable({
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

    //groupPersonsCfg
    sddb.groupPersonsCfg = {
        tableAdd: sddb.tableGroupPersonsAdd,
        tableRemove: sddb.tableGroupPersonsRemove,
        url: "/PersonGroupSrv/GetGroupPersons",
        urlNot: "/PersonGroupSrv/GetGroupPersonsNot",
        relatedViewId: "groupPersonsView",
        relatedViewBtnGroupClass: "tdo-btngroup-grouppersons",
        relatedViewPanelId: "groupPersonsViewPanel",
        relatedViewPanelText: function (selectedRecord) { return selectedRecord.PrsGroupName; },
        urlEdit: "/PersonGroupSrv/EditGroupPersons",
        btnEditId: "btnEditGroupPersons",
        btnCancelId: "groupPersonsViewBtnCancel",
        btnOkId: "groupPersonsViewBtnOk"
    };

    sddb.wireButtonsForRelated(sddb.groupPersonsCfg);

    //--------------------------------------groupManagersCfg--------------------------------------//

    //tableGroupManagersAdd
    sddb.tableGroupManagersAdd = $("#tableGroupManagersAdd").DataTable({
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
    sddb.tableGroupManagersRemove = $("#tableGroupManagersRemove").DataTable({
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

    //groupManagersCfg
    sddb.groupManagersCfg = {
        tableAdd: sddb.tableGroupManagersAdd,
        tableRemove: sddb.tableGroupManagersRemove,
        url: "/PersonGroupSrv/GetGroupManagers",
        urlNot: "/PersonGroupSrv/GetGroupManagersNot",
        relatedViewId: "groupManagersView",
        relatedViewBtnGroupClass: "tdo-btngroup-groupmanagers",
        relatedViewPanelId: "groupManagersViewPanel",
        relatedViewPanelText: function (selectedRecord) { return selectedRecord.PrsGroupName; },
        urlEdit: "/PersonGroupSrv/EditGroupManagers",
        btnEditId: "btnEditGroupManagers",
        btnCancelId: "groupManagersViewBtnCancel",
        btnOkId: "groupManagersViewBtnOk"
    };

    sddb.wireButtonsForRelated(sddb.groupManagersCfg);

    //-----------------------------------------mainView------------------------------------------//


    //---------------------------------------editFormView----------------------------------------//


    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});

