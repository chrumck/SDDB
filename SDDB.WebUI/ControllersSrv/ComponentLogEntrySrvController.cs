using System;
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

        // GET: /ComponentLogEntrySrv/Get
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await compLogEntryService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id,
                x.LogEntryDateTime,
                Component_ = new { x.Component.CompName, x.Component.CompAltName },
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                ComponentStatus_ = new { x.ComponentStatus.CompStatusName },
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectCode },
                AssignedToAssemblyDb_ = new { x.AssignedToAssemblyDb.AssyName, x.AssignedToAssemblyDb.AssyAltName },
                x.LastCalibrationDate, x.Comments, x.IsActive_bl,
                x.Component_Id, x.EnteredByPerson_Id, x.ComponentStatus_Id, x.AssignedToProject_Id, x.AssignedToAssemblyDb_Id
            });

            ViewBag.ServiceName = "ComponentLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /ComponentLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await compLogEntryService.GetAsync(UserId,ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id,
                x.LogEntryDateTime,
                Component_ = new { x.Component.CompName, x.Component.CompAltName },
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                ComponentStatus_ = new { x.ComponentStatus.CompStatusName },
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectCode },
                AssignedToAssemblyDb_ = new { x.AssignedToAssemblyDb.AssyName, x.AssignedToAssemblyDb.AssyAltName },
                x.LastCalibrationDate, x.Comments, x.IsActive_bl,
                x.Component_Id, x.EnteredByPerson_Id, x.ComponentStatus_Id, x.AssignedToProject_Id, x.AssignedToAssemblyDb_Id
            });

            ViewBag.ServiceName = "ComponentLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /ComponentLogEntrySrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds = null, string[] componentIds = null, string[] personIds = null, bool getActive = true)
        {
            var data = (await compLogEntryService.GetByAltIdsAsync(UserId, projectIds, componentIds, personIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id,
                x.LogEntryDateTime,
                Component_ = new { x.Component.CompName, x.Component.CompAltName },
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                ComponentStatus_ = new { x.ComponentStatus.CompStatusName },
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectCode },
                AssignedToAssemblyDb_ = new { x.AssignedToAssemblyDb.AssyName, x.AssignedToAssemblyDb.AssyAltName },
                x.LastCalibrationDate, x.Comments, x.IsActive_bl,
                x.Component_Id, x.EnteredByPerson_Id, x.ComponentStatus_Id, x.AssignedToProject_Id, x.AssignedToAssemblyDb_Id
            });

            ViewBag.ServiceName = "ComponentLogEntryService.GetByAltIdsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
                
        
        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentLogEntrySrv/Edit
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Edit(ComponentLogEntry[] records)
        {
            var serviceResult = await compLogEntryService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentLogEntryService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /ComponentLogEntrySrv/Delete
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await compLogEntryService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentLogEntryService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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