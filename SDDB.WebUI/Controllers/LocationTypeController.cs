using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class LocationTypeController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: LocationType
        [Authorize(Roles = "LocationType_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}