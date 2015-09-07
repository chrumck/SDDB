using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class AssemblyDbController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: AssemblyDb
        [Authorize(Roles = "Assembly_View")]
        public ActionResult Index(string LocationId = null)
        {
            ViewBag.LocationId = LocationId;
            return View();
        }

        // POST: AssemblyDb
        [HttpPost]
        [Authorize(Roles = "Assembly_View")]
        public ActionResult Index(string[] AssemblyIds = null)
        {
            ViewBag.AssemblyIds = AssemblyIds;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}