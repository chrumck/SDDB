using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;
using System.Collections.Generic;

namespace SDDB.WebUI.ControllersSrv
{
    public class ComponentTypeSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ComponentTypeService compTypeService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ComponentTypeSrvController(ComponentTypeService compTypeService)
        {
            this.compTypeService = compTypeService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /ComponentTypeSrv/Get
        [DBSrvAuth("ComponentType_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentTypeService.GetAsync";
            var records = await compTypeService.GetAsync(getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /ComponentTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("ComponentType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentTypeService.GetAsync";
            var records = await compTypeService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /ComponentTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentTypeService.LookupAsync";
            var records = await compTypeService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("ComponentType_Edit")]
        public async Task<ActionResult> Edit(ComponentType[] records)
        {
            ViewBag.ServiceName = "ComponentTypeService.EditAsync";
            var newEntryIds = await compTypeService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /ComponentTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("ComponentType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ComponentTypeService.DeleteAsync";
            await compTypeService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<ComponentType> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.CompTypeName,
                    x.CompTypeAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<ComponentType> records)
        {
            return records
                .OrderBy(x => x.CompTypeName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.CompTypeName
                    }
                );
        }

        #endregion
    }
}