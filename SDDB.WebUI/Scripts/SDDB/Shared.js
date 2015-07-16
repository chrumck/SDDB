/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
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

    //Enable modified field detection
    $(".modifiable").change(function () { $(this).data("ismodified", true); });


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
function showModalNothingSelected(bodyText) {
    $("#ModalInfoLabel").text("Nothing Selected");
    if (typeof bodyText !== "undefined") $("#ModalInfoBody").text(bodyText);
    else $("#ModalInfoBody").text("Please select one or more rows.");
    $("#ModalInfoBodyPre").empty().hide();
    $("#ModalInfo").modal("show");
}

//Show Modal Selected other than one row
function showModalSelectOne(bodyText) {
    $("#ModalInfoLabel").text("Select One Row");
    if (typeof bodyText !== "undefined") $("#ModalInfoBody").text(bodyText);
    else $("#ModalInfoBody").text("Please select one row.");
    $("#ModalInfoBodyPre").empty().hide();
    $("#ModalInfo").modal("show");
}

//Show Modal Delete
function showModalDelete(noOfRows) {
    $("#ModalDeleteBody").text("Confirm deleting " + noOfRows + " row(s).");
    $("#ModalDelete").modal("show");
}

//Wire Up ModalDeleteBtnOk
$("#ModalDeleteBtnOk").click(function () {
    $("#ModalDelete").modal("hide");
    DeleteRecords();
});

//showModalWait
function showModalWait() {
    $("#ModalWait").modal({
        show: true,
        backdrop: "static",
        keyboard: false
    });
}

//hideModalWait
function hideModalWait() {
    $("#ModalWait").modal("hide");
}

//showModalFail
function showModalFail(label, body, bodyPre) {
    label = (typeof label !== "undefined") ? label : "Undefined Error";
    body = (typeof body !== "undefined") ? body : "";
    bodyPre = (typeof bodyPre !== "undefined") ? bodyPre : "";
  
    $("#ModalInfoLabel").text(label);
    $("#ModalInfoBody").html(body);
    if (bodyPre != "") $("#ModalInfoBodyPre").text(bodyPre).show();
    else $("#ModalInfoBodyPre").hide();
    $("#ModalInfo").modal("show");
}

