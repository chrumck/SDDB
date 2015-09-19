/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared_Views.js" />

//--------------------------------------Global Properties------------------------------------//

var MsFilterByProject = {};
var MsFilterByType = {};
var MsFilterByLoc = {};
var RecordTemplate = {
    Id: "RecordTemplateId",
    AssyName: null,
    AssyAltName: null,
    AssyAltName2: null,
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
    AssyReadingIntervalSecs: null,
    IsReference_bl: null,
    TechnicalDetails: null,
    PowerSupplyDetails: null,
    HSEDetails: null,
    Comments: null,
    IsActive_bl: null,
    AssemblyType_Id: null,
    AssemblyStatus_Id: null,
    AssignedToLocation_Id: null
};

var LabelTextCreate = "Create Assembly";
var LabelTextEdit = "Edit Assembly";
var UrlEdit = "/AssemblyDbSrv/GetByIds";

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//
            
    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        showColumnSet(TableMainColumnSets, 1);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        showColumnSet(TableMainColumnSets, 2);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        showColumnSet(TableMainColumnSets, 3);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        showColumnSet(TableMainColumnSets, 4);
    });

    //wire up dropdownId5 - Show Assy Components
    $("#dropdownId5").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/Component?AssemblyId="
            + TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data())
    });

    //wire up dropdownId6 - Show Assy Log
    $("#dropdownId6").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/AssemblyLogEntry?AssemblyId="
            + TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data())
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/AssemblyTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByType
    $(MsFilterByType).on('selectionchange', function (e, m) { refreshMainView(); });

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
    $(MsFilterByProject).on('selectionchange', function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByLoc
    MsFilterByLoc = $("#MsFilterByLoc").magicSuggest({
        data: "/LocationSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: MsFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByLoc
    $(MsFilterByLoc).on('selectionchange', function (e, m) { refreshMainView(); });


    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6],
        [7, 8, 9, 10, 11, 12],
        [13, 14, 15, 16, 17, 18],
        [19, 20, 21, 22, 23]
    ];
        
    //TableMain AssemblyDbs
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" },//1
            //------------------------------------------------first set of columns
            { data: "AssyAltName", name: "AssyAltName" },//2
            { data: "AssyAltName2", name: "AssyAltName2" },//3
            { data: "AssemblyType_", render: function (data, type, full, meta) { return data.AssyTypeName }, name: "AssemblyType_" }, //4
            { data: "AssemblyStatus_", render: function (data, type, full, meta) { return data.AssyStatusName }, name: "AssemblyStatus_" }, //5
            { data: "AssignedToLocation_", render: function (data, type, full, meta) { return data.LocName + " - " + data.ProjectCode }, name: "AssignedToLocation" }, //6
            //------------------------------------------------second set of columns
            { data: "AssyGlobalX", name: "AssyGlobalX" },//7
            { data: "AssyGlobalY", name: "AssyGlobalY" },//8
            { data: "AssyGlobalZ", name: "AssyGlobalZ" },//9
            { data: "AssyLocalXDesign", name: "AssyLocalXDesign" },//10
            { data: "AssyLocalYDesign", name: "AssyLocalYDesign" },//11
            { data: "AssyLocalZDesign", name: "AssyLocalZDesign" },//12
            //------------------------------------------------third set of columns
            { data: "AssyLocalXAsBuilt", name: "AssyLocalXAsBuilt" },//13
            { data: "AssyLocalYAsBuilt", name: "AssyLocalYAsBuilt" },//14
            { data: "AssyLocalZAsBuilt", name: "AssyLocalZAsBuilt" },//15
            { data: "AssyStationing", name: "AssyStationing" },//16
            { data: "AssyLength", name: "AssyLength" },//17
            { data: "AssyReadingIntervalSecs", name: "AssyReadingIntervalSecs" },//18
            //------------------------------------------------Fourth set of columns
            { data: "IsReference_bl", name: "IsReference_bl" },//19
            { data: "TechnicalDetails", name: "TechnicalDetails" },//20
            { data: "PowerSupplyDetails", name: "PowerSupplyDetails" },//21
            { data: "HSEDetails", name: "HSEDetails" },//22
            { data: "Comments", name: "Comments" },//23
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//24
            { data: "AssemblyType_Id", name: "AssemblyType_Id" },//25
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//26
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" }//27
        ],
        columnDefs: [
            { targets: [0, 19, 24, 25, 26, 27], searchable: false },  //"orderable": false, "visible": false
            //first set of columns - responsive
            { targets: [2, 3], className: "hidden-xs" },
            { targets: [4, 6], className: "hidden-xs hidden-sm" }, 
            //second set of columns - responsive
            { targets: [8, 9], className: "hidden-xs" }, 
            { targets: [10, 11, 12], className: "hidden-xs hidden-sm" }, 
            //third set of columns - responsive
            { targets: [13, 14, 15, 17], className: "hidden-xs" }, 
            { targets: [18], className: "hidden-xs hidden-sm " }, 
            //fourth set of columns - responsive
            { targets: [19, 20, 21], className: "hidden-xs" },
            { targets: [22], className: "hidden-xs hidden-sm" } 
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

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "AssemblyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel").click(function () {
        switchView("EditFormView","MainView", "tdo-btngroup-main", true);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
            showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
            return;
        }
        showModalWait();
        var createMultiple = $("#CreateMultiple").val() != "" ? $("#CreateMultiple").val() : 1;
        submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/AssemblyDbSrv/Edit", createMultiple)
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("EditFormView", "MainView", "tdo-btngroup-main", true, TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
    });

    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);

    $("#InitialView").addClass("hidden");
    $("#MainView").removeClass("hidden");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/AssemblyDbSrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {
    var deferred0 = $.Deferred();

    TableMain.clear().search("").draw();

    if (MsFilterByType.getValue().length == 0 &&
        MsFilterByProject.getValue().length == 0 &&
        MsFilterByLoc.getValue().length == 0)
    {
        if (typeof AssemblyIds !== "undefined" && AssemblyIds != null && AssemblyIds.length > 0) {
            refreshTblGenWrp(TableMain, "/AssemblyDbSrv/GetByIds", { ids: AssemblyIds, getActive: GetActive }, "POST")
                .done(deferred0.resolve);
        }
        return deferred0.promise();
    }

    refreshTblGenWrp(TableMain, "/AssemblyDbSrv/GetByAltIds2",
        {
            projectIds: MsFilterByProject.getValue(),
            typeIds: MsFilterByType.getValue(),
            locIds: MsFilterByLoc.getValue(),
            getActive: GetActive
        },
        "POST")
        .done(deferred0.resolve);

    return deferred0.promise();
}

//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() {
    var deferred0 = $.Deferred();
    if (typeof LocationId !== "undefined" && LocationId != "") {
        showModalWait();
        $.ajax({
            type: "POST",
            url: "/LocationSrv/GetByIds",
            timeout: 120000,
            data: { ids: [LocationId], getActive: true },
            dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                msSetSelectionSilent(MsFilterByLoc, [{ id: data[0].Id, name: data[0].LocName }]);
                deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { deferred0.resolve(); }
    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//
