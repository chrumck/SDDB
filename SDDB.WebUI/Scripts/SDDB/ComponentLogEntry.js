/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared_Views.js" />

//--------------------------------------Global Properties------------------------------------//

var RecordTemplate = {
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

var MsFilterByProject;
var MsFilterByComponent;
var MsFilterByPerson;


LabelTextCreate = "Create Log Entry";
LabelTextEdit = "Edit Log Entry";
UrlFillForEdit = "/ComponentLogEntrySrv/GetByIds";
UrlEdit = "/ComponentLogEntrySrv/Edit";
UrlDelete = "/ComponentLogEntrySrv/Delete";

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" }).on("dp.hide", function (e) { refreshMainView(); });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" }).on("dp.hide", function (e) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByProject
    $(MsFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByComponent
    MsFilterByComponent = $("#MsFilterByComponent").magicSuggest({
        data: "/ComponentSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByComponent
    $(MsFilterByComponent).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/LookupFromProject",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByPerson
    $(MsFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });


    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6, 7, 8]
    ];

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
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
            //"orderable": false, "visible": false
            { targets: [0, 1, 7, 9, 10, 11, 12, 13, 14], searchable: false },
            // - first set of columns
            { targets: [3, 4], className: "hidden-xs" },
            { targets: [5, 6], className: "hidden-xs hidden-sm" }, 
            { targets: [7, 8], className: "hidden-xs hidden-sm hidden-md" }
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
    showColumnSet(TableMainColumnSets, 1);

    //---------------------------------------EditFormView----------------------------------------//

    //Initialize DateTimePicker
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Enable DateTimePicker
    $("#LastCalibrationDate").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "Component_Id", "/ComponentSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh Main view 
function refreshMainView() {
    var deferred0 = $.Deferred();

    TableMain.clear().search("").draw();

    if (($("#FilterDateStart").val() == "" || $("#FilterDateEnd").val() == "") &&
         (MsFilterByProject.getValue().length == 0 && MsFilterByComponent.getValue().length == 0 &&
                MsFilterByPerson.getValue().length == 0)
        ) { return deferred0.resolve(); }

    var endDate = ($("#FilterDateEnd").val() == "") ? "" : moment($("#FilterDateEnd").val())
        .hour(23).minute(59).format("YYYY-MM-DD HH:mm");

    refreshTblGenWrp(TableMain, "/ComponentLogEntrySrv/GetByAltIds",
        {
            projectIds: MsFilterByProject.getValue(),
            componentIds: MsFilterByComponent.getValue(),
            personIds: MsFilterByPerson.getValue(),
            startDate: $("#FilterDateStart").val(),
            endDate: endDate,
            getActive: GetActive
        },
        "POST")
        .done(deferred0.resolve);

    return deferred0.promise();
}

//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() {
    var deferred0 = $.Deferred();

    if (typeof ComponentId !== "undefined" && ComponentId != "") {
        showModalWait();
        $.ajax({
            type: "POST", url: "/ComponentSrv/GetByIds",
            timeout: 120000, data: { ids: [ComponentId], getActive: true }, dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                msSetSelectionSilent(MsFilterByComponent, [{ id: data[0].Id, name: data[0].CompName, }]);
                return deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else {
        $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
        $("#FilterDateEnd").val(moment().format("YYYY-MM-DD"));
        return deferred0.resolve();
    }

    return deferred0.promise();
}

//---------------------------------------Helper Methods--------------------------------------//


