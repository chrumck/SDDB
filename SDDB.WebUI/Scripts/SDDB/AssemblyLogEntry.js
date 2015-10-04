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

var MsFilterByProject;
var MsFilterByAssembly;
var MsFilterByPerson;


LabelTextCreate = "Create Log Entry";
LabelTextEdit = "Edit Log Entry";
UrlFillForEdit = "/AssemblyLogEntrySrv/GetByIds";
UrlEdit = "/AssemblyLogEntrySrv/Edit";
UrlDelete = "/AssemblyLogEntrySrv/Delete";

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

    //Initialize MagicSuggest MsFilterByAssembly
    MsFilterByAssembly = $("#MsFilterByAssembly").magicSuggest({
        data: "/AssemblyDbSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByAssembly
    $(MsFilterByAssembly).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
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
        [2, 3, 4, 5],
        [6, 7, 8, 9, 10, 11],
        [12, 13, 14, 15, 16, 17],
    ];

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
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
            //"orderable": false, "visible": false
            { targets: [0, 1, 18, 19, 20, 21, 22], searchable: false },
            // - first set of columns
            { targets: [3, 4], className: "hidden-xs" },
            { targets: [5], className: "hidden-xs hidden-sm" },
            // - second set of columns
            { targets: [7, 8], className: "hidden-xs" },
            { targets: [9, 10, 11], className: "hidden-xs hidden-sm" },
            // - third set of columns
            { targets: [12, 13, 14], className: "hidden-xs" }, 
            { targets: [16, 17], className: "hidden-xs hidden-sm" },
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
    msAddToMsArray(MagicSuggests, "AssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/Lookup", 1);

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

    if ( ($("#FilterDateStart").val() == "" || $("#FilterDateEnd").val() == "") &&
         (MsFilterByProject.getValue().length == 0 && MsFilterByAssembly.getValue().length == 0 &&
                MsFilterByPerson.getValue().length == 0)
        ) { return deferred0.resolve(); }

    var endDate = ($("#FilterDateEnd").val() == "") ? "" : moment($("#FilterDateEnd").val())
        .hour(23).minute(59).format("YYYY-MM-DD HH:mm");

    refreshTblGenWrp(TableMain, "/AssemblyLogEntrySrv/GetByAltIds",
        {
            projectIds: MsFilterByProject.getValue(),
            assyIds: MsFilterByAssembly.getValue(),
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

    if (typeof AssemblyId !== "undefined" && AssemblyId != "") {
        showModalWait();
        $.ajax({
            type: "POST", url: "/AssemblyDbSrv/GetByIds",
            timeout: 120000, data: { ids: [AssemblyId], getActive: true }, dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                msSetSelectionSilent(MsFilterByAssembly, [{ id: data[0].Id, name: data[0].AssyName, }]);
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


