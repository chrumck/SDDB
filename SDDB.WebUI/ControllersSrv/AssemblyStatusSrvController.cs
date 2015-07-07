using System.Linq;
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
            var data = (await assyStatusService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.AssyStatusName, x.AssyStatusAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "AssemblyStatusService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyStatusSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("AssemblyStatus_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await assyStatusService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.AssyStatusName, x.AssyStatusAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "AssemblyStatusService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyStatusSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await assyStatusService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyStatusService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.AssyStatusName)
                .Select(x => new { id = x.Id, name = x.AssyStatusName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyStatusSrv/Edit
        [HttpPost]
        [DBSrvAuth("AssemblyStatus_Edit")]
        public async Task<ActionResult> Edit(AssemblyStatus[] records)
        {
            var serviceResult = await assyStatusService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyStatusService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /AssemblyStatusSrv/Delete
        [HttpPost]
        [DBSrvAuth("AssemblyStatus_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await assyStatusService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyStatusService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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