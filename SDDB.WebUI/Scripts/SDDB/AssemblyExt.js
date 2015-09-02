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
    AssyName: null,
    AssyAltName: null,
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
    AssemblyType_Id: null,
    AssemblyStatus_Id: null,
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
            var editFormLabel = "Edit " + MsFilterByModel.getSelection()[0].name;
            fillFormForEditGeneric(CurrIds, "POST", "/AssemblyDbSrv/GetByIds", GetActive, "EditForm", editFormLabel, MagicSuggests)
                .always(hideModalWait)
                .done(function (currRecords) {
                    CurrRecords = currRecords;
                    disableAllMs(MagicSuggests);
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
        if (noOfRows != 1) { showModalSelectOne(); }
        else { window.open("/AssemblyLogEntry?assyId=" + TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data()) }
    });

    //Initialize MagicSuggest MsFilterByModel
    MsFilterByModel = $("#MsFilterByModel").magicSuggest({
        data: "/AssemblyModelSrv/Lookup",
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

    //TableMain AssemblyExts
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" },//1
            //------------------------------------------------first set of columns
            { data: "AssyAltName", name: "AssyAltName" },//2
            { data: "AssemblyType_", render: function (data, type, full, meta) { return data.AssyTypeName }, name: "AssemblyType_" }, //3
            { data: "AssemblyStatus_", render: function (data, type, full, meta) { return data.AssyStatusName }, name: "AssemblyStatus_" }, //4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //5
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
            { data: "AssemblyType_Id", name: "AssemblyType_Id" },//21
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//22
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//23
        ],
        columnDefs: [
            { targets: [0, 21, 22, 23], visible: false }, // - never show
            { targets: [0, 21, 22, 23], searchable: false },  //"orderable": false, "visible": false
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
    addToMSArray(MagicSuggests, "AssemblyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup");
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
  
    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        if (formIsValid("EditForm", false)) {
            showModalWait();
            submitEditsGeneric("EditForm", [], CurrRecords, "POST", "/AssemblyDbSrv/EditExt")
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
        
    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    if (MsFilterByModel.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable");
        TableMain.clear().search("").draw();
        MsFilterByProject.disable();
        MsFilterByProject.clear(true);
    }
    else {
        refreshTblGenWrp(TableMain, "/AssemblyDbSrv/GetByAltIds",
            {
                projectIds: MsFilterByProject.getValue(),
                modelIds: MsFilterByModel.getValue(),
                getActive: GetActive
            },
            "POST")
            .done(function myfunction() {
                $("#ChBoxShowDeleted").bootstrapToggle("enable");
                MsFilterByProject.enable();
                updateViewsForModelGeneric(TableMain, "/AssemblyModelSrv/GetByIds", MsFilterByModel.getValue());
            });
    }
}


//---------------------------------------Helper Methods--------------------------------------//

