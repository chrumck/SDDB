/// <reference path="../jquery-2.1.3.js" />
/// <reference path="../jquery-2.1.3.intellisense.js" />
/// <reference path="../jquery.validate.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />

//---------------------------------------Global Settings-------------------------------------//

$(document).ready(function () {

    //disable AJAX caching
    $.ajaxSetup({ cache: false });
    
    //Enable jqueryUI selectable
    if (!Modernizr.touch) {
        $(".selectable").selectable({ filter: "tr" });
    }
    else {
        $(".selectable").on("click", "tr", function () { $(this).toggleClass("ui-selected"); });
    }

});

//----------------------------------Custom Unobtrusive Validation----------------------------//

//client validation for dbisinteger
$.validator.unobtrusive.adapters.addBool("dbisinteger");
$.validator.addMethod("dbisinteger", function (value, element, valParams) {
    if (value) return value == parseInt(value, 10);
    return true;
});

//client validation for dbisbool
$.validator.unobtrusive.adapters.addBool("dbisbool");
$.validator.addMethod("dbisbool", function (value, element, valParams) {
    if (value) return (value.toLowerCase() == "true" || value.toLowerCase() == "false");
    return true;
});

//client validation for dbdateiso
$.validator.unobtrusive.adapters.addBool("dbisdateiso");
$.validator.addMethod("dbisdateiso", function (value, element, valParams) {
    if (value) return moment(value, "YYYY-MM-DD").isValid();
    return true;
});

//client validation for dbdatetimeiso
$.validator.unobtrusive.adapters.addBool("dbisdatetimeiso");
$.validator.addMethod("dbisdatetimeiso", function (value, element, valParams) {
    if (value) return moment(value, "YYYY-MM-DD HH:mm").isValid();
    return true;
});


//---------------------------------------Modal Dialogs---------------------------------------//

$(document).ready(function () {

    //Get focus on ModalInfoBtnOk
    $("#ModalInfo").on("shown.bs.modal", function () { $("#ModalInfoBtnOk").focus(); });

    //Wire Up ModalInfoBtnOk
    $("#ModalInfoBtnOk").click(function () { $("#ModalInfo").modal("hide"); });

    //Get focus on ModalDeleteBtnCancel
    $("#ModalDelete").on("shown.bs.modal", function () { $("#ModalDeleteBtnCancel").focus(); });

    //Wire Up ModalDeleteBtnCancel 
    $("#ModalDeleteBtnCancel").click(function () { $("#ModalDelete").modal("hide"); });

});


//------------------------------------Common Main Methods----------------------------------//

//Show Modal Nothing Selected
function ShowModalNothingSelected() {
    $("#ModalInfoLabel").text("Nothing Selected");
    $("#ModalInfoBody").text("Please select one or more rows.");
    $("#ModalInfoBodyPre").empty().hide();
    $("#ModalInfo").modal("show");
}

//Show Modal Selected other than one row
function ShowModalSelectOne() {
    $("#ModalInfoLabel").text("Selected One Row");
    $("#ModalInfoBody").text("Please select only one row.");
    $("#ModalInfoBodyPre").empty().hide();
    $("#ModalInfo").modal("show");
}

//Show Modal Delete
function ShowModalDelete(noOfRows) {
    $("#ModalDeleteBody").text("Confirm deleting " + noOfRows + " row(s).");
    $("#ModalDelete").modal("show");
}

//Wire Up ModalDeleteBtnOk
$("#ModalDeleteBtnOk").click(function () {
    $("#ModalDelete").modal("hide");
    DeleteRecords();
});

//ShowModalFail
function ShowModalFail(label, body, bodyPre) {
    label = (typeof label !== "undefined") ? label : "Undefined Error";
    body = (typeof body !== "undefined") ? body : "";
    bodyPre = (typeof bodyPre !== "undefined") ? bodyPre : "";
  
    $("#ModalInfoLabel").text(label);
    $("#ModalInfoBody").html(body);
    if (bodyPre != "") $("#ModalInfoBodyPre").text(bodyPre).show();
    $("#ModalInfo").modal("show");
}

//ShowModalAJAXFail
function ShowModalAJAXFail(xhr, status, error) {
    if (typeof xhr.responseJSON !== "undefined") {
        var errMessage = xhr.responseJSON.responseText.substr(0, 512)
    }
    else if (typeof xhr.responseText !== "undefined") {
        var errMessage = xhr.responseText.substr(0, 512)
    }
    if (typeof errMessage == "undefined" || errMessage == "") { errMessage = "No error details available." }
    $("#ModalInfoLabel").text("Server Error");
    $("#ModalInfoBody").html("Error type: <strong>" + error + "</strong> , Status: <strong>" + status + "</strong>");
    $("#ModalInfoBodyPre").text(errMessage).show();
    $("#ModalInfo").modal("show");
}

