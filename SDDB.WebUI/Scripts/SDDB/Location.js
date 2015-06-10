/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.3.js" />
/// <reference path="../jquery-2.1.3.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        IsCreate = true;
        FillFormForCreate();
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        var selectedRows = TableMain.rows(".ui-selected").data();
        if (selectedRows.length == 0) ShowModalNothingSelected();
        else FillFormForEdit();
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows == 0) ShowModalNothingSelected();
        else ShowModalDelete(noOfRows);
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(true);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
        TableMain.columns([14, 15, 16, 17, 18]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(true);
        TableMain.columns([14, 15, 16, 17, 18]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
        TableMain.columns([14, 15, 16, 17, 18]).visible(true);
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/LocationTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByType).on('selectionchange', function (e, m) { RefreshMainView(); });

    //Initialize MagicSuggest MsFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on('selectionchange', function (e, m) { RefreshMainView(); });

    //---------------------------------------DataTables------------

    //Enable jqueryUI selectable
    if (!Modernizr.touch) {
        $(".selectable").selectable({ filter: "tr" });
    }
    else {
        $(".selectable").on("click", "tr", function () { $(this).toggleClass("ui-selected"); });
    }

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (($(this).prop("checked")) ? false : true)
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        else $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        RefreshMainView();
    });

    //TableMain Locations
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LocName", name: "LocName" },//1
            //------------------------------------------------first set of columns
            { data: "LocAltName", name: "LocAltName" },//2
            { data: "LocTypeName", name: "LocTypeName" },//3  
            { data: "AssignedToProject", render: function (data, type, full, meta) { return data.ProjectName }, name: "AssignedToProject" }, //4
            { data: "ContactPerson", render: function (data, type, full, meta) { return data.Initials }, name: "ContactPerson" },//5
            { data: "Address", name: "Address" },//6
            { data: "City", name: "City" },//7
            //------------------------------------------------second set of columns
            { data: "ZIP", name: "ZIP" },//8
            { data: "State", name: "State" },//9
            { data: "Country", name: "Country" },//10
            { data: "LocX", name: "LocX" },//11
            { data: "LocY", name: "LocY" },//12
            { data: "LocZ", name: "LocZ" },//13
            //------------------------------------------------third set of columns
            { data: "LocStationing", name: "LocStationing" },//14
            { data: "CertOfApprReqd", name: "CertOfApprReqd" },//15
            { data: "RightOfEntryReqd", name: "RightOfEntryReqd" },//16
            { data: "AccessInfo", name: "AccessInfo" },//17
            { data: "Comments", name: "Comments" },//18
            //------------------------------------------------never visible
            { data: "IsActive", name: "IsActive" },//19
            { data: "LocationType_Id", name: "LocationType_Id" },//20
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//21
            { data: "ContactPerson_Id", name: "ContactPerson_Id" },//22
        ],
        columnDefs: [
            { targets: [0, 19, 20, 21, 22], visible: false }, // - never show
            { targets: [0, 15, 16, 19, 20, 21, 22 ], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 4, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6, 7], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [8, 9, 10, 11, 12, 13], visible: false }, // - second set of columns - to toggle with options
            { targets: [11, 12, 13], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [9, 10], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [14, 15, 16, 17, 18], visible: false }, // - third set of columns - to toggle with options
            { targets: [15, 16, 17], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [18], className: "hidden-xs hidden-sm hidden-md" } // - third set of columns
        ],
        order: [[1, "asc"]],
        bAutoWidth: false,
        dom: "<lf<t>p>",
        language: {
            search: "",
            lengthMenu: "_MENU_",
            paginate: { previous: "", next: "" }
        }
    });

    //---------------------------------------EditFormView----------------------------------------//

    //Enable modified field detection
    $(".modifiable").change(function () { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    AddToMSArray(MagicSuggests, "LocationType_Id", "/LocationTypeSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "ContactPerson_Id", "/PersonSrv/Lookup", 1);
    

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        IsCreate = false;
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        MsValidate(MagicSuggests);
        if (FormIsValid("EditForm", IsCreate) && MsIsValid(MagicSuggests)) SubmitEdits();
    });


});

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var IsCreate = false;
var MsFilterByProject = {}; var MsFilterByType = {};
var MagicSuggests = [];
var CurrRecord = {};

//--------------------------------------Main Methods---------------------------------------//

