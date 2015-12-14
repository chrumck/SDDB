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
    Component_Id: null,
    ComponentStatus_Id: null,
    AssignedToProject_Id: null,
    AssignedToAssemblyDb_Id: null,
    LastCalibrationDate: null,
    Comments: null,
    IsActive_bl: null
};

var msFilterByProject;
var msFilterByComponent;
var msFilterByPerson;


labelTextCreate = "Create Log Entry";
labelTextEdit = "Edit Log Entry";
urlFillForEdit = "/ComponentLogEntrySrv/GetByIds";
urlEdit = "/ComponentLogEntrySrv/Edit";
urlDelete = "/ComponentLogEntrySrv/Delete";

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

    //Initialize MagicSuggest msFilterByComponent
    msFilterByComponent = $("#msFilterByComponent").magicSuggest({
        data: "/ComponentSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByComponent
    $(msFilterByComponent).on("selectionchange", function (e, m) { sddb.refreshMainView(); });

    //Initialize MagicSuggest msFilterByComponent
    msFilterByCompType = $("#msFilterByCompType").magicSuggest({
        data: "/ComponentTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByComponent
    $(msFilterByCompType).on("selectionchange", function (e, m) { sddb.refreshMainView(); });

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
        [2, 3, 4, 5, 6, 7, 8]
    ];

    //tableMain PersonLogEntrys
    tableMain = $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            {
                data: "Component_", name: "Component_",
                render: function (data, type, full, meta) { return data.CompName }
            }, //2
            {
                data: "LastSavedByPerson_", name: "LastSavedByPerson_",
                render: function (data, type, full, meta) { return data.LastName + " " + data.Initials }
            }, //3
            {
                data: "ComponentStatus_", name: "ComponentStatus_",
                render: function (data, type, full, meta) { return data.CompStatusName }
            }, //4
            {
                data: "AssignedToProject_", name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }
            }, //5
            {
                data: "AssignedToAssemblyDb_", name: "AssignedToAssemblyDb_",
                render: function (data, type, full, meta) { return data.AssyName }
            }, //6
            { data: "LastCalibrationDate", name: "LastCalibrationDate" },//7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//9
            { data: "Component_Id", name: "Component_Id" },//10
            { data: "LastSavedByPerson_Id", name: "LastSavedByPerson_Id" },//11
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//12
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//13
            { data: "AssignedToAssemblyDb_Id", name: "AssignedToAssemblyDb_Id" }//14
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 1, 7, 9, 10, 11, 12, 13, 14], searchable: false },
            // - first set of columns
            { targets: [3, 4], className: "hidden-xs" },
            { targets: [5, 6], className: "hidden-xs hidden-sm" }, 
            { targets: [7, 8], className: "hidden-xs hidden-sm hidden-md" }
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
    sddb.msAddToArray("Component_Id", "/ComponentSrv/Lookup");
    sddb.msAddToArray("ComponentStatus_Id", "/ComponentStatusSrv/Lookup");
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup");
    sddb.msAddToArray("AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup");

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

    var endDate = ($("#filterDateEnd").val() == "") ? "": moment($("#filterDateEnd").val())
        .hour(23).minute(59).format("YYYY-MM-DD HH:mm");

    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(tableMain, "/ComponentLogEntrySrv/GetByAltIds",
        {
            projectIds: msFilterByProject.getValue(),
            componentIds: msFilterByComponent.getValue(),
            compTypeIds: msFilterByCompType.getValue(),
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
    if (ComponentId) {
        return sddb.modalWaitWrapper(function () {
            return $.ajax({type: "POST", url: "/ComponentSrv/GetByIds", timeout: 120000,
                    data: { ids: [ComponentId], getActive: true }, dataType: "json"})
                .then(function (data) {
                    sddb.msSetSelectionSilent(msFilterByComponent, [{ id: data[0].Id, name: data[0].CompName, }]);
                });
            });
    }
    return $.Deferred().resolve();
}

//---------------------------------------Helper Methods--------------------------------------//
