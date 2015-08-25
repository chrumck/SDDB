using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class AssemblyDbSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private AssemblyDbService assemblyService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public AssemblyDbSrvController(AssemblyDbService assemblyService)
        {
            this.assemblyService = assemblyService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//
                
        // POST: /AssemblyDbSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyDbService.GetAsync";
            var records = await assemblyService.GetAsync(ids, getActive).ConfigureAwait(false);
            return DbJson( filterForJsonFull(records) );
        }

        // POST: /AssemblyDbSrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds, string[] modelIds, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyDbService.GetByAltIdsAsync";
            var records = await assemblyService.GetByAltIdsAsync(projectIds, modelIds, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // POST: /AssemblyDbSrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByAltIds2(string[] projectIds, string[] typeIds, string[] locIds, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyDbService.GetByAltIdsAsync";
            var records = await assemblyService.GetByAltIdsAsync(projectIds, typeIds, locIds, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // POST: /AssemblyDbSrv/CountByAltIds2
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> CountByAltIds2(string[] projectIds, string[] typeIds, string[] locIds, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyDbService.CountByAltIdsAsync";
            var count = await assemblyService.CountByAltIdsAsync(projectIds, typeIds, locIds, getActive).ConfigureAwait(false);
            return DbJson(count);
        }
        
        // GET: /AssemblyDbSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyDbService.LookupAsync";
            var records = await assemblyService.LookupAsync(query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        // GET: /AssemblyDbSrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (!String.IsNullOrEmpty(projectIds)) { projectIdsArray = projectIds.Split(','); }

            ViewBag.ServiceName = "AssemblyDbService.LookupByProjAsync";
            var records = await assemblyService.LookupByProjAsync(projectIdsArray, query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        // GET: /AssemblyDbSrv/LookupByLoc
        public async Task<ActionResult> LookupByLoc(string locId = null, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyDbService.LookupByLocAsync";
            var records = await assemblyService.LookupByLocAsync(locId, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        // GET: /AssemblyDbSrv/LookupByLocDTables
        public async Task<ActionResult> LookupByLocDTables(string locId = null, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyDbService.LookupByLocAsync";
            var records = await assemblyService.LookupByLocAsync(locId, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonDTables(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyDbSrv/Edit
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Edit(AssemblyDb[] records)
        {
            ViewBag.ServiceName = "AssemblyDbService.EditAsync";
            var newEntryIds = await assemblyService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /AssemblyDbSrv/EditStatus
        [HttpPost]
        [DBSrvAuth("Assembly_EditStatus")]
        public async Task<ActionResult> EditStatus(string[] ids, string statusId)
        {
            ViewBag.ServiceName = "AssemblyDbService.EditStatusAsync";
            await assemblyService.EditStatusAsync(ids, statusId).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        // POST: /AssemblyDbSrv/Delete
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "AssemblyDbService.DeleteAsync";
            await assemblyService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyDbSrv/EditExt
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> EditExt(AssemblyExt[] records)
        {
            ViewBag.ServiceName = "AssemblyDbService.EditExtendedAsync";
            await assemblyService.EditExtendedAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<AssemblyDb> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.AssyName,
                x.AssyAltName,
                x.AssyAltName2,
                AssemblyType_ = new {
                    x.AssemblyType.AssyTypeName
                },
                AssemblyStatus_ = new {
                    x.AssemblyStatus.AssyStatusName
                },
                AssemblyModel_ = new {
                    x.AssemblyModel.AssyModelName
                },
                AssignedToProject_ = new {
                    x.AssignedToProject.ProjectCode,
                    x.AssignedToProject.ProjectName
                },
                AssignedToLocation_ = new {
                    x.AssignedToLocation.LocName,
                    x.AssignedToLocation.LocationType.LocTypeName
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
                x.AssyReadingIntervalSecs,
                x.IsReference_bl,
                x.TechnicalDetails,
                x.PowerSupplyDetails,
                x.HSEDetails,
                x.Comments,
                x.IsActive_bl,
                x.AssemblyExt.Attr01,
                x.AssemblyExt.Attr02,
                x.AssemblyExt.Attr03,
                x.AssemblyExt.Attr04,
                x.AssemblyExt.Attr05,
                x.AssemblyExt.Attr06,
                x.AssemblyExt.Attr07,
                x.AssemblyExt.Attr08,
                x.AssemblyExt.Attr09,
                x.AssemblyExt.Attr10,
                x.AssemblyExt.Attr11,
                x.AssemblyExt.Attr12,
                x.AssemblyExt.Attr13,
                x.AssemblyExt.Attr14,
                x.AssemblyExt.Attr15,
                x.AssemblyType_Id,
                x.AssemblyStatus_Id,
                x.AssemblyModel_Id,
                x.AssignedToProject_Id,
                x.AssignedToLocation_Id
            })
            .ToList();
        }


        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<AssemblyDb> records)
        {
            return records
                .OrderBy(x => x.AssyName)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.AssyName + " - " + x.AssignedToProject.ProjectName
                })
                .ToList();
        }

        //filterForJsonDTables - filter data from service to be passed as response
        private object filterForJsonDTables(List<AssemblyDb> records)
        {
            return records
                .OrderBy(x => x.AssyName)
                .Select(x => new
                {
                    Id = x.Id,
                    AssyName = x.AssyName
                })
                .ToList();
        }



        #endregion
    }
}