using System;
using System.Web.Mvc;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using System.Net;

namespace SDDB.WebUI.Infrastructure
{
    public class DBSrvValidateAttribute : FilterAttribute, IActionFilter
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public ILogger Logger { get; set; }

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                string errorsFromModelState = "";
                foreach (ModelState state in filterContext.Controller.ViewData.ModelState.Values)
                {
                    foreach (ModelError error in state.Errors)
                    {
                        errorsFromModelState = String.IsNullOrEmpty(errorsFromModelState) ? errorsFromModelState : errorsFromModelState + "\n";
                        errorsFromModelState += error.ErrorMessage;
                    }
                }

                var filterResult = new DBResult
                {
                    ActionName = filterContext.RouteData.Values["action"].ToString(),
                    ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                    ServiceName = "DBSrvValidateAttribute",
                    UserName = filterContext.HttpContext.User.Identity.Name,
                    UserHostAddress = filterContext.HttpContext.Request.UserHostAddress,
                    StatusCode = HttpStatusCode.BadRequest,
                    StatusDescription = "Error 0102: Error(s) in submitted values:\n" + errorsFromModelState,
                };
                Logger.LogResult(filterResult);
                filterContext.HttpContext.Response.StatusCode = (int)filterResult.StatusCode;
                filterContext.Result = new JsonResult()
                {
                    Data = new { Success = "False", responseText = filterResult.StatusDescription },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //not implemented
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}
