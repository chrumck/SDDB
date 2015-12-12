/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var recordTemplate = {
    Id: "RecordTemplateId",
    ActivityTypeName: null,
    ActivityTypeAltName: null,
    Comments: null,
    IsActive_bl: null
};

labelTextCreate = "Create Activity Type";
labelTextEdit = "Edit Activity Type";
urlFillForEdit = "/PersonActivityTypeSrv/GetByIds";
urlEdit = "/PersonActivityTypeSrv/Edit";
urlDelete = "/PersonActivityTypeSrv/Delete";
urlRefreshMainView = "/PersonActivityTypeSrv/Get";

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//
    
    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3]
    ];

    //tableMain PersonActivity Types
    tableMain = $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ActivityTypeName", name: "ActivityTypeName" },//1
            { data: "ActivityTypeAltName", name: "ActivityTypeAltName" },//2
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
    });
    //showing the first Set of columns on startup;
    showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //--------------------------------------View Initialization------------------------------------//

    refreshMainView();
    switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//---------------------------------------Helper Methods--------------------------------------//

