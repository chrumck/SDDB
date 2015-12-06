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

LabelTextCreate = function () { return "New Activity for " + $("#FilterDateStart").val(); };
LabelTextEdit = function () { return "Edit Activity for " + $("#FilterDateStart").val(); };
LabelTextCopy = function () { return "Copy Activity to " + moveToDate.format("YYYY-MM-DD"); };

var panelTableMainClass = "panel-tdo-success";

callBackAfterCreate = function () {
    $("#EditFormBtnOkFiles").prop("disabled", false);
    $("#LogEntryPersonsView").addClass("hidden");
    var logEntryDT = moment($("#FilterDateStart").val()).hour(moment().hour()).minute(0);
    $("#EntryDTPicker").data("DateTimePicker").date(logEntryDT);
    $("#LogEntryDateTime").val(logEntryDT.format("YYYY-MM-DD HH:mm"));
    $("#HoursWorkedPicker").data("DateTimePicker").date("00:00");
    $("#ManHours").val(0);
    MagicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);
    MagicSuggests[3].disable();
    MagicSuggests[4].disable();
    TableLogEntryAssysAdd.clear().search("").draw();
    TableLogEntryAssysRemove.clear().search("").draw();
    TableLogEntryPersonsAdd.clear().search("").draw();
    TableLogEntryPersonsRemove.clear().search("").draw();

    return $.Deferred().resolve();
};

callBackAfterEdit = function (currRecords) {
    if (CurrIds.length > 1) { $("#EditFormBtnOkFiles").prop("disabled", true); }
    else { $("#EditFormBtnOkFiles").prop("disabled", false); }
    $("#LogEntryPersonsView").addClass("hidden");
    TableLogEntryPersonsAdd.clear().search("").draw();
    TableLogEntryPersonsRemove.clear().search("").draw();
    $("#EntryDTPicker").data("DateTimePicker").date(moment($("#LogEntryDateTime").val()));
    $("#LogEntryDateTime").data("ismodified", false);
    $("#HoursWorkedPicker").data("DateTimePicker").date(moment($("#ManHours").val(), "HH"));
    $("#ManHours").data("ismodified", false);

    return modalWaitWrapper(function () {
        return fillFormForRelatedGeneric(
                    TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                    { logEntryId: CurrIds[0] },
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                    { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                    "GET", "AssemblyDbSrv/LookupByLocDTables",
                    { locId: MagicSuggests[3].getValue()[0], getActive: true } );
    });
};

callBackBeforeCopy = function () {
    return showModalDatePrompt("NOTE: Assemblies, People and Files are not copied!",
                "Copy To Date:", $("#FilterDateStart").val())
        .then(function (outputDate) { moveToDate = outputDate; });
};

callBackAfterCopy = function () {
    $("#EditFormBtnOkFiles").prop("disabled", false);
    $("#LogEntryPersonsView").addClass("hidden");
    TableLogEntryAssysAdd.clear().search("").draw();
    TableLogEntryAssysRemove.clear().search("").draw();
    TableLogEntryPersonsAdd.clear().search("").draw();
    TableLogEntryPersonsRemove.clear().search("").draw();

    var copyToDateTime = moment($("#LogEntryDateTime").val())
            .year(moveToDate.year()).dayOfYear(moveToDate.dayOfYear());
    $("#EntryDTPicker").data("DateTimePicker").date(copyToDateTime);
    $("#LogEntryDateTime").val(copyToDateTime.format("YYYY-MM-DD HH:mm"));
    $("#HoursWorkedPicker").data("DateTimePicker").date(moment($("#ManHours").val(), "HH"));
    MagicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);

    return modalWaitWrapper(function () {
        return refreshTableGeneric(TableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
            { getActive: true, locId: MagicSuggests[3].getValue()[0] }, "GET");
    });
};

refreshMainView = function () {
    TableMain.clear().search("").draw();
    if ($("#FilterDateStart").val() === "") { return $.Deferred().resolve(); }

    var endDate = moment($("#FilterDateStart").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");
    return modalWaitWrapper(function () {
        return refreshTableGeneric(TableMain, "/PersonLogEntrySrv/GetByAltIds",
        {
            personIds: [UserId],
            startDate: $("#FilterDateStart").val(),
            endDate: endDate,
            getActive: GetActive,
            filterForPLEView: false
        },
        "POST");
    });
}

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//
        
    //---------------------------------------DataTables------------
    
    //TableMainColumnSets
    TableMainColumnSets = [
        [1],
        [2, 3, 4, 5, 6, 7, 8, 9]
    ];

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
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
    showColumnSet(TableMainColumnSets, 1);

    //---------------------------------------EditFormView----------------------------------------//

    //Initialize DateTimePicker - EntryDTPicker
    $("#EntryDTPicker").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            var newEntryDate = moment($("#LogEntryDateTime").val()).minute(0)
                .hour($("#EntryDTPicker").data("DateTimePicker").date().hour());
            $("#LogEntryDateTime").val(newEntryDate.format("YYYY-MM-DD HH:mm"));
            $("#LogEntryDateTime").data("ismodified", true);
        });

    //Initialize DateTimePicker - HoursWorkedPicker
    $("#HoursWorkedPicker").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            $("#ManHours").val($("#HoursWorkedPicker").data("DateTimePicker").date().hour());
            $("#ManHours").data("ismodified", true);
        });

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1, null, {}, false, false);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1, null, {}, false, true);
    msAddToMsArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    msAddToMsArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue }, false, false);
    
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

    //Wire Up EditFormBtnPrsAddRemove
    $("#EditFormBtnPrsAddRemove").click(function () {
        TableLogEntryPersonsAdd.clear().search("").draw();
        TableLogEntryPersonsRemove.clear().search("").draw();

        if ($("#LogEntryPersonsView").hasClass("hidden")) {
            showModalWait();
            fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                "GET", "/PersonSrv/Get", { getActive: true })
                .always(hideModalWait)
                .done(function () {
                    $("#LogEntryPersonsView").removeClass("hidden");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
        else {
            $("#LogEntryPersonsView").addClass("hidden");
        }
    });
           
    //--------------------------------------View Initialization------------------------------------//

    $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
    refreshMainView();
    switchView(InitialViewId, MainViewId, MainViewBtnGroupClass);
  

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//



//---------------------------------------Helper Methods--------------------------------------//
