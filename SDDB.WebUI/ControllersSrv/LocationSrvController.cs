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
    public class LocationSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private LocationService locationService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public LocationSrvController(LocationService locationService)
        {
            this.locationService = locationService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // POST: /LocationSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Location_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "LocationService.GetAsync";
            var records = await locationService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /LocationSrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Location_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds, string[] typeIds, bool getActive = true)
        {
            ViewBag.ServiceName = "LocationService.GetByAltIdsAsync";
            var records = await locationService.GetByAltIdsAsync(projectIds, typeIds, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "LocationService.LookupAsync";
            var records = await locationService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationSrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (!String.IsNullOrEmpty(projectIds)) { projectIdsArray = projectIds.Split(','); }

            ViewBag.ServiceName = "LocationService.LookupByProjAsync";
            var records = await locationService.LookupByProjAsync(projectIdsArray, query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /LocationSrv/Edit
        [HttpPost]
        [DBSrvAuth("Location_Edit")]
        public async Task<ActionResult> Edit(Location[] records)
        {
            ViewBag.ServiceName = "LocationService.EditAsync";
            var newEntryIds = await locationService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /LocationSrv/Delete
        [HttpPost]
        [DBSrvAuth("Location_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "LocationService.DeleteAsync";
            await locationService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<Location> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.LocName,
                x.LocAltName,
                x.LocAltName2,
                LocationType_ = new
                {
                    x.LocationType.LocTypeName
                },
                AssignedToProject_ = new 
                {
                    x.AssignedToProject.ProjectName,
                    x.AssignedToProject.ProjectAltName, 
                    x.AssignedToProject.ProjectCode 
                },
                ContactPerson_ = new {
                    x.ContactPerson.FirstName,
                    x.ContactPerson.LastName,
                    x.ContactPerson.Initials 
                },
                x.Address,
                x.LocX,
                x.LocY,
                x.LocZ,
                x.LocStationing,
                x.CertOfApprReqd_bl,
                x.RightOfEntryReqd_bl,
                x.AccessInfo,
                x.Comments,
                x.IsActive_bl,
                x.LocationType_Id,
                x.AssignedToProject_Id,
                x.ContactPerson_Id
            })
            .ToList();
        }


        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<Location> records)
        {
            return records
                .OrderBy(x => x.LocName)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.LocName + " - " + x.AssignedToProject.ProjectName
                })
                .ToList();
        }


        #endregion
    }
}