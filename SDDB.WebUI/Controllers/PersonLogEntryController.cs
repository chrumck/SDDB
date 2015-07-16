using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class PersonLogEntryController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: PersonLogEntry
        [Authorize(Roles = "PersonLogEntry_View")]
        public ActionResult Index(string personId = null)
        {
            ViewBag.personId = personId;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}