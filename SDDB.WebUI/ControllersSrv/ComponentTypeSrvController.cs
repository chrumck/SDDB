using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

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
            var data = (await compTypeService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.CompTypeName, x.CompTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "ComponentTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /ComponentTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("ComponentType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await compTypeService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.CompTypeName, x.CompTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "ComponentTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /ComponentTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await compTypeService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentTypeService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.CompTypeName)
                .Select(x => new { id = x.Id, name = x.CompTypeName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        
        // POST: /ComponentTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("ComponentType_Edit")]
        public async Task<ActionResult> Edit(ComponentType[] records)
        {
            var serviceResult = await compTypeService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentTypeService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /ComponentTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("ComponentType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await compTypeService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentTypeService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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