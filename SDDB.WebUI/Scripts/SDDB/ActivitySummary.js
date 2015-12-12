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

//--------------------------------------Global Properties------------------------------------//

var msFilterByPerson;

$(document).ready(function () {

    //-----------------------------------------mainView------------------------------------------//


    //Initialize DateTimePicker filterDateStart
    $("#filterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });


    //Initialize MagicSuggest msFilterByPerson
    msFilterByPerson = $("#msFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        maxSelection: 1,
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for msFilterByPerson
    $(msFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });

    //Hide msFilterByPerson if !CanViewOthers
    if (typeof CanViewOthers === "undefined" || !CanViewOthers) { msFilterByPerson.disable(); } 
           
    //--------------------------------------View Initialization------------------------------------//
           
    $("#initialView").addClass("hidden");
    $("#mainView").removeClass("hidden");

    var weekOffset = $("#thNo6").is(":hidden") && $("#thNo7").is(":hidden") ? 1: 0;
    $("#filterDateStart").val(moment().day(weekOffset).format("YYYY-MM-DD"));

    msFilterByPerson.setSelection([{ id: UserId, name: UserFullName }]);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    var tableMain = $("#tableMain");
    var startDate = $("#filterDateStart").val();

    tableMain.find("tbody").empty();
    clearSumTblFirstRowHelper(tableMain);

    if (startDate != "" && msFilterByPerson.getSelection().length != 0) {
        showModalWait();
        var endDate = moment(startDate).add(6, "days").hour(23).minute(59).format("YYYY-MM-DD HH:mm");
        $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/GetActivitySummaries",
            timeout: 120000,
            data: {
                personId: (msFilterByPerson.getSelection())[0].id,
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

    var projectNames = getProjectNamesHelper(records);

    $.each(projectNames, function (i, projectName) {

        var row = $("<tr/>");
        row.append($("<td />").text(projectName));
        $.each(columnDates, function (j, columnDate) {
            var columnHours = getColumnHoursHelper(records, columnDate, projectName, i == 0);
            row.append($("<td />").text(columnHours));
            if (j >= columnDates.length - 2) { row.find("td:last").addClass("hidden-xs"); }
        });
        tableMain.append(row);
    });
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
function getProjectNamesHelper(records) {
    var projectNames = [];
    projectNames.push("_TOTAL_");
    $.each(records, function (i, record) {
        $.each(record.SummaryDetails, function (j, detail) {
            if ($.inArray(detail.ProjectName, projectNames) == -1) { projectNames.push(detail.ProjectName); }
        });
    });
    return projectNames;
}

//getColumnHoursHelper - used by fillSummaryTableHelper
function getColumnHoursHelper(records, columnDate, projectName, isTotalHoursRow) {
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
}

