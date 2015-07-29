using System;
using System.Net;
using System.Web.Mvc;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Diagnostics;


namespace SDDB.WebUI.Infrastructure
{
    public class DBExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public ILogger Logger { get; set; }

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        public void OnException(ExceptionContext exceptionContext)
        {
            if (exceptionContext.ExceptionHandled) { return; }
                        
            var filterResult = new DBResult
            {
                ActionName = exceptionContext.RouteData.Values["action"].ToString(),
                ControllerName = exceptionContext.RouteData.Values["controller"].ToString(),
                ServiceName = exceptionContext.Controller.ViewBag.ServiceName,
                UserName = exceptionContext.HttpContext.User.Identity.Name,
                UserHostAddress = exceptionContext.HttpContext.Request.UserHostAddress,
                StatusCode = getStatusCodeHelper(exceptionContext),
                StatusDescription = "Exception thrown: " + exceptionContext.Exception.ToString()
            };
            Logger.LogResult(filterResult);

            setResponseResultHelper(exceptionContext);
                        
            exceptionContext.ExceptionHandled = true;
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        //getStatusCodeHelper
        private HttpStatusCode getStatusCodeHelper(ExceptionContext exceptionContext)
        {
            var exceptionType = exceptionContext.Exception.GetBaseException().GetType();
            if (exceptionType == typeof(ArgumentNullException)) { return HttpStatusCode.BadRequest; }
            if (exceptionType == typeof(DbBadRequestException)) { return HttpStatusCode.BadRequest; }
            return HttpStatusCode.InternalServerError;
        }

        //setResponseResultHelper
        private void setResponseResultHelper(ExceptionContext exceptionContext)
        {
            var exceptionType = exceptionContext.Exception.GetBaseException().GetType();
            var exceptionMessage = exceptionContext.Exception.GetBaseException().Message;
            var responseText = String.Empty;
            if (exceptionType == typeof(ArgumentNullException))
            {
                responseText = "Error(s) in submitted parameters:\n" + exceptionMessage;
                exceptionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            if (exceptionType == typeof(DbBadRequestException))
            {
                responseText = "Error(s) in submited request:\n " + exceptionMessage;
                exceptionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            if (exceptionType != typeof(DbBadRequestException) && exceptionType != typeof(ArgumentNullException))
            {
                responseText = "Oops, Something went wrong!\n" +
                      "This error has been recorded. Contact SDDB Admin to help resolve the issue.";
                exceptionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            if (exceptionContext.HttpContext.Request.IsAjaxRequest())
            {
                exceptionContext.Result = new JsonResult()
                {
                    Data = new { Success = "False", responseText = responseText },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                exceptionContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary { { "action", "Error" }, { "controller", "Home" } });
            }
        }


        #endregion
    }
}
