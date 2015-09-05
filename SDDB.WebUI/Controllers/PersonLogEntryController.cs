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
        public ActionResult Index(string PersonId = null)
        {
            ViewBag.PersonId = PersonId;
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}