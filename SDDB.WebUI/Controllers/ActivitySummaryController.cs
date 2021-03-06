﻿using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class ActivitySummaryController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: ActivitySummary
        [Authorize(Roles = "YourActivity_View,ActivitySummary_ViewOthers")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}