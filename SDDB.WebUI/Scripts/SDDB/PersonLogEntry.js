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
        CurrRecords[0] = RecordTemplate;
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Log Entry", "MainView");
        MagicSuggests[3].disable();
        MagicSuggests[4].disable();
        TableLogEntryAssysAdd.clear().search("").draw();
        TableLogEntryAssysRemove.clear().search("").draw();
        $("#EditFormBtnOkFiles").removeClass("disabled");

        showModalWait();
        fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
            "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", {  },
            "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", {  },
            "GET", "/PersonSrv/Get", { getActive: true })
            .always(hideModalWait)
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            if (GetActive) $("#EditFormGroupIsActive").addClass("hide");
            else $("#EditFormGroupIsActive").removeClass("hide");

            if (CurrIds.length > 1) $("#EditFormBtnOkFiles").addClass("disabled");
            else $("#EditFormBtnOkFiles").removeClass("disabled");

            showModalWait();

            $.when(
                fillFormForEditGeneric(CurrIds, "POST", "/PersonLogEntrySrv/GetByIds",
                    GetActive, "EditForm", "Edit Person Log Entry", MagicSuggests),

                fillFormForRelatedGeneric(TableLogEntryPersonsAdd, TableLogEntryPersonsRemove, CurrIds,
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersons", { logEntryId: CurrIds[0] },
                    "GET", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot", { logEntryId: CurrIds[0] },
                    "GET", "/PersonSrv/Get", { getActive: true })
                )
                .then(function (currRecords) {
                    CurrRecords = currRecords;
                    return fillFormForRelatedGeneric(TableLogEntryAssysAdd, TableLogEntryAssysRemove, CurrIds,
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssys", { logEntryId: CurrIds[0] },
                        "GET", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot", { logEntryId: CurrIds[0], locId: MagicSuggests[3].getValue()[0] },
                        "GET", "AssemblyDbSrv/LookupByLocDTables", { getActive: true });

                })
                .always(hideModalWait)
                .done(function () {
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Initialize DateTimePicker FilterDateEnd
    $("#FilterDateEnd").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
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
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByAssy
    $(MsFilterByAssy).on('selectionchange', function (e, m) { refreshMainView(); });
          
        
    //---------------------------------------DataTables------------
    
    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) { GetActive = true; $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary"); }
        else { GetActive = false; $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger"); }
        refreshMainView();
    });

    //TableMain PersonLogEntrys
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LogEntryDateTime", name: "LogEntryDateTime" },//1
            //------------------------------------------------first set of columns
            { data: "EnteredByPerson_", render: function (data, type, full, meta) { return data.LastName + " " + data.Initials }, name: "EnteredByPerson_" }, //2
            { data: "PersonActivityType_", render: function (data, type, full, meta) { return data.ActivityTypeName }, name: "PersonActivityType_" }, //3
            { data: "ManHours", name: "ManHours" },//4
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject_" }, //5
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
            { targets: [5, 6], className: "hidden-xs" }, // - first set of columns
            { targets: [3, 4], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7, 8], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns
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

    //Initialize DateTimePicker
    $("#LogEntryDateTime").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    addToMSArray(MagicSuggests, "EnteredByPerson_Id", "/PersonSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    addToMSArray(MagicSuggests, "AssignedToLocation_Id", "/LocationSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    addToMSArray(MagicSuggests, "AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj", 1, null,
        { projectIds: MagicSuggests[2].getValue });
    
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
    


    //--------------------------------------View Initialization------------------------------------//

    $("#FilterDateStart").val(moment().format("YYYY-MM-DD"));
    $("#FilterDateEnd").val(moment().format("YYYY-MM-DD"));

    if (typeof PersonId !== "undefined" && PersonId != "") {
        showModalWait();
        $.ajax({
            type: "POST", url: "/PersonSrv/GetAllByIds", timeout: 20000,
            data: { ids: [PersonId], getActive: true }, dataType: "json"
        })
            .always(hideModalWait)
            .done(function (data) {
                MsFilterByPerson.setSelection([{ id: data[0].Id, name: data[0].FirstName + " " + data[0].LastName + " " + data[0].Initials }]);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }

    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");
    

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    if ($("#FilterDateStart").val() == "" || $("#FilterDateEnd").val() == "" ||
            (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0
                && MsFilterByPerson.getValue().length == 0 && MsFilterByAssy.getValue().length == 0)
        ) {
        $("#ChBoxShowDeleted").bootstrapToggle("disable")
        TableMain.clear().search("").draw();
    }
    else {
        var endDate = moment($("#FilterDateEnd").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");

        refreshTblGenWrp(TableMain, "/PersonLogEntrySrv/GetByAltIds",
            {
                personIds: MsFilterByPerson.getValue(),
                typeIds: MsFilterByType.getValue(),
                projectIds: MsFilterByProject.getValue(),
                assyIds: MsFilterByAssy.getValue(),
                startDate: $("#FilterDateStart").val(),
                endDate: endDate,
                getActive: GetActive
            },
            "POST")
            .done(function () { $("#ChBoxShowDeleted").bootstrapToggle("enable"); })
    }
}

//---------------------------------------Helper Methods--------------------------------------//
