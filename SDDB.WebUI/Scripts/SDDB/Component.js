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
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(false);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(true);
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(false);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(true);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(false);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
        TableMain.columns([14, 15, 16, 17, 18, 19]).visible(false);
        TableMain.columns([20, 21, 22, 23, 24, 25]).visible(true);
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/ComponentTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByType).on('selectionchange', function (e, m) { RefreshMainView(); });

    //Initialize MagicSuggest msFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on('selectionchange', function (e, m) {
        if (this.getValue().length == 0) { MsFilterByLoc.disable(); MsFilterByLoc.clear(true); }
        else MsFilterByLoc.enable();
        RefreshMainView();
    });

    //Initialize MagicSuggest MsFilterByLoc
    MsFilterByLoc = $("#MsFilterByLoc").magicSuggest({
        data: "/LocationSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: MsFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByLoc).on('selectionchange', function (e, m) { RefreshMainView(); });
    MsFilterByLoc.disable();


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

    //TableMain Components
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompName", name: "CompName" },//1
            //------------------------------------------------first set of columns
            { data: "CompAltName", name: "CompAltName" },//2
            { data: "CompAltName2", name: "CompAltName2" },//3
            { data: "CompTypeName", name: "CompTypeName" },//4  
            { data: "CompStatusName", name: "CompStatusName" },//5
            { data: "CompModelName", name: "CompModelName" },//6
            { data: "AssignedToProject", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject" }, //7
            //------------------------------------------------second set of columns
            { data: "AssignedToLocation", render: function (data, type, full, meta) { return data.LocName + " - " + data.LocTypeName }, name: "AssignedToLocation" }, //8
            { data: "CompGlobalX", name: "CompGlobalX" },//9
            { data: "CompGlobalY", name: "CompGlobalY" },//10
            { data: "CompGlobalZ", name: "CompGlobalZ" },//11
            { data: "CompLocalXDesign", name: "CompLocalXDesign" },//12
            { data: "CompLocalYDesign", name: "CompLocalYDesign" },//13
            //------------------------------------------------third set of columns
            { data: "CompLocalZDesign", name: "CompLocalZDesign" },//14
            { data: "CompLocalXAsBuilt", name: "CompLocalXAsBuilt" },//15
            { data: "CompLocalYAsBuilt", name: "CompLocalYAsBuilt" },//16
            { data: "CompLocalZAsBuilt", name: "CompLocalZAsBuilt" },//17
            { data: "CompStationing", name: "CompStationing" },//18
            { data: "CompLength", name: "CompLength" },//19
            //------------------------------------------------Fourth set of columns
            { data: "CompReadingIntervalSecs", name: "CompReadingIntervalSecs" },//20
            { data: "IsReference", name: "IsReference" },//21
            { data: "TechnicalDetails", name: "TechnicalDetails" },//22
            { data: "PowerSupplyDetails", name: "PowerSupplyDetails" },//23
            { data: "HSEDetails", name: "HSEDetails" },//24
            { data: "Comments", name: "Comments" },//25
            //------------------------------------------------never visible
            { data: "IsActive", name: "IsActive" },//26
            { data: "ComponentType_Id", name: "ComponentType_Id" },//27
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//28
            { data: "ComponentModel_Id", name: "ComponentModel_Id" },//29
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//30
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" }//31
        ],
        columnDefs: [
            { targets: [0, 26, 27, 28, 29, 30, 31], visible: false }, // - never show
            { targets: [0, 21, 26, 27, 28, 29, 30, 31], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 3, 4], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6, 7], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [8, 9, 10, 11, 12, 13], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 10], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [11, 12, 13], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [14, 15, 16, 17, 18, 19], visible: false }, // - third set of columns - to toggle with options
            { targets: [18, 19], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [14, 15, 16], className: "hidden-xs hidden-sm hidden-md" }, // - third set of columns

            { targets: [20, 21, 22, 23, 24, 25], visible: false }, // - fourth set of columns - to toggle with options
            { targets: [21, 22, 23], className: "hidden-xs hidden-sm" }, // - fourth set of columns
            { targets: [24, 25], className: "hidden-xs hidden-sm hidden-md" } // - fourth set of columns
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
    AddToMSArray(MagicSuggests, "ComponentType_Id", "/ComponentTypeSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "ComponentModel_Id", "/ComponentModelSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/Lookup", 1);

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
var MsFilterByProject = {}; var MsFilterByType = {}; var MsFilterByLoc = {};
var MagicSuggests = [];
var CurrRecord = {};

//--------------------------------------Main Methods---------------------------------------//

//FillFormForCreate
function FillFormForCreate() {
    ClearFormInputs("EditForm", MagicSuggests);
    $("#EditFormLabel").text("Create Component");
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
        type: "POST", url: "/ComponentSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
         
            CurrRecord.CompName = data[0].CompName;
            CurrRecord.CompAltName = data[0].CompAltName;
            CurrRecord.CompAltName2 = data[0].CompAltName2;
            CurrRecord.CompGlobalX = data[0].CompGlobalX;
            CurrRecord.CompGlobalY = data[0].CompGlobalY;
            CurrRecord.CompGlobalZ = data[0].CompGlobalZ;
            CurrRecord.CompLocalXDesign = data[0].CompLocalXDesign;
            CurrRecord.CompLocalYDesign = data[0].CompLocalYDesign;
            CurrRecord.CompLocalZDesign = data[0].CompLocalZDesign;
            CurrRecord.CompLocalXAsBuilt = data[0].CompLocalXAsBuilt;
            CurrRecord.CompLocalYAsBuilt = data[0].CompLocalYAsBuilt;
            CurrRecord.CompLocalZAsBuilt = data[0].CompLocalZAsBuilt;
            CurrRecord.CompStationing = data[0].CompStationing;
            CurrRecord.CompLength = data[0].CompLength;
            CurrRecord.CompReadingIntervalSecs = data[0].CompReadingIntervalSecs;
            CurrRecord.IsReference = data[0].IsReference;
            CurrRecord.TechnicalDetails = data[0].TechnicalDetails;
            CurrRecord.PowerSupplyDetails = data[0].PowerSupplyDetails;
            CurrRecord.HSEDetails = data[0].HSEDetails;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.ComponentType_Id = data[0].ComponentType_Id;
            CurrRecord.ComponentStatus_Id = data[0].ComponentStatus_Id;
            CurrRecord.ComponentModel_Id = data[0].ComponentModel_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            CurrRecord.AssignedToLocation_Id = data[0].AssignedToLocation_Id;
            
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.CompName != dbEntry.CompName) FormInput.CompName = "_VARIES_";
                if (FormInput.CompAltName != dbEntry.CompAltName) FormInput.CompAltName = "_VARIES_";
                if (FormInput.CompAltName2 != dbEntry.CompAltName2) FormInput.CompAltName2 = "_VARIES_";
                if (FormInput.CompGlobalX != dbEntry.CompGlobalX) FormInput.CompGlobalX = "_VARIES_";
                if (FormInput.CompGlobalY != dbEntry.CompGlobalY) FormInput.CompGlobalY = "_VARIES_";
                if (FormInput.CompGlobalZ != dbEntry.CompGlobalZ) FormInput.CompGlobalZ = "_VARIES_";
                if (FormInput.CompLocalXDesign != dbEntry.CompLocalXDesign) FormInput.CompLocalXDesign = "_VARIES_";
                if (FormInput.CompLocalYDesign != dbEntry.CompLocalYDesign) FormInput.CompLocalYDesign = "_VARIES_";
                if (FormInput.CompLocalZDesign != dbEntry.CompLocalZDesign) FormInput.CompLocalZDesign = "_VARIES_";
                if (FormInput.CompLocalXAsBuilt != dbEntry.CompLocalXAsBuilt) FormInput.CompLocalXAsBuilt = "_VARIES_";
                if (FormInput.CompLocalYAsBuilt != dbEntry.CompLocalYAsBuilt) FormInput.CompLocalYAsBuilt = "_VARIES_";
                if (FormInput.CompLocalZAsBuilt != dbEntry.CompLocalZAsBuilt) FormInput.CompLocalZAsBuilt = "_VARIES_";
                if (FormInput.CompStationing != dbEntry.CompStationing) FormInput.CompStationing = "_VARIES_";
                if (FormInput.CompLength != dbEntry.CompLength) FormInput.CompLength = "_VARIES_";
                if (FormInput.CompReadingIntervalSecs != dbEntry.CompReadingIntervalSecs) FormInput.CompReadingIntervalSecs = "_VARIES_";
                if (FormInput.IsReference != dbEntry.IsReference) FormInput.IsReference = "_VARIES_";
                if (FormInput.TechnicalDetails != dbEntry.TechnicalDetails) FormInput.TechnicalDetails = "_VARIES_";
                if (FormInput.PowerSupplyDetails != dbEntry.PowerSupplyDetails) FormInput.PowerSupplyDetails = "_VARIES_";
                if (FormInput.HSEDetails != dbEntry.HSEDetails) FormInput.HSEDetails = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";

                if (FormInput.ComponentType_Id != dbEntry.ComponentType_Id) { FormInput.ComponentType_Id = "_VARIES_"; FormInput.CompTypeName = "_VARIES_"; }
                else FormInput.CompTypeName = dbEntry.CompTypeName;
                if (FormInput.ComponentStatus_Id != dbEntry.ComponentStatus_Id) { FormInput.ComponentStatus_Id = "_VARIES_"; FormInput.CompStatusName = "_VARIES_"; }
                else FormInput.CompStatusName = dbEntry.CompStatusName;
                if (FormInput.ComponentModel_Id != dbEntry.ComponentModel_Id) { FormInput.ComponentModel_Id = "_VARIES_"; FormInput.CompModelName = "_VARIES_"; }
                else FormInput.CompModelName = dbEntry.CompModelName;
                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
                if (FormInput.AssignedToLocation_Id != dbEntry.AssignedToLocation_Id) { FormInput.AssignedToLocation_Id = "_VARIES_"; FormInput.AssignedToLocation = "_VARIES_"; }
                else FormInput.AssignedToLocation = dbEntry.AssignedToLocation.LocName;
            });

            ClearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Component");

            $("#CompName").val(FormInput.CompName);
            $("#CompAltName").val(FormInput.CompAltName);
            $("#CompAltName2").val(FormInput.CompAltName2);
            $("#CompGlobalX").val(FormInput.CompGlobalX);
            $("#CompGlobalY").val(FormInput.CompGlobalY);
            $("#CompGlobalZ").val(FormInput.CompGlobalZ);
            $("#CompLocalXDesign").val(FormInput.CompLocalXDesign);
            $("#CompLocalYDesign").val(FormInput.CompLocalYDesign);
            $("#CompLocalZDesign").val(FormInput.CompLocalZDesign);
            $("#CompLocalXAsBuilt").val(FormInput.CompLocalXAsBuilt);
            $("#CompLocalYAsBuilt").val(FormInput.CompLocalYAsBuilt);
            $("#CompLocalZAsBuilt").val(FormInput.CompLocalZAsBuilt);
            $("#CompStationing").val(FormInput.CompStationing);
            $("#CompLength").val(FormInput.CompLength);
            $("#CompReadingIntervalSecs").val(FormInput.CompReadingIntervalSecs);
            if (FormInput.IsReference == true) $("#IsReference").prop("checked", true);
            $("#TechnicalDetails").val(FormInput.TechnicalDetails);
            $("#PowerSupplyDetails").val(FormInput.PowerSupplyDetails);
            $("#HSEDetails").val(FormInput.HSEDetails);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);

            if (FormInput.ComponentType_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.ComponentType_Id, name: FormInput.CompTypeName }], true);
            if (FormInput.ComponentStatus_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.ComponentStatus_Id, name: FormInput.CompStatusName }], true);
            if (FormInput.ComponentModel_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.ComponentModel_Id, name: FormInput.CompModelName }], true);
            if (FormInput.AssignedToProject_Id != null) MagicSuggests[3].addToSelection([{ id: FormInput.AssignedToProject_Id, name: FormInput.AssignedToProject }], true);
            if (FormInput.AssignedToLocation_Id != null) MagicSuggests[4].addToSelection([{ id: FormInput.AssignedToLocation_Id, name: FormInput.AssignedToLocation }], true);


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
               
        editRecord.CompName = ($("#CompName").data("ismodified")) ? $("#CompName").val() : CurrRecord.CompName;
        editRecord.CompAltName = ($("#CompAltName").data("ismodified")) ? $("#CompAltName").val() : CurrRecord.CompAltName;
        editRecord.CompAltName2 = ($("#CompAltName2").data("ismodified")) ? $("#CompAltName2").val() : CurrRecord.CompAltName2;
        editRecord.CompGlobalX = ($("#CompGlobalX").data("ismodified")) ? $("#CompGlobalX").val() : CurrRecord.CompGlobalX;
        editRecord.CompGlobalY = ($("#CompGlobalY").data("ismodified")) ? $("#CompGlobalY").val() : CurrRecord.CompGlobalY;
        editRecord.CompGlobalZ = ($("#CompGlobalZ").data("ismodified")) ? $("#CompGlobalZ").val() : CurrRecord.CompGlobalZ;
        editRecord.CompLocalXDesign = ($("#CompLocalXDesign").data("ismodified")) ? $("#CompLocalXDesign").val() : CurrRecord.CompLocalXDesign;
        editRecord.CompLocalYDesign = ($("#CompLocalYDesign").data("ismodified")) ? $("#CompLocalYDesign").val() : CurrRecord.CompLocalYDesign;
        editRecord.CompLocalZDesign = ($("#CompLocalZDesign").data("ismodified")) ? $("#CompLocalZDesign").val() : CurrRecord.CompLocalZDesign;
        editRecord.CompLocalXAsBuilt = ($("#CompLocalXAsBuilt").data("ismodified")) ? $("#CompLocalXAsBuilt").val() : CurrRecord.CompLocalXAsBuilt;
        editRecord.CompLocalYAsBuilt = ($("#CompLocalYAsBuilt").data("ismodified")) ? $("#CompLocalYAsBuilt").val() : CurrRecord.CompLocalYAsBuilt;
        editRecord.CompLocalZAsBuilt = ($("#CompLocalZAsBuilt").data("ismodified")) ? $("#CompLocalZAsBuilt").val() : CurrRecord.CompLocalZAsBuilt;
        editRecord.CompStationing = ($("#CompStationing").data("ismodified")) ? $("#CompStationing").val() : CurrRecord.CompStationing;
        editRecord.CompLength = ($("#CompLength").data("ismodified")) ? $("#CompLength").val() : CurrRecord.CompLength;
        editRecord.CompReadingIntervalSecs = ($("#CompReadingIntervalSecs").data("ismodified")) ? $("#CompReadingIntervalSecs").val() : CurrRecord.CompReadingIntervalSecs;
        editRecord.IsReference = ($("#IsReference").data("ismodified")) ? (($("#IsReference").prop("checked")) ? true : false) : CurrRecord.IsReference;
        editRecord.TechnicalDetails = ($("#TechnicalDetails").data("ismodified")) ? $("#TechnicalDetails").val() : CurrRecord.TechnicalDetails;
        editRecord.PowerSupplyDetails = ($("#PowerSupplyDetails").data("ismodified")) ? $("#PowerSupplyDetails").val() : CurrRecord.PowerSupplyDetails;
        editRecord.HSEDetails = ($("#HSEDetails").data("ismodified")) ? $("#HSEDetails").val() : CurrRecord.HSEDetails;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.ComponentType_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.ComponentType_Id;
        editRecord.ComponentStatus_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.ComponentStatus_Id;
        editRecord.ComponentModel_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.ComponentModel_Id;
        editRecord.AssignedToProject_Id = (MagicSuggests[3].isModified) ? magicResults[3] : CurrRecord.AssignedToProject_Id;
        editRecord.AssignedToLocation_Id = (MagicSuggests[4].isModified) ? magicResults[4] : CurrRecord.AssignedToLocation_Id;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/ComponentSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
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
        type: "POST", url: "/ComponentSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshMainView(); })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//refresh view after magicsuggest update
function RefreshMainView() {
    if (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0
        && MsFilterByLoc.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        RefreshTable(TableMain, "/ComponentSrv/GetByTypeLocIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
            "POST", MsFilterByProject.getValue(), [], MsFilterByType.getValue(), MsFilterByLoc.getValue());
        $("#ChBoxShowDeleted").bootstrapToggle("enable")
    }

}


//---------------------------------------Helper Methods--------------------------------------//


