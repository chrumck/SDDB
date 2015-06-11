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

        // GET: /LocationSrv/Get
        [DBSrvAuth("Location_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await locationService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LocName, x.LocAltName, x.LocationType.LocTypeName, 
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                ContactPerson = new { x.ContactPerson.FirstName, x.ContactPerson.LastName, x.ContactPerson.Initials },
                x.Address, x.City, x.ZIP, x.State, x.Country, x.LocX, x.LocY, x.LocZ, x.LocStationing,
                x.CertOfApprReqd, x.RightOfEntryReqd, x.AccessInfo ,x.Comments, x.IsActive,
                x.LocationType_Id,x.AssignedToProject_Id, x.ContactPerson_Id 
            });

            ViewBag.ServiceName = "LocationService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Location_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await locationService.GetAsync(UserId, ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LocName, x.LocAltName, x.LocationType.LocTypeName, 
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                ContactPerson = new { x.ContactPerson.FirstName, x.ContactPerson.LastName, x.ContactPerson.Initials },
                x.Address, x.City, x.ZIP, x.State, x.Country, x.LocX, x.LocY, x.LocZ, x.LocStationing,
                x.CertOfApprReqd, x.RightOfEntryReqd, x.AccessInfo ,x.Comments, x.IsActive,
                x.LocationType_Id,x.AssignedToProject_Id, x.ContactPerson_Id
            });

            ViewBag.ServiceName = "LocationService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationSrv/GetByProjectIds
        [HttpPost]
        [DBSrvAuth("Location_View")]
        public async Task<ActionResult> GetByProjectIds(string[] projectIds = null, bool getActive = true)
        {
            var data = (await locationService.GetByProjectAsync(UserId, projectIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.LocName, x.LocAltName, x.LocationType.LocTypeName, 
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                ContactPerson = new { x.ContactPerson.FirstName, x.ContactPerson.LastName, x.ContactPerson.Initials },
                x.Address, x.City, x.ZIP, x.State, x.Country, x.LocX, x.LocY, x.LocZ, x.LocStationing,
                x.CertOfApprReqd, x.RightOfEntryReqd, x.AccessInfo ,x.Comments, x.IsActive,
                x.LocationType_Id,x.AssignedToProject_Id, x.ContactPerson_Id
            });

            ViewBag.ServiceName = "LocationService.GetByProjectAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationSrv/GetByTypeIds
        [HttpPost]
        [DBSrvAuth("Location_View")]
        public async Task<ActionResult> GetByTypeIds(string[] projectIds = null, string[] typeIds = null, bool getActive = true)
        {
            var data = (await locationService.GetByTypeAsync(UserId, projectIds, typeIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.LocName, x.LocAltName, x.LocationType.LocTypeName, 
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                ContactPerson = new { x.ContactPerson.FirstName, x.ContactPerson.LastName, x.ContactPerson.Initials },
                x.Address, x.City, x.ZIP, x.State, x.Country, x.LocX, x.LocY, x.LocZ, x.LocStationing,
                x.CertOfApprReqd, x.RightOfEntryReqd, x.AccessInfo ,x.Comments, x.IsActive,
                x.LocationType_Id,x.AssignedToProject_Id, x.ContactPerson_Id
            });

            ViewBag.ServiceName = "LocationService.GetByTypeAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await locationService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "LocationService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.LocName)
                .Select(x => new { id = x.Id, name = x.LocName + " - " + x.AssignedToProject.ProjectCode }), JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationSrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds = null, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (projectIds != null && projectIds != "") projectIdsArray = projectIds.Split(',');

            var records = await locationService.LookupByProjAsync(UserId, projectIdsArray, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "LocationService.LookupByProjAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.LocName)
                .Select(x => new { id = x.Id, name = x.LocName + " - " + x.AssignedToProject.ProjectCode }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /LocationSrv/Edit
        [HttpPost]
        [DBSrvAuth("Location_Edit")]
        public async Task<ActionResult> Edit(Location[] records)
        {
            var serviceResult = await locationService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "LocationService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /LocationSrv/Delete
        [HttpPost]
        [DBSrvAuth("Location_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await locationService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "LocationService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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