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