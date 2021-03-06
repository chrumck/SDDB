﻿/*global sddb*/
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
    recordTemplate : {
        Id: "RecordTemplateId",
        AssyTypeName: null,
        AssyTypeAltName: null,
        Comments: null,
        IsActive_bl: null,
        Attr01Type: null,
        Attr01Desc: null,
        Attr02Type: null,
        Attr02Desc: null,
        Attr03Type: null,
        Attr03Desc: null,
        Attr04Type: null,
        Attr04Desc: null,
        Attr05Type: null,
        Attr05Desc: null,
        Attr06Type: null,
        Attr06Desc: null,
        Attr07Type: null,
        Attr07Desc: null,
        Attr08Type: null,
        Attr08Desc: null,
        Attr09Type: null,
        Attr09Desc: null,
        Attr10Type: null,
        Attr10Desc: null,
        Attr11Type: null,
        Attr11Desc: null,
        Attr12Type: null,
        Attr12Desc: null,
        Attr13Type: null,
        Attr13Desc: null,
        Attr14Type: null,
        Attr14Desc: null,
        Attr15Type: null,
        Attr15Desc: null
    },

    tableMainColumnSets: [
        [1],
        [2, 3],
        [4, 5, 6, 7, 8, 9],
        [10, 11, 12, 13, 14, 15],
        [16, 17, 18, 19, 20, 21],
        [22, 23, 24, 25, 26, 27],
        [28, 29, 30, 31, 32, 33]
    ],
    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyTypeName", name: "AssyTypeName" },//1
            //------------------------------------------------first set of columns
            { data: "AssyTypeAltName", name: "AssyTypeAltName" },//2
            { data: "Comments", name: "Comments" },//3
            //------------------------------------------------second set of columns
            { data: "Attr01Type", name: "Attr01Type" },//4
            { data: "Attr01Desc", name: "Attr01Desc" },//5
            { data: "Attr02Type", name: "Attr02Type" },//6
            { data: "Attr02Desc", name: "Attr02Desc" },//7
            { data: "Attr03Type", name: "Attr03Type" },//8
            { data: "Attr03Desc", name: "Attr03Desc" },//9
            //------------------------------------------------third set of columns
            { data: "Attr04Type", name: "Attr04Type" },//10
            { data: "Attr04Desc", name: "Attr04Desc" },//11
            { data: "Attr05Type", name: "Attr05Type" },//12
            { data: "Attr05Desc", name: "Attr05Desc" },//13
            { data: "Attr06Type", name: "Attr06Type" },//14
            { data: "Attr06Desc", name: "Attr06Desc" },//15
            //------------------------------------------------fourth set of columns
            { data: "Attr07Type", name: "Attr07Type" },//16
            { data: "Attr07Desc", name: "Attr07Desc" },//17
            { data: "Attr08Type", name: "Attr08Type" },//18
            { data: "Attr08Desc", name: "Attr08Desc" },//19
            { data: "Attr09Type", name: "Attr09Type" },//20
            { data: "Attr09Desc", name: "Attr09Desc" },//21
            //------------------------------------------------fifth set of columns
            { data: "Attr10Type", name: "Attr10Type" },//22
            { data: "Attr10Desc", name: "Attr10Desc" },//23
            { data: "Attr11Type", name: "Attr11Type" },//24
            { data: "Attr11Desc", name: "Attr11Desc" },//25
            { data: "Attr12Type", name: "Attr12Type" },//26
            { data: "Attr12Desc", name: "Attr12Desc" },//27
            //------------------------------------------------sixth set of columns
            { data: "Attr13Type", name: "Attr13Type" },//28
            { data: "Attr13Desc", name: "Attr13Desc" },//29
            { data: "Attr14Type", name: "Attr14Type" },//30
            { data: "Attr14Desc", name: "Attr14Desc" },//31
            { data: "Attr15Type", name: "Attr15Type" },//32
            { data: "Attr15Desc", name: "Attr15Desc" },//33
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" }//34
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34], searchable: false },
            //first set of columns - responsive
            { targets: [3], className: "hidden-xs" }, 
            //second set of columns - responsive
            { targets: [6, 7], className: "hidden-xs" }, 
            { targets: [8, 9], className: "hidden-xs hidden-sm" }, 
            //third set of columns - responsive
            { targets: [12, 13], className: "hidden-xs" },
            { targets: [14, 15], className: "hidden-xs hidden-sm" },
            //fourth set of columns - responsive
            { targets: [18, 19], className: "hidden-xs" },
            { targets: [20, 21], className: "hidden-xs hidden-sm" },
            //fifth set of columns - responsive
            { targets: [24, 25], className: "hidden-xs" },
            { targets: [26, 27], className: "hidden-xs hidden-sm" },
            //sixth set of columns - responsive
            { targets: [30, 31], className: "hidden-xs" },
            { targets: [32, 33], className: "hidden-xs hidden-sm" }
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

    labelTextCreate: "Create Assembly Type",
    labelTextEdit: "Edit Assembly Type",
    urlFillForEdit: "/AssemblyTypeSrv/GetByIds",
    urlEdit: "/AssemblyTypeSrv/Edit",
    urlDelete: "/AssemblyTypeSrv/Delete",
    urlRefreshMainView: "/AssemblyTypeSrv/Get"

});

//callBackAfterCreate
sddb.callBackAfterCreate = function () {
    "use strict";
    $("#editForm select").find("option:first").prop("selected", "selected");
    return $.Deferred().resolve();
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //--------------------------------------View Initialization------------------------------------//

    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});

