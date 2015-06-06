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
        RefreshTable(TableMain, "/ProjectSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
    });

    //TableMain Projects
    TableMain = $("#TableMain").DataTable({
        ajax: { url: "/ProjectSrv/Get?getActive=" + (($("#ChBoxShowDeleted").prop("checked")) ? false : true) },
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectAltName", name: "ProjectAltName" },//2
            { data: "ProjectCode", name: "ProjectCode" },//3
            { data: "ProjectManager_Id", name: "ProjectManager_Id" },//4
            {
                data: "ProjectManager",
                render: function (data, type, full, meta) { return data.FirstName + " " + data.LastName; },
                name: "PMFullName"
            }, //5
            { data: "Comments", name: "Comments" },//6
            { data: "IsActive", name: "IsActive" }//7
        ],
        columnDefs: [
            { targets: [0, 4, 7], visible: false }, // - never show
            { targets: [0, 4, 7], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6], className: "hidden-xs hidden-sm hidden-md" } // - first set of columns
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
    AddToMSArray(MagicSuggests, "ProjectManager_Id", "/PersonSrv/LookupAll", 1);

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
var MagicSuggests = [];
var CurrRecord = {};

//--------------------------------------Main Methods---------------------------------------//

//FillFormForCreate
function FillFormForCreate() {
    ClearFormInputs("EditForm", MagicSuggests);
    $("#EditFormLabel").text("Create Project");
    $("[data-val-dbisunique]").prop("disabled", false);
    DisableUniqueMs(MagicSuggests, false);
    $(".modifiable").data("ismodified", true);
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
        type: "POST", url: "/ProjectSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.ProjectName = data[0].ProjectName;
            CurrRecord.ProjectAltName = data[0].ProjectAltName;
            CurrRecord.ProjectCode = data[0].ProjectCode;
            CurrRecord.ProjectManager_Id = data[0].ProjectManager_Id;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;

            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.ProjectName != dbEntry.ProjectName) FormInput.ProjectName = "_VARIES_";
                if (FormInput.ProjectAltName != dbEntry.ProjectAltName) FormInput.ProjectAltName = "_VARIES_";
                if (FormInput.ProjectCode != dbEntry.ProjectCode) FormInput.ProjectCode = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";

                if (FormInput.ProjectManager_Id != dbEntry.ProjectManager_Id) { FormInput.ProjectManager_Id = "_VARIES_"; FormInput.ProjectManager = "_VARIES_"; }
                else FormInput.ProjectManager = dbEntry.ProjectManager.FirstName + " " + dbEntry.ProjectManager.LastName;

            });

            ClearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Project");

            $("#ProjectName").val(FormInput.ProjectName);
            $("#ProjectAltName").val(FormInput.ProjectAltName);
            $("#ProjectCode").val(FormInput.ProjectCode);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);
            if (FormInput.ProjectManager_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.ProjectManager_Id, name: FormInput.ProjectManager }], true);

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

    var magicResults = [];
    $.each(MagicSuggests, function (i, ms) {
        var msValue = (ms.getSelection().length != 0) ? (ms.getSelection())[0].id : null;
        magicResults.push(msValue);
    });

    var editRecords = [];
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    if (IsCreate == true) ids = ["newEntryId"];

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;

        editRecord.ProjectName = ($("#ProjectName").data("ismodified")) ? $("#ProjectName").val() : CurrRecord.ProjectName;
        editRecord.ProjectAltName = ($("#ProjectAltName").data("ismodified")) ? $("#ProjectAltName").val() : CurrRecord.ProjectAltName;
        editRecord.ProjectCode = ($("#ProjectCode").data("ismodified")) ? $("#ProjectCode").val() : CurrRecord.ProjectCode;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.ProjectManager_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.ProjectManager_Id;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/ProjectSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            RefreshTable(TableMain, "/ProjectSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
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
        type: "POST", url: "/ProjectSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshTable(TableMain, "/ProjectSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true)); })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//---------------------------------------Helper Methods--------------------------------------//


