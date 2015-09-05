using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class ComponentLogEntryController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: ComponentLogEntry
        [Authorize(Roles = "Component_View")]
        public ActionResult Index(string ComponentId = null)
        {
            ViewBag.ComponentId = ComponentId;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}