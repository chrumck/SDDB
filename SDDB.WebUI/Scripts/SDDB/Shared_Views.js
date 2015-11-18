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
var SelectedColumnSet = 1;

var InitialViewId = "InitialView";

var MainViewId = "MainView";
var MainViewBtnGroupClass = "tdo-btngroup-main";

var EditFormId = "EditForm";
var EditFormViewId = "EditFormView";
var EditFormBtnGroupCreateClass = "tdo-btngroup-edit";
var EditFormBtnGroupEditClass = "tdo-btngroup-edit";
var LabelTextCreate = "Create Record";
var LabelTextEdit = "Edit Record";
var LabelTextCopy = "Copy Record";

var HttpTypeFillForEdit = "POST";
var UrlFillForEdit = "";
var HttpTypeEdit = "POST";
var UrlEdit = "";
var UrlDelete = "";

var callBackBeforeCreate = function () { return $.Deferred().resolve(); };
var callBackBeforeEdit = function (currRecords) { return $.Deferred().resolve(); };
var callBackBeforeSubmitEdit = function (data) { return $.Deferred().resolve(); };
var callBackAfterSubmitEdit = function (data) { return $.Deferred().resolve(); };
var callBackBeforeCopy = function () { return $.Deferred().resolve(); };

//-------------------------------------------------------------------------------------------//

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric(EditFormId, MagicSuggests, LabelTextCreate, MainViewId);
        callBackBeforeCreate()
            .done(function () {
                saveViewSettings(TableMain);
                switchView(MainViewId, EditFormViewId, EditFormBtnGroupCreateClass);
            });
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        fillFormForEditGenericWrp(CurrIds, HttpTypeFillForEdit, UrlFillForEdit,
                GetActive, EditFormId, LabelTextEdit, MagicSuggests)
            .then(function (currRecords) {
                CurrRecords = currRecords;
                return callBackBeforeEdit(currRecords);
            })
            .done(function () {
                saveViewSettings(TableMain);
                switchView(MainViewId, EditFormViewId, EditFormBtnGroupEditClass);
            });
    });

    //Wire up BtnCopy
    $("#BtnCopy").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        modalWaitWrapper(function () {
            return fillFormForCopyGeneric(CurrIds, HttpTypeFillForEdit, UrlFillForEdit,
                GetActive, EditFormId, LabelTextCopy, MagicSuggests);
        })
            .then(function (currRecords) {
                CurrIds = [];
                CurrRecords = [];
                CurrRecords[0] = $.extend(true, {}, RecordTemplate);
                return callBackBeforeCopy();
            })
            .done(function () {
                saveViewSettings(TableMain);
                switchView(MainViewId, EditFormViewId, EditFormBtnGroupEditClass);
            });
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length === 0) {
            showModalNothingSelected();
            return;
        }
        showModalDelete(CurrIds.length);
    });

    //wire up columnsSelectId1
    $("#columnsSelectId1").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 1);
    });

    //wire up columnsSelectId2
    $("#columnsSelectId2").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 2);
    });

    //wire up columnsSelectId3
    $("#columnsSelectId3").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 3);
    });

    //wire up columnsSelectId4
    $("#columnsSelectId4").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 4);
    });

    //wire up columnsSelectId5
    $("#columnsSelectId5").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 5);
    });

    //wire up columnsSelectId6
    $("#columnsSelectId6").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 6);
    });

    //wire up columnsSelectId7
    $("#columnsSelectId7").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 7);
    });

    //wire up columnsSelectId8
    $("#columnsSelectId8").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        showColumnSet(TableMainColumnSets, 8);
    });

    //---------------------------------------DataTables------------

    //wire up BtnTableMainExport
    $("#BtnTableMainExport").click(function (event) {
        exportTableToTxt(TableMain);
    });

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

//---------------------------------------EditFormView----------------------------------------//

//Wire Up EditFormBtnCancel
$("#" + EditFormId + "BtnCancel").click(function () {
    switchView(EditFormViewId, MainViewId, MainViewBtnGroupClass, TableMain);
});

//Wire Up EditFormBtnOk
$("#EditFormBtnOk").click(function () {
    msValidate(MagicSuggests);
    if (!formIsValid(EditFormId, CurrIds.length === 0) || !msIsValid(MagicSuggests)) {
        showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
        return;
    }
    callBackBeforeSubmitEdit()
        .then(function () {
            var createMultiple = $("#CreateMultiple").val() !== "" ? $("#CreateMultiple").val() : 1;
            return submitEditsGenericWrp(EditFormId, MagicSuggests, CurrRecords, HttpTypeEdit, UrlEdit, createMultiple);
        })
        .then(function (data, currRecords) {
            CurrRecords = currRecords;
            if (CurrIds.length === 0) {
                CurrIds = data.newEntryIds;
                for (var i = 0; i < CurrIds.length; i++) {
                    CurrRecords[i].Id = CurrIds[i];
                }
            }
            return callBackAfterSubmitEdit(data);
        })
        .then(function () {
            return refreshMainView();
        })
        .done(function () {
            switchView(EditFormViewId, MainViewId, MainViewBtnGroupClass, TableMain);
        });
});


//--------------------------------------- Main Methods---------------------------------------//

//Delete Records from DB
function deleteRecords() {
    deleteRecordsGenericWrp(CurrIds, UrlDelete, refreshMainView)
        .done(function () {
            CurrIds = [];
            CurrRecords = [];
        });
}

//---------------------------------------Helper Methods--------------------------------------//

//showColumnSet
function showColumnSet(columnSetArray, columnSetIdx) {
    TableMain.columns().visible(false);
    TableMain.columns(columnSetArray[0]).visible(true);
    TableMain.columns(columnSetArray[columnSetIdx]).visible(true);
    SelectedColumnSet = columnSetIdx;
}




