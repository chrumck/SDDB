using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class YourActivityController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: YourActivity
        [Authorize(Roles = "YourActivity_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}