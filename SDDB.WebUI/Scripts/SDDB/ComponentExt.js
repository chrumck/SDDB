/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain;
var MagicSuggests = [];
var MsFilterByProject = {};
var MsFilterByModel = {};

var RecordTemplate = {
    Id: "RecordTemplateId",
    CompName: null,
    CompAltName: null,
    Attr01: null,
    Attr02: null,
    Attr03: null,
    Attr04: null,
    Attr05: null,
    Attr06: null,
    Attr07: null,
    Attr08: null,
    Attr09: null,
    Attr10: null,
    Attr11: null,
    Attr12: null,
    Attr13: null,
    Attr14: null,
    Attr15: null,
    ComponentType_Id: null,
    ComponentStatus_Id: null,
    AssignedToProject_Id: null,
};
var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

var DatePickers = [];

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else {
            showModalWait();
            fillFormForEditGeneric(CurrIds, "POST", "/ComponentSrv/GetByIds", GetActive,
                    "EditForm", null, MagicSuggests)
                .always(hideModalWait)
                .done(function (currRecords) {
                    CurrRecords = currRecords;
                    msDisableAll(MagicSuggests);
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(true);
        TableMain.columns([6, 7, 8, 9, 10]).visible(false);
        TableMain.columns([11, 12, 13, 14, 15]).visible(false);
        TableMain.columns([16, 17, 18, 19, 20]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(true);
        TableMain.columns([11, 12, 13, 14, 15]).visible(false);
        TableMain.columns([16, 17, 18, 19, 20]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(false);
        TableMain.columns([11, 12, 13, 14, 15]).visible(true);
        TableMain.columns([16, 17, 18, 19, 20]).visible(false);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(false);
        TableMain.columns([11, 12, 13, 14, 15]).visible(false);
        TableMain.columns([16, 17, 18, 19, 20]).visible(true);
    });

    //wire up dropdownId5
    $("#dropdownId5").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) {
            showModalSelectOne();
            return;
        }
        window.open("/ComponentLogEntry?ComponentId=" +
            TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data())
    });

    //wire up dropdownId6
    $("#dropdownId6").click(function (event) {
        event.preventDefault();
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        var newWindowName = moment().format("YYYYDDMMHHmmss");
        window.open("about:blank", newWindowName);
        submitFormFromArray("POST", "/Component", newWindowName, CurrIds, "ComponentIds");
    });

    //Initialize MagicSuggest MsFilterByModel
    MsFilterByModel = $("#MsFilterByModel").magicSuggest({
        data: "/ComponentModelSrv/Lookup",
        maxSelection: 1,
        required: true,
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByModel
    $(MsFilterByModel).on('selectionchange', function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest msFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        disabled: true,
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


    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) {
            GetActive = true;
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        } else {
            GetActive = false;
            $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        }
        refreshMainView();
    });

    //TableMain ComponentExts
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompName", name: "CompName" },//1
            //------------------------------------------------first set of columns
            { data: "CompAltName", name: "CompAltName" },//2
            { data: "ComponentType_", 
                render: function (data, type, full, meta) { return data.CompTypeName }, 
                name: "ComponentType_" }, //3
            { data: "ComponentStatus_", 
                render: function (data, type, full, meta) { return data.CompStatusName }, 
                name: "ComponentStatus_" }, //4
            { data: "AssignedToProject_", 
                render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode },
                name: "AssignedToProject_" }, //5
            //------------------------------------------------second set of columns
            { data: "Attr01", name: "Attr01" },//6
            { data: "Attr02", name: "Attr02" },//7
            { data: "Attr03", name: "Attr03" },//8
            { data: "Attr04", name: "Attr04" },//9
            { data: "Attr05", name: "Attr05" },//10
            //------------------------------------------------third set of columns
            { data: "Attr06", name: "Attr06" },//11
            { data: "Attr07", name: "Attr07" },//12
            { data: "Attr08", name: "Attr08" },//13
            { data: "Attr09", name: "Attr09" },//14
            { data: "Attr10", name: "Attr10" },//15
            //------------------------------------------------fourth set of columns
            { data: "Attr11", name: "Attr11" },//16
            { data: "Attr12", name: "Attr12" },//17
            { data: "Attr13", name: "Attr13" },//18
            { data: "Attr14", name: "Attr14" },//19
            { data: "Attr15", name: "Attr15" },//20
            //------------------------------------------------never visible
            { data: "ComponentType_Id", name: "ComponentType_Id" },//21
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//22
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//23
            { data: "ComponentModel_Id", name: "ComponentModel_Id" },//24
            { data: "ComponentModel_",
                render: function (data, type, full, meta) { return data.CompModelName },
                name: "ComponentModel_" }, //25
        ],
        columnDefs: [
            { targets: [0, 21, 22, 23, 24, 25], visible: false }, // - never show
            { targets: [0, 21, 22, 23, 24, 25], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 3, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [6, 7, 8, 9, 10], visible: false }, // - second set of columns - to toggle with options
            { targets: [7, 8], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [9, 10], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [11, 12, 13, 14, 15], visible: false }, // - third set of columns - to toggle with options
            { targets: [12, 13], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [14, 15], className: "hidden-xs hidden-sm hidden-md" }, // - third set of columns

            { targets: [16, 17, 18, 19, 20], visible: false }, // - fourth set of columns - to toggle with options
            { targets: [17, 18], className: "hidden-xs hidden-sm" }, // - fourth set of columns
            { targets: [19, 20], className: "hidden-xs hidden-sm hidden-md" } // - fourth set of columns
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

    //---------------------------------------EditFormView----------------------------------------//

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "ComponentType_Id", "/ComponentTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup");
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        if (formIsValid("EditForm", false)) {
            showModalWait();
            submitEditsGeneric("EditForm", [], CurrRecords, "POST", "/ComponentSrv/EditExt")
                .always(hideModalWait)
                .done(function () {
                    refreshMainView();
                    $("#MainView").removeClass("hide");
                    $("#EditFormView").addClass("hide");
                    window.scrollTo(0, 0);
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
        }
    });

    //--------------------------------------View Initialization------------------------------------//

    refreshMainView();

    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    TableMain.clear().search("").draw();
    $("#PanelTableMainTitle").text("Components Extended");

    if (MsFilterByModel.getValue().length == 0) {
        MsFilterByProject.clear(true);
        MsFilterByProject.disable();
        if (typeof ComponentIds !== "undefined" && ComponentIds != null && ComponentIds.length > 0) {
            fillMainTableFromIdsHelper();
        }
        return;
    }

    fillMainTableFromAltIdsHelper();
}

