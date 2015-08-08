using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class ComponentModelSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ComponentModelService compModelService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ComponentModelSrvController(ComponentModelService compModelService)
        {
            this.compModelService = compModelService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /ComponentModelSrv/Get
        [DBSrvAuth("ComponentModel_View,Component_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentModelService.GetAsync";
            var records = (await compModelService.GetAsync(getActive).ConfigureAwait(false));
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /ComponentModelSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("ComponentModel_View,Component_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentModelService.GetAsync";
            var records = (await compModelService.GetAsync(ids, getActive).ConfigureAwait(false));
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /ComponentModelSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentModelService.LookupAsync";
            var records = await compModelService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentModelSrv/Edit
        [HttpPost]
        [DBSrvAuth("ComponentModel_Edit")]
        public async Task<ActionResult> Edit(ComponentModel[] records)
        {
            ViewBag.ServiceName = "ComponentModelService.EditAsync";
            var newEntryIds = await compModelService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /ComponentModelSrv/Delete
        [HttpPost]
        [DBSrvAuth("ComponentModel_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ComponentModelService.DeleteAsync";
            await compModelService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<ComponentModel> records)
        {
            return records.Select(x =>
            {
                var Attr01Type = x.Attr01Type.ToString();
                var Attr02Type = x.Attr02Type.ToString();
                var Attr03Type = x.Attr03Type.ToString();
                var Attr04Type = x.Attr04Type.ToString();
                var Attr05Type = x.Attr05Type.ToString();
                var Attr06Type = x.Attr06Type.ToString();
                var Attr07Type = x.Attr07Type.ToString();
                var Attr08Type = x.Attr08Type.ToString();
                var Attr09Type = x.Attr09Type.ToString();
                var Attr10Type = x.Attr10Type.ToString();
                var Attr11Type = x.Attr11Type.ToString();
                var Attr12Type = x.Attr12Type.ToString();
                var Attr13Type = x.Attr13Type.ToString();
                var Attr14Type = x.Attr14Type.ToString();
                var Attr15Type = x.Attr15Type.ToString();

                return new
                {
                    x.Id,
                    x.CompModelName,
                    x.CompModelAltName,
                    x.Comments,
                    x.IsActive_bl,
                    Attr01Type,
                    x.Attr01Desc,
                    Attr02Type,
                    x.Attr02Desc,
                    Attr03Type,
                    x.Attr03Desc,
                    Attr04Type,
                    x.Attr04Desc,
                    Attr05Type,
                    x.Attr05Desc,
                    Attr06Type,
                    x.Attr06Desc,
                    Attr07Type,
                    x.Attr07Desc,
                    Attr08Type,
                    x.Attr08Desc,
                    Attr09Type,
                    x.Attr09Desc,
                    Attr10Type,
                    x.Attr10Desc,
                    Attr11Type,
                    x.Attr11Desc,
                    Attr12Type,
                    x.Attr12Desc,
                    Attr13Type,
                    x.Attr13Desc,
                    Attr14Type,
                    x.Attr14Desc,
                    Attr15Type,
                    x.Attr15Desc
                };
            })
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<ComponentModel> records)
        {
            return records
                .OrderBy(x => x.CompModelName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.CompModelName
                    });
        }


        #endregion
    }
}