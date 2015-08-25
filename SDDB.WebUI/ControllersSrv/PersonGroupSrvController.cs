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
    public class PersonGroupSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonGroupService personGroupService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public PersonGroupSrvController(PersonGroupService personGroupService)
        {
            this.personGroupService = personGroupService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /PersonGroupSrv/Get
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "PersonGroupService.GetAsync"; 
            var records = await personGroupService.GetAsync(getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // POST: /PersonGroupSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonGroupService.GetAsync";
            var records = await personGroupService.GetAsync(ids, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // GET: /PersonGroupSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "PersonGroupService.LookupAsync";
            var records = await personGroupService.LookupAsync(query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonGroupSrv/Edit
        [HttpPost]
        [DBSrvAuth("PersonGroup_Edit")]
        public async Task<ActionResult> Edit(PersonGroup[] records)
        {
            ViewBag.ServiceName = "PersonGroupService.EditAsync";
            var newEntryIds = await personGroupService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /PersonGroupSrv/Delete
        [HttpPost]
        [DBSrvAuth("PersonGroup_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "PersonGroupService.DeleteAsync";
            await personGroupService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonGroupSrv/GetGroupManagers
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetGroupManagers(string id)
        {
            ViewBag.ServiceName = "PersonGroupService.GetGroupManagersAsync";
            var records = await personGroupService.GetGroupManagersAsync(id).ConfigureAwait(false);
            return DbJson(filterForJsonPersons(records));
        }

        // GET: /PersonGroupSrv/GetGroupManagersNot
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetGroupManagersNot(string id)
        {
            ViewBag.ServiceName = "PersonGroupService.GetGroupManagersNotAsync";
            var records = await personGroupService.GetGroupManagersNotAsync(id).ConfigureAwait(false);
            return DbJson(filterForJsonPersons(records));
        }

        // POST: /PersonGroupSrv/EditGroupManagers
        [HttpPost]
        [DBSrvAuth("PersonGroup_Edit,Person_Edit")]
        public async Task<ActionResult> EditGroupManagers(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "PersonService.AddRemoveRelated";
            await personGroupService.AddRemoveRelated(ids, idsAddRem, x => x.GroupManagers, isAdd).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonGroupSrv/GetGroupPersons
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetGroupPersons(string id)
        {
            ViewBag.ServiceName = "PersonGroupService.GetGroupPersonsAsync";
            var records = await personGroupService.GetGroupPersonsAsync(id).ConfigureAwait(false);
            return DbJson(filterForJsonPersons(records));
        }

        // GET: /PersonGroupSrv/GetGroupPersonsNot
        [DBSrvAuth("PersonGroup_View,Person_View")]
        public async Task<ActionResult> GetGroupPersonsNot(string id)
        {
            ViewBag.ServiceName = "PersonGroupService.GetGroupPersonsNotAsync";
            var records = await personGroupService.GetGroupPersonsNotAsync(id).ConfigureAwait(false);
            return DbJson(filterForJsonPersons(records));
        }

        // POST: /PersonGroupSrv/EditGroupPersons
        [HttpPost]
        [DBSrvAuth("PersonGroup_Edit,Person_Edit")]
        public async Task<ActionResult> EditGroupPersons(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "PersonService.AddRemoveRelated";
            await personGroupService.AddRemoveRelated(ids, idsAddRem, x => x.GroupPersons, isAdd).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<PersonGroup> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.PrsGroupName,
                    x.PrsGroupAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<PersonGroup> records)
        {
            return records
                .OrderBy(x => x.PrsGroupName)
                .Select(x => new 
                {
                    id = x.Id, 
                    name = x.PrsGroupName
                });
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