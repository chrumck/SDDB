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
var MsFilterByProject = {}; var MsFilterByType = {}; var MsFilterByPerson = {};
var MagicSuggests = [];
var CurrRecord = {};

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

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { RefreshMainView(); });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { RefreshMainView(); });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/PersonActivityTypeSrv/Lookup",
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

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByPerson).on('selectionchange', function (e, m) { RefreshMainView(); });
   
        
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

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            { data: "EnteredByPerson", render: function (data, type, full, meta) { return data.LastName + " " + data.Initials }, name: "EnteredByPerson" }, //2
            { data: "ActivityTypeName", name: "ActivityTypeName" },//3
            { data: "ManHours", name: "ManHours" },//4
            { data: "AssignedToProject", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject" }, //5
            { data: "EventName", name: "EventName" },//6
            { data: "Comments", name: "Comments" },//7
            //------------------------------------------------never visible
            { data: "IsActive", name: "IsActive" },//8
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//9
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//10
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//11
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" },//12
        ],
        columnDefs: [
            { targets: [0, 8, 9, 10, 11, 12], visible: false }, // - never show
            { targets: [0, 1, 4, 8, 9, 10, 11, 12], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 4, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6, 7], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns
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


    //Initialize DateTimePicker
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    AddToMSArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);

    AddToMSArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1);
    
    AddToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);

    AddToMSArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });

    $(MagicSuggests[2]).on('selectionchange', function (e, m) {
        if (this.getValue().length == 0) { MagicSuggests[3].disable(); MagicSuggests[3].clear(true); }
        else { MagicSuggests[3].enable(); MagicSuggests[3].clear(true); }
    });
    


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

    $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#FilterDateEnd").val(moment().format("YYYY-MM-DD"));

    //if (typeof assyId !== "undefined" && assyId != "") {
    //    $.ajax({
    //        type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 20000,
    //        data: { ids: [assyId], getActive: true }, dataType: "json",
    //        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    //    })
    //        .always(function () { $("#ModalWait").modal("hide"); })
    //        .done(function (data) {
    //            MsFilterByAssy.setSelection([{ id: data[0].Id, name: data[0].AssyName,  }]);
    //        })
    //        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    //}

    

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//FillFormForCreate
function FillFormForCreate() {
    ClearFormInputs("EditForm", MagicSuggests);
    $("#EditFormLabel").text("Create Person Log Entry");
    $("[data-val-dbisunique]").prop("disabled", false);
    DisableUniqueMs(MagicSuggests, false);
    MagicSuggests[3].disable();
    $(".modifiable").data("ismodified", true); SetMsAsModified(MagicSuggests, true);
    $("#EditFormGroupIsActive").addClass("hide"); $("#IsActive").prop("checked", true)
    $("#MainView").addClass("hide");
    $("#EditFormView").removeClass("hide");
}

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/PersonLogEntrySrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.LogEntryDateTime = data[0].LogEntryDateTime;
            CurrRecord.ManHours = data[0].ManHours;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.EnteredByPerson_Id = data[0].EnteredByPerson_Id;
            CurrRecord.PersonActivityType_Id = data[0].PersonActivityType_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            CurrRecord.AssignedToProjectEvent_Id = data[0].AssignedToProjectEvent_Id;
                        
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.LogEntryDateTime != dbEntry.LogEntryDateTime) FormInput.LogEntryDateTime = "_VARIES_";
                if (FormInput.ManHours != dbEntry.ManHours) FormInput.ManHours = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";

                if (FormInput.EnteredByPerson_Id != dbEntry.EnteredByPerson_Id) { FormInput.EnteredByPerson_Id = "_VARIES_"; FormInput.EnteredByPerson = "_VARIES_"; }
                else FormInput.EnteredByPerson = dbEntry.EnteredByPerson.FirstName + " " + dbEntry.EnteredByPerson.LastName + " " + dbEntry.EnteredByPerson.Initials;
                if (FormInput.PersonActivityType_Id != dbEntry.PersonActivityType_Id) { FormInput.PersonActivityType_Id = "_VARIES_"; FormInput.ActivityTypeName = "_VARIES_"; }
                else FormInput.ActivityTypeName = dbEntry.ActivityTypeName;
                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
                if (FormInput.AssignedToProjectEvent_Id != dbEntry.AssignedToProjectEvent_Id) { FormInput.AssignedToProjectEvent_Id = "_VARIES_"; FormInput.EventName = "_VARIES_"; }
                else FormInput.EventName = dbEntry.EventName;
            });

            ClearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Person Log Entry");

            $("#LogEntryDateTime").val(FormInput.LogEntryDateTime);
            $("#ManHours").val(FormInput.ManHours);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);

            if (FormInput.EnteredByPerson_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.EnteredByPerson_Id, name: FormInput.EnteredByPerson }], true);
            if (FormInput.PersonActivityType_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.PersonActivityType_Id, name: FormInput.ActivityTypeName }], true);
            if (FormInput.AssignedToProject_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.AssignedToProject_Id, name: FormInput.AssignedToProject }], true);
            if (FormInput.AssignedToProjectEvent_Id != null) MagicSuggests[3].addToSelection([{ id: FormInput.AssignedToProjectEvent_Id, name: FormInput.EventName }], true);

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
    if (IsCreate == true) ids = ["newEntryId"];

    var magicResults = [];
    $.each(MagicSuggests, function (i, ms) {
        var msValue = (ms.getSelection().length != 0) ? (ms.getSelection())[0].id : null;
        magicResults.push(msValue);
    });

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;

        editRecord.LogEntryDateTime = ($("#LogEntryDateTime").data("ismodified")) ? $("#LogEntryDateTime").val() : CurrRecord.LogEntryDateTime;
        editRecord.ManHours = ($("#ManHours").data("ismodified")) ? $("#ManHours").val() : CurrRecord.ManHours;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.EnteredByPerson_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.EnteredByPerson_Id;
        editRecord.PersonActivityType_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.PersonActivityType_Id;
        editRecord.AssignedToProject_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.AssignedToProject_Id;
        editRecord.AssignedToProjectEvent_Id = (MagicSuggests[3].isModified) ? magicResults[3] : CurrRecord.AssignedToProjectEvent_Id;
        
        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/PersonLogEntrySrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
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
        type: "POST", url: "/PersonLogEntrySrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshMainView(); })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//refresh view after magicsuggest update
function RefreshMainView() {
    if ($("#FilterDateStart").val() == "" || $("#FilterDateEnd").val() == "" ||
            (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0
                && MsFilterByPerson.getValue().length == 0)
        ){
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = moment($("#FilterDateEnd").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");
        RefreshTable(TableMain, "/PersonLogEntrySrv/GetByFilterIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
            "POST", MsFilterByProject.getValue(), [], MsFilterByType.getValue(), [], [],
            MsFilterByPerson.getValue(), $("#FilterDateStart").val(), endDate);
        $("#ChBoxShowDeleted").bootstrapToggle("enable")
    }

}


//---------------------------------------Helper Methods--------------------------------------//


