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

var tableProjectPersonsAdd = {},
    tableProjectPersonsRemove = {};

var recordTemplate = {
    Id: "RecordTemplateId",
    ProjectName: null,
    ProjectAltName: null,
    ProjectCode: null,
    Comments: null,
    IsActive_bl: null,
    ProjectManager_Id: null
};

labelTextCreate = "Create Project";
labelTextEdit = "Edit Project";
urlFillForEdit = "/ProjectSrv/GetByIds";
urlEdit = "/ProjectSrv/Edit";
urlDelete = "/ProjectSrv/Delete";
urlRefreshMainView = "/ProjectSrv/Get";

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//
    
    //Wire Up btnEditProjectPersons
    $("#btnEditProjectPersons").click(function () {
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        if (currentIds.length == 1) {
            var selectedRecord = tableMain.row(".ui-selected", { page: "current" }).data();
            $("#projectPersonsViewPanel").text(selectedRecord.ProjectName + " " + selectedRecord.ProjectCode);
        }
        else { $("#projectPersonsViewPanel").text("_MULTIPLE_"); }

        sddb.showModalWait();
        sddb.fillFormForRelatedGeneric(tableProjectPersonsAdd, tableProjectPersonsRemove, currentIds,
                "GET", "/ProjectSrv/GetProjectPersons", { ids: currentIds },
                "GET", "/ProjectSrv/GetProjectPersonsNot", { ids: currentIds })
            .always(sddb.hideModalWait)
            .done(function () {
                sddb.saveViewSettings(tableMain);
                sddb.switchView("mainView", "projectPersonsView", "tdo-btngroup-projectpersons");
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });
    });
    
    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = tableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            sddb.showModalSelectOne();
            return;
        }
        window.open("/Location?ProjectId=" +
            tableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
    });

    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5]
    ];
    
    //tableMain Projects
    tableMain = $("#tableMain").DataTable({
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
    sddb.showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("ProjectManager_Id", "/PersonSrv/LookupAll");
    
    //----------------------------------------projectPersonsView----------------------------------------//

    //Wire Up projectPersonsViewBtnCancel
    $("#projectPersonsViewBtnCancel").click(function () {
        sddb.switchView("projectPersonsView", "mainView", "tdo-btngroup-main", tableMain);
    });

    //Wire Up projectPersonsViewBtnOk
    $("#projectPersonsViewBtnOk").click(function () {
        if (tableProjectPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            tableProjectPersonsRemove.rows(".ui-selected", { page: "current" }).data().length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        sddb.showModalWait();
        sddb.submitEditsForRelatedGeneric(
                currentIds,
                tableProjectPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                tableProjectPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/ProjectSrv/EditProjectPersons")
            .always(sddb.hideModalWait)
            .done(function () {
                sddb.refreshMainView()
                    .done(function () {
                        sddb.switchView("projectPersonsView", "mainView", "tdo-btngroup-main", tableMain);
                    });
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });
    });

    //---------------------------------------DataTables------------

    //tableProjectPersonsAdd
    tableProjectPersonsAdd = $("#tableProjectPersonsAdd").DataTable({
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
    tableProjectPersonsRemove = $("#tableProjectPersonsRemove").DataTable({
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

    sddb.refreshMainView();
    sddb.switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//---------------------------------------Helper Methods--------------------------------------//



