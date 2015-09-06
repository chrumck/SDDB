using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class ComponentExtController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: ComponentExt
        [Authorize(Roles = "Component_View")]
        public ActionResult Index()
        {
            return View();
        }

        // POST: ComponentExt
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