using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class DocumentTypeSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private DocumentTypeService docTypeService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DocumentTypeSrvController(DocumentTypeService docTypeService)
        {
            this.docTypeService = docTypeService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /DocumentTypeSrv/Get
        [DBSrvAuth("DocumentType_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await docTypeService.GetAsync(getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.DocTypeName, x.DocTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "DocumentTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /DocumentTypeSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("DocumentType_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await docTypeService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.DocTypeName, x.DocTypeAltName, x.Comments, x.IsActive
            });

            ViewBag.ServiceName = "DocumentTypeService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /DocumentTypeSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await docTypeService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "DocumentTypeService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.DocTypeName)
                .Select(x => new { id = x.Id, name = x.DocTypeName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /DocumentTypeSrv/Edit
        [HttpPost]
        [DBSrvAuth("DocumentType_Edit")]
        public async Task<ActionResult> Edit(DocumentType[] records)
        {
            var serviceResult = await docTypeService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "DocumentTypeService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /DocumentTypeSrv/Delete
        [HttpPost]
        [DBSrvAuth("DocumentType_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await docTypeService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "DocumentTypeService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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