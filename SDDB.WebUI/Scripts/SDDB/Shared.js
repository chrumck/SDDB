/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../jquery.validate.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />

//--------------------------------------Global Properties------------------------------------//

var windowYpos = 0;
var tablePage = 0;
var tableOrder;
var tableSearch;
var tableSelectedIds;

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

    //collapse navbar if any menu item with collapsenav class clicked
    $(".collapsenav").on("click", function () {
        $(".navbar-collapse").collapse("hide");
    });

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
    if (typeof bodyText !== "undefined") { $("#ModalInfoBody").text(bodyText); }
    else { $("#ModalInfoBody").text("Please select one or more rows."); }
    $("#ModalInfoBodyPre").empty().addClass("hidden");
    $("#ModalInfo").modal("show");
}

//Show Modal Selected other than one row
function showModalSelectOne(bodyText) {
    $("#ModalInfoLabel").text("Select One Row");
    if (typeof bodyText !== "undefined") { $("#ModalInfoBody").text(bodyText); }
    else { $("#ModalInfoBody").text("Please select one row."); }
    $("#ModalInfoBodyPre").empty().addClass("hidden");
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
    deleteRecords();
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
    if (bodyPre != "") { $("#ModalInfoBodyPre").text(bodyPre).removeClass("hidden"); }
    else { $("#ModalInfoBodyPre").addClass("hidden"); }
    $("#ModalInfo").modal("show");
}

//showModalAJAXFail
function showModalAJAXFail(xhr, status, error) {
    var errMessage = "No error details available.";
    if (typeof xhr.responseJSON !== "undefined") { errMessage = xhr.responseJSON.responseText.substr(0, 512); }
    else if (typeof xhr.responseText !== "undefined") { errMessage = xhr.responseText.substr(0, 512); }
    $("#ModalInfoLabel").text("Server Error");
    $("#ModalInfoBody").html("Error type: <strong>" + error + "</strong> , Status: <strong>" + status + "</strong>");
    $("#ModalInfoBodyPre").text(errMessage).removeClass("hidden");
    setTimeout(function () { $("#ModalInfo").modal("show"); }, 10);
}

//switchView 
function switchView(fromViewId, toViewId, toBtnGroupClass, scrollToWindowYpos, dataTable) {
    if (toBtnGroupClass) {
        $("[class*='tdo-btngroup']").addClass("hidden");
        $("[class~='" + toBtnGroupClass + "']").removeClass("hidden");
    }
    $("#" + fromViewId).addClass("hidden");
    $("#" + toViewId).removeClass("hidden");
    if (dataTable) {
        dataTable.order(tableOrder).search(tableSearch).page(tablePage).draw(false);
        var indexesToSelect = dataTable.rows(function (idx, data, node) {
            return $.inArray(data.Id, tableSelectedIds) != -1;
        }).eq(0).toArray();
        dataTable.rows(indexesToSelect).nodes().to$().addClass("ui-selected");
    }
    if (scrollToWindowYpos) { window.scrollTo(0, windowYpos); } else { window.scrollTo(0, 0); }
}

//saveViewSettings
function saveViewSettings(dataTable) {
    windowYpos = window.pageYOffset || document.documentElement.scrollTop;
    if (dataTable) {
        tablePage = dataTable.page();
        tableOrder = dataTable.order();
        tableSearch = dataTable.search();
        tableSelectedIds = dataTable.cells(".ui-selected", "Id:name").data().toArray();
    }
}

//-----------------------------------------------------------------------------

