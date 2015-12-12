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

labelTextCreate = function () { return "New Activity for " + $("#filterDateStart").val(); };
labelTextEdit = function () { return "Edit Activity for " + $("#filterDateStart").val(); };
labelTextCopy = function () { return "Copy Activity to " + moveToDate.format("YYYY-MM-DD"); };

var panelTableMainClass = "panel-tdo-success";

callBackAfterCreate = function () {
    $("#editFormBtnOkFiles").prop("disabled", false);
    $("#logEntryPersonsView").addClass("hidden");
    var logEntryDT = moment($("#filterDateStart").val()).hour(moment().hour()).minute(0);
    $("#entryDTPicker").data("DateTimePicker").date(logEntryDT);
    $("#LogEntryDateTime").val(logEntryDT.format("YYYY-MM-DD HH:mm"));
    $("#hoursWorkedPicker").data("DateTimePicker").date("00:00");
    $("#ManHours").val(0);
    magicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);
    magicSuggests[3].disable();
    magicSuggests[4].disable();
    tableLogEntryAssysAdd.clear().search("").draw();
    tableLogEntryAssysRemove.clear().search("").draw();
    tableLogEntryPersonsAdd.clear().search("").draw();
    tableLogEntryPersonsRemove.clear().search("").draw();

    return $.Deferred().resolve();
};

callBackAfterEdit = function (currRecords) {
    if (currentIds.length > 1) { $("#editFormBtnOkFiles").prop("disabled", true); }
    else { $("#editFormBtnOkFiles").prop("disabled", false); }
    $("#logEntryPersonsView").addClass("hidden");
    tableLogEntryPersonsAdd.clear().search("").draw();
    tableLogEntryPersonsRemove.clear().search("").draw();
    $("#entryDTPicker").data("DateTimePicker").date(moment($("#LogEntryDateTime").val()));
    $("#LogEntryDateTime").data("ismodified", false);
    $("#hoursWorkedPicker").data("DateTimePicker").date(moment($("#ManHours").val(), "HH"));
    $("#ManHours").data("ismodified", false);

    return modalWaitWrapper(function () {
        return fillFormForRelatedGeneric(
                    tableLogEntryAssysAdd, tableLogEntryAssysRemove, currentIds,
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                    { logEntryId: currentIds[0] },
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                    { logEntryId: currentIds[0], locId: magicSuggests[3].getValue()[0] },
                    "GET", "AssemblyDbSrv/LookupByLocDTables",
                    { locId: magicSuggests[3].getValue()[0], getActive: true } );
    });
};

callBackBeforeCopy = function () {
    return showModalDatePrompt("NOTE: Assemblies, People and Files are not copied!",
                "Copy To Date:", $("#filterDateStart").val())
        .then(function (outputDate) { moveToDate = outputDate; });
};

callBackAfterCopy = function () {
    $("#editFormBtnOkFiles").prop("disabled", false);
    $("#logEntryPersonsView").addClass("hidden");
    tableLogEntryAssysAdd.clear().search("").draw();
    tableLogEntryAssysRemove.clear().search("").draw();
    tableLogEntryPersonsAdd.clear().search("").draw();
    tableLogEntryPersonsRemove.clear().search("").draw();

    var copyToDateTime = moment($("#LogEntryDateTime").val())
            .year(moveToDate.year()).dayOfYear(moveToDate.dayOfYear());
    $("#entryDTPicker").data("DateTimePicker").date(copyToDateTime);
    $("#LogEntryDateTime").val(copyToDateTime.format("YYYY-MM-DD HH:mm"));
    $("#hoursWorkedPicker").data("DateTimePicker").date(moment($("#ManHours").val(), "HH"));
    magicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);

    return modalWaitWrapper(function () {
        return refreshTableGeneric(tableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
            { getActive: true, locId: magicSuggests[3].getValue()[0] }, "GET");
    });
};

