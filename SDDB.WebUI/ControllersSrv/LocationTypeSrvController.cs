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
            ViewBag.ServiceName = "LocationTypeService.GetAsync";
            var records = await locationTypeService.GetAsync(getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // POST: /LocationTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("LocationType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "LocationTypeService.GetAsync";
            var records = await locationTypeService.GetAsync(ids, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonFull(records));
        }

        // GET: /LocationTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "LocationTypeService.LookupAsync";
            var records = await locationTypeService.LookupAsync(query, getActive).ConfigureAwait(false);
            return DbJson(filterForJsonLookup(records));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /LocationTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("LocationType_Edit")]
        public async Task<ActionResult> Edit(LocationType[] records)
        {
            ViewBag.ServiceName = "LocationTypeService.EditAsync";
            var newEntryIds = await locationTypeService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /LocationTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("LocationType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "LocationTypeService.DeleteAsync";
            await locationTypeService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<LocationType> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.LocTypeName,
                    x.LocTypeAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<LocationType> records)
        {
            return records
                .OrderBy(x => x.LocTypeName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.LocTypeName
                    }
                );
        }

        #endregion
    }
}