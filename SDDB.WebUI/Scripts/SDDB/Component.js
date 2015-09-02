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
var MsFilterByProject = {};
var MsFilterByType = {};
var MsFilterByLoc = {};
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    CompName: null,
    CompAltName: null,
    CompAltName2: null,
    PositionInAssy: null,
    ProgramAddress: null,
    CalibrationReqd_bl: null,
    LastCalibrationDate: null,
    Comments: null,
    IsActive_bl: null,
    ComponentType_Id: null,
    ComponentStatus_Id: null,
    ComponentModel_Id: null,
    AssignedToProject_Id: null,
    AssignedToAssemblyDb_Id: null
};
var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        $("#EditFormCreateMultiple").removeClass("hide");
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Component", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else {
            if (GetActive) { $("#EditFormGroupIsActive").addClass("hide"); }
            else { $("#EditFormGroupIsActive").removeClass("hide"); }

            $("#EditFormCreateMultiple").addClass("hide");

            showModalWait();

            fillFormForEditGeneric(CurrIds, "POST", "/ComponentSrv/GetByIds", GetActive, "EditForm", "Edit Component", MagicSuggests)
                .always(hideModalWait)
                .done(function (currRecords) {
                    CurrRecords = currRecords;
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(true);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(true);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected", { page: "current" }).data().length;
        if (noOfRows != 1) showModalSelectOne();
        else window.open("/ComponentLogEntry?compId=" + TableMain.cell(".ui-selected", "Id:name", { page: "current" }).data())
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/ComponentTypeSrv/Lookup",
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

    //Initialize MagicSuggest MsFilterByAssy
    MsFilterByAssy = $("#MsFilterByAssy").magicSuggest({
        data: "/AssemblyDbSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: MsFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByAssy
    $(MsFilterByAssy).on('selectionchange', function (e, m) { refreshMainView(); });


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

    //TableMain Components
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompName", name: "CompName" },//1
            //------------------------------------------------first set of columns
            { data: "CompAltName", name: "CompAltName" },//2
            { data: "CompAltName2", name: "CompAltName2" },//3
            { data: "ComponentType_", render: function (data, type, full, meta) { return data.CompTypeName }, name: "ComponentType_" }, //4
            { data: "ComponentStatus_", render: function (data, type, full, meta) { return data.CompStatusName }, name: "ComponentStatus_" }, //5
            { data: "ComponentModel_", render: function (data, type, full, meta) { return data.CompModelName }, name: "ComponentModel_" }, //6
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //7
            //------------------------------------------------second set of columns
            { data: "AssignedToAssemblyDb_", render: function (data, type, full, meta) { return data.AssyName }, name: "AssignedToAssemblyDb_" }, //8
            { data: "PositionInAssy", name: "PositionInAssy" },//9
            { data: "ProgramAddress", name: "ProgramAddress" },//10
            { data: "CalibrationReqd_bl", name: "CalibrationReqd_bl" },//11
            { data: "LastCalibrationDate", name: "LastCalibrationDate" },//12
            { data: "Comments", name: "Comments" },//13
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//14
            { data: "ComponentType_Id", name: "ComponentType_Id" },//15
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//16
            { data: "ComponentModel_Id", name: "ComponentModel_Id" },//17
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//18
            { data: "AssignedToAssemblyDb_Id", name: "AssignedToAssemblyDb_Id" }//19
        ],
        columnDefs: [
            { targets: [0, 14, 15, 16, 17, 18, 19], visible: false }, // - never show
            { targets: [0, 11, 12, 14, 15, 16, 17, 18, 19], searchable: false },  //"orderable": false, "visible": false
            { targets: [4, 6, 7], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [2, 3], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [8, 9, 10, 11, 12, 13], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 11, 12], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [10, 13], className: "hidden-xs hidden-sm hidden-md" } // - second set of columns
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
    addToMSArray(MagicSuggests, "ComponentType_Id", "/ComponentTypeSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "ComponentModel_Id", "/ComponentModelSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

    //Enable DateTimePicker
    $("[data-val-dbisdateiso]").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide");
        window.scrollTo(0, 0);
    });
    
    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            showModalWait();
            var createMultiple = $("#CreateMultiple").val() != "" ? $("#CreateMultiple").val() : 1;
            submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/ComponentSrv/Edit", createMultiple)
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

    if (typeof assyId !== "undefined" && assyId != "") {
        showModalWait();
        $.ajax({
            type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 120000,
            data: { ids: [assyId], getActive: true }, dataType: "json",
        })
            .always(hideModalWait)
            .done(function (data) {
                MsFilterByAssy.setSelection([{ id: data[0].Id, name: data[0].AssyName, }]);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/ComponentSrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {
    if (MsFilterByType.getValue().length == 0 &&
            MsFilterByProject.getValue().length == 0 &&
            MsFilterByLoc.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable");
        TableMain.clear().search("").draw();
    }
    else {
        refreshTblGenWrp(TableMain, "/ComponentSrv/GetByAltIds2",
            {
                projectIds: MsFilterByProject.getValue(),
                typeIds: MsFilterByType.getValue(),
                locIds: MsFilterByLoc.getValue(),
                getActive: GetActive
            },
            "POST")
            .done($("#ChBoxShowDeleted").bootstrapToggle("enable"))
    }
}

function refreshMainView() {
    if (MsFilterByType.getValue().length == 0 &&
        MsFilterByProject.getValue().length == 0 &&
        MsFilterByAssy.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        refreshTblGenWrp(TableMain, "/ComponentSrv/GetByAltIds2",
            {
                projectIds: MsFilterByProject.getValue(),
                typeIds: MsFilterByType.getValue(),
                assyIds: MsFilterByAssy.getValue(),
                getActive: GetActive
            },
            "POST")
            .done($("#ChBoxShowDeleted").bootstrapToggle("enable"))
    }
}



//---------------------------------------Helper Methods--------------------------------------//

