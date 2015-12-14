/*global sddb*/
/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//

//setting up sddb
sddb.setConfig({
    recordTemplate: {
        Id: "RecordTemplateId",
        AssyStatusName: null,
        AssyStatusAltName: null,
        Comments: null,
        IsActive_bl: null
    },

    tableMainColumnSets : [
        [1],
        [2, 3]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" }, //0
            { data: "AssyStatusName", name: "AssyStatusName" }, //1
            { data: "AssyStatusAltName", name: "AssyStatusAltName" }, //2
            { data: "Comments", name: "Comments" }, //3
            { data: "IsActive_bl", name: "IsActive_bl" } //4
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

    labelTextCreate: "Create Assembly Status",
    labelTextEdit: "Edit Assembly Status",
    urlFillForEdit: "/AssemblyStatusSrv/GetByIds",
    urlEdit: "/AssemblyStatusSrv/Edit",
    urlDelete: "/AssemblyStatusSrv/Delete",
    urlRefreshMainView: "/AssemblyStatusSrv/Get"

});


//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});
