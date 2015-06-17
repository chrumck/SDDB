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
        FillFormForCreate("EditForm", MagicSuggests, "Create Person");
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

    //Wire Up BtnEditPrsProj 
    $("#BtnEditPrsProj").click(function () {
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows == 0) ShowModalNothingSelected();
        else FillPrsProjForEdit(noOfRows);
    });

    //Wire Up BtnEditPersonGroups 
    $("#BtnEditPersonGroups").click(function () {
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows == 0) ShowModalNothingSelected();
        else FillPersonGroupsForEdit(noOfRows);
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7 ]).visible(true);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(true);
    });

    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (($(this).prop("checked")) ? false : true)
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        else $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        RefreshTable(TableMain, "/PersonSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
    });

    //TableMain Persons
    TableMain = $("#TableMain").DataTable({
        ajax: { url: "/PersonSrv/Get?getActive=" + (($("#ChBoxShowDeleted").prop("checked")) ? false : true) },
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            { data: "Initials", name: "Initials" },//3
            { data: "Phone", name: "Phone" },//4
            { data: "PhoneMobile", name: "PhoneMobile" },//5
            { data: "Email", name: "Email" },//6
            { data: "Comments", name: "Comments" },//7
            { data: "IsActive", name: "IsActive" },//8
            { data: "IsCurrentEmployee", name: "IsCurrentEmployee" },//9
            { data: "EmployeePosition", name: "EmployeePosition" },//10
            { data: "IsSalaried", name: "IsSalaried" },//11
            { data: "EmployeeStart", name: "EmployeeStart" },//12
            { data: "EmployeeEnd", name: "EmployeeEnd" },//13
            { data: "EmployeeDetails", name: "EmployeeDetails" }//14
        ],
        columnDefs: [
            { targets: [0, 8], visible: false }, // - never show
            { targets: [0, 8, 9, 11, 12, 13], searchable: false },  //"orderable": false, "visible": false
            { targets: [4, 5, 6], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [9, 10, 11, 12, 13, 14], visible: false }, // - second set of columns - to toggle with options
            { targets: [10, 12, 13], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [11, 14], className: "hidden-xs hidden-sm hidden-md" } // - second set of columns
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
    $(".datepicker").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

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

    //----------------------------------------PrsProjView----------------------------------------//

    //Wire Up PrsProjViewBtnCancel
    $("#PrsProjViewBtnCancel, #PrsProjViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#PrsProjView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up PrsProjViewBtnOk
    $("#PrsProjViewBtnOk").click(function () {
        if (TableProjectsAdd.rows(".ui-selected").data().length +
            TableProjectsRemove.rows(".ui-selected").data().length == 0) ShowModalNothingSelected();
        else SubmitPrsProjEdits();
    });

    //---------------------------------------DataTables------------

    //TableProjectsAdd
    TableProjectsAdd = $("#TableProjectsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectCode", name: "ProjectCode" }//2
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

    //TableProjectsRemove
    TableProjectsRemove = $("#TableProjectsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectCode", name: "ProjectCode" }//2
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

    //----------------------------------------PersonGroupsView----------------------------------------//

    //Wire Up PersonGroupsViewBtnCancel
    $("#PersonGroupsViewBtnCancel, #PersonGroupsViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#PersonGroupsView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up PersonGroupsViewBtnOk
    $("#PersonGroupsViewBtnOk").click(function () {
        if (TablePersonGroupsAdd.rows(".ui-selected").data().length +
            TablePersonGroupsRemove.rows(".ui-selected").data().length == 0) ShowModalNothingSelected();
        else SubmitPersonGroupsEdits();
    });

    //---------------------------------------DataTables------------

    //TablePersonGroupsAdd
    TablePersonGroupsAdd = $("#TablePersonGroupsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
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

    //TablePersonGroupsRemove
    TablePersonGroupsRemove = $("#TablePersonGroupsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
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
var TableProjectsAdd = {};
var TableProjectsRemove = {};
var TablePersonGroupsAdd = {};
var TablePersonGroupsRemove = {};
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
        type: "POST", url: "/PersonSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.LastName = data[0].LastName;
            CurrRecord.FirstName = data[0].FirstName;
            CurrRecord.Initials = data[0].Initials;
            CurrRecord.Phone = data[0].Phone;
            CurrRecord.PhoneMobile = data[0].PhoneMobile;
            CurrRecord.Email = data[0].Email;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;
            CurrRecord.IsCurrentEmployee = data[0].IsCurrentEmployee;
            CurrRecord.EmployeePosition = data[0].EmployeePosition;
            CurrRecord.IsSalaried = data[0].IsSalaried;
            CurrRecord.EmployeeStart = data[0].EmployeeStart;
            CurrRecord.EmployeeEnd = data[0].EmployeeEnd;
            CurrRecord.EmployeeDetails = data[0].EmployeeDetails;

            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.LastName != dbEntry.LastName) FormInput.LastName = "_VARIES_";
                if (FormInput.FirstName != dbEntry.FirstName) FormInput.FirstName = "_VARIES_";
                if (FormInput.Initials != dbEntry.Initials) FormInput.Initials = "_VARIES_";
                if (FormInput.Phone != dbEntry.Phone) FormInput.Phone = "_VARIES_";
                if (FormInput.PhoneMobile != dbEntry.PhoneMobile) FormInput.PhoneMobile = "_VARIES_";
                if (FormInput.Email != dbEntry.Email) FormInput.Email = "_VARIES_";
                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";
                if (FormInput.IsCurrentEmployee != dbEntry.IsCurrentEmployee) FormInput.IsCurrentEmployee = "_VARIES_";
                if (FormInput.EmployeePosition != dbEntry.EmployeePosition) FormInput.EmployeePosition = "_VARIES_";
                if (FormInput.IsSalaried != dbEntry.IsSalaried) FormInput.IsSalaried = "_VARIES_";
                if (FormInput.EmployeeStart != dbEntry.EmployeeStart) FormInput.EmployeeStart = "_VARIES_";
                if (FormInput.EmployeeEnd != dbEntry.EmployeeEnd) FormInput.EmployeeEnd = "_VARIES_";
                if (FormInput.EmployeeDetails != dbEntry.EmployeeDetails) FormInput.EmployeeDetails = "_VARIES_";
            });

            ClearFormInputs("EditForm");
            $("#EditFormLabel").text("Edit Person");

            $("#LastName").val(FormInput.LastName);
            $("#FirstName").val(FormInput.FirstName);
            $("#Initials").val(FormInput.Initials);
            $("#Phone").val(FormInput.Phone);
            $("#PhoneMobile").val(FormInput.PhoneMobile);
            $("#Email").val(FormInput.Email);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);
            if (FormInput.IsCurrentEmployee == true) $("#IsCurrentEmployee").prop("checked", true);
            $("#EmployeePosition").val(FormInput.EmployeePosition);
            if (FormInput.IsSalaried == true) $("#IsSalaried").prop("checked", true);
            $("#EmployeeStart").val(FormInput.EmployeeStart);
            $("#EmployeeEnd").val(FormInput.EmployeeEnd);
            $("#EmployeeDetails").val(FormInput.EmployeeDetails);

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

        editRecord.LastName = ($("#LastName").data("ismodified")) ? $("#LastName").val() : CurrRecord.LastName;
        editRecord.FirstName = ($("#FirstName").data("ismodified")) ? $("#FirstName").val() : CurrRecord.FirstName;
        editRecord.Initials = ($("#Initials").data("ismodified")) ? $("#Initials").val() : CurrRecord.Initials;
        editRecord.Phone = ($("#Phone").data("ismodified")) ? $("#Phone").val() : CurrRecord.Phone;
        editRecord.PhoneMobile = ($("#PhoneMobile").data("ismodified")) ? $("#PhoneMobile").val() : CurrRecord.PhoneMobile;
        editRecord.Email = ($("#Email").data("ismodified")) ? $("#Email").val() : CurrRecord.Email;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;
        editRecord.IsCurrentEmployee = ($("#IsCurrentEmployee").data("ismodified")) ? (($("#IsCurrentEmployee").prop("checked")) ? true : false) : CurrRecord.IsCurrentEmployee;
        editRecord.EmployeePosition = ($("#EmployeePosition").data("ismodified")) ? $("#EmployeePosition").val() : CurrRecord.EmployeePosition;
        editRecord.IsSalaried = ($("#IsSalaried").data("ismodified")) ? (($("#IsSalaried").prop("checked")) ? true : false) : CurrRecord.IsSalaried;
        editRecord.EmployeeStart = ($("#EmployeeStart").data("ismodified")) ? $("#EmployeeStart").val() : CurrRecord.EmployeeStart;
        editRecord.EmployeeEnd = ($("#EmployeeEnd").data("ismodified")) ? $("#EmployeeEnd").val() : CurrRecord.EmployeeEnd;
        editRecord.EmployeeDetails = ($("#EmployeeDetails").data("ismodified")) ? $("#EmployeeDetails").val() : CurrRecord.EmployeeDetails;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/PersonSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            RefreshTable(TableMain, "/PersonSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
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
        type: "POST", url: "/PersonSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { RefreshTable(TableMain, "/PersonSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true)); })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//---------------------------------------------------------------------------------------------

