/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.3.js" />
/// <reference path="../jquery-2.1.3.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var IsCreate = false;
var MagicSuggests = [];
var CurrRecord = {};


$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        IsCreate = true;
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Assembly Model", "MainView");
        $("#EditForm select").find("option:first").prop('selected', 'selected');
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        var selectedRows = TableMain.rows(".ui-selected").data();
        if (selectedRows.length == 0) showModalNothingSelected();
        else { IsCreate = false; FillFormForEdit(); }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows == 0) showModalNothingSelected();
        else showModalDelete(noOfRows);
    });


    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(true);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(true);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(true);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId4
    $("#dropdownId4").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(true);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(false);
    });

    //wire up dropdownId5
    $("#dropdownId5").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7, 8]).visible(false);
        TableMain.columns([9, 10, 11, 12, 13, 14]).visible(false);
        TableMain.columns([15, 16, 17, 18, 19, 20]).visible(false);
        TableMain.columns([21, 22, 23, 24, 25, 26]).visible(false);
        TableMain.columns([27, 28, 29, 30, 31, 32]).visible(true);
    });
    
    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (($(this).prop("checked")) ? false : true)
            $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary");
        else $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger");
        refreshTable(TableMain, "/AssemblyModelSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
    });

    //TableMain Assembly Models
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "AssyModelName", name: "AssyModelName" },//1
            { data: "AssyModelAltName", name: "AssyModelAltName" },//2
            //------------------------------------------------first set of columns
            { data: "Attr01Type", name: "Attr01Type" },//3
            { data: "Attr01Desc", name: "Attr01Desc" },//4
            { data: "Attr02Type", name: "Attr02Type" },//5
            { data: "Attr02Desc", name: "Attr02Desc" },//6
            { data: "Attr03Type", name: "Attr03Type" },//7
            { data: "Attr03Desc", name: "Attr03Desc" },//8
            //------------------------------------------------second set of columns
            { data: "Attr04Type", name: "Attr04Type" },//9
            { data: "Attr04Desc", name: "Attr04Desc" },//10
            { data: "Attr05Type", name: "Attr05Type" },//11
            { data: "Attr05Desc", name: "Attr05Desc" },//12
            { data: "Attr06Type", name: "Attr06Type" },//13
            { data: "Attr06Desc", name: "Attr06Desc" },//14
            //------------------------------------------------third set of columns
            { data: "Attr07Type", name: "Attr07Type" },//15
            { data: "Attr07Desc", name: "Attr07Desc" },//16
            { data: "Attr08Type", name: "Attr08Type" },//17
            { data: "Attr08Desc", name: "Attr08Desc" },//18
            { data: "Attr09Type", name: "Attr09Type" },//19
            { data: "Attr09Desc", name: "Attr09Desc" },//20
            //------------------------------------------------fourth set of columns
            { data: "Attr10Type", name: "Attr10Type" },//21
            { data: "Attr10Desc", name: "Attr10Desc" },//22
            { data: "Attr11Type", name: "Attr11Type" },//23
            { data: "Attr11Desc", name: "Attr11Desc" },//24
            { data: "Attr12Type", name: "Attr12Type" },//25
            { data: "Attr12Desc", name: "Attr12Desc" },//26
            //------------------------------------------------fifth set of columns
            { data: "Attr13Type", name: "Attr13Type" },//27
            { data: "Attr13Desc", name: "Attr13Desc" },//28
            { data: "Attr14Type", name: "Attr14Type" },//29
            { data: "Attr14Desc", name: "Attr14Desc" },//30
            { data: "Attr15Type", name: "Attr15Type" },//31
            { data: "Attr15Desc", name: "Attr15Desc" },//32
            //------------------------------------------------never visible
            { data: "Comments", name: "Comments" },//33
            { data: "IsActive", name: "IsActive" },//34
        ],
        columnDefs: [
            { targets: [0, 34], visible: false }, // - never show
            { targets: [0, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 34], searchable: false },  //"orderable": false, "visible": false
            { targets: [3, 4, 5, 6], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7, 8], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [9, 10, 11, 12, 13, 14], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 10, 11, 12], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [13, 14], className: "hidden-xs hidden-sm hidden-md" }, // - second set of columns

            { targets: [15, 16, 17, 18, 19, 20], visible: false }, // - third set of columns - to toggle with options
            { targets: [15, 16, 17, 18], className: "hidden-xs hidden-sm" }, // - third set of columns
            { targets: [19, 20], className: "hidden-xs hidden-sm hidden-md" }, // - third set of columns

            { targets: [21, 22, 23, 24, 25, 26], visible: false }, // - fourth set of columns - to toggle with options
            { targets: [21, 22, 23, 24], className: "hidden-xs hidden-sm" }, // - fourth set of columns
            { targets: [25, 26], className: "hidden-xs hidden-sm hidden-md" }, // - fourth set of columns

            { targets: [27, 28, 29, 30, 31, 32], visible: false }, // - fifth set of columns - to toggle with options
            { targets: [27, 28, 29, 30], className: "hidden-xs hidden-sm" }, // - fifth set of columns
            { targets: [31, 32], className: "hidden-xs hidden-sm hidden-md" } // - fifth set of columns
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

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        IsCreate = false;
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", IsCreate) && msIsValid(MagicSuggests)) SubmitEdits();
    });


    //--------------------------------------View Initialization------------------------------------//

    refreshTable(TableMain, "/AssemblyModelSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));


    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//FillFormForEdit
function FillFormForEdit() {
    if ($("#ChBoxShowDeleted").prop("checked")) $("#EditFormGroupIsActive").removeClass("hide");
    else $("#EditFormGroupIsActive").addClass("hide");

    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/AssemblyModelSrv/GetByIds", timeout: 20000,
        data: { ids: ids, getActive: (($("#ChBoxShowDeleted").prop("checked")) ? false : true) }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {

            CurrRecord.AssyModelName = data[0].AssyModelName;
            CurrRecord.AssyModelAltName = data[0].AssyModelAltName;
            CurrRecord.Attr01Type = data[0].Attr01Type;CurrRecord.Attr01Desc = data[0].Attr01Desc;
            CurrRecord.Attr02Type = data[0].Attr02Type;CurrRecord.Attr02Desc = data[0].Attr02Desc;
            CurrRecord.Attr03Type = data[0].Attr03Type;CurrRecord.Attr03Desc = data[0].Attr03Desc;
            CurrRecord.Attr04Type = data[0].Attr04Type;CurrRecord.Attr04Desc = data[0].Attr04Desc;
            CurrRecord.Attr05Type = data[0].Attr05Type;CurrRecord.Attr05Desc = data[0].Attr05Desc;
            CurrRecord.Attr06Type = data[0].Attr06Type;CurrRecord.Attr06Desc = data[0].Attr06Desc;
            CurrRecord.Attr07Type = data[0].Attr07Type;CurrRecord.Attr07Desc = data[0].Attr07Desc;
            CurrRecord.Attr08Type = data[0].Attr08Type;CurrRecord.Attr08Desc = data[0].Attr08Desc;
            CurrRecord.Attr09Type = data[0].Attr09Type;CurrRecord.Attr09Desc = data[0].Attr09Desc;
            CurrRecord.Attr10Type = data[0].Attr10Type;CurrRecord.Attr10Desc = data[0].Attr10Desc;
            CurrRecord.Attr11Type = data[0].Attr11Type;CurrRecord.Attr11Desc = data[0].Attr11Desc;
            CurrRecord.Attr12Type = data[0].Attr12Type;CurrRecord.Attr12Desc = data[0].Attr12Desc;
            CurrRecord.Attr13Type = data[0].Attr13Type;CurrRecord.Attr13Desc = data[0].Attr13Desc;
            CurrRecord.Attr14Type = data[0].Attr14Type;CurrRecord.Attr14Desc = data[0].Attr14Desc;
            CurrRecord.Attr15Type = data[0].Attr15Type;CurrRecord.Attr15Desc = data[0].Attr15Desc;
            CurrRecord.Comments = data[0].Comments;
            CurrRecord.IsActive = data[0].IsActive;

            var FormInput = $.extend(true, {}, CurrRecord);
            $.each(data, function (i, dbEntry) {
                if (FormInput.AssyModelName != dbEntry.AssyModelName) FormInput.AssyModelName = "_VARIES_";
                if (FormInput.AssyModelAltName != dbEntry.AssyModelAltName) FormInput.AssyModelAltName = "_VARIES_";

                if (FormInput.Attr01Type != dbEntry.Attr01Type) FormInput.Attr01Type = "_VARIES_";
                if (FormInput.Attr01Desc != dbEntry.Attr01Desc) FormInput.Attr01Desc = "_VARIES_";
                if (FormInput.Attr02Type != dbEntry.Attr02Type) FormInput.Attr02Type = "_VARIES_";
                if (FormInput.Attr02Desc != dbEntry.Attr02Desc) FormInput.Attr02Desc = "_VARIES_";
                if (FormInput.Attr03Type != dbEntry.Attr03Type) FormInput.Attr03Type = "_VARIES_";
                if (FormInput.Attr03Desc != dbEntry.Attr03Desc) FormInput.Attr03Desc = "_VARIES_";
                if (FormInput.Attr04Type != dbEntry.Attr04Type) FormInput.Attr04Type = "_VARIES_";
                if (FormInput.Attr04Desc != dbEntry.Attr04Desc) FormInput.Attr04Desc = "_VARIES_";
                if (FormInput.Attr05Type != dbEntry.Attr05Type) FormInput.Attr05Type = "_VARIES_";
                if (FormInput.Attr05Desc != dbEntry.Attr05Desc) FormInput.Attr05Desc = "_VARIES_";
                if (FormInput.Attr06Type != dbEntry.Attr06Type) FormInput.Attr06Type = "_VARIES_";
                if (FormInput.Attr06Desc != dbEntry.Attr06Desc) FormInput.Attr06Desc = "_VARIES_";
                if (FormInput.Attr07Type != dbEntry.Attr07Type) FormInput.Attr07Type = "_VARIES_";
                if (FormInput.Attr07Desc != dbEntry.Attr07Desc) FormInput.Attr07Desc = "_VARIES_";
                if (FormInput.Attr08Type != dbEntry.Attr08Type) FormInput.Attr08Type = "_VARIES_";
                if (FormInput.Attr08Desc != dbEntry.Attr08Desc) FormInput.Attr08Desc = "_VARIES_";
                if (FormInput.Attr09Type != dbEntry.Attr09Type) FormInput.Attr09Type = "_VARIES_";
                if (FormInput.Attr09Desc != dbEntry.Attr09Desc) FormInput.Attr09Desc = "_VARIES_";
                if (FormInput.Attr10Type != dbEntry.Attr10Type) FormInput.Attr10Type = "_VARIES_";
                if (FormInput.Attr10Desc != dbEntry.Attr10Desc) FormInput.Attr10Desc = "_VARIES_";
                if (FormInput.Attr11Type != dbEntry.Attr11Type) FormInput.Attr11Type = "_VARIES_";
                if (FormInput.Attr11Desc != dbEntry.Attr11Desc) FormInput.Attr11Desc = "_VARIES_";
                if (FormInput.Attr12Type != dbEntry.Attr12Type) FormInput.Attr12Type = "_VARIES_";
                if (FormInput.Attr12Desc != dbEntry.Attr12Desc) FormInput.Attr12Desc = "_VARIES_";
                if (FormInput.Attr13Type != dbEntry.Attr13Type) FormInput.Attr13Type = "_VARIES_";
                if (FormInput.Attr13Desc != dbEntry.Attr13Desc) FormInput.Attr13Desc = "_VARIES_";
                if (FormInput.Attr14Type != dbEntry.Attr14Type) FormInput.Attr14Type = "_VARIES_";
                if (FormInput.Attr14Desc != dbEntry.Attr14Desc) FormInput.Attr14Desc = "_VARIES_";
                if (FormInput.Attr15Type != dbEntry.Attr15Type) FormInput.Attr15Type = "_VARIES_";
                if (FormInput.Attr15Desc != dbEntry.Attr15Desc) FormInput.Attr15Desc = "_VARIES_";

                if (FormInput.Comments != dbEntry.Comments) FormInput.Comments = "_VARIES_";
                if (FormInput.IsActive != dbEntry.IsActive) FormInput.IsActive = "_VARIES_";
            });

            clearFormInputs("EditForm", MagicSuggests);
            $("#EditFormLabel").text("Edit Assembly Model");

            $("#AssyModelName").val(FormInput.AssyModelName);
            $("#AssyModelAltName").val(FormInput.AssyModelAltName);

            $("#Attr01Type").val(FormInput.Attr01Type); $("#Attr01Desc").val(FormInput.Attr01Desc);
            $("#Attr02Type").val(FormInput.Attr02Type); $("#Attr02Desc").val(FormInput.Attr02Desc);
            $("#Attr03Type").val(FormInput.Attr03Type); $("#Attr03Desc").val(FormInput.Attr03Desc);
            $("#Attr04Type").val(FormInput.Attr04Type); $("#Attr04Desc").val(FormInput.Attr04Desc);
            $("#Attr05Type").val(FormInput.Attr05Type); $("#Attr05Desc").val(FormInput.Attr05Desc);
            $("#Attr06Type").val(FormInput.Attr06Type); $("#Attr06Desc").val(FormInput.Attr06Desc);
            $("#Attr07Type").val(FormInput.Attr07Type); $("#Attr07Desc").val(FormInput.Attr07Desc);
            $("#Attr08Type").val(FormInput.Attr08Type); $("#Attr08Desc").val(FormInput.Attr08Desc);
            $("#Attr09Type").val(FormInput.Attr09Type); $("#Attr09Desc").val(FormInput.Attr09Desc);
            $("#Attr10Type").val(FormInput.Attr10Type); $("#Attr10Desc").val(FormInput.Attr10Desc);
            $("#Attr11Type").val(FormInput.Attr11Type); $("#Attr11Desc").val(FormInput.Attr11Desc);
            $("#Attr12Type").val(FormInput.Attr12Type); $("#Attr12Desc").val(FormInput.Attr12Desc);
            $("#Attr13Type").val(FormInput.Attr13Type); $("#Attr13Desc").val(FormInput.Attr13Desc);
            $("#Attr14Type").val(FormInput.Attr14Type); $("#Attr14Desc").val(FormInput.Attr14Desc);
            $("#Attr15Type").val(FormInput.Attr15Type); $("#Attr15Desc").val(FormInput.Attr15Desc);
            $("#Comments").val(FormInput.Comments);
            if (FormInput.IsActive == true) $("#IsActive").prop("checked", true);

            if (data.length == 1) {
                $("[data-val-dbisunique]").prop("disabled", false);
                disableUniqueMs(MagicSuggests, false);
            }
            else {
                $("[data-val-dbisunique]").prop("disabled", true);
                disableUniqueMs(MagicSuggests, true);
            }

            $("#MainView").addClass("hide");
            $("#EditFormView").removeClass("hide");
        })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
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

    var magicResults = [];
    $.each(MagicSuggests, function (i, ms) {
        var msValue = (ms.getSelection().length != 0) ? (ms.getSelection())[0].id : null;
        magicResults.push(msValue);
    });

    var editRecords = [];
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    if (IsCreate == true) ids = ["newEntryId"];

    $.each(ids, function (i, id) {
        var editRecord = {};
        editRecord.Id = id;

        editRecord.AssyModelName = ($("#AssyModelName").data("ismodified")) ? $("#AssyModelName").val() : CurrRecord.AssyModelName;
        editRecord.AssyModelAltName = ($("#AssyModelAltName").data("ismodified")) ? $("#AssyModelAltName").val() : CurrRecord.AssyModelAltName;
        editRecord.Attr01Type = ($("#Attr01Type").data("ismodified")) ? $("#Attr01Type").val() : CurrRecord.Attr01Type;
        editRecord.Attr01Desc = ($("#Attr01Desc").data("ismodified")) ? $("#Attr01Desc").val() : CurrRecord.Attr01Desc;
        editRecord.Attr02Type = ($("#Attr02Type").data("ismodified")) ? $("#Attr02Type").val() : CurrRecord.Attr02Type;
        editRecord.Attr02Desc = ($("#Attr02Desc").data("ismodified")) ? $("#Attr02Desc").val() : CurrRecord.Attr02Desc;
        editRecord.Attr03Type = ($("#Attr03Type").data("ismodified")) ? $("#Attr03Type").val() : CurrRecord.Attr03Type;
        editRecord.Attr03Desc = ($("#Attr03Desc").data("ismodified")) ? $("#Attr03Desc").val() : CurrRecord.Attr03Desc;
        editRecord.Attr04Type = ($("#Attr04Type").data("ismodified")) ? $("#Attr04Type").val() : CurrRecord.Attr04Type;
        editRecord.Attr04Desc = ($("#Attr04Desc").data("ismodified")) ? $("#Attr04Desc").val() : CurrRecord.Attr04Desc;
        editRecord.Attr05Type = ($("#Attr05Type").data("ismodified")) ? $("#Attr05Type").val() : CurrRecord.Attr05Type;
        editRecord.Attr05Desc = ($("#Attr05Desc").data("ismodified")) ? $("#Attr05Desc").val() : CurrRecord.Attr05Desc;
        editRecord.Attr06Type = ($("#Attr06Type").data("ismodified")) ? $("#Attr06Type").val() : CurrRecord.Attr06Type;
        editRecord.Attr06Desc = ($("#Attr06Desc").data("ismodified")) ? $("#Attr06Desc").val() : CurrRecord.Attr06Desc;
        editRecord.Attr07Type = ($("#Attr07Type").data("ismodified")) ? $("#Attr07Type").val() : CurrRecord.Attr07Type;
        editRecord.Attr07Desc = ($("#Attr07Desc").data("ismodified")) ? $("#Attr07Desc").val() : CurrRecord.Attr07Desc;
        editRecord.Attr08Type = ($("#Attr08Type").data("ismodified")) ? $("#Attr08Type").val() : CurrRecord.Attr08Type;
        editRecord.Attr08Desc = ($("#Attr08Desc").data("ismodified")) ? $("#Attr08Desc").val() : CurrRecord.Attr08Desc;
        editRecord.Attr09Type = ($("#Attr09Type").data("ismodified")) ? $("#Attr09Type").val() : CurrRecord.Attr09Type;
        editRecord.Attr09Desc = ($("#Attr09Desc").data("ismodified")) ? $("#Attr09Desc").val() : CurrRecord.Attr09Desc;
        editRecord.Attr10Type = ($("#Attr10Type").data("ismodified")) ? $("#Attr10Type").val() : CurrRecord.Attr10Type;
        editRecord.Attr10Desc = ($("#Attr10Desc").data("ismodified")) ? $("#Attr10Desc").val() : CurrRecord.Attr10Desc;
        editRecord.Attr11Type = ($("#Attr11Type").data("ismodified")) ? $("#Attr11Type").val() : CurrRecord.Attr11Type;
        editRecord.Attr11Desc = ($("#Attr11Desc").data("ismodified")) ? $("#Attr11Desc").val() : CurrRecord.Attr11Desc;
        editRecord.Attr12Type = ($("#Attr12Type").data("ismodified")) ? $("#Attr12Type").val() : CurrRecord.Attr12Type;
        editRecord.Attr12Desc = ($("#Attr12Desc").data("ismodified")) ? $("#Attr12Desc").val() : CurrRecord.Attr12Desc;
        editRecord.Attr13Type = ($("#Attr13Type").data("ismodified")) ? $("#Attr13Type").val() : CurrRecord.Attr13Type;
        editRecord.Attr13Desc = ($("#Attr13Desc").data("ismodified")) ? $("#Attr13Desc").val() : CurrRecord.Attr13Desc;
        editRecord.Attr14Type = ($("#Attr14Type").data("ismodified")) ? $("#Attr14Type").val() : CurrRecord.Attr14Type;
        editRecord.Attr14Desc = ($("#Attr14Desc").data("ismodified")) ? $("#Attr14Desc").val() : CurrRecord.Attr14Desc;
        editRecord.Attr15Type = ($("#Attr15Type").data("ismodified")) ? $("#Attr15Type").val() : CurrRecord.Attr15Type;
        editRecord.Attr15Desc = ($("#Attr15Desc").data("ismodified")) ? $("#Attr15Desc").val() : CurrRecord.Attr15Desc;
        editRecord.Comments = ($("#Comments").data("ismodified")) ? $("#Comments").val() : CurrRecord.Comments;
        editRecord.IsActive = ($("#IsActive").data("ismodified")) ? (($("#IsActive").prop("checked")) ? true : false) : CurrRecord.IsActive;

        editRecord.ModifiedProperties = modifiedProperties;

        editRecords.push(editRecord);
    });

    $.ajax({
        type: "POST", url: "/AssemblyModelSrv/Edit", timeout: 20000, data: { records: editRecords }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            refreshTable(TableMain, "/AssemblyModelSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true));
            IsCreate = false;
            $("#MainView").removeClass("hide");
            $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
        })
        .fail(function (xhr, status, error) {
            showModalAJAXFail(xhr, status, error);
        });
}

//Delete Records from DB
function DeleteRecords() {
    var ids = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $.ajax({
        type: "POST", url: "/AssemblyModelSrv/Delete", timeout: 20000, data: { ids: ids }, dataType: "json",
        beforeSend: function () { $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false }); }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { refreshTable(TableMain, "/AssemblyModelSrv/Get", (($("#ChBoxShowDeleted").prop("checked")) ? false : true)); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}

//---------------------------------------Helper Methods--------------------------------------//