//FillFormForCreate
function FillFormForCreate() {
    ClearFormInputs("EditForm", MagicSuggests);
    $("#EditFormLabel").text("Create Location");
    $("[data-val-dbisunique]").prop("disabled", false);
    DisableUniqueMs(MagicSuggests, false);
    $(".modifiable").data("ismodified", true);
    $("#EditFormGroupIsActive").addClass("hide"); $("#IsActive").prop("checked", true)
    $("#CreateMultipleRow").removeClass("hide");
    $("#MainView").addClass("hide");
    $("#EditFormView").removeClass("hide");
}

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    $("#CreateMultipleRow").addClass("hide");

    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/LocationSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
           
            CurrRecord.LocName = data[0].LocName;
            CurrRecord.LocAltName = data[0].LocAltName;
            CurrRecord.Address = data[0].Address;
            CurrRecord.City = data[0].City;
            CurrRecord.ZIP = data[0].ZIP;
            CurrRecord.State = data[0].State;
            CurrRecord.Country = data[0].Country;
            CurrRecord.LocX = data[0].LocX;
            CurrRecord.LocY = data[0].LocY;
            CurrRecord.LocZ = data[0].LocZ;
            CurrRecord.LocStationing = data[0].LocStationing;
            CurrRecord.CertOfApprReqd = data[0].CertOfApprReqd;
            CurrRecord.RightOfEntryReqd = data[0].RightOfEntryReqd;
            CurrRecord.AccessInfo = data[0].AccessInfo;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.LocationType_Id = data[0].LocationType_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            CurrRecord.ContactPerson_Id = data[0].ContactPerson_Id;
            
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.LocName != dbEntry.LocName) FormInput.LocName = "_VARIES_";
                if (FormInput.LocAltName != dbEntry.LocAltName) FormInput.LocAltName = "_VARIES_";
                if (FormInput.Address != dbEntry.Address) FormInput.Address = "_VARIES_";
                if (FormInput.City != dbEntry.City) FormInput.City = "_VARIES_";
                if (FormInput.ZIP != dbEntry.ZIP) FormInput.ZIP = "_VARIES_";
                if (FormInput.State != dbEntry.State) FormInput.State = "_VARIES_";
                if (FormInput.Country != dbEntry.Country) FormInput.Country = "_VARIES_";
                if (FormInput.LocX != dbEntry.LocX) FormInput.LocX = "_VARIES_";
                if (FormInput.LocY != dbEntry.LocY) FormInput.LocY = "_VARIES_";
                if (FormInput.LocZ != dbEntry.LocZ) FormInput.LocZ = "_VARIES_";
                if (FormInput.LocStationing != dbEntry.LocStationing) FormInput.LocStationing = "_VARIES_";
                if (FormInput.CertOfApprReqd != dbEntry.CertOfApprReqd) FormInput.CertOfApprReqd = "_VARIES_";
                if (FormInput.RightOfEntryReqd != dbEntry.RightOfEntryReqd) FormInput.RightOfEntryReqd = "_VARIES_";
                if (FormInput.AccessInfo != dbEntry.AccessInfo) FormInput.AccessInfo = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";

                if (FormInput.LocationType_Id != dbEntry.LocationType_Id) { FormInput.LocationType_Id = "_VARIES_"; FormInput.LocTypeName = "_VARIES_"; }
                else FormInput.LocTypeName = dbEntry.LocTypeName;
                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
                if (FormInput.ContactPerson_Id != dbEntry.ContactPerson_Id) { FormInput.ContactPerson_Id = "_VARIES_"; FormInput.ContactPerson = "_VARIES_"; }
                else FormInput.ContactPerson = dbEntry.ContactPerson.FirstName + " " + dbEntry.ContactPerson.LastName;
            });

            ClearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Location");

            $("#LocName").val(FormInput.LocName);
            $("#LocAltName").val(FormInput.LocAltName);
            $("#Address").val(FormInput.Address);
            $("#City").val(FormInput.City);
            $("#ZIP").val(FormInput.ZIP);
            $("#State").val(FormInput.State);
            $("#Country").val(FormInput.Country);
            $("#LocX").val(FormInput.LocX);
            $("#LocY").val(FormInput.LocY);
            $("#LocZ").val(FormInput.LocZ);
            $("#LocStationing").val(FormInput.LocStationing);
            if (FormInput.CertOfApprReqd == true) $("#CertOfApprReqd").prop("checked", true);
            if (FormInput.RightOfEntryReqd == true) $("#RightOfEntryReqd").prop("checked", true);
            $("#AccessInfo").val(FormInput.AccessInfo);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);

            if (FormInput.LocationType_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.LocationType_Id, name: FormInput.LocTypeName }], true);
            if (FormInput.AssignedToProject_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.AssignedToProject_Id, name: FormInput.AssignedToProject }], true);
            if (FormInput.ContactPerson_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.ContactPerson_Id, name: FormInput.ContactPerson }], true);

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                DisableUniqueMs(MagicSuggests, false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
                DisableUniqueMs(MagicSuggests, true);
            }

            $("#MainView").addClass("hide");
            $("#EditFormView").removeClass("hide");
        })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//SubmitEdits to DB
