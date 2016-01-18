/*global sddb */
/// <reference path="Shared_Views.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//

//setting up sddb
sddb.setConfig({
    recordTemplate: {
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
    },

    urlEdit : "/PersonLogEntrySrv/Edit",
    urlDelete : "/PersonLogEntrySrv/Delete"

});

//callBackBeforeSubmitEdit
sddb.callBackBeforeSubmitEdit = function () {
    "use strict";
    //confirmSubmitAddRemoveHelper
    var confirmSubmitAddRemoveHelper = function () {
        if (sddb.tableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).data().length +
            sddb.tableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).data().length +
            sddb.tableLogEntryPersonsAdd.rows(".ui-selected", { page: "current" }).data().length +
            sddb.tableLogEntryPersonsRemove.rows(".ui-selected", { page: "current" }).data().length > 0) {
            return sddb.showModalConfirm("There are Assemblies and/or People selected in the Add/Remove tables." +
                    "Do you wish to add/remove selected?", "Confirm Add/Remove")
                .then(
                    function () { return $.Deferred().resolve(); },
                    function () {
                        sddb.tableLogEntryAssysAdd.rows().nodes().to$().removeClass("ui-selected");
                        sddb.tableLogEntryAssysRemove.rows().nodes().to$().removeClass("ui-selected");
                        sddb.tableLogEntryPersonsAdd.rows().nodes().to$().removeClass("ui-selected");
                        sddb.tableLogEntryPersonsRemove.rows().nodes().to$().removeClass("ui-selected");
                        return $.Deferred().resolve();
                    }
                );
        }
        return $.Deferred().resolve();
    },
    //confirmNoAssembliesHelper
    confirmNoAssembliesHelper = function () {
        if (sddb.tableLogEntryAssysRemove.rows().data().length +
            sddb.tableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).data().length -
            sddb.tableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).data().length <= 0) {
            return sddb.showModalConfirm("There are no Assemblies added to your entry(ies). " +
                "Are you sure you want to proceed?", "Missing ASSEMBLIES !", "no", "btn btn-danger");
        }
        return $.Deferred().resolve();
    };
    //main
    return confirmSubmitAddRemoveHelper().then(confirmNoAssembliesHelper);
};
//callBackAfterSubmitEdit        
sddb.callBackAfterSubmitEdit = function () {
    "use strict";
    return sddb.addRemoveAssembliesNow(true).then(function () { return sddb.addRemovePersonsNow(true); });
};
//addRemoveAssembliesNow
sddb.addRemoveAssembliesNow = function (doNotrefreshRelatedTable) {
    "use strict";
    return sddb.modalWaitWrapper(function () {
        return sddb.submitEditsForRelatedGeneric(sddb.cfg.currentIds,
                sddb.tableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                sddb.tableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonLogEntrySrv/EditPrsLogEntryAssys")
            .then(function () {
                if (doNotrefreshRelatedTable) { return $.Deferred().resolve(); }
                return sddb.fillFormForRelatedGeneric(
                        sddb.tableLogEntryAssysAdd, sddb.tableLogEntryAssysRemove, sddb.cfg.currentIds,
                        "POST", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                        { ids: sddb.cfg.currentIds },
                        "POST", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                        { ids: sddb.cfg.currentIds, locId: sddb.cfg.magicSuggests[3].getValue()[0] });
            });
    });
};
//addRemovePersonsNow
sddb.addRemovePersonsNow = function (doNotrefreshRelatedTable) {
    "use strict";
    return sddb.modalWaitWrapper(function () {
        return sddb.submitEditsForRelatedGeneric(sddb.cfg.currentIds,
                sddb.tableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                sddb.tableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
                "/PersonLogEntrySrv/EditPrsLogEntryPersons")
            .then(function () {
                if (doNotrefreshRelatedTable) { return $.Deferred().resolve(); }
                return sddb.fillFormForRelatedGeneric(
                    sddb.tableLogEntryPersonsAdd, sddb.tableLogEntryPersonsRemove, sddb.cfg.currentIds,
                    "POST", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { ids: sddb.cfg.currentIds },
                    "POST", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { ids: sddb.cfg.currentIds });
            });
    });
};
//showModalChngSts
sddb.showModalChngSts = function () {
    "use strict";
    $("#modalChngStsBody").text("Chagning status of " + sddb.chngStsAssyIds.length + " assembly(ies).");
    sddb.modalChngStsMs.clear(true);
    $("#modalChngSts").modal("show");
};
//changeAssyStatus
sddb.changeAssyStatus = function () {
    "use strict";
    if (sddb.modalChngStsMs.getValue().length == 1) {
        sddb.tableLogEntryAssysAdd.rows(".ui-selected", { page: "current" }).nodes().to$()
            .removeClass("ui-selected");
        sddb.tableLogEntryAssysRemove.rows(".ui-selected", { page: "current" }).nodes().to$()
            .removeClass("ui-selected");
        sddb.showModalWait();
        $.ajax({
            type: "POST",
            url: "/AssemblyDbSrv/EditStatus",
            timeout: 120000,
            data: {
                ids: sddb.chngStsAssyIds,
                statusId: sddb.modalChngStsMs.getValue()[0]
            },
            dataType: "json"
        })
        .always(sddb.hideModalWait)
        .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });
    }
};
//qcSelected
sddb.qcSelected = function () {
    "use strict";
    return sddb.modalWaitWrapper(function () {
        return $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/QcLogEntries",
            timeout: 120000,
            data: { ids: sddb.cfg.currentIds },
            dataType: "json"
        });
    })
        .then(sddb.refreshMainView);
};

