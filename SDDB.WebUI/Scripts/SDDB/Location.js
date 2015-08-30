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
var MagicSuggests = [];
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
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Location", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else {
            if (GetActive) { $("#EditFormGroupIsActive").addClass("hide"); }
            else { $("#EditFormGroupIsActive").removeClass("hide"); }

            $("#EditFormCreateMultiple").addClass("hide");

            showModalWait();

            fillFormForEditGeneric(CurrIds, "POST", "/LocationSrv/GetByIds", GetActive, "EditForm", "Edit Location", MagicSuggests)
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
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6]).visible(true);
        TableMain.columns([7, 8, 9, 10, 11]).visible(false);
        TableMain.columns([12, 13, 14, 15]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6]).visible(false);
        TableMain.columns([7, 8, 9, 10, 11]).visible(true);
        TableMain.columns([12, 13, 14, 15]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6]).visible(false);
        TableMain.columns([7, 8, 9, 10, 11]).visible(false);
        TableMain.columns([12, 13, 14, 15]).visible(true);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows != 1) showModalSelectOne();
        else window.open("/AssemblyDb?LocId=" + TableMain.cell(".ui-selected", "Id:name").data())
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
    $(MsFilterByType).on('selectionchange', function (e, m) { refreshMainView(); });

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

    //TableMain Locations
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LocName", name: "LocName" },//1
            //------------------------------------------------first set of columns
            { data: "LocAltName", name: "LocAltName" },//2
            { data: "LocAltName2", name: "LocAltName2" },//3
            { data: "LocationType_", render: function (data, type, full, meta) { return data.LocTypeName }, name: "LocationType_" }, //4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //5
            { data: "ContactPerson_", render: function (data, type, full, meta) { return data.Initials }, name: "ContactPerson_" },//6
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
            { data: "ContactPerson_Id", name: "ContactPerson_Id" },//19
        ],
        columnDefs: [
            { targets: [0, 16, 17, 18, 19], visible: false }, // - never show
            { targets: [0, 12, 13, 16, 17, 18, 19], searchable: false },  //"orderable": false, "visible": false
            { targets: [4, 5, 6], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [7, 8, 9, 10, 11], visible: false }, // - second set of columns - to toggle with options
            { targets: [8, 9, 10], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [11], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [12, 13, 14, 15], visible: false }, // - third set of columns - to toggle with options
            { targets: [12, 13, 14], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" } // - third set of columns
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
    addToMSArray(MagicSuggests, "LocationType_Id", "/LocationTypeSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "ContactPerson_Id", "/PersonSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            showModalWait();
            var createMultiple = $("#CreateMultiple").val() != "" ? $("#CreateMultiple").val() : 1;
            submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/LocationSrv/Edit", createMultiple)
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

    if (typeof ProjectId !== "undefined" && ProjectId != "") {
        $.ajax({
            type: "POST", url: "/ProjectSrv/GetByIds", timeout: 20000,
            data: { ids: [ProjectId], getActive: true }, dataType: "json",
            beforeSend: function () { showModalWait(); }
        })
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                MsFilterByProject.setSelection([{ id: data[0].Id, name: data[0].ProjectName + " - " + data[0].ProjectCode }]);
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
    CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    deleteRecordsGeneric(CurrIds, "/LocationSrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {
    if (MsFilterByType.getValue().length == 0 &&
        MsFilterByProject.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable");
        TableMain.clear().search("").draw();
    }
    else {
        refreshTblGenWrp(TableMain, "/LocationSrv/GetByAltIds",
            {
                projectIds: MsFilterByProject.getValue(),
                typeIds: MsFilterByType.getValue(),
                getActive: GetActive
            },
            "POST")
            .done($("#ChBoxShowDeleted").bootstrapToggle("enable"))
    }
}



//---------------------------------------Helper Methods--------------------------------------//