refreshMainView = function () {
    tableMain.clear().search("").draw();
    if ($("#filterDateStart").val() === "") { return $.Deferred().resolve(); }

    var endDate = moment($("#filterDateStart").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");
    return modalWaitWrapper(function () {
        return refreshTableGeneric(tableMain, "/PersonLogEntrySrv/GetByAltIds",
        {
            personIds: [UserId],
            startDate: $("#filterDateStart").val(),
            endDate: endDate,
            getActive: currentActive,
            filterForPLEView: false
        },
        "POST");
    });
}

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//
        
    //---------------------------------------DataTables------------
    
    //tableMainColumnSets
    tableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6, 7, 8, 9]
    ];

    //tableMain PersonLogEntrys
    tableMain = $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            {
                data: "LogEntryDateTime",
                name: "LogEntryDateTime",
                render: function (data, type, full, meta) { return moment(data).format("HH:mm"); }
            },//1
            //------------------------------------------------first set of columns
            {
                data: "EnteredByPerson_",
                name: "EnteredByPerson_",
                render: function (data, type, full, meta) { return data.Initials; }
            }, //2
            {
                data: "PersonActivityType_",
                name: "PersonActivityType_",
                render: function (data, type, full, meta) { return data.ActivityTypeName; }
            }, //3
            { data: "ManHours", name: "ManHours" },//4
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName; }
            }, //5
            {
                data: "AssignedToLocation_",
                name: "AssignedToLocation_",
                render: function (data, type, full, meta) { return data.LocName; }
            }, //6
            {
                data: "AssignedToProjectEvent_",
                name: "AssignedToProjectEvent_",
                render: function (data, type, full, meta) { return data.EventName; }
            }, //7
            { data: "Comments", name: "Comments" },//8
            { data: "PrsLogEntryFilesCount", name: "PrsLogEntryFilesCount" },//9
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//10
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//11
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//12
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//13
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" },//14
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" }//15
        ],
        columnDefs: [
            // - never show
            { targets: [0, 10, 11, 12, 13, 14, 15], visible: false },
            //"orderable": false, "visible": false
            { targets: [0, 1, 4, 9, 10, 11, 12, 13, 14, 15], searchable: false },  
            // - first set of columns
            { targets: [3, 6], className: "hidden-xs" },
            { targets: [7, 9], className: "hidden-xs hidden-sm" },
            { targets: [8], className: "hidden-xs hidden-sm hidden-md" }
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
    //showing the first Set of columns on startup;
    showColumnSet(1, tableMainColumnSets);

    //---------------------------------------editFormView----------------------------------------//

    //Initialize DateTimePicker - entryDTPicker
    $("#entryDTPicker").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            var newEntryDate = moment($("#LogEntryDateTime").val()).minute(0)
                .hour($("#entryDTPicker").data("DateTimePicker").date().hour());
            $("#LogEntryDateTime").val(newEntryDate.format("YYYY-MM-DD HH:mm"));
            $("#LogEntryDateTime").data("ismodified", true);
        });

    //Initialize DateTimePicker - hoursWorkedPicker
    $("#hoursWorkedPicker").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            $("#ManHours").val($("#hoursWorkedPicker").data("DateTimePicker").date().hour());
            $("#ManHours").data("ismodified", true);
        });

    //Initialize MagicSuggest Array
    msAddToMsArray(magicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    msAddToMsArray(magicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1, null, {}, false, false);
    msAddToMsArray(magicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1, null, {}, false, true);
    msAddToMsArray(magicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: magicSuggests[2].getValue });
    msAddToMsArray(magicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: magicSuggests[2].getValue }, false, false);
    
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
        if (currentIds.length == 1) {
            refreshTblGenWrp(tableLogEntryAssysAdd, "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                { logEntryId: currentIds[0], locId: magicSuggests[3].getValue()[0] }, "GET")
                .done(function () { $("#AssignedToLocation_Id input").focus(); });
        }
        else {
            refreshTblGenWrp(tableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
            { getActive: true, locId: magicSuggests[3].getValue()[0] }, "GET")
            .done(function () { $("#AssignedToLocation_Id input").focus(); });
        }
    });

    //Wire Up editFormBtnPrsAddRemove
    $("#editFormBtnPrsAddRemove").click(function () {
        tableLogEntryPersonsAdd.clear().search("").draw();
        tableLogEntryPersonsRemove.clear().search("").draw();

        if ($("#logEntryPersonsView").hasClass("hidden")) {
            showModalWait();
            fillFormForRelatedGeneric(tableLogEntryPersonsAdd, tableLogEntryPersonsRemove, currentIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: currentIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: currentIds[0] },
                "GET", "/PersonSrv/Get", { getActive: true })
                .always(hideModalWait)
                .done(function () {
                    $("#logEntryPersonsView").removeClass("hidden");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
        else {
            $("#logEntryPersonsView").addClass("hidden");
        }
    });
           
    //--------------------------------------View Initialization------------------------------------//

    $("#filterDateStart").val(moment().format("YYYY-MM-DD"));
    refreshMainView();
    switchView(initialViewId, mainViewId, mainViewBtnGroupClass);
  

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//



//---------------------------------------Helper Methods--------------------------------------//
