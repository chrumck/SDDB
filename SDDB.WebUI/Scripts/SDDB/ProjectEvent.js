/*global sddb, ProjectId*/
/// <reference path="Shared_Views.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//

//setting up sddb
sddb.setConfig({
    recordTemplate: {
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
    },

    tableMainColumnSets: [
        [1],
        [2, 3, 4, 5, 6, 7, 8]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "EventName", name: "EventName" },//1
            //------------------------------------------------first set of columns
            { data: "EventAltName", name: "EventAltName" },//2
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.ProjectName; 
                }
            }, //3
            { data: "EventCreated", name: "EventCreated" },//4
            {
                data: "CreatedByPerson_",
                name: "CreatedByPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.Initials; 
                }
            },//5
            { data: "EventClosed", name: "EventClosed" },//6
            {
                data: "ClosedByPerson_",
                name: "ClosedByPerson_",
                render: function (data, type, full, meta) { 
                    "use strict";
                    return data.Initials; 
                }
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
    }),

    labelTextCreate: "Create Event",
    labelTextEdit: "Edit Event",
    urlFillForEdit: "/ProjectEventSrv/GetByIds",
    urlEdit: "/ProjectEventSrv/Edit",
    urlDelete: "/ProjectEventSrv/Delete",

    urlRefreshMainView: "/ProjectEventSrv/GetByProjectIds",
    dataRefreshMainView: function () {
        "use strict";
        return { projectIds: sddb.msFilterByProject.getValue(), getActive: sddb.cfg.currentActive };
    },
    httpTypeRefreshMainView: "POST"

});

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//

    //Initialize MagicSuggest sddb.msFilterByProject
    sddb.msFilterByProject = sddb.msSetFilter("msFilterByProject", "/ProjectSrv/Lookup");

    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup");
    sddb.msAddToArray("CreatedByPerson_Id", "/PersonSrv/LookupFromProject");
    sddb.msAddToArray("ClosedByPerson_Id", "/PersonSrv/LookupFromProject");

    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});



