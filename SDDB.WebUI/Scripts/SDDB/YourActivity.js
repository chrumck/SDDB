/*global sddb, UserFullName, UserId */
/// <reference path="Shared_Views.js" />
/// <reference path="PersonLogEntryShared.js" />

//----------------------------------------------additional sddb setup------------------------------------------------//
//moveToDate needed for copying to another date
sddb.moveToDate = {};

//setting up sddb
sddb.setConfig({

    tableMainColumnSets: [
        [1],
        [2, 3, 4, 5, 6, 7, 8, 9]
    ],

    tableMain: $("#tableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            {
                data: "LogEntryDateTime",
                name: "LogEntryDateTime",
                render: function (data, type, full, meta) {
                    "use strict";
                    return moment(data).format("HH:mm");
                }
            },//1
            //------------------------------------------------first set of columns
            {
                data: "EnteredByPerson_",
                name: "EnteredByPerson_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.Initials; 
                }
            }, //2
            {
                data: "PersonActivityType_",
                name: "PersonActivityType_",
                render: function (data, type, full, meta) { 
                    "use strict";
                    return data.ActivityTypeName; 
                }
            }, //3
            { data: "ManHours", name: "ManHours" },//4
            {
                data: "AssignedToProject_",
                name: "AssignedToProject_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.ProjectName;
                }
            }, //5
            {
                data: "AssignedToLocation_",
                name: "AssignedToLocation_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.LocName;
                }
            }, //6
            {
                data: "AssignedToProjectEvent_",
                name: "AssignedToProjectEvent_",
                render: function (data, type, full, meta) {
                    "use strict";
                    return data.EventName; 
                }
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
    }),

    tableMainPanelClassActive: "panel-tdo-success",


    labelTextCreate: function () {
        "use strict";
        return "New Activity for " + $("#filterDateStart").val();
    },
    labelTextEdit: function () {
        "use strict";
        return "Edit Activity for " + $("#filterDateStart").val();
    },
    labelTextCopy: function () {
        "use strict";
        return "Copy Activity to " + sddb.moveToDate.format("YYYY-MM-DD");
    },

    urlFillForEdit: "/PersonLogEntrySrv/GetYourByIds"
});

