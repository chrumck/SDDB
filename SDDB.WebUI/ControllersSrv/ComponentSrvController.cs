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
    public class ComponentSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private ComponentService componentService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public ComponentSrvController(ComponentService componentService)
        {
            this.componentService = componentService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /ComponentSrv/Get
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await componentService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.CompName, x.CompAltName, x.CompAltName2, 
                x.ComponentType.CompTypeName, x.ComponentStatus.CompStatusName, x.ComponentModel.CompModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToAssemblyDb = new { x.AssignedToAssemblyDb.AssyName, x.AssignedToAssemblyDb.AssyAltName },
                x.PositionInAssy, x.ProgramAddress, x.CalibrationReqd, x.LastCalibrationDate, x.Comments, x.IsActive,
                x.ComponentExt.Attr01,x.ComponentExt.Attr02,x.ComponentExt.Attr03,x.ComponentExt.Attr04,x.ComponentExt.Attr05,
                x.ComponentExt.Attr06,x.ComponentExt.Attr07,x.ComponentExt.Attr08,x.ComponentExt.Attr09,x.ComponentExt.Attr10,
                x.ComponentExt.Attr11,x.ComponentExt.Attr12,x.ComponentExt.Attr13,x.ComponentExt.Attr14,x.ComponentExt.Attr15,
                x.ComponentType_Id, x.ComponentStatus_Id, x.ComponentModel_Id, x.AssignedToProject_Id, x.AssignedToAssemblyDb_Id
            });

            ViewBag.ServiceName = "ComponentService.GetAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return new DBJsonDateISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        
        // POST: /ComponentSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await componentService.GetAsync(UserId, ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.CompName, x.CompAltName, x.CompAltName2, 
                x.ComponentType.CompTypeName, x.ComponentStatus.CompStatusName, x.ComponentModel.CompModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToAssemblyDb = new { x.AssignedToAssemblyDb.AssyName, x.AssignedToAssemblyDb.AssyAltName },
                x.PositionInAssy, x.ProgramAddress, x.CalibrationReqd, x.LastCalibrationDate, x.Comments, x.IsActive,
                x.ComponentExt.Attr01,x.ComponentExt.Attr02,x.ComponentExt.Attr03,x.ComponentExt.Attr04,x.ComponentExt.Attr05,
                x.ComponentExt.Attr06,x.ComponentExt.Attr07,x.ComponentExt.Attr08,x.ComponentExt.Attr09,x.ComponentExt.Attr10,
                x.ComponentExt.Attr11,x.ComponentExt.Attr12,x.ComponentExt.Attr13,x.ComponentExt.Attr14,x.ComponentExt.Attr15,
                x.ComponentType_Id, x.ComponentStatus_Id, x.ComponentModel_Id, x.AssignedToProject_Id, x.AssignedToAssemblyDb_Id
            });

            ViewBag.ServiceName = "ComponentService.GetAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return new DBJsonDateISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /ComponentSrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds = null, string[] modelIds = null, bool getActive = true)
        {
            var data = (await componentService.GetByAltIdsAsync(UserId, projectIds, modelIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.CompName, x.CompAltName, x.CompAltName2, 
                x.ComponentType.CompTypeName, x.ComponentStatus.CompStatusName, x.ComponentModel.CompModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToAssemblyDb = new { x.AssignedToAssemblyDb.AssyName, x.AssignedToAssemblyDb.AssyAltName },
                x.PositionInAssy, x.ProgramAddress, x.CalibrationReqd, x.LastCalibrationDate, x.Comments, x.IsActive,
                x.ComponentExt.Attr01,x.ComponentExt.Attr02,x.ComponentExt.Attr03,x.ComponentExt.Attr04,x.ComponentExt.Attr05,
                x.ComponentExt.Attr06,x.ComponentExt.Attr07,x.ComponentExt.Attr08,x.ComponentExt.Attr09,x.ComponentExt.Attr10,
                x.ComponentExt.Attr11,x.ComponentExt.Attr12,x.ComponentExt.Attr13,x.ComponentExt.Attr14,x.ComponentExt.Attr15,
                x.ComponentType_Id, x.ComponentStatus_Id, x.ComponentModel_Id, x.AssignedToProject_Id, x.AssignedToAssemblyDb_Id
            });

            ViewBag.ServiceName = "ComponentService.GetByAltIdsAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return new DBJsonDateISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: /ComponentSrv/GetByAltIds2
        [HttpPost]
        [DBSrvAuth("Component_View")]
        public async Task<ActionResult> GetByAltIds2(string[] projectIds = null, string[] typeIds = null, string[] assyIds = null, bool getActive = true)
        {
            var data = (await componentService.GetByAltIdsAsync(UserId, projectIds, typeIds, assyIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.CompName, x.CompAltName, x.CompAltName2, 
                x.ComponentType.CompTypeName, x.ComponentStatus.CompStatusName, x.ComponentModel.CompModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToAssemblyDb = new { x.AssignedToAssemblyDb.AssyName, x.AssignedToAssemblyDb.AssyAltName },
                x.PositionInAssy, x.ProgramAddress, x.CalibrationReqd, x.LastCalibrationDate, x.Comments, x.IsActive,
                x.ComponentExt.Attr01,x.ComponentExt.Attr02,x.ComponentExt.Attr03,x.ComponentExt.Attr04,x.ComponentExt.Attr05,
                x.ComponentExt.Attr06,x.ComponentExt.Attr07,x.ComponentExt.Attr08,x.ComponentExt.Attr09,x.ComponentExt.Attr10,
                x.ComponentExt.Attr11,x.ComponentExt.Attr12,x.ComponentExt.Attr13,x.ComponentExt.Attr14,x.ComponentExt.Attr15,
                x.ComponentType_Id, x.ComponentStatus_Id, x.ComponentModel_Id, x.AssignedToProject_Id, x.AssignedToAssemblyDb_Id
            });

            ViewBag.ServiceName = "ComponentService.GetByAltIdsAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return new DBJsonDateISO { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        // GET: /ComponentSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await componentService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentService.LookupAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json(records.OrderBy(x => x.CompName)
                .Select(x => new { id = x.Id, name = x.CompName }), JsonRequestBehavior.AllowGet);
        }

        // GET: /ComponentSrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds = null, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (projectIds != null && projectIds != "") projectIdsArray = projectIds.Split(',');

            var records = await componentService.LookupByProjAsync(UserId, projectIdsArray, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentService.LookupByProjAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json(records.OrderBy(x => x.CompName)
                .Select(x => new { id = x.Id, name = x.CompName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentSrv/Edit
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Edit(Component[] records)
        {
            var serviceResult = await componentService.EditAsync(UserId, records).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentService.EditAsync";
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

        // POST: /ComponentSrv/Delete
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await componentService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentService.DeleteAsync";
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

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /ComponentSrv/EditExt
        [HttpPost]
        [DBSrvAuth("Component_Edit")]
        public async Task<ActionResult> EditExt(ComponentExt[] records)
        {
            var serviceResult = await componentService.EditExtAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "ComponentService.EditExtAsync";
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