/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />
/// <reference path="PersonLogEntryFiles.js" />
/// <reference path="Shared_Views.js" />

//--------------------------------------Global Properties------------------------------------//

var MsFilterByPerson,
    MsFilterByType,
    MsFilterByProject,
    MsFilterByAssy,

    TableLogEntryAssysAdd,
    TableLogEntryAssysRemove,
    TableLogEntryPersonsAdd,
    TableLogEntryPersonsRemove,

    ModalChngStsMs,
    ChngStsAssyIds = [];

var RecordTemplate = {
    Id: "RecordTemplateId",
    LogEntryDateTime: null,
    EnteredByPerson_Id: null,
    PersonActivityType_Id: null,
    ManHours: null,
    AssignedToProject_Id: null,
    AssignedToLocation_Id: null,
    AssignedToProjectEvent_Id: null,
    QcdByPerson_Id: null,
    QcdDateTime: null,
    Comments: null,
    IsActive_bl: null
};

LabelTextCreate = "Create Activity";
LabelTextEdit = "Edit Activity";
LabelTextCopy = "Copy Activity";
UrlFillForEdit = "/PersonLogEntrySrv/GetByIds";
UrlEdit = "/PersonLogEntrySrv/Edit";
UrlDelete = "/PersonLogEntrySrv/Delete";

callBackAfterCreate = function () {
    $("#EditFormBtnOkFiles").removeClass("disabled");
    $("#LogEntryDateTime").val(moment().format("YYYY-MM-DD HH:mm"));
    MagicSuggests[0].setValue([UserId]);
    $("#ManHours").val(0);
    MagicSuggests[3].disable();
    MagicSuggests[4].disable();
    TableLogEntryAssysAdd.clear().search("").draw();
    TableLogEntryAssysRemove.clear().search("").draw();
    TableLogEntryPersonsRemove.clear().search("").draw();

    return modalWaitWrapper(function () {
        return refreshTableGeneric(TableLogEntryPersonsAdd, "/PersonSrv/Get", { getActive: true }, "GET");
    });
};

callBackAfterEdit = function (currRecords) {
    if (CurrIds.length > 1) { $("#EditFormBtnOkFiles").addClass("disabled"); }
    else { $("#EditFormBtnOkFiles").removeClass("disabled"); }

    return modalWaitWrapper(function () {
        return $.when(
            fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons",
                { logEntryId: CurrIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot",
                { logEntryId: CurrIds[0] },
                "GET", "/PersonSrv/Get",
                { getActive: true }),
            fillFormForRelatedGeneric(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                { logEntryId: CurrIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                "GET", "AssemblyDbSrv/LookupByLocDTables",
                { locId: MagicSuggests[3].getValue()[0], getActive: true })
            );
    });
};

callBackBeforeCopy = function () {
    return showModalConfirm("NOTE: Assemblies, People and Files are not copied!", "Confirm Copy");
};

callBackAfterCopy = function () {
    $("#EditFormBtnOkFiles").removeClass("disabled");
    TableLogEntryAssysAdd.clear().search("").draw();
    TableLogEntryAssysRemove.clear().search("").draw();
    TableLogEntryPersonsRemove.clear().search("").draw();
    MagicSuggests[5].clear();
    $("#QcdDateTime").val("");

    return modalWaitWrapper(function () {
        return $.when(
                refreshTableGeneric(TableLogEntryPersonsAdd,
                    "/PersonSrv/Get", { getActive: true }, "GET"),
                refreshTableGeneric(TableLogEntryAssysAdd,
                    "AssemblyDbSrv/LookupByLocDTables",
                    { getActive: true, locId: MagicSuggests[3].getValue()[0] }, "GET")
            );
    });
};

callBackBeforeSubmitEdit = function () {
    //confirmSubmitAddRemoveHelper
    var confirmSubmitAddRemoveHelper = function () {
        if (TableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).data().length +
            TableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).data().length +
            TableLogEntryPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            TableLogEntryPersonsRemove.rows(".ui-selected", { page: "current" }).data().length > 0) {
            return showModalConfirm("There are Assemblies and/or People selected in the Add/Remove tables." +
                    "Do you wish to add/remove selected?", "Confirm Add/Remove")
                .then(
                    function () { return $.Deferred().resolve(); },
                    function () {
                        TableLogEntryAssysAdd.rows().nodes().to$().removeClass("ui-selected");
                        TableLogEntryAssysRemove.rows().nodes().to$().removeClass("ui-selected");
                        TableLogEntryPersonsAdd.rows().nodes().to$().removeClass("ui-selected");
                        TableLogEntryPersonsRemove.rows().nodes().to$().removeClass("ui-selected");
                        return $.Deferred().resolve();
                    }
                );
        }
        return $.Deferred().resolve();
    },
    //confirmNoAssembliesHelper
    confirmNoAssembliesHelper = function () {
        if (TableLogEntryAssysRemove.rows().data().length +
            TableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).data().length -
            TableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).data().length <= 0) {
            return showModalConfirm("There are no Assemblies added to your entry(ies). " + 
                "Are you sure you want to proceed?", "Missing ASSEMBLIES !","no");
        }
        return $.Deferred().resolve();
    }
    //main
    return confirmSubmitAddRemoveHelper().then(confirmNoAssembliesHelper);
};


