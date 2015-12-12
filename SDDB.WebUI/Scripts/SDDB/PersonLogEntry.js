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
/// <reference path="PersonLogEntryShared.js" />

//--------------------------------------Global Properties------------------------------------//

labelTextCreate = "Create Activity";
labelTextEdit = "Edit Activity";
labelTextCopy = "Copy Activity";

callBackAfterCreate = function () {
    $("#editFormBtnOkFiles").prop("disabled", false);
    $("#LogEntryDateTime").val(moment().format("YYYY-MM-DD HH:mm"));
    $("#ManHours").val(0);
    magicSuggests[0].setValue([UserId]);
    magicSuggests[3].disable();
    magicSuggests[4].disable();
    tableLogEntryAssysAdd.clear().search("").draw();
    tableLogEntryAssysRemove.clear().search("").draw();
    tableLogEntryPersonsAdd.clear().search("").draw();
    tableLogEntryPersonsRemove.clear().search("").draw();

    return modalWaitWrapper(function () {
        return refreshTableGeneric(tableLogEntryPersonsAdd, "/PersonSrv/Get", { getActive: true }, "GET");
    });
};

callBackAfterEdit = function (currRecords) {
    if (currentIds.length > 1) { $("#editFormBtnOkFiles").prop("disabled", true); }
    else { $("#editFormBtnOkFiles").prop("disabled", false); }

    return modalWaitWrapper(function () {
        return $.when(
            fillFormForRelatedGeneric(tableLogEntryPersonsAdd, tableLogEntryPersonsRemove, currentIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons",
                { logEntryId: currentIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot",
                { logEntryId: currentIds[0] },
                "GET", "/PersonSrv/Get",
                { getActive: true }),
            fillFormForRelatedGeneric(tableLogEntryAssysAdd, tableLogEntryAssysRemove, currentIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                { logEntryId: currentIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                { logEntryId: currentIds[0], locId: magicSuggests[3].getValue()[0] },
                "GET", "AssemblyDbSrv/LookupByLocDTables",
                { locId: magicSuggests[3].getValue()[0], getActive: true })
        );
    });
};

callBackBeforeCopy = function () {
    return showModalConfirm("NOTE: Assemblies, People and Files are not copied! Continue?", "Confirm Copy");
};

callBackAfterCopy = function () {
    $("#editFormBtnOkFiles").prop("disabled", false);
    tableLogEntryAssysAdd.clear().search("").draw();
    tableLogEntryAssysRemove.clear().search("").draw();
    tableLogEntryPersonsAdd.clear().search("").draw();
    tableLogEntryPersonsRemove.clear().search("").draw();

    magicSuggests[5].clear();
    $("#QcdDateTime").val("");

    return modalWaitWrapper(function () {
        return $.when(
                refreshTableGeneric(tableLogEntryPersonsAdd,
                    "/PersonSrv/Get", { getActive: true }, "GET"),
                refreshTableGeneric(tableLogEntryAssysAdd,
                    "AssemblyDbSrv/LookupByLocDTables",
                    { getActive: true, locId: magicSuggests[3].getValue()[0] }, "GET")
            );
    });
};

refreshMainView = function () {
    tableMain.clear().search("").draw();
    if ($("#filterDateStart").val() === "" || $("#filterDateEnd").val() === "") { return $.Deferred().resolve(); }

    var endDate = moment($("#filterDateEnd").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");
    return modalWaitWrapper(function () {
        return refreshTableGeneric(tableMain, "/PersonLogEntrySrv/GetByAltIds",
        {
            personIds: msFilterByPerson.getValue(),
            typeIds: msFilterByType.getValue(),
            projectIds: msFilterByProject.getValue(),
            assyIds: msFilterByAssy.getValue(),
            startDate: $("#filterDateStart").val(),
            endDate: endDate,
            getActive: currentActive,
            filterForPLEView: true
        },
        "POST");
    });
};

//fillFiltersFromRequestParams
fillFiltersFromRequestParams = function () {
    var deferred0 = $.Deferred();

    $("#filterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#filterDateEnd").val(moment().format("YYYY-MM-DD"));

    if (PersonId) {
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
                    msSetSelectionSilent(msFilterByPerson, [{
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
};

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        currentIds = tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (currentIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        saveViewSettings(tableMain);
        showModalConfirm("Confirm that you want to QC the entry(ies).", "Confirm QC")
            .then(function () { return qcSelected(); })
            .done(function () { loadViewSettings(tableMain); });
    });

    //Initialize MagicSuggest msFilterByPerson
    msFilterByPerson = $("#msFilterByPerson").magicSuggest({
        data: "/PersonSrv/LookupFromProject",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByPerson
    $(msFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest msFilterByType
    msFilterByType = $("#msFilterByType").magicSuggest({
        data: "/PersonActivityTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByType
    $(msFilterByType).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest msFilterByProject
    msFilterByProject = $("#msFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByProject
    $(msFilterByProject).on("selectionchange", function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest msFilterByAssy
    msFilterByAssy = $("#msFilterByAssy").magicSuggest({
        data: "/AssemblyDbSrv/LookupByProj",
        allowFreeEntries: false,
        dataUrlParams: { projectIds: msFilterByProject.getValue },
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByAssy
    $(msFilterByAssy).on("selectionchange", function (e, m) { refreshMainView(); });
        
    //---------------------------------------DataTables------------

    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
    ];

    //tableMain PersonLogEntrys
    tableMain = $("#tableMain").DataTable({
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
    showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //Initialize DateTimePicker
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    msAddToMsArray(magicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    msAddToMsArray(magicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1);
    msAddToMsArray(magicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(magicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: magicSuggests[2].getValue });
    msAddToMsArray(magicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: magicSuggests[2].getValue });
    msAddToMsArray(magicSuggests, "QcdByPerson_Id", "/PersonSrv/Lookup", 1);

    //Initialize MagicSuggest Array Event - AssignedToProject_Id
    $(magicSuggests[2]).on("selectionchange", function (e, m) {
        magicSuggests[3].clear();
        magicSuggests[4].clear();
        tableLogEntryAssysAdd.clear().search("").draw();
        if (this.getValue().length === 0) {
            magicSuggests[3].disable();
            magicSuggests[4].disable();
        }
        else {
            magicSuggests[3].enable();
            magicSuggests[4].enable();
        }
    });

    //Initialize MagicSuggest Array Event - AssignedToLocation_Id
    $(magicSuggests[3]).on("selectionchange", function (e, m) {
        if (this.getValue().length === 0) {
            tableLogEntryAssysAdd.clear().search("").draw();
            return;
        }
        modalWaitWrapper(function () {
            if (currentIds.length == 1) {
                return refreshTableGeneric(tableLogEntryAssysAdd, "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                    { logEntryId: currentIds[0], locId: magicSuggests[3].getValue()[0] }, "GET");
            }
            return refreshTableGeneric(tableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
            { getActive: true, locId: magicSuggests[3].getValue()[0] }, "GET");

        })
            .done(function () { $("#AssignedToLocation_Id input").focus(); });
    });

    //Wire Up editFormBtnQcSelected
    $("#editFormBtnQcSelected").click(function () {
        if (currentIds.length === 0) {
            showModalFail("QC not possible!", "You cannot QC an entry while it is being created.");
            return;
        }
        showModalConfirm("Confirm that you want to QC the entry(ies).", "Confirm QC","no")
            .then(qcSelected)
            .done(function () {
                modalWaitWrapper(function () {
                    return fillFormForEditGeneric(currentIds, "POST", "/PersonLogEntrySrv/GetByIds",
                            currentActive, "editForm", "Edit Person Activity", magicSuggests);
                });
            });
    });
        
    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);
    switchView(initialViewId, mainViewId, mainViewBtnGroupClass);


    //--------------------------------End of execution at Start-----------
});



