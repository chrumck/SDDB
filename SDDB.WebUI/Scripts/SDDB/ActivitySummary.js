/*global sddb, UserId, UserFullName, CanViewOthers*/
/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />
/// <reference path="Shared_Views.js" />


//refresh view after magicsuggest update
sddb.refreshMainView = function () {
    "use strict";
    //fillSummaryTableHelper
    var fillSummaryTableHelper = function (targetTable, startDate, records) {

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
        fillSumTblFirstRowHelper = function (targetTable, columnDates) {
            targetTable.find("thead th").each(function (i) {
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
        //
        columnDates = getColumnDatesHelper(startDate),
        projectNames = getProjectNamesHelper(records);

        //main
        fillSumTblFirstRowHelper(targetTable, columnDates);

        $.each(projectNames, function (i, projectName) {
            var row = $("<tr/>");
            row.append($("<td />").text(projectName));
            $.each(columnDates, function (j, columnDate) {
                var columnHours = getColumnHoursHelper(records, columnDate, projectName, i === 0);
                row.append($("<td />").text(columnHours));
                if (j >= columnDates.length - 2) { row.find("td:last").addClass("hidden-xs"); }
            });
            targetTable.append(row);
        });
    },
    //clearSumTblFirstRowHelper
    clearSumTblFirstRowHelper = function (targetTable) {
        targetTable.find("thead th").each(function (i) {
            if (i === 0) { return true; }
            $(this).text(i);
        });
    },
    //
    targetTable = $("#tableMain"),
    startDate = $("#filterDateStart").val(),
    endDate;

    //main
    targetTable.find("tbody").empty();
    clearSumTblFirstRowHelper(targetTable);

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
                fillSummaryTableHelper(targetTable, startDate, records);
            })
            .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });
    }
};

//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {
    "use strict";
    //-----------------------------------------mainView------------------------------------------//
        
    //Initialize DateTimePicker filterDateStart
    $("#filterDateStart").on("dp.hide", function (event) { sddb.refreshMainView(); });
    
    //Initialize MagicSuggest sddb.msFilterByPerson
    sddb.msFilterByPerson = sddb.msSetFilter("msFilterByPerson", "/PersonSrv/Lookup", { maxSelection: 1 });
    
    //Hide sddb.msFilterByPerson if !CanViewOthers
    if (typeof CanViewOthers === "undefined" || !CanViewOthers) { sddb.msFilterByPerson.disable(); } 
           
    //--------------------------------------View Initialization------------------------------------//

    sddb.switchView();

    sddb.weekOffset = $("#thNo6").is(":hidden") && $("#thNo7").is(":hidden") ? 1: 0;
    $("#filterDateStart").val(moment().day(sddb.weekOffset).format("YYYY-MM-DD"));

    sddb.msFilterByPerson.setSelection([{ id: UserId, name: UserFullName }]);
    
    //--------------------------------End of setup after page load---------------------------------//
});