//callBackAfterCreate
sddb.callBackAfterCreate = function () {
    "use strict";
    var newLogEntryDT = moment($("#filterDateStart").val()).hour(moment().hour()).minute(0);
    $("#editFormBtnOkFiles").prop("disabled", false);
    $("#logEntryPersonsView").addClass("hidden");
    $("#LogEntryDateTime").val(newLogEntryDT.format("YYYY-MM-DD HH:mm"));
    $("#entryDTPicker").data("DateTimePicker").date(newLogEntryDT);
    $("#ManHours").val(0);
    $("#hoursWorkedPicker").data("DateTimePicker").date("00:00");
    sddb.cfg.magicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);
    sddb.cfg.magicSuggests[3].disable();
    sddb.cfg.magicSuggests[4].disable();
    sddb.tableLogEntryAssysAdd.clear().search("").draw();
    sddb.tableLogEntryAssysRemove.clear().search("").draw();
    sddb.tableLogEntryPersonsAdd.clear().search("").draw();
    sddb.tableLogEntryPersonsRemove.clear().search("").draw();

    return $.Deferred().resolve();
};
//callBackAfterEdit
sddb.callBackAfterEdit = function (currRecords) {
    "use strict";
    var newLogEntryDT = moment($("#filterDateStart").val()).hour(moment().hour()).minute(0);

    if (sddb.cfg.currentIds.length > 1) { $("#editFormBtnOkFiles").prop("disabled", true); }
    else { $("#editFormBtnOkFiles").prop("disabled", false); }

    $("#logEntryPersonsView").addClass("hidden");
    sddb.tableLogEntryPersonsAdd.clear().search("").draw();
    sddb.tableLogEntryPersonsRemove.clear().search("").draw();

    if ($("#LogEntryDateTime").val() === "_VARIES_") {
        $("#LogEntryDateTime").val(newLogEntryDT.format("YYYY-MM-DD HH:mm"));
    }
    $("#entryDTPicker").data("DateTimePicker").date(moment($("#LogEntryDateTime").val()));
    $("#LogEntryDateTime").data("ismodified", false);

    if ($("#ManHours").val() === "_VARIES_") {
        $("#ManHours").val(0);
    }
    $("#hoursWorkedPicker").data("DateTimePicker").date(moment($("#ManHours").val(), "HH"));
    $("#ManHours").data("ismodified", false);

    return sddb.modalWaitWrapper(function () {
        return sddb.fillFormForRelatedGeneric(sddb.tableLogEntryAssysAdd, sddb.tableLogEntryAssysRemove,
                sddb.cfg.currentIds,
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryAssys",
                { ids: sddb.cfg.currentIds },
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                { ids: sddb.cfg.currentIds, locId: sddb.cfg.magicSuggests[3].getValue()[0] });
    });
};
//callBackBeforeCopy
sddb.callBackBeforeCopy = function () {
    "use strict";
    return sddb.showModalDatePrompt("NOTE: Assemblies, People and Files are not copied!",
                "Copy To Date:", $("#filterDateStart").val())
        .then(function (outputDate) { sddb.moveToDate = outputDate; });
};
//callBackAfterCopy
sddb.callBackAfterCopy = function () {
    "use strict";
    $("#editFormBtnOkFiles").prop("disabled", false);
    $("#logEntryPersonsView").addClass("hidden");
    sddb.tableLogEntryAssysAdd.clear().search("").draw();
    sddb.tableLogEntryAssysRemove.clear().search("").draw();
    sddb.tableLogEntryPersonsAdd.clear().search("").draw();
    sddb.tableLogEntryPersonsRemove.clear().search("").draw();

    var copyToDateTime = moment($("#LogEntryDateTime").val())
            .year(sddb.moveToDate.year()).dayOfYear(sddb.moveToDate.dayOfYear());
    $("#LogEntryDateTime").val(copyToDateTime.format("YYYY-MM-DD HH:mm"));
    $("#entryDTPicker").data("DateTimePicker").date(copyToDateTime);
    $("#hoursWorkedPicker").data("DateTimePicker").date(moment($("#ManHours").val(), "HH"));
    sddb.cfg.magicSuggests[0].setSelection([{ id: UserId, name: UserFullName }]);

    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(sddb.tableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
            { getActive: true, locId: sddb.cfg.magicSuggests[3].getValue()[0] });
    });
};
//refreshMainView
sddb.refreshMainView = function () {
    "use strict";
    sddb.cfg.tableMain.clear().search("").draw();
    if ($("#filterDateStart").val() === "") { return $.Deferred().resolve(); }

    var endDate = moment($("#filterDateStart").val()).hour(23).minute(59).format("YYYY-MM-DD HH:mm");
    return sddb.modalWaitWrapper(function () {
        return sddb.refreshTableGeneric(sddb.cfg.tableMain, "/PersonLogEntrySrv/GetYourByAltIds",
        {
            personIds: [UserId],
            startDate: $("#filterDateStart").val(),
            endDate: endDate,
            getActive: sddb.cfg.currentActive
        },
        "POST");
    });
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//

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
    sddb.msAddToArray("EnteredByPerson_Id", "/PersonSrv/Lookup");
    sddb.msAddToArray("PersonActivityType_Id", "/PersonActivityTypeSrv/Lookup", { editable: false });
    sddb.msAddToArray("AssignedToProject_Id", "/ProjectSrv/Lookup", {}, function () {
        sddb.cfg.magicSuggests[3].clear();
        sddb.cfg.magicSuggests[4].clear();
        sddb.tableLogEntryAssysAdd.clear().search("").draw();
        if (this.getValue().length === 0) {
            sddb.cfg.magicSuggests[3].disable();
            sddb.cfg.magicSuggests[4].disable();
        }
        else {
            sddb.cfg.magicSuggests[3].enable();
            sddb.cfg.magicSuggests[4].enable();
        }
    });
    sddb.msAddToArray("AssignedToLocation_Id", "/LocationSrv/LookupByProj",
        { dataUrlParams: { projectIds: sddb.cfg.magicSuggests[2].getValue } }, function () {
            sddb.tableLogEntryAssysAdd.clear().search("").draw();
            if (this.getValue().length === 0) { return; }
            sddb.modalWaitWrapper(function () {
                if (sddb.cfg.currentIds.length !== 0) {
                    return sddb.refreshTableGeneric(sddb.tableLogEntryAssysAdd,
                        "/PersonLogEntrySrv/GetPrsLogEntryAssysNot",
                        { ids: sddb.cfg.currentIds, locId: sddb.cfg.magicSuggests[3].getValue()[0] }, "POST");
                }
                return sddb.refreshTableGeneric(sddb.tableLogEntryAssysAdd, "AssemblyDbSrv/LookupByLocDTables",
                        { getActive: true, locId: sddb.cfg.magicSuggests[3].getValue()[0] });
            })
                .done(function () { $("#AssignedToLocation_Id input").focus(); });
        });
    sddb.msAddToArray("AssignedToProjectEvent_Id", "/ProjectEventSrv/LookupByProj",
        { dataUrlParams: { projectIds: sddb.cfg.magicSuggests[2].getValue } });
    
    //Wire Up editFormBtnPrsAddRemove
    $("#editFormBtnPrsAddRemove").click(function () {
        sddb.tableLogEntryPersonsAdd.clear().search("").draw();
        sddb.tableLogEntryPersonsRemove.clear().search("").draw();
        if (!$("#logEntryPersonsView").hasClass("hidden")) {
            $("#logEntryPersonsView").addClass("hidden");
            return;
        }
        sddb.modalWaitWrapper(function () {
            return sddb.fillFormForRelatedGeneric(sddb.tableLogEntryPersonsAdd, sddb.tableLogEntryPersonsRemove,
                sddb.cfg.currentIds,
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryPersons",
                { ids: sddb.cfg.currentIds },
                "POST", "/PersonLogEntrySrv/GetPrsLogEntryPersonsNot",
                { ids: sddb.cfg.currentIds });
        })
            .done(function () {
                $("#logEntryPersonsView").removeClass("hidden");
            });

    });

    //--------------------------------------View Initialization------------------------------------//

    $("#filterDateStart").val(moment().format("YYYY-MM-DD"));
    sddb.refreshMainView();
    sddb.switchView();

    //--------------------------------End of setup after page load---------------------------------//   
});

