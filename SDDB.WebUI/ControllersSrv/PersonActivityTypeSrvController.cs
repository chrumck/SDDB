using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class PersonActivityTypeSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonActivityTypeService prsActivityTypeService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public PersonActivityTypeSrvController(PersonActivityTypeService prsActivityTypeService)
        {
            this.prsActivityTypeService = prsActivityTypeService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /PersonActivityTypeSrv/Get
        [DBSrvAuth("PersonActivityType_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await prsActivityTypeService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.ActivityTypeName, x.ActivityTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "PersonActivityTypeService.GetAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonActivityTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonActivityType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await prsActivityTypeService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.ActivityTypeName, x.ActivityTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "PersonActivityTypeService.GetAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonActivityTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await prsActivityTypeService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonActivityTypeService.LookupAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json(records.OrderBy(x => x.ActivityTypeName)
                .Select(x => new { id = x.Id, name = x.ActivityTypeName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonActivityTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("PersonActivityType_Edit")]
        public async Task<ActionResult> Edit(PersonActivityType[] records)
        {
            var serviceResult = await prsActivityTypeService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonActivityTypeService.EditAsync";
            ViewBag.StatusCode = serviceResult.StatusCode; 
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)serviceResult.StatusCode;
                return Json(new { Success = "False", responseText = serviceResult.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: /PersonActivityTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("PersonActivityType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await prsActivityTypeService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "PersonActivityTypeService.DeleteAsync";
            ViewBag.StatusCode = serviceResult.StatusCode;
            ViewBag.StatusDescription = serviceResult.StatusDescription;

            if (serviceResult.StatusCode == HttpStatusCode.OK)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
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