//showModalAJAXFail
function showModalAJAXFail(xhr, status, error) {
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

//-----------------------------------------------------------------------------

//Refresh  table from AJAX
function refreshTable(table, url, getActive, httpType, projectIds, modelIds, typeIds,
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

    table.clear().search("").draw();
    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

    $.ajax({
        type: httpType, url: url, timeout: 20000,
        data: {getActive: getActive, projectIds: projectIds, modelIds: modelIds, typeIds: typeIds,
            locIds: locIds, assyIds: assyIds, personIds: personIds, startDate: startDate, endDate: endDate
        },
        dataType: "json",
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            table.rows.add(data).order([1, "asc"]).draw();
        })
        .fail(function (xhr, status, error) {
            showModalAJAXFail(xhr, status, error);
        });
}

//Refresh  table from AJAX - generic version
function refreshTableGeneric(table, url, data, httpType) {

    var deferred0 = $.Deferred(); 

    httpType = (typeof httpType !== "undefined") ? httpType : "GET";
    data = (typeof data !== "undefined") ? data : {};

    table.clear().search("").draw();

    $.ajax({ type: httpType, url: url, timeout: 20000, data: data, dataType: "json", })
        .done(function (data) {
            table.rows.add(data).order([1, "asc"]).draw();
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

    return deferred0.promise();
}

//Refresh  table from AJAX - generic version - showing wait dialogs
function refreshTblGenWrp(table, url, data, httpType) {
    var deferred0 = $.Deferred();

    showModalWait();

    refreshTableGeneric(table, url, data, httpType)
        .always(hideModalWait)
        .done(deferred0.resolve)
        .fail(function (xhr, status, error) {
            showModalAJAXFail(xhr, status, error);
            deferred0.reject(xhr, status, error);
        });

    return deferred0.promise();
}

//-----------------------------------------------------------------------------

//checking if form is valid
function formIsValid(id, isCreate) {
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
function clearFormInputs(formId, msArray) {
    $("#" + formId + " :input").prop("checked", false).prop("selected", false)
        .not(":button, :submit, :reset, :radio, :checkbox").val("");
    $("#" + formId + " [data-valmsg-for]").empty();
    $("#" + formId + " .input-validation-error").removeClass("input-validation-error");
    $("#" + formId + " .modifiable").data("ismodified", false);

    if (typeof msArray !== "undefined") {
        $.each(msArray, function (i, ms) { ms.clear(true); ms.isModified = false; });
    }
}

//Prepare Form For Create
function fillFormForCreateGeneric(formId, msArray, labelText, mainViewId) {

    clearFormInputs(formId, msArray);
    $("#" + formId + "Label").text(labelText);
    $("#" + formId + " [data-val-dbisunique]").prop("disabled", false); disableUniqueMs(msArray, false);
    $("#" + formId + " .modifiable").data("ismodified", true); setMsAsModified(msArray, true);
    $("#" + formId + "GroupIsActive").addClass("hide"); $("#IsActive").prop("checked", true); $("#IsActive_bl").prop("checked", true)
    $("#" + formId + "CreateMultiple").removeClass("hide");
    $("#" + mainViewId).addClass("hide");
    $("#" + formId + "View").removeClass("hide");
}

//FillFormForEdit Generic version
function fillFormForEditGeneric(ids, httpType, url, getActive, formId, labelText, msArray) {

    var deferred0 = $.Deferred();

    clearFormInputs(formId, msArray);
    $("#" + formId + "Label").text(labelText);

    $.ajax({ type: httpType, url: url, timeout: 20000, data: { ids: ids, getActive: getActive }, dataType: "json" })
        .done(function (data) {

            var currRecord = {};
            for (var property in data[0]) {
                if (data[0].hasOwnProperty(property) && property != "Id" && property.slice(-1) != "_") {
                    currRecord[property] = data[0][property];
                }
            }

            var formInput = $.extend(true, {}, currRecord);

            $.each(data, function (i, dbEntry) {
                for (var property in dbEntry) {
                    if (dbEntry.hasOwnProperty(property) && property != "Id" && property.slice(-1) != "_") {
                        if (property.slice(-3) == "_Id") {
                            if (formInput[property] != dbEntry[property]) {
                                formInput[property] = "_VARIES_"; formInput[property.slice(0, -2)] = "_VARIES_";
                            }
                            else {
                                formInput[property.slice(0, -2)] = "";
                                for (var subProp in dbEntry[property.slice(0, -2)]) {
                                    if (dbEntry[property.slice(0, -2)][subProp] != null) {
                                        if (formInput[property.slice(0, -2)] != "") formInput[property.slice(0, -2)] += " ";
                                        formInput[property.slice(0, -2)] += dbEntry[property.slice(0, -2)][subProp];
                                    }
                                }
                            }
                        }
                        else {
                            if (formInput[property] != dbEntry[property]) formInput[property] = "_VARIES_";
                        }
                    }
                }
            });

            for (var property in formInput) {
                if (formInput.hasOwnProperty(property) && property != "Id" && property.slice(-1) != "_") {
                    if (property.slice(-3) == "_Id") {
                        $.each(msArray, function (i, ms) {
                            if (ms.id == property) {
                                if (formInput[property] != null) ms.addToSelection([{ id: formInput[property], name: formInput[property.slice(0, -2)] }], true);
                                return false;
                            }
                        });
                    }
                    else if (property.slice(-3) == "_bl") {
                        if (formInput[property] == true) $("#" + property).prop("checked", true);
                    }
                    else {
                        $("#" + property).val(formInput[property]);
                    }
                }
            }

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                disableUniqueMs(msArray, false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
                disableUniqueMs(msArray, true);
            }

            deferred0.resolve(currRecord);
        })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

    return deferred0.promise();
}

//SubmitEdits to DB - generic version
function submitEditsGeneric(ids, formId, msArray, currRecord, httpType, url) {

    var deferred0 = $.Deferred();

    var modifiedProperties = [];
    $("#" + formId + " .modifiable").each(function (index) {
        if ($(this).data("ismodified")) modifiedProperties.push($(this).prop("id"));
    });

    $.each(msArray, function (i, ms) {
        if (ms.isModified == true) modifiedProperties.push(ms.id);
    });

    var editRecords = [];

    if (ids.length == 0) ids = ["newEntryId"];
    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;

        for (var property in currRecord) {
            if (currRecord.hasOwnProperty(property) && property != "Id" && property.slice(-1) != "_") {
                if (property.slice(-3) == "_Id") {
                    $.each(msArray, function (i, ms) {
                        if (ms.id == property) {
                            editRecord[property] = (ms.isModified && ms.getSelection().length != 0) ?
                                (ms.getSelection())[0].id : currRecord[property];
                            return false;
                        }
                    });
                }
                else if (property.slice(-3) == "_bl") {
                    editRecord[property] = ($.inArray(property, modifiedProperties) != -1) ?
                        $("#" + property).prop("checked") : currRecord[property];
                }
                else {
                    editRecord[property] = ($.inArray(property, modifiedProperties) != -1) ?
                        $("#" + property).val() : currRecord[property];
                }
            }
        }

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({ type: httpType, url: url, timeout: 20000, data: { records: editRecords }, dataType: "json" })
        .done(function (data) { deferred0.resolve(data); })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

    return deferred0.promise();
}

//-----------------------------------------------------------------------------

//Fill Form for Edit from n:n related table - generic version
function fillFormForRelatedGeneric(tableAdd, tableRemove, ids,
    httpType, url, data, httpTypeNot, urlNot, dataNot, httpTypeMany, urlMany, dataMany) {

    var deferred0 = $.Deferred();

    tableAdd.clear().search("").draw(); tableRemove.clear().search("").draw();

    if (ids.length == 1) {
        $.when(
            $.ajax({type: httpTypeNot, url: urlNot, timeout: 20000, data: dataNot, dataType: "json" }),
            $.ajax({type: httpType, url: url, timeout: 20000, data: data, dataType: "json" })
        )
        .done(function (done1, done2) {
            tableAdd.rows.add(done1[0]).order([1, "asc"]).draw();
            tableRemove.rows.add(done2[0]).order([1, "asc"]).draw();
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
    }
    else {
        $.ajax({ type: httpTypeMany, url: urlMany, timeout: 20000, data: dataMany, dataType: "json" })
            .done(function (data) {
                tableAdd.rows.add(data).order([1, "asc"]).draw();
                if (ids.length != 0) tableRemove.rows.add(data).order([1, "asc"]).draw();
                deferred0.resolve();
            })
            .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
    }

    return deferred0.promise();
}

//Submit Edits for n:n related table - generic version
function submitEditsForRelatedGeneric(ids, idsAdd, idsRemove, url) {

    var deferred0 = $.Deferred();
    var deferred1 = $.Deferred(); var deferred2 = $.Deferred();

    if (idsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({type: "POST", url: url, timeout: 20000, data: { ids: ids, idsAddRem: idsAdd, isAdd: true }, dataType: "json" })
            .done(function () { deferred1.resolve(); })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    if (idsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({ type: "POST", url: url, timeout: 20000, data: { ids: ids, idsAddRem: idsRemove, isAdd: false }, dataType: "json" })
                .done(function () { deferred2.resolve(); })
                .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
        }, 500);
    }

    $.when(deferred1, deferred2)
        .done(function () { deferred0.resolve(); })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

    return deferred0.promise();
}

//-----------------------------------------------------------------------------

//initialize MagicSuggest and add to MagicSuggest array
function addToMSArray(msArray, id, url, maxSelection, minChars, dataUrlParams, disabled, editable) {

    disabled = (typeof disabled !== "undefined" && disabled == false) ? false : true;
    editable = (typeof editable !== "undefined" && editable == false) ? false : true;

    var element = $("#" + id);
    var dataValRequired = $(element).attr("data-val-required");
    var dataValDbisunique = $(element).attr("data-val-dbisunique");
        
    var ms = element.magicSuggest({
        invalidCls: "input-validation-error",
        data: url,
        dataUrlParams: dataUrlParams,
        allowFreeEntries: false,
        disabled: disabled,
        editable: editable,
        minChars: minChars,
        maxSelection: maxSelection,
        required: (typeof dataValRequired !== "undefined") ? true : false,
        resultAsString: true,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        }
    });

    ms.id = id;
    ms.dataValRequired = dataValRequired;
    ms.dataValDbisunique = dataValDbisunique;
    ms.isModified = false;

    $(ms).on("blur", function (e, m) {
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

    $(ms).on("selectionchange", function (e, m) { this.isModified = true });

    if (!editable) { $(ms).on("focus", function (e, m) { this.expand(); }); }

    msArray.push(ms);
}

//check if MagicSuggests in MagicSugest Array are valid
function msIsValid(msArray) {
    var msCheck = true;
    $.each(msArray, function (i, ms) {
        if (!ms.isValid()) msCheck = false;
    });
    return msCheck;
}

//change formatting of invalid MagicSugest's
function msValidate(msArray) {
    $.each(msArray, function (i, ms) {
        if (!ms.isValid()) {
            $("#" + ms.id).addClass("input-validation-error");
            $("[data-valmsg-for=" + ms.id + "]").text("Input missing or invalid.").removeClass("field-validation-valid")
                .addClass("field-validation-error");
        }
    });
}

//enable or disable DbUnique MagicSuggests
function disableUniqueMs(msArray, disable) {
    disable = (typeof disable !== "undefined" && disable == false) ? false : true;
    $.each(msArray, function (i, ms) {
        if (disable == true && typeof ms.dataValDbisunique !== "undefined" && ms.dataValDbisunique == "true") ms.disable();
        else ms.enable();
    });
}

//set MagicSuggests as modified ot not
function setMsAsModified(msArray, isModified) {
    isModified = (typeof isModified !== "undefined" && isModified == false) ? false : true;
    $.each(msArray, function (i, ms) { ms.isModified = isModified; });
}

