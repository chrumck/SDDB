using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using SDDB.Domain.DbContexts;
using SDDB.Domain.Entities;

namespace SDDB.WebUI.Infrastructure
{
    public static class HtmlCustomHelpers
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

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

        public static HtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelExpression, string firstElement)
        {
            var typeOfProperty = modelExpression.ReturnType;
            if (!typeOfProperty.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not an enum", typeOfProperty));
            var test = Enum.GetValues(typeof(MAttrTypes));
            var enumValues = new SelectList(test);
            return htmlHelper.DropDownListFor(modelExpression, enumValues, firstElement);
        } 

    }
}