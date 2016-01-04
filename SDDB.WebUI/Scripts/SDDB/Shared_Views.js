/// <reference path="Shared.js" />

"use strict";

//--------------------------------------Global Variables-------------------------------------//

//set up global sddb object
var sddb = sddbConstructor();

//----------------------------------Custom Unobtrusive Validation----------------------------//

//client validation for dbisinteger
$.validator.unobtrusive.adapters.addBool("dbisinteger");
$.validator.addMethod("dbisinteger", function (value, element, valParams) {
    if (value) { return value == parseInt(value, 10); }
    return true;
});

//client validation for dbisbool
$.validator.unobtrusive.adapters.addBool("dbisbool");
$.validator.addMethod("dbisbool", function (value, element, valParams) {
    if (value) { return (value.toLowerCase() == "true" || value.toLowerCase() == "false"); }
    return true;
});

//client validation for dbdateiso
$.validator.unobtrusive.adapters.addBool("dbisdateiso");
$.validator.addMethod("dbisdateiso", function (value, element, valParams) {
    if (value) { return moment(value, "YYYY-MM-DD").isValid(); }
    return true;
});

//client validation for dbdatetimeiso
$.validator.unobtrusive.adapters.addBool("dbisdatetimeiso");
$.validator.addMethod("dbisdatetimeiso", function (value, element, valParams) {
    if (value) { return moment(value, "YYYY-MM-DD HH:mm").isValid(); }
    return true;
});

//---------------------------------------Global Settings-------------------------------------//

//disable AJAX caching
$.ajaxSetup({ cache: false });


//----------------------------------------------setup after page load------------------------------------------------//
$(document).ready(function () {

    //---------------------------------------Global Settings-------------------------------------//

    //Enable jqueryUI selectable
    if (!Modernizr.touch) {
        $(".selectable").selectable({ filter: "tr" });
    }
    else {
        $(".selectable").on("click", "tr", function () { $(this).toggleClass("ui-selected"); });
    }

    //Enable modified field detection
    $(".modifiable").change(function () { $(this).data("ismodified", true); });

    //collapse navbar if any menu item with collapsenav class clicked
    $(".collapsenav").on("click", function () {
        $(".navbar-collapse").collapse("hide");
    });

    //---------------------------------------Modal Dialogs---------------------------------------//

    //Get focus on modalInfoBtnOk
    $("#modalInfo").on("shown.bs.modal", function () { $("#modalInfoBtnOk").focus(); });

    //Wire Up modalInfoBtnOk
    $("#modalInfoBtnOk").click(function () { $("#modalInfo").modal("hide"); });

    //Get focus on modalConfirmBtnYes
    $("#modalConfirm").on("shown.bs.modal", function () { $("#modalConfirmBtnYes").focus(); });

    //Enable DatePicker on modalDatePromptInput
    $("#modalDatePromptInput").datetimepicker({ format: "YYYY-MM-DD", inline: true });

    //Get focus on modalDatePromptBtnOk
    $("#modalDatePrompt").on("shown.bs.modal", function () { $("#modalDatePromptBtnOk").focus(); });

    //-----------------------------------------mainView------------------------------------------//

    //Wire up btnCreate
    $("#btnCreate").click(function (event) { sddb.prepareFormForCreate(); });

    //Wire up btnEdit
    $("#btnEdit").click(function (event) { sddb.prepareFormForEdit(); });

    //Wire up btnCopy
    $("#btnCopy").click(function (event) { sddb.prepareFormForCopy(); });

    //Wire up btnDelete 
    $("#btnDelete").click(function (event) { sddb.confirmAndDelete(); });

    //TODO: refactor columnSelectId so it's not repeated n times
    //wire up columnsSelectId1
    $("#columnsSelectId1").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(1);
    });

    //wire up columnsSelectId2
    $("#columnsSelectId2").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(2);
    });

    //wire up columnsSelectId3
    $("#columnsSelectId3").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(3);
    });

    //wire up columnsSelectId4
    $("#columnsSelectId4").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(4);
    });

    //wire up columnsSelectId5
    $("#columnsSelectId5").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(5);
    });

    //wire up columnsSelectId6
    $("#columnsSelectId6").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(6);
    });

    //wire up columnsSelectId7
    $("#columnsSelectId7").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(7);
    });

    //wire up columnsSelectId8
    $("#columnsSelectId8").click(function (event) {
        event.preventDefault();
        if ($(this).parent().hasClass("disabled")) { return; }
        sddb.showColumnSet(8);
    });

    //---------------------------------------DataTables------------

    //wire up btnTableMainExport
    $("#btnTableMainExport").click(function (event) { sddb.exportTableToTxt(); });

    //Wire up chBoxShowDeleted
    $("#chBoxShowDeleted").change(function (event) { sddb.switchTableToActive(!$(this).prop("checked")); });

    //initializing table view
    sddb.showColumnSet();

    //---------------------------------------editFormView----------------------------------------//

    //Wire Up editFormBtnCancel
    $("#editFormBtnCancel").click(function (event) { sddb.cancelEditForm(); });

    //Wire Up editFormBtnOk
    $("#editFormBtnOk").click(function (event) { sddb.submitEditForm(); });

    //Enable DateTimePicker
    $("[data-val-dbisdatetimeiso]").datetimepicker({ format: "YYYY-MM-DD HH:mm" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });
    
    //Enable DateTimePicker
    $("[data-val-dbisdateiso]").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //--------------------------------End of setup after page load---------------------------------//
});







