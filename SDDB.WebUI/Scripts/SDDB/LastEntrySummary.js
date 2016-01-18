/*global sddb*/
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
    //clearSumTblFirstRowHelper - used by sddb.refreshMainView
    var clearSumTblFirstRowHelper = function (table) {
        table.find("thead th").each(function (i) {
            if (i === 0) { return true; }
            $(this).text(i);
        });
    },

    //fillSummaryTableHelper - used by sddb.refreshMainView
    fillSummaryTableHelper = function (table, startDate, records) {
        //getColumnDatesHelper - used by fillSummaryTableHelper
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
        //fillSumTblFirstRowHelper - used by fillSummaryTableHelper
        fillSumTblFirstRowHelper = function (table, columnDates) {
            table.find("thead th").each(function (i) {
                if (i === 0) { return true; }
                $(this).text(columnDates[i - 1].format("dd DD"));
            });
        },
        //getProjectNamesHelper - used by fillSummaryTableHelper
        getCurrentProjectsHelper = function () {
            sddb.showModalWait();
            return $.ajax({
                type: "POST",
                url: "/ProjectSrv/Lookup",
                timeout: 120000,
                data: { getActive: true },
                dataType: "json"
            })
                .always(sddb.hideModalWait)
                .fail(function (xhr, status, error) { sddb.showModalFail(xhr, status, error); });
        },
        //getColumnHoursHelper - used by fillSummaryTableHelper
        getColumnDataHelper = function (records, columnDate, project) {
            var tableData = "<span style='color:red'><em>No Entries</em></span>";

            $.each(records, function (i, record) {
                if (moment(record.SummaryDay).isSame(columnDate) && record.ProjectId == project.id) {
                    var qtyText = (record.TotalEntries === 1) ? "entry: " : "entries, last: ";
                    tableData = "<strong>" + record.TotalEntries + " " + qtyText + "</strong> " +
                        record.LastEntryDateTime + "<br />" + "<strong>" + record.LastEntryPersonInitials +
                        ":</strong> " + $($.parseHTML(record.LastEntryComments)).text();
                }
            });
            return tableData;
        },
        //
        columnDates = getColumnDatesHelper(startDate);

        //main
        fillSumTblFirstRowHelper(table, columnDates);

        getCurrentProjectsHelper().done(function (currentProjects) {
            $.each(currentProjects, function (i, project) {

                var row = $("<tr/>");
                row.append($("<td />").text(project.name));
                $.each(columnDates, function (j, columnDate) {
                    var tableData = getColumnDataHelper(records, columnDate, project);
                    row.append($("<td />").html(tableData));
                    if (j >= columnDates.length - 2) { row.find("td:last").addClass("hidden-xs"); }
                });
                table.append(row);
            });
        });
    },
    //
    targetTable = $("#tableMain"),
    startDate = $("#filterDateStart").val(),
    endDate;

    //main
    targetTable.find("tbody").empty();
    clearSumTblFirstRowHelper(targetTable);

    if (startDate !== "" && sddb.msFilterByActivityType.getSelection().length !== 0) {
        sddb.showModalWait();
        endDate = moment(startDate).add(6, "days").hour(23).minute(59).format("YYYY-MM-DD HH:mm");
        $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/GetLastEntrySummaries",
            timeout: 120000,
            data: {
                activityTypeId: (sddb.msFilterByActivityType.getSelection())[0].id,
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
    $("#filterDateStart").on("dp.hide", function (e) { sddb.refreshMainView(); });

    //Initialize MagicSuggest msFilterByActivityType
    sddb.msFilterByActivityType = sddb.msSetFilter("msFilterByActivityType", "/PersonActivityTypeSrv/Lookup",
        { maxSelection: 1 });
           
    //--------------------------------------View Initialization------------------------------------//
           
    sddb.switchView();

    sddb.weekOffset = $("#thNo6").is(":hidden") && $("#thNo7").is(":hidden") ? 1 : 0;
    $("#filterDateStart").data("DateTimePicker").date(moment().day(sddb.weekOffset));
    sddb.refreshMainView();


    //--------------------------------End of setup after page load---------------------------------//
});

