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
            ViewBag.ServiceName = "ComponentTypeService.GetAsync";
            var records = await compTypeService.GetAsync(getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // POST: /ComponentTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Component_View,ComponentType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentTypeService.GetAsync";
            var records = await compTypeService.GetAsync(ids, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // GET: /ComponentTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "ComponentTypeService.LookupAsync";
            var records = await compTypeService.LookupAsync(query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("ComponentType_Edit")]
        public async Task<ActionResult> Edit(ComponentType[] records)
        {
            ViewBag.ServiceName = "ComponentTypeService.EditAsync";
            var newEntryIds = await compTypeService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /ComponentTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("ComponentType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "ComponentTypeService.DeleteAsync";
            await compTypeService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<ComponentType> records)
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
                        x.CompTypeName,
                        x.CompTypeAltName,
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
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<ComponentType> records)
        {
            return records
                .OrderBy(x => x.CompTypeName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.CompTypeName
                    }
                );
        }

        #endregion
    }
}