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

    //Initialize MagicSuggest msFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on('selectionchange', function (e, m) { RefreshMainView(); });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/AssemblyTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByType).on('selectionchange', function (e, m) { RefreshMainView(); });




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

    //TableMain AssemblyDbs
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" },//1
            //------------------------------------------------first set of columns
            { data: "AssyAltName", name: "AssyAltName" },//2
            { data: "AssyAltName2", name: "AssyAltName2" },//3
            { data: "AssyTypeName", name: "AssyTypeName" },//4  
            { data: "AssyStatusName", name: "AssyStatusName" },//5
            { data: "AssyModelName", name: "AssyModelName" },//6
            { data: "AssignedToProject", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject" }, //7
            //------------------------------------------------second set of columns
            { data: "AssignedToLocation", render: function (data, type, full, meta) { return data.LocName + " - " + data.LocTypeName }, name: "AssignedToLocation" }, //8
            { data: "AssyGlobalX", name: "AssyGlobalX" },//9
            { data: "AssyGlobalY", name: "AssyGlobalY" },//10
            { data: "AssyGlobalZ", name: "AssyGlobalZ" },//11
            { data: "AssyLocalXDesign", name: "AssyLocalXDesign" },//12
            { data: "AssyLocalYDesign", name: "AssyLocalYDesign" },//13
            //------------------------------------------------third set of columns
            { data: "AssyLocalZDesign", name: "AssyLocalZDesign" },//14
            { data: "AssyLocalXAsBuilt", name: "AssyLocalXAsBuilt" },//15
            { data: "AssyLocalYAsBuilt", name: "AssyLocalYAsBuilt" },//16
            { data: "AssyLocalZAsBuilt", name: "AssyLocalZAsBuilt" },//17
            { data: "AssyStationing", name: "AssyStationing" },//18
            { data: "AssyLength", name: "AssyLength" },//19
            //------------------------------------------------Fourth set of columns
            { data: "AssyReadingIntervalSecs", name: "AssyReadingIntervalSecs" },//20
            { data: "IsReference", name: "IsReference" },//21
            { data: "TechnicalDetails", name: "TechnicalDetails" },//22
            { data: "PowerSupplyDetails", name: "PowerSupplyDetails" },//23
            { data: "HSEDetails", name: "HSEDetails" },//24
            { data: "Comments", name: "Comments" },//25
            //------------------------------------------------never visible
            { data: "IsActive", name: "IsActive" },//26
            { data: "AssemblyType_Id", name: "AssemblyType_Id" },//27
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//28
            { data: "AssemblyModel_Id", name: "AssemblyModel_Id" },//29
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
    AddToMSArray(MagicSuggests, "AssemblyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssemblyModel_Id", "/AssemblyModelSrv/Lookup", 1);
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
var MsFilterByProject = {}; var MsFilterByType = {};
var MagicSuggests = [];
var CurrRecord = {};

//--------------------------------------Main Methods---------------------------------------//

//FillFormForCreate
function FillFormForCreate() {
    ClearFormInputs("EditForm", MagicSuggests);
    $("#EditFormLabel").text("Create Assembly");
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
        type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
         
            CurrRecord.AssyName = data[0].AssyName;
            CurrRecord.AssyAltName = data[0].AssyAltName;
            CurrRecord.AssyAltName2 = data[0].AssyAltName2;
            CurrRecord.AssyGlobalX = data[0].AssyGlobalX;
            CurrRecord.AssyGlobalY = data[0].AssyGlobalY;
            CurrRecord.AssyGlobalZ = data[0].AssyGlobalZ;
            CurrRecord.AssyLocalXDesign = data[0].AssyLocalXDesign;
            CurrRecord.AssyLocalYDesign = data[0].AssyLocalYDesign;
            CurrRecord.AssyLocalZDesign = data[0].AssyLocalZDesign;
            CurrRecord.AssyLocalXAsBuilt = data[0].AssyLocalXAsBuilt;
            CurrRecord.AssyLocalYAsBuilt = data[0].AssyLocalYAsBuilt;
            CurrRecord.AssyLocalZAsBuilt = data[0].AssyLocalZAsBuilt;
            CurrRecord.AssyStationing = data[0].AssyStationing;
            CurrRecord.AssyLength = data[0].AssyLength;
            CurrRecord.AssyReadingIntervalSecs = data[0].AssyReadingIntervalSecs;
            CurrRecord.IsReference = data[0].IsReference;
            CurrRecord.TechnicalDetails = data[0].TechnicalDetails;
            CurrRecord.PowerSupplyDetails = data[0].PowerSupplyDetails;
            CurrRecord.HSEDetails = data[0].HSEDetails;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.AssemblyType_Id = data[0].AssemblyType_Id;
            CurrRecord.AssemblyStatus_Id = data[0].AssemblyStatus_Id;
            CurrRecord.AssemblyModel_Id = data[0].AssemblyModel_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            CurrRecord.AssignedToLocation_Id = data[0].AssignedToLocation_Id;
            
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.AssyName != dbEntry.AssyName) FormInput.AssyName = "_VARIES_";
                if (FormInput.AssyAltName != dbEntry.AssyAltName) FormInput.AssyAltName = "_VARIES_";
                if (FormInput.AssyAltName2 != dbEntry.AssyAltName2) FormInput.AssyAltName2 = "_VARIES_";
                if (FormInput.AssyGlobalX != dbEntry.AssyGlobalX) FormInput.AssyGlobalX = "_VARIES_";
                if (FormInput.AssyGlobalY != dbEntry.AssyGlobalY) FormInput.AssyGlobalY = "_VARIES_";
                if (FormInput.AssyGlobalZ != dbEntry.AssyGlobalZ) FormInput.AssyGlobalZ = "_VARIES_";
                if (FormInput.AssyLocalXDesign != dbEntry.AssyLocalXDesign) FormInput.AssyLocalXDesign = "_VARIES_";
                if (FormInput.AssyLocalYDesign != dbEntry.AssyLocalYDesign) FormInput.AssyLocalYDesign = "_VARIES_";
                if (FormInput.AssyLocalZDesign != dbEntry.AssyLocalZDesign) FormInput.AssyLocalZDesign = "_VARIES_";
                if (FormInput.AssyLocalXAsBuilt != dbEntry.AssyLocalXAsBuilt) FormInput.AssyLocalXAsBuilt = "_VARIES_";
                if (FormInput.AssyLocalYAsBuilt != dbEntry.AssyLocalYAsBuilt) FormInput.AssyLocalYAsBuilt = "_VARIES_";
                if (FormInput.AssyLocalZAsBuilt != dbEntry.AssyLocalZAsBuilt) FormInput.AssyLocalZAsBuilt = "_VARIES_";
                if (FormInput.AssyStationing != dbEntry.AssyStationing) FormInput.AssyStationing = "_VARIES_";
                if (FormInput.AssyLength != dbEntry.AssyLength) FormInput.AssyLength = "_VARIES_";
                if (FormInput.AssyReadingIntervalSecs != dbEntry.AssyReadingIntervalSecs) FormInput.AssyReadingIntervalSecs = "_VARIES_";
                if (FormInput.IsReference != dbEntry.IsReference) FormInput.IsReference = "_VARIES_";
                if (FormInput.TechnicalDetails != dbEntry.TechnicalDetails) FormInput.TechnicalDetails = "_VARIES_";
                if (FormInput.PowerSupplyDetails != dbEntry.PowerSupplyDetails) FormInput.PowerSupplyDetails = "_VARIES_";
                if (FormInput.HSEDetails != dbEntry.HSEDetails) FormInput.HSEDetails = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";

                if (FormInput.AssemblyType_Id != dbEntry.AssemblyType_Id) { FormInput.AssemblyType_Id = "_VARIES_"; FormInput.AssyTypeName = "_VARIES_"; }
                else FormInput.AssyTypeName = dbEntry.AssyTypeName;
                if (FormInput.AssemblyStatus_Id != dbEntry.AssemblyStatus_Id) { FormInput.AssemblyStatus_Id = "_VARIES_"; FormInput.AssyStatusName = "_VARIES_"; }
                else FormInput.AssyStatusName = dbEntry.AssyStatusName;
                if (FormInput.AssemblyModel_Id != dbEntry.AssemblyModel_Id) { FormInput.AssemblyModel_Id = "_VARIES_"; FormInput.AssyModelName = "_VARIES_"; }
                else FormInput.AssyModelName = dbEntry.AssyModelName;
                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
                if (FormInput.AssignedToLocation_Id != dbEntry.AssignedToLocation_Id) { FormInput.AssignedToLocation_Id = "_VARIES_"; FormInput.AssignedToLocation = "_VARIES_"; }
                else FormInput.AssignedToLocation = dbEntry.AssignedToLocation.LocName;
            });

            ClearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Assembly");

            $("#AssyName").val(FormInput.AssyName);
            $("#AssyAltName").val(FormInput.AssyAltName);
            $("#AssyAltName2").val(FormInput.AssyAltName2);
            $("#AssyGlobalX").val(FormInput.AssyGlobalX);
            $("#AssyGlobalY").val(FormInput.AssyGlobalY);
            $("#AssyGlobalZ").val(FormInput.AssyGlobalZ);
            $("#AssyLocalXDesign").val(FormInput.AssyLocalXDesign);
            $("#AssyLocalYDesign").val(FormInput.AssyLocalYDesign);
            $("#AssyLocalZDesign").val(FormInput.AssyLocalZDesign);
            $("#AssyLocalXAsBuilt").val(FormInput.AssyLocalXAsBuilt);
            $("#AssyLocalYAsBuilt").val(FormInput.AssyLocalYAsBuilt);
            $("#AssyLocalZAsBuilt").val(FormInput.AssyLocalZAsBuilt);
            $("#AssyStationing").val(FormInput.AssyStationing);
            $("#AssyLength").val(FormInput.AssyLength);
            $("#AssyReadingIntervalSecs").val(FormInput.AssyReadingIntervalSecs);
            if (FormInput.IsReference == true) $("#IsReference").prop("checked", true);
            $("#TechnicalDetails").val(FormInput.TechnicalDetails);
            $("#PowerSupplyDetails").val(FormInput.PowerSupplyDetails);
            $("#HSEDetails").val(FormInput.HSEDetails);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);

            if (FormInput.AssemblyType_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.AssemblyType_Id, name: FormInput.AssyTypeName }], true);
            if (FormInput.AssemblyStatus_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.AssemblyStatus_Id, name: FormInput.AssyStatusName }], true);
            if (FormInput.AssemblyModel_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.AssemblyModel_Id, name: FormInput.AssyModelName }], true);
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
               
        editRecord.AssyName = ($("#AssyName").data("ismodified")) ? $("#AssyName").val() : CurrRecord.AssyName;
        editRecord.AssyAltName = ($("#AssyAltName").data("ismodified")) ? $("#AssyAltName").val() : CurrRecord.AssyAltName;
        editRecord.AssyAltName2 = ($("#AssyAltName2").data("ismodified")) ? $("#AssyAltName2").val() : CurrRecord.AssyAltName2;
        editRecord.AssyGlobalX = ($("#AssyGlobalX").data("ismodified")) ? $("#AssyGlobalX").val() : CurrRecord.AssyGlobalX;
        editRecord.AssyGlobalY = ($("#AssyGlobalY").data("ismodified")) ? $("#AssyGlobalY").val() : CurrRecord.AssyGlobalY;
        editRecord.AssyGlobalZ = ($("#AssyGlobalZ").data("ismodified")) ? $("#AssyGlobalZ").val() : CurrRecord.AssyGlobalZ;
        editRecord.AssyLocalXDesign = ($("#AssyLocalXDesign").data("ismodified")) ? $("#AssyLocalXDesign").val() : CurrRecord.AssyLocalXDesign;
        editRecord.AssyLocalYDesign = ($("#AssyLocalYDesign").data("ismodified")) ? $("#AssyLocalYDesign").val() : CurrRecord.AssyLocalYDesign;
        editRecord.AssyLocalZDesign = ($("#AssyLocalZDesign").data("ismodified")) ? $("#AssyLocalZDesign").val() : CurrRecord.AssyLocalZDesign;
        editRecord.AssyLocalXAsBuilt = ($("#AssyLocalXAsBuilt").data("ismodified")) ? $("#AssyLocalXAsBuilt").val() : CurrRecord.AssyLocalXAsBuilt;
        editRecord.AssyLocalYAsBuilt = ($("#AssyLocalYAsBuilt").data("ismodified")) ? $("#AssyLocalYAsBuilt").val() : CurrRecord.AssyLocalYAsBuilt;
        editRecord.AssyLocalZAsBuilt = ($("#AssyLocalZAsBuilt").data("ismodified")) ? $("#AssyLocalZAsBuilt").val() : CurrRecord.AssyLocalZAsBuilt;
        editRecord.AssyStationing = ($("#AssyStationing").data("ismodified")) ? $("#AssyStationing").val() : CurrRecord.AssyStationing;
        editRecord.AssyLength = ($("#AssyLength").data("ismodified")) ? $("#AssyLength").val() : CurrRecord.AssyLength;
        editRecord.AssyReadingIntervalSecs = ($("#AssyReadingIntervalSecs").data("ismodified")) ? $("#AssyReadingIntervalSecs").val() : CurrRecord.AssyReadingIntervalSecs;
        editRecord.IsReference = ($("#IsReference").data("ismodified")) ? (($("#IsReference").prop("checked")) ? true : false) : CurrRecord.IsReference;
        editRecord.TechnicalDetails = ($("#TechnicalDetails").data("ismodified")) ? $("#TechnicalDetails").val() : CurrRecord.TechnicalDetails;
        editRecord.PowerSupplyDetails = ($("#PowerSupplyDetails").data("ismodified")) ? $("#PowerSupplyDetails").val() : CurrRecord.PowerSupplyDetails;
        editRecord.HSEDetails = ($("#HSEDetails").data("ismodified")) ? $("#HSEDetails").val() : CurrRecord.HSEDetails;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.AssemblyType_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.AssemblyType_Id;
        editRecord.AssemblyStatus_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.AssemblyStatus_Id;
        editRecord.AssemblyModel_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.AssemblyModel_Id;
        editRecord.AssignedToProject_Id = (MagicSuggests[3].isModified) ? magicResults[3] : CurrRecord.AssignedToProject_Id;
        editRecord.AssignedToLocation_Id = (MagicSuggests[4].isModified) ? magicResults[4] : CurrRecord.AssignedToLocation_Id;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/AssemblyDbSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
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
        type: "POST", url: "/AssemblyDbSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
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
        RefreshTable(TableMain, "/AssemblyDbSrv/GetByTypeIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
            "POST", MsFilterByProject.getValue(), [], MsFilterByType.getValue());
        $("#ChBoxShowDeleted").bootstrapToggle("enable")
    }

}


//---------------------------------------Helper Methods--------------------------------------//


