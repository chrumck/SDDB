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
var TableLogEntryAssysAdd = {}; var TableLogEntryAssysRemove = {};
var TableLogEntryPersonsAdd = {}; var TableLogEntryPersonsRemove = {};
var IsCreate = false;
var MsFilterByProject = {}; var MsFilterByType = {}; var MsFilterByPerson = {};
var MagicSuggests = [];
var CurrRecord = {}; var currIds = [];

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        IsCreate = true; currIds = [];
        FillFormForCreate("EditForm", MagicSuggests, "Create Log Entry", "MainView");
        MagicSuggests[3].disable(); MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw();
        TableLogEntryAssysRemove.clear().search("").draw();
        TableLogEntryPersonsAdd.clear().search("").draw();
        TableLogEntryPersonsRemove.clear().search("").draw();
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
            { data: "AssignedToLocation", render: function (data, type, full, meta) { return data.LocName }, name: "AssignedToLocation" }, //6
            { data: "EventName", name: "EventName" },//7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive", name: "IsActive" },//9
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//10
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//11
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//12
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" },//13
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" },//14
        ],
        columnDefs: [
            { targets: [0, 9, 10, 11, 12, 13, 14], visible: false }, // - never show
            { targets: [0, 1, 4, 9, 10, 11, 12, 13, 14], searchable: false },  //"orderable": false, "visible": false
            { targets: [5, 6], className: "hidden-xs" }, // - first set of columns
            { targets: [3, 4], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7, 8], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns
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
    $(MagicSuggests[2]).on('selectionchange', function (e, m) {
        if (this.getValue().length == 0) {
            MagicSuggests[3].disable(); MagicSuggests[3].clear(true);
            MagicSuggests[4].disable(); MagicSuggests[4].clear(true);
            TableLogEntryAssysAdd.clear().search("").draw();
            TableLogEntryAssysRemove.clear().search("").draw();
        }
        else {
            MagicSuggests[3].enable(); MagicSuggests[3].clear(true);
            MagicSuggests[4].enable(); MagicSuggests[4].clear(true);
        }
    });

    AddToMSArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    $(MagicSuggests[3]).on('selectionchange', function (e, m) {
        if (this.getValue().length == 0) {
            TableLogEntryAssysAdd.clear().search("").draw();
            TableLogEntryAssysRemove.clear().search("").draw();
        }
        else {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
            FillLogEntryAssys()
                .always(function () {
                    $("#ModalWait").modal("hide");
                    $("#AssignedToLocation_Id input").focus();
                })
                .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
        }
    });

    AddToMSArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });


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

    //------------------------------------DataTables - Log Entry Assemblies ---

    //TableLogEntryAssysAdd
    TableLogEntryAssysAdd = $("#TableLogEntryAssysAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" }//1
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false }  //"orderable": false, "visible": false
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
        pageLength: 10
    });

    //TableLogEntryAssysRemove
    TableLogEntryAssysRemove = $("#TableLogEntryAssysRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" }//1
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false }  //"orderable": false, "visible": false
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
        pageLength: 10
    });
    
    //------------------------------------DataTables - Log Entry Persons ---

    //TableLogEntryPersonsAdd
    TableLogEntryPersonsAdd = $("#TableLogEntryPersonsAdd").DataTable({
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
        pageLength: 10
    });

    //TableLogEntryPersonsRemove
    TableLogEntryPersonsRemove = $("#TableLogEntryPersonsRemove").DataTable({
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
        pageLength: 10
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

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    currIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/PersonLogEntrySrv/GetByIds", timeout: 20000,
        data: { ids: currIds, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
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
            CurrRecord.AssignedToLocation_Id = data[0].AssignedToLocation_Id;
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
                if (FormInput.AssignedToLocation_Id != dbEntry.AssignedToLocation_Id) { FormInput.AssignedToLocation_Id = "_VARIES_"; FormInput.AssignedToLocation = "_VARIES_"; }
                else FormInput.AssignedToLocation = dbEntry.AssignedToLocation.LocName;
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
            if (FormInput.AssignedToLocation_Id != null) MagicSuggests[3].addToSelection([{ id: FormInput.AssignedToLocation_Id, name: FormInput.AssignedToLocation }], true);
            if (FormInput.AssignedToProjectEvent_Id != null) MagicSuggests[4].addToSelection([{ id: FormInput.AssignedToProjectEvent_Id, name: FormInput.EventName }], true);

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                DisableUniqueMs(MagicSuggests, false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
                DisableUniqueMs(MagicSuggests, true);
            }

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
            FillLogEntryAssys()
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
                    FillLogEntryPersons()
                        .always(function () { $("#ModalWait").modal("hide"); })
                        .done(function () {
                            $("#MainView").addClass("hide");
                            $("#EditFormView").removeClass("hide");
                        })
                        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
                })
                .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
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
        editRecord.AssignedToLocation_Id = (MagicSuggests[3].isModified) ? magicResults[3] : CurrRecord.AssignedToLocation_Id;
        editRecord.AssignedToProjectEvent_Id = (MagicSuggests[4].isModified) ? magicResults[4] : CurrRecord.AssignedToProjectEvent_Id;
        
        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/PersonLogEntrySrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
            EditLogEntryAssys((IsCreate) ? data.ReturnIds : ids)
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function () {
                $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
                EditLogEntryPersons((IsCreate) ? data.ReturnIds : ids)
                    .always(function () { $("#ModalWait").modal("hide"); })
                    .done(function () {
                        RefreshMainView();
                        IsCreate = false;
                        $("#MainView").removeClass("hide");
                        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
                    })
                    .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
            })
            .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
        })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
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

