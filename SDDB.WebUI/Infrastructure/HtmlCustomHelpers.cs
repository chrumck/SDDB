using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;

using SDDB.Domain.DbContexts;
using SDDB.Domain.Entities;
using System.Security.Claims;

namespace SDDB.WebUI.Infrastructure
{
    public static class HtmlCustomHelpers
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        //returns userId string
        public static string GetUserId(this HtmlHelper html)
        {
            return ((ClaimsIdentity)html.ViewContext.HttpContext.User.Identity).FindFirst(ClaimTypes.Sid).Value;
        }

        //returns full Person name based on user name
        public static string GetUserFullName(this HtmlHelper html)
        {
            var dbContext = DependencyResolver.Current.GetService<EFDbContext>();

            var person = dbContext.Persons.SingleOrDefault(x => x.DBUser.UserName == html.ViewContext.HttpContext.User.Identity.Name);

            if (person != null)
            {
                return person.FirstName + " " + person.LastName;
            }
            else
            {
                return "";
            }
        }

        //Checks if person is a group manager
        public static bool UserIsGrManager(this HtmlHelper html)
        {
            var dbContext = DependencyResolver.Current.GetService<EFDbContext>();

            var userId = ((ClaimsIdentity)html.ViewContext.HttpContext.User.Identity).FindFirst(ClaimTypes.Sid).Value;
            if (string.IsNullOrEmpty(userId)) { return false; }
            return dbContext.PersonGroups.Any(x => x.GroupManagers.Select( y => y.Id).Contains(userId));
            
        }

        //html helper to pass array to the javascipt variable in razor view
        public static string JsonSerialize(this HtmlHelper html, object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }
        
    }
}