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
    public class PersonSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonService personService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public PersonSrvController(PersonService personService)
        {
            this.personService = personService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /PersonSrv/GetAll
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetAll(bool getActive = true)
        {
            ViewBag.ServiceName = "PersonService.GetAllAsync";
            var records = await personService.GetAllAsync(getActive).ConfigureAwait(false);
            return new DBJsonDateISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /PersonSrv/GetAllByIds
        [HttpPost]
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetAllByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonService.GetAllAsync"; 
            var records = await personService.GetAllAsync(ids, getActive).ConfigureAwait(false);
            return new DBJsonDateISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /PersonSrv/PersonsWoDBUser
        [DBSrvAuth("Person_View,DBUser_View")]
        public async Task<ActionResult> PersonsWoDBUser(bool getActive = true)
        {
            ViewBag.ServiceName = "PersonService.GetWoDBUserAsync";
            var records = await personService.GetWoDBUserAsync(getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/LookupAll
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> LookupAll(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "PersonService.LookupAllAsync";
            var records = await personService.LookupAllAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/Get
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "PersonService.GetAsync";
            var records = await personService.GetAsync(getActive).ConfigureAwait(false);
            return new DBJsonDateISO { Data = filterForJsonFull(records), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /PersonSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "PersonService.LookupAsync";
            var records = await personService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records) , JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonSrv/Edit
        [HttpPost]
        [DBSrvAuth("Person_Edit")]
        public async Task<ActionResult> Edit(Person[] records)
        {
            ViewBag.ServiceName = "PersonService.EditAsync";
            var newEntryIds = await personService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/Delete
        [HttpPost]
        [DBSrvAuth("Person_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "PersonService.DeleteAsync";
            await personService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonSrv/GetPersonProjects
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> GetPersonProjects(string id)
        {
            ViewBag.ServiceName = "PersonService.GetPersonProjectsAsync"; 
            var records = await personService.GetPersonProjectsAsync(id).ConfigureAwait(false);
            return Json(filterForJsonProjects(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/GetPersonProjectsNot
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> GetPersonProjectsNot(string id)
        {
            ViewBag.ServiceName = "PersonService.GetPersonProjectsNotAsync";
            var records = await personService.GetPersonProjectsNotAsync(id).ConfigureAwait(false);
            return Json(filterForJsonProjects(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/EditPersonProjects
        [HttpPost]
        [DBSrvAuth("Person_Edit,Project_Edit")]
        public async Task<ActionResult> EditPersonProjects(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "PersonService.EditPersonProjectsAsync";
            await personService.AddRemoveRelated(ids, idsAddRem, x => x.PersonProjects, isAdd).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonSrv/GetPersonGroups
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetPersonGroups(string id)
        {
            ViewBag.ServiceName = "PersonService.GetPersonGroupsAsync"; 
            var records = await personService.GetPersonGroupsAsync(id).ConfigureAwait(false);
            return Json(filterForJsonPrsGroups(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/GetPersonGroupsNot
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetPersonGroupsNot(string id)
        {
            ViewBag.ServiceName = "PersonService.GetPersonGroupsNotAsync";
            var records = await personService.GetPersonGroupsNotAsync(id).ConfigureAwait(false);
            return Json(filterForJsonPrsGroups(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/EditPersonGroups
        [HttpPost]
        [DBSrvAuth("Person_Edit,PersonGroup_Edit")]
        public async Task<ActionResult> EditPersonGroups(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "PersonService.EditPersonGroupsAsync";
            await personService.AddRemoveRelated(ids, idsAddRem, x => x.PersonGroups, isAdd).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonSrv/GetManagedGroups
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetManagedGroups(string id)
        {
            ViewBag.ServiceName = "PersonService.GetManagedGroupsAsyn";
            var records = await personService.GetManagedGroupsAsync(id).ConfigureAwait(false);
            return Json(filterForJsonPrsGroups(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/GetManagedGroupsNot
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetManagedGroupsNot(string id)
        {
            ViewBag.ServiceName = "PersonService.GetManagedGroupsNotAsync";
            var records = await personService.GetManagedGroupsNotAsync(id).ConfigureAwait(false);
            return Json(filterForJsonPrsGroups(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/EditManagedGroups
        [HttpPost]
        [DBSrvAuth("Person_Edit,PersonGroup_Edit")]
        public async Task<ActionResult> EditManagedGroups(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "PersonService.EditManagedGroupsAsync";
            await personService.AddRemoveRelated(ids, idsAddRem, x => x.ManagedGroups, isAdd).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<Person> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.LastName,
                    x.FirstName,
                    x.Initials,
                    x.Phone,
                    x.PhoneMobile,
                    x.Email,
                    x.Comments,
                    x.IsActive_bl,
                    x.IsCurrentEmployee_bl,
                    x.EmployeePosition,
                    x.IsSalaried_bl,
                    x.EmployeeStart,
                    x.EmployeeEnd,
                    x.EmployeeDetails
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<Person> records)
        {
            return records
                .OrderBy(x => x.LastName)
                .Select(x => new {
                    id = x.Id,
                    name = x.FirstName + " " + x.LastName + " " + x.Initials 
                });
        }

        //filterForJsonProjects - filter data from service to be passed as response
        private object filterForJsonProjects(List<Project> records)
        {
            return records
                .Select(x => new {
                    x.Id,
                    x.ProjectName,
                    x.ProjectAltName,
                    x.ProjectCode 
                });
        }

        //filterForJsonPrsGroups - filter data from service to be passed as response
        private object filterForJsonPrsGroups(List<PersonGroup> records)
        {
            return records
                .Select(x => new {
                    x.Id,
                    x.PrsGroupName,
                    x.PrsGroupAltName
                });
        }

        #endregion
    }
}