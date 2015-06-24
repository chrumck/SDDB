using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class LocationTypeSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private LocationTypeService locationTypeService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public LocationTypeSrvController(LocationTypeService locationTypeService)
        {
            this.locationTypeService = locationTypeService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /LocationTypeSrv/Get
        [DBSrvAuth("LocationType_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await locationTypeService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LocTypeName, x.LocTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "LocationTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("LocationType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await locationTypeService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.LocTypeName, x.LocTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "LocationTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /LocationTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await locationTypeService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "LocationTypeService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.LocTypeName)
                .Select(x => new { id = x.Id, name = x.LocTypeName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /LocationTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("LocationType_Edit")]
        public async Task<ActionResult> Edit(LocationType[] records)
        {
            var serviceResult = await locationTypeService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "LocationTypeService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /LocationTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("LocationType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await locationTypeService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "LocationTypeService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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