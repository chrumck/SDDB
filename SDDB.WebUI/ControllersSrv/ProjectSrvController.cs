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
    public class ProjectSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ProjectService projectService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ProjectSrvController(ProjectService projectService)
        {
            this.projectService = projectService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /ProjectSrv/Get
        [DBSrvAuth("Project_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await projectService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.ProjectName, x.ProjectAltName, x.ProjectCode, x.ProjectManager_Id, 
                ProjectManager = new { x.ProjectManager.LastName, x.ProjectManager.FirstName } ,
                x.Comments, x.IsActive});

            ViewBag.ServiceName = "ProjectService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Project_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await projectService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.ProjectName, x.ProjectAltName, x.ProjectCode, x.ProjectManager_Id,
                ProjectManager = new { x.ProjectManager.LastName, x.ProjectManager.FirstName },
                x.Comments, x.IsActive});

            ViewBag.ServiceName = "ProjectService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await projectService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "ProjectService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.ProjectName)
                .Select(x => new { id = x.Id, name = x.ProjectName + " " + x.ProjectCode }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ProjectSrv/Edit
        [HttpPost]
        [DBSrvAuth("Project_Edit")]
        public async Task<ActionResult> Edit(Project[] records)
        {
            var serviceResult = await projectService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "ProjectService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /ProjectSrv/Delete
        [HttpPost]
        [DBSrvAuth("Project_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await projectService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "ProjectService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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