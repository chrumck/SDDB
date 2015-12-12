/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var msFilterByActivityType;

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//


    //Initialize DateTimePicker filterDateStart
    $("#filterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });


    //Initialize MagicSuggest msFilterByActivityType
    msFilterByActivityType = $("#msFilterByActivityType").magicSuggest({
        data: "/PersonActivityTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        maxSelection: 1,
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByActivityType
    $(msFilterByActivityType).on("selectionchange", function (e, m) { refreshMainView(); });
           
    //--------------------------------------View Initialization------------------------------------//
           
    $("#initialView").addClass("hidden");
    $("#mainView").removeClass("hidden");

    var weekOffset = 0
    if ($("#thNo6").is(":hidden") && $("#thNo7").is(":hidden")) { weekOffset = 1; }
    $("#filterDateStart").val(moment().day(weekOffset).format("YYYY-MM-DD"));

    refreshMainView();


    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    var tableMain = $("#tableMain");
    var startDate = $("#filterDateStart").val();

    tableMain.find("tbody").empty();
    clearSumTblFirstRowHelper(tableMain);

    if (startDate != "" && msFilterByActivityType.getSelection().length != 0) {
        showModalWait();
        var endDate = moment(startDate).add(6, "days").hour(23).minute(59).format("YYYY-MM-DD HH:mm");
        $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/GetLastEntrySummaries",
            timeout: 120000,
            data: {
                activityTypeId: (msFilterByActivityType.getSelection())[0].id,
                startDate: startDate,
                endDate: endDate,
                getActive: true
            },
            dataType: "json"
        })
            .always(hideModalWait)
            .done(function (records) {
                fillSummaryTableHelper(tableMain, startDate, records);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
}

//---------------------------------------Helper Methods--------------------------------------//

//fillSummaryTableHelper - used by refreshMainView
function fillSummaryTableHelper(tableMain, startDate, records) {
    
    var columnDates = getColumnDatesHelper(startDate);

    fillSumTblFirstRowHelper(tableMain, columnDates);

    getCurrentProjectsHelper().done(function (currentProjects) {
        $.each(currentProjects, function (i, project) {

            var row = $("<tr/>");
            row.append($("<td />").text(project.name));
            $.each(columnDates, function (j, columnDate) {
                var tableData = getColumnDataHelper(records, columnDate, project);
                row.append($("<td />").html(tableData));
                if (j >= columnDates.length - 2) { row.find("td:last").addClass("hidden-xs"); }
            });
            tableMain.append(row);
        });
    })
}

//getColumnHoursHelper - used by fillSummaryTableHelper
function getColumnDataHelper(records, columnDate, project) {
    var tableData = "<span style='color:red'><em>No Entries</em></span>";
    $.each(records, function (i, record) {
        if (moment(record.SummaryDay).isSame(columnDate) && record.ProjectId == project.id) {
            var qtyText = (record.TotalEntries === 1) ? "entry: " : "entries, last: ";
            tableData = "<strong>" + record.TotalEntries + " " + qtyText + "</strong> " + record.LastEntryDateTime + "<br />" +
                "<strong>" + record.LastEntryPersonInitials + ":</strong> " + $($.parseHTML(record.LastEntryComments)).text();
        }
    });
    return tableData;
}

//getColumnDatesHelper - used by fillSummaryTableHelper
function getColumnDatesHelper(startDate) {
    var columnDate = moment(startDate);
    var columnDates = [];
    for (var i = 0; i < 7; i++) {
        columnDates.push(columnDate.clone());
        columnDate.add(1, "days");
    }
    return columnDates;
}

//fillSumTblFirstRowHelper - used by fillSummaryTableHelper
function fillSumTblFirstRowHelper(tableMain, columnDates) {
    tableMain.find("thead th").each(function (i) {
        if (i == 0) { return true; }
        $(this).text(columnDates[i - 1].format("dd DD"));
    });
}

//clearSumTblFirstRowHelper - used by refreshMainView
function clearSumTblFirstRowHelper(tableMain) {
    tableMain.find("thead th").each(function (i) {
        if (i == 0) { return true; }
        $(this).text(i);
    });
}

//getProjectNamesHelper - used by fillSummaryTableHelper
function getCurrentProjectsHelper() {
    showModalWait();
    return $.ajax({
            type: "POST",
            url: "/ProjectSrv/Lookup",
            timeout: 120000,
            data: { getActive: true },
            dataType: "json"
        })
        .always(hideModalWait)
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}



