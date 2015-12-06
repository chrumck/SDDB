/// <reference path="../DataTables/jquery.dataTables.js" />
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
    ChngStsAssyIds = [],
    moveToDate,

    RecordTemplate = {
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

UrlFillForEdit = "/PersonLogEntrySrv/GetByIds";
UrlEdit = "/PersonLogEntrySrv/Edit";
UrlDelete = "/PersonLogEntrySrv/Delete";

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
                "Are you sure you want to proceed?", "Missing ASSEMBLIES !", "no");
        }
        return $.Deferred().resolve();
    }
    //main
    return confirmSubmitAddRemoveHelper().then(confirmNoAssembliesHelper);
};


callBackAfterSubmitEdit = function () {
    return addRemoveAssembliesNow(true).then(function () { return addRemovePersonsNow(true); });
};
//addRemoveAssembliesNow
addRemoveAssembliesNow = function (doNotrefreshRelatedTable) {
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
                        { locId: MagicSuggests[3].getValue()[0], getActive: true });
            });
    });
};
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
};
//showModalChngSts
showModalChngSts = function () {
    $("#ModalChngStsBody").text("Chagning status of " + ChngStsAssyIds.length + " assembly(ies).");
    ModalChngStsMs.clear(true);
    $("#ModalChngSts").modal("show");
};
//changeAssyStatus
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
};
//qcSelected
qcSelected = function () {
    return modalWaitWrapper(function () {
        return $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/QcLogEntries",
            timeout: 120000,
            data: { ids: CurrIds },
            dataType: "json"
        });
    })
        .then(refreshMainView)
};

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });


    //---------------------------------------EditFormView----------------------------------------//

    //Wire Up EditFormBtnAddRemoveAssys
    $("#EditFormBtnAddRemoveAssys").click(function () {
        if (TableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            TableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length == 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length === 0) {
            showModalConfirm("Adding/Removing Assemblies requires saving the Entry. Save Entry?",
                    "Confirm Saving Entry", "yes")
                .done(function () { return submitEditForm(doNothingAndResolve, function () { addRemoveAssembliesNow(); }, true); });
            return;
        }
        addRemoveAssembliesNow();
    });

    //Wire Up EditFormBtnChngSts
    $("#EditFormBtnChngSts").click(function () {
        ChngStsAssyIds = $.merge(TableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            TableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray());
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
                    "Confirm Saving Entry", "yes")
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

});