using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;

namespace SDDB.WebUI.Infrastructure
{
    public class DBSrvAuthAttribute : FilterAttribute, IAuthenticationFilter
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private string dbRoles;
        private bool ajaxRequired;
        public ILogger Logger { get; set; }

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DBSrvAuthAttribute(string dbRoles = "", bool ajaxRequired = true)
        {
            this.dbRoles = dbRoles;
            this.ajaxRequired = ajaxRequired;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        public void OnAuthentication(AuthenticationContext context)
        {
            var filterResult = new DBResult
            {
                ActionName = context.RouteData.Values["action"].ToString(),
                ControllerName = context.RouteData.Values["controller"].ToString(),
                ServiceName = "DBSrvAuthAttribute",
                UserName = context.Principal.Identity.Name,
                UserHostAddress = context.HttpContext.Request.UserHostAddress
            };

            var dbRolesArray = dbRoles.Split(','); var isInRole = false;
            foreach (var dbRole in dbRolesArray)
            {
                if (context.Principal.IsInRole(dbRole)) isInRole = true; break;
            }


            if (!context.HttpContext.Request.IsAuthenticated || (dbRoles != "" && !isInRole))
            {
                filterResult.StatusCode = HttpStatusCode.Forbidden;
                filterResult.StatusDescription = "Request not authorized, contact SDDB administrator to obtain appropriate privileges";
                Logger.LogResult(filterResult);
                context.HttpContext.Response.StatusCode = (int)filterResult.StatusCode;
                context.Result = new JsonResult()
                {
                    Data = new { Success = "False", responseText = filterResult.StatusDescription },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return;
            }

            if (ajaxRequired && !context.HttpContext.Request.IsAjaxRequest())
            {
                filterResult.StatusCode = HttpStatusCode.Gone;
                filterResult.StatusDescription = "Error 0101: AJAX request allowed only.";
                Logger.LogResult(filterResult);
                context.HttpContext.Response.StatusCode = (int)filterResult.StatusCode;
                context.Result = new JsonResult()
                {
                    Data = new { Success = "False", responseText = filterResult.StatusDescription },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return;
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //not implemented
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}
