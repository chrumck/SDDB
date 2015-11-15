using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class LastEntrySummaryController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: ActivitySummary
        [Authorize(Roles = "PersonLogEntry_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}