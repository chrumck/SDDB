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
            ViewBag.ServiceName = "ProjectService.GetAsync";
            var records = await projectService.GetAsync(getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Project_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ProjectService.GetAsync";
            var records = await projectService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ProjectService.LookupAsync";
            var records = await projectService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ProjectSrv/Edit
        [HttpPost]
        [DBSrvAuth("Project_Edit")]
        public async Task<ActionResult> Edit(Project[] records)
        {
            ViewBag.ServiceName = "ProjectService.EditAsync";
            var newEntryIds = await projectService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /ProjectSrv/Delete
        [HttpPost]
        [DBSrvAuth("Project_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ProjectService.DeleteAsync";
            await projectService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }


        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /ProjectSrv/GetProjectPersons
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> GetProjectPersons(string id)
        {
            ViewBag.ServiceName = "ProjectService.GetProjectPersonsAsync";
            var records = await projectService.GetProjectPersonsAsync(id).ConfigureAwait(false);
            return Json(filterForJsonPersons(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /ProjectSrv/GetProjectPersonsNot
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> GetProjectPersonsNot(string id)
        {
            ViewBag.ServiceName = "ProjectService.GetProjectPersonsNotAsync";
            var records = await projectService.GetProjectPersonsNotAsync(id).ConfigureAwait(false);
            return Json(filterForJsonPersons(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/EditProjectPersons
        [HttpPost]
        [DBSrvAuth("Person_Edit,Project_Edit")]
        public async Task<ActionResult> EditProjectPersons(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "ProjectService.AddRemoveRelated";
            await projectService.AddRemoveRelated(ids, idsAddRem, x => x.ProjectPersons, isAdd).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }
        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<Project> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.ProjectName,
                x.ProjectAltName,
                x.ProjectCode,
                ProjectManager_ = new { x.ProjectManager.LastName, x.ProjectManager.FirstName, x.ProjectManager.Initials },
                x.Comments,
                x.IsActive_bl,
                x.ProjectManager_Id
            })
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<Project> records)
        {
            return records
                .OrderBy(x => x.ProjectName)
                .Select(x => new {
                    id = x.Id,
                    name = x.ProjectName + " " + x.ProjectCode
                })
                .ToList();
        }

        //filterForJsonPersons - filter data from service to be passed as response
        private object filterForJsonPersons(List<Person> records)
        {
            return records
                .Select(x => new
                {
                    x.Id,
                    x.LastName,
                    x.FirstName,
                    x.Initials
                });
        }

        #endregion
    }
}