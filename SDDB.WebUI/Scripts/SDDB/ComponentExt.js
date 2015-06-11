/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.3.js" />
/// <reference path="../jquery-2.1.3.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        var selectedRows = TableMain.rows(".ui-selected").data();
        if (selectedRows.length == 0) ShowModalNothingSelected();
        else FillFormForEdit();
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(true);
        TableMain.columns([6, 7, 8, 9, 10]).visible(false);
        TableMain.columns([11, 12, 13, 14, 15]).visible(false);
        TableMain.columns([16, 17, 18, 19, 20]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(true);
        TableMain.columns([11, 12, 13, 14, 15]).visible(false);
        TableMain.columns([16, 17, 18, 19, 20]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(false);
        TableMain.columns([11, 12, 13, 14, 15]).visible(true);
        TableMain.columns([16, 17, 18, 19, 20]).visible(false);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(false);
        TableMain.columns([11, 12, 13, 14, 15]).visible(false);
        TableMain.columns([16, 17, 18, 19, 20]).visible(true);
    });

    //Initialize MagicSuggest MsFilterByModel
    MsFilterByModel = $("#MsFilterByModel").magicSuggest({
        data: "/ComponentModelSrv/Lookup",
        maxSelection: 1,
        required: true,
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });

    //wire up MsFilterByModel event selectionchange
    $(MsFilterByModel).on('selectionchange', function (e, m) {
        if (this.getValue().length == 0) {
            MsFilterByProject.disable(); MsFilterByProject.clear(true);
            $("#ChBoxShowDeleted").bootstrapToggle("disable")
            TableMain.clear().search("").draw();
        }
        else {
            RefreshTable(TableMain, "/ComponentSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
                "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
            MsFilterByProject.enable();
            $("#ChBoxShowDeleted").bootstrapToggle("enable")
            $("#EditFormLabel").text("Edit " + MsFilterByModel.getSelection()[0].name);

            UpdateViewsForModel();
        }
    });

    //Initialize MagicSuggest msFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        disabled: true,
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByProject).on('selectionchange', function (e, m) {
        RefreshTable(TableMain, "/ComponentSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
                    "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
    });


    //---------------------------------------DataTables------------

    //Enable jqueryUI selectable
    if (!Modernizr.touch) {
        $(".selectable").selectable({ filter: "tr" });
    }
    else {
        $(".selectable").on("click", "tr", function () { $(this).toggleClass("ui-selected"); });
    }

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (($(this).prop("checked")) ? false : true)
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        else $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        RefreshTable(TableMain, "/ComponentSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
                    "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
    });

    //TableMain Components
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "CompName", name: "CompName" },//1
            //------------------------------------------------first set of columns
            { data: "CompAltName", name: "CompAltName" },//2
            { data: "CompTypeName", name: "CompTypeName" },//3  
            { data: "CompStatusName", name: "CompStatusName" },//4
            { data: "AssignedToProject", render: function (data, type, full, meta) { return data.ProjectName + " " + data.ProjectCode }, name: "AssignedToProject" }, //5
            //------------------------------------------------second set of columns
            { data: "Attr01", name: "Attr01" },//6
            { data: "Attr02", name: "Attr02" },//7
            { data: "Attr03", name: "Attr03" },//8
            { data: "Attr04", name: "Attr04" },//9
            { data: "Attr05", name: "Attr05" },//10
            //------------------------------------------------third set of columns
            { data: "Attr06", name: "Attr06" },//11
            { data: "Attr07", name: "Attr07" },//12
            { data: "Attr08", name: "Attr08" },//13
            { data: "Attr09", name: "Attr09" },//14
            { data: "Attr10", name: "Attr10" },//15
            //------------------------------------------------fourth set of columns
            { data: "Attr11", name: "Attr11" },//16
            { data: "Attr12", name: "Attr12" },//17
            { data: "Attr13", name: "Attr13" },//18
            { data: "Attr14", name: "Attr14" },//19
            { data: "Attr15", name: "Attr15" },//20
            //------------------------------------------------never visible
            { data: "ComponentType_Id", name: "ComponentType_Id" },//21
            { data: "ComponentStatus_Id", name: "ComponentStatus_Id" },//22
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//23
        ],
        columnDefs: [
            { targets: [0, 21, 22, 23], visible: false }, // - never show
            { targets: [0, 21, 22, 23], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 3, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [6, 7, 8, 9, 10], visible: false }, // - second set of columns - to toggle with options
            { targets: [7, 8], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [9, 10], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [11, 12, 13, 14, 15], visible: false }, // - third set of columns - to toggle with options
            { targets: [12, 13], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [14, 15], className: "hidden-xs hidden-sm hidden-md" }, // - third set of columns

            { targets: [16, 17, 18, 19, 20], visible: false }, // - fourth set of columns - to toggle with options
            { targets: [17, 18], className: "hidden-xs hidden-sm" }, // - fourth set of columns
            { targets: [19, 20], className: "hidden-xs hidden-sm hidden-md" } // - fourth set of columns
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

    //Enable modified field detection
    $(".modifiable").change(function () { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        if (FormIsValid("EditForm", false) ) SubmitEdits();
    });


});

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var MsFilterByProject = {}; var MsFilterByModel = {};
var CurrRecord = {};
var DatePickers = [];

