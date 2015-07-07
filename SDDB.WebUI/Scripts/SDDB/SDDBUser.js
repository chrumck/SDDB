/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.3.js" />
/// <reference path="../jquery-2.1.3.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global  Properties-----------------------------------//

var TableMain = {};
var TableDBRolesAdd = {};
var TableDBRolesRemove = {};
var IsCreate = false;
var MagicSuggests = [];
var CurrRecord = {};

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        IsCreate = true;
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create SDDB User", "MainView");
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

    //Wire up BtnEditRoles 
    $("#BtnEditRoles").click(function () {
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows == 0) showModalNothingSelected();
        else FillDBRoleForEdit(noOfRows);
    });


    //---------------------------------------DataTables------------

    //TableMain DBUsers
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },
            { data: "LastName", name: "LastName" },
            { data: "FirstName", name: "FirstName" },
            { data: "UserName", name: "UserName" },
            { data: "Email", name: "Email" },
            { data: "LDAPAuthenticated", name: "LDAPAuthenticated" }
        ],
        columnDefs: [
            { targets: [0, 5], searchable: false },  //"orderable": false, "visible": false
            { targets: [0], visible: false },
            { targets: [4, 5], className: "hidden-xs hidden-sm" }
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
    addToMSArray(MagicSuggests, "Id", "/PersonSrv/PersonsWoDBUser", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        IsCreate = false;
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
        window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", IsCreate) && msIsValid(MagicSuggests)) SubmitEdits();
    });


    //----------------------------------------DBRolesView----------------------------------------//

    //Wire Up EditFormBtnCancel
    $("#DBRolesViewBtnCancel, #DBRolesViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#DBRolesView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#DBRolesViewBtnOk").click(function () {
        if (TableDBRolesAdd.rows(".ui-selected").data().length +
            TableDBRolesRemove.rows(".ui-selected").data().length == 0) showModalNothingSelected();
        else SubmitRolesEdits();
    });

    //---------------------------------------DataTables----------------------------------

    //TableDBRolesAdd
    TableDBRolesAdd = $("#TableDBRolesAdd").DataTable({
        columns: [
            { data: "Name", name: "Name" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered:"(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //TableDBRolesRemove
    TableDBRolesRemove = $("#TableDBRolesRemove").DataTable({
        columns: [
            { data: "Name", name: "Name" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered:"(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });


    //--------------------------------------View Initialization------------------------------------//

    refreshTblGenWrp(TableMain, "/DBUserSrv/Get", {},"GET");


    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/DBUserSrv/GetByIds", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.Id = data[0].Id;
            CurrRecord.UserName = data[0].UserName;
            CurrRecord.Email = data[0].Email;
            CurrRecord.LDAPAuthenticated = data[0].LDAPAuthenticated;

            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.Id != dbEntry.Id) { FormInput.Id = "_VARIES_"; FormInput.PersonName = "_VARIES_"; }
                else FormInput.PersonName = dbEntry.FirstName + " " + dbEntry.LastName;

                if (FormInput.UserName != dbEntry.UserName) FormInput.UserName = "_VARIES_";
                if (FormInput.Email != dbEntry.Email) FormInput.Email = "_VARIES_";
                if (FormInput.LDAPAuthenticated != dbEntry.LDAPAuthenticated) FormInput.LDAPAuthenticated = "_VARIES_";
            });

            clearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit SDDB User");

            if (FormInput.Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.Id, name: FormInput.PersonName }], true);
            $("#UserName").val(FormInput.UserName);
            $("#Email").val(FormInput.Email);
            if (FormInput.LDAPAuthenticated == true) $("#LDAPAuthenticated").prop("checked", true);

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                disableUniqueMs(MagicSuggests, false);
            }
            else {

                $("[data-val-dbisunique]").prop("disabled", true);
                disableUniqueMs(MagicSuggests, true);
            }

            MagicSuggests[0].disable();
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

    var magicResults = [];
    $.each(MagicSuggests, function (i, ms) {
        var msValue = (ms.getSelection().length != 0) ? (ms.getSelection())[0].id : null;
        magicResults.push(msValue);
    });

    var editRecords = [];
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    if (IsCreate == true) ids = [magicResults[0]];

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;

        editRecord.UserName = ($("#UserName").data("ismodified")) ? $("#UserName").val() : CurrRecord.UserName;
        editRecord.Email = ($("#Email").data("ismodified")) ? $("#Email").val() : CurrRecord.Email;
        editRecord.LDAPAuthenticated = ($("#LDAPAuthenticated").data("ismodified")) ?
            (($("#LDAPAuthenticated").prop("checked")) ? true : false) : CurrRecord.LDAPAuthenticated;
        editRecord.Password = ($("#Password").data("ismodified")) ? $("#Password").val() : CurrRecord.Password;
        editRecord.PasswordConf = ($("#PasswordConf").data("ismodified")) ? $("#PasswordConf").val() : CurrRecord.PasswordConf;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/DBUserSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            refreshTblGenWrp(TableMain, "/DBUserSrv/Get", {}, "GET");
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
        type: "POST", url: "/DBUserSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { refreshTblGenWrp(TableMain, "/DBUserSrv/Get", {}, "GET"); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//---------------------------------------------------------------------------------------------

//Fill DBRolesForm for Edit
function FillDBRoleForEdit(noOfRows) {
    if (noOfRows == 1) {
        var selectedRecord = TableMain.row(".ui-selected").data()
        $("#DBRolesViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);

        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

        $.when(
            $.ajax({ type: "GET", url: "/DBUserSrv/GetUserRolesNot", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json" }),
            $.ajax({ type: "GET", url: "/DBUserSrv/GetUserRoles", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json" })
            )
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (done1, done2) {
                TableDBRolesAdd.clear().search(""); TableDBRolesAdd.rows.add(done1[0]).order([0, 'asc']).draw();
                TableDBRolesRemove.clear().search(""); TableDBRolesRemove.rows.add(done2[0]).order([0, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#DBRolesView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
    else {
        $("#DBRolesViewPanel").text("_MULTIPLE_")

        $.ajax({
            type: "GET", url: "/DBUserSrv/GetAllRoles", timeout: 20000, dataType: "json",
            beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
        })
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                TableDBRolesAdd.clear().search(""); TableDBRolesAdd.rows.add(data).order([0, 'asc']).draw();
                TableDBRolesRemove.clear().search(""); TableDBRolesRemove.rows.add(data).order([0, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#DBRolesView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
}

//Submit Roles Edits to SDDB
function SubmitRolesEdits() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsAdd = TableDBRolesAdd.cells(".ui-selected", "Name:name").data().toArray();
    var dbRecordsRemove = TableDBRolesRemove.cells(".ui-selected", "Name:name").data().toArray();

    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

    var deferred1 = $.Deferred();
    if (dbRecordsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({
            type: "POST", url: "/DBUserSrv/EditRoles", timeout: 20000,
            data: { ids: ids, dbRoles: dbRecordsAdd, isAdd: true }, dataType: "json"
        })
            .done(function () { deferred1.resolve(); })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    var deferred2 = $.Deferred();
    if (dbRecordsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({
                type: "POST", url: "/DBUserSrv/EditRoles", timeout: 20000,
                data: { ids: ids, dbRoles: dbRecordsRemove, isAdd: false }, dataType: "json"
            })
                .done(function () { deferred2.resolve(); })
                .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
        }, 500);
    }

    $.when(deferred1, deferred2)
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () {
            refreshTblGenWrp(TableMain, "/DBUserSrv/Get", {}, "GET");
            $("#MainView").removeClass("hide");
            $("#DBRolesView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//---------------------------------------Helper Methods--------------------------------------//






