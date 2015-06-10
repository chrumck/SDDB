using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class ComponentController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: Document
        [Authorize(Roles = "Component_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}