//---------------------------------------Helper Methods--------------------------------------//

//fillMainTableFromAltIdsHelper - used by refreshMainView
function fillMainTableFromAltIdsHelper() {
    refreshTblGenWrp(TableMain, "/ComponentSrv/GetByAltIds",
        {
            projectIds: MsFilterByProject.getValue(),
            modelIds: MsFilterByModel.getValue(),
            getActive: GetActive
        },
        "POST")
        .done(function () {
            MsFilterByProject.enable();
            updateViewsForModelGeneric(TableMain, "/ComponentModelSrv/GetByIds",
                MsFilterByModel.getValue(), "PanelTableMainTitle", "EditFormLabel");
        });
}

//fillMainTableFromIdsHelper - used by refreshMainView
function fillMainTableFromIdsHelper() {
    refreshTblGenWrp(TableMain, "/ComponentSrv/GetByIds", { ids: ComponentIds, getActive: GetActive }, "POST")
        .done(function () {
            var modelIds = TableMain.column("ComponentModel_Id:name").data().toArray();
            if (!modelIdsAreSame(modelIds)) {
                TableMain.clear().search("").draw();
                return;
            }
            updateViewsForModelGeneric(TableMain, "/ComponentModelSrv/GetByIds",
                modelIds[0], "PanelTableMainTitle", "EditFormLabel");
        });
}
