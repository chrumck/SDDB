using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //constructor----------------------------------------------------------------------------------------------------------//


        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        // GET: Error
        public ActionResult Error()
        {
            return View();
        }

        // GET: Error
        public ActionResult Maintenance()
        {
            return View();
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion

    }
}
