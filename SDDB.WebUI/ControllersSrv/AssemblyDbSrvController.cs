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
    public class AssemblyDbSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private AssemblyDbService assemblyService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public AssemblyDbSrvController(AssemblyDbService assemblyService)
        {
            this.assemblyService = assemblyService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /AssemblyDbSrv/Get
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await assemblyService.GetAsync(UserId, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.AssyName, x.AssyAltName, x.AssyAltName2, 
                x.AssemblyType.AssyTypeName, x.AssemblyStatus.AssyStatusName, x.AssemblyModel.AssyModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName, x.AssignedToLocation.LocationType.LocTypeName },
                x.AssyGlobalX, x.AssyGlobalY, x.AssyGlobalZ, x.AssyLocalXDesign, x.AssyLocalYDesign, x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt, x.AssyLocalYAsBuilt, x.AssyLocalZAsBuilt, x.AssyStationing, x.AssyLength, x.AssyReadingIntervalSecs,
                x.IsReference , x.TechnicalDetails, x.PowerSupplyDetails, x.HSEDetails, x.Comments, x.IsActive,
                x.AssemblyExt.Attr01,x.AssemblyExt.Attr02,x.AssemblyExt.Attr03,x.AssemblyExt.Attr04,x.AssemblyExt.Attr05,
                x.AssemblyExt.Attr06,x.AssemblyExt.Attr07,x.AssemblyExt.Attr08,x.AssemblyExt.Attr09,x.AssemblyExt.Attr10,
                x.AssemblyExt.Attr11,x.AssemblyExt.Attr12,x.AssemblyExt.Attr13,x.AssemblyExt.Attr14,x.AssemblyExt.Attr15,
                x.AssemblyType_Id, x.AssemblyStatus_Id, x.AssemblyModel_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id
            });

            ViewBag.ServiceName = "AssemblyDbService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        // POST: /AssemblyDbSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await assemblyService.GetAsync(UserId, ids, getActive).ConfigureAwait(false)).Select(x => new {
                x.Id, x.AssyName, x.AssyAltName, x.AssyAltName2, 
                x.AssemblyType.AssyTypeName, x.AssemblyStatus.AssyStatusName, x.AssemblyModel.AssyModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName, x.AssignedToLocation.LocationType.LocTypeName },
                x.AssyGlobalX, x.AssyGlobalY, x.AssyGlobalZ, x.AssyLocalXDesign, x.AssyLocalYDesign, x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt, x.AssyLocalYAsBuilt, x.AssyLocalZAsBuilt, x.AssyStationing, x.AssyLength, x.AssyReadingIntervalSecs,
                x.IsReference , x.TechnicalDetails, x.PowerSupplyDetails, x.HSEDetails, x.Comments, x.IsActive,
                x.AssemblyExt.Attr01,x.AssemblyExt.Attr02,x.AssemblyExt.Attr03,x.AssemblyExt.Attr04,x.AssemblyExt.Attr05,
                x.AssemblyExt.Attr06,x.AssemblyExt.Attr07,x.AssemblyExt.Attr08,x.AssemblyExt.Attr09,x.AssemblyExt.Attr10,
                x.AssemblyExt.Attr11,x.AssemblyExt.Attr12,x.AssemblyExt.Attr13,x.AssemblyExt.Attr14,x.AssemblyExt.Attr15,
                x.AssemblyType_Id, x.AssemblyStatus_Id, x.AssemblyModel_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id
            });

            ViewBag.ServiceName = "AssemblyDbService.GetAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyDbSrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds = null, string[] modelIds = null, bool getActive = true)
        {
            var data = (await assemblyService.GetByAltIdsAsync(UserId, projectIds, modelIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.AssyName, x.AssyAltName, x.AssyAltName2, 
                x.AssemblyType.AssyTypeName, x.AssemblyStatus.AssyStatusName, x.AssemblyModel.AssyModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName, x.AssignedToLocation.LocationType.LocTypeName },
                x.AssyGlobalX, x.AssyGlobalY, x.AssyGlobalZ, x.AssyLocalXDesign, x.AssyLocalYDesign, x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt, x.AssyLocalYAsBuilt, x.AssyLocalZAsBuilt, x.AssyStationing, x.AssyLength, x.AssyReadingIntervalSecs,
                x.IsReference , x.TechnicalDetails, x.PowerSupplyDetails, x.HSEDetails, x.Comments, x.IsActive,
                x.AssemblyExt.Attr01,x.AssemblyExt.Attr02,x.AssemblyExt.Attr03,x.AssemblyExt.Attr04,x.AssemblyExt.Attr05,
                x.AssemblyExt.Attr06,x.AssemblyExt.Attr07,x.AssemblyExt.Attr08,x.AssemblyExt.Attr09,x.AssemblyExt.Attr10,
                x.AssemblyExt.Attr11,x.AssemblyExt.Attr12,x.AssemblyExt.Attr13,x.AssemblyExt.Attr14,x.AssemblyExt.Attr15,
                x.AssemblyType_Id, x.AssemblyStatus_Id, x.AssemblyModel_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id
            });

            ViewBag.ServiceName = "AssemblyDbService.GetByAltIdsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyDbSrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("Assembly_View")]
        public async Task<ActionResult> GetByAltIds(string[] projectIds = null, string[] typeIds = null, string[] locIds = null, bool getActive = true)
        {
            var data = (await assemblyService.GetByAltIdsAsync(UserId, projectIds, typeIds, locIds, getActive).ConfigureAwait(false)).Select(x => new
            {
                x.Id, x.AssyName, x.AssyAltName, x.AssyAltName2, 
                x.AssemblyType.AssyTypeName, x.AssemblyStatus.AssyStatusName, x.AssemblyModel.AssyModelName,
                AssignedToProject = new { x.AssignedToProject.ProjectName, x.AssignedToProject.ProjectAltName, x.AssignedToProject.ProjectCode },
                AssignedToLocation = new { x.AssignedToLocation.LocName, x.AssignedToLocation.LocAltName, x.AssignedToLocation.LocationType.LocTypeName },
                x.AssyGlobalX, x.AssyGlobalY, x.AssyGlobalZ, x.AssyLocalXDesign, x.AssyLocalYDesign, x.AssyLocalZDesign,
                x.AssyLocalXAsBuilt, x.AssyLocalYAsBuilt, x.AssyLocalZAsBuilt, x.AssyStationing, x.AssyLength, x.AssyReadingIntervalSecs,
                x.IsReference , x.TechnicalDetails, x.PowerSupplyDetails, x.HSEDetails, x.Comments, x.IsActive,
                x.AssemblyExt.Attr01,x.AssemblyExt.Attr02,x.AssemblyExt.Attr03,x.AssemblyExt.Attr04,x.AssemblyExt.Attr05,
                x.AssemblyExt.Attr06,x.AssemblyExt.Attr07,x.AssemblyExt.Attr08,x.AssemblyExt.Attr09,x.AssemblyExt.Attr10,
                x.AssemblyExt.Attr11,x.AssemblyExt.Attr12,x.AssemblyExt.Attr13,x.AssemblyExt.Attr14,x.AssemblyExt.Attr15,
                x.AssemblyType_Id, x.AssemblyStatus_Id, x.AssemblyModel_Id, x.AssignedToProject_Id, x.AssignedToLocation_Id
            });

            ViewBag.ServiceName = "AssemblyDbService.GetByAltIdsAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        // GET: /AssemblyDbSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await assemblyService.LookupAsync(UserId, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyDbService.LookupAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.AssyName)
                .Select(x => new { id = x.Id, name = x.AssyName + " - " + x.AssignedToProject.ProjectCode }), JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyDbSrv/LookupByProj
        public async Task<ActionResult> LookupByProj(string projectIds = null, string query = "", bool getActive = true)
        {
            string[] projectIdsArray = null;
            if (projectIds != null && projectIds != "") projectIdsArray = projectIds.Split(',');

            var records = await assemblyService.LookupByProjAsync(UserId, projectIdsArray, query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyDbService.LookupByProjAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.AssyName)
                .Select(x => new { id = x.Id, name = x.AssyName + " - " + x.AssignedToProject.ProjectCode }), JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyDbSrv/LookupByLoc
        public async Task<ActionResult> LookupByLoc(string locId = null, bool getActive = true)
        {
            var records = await assemblyService.LookupByLocAsync(UserId, locId, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyDbService.LookupByLocAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.AssyName)
                .Select(x => new { id = x.Id, name = x.AssyName + " - " + x.AssignedToProject.ProjectCode }), JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyDbSrv/LookupByLocDTables
        public async Task<ActionResult> LookupByLocDTables(string locId = null, bool getActive = true)
        {
            var records = await assemblyService.LookupByLocAsync(UserId, locId, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyDbService.LookupByLocAsync"; ViewBag.StatusCode = HttpStatusCode.OK;

            return Json(records.OrderBy(x => x.AssyName)
                .Select(x => new { Id = x.Id, AssyName = x.AssyName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyDbSrv/Edit
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Edit(AssemblyDb[] records)
        {
            var serviceResult = await assemblyService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyDbService.EditAsync"; ViewBag.StatusCode = serviceResult.StatusCode; 
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

        // POST: /AssemblyDbSrv/Delete
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await assemblyService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyDbService.DeleteAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyDbSrv/EditExt
        [HttpPost]
        [DBSrvAuth("Assembly_Edit")]
        public async Task<ActionResult> EditExt(AssemblyExt[] records)
        {
            var serviceResult = await assemblyService.EditExtAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyDbService.EditExtAsync"; ViewBag.StatusCode = serviceResult.StatusCode;
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