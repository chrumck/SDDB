using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class ComponentSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ComponentService componentService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ComponentSrvController(ComponentService componentService)
        {
            this.componentService = componentService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//
       
        // POST: /ComponentSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentService.GetAsync";
            var records = await componentService.GetAsync(ids, getActive).ConfigureAwait(false);
            return DbJsonDate(filterForJsonFull(records));
        }

        // POST: /ComponentSrv/GetByAltIds2
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByAltIds2(string[] projectIds, string[] typeIds, string[] assyIds, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentService.GetByAltIdsAsync";
            var records = await componentService.GetByAltIdsAsync(projectIds, typeIds, assyIds, getActive).ConfigureAwait(false);
            return DbJsonDate(filterForJsonFull(records));
        }


        // GET: /ComponentSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentService.LookupAsync";
            var records = await componentService.LookupAsync(query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        // GET: /ComponentSrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (!String.IsNullOrEmpty(projectIds)) { projectIdsArray = projectIds.Split(','); }

            ViewBag.ServiceName = "ComponentService.LookupByProjAsync";
            var records = await componentService.LookupByProjAsync(projectIdsArray, query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentSrv/Edit
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Edit(Component[] records)
        {
            ViewBag.ServiceName = "ComponentService.EditAsync";
            var newEntryIds = await componentService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /ComponentSrv/Delete
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ComponentService.DeleteAsync";
            await componentService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentSrv/EditExt
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> EditExt(ComponentExt[] records)
        {
            ViewBag.ServiceName = "ComponentService.EditExtendedAsync";
            await componentService.EditExtendedAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<Component> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.CompName,
                x.CompAltName,
                x.CompAltName2,
                ComponentType_ = new
                {
                    x.ComponentType.CompTypeName
                },
                ComponentStatus_ = new
                {
                    x.ComponentStatus.CompStatusName
                },
                AssignedToProject_ = new
                {
                    x.AssignedToProject.ProjectCode,
                    x.AssignedToProject.ProjectName
                },
                AssignedToAssemblyDb_ = new
                {
                    x.AssignedToAssemblyDb.AssyName,
                    x.AssignedToAssemblyDb.AssyAltName
                },
                x.PositionInAssy,
                x.ProgramAddress,
                x.CalibrationReqd_bl,
                x.LastCalibrationDate,
                x.Comments,
                x.IsActive_bl,
                x.ComponentExt.Attr01,
                x.ComponentExt.Attr02,
                x.ComponentExt.Attr03,
                x.ComponentExt.Attr04,
                x.ComponentExt.Attr05,
                x.ComponentExt.Attr06,
                x.ComponentExt.Attr07,
                x.ComponentExt.Attr08,
                x.ComponentExt.Attr09,
                x.ComponentExt.Attr10,
                x.ComponentExt.Attr11,
                x.ComponentExt.Attr12,
                x.ComponentExt.Attr13,
                x.ComponentExt.Attr14,
                x.ComponentExt.Attr15,
                x.ComponentType_Id,
                x.ComponentStatus_Id,
                x.AssignedToProject_Id,
                x.AssignedToAssemblyDb_Id
            })
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<Component> records)
        {
            return records
                .OrderBy(x => x.CompName)
                .Select(x => new
                    {
                        id = x.Id,
                        name = x.CompName + " - " + x.AssignedToProject.ProjectName
                    })
                .ToList();
        }


        #endregion
    }
}