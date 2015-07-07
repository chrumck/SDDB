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
var MsFilterByProject = {};
var MagicSuggests = [];
var CurrRecord = {};


$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        IsCreate = true;
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Event", "MainView");
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

    //TableMain ProjectEvents
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "EventName", name: "EventName" },//1
            //------------------------------------------------first set of columns
            { data: "EventAltName", name: "EventAltName" },//2
            { data: "AssignedToProject", render: function (data, type, full, meta) { return data.ProjectName }, name: "AssignedToProject" }, //3
            { data: "EventCreated", name: "EventCreated" },//4
            { data: "CreatedByPerson", render: function (data, type, full, meta) { return data.Initials }, name: "CreatedByPerson" },//5
            { data: "EventClosed", name: "EventClosed" },//6
            { data: "ClosedByPerson", render: function (data, type, full, meta) { return data.Initials }, name: "ClosedByPerson" }, //7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive", name: "IsActive" },//9
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//10
            { data: "CreatedByPerson_Id", name: "CreatedByPerson_Id" },//11
            { data: "ClosedByPerson_Id", name: "ClosedByPerson_Id" },//12
        ],
        columnDefs: [
            { targets: [0, 9, 10, 11, 12], visible: false }, // - never show
            { targets: [0, 9, 10, 11, 12], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 4, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6, 7, 8], className: "hidden-xs hidden-sm hidden-md" } // - first set of columns
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
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "CreatedByPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "ClosedByPerson_Id", "/PersonSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        IsCreate = false;
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Enable DateTimePicker
    $("[data-val-dbisdatetimeiso]").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", IsCreate) && msIsValid(MagicSuggests)) SubmitEdits();
    });


    //--------------------------------------View Initialization------------------------------------//

    RefreshMainView();

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/ProjectEventSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.EventName = data[0].EventName;
            CurrRecord.EventAltName = data[0].EventAltName;
            CurrRecord.EventCreated = data[0].EventCreated;
            CurrRecord.EventClosed = data[0].EventClosed;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            CurrRecord.CreatedByPerson_Id = data[0].CreatedByPerson_Id;
            CurrRecord.ClosedByPerson_Id = data[0].ClosedByPerson_Id;
            
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.EventName != dbEntry.EventName) FormInput.EventName = "_VARIES_";
                if (FormInput.EventAltName != dbEntry.EventAltName) FormInput.EventAltName = "_VARIES_";
                if (FormInput.EventCreated != dbEntry.EventCreated) FormInput.EventCreated = "_VARIES_";
                if (FormInput.EventClosed != dbEntry.EventClosed) FormInput.EventClosed = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";

                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
                if (FormInput.CreatedByPerson_Id != dbEntry.CreatedByPerson_Id) { FormInput.CreatedByPerson_Id = "_VARIES_"; FormInput.CreatedByPerson = "_VARIES_"; }
                else FormInput.CreatedByPerson = dbEntry.CreatedByPerson.FirstName + " " + dbEntry.CreatedByPerson.LastName;
                if (FormInput.ClosedByPerson_Id != dbEntry.ClosedByPerson_Id) { FormInput.ClosedByPerson_Id = "_VARIES_"; FormInput.ClosedByPerson = "_VARIES_"; }
                else FormInput.ClosedByPerson = dbEntry.ClosedByPerson.FirstName + " " + dbEntry.ClosedByPerson.LastName;
            });

            clearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Event");

            $("#EventName").val(FormInput.EventName);
            $("#EventAltName").val(FormInput.EventAltName);
            $("#EventCreated").val(FormInput.EventCreated);
            $("#EventClosed").val(FormInput.EventClosed);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);
            if (FormInput.AssignedToProject_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.AssignedToProject_Id, name: FormInput.AssignedToProject }], true);
            if (FormInput.CreatedByPerson_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.CreatedByPerson_Id, name: FormInput.CreatedByPerson }], true);
            if (FormInput.ClosedByPerson_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.ClosedByPerson_Id, name: FormInput.ClosedByPerson }], true);
            
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

        editRecord.EventName = ($("#EventName").data("ismodified")) ? $("#EventName").val() : CurrRecord.EventName;
        editRecord.EventAltName = ($("#EventAltName").data("ismodified")) ? $("#EventAltName").val() : CurrRecord.EventAltName;
        editRecord.EventCreated = ($("#EventCreated").data("ismodified")) ? $("#EventCreated").val() : CurrRecord.EventCreated;
        editRecord.EventClosed = ($("#EventClosed").data("ismodified")) ? $("#EventClosed").val() : CurrRecord.EventClosed;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.AssignedToProject_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.AssignedToProject_Id;
        editRecord.CreatedByPerson_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.CreatedByPerson_Id;
        editRecord.ClosedByPerson_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.ClosedByPerson_Id;
        
        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/ProjectEventSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
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
        type: "POST", url: "/ProjectEventSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshMainView(); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//refresh view after magicsuggest update
function RefreshMainView() {
    refreshTable(TableMain, "/ProjectEventSrv/GetByProjectIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
        "POST", MsFilterByProject.getValue());
    $("#ChBoxShowDeleted").bootstrapToggle("enable")
}

//---------------------------------------Helper Methods--------------------------------------//


