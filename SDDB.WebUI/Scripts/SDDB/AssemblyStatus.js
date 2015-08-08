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
var MsFilterByProject = {};
var MsFilterByType = {};
var MsFilterByLoc = {};
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    AssyStatusName: null,
    AssyStatusAltName: null,
    Comments: null,
    IsActive_bl: null
};
var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Assembly Status", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else {
            if (GetActive) { $("#EditFormGroupIsActive").addClass("hide"); }
            else { $("#EditFormGroupIsActive").removeClass("hide"); }

            showModalWait();

            fillFormForEditGeneric(CurrIds, "POST", "/AssemblyStatusSrv/GetByIds", GetActive, "EditForm", "Edit Assembly Status", MagicSuggests)
                .always(hideModalWait)
                .done(function (currRecords) {
                    CurrRecords = currRecords;
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });
    
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) {
            GetActive = true;
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        } else {
            GetActive = false;
            $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        }
        refreshMainView();
    });

    //TableMain Assembly Statuss
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyStatusName", name: "AssyStatusName" },//1
            { data: "AssyStatusAltName", name: "AssyStatusAltName" },//2
            { data: "Comments", name: "Comments" },//3
            { data: "IsActive_bl", name: "IsActive_bl" },//4
        ],
        columnDefs: [
            { targets: [0, 4], visible: false }, // - never show
            { targets: [0, 4], searchable: false },  //"orderable": false, "visible": false
            { targets: [3], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" } // - first set of columns
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
        
    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {
            showModalWait();
            submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/AssemblyStatusSrv/Edit")
                .always(hideModalWait)
                .done(function () {
                    refreshMainView();
                    $("#MainView").removeClass("hide");
                    $("#EditFormView").addClass("hide");
                    window.scrollTo(0, 0);
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
        }
    });

    //--------------------------------------View Initialization------------------------------------//

    refreshMainView();
    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    deleteRecordsGeneric(CurrIds, "/AssemblyStatusSrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {
    refreshTblGenWrp(TableMain, "/AssemblyStatusSrv/Get", { getActive: GetActive });
}



//---------------------------------------Helper Methods--------------------------------------//

