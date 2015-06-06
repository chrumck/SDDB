using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: Project
        [Authorize(Roles = "Project_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}