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
    public class ProjectEventSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ProjectEventService projectEventService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ProjectEventSrvController(ProjectEventService projectEventService)
        {
            this.projectEventService = projectEventService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /ProjectEventSrv/Get
        [DBSrvAuth("ProjectEvent_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "ProjectEventService.GetAsync";
            var records = await projectEventService.GetAsync(getActive).ConfigureAwait(false);
            return new DBJsonDateTimeISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /ProjectEventSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("ProjectEvent_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ProjectEventService.GetAsync";
            var records = await projectEventService.GetAsync(ids, getActive).ConfigureAwait(false);
            return new DBJsonDateTimeISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /ProjectEventSrv/GetByProjectIds
        [HttpPost]
        [DBSrvAuth("ProjectEvent_View")]
        public async Task<ActionResult> GetByProjectIds(string[] projectIds, bool getActive = true)
        {
            ViewBag.ServiceName = "ProjectEventService.GetByProjectAsync";
            var records = await projectEventService.GetByProjectAsync(projectIds, getActive).ConfigureAwait(false);
            return new DBJsonDateTimeISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /ProjectEventSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ProjectEventService.LookupAsync";
            var records = await projectEventService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectEventSrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds, string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ProjectEventService.LookupByProjAsync";
            
            string[] projectIdsArray = null;
            if (projectIds != null && projectIds != "") projectIdsArray = projectIds.Split(',');
            var records = await projectEventService.LookupByProjAsync(projectIdsArray, query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ProjectEventSrv/Edit
        [HttpPost]
        [DBSrvAuth("ProjectEvent_Edit")]
        public async Task<ActionResult> Edit(ProjectEvent[] records)
        {
            ViewBag.ServiceName = "ProjectEventService.EditAsync";
            var newEntryIds = await projectEventService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /ProjectEventSrv/Delete
        [HttpPost]
        [DBSrvAuth("ProjectEvent_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ProjectEventService.DeleteAsync";
            await projectEventService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<ProjectEvent> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.EventName,
                x.EventAltName,
                AssignedToProject_ = new {
                    x.AssignedToProject.ProjectName,
                    x.AssignedToProject.ProjectAltName,
                    x.AssignedToProject.ProjectCode
                },
                x.EventCreated,
                CreatedByPerson_ = new {
                    x.CreatedByPerson.FirstName,
                    x.CreatedByPerson.LastName,
                    x.CreatedByPerson.Initials
                },
                x.EventClosed,
                ClosedByPerson_ = new {
                    x.ClosedByPerson.FirstName,
                    x.ClosedByPerson.LastName,
                    x.ClosedByPerson.Initials
                },
                x.Comments,
                x.IsActive_bl,
                x.AssignedToProject_Id,
                x.CreatedByPerson_Id,
                x.ClosedByPerson_Id,
            })
            .ToList();
        }


        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<ProjectEvent> records)
        {
            return records
                .OrderBy(x => x.EventName)
                .Select(x => new {
                    id = x.Id,
                    name = x.EventName 
                })
                .ToList();
        }

        #endregion
    }
}