/*global sddb */
/// <reference path="Shared_Views.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//

//setting up sddb
sddb.setConfig({
    recordTemplate: {
        Id: "RecordTemplateId",
        ProjectName: null,
        ProjectAltName: null,
        ProjectCode: null,
        Comments: null,
        IsActive_bl: null,
        ProjectManager_Id: null
    },

    tableMainColumnSets: [
        [1],
        [2, 3, 4, 5]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectAltName", name: "ProjectAltName" },//2
            { data: "ProjectCode", name: "ProjectCode" },//3
            {
                data: "ProjectManager_",
                name: "ProjectManager_",
                render: function (data, type, full, meta) { 
                    "use strict";
                    return data.Initials;
                }
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
    }),

    labelTextCreate: "Create Project",
    labelTextEdit: "Edit Project",
    urlFillForEdit: "/ProjectSrv/GetByIds",
    urlEdit: "/ProjectSrv/Edit",
    urlDelete: "/ProjectSrv/Delete",
    urlRefreshMainView: "/ProjectSrv/Get"

});

sddb.callBackBeforeCopy = function () {
    "use strict";
    return sddb.showModalConfirm("NOTE: Project people are not copied.", "Confirm Copy");
};

//----------------------------------------------setup after page load------------------------------------------------//

$(document).ready(function () {
    "use strict";
    //--------------------------------------projectPersonsCfg--------------------------------------//

    //tableProjectPersonsAdd
    sddb.tableProjectPersonsAdd = $("#tableProjectPersonsAdd").DataTable({
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

    //tableProjectPersonsRemove
    sddb.tableProjectPersonsRemove = $("#tableProjectPersonsRemove").DataTable({
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

    //projectPersonsCfg
    sddb.projectPersonsCfg = {
        tableAdd: sddb.tableProjectPersonsAdd,
        tableRemove: sddb.tableProjectPersonsRemove,
        url: "/ProjectSrv/GetProjectPersons",
        urlNot: "/ProjectSrv/GetProjectPersonsNot",
        relatedViewId: "projectPersonsView",
        relatedViewBtnGroupClass: "tdo-btngroup-projectpersons",
        relatedViewPanelId: "projectPersonsViewPanel",
        relatedViewPanelText: function (selectedRecord) {
            return selectedRecord.ProjectName + " " + selectedRecord.ProjectCode;
        },
        urlEdit: "/ProjectSrv/EditProjectPersons",
        btnEditId: "btnEditProjectPersons",
        btnCancelId: "projectPersonsViewBtnCancel",
        btnOkId: "projectPersonsViewBtnOk"
    };

    sddb.wireButtonsForRelated(sddb.projectPersonsCfg);
    
    //-----------------------------------------mainView------------------------------------------//

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        sddb.sendSingleIdToNewWindow("/Location?ProjectId=");
    });

    //---------------------------------------editFormView----------------------------------------//

    sddb.msAddToArray("ProjectManager_Id", "/PersonSrv/LookupAll");

    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});


