using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: Document
        [Authorize(Roles = "Location_View")]
        public ActionResult Index(string projectId = null)
        {
            ViewBag.projectId = projectId;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}