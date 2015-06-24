/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.3.js" />
/// <reference path="../jquery-2.1.3.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var IsCreate = false;
var MsFilterByProject = {}; var MsFilterByType = {}; var MsFilterByAssy = {};
var MagicSuggests = [];
var CurrRecord = {};

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        IsCreate = true;
        FillFormForCreateGeneric("EditForm", MagicSuggests, "Create Component", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        var selectedRows = TableMain.rows(".ui-selected").data();
        if (selectedRows.length == 0) ShowModalNothingSelected();
        else { IsCreate = false; FillFormForEdit(); }
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
        TableMain.columns([8, 9, 10, 11, 12]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12]).visible(true);
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

    //Initialize MagicSuggest MsFilterByAssy
    MsFilterByAssy = $("#MsFilterByAssy").magicSuggest({
        data: "/AssemblyDbSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: MsFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByAssy).on('selectionchange', function (e, m) { RefreshMainView(); });
        
    //---------------------------------------DataTables------------
        
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
            { data: "AssignedToAssemblyDb", render: function (data, type, full, meta) { return data.AssyName }, name: "AssignedToAssemblyDb" }, //8
            { data: "PositionInAssy", name: "PositionInAssy" },//9
            { data: "ProgramAddress", name: "ProgramAddress" },//10
            { data: "CalibrationReqd", name: "CalibrationReqd" },//11
            { data: "Comments", name: "Comments" },//12
            //------------------------------------------------never visible
            { data: "IsActive", name: "IsActive" },//13
            { data: "ComponentType_Id", name: "ComponentType_Id" },//14
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//15
            { data: "ComponentModel_Id", name: "ComponentModel_Id" },//16
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//17
            { data: "AssignedToAssemblyDb_Id", name: "AssignedToAssemblyDb_Id" }//18
        ],
        columnDefs: [
            { targets: [0, 13, 14, 15, 16, 17, 18], visible: false }, // - never show
            { targets: [0, 11, 13, 14, 15, 16, 17, 18], searchable: false },  //"orderable": false, "visible": false
            { targets: [4, 6, 7], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [2, 3], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [8, 9, 10, 11, 12], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 10, 11], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [12], className: "hidden-xs hidden-sm hidden-md" } // - second set of columns
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

    //Enable modified field detection
    $(".modifiable").change(function () { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    AddToMSArray(MagicSuggests, "ComponentType_Id", "/ComponentTypeSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "ComponentStatus_Id", "/ComponentStatusSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "ComponentModel_Id", "/ComponentModelSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToAssemblyDb_Id", "/AssemblyDbSrv/Lookup", 1);

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

    //--------------------------------------View Initialization------------------------------------//

    if (typeof assyId !== "undefined" && assyId != "") {
        $.ajax({
            type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 20000,
            data: { ids: [assyId], getActive: true }, dataType: "json",
            beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
        })
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                MsFilterByAssy.setSelection([{ id: data[0].Id, name: data[0].AssyName,  }]);
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }



    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    $("#EditFormCreateMultiple").addClass("hide");

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
            CurrRecord.PositionInAssy = data[0].PositionInAssy;
            CurrRecord.ProgramAddress = data[0].ProgramAddress;
            CurrRecord.CalibrationReqd = data[0].CalibrationReqd;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.ComponentType_Id = data[0].ComponentType_Id;
            CurrRecord.ComponentStatus_Id = data[0].ComponentStatus_Id;
            CurrRecord.ComponentModel_Id = data[0].ComponentModel_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            CurrRecord.AssignedToAssemblyDb_Id = data[0].AssignedToAssemblyDb_Id;
            
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.CompName != dbEntry.CompName) FormInput.CompName = "_VARIES_";
                if (FormInput.CompAltName != dbEntry.CompAltName) FormInput.CompAltName = "_VARIES_";
                if (FormInput.CompAltName2 != dbEntry.CompAltName2) FormInput.CompAltName2 = "_VARIES_";
                if (FormInput.PositionInAssy != dbEntry.PositionInAssy) FormInput.PositionInAssy = "_VARIES_";
                if (FormInput.ProgramAddress != dbEntry.ProgramAddress) FormInput.ProgramAddress = "_VARIES_";
                if (FormInput.CalibrationReqd != dbEntry.CalibrationReqd) FormInput.CalibrationReqd = "_VARIES_";
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
                if (FormInput.AssignedToAssemblyDb_Id != dbEntry.AssignedToAssemblyDb_Id) { FormInput.AssignedToAssemblyDb_Id = "_VARIES_"; FormInput.AssignedToAssemblyDb = "_VARIES_"; }
                else FormInput.AssignedToAssemblyDb = dbEntry.AssignedToAssemblyDb.AssyName;
            });

            ClearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Component");

            $("#CompName").val(FormInput.CompName);
            $("#CompAltName").val(FormInput.CompAltName);
            $("#CompAltName2").val(FormInput.CompAltName2);
            $("#PositionInAssy").val(FormInput.PositionInAssy);
            $("#ProgramAddress").val(FormInput.ProgramAddress);
            if (FormInput.CalibrationReqd == true) $("#CalibrationReqd").prop("checked", true);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);

            if (FormInput.ComponentType_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.ComponentType_Id, name: FormInput.CompTypeName }], true);
            if (FormInput.ComponentStatus_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.ComponentStatus_Id, name: FormInput.CompStatusName }], true);
            if (FormInput.ComponentModel_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.ComponentModel_Id, name: FormInput.CompModelName }], true);
            if (FormInput.AssignedToProject_Id != null) MagicSuggests[3].addToSelection([{ id: FormInput.AssignedToProject_Id, name: FormInput.AssignedToProject }], true);
            if (FormInput.AssignedToAssemblyDb_Id != null) MagicSuggests[4].addToSelection([{ id: FormInput.AssignedToAssemblyDb_Id, name: FormInput.AssignedToAssemblyDb }], true);


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
        editRecord.PositionInAssy = ($("#PositionInAssy").data("ismodified")) ? $("#PositionInAssy").val() : CurrRecord.PositionInAssy;
        editRecord.ProgramAddress = ($("#ProgramAddress").data("ismodified")) ? $("#ProgramAddress").val() : CurrRecord.ProgramAddress;
        editRecord.CalibrationReqd = ($("#CalibrationReqd").data("ismodified")) ? (($("#CalibrationReqd").prop("checked")) ? true : false) : CurrRecord.CalibrationReqd;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.ComponentType_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.ComponentType_Id;
        editRecord.ComponentStatus_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.ComponentStatus_Id;
        editRecord.ComponentModel_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.ComponentModel_Id;
        editRecord.AssignedToProject_Id = (MagicSuggests[3].isModified) ? magicResults[3] : CurrRecord.AssignedToProject_Id;
        editRecord.AssignedToAssemblyDb_Id = (MagicSuggests[4].isModified) ? magicResults[4] : CurrRecord.AssignedToAssemblyDb_Id;

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
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error);
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
        && MsFilterByAssy.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        RefreshTable(TableMain, "/ComponentSrv/GetByTypeAssyIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
            "POST", MsFilterByProject.getValue(), [], MsFilterByType.getValue(), [], MsFilterByAssy.getValue());
        $("#ChBoxShowDeleted").bootstrapToggle("enable")
    }

}


//---------------------------------------Helper Methods--------------------------------------//


