using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class ComponentLogEntrySrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ComponentLogEntryService compLogEntryService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ComponentLogEntrySrvController(ComponentLogEntryService compLogEntryService)
        {
            this.compLogEntryService = compLogEntryService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // POST: /ComponentLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentLogEntryService.GetAsync";
            var records = await compLogEntryService.GetAsync(ids, getActive).ConfigureAwait(false);
            return new DBJsonDateTimeISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /ComponentLogEntrySrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds, string[] componentIds, string[] personIds,
            DateTime? startDate, DateTime? endDate, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentLogEntryService.GetByAltIdsAsync"; 
            var records = await compLogEntryService.GetByAltIdsAsync(projectIds, componentIds, personIds, startDate, endDate, getActive)
                .ConfigureAwait(false);
            return new DBJsonDateTimeISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
                
        
        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentLogEntrySrv/Edit
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Edit(ComponentLogEntry[] records)
        {
            ViewBag.ServiceName = "ComponentLogEntryService.EditAsync";
            var newEntryIds = await compLogEntryService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /ComponentLogEntrySrv/Delete
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ComponentLogEntryService.DeleteAsync";
            await compLogEntryService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<ComponentLogEntry> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.LogEntryDateTime,
                Component_ = new
                {
                    x.Component.CompName,
                    x.Component.CompAltName
                },
                LastSavedByPerson_ = new
                {
                    x.LastSavedByPerson.FirstName,
                    x.LastSavedByPerson.LastName,
                    x.LastSavedByPerson.Initials
                },
                ComponentStatus_ = new
                {
                    x.ComponentStatus.CompStatusName
                },
                AssignedToProject_ = new
                {
                    x.AssignedToProject.ProjectName,
                    x.AssignedToProject.ProjectCode
                },
                AssignedToAssemblyDb_ = new
                {
                    x.AssignedToAssemblyDb.AssyName,
                    x.AssignedToAssemblyDb.AssyAltName
                },
                x.LastCalibrationDate,
                x.Comments,
                x.IsActive_bl,
                x.Component_Id,
                x.LastSavedByPerson_Id,
                x.ComponentStatus_Id,
                x.AssignedToProject_Id,
                x.AssignedToAssemblyDb_Id
            })
            .ToList();
        }


        #endregion
    }
}