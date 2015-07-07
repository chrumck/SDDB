/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain;
var TableLogEntryAssysAdd; var TableLogEntryAssysRemove;
var TableLogEntryPersonsAdd; var TableLogEntryPersonsRemove;
var MsFilterByProject; var MsFilterByType; var MsFilterByPerson;
var MagicSuggests = [];
var CurrRecord = {
    Id: null,
    LogEntryDateTime: null,
    EnteredByPerson_Id: null,
    PersonActivityType_Id: null,
    ManHours: null,
    AssignedToProject_Id: null,
    AssignedToLocation_Id: null,
    AssignedToProjectEvent_Id: null,
    Comments: null,
    IsActive_bl: null,
};
var CurrIds = []; var FileCurrNames = [];
var GetActive = true;
var SelectedRecord;
var DlToken; var DlTimer; var DlAttempts;
var XHR = new window.XMLHttpRequest();

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Log Entry", "MainView");
        MagicSuggests[3].disable(); MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw(); TableLogEntryAssysRemove.clear().search("").draw();
        $("#EditFormBtnOkFiles").removeClass("disabled");

        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
        fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
            "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
            "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
            "GET", "/PersonSrv/Get", { getActive: true })
            .always(function () { $("#ModalWait").modal("hide"); })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            if (GetActive) $("#EditFormGroupIsActive").addClass("hide");
            else $("#EditFormGroupIsActive").removeClass("hide");

            if (CurrIds.length > 1) $("#EditFormBtnOkFiles").addClass("disabled");
            else $("#EditFormBtnOkFiles").removeClass("disabled");

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            $.when(
                fillFormForEditGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                    GetActive, "EditForm", "Edit Person Log Entry", MagicSuggests),

                fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                    "GET", "/PersonSrv/Get", { getActive: true })
                )
                .then(function (currRecord) {
                    CurrRecord = currRecord;
                    return fillFormForRelatedGeneric(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys", { logEntryId: CurrIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot", { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                        "GET", "AssemblyDbSrv/LookupByLocDTables", { getActive: true });

                })
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else showModalDelete(CurrIds.length);
    });

    //Wire up BtnEditLogEntryFiles 
    $("#BtnEditLogEntryFiles").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length != 1) showModalSelectOne();
        else {
            SelectedRecord = TableMain.row(".ui-selected").data();
            fillLogEntryFilesForm();
        }
    });

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/PersonActivityTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByType).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });
   
        
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) { GetActive = true; $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary"); }
        else { GetActive = false; $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger"); }
        refreshMainView();
    });

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            { data: "EnteredByPerson_", render: function (data, type, full, meta) { return data.LastName + " " + data.Initials }, name: "EnteredByPerson_" }, //2
            { data: "PersonActivityType_", render: function (data, type, full, meta) { return data.ActivityTypeName }, name: "PersonActivityType_" }, //3
            { data: "ManHours", name: "ManHours" },//4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //5
            { data: "AssignedToLocation_", render: function (data, type, full, meta) { return data.LocName }, name: "AssignedToLocation_" }, //6
            { data: "AssignedToProjectEvent_", render: function (data, type, full, meta) { return data.EventName }, name: "AssignedToProjectEvent_" }, //7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//9
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

    //Initialize DateTimePicker
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    addToMSArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    addToMSArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    
    //Initialize MagicSuggest Array Event
    $(MagicSuggests[2]).on("selectionchange", function (e, m) {
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

    //Initialize MagicSuggest Array Event
    $(MagicSuggests[3]).on("selectionchange", function (e, m) {
        if (this.getValue().length == 0) {
            TableLogEntryAssysAdd.clear().search("").draw();
        }
        else {
            
            if (CurrIds.length == 1) {
                refreshTblGenWrp(TableLogEntryAssysAdd, "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                    { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] }, "GET")
                    .done(function () { $("#AssignedToLocation_Id input").focus(); });
            }
            else {
                refreshTblGenWrp(TableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
                { getActive: true, locId: MagicSuggests[3].getValue()[0] }, "GET")
                .done(function () { $("#AssignedToLocation_Id input").focus(); });
            }
        }
    });


    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            submitEdits();
        }
    });

    //Wire Up EditFormBtnOkFiles
    $("#EditFormBtnOkFiles").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            SelectedRecord = TableMain.row(".ui-selected").data();
            submitEdits().done(function () { setTimeout(fillLogEntryFilesForm, 200); });
        }
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

    //--------------------------------------LogEntryFilesView---------------------------------------//

    //Wire Up EditFormBtnCancel
    $("#LogEntryFilesViewBtnCancel, #LogEntryFilesViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#LogEntryFilesView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up LogEntryFilesBtnDload
    $("#LogEntryFilesBtnDload").click(function () {
        var names = TableLogEntryFiles.cells(".ui-selected", "Name:name").data().toArray();
        if (names.length == 0) showModalNothingSelected();
        else {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            DlToken = new Date().getTime(); DlAttempts = 60;
            DlTimer = window.setInterval(function () {
                if ((getCookie("DlToken") == DlToken) || (DlAttempts == 0)) {
                    $("#ModalWait").modal("hide");
                    window.clearInterval(DlTimer);
                    expireCookie("DlToken");
                    if (DlAttempts == 0) showModalFail("Server Error", "Server response timed out.");
                    else if ($("#LogEntryFilesIframe").contents().find("body").html() != "")
                        showModalFail("Server Error", $("#LogEntryFilesIframe").contents().find("body").html());
                }
                else DlAttempts--;
            }, 500);

            $("#LogEntryFilesIframe").contents().find("body").html("");

            var form = $('<form method="POST" action="/PersonLogEntrySrv/DownloadFiles" target="LogEntryFilesIframe">');
            form.append($('<input type="hidden" name="DlToken" value="' + DlToken + '">'));
            form.append($('<input type="hidden" name="id" value="' + CurrIds[0] + '">'));
            $.each(names, function (i, name) {form.append($('<input type="hidden" name="names[' + i + ']" value="' + name + '">')); });
            $("body").append(form);
            form.submit();
        }
    });

    //wire up LogEntryFilesBtnUpload
    $("#LogEntryFilesBtnUpload").on("change", function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {

                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                $("#ModalUpload").modal({ show: true, backdrop: "static", keyboard: false });
                $.ajax({
                    type: "POST", url: "/PersonLogEntrySrv/UploadFiles?id=" + CurrIds[0],
                    contentType: false, processData: false, data: data,
                    xhr: function () {
                        XHR.upload.addEventListener("progress", function (e) {
                            if (e.lengthComputable) {
                                var PROGRESS = "Progress: " + Math.round((e.loaded / e.total) * 100) + "%";
                                $("#ModalUploadBody").text(PROGRESS);
                            }
                        }, false); return XHR;
                    }
                })
                    .always(function () { $("#ModalUpload").modal("hide"); })
                    .done(function () { setTimeout(fillLogEntryFilesForm, 200); })
                    .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

            } else showModalFail("Browser Error","This browser doesn't support HTML5 file uploads!"); 
        }
        $(e.target).val("");
    });

    //Wire Up ModalUploadBtnAbort
    $("#ModalUploadBtnAbort").click(function () { XHR.abort(); $("#ModalUpload").modal("hide"); });

    //Wire Up LogEntryFilesBtnDelete 
    $("#LogEntryFilesBtnDelete").click(function () {
        FileCurrNames = TableLogEntryFiles.cells(".ui-selected", "Name:name").data().toArray();
        if (FileCurrNames.length == 0) showModalNothingSelected();
        else {
            $("#ModalDeleteFilesBody").text("Confirm deleting " + FileCurrNames.length + " file(s).");
            $("#ModalDeleteFiles").modal("show");
        }
    });

    //Get focus on ModalDeleteBtnCancel
    $("#ModalDeleteFiles").on("shown.bs.modal", function () { $("#ModalDeleteFilesBtnCancel").focus(); });

    //Wire Up ModalDeleteBtnCancel 
    $("#ModalDeleteFilesBtnCancel").click(function () { $("#ModalDeleteFiles").modal("hide"); });

    //Wire Up ModalDeleteFilesBtnOk
    $("#ModalDeleteFilesBtnOk").click(function () {
        $("#ModalDeleteFiles").modal("hide");
        $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
        $.ajax({
            type: "POST", url: "/PersonLogEntrySrv/DeleteFiles", timeout: 20000,
            data: { id: CurrIds[0], names: FileCurrNames }, dataType: "json"
        })
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function () { setTimeout(fillLogEntryFilesForm, 200); })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //------------------------------------DataTables - Log Entry Files ---

    //TableLogEntryFiles
    TableLogEntryFiles = $("#TableLogEntryFiles").DataTable({
        columns: [
            { data: "Name", name: "Name" },//0
            { data: "Size", name: "Size" },//1
            { data: "Modified", name: "Modified" }//2
        ],
        columnDefs: [
            { targets: [], visible: false }, // - never show
            { targets: [], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs" }
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
    //        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    //}

    

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function DeleteRecords() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
    $.ajax({ type: "POST", url: "/PersonLogEntrySrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json"})
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { refreshMainView(); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//refresh view after magicsuggest update
function refreshMainView() {
    if ($("#FilterDateStart").val() == "" || $("#FilterDateEnd").val() == "" ||
            (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0
                && MsFilterByPerson.getValue().length == 0)
        ){
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = moment($("#FilterDateEnd").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");

        refreshTblGenWrp(TableMain, "/PersonLogEntrySrv/GetByAltIds", { personIds: MsFilterByPerson.getValue(), 
                projectIds: MsFilterByProject.getValue(), typeIds: MsFilterByType.getValue(),
                startDate: $("#FilterDateStart").val(), endDate: endDate, getActive: GetActive}, "POST")
            .done(function () { $("#ChBoxShowDeleted").bootstrapToggle("enable"); })
    }
}

//submit edits to DB
function submitEdits() {
    var deferred0 = $.Deferred();

    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

    submitEditsGeneric(CurrIds, "EditForm", MagicSuggests, CurrRecord, "POST", "/PersonLogEntrySrv/Edit")
        .then(function (data) {

            var deferred1 = $.Deferred();

            var ids = (CurrIds.length == 0) ? data.ReturnIds : CurrIds;
            var idsAssysAdd = TableLogEntryAssysAdd.cells(".ui-selected", "Id:name").data().toArray();
            var idsAssysRemove = TableLogEntryAssysRemove.cells(".ui-selected", "Id:name").data().toArray();
            var idsPersonsAdd = TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name").data().toArray();
            var idsPersonsRemove = TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name").data().toArray();

            $.when(
                submitEditsForRelatedGeneric(ids, idsAssysAdd, idsAssysRemove, "/PersonLogEntrySrv/EditPrsLogEntryAssys"),
                submitEditsForRelatedGeneric(ids, idsPersonsAdd, idsPersonsRemove, "/PersonLogEntrySrv/EditPrsLogEntryPersons")
                )
                .done(function () { deferred1.resolve(); })
                .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });

            return deferred1.promise();
        })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () {
            refreshMainView();
            $("#MainView").removeClass("hide");
            $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); deferred0.reject(); });

    return deferred0.promise();
}

//Fill form showing log entry files
function fillLogEntryFilesForm() {
    var deferred0 = $.Deferred();

    $("#LogEntryFilesViewPanel").text(SelectedRecord.EnteredByPerson_.FirstName + " " +
        SelectedRecord.EnteredByPerson_.LastName + " - " + SelectedRecord.LogEntryDateTime);

    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

    refreshTableGeneric(TableLogEntryFiles, "/PersonLogEntrySrv/GetFiles", { id: CurrIds[0] }, "GET")
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () {
            $("#MainView").addClass("hide");
            $("#LogEntryFilesView").removeClass("hide");
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); deferred0.reject(); });

    return deferred0.promise();
}

//---------------------------------------Helper Methods--------------------------------------//

//gets cookie by name
function getCookie(name) {
    var parts = document.cookie.split(name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}

//expires cookie by name
function expireCookie(name) {
    document.cookie = encodeURIComponent(name) + "=deleted; expires=" + new Date(0).toUTCString();
}



