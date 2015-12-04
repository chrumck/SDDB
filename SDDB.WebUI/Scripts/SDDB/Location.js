/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var RecordTemplate = {
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
};

var MsFilterByProject = {};
var MsFilterByType = {};

LabelTextCreate = "Create Location";
LabelTextEdit = "Edit Location";
UrlFillForEdit = "/LocationSrv/GetByIds";
UrlEdit = "/LocationSrv/Edit";
UrlDelete = "/LocationSrv/Delete";

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//
            
    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) { showModalSelectOne(); }
        else {
            window.open("/AssemblyDb?LocationId=" +
                TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
        }
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/LocationTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByType
    $(MsFilterByType).on("selectionchange", function (e, m) { refreshMainView(); });

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
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11],
        [12, 13, 14, 15]
    ];

    //TableMain Locations
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LocName", name: "LocName" },//1
            //------------------------------------------------first set of columns
            { data: "LocAltName", name: "LocAltName" },//2
            { data: "LocAltName2", name: "LocAltName2" },//3
            {
                data: "LocationType_",
                name: "LocationType_",
                render: function (data, type, full, meta) { return data.LocTypeName; }
            }, //4
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode; }
            }, //5
            {
                data: "ContactPerson_",
                name: "ContactPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
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
            { targets: [4, 5, 6], className: "hidden-xs hidden-sm" },
            { targets: [6], className: "hidden-xs hidden-sm hidden-md" },
            //2nd set of columns - responsive
            { targets: [7, 8, 9, 10, 11], visible: false }, 
            { targets: [8, 9, 10], className: "hidden-xs hidden-sm" },
            { targets: [11], className: "hidden-xs hidden-sm hidden-md" },
            //3rd set of columns - responsive
            { targets: [12, 13, 14, 15], visible: false }, 
            { targets: [12, 13, 14], className: "hidden-xs hidden-sm" },
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }
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
    msAddToMsArray(MagicSuggests, "LocationType_Id", "/LocationTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ContactPerson_Id", "/PersonSrv/LookupAll", 1);
        
    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    var deferred0 = $.Deferred();

    TableMain.clear().search("").draw();

    if (MsFilterByType.getValue().length === 0 && MsFilterByProject.getValue().length === 0) {
        return deferred0.resolve();
    }
    refreshTblGenWrp(TableMain, "/LocationSrv/GetByAltIds",
        {
            projectIds: MsFilterByProject.getValue(),
            typeIds: MsFilterByType.getValue(),
            getActive: GetActive
        },
        "POST")
        .done(deferred0.resolve);

    return deferred0.promise();
}

//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() {
    var deferred0 = $.Deferred();
    if (typeof ProjectId !== "undefined" && ProjectId !== "") {
        showModalWait();
        $.ajax({
            type: "POST",
            url: "/ProjectSrv/GetByIds",
            timeout: 120000,
            data: { ids: [ProjectId], getActive: true },
            dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                msSetSelectionSilent(MsFilterByProject, 
                    [{ id: data[0].Id, name: data[0].ProjectName + " - " + data[0].ProjectCode }]);
                return deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { return deferred0.resolve(); }
    return deferred0.promise();
}

//---------------------------------------Helper Methods--------------------------------------//