//Refresh main table from AJAX
function RefreshTable(table, url, getActive, httpType, projectIds, modelIds, typeIds,
    locIds, assyIds, personIds, startDate, endDate) {

    getActive = (typeof getActive !== "undefined" && getActive == false) ? false : true;
    httpType = (typeof httpType !== "undefined") ? httpType : "GET";
    projectIds = (typeof projectIds !== "undefined") ? projectIds : [];
    modelIds = (typeof modelIds !== "undefined") ? modelIds : [];
    typeIds = (typeof typeIds !== "undefined") ? typeIds : [];
    locIds = (typeof locIds !== "undefined") ? locIds : [];
    assyIds = (typeof assyIds !== "undefined") ? assyIds : [];
    personIds = (typeof personIds !== "undefined") ? personIds : [];
    startDate = (typeof startDate !== "undefined") ? startDate : {};
    endDate = (typeof endDate !== "undefined") ? endDate : {};

    $.ajax({
        type: httpType, url: url, timeout: 20000,
        data: {getActive: getActive, projectIds: projectIds, modelIds: modelIds, typeIds: typeIds,
            locIds: locIds, assyIds: assyIds, personIds: personIds, startDate: startDate, endDate: endDate
        },
        dataType: "json",
        beforeSend: function () {
            table.clear().search("").draw();
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
        }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            table.rows.add(data.data).order([1, 'asc']).draw();
        })
        .fail(function (xhr, status, error) {
            ShowModalAJAXFail(xhr, status, error);
        });
}

//checking if form is valid
function FormIsValid(id, isCreate) {
    if ($("#" + id).valid()) return true;
    else if (isCreate) return false;
    else {
        var isValid = true;
        $("#" + id + " input").each(function (index) {
            if ($(this).data("ismodified") && $(this).hasClass("input-validation-error")) isValid = false;
        });
        return isValid;
    }
}

//Clear inputs from forms and reset .ismodified to false
function ClearFormInputs(formId, msArray) {
    $("#" + formId + " :input").prop("checked", false).prop("selected", false)
        .not(":button, :submit, :reset, :radio, :checkbox").val("");
    $("#" + formId + " [data-valmsg-for]").empty();
    $("#" + formId + " .input-validation-error").removeClass("input-validation-error");
    $("#" + formId + " .modifiable").data("ismodified", false);

    if (typeof msArray !== "undefined") {
        $.each(msArray, function (i, ms) { ms.clear(true); ms.isModified = false; });
    }
}

//initialize MagicSuggest and add to MagicSuggest array
function AddToMSArray(msArray, id, url, maxSelection, minChars, dataUrlParams, disabled) {

    disabled = (typeof disabled !== "undefined" && disabled == false) ? false : true;

    var element = $("#" + id);
    var dataValRequired = $(element).attr("data-val-required");
    var dataValDbisunique = $(element).attr("data-val-dbisunique");
        
    var ms = element.magicSuggest({
        invalidCls: "input-validation-error",
        data: url,
        dataUrlParams: dataUrlParams,
        allowFreeEntries: false,
        disabled: disabled,
        minChars: minChars,
        maxSelection: maxSelection,
        required: (typeof dataValRequired !== "undefined") ? true : false,
        resultAsString: true,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        }
    });

    ms.id = id;
    ms.dataValRequired = dataValRequired;
    ms.dataValDbisunique = dataValDbisunique;
    ms.isModified = false;

    $(ms).on('blur', function (e, m) {
        if (!this.isValid()) {
            $("#" + this.id).addClass("input-validation-error");
            $("[data-valmsg-for=" + this.id + "]").text("Input missing or invalid.").removeClass("field-validation-valid")
                .addClass("field-validation-error");
        }
        else {
            $("#" + this.id).removeClass("input-validation-error");
            $("[data-valmsg-for=" + this.id + "]").text("").addClass("field-validation-valid")
                .removeClass("field-validation-error");
        }
    });

    $(ms).on('selectionchange', function (e, m) { this.isModified = true });

    msArray.push(ms);
}

//check if MagicSuggests in MagicSugest Array are valid
function MsIsValid(msArray) {
    var msCheck = true;
    $.each(msArray, function (i, ms) {
        if (!ms.isValid()) msCheck = false;
    });
    return msCheck;
}

//change formatting of invalid MagicSugest's
function MsValidate(msArray) {
    $.each(msArray, function (i, ms) {
        if (!ms.isValid()) {
            $("#" + ms.id).addClass("input-validation-error");
            $("[data-valmsg-for=" + ms.id + "]").text("Input missing or invalid.").removeClass("field-validation-valid")
                .addClass("field-validation-error");
        }
    });
}

//enable or disable DbUnique MagicSuggests
function DisableUniqueMs(msArray, disable) {
    disable = (typeof disable !== "undefined" && disable == false) ? false : true;
    $.each(msArray, function (i, ms) {
        if (disable == true && typeof ms.dataValDbisunique !== "undefined" && ms.dataValDbisunique == "true") ms.disable();
        else ms.enable();
    });
}

//set MagicSuggests as modified ot not
function SetMsAsModified(msArray, isModified) {
    isModified = (typeof isModified !== "undefined" && isModified == false) ? false : true;
    $.each(msArray, function (i, ms) { ms.isModified = isModified; });
}

//Prepare Form For Create
function FillFormForCreate(formId, msArray, labelText) {

    setSelect = (typeof setSelect !== "undefined" && setSelect == true) ? true : false;
    
    ClearFormInputs(formId, msArray);
    $("#" + formId + "Label").text(labelText);
    $("[data-val-dbisunique]").prop("disabled", false); DisableUniqueMs(msArray, false);
    $(".modifiable").data("ismodified", true); SetMsAsModified(msArray, true);
    $("#EditFormGroupIsActive").addClass("hide"); $("#IsActive").prop("checked", true)
    $("#CreateMultipleRow").removeClass("hide");
    $("#MainView").addClass("hide");
    $("#" + formId + "View").removeClass("hide");
}
