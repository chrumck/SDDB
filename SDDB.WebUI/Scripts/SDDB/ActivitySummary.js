/*global sddb, sddbConf*/
/*global UserId, UserFullName, CanViewOthers*/
/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

"use strict";

//--------------------------------------Global Properties------------------------------------//

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//
        
    //Initialize DateTimePicker filterDateStart
    $("#filterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (event) { sddb.refreshMainView(); });


    //Initialize MagicSuggest sddb.msFilterByPerson
    sddb.msFilterByPerson = $("#msFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { sddb.showModalFail(xhr, status, error); }
        },
        maxSelection: 1,
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for sddb.msFilterByPerson
    $(sddb.msFilterByPerson).on("selectionchange", function (event) { sddb.refreshMainView(); });

    //Hide sddb.msFilterByPerson if !CanViewOthers
    if (typeof CanViewOthers === "undefined" || !CanViewOthers) { sddb.msFilterByPerson.disable(); } 
           
    //--------------------------------------View Initialization------------------------------------//

    sddb.switchView();

    sddb.weekOffset = $("#thNo6").is(":hidden") && $("#thNo7").is(":hidden") ? 1: 0;
    $("#filterDateStart").val(moment().day(sddb.weekOffset).format("YYYY-MM-DD"));

    sddb.msFilterByPerson.setSelection([{ id: UserId, name: UserFullName }]);
    
    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
sddb.refreshMainView = function () {
    //fillSummaryTableHelper
    var fillSummaryTableHelper = function (tableMain, startDate, records) {

        //getColumnDatesHelper for fillSummaryTableHelper
        var getColumnDatesHelper = function (startDate) {
            var columnDate = moment(startDate),
            columnDates = [],
            i;

            for (i = 0; i < 7; i += 1) {
                columnDates.push(columnDate.clone());
                columnDate.add(1, "days");
            }
            return columnDates;
        },
        //fillSumTblFirstRowHelper for fillSummaryTableHelper
        fillSumTblFirstRowHelper = function (tableMain, columnDates) {
            tableMain.find("thead th").each(function (i) {
                if (i === 0) { return true; }
                $(this).text(columnDates[i - 1].format("dd DD"));
            });
        },
        //getProjectNamesHelper for fillSummaryTableHelper
        getProjectNamesHelper = function (records) {
            var projectNames = [];
            projectNames.push("_TOTAL_");
            $.each(records, function (i, record) {
                $.each(record.SummaryDetails, function (j, detail) {
                    if ($.inArray(detail.ProjectName, projectNames) == -1) { projectNames.push(detail.ProjectName); }
                });
            });
            return projectNames;
        },
        //getColumnHoursHelper for fillSummaryTableHelper
        getColumnHoursHelper = function (records, columnDate, projectName, isTotalHoursRow) {
            var columnHours = 0;
            $.each(records, function (i, record) {
                if (moment(record.SummaryDay).isSame(columnDate)) {
                    if (isTotalHoursRow) {
                        columnHours = record.TotalManHours;
                        return false;
                    }
                    $.each(record.SummaryDetails, function (j, detail) {
                        if (detail.ProjectName == projectName) {
                            columnHours = detail.ManHours;
                            return false;
                        }
                    });
                    return false;
                }
            });
            return columnHours;
        },
        columnDates = getColumnDatesHelper(startDate),
        projectNames = getProjectNamesHelper(records);

        //main
        fillSumTblFirstRowHelper(tableMain, columnDates);

        $.each(projectNames, function (i, projectName) {
            var row = $("<tr/>");
            row.append($("<td />").text(projectName));
            $.each(columnDates, function (j, columnDate) {
                var columnHours = getColumnHoursHelper(records, columnDate, projectName, i === 0);
                row.append($("<td />").text(columnHours));
                if (j >= columnDates.length - 2) { row.find("td:last").addClass("hidden-xs"); }
            });
            tableMain.append(row);
        });
    },
    //clearSumTblFirstRowHelper
    clearSumTblFirstRowHelper = function (tableMain) {
        tableMain.find("thead th").each(function (i) {
            if (i === 0) { return true; }
            $(this).text(i);
        });
    },
    //private vars
    tableMain = $("#tableMain"),
    startDate = $("#filterDateStart").val(),
    endDate;

    //main
    tableMain.find("tbody").empty();
    clearSumTblFirstRowHelper(tableMain);

    if (startDate !== "" && sddb.msFilterByPerson.getSelection().length !== 0) {
        sddb.showModalWait();
        endDate = moment(startDate).add(6, "days").hour(23).minute(59).format("YYYY-MM-DD HH:mm");
        $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/GetActivitySummaries",
            timeout: 120000,
            data: {
                personId: (sddb.msFilterByPerson.getSelection())[0].id,
                startDate: startDate,
                endDate: endDate,
                getActive: true
            },
            dataType: "json"
        })
            .always(sddb.hideModalWait)
            .done(function (records) {
                fillSummaryTableHelper(tableMain, startDate, records);
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });
    }
};

//---------------------------------------Helper Methods--------------------------------------//


