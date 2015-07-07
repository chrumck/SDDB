using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

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
            var data = (await assyTypeService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.AssyTypeName, x.AssyTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "AssemblyTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("AssemblyType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await assyTypeService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.AssyTypeName, x.AssyTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "AssemblyTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await assyTypeService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyTypeService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.AssyTypeName)
                .Select(x => new { id = x.Id, name = x.AssyTypeName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------
        
        // POST: /AssemblyTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("AssemblyType_Edit")]
        public async Task<ActionResult> Edit(AssemblyType[] records)
        {
            var serviceResult = await assyTypeService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyTypeService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /AssemblyTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("AssemblyType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await assyTypeService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyTypeService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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