//----------------------------------------------setup after page load------------------------------------------------//

$(document).ready(function () {
    "use strict";

    //-----------------------------------------mainView------------------------------------------//

    //filterDateStart event dp.hide
    $("#filterDateStart").on("dp.hide", function (e) { sddb.refreshMainView(); });

    //filterDateEnd event dp.hide
    $("#filterDateEnd").on("dp.hide", function (e) { sddb.refreshMainView(); });
       
    //---------------------------------------editFormView----------------------------------------//

    //Wire Up editFormBtnAddRemoveAssys
    $("#editFormBtnAddRemoveAssys").click(function () {
        if (sddb.tableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            sddb.tableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        if (sddb.cfg.currentIds.length === 0) {
            sddb.showModalConfirm("Adding/Removing Assemblies requires saving the Entry. Save Entry?",
                    "Confirm Saving Entry", "yes")
                .done(function () {
                    sddb.submitEditForm(sddb.doNothingAndResolve,
                        function () { sddb.addRemoveAssembliesNow(); }, true);
                });
            return;
        }
        sddb.addRemoveAssembliesNow();
    });

    //Wire Up editFormBtnChngSts
    $("#editFormBtnChngSts").click(function () {
        sddb.chngStsAssyIds = $.merge(
            sddb.tableLogEntryAssysAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray(),
            sddb.tableLogEntryAssysRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray()
        );
        if (sddb.chngStsAssyIds.length === 0) {
            sddb.showModalNothingSelected("Please select one or more assemblies.");
            return;
        }
        sddb.showModalChngSts();
    });

    //Wire Up editFormBtnAddRemovePersons
    $("#editFormBtnAddRemovePersons").click(function () {
        if (sddb.tableLogEntryPersonsAdd.cells(".ui-selected", "Id:name", { page: "current" }).data().length +
            sddb.tableLogEntryPersonsRemove.cells(".ui-selected", "Id:name", { page: "current" }).data().length === 0) {
            sddb.showModalNothingSelected();
            return;
        }
        if (sddb.cfg.currentIds.length === 0) {
            sddb.showModalConfirm("Adding/Removing Persons requires saving the Entry. Save Entry?",
                    "Confirm Saving Entry", "yes")
                .done(function () {
                    return sddb.submitEditForm(sddb.doNothingAndResolve,
                        function () { sddb.addRemovePersonsNow(); }, true);
                });
            return;
        }
        sddb.addRemovePersonsNow();
    });

    //------------------------------------DataTables - Log Entry Assemblies ---

    //tableLogEntryAssysAdd
    sddb.tableLogEntryAssysAdd = $("#tableLogEntryAssysAdd").DataTable({
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
    sddb.tableLogEntryAssysRemove = $("#tableLogEntryAssysRemove").DataTable({
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
    sddb.tableLogEntryPersonsAdd = $("#tableLogEntryPersonsAdd").DataTable({
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
    sddb.tableLogEntryPersonsRemove = $("#tableLogEntryPersonsRemove").DataTable({
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

    sddb.chngStsAssyIds = [];

    //Wire Up ModalDeleteBtnCancel 
    $("#modalChngStsBtnCancel").click(function () { $("#modalChngSts").modal("hide"); });

    //Get focus on modalChngStsMs
    $("#modalChngSts").on("shown.bs.modal", function () { $("#modalChngStsMs :input").focus(); });

    //Wire up MagicSuggest modalChngStsMs
    sddb.modalChngStsMs = $("#modalChngStsMs").magicSuggest({
        data: "/AssemblyStatusSrv/Lookup",
        allowFreeEntries: false,
        maxSelection: 1,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });

    //Wire Up modalChngStsBtnOk
    $("#modalChngStsBtnOk").click(function () {
        $("#modalChngSts").modal("hide");
        sddb.changeAssyStatus();
    });

    //-----------------------------------------filesView-----------------------------------------//

    //filesTable
    sddb.filesTable = $("#filesTable").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "FileName", name: "FileName" },//1
            { data: "FileType", name: "FileType" },//2
            { data: "FileSize", name: "FileSize" },//3
            { data: "FileDateTime", name: "FileDateTime" },//4
            {
                data: "LastSavedByPerson_",
                name: "LastSavedByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            } //5
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 5], className: "hidden-xs" }, // - first set of columns
            { targets: [4], className: "hidden-xs hidden-sm" } // - first set of columns
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

    //personLogEntryFilesCfg
    sddb.personLogEntryFilesCfg = {
        filesTable: sddb.filesTable,
        listFilesUrl: "/PersonLogEntrySrv/ListFiles",
        dlFilesUrl: "/PersonLogEntrySrv/DownloadFiles",
        ulFilesUrl: "/PersonLogEntrySrv/UploadFiles",
        deleteFilesUrl: "/PersonLogEntrySrv/DeleteFiles",
        filesViewLabelText: "Activity Files",
        filesViewPanelText: function (selectedRecord) {
            return selectedRecord.EnteredByPerson_.FirstName + " " + selectedRecord.EnteredByPerson_.LastName +
                " - " + selectedRecord.LogEntryDateTime;
        }
    };

    sddb.wireButtonsForFiles(sddb.personLogEntryFilesCfg);

    //--------------------------------------View Initialization------------------------------------//

    

    //--------------------------------End of setup after page load---------------------------------//   
});
    