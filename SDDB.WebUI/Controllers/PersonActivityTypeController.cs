using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class PersonActivityTypeController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: PersonActivityType
        [Authorize(Roles = "PersonActivityType_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}