//---------------------------------------------------------------------------------------------

//Fill Log Entry Assys to add and to remove
function FillLogEntryAssys() {

    var deferred1 = $.Deferred();

    if (currIds.length == 1) {
        $.when(
            $.ajax({
                type: "GET", url: "/PersonLogEntrySrv/GetPrsLogEntryAssysNot", timeout: 20000,
                data: { logEntryId: currIds[0], locId: MagicSuggests[3].getValue}, dataType: "json"
            }),
            $.ajax({
                type: "GET", url: "/PersonLogEntrySrv/GetPrsLogEntryAssys", timeout: 20000,
                data: { logEntryId: currIds[0] }, dataType: "json"
            })
        )
        .done(function (done1, done2) {
            TableLogEntryAssysAdd.clear().search(""); TableLogEntryAssysAdd.rows.add(done1[0].data).order([1, 'asc']).draw();
            TableLogEntryAssysRemove.clear().search(""); TableLogEntryAssysRemove.rows.add(done2[0].data).order([1, 'asc']).draw();
            deferred1.resolve();
        })
        .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }
    else {
        $.when(
            $.ajax({
                type: "GET", url: "AssemblyDbSrv/LookupByLocDTables", timeout: 20000,
                data: { locId: MagicSuggests[3].getValue, getActive: true }, dataType: "json"
            }),
            $.ajax({
                type: "GET", url: "AssemblyDbSrv/LookupByLocDTables", timeout: 20000,
                data: { getActive: true }, dataType: "json",
            })
        )
        .done(function (done1, done2) {
            TableLogEntryAssysAdd.clear().search(""); TableLogEntryAssysAdd.rows.add(done1[0]).order([1, 'asc']).draw();
            if (!IsCreate) { TableLogEntryAssysRemove.clear().search(""); TableLogEntryAssysRemove.rows.add(done2[0]).order([1, 'asc']).draw(); }
            else TableLogEntryAssysRemove.clear().search("");

            deferred1.resolve();
        })
        .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    return deferred1.promise();
}

//Submit Log Entry Assemblies Edits to SDDB
function EditLogEntryAssys(logEntryIds) {

    var dbRecordsAdd = TableLogEntryAssysAdd.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsRemove = TableLogEntryAssysRemove.cells(".ui-selected", "Id:name").data().toArray();

    var deferred1 = $.Deferred();
    if (dbRecordsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({
            type: "POST", url: "/PersonLogEntrySrv/EditPrsLogEntryAssys", timeout: 20000,
            data: { logEntryIds: logEntryIds, assyIds: dbRecordsAdd, isAdd: true }, dataType: "json"
        })
            .done(function () { deferred1.resolve(); })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    var deferred2 = $.Deferred();
    if (dbRecordsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({
                type: "POST", url: "/PersonLogEntrySrv/EditPrsLogEntryAssys", timeout: 20000,
                data: { logEntryIds: logEntryIds, assyIds: dbRecordsRemove, isAdd: false }, dataType: "json"
            })
                .done(function () { deferred2.resolve(); })
                .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
        }, 500);
    }

    var deferred3 = $.Deferred();
    $.when(deferred1, deferred2)
        .done(function () { deferred3.resolve();})
        .fail(function (xhr, status, error) { deferred3.reject(xhr, status, error); });

    return deferred3.promise();
}

