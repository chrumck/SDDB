/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain;
var MsFilterByProject = {};
var MsFilterByType = {};
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    DocName: null,
    DocAltName: null,
    DocLastVersion: null,
    DocFilePath: null,
    Comments: null,
    IsActive_bl: null,
    DocumentType_Id: null,
    AuthorPerson_Id: null,
    ReviewerPerson_Id: null,
    AssignedToProject_Id: null,
    RelatesToAssyType_Id: null,
    RelatesToCompType_Id: null
};
var CurrRecords = [];
var CurrIds = [];
var GetActive = true;

$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        CurrRecords = [];
        CurrRecords[0] = $.extend(true, {}, RecordTemplate);
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Document", "MainView");
        saveViewSettings(TableMain);
        switchView("MainView", "EditFormView", "tdo-btngroup-edit");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();
	fillFormForEditGeneric(CurrIds, "POST", "/DocumentSrv/GetByIds", 
		GetActive, "EditForm", "Edit Document", MagicSuggests)
            .always(hideModalWait)
            .done(function (currRecords) {
                CurrRecords = currRecords;
                saveViewSettings(TableMain);
                switchView("MainView", "EditFormView", "tdo-btngroup-edit");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6]).visible(true);
        TableMain.columns([7, 8, 9, 10, 11]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([2, 3, 4, 5, 6]).visible(false);
        TableMain.columns([7, 8, 9, 10, 11]).visible(true);
    });


    //Initialize MagicSuggest MsFilterByType
    MsFilterByType = $("#MsFilterByType").magicSuggest({
        data: "/DocumentTypeSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByType
    $(MsFilterByType).on('selectionchange', function (e, m) { refreshMainView(); });

    //Initialize MagicSuggest MsFilterByProject
    MsFilterByProject = $("#MsFilterByProject").magicSuggest({
        data: "/ProjectSrv/Lookup",
        allowFreeEntries: false,
        ajaxConfig: {
            error: function (xhr, status, error) { showModalAJAXFail(xhr, status, error); }
        },
        infoMsgCls: "hidden",
        style: "min-width: 240px;"
    });
    //Wire up on change event for MsFilterByProject
    $(MsFilterByProject).on('selectionchange', function (e, m) { refreshMainView(); });

    //---------------------------------------DataTables------------

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

    //TableMain Documents
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "DocName", name: "DocName" },//1
            { data: "DocAltName", name: "DocAltName" },//2
            { data: "DocumentType_", render: function (data, type, full, meta) { return data.DocTypeName }, name: "DocumentType_" }, //3
            { data: "DocLastVersion", name: "DocLastVersion" },//4
            { data: "AuthorPerson_", render: function (data, type, full, meta) { return data.Initials }, name: "AuthorPerson_" },//5
            { data: "ReviewerPerson_", render: function (data, type, full, meta) { return data.Initials }, name: "ReviewerPerson_" }, //6
            { data: "AssignedToProject_", render: function (data, type, full, meta) { return data.ProjectName }, name: "AssignedToProject_" }, //7
            { data: "RelatesToAssyType_", render: function (data, type, full, meta) { return data.AssyTypeName }, name: "RelatesToAssyType_" }, //8
            { data: "RelatesToCompType_", render: function (data, type, full, meta) { return data.CompTypeName }, name: "RelatesToCompType_" }, //9
            { data: "DocFilePath", name: "DocFilePath" },//10
            { data: "Comments", name: "Comments" },//11
            { data: "IsActive_bl", name: "IsActive_bl" },//12
            { data: "DocumentType_Id", name: "DocumentType_Id" },//13
            { data: "AuthorPerson_Id", name: "AuthorPerson_Id" },//14
            { data: "ReviewerPerson_Id", name: "ReviewerPerson_Id" },//15
            { data: "AssignedToProject_Id", name: "AssignedToProject_Id" },//16
            { data: "RelatesToAssyType_Id", name: "RelatesToAssyType_Id" },//17
            { data: "RelatesToCompType_Id", name: "RelatesToCompType_Id" },//18
        ],
        columnDefs: [
            { targets: [0, 12, 13, 14, 15, 16, 17, 18], visible: false }, // - never show
            { targets: [0, 12, 13, 14, 15, 16, 17, 18], searchable: false },  //"orderable": false, "visible": false
            { targets: [2, 4, 5], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [6], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [7, 8, 9, 10, 11], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 10], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [11], className: "hidden-xs hidden-sm hidden-md" } // - second set of columns
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

    //Initialize MagicSuggest Array
    msAddToMsArray(MagicSuggests, "DocumentType_Id", "/DocumentTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AuthorPerson_Id", "/PersonSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "ReviewerPerson_Id", "/PersonSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "AssignedToProject_Id", "/ProjectSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "RelatesToAssyType_Id", "/AssemblyTypeSrv/Lookup", 1);
    msAddToMsArray(MagicSuggests, "RelatesToCompType_Id", "/ComponentTypeSrv/Lookup", 1);
    
    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel").click(function () {
        switchView("EditFormView","MainView", "tdo-btngroup-main", true);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (!formIsValid("EditForm", CurrIds.length == 0) || !msIsValid(MagicSuggests)) {
            showModalFail("Errors in Form", "The form has missing or invalid inputs. Please correct.");
            return;
        }
        showModalWait();
        submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/DocumentSrv/Edit")
            .always(hideModalWait)
            .done(function () {
                refreshMainView()
                    .done(function () {
                        switchView("EditFormView", "MainView", "tdo-btngroup-main", true, TableMain);
                    });
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
    });

    //--------------------------------------View Initialization------------------------------------//

    $("#InitialView").addClass("hidden");
    $("#MainView").removeClass("hidden");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//


//Delete Records from DB
function deleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name", { page: "current" }).data().toArray();
    deleteRecordsGeneric(CurrIds, "/DocumentSrv/Delete", refreshMainView);
}

//refresh view after magicsuggest update
function refreshMainView() {
    var deferred0 = $.Deferred();

    TableMain.clear().search("").draw();

    if (MsFilterByType.getValue().length == 0 && MsFilterByProject.getValue().length == 0) {
        return deferred0.resolve();
    }
    refreshTblGenWrp(TableMain, "/DocumentSrv/GetByAltIds",
        {
            projectIds: MsFilterByProject.getValue(),
            typeIds: MsFilterByType.getValue(),
            getActive: GetActive
        },
        "POST")
        .done(deferred0.resolve);

    return deferred0.promise();
}


//---------------------------------------Helper Methods--------------------------------------//

