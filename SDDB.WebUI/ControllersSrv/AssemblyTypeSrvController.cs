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
    public class AssemblyTypeSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private AssemblyTypeService assyTypeService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public AssemblyTypeSrvController(AssemblyTypeService assyTypeService)
        {
            this.assyTypeService = assyTypeService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /AssemblyTypeSrv/Get
        [DBSrvAuth("AssemblyType_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyTypeService.GetAsync";
            var records = await assyTypeService.GetAsync(getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("AssemblyType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyTypeService.GetAsync";
            var records = await assyTypeService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyTypeService.LookupAsync";
            var records = await assyTypeService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        
        // POST: /AssemblyTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("AssemblyType_Edit")]
        public async Task<ActionResult> Edit(AssemblyType[] records)
        {
            ViewBag.ServiceName = "AssemblyTypeService.EditAsync";
            var newEntryIds = await assyTypeService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("AssemblyType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "AssemblyTypeService.DeleteAsync";
            await assyTypeService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<AssemblyType> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.AssyTypeName,
                    x.AssyTypeAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<AssemblyType> records)
        {
            return records
                .OrderBy(x => x.AssyTypeName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.AssyTypeName 
                    }
                );
        }

        #endregion
    }
}