//---------------------------------------------------------------------------------------------

//Fill Log Entry Persons to add and to remove
function FillLogEntryPersons() {

    var deferred1 = $.Deferred();

    if (currIds.length == 1) {
        $.when(
            $.ajax({
                type: "GET", url: "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", timeout: 20000,
                data: { logEntryId: currIds[0] }, dataType: "json"
            }),
            $.ajax({
                type: "GET", url: "/PersonLogEntrySrv/GetPrsLogEntryPersons", timeout: 20000,
                data: { logEntryId: currIds[0] }, dataType: "json"
            })
        )
        .done(function (done1, done2) {
            TableLogEntryPersonsAdd.clear().search(""); TableLogEntryPersonsAdd.rows.add(done1[0].data).order([1, 'asc']).draw();
            TableLogEntryPersonsRemove.clear().search(""); TableLogEntryPersonsRemove.rows.add(done2[0].data).order([1, 'asc']).draw();
            deferred1.resolve();
        })
        .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }
    else {
        $.ajax({ type: "GET", url: "/PersonSrv/Get", timeout: 20000, data: { getActive: true }, dataType: "json" })
            .done(function (data) {
                TableLogEntryPersonsAdd.clear().search(""); TableLogEntryPersonsAdd.rows.add(data.data).order([1, 'asc']).draw();
                if (!IsCreate) { TableLogEntryPersonsRemove.clear().search(""); TableLogEntryPersonsRemove.rows.add(data.data).order([1, 'asc']).draw(); }
                else TableLogEntryPersonsRemove.clear().search("");

                deferred1.resolve();
            })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    return deferred1.promise();
}

//Submit Log Entry Persons Edits to SDDB
function EditLogEntryPersons(logEntryIds) {

    var dbRecordsAdd = TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name").data().toArray();
    var dbRecordsRemove = TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name").data().toArray();

    var deferred1 = $.Deferred();
    if (dbRecordsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({
            type: "POST", url: "/PersonLogEntrySrv/EditPrsLogEntryPersons", timeout: 20000,
            data: { logEntryIds: logEntryIds, personIds: dbRecordsAdd, isAdd: true }, dataType: "json"
        })
            .done(function () { deferred1.resolve(); })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    var deferred2 = $.Deferred();
    if (dbRecordsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({
                type: "POST", url: "/PersonLogEntrySrv/EditPrsLogEntryPersons", timeout: 20000,
                data: { logEntryIds: logEntryIds, personIds: dbRecordsRemove, isAdd: false }, dataType: "json"
            })
                .done(function () { deferred2.resolve(); })
                .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
        }, 500);
    }

    var deferred3 = $.Deferred();
    $.when(deferred1, deferred2)
        .done(function () { deferred3.resolve(); })
        .fail(function (xhr, status, error) { deferred3.reject(xhr, status, error); });

    return deferred3.promise();
}


//---------------------------------------------------------------------------------------------

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


