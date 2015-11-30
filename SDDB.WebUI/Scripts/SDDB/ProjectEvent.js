/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var MsFilterByProject = {};

var RecordTemplate = {
    Id: "RecordTemplateId",
    EventName: null,
    EventAltName: null,
    EventCreated: null,
    EventClosed: null,
    Comments: null,
    IsActive_bl: null,
    AssignedToProject_Id: null,
    CreatedByPerson_Id: null,
    ClosedByPerson_Id: null
};

LabelTextCreate = "Create Event";
LabelTextEdit = "Edit Event";
UrlFillForEdit = "/ProjectEventSrv/GetByIds";
UrlEdit = "/ProjectEventSrv/Edit";
UrlDelete = "/ProjectEventSrv/Delete";

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Initialize MagicSuggest msFilterByProject
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


    //---------------------------------------DataTables------------
    
    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6, 7, 8]
    ];
    
    //TableMain ProjectEvents
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "EventName", name: "EventName" },//1
            //------------------------------------------------first set of columns
            { data: "EventAltName", name: "EventAltName" },//2
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName; }
            }, //3
            { data: "EventCreated", name: "EventCreated" },//4
            {
                data: "CreatedByPerson_",
                name: "CreatedByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            },//5
            { data: "EventClosed", name: "EventClosed" },//6
            {
                data: "ClosedByPerson_",
                name: "ClosedByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            }, //7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//9
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//10
            { data: "CreatedByPerson_Id", name: "CreatedByPerson_Id" },//11
            { data: "ClosedByPerson_Id", name: "ClosedByPerson_Id" }//12
        ],
        columnDefs: [
            //searchable: false
            { targets: [0, 9, 10, 11, 12], searchable: false },
            // - first set of columns
            { targets: [2, 4, 5], className: "hidden-xs hidden-sm" },
            { targets: [6, 7, 8], className: "hidden-xs hidden-sm hidden-md" }
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
    showColumnSet(TableMainColumnSets, 1);

    //---------------------------------------EditFormView----------------------------------------//

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "CreatedByPerson_Id", "/PersonSrv/LookupFromProject", 1);
    msAddToMsArray(MagicSuggests, "ClosedByPerson_Id", "/PersonSrv/LookupFromProject", 1);

    //Enable DateTimePicker
    $("[data-val-dbisdatetimeiso]").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //--------------------------------------View Initialization------------------------------------//

    refreshMainView();
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh Main view 
function refreshMainView() {
    var deferred0 = $.Deferred();
    refreshTblGenWrp(TableMain, "/ProjectEventSrv/GetByProjectIds",
        {
            projectIds: MsFilterByProject.getValue(),
            getActive: GetActive
        },
        "POST")
        .done(deferred0.resolve);
    return deferred0.promise();	
}


//---------------------------------------Helper Methods--------------------------------------//

