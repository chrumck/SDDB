/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />
/// <reference path="PersonLogEntryFiles.js" />

//--------------------------------------Global Properties------------------------------------//
var TableMain;
var TableLogEntryAssysAdd;
var TableLogEntryAssysRemove;
var TableLogEntryPersonsAdd;
var TableLogEntryPersonsRemove;
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    LogEntryDateTime: null,
    EnteredByPerson_Id: null,
    PersonActivityType_Id: null,
    ManHours: null,
    AssignedToProject_Id: null,
    AssignedToLocation_Id: null,
    AssignedToProjectEvent_Id: null,
    Comments: null,
    IsActive_bl: null
};
var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

var ModalChngStsMs;
var ChngStsAssyIds = [];

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else showModalDelete(CurrIds.length);
    });

    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //---------------------------------------EditFormView----------------------------------------//

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel").click(function () {
        switchView("EditFormView", "MainView", "tdo-btngroup-main", TableMain);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        checkAndSubmitEdits().done(function () {
            refreshMainView().done(function() {
                switchView("EditFormView", "MainView", "tdo-btngroup-main", TableMain);
            });
        });
    });

    //Wire Up EditFormBtnOkFiles
    $("#EditFormBtnOkFiles").click(function () {
        var entryFilesPanelText = $("#LogEntryDateTime").val() + " " + MagicSuggests[0].getSelection()[0].name;
        checkAndSubmitEdits().done(function () {
            setTimeout(function () {
                fillLogEntryFilesForm(entryFilesPanelText)
                    .done(function () { switchView("EditFormView", "LogEntryFilesView", "tdo-btngroup-logentryfiles"); });
            }, 200);
        });
    });

    //Wire Up EditFormBtnAddRemoveAssys
    $("#EditFormBtnAddRemoveAssys").click(function () {
        if (TableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            TableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length == 0) {
            showModalNothingSelected();
            return;
        }
        msValidate(MagicSuggests);
        if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
            showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct before adding/removing assemblies.");
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

    //Wire Up EditFormBtnAddRemovePersons
    $("#EditFormBtnAddRemovePersons").click(function () {
        if (TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length == 0) {
            showModalNothingSelected();
            return;
        }
        msValidate(MagicSuggests);
        if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
            showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct before adding/removing people.");
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
    


    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function deleteRecords() {
    var ids = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGenericWrp(CurrIds, "/PersonLogEntrySrv/Delete", refreshMainView);
}

//checkAndSubmitEdits
function checkAndSubmitEdits() {
    msValidate(MagicSuggests);
    if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
        showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
        return $.Deferred().reject();
    }
    return confirmSubmitAddRemoveHelper()
        .then(confirmNoAssembliesHelper)
        .then(submitEditsHelper);

    //helpers---------------------------------------

    //confirmSubmitAddRemoveHelper
    function confirmSubmitAddRemoveHelper() {
        var addRemoveCount = TableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).data().length +
                TableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).data().length +
                TableLogEntryPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
                TableLogEntryPersonsRemove.rows(".ui-selected", { page: "current" }).data().length;

        if (addRemoveCount == 0) { return $.Deferred().resolve(); }

        return showModalConfirm("There are Assemblies and/or People selected in the Add/Remove tables.\n" +
                "Do you wish to add/remove selected?","Confirm Add/Remove")
            .then(
                function () {
                    return addRemoveAssembliesNow().then(addRemovePersonsNow);
                },
                function () {
                    return $.Deferred().resolve();
                }
            );
    }

    //confirmNoAssembliesHelper
    function confirmNoAssembliesHelper() {
        if (TableLogEntryAssysRemove.rows().data().length == 0) {
            return showModalConfirm("There are no Assemblies added to your entry(ies).\nAre you sure you want to proceed?",
                "Missing ASSEMBLIES");
        }
        return $.Deferred().resolve();
    }

    //submitEditsHelper
    function submitEditsHelper() {
        return submitEditsGenericWrp("EditForm", MagicSuggests, CurrRecords, "POST", "/PersonLogEntrySrv/Edit")
            .then(function (data, currRecords) {
                CurrRecords = currRecords;
                if (CurrIds.length == 0) {
                    CurrIds = data.newEntryIds;
                    for (var i = 0; i < CurrIds.length; i++) {
                        CurrRecords[i].Id = CurrIds[i];
                    }
                }
                return $.Deferred().resolve();
            });
    }
}

//addRemoveAssembliesNow
function addRemoveAssembliesNow() {
    return updateCurrIdsHelper()
        .then(function () {
            var idsAssysAdd =
                TableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
            var idsAssysRemove =
                TableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
            return submitEditsForRelatedGenericWrp(CurrIds, idsAssysAdd, idsAssysRemove,
                "/PersonLogEntrySrv/EditPrsLogEntryAssys");
        })
        .then(function () {
            return fillFormForRelatedGenericWrp(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys", { logEntryId: CurrIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                "GET", "AssemblyDbSrv/LookupByLocDTables",
                { locId: MagicSuggests[3].getValue()[0], getActive: true });
        });
}

//addRemovePersonsNow
function addRemovePersonsNow() {
    return updateCurrIdsHelper()
        .then(function () {
            var idsPersonsAdd =
                TableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
            var idsPersonsRemove =
                TableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
            return submitEditsForRelatedGenericWrp(CurrIds, idsPersonsAdd, idsPersonsRemove,
                "/PersonLogEntrySrv/EditPrsLogEntryPersons");
        })
        .then(function () {
            return fillFormForRelatedGenericWrp(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                "GET", "/PersonSrv/Get", { getActive: true });
        });
}

//updateCurrIdsHelper
function updateCurrIdsHelper() {
    var deferred0 = $.Deferred();
    if (CurrIds.length != 0) {
        return deferred0.resolve();
    }
    submitEditsGenericWrp("EditForm", MagicSuggests, CurrRecords, "POST", "/PersonLogEntrySrv/Edit")
        .then(function (data, currRecords) {
            CurrRecords = currRecords;
            CurrIds = data.newEntryIds;
            for (var i = 0; i < CurrIds.length; i++) {
                CurrRecords[i].Id = CurrIds[i];
            }
            return refreshMainView();
        })
        .done(function () { deferred0.resolve(); });
    return deferred0.promise();
}

//Show modal for chagning assembly(ies) status
function showModalChngSts() {
    $("#ModalChngStsBody").text("Chagning status of " + ChngStsAssyIds.length + " assembly(ies).");
    ModalChngStsMs.clear(true);
    $("#ModalChngSts").modal("show");
}

//change status of selected assemblies
function changeAssyStatus() {
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
}

//---------------------------------------Helper Methods--------------------------------------//


