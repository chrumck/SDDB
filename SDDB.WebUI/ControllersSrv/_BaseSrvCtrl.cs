using System;
using System.Security.Claims;
using System.Web.Mvc;

using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI.ControllersSrv
{
    [DBSrvAuth]
    [DBSrvValidate]
    [DBLog]
    public abstract class BaseSrvCtrl : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public string UserId
        {
            get {return ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;}
        }

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        //modified Json method to increase the maxium serialization and default AllowGet
        protected virtual JsonResult DbJson(object data)
        {
            return new JsonResult()
            { 
                Data = data, 
                JsonRequestBehavior = JsonRequestBehavior.AllowGet, 
                MaxJsonLength = Int32.MaxValue 
            };
        }

        //modified Json method to increase the maxium serialization and default AllowGet - returns JsonResultDateISO
        protected virtual JsonResultDateISO DbJsonDate(object data)
        {
            return new JsonResultDateISO()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        //modified Json method to increase the maxium serialization and default AllowGet - returns JsonResultDateISO
        protected virtual JsonResultDateTimeISO DbJsonDateTime(object data)
        {
            return new JsonResultDateTimeISO()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers



        #endregion
    }
}