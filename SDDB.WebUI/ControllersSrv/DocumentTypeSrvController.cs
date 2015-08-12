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
    public class DocumentTypeSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private DocumentTypeService documentTypeService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DocumentTypeSrvController(DocumentTypeService documentTypeService)
        {
            this.documentTypeService = documentTypeService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /DocumentTypeSrv/Get
        [DBSrvAuth("DocumentType_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            ViewBag.ServiceName = "DocumentTypeService.GetAsync";
            var records = await documentTypeService.GetAsync(getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /DocumentTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("DocumentType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "DocumentTypeService.GetAsync";
            var records = await documentTypeService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /DocumentTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "DocumentTypeService.LookupAsync";
            var records = await documentTypeService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /DocumentTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("DocumentType_Edit")]
        public async Task<ActionResult> Edit(DocumentType[] records)
        {
            ViewBag.ServiceName = "DocumentTypeService.EditAsync";
            var newEntryIds = await documentTypeService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /DocumentTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("DocumentType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "DocumentTypeService.DeleteAsync";
            await documentTypeService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<DocumentType> records)
        {
            return records.Select(x =>
                new
                {
                    x.Id,
                    x.DocTypeName,
                    x.DocTypeAltName,
                    x.Comments,
                    x.IsActive_bl
                }
            )
            .ToList();
        }

        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<DocumentType> records)
        {
            return records
                .OrderBy(x => x.DocTypeName)
                .Select(x =>
                    new
                    {
                        id = x.Id,
                        name = x.DocTypeName
                    }
                );
        }

        #endregion
    }
}