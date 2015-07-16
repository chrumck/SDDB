/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//


var TableMain;
var TableLogEntryAssysAdd;
var TableLogEntryAssysRemove;
var TableLogEntryPersonsAdd;
var TableLogEntryPersonsRemove;
var MagicSuggests = [];
var CurrRecord = {
    Id: null,
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
var CurrIds = [];
var GetActive = true;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Activity", "MainView");
        MagicSuggests[3].disable();
        MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw();
        TableLogEntryAssysRemove.clear().search("").draw();
        TableLogEntryPersonsAdd.clear().search("").draw();
        TableLogEntryPersonsRemove.clear().search("").draw();
        $("#LogEntryPersonsView").addClass("hide");
        MagicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);
        $("#LogEntryDateTime").val(moment().minute(0).format("YYYY-MM-DD HH:mm"));
        $("#ManHours").val(0);
        $("#HoursWorkedPicker").data("DateTimePicker").date("00:00");

    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length != 1) showModalSelectOne();
        else {
            if (GetActive) $("#EditFormGroupIsActive").addClass("hide");
            else $("#EditFormGroupIsActive").removeClass("hide");

            TableLogEntryPersonsAdd.clear().search("").draw();
            TableLogEntryPersonsRemove.clear().search("").draw();
            $("#LogEntryPersonsView").addClass("hide");

            showModalWait();

            $.when(
                fillFormForEditGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                    GetActive, "EditForm", "Edit Activity", MagicSuggests)
                )
                .then(function (currRecord) {
                    CurrRecord = currRecord;
                    return fillFormForRelatedGeneric(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys", { logEntryId: CurrIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot", { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                        "GET", "AssemblyDbSrv/LookupByLocDTables", { getActive: true });

                })
                .always(hideModalWait)
                .done(function () {
                    $("#LogEntryTime").data('DateTimePicker').date(moment($("#LogEntryDateTime").val()));
                    $("#HoursWorkedPicker").data('DateTimePicker').date(moment($("#ManHours").val(),"HH"));
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });
        
    //---------------------------------------DataTables------------

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", render: function (data, type, full, meta) { return moment(data).format("HH:mm") }, name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            { data: "EnteredByPerson_", render: function (data, type, full, meta) { return data.Initials }, name: "EnteredByPerson_" }, //2
            { data: "PersonActivityType_", render: function (data, type, full, meta) { return data.ActivityTypeName }, name: "PersonActivityType_" }, //3
            { data: "ManHours", name: "ManHours" },//4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName }, name: "AssignedToProject_" }, //5
            { data: "AssignedToLocation_", render: function (data, type, full, meta) { return data.LocName }, name: "AssignedToLocation_" }, //6
            { data: "AssignedToProjectEvent_", render: function (data, type, full, meta) { return data.EventName }, name: "AssignedToProjectEvent_" }, //7
            { data: "Comments", name: "Comments" },//8
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" },//9
            { data: "EnteredByPerson_Id", name: "EnteredByPerson_Id" },//10
            { data: "PersonActivityType_Id", name: "PersonActivityType_Id" },//11
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//12
            { data: "AssignedToLocation_Id", name: "AssignedToLocation_Id" },//13
            { data: "AssignedToProjectEvent_Id", name: "AssignedToProjectEvent_Id" },//14
        ],
        columnDefs: [
            { targets: [0, 9, 10, 11, 12, 13, 14], visible: false }, // - never show
            { targets: [0, 1, 4, 9, 10, 11, 12, 13, 14], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 6], className: "hidden-xs" }, // - first set of columns
            { targets: [7, 8], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns
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
            $("#LogEntryDateTime").val(moment($("#FilterDateStart").val()).hour($(this).data('DateTimePicker').date().hour()).format("YYYY-MM-DD HH:mm"));
            $("#LogEntryDateTime").data("ismodified", true);
        });

    //Initialize DateTimePicker - HoursWorkedPicker
    $("#HoursWorkedPicker").datetimepicker({ format: "HH", inline: true })
        .on("dp.change", function (e) {
            $("#ManHours").val($(this).data('DateTimePicker').date().hour());
            $("#ManHours").data("ismodified", true);
        });

    //Initialize MagicSuggest Array
    addToMSArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1, null, {},
        false, false);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1, null,
        {}, false, false);
    addToMSArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    addToMSArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue }, false, false);
    
    //Initialize MagicSuggest Array Event - AssignedToProject_Id
    $(MagicSuggests[2]).on("selectionchange", function (e, m) {
        MagicSuggests[3].clear(true);
        MagicSuggests[3].isModified = false;
        MagicSuggests[4].clear(true);
        MagicSuggests[4].isModified = false;
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
        }
        else {
            
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
                getActive: GetActive
            },
            "POST")
            .done(function () { $("#ChBoxShowDeleted").bootstrapToggle("enable"); })
    }
}

//---------------------------------------Helper Methods--------------------------------------//
