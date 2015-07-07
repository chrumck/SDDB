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
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Document", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        var selectedRows = TableMain.rows(".ui-selected").data();
        if (selectedRows.length == 0) showModalNothingSelected();
        else { IsCreate = false; FillFormForEdit(); }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows == 0) showModalNothingSelected();
        else showModalDelete(noOfRows);
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6]).visible(true);
        TableMain.columns([7, 8, 9, 10, 11]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6]).visible(false);
        TableMain.columns([7, 8, 9, 10, 11]).visible(true);
    });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/DocumentTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByType).on('selectionchange', function (e, m) { RefreshMainView(); });

    //Initialize MagicSuggest msFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on('selectionchange', function (e, m) { RefreshMainView(); });


    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (($(this).prop("checked")) ? false : true)
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        else $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        RefreshMainView();
    });

    //TableMain Documents
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "DocName", name: "DocName" },//1
            { data: "DocAltName", name: "DocAltName" },//2
            { data: "DocTypeName", name: "DocTypeName" },//3  
            { data: "DocLastVersion", name: "DocLastVersion" },//4
            { data: "AuthorPerson", render: function (data, type, full, meta) { return data.Initials }, name: "AuthorPerson" },//5
            { data: "ReviewerPerson", render: function (data, type, full, meta) { return data.Initials }, name: "ReviewerPerson" }, //6
            { data: "AssignedToProject", render: function (data, type, full, meta) { return data.ProjectName }, name: "AssignedToProject" }, //7
            { data: "AssyTypeName", name: "AssyTypeName" },//8
            { data: "CompTypeName", name: "CompTypeName" },//9
            { data: "DocFilePath", name: "DocFilePath" },//10
            { data: "Comments", name: "Comments" },//11
            { data: "IsActive", name: "IsActive" },//12
            { data: "DocumentType_Id", name: "DocumentType_Id" },//13
            { data: "AuthorPerson_Id", name: "AuthorPerson_Id" },//14
            { data: "ReviewerPerson_Id", name: "ReviewerPerson_Id" },//15
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//16
            { data: "RelatesToAssyType_Id", name: "RelatesToAssyType_Id" },//17
            { data: "RelatesToCompType_Id", name: "RelatesToCompType_Id" },//18
        ],
        columnDefs: [
            { targets: [0, 12, 13, 14, 15, 16, 17, 18], visible: false }, // - never show
            { targets: [0, 12, 13, 14, 15, 16, 17, 18], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 4, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [7, 8, 9, 10, 11], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 10], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [11], className: "hidden-xs hidden-sm hidden-md" } // - second set of columns
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
    addToMSArray(MagicSuggests, "DocumentType_Id", "/DocumentTypeSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AuthorPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "ReviewerPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "RelatesToAssyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "RelatesToCompType_Id", "/ComponentTypeSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        IsCreate = false;
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", IsCreate) && msIsValid(MagicSuggests)) SubmitEdits();
    });


});

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var IsCreate = false;
var MsFilterByProject = {}; var MsFilterByType = {};
var MagicSuggests = [];
var CurrRecord = {};

