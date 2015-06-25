using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;
using System;

namespace SDDB.WebUI.ControllersSrv
{
    public class PersonLogEntrySrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonLogEntryService prsLogEntryService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public PersonLogEntrySrvController(PersonLogEntryService prsLogEntryService)
        {
            this.prsLogEntryService = prsLogEntryService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /PersonLogEntrySrv/Get
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await prsLogEntryService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                PersonActivityType_ = new { x.PersonActivityType.ActivityTypeName },
                x.ManHours, 
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                AssignedToProjectEvent_ = new { x.AssignedToProjectEvent.EventName },  
                x.Comments, x.IsActive_bl,
                x.EnteredByPerson_Id ,x.PersonActivityType_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id, x.AssignedToProjectEvent_Id
            });

            ViewBag.ServiceName = "PersonLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
        // GET: /PersonLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await prsLogEntryService.GetAsync(UserId, ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                PersonActivityType_ = new { x.PersonActivityType.ActivityTypeName },
                x.ManHours, 
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                AssignedToProjectEvent_ = new { x.AssignedToProjectEvent.EventName },  
                x.Comments, x.IsActive_bl,
                x.EnteredByPerson_Id ,x.PersonActivityType_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id, x.AssignedToProjectEvent_Id
            });

            ViewBag.ServiceName = "PersonLogEntryService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /PersonLogEntrySrv/GetByFilterIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetByFilterIds(string[] personIds = null, string[] projectIds = null, string[] typeIds = null,
            DateTime? startDate = null, DateTime? endDate = null, bool getActive = true)
        {
            var data = (await prsLogEntryService.GetByFiltersAsync(UserId, personIds, projectIds, typeIds, startDate, endDate, getActive)
                .ConfigureAwait(false)).Select(x => new {
                x.Id, x.LogEntryDateTime,
                EnteredByPerson_ = new { x.EnteredByPerson.FirstName, x.EnteredByPerson.LastName, x.EnteredByPerson.Initials },
                PersonActivityType_ = new { x.PersonActivityType.ActivityTypeName },
                x.ManHours, 
                AssignedToProject_ = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation_ = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName },
                AssignedToProjectEvent_ = new { x.AssignedToProjectEvent.EventName },  
                x.Comments, x.IsActive_bl,
                x.EnteredByPerson_Id ,x.PersonActivityType_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id, x.AssignedToProjectEvent_Id
            });

            ViewBag.ServiceName = "PersonLogEntryService.GetByFiltersAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        // GET: /PersonLogEntrySrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await prsLogEntryService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            var data = records.OrderBy(x => x.LogEntryDateTime)
                .Select(x => new { id = x.Id, name = x.LogEntryDateTime + " " + x.EnteredByPerson.LastName + " " + x.EnteredByPerson.Initials });

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /PersonLogEntrySrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds = null, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (projectIds != null && projectIds != "") projectIdsArray = projectIds.Split(',');

            var records = await prsLogEntryService.LookupByProjAsync(UserId, projectIdsArray, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.LookupByProjAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            var data = records.OrderBy(x => x.LogEntryDateTime)
                .Select(x => new { id = x.Id, name = x.LogEntryDateTime + " " + x.EnteredByPerson.LastName + " " + x.EnteredByPerson.Initials });

            return new DBJsonDateTimeISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonLogEntrySrv/Edit
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit")]
        public async Task<ActionResult> Edit(PersonLogEntry[] records)
        {
            var serviceResult = await prsLogEntryService.EditAsync(UserId,records).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK; return Json(new {
                    Success = "True", ReturnIds = serviceResult.ReturnIds }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription,
                    ReturnIds = serviceResult.ReturnIds }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /PersonLogEntrySrv/Delete
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await prsLogEntryService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/GetPrsLogEntryAssys
        [DBSrvAuth("PersonLogEntry_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssys(string logEntryId)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryAssysAsync(logEntryId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.AssyName, x.AssyAltName });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonLogEntrySrv/GetPrsLogEntryAssysNot
        [DBSrvAuth("PersonLogEntry_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssysNot(string logEntryId, string locId = null)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryAssysNotAsync(logEntryId, locId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.AssyName, x.AssyAltName });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonLogEntrySrv/EditPrsLogEntryAssys
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,Assembly_Edit")]
        public async Task<ActionResult> EditPrsLogEntryAssys(string[] ids, string[] idsAddRem, bool isAdd)
        {
            var serviceResult = await prsLogEntryService.EditPrsLogEntryAssysAsync(ids, idsAddRem, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.EditPrsLogEntryAssysAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/GetPrsLogEntryPersons
        [DBSrvAuth("PersonLogEntry_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersons(string logEntryId)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryPersonsAsync(logEntryId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.FirstName, x.LastName, x.Initials });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonLogEntrySrv/GetPrsLogEntryPersonsNot
        [DBSrvAuth("PersonLogEntry_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersonsNot(string logEntryId)
        {
            var data = (await prsLogEntryService.GetPrsLogEntryPersonsNotAsync(UserId, logEntryId).ConfigureAwait(false))
                .Select(x => new { x.Id, x.FirstName, x.LastName, x.Initials });

            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonLogEntrySrv/EditPrsLogEntryPersons
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,Person_Edit")]
        public async Task<ActionResult> EditPrsLogEntryPersons(string[] ids, string[] idsAddRem, bool isAdd)
        {
            var serviceResult = await prsLogEntryService.EditPrsLogEntryPersonsAsync(ids, idsAddRem, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonLogEntryService.EditPrsLogEntryPersonsAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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