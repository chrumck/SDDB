/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />

//--------------------------------------Global Properties------------------------------------//

var TableMain = {};
var TableProjectsAdd = {}; var TableProjectsRemove = {};
var TablePersonGroupsAdd = {}; var TablePersonGroupsRemove = {};
var TableManagedGroupsAdd = {}; var TableManagedGroupsRemove = {};
var IsCreate = false;
var MagicSuggests = [];
var CurrRecord = {
    Id: null,
    FirstName: null,
    LastName: null,
    Initials: null,
    Phone: null,
    PhoneMobile: null,
    Email: null,
    Comments: null,
    IsCurrentEmployee_bl: null,
    EmployeePosition: null,
    IsSalaried_bl: null,
    EmployeeStart: null,
    EmployeeEnd: null,
    EmployeeDetails: null,
    IsActive_bl: null
};
var CurrIds = [];
var GetActive = true;
var SelectedRecord;


$(document).ready(function () {

    //-----------------------------------------MainView------------------------------------------//

    //Wire up BtnCreate
    $("#BtnCreate").click(function () {
        CurrIds = [];
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create Person", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            if (GetActive) $("#EditFormGroupIsActive").addClass("hide");
            else $("#EditFormGroupIsActive").removeClass("hide");

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            fillFormForEditGeneric(CurrIds, "POST", "/PersonSrv/GetAllByIds", GetActive, "EditForm", "Edit Person", MagicSuggests)
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function (currRecord) {
                    CurrRecord = currRecord;
                    $("#MainView").addClass("hide");
                    $("#EditFormView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else showModalDelete(CurrIds.length);
    });

    //Wire Up BtnEditPrsProj 
    $("#BtnEditPrsProj").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            SelectedRecord = TableMain.row(".ui-selected").data()
            if (CurrIds.length == 1) $("#PrsProjViewPanel").text(SelectedRecord.FirstName + " " + SelectedRecord.LastName);
            else $("#PrsProjViewPanel").text("_MULTIPLE_");

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            fillFormForRelatedGeneric(TableProjectsAdd, TableProjectsRemove, CurrIds, "GET", "/PersonSrv/GetPersonProjects", { id: CurrIds[0] },
            "GET", "/PersonSrv/GetPersonProjectsNot", { id: CurrIds[0] }, "GET", "/ProjectSrv/Get", { getActive: true })
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").addClass("hide");
                    $("#PrsProjView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire Up BtnEditPersonGroups 
    $("#BtnEditPersonGroups").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            SelectedRecord = TableMain.row(".ui-selected").data()
            if (CurrIds.length == 1) $("#PersonGroupsViewPanel").text(SelectedRecord.FirstName + " " + SelectedRecord.LastName);
            else $("#PersonGroupsViewPanel").text("_MULTIPLE_");

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            fillFormForRelatedGeneric(TablePersonGroupsAdd, TablePersonGroupsRemove, CurrIds, "GET", "/PersonSrv/GetPersonGroups", { id: CurrIds[0] },
            "GET", "/PersonSrv/GetPersonGroupsNot", { id: CurrIds[0] }, "GET", "/PersonGroupSrv/Get", { getActive: true })
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").addClass("hide");
                    $("#PersonGroupsView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //Wire Up BtnEditManagedGroups 
    $("#BtnEditManagedGroups").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) showModalNothingSelected();
        else {
            SelectedRecord = TableMain.row(".ui-selected").data()
            if (CurrIds.length == 1) $("#ManagedGroupsViewPanel").text(SelectedRecord.FirstName + " " + SelectedRecord.LastName);
            else $("#ManagedGroupsViewPanel").text("_MULTIPLE_");

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            fillFormForRelatedGeneric(TableManagedGroupsAdd, TableManagedGroupsRemove, CurrIds, "GET", "/PersonSrv/GetManagedGroups", { id: CurrIds[0] },
            "GET", "/PersonSrv/GetManagedGroupsNot", { id: CurrIds[0] }, "GET", "/PersonGroupSrv/Get", { getActive: true })
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").addClass("hide");
                    $("#ManagedGroupsView").removeClass("hide");
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //wire up dropdownId1
    $("#dropdownId1").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7 ]).visible(true);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(false);
    });

    //wire up dropdownId2
    $("#dropdownId2").click(function (event) {
        event.preventDefault();
        TableMain.columns([3, 4, 5, 6, 7]).visible(false);
        TableMain.columns([8, 9, 10, 11, 12, 13]).visible(true);
    });

    //wire up dropdownId3
    $("#dropdownId3").click(function (event) {
        event.preventDefault();
        var noOfRows = TableMain.rows(".ui-selected").data().length;
        if (noOfRows != 1) { showModalSelectOne(); }
        else { window.open("/PersonLogEntry?personId=" + TableMain.cell(".ui-selected", "Id:name").data()); }
    });

    //---------------------------------------DataTables------------

    //Wire up ChBoxShowDeleted
    $("#ChBoxShowDeleted").change(function (event) {
        if (!$(this).prop("checked")) { GetActive = true; $("#PanelTableMain").removeClass("panel-tdo-danger").addClass("panel-primary"); }
        else { GetActive = false; $("#PanelTableMain").removeClass("panel-primary").addClass("panel-tdo-danger"); }
        refreshTblGenWrp(TableMain, "/PersonSrv/GetAll", { getActive: GetActive });
    });

    //TableMain Persons
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "LastName", name: "LastName" },//1
            { data: "FirstName", name: "FirstName" },//2
            //------------------------------------------------first set of columns
            { data: "Initials", name: "Initials" },//3
            { data: "Phone", name: "Phone" },//4
            { data: "PhoneMobile", name: "PhoneMobile" },//5
            { data: "Email", name: "Email" },//6
            { data: "Comments", name: "Comments" },//7
            //------------------------------------------------second set of columns
            { data: "IsCurrentEmployee_bl", name: "IsCurrentEmployee_bl" },//8
            { data: "EmployeePosition", name: "EmployeePosition" },//9
            { data: "IsSalaried_bl", name: "IsSalaried_bl" },//10
            { data: "EmployeeStart", name: "EmployeeStart" },//11
            { data: "EmployeeEnd", name: "EmployeeEnd" },//12
            { data: "EmployeeDetails", name: "EmployeeDetails" },//13
            //------------------------------------------------never visible
            { data: "IsActive_bl", name: "IsActive_bl" }//14
        ],
        columnDefs: [
            { targets: [0, 14], visible: false }, // - never show
            { targets: [0, 8, 10, 11, 12, 14], searchable: false },  //"orderable": false, "visible": false
            { targets: [4, 5, 6], className: "hidden-xs hidden-sm" }, // - first set of columns
            { targets: [7], className: "hidden-xs hidden-sm hidden-md" }, // - first set of columns

            { targets: [8, 9, 10, 11, 12, 13], visible: false }, // - second set of columns - to toggle with options
            { targets: [9, 11, 12], className: "hidden-xs hidden-sm" }, // - second set of columns
            { targets: [10, 13], className: "hidden-xs hidden-sm hidden-md" } // - second set of columns
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

    //Enable DatePicker
    $(".datepicker").datetimepicker({ format: "YYYY-MM-DD" })
        .on("dp.change", function (e) { $(this).data("ismodified", true); });

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {

            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            submitEditsGeneric(CurrIds, "EditForm", MagicSuggests, CurrRecord, "POST", "/PersonSrv/Edit")
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    refreshTableGeneric(TableMain, "/PersonSrv/GetAll", { getActive: GetActive });
                    $("#MainView").removeClass("hide");
                    $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
        }
    });

    //----------------------------------------PrsProjView----------------------------------------//

    //Wire Up PrsProjViewBtnCancel
    $("#PrsProjViewBtnCancel, #PrsProjViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#PrsProjView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up PrsProjViewBtnOk
    $("#PrsProjViewBtnOk").click(function () {
        if (TableProjectsAdd.rows(".ui-selected").data().length +
            TableProjectsRemove.rows(".ui-selected").data().length == 0) showModalNothingSelected();
        else {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            submitEditsForRelatedGeneric(CurrIds, TableProjectsAdd.cells(".ui-selected", "Id:name").data().toArray(),
                    TableProjectsRemove.cells(".ui-selected", "Id:name").data().toArray(), "/PersonSrv/EditPersonProjects")
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").removeClass("hide");
                    $("#PrsProjView").addClass("hide"); window.scrollTo(0, 0);
                    refreshTblGenWrp(TableMain, "/PersonSrv/GetAll", { getActive: GetActive });
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //---------------------------------------DataTables------------

    //TableProjectsAdd
    TableProjectsAdd = $("#TableProjectsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectCode", name: "ProjectCode" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //TableProjectsRemove
    TableProjectsRemove = $("#TableProjectsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "ProjectName", name: "ProjectName" },//1
            { data: "ProjectCode", name: "ProjectCode" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //----------------------------------------PersonGroupsView----------------------------------------//

    //Wire Up PersonGroupsViewBtnCancel
    $("#PersonGroupsViewBtnCancel, #PersonGroupsViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#PersonGroupsView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up PersonGroupsViewBtnOk
    $("#PersonGroupsViewBtnOk").click(function () {
        if (TablePersonGroupsAdd.rows(".ui-selected").data().length +
            TablePersonGroupsRemove.rows(".ui-selected").data().length == 0) showModalNothingSelected();
        else {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            submitEditsForRelatedGeneric(CurrIds, TablePersonGroupsAdd.cells(".ui-selected", "Id:name").data().toArray(),
                    TablePersonGroupsRemove.cells(".ui-selected", "Id:name").data().toArray(), "/PersonSrv/EditPersonGroups")
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").removeClass("hide");
                    $("#PersonGroupsView").addClass("hide"); window.scrollTo(0, 0);
                    refreshTblGenWrp(TableMain, "/PersonSrv/GetAll", { getActive: GetActive });
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });
    
    //---------------------------------------DataTables------------

    //TablePersonGroupsAdd
    TablePersonGroupsAdd = $("#TablePersonGroupsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //TablePersonGroupsRemove
    TablePersonGroupsRemove = $("#TablePersonGroupsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //----------------------------------------ManagedGroupsView----------------------------------------//

    //Wire Up ManagedGroupsViewBtnCancel
    $("#ManagedGroupsViewBtnCancel, #ManagedGroupsViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#ManagedGroupsView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up ManagedGroupsViewBtnOk
    $("#ManagedGroupsViewBtnOk").click(function () {
        if (TableManagedGroupsAdd.rows(".ui-selected").data().length +
            TableManagedGroupsRemove.rows(".ui-selected").data().length == 0) showModalNothingSelected();
        else {
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });

            submitEditsForRelatedGeneric(CurrIds, TableManagedGroupsAdd.cells(".ui-selected", "Id:name").data().toArray(),
                    TableManagedGroupsRemove.cells(".ui-selected", "Id:name").data().toArray(), "/PersonSrv/EditManagedGroups")
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    $("#MainView").removeClass("hide");
                    $("#ManagedGroupsView").addClass("hide"); window.scrollTo(0, 0);
                    refreshTblGenWrp(TableMain, "/PersonSrv/GetAll", { getActive: GetActive });
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
        }
    });

    //---------------------------------------DataTables------------

    //TableManagedGroupsAdd
    TableManagedGroupsAdd = $("#TableManagedGroupsAdd").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });

    //TableManagedGroupsRemove
    TableManagedGroupsRemove = $("#TableManagedGroupsRemove").DataTable({
        columns: [
            { data: "Id", name: "Id" },//0
            { data: "PrsGroupName", name: "PrsGroupName" },//1
            { data: "PrsGroupAltName", name: "PrsGroupAltName" }//2
        ],
        columnDefs: [
            { targets: [0], visible: false }, // - never show
            { targets: [0], searchable: false },  //"orderable": false, "visible": false
            { targets: [2], className: "hidden-xs hidden-sm" }
        ],
        bAutoWidth: false,
        language: {
            search: "",
            lengthMenu: "_MENU_",
            info: "_START_ - _END_ of _TOTAL_",
            infoEmpty: "",
            infoFiltered: "(filtered)",
            paginate: { previous: "", next: "" }
        },
        pageLength: 100
    });


    //--------------------------------------View Initialization------------------------------------//

    refreshTblGenWrp(TableMain, "/PersonSrv/GetAll", { getActive: GetActive });

    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
    $.ajax({ type: "POST", url: "/PersonSrv/Delete", timeout: 20000, data: { ids: CurrIds }, dataType: "json" })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { refreshTableGeneric(TableMain, "/PersonSrv/GetAll", { getActive: GetActive }); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}



//---------------------------------------Helper Methods--------------------------------------//



