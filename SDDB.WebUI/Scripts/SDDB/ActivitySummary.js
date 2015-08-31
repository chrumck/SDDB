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
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByPerson
    $(MsFilterByPerson).on("selectionchange", function (e, m) { refreshMainView(); });

    //Hide MsFilterByPerson if !CanViewOthers
    if (typeof CanViewOthers === "undefined" || !CanViewOthers) { MsFilterByPerson.disable(); } 
           
    //--------------------------------------View Initialization------------------------------------//

    $("#FilterDateStart").val(moment().day(0).format("YYYY-MM-DD"));
    MsFilterByPerson.setSelection([{ id: UserId, name: UserFullName }]);
       
    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");
  

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//refresh view after magicsuggest update
function refreshMainView() {
    var tableId = "TableMain";
    $("#" + tableId + " tbody").empty();
    if ($("#FilterDateStart").val() != "") {
        showModalWait();
        var startDate = $("#FilterDateStart").val();
        var endDate = moment(startDate).add(6, "days").hour(23).minute(59).format("YYYY-MM-DD HH:mm");
        $.ajax({
            type: "POST",
            url: "/PersonLogEntrySrv/GetActivitySummaries",
            timeout: 20000,
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
                fillSummaryTable(tableId, records);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    }
}

//---------------------------------------Helper Methods--------------------------------------//

function fillSummaryTable(tableId, records) {
    var colDate = moment($("#FilterDateStart").val());
    var colDates = [];
    $("#" + tableId + " thead th").each(function (i) {
        if (i == 0) { return true; }
        $(this).text(colDate.format("dd DD"));
        colDates.push(colDate);
        colDate.add(1, "days");
    });

    var maxNoOfRows = 0;
    var projectNames = [];
    for (var i in records) {
        maxNoOfRows = Math.max(maxNoOfRows, records[i].SummaryDetails.length)
        for (var j in records[i].SummaryDetails) {

        }
    }


    for (var i = 0; i <= maxNoOfRows; i++) {
        var row = $("<tr/>");
        var columnHours = 0
        if (i == 0) {
            row.append($("<td />").text("TOTAL"));
            for (var i in colDates) {
                
            }
        }
    }

    //$.each(records, function (i, record) {
    //    columnNumber = i + 2;
    //    columnName = moment(record.SummaryDay).format("dd DD")
    //    $("#" + tableId + " thead th:nth-child(" + columnNumber + ")").text(columnName)
    //});
    
}