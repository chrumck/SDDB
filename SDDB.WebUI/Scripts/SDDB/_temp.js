/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../jquery.validate.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//pulls type information and formats form fields including validation reset
function updateFormForExtended(httpType, url, data, formId) {
    var deferred0 = $.Deferred();
    $.ajax({ type: httpType, url: url, data: data, timeout: 120000, dataType: "json" })
        .done(function (data) {
            var typeHasAttrs = false;
            var entityType = data[0];

            for (var prop in entityType) {
                if (prop.indexOf("Attr") == -1) { continue; }
                if (prop.indexOf("Type") != -1 && entityType[prop] != "NotUsed") { typeHasAttrs = true; }

                changeFormFieldDescriptionHelper(formId, entityType, prop);
                changeFormFieldValidationHelper(formId, entityType, prop);
            }
            $("#" + formId).removeData("validator");
            $("#" + formId).removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse("#" + formId)
            deferred0.resolve(typeHasAttrs);
        })
        .fail(function (xhr, status, error) { deferred0.reject(xhr, status, error); });
    return deferred0.promise();
}

//updateFormForExtendedWrp
function updateFormForExtendedWrp(httpType, url, data, formId) {
    var deferred0 = $.Deferred();
    showModalWait();
    updateViewsForTypeGeneric(httpType, url, data, formId)
        .always(hideModalWait)
        .done(deferred0.resolve)
        .fail(function (xhr, status, error) {
            showModalAJAXFail(xhr, status, error);
            deferred0.reject(xhr, status, error);
        });
    return deferred0.promise();
}

//changeFormFieldDescriptionHelper
function changeFormFieldDescriptionHelper(formId, entityType, prop) {
    if (prop.indexOf("Desc") != -1) {
        var attrName = prop.slice(prop.indexOf("Attr"), 6);
        $("#" + formId + " label[for=" + attrName + "]").text(entityType[prop]);
        $("#" + formId + " #" + attrName).prop("placeholder", entityType[prop]);
    }
}

//changeFormFieldValidationHelper
function changeFormFieldValidationHelper(formId, entityType, prop) {
    if (prop.indexOf("Type") != -1) {
        var attrName = prop.slice(prop.indexOf("Attr"), 6);
        var $targetEl = $("#" + formId + " #" + attrName);

        removeValidationAttributesHelper($targetEl);
        if ($targetEl.data("DateTimePicker")) { $targetEl.data("DateTimePicker").destroy(); }
        $("#" + formId + " #FrmGrp" + attrName).removeClass("hidden");

        if (entityType[prop] == "NotUsed") {
            $("#" + formId + " #FrmGrp" + attrName).addClass("hidden");
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
}

//removeValidationAttributesHelper
function removeValidationAttributesHelper($element) {
    var attrsToRemove = "";
    $.each($element[0].attributes, function (i, attrib) {
        if (typeof attrib !== "undefined" && attrib.name.indexOf("data-val") == 0) {
            attrsToRemove += (attrsToRemove == "") ? attrib.name : " " + attrib.name;
        }
    });
    $element.removeAttr(attrsToRemove);
}
