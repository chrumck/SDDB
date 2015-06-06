using System;
using System.Net;
using System.Web.Mvc;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using System.Web.Routing;
using System.Threading.Tasks;


namespace SDDB.WebUI.Infrastructure
{
    public class DBExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public ILogger Logger { get; set; }

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                var filterResult = new DBResult
                {
                    ActionName = filterContext.RouteData.Values["action"].ToString(),
                    ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                    ServiceName = "DBExceptionAttribute",
                    UserName = filterContext.HttpContext.User.Identity.Name,
                    UserHostAddress = filterContext.HttpContext.Request.UserHostAddress,
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusDescription = "Exception thrown: " + filterContext.Exception.ToString()
                };
                Logger.LogResult(filterResult);

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = (int)filterResult.StatusCode;
                    filterContext.Result = new JsonResult()
                    {
                        Data = new { Success = "False", responseText = "Oops, Something went wrong!\nThis error has been recorded. Contact SDDB Admin to help resolve the issue." },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { {"action","Error"}, {"controller","Home"} });
                }
                filterContext.ExceptionHandled = true;
            }
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}
