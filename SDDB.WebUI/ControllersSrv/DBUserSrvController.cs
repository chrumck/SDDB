using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class DBUserSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private DBUserService dbUserService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DBUserSrvController(DBUserService dbUserService)
        {
            this.dbUserService = dbUserService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /DBUserSrv/Get
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> Get()
        {
            var users = await dbUserService.GetAsync().ConfigureAwait(false);
            var data = users.OrderBy(x => x.Person.LastName).Select(x => new {
                x.Id, x.Person.LastName, x.Person.FirstName, x.UserName, x.Email, x.LDAPAuthenticated });

            ViewBag.ServiceName = "DBUserService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        //POST: /DBUserSrv/Get By Ids
        [HttpPost]
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetByIds(string[] ids)
        {
            var users = await dbUserService.GetAsync().ConfigureAwait(false);

            var data = users.Where(x => ids.Contains((x.Id))).OrderBy(x => x.Person.LastName).Select(x => new {
                x.Id, x.Person.LastName, x.Person.FirstName, x.UserName, x.Email, x.LDAPAuthenticated });

            ViewBag.ServiceName = "DBUserService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /DBUserSrv/Edit
        [HttpPost]
        [DBSrvAuth("DBUser_Edit")]
        public async Task<ActionResult> Edit(DBUser[] records)
        {
            var serviceResult = await dbUserService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "DBUserService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode; 
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /DBUserSrv/Delete
        [HttpPost]
        [DBSrvAuth("DBUser_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await dbUserService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "DBUserService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }


        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /DBUserSrv/GetAllRoles
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetAllRoles()
        {
            var data = (await dbUserService.GetAllRolesAsync().ConfigureAwait(false)).Select(x => new { Name = x });

            ViewBag.ServiceName = "DBUserService.GetAllRolesAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /DBUserSrv/GetUserRoles
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetUserRoles(string id)
        {
            var data = (await dbUserService.GetUserRolesAsync(id).ConfigureAwait(false)).Select(x => new { Name = x });

            ViewBag.ServiceName = "DBUserService.GetUserRolesAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /DBUserSrv/GetUserRolesNot
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetUserRolesNot(string id)
        {
            var data = (await dbUserService.GetUserRolesNotAsync(id).ConfigureAwait(false)).Select(x => new { Name = x });

            ViewBag.ServiceName = "DBUserService.GetUserRolesNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // POST: /DBUserSrv/EditRoles
        [HttpPost]
        [DBSrvAuth("DBUser_Edit")]
        public async Task<ActionResult> EditRoles(string[] ids, string[] dbRoles, bool isAdd)
        {
            var serviceResult = await dbUserService.EditRolesAsync(ids, dbRoles, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "DBUserService.EditRolesAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}