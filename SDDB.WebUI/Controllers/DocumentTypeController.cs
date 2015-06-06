using System.Web.Mvc;

namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class DocumentTypeController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: DocumentType
        [Authorize(Roles = "DocumentType_View")]
        public ActionResult Index()
        {
            return View();
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers


        #endregion
    }
}