//Refresh  table from AJAX - generic version
function refreshTableGeneric(table, url, data, httpType) {

    var deferred0 = $.Deferred(); 

    httpType = (typeof httpType !== "undefined") ? httpType : "GET";
    data = (typeof data !== "undefined") ? data : {};

    table.clear().search("").draw();

    $.ajax({ type: httpType, url: url, timeout: 120000, data: data, dataType: "json", })
        .done(function (dbEntries) {
            table.rows.add(dbEntries).order([1, "asc"]).draw();
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

//Delete Records from DB - generic version
function deleteRecordsGeneric(ids, url, callback) {
    showModalWait();
    $.ajax({ type: "POST", url: url, timeout: 120000, data: { ids: ids }, dataType: "json" })
        .always(hideModalWait)
        .done(callback)
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//-----------------------------------------------------------------------------

//checking if form is valid
function formIsValid(id, isCreate) {
    if ($("#" + id).valid()) { return true; }
    if (isCreate) { return false; }
    var isValid = true;
    $("#" + id + " input").each(function (index) {
        if ($(this).data("ismodified") && $(this).hasClass("input-validation-error")) {
            isValid = false;
            return true;
        }
        $(this).removeClass("input-validation-error");
        $("[data-valmsg-for='" + this.id + "']").empty();
    });
    return isValid;
}

//Clear inputs from forms and reset .ismodified to false
function clearFormInputs(formId, msArray) {
    $("#" + formId + " :input").prop("checked", false).prop("selected", false)
        .not(":button, :submit, :reset, :radio, :checkbox").val("");
    $("#" + formId + " [data-valmsg-for]").empty();
    $("#" + formId + " .input-validation-error").removeClass("input-validation-error");
    $("#" + formId + " .modifiable").data("ismodified", false);

    if (msArray) { $.each(msArray, function (i, ms) { ms.clear(true); ms.isModified = false; });
    }
}

//Prepare Form For Create
function fillFormForCreateGeneric(formId, msArray, labelText) {
    clearFormInputs(formId, msArray);
    $("#" + formId + "Label").text(labelText);
    $("#" + formId + " [data-val-dbisunique]").prop("disabled", false);
    msDisableUnique(msArray, false);
    $("#" + formId + " .modifiable").data("ismodified", true);
    msSetAsModified(msArray, true);
    $("#" + formId + "GroupIsActive").addClass("hidden");
    $("#IsActive").prop("checked", true);
    $("#IsActive_bl").prop("checked", true)
    $("#" + formId + "CreateMultiple").removeClass("hidden");
}

//FillFormForEdit Generic version
function fillFormForEditGeneric(ids, httpType, url, getActive, formId, labelText, msArray) {

    if (GetActive) { $("#" + formId + "GroupIsActive").addClass("hidden"); }
    else { $("#" + formId + "GroupIsActive").removeClass("hidden"); }
    $("#" + formId + "CreateMultiple").addClass("hidden");

    var deferred0 = $.Deferred();

    clearFormInputs(formId, msArray);
    if (labelText) {$("#" + formId + "Label").text(labelText);}

    $.ajax({ type: httpType, url: url, timeout: 120000, data: { ids: ids, getActive: getActive }, dataType: "json" })
        .done(function (dbEntries) {
            var formInput = $.extend(true, {}, dbEntries[0]);
            $.each(dbEntries, function (i, dbEntry) {
                for (var property in dbEntry) {
                    if (!dbEntry.hasOwnProperty(property) || property == "Id" || property.slice(-1) == "_") {
                        continue;
                    }
                    if (property.slice(-3) == "_Id") {
                        if (formInput[property] != dbEntry[property]) {
                            formInput[property] = "_VARIES_";
                            formInput[property.slice(0, -2)] = "_VARIES_";
                        }
                        else {
                            formInput[property.slice(0, -2)] = "";
                            for (var subProp in dbEntry[property.slice(0, -2)]) {
                                if (dbEntry[property.slice(0, -2)][subProp] != null) {
                                    if (formInput[property.slice(0, -2)] != "") { formInput[property.slice(0, -2)] += " "; }
                                    formInput[property.slice(0, -2)] += dbEntry[property.slice(0, -2)][subProp];
                                }
                            }
                        }
                        continue;
                    }
                    if (formInput[property] != dbEntry[property]) { formInput[property] = "_VARIES_"; }
                }
            });

            for (var property in formInput) {
                if (!formInput.hasOwnProperty(property) || property == "Id" || property.slice(-1) == "_") {
                    continue;
                }
                if (property.slice(-3) == "_Id") {
                    $.each(msArray, function (i, ms) {
                        if (ms.id == property) {
                            if (formInput[property] != null) {
                                ms.addToSelection([{ id: formInput[property], name: formInput[property.slice(0, -2)] }], true);
                            }
                            return false;
                        }
                    });
                    continue;
                }
                if (property.slice(-3) == "_bl") {
                    if (formInput[property] == true) { $("#" + formId + " #" + property).prop("checked", true); }
                    continue;
                }
                $("#" + formId + " #" + property).val(formInput[property]);
            }

            if (dbEntries.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                msDisableUnique(msArray, false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
                msDisableUnique(msArray, true);
            }

            deferred0.resolve(dbEntries);
        })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

    return deferred0.promise();
}

//wraps FormForEditGeneric in modalWait and shows modalAJAXFail if failed
function fillFormForEditGenericWrp(ids, httpType, url, getActive, formId, labelText, msArray) {
    var deferred0 = $.Deferred();
    showModalWait();
    fillFormForEditGeneric(ids, httpType, url, getActive, formId, labelText, msArray)
        .always(hideModalWait)
        .done(function (currRecords) { deferred0.resolve(currRecords); })
        .fail(function (xhr, status, error) {
            showModalAJAXFail(xhr, status, error);
            deferred0.reject(xhr, status, error);
        });
    return deferred0.promise();
}

//SubmitEdits to DB - generic version
function submitEditsGeneric(formId, msArray, currRecords, httpType, url, noOfNewRecords) {

    noOfNewRecords = (typeof noOfNewRecords !== "undefined" && noOfNewRecords >= 1 && noOfNewRecords <= 100) ? noOfNewRecords : 1;
    currRecords = $.extend(true, [], currRecords);

    var deferred0 = $.Deferred();

    var modifiedProperties = [];
    $("#" + formId + " .modifiable").each(function (index) {
        if ($(this).data("ismodified")) modifiedProperties.push($(this).prop("id"));
    });
    $.each(msArray, function (i, ms) {
        if (ms.isModified == true) modifiedProperties.push(ms.id);
    });
    if (modifiedProperties.length == 0) { deferred0.resolve([]); }

    $.each(currRecords, function (i, currRecord) {
        for (var property in currRecord) {
            if (!currRecord.hasOwnProperty(property) || property == "Id") {
                continue;
            }
            if (property.slice(-1) == "_") {
                currRecord[property] = (function () { return; })();
                continue;
            }
            if (property.slice(-3) == "_Id") {
                $.each(msArray, function (i, ms) {
                    if (ms.id == property && ms.isModified) {
                        currRecord[property] = ms.getSelection().length != 0 ? (ms.getSelection())[0].id : "";
                        return false;
                    }
                });
                continue;
            }
            if (property.slice(-3) == "_bl" && $.inArray(property, modifiedProperties) != -1) {
                currRecord[property] = $("#" + formId + " #" + property).prop("checked");
                continue;
            }
            if ($.inArray(property, modifiedProperties) != -1) {
                currRecord[property] = $("#" + formId +" #" + property).val();
                continue;
            }
        }
        currRecord.ModifiedProperties = modifiedProperties;
    });

    if (currRecords.length == 1 && noOfNewRecords > 1) {
        multiplyRecordsAndModifyUniqueProps(formId, currRecords, noOfNewRecords);
    }

    $.ajax({ type: httpType, url: url, timeout: 120000, data: { records: currRecords }, dataType: "json" })
        .done(function (newEntryIds) { deferred0.resolve(newEntryIds); })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

    return deferred0.promise();
}

//wraps submitEditsGeneric in modalWait and shows modalAJAXFail if failed
function submitEditsGenericWrp(formId, msArray, currRecords, httpType, url, noOfNewRecords) {
    var deferred0 = $.Deferred();
    showModalWait();
    submitEditsGeneric(formId, msArray, currRecords, httpType, url, noOfNewRecords)
        .always(hideModalWait)
        .done(function (newEntryIds) { deferred0.resolve(newEntryIds); })
        .fail(function (xhr, status, error) {
            showModalAJAXFail(xhr, status, error);
            deferred0.reject(xhr, status, error);
            });
    return deferred0.promise();
}

//look up if the input field has dbisunique class and modify record prop to be unique
function multiplyRecordsAndModifyUniqueProps(formId, currRecords, noOfNewRecords) {

    for (var i = 1; i < noOfNewRecords; i++) {
        currRecords[i] = $.extend(true, {}, currRecords[0]);
    }

    $.each(currRecords, function (i, currRecord) {
        for (var property in currRecord) {
            if (!currRecord.hasOwnProperty(property) ||
            property == "Id" ||
            property.slice(-3) == "_Id" ||
            property.slice(-1) == "_") {
                continue;
            }
            if ($("#" + formId + " #" + property).data("valDbisunique") == true) {
                var j = i + 1;
                currRecord[property] = currRecord[property] + "_" + ("00" + j).slice(-3);
            }
        }
    });
}

//-----------------------------------------------------------------------------

//Fill Form for Edit from n:n related table - generic version
function fillFormForRelatedGeneric(tableAdd, tableRemove, ids,
    httpType, url, data, httpTypeNot, urlNot, dataNot, httpTypeMany, urlMany, dataMany, sortColumn) {

    sortColumn = (typeof sortColumn !== "undefined") ? sortColumn : 1;

    var deferred0 = $.Deferred();

    tableAdd.clear().search("").draw();
    tableRemove.clear().search("").draw();

    if (ids.length == 1) {
        $.when(
            $.ajax({type: httpTypeNot, url: urlNot, timeout: 120000, data: dataNot, dataType: "json" }),
            $.ajax({type: httpType, url: url, timeout: 120000, data: data, dataType: "json" })
        )
        .done(function (done1, done2) {
            tableAdd.rows.add(done1[0]).order([sortColumn, "asc"]).draw();
            tableRemove.rows.add(done2[0]).order([sortColumn, "asc"]).draw();
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
    }
    else {
        $.ajax({ type: httpTypeMany, url: urlMany, timeout: 120000, data: dataMany, dataType: "json" })
            .done(function (data) {
                tableAdd.rows.add(data).order([sortColumn, "asc"]).draw();
                if (ids.length != 0) tableRemove.rows.add(data).order([sortColumn, "asc"]).draw();
                deferred0.resolve();
            })
            .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
    }

    return deferred0.promise();
}

//Submit Edits for n:n related table - generic version
function submitEditsForRelatedGeneric(ids, idsAdd, idsRemove, url) {

    var deferred0 = $.Deferred();
    var deferred1 = $.Deferred();
    var deferred2 = $.Deferred();

    if (idsAdd.length == 0) deferred1.resolve();
    else {
        $.ajax({type: "POST", url: url, timeout: 120000, data: { ids: ids, idsAddRem: idsAdd, isAdd: true }, dataType: "json" })
            .done(function () { deferred1.resolve(); })
            .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
    }

    if (idsRemove.length == 0) deferred2.resolve();
    else {
        setTimeout(function () {
            $.ajax({ type: "POST", url: url, timeout: 120000, data: { ids: ids, idsAddRem: idsRemove, isAdd: false }, dataType: "json" })
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

//msSetSelectionSilent - version of setSelection not triggering events
function msSetSelectionSilent(ms, itemsArray) {
    ms.clear(true);
    ms.addToSelection(itemsArray, true);
}

//initialize MagicSuggest and add to MagicSuggest array
function msAddToMsArray(msArray, id, url, maxSelection, minChars, dataUrlParams, disabled, editable) {

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

//enable or disable DbUnique MagicSuggests in ms array
function msDisableUnique(msArray, disable) {
    disable = (typeof disable !== "undefined" && disable == false) ? false : true;
    $.each(msArray, function (i, ms) {
        if (disable == true && typeof ms.dataValDbisunique !== "undefined" && ms.dataValDbisunique == "true") { ms.disable(); }
        else { ms.enable(); }
    });
}

//enable or disable All MagicSuggests in ms array
function msDisableAll(msArray, disable) {
    disable = (typeof disable !== "undefined" && disable == false) ? false : true;
    $.each(msArray, function (i, ms) {
        if (disable == true) { ms.disable(); }
        else { ms.enable(); }
    });
}

//set MagicSuggests as modified ot not
function msSetAsModified(msArray, isModified) {
    isModified = (typeof isModified !== "undefined" && isModified == false) ? false : true;
    $.each(msArray, function (i, ms) { ms.isModified = isModified; });
}

//-----------------------------------------------------------------------------

//Pulls Model Information and formats edit form and column names
function updateViewsForModelGeneric(table, url, modelId, tableTitleId, editFormLabelId) {
    var deferred0 = $.Deferred();
    showModalWait();
    $.ajax({ type: "POST", url: url, timeout: 120000, data: { ids: [modelId] }, dataType: "json" })
        .always(hideModalWait)
        .done(function (data) {
            var modelName = (data[0].CompModelName) ? data[0].CompModelName : data[0].AssyModelName;
            if (tableTitleId) { $("#" + tableTitleId).text(modelName); }
            if (editFormLabelId) { $("#" + editFormLabelId).text("Edit " + modelName); }

            for (var prop in data[0]) {
                if (prop.indexOf("Attr") != -1 && prop.indexOf("Desc") != -1) {

                    var attrName = prop.slice(prop.indexOf("Attr"), 6);

                    $(table.column(attrName + ":name").header()).text(data[0][prop]);
                    $("label[for=" + attrName + "]").text(data[0][prop]);
                    $("#" + attrName).prop("placeholder", data[0][prop]);
                }
                if (prop.indexOf("Attr") != -1 && prop.indexOf("Type") != -1) {

                    var attrName = prop.slice(prop.indexOf("Attr"), 6);
                    var $targetEl = $("#" + attrName);
                    var attrsToRemove = "";

                    $.each($targetEl[0].attributes, function (i, attrib) {
                        if (typeof attrib !== "undefined" && attrib.name.indexOf("data-val") == 0) {
                            attrsToRemove += (attrsToRemove == "") ? attrib.name : " " + attrib.name;
                        }
                    });

                    var picker = $targetEl.data("DateTimePicker");
                    if (typeof picker !== "undefined") { picker.destroy(); }

                    $("#FrmGrp" + attrName).removeClass("hidden");

                    switch (data[0][prop]) {
                        case "NotUsed":
                            $("#FrmGrp" + attrName).addClass("hidden");
                            break;
                        case "String":
                            $targetEl.removeAttr(attrsToRemove).attr({
                                "data-val": "true",
                                "data-val-length": "The field must be a string with a maximum length of 255.",
                                "data-val-length-max": "255"
                            });
                            break;
                        case "Int":
                            $targetEl.removeAttr(attrsToRemove).attr({
                                "data-val": "true",
                                "data-val-dbisinteger": "The field must be an integer."
                            });
                            break;
                        case "Decimal":
                            $targetEl.removeAttr(attrsToRemove).attr({
                                "data-val": "true",
                                "data-val-number": "The field must be a number."
                            });
                            break;
                        case "DateTime":
                            $targetEl.removeAttr(attrsToRemove).attr({
                                "data-val": "true",
                                "data-val-dbisdatetimeiso": "The field must have YYYY-MM-DD HH:mm format."
                            });
                            $targetEl.datetimepicker({ format: "YYYY-MM-DD HH:mm" })
                                .on("dp.change", function (e) { $(this).data("ismodified", true); });
                            break;
                        case "Bool":
                            $targetEl.removeAttr(attrsToRemove).attr({
                                "data-val": "true",
                                "data-val-dbisbool": "The field must be 'true' or 'false'."
                            });
                            break;
                    }
                }
            }
            $("#EditForm").removeData("validator");
            $("#EditForm").removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse("#EditForm")
            deferred0.resolve();
        })
        .fail(function (xhr, status, error) {
            showModalAJAXFail(xhr, status, error);
            deferred0.reject(xhr, status, error);
        });
    return deferred0.promise();
}

//checkAllEqualInArray - checks if all items in array are same
function modelIdsAreSame(modelIds) {
    if (!modelIds || modelIds.length == 0) {
        return false;
    }
    for (var i in modelIds) {
        if (modelIds[i] == "" || modelIds[i] == null || modelIds[i] != modelIds[0]) {
            return false;
        }
    }
    return true;
}

//-----------------------------------------------------------------------------

//opens new window by submitting a form - needed to POST version of window.open
function submitFormFromArray(verb, url, target, dataArray, parameterName) {
    var form = document.createElement("form");
    form.method = verb;
    form.action = url;
    form.target = target || "_self";
    $.each(dataArray, function (i, arrayElement) {
        var input = document.createElement("input");
        input.name = parameterName + "[" + i + "]";
        input.value = arrayElement;
        form.appendChild(input);
    });
    form.style.display = 'none';
    document.body.appendChild(form);
    form.submit();
};




