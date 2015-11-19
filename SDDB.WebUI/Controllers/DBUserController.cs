using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web;

using SDDB.Domain.Services;
using SDDB.Domain.Entities;
using SDDB.WebUI.Models;
using SDDB.Domain.Abstract;
using System.Net;


namespace SDDB.WebUI.Controllers
{
    [Authorize]
    public class DBUserController : Controller
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private DBUserService dbUserService;
        private ILogger logger;
        
        //Constructors---------------------------------------------------------------------------------------------------------//

        public DBUserController(DBUserService dbUserService, ILogger logger)
        {
            this.dbUserService = dbUserService;
            this.logger = logger;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        // GET: DBUser
        [Authorize(Roles = "DBUser_View")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: DBUser/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                ModelState.AddModelError("", "You have insufficient privileges to perform this action.");
                ViewBag.Authorized = true;
            }
            else
            {
                ViewBag.Authorized = false;
            }

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // POST: DBUser/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel login, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                logger.LogResult(new DBResult
                {
                    ActionName = RouteData.Values["action"].ToString(),
                    ControllerName = RouteData.Values["controller"].ToString(),
                    UserHostAddress = Request.UserHostAddress,
                    StatusCode = HttpStatusCode.Unauthorized,
                    StatusDescription = getErrorsFromModelStateHelper() + "; Attempted UserName: " + login.UserName
                });

                ViewBag.returnUrl = returnUrl;
                return View(login);
            }

            var identity = await dbUserService.LoginAsync(login.UserName, login.Password).ConfigureAwait(false);
            if (identity == null)
            {
                logger.LogResult(new DBResult
                {
                    ActionName = RouteData.Values["action"].ToString(),
                    ControllerName = RouteData.Values["controller"].ToString(),
                    ServiceName = "DBUserService.LoginAsync",
                    UserHostAddress = Request.UserHostAddress,
                    StatusCode = HttpStatusCode.Unauthorized,
                    StatusDescription = "Invalid name or password. Attempted UserName: " + login.UserName
                });

                ModelState.AddModelError("", "Invalid name or password.");

                ViewBag.returnUrl = returnUrl;
                return View(login);
            }
            
            var user = await dbUserService.FindByNameAsync(login.UserName).ConfigureAwait(false);
            identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id));

            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)) return RedirectToAction("Index", "Home");
            else return Redirect(returnUrl);
        }

        // GET: DBUser/Logout
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //getErrorsFromModelStateHelper
        private string getErrorsFromModelStateHelper()
        {
            string errorsFromModelState = "";
            foreach (ModelState state in ModelState.Values)
            {
                foreach (ModelError error in state.Errors)
                {
                    errorsFromModelState = String.IsNullOrEmpty(errorsFromModelState) ? errorsFromModelState : errorsFromModelState + " \n";
                    errorsFromModelState += error.ErrorMessage;
                }
            }
            return errorsFromModelState;
        }


        #endregion
    }
}