//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/DocumentSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.DocName = data[0].DocName;
            CurrRecord.DocAltName = data[0].DocAltName;
            CurrRecord.DocLastVersion = data[0].DocLastVersion;
            CurrRecord.DocFilePath = data[0].DocFilePath;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.DocumentType_Id = data[0].DocumentType_Id;
            CurrRecord.AuthorPerson_Id = data[0].AuthorPerson_Id;
            CurrRecord.ReviewerPerson_Id = data[0].ReviewerPerson_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            CurrRecord.RelatesToAssyType_Id = data[0].RelatesToAssyType_Id;
            CurrRecord.RelatesToCompType_Id = data[0].RelatesToCompType_Id;

            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.DocName != dbEntry.DocName) FormInput.DocName = "_VARIES_";
                if (FormInput.DocAltName != dbEntry.DocAltName) FormInput.DocAltName = "_VARIES_";
                if (FormInput.DocLastVersion != dbEntry.DocLastVersion) FormInput.DocLastVersion = "_VARIES_";
                if (FormInput.DocFilePath != dbEntry.DocFilePath) FormInput.DocFilePath = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";

                if (FormInput.DocumentType_Id != dbEntry.DocumentType_Id) { FormInput.DocumentType_Id = "_VARIES_"; FormInput.DocTypeName = "_VARIES_"; }
                else FormInput.DocTypeName = dbEntry.DocTypeName;
                if (FormInput.AuthorPerson_Id != dbEntry.AuthorPerson_Id) { FormInput.AuthorPerson_Id = "_VARIES_"; FormInput.AuthorPerson = "_VARIES_"; }
                else FormInput.AuthorPerson = dbEntry.AuthorPerson.FirstName + " " + dbEntry.AuthorPerson.LastName;
                if (FormInput.ReviewerPerson_Id != dbEntry.ReviewerPerson_Id) { FormInput.ReviewerPerson_Id = "_VARIES_"; FormInput.ReviewerPerson = "_VARIES_"; }
                else FormInput.ReviewerPerson = dbEntry.ReviewerPerson.FirstName + " " + dbEntry.ReviewerPerson.LastName;
                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
                if (FormInput.RelatesToAssyType_Id != dbEntry.RelatesToAssyType_Id) { FormInput.RelatesToAssyType_Id = "_VARIES_"; FormInput.RelatesToAssyType = "_VARIES_"; }
                else FormInput.RelatesToAssyType = dbEntry.AssyTypeName;
                if (FormInput.RelatesToCompType_Id != dbEntry.RelatesToCompType_Id) { FormInput.RelatesToCompType_Id = "_VARIES_"; FormInput.RelatesToCompType = "_VARIES_"; }
                else FormInput.RelatesToCompType = dbEntry.CompTypeName;
            });

            clearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Document");

            $("#DocName").val(FormInput.DocName);
            $("#DocAltName").val(FormInput.DocAltName);
            $("#DocLastVersion").val(FormInput.DocLastVersion);
            $("#DocFilePath").val(FormInput.DocFilePath);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);
            if (FormInput.DocumentType_Id != null)  MagicSuggests[0].addToSelection([{ id: FormInput.DocumentType_Id, name: FormInput.DocTypeName }], true);
            if (FormInput.AuthorPerson_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.AuthorPerson_Id, name: FormInput.AuthorPerson }], true);
            if (FormInput.ReviewerPerson_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.ReviewerPerson_Id, name: FormInput.ReviewerPerson }], true);
            if (FormInput.AssignedToProject_Id != null) MagicSuggests[3].addToSelection([{ id: FormInput.AssignedToProject_Id, name: FormInput.AssignedToProject }], true);
            if (FormInput.RelatesToAssyType_Id != null) MagicSuggests[4].addToSelection([{ id: FormInput.RelatesToAssyType_Id, name: FormInput.RelatesToAssyType }], true);
            if (FormInput.RelatesToCompType_Id != null) MagicSuggests[5].addToSelection([{ id: FormInput.RelatesToCompType_Id, name: FormInput.RelatesToCompType }], true);

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                disableUniqueMs(MagicSuggests, false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
                disableUniqueMs(MagicSuggests, true);
            }

            $("#MainView").addClass("hide");
            $("#EditFormView").removeClass("hide");
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
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
    if (IsCreate == true) ids = ["newEntryId"];

    var magicResults = [];
    $.each(MagicSuggests, function (i, ms) {
        var msValue = (ms.getSelection().length != 0) ? (ms.getSelection())[0].id : null;
        magicResults.push(msValue);
    });

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;

        editRecord.DocName = ($("#DocName").data("ismodified")) ? $("#DocName").val() : CurrRecord.DocName;
        editRecord.DocAltName = ($("#DocAltName").data("ismodified")) ? $("#DocAltName").val() : CurrRecord.DocAltName;
        editRecord.DocLastVersion = ($("#DocLastVersion").data("ismodified")) ? $("#DocLastVersion").val() : CurrRecord.DocLastVersion;
        editRecord.DocFilePath = ($("#DocFilePath").data("ismodified")) ? $("#DocFilePath").val() : CurrRecord.DocFilePath;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.DocumentType_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.DocumentType_Id;
        editRecord.AuthorPerson_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.AuthorPerson_Id;
        editRecord.ReviewerPerson_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.ReviewerPerson_Id;
        editRecord.AssignedToProject_Id = (MagicSuggests[3].isModified) ? magicResults[3] : CurrRecord.AssignedToProject_Id;
        editRecord.RelatesToAssyType_Id = (MagicSuggests[4].isModified) ? magicResults[4] : CurrRecord.RelatesToAssyType_Id;
        editRecord.RelatesToCompType_Id = (MagicSuggests[5].isModified) ? magicResults[5] : CurrRecord.RelatesToCompType_Id;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/DocumentSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
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
            showModalAJAXFail(xhr, status, error);
        });
}

//Delete Records from DB
function DeleteRecords() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/DocumentSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshMainView(); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//refresh view after magicsuggest update
function RefreshMainView() {
    if (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        refreshTable(TableMain, "/DocumentSrv/GetByTypeIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
            "POST", MsFilterByProject.getValue(), [], MsFilterByType.getValue());
        $("#ChBoxShowDeleted").bootstrapToggle("enable")
    }

}

//---------------------------------------Helper Methods--------------------------------------//