//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    
    $.ajax({
        type: "POST", url: "/ComponentSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
         
            CurrRecord.CompName = data[0].CompName;
            CurrRecord.CompAltName = data[0].CompAltName;
            CurrRecord.Attr01 = data[0].Attr01;
            CurrRecord.Attr02 = data[0].Attr02;
            CurrRecord.Attr03 = data[0].Attr03;
            CurrRecord.Attr04 = data[0].Attr04;
            CurrRecord.Attr05 = data[0].Attr05;
            CurrRecord.Attr06 = data[0].Attr06;
            CurrRecord.Attr07 = data[0].Attr07;
            CurrRecord.Attr08 = data[0].Attr08;
            CurrRecord.Attr09 = data[0].Attr09;
            CurrRecord.Attr10 = data[0].Attr10;
            CurrRecord.Attr11 = data[0].Attr11;
            CurrRecord.Attr12 = data[0].Attr12;
            CurrRecord.Attr13 = data[0].Attr13;
            CurrRecord.Attr14 = data[0].Attr14;
            CurrRecord.Attr15 = data[0].Attr15;
            CurrRecord.ComponentType_Id = data[0].ComponentType_Id;
            CurrRecord.ComponentStatus_Id = data[0].ComponentStatus_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.CompName != dbEntry.CompName) FormInput.CompName = "_VARIES_";
                if (FormInput.CompAltName != dbEntry.CompAltName) FormInput.CompAltName = "_VARIES_";
                if (FormInput.Attr01 != dbEntry.Attr01) FormInput.Attr01 = "_VARIES_";
                if (FormInput.Attr02 != dbEntry.Attr02) FormInput.Attr02 = "_VARIES_";
                if (FormInput.Attr03 != dbEntry.Attr03) FormInput.Attr03 = "_VARIES_";
                if (FormInput.Attr04 != dbEntry.Attr04) FormInput.Attr04 = "_VARIES_";
                if (FormInput.Attr05 != dbEntry.Attr05) FormInput.Attr05 = "_VARIES_";
                if (FormInput.Attr06 != dbEntry.Attr06) FormInput.Attr06 = "_VARIES_";
                if (FormInput.Attr07 != dbEntry.Attr07) FormInput.Attr07 = "_VARIES_";
                if (FormInput.Attr08 != dbEntry.Attr08) FormInput.Attr08 = "_VARIES_";
                if (FormInput.Attr09 != dbEntry.Attr09) FormInput.Attr09 = "_VARIES_";
                if (FormInput.Attr10 != dbEntry.Attr10) FormInput.Attr10 = "_VARIES_";
                if (FormInput.Attr11 != dbEntry.Attr11) FormInput.Attr11 = "_VARIES_";
                if (FormInput.Attr12 != dbEntry.Attr12) FormInput.Attr12 = "_VARIES_";
                if (FormInput.Attr13 != dbEntry.Attr13) FormInput.Attr13 = "_VARIES_";
                if (FormInput.Attr14 != dbEntry.Attr14) FormInput.Attr14 = "_VARIES_";
                if (FormInput.Attr15 != dbEntry.Attr15) FormInput.Attr15 = "_VARIES_";

                if (FormInput.ComponentType_Id != dbEntry.ComponentType_Id) { FormInput.ComponentType_Id = "_VARIES_"; FormInput.CompTypeName = "_VARIES_"; }
                else FormInput.CompTypeName = dbEntry.CompTypeName;
                if (FormInput.ComponentStatus_Id != dbEntry.ComponentStatus_Id) { FormInput.ComponentStatus_Id = "_VARIES_"; FormInput.CompStatusName = "_VARIES_"; }
                else FormInput.CompStatusName = dbEntry.CompStatusName;
                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
            });

            ClearFormInputs("EditForm");


            $("#CompName").val(FormInput.CompName);
            $("#CompAltName").val(FormInput.CompAltName);
            $("#CompTypeName").val(FormInput.CompTypeName);
            $("#CompStatusName").val(FormInput.CompStatusName);
            $("#AssignedToProject").val(FormInput.AssignedToProject);
            $("#Attr01").val(FormInput.Attr01);
            $("#Attr02").val(FormInput.Attr02);
            $("#Attr03").val(FormInput.Attr03);
            $("#Attr04").val(FormInput.Attr04);
            $("#Attr05").val(FormInput.Attr05);
            $("#Attr06").val(FormInput.Attr06);
            $("#Attr07").val(FormInput.Attr07);
            $("#Attr08").val(FormInput.Attr08);
            $("#Attr09").val(FormInput.Attr09);
            $("#Attr10").val(FormInput.Attr10);
            $("#Attr11").val(FormInput.Attr11);
            $("#Attr12").val(FormInput.Attr12);
            $("#Attr13").val(FormInput.Attr13);
            $("#Attr14").val(FormInput.Attr14);
            $("#Attr15").val(FormInput.Attr15);

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
            }

            $("#MainView").addClass("hide");
            $("#EditFormView").removeClass("hide");
        })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//SubmitEdits to DB
