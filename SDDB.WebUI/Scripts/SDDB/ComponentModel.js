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
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    CompModelName: null,
    CompModelAltName: null,
    Comments: null,
    IsActive_bl: null,
    Attr01Type: null,
    Attr01Desc: null,
    Attr02Type: null,
    Attr02Desc: null,
    Attr03Type: null,
    Attr03Desc: null,
    Attr04Type: null,
    Attr04Desc: null,
    Attr05Type: null,
    Attr05Desc: null,
    Attr06Type: null,
    Attr06Desc: null,
    Attr07Type: null,
    Attr07Desc: null,
    Attr08Type: null,
    Attr08Desc: null,
    Attr09Type: null,
    Attr09Desc: null,
    Attr10Type: null,
    Attr10Desc: null,
    Attr11Type: null,
    Attr11Desc: null,
    Attr12Type: null,
    Attr12Desc: null,
    Attr13Type: null,
    Attr13Desc: null,
    Attr14Type: null,
    Attr14Desc: null,
    Attr15Type: null,
    Attr15Desc: null
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
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Component Model", "MainView");
        $("#EditForm select").find("option:first").prop('selected', 'selected');
        saveViewSettings(TableMain);
        switchView("MainView", "EditFormView", "tdo-btngroup-edit");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
    	fillFormForEditGeneric(CurrIds, "POST", "/ComponentModelSrv/GetByIds", 
		GetActive, "EditForm", "Edit Component Model", MagicSuggests)
            .always(hideModalWait)
            .done(function (currRecords) {
                CurrRecords = currRecords;
                saveViewSettings(TableMain);
                switchView("MainView", "EditFormView", "tdo-btngroup-edit");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(true);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(true);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(true);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(true);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId5
    $("#dropdownId5").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(true);
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

    //TableMain Component Models
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompModelName", name: "CompModelName" },//1
            { data: "CompModelAltName", name: "CompModelAltName" },//2
            //------------------------------------------------first set of columns
            { data: "Attr01Type", name: "Attr01Type" },//3
            { data: "Attr01Desc", name: "Attr01Desc" },//4
            { data: "Attr02Type", name: "Attr02Type" },//5
            { data: "Attr02Desc", name: "Attr02Desc" },//6
            { data: "Attr03Type", name: "Attr03Type" },//7
            { data: "Attr03Desc", name: "Attr03Desc" },//8
            //------------------------------------------------second set of columns
            { data: "Attr04Type", name: "Attr04Type" },//9
            { data: "Attr04Desc", name: "Attr04Desc" },//10
            { data: "Attr05Type", name: "Attr05Type" },//11
            { data: "Attr05Desc", name: "Attr05Desc" },//12
            { data: "Attr06Type", name: "Attr06Type" },//13
            { data: "Attr06Desc", name: "Attr06Desc" },//14
            //------------------------------------------------third set of columns
            { data: "Attr07Type", name: "Attr07Type" },//15
            { data: "Attr07Desc", name: "Attr07Desc" },//16
            { data: "Attr08Type", name: "Attr08Type" },//17
            { data: "Attr08Desc", name: "Attr08Desc" },//18
            { data: "Attr09Type", name: "Attr09Type" },//19
            { data: "Attr09Desc", name: "Attr09Desc" },//20
            //------------------------------------------------fourth set of columns
            { data: "Attr10Type", name: "Attr10Type" },//21
            { data: "Attr10Desc", name: "Attr10Desc" },//22
            { data: "Attr11Type", name: "Attr11Type" },//23
            { data: "Attr11Desc", name: "Attr11Desc" },//24
            { data: "Attr12Type", name: "Attr12Type" },//25
            { data: "Attr12Desc", name: "Attr12Desc" },//26
            //------------------------------------------------fifth set of columns
            { data: "Attr13Type", name: "Attr13Type" },//27
            { data: "Attr13Desc", name: "Attr13Desc" },//28
            { data: "Attr14Type", name: "Attr14Type" },//29
            { data: "Attr14Desc", name: "Attr14Desc" },//30
            { data: "Attr15Type", name: "Attr15Type" },//31
            { data: "Attr15Desc", name: "Attr15Desc" },//32
            //------------------------------------------------never visible
            { data: "Comments", name: "Comments" },//33
            { data: "IsActive_bl", name: "IsActive_bl" },//34
        ],
        columnDefs: [
            { targets: [0, 34], visible: false }, // - never show
            { targets: [0, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 34], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 4, 5, 6], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7, 8], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [9, 10, 11, 12, 13, 14], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 10, 11, 12], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [13, 14], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [15, 16, 17, 18, 19, 20], visible: false }, // - third set of columns - to toggle with options
            { targets: [15, 16, 17, 18], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [19, 20], className: "hidden-xs hidden-sm hidden-md" }, // - third set of columns

            { targets: [21, 22, 23, 24, 25, 26], visible: false }, // - fourth set of columns - to toggle with options
            { targets: [21, 22, 23, 24], className: "hidden-xs hidden-sm" }, // - fourth set of columns
            { targets: [25, 26], className: "hidden-xs hidden-sm hidden-md" }, // - fourth set of columns

            { targets: [27, 28, 29, 30, 31, 32], visible: false }, // - fifth set of columns - to toggle with options
            { targets: [27, 28, 29, 30], className: "hidden-xs hidden-sm" }, // - fifth set of columns
            { targets: [31, 32], className: "hidden-xs hidden-sm hidden-md" } // - fifth set of columns
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
    $("#EditFormBtnCancel").click(function () {
        switchView("EditFormView", "MainView", "tdo-btngroup-main", true);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
            showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
            return;
        }
        showModalWait();
        submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/ComponentModelSrv/Edit")
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("EditFormView", "MainView", "tdo-btngroup-main", true, TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });

    });

    //--------------------------------------View Initialization------------------------------------//

    refreshMainView();
    $("#InitialView").addClass("hidden");
    $("#MainView").removeClass("hidden");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/ComponentModelSrv/Delete", refreshMainView);
}

//refresh Main view 
function refreshMainView() {
    var deferred0 = $.Deferred();
    refreshTblGenWrp(TableMain, "/ComponentModelSrv/Get", { getActive: GetActive }).done(deferred0.resolve);
    return deferred0.promise();
}



//---------------------------------------Helper Methods--------------------------------------//

