/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared_Views.js" />

"use strict";

//--------------------------------------Global Properties------------------------------------//

var recordTemplate = {
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
};

var msFilterByProject;
var msFilterByAssembly;
var msFilterByPerson;


labelTextCreate = "Create Log Entry";
labelTextEdit = "Edit Log Entry";
urlFillForEdit = "/AssemblyLogEntrySrv/GetByIds";
urlEdit = "/AssemblyLogEntrySrv/Edit";
urlDelete = "/AssemblyLogEntrySrv/Delete";

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//
    
    //Initialize DateTimePicker filterDateStart
    $("#filterDateStart").datetimepicker({ format: "YYYY-MM-DD" }).on("dp.hide", function (e) { sddb.refreshMainView(); });

    //Initialize DateTimePicker filterDateEnd
    $("#filterDateEnd").datetimepicker({ format: "YYYY-MM-DD" }).on("dp.hide", function (e) { sddb.refreshMainView(); });

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

    //Initialize MagicSuggest msFilterByAssembly
    msFilterByAssembly = $("#msFilterByAssembly").magicSuggest({
        data: "/AssemblyDbSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByAssembly
    $(msFilterByAssembly).on("selectionchange", function (e, m) { sddb.refreshMainView(); });

    //Initialize MagicSuggest msFilterByAssembly
    msFilterByAssyType = $("#msFilterByAssyType").magicSuggest({
        data: "/AssemblyTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByAssembly
    $(msFilterByAssyType).on("selectionchange", function (e, m) { sddb.refreshMainView(); });

    //Initialize MagicSuggest msFilterByPerson
    msFilterByPerson = $("#msFilterByPerson").magicSuggest({
        data: "/PersonSrv/LookupFromProject",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByPerson
    $(msFilterByPerson).on("selectionchange", function (e, m) { sddb.refreshMainView(); });
   
        
    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5],
        [6, 7, 8, 9, 10, 11],
        [12, 13, 14, 15, 16, 17]
    ];

    //tableMain PersonLogEntrys
    tableMain = $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            {
                data: "AssemblyDb_", name: "AssemblyDb_",
                render: function (data, type, full, meta) { return data.AssyName },
            }, //2
            {
                data: "LastSavedByPerson_", name: "LastSavedByPerson_",
                render: function (data, type, full, meta) { return data.LastName + " " + data.Initials },
            }, //3
            {
                data: "AssemblyStatus_", name: "AssemblyStatus_",
                render: function (data, type, full, meta) { return data.AssyStatusName },
            }, //4
            {
                data: "AssignedToLocation_", name: "AssignedToLocation_",
                render: function (data, type, full, meta) { return data.LocName + " - " + data.ProjectName },
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
    });
    //showing the first Set of columns on startup;
    sddb.showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //Initialize DateTimePicker
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Enable DateTimePicker
    $("#LastCalibrationDate").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    sddb.msAddToArray("AssemblyDb_Id", "/AssemblyDbSrv/Lookup");
    sddb.msAddToArray("AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup");
    sddb.msAddToArray("AssignedToLocation_Id", "/LocationSrv/Lookup");

    //--------------------------------------View Initialization------------------------------------//

    sddb.fillFiltersFromRequestParams().done(sddb.refreshMainView);
    sddb.switchView(initialViewId, mainViewId, mainViewBtnGroupClass);
    
    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh Main view 
sddb.refreshMainView = function () {
    tableMain.clear().search("").draw();
    if ($("#filterDateStart").val() == "" || $("#filterDateEnd").val() == "") { return $.Deferred().resolve(); }

    var endDate = ($("#filterDateEnd").val() == "") ? "" : moment($("#filterDateEnd").val())
        .hour(23).minute(59).format("YYYY-MM-DD HH:mm");

    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(tableMain, "/AssemblyLogEntrySrv/GetByAltIds",
        {
            projectIds: msFilterByProject.getValue(),
            assyIds: msFilterByAssembly.getValue(),
            assyTypeIds: msFilterByAssyType.getValue(),
            personIds: msFilterByPerson.getValue(),
            startDate: $("#filterDateStart").val(),
            endDate: endDate,
            getActive: currentActive
        },
        "POST");
    });
}

//fillFiltersFromRequestParams
sddb.fillFiltersFromRequestParams = function () {
    $("#filterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#filterDateEnd").val(moment().format("YYYY-MM-DD"));
    if (AssemblyId) {
        return sddb.modalWaitWrapper(function () {
            return $.ajax({ type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 120000, 
                    data: { ids: [AssemblyId], getActive: true }, dataType: "json" })
                .then(function (data) {
                    sddb.msSetSelectionSilent(msFilterByAssembly, [{ id: data[0].Id, name: data[0].AssyName, }]);
                });
        });
    }
    return $.Deferred().resolve();
}

//---------------------------------------Helper Methods--------------------------------------//


