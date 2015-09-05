using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class AssemblyDbController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: Document
        [Authorize(Roles = "Assembly_View")]
        public ActionResult Index(string LocationId = null)
        {
            ViewBag.LocationId = LocationId;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}