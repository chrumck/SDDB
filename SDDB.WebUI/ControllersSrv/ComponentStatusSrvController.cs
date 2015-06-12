using System.Linq;
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
            var data = (await compStatusService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.CompStatusName, x.CompStatusAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "ComponentStatusService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /ComponentStatusSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("ComponentStatus_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await compStatusService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.CompStatusName, x.CompStatusAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "ComponentStatusService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /ComponentStatusSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await compStatusService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentStatusService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.CompStatusName)
                .Select(x => new { id = x.Id, name = x.CompStatusName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentStatusSrv/Edit
        [HttpPost]
        [DBSrvAuth("ComponentStatus_Edit")]
        public async Task<ActionResult> Edit(ComponentStatus[] records)
        {
            var serviceResult = await compStatusService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentStatusService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /ComponentStatusSrv/Delete
        [HttpPost]
        [DBSrvAuth("ComponentStatus_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await compStatusService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentStatusService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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