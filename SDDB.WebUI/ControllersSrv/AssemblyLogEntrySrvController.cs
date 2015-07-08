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

        // GET: /AssemblyLogEntrySrv/Get
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await assyLogEntryService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                Assembly_ = new { x.AssemblyDb.AssyName, x.AssemblyDb.AssyAltName },
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                AssemblyStatus_ = new { x.AssemblyStatus.AssyStatusName },
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                x.AssyGlobalX, x.AssyGlobalY, x.AssyGlobalZ,
                x.AssyLocalXDesign, x.AssyLocalYDesign, x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt, x.AssyLocalYAsBuilt, x.AssyLocalZAsBuilt,
                x.AssyStationing, x.AssyLength, x.Comments, x.IsActive_bl,
                x.AssemblyDb_Id, x.EnteredByPerson_Id, x.AssemblyStatus_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id
            });

            ViewBag.ServiceName = "AssemblyLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /AssemblyLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await assyLogEntryService.GetAsync(UserId,ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                Assembly_ = new { x.AssemblyDb.AssyName, x.AssemblyDb.AssyAltName },
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                AssemblyStatus_ = new { x.AssemblyStatus.AssyStatusName },
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                x.AssyGlobalX, x.AssyGlobalY, x.AssyGlobalZ,
                x.AssyLocalXDesign, x.AssyLocalYDesign, x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt, x.AssyLocalYAsBuilt, x.AssyLocalZAsBuilt,
                x.AssyStationing, x.AssyLength, x.Comments, x.IsActive_bl,
                x.AssemblyDb_Id, x.EnteredByPerson_Id, x.AssemblyStatus_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id
            });

            ViewBag.ServiceName = "AssemblyLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /AssemblyLogEntrySrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds = null, string[] assyIds = null, string[] personIds = null,
            DateTime? startDate = null, DateTime? endDate = null, bool getActive = true)
        {
            var data = (await assyLogEntryService.GetByAltIdsAsync(UserId, projectIds, assyIds, personIds, startDate, endDate, getActive)
                .ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                Assembly_ = new { x.AssemblyDb.AssyName, x.AssemblyDb.AssyAltName },
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                AssemblyStatus_ = new { x.AssemblyStatus.AssyStatusName },
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                x.AssyGlobalX, x.AssyGlobalY, x.AssyGlobalZ,
                x.AssyLocalXDesign, x.AssyLocalYDesign, x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt, x.AssyLocalYAsBuilt, x.AssyLocalZAsBuilt,
                x.AssyStationing, x.AssyLength, x.Comments, x.IsActive_bl,
                x.AssemblyDb_Id, x.EnteredByPerson_Id, x.AssemblyStatus_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id
            });

            ViewBag.ServiceName = "AssemblyLogEntryService.GetByAltIdsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
                
        
        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyLogEntrySrv/Edit
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Edit(AssemblyLogEntry[] records)
        {
            var serviceResult = await assyLogEntryService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyLogEntryService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /AssemblyLogEntrySrv/Delete
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await assyLogEntryService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyLogEntryService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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