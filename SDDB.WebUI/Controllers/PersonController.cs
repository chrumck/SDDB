using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: Person
        [Authorize(Roles = "Person_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}