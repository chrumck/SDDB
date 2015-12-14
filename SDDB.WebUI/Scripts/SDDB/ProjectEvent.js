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

var msFilterByProject = {};

var recordTemplate = {
    Id: "RecordTemplateId",
    EventName: null,
    EventAltName: null,
    EventCreated: null,
    EventClosed: null,
    Comments: null,
    IsActive_bl: null,
    AssignedToProject_Id: null,
    CreatedByPerson_Id: null,
    ClosedByPerson_Id: null
};

labelTextCreate = "Create Event";
labelTextEdit = "Edit Event";
urlFillForEdit = "/ProjectEventSrv/GetByIds";
urlEdit = "/ProjectEventSrv/Edit";
urlDelete = "/ProjectEventSrv/Delete";

var urlRefreshMainView = "/ProjectEventSrv/GetByProjectIds";
var dataRefreshMainView = function () { return { projectIds: msFilterByProject.getValue(), getActive: currentActive }; };
var httpTypeRefreshMainView = "POST";

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//

    //Initialize MagicSuggest msFilterByProject
    msFilterByProject = $("#msFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByProject
    $(msFilterByProject).on("selectionchange", function (e, m) { sddb.refreshMainView(); });


    //---------------------------------------DataTables------------
    
    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6, 7, 8]
    ];
    
    //tableMain ProjectEvents
    tableMain = $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "EventName", name: "EventName" },//1
            //------------------------------------------------first set of columns
            { data: "EventAltName", name: "EventAltName" },//2
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName; }
            }, //3
            { data: "EventCreated", name: "EventCreated" },//4
            {
                data: "CreatedByPerson_",
                name: "CreatedByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            },//5
            { data: "EventClosed", name: "EventClosed" },//6
            {
                data: "ClosedByPerson_",
                name: "ClosedByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            }, //7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//9
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//10
            { data: "CreatedByPerson_Id", name: "CreatedByPerson_Id" },//11
            { data: "ClosedByPerson_Id", name: "ClosedByPerson_Id" }//12
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 9, 10, 11, 12], searchable: false },
            // - first set of columns
            { targets: [2, 4, 5], className: "hidden-xs hidden-sm" },
            { targets: [6, 7, 8], className: "hidden-xs hidden-sm hidden-md" }
        ],
        order: [[1, "asc"]],
        bAutoWidth: false,
        lengthMenu: [10, 25, 50, 75, 100, 500, 1000, 5000],
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
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup");
    sddb.msAddToArray("CreatedByPerson_Id", "/PersonSrv/LookupFromProject");
    sddb.msAddToArray("ClosedByPerson_Id", "/PersonSrv/LookupFromProject");

    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//---------------------------------------Helper Methods--------------------------------------//