function SubmitEdits() {

    var modifiedProperties = [];
    $(".modifiable").each(function (index) {
        if ($(this).data("ismodified")) modifiedProperties.push($(this).prop("id"));
    });

    var editRecords = [];
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;
               
        editRecord.Attr01 = ($("#Attr01").data("ismodified")) ? $("#Attr01").val() : CurrRecord.Attr01;
        editRecord.Attr02 = ($("#Attr02").data("ismodified")) ? $("#Attr02").val() : CurrRecord.Attr02;
        editRecord.Attr03 = ($("#Attr03").data("ismodified")) ? $("#Attr03").val() : CurrRecord.Attr03;
        editRecord.Attr04 = ($("#Attr04").data("ismodified")) ? $("#Attr04").val() : CurrRecord.Attr04;
        editRecord.Attr05 = ($("#Attr05").data("ismodified")) ? $("#Attr05").val() : CurrRecord.Attr05;
        editRecord.Attr06 = ($("#Attr06").data("ismodified")) ? $("#Attr06").val() : CurrRecord.Attr06;
        editRecord.Attr07 = ($("#Attr07").data("ismodified")) ? $("#Attr07").val() : CurrRecord.Attr07;
        editRecord.Attr08 = ($("#Attr08").data("ismodified")) ? $("#Attr08").val() : CurrRecord.Attr08;
        editRecord.Attr09 = ($("#Attr09").data("ismodified")) ? $("#Attr09").val() : CurrRecord.Attr09;
        editRecord.Attr10 = ($("#Attr10").data("ismodified")) ? $("#Attr10").val() : CurrRecord.Attr10;
        editRecord.Attr11 = ($("#Attr11").data("ismodified")) ? $("#Attr11").val() : CurrRecord.Attr11;
        editRecord.Attr12 = ($("#Attr12").data("ismodified")) ? $("#Attr12").val() : CurrRecord.Attr12;
        editRecord.Attr13 = ($("#Attr13").data("ismodified")) ? $("#Attr13").val() : CurrRecord.Attr13;
        editRecord.Attr14 = ($("#Attr14").data("ismodified")) ? $("#Attr14").val() : CurrRecord.Attr14;
        editRecord.Attr15 = ($("#Attr15").data("ismodified")) ? $("#Attr15").val() : CurrRecord.Attr15;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/ComponentSrv/EditExt", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            RefreshTable(TableMain, "/ComponentSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
                "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
            $("#MainView").removeClass("hide");
            $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });
}

//Pulls Model Information and formats edit form and column names
function UpdateViewsForModel() {
    var modelId = MsFilterByModel.getValue();
    var dataString = { val:"true", valLength:"The field must be a string with a maximum length of 255.", valLengthMax: "255" };
    $.ajax({
        type: "POST", url: "/ComponentModelSrv/GetByIds", timeout: 20000, data: { ids: [modelId] }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            for (var prop in data[0]) {
                if (prop.indexOf("Attr") != -1 && prop.indexOf("Desc") != -1) {

                    var attrName = prop.slice(prop.indexOf("Attr"), 6);

                    $(TableMain.column(attrName + ":name").header()).text(data[0][prop]);
                    $("label[for=" + attrName + "]").text(data[0][prop]);
                    $("#" + attrName).prop("placeholder",data[0][prop]);
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

                    var picker = $targetEl.data("DateTimePicker"); if (typeof picker !== "undefined") picker.destroy();

                    $("#FrmGrp" + attrName).removeClass("hide");

                    switch (data[0][prop]) {
                        case "NotUsed":
                            $("#FrmGrp" + attrName).addClass("hide");
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
            $("#EditForm").removeData("validator"); $("#EditForm").removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse("#EditForm")
        })
        .fail(function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); });

}

//---------------------------------------Helper Methods--------------------------------------//


