/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//


$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric("EditForm", MagicSuggests, "New Activity for " + $("#FilterDateStart").val(), "MainView");
        MagicSuggests[3].disable();
        MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw();
        TableLogEntryAssysRemove.clear().search("").draw();
        TableLogEntryPersonsAdd.clear().search("").draw();
        TableLogEntryPersonsRemove.clear().search("").draw();
        $("#LogEntryPersonsView").addClass("hide");
        MagicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);
        $("#LogEntryDateTime").val(moment($("#FilterDateStart").val()).hour(moment().hour()).format("YYYY-MM-DD HH:mm"));
        $("#LogEntryTime").data('DateTimePicker').date(moment($("#LogEntryDateTime").val()));
        $("#ManHours").val(0);
        $("#HoursWorkedPicker").data("DateTimePicker").date("00:00");

    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length != 1) {
            showModalSelectOne();
            return;
        }
        if (GetActive) { $("#EditFormGroupIsActive").addClass("hide"); }
        else { $("#EditFormGroupIsActive").removeClass("hide"); }
        TableLogEntryPersonsAdd.clear().search("").draw();
        TableLogEntryPersonsRemove.clear().search("").draw();
        $("#LogEntryPersonsView").addClass("hide");

        showModalWait();

        $.when(
            fillFormForEditGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                GetActive, "EditForm", "Edit Activity", MagicSuggests)
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
                    { getActive: true }
                );

            })
            .always(hideModalWait)
            .done(function () {
                $("#LogEntryTime").data('DateTimePicker').date(moment($("#LogEntryDateTime").val()));
                $("#LogEntryDateTime").data("ismodified", false);
                $("#HoursWorkedPicker").data('DateTimePicker').date(moment($("#ManHours").val(), "HH"));
                $("#ManHours").data("ismodified", false);
                $("#MainView").addClass("hide");
                $("#EditFormView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });
        
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) { GetActive = true; $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-tdo-success"); }
        else { GetActive = false; $("#PanelTableMain").removeClass("panel-tdo-success").addClass("panel-tdo-danger"); }
        refreshMainView();
    });

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            {
                data: "LogEntryDateTime", name: "LogEntryDateTime",
                render: function (data, type, full, meta) { return moment(data).format("HH:mm") }
            },//1
            //------------------------------------------------first set of columns
            {
                data: "EnteredByPerson_", name: "EnteredByPerson_",
                render: function (data, type, full, meta) { return data.Initials }
            }, //2
            {
                data: "PersonActivityType_", name: "PersonActivityType_",
                render: function (data, type, full, meta) { return data.ActivityTypeName }
            }, //3
            { data: "ManHours", name: "ManHours" },//4
            {
                data: "AssignedToProject_", name: "AssignedToProject_",
                render: function (data, type, full, meta) { return data.ProjectName }
            }, //5
            {
                data: "AssignedToLocation_", name: "AssignedToLocation_",
                render: function (data, type, full, meta) { return data.LocName }
            }, //6
            {
                data: "AssignedToProjectEvent_", name: "AssignedToProjectEvent_",
                render: function (data, type, full, meta) { return data.EventName }
            }, //7
            { data: "Comments", name: "Comments" },//8
            { data: "PersonLogEntryFilesCount", name: "PersonLogEntryFilesCount" },//9
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//10
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//11
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//12
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//13
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" },//14
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" }//15
        ],
        columnDefs: [
            { targets: [0, 10, 11, 12, 13, 14, 15], visible: false }, // - never show
            { targets: [0, 1, 4, 9, 10, 11, 12, 13, 14, 15], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 6], className: "hidden-xs" }, // - first set of columns
            { targets: [7, 9], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [8], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns
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

    //Initialize DateTimePicker - LogEntryTime
    $("#LogEntryTime").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            var entryDateTime = moment($("#FilterDateStart").val())
                .hour($(this).data('DateTimePicker').date().hour())
                .format("YYYY-MM-DD HH:mm");
            $("#LogEntryDateTime").val(entryDateTime);
            $("#LogEntryDateTime").data("ismodified", true);
        });

    //Initialize DateTimePicker - HoursWorkedPicker
    $("#HoursWorkedPicker").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            $("#ManHours").val($(this).data('DateTimePicker').date().hour());
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
        if (this.getValue().length == 0) {
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
        if (this.getValue().length == 0) {
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

        if ($("#LogEntryPersonsView").hasClass("hide")) {
            showModalWait();
            fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                "GET", "/PersonSrv/Get", { getActive: true } )
                .always(hideModalWait)
                .done(function () {
                    $("#LogEntryPersonsView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
        else {
            $("#LogEntryPersonsView").addClass("hide");
        }
    });
           
    //--------------------------------------View Initialization------------------------------------//

    $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
    refreshMainView();
    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");
  

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    if ($("#FilterDateStart").val() == "") {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = moment($("#FilterDateStart").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");

        refreshTblGenWrp(TableMain, "/PersonLogEntrySrv/GetByAltIds",
            {
                personIds: [UserId],
                startDate: $("#FilterDateStart").val(),
                endDate: endDate,
                getActive: GetActive,
                filterForPLEView: false
            },
            "POST")
            .done(function () { $("#ChBoxShowDeleted").bootstrapToggle("enable"); })
    }
}

//---------------------------------------Helper Methods--------------------------------------//
