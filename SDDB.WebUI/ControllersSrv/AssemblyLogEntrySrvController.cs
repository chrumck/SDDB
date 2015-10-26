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
    public class AssemblyLogEntrySrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private AssemblyLogEntryService assyLogEntryService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public AssemblyLogEntrySrvController(AssemblyLogEntryService assyLogEntryService)
        {
            this.assyLogEntryService = assyLogEntryService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // POST: /AssemblyLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyLogEntryService.GetAsync";
            var records = (await assyLogEntryService.GetAsync(ids, getActive).ConfigureAwait(false));
            return DbJsonDateTime(filterForJsonFull(records));
        }

        // POST: /AssemblyLogEntrySrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds, string[] assyIds, string[] personIds, 
            DateTime? startDate, DateTime? endDate, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyLogEntryService.GetByAltIdsAsync";
            var records = await assyLogEntryService.GetByAltIdsAsync(projectIds, assyIds, personIds, startDate, endDate, getActive)
                .ConfigureAwait(false);
            return DbJsonDateTime(filterForJsonFull(records));
        }
                
        
        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyLogEntrySrv/Edit
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Edit(AssemblyLogEntry[] records)
        {
            ViewBag.ServiceName = "AssemblyLogEntryService.EditAsync";
            var newEntryIds = await assyLogEntryService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /AssemblyLogEntrySrv/Delete
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "AssemblyLogEntryService.DeleteAsync";
            await assyLogEntryService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<AssemblyLogEntry> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.LogEntryDateTime,
                AssemblyDb_ = new
                {
                    x.AssemblyDb.AssyName,
                    x.AssemblyDb.AssyAltName
                },
                LastSavedByPerson_ = new
                {
                    x.LastSavedByPerson.FirstName,
                    x.LastSavedByPerson.LastName,
                    x.LastSavedByPerson.Initials
                },
                AssemblyStatus_ = new
                {
                    x.AssemblyStatus.AssyStatusName
                },
                AssignedToLocation_ = new
                {
                    x.AssignedToLocation.LocName,
                    x.AssignedToLocation.LocAltName,
                    x.AssignedToLocation.AssignedToProject.ProjectName,
                },
                x.AssyGlobalX,
                x.AssyGlobalY,
                x.AssyGlobalZ,
                x.AssyLocalXDesign,
                x.AssyLocalYDesign,
                x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt,
                x.AssyLocalYAsBuilt,
                x.AssyLocalZAsBuilt,
                x.AssyStationing,
                x.AssyLength,
                x.Comments,
                x.IsActive_bl,
                x.AssemblyDb_Id,
                x.LastSavedByPerson_Id,
                x.AssemblyStatus_Id,
                x.AssignedToLocation_Id
            })
            .ToList();
        }


        #endregion
    }
}