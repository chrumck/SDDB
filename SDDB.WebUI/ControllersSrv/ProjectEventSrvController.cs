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
            var data = (await projectEventService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.EventName, x.EventAltName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                x.EventCreated, CreatedByPerson = new { x.CreatedByPerson.FirstName, x.CreatedByPerson.LastName, x.CreatedByPerson.Initials },
                x.EventClosed, ClosedByPerson = new { x.ClosedByPerson.FirstName, x.ClosedByPerson.LastName, x.ClosedByPerson.Initials },
                x.Comments, x.IsActive, x.AssignedToProject_Id, x.CreatedByPerson_Id, x.ClosedByPerson_Id, 
            });

            ViewBag.ServiceName = "ProjectEventService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectEventSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("ProjectEvent_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await projectEventService.GetAsync(UserId, ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.EventName, x.EventAltName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                x.EventCreated, CreatedByPerson = new { x.CreatedByPerson.FirstName, x.CreatedByPerson.LastName, x.CreatedByPerson.Initials },
                x.EventClosed, ClosedByPerson = new { x.ClosedByPerson.FirstName, x.ClosedByPerson.LastName, x.ClosedByPerson.Initials },
                x.Comments, x.IsActive, x.AssignedToProject_Id, x.CreatedByPerson_Id, x.ClosedByPerson_Id, 
            });

            ViewBag.ServiceName = "ProjectEventService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectEventSrv/GetByProjectIds
        [HttpPost]
        [DBSrvAuth("ProjectEvent_View")]
        public async Task<ActionResult> GetByProjectIds(string[] projectIds, bool getActive = true)
        {
            var data = (await projectEventService.GetByProjectAsync(UserId, projectIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.EventName, x.EventAltName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                x.EventCreated, CreatedByPerson = new { x.CreatedByPerson.FirstName, x.CreatedByPerson.LastName, x.CreatedByPerson.Initials },
                x.EventClosed, ClosedByPerson = new { x.ClosedByPerson.FirstName, x.ClosedByPerson.LastName, x.ClosedByPerson.Initials },
                x.Comments, x.IsActive, x.AssignedToProject_Id, x.CreatedByPerson_Id, x.ClosedByPerson_Id, 
            });

            ViewBag.ServiceName = "ProjectEventService.GetByProjectAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectEventSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await projectEventService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "ProjectEventService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.EventName)
                .Select(x => new { id = x.Id, name = x.EventName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ProjectEventSrv/Edit
        [HttpPost]
        [DBSrvAuth("ProjectEvent_Edit")]
        public async Task<ActionResult> Edit(ProjectEvent[] records)
        {
            var serviceResult = await projectEventService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "ProjectEventService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /ProjectEventSrv/Delete
        [HttpPost]
        [DBSrvAuth("ProjectEvent_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await projectEventService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "ProjectEventService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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