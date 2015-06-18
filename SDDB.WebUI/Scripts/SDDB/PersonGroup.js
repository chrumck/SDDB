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
        FillFormForCreate("EditForm", MagicSuggests, "Create Person Group", "MainView");
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

    //Wire Up BtnEditGroupManagers 
    $("#BtnEditGroupManagers").click(function () {
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows == 0) ShowModalNothingSelected();
        else FillGroupManagersForEdit(noOfRows);
    });
               
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (($(this).prop("checked")) ? false : true)
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        else $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        RefreshTable(TableMain, "/PersonGroupSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
    });

    //TableMain PersonGroups
    TableMain = $("#TableMain").DataTable({
        ajax: { url: "/PersonGroupSrv/Get?getActive=" + (($("#ChBoxShowDeleted").prop("checked")) ? false : true) },
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" },//2
            { data: "Comments", name: "Comments" },//3
            { data: "IsActive", name: "IsActive" }//4
        ],
        columnDefs: [
            { targets: [0, 4], visible: false }, // - never show
            { targets: [0, 4], searchable: false },  //"orderable": false, "visible": false
            { targets: [3], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" } // - first set of columns
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

    //Enable DatePicker

    //Initialize MagicSuggest Array

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


    //----------------------------------------GroupManagersView----------------------------------------//

    //Wire Up GroupManagersViewBtnCancel
    $("#GroupManagersViewBtnCancel, #GroupManagersViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#GroupManagersView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up GroupManagersViewBtnOk
    $("#GroupManagersViewBtnOk").click(function () {
        if (TableGroupManagersAdd.rows(".ui-selected").data().length +
            TableGroupManagersRemove.rows(".ui-selected").data().length == 0) ShowModalNothingSelected();
        else SubmitGroupManagersEdits();
    });

    //---------------------------------------DataTables------------

    //TableGroupManagersAdd
    TableGroupManagersAdd = $("#TableGroupManagersAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //TableGroupManagersRemove
    TableGroupManagersRemove = $("#TableGroupManagersRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" }//3
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });


});
//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var TableGroupManagersAdd = {};
var TableGroupManagersRemove = {};
var IsCreate = false;
var MagicSuggests = [];
var CurrRecord = {};


//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/PersonGroupSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.PrsGroupName = data[0].PrsGroupName;
            CurrRecord.PrsGroupAltName = data[0].PrsGroupAltName;           
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;

            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.PrsGroupName != dbEntry.PrsGroupName) FormInput.PrsGroupName = "_VARIES_";
                if (FormInput.PrsGroupAltName != dbEntry.PrsGroupAltName) FormInput.PrsGroupAltName = "_VARIES_";                
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";
            });

            ClearFormInputs("EditForm");
            $("#EditFormLabel").text("Edit Person Group");

            $("#PrsGroupName").val(FormInput.PrsGroupName);
            $("#PrsGroupAltName").val(FormInput.PrsGroupAltName);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);

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

        editRecord.PrsGroupName = ($("#PrsGroupName").data("ismodified")) ? $("#PrsGroupName").val() : CurrRecord.PrsGroupName;
        editRecord.PrsGroupAltName = ($("#PrsGroupAltName").data("ismodified")) ? $("#PrsGroupAltName").val() : CurrRecord.PrsGroupAltName;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/PersonGroupSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            RefreshTable(TableMain, "/PersonGroupSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
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
        type: "POST", url: "/PersonGroupSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshTable(TableMain, "/PersonGroupSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true)); })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//---------------------------------------------------------------------------------------------

//Fill GroupManagers For Edit 
function FillGroupManagersForEdit(noOfRows) {
    if (noOfRows == 1) {
        var selectedRecord = TableMain.row(".ui-selected").data()
        $("#GroupManagersViewPanel").text(selectedRecord.PrsGroupName);

        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

        $.when(
            $.ajax({ type: "GET", url: "/PersonGroupSrv/GetGroupManagersNot", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json" }),
            $.ajax({ type: "GET", url: "/PersonGroupSrv/GetGroupManagers", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json" })
            )
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (done1, done2) {
                TableGroupManagersAdd.clear().search(""); TableGroupManagersAdd.rows.add(done1[0].data).order([1, 'asc']).draw();
                TableGroupManagersRemove.clear().search(""); TableGroupManagersRemove.rows.add(done2[0].data).order([1, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#GroupManagersView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }
    else {
        $("#GroupManagersViewPanel").text("_MULTIPLE_")

        $.ajax({
            type: "GET", url: "/PersonSrv/Get", timeout: 20000, data: { getActive: true }, dataType: "json",
            beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
        })
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                TableGroupManagersAdd.clear().search(""); TableGroupManagersAdd.rows.add(data.data).order([1, 'asc']).draw();
                TableGroupManagersRemove.clear().search(""); TableGroupManagersRemove.rows.add(data.data).order([1, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#GroupManagersView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }
}

//Submit GroupManagers Edits to SDDB
function SubmitGroupManagersEdits() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsAdd = TableGroupManagersAdd.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsRemove = TableGroupManagersRemove.cells(".ui-selected", "Id:name").data().toArray();

    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

    var deferred1 = $.Deferred();
    if (dbRecordsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({
            type: "POST", url: "/PersonGroupSrv/EditGroupManagers", timeout: 20000,
            data: { personIds: dbRecordsAdd, groupIds: ids, isAdd: true }, dataType: "json"
        })
            .done(function () { deferred1.resolve(); })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    var deferred2 = $.Deferred();
    if (dbRecordsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({
                type: "POST", url: "/PersonGroupSrv/EditGroupManagers", timeout: 20000,
                data: { personIds: dbRecordsRemove, groupIds: ids, isAdd: false }, dataType: "json"
            })
                .done(function () { deferred2.resolve(); })
                .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
        }, 500);
    }

    $.when(deferred1, deferred2)
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () {
            RefreshTable(TableMain, "/PersonGroupSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
            $("#MainView").removeClass("hide");
            $("#GroupManagersView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}


//---------------------------------------Helper Methods--------------------------------------//



