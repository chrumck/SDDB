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

var msFilterByPerson,
    msFilterByType,
    msFilterByProject,
    msFilterByAssy,

    tableLogEntryAssysAdd,
    tableLogEntryAssysRemove,
    tableLogEntryPersonsAdd,
    tableLogEntryPersonsRemove,

    modalChngStsMs,
    ChngStsAssyIds = [],
    moveToDate,

    recordTemplate = {
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

urlFillForEdit = "/PersonLogEntrySrv/GetByIds";
urlEdit = "/PersonLogEntrySrv/Edit";
urlDelete = "/PersonLogEntrySrv/Delete";

callBackBeforeSubmitEdit = function () {
    //confirmSubmitAddRemoveHelper
    var confirmSubmitAddRemoveHelper = function () {
        if (tableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).data().length +
            tableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).data().length +
            tableLogEntryPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            tableLogEntryPersonsRemove.rows(".ui-selected", { page: "current" }).data().length > 0) {
            return showModalConfirm("There are Assemblies and/or People selected in the Add/Remove tables." +
                    "Do you wish to add/remove selected?", "Confirm Add/Remove")
                .then(
                    function () { return $.Deferred().resolve(); },
                    function () {
                        tableLogEntryAssysAdd.rows().nodes().to$().removeClass("ui-selected");
                        tableLogEntryAssysRemove.rows().nodes().to$().removeClass("ui-selected");
                        tableLogEntryPersonsAdd.rows().nodes().to$().removeClass("ui-selected");
                        tableLogEntryPersonsRemove.rows().nodes().to$().removeClass("ui-selected");
                        return $.Deferred().resolve();
                    }
                );
        }
        return $.Deferred().resolve();
    },
    //confirmNoAssembliesHelper
    confirmNoAssembliesHelper = function () {
        if (tableLogEntryAssysRemove.rows().data().length +
            tableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).data().length -
            tableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).data().length <= 0) {
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
        return submitEditsForRelatedGeneric(currentIds,
                tableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                tableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonLogEntrySrv/EditPrsLogEntryAssys")
            .then(function () {
                if (doNotrefreshRelatedTable) { return $.Deferred().resolve(); }
                return fillFormForRelatedGeneric(tableLogEntryAssysAdd, tableLogEntryAssysRemove, currentIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                        { logEntryId: currentIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                        { logEntryId: currentIds[0], locId: magicSuggests[3].getValue()[0] },
                        "GET", "AssemblyDbSrv/LookupByLocDTables",
                        { locId: magicSuggests[3].getValue()[0], getActive: true });
            });
    });
};
//addRemovePersonsNow
addRemovePersonsNow = function (doNotrefreshRelatedTable) {
    return modalWaitWrapper(function () {
        return submitEditsForRelatedGenericWrp(currentIds,
                tableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                tableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonLogEntrySrv/EditPrsLogEntryPersons")
            .then(function () {
                if (doNotrefreshRelatedTable) { return $.Deferred().resolve(); }
                return fillFormForRelatedGenericWrp(tableLogEntryPersonsAdd, tableLogEntryPersonsRemove, currentIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: currentIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: currentIds[0] },
                        "GET", "/PersonSrv/Get", { getActive: true });
            });
    });
};
//showModalChngSts
showModalChngSts = function () {
    $("#modalChngStsBody").text("Chagning status of " + ChngStsAssyIds.length + " assembly(ies).");
    modalChngStsMs.clear(true);
    $("#modalChngSts").modal("show");
};
//changeAssyStatus
changeAssyStatus = function () {
    if (modalChngStsMs.getValue().length == 1) {
        tableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).nodes().to$().removeClass("ui-selected");
        tableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).nodes().to$().removeClass("ui-selected");
        showModalWait();
        $.ajax({
            type: "POST", url: "/AssemblyDbSrv/EditStatus",
            timeout: 120000,
            data: {
                ids: ChngStsAssyIds,
                statusId: modalChngStsMs.getValue()[0]
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
            data: { ids: currentIds },
            dataType: "json"
        });
    })
        .then(refreshMainView)
};

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//

    //Initialize DateTimePicker filterDateStart
    $("#filterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize DateTimePicker filterDateEnd
    $("#filterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });


    //---------------------------------------editFormView----------------------------------------//

    //Wire Up editFormBtnAddRemoveAssys
    $("#editFormBtnAddRemoveAssys").click(function () {
        if (tableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            tableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length == 0) {
            showModalNothingSelected();
            return;
        }
        if (currentIds.length === 0) {
            showModalConfirm("Adding/Removing Assemblies requires saving the Entry. Save Entry?",
                    "Confirm Saving Entry", "yes")
                .done(function () { return submitEditForm(doNothingAndResolve, function () { addRemoveAssembliesNow(); }, true); });
            return;
        }
        addRemoveAssembliesNow();
    });

    //Wire Up editFormBtnChngSts
    $("#editFormBtnChngSts").click(function () {
        ChngStsAssyIds = $.merge(tableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            tableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray());
        if (ChngStsAssyIds.length == 0) {
            showModalNothingSelected("Please select one or more assemblies.");
            return;
        }
        showModalChngSts();
    });

    //Wire Up editFormBtnAddRemoveAssys
    $("#editFormBtnAddRemovePersons").click(function () {
        if (tableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            tableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length == 0) {
            showModalNothingSelected();
            return;
        }
        if (currentIds.length === 0) {
            showModalConfirm("Adding/Removing Persons requires saving the Entry. Save Entry?",
                    "Confirm Saving Entry", "yes")
                .done(function () { return submitEditForm(doNothingAndResolve, function () { addRemovePersonsNow(); }, true); });
            return;
        }
        addRemovePersonsNow();
    });

    //------------------------------------DataTables - Log Entry Assemblies ---

    //tableLogEntryAssysAdd
    tableLogEntryAssysAdd = $("#tableLogEntryAssysAdd").DataTable({
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

    //tableLogEntryAssysRemove
    tableLogEntryAssysRemove = $("#tableLogEntryAssysRemove").DataTable({
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

    //tableLogEntryPersonsAdd
    tableLogEntryPersonsAdd = $("#tableLogEntryPersonsAdd").DataTable({
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

    //tableLogEntryPersonsRemove
    tableLogEntryPersonsRemove = $("#tableLogEntryPersonsRemove").DataTable({
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
    $("#modalChngStsBtnCancel").click(function () { $("#modalChngSts").modal("hide"); });

    //Get focus on modalChngStsMs
    $("#modalChngSts").on("shown.bs.modal", function () { $("#modalChngStsMs :input").focus(); });

    //Wire up MagicSuggest modalChngStsMs
    modalChngStsMs = $("#modalChngStsMs").magicSuggest({
        data: "/AssemblyStatusSrv/Lookup",
        allowFreeEntries: false,
        maxSelection: 1,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });

    //Wire Up modalChngStsBtnOk
    $("#modalChngStsBtnOk").click(function () {
        $("#modalChngSts").modal("hide");
        changeAssyStatus();
    });

});