callBackAfterSubmitEdit = function () {
    return addRemoveAssembliesNow(true).then(function () { return addRemovePersonsNow(true); });
};


refreshMainView = function () {
    var deferred0 = $.Deferred();
    var endDate = moment($("#FilterDateEnd").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");

    TableMain.clear().search("").draw();

    if (($("#FilterDateStart").val() === "" || $("#FilterDateEnd").val() === "") ||
            (
                $("#FilterDateStart").val() === "" ||
                $("#FilterDateEnd").val() === "" &&
                MsFilterByType.getValue().length === 0 &&
                MsFilterByProject.getValue().length === 0 &&
                MsFilterByPerson.getValue().length === 0 &&
                MsFilterByAssy.getValue().length === 0
            )
        ) { return deferred0.resolve(); }

    refreshTblGenWrp(TableMain, "/PersonLogEntrySrv/GetByAltIds",
        {
            personIds: MsFilterByPerson.getValue(),
            typeIds: MsFilterByType.getValue(),
            projectIds: MsFilterByProject.getValue(),
            assyIds: MsFilterByAssy.getValue(),
            startDate: $("#FilterDateStart").val(),
            endDate: endDate,
            getActive: GetActive,
            filterForPLEView: true
        },
        "POST")
        .done(deferred0.resolve);

    return deferred0.promise();
};

//addRemoveAssembliesNow
var addRemoveAssembliesNow = function (doNotrefreshRelatedTable) {
    return modalWaitWrapper(function () {
        return submitEditsForRelatedGeneric(CurrIds,
                TableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                TableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonLogEntrySrv/EditPrsLogEntryAssys")
            .then(function () {
                if (doNotrefreshRelatedTable) { return $.Deferred().resolve(); }
                return fillFormForRelatedGeneric(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                        { logEntryId: CurrIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                        { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                        "GET", "AssemblyDbSrv/LookupByLocDTables",
                        { locId: MagicSuggests[3].getValue()[0], getActive: true } );
            });
    });
},
//addRemovePersonsNow
addRemovePersonsNow = function (doNotrefreshRelatedTable) {
    return modalWaitWrapper(function () {
        return submitEditsForRelatedGenericWrp(CurrIds,
                TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonLogEntrySrv/EditPrsLogEntryPersons")
            .then(function () {
                if (doNotrefreshRelatedTable) { return $.Deferred().resolve(); }
                return fillFormForRelatedGenericWrp(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                        "GET", "/PersonSrv/Get", { getActive: true });
            });
    });
},
//Show modal for chagning assembly(ies) status
showModalChngSts = function () {
    $("#ModalChngStsBody").text("Chagning status of " + ChngStsAssyIds.length + " assembly(ies).");
    ModalChngStsMs.clear(true);
    $("#ModalChngSts").modal("show");
},
//change status of selected assemblies
changeAssyStatus = function () {
    if (ModalChngStsMs.getValue().length == 1) {
        TableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).nodes().to$().removeClass("ui-selected");
        TableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).nodes().to$().removeClass("ui-selected");
        showModalWait();
        $.ajax({
            type: "POST", url: "/AssemblyDbSrv/EditStatus",
            timeout: 120000,
            data: {
                ids: ChngStsAssyIds,
                statusId: ModalChngStsMs.getValue()[0]
            },
            dataType: "json"
        })
        .always(hideModalWait)
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
},
//fillFiltersFromRequestParams
fillFiltersFromRequestParams = function () {
    var deferred0 = $.Deferred();

    $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#FilterDateEnd").val(moment().format("YYYY-MM-DD"));

    if (typeof PersonId !== "undefined" && PersonId !== "") {
        showModalWait();
        $.ajax({
            type: "POST",
            url: "/PersonSrv/GetAllByIds",
            timeout: 120000,
            data: { ids: [PersonId], getActive: true },
            dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                if (typeof data[0].Id !== undefined) {
                    msSetSelectionSilent(MsFilterByPerson, [{
                        id: data[0].Id,
                        name: data[0].FirstName + " " + data[0].LastName + " " + data[0].Initials
                    }]);
                }
                return deferred0.resolve();
            })
            .fail(function (xhr, status, error) {
                showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
    }
    else { return deferred0.resolve(); }

    return deferred0.promise();
},
//QC selected assemblies
qcSelected = function () {
    showModalWait();
    return $.ajax({
        type: "POST",
        url: "/PersonLogEntrySrv/QcLogEntries",
        timeout: 120000,
        data: { ids: CurrIds },
        dataType: "json"
    })
        .then(function () { return refreshMainView(); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); })
        .always(hideModalWait);

}

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        saveViewSettings(TableMain);
        showModalConfirm("Confirm that you want to QC the entry(ies).", "Confirm QC")
            .then(function () { return qcSelected(); })
            .done(function () { loadViewSettings(TableMain); });
    });

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/LookupFromProject",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByPerson
    $(MsFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/PersonActivityTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByType
    $(MsFilterByType).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByProject
    $(MsFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByAssy
    MsFilterByAssy = $("#MsFilterByAssy").magicSuggest({
        data: "/AssemblyDbSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: MsFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByAssy
    $(MsFilterByAssy).on("selectionchange", function (e, m) { refreshMainView(); });
        
    //---------------------------------------DataTables------------

    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
    ];

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            {
                data: "EnteredByPerson_",
                name: "EnteredByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            }, //2
            { data: "PrsLogEntryPersonsInitials", name: "PrsLogEntryPersonsInitials" },//3
            {
                data: "PersonActivityType_",
                name: "PersonActivityType_",
                render: function (data, type, full, meta) { return data.ActivityTypeName; }
            }, //4
            { data: "ManHours", name: "ManHours" },//5
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode; }
            }, //6
            {
                data: "AssignedToLocation_",
                name: "AssignedToLocation_",
                render: function (data, type, full, meta) { return data.LocName; }
            }, //7
            {
                data: "AssignedToProjectEvent_",
                name: "AssignedToProjectEvent_",
                render: function (data, type, full, meta) { return data.EventName; }
            }, //8
            { data: "PrsLogEntryFilesCount", name: "PrsLogEntryFilesCount" },//9
            { data: "PrsLogEntryAssysCount", name: "PrsLogEntryAssysCount" },//10
            {
                data: "QcdByPerson_",
                name: "QcdByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            }, //11
            { data: "Comments", name: "Comments" },//12
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//13
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//14
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//15
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//16
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" },//17
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" }//18
        ],
        columnDefs: [
            // - never show
            { targets: [0, 13, 14, 15, 16, 17, 18], visible: false },
            //"orderable": false, "visible": false
            { targets: [0, 1, 5, 9, 10, 13, 14, 15, 16, 17, 18], searchable: false },
             // - first set of columns
            { targets: [4, 5], className: "hidden-xs" },
            { targets: [3, 12], className: "hidden-xs hidden-sm" },
            { targets: [7, 8, 9, 10, 11], className: "hidden-xs hidden-sm hidden-md" }
        ],
        order: [[1, "asc"]],
        bAutoWidth: false,
        lengthMenu: [10, 25, 50, 75, 100, 500, 1000, 5000],
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        }
    });
    //showing the first Set of columns on startup;
    showColumnSet(TableMainColumnSets, 1);

    //---------------------------------------EditFormView----------------------------------------//

    //Wire Up EditFormBtnOkFiles
    $("#EditFormBtnOkFiles").click(function () {
        $("#LogEntryFilesViewPanel").text($("#LogEntryDateTime").val() +
            " " +MagicSuggests[0].getSelection()[0].name);
        submitEditForm()
            .then(function () { return fillLogEntryFilesForm(); })
            .done(function () { switchView(MainViewId, "LogEntryFilesView", "tdo-btngroup-logentryfiles"); });
    });

    //Initialize DateTimePicker
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    msAddToMsArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    msAddToMsArray(MagicSuggests, "QcdByPerson_Id", "/PersonSrv/Lookup", 1);

    //Initialize MagicSuggest Array Event - AssignedToProject_Id
    $(MagicSuggests[2]).on("selectionchange", function (e, m) {
        MagicSuggests[3].clear();
        MagicSuggests[4].clear();
        TableLogEntryAssysAdd.clear().search("").draw();
        if (this.getValue().length === 0) {
            MagicSuggests[3].disable();
            MagicSuggests[4].disable();
        }
        else {
            MagicSuggests[3].enable();
            MagicSuggests[4].enable();
        }
    });

    //Initialize MagicSuggest Array Event - AssignedToLocation_Id
    $(MagicSuggests[3]).on("selectionchange", function (e, m) {
        if (this.getValue().length === 0) {
            TableLogEntryAssysAdd.clear().search("").draw();
            return;
        }
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
    });

    //Wire Up EditFormBtnQcSelected
    $("#EditFormBtnQcSelected").click(function () {
        if (CurrIds.length === 0) {
            showModalFail("QC not possible!", "You cannot QC an entry while it is being created.");
            return;
        }
        showModalConfirm("Confirm that you want to QC the entry(ies).", "Confirm QC","no")
            .then(function () {
                return qcSelected();
            })
            .done(function () {
                fillFormForEditGenericWrp(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                GetActive, "EditForm", "Edit Person Activity", MagicSuggests);
            });
    });
        
    //Wire Up EditFormBtnAddRemoveAssys
    $("#EditFormBtnAddRemoveAssys").click(function () {
        if (TableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            TableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length == 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length === 0) {
            showModalConfirm("Adding/Removing Assemblies requires saving the Entry. Save Entry?",
                    "Confirm Saving Entry", "no")
                .done(function () { return submitEditForm(doNothingAndResolve, function () { addRemoveAssembliesNow(); }, true); });
            return;
        }
        addRemoveAssembliesNow();
    });

    //Wire Up EditFormBtnChngSts
    $("#EditFormBtnChngSts").click(function () {
        ChngStsAssyIds = $.merge(TableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            TableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray() );
        if (ChngStsAssyIds.length == 0) {
            showModalNothingSelected("Please select one or more assemblies.");
            return;
        }
        showModalChngSts();
    });

    //Wire Up EditFormBtnAddRemoveAssys
    $("#EditFormBtnAddRemovePersons").click(function () {
        if (TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length == 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length === 0) {
            showModalConfirm("Adding/Removing Persons requires saving the Entry. Save Entry?",
                    "Confirm Saving Entry", "no")
                .done(function () { return submitEditForm(doNothingAndResolve, function () { addRemovePersonsNow(); }, true); });
            return;
        }
        addRemovePersonsNow();
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
    
    //----------------------------------Modal Dialog - Change Assy Status -----------------------//

    //Wire Up ModalDeleteBtnCancel 
    $("#ModalChngStsBtnCancel").click(function () { $("#ModalChngSts").modal("hide"); });

    //Get focus on ModalChngStsMs
    $("#ModalChngSts").on("shown.bs.modal", function () { $("#ModalChngStsMs :input").focus(); });

    //Wire up MagicSuggest ModalChngStsMs
    ModalChngStsMs = $("#ModalChngStsMs").magicSuggest({
        data: "/AssemblyStatusSrv/Lookup",
        allowFreeEntries: false,
        maxSelection: 1,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });

    //Wire Up ModalChngStsBtnOk
    $("#ModalChngStsBtnOk").click(function () {
        $("#ModalChngSts").modal("hide");
        changeAssyStatus();
    });
    
    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);


    //--------------------------------End of execution at Start-----------
});



