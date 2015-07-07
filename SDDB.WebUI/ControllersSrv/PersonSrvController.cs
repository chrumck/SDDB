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

        // GET: /PersonSrv/Get
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = await personService.GetAsync(getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return new DBJsonDateISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /PersonSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = await personService.GetAsync(ids, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;
            
            return new DBJsonDateISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /PersonSrv/PersonsWoDBUser
        [DBSrvAuth("Person_View,DBUser_View")]
        public async Task<ActionResult> PersonsWoDBUser(bool getActive = true)
        {
            var persons = await personService.GetWoDBUserAsync(getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.GetWoDBUserAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(persons.OrderBy(p => p.LastName)
                .Select(x => new { id = x.Id, name = x.FirstName + " " + x.LastName + " " + x.Initials }), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/LookupAll
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> LookupAll(string query = "", bool getActive = true)
        {
            var records = await personService.LookupAllAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.LookupAllAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.LastName)
                .Select(x => new { id = x.Id, name = x.FirstName + " " + x.LastName + " " + x.Initials }), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await personService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.LastName)
                .Select(x => new { id = x.Id, name = x.FirstName + " " + x.LastName + " " + x.Initials }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonSrv/Edit
        [HttpPost]
        [DBSrvAuth("Person_Edit")]
        public async Task<ActionResult> Edit(Person[] records)
        {
            var serviceResult = await personService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /PersonSrv/Delete
        [HttpPost]
        [DBSrvAuth("Person_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await personService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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

        // GET: /PersonSrv/GetPersonProjects
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> GetPersonProjects(string id)
        {
            var data = (await personService.GetPersonProjectsAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.ProjectName, x.ProjectAltName, x.ProjectCode });

            ViewBag.ServiceName = "PersonService.GetPersonProjectsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/GetPersonProjectsNot
        [DBSrvAuth("Person_View,Project_View")]
        public async Task<ActionResult> GetPersonProjectsNot(string id)
        {
            var data = (await personService.GetPersonProjectsNotAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.ProjectName, x.ProjectAltName, x.ProjectCode });

            ViewBag.ServiceName = "PersonService.GetPersonProjectsNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/EditPersonProjects
        [HttpPost]
        [DBSrvAuth("Person_Edit,Project_Edit")]
        public async Task<ActionResult> EditPersonProjects(string[] personIds, string[] projectIds, bool isAdd)
        {
            var serviceResult = await personService.EditPersonProjectsAsync(personIds, projectIds, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.EditPersonProjectsAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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

        // GET: /PersonSrv/GetPersonGroups
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetPersonGroups(string id)
        {
            var data = (await personService.GetPersonGroupsAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.PrsGroupName, x.PrsGroupAltName });

            ViewBag.ServiceName = "PersonService.GetPersonGroupsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/GetPersonGroupsNot
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetPersonGroupsNot(string id)
        {
            var data = (await personService.GetPersonGroupsNotAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.PrsGroupName, x.PrsGroupAltName });

            ViewBag.ServiceName = "PersonService.GetPersonGroupsNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/EditPersonGroups
        [HttpPost]
        [DBSrvAuth("Person_Edit,PersonGroup_Edit")]
        public async Task<ActionResult> EditPersonGroups(string[] personIds, string[] groupIds, bool isAdd)
        {
            var serviceResult = await personService.EditPersonGroupsAsync(personIds, groupIds, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.EditPersonGroupsAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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

        // GET: /PersonSrv/GetManagedGroups
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetManagedGroups(string id)
        {
            var data = (await personService.GetManagedGroupsAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.PrsGroupName, x.PrsGroupAltName });

            ViewBag.ServiceName = "PersonService.GetManagedGroupsAsyn"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonSrv/GetManagedGroupsNot
        [DBSrvAuth("Person_View,PersonGroup_View")]
        public async Task<ActionResult> GetManagedGroupsNot(string id)
        {
            var data = (await personService.GetManagedGroupsNotAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.PrsGroupName, x.PrsGroupAltName });

            ViewBag.ServiceName = "PersonService.GetManagedGroupsNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonSrv/EditManagedGroups
        [HttpPost]
        [DBSrvAuth("Person_Edit,PersonGroup_Edit")]
        public async Task<ActionResult> EditManagedGroups(string[] personIds, string[] groupIds, bool isAdd)
        {
            var serviceResult = await personService.EditManagedGroupsAsync(personIds, groupIds, isAdd).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonService.EditManagedGroupsAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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