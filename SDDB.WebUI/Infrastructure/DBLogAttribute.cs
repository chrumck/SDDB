using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;

namespace SDDB.WebUI.Infrastructure
{
    public class DBLogAttribute : FilterAttribute, IActionFilter
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public ILogger Logger { get; set; }
        
        private DBResult filterResult;
        private Stopwatch timer;

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterResult = new DBResult
            {
                DtStart = DateTime.Now,
                ActionName = filterContext.RouteData.Values["action"].ToString(),
                ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                UserName = filterContext.HttpContext.User.Identity.Name,
                UserHostAddress = filterContext.HttpContext.Request.UserHostAddress
            };
            timer = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            timer.Stop();
            if (filterContext.Exception == null)
            {
                filterResult.DtEnd = filterResult.DtStart + timer.Elapsed;
                filterResult.ServiceName = filterContext.Controller.ViewBag.ServiceName;
                filterResult.StatusCode = filterContext.Controller.ViewBag.StatusCode ?? HttpStatusCode.OK ;
                filterResult.StatusDescription = filterContext.Controller.ViewBag.StatusDescription;

                Logger.LogResult(filterResult);
            }
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}
