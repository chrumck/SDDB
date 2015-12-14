/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

"use strict";

//--------------------------------------Global Properties------------------------------------//

var recordTemplate = {
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

var msFilterByProject = {};
var msFilterByType = {};

labelTextCreate = "Create Location";
labelTextEdit = "Edit Location";
urlFillForEdit = "/LocationSrv/GetByIds";
urlEdit = "/LocationSrv/Edit";
urlDelete = "/LocationSrv/Delete";

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//
            
    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        var noOfRows = tableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) { sddb.showModalSelectOne(); }
        else {
            window.open("/AssemblyDb?LocationId=" +
                tableMain.cell(".ui-selected", "Id:name", { page: "current" }).data());
        }
    });

    //Initialize MagicSuggest msFilterByType
    msFilterByType = $("#msFilterByType").magicSuggest({
        data: "/LocationTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByType
    $(msFilterByType).on("selectionchange", function (e, m) { sddb.refreshMainView(); });

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
    

    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11],
        [12, 13, 14, 15]
    ];

    //tableMain Locations
    tableMain = $("#tableMain").DataTable({
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
    sddb.showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //Initialize MagicSuggest Array
    sddb.msAddToArray("LocationType_Id", "/LocationTypeSrv/Lookup");
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup");
    sddb.msAddToArray("ContactPerson_Id", "/PersonSrv/LookupFromProject");
        
    //--------------------------------------View Initialization------------------------------------//

    sddb.fillFiltersFromRequestParams().done(sddb.refreshMainView);
    sddb.switchView(initialViewId, mainViewId, mainViewBtnGroupClass);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
sddb.refreshMainView = function () {
    tableMain.clear().search("").draw();
    if (msFilterByType.getValue().length === 0 && msFilterByProject.getValue().length === 0) {
        return $.Deferred().resolve();
    }
    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(tableMain, "/LocationSrv/GetByAltIds",
        {
            projectIds: msFilterByProject.getValue(),
            typeIds: msFilterByType.getValue(),
            getActive: currentActive
        },
        "POST");
    });
}

//fillFiltersFromRequestParams
sddb.fillFiltersFromRequestParams = function () {
    var deferred0 = $.Deferred();
    if (typeof ProjectId !== "undefined" && ProjectId !== "") {
        sddb.showModalWait();
        $.ajax({
            type: "POST",
            url: "/ProjectSrv/GetByIds",
            timeout: 120000,
            data: { ids: [ProjectId], getActive: true },
            dataType: "json"
        })
            .always(sddb.hideModalWait)
            .done(function (data) {
                sddb.msSetSelectionSilent(msFilterByProject, 
                    [{ id: data[0].Id, name: data[0].ProjectName + " - " + data[0].ProjectCode }]);
                return deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                sddb.showModalFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { return deferred0.resolve(); }
    return deferred0.promise();
}

//---------------------------------------Helper Methods--------------------------------------//

