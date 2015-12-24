/*global sddb, AssemblyId, ComponentIds */
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
        DocName: null,
        DocAltName: null,
        DocLastVersion: null,
        DocFilePath: null,
        Comments: null,
        IsActive_bl: null,
        DocumentType_Id: null,
        AuthorPerson_Id: null,
        ReviewerPerson_Id: null,
        AssignedToProject_Id: null,
        RelatesToAssyType_Id: null,
        RelatesToCompType_Id: null
    },

    tableMainColumnSets: [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "DocName", name: "DocName" },//1
            { data: "DocAltName", name: "DocAltName" },//2
            {
                data: "DocumentType_",
                name: "DocumentType_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.DocTypeName;
                }
            }, //3
            { data: "DocLastVersion", name: "DocLastVersion" },//4
            {
                data: "AuthorPerson_",
                name: "AuthorPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.Initials;
                }
            },//5
            {
                data: "ReviewerPerson_",
                name: "ReviewerPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.Initials;
                }
            }, //6
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.ProjectName;
                }
            }, //7
            {
                data: "RelatesToAssyType_",
                name: "RelatesToAssyType_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.AssyTypeName;
                }
            }, //8
            {
                data: "RelatesToCompType_",
                name: "RelatesToCompType_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.CompTypeName;
                }
            }, //9
            { data: "DocFilePath", name: "DocFilePath" },//10
            { data: "Comments", name: "Comments" },//11
            { data: "IsActive_bl", name: "IsActive_bl" },//12
            { data: "DocumentType_Id", name: "DocumentType_Id" },//13
            { data: "AuthorPerson_Id", name: "AuthorPerson_Id" },//14
            { data: "ReviewerPerson_Id", name: "ReviewerPerson_Id" },//15
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//16
            { data: "RelatesToAssyType_Id", name: "RelatesToAssyType_Id" },//17
            { data: "RelatesToCompType_Id", name: "RelatesToCompType_Id" }//18
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 12, 13, 14, 15, 16, 17, 18], searchable: false },
            //1st set of columns - responsive
            { targets: [2, 4, 5], className: "hidden-xs hidden-sm" },
            { targets: [6], className: "hidden-xs hidden-sm hidden-md" },
            //2nd set of columns - responsive
            { targets: [7, 8, 9, 10, 11], visible: false },
            { targets: [9, 10], className: "hidden-xs hidden-sm" },
            { targets: [11], className: "hidden-xs hidden-sm hidden-md" }
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

    labelTextCreate: "Create Document",
    labelTextEdit: "Edit Document",
    urlFillForEdit: "/DocumentSrv/GetByIds",
    urlEdit: "/DocumentSrv/Edit",
    urlDelete: "/DocumentSrv/Delete"

    
});

//refresh view after magicsuggest update
sddb.refreshMainView = function () {
    "use strict";
    sddb.cfg.tableMain.clear().search("").draw();
    if (sddb.msFilterByType.getValue().length === 0 && sddb.msFilterByProject.getValue().length === 0) {
        return $.Deferred().resolve();
    }
    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/DocumentSrv/GetByAltIds",
        {
            projectIds: sddb.msFilterByProject.getValue(),
            typeIds: sddb.msFilterByType.getValue(),
            getActive: sddb.cfg.currentActive
        },
        "POST");
    });
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//

    //Initialize MagicSuggest msFilterByType
    sddb.msFilterByType = sddb.msSetFilter("msFilterByType", "/DocumentTypeSrv/Lookup");

    //Initialize MagicSuggest sddb.msFilterByProject
    sddb.msFilterByProject = sddb.msSetFilter("msFilterByProject", "/ProjectSrv/Lookup");
    
    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("DocumentType_Id", "/DocumentTypeSrv/Lookup");
    sddb.msAddToArray("AuthorPerson_Id", "/PersonSrv/Lookup");
    sddb.msAddToArray("ReviewerPerson_Id", "/PersonSrv/Lookup");
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup");
    sddb.msAddToArray("RelatesToAssyType_Id", "/AssemblyTypeSrv/Lookup");
    sddb.msAddToArray("RelatesToCompType_Id", "/ComponentTypeSrv/Lookup");

    //--------------------------------------View Initialization------------------------------------//

    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});