function SubmitEdits() {

    var modifiedProperties = [];
    $(".modifiable").each(function (index) {
        if ($(this).data("ismodified")) modifiedProperties.push($(this).prop("id"));
    });

    $.each(MagicSuggests, function (i, ms) {
        if (ms.isModified == true) modifiedProperties.push(ms.id);
    });

    var editRecords = [];
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    if (IsCreate == true) {
        var multipleCount = ($("#CreateMultiple").val() == "") ? 1 : $("#CreateMultiple").val();
        ids = []; for (var i = 1; i <= multipleCount; i++) {
            var id = "newEntryId"; ids.push(id);
        }
    }

    var magicResults = [];
    $.each(MagicSuggests, function (i, ms) {
        var msValue = (ms.getSelection().length != 0) ? (ms.getSelection())[0].id : null;
        magicResults.push(msValue);
    });

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;
               
        editRecord.LocName = ($("#LocName").data("ismodified")) ? $("#LocName").val() : CurrRecord.LocName;
        editRecord.LocAltName = ($("#LocAltName").data("ismodified")) ? $("#LocAltName").val() : CurrRecord.LocAltName;
        editRecord.Address = ($("#Address").data("ismodified")) ? $("#Address").val() : CurrRecord.Address;
        editRecord.City = ($("#City").data("ismodified")) ? $("#City").val() : CurrRecord.City;
        editRecord.ZIP = ($("#ZIP").data("ismodified")) ? $("#ZIP").val() : CurrRecord.ZIP;
        editRecord.State = ($("#State").data("ismodified")) ? $("#State").val() : CurrRecord.State;
        editRecord.Country = ($("#Country").data("ismodified")) ? $("#Country").val() : CurrRecord.Country;
        editRecord.LocX = ($("#LocX").data("ismodified")) ? $("#LocX").val() : CurrRecord.LocX;
        editRecord.LocY = ($("#LocY").data("ismodified")) ? $("#LocY").val() : CurrRecord.LocY;
        editRecord.LocZ = ($("#LocZ").data("ismodified")) ? $("#LocZ").val() : CurrRecord.LocZ;
        editRecord.LocStationing = ($("#LocStationing").data("ismodified")) ? $("#LocStationing").val() : CurrRecord.LocStationing;
        editRecord.CertOfApprReqd = ($("#CertOfApprReqd").data("ismodified")) ? (($("#CertOfApprReqd").prop("checked")) ? true : false) : CurrRecord.CertOfApprReqd;
        editRecord.RightOfEntryReqd = ($("#RightOfEntryReqd").data("ismodified")) ? (($("#RightOfEntryReqd").prop("checked")) ? true : false) : CurrRecord.RightOfEntryReqd;
        editRecord.AccessInfo = ($("#AccessInfo").data("ismodified")) ? $("#AccessInfo").val() : CurrRecord.AccessInfo;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.LocationType_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.LocationType_Id;
        editRecord.AssignedToProject_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.AssignedToProject_Id;
        editRecord.ContactPerson_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.ContactPerson_Id;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/LocationSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            RefreshMainView();
            IsCreate = false;
            $("#MainView").removeClass("hide");
            $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) {
            ShowModalAJAXFail(xhr, status, error);
        });
}

//Delete Records from DB
function DeleteRecords() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/LocationSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshMainView(); })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//refresh view after magicsuggest update
function RefreshMainView() {
    if (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        RefreshTable(TableMain, "/LocationSrv/GetByTypeIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
            "POST", MsFilterByProject.getValue(), [], MsFilterByType.getValue());
        $("#ChBoxShowDeleted").bootstrapToggle("enable")
    }

}

//---------------------------------------Helper Methods--------------------------------------//


