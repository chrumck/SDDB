using System.Security.Claims;
using System.Web.Mvc;

using SDDB.WebUI.Infrastructure;
namespace SDDB.WebUI.ControllersSrv
{
    [DBSrvAuth]
    [DBSrvValidate]
    [DBLog]
    public abstract class BaseSrvCtrl : Controller
    {
        public string UserId
        {
            get {return ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;}
        }
    }
}