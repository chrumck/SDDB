﻿/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../jquery.validate.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="../FileSaver.js" />

//sddbConstructor
var sddbConstructor = function (customConfig) {
    "use strict";

    //private variables----------------------------------------------------------------------------------------------//
    var windowYpos = 0,
    tablePage = 0,
    tableOrder,
    tableSearch,
    tableSelectedIds,

    //defaultConfig
    defaultConfig = {
        currentIds: [],
        currentRecords: [],
        currentActive: true,
        recordTemplate: {},
        magicSuggests: [],

        tableMain: {},
        tableMainColumnSets: [[1], [2, 3]],
        selectedColumnSet: 1,
        tableMainPanelId: "panelTableMain",
        tableMainPanelClassActive: "panel-primary",
        tableMainPanelClassInActive: "panel-tdo-danger",

        initialViewId: "initialView",
        mainViewId: "mainView",
        editFormId: "editForm",
        editFormViewId: "editFormView",

        mainViewBtnGroupClass: "tdo-btngroup-main",
        editFormBtnGroupCreateClass: "tdo-btngroup-edit",
        editFormBtnGroupEditClass: "tdo-btngroup-edit",

        labelTextCreate: "Create Record",
        labelTextEdit: "Edit Record",
        labelTextCopy: "Copy Record",

        httpTypeFillForEdit: "POST",
        urlFillForEdit: "",

        httpTypeEdit: "POST",
        urlEdit: "",

        urlDelete: "",

        urlRefreshMainView: "",
        dataRefreshMainView: function () { return { getActive: cfg.currentActive }; },
        httpTypeRefreshMainView: "GET"
    },

    //config
    cfg = $.extend(true, {}, defaultConfig, customConfig),

    //private functions----------------------------------------------------------------------------------------------//

    //getFormInputFromDbEntriesHelper
    getFormInputFromDbEntriesHelper = function (dbEntries) {
        var formInput = $.extend(true, {}, dbEntries[0]);
        $.each(dbEntries, function (i, dbEntry) {
            var property,
            msNameProperty,
            subProp;

            for (property in dbEntry) {
                if (!dbEntry.hasOwnProperty(property) || property == "Id" || property.slice(-1) == "_") {
                    continue;
                }
                if (property.slice(-3) == "_Id") {
                    msNameProperty = property.slice(0, -2);
                    if (formInput[property] != dbEntry[property]) {
                        formInput[property] = "_VARIES_";
                        formInput[msNameProperty] = "_VARIES_";
                    }
                    else {
                        formInput[msNameProperty] = "";
                        for (subProp in dbEntry[msNameProperty]) {
                            if (subProp !== "Id" && dbEntry[msNameProperty][subProp] !== null) {
                                if (formInput[msNameProperty] !== "") {
                                    formInput[msNameProperty] += " ";
                                }
                                formInput[msNameProperty] += dbEntry[msNameProperty][subProp];
                            }
                        }
                    }
                }
                else {
                    if (formInput[property] != dbEntry[property]) {
                        formInput[property] = "_VARIES_";
                    }
                }
            }
        });
        return formInput;
    },

    //setFormFromFormInputHelper
    setFormFromFormInputHelper = function (formInput, formId, msArray) {
        for (var property in formInput) {
            if (!formInput.hasOwnProperty(property) || property == "Id" || property.slice(-1) == "_") { continue; }
            if (property.slice(-3) == "_Id" && msArray) {
                $.each(msArray, function (i, ms) {
                    if (ms.id == property) {
                        if (formInput[property] !== null) {
                            sddbObj.msSetSelectionSilent(ms,
                                [{ id: formInput[property], name: formInput[property.slice(0, -2)] }]);
                        }
                        return false;
                    }
                });
                continue;
            }
            if (property.slice(-3) == "_bl") {
                if (formInput[property] === true) { $("#" + formId + " #" + property).prop("checked", true); }
                continue;
            }
            $("#" + formId + " #" + property).val(formInput[property]);
        }
    },

    //object to return-----------------------------------------------------------------------------------------------//
    sddbObj = {};
    sddbObj.cfg = cfg;

    //object to return - public methods------------------------------------------------------------------------------//

    //setConfig
    sddbObj.setConfig = function (newConfig) {
        $.extend(true, cfg, newConfig);
    };

    //-----------------------------------------------------------------------------

    //doNothingAndResolve
    sddbObj.doNothingAndResolve = function () { return $.Deferred().resolve(); };

    //-----------------------------------------------------------------------------

    //Show Modal Nothing Selected
    sddbObj.showModalNothingSelected = function (bodyText) {
        $("#modalInfoLabel").text("Nothing Selected");
        $("#modalInfoBody").text(bodyText || "Please select one or more rows.");
        $("#modalInfoBodyPre").empty().addClass("hidden");
        $("#modalInfo").modal("show");
    };

    //Show Modal Selected other than one row
    sddbObj.showModalSelectOne = function (bodyText) {
        $("#modalInfoLabel").text("Select One Row");
        $("#modalInfoBody").text(bodyText || "Please select one row.");
        $("#modalInfoBodyPre").empty().addClass("hidden");
        $("#modalInfo").modal("show");
    };

    //showModalWait
    sddbObj.showModalWait = function () {
        $("#modalWait").modal({ show: true, backdrop: "static", keyboard: false });
    };

    //hideModalWait
    sddbObj.hideModalWait = function () {
        $("#modalWait").modal("hide");
    };

    //showModalFail
    sddbObj.showModalFail = function (label, body, bodyPre) {
        $("#modalInfoLabel").text(label || "Undefined Error");
        $("#modalInfoBody").html(body || "");
        if (bodyPre) { $("#modalInfoBodyPre").text(bodyPre).removeClass("hidden"); }
        else { $("#modalInfoBodyPre").text("").addClass("hidden"); }
        $("#modalInfo").modal("show");
    };

    //showModalAJAXFail
    sddbObj.showModalAJAXFail = function (xhr, status, error) {
        var errMessage = "No error details available.";
        if (xhr.responseJSON) { errMessage = xhr.responseJSON.responseText.substr(0, 512); }
        else if (xhr.responseText) { errMessage = xhr.responseText.substr(0, 512); }
        errMessage = $($.parseHTML(errMessage)).text();
        $("#modalInfoLabel").text("Server Error");
        $("#modalInfoBody").html("Error type: <strong>" + error +
            "</strong> , Status: <strong>" + status + "</strong>");
        $("#modalInfoBodyPre").text(errMessage).removeClass("hidden");
        setTimeout(function () { $("#modalInfo").modal("show"); }, 100);
    };

    //showModalConfirm
    sddbObj.showModalConfirm = function (bodyText, labelText, focusOn, btnYesClass) {
        var deferred0 = $.Deferred();
        $("#modalConfirmBody").text(bodyText || "Please confirm...");
        $("#modalConfirmLabel").text(labelText || "Please Confirm");
        $("#modalConfirmBtnNo, #modalConfirmBtnYes").off("click");
        $("#modalConfirmBtnNo, #modalConfirmBtnYes").click(function () { $("#modalConfirm").modal("hide"); });
        $("#modalConfirmBtnNo").click(function () { return deferred0.reject(); });
        $("#modalConfirmBtnYes").click(function () { return deferred0.resolve(); });
        $("#modalConfirm").modal("show");
        if (focusOn && focusOn.toLowerCase() === "no") { $("#modalConfirmBtnNo").focus(); }
        if (focusOn && focusOn.toLowerCase() === "yes") { $("#modalConfirmBtnYes").focus(); }
        $("#modalConfirmBtnYes").removeClass().addClass(btnYesClass || "btn btn-primary");
        return deferred0.promise();
    };

    //showModalDatePrompt
    sddbObj.showModalDatePrompt = function (bodyText, labelText, promptDate) {
        var deferred0 = $.Deferred();

        $("#modalDatePromptBody").text(bodyText || "");
        $("#modalDatePromptLabel").text(labelText || "Please Select Date");

        if (promptDate) { $("#modalDatePromptInput").data("DateTimePicker").date(moment(promptDate)); }
        else { $("#modalDatePromptInput").data("DateTimePicker").date(moment().format("YYYY-MM-DD")); }

        $("#modalDatePromptBtnCancel, #modalDatePromptBtnOk").off("click");
        $("#modalDatePromptBtnCancel, #modalDatePromptBtnOk")
            .click(function () { $("#modalDatePrompt").modal("hide"); });
        $("#modalDatePromptBtnCancel").click(deferred0.reject);
        $("#modalDatePromptBtnOk").click(function () {
            return deferred0.resolve($("#modalDatePromptInput").data("DateTimePicker").date());
        });
        $("#modalDatePrompt").modal("show");
        return deferred0.promise();
    };

    //-----------------------------------------------------------------------------

    //wraps callBackToWrap in modalWait and shows modalAJAXFail if failed
    sddbObj.modalWaitWrapper = function (callBackToWrap) {
        sddbObj.showModalWait();
        return callBackToWrap()
            .always(sddbObj.hideModalWait)
            .fail(function (xhr, status, error) { sddbObj.showModalAJAXFail(xhr, status, error); });
    };

    //-----------------------------------------------------------------------------

    //switchView 
    sddbObj.switchView = function (fromViewId, toViewId, toBtnGroupClass, dataTable) {
        fromViewId = fromViewId || cfg.initialViewId;
        toViewId = toViewId || cfg.mainViewId;
        toBtnGroupClass = toBtnGroupClass || cfg.mainViewBtnGroupClass;

        $("#" + fromViewId).addClass("hidden");
        $("#" + toViewId).removeClass("hidden");
        $("[class*='tdo-btngroup']").addClass("hidden");
        $("[class~='" + toBtnGroupClass + "']").removeClass("hidden");
        
        if (dataTable) {
            sddbObj.loadViewSettings(dataTable);
        }
        else {
            window.scrollTo(0, 0);
        }
    };

    //saveViewSettings
    sddbObj.saveViewSettings = function (dataTable) {
        dataTable = dataTable || cfg.tableMain;
        windowYpos = window.pageYOffset || document.documentElement.scrollTop;
        if (dataTable) {
            tablePage = dataTable.page();
            tableOrder = dataTable.order();
            tableSearch = dataTable.search();
            tableSelectedIds = dataTable.cells(".ui-selected", "Id:name").data().toArray();
        }
    };

    //loadViewSettings
    sddbObj.loadViewSettings = function (dataTable) {
        dataTable.order(tableOrder).search(tableSearch).page(tablePage).draw(false);
        var indexesToSelect = dataTable.rows(function (idx, data, node) {
            return $.inArray(data.Id, tableSelectedIds) != -1;
        }).eq(0).toArray();
        dataTable.rows(indexesToSelect).nodes().to$().addClass("ui-selected");
        window.scrollTo(0, windowYpos);
    };

    //-----------------------------------------------------------------------------

    //Refresh  table from AJAX - generic version
    sddbObj.refreshTableGeneric = function (table, url, data, httpType) {
        httpType = httpType || "GET";
        data = data || {};
        table.clear().search("").draw();
        return $.ajax({ type: httpType, url: url, timeout: 120000, data: data, dataType: "json" })
            .then(function (dbEntries) {
                table.rows.add(dbEntries).order([1, "asc"]).draw();
            });
    };

    //Refresh  table from AJAX - generic version - showing wait dialogs
    sddbObj.refreshTblGenWrp = function (table, url, data, httpType) {
        var deferred0 = $.Deferred();
        sddbObj.showModalWait();
        sddbObj.refreshTableGeneric(table, url, data, httpType)
            .always(sddbObj.hideModalWait)
            .done(deferred0.resolve)
            .fail(function (xhr, status, error) {
                sddbObj.showModalAJAXFail(xhr, status, error);
                deferred0.reject(xhr, status, error);
            });
        return deferred0.promise();
    };

    //exportTableToTxt - exports table data to a txt tab separated file
    sddbObj.exportTableToTxt = function (table) {
        table = table || cfg.tableMain;

        //convertObjectToStringHelper
        var convertObjectToStringHelper = function (dataObject, retrievePropNames) {
            var outputString = "",
            property;

            for (property in dataObject) {
                if (!dataObject.hasOwnProperty(property)) {
                    continue;
                }
                if (typeof dataObject[property] === "object" && dataObject[property] !== null) {
                    outputString += convertObjectToStringHelper(dataObject[property], retrievePropNames);
                    continue;
                }
                if (retrievePropNames) {
                    outputString += property + "\t";
                    continue;
                }
                outputString += dataObject[property] !== null ? dataObject[property] + "\t" : "\t";
            }
            outputString = outputString.replace(/\n/g, " ");
            return outputString;
        };

        //main
        sddbObj.showModalWait();
        setTimeout(function () {
            var tableData = table.rows({ search: "applied" }).data().toArray(),
            txtOutput = convertObjectToStringHelper(tableData[0], true),
            fileData;

            txtOutput.slice(-1);
            txtOutput += "\n";
            $.each(tableData, function (index, tableRow) {
                txtOutput += convertObjectToStringHelper(tableRow);
                txtOutput.slice(-1);
                txtOutput += "\n";
            });
            fileData = new Blob([txtOutput], { type: "text/plain;charset=utf-8" });
            sddbObj.hideModalWait();
            saveAs(fileData, "SDDBdataExport_" + moment().format("YYYYMMDD_HHmmss") + ".txt");
        }, 200);
    };

    //showColumnSet
    sddbObj.showColumnSet = function (columnSetIdx, columnSetArray) {
        columnSetIdx = columnSetIdx || 1;
        columnSetArray = columnSetArray || cfg.tableMainColumnSets;
        if (!cfg.tableMain) { return; }
        cfg.tableMain.columns().visible(false);
        cfg.tableMain.columns(columnSetArray[0]).visible(true);
        cfg.tableMain.columns(columnSetArray[columnSetIdx]).visible(true);
        cfg.selectedColumnSet = columnSetIdx;
    };

    //setCurrentActive
    sddbObj.setCurrentActive = function (newCurrentActive) {
        cfg.currentActive = newCurrentActive;
    };

    //switchTableToActive
    sddbObj.switchTableToActive = function (newCurrentActive) {
        sddbObj.setCurrentActive(newCurrentActive);

        if (newCurrentActive) {
            $("#" + cfg.tableMainPanelId)
                .removeClass(cfg.tableMainPanelClassInActive)
                .addClass(cfg.tableMainPanelClassActive);
        } else {
            $("#" + cfg.tableMainPanelId)
                .removeClass(cfg.tableMainPanelClassActive)
                .addClass(cfg.tableMainPanelClassInActive);
        }

        sddbObj.refreshMainView();
    };

    //-----------------------------------------------------------------------------

    //checking if form is valid
    sddbObj.formIsValid = function (id, isCreate) {
        if ($("#" + id).valid()) { return true; }
        if (isCreate) { return false; }
        var isValid = true;
        $("#" + id + " input").each(function (index) {
            if (!$(this).hasClass("modifiable")) { return true; }
            if ($(this).data("ismodified") && $(this).hasClass("input-validation-error")) {
                isValid = false;
                return true;
            }
            $(this).removeClass("input-validation-error");
            $("[data-valmsg-for='" + this.id + "']").empty();
        });
        return isValid;
    };

    //Clear inputs from forms and reset .ismodified to false
    sddbObj.clearFormInputs = function (formId, msArray) {
        $("#" + formId + " :input").prop("checked", false).prop("selected", false)
            .not(":button, :submit, :reset, :radio, :checkbox").val("");
        $("#" + formId + " [data-valmsg-for]").empty();
        $("#" + formId + " .input-validation-error").removeClass("input-validation-error");
        $("#" + formId + " .modifiable").data("ismodified", false);

        if (msArray) {
            $.each(msArray, function (i, ms) {
                ms.clear(true);
                ms.isModified = false;
            });
        }
    };

    //-----------------------------------------------------------------------------

    //Prepare Form For Create
    sddbObj.fillFormForCreateGeneric = function (formId, msArray, labelText) {
        sddbObj.clearFormInputs(formId, msArray);
        if (labelText) { $("#" + formId + "Label").text(labelText); }
        $("#" + formId + " [data-val-dbisunique][class ~= 'modifiable']").prop("disabled", false);
        sddbObj.msDisableUnique(msArray, false);
        $("#" + formId + " .modifiable").data("ismodified", true);
        sddbObj.msSetAsModified(msArray, true);
        $("#" + formId + "GroupIsActive").addClass("hidden");
        $("#" + formId + "CreateMultiple").removeClass("hidden");
        $("#IsActive_bl").prop("checked", true);
    };

    //-----------------------------------------------------------------------------

    //fillFormForCopyGeneric
    sddbObj.fillFormForCopyGeneric = function (ids, httpType, url, getActive, formId, labelText, msArray) {
        return $.ajax({
            type: httpType,
            url: url,
            timeout: 120000,
            data: { ids: ids, getActive: getActive },
            dataType: "json"
        })
            .then(function (dbEntries) {
                return sddbObj.fillFormForCopyFromDbEntries(dbEntries, formId, labelText, msArray);
            });
    };

    //fillFormForCopyFromDbEntries - takes dbEntries instead of executing AJAX request
    sddbObj.fillFormForCopyFromDbEntries = function (dbEntries, formId, labelText, msArray) {
        sddbObj.fillFormForCreateGeneric(formId, msArray, labelText);
        setFormFromFormInputHelper(getFormInputFromDbEntriesHelper(dbEntries), formId, msArray);
        $("#IsActive_bl").prop("checked", true);
        $("#" + formId + " [data-val-dbisunique]").each(function (index, element) {
            if ($(element).val() === "") { return true; }
            $(element).val($(element).val() + "_COPY");
        });
        if (msArray) {
            $.each(msArray, function (i, ms) {
                if (ms.dataValDbisunique) {
                    ms.clear(true);
                    return true;
                }
                if (ms.getValue()[0] === "_VARIES_") {
                    ms.clear(true);
                }
            });
        }
        return $.Deferred().resolve(dbEntries);
    };

    //-----------------------------------------------------------------------------

    //FillFormForEdit Generic version
    sddbObj.fillFormForEditGeneric = function (ids, httpType, url, getActive, formId, labelText, msArray) {
        var deferred0 = $.Deferred();
        $.ajax({
            type: httpType,
            url: url,
            timeout: 120000,
            data: { ids: ids, getActive: getActive },
            dataType: "json"
        })
            .done(function (dbEntries) {
                sddbObj.fillFormForEditFromDbEntries(getActive, dbEntries, formId, labelText, msArray)
                    .done(function () { deferred0.resolve(dbEntries); });
            })
            .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
        return deferred0.promise();
    };
        
    //fillFormForEditFromDbEntries - takes dbEntries instead of executing AJAX request
    sddbObj.fillFormForEditFromDbEntries = function (getActive, dbEntries, formId, labelText, msArray) {

        //setFormForUniqueFieldsHelper
        var setFormForUniqueFieldsHelper = function () {
            if (dbEntries.length == 1) {
                $("#" + formId + " [data-val-dbisunique][class ~= 'modifiable']").prop("disabled", false);
                sddbObj.msDisableUnique(msArray, false);
            }
            else {
                $("#" + formId + " [data-val-dbisunique][class ~= 'modifiable']").prop("disabled", true);
                sddbObj.msDisableUnique(msArray, true);
            }
        },

        //setFormforEditHelper
        setFormforEditHelper = function () {
            if (getActive) { $("#" + formId + "GroupIsActive").addClass("hidden"); }
            else { $("#" + formId + "GroupIsActive").removeClass("hidden"); }
            $("#" + formId + "CreateMultiple").addClass("hidden");
            sddbObj.clearFormInputs(formId, msArray);
            if (labelText) { $("#" + formId + "Label").text(labelText); }
        };

        //main
        setFormforEditHelper();
        setFormFromFormInputHelper(getFormInputFromDbEntriesHelper(dbEntries), formId, msArray);
        setFormForUniqueFieldsHelper();
        return $.Deferred().resolve(dbEntries);
    };

    //-----------------------------------------------------------------------------

    //SubmitEdits to DB - generic version
    sddbObj.submitEditsGeneric = function (formId, msArray, dbEntries, httpType, url, noOfNewRecords) {

        //multiplyRecordsAndModifyUniquePropsHelper
        var multiplyRecordsAndModifyUniquePropsHelper = function () {
            var i;

            for (i = 1; i < noOfNewRecords; i += 1) {
                dbEntriesClone[i] = $.extend(true, {}, dbEntriesClone[0]);
            }

            $.each(dbEntriesClone, function (i, currRecord) {
                var property, j;

                for (property in currRecord) {
                    if (!currRecord.hasOwnProperty(property) ||
                        property === "Id" || property.slice(-3) === "_Id" ||
                        property.slice(-3) === "_bl" || property.slice(-1) === "_" ||
                        currRecord[property] === null || currRecord[property] === "") {
                        continue;
                    }
                    if ($("#" + formId + " #" + property).data("valDbisunique") === true) {
                        j = i + 1;
                        currRecord[property] = currRecord[property] + "_" + ("00" + j).slice(-3);
                    }
                }
            });
        },

        //private variables
        dbEntriesClone = $.extend(true, [], dbEntries),
        deferred0 = $.Deferred(),
        modifiedProperties = [];

        noOfNewRecords = (noOfNewRecords && noOfNewRecords >= 1 && noOfNewRecords <= 100) ? noOfNewRecords : 1;

        $("#" + formId + " .modifiable").each(function (index) {
            if ($(this).data("ismodified")) { modifiedProperties.push($(this).prop("id")); }
        });
        $.each(msArray, function (i, ms) {
            if (ms.isModified === true) { modifiedProperties.push(ms.id); }
        });

        $.each(dbEntriesClone, function (i, currRecord) {
            for (var property in currRecord) {
                if (!currRecord.hasOwnProperty(property) ||
                        property == "Id" ||
                        $.inArray(property, modifiedProperties) == -1) {
                    continue;
                }
                if (property.slice(-3) == "_Id") {
                    $.each(msArray, function (i, ms) {
                        if (ms.id == property) {
                            currRecord[property] = ms.getSelection().length !== 0 ? (ms.getSelection())[0].id : "";
                            currRecord[property.slice(0, -2)] = (function () { return; })();
                            return false;
                        }
                    });
                    continue;
                }
                if (property.slice(-3) == "_bl") {
                    currRecord[property] = $("#" + formId + " #" + property).prop("checked");
                    continue;
                }
                currRecord[property] = $("#" + formId + " #" + property).val();
            }
            currRecord.ModifiedProperties = modifiedProperties;
        });

        if (dbEntriesClone.length == 1 && noOfNewRecords > 1) { multiplyRecordsAndModifyUniquePropsHelper(); }

        if (modifiedProperties.length === 0) {
            return deferred0.resolve({
                "Success": "True",
                "newEntryIds": [],
                "propsModified": false
            }, dbEntriesClone);
        }

        $.ajax({ type: httpType, url: url, timeout: 120000, data: { records: dbEntriesClone }, dataType: "json" })
            .done(function (data) {
                $("#" + formId + " .modifiable").data("ismodified", false);
                sddbObj.msSetAsModified(msArray, false);
                data.propsModified = true;
                deferred0.resolve(data, dbEntriesClone);
            })
            .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

        return deferred0.promise();
    };
        
    //-----------------------------------------------------------------------------

    //Fill Form for Edit from n:n related table - generic version
    sddbObj.fillFormForRelatedGeneric = function (tableAdd, tableRemove, ids,
            httpType, url, data, httpTypeNot, urlNot, dataNot, httpTypeMany, urlMany, dataMany, sortColumn) {

        sortColumn = (typeof sortColumn !== "undefined") ? sortColumn : 1;

        var deferred0 = $.Deferred();

        tableAdd.clear().search("").draw();
        tableRemove.clear().search("").draw();

        if (ids.length == 1) {
            $.when(
                $.ajax({ type: httpTypeNot, url: urlNot, timeout: 120000, data: dataNot, dataType: "json" }),
                $.ajax({ type: httpType, url: url, timeout: 120000, data: data, dataType: "json" })
            )
            .done(function (done1, done2) {
                tableAdd.rows.add(done1[0]).order([sortColumn, "asc"]).draw();
                tableRemove.rows.add(done2[0]).order([sortColumn, "asc"]).draw();
                return deferred0.resolve();
            })
            .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
        }
        else {
            $.ajax({ type: httpTypeMany, url: urlMany, timeout: 120000, data: dataMany, dataType: "json" })
                .done(function (data) {
                    tableAdd.rows.add(data).order([sortColumn, "asc"]).draw();
                    if (ids.length !== 0) { tableRemove.rows.add(data).order([sortColumn, "asc"]).draw(); }
                    return deferred0.resolve();
                })
                .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
        }
        return deferred0.promise();
    };
        
    //Submit Edits for n:n related table - generic version
    sddbObj.submitEditsForRelatedGeneric = function (ids, idsAdd, idsRemove, url) {

        var deferred0 = $.Deferred(),
        deferred1 = $.Deferred(),
        deferred2 = $.Deferred();

        if (idsAdd.length === 0) { deferred1.resolve(); }
        else {
            $.ajax({
                type: "POST",
                url: url,
                timeout: 120000,
                data: { ids: ids, idsAddRem: idsAdd, isAdd: true },
                dataType: "json"
            })
                .done(function () { deferred1.resolve(); })
                .fail(function (xhr, status, error) { deferred1.reject(xhr, status, error); });
        }

        if (idsRemove.length === 0) { deferred2.resolve(); }
        else {
            setTimeout(function () {
                $.ajax({
                    type: "POST",
                    url: url,
                    timeout: 120000,
                    data: { ids: ids, idsAddRem: idsRemove, isAdd: false },
                    dataType: "json"
                })
                    .done(function () { deferred2.resolve(); })
                    .fail(function (xhr, status, error) { deferred2.reject(xhr, status, error); });
            }, 500);
        }

        $.when(deferred1, deferred2)
            .done(function () { return deferred0.resolve(); })
            .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });

        return deferred0.promise();
    };
        
    //-----------------------------------------------------------------------------
    
    //refactor to accept an object for ms settings, get rid of disabled (it doesn't work anyway)
    //old params (msArray, id, url, maxSelection, minChars, dataUrlParams, disabled, editable)
    //msAddToMsArrayNew
    sddbObj.msAddToArray = function (id, url, customSettings, onSelectionHandler, msArray) {
        msArray = msArray || cfg.magicSuggests;

        var element = $("#" + id),
        dataValRequired = $(element).attr("data-val-required"),
        dataValDbisunique = $(element).attr("data-val-dbisunique"),
        defaultSettings = {
            data: url,
            invalidCls: "input-validation-error",
            allowFreeEntries: false,
            maxSelection: 1,
            editable: true,
            required: (typeof dataValRequired !== "undefined") ? true : false,
            resultAsString: true,
            ajaxConfig: { error: function (xhr, status, error) { sddbObj.showModalAJAXFail(xhr, status, error); } }
        },
        settings = $.extend(true, {}, defaultSettings, customSettings),
        ms = element.magicSuggest(settings);

        ms.id = id;
        ms.dataValRequired = dataValRequired;
        ms.dataValDbisunique = dataValDbisunique;
        ms.isModified = false;

        if ($(element).hasClass("modifiable")) {
            ms.modifiable = true;
            $(ms).on("selectionchange", function (e, m) { this.isModified = true; });

            $(ms).on("blur", function (e, m) {
                if (!this.isValid()) {
                    $("#" + this.id).addClass("input-validation-error");
                    $("[data-valmsg-for=" + this.id + "]")
                        .text("Input missing or invalid.").removeClass("field-validation-valid")
                        .addClass("field-validation-error");
                }
                else {
                    $("#" + this.id).removeClass("input-validation-error");
                    $("[data-valmsg-for=" + this.id + "]").text("").addClass("field-validation-valid")
                        .removeClass("field-validation-error");
                }
            });
            if (!settings.editable) { $(ms).on("focus", function (e, m) { this.expand(); }); }
        }
        else {
            ms.modifiable = false;
            ms.disable();
        }

        if (onSelectionHandler) { $(ms).on("selectionchange", onSelectionHandler); }

        msArray.push(ms);
    };

    //check if magicSuggests in MagicSugest Array are valid
    sddbObj.msIsValid = function (msArray) {
        if (!msArray) { return; }
        var msCheck = true;
        $.each(msArray, function (i, ms) {
            if (!ms.modifiable) { return true; }
            if (!ms.isValid()) { msCheck = false; }
        });
        return msCheck;
    };

    //change formatting of invalid MagicSugest's
    sddbObj.msValidate = function (msArray) {
        if (!msArray) { return; }
        $.each(msArray, function (i, ms) {
            if (!ms.modifiable) { return true; }
            if (!ms.isValid()) {
                $("#" + ms.id).addClass("input-validation-error");
                $("[data-valmsg-for=" + ms.id + "]").text("Input missing or invalid.")
                    .removeClass("field-validation-valid")
                    .addClass("field-validation-error");
            }
        });
    };

    //enable or disable DbUnique magicSuggests in ms array
    sddbObj.msDisableUnique = function (msArray, disable) {
        if (!msArray) { return; }
        disable = (typeof disable !== "undefined" && disable === false) ? false : true;
        $.each(msArray, function (i, ms) {
            if (!ms.modifiable) { return true; }
            if (disable && ms.dataValDbisunique) { ms.disable(); }
            else { ms.enable(); }
        });
    };

    //enable or disable All magicSuggests in ms array
    sddbObj.msDisableAll = function (msArray, disable) {
        if (!msArray) { return; }
        disable = (typeof disable !== "undefined" && disable === false) ? false : true;
        $.each(msArray, function (i, ms) {
            if (!ms.modifiable) { return true; }
            if (disable) { ms.disable(); }
            else { ms.enable(); }
        });
    };

    //set magicSuggests as modified ot not
    sddbObj.msSetAsModified = function (msArray, isModified) {
        if (!msArray) { return; }
        isModified = (typeof isModified !== "undefined" && isModified === false) ? false : true;
        $.each(msArray, function (i, ms) {
            if (!ms.modifiable) { return true; }
            ms.isModified = isModified;
        });
    };

    //msSetSelectionSilent - version of setSelection not triggering events
    sddbObj.msSetSelectionSilent = function (ms, itemsArray) {
        ms.clear(true);
        ms.addToSelection(itemsArray, true);
    };

    //-----------------------------------------------------------------------------

    //Pulls type information and formats table column names
    sddbObj.updateTableForExtended = function (httpType, url, data, table) {
        return $.ajax({ type: httpType, url: url, data: data, timeout: 120000, dataType: "json" })
            .then(function (data) {
                var typeHasAttrs = false,
                entityType = data[0],
                prop;

                for (prop in entityType) {
                    if (prop.indexOf("Attr") == -1 || prop.indexOf("Desc") == -1) { continue; }
                    if (entityType[prop] !== null && entityType[prop] !== "") { typeHasAttrs = true; }
                    $(table.column(prop.slice(prop.indexOf("Attr"), 6) + ":name").header()).text(entityType[prop]);
                }
                return typeHasAttrs;
            });
    };

    //pulls type information and formats form fields including validation reset
    sddbObj.updateFormForExtended = function (httpType, url, data, formId) {

        //changeFormFieldDescriptionHelper
        var changeFormFieldDescriptionHelper = function (formId, entityType, prop) {
            if (prop.indexOf("Desc") != -1) {
                var attrName = prop.slice(prop.indexOf("Attr"), 6);
                $("#" + formId + " label[for=" + attrName + "]").text(entityType[prop]);
                $("#" + formId + " #" + attrName).prop("placeholder", entityType[prop]);
            }
        },

        //changeFormFieldValidationHelper
        changeFormFieldValidationHelper = function (formId, entityType, prop) {
            var attrName,
            $targetEl;

            if (prop.indexOf("Type") != -1) {
                attrName = prop.slice(prop.indexOf("Attr"), 6);
                $targetEl = $("#" + formId + " #" + attrName);

                removeValidationAttributesHelper($targetEl);
                if ($targetEl.data("DateTimePicker")) {
                    $targetEl.off("dp.change");
                    $targetEl.data("DateTimePicker").destroy();
                }
                $("#" + formId + " #frmGrp" + attrName).removeClass("hidden");

                if (entityType[prop] == "NotUsed") {
                    $("#" + formId + " #frmGrp" + attrName).addClass("hidden");
                }
                if (entityType[prop] == "String") {
                    $targetEl.attr({
                        "data-val": "true",
                        "data-val-length": "The field must be a string with a maximum length of 255.",
                        "data-val-length-max": "255"
                    });
                }
                if (entityType[prop] == "Int") {
                    $targetEl.attr({
                        "data-val": "true",
                        "data-val-dbisinteger": "The field must be an integer."
                    });
                }
                if (entityType[prop] == "Decimal") {
                    $targetEl.attr({
                        "data-val": "true",
                        "data-val-number": "The field must be a number."
                    });
                }
                if (entityType[prop] == "DateTime") {
                    $targetEl.attr({
                        "data-val": "true",
                        "data-val-dbisdatetimeiso": "The field must have YYYY-MM-DD HH:mm format."
                    });
                    $targetEl.datetimepicker({ format: "YYYY-MM-DD HH:mm" })
                        .on("dp.change", function (e) { $(this).data("ismodified", true); });
                }
                if (entityType[prop] == "Bool") {
                    $targetEl.attr({
                        "data-val": "true",
                        "data-val-dbisbool": "The field must be 'true' or 'false'."
                    });
                }
            }
        },

        //removeValidationAttributesHelper
        removeValidationAttributesHelper = function ($element) {
            var attrsToRemove = "";
            $.each($element[0].attributes, function (i, attrib) {
                if (typeof attrib !== "undefined" && attrib.name.indexOf("data-val") === 0) {
                    attrsToRemove += (attrsToRemove === "") ? attrib.name : " " + attrib.name;
                }
            });
            $element.removeAttr(attrsToRemove);
        },

        //private variables
        deferred0 = $.Deferred();

        $.ajax({ type: httpType, url: url, data: data, timeout: 120000, dataType: "json" })
            .done(function (data) {
                var typeHasAttrs = false,
                entityType = data[0],
                prop;

                for (prop in entityType) {
                    if (prop.indexOf("Attr") == -1) { continue; }
                    if (prop.indexOf("Type") != -1 && entityType[prop] != "NotUsed") { typeHasAttrs = true; }
                    changeFormFieldDescriptionHelper(formId, entityType, prop);
                    changeFormFieldValidationHelper(formId, entityType, prop);
                }
                $("#" + formId).removeData("validator");
                $("#" + formId).removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#" + formId);
                deferred0.resolve(typeHasAttrs);
            })
            .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
        return deferred0.promise();
    };
        
    //-----------------------------------------------------------------------------

    //sendSingleIdToNewWindow
    sddbObj.sendSingleIdToNewWindow = function (newUrl) {
        cfg.currentIds = cfg.tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (cfg.currentIds.length !== 1) {
            sddbObj.showModalSelectOne();
            return;
        }
        window.open(newUrl + cfg.currentIds[0]);
    };

    //opens new window by submitting a form - needed to POST version of window.open
    sddbObj.submitFormFromArray = function (verb, url, target, dataArray, parameterName) {
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
        form.style.display = "none";
        document.body.appendChild(form);
        form.submit();
    };

    //---------------------------------------------------------------------------------------------------------------//

    //callBackAfterCreate
    sddbObj.callBackAfterCreate = function () { return $.Deferred().resolve(); };

    //callBackAfterEdit
    sddbObj.callBackAfterEdit = function (dbEntries) { return $.Deferred().resolve(); };

    //callBackBeforeCopy
    sddbObj.callBackBeforeCopy = function (dbEntries) { return $.Deferred().resolve(); };

    //callBackAfterCopy
    sddbObj.callBackAfterCopy = function (dbEntries) { return $.Deferred().resolve(); };

    //callBackBeforeSubmitEdit
    sddbObj.callBackBeforeSubmitEdit = function () { return $.Deferred().resolve(); };

    //callBackAfterSubmitEdit
    sddbObj.callBackAfterSubmitEdit = function (data) { return $.Deferred().resolve(); };

    //prepareFormForCreate
    sddbObj.prepareFormForCreate = function (callBackAfter) {

        callBackAfter = callBackAfter || sddbObj.callBackAfterCreate;

        var deferred0 = $.Deferred();

        cfg.currentIds = [];
        cfg.currentRecords = [];
        cfg.currentRecords[0] = $.extend(true, {}, cfg.recordTemplate);
        sddbObj.fillFormForCreateGeneric(cfg.editFormId, cfg.magicSuggests,
            cfg.labelTextCreate, cfg.mainViewId);
        callBackAfter()
            .done(function () {
                sddbObj.saveViewSettings(cfg.tableMain);
                sddbObj.switchView(cfg.mainViewId, cfg.editFormViewId, cfg.editFormBtnGroupCreateClass);
                deferred0.resolve();
            })
            .fail(deferred0.reject);

        return deferred0.promise();
    };

    //prepareFormForEdit
    sddbObj.prepareFormForEdit = function (callBackAfter) {

        callBackAfter = callBackAfter || sddbObj.callBackAfterEdit;

        var deferred0 = $.Deferred();

        cfg.currentIds = cfg.tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (cfg.currentIds.length === 0) {
            sddbObj.showModalNothingSelected();
            return deferred0.reject();
        }
        sddbObj.modalWaitWrapper(function () {
            return sddbObj.fillFormForEditGeneric(cfg.currentIds, cfg.httpTypeFillForEdit, cfg.urlFillForEdit,
                cfg.currentActive, cfg.editFormId, cfg.labelTextEdit, cfg.magicSuggests);
        })
            .then(function (dbEntries) {
                cfg.currentRecords = dbEntries;
                return callBackAfter(dbEntries);
            })
            .done(function () {
                sddbObj.saveViewSettings(cfg.tableMain);
                sddbObj.switchView(cfg.mainViewId, cfg.editFormViewId, cfg.editFormBtnGroupEditClass);
                deferred0.resolve();
            })
            .fail(deferred0.reject);

        return deferred0.promise();
    };

    //prepareFormForCopy
    sddbObj.prepareFormForCopy = function (callBackBefore, callBackAfter) {

        callBackBefore = callBackBefore || sddbObj.callBackBeforeCopy;
        callBackAfter = callBackAfter || sddbObj.callBackAfterCopy;

        var deferred0 = $.Deferred();

        cfg.currentIds = cfg.tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (cfg.currentIds.length !== 1) {
            sddbObj.showModalSelectOne();
            return deferred0.reject();
        }
        callBackBefore()
            .then(function () {
                return sddbObj.modalWaitWrapper(function () {
                    return sddbObj.fillFormForCopyGeneric(cfg.currentIds,
                        cfg.httpTypeFillForEdit, cfg.urlFillForEdit, cfg.currentActive,
                        cfg.editFormId, cfg.labelTextCopy, cfg.magicSuggests);
                });
            })
            .then(function (dbEntries) {
                cfg.currentIds = [];
                cfg.currentRecords = [];
                cfg.currentRecords[0] = $.extend(true, {}, cfg.recordTemplate);
                return callBackAfter(dbEntries);
            })
            .done(function () {
                sddbObj.saveViewSettings(cfg.tableMain);
                sddbObj.switchView(cfg.mainViewId, cfg.editFormViewId, cfg.editFormBtnGroupEditClass);
                deferred0.resolve();
            })
            .fail(deferred0.reject);

        return deferred0.promise();
    };

    //submitEditForm
    sddbObj.submitEditForm = function (callBackBefore, callBackAfter, doNotSwitchToMainView) {

        callBackBefore = callBackBefore || sddbObj.callBackBeforeSubmitEdit;
        callBackAfter = callBackAfter || sddbObj.callBackAfterSubmitEdit;

        var deferred0 = $.Deferred();

        sddbObj.msValidate(cfg.magicSuggests);
        if (!sddbObj.formIsValid(cfg.editFormId, cfg.currentIds.length === 0) ||
                !sddbObj.msIsValid(cfg.magicSuggests)) {
            sddbObj.showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
            return deferred0.reject();
        }
        callBackBefore()
            .then(function () {
                var createMultiple = $("#createMultiple").val() !== "" ? $("#createMultiple").val() : 1;
                return sddbObj.modalWaitWrapper(function () {
                    return sddbObj.submitEditsGeneric(cfg.editFormId, cfg.magicSuggests,
                        cfg.currentRecords, cfg.httpTypeEdit, cfg.urlEdit, createMultiple);
                });
            })
            .then(function (data, dbEntries) {
                cfg.currentRecords = dbEntries;
                if (cfg.currentIds.length === 0 && data.newEntryIds) {
                    cfg.currentIds = data.newEntryIds;
                    for (var i = 0; i < cfg.currentIds.length; i += 1) {
                        cfg.currentRecords[i].Id = cfg.currentIds[i];
                    }
                }
                return callBackAfter(data);
            })
            .then(function () { return sddbObj.refreshMainView(); })
            .done(function () {
                if (!doNotSwitchToMainView) {
                    sddbObj.switchView(cfg.editFormViewId, cfg.mainViewId,
                        cfg.mainViewBtnGroupClass, cfg.tableMain);
                }
                deferred0.resolve();
            })
            .fail(deferred0.reject);

        return deferred0.promise();
    };

    //cancelEditForm
    sddbObj.cancelEditForm = function () {
        sddbObj.switchView(cfg.editFormViewId, cfg.mainViewId, cfg.mainViewBtnGroupClass, cfg.tableMain);
    };

    //confirmAndDelete
    sddbObj.confirmAndDelete = function () {
        cfg.currentIds = cfg.tableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (cfg.currentIds.length === 0) {
            sddbObj.showModalNothingSelected();
            return;
        }
        sddbObj.showModalConfirm("Confirm deleting " + cfg.currentIds.length +
                " row(s).", "Confirm Delete", "no", "btn btn-danger")
            .done(sddbObj.deleteRecords);
    };

    //Delete Records from DB
    sddbObj.deleteRecords = function () {
        sddbObj.modalWaitWrapper(function () {
            return $.ajax({
                type: "POST",
                url: cfg.urlDelete,
                timeout: 120000,
                data: { ids: cfg.currentIds },
                dataType: "json"
            });
        })
            .done(function () {
                cfg.currentIds = [];
                cfg.currentRecords = [];
                sddbObj.refreshMainView();
            });
    };

    //refresh Main view 
    sddbObj.refreshMainView = function () {
        return sddbObj.modalWaitWrapper(function () {
            return sddbObj.refreshTableGeneric(cfg.tableMain, cfg.urlRefreshMainView,
                cfg.dataRefreshMainView(), cfg.httpTypeRefreshMainView);
        });
    };

    //---------------------------------------------------------------------------------------------------------------//

    return sddbObj;
};

