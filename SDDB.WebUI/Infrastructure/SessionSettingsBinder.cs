using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SDDB.Domain.Entities;

namespace SDDB.WebUI.Infrastructure
{
    public class SessionSettingsModelBinder : IModelBinder
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private const string sessionKey = "SessionSettings";

        //Constructors---------------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        //BindModel method
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            SessionSettings sessionSettings = null;
            if (controllerContext.HttpContext.Session != null)
            {
                sessionSettings = (SessionSettings)controllerContext.HttpContext.Session[sessionKey];
            }
            
            if (sessionSettings == null)
            {
                sessionSettings = new SessionSettings();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionKey] = sessionSettings;
                }
            }
            return sessionSettings;
        }

    }
}