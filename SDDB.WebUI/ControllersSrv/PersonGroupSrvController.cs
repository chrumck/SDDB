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
    public class PersonGroupSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonGroupService personGroupService;
        private PersonService personService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public PersonGroupSrvController(PersonGroupService personGroupService, PersonService personService)
        {
            this.personGroupService = personGroupService;
            this.personService = personService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /PersonGroupSrv/Get
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await personGroupService.GetAsync(getActive).ConfigureAwait(false));

            ViewBag.ServiceName = "PersonGroupService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonGroupSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await personGroupService.GetAsync(ids, getActive).ConfigureAwait(false));

            ViewBag.ServiceName = "PersonGroupService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonGroupSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = (await personGroupService.LookupAsync(UserId, query, getActive).ConfigureAwait(false));

            ViewBag.ServiceName = "PersonGroupService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.PrsGroupName)
                .Select(x => new { id = x.Id, name = x.PrsGroupName}), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonGroupSrv/Edit
        [HttpPost]
        [DBSrvAuth("PersonGroup_Edit")]
        public async Task<ActionResult> Edit(PersonGroup[] records)
        {
            var serviceResult = await personGroupService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonGroupService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /PersonGroupSrv/Delete
        [HttpPost]
        [DBSrvAuth("PersonGroup_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await personGroupService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonGroupService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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

        // GET: /PersonGroupSrv/GetGroupManagers
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetGroupManagers(string id)
        {
            var data = (await personGroupService.GetGroupManagersAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.LastName, x.FirstName, x.Initials });

            ViewBag.ServiceName = "PersonGroupService.GetGroupManagersAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonGroupSrv/GetGroupManagersNot
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetGroupManagersNot(string id)
        {
            var data = (await personGroupService.GetGroupManagersNotAsync(id).ConfigureAwait(false))
                .Select(x => new { x.Id, x.LastName, x.FirstName, x.Initials });

            ViewBag.ServiceName = "PersonGroupService.GetGroupManagersNotAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonGroupSrv/EditGroupManagers
        [HttpPost]
        [DBSrvAuth("PersonGroup_Edit,Person_Edit")]
        public async Task<ActionResult> EditGroupManagers(string[] personIds, string[] groupIds, bool isAdd)
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