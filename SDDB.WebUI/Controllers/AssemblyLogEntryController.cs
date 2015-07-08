using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class AssemblyLogEntryController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: AssemblyLogEntry
        [Authorize(Roles = "Assembly_View")]
        public ActionResult Index(string assyId = null)
        {
            ViewBag.assyId = assyId;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}