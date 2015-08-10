using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;
using System.Collections.Generic;

namespace SDDB.WebUI.ControllersSrv
{
    public class DocumentSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private DocumentService docService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DocumentSrvController(DocumentService docService)
        {
            this.docService = docService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // POST: /DocumentSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Document_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "DocumentService.GetAsync";
            var records = await docService.GetAsync(ids, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // POST: /DocumentSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Document_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds, string[] typeIds = null, bool getActive = true)
        {
            ViewBag.ServiceName = "DocumentService.GetByAltIdsAsync";
            var records = await docService.GetByAltIdsAsync(projectIds,typeIds, getActive).ConfigureAwait(false);
            return Json(filterForJsonFull(records), JsonRequestBehavior.AllowGet);
        }

        // GET: /DocumentSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            ViewBag.ServiceName = "DocumentService.LookupAsync";
            var records = await docService.LookupAsync(query, getActive).ConfigureAwait(false);
            return Json(filterForJsonLookup(records), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /DocumentSrv/Edit
        [HttpPost]
        [DBSrvAuth("Document_Edit")]
        public async Task<ActionResult> Edit(Document[] records)
        {
            ViewBag.ServiceName = "DocumentService.EditAsync";
            var newEntryIds = await docService.EditAsync(records).ConfigureAwait(false);
            return Json(new { Success = "True", newEntryIds = newEntryIds }, JsonRequestBehavior.AllowGet);
        }

        // POST: /DocumentSrv/Delete
        [HttpPost]
        [DBSrvAuth("Document_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "DocumentService.DeleteAsync";
            await docService.DeleteAsync(ids).ConfigureAwait(false);
            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //filterForJsonFull - filter data from service to be passed as response
        private object filterForJsonFull(List<Document> records)
        {
            return records.Select(x => new
            {
                x.Id,
                x.DocName,
                x.DocAltName,
                DocumentType_ = new
                {
                    x.DocumentType.DocTypeName
                },
                x.DocLastVersion,
                AuthorPerson_ = new
                {
                    x.AuthorPerson.FirstName,
                    x.AuthorPerson.LastName,
                    x.AuthorPerson.Initials
                },
                ReviewerPerson_ = new
                {
                    x.ReviewerPerson.FirstName,
                    x.ReviewerPerson.LastName,
                    x.ReviewerPerson.Initials
                },
                AssignedToProject_ = new
                {
                    x.AssignedToProject.ProjectName,
                    x.AssignedToProject.ProjectAltName,
                    x.AssignedToProject.ProjectCode
                },
                RelatesToAssyType_ = new
                {
                    x.RelatesToAssyType.AssyTypeName
                },
                RelatesToCompType_ = new {
                    x.RelatesToCompType.CompTypeName
                },
                x.DocFilePath,
                x.Comments,
                x.IsActive_bl,
                x.DocumentType_Id,
                x.AuthorPerson_Id,
                x.ReviewerPerson_Id,
                x.AssignedToProject_Id,
                x.RelatesToAssyType_Id,
                x.RelatesToCompType_Id,
            })
            .ToList();
        }


        //filterForJsonLookup - filter data from service to be passed as response
        private object filterForJsonLookup(List<Document> records)
        {
            return records
                .OrderBy(x => x.DocName)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.DocName + " - " + x.AssignedToProject.ProjectName
                })
                .ToList();
        }

        #endregion
    }
}