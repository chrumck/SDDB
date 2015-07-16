using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    public class AssemblyModelSrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private AssemblyModelService assyModelService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public AssemblyModelSrvController(AssemblyModelService assyModelService)
        {
            this.assyModelService = assyModelService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: /AssemblyModelSrv/Get
        [DBSrvAuth("AssemblyModel_View,Assembly_View")]
        public async Task<ActionResult> Get(bool getActive = true)
        {
            var data = (await assyModelService.GetAsync(getActive).ConfigureAwait(false)).Select(x => {
                var Attr01Type = x.Attr01Type.ToString(); var Attr02Type = x.Attr02Type.ToString(); var Attr03Type = x.Attr03Type.ToString();
                var Attr04Type = x.Attr04Type.ToString(); var Attr05Type = x.Attr05Type.ToString(); var Attr06Type = x.Attr06Type.ToString();
                var Attr07Type = x.Attr07Type.ToString(); var Attr08Type = x.Attr08Type.ToString(); var Attr09Type = x.Attr09Type.ToString();
                var Attr10Type = x.Attr10Type.ToString(); var Attr11Type = x.Attr11Type.ToString(); var Attr12Type = x.Attr12Type.ToString();
                var Attr13Type = x.Attr13Type.ToString(); var Attr14Type = x.Attr14Type.ToString(); var Attr15Type = x.Attr15Type.ToString();
                return new {
                x.Id, x.AssyModelName, x.AssyModelAltName, x.Comments, x.IsActive,
                Attr01Type,x.Attr01Desc,Attr02Type,x.Attr02Desc,Attr03Type,x.Attr03Desc,Attr04Type,x.Attr04Desc,Attr05Type,x.Attr05Desc,
                Attr06Type,x.Attr06Desc,Attr07Type,x.Attr07Desc,Attr08Type,x.Attr08Desc,Attr09Type,x.Attr09Desc,Attr10Type,x.Attr10Desc,
                Attr11Type,x.Attr11Desc,Attr12Type,x.Attr12Desc,Attr13Type,x.Attr13Desc,Attr14Type,x.Attr14Desc,Attr15Type,x.Attr15Desc
                };
            });

            ViewBag.ServiceName = "AssemblyModelService.GetAsync"; 
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /AssemblyModelSrv/GetByIds
        [HttpPost]
        [DBSrvAuth("AssemblyModel_View,Assembly_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            var data = (await assyModelService.GetAsync(ids, getActive).ConfigureAwait(false)).Select(x => {
                var Attr01Type = x.Attr01Type.ToString(); var Attr02Type = x.Attr02Type.ToString(); var Attr03Type = x.Attr03Type.ToString();
                var Attr04Type = x.Attr04Type.ToString(); var Attr05Type = x.Attr05Type.ToString(); var Attr06Type = x.Attr06Type.ToString();
                var Attr07Type = x.Attr07Type.ToString(); var Attr08Type = x.Attr08Type.ToString(); var Attr09Type = x.Attr09Type.ToString();
                var Attr10Type = x.Attr10Type.ToString(); var Attr11Type = x.Attr11Type.ToString(); var Attr12Type = x.Attr12Type.ToString();
                var Attr13Type = x.Attr13Type.ToString(); var Attr14Type = x.Attr14Type.ToString(); var Attr15Type = x.Attr15Type.ToString();
                return new {
                x.Id, x.AssyModelName, x.AssyModelAltName, x.Comments, x.IsActive,
                Attr01Type,x.Attr01Desc,Attr02Type,x.Attr02Desc,Attr03Type,x.Attr03Desc,Attr04Type,x.Attr04Desc,Attr05Type,x.Attr05Desc,
                Attr06Type,x.Attr06Desc,Attr07Type,x.Attr07Desc,Attr08Type,x.Attr08Desc,Attr09Type,x.Attr09Desc,Attr10Type,x.Attr10Desc,
                Attr11Type,x.Attr11Desc,Attr12Type,x.Attr12Desc,Attr13Type,x.Attr13Desc,Attr14Type,x.Attr14Desc,Attr15Type,x.Attr15Desc
                };
            });

            ViewBag.ServiceName = "AssemblyModelService.GetAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json( data , JsonRequestBehavior.AllowGet);
        }

        // GET: /AssemblyModelSrv/Lookup
        public async Task<ActionResult> Lookup(string query = "", bool getActive = true)
        {
            var records = await assyModelService.LookupAsync(query, getActive).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyModelService.LookupAsync";
            ViewBag.StatusCode = HttpStatusCode.OK;
            return Json(records.OrderBy(x => x.AssyModelName)
                .Select(x => new { id = x.Id, name = x.AssyModelName }), JsonRequestBehavior.AllowGet);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /AssemblyModelSrv/Edit
        [HttpPost]
        [DBSrvAuth("AssemblyModel_Edit")]
        public async Task<ActionResult> Edit(AssemblyModel[] records)
        {
            var serviceResult = await assyModelService.EditAsync(records).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyModelService.EditAsync";
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

        // POST: /AssemblyModelSrv/Delete
        [HttpPost]
        [DBSrvAuth("AssemblyModel_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            var serviceResult = await assyModelService.DeleteAsync(ids).ConfigureAwait(false);

            ViewBag.ServiceName = "AssemblyModelService.DeleteAsync";
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