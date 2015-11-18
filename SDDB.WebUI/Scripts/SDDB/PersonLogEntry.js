/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var MsFilterByPerson;
var MsFilterByType;
var MsFilterByProject;
var MsFilterByAssy;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Activity", "MainView");

        $("#LogEntryDateTime").val(moment().format("YYYY-MM-DD HH:mm"));
        MagicSuggests[0].setValue([UserId]);
        $("#ManHours").val(0);
        MagicSuggests[3].disable();
        MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw();
        TableLogEntryAssysRemove.clear().search("").draw();
        $("#EditFormBtnOkFiles").removeClass("disabled");

        refreshTblGenWrp(TableLogEntryPersonsAdd, "/PersonSrv/Get", { getActive: true }, "GET")
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "EditFormView", "tdo-btngroup-edit");
            });
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }

        if (CurrIds.length > 1) { $("#EditFormBtnOkFiles").addClass("disabled"); }
        else { $("#EditFormBtnOkFiles").removeClass("disabled"); }

        showModalWait();
        $.when(
            fillFormForEditGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                GetActive, "EditForm", "Edit Person Activity", MagicSuggests),

            fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                "GET", "/PersonSrv/Get", { getActive: true })
            )
            .then(function (currRecords) {
                CurrRecords = currRecords;
                return fillFormForRelatedGeneric(
                    TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                    { logEntryId: CurrIds[0] },
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                    { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                    "GET", "AssemblyDbSrv/LookupByLocDTables",
                    { locId: MagicSuggests[3].getValue()[0], getActive: true });
            })
            .always(hideModalWait)
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "EditFormView", "tdo-btngroup-edit");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnCopy
    $("#BtnCopy").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        $("#EditFormBtnOkFiles").removeClass("disabled");
        TableLogEntryAssysAdd.clear().search("").draw();
        TableLogEntryAssysRemove.clear().search("").draw();
        showModalWait();
        $.when(
            fillFormForCopyGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                GetActive, "EditForm", "Copy Person Activity", MagicSuggests),
            refreshTableGeneric(TableLogEntryPersonsAdd, "/PersonSrv/Get", { getActive: true }, "GET")
            )
            .then(function (currRecords) {
                CurrIds =[];
                CurrRecords =[];
                CurrRecords[0] = $.extend(true, {}, RecordTemplate);
                MagicSuggests[5].clear();
                $("#QcdDateTime").val("");
                return refreshTableGeneric(TableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
                { getActive: true, locId: MagicSuggests[3].getValue()[0] }, "GET");
            })
            .always(hideModalWait)
            .done(function () {
                saveViewSettings(TableMain);
                switchView("MainView", "EditFormView", "tdo-btngroup-edit");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

    });


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
    $(MsFilterByAssy).on('selectionchange', function (e, m) { refreshMainView(); });
          
        
    //---------------------------------------DataTables------------
    
    //wire up BtnTableMainExport
    $("#BtnTableMainExport").click(function (event) {
        exportTableToTxt(TableMain);
    });

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) {
            GetActive = true;
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        } else {
            GetActive = false;
            $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        }
        refreshMainView();
    });

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
            { data: "PersonLogEntryPersonsInitials", name: "PersonLogEntryPersonsInitials" },//3
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
            { data: "PersonLogEntryFilesCount", name: "PersonLogEntryFilesCount" },//9
            { data: "PersonLogEntryAssysCount", name: "PersonLogEntryAssysCount" },//10
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
            { targets: [0, 13, 14, 15, 16, 17, 18], visible: false }, // - never show
            { targets: [0, 1, 5, 9, 10, 13, 14, 15, 16, 17, 18], searchable: false },  //"orderable": false, "visible": false
            { targets: [4, 5], className: "hidden-xs" }, // - first set of columns
            { targets: [3, 12], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7, 8, 9, 10, 11], className: "hidden-xs hidden-sm hidden-md" } // - first set of columns
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

    //---------------------------------------EditFormView----------------------------------------//

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
            showModalFail("QC failed!", "You cannot QC an entry while it is being created.");
            return;
        }
        showModalConfirm("Confirm that you want to QC the entry(ies).", "Confirm QC")
            .then(function () {
                return qcSelected();
            })
            .done(function () {
                fillFormForEditGenericWrp(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                GetActive, "EditForm", "Edit Person Activity", MagicSuggests);
            });
    });

    //--------------------------------------View Initialization------------------------------------//

    fillFiltersFromRequestParams().done(refreshMainView);

    $("#InitialView").addClass("hidden");
    $("#MainView").removeClass("hidden");
   

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
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
}

//fillFiltersFromRequestParams
function fillFiltersFromRequestParams() {
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
}


//QC selected assemblies
function qcSelected() {
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

//---------------------------------------Helper Methods--------------------------------------//
