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
    public class DbUserSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private DBUserService dbUserService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DbUserSrvController(DBUserService dbUserService)
        {
            this.dbUserService = dbUserService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /DBUserSrv/Get
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> Get()
        {
            ViewBag.ServiceName = "DBUserService.GetAsync";
            var records = await dbUserService.GetAsync().ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        //POST: /DBUserSrv/Get By Ids
        [HttpPost]
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetByIds(string[] ids)
        {
            ViewBag.ServiceName = "DBUserService.GetAsync";
            var records = await dbUserService.GetAsync(ids).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /DBUserSrv/Edit
        [HttpPost]
        [DBSrvAuth("DBUser_Edit")]
        public async Task<ActionResult> Edit(DBUser[] records)
        {
            ViewBag.ServiceName = "DBUserService.EditAsync";
            await dbUserService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        // POST: /DBUserSrv/Delete
        [HttpPost]
        [DBSrvAuth("DBUser_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "DBUserService.DeleteAsync";
            await dbUserService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }


        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /DBUserSrv/GetAllRoles
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetAllRoles()
        {
            ViewBag.ServiceName = "DBUserService.GetAllRolesAsync";
            var records = (await dbUserService.GetAllRolesAsync().ConfigureAwait(false)).Select(x => new { Name = x });
            return DbJson(records);
        }

        // GET: /DBUserSrv/GetUserRoles
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetUserRoles(string id)
        {
            ViewBag.ServiceName = "DBUserService.GetUserRolesAsync";
            var records = (await dbUserService.GetUserRolesAsync(id).ConfigureAwait(false)).Select(x => new { Name = x });
            return DbJson(records);
        }

        // GET: /DBUserSrv/GetUserRolesNot
        [DBSrvAuth("DBUser_View")]
        public async Task<ActionResult> GetUserRolesNot(string id)
        {
            ViewBag.ServiceName = "DBUserService.GetUserRolesNotAsync";
            var records = (await dbUserService.GetUserRolesNotAsync(id).ConfigureAwait(false)).Select(x => new { Name = x });
            return DbJson(records);
        }

        // POST: /DBUserSrv/EditRoles
        [HttpPost]
        [DBSrvAuth("DBUser_Edit")]
        public async Task<ActionResult> EditRoles(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "DBUserService.EditRolesAsync";
            await dbUserService.AddRemoveRolesAsync(ids, idsAddRem, isAdd).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //take data and select fields for JSON - full data set
        private object filterForJsonFull(List<DBUser> records)
        {
            return records.Select(x => new 
            {
                x.Id,
                x.Person.LastName,
                x.Person.FirstName,
                x.UserName,
                x.Email,
                x.LDAPAuthenticated_bl 
            }).ToList();
        }

        #endregion
    }
}