/*global sddb, ProjectId*/
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
        LocName: null,
        LocAltName: null,
        LocAltName2: null,
        Address: null,
        LocX: null,
        LocY: null,
        LocZ: null,
        LocStationing: null,
        CertOfApprReqd_bl: null,
        RightOfEntryReqd_bl: null,
        AccessInfo: null,
        Comments: null,
        IsActive_bl: null,
        LocationType_Id: null,
        AssignedToProject_Id: null,
        ContactPerson_Id: null
    },

    tableMainColumnSets: [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11],
        [12, 13, 14, 15]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LocName", name: "LocName" },//1
            //------------------------------------------------first set of columns
            { data: "LocAltName", name: "LocAltName" },//2
            { data: "LocAltName2", name: "LocAltName2" },//3
            {
                data: "LocationType_",
                name: "LocationType_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.LocTypeName;
                }
            }, //4
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.ProjectName + " " + data.ProjectCode;
                }
            }, //5
            {
                data: "ContactPerson_",
                name: "ContactPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.Initials;
                }
            },//6
            //------------------------------------------------second set of columns
            { data: "Address", name: "Address" },//7
            { data: "LocX", name: "LocX" },//8
            { data: "LocY", name: "LocY" },//9
            { data: "LocZ", name: "LocZ" },//10
            { data: "LocStationing", name: "LocStationing" },//11
            //------------------------------------------------third set of columns
            { data: "CertOfApprReqd_bl", name: "CertOfApprReqd_bl" },//12
            { data: "RightOfEntryReqd_bl", name: "RightOfEntryReqd_bl" },//13
            { data: "AccessInfo", name: "AccessInfo" },//14
            { data: "Comments", name: "Comments" },//15
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//16
            { data: "LocationType_Id", name: "LocationType_Id" },//17
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//18
            { data: "ContactPerson_Id", name: "ContactPerson_Id" }//19
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 12, 13, 16, 17, 18, 19], searchable: false },
            //1st set of columns - responsive
            { targets: [4], className: "hidden-xs" },
            { targets: [5, 6], className: "hidden-xs hidden-sm" },
            //2nd set of columns - responsive
            { targets: [8, 9, 10], className: "hidden-xs" },
            { targets: [11], className: "hidden-xs hidden-sm" },
            //3rd set of columns - responsive
            { targets: [12, 13, 14], className: "hidden-xs" },
            { targets: [], className: "hidden-xs hidden-sm" }
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

    labelTextCreate: "Create Location",
    labelTextEdit: "Edit Location",
    urlFillForEdit: "/LocationSrv/GetByIds",
    urlEdit: "/LocationSrv/Edit",
    urlDelete: "/LocationSrv/Delete"

});

//refresh view after magicsuggest update
sddb.refreshMainView = function () {
    "use strict";
    sddb.cfg.tableMain.clear().search("").draw();
    if (sddb.msFilterByType.getValue().length === 0 && sddb.msFilterByProject.getValue().length === 0) {
        return $.Deferred().resolve();
    }
    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/LocationSrv/GetByAltIds",
        {
            projectIds: sddb.msFilterByProject.getValue(),
            typeIds: sddb.msFilterByType.getValue(),
            getActive: sddb.cfg.currentActive
        },
        "POST");
    });
};

//fillFiltersFromRequestParams
sddb.fillFiltersFromRequestParams = function () {
    "use strict";
    if (!ProjectId) { return $.Deferred().resolve(); }

    return sddb.modalWaitWrapper(function () {
        return $.ajax({
            type: "POST",
            url: "/ProjectSrv/GetByIds",
            timeout: 120000,
            data: { ids: [ProjectId], getActive: true },
            dataType: "json"
        })
            .then(function (data) {
                if (!data || data.length === 0) { return; }
                sddb.msSetSelectionSilent(sddb.msFilterByProject,
                    [{ id: data[0].Id, name: data[0].ProjectName + " - " + data[0].ProjectCode }]);
            });
    });
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//
       
    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        sddb.sendSingleIdToNewWindow("/AssemblyDb?LocationId=");
    });

    //Initialize MagicSuggest msFilterByType
    sddb.msFilterByType = sddb.msSetFilter("msFilterByType", "/LocationTypeSrv/Lookup");

    //Initialize MagicSuggest sddb.msFilterByProject
    sddb.msFilterByProject = sddb.msSetFilter("msFilterByProject", "/ProjectSrv/Lookup");

    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("LocationType_Id", "/LocationTypeSrv/Lookup");
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup");
    sddb.msAddToArray("ContactPerson_Id", "/PersonSrv/LookupFromProject");

    //--------------------------------------View Initialization------------------------------------//

    sddb.fillFiltersFromRequestParams().done(sddb.refreshMainView);
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});

