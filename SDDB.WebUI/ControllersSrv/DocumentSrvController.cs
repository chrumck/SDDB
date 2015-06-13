using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

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

        // GET: /DocumentSrv/Get
        [DBSrvAuth("Document_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await docService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.DocName, x.DocAltName, x.DocumentType.DocTypeName, x.DocLastVersion, 
                AuthorPerson = new { x.AuthorPerson.FirstName, x.AuthorPerson.LastName, x.AuthorPerson.Initials },
                ReviewerPerson = new { x.ReviewerPerson.FirstName, x.ReviewerPerson.LastName, x.ReviewerPerson.Initials },
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                x.RelatesToAssyType.AssyTypeName, x.RelatesToCompType.CompTypeName, x.DocFilePath, 
                x.Comments, x.IsActive,
                x.DocumentType_Id, x.AuthorPerson_Id, x.ReviewerPerson_Id, x.AssignedToProject_Id, x.RelatesToAssyType_Id, x.RelatesToCompType_Id, 
            });

            ViewBag.ServiceName = "DocumentService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /DocumentSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Document_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await docService.GetAsync(UserId, ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.DocName, x.DocAltName, x.DocumentType.DocTypeName, x.DocLastVersion, 
                AuthorPerson = new { x.AuthorPerson.FirstName, x.AuthorPerson.LastName, x.AuthorPerson.Initials },
                ReviewerPerson = new { x.ReviewerPerson.FirstName, x.ReviewerPerson.LastName, x.ReviewerPerson.Initials },
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                x.RelatesToAssyType.AssyTypeName, x.RelatesToCompType.CompTypeName, x.DocFilePath, 
                x.Comments, x.IsActive,
                x.DocumentType_Id,x.AuthorPerson_Id,x.ReviewerPerson_Id,x.AssignedToProject_Id,x.RelatesToAssyType_Id,x.RelatesToCompType_Id, 
            });

            ViewBag.ServiceName = "DocumentService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

         // GET: /DocumentSrv/GetByTypeIds
        [HttpPost]
        [DBSrvAuth("Document_View")]
        public async Task<ActionResult> GetByTypeIds(string[] projectIds, string[] typeIds = null, bool getActive = true)
        {
            var data = (await docService.GetByTypeAsync(UserId, projectIds,typeIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.DocName, x.DocAltName, x.DocumentType.DocTypeName, x.DocLastVersion, 
                AuthorPerson = new { x.AuthorPerson.FirstName, x.AuthorPerson.LastName, x.AuthorPerson.Initials },
                ReviewerPerson = new { x.ReviewerPerson.FirstName, x.ReviewerPerson.LastName, x.ReviewerPerson.Initials },
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                x.RelatesToAssyType.AssyTypeName, x.RelatesToCompType.CompTypeName, x.DocFilePath, 
                x.Comments, x.IsActive,
                x.DocumentType_Id,x.AuthorPerson_Id,x.ReviewerPerson_Id,x.AssignedToProject_Id,x.RelatesToAssyType_Id,x.RelatesToCompType_Id, 
            });

            ViewBag.ServiceName = "DocumentService.GetByTypeAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /DocumentSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await docService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "DocumentService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.DocName)
                .Select(x => new { id = x.Id, name = x.DocName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /DocumentSrv/Edit
        [HttpPost]
        [DBSrvAuth("Document_Edit")]
        public async Task<ActionResult> Edit(Document[] records)
        {
            var serviceResult = await docService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "DocumentService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /DocumentSrv/Delete
        [HttpPost]
        [DBSrvAuth("Document_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await docService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "DocumentService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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