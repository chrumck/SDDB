using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class ComponentStatusSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ComponentStatusService compStatusService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ComponentStatusSrvController(ComponentStatusService compStatusService)
        {
            this.compStatusService = compStatusService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /ComponentStatusSrv/Get
        [DBSrvAuth("ComponentStatus_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentStatusService.GetAsync";
            var records = await compStatusService.GetAsync(getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // POST: /ComponentStatusSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("ComponentStatus_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentStatusService.GetAsync";
            var records = await compStatusService.GetAsync(ids, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // GET: /ComponentStatusSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentStatusService.LookupAsync";
            var records = await compStatusService.LookupAsync(query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentStatusSrv/Edit
        [HttpPost]
        [DBSrvAuth("ComponentStatus_Edit")]
        public async Task<ActionResult> Edit(ComponentStatus[] records)
        {
            ViewBag.ServiceName = "ComponentStatusService.EditAsync";
            var newEntryIds = await compStatusService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /ComponentStatusSrv/Delete
        [HttpPost]
        [DBSrvAuth("ComponentStatus_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ComponentStatusService.DeleteAsync";
            await compStatusService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<ComponentStatus> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.CompStatusName,
                    x.CompStatusAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<ComponentStatus> records)
        {
            return records
                .OrderBy(x => x.CompStatusName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.CompStatusName
                    }
                );
        }


        #endregion
    }
}