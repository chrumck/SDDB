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
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(true);
        TableMain.columns([11, 12, 13, 14, 15]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5]).visible(false);
        TableMain.columns([6, 7, 8, 9, 10]).visible(false);
        TableMain.columns([11, 12, 13, 14, 15]).visible(true);
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
        RefreshTable(TableMain, "/AssemblyDbSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
                    "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
    });

    //Initialize MagicSuggest MsFilterByModel
    MsFilterByModel = $("#MsFilterByModel").magicSuggest({
        data: "/AssemblyModelSrv/Lookup",
        maxSelection: 1,
        required: true,
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        },
        style: "min-width: 240px;"
    });
    $(MsFilterByModel).on('selectionchange', function (e, m) {
        var test = this.getValue().length;
        if (this.getValue().length == 0) {
            MsFilterByProject.disable(); MsFilterByProject.clear(true);
            $("#BtnEdit").prop("disabled", true);
            TableMain.clear().search("").draw();
        }
        else {
        RefreshTable(TableMain, "/AssemblyDbSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
            "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
        MsFilterByProject.enable();
        $("#BtnEdit").prop("disabled", false);
        }
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
        RefreshTable(TableMain, "/AssemblyDbSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
                    "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
    });

    //TableMain AssemblyDbs
    TableMain = $("#TableMain").DataTable({
        //ajax: {
        //    //url: "/AssemblyDbSrv/GetByModelIds",
        //    //type: "POST",
        //    //data: { getActive: ($("#ChBoxShowDeleted").prop("checked")) ? false : true }
        //},
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyName", name: "AssyName" },//1
            //------------------------------------------------first set of columns
            { data: "AssyAltName", name: "AssyAltName" },//2
            { data: "AssyTypeName", name: "AssyTypeName" },//3  
            { data: "AssyStatusName", name: "AssyStatusName" },//4
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
            //------------------------------------------------never visible
            { data: "AssemblyType_Id", name: "AssemblyType_Id" },//16
            { data: "AssemblyStatus_Id", name: "AssemblyStatus_Id" },//17
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//18
        ],
        columnDefs: [
            { targets: [0, 16, 17, 18], visible: false }, // - never show
            { targets: [0, 16, 17, 18], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 3, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [6, 7, 8, 9, 10], visible: false }, // - second set of columns - to toggle with options
            { targets: [7, 8], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [9, 10], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [11, 12, 13, 14, 15], visible: false }, // - third set of columns - to toggle with options
            { targets: [12, 13], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [14, 15], className: "hidden-xs hidden-sm hidden-md" } // - third set of columns
        ],
        order: [[1, "asc"]],
        bAutoWidth: false,
        dom: "<lf<t>p>",
        language: {
            search: "",
            lengthMenu: "_MENU_",
            paginate: { previous: "", next: "" }
        }
    });

    //---------------------------------------EditFormView----------------------------------------//

    //Enable modified field detection
    $(".modifiable").change(function () { $(this).data("ismodified", true); });

    //Initialize MagicSuggest Array
    AddToMSArray(MagicSuggests, "AssemblyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssemblyStatus_Id", "/AssemblyStatusSrv/Lookup", 1);
    AddToMSArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        IsCreate = false;
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        MsValidate(MagicSuggests);
        if (FormIsValid("EditForm", IsCreate) && MsIsValid(MagicSuggests)) SubmitEdits();
    });


});

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var IsCreate = false;
var MsFilterByProject = {};
var MsFilterByModel = {};
var MagicSuggests = [];
var CurrRecord = {};

//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    
    $.ajax({
        type: "POST", url: "/AssemblyDbSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
         
            CurrRecord.AssyName = data[0].AssyName;
            CurrRecord.AssyAltName = data[0].AssyAltName;
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
            CurrRecord.AssemblyType_Id = data[0].AssemblyType_Id;
            CurrRecord.AssemblyStatus_Id = data[0].AssemblyStatus_Id;
            CurrRecord.AssignedToProject_Id = data[0].AssignedToProject_Id;
            
            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.AssyName != dbEntry.AssyName) FormInput.AssyName = "_VARIES_";
                if (FormInput.AssyAltName != dbEntry.AssyAltName) FormInput.AssyAltName = "_VARIES_";
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

                if (FormInput.AssemblyType_Id != dbEntry.AssemblyType_Id) { FormInput.AssemblyType_Id = "_VARIES_"; FormInput.AssyTypeName = "_VARIES_"; }
                else FormInput.AssyTypeName = dbEntry.AssyTypeName;
                if (FormInput.AssemblyStatus_Id != dbEntry.AssemblyStatus_Id) { FormInput.AssemblyStatus_Id = "_VARIES_"; FormInput.AssyStatusName = "_VARIES_"; }
                else FormInput.AssyStatusName = dbEntry.AssyStatusName;
                if (FormInput.AssignedToProject_Id != dbEntry.AssignedToProject_Id) { FormInput.AssignedToProject_Id = "_VARIES_"; FormInput.AssignedToProject = "_VARIES_"; }
                else FormInput.AssignedToProject = dbEntry.AssignedToProject.ProjectName + " " + dbEntry.AssignedToProject.ProjectCode;
            });

            ClearFormInputs("EditForm", MagicSuggests);

            $("#AssyName").val(FormInput.AssyName);
            $("#AssyAltName").val(FormInput.AssyAltName);
            $("#AssemblyExt_Attr01").val(FormInput.Attr01);
            $("#AssemblyExt_Attr02").val(FormInput.Attr02);
            $("#AssemblyExt_Attr03").val(FormInput.Attr03);
            $("#AssemblyExt_Attr04").val(FormInput.Attr04);
            $("#AssemblyExt_Attr05").val(FormInput.Attr05);
            $("#AssemblyExt_Attr06").val(FormInput.Attr06);
            $("#AssemblyExt_Attr07").val(FormInput.Attr07);
            $("#AssemblyExt_Attr08").val(FormInput.Attr08);
            $("#AssemblyExt_Attr09").val(FormInput.Attr09);
            $("#AssemblyExt_Attr10").val(FormInput.Attr10);

            if (FormInput.AssemblyType_Id != null) MagicSuggests[0].addToSelection([{ id: FormInput.AssemblyType_Id, name: FormInput.AssyTypeName }], true);
            if (FormInput.AssemblyStatus_Id != null) MagicSuggests[1].addToSelection([{ id: FormInput.AssemblyStatus_Id, name: FormInput.AssyStatusName }], true);
            if (FormInput.AssignedToProject_Id != null) MagicSuggests[2].addToSelection([{ id: FormInput.AssignedToProject_Id, name: FormInput.AssignedToProject }], true);

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                DisableUniqueMs(MagicSuggests, false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
                DisableUniqueMs(MagicSuggests, true);
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

    $.each(MagicSuggests, function (i, ms) {
        if (ms.isModified == true) modifiedProperties.push(ms.id);
    });

    var editRecords = [];
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();

    var magicResults = [];
    $.each(MagicSuggests, function (i, ms) {
        var msValue = (ms.getSelection().length != 0) ? (ms.getSelection())[0].id : null;
        magicResults.push(msValue);
    });

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;
               
        editRecord.AssyName = ($("#AssyName").data("ismodified")) ? $("#AssyName").val() : CurrRecord.AssyName;
        editRecord.AssyAltName = ($("#AssyAltName").data("ismodified")) ? $("#AssyAltName").val() : CurrRecord.AssyAltName;
        editRecord.AssemblyExt = {};
        editRecord.AssemblyExt.Attr01 = ($("#AssemblyExt_Attr01").data("ismodified")) ? $("#AssemblyExt_Attr01").val() : CurrRecord.Attr01;
        editRecord.AssemblyExt.Attr02 = ($("#AssemblyExt_Attr02").data("ismodified")) ? $("#AssemblyExt_Attr02").val() : CurrRecord.Attr02;
        editRecord.AssemblyExt.Attr03 = ($("#AssemblyExt_Attr03").data("ismodified")) ? $("#AssemblyExt_Attr03").val() : CurrRecord.Attr03;
        editRecord.AssemblyExt.Attr04 = ($("#AssemblyExt_Attr04").data("ismodified")) ? $("#AssemblyExt_Attr04").val() : CurrRecord.Attr04;
        editRecord.AssemblyExt.Attr05 = ($("#AssemblyExt_Attr05").data("ismodified")) ? $("#AssemblyExt_Attr05").val() : CurrRecord.Attr05;
        editRecord.AssemblyExt.Attr06 = ($("#AssemblyExt_Attr06").data("ismodified")) ? $("#AssemblyExt_Attr06").val() : CurrRecord.Attr06;
        editRecord.AssemblyExt.Attr07 = ($("#AssemblyExt_Attr07").data("ismodified")) ? $("#AssemblyExt_Attr07").val() : CurrRecord.Attr07;
        editRecord.AssemblyExt.Attr08 = ($("#AssemblyExt_Attr08").data("ismodified")) ? $("#AssemblyExt_Attr08").val() : CurrRecord.Attr08;
        editRecord.AssemblyExt.Attr09 = ($("#AssemblyExt_Attr09").data("ismodified")) ? $("#AssemblyExt_Attr09").val() : CurrRecord.Attr09;
        editRecord.AssemblyExt.Attr10 = ($("#AssemblyExt_Attr10").data("ismodified")) ? $("#AssemblyExt_Attr10").val() : CurrRecord.Attr10;

        editRecord.AssemblyType_Id = (MagicSuggests[0].isModified) ? magicResults[0] : CurrRecord.AssemblyType_Id;
        editRecord.AssemblyStatus_Id = (MagicSuggests[1].isModified) ? magicResults[1] : CurrRecord.AssemblyStatus_Id;
        editRecord.AssignedToProject_Id = (MagicSuggests[2].isModified) ? magicResults[2] : CurrRecord.AssignedToProject_Id;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/AssemblyDbSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            RefreshTable(TableMain, "/AssemblyDbSrv/GetByModelIds", ($("#ChBoxShowDeleted").prop("checked") ? false : true),
                "POST", MsFilterByProject.getValue(), MsFilterByModel.getValue());
            IsCreate = false;
            $("#MainView").removeClass("hide");
            $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) {
            ShowModalAJAXFail(xhr, status, error);
        });
}

//---------------------------------------Helper Methods--------------------------------------//


