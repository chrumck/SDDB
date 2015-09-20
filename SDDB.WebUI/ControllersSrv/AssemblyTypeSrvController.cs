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
            ViewBag.ServiceName = "AssemblyTypeService.GetAsync";
            var records = await assyTypeService.GetAsync(getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // POST: /AssemblyTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("AssemblyType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyTypeService.GetAsync";
            var records = await assyTypeService.GetAsync(ids, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // GET: /AssemblyTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "AssemblyTypeService.LookupAsync";
            var records = await assyTypeService.LookupAsync(query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------
        
        // POST: /AssemblyTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("AssemblyType_Edit")]
        public async Task<ActionResult> Edit(AssemblyType[] records)
        {
            ViewBag.ServiceName = "AssemblyTypeService.EditAsync";
            var newEntryIds = await assyTypeService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /AssemblyTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("AssemblyType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "AssemblyTypeService.DeleteAsync";
            await assyTypeService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<AssemblyType> records)
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
                        x.AssyTypeName,
                        x.AssyTypeAltName,
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
        private object filterForJsonLookup(List<AssemblyType> records)
        {
            return records
                .OrderBy(x => x.AssyTypeName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.AssyTypeName 
                    }
                );
        }

        #endregion
    }
}