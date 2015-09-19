/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../jquery.validate.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain;
var TableMainColumnSets;
var MagicSuggests = [];
var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

var MainViewId = "MainView";

var FormId = "EditForm";
var FormViewId = "EditFormView"
var FormBtnGroupCreateClass = "tdo-btngroup-edit";
var FormBtnGroupEditClass = "tdo-btngroup-edit";
var LabelTextCreate = "Create Record";
var LabelTextEdit = "Edit Record";
var HttpTypeEdit = "POST";
var UrlEdit = "";
var CallBackBeforeCreate;
var CallBackBeforeEdit;

//-------------------------------------------------------------------------------------------//

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric(FormId, MagicSuggests, LabelTextCreate, MainViewId);
        if (CallBackBeforeCreate) { CallBackBeforeCreate(); }
        saveViewSettings(TableMain);
        switchView(MainViewId, FormViewId, FormBtnGroupCreateClass);
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
        fillFormForEditGeneric(CurrIds, HttpTypeEdit, UrlEdit, GetActive, FormId, LabelTextEdit, MagicSuggests)
            .always(hideModalWait)
            .done(function (currRecords) {
                CurrRecords = currRecords;
                if (CallBackBeforeEdit) { CallBackBeforeEdit(); }
                saveViewSettings(TableMain);
                switchView(MainViewId, FormViewId, FormBtnGroupEditClass);
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
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


});

//------------------------------------Common Main Methods----------------------------------//


//---------------------------------------Helper Methods--------------------------------------//

//showColumnSet
function showColumnSet(columnSetArray, columnSetIdx) {
    TableMain.columns().visible(false);
    TableMain.columns(columnSetArray[0]).visible(true);
    TableMain.columns(columnSetArray[columnSetIdx]).visible(true);
}




