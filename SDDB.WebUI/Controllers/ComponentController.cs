using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class ComponentController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: Component
        [Authorize(Roles = "Component_View")]
        public ActionResult Index(string AssemblyId = null)
        {
            ViewBag.AssemblyId = AssemblyId;
            return View();
        }

        // POST: Component
        [HttpPost]
        [Authorize(Roles = "Component_View")]
        public ActionResult Index(string[] ComponentIds = null)
        {
            ViewBag.ComponentIds = ComponentIds;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}