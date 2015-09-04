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



$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//


    //Initialize DateTimePicker FilterDateStart
    $("#FilterDateStart").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.hide", function (e) { refreshMainView(); });


    //Initialize MagicSuggest MsFilterByPerson
    MsFilterByPerson = $("#MsFilterByPerson").magicSuggest({
        data: "/PersonSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        maxSelection: 1,
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByPerson
    $(MsFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });

    //Hide MsFilterByPerson if !CanViewOthers
    if (typeof CanViewOthers === "undefined" || !CanViewOthers) { MsFilterByPerson.disable(); } 
           
    //--------------------------------------View Initialization------------------------------------//
           
    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    var weekOffset = 0
    if ($("#thNo6").is(":hidden") && $("#thNo7").is(":hidden")) { weekOffset = 1; }
    $("#FilterDateStart").val(moment().day(weekOffset).format("YYYY-MM-DD"));

    MsFilterByPerson.setSelection([{ id: UserId, name: UserFullName }]);

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    var $table = $("#TableMain");
    var startDate = $("#FilterDateStart").val();

    $table.find("tbody").empty();
    clearSumTblFirstRowHelper($table);

    if (startDate != "" && MsFilterByPerson.getSelection().length != 0) {
        showModalWait();
        var endDate = moment(startDate).add(6, "days").hour(23).minute(59).format("YYYY-MM-DD HH:mm");
        $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/GetActivitySummaries",
            timeout: 120000,
            data: {
                personId: (MsFilterByPerson.getSelection())[0].id,
                startDate: startDate,
                endDate: endDate,
                getActive: true
            },
            dataType: "json"
        })
            .always(hideModalWait)
            .done(function (records) {
                fillSummaryTableHelper($table, startDate, records);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
}

//---------------------------------------Helper Methods--------------------------------------//

//fillSummaryTableHelper - used by refreshMainView
function fillSummaryTableHelper($table, startDate, records) {
    
    var columnDates = getColumnDatesHelper(startDate);

    fillSumTblFirstRowHelper($table, columnDates);

    var projectNames = getProjectNamesHelper(records);

    $.each(projectNames, function (i, projectName) {

        var row = $("<tr/>");
        row.append($("<td />").text(projectName));
        $.each(columnDates, function (j, columnDate) {
            var columnHours = getColumnHoursHelper(records, columnDate, projectName, i == 0);
            row.append($("<td />").text(columnHours));
            if (j >= columnDates.length - 2) { row.find("td:last").addClass("hidden-xs"); }
        });
        $table.append(row);
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
function fillSumTblFirstRowHelper($table, columnDates) {
    $table.find("thead th").each(function (i) {
        if (i == 0) { return true; }
        $(this).text(columnDates[i - 1].format("dd DD"));
    });
}

//clearSumTblFirstRowHelper - used by refreshMainView
function clearSumTblFirstRowHelper($table) {
    $table.find("thead th").each(function (i) {
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