//Fill PrsProjects For Edit 
function FillPrsProjForEdit(noOfRows) {
    if (noOfRows == 1) {
        var selectedRecord = TableMain.row(".ui-selected").data()
        $("#PrsProjViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);

        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

        $.when(
            $.ajax({ type: "GET", url: "/PersonSrv/GetPersonProjectsNot", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json"}),
            $.ajax({ type: "GET", url: "/PersonSrv/GetPersonProjects", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json"})
            )
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (done1, done2) {
                TableProjectsAdd.clear().search(""); TableProjectsAdd.rows.add(done1[0].data).order([1, 'asc']).draw();
                TableProjectsRemove.clear().search(""); TableProjectsRemove.rows.add(done2[0].data).order([1, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#PrsProjView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }
    else {
        $("#PrsProjViewPanel").text("_MULTIPLE_")

        $.ajax({
            type: "GET", url: "/ProjectSrv/Get", timeout: 20000, data: { getActive: true }, dataType: "json",
            beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
        })
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                TableProjectsAdd.clear().search(""); TableProjectsAdd.rows.add(data.data).order([1, 'asc']).draw();
                TableProjectsRemove.clear().search(""); TableProjectsRemove.rows.add(data.data).order([1, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#PrsProjView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }
}

//Submit Person Projects Edits to SDDB
function SubmitPrsProjEdits() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsAdd = TableProjectsAdd.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsRemove = TableProjectsRemove.cells(".ui-selected", "Id:name").data().toArray();

    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

    var deferred1 = $.Deferred();
    if (dbRecordsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({
            type: "POST", url: "/PersonSrv/EditPersonProjects", timeout: 20000,
            data: { personIds: ids, projectIds: dbRecordsAdd, isAdd: true }, dataType: "json"
        })
            .done( function () { deferred1.resolve();})
            .fail( function (xhr, status, error) { deferred1.reject(xhr, status, error); } );
    }

    var deferred2 = $.Deferred();
    if (dbRecordsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({
                type: "POST", url: "/PersonSrv/EditPersonProjects", timeout: 20000,
                data: { personIds: ids, projectIds: dbRecordsRemove, isAdd: false }, dataType: "json"
            })
                .done(function () { deferred2.resolve(); })
                .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
        }, 500);
    }

    $.when(deferred1, deferred2)
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () {
            RefreshTable(TableMain, "/PersonSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
            $("#MainView").removeClass("hide");
            $("#PrsProjView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) { ajaxResult = false; ShowModalAJAXFail(xhr, status, error); } );
}

//---------------------------------------------------------------------------------------------

//Fill PersonGroups For Edit 
function FillPersonGroupsForEdit(noOfRows) {
    if (noOfRows == 1) {
        var selectedRecord = TableMain.row(".ui-selected").data()
        $("#PersonGroupsViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);

        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

        $.when(
            $.ajax({ type: "GET", url: "/PersonSrv/GetPersonGroupsNot", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json" }),
            $.ajax({ type: "GET", url: "/PersonSrv/GetPersonGroups", timeout: 20000, data: { id: selectedRecord.Id }, dataType: "json" })
            )
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (done1, done2) {
                TablePersonGroupsAdd.clear().search(""); TablePersonGroupsAdd.rows.add(done1[0].data).order([1, 'asc']).draw();
                TablePersonGroupsRemove.clear().search(""); TablePersonGroupsRemove.rows.add(done2[0].data).order([1, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#PersonGroupsView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }
    else {
        $("#PersonGroupsViewPanel").text("_MULTIPLE_")

        $.ajax({
            type: "GET", url: "/PersonGroupSrv/Get", timeout: 20000, data: { getActive: true }, dataType: "json",
            beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
        })
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (data) {
                TablePersonGroupsAdd.clear().search(""); TablePersonGroupsAdd.rows.add(data.data).order([1, 'asc']).draw();
                TablePersonGroupsRemove.clear().search(""); TablePersonGroupsRemove.rows.add(data.data).order([1, 'asc']).draw();
                $("#MainView").addClass("hide");
                $("#PersonGroupsView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
    }
}

//Submit Person PersonGroups Edits to SDDB
function SubmitPersonGroupsEdits() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsAdd = TablePersonGroupsAdd.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsRemove = TablePersonGroupsRemove.cells(".ui-selected", "Id:name").data().toArray();

    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

    var deferred1 = $.Deferred();
    if (dbRecordsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({
            type: "POST", url: "/PersonSrv/EditPersonGroups", timeout: 20000,
            data: { personIds: ids, groupIds: dbRecordsAdd, isAdd: true }, dataType: "json"
        })
            .done(function () { deferred1.resolve(); })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    var deferred2 = $.Deferred();
    if (dbRecordsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({
                type: "POST", url: "/PersonSrv/EditPersonGroups", timeout: 20000,
                data: { personIds: ids, groupIds: dbRecordsRemove, isAdd: false }, dataType: "json"
            })
                .done(function () { deferred2.resolve(); })
                .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
        }, 500);
    }

    $.when(deferred1, deferred2)
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () {
            RefreshTable(TableMain, "/PersonSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
            $("#MainView").removeClass("hide");
            $("#PersonGroupsView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) { ajaxResult = false; ShowModalAJAXFail(xhr, status, error); });
}

//---------------------------------------Helper Methods--------------------------------------//



