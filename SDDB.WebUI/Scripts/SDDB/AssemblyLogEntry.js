/*global sddb, AssemblyId */
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
    recordTemplate : {
        Id: "RecordTemplateId",
        LogEntryDateTime: null,
        AssemblyDb_Id: null,
        AssemblyStatus_Id: null,
        AssignedToLocation_Id: null,
        AssyGlobalX: null,
        AssyGlobalY: null,
        AssyGlobalZ: null,
        AssyLocalXDesign: null,
        AssyLocalYDesign: null,
        AssyLocalZDesign: null,
        AssyLocalXAsBuilt: null,
        AssyLocalYAsBuilt: null,
        AssyLocalZAsBuilt: null,
        AssyStationing: null,
        AssyLength: null,
        Comments: null,
        IsActive_bl: null
    },

    tableMainColumnSets : [
        [1],
        [2, 3, 4, 5],
        [6, 7, 8, 9, 10, 11],
        [12, 13, 14, 15, 16, 17]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            {
                data: "AssemblyDb_",
                name: "AssemblyDb_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.AssyName;
                }
            }, //2
            {
                data: "LastSavedByPerson_",
                name: "LastSavedByPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.LastName + " " + data.Initials;
                }
            }, //3
            {
                data: "AssemblyStatus_",
                name: "AssemblyStatus_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.AssyStatusName;
                }
            }, //4
            {
                data: "AssignedToLocation_",
                name: "AssignedToLocation_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.LocName + " - " + data.ProjectName;
                }
            }, //5
            //------------------------------------------------second set of columns
            { data: "AssyGlobalX", name: "AssyGlobalX" },//6
            { data: "AssyGlobalY", name: "AssyGlobalY" },//7
            { data: "AssyGlobalZ", name: "AssyGlobalZ" },//8
            { data: "AssyLocalXDesign", name: "AssyLocalXDesign" },//9
            { data: "AssyLocalYDesign", name: "AssyLocalYDesign" },//10
            { data: "AssyLocalZDesign", name: "AssyLocalZDesign" },//11
            //------------------------------------------------third set of columns
            { data: "AssyLocalXAsBuilt", name: "AssyLocalXAsBuilt" },//12
            { data: "AssyLocalYAsBuilt", name: "AssyLocalYAsBuilt" },//13
            { data: "AssyLocalZAsBuilt", name: "AssyLocalZAsBuilt" },//14
            { data: "AssyStationing", name: "AssyStationing" },//15
            { data: "AssyLength", name: "AssyLength" },//16
            { data: "Comments", name: "Comments" },//17
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//18
            { data: "AssemblyDb_Id", name: "AssemblyDb_Id" },//19
            { data: "LastSavedByPerson_Id", name: "LastSavedByPerson_Id" },//20
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//21
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" }//22
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 1, 18, 19, 20, 21, 22], searchable: false },
            // - first set of columns
            { targets: [3, 4], className: "hidden-xs" },
            { targets: [5], className: "hidden-xs hidden-sm" },
            // - second set of columns
            { targets: [7, 8], className: "hidden-xs" },
            { targets: [9, 10, 11], className: "hidden-xs hidden-sm" },
            // - third set of columns
            { targets: [12, 13, 14], className: "hidden-xs" },
            { targets: [16, 17], className: "hidden-xs hidden-sm" }
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


    labelTextCreate: "Create Log Entry",
    labelTextEdit: "Edit Log Entry",
    urlFillForEdit: "/AssemblyLogEntrySrv/GetByIds",
    urlEdit: "/AssemblyLogEntrySrv/Edit",
    urlDelete: "/AssemblyLogEntrySrv/Delete"

});

//fillFiltersFromRequestParams
sddb.fillFiltersFromRequestParams = function () {
    "use strict";

    $("#filterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#filterDateEnd").val(moment().format("YYYY-MM-DD"));
    if (AssemblyId) {
        return sddb.modalWaitWrapper(function () {
            return $.ajax({
                type: "POST",
                url: "/AssemblyDbSrv/GetByIds",
                timeout: 120000,
                data: { ids: [AssemblyId], getActive: true },
                dataType: "json"
            })
                .then(function (data) {
                    sddb.msSetSelectionSilent(sddb.msFilterByAssembly, [{ id: data[0].Id, name: data[0].AssyName }]);
                });
        });
    }
    return $.Deferred().resolve();
};

//refresh Main view 
sddb.refreshMainView = function () {
    "use strict";

    sddb.cfg.tableMain.clear().search("").draw();
    if ($("#filterDateStart").val() === "" || $("#filterDateEnd").val() === "") { return $.Deferred().resolve(); }

    var endDate = ($("#filterDateEnd").val() === "") ? "" : moment($("#filterDateEnd").val())
        .hour(23).minute(59).format("YYYY-MM-DD HH:mm");

    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/AssemblyLogEntrySrv/GetByAltIds",
        {
            projectIds: sddb.msFilterByProject.getValue(),
            assyIds: sddb.msFilterByAssembly.getValue(),
            assyTypeIds: sddb.msFilterByAssyType.getValue(),
            personIds: sddb.msFilterByPerson.getValue(),
            startDate: $("#filterDateStart").val(),
            endDate: endDate,
            getActive: sddb.cfg.currentActive
        },
        "POST");
    });
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";

    //-----------------------------------------mainView------------------------------------------//
    
    //filterDateStart event dp.hide
    $("#filterDateStart").on("dp.hide", function (e) { sddb.refreshMainView(); });

    //filterDateEnd event dp.hide
    $("#filterDateEnd").on("dp.hide", function (e) { sddb.refreshMainView(); });

    //Initialize MagicSuggest sddb.msFilterByProject
    sddb.msFilterByProject = sddb.msSetFilter("msFilterByProject", "/ProjectSrv/Lookup");
    
    //Initialize MagicSuggest sddb.msFilterByAssembly
    sddb.msFilterByAssembly = sddb.msSetFilter("msFilterByAssembly", "/AssemblyDbSrv/Lookup");
    
    //Initialize MagicSuggest sddb.msFilterByAssyType
    sddb.msFilterByAssyType = sddb.msSetFilter("msFilterByAssyType", "/AssemblyTypeSrv/Lookup");
    
    //Initialize MagicSuggest sddb.msFilterByPerson
    sddb.msFilterByPerson = sddb.msSetFilter("msFilterByPerson", "/PersonSrv/LookupFromProject");
    
    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("AssemblyDb_Id", "/AssemblyDbSrv/Lookup");
    sddb.msAddToArray("AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup");
    sddb.msAddToArray("AssignedToLocation_Id", "/LocationSrv/Lookup");

    //--------------------------------------View Initialization------------------------------------//

    sddb.fillFiltersFromRequestParams().done(sddb.refreshMainView);
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});


