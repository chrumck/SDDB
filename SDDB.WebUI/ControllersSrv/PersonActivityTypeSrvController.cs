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
    public class PersonActivityTypeSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonActivityTypeService personActivityTypeService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public PersonActivityTypeSrvController(PersonActivityTypeService personActivityTypeService)
        {
            this.personActivityTypeService = personActivityTypeService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /PersonActivityTypeSrv/Get
        [DBSrvAuth("PersonActivityType_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "PersonActivityTypeService.GetAsync";
            var records = await personActivityTypeService.GetAsync(getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonActivityTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonActivityType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonActivityTypeService.GetAsync";
            var records = await personActivityTypeService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /PersonActivityTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "PersonActivityTypeService.LookupAsync";
            var records = await personActivityTypeService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonActivityTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("PersonActivityType_Edit")]
        public async Task<ActionResult> Edit(PersonActivityType[] records)
        {
            ViewBag.ServiceName = "PersonActivityTypeService.EditAsync";
            var newEntryIds = await personActivityTypeService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /PersonActivityTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("PersonActivityType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "PersonActivityTypeService.DeleteAsync";
            await personActivityTypeService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<PersonActivityType> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.ActivityTypeName,
                    x.ActivityTypeAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<PersonActivityType> records)
        {
            return records
                .OrderBy(x => x.ActivityTypeName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.ActivityTypeName
                    }
                );
        }

        #endregion
    }
}