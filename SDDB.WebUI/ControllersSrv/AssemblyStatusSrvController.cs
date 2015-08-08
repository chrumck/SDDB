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
    public class AssemblyStatusSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private AssemblyStatusService assyStatusService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public AssemblyStatusSrvController(AssemblyStatusService assyStatusService)
        {
            this.assyStatusService = assyStatusService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /AssemblyStatusSrv/Get
        [DBSrvAuth("AssemblyStatus_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyStatusService.GetAsync";
            var records = await assyStatusService.GetAsync(getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyStatusSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("AssemblyStatus_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyStatusService.GetAsync";
            var records = await assyStatusService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyStatusSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyStatusService.LookupAsync";
            var records = await assyStatusService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyStatusSrv/Edit
        [HttpPost]
        [DBSrvAuth("AssemblyStatus_Edit")]
        public async Task<ActionResult> Edit(AssemblyStatus[] records)
        {
            ViewBag.ServiceName = "AssemblyStatusService.EditAsync";
            var newEntryIds = await assyStatusService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyStatusSrv/Delete
        [HttpPost]
        [DBSrvAuth("AssemblyStatus_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "AssemblyStatusService.DeleteAsync";
            await assyStatusService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<AssemblyStatus> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.AssyStatusName,
                    x.AssyStatusAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<AssemblyStatus> records)
        {
            return records
                .OrderBy(x => x.AssyStatusName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.AssyStatusName
                    }
                );
        }


        #endregion
    }
}