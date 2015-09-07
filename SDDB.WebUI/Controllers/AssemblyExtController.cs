using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class AssemblyExtController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: AssemblyExt
        [Authorize(Roles = "Assembly_View")]
        public ActionResult Index()
        {
            return View();
        }

        // POST: AssemblyExt
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