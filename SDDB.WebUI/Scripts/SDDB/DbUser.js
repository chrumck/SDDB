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
var TableDBRolesAdd = {};
var TableDBRolesRemove = {};
var MagicSuggests = [];
var RecordTemplate = {
    Id: "RecordTemplateId",
    LastName: null,
    FirstName: null,
    UserName: null,
    Email: null,
    LDAPAuthenticated_bl: null,
    Password: null,
    PasswordConf:null
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
        MagicSuggests[0].enable();
        fillFormForCreateGeneric("EditForm", MagicSuggests, "Create SDDB User", "MainView");
    });

    //Wire up BtnEdit
    $("#BtnEdit").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }

        showModalWait();

        fillFormForEditGeneric(CurrIds, "POST", "/DBUserSrv/GetByIds", null, "EditForm", "Edit SDDB User", MagicSuggests)
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function (currRecords) {
                CurrRecords = currRecords;

                //Id not handled by fillFormForEditGeneric, has to be set -------
                if (CurrRecords.length == 1) {
                    MagicSuggests[0].addToSelection([{
                        id: currRecords[0].Id,
                        name: currRecords[0].FirstName + " " + currRecords[0].LastName
                    }], true);
                }
                else {
                    MagicSuggests[0].addToSelection([{ id: "_VARIES_", name: "_VARIES_" }], true);
                }
                MagicSuggests[0].disable();
                //---------------------------------------------------------------

                $("#MainView").addClass("hide");
                $("#EditFormView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
    });

    //Wire up BtnDelete 
    $("#BtnDelete").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) { showModalNothingSelected(); }
        else { showModalDelete(CurrIds.length); }
    });

    //Wire Up BtnEditRoles 
    $("#BtnEditRoles").click(function () {
        CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
        if (CurrIds.length == 0) {
            showModalNothingSelected();
            return;
        }
        if (CurrIds.length == 1) {
            var selectedRecord = TableMain.row(".ui-selected").data()
            $("#DBRolesViewPanel").text(selectedRecord.FirstName + " " + selectedRecord.LastName);
        }
        else { $("#DBRolesViewPanel").text("_MULTIPLE_"); }

        showModalWait();

        fillFormForRelatedGeneric(TableDBRolesAdd, TableDBRolesRemove, CurrIds, "GET", "/DBUserSrv/GetUserRoles", { id: CurrIds[0] },
        "GET", "/DBUserSrv/GetUserRolesNot", { id: CurrIds[0] }, "GET", "/DBUserSrv/GetAllRoles", null, 0)
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function () {
                $("#MainView").addClass("hide");
                $("#DBRolesView").removeClass("hide");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

    });

    
    //---------------------------------------DataTables------------

    //TableMain DBUsers
    TableMain = $("#TableMain").DataTable({
        columns: [
            { data: "Id", name: "Id" },
            { data: "LastName", name: "LastName" },
            { data: "FirstName", name: "FirstName" },
            { data: "UserName", name: "UserName" },
            { data: "Email", name: "Email" },
            { data: "LDAPAuthenticated_bl", name: "LDAPAuthenticated_bl" }
        ],
        columnDefs: [
            { targets: [0, 5], searchable: false },  //"orderable": false, "visible": false
            { targets: [0], visible: false },
            { targets: [4, 5], className: "hidden-xs hidden-sm" }
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

    ///Initialize MagicSuggest Array
    addToMSArray(MagicSuggests, "Id", "/PersonSrv/PersonsWoDBUser", 1);

    //Wire Up EditFormBtnCancel
    $("#EditFormBtnCancel, #EditFormBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#EditFormView").addClass("hide"); window.scrollTo(0, 0);
        window.scrollTo(0, 0);
    });

    //Wire Up EditFormBtnOk
    $("#EditFormBtnOk").click(function () {
        msValidate(MagicSuggests);
        if (formIsValid("EditForm", CurrIds.length == 0) && msIsValid(MagicSuggests)) {

            showModalWait();

            //Id not handled by submitEditsGeneric, has to be set
            if (CurrRecords.length == 1) { CurrRecords[0].Id = MagicSuggests[0].getValue()[0]; }

            //Password and PasswordConf not returned in CurrentRecords by server, needs to be added manually
            for (var i = 0; i < CurrRecords.length; i++) {
                CurrRecords[i].Password = "";
                CurrRecords[i].PasswordConf = "";
            }
           
            submitEditsGeneric("EditForm", MagicSuggests, CurrRecords, "POST", "/DbUserSrv/Edit")
                .always(function () { $("#ModalWait").modal("hide"); })
                .done(function () {
                    refreshTableGeneric(TableMain, "/DbUserSrv/Get", null);
                    $("#MainView").removeClass("hide");
                    $("#EditFormView").addClass("hide");
                    window.scrollTo(0, 0);
                })
                .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error) });
        }
    });
    
    //----------------------------------------DBRolesView----------------------------------------//

    //Wire Up DBRolesViewBtnCancel
    $("#DBRolesViewBtnCancel, #DBRolesViewBtnBack").click(function () {
        $("#MainView").removeClass("hide");
        $("#DBRolesView").addClass("hide"); window.scrollTo(0, 0);
    });

    //Wire Up DBRolesViewBtnOk
    $("#DBRolesViewBtnOk").click(function () {
        if (TableDBRolesAdd.rows(".ui-selected").data().length +
            TableDBRolesRemove.rows(".ui-selected").data().length == 0) {
            showModalNothingSelected();
            return;
        }
        showModalWait();

        submitEditsForRelatedGeneric(CurrIds, TableDBRolesAdd.cells(".ui-selected", "Name:name").data().toArray(),
                TableDBRolesRemove.cells(".ui-selected", "Name:name").data().toArray(), "/DBUserSrv/EditRoles")
            .always(function () { $("#ModalWait").modal("hide"); })
            .done(function () {
                $("#MainView").removeClass("hide");
                $("#DBRolesView").addClass("hide");
                window.scrollTo(0, 0);
                refreshTblGenWrp(TableMain, "/DBUserSrv/Get", {}, "GET");
            })
            .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });

    });

    //---------------------------------------DataTables------------

    //TableDBRolesAdd
    TableDBRolesAdd = $("#TableDBRolesAdd").DataTable({
        columns: [
            { data: "Name", name: "Name" }
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

    //TableDBRolesRemove
    TableDBRolesRemove = $("#TableDBRolesRemove").DataTable({
        columns: [
            { data: "Name", name: "Name" }
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

    refreshTblGenWrp(TableMain, "/DBUserSrv/Get", {}, "GET");

    $("#InitialView").addClass("hide");
    $("#MainView").removeClass("hide");

    //--------------------------------End of execution at Start-----------
});


//--------------------------------------Main Methods---------------------------------------//

//Delete Records from DB
function DeleteRecords() {
    CurrIds = TableMain.cells(".ui-selected", "Id:name").data().toArray();
    showModalWait();
    $.ajax({ type: "POST", url: "/DbUserSrv/Delete", timeout: 20000, data: { ids: CurrIds }, dataType: "json"})
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function () { refreshTableGeneric(TableMain, "/DbUserSrv/Get"); })
        .fail(function (xhr, status, error) { showModalAJAXFail(xhr, status, error); });
}


//---------------------------------------Helper Methods--------------------------------------//



