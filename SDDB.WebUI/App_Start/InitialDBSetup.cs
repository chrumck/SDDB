using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Abstract;

namespace SDDB.WebUI
{
    public class InitialDBSetup
    {
        //Run this method at the time of App startup
        public static void RunAtStartup()
        {
            var dbRoles = new DBRole[]
            {
                new DBRole {Name = "_DBAdmin" }, new DBRole {Name = "_DBManager" }, new DBRole {Name = "_DBPowerUser" },
                
                new DBRole {Name = "DBUser_View" }, new DBRole {Name = "DBUser_Edit" },
                new DBRole {Name = "Person_View" },new DBRole {Name = "Person_Edit" },
                new DBRole {Name = "PersonGroup_View" },new DBRole {Name = "PersonGroup_Edit" },
                new DBRole {Name = "PersonActivityType_View" },new DBRole {Name = "PersonActivityType_Edit" },
                
                new DBRole {Name = "Project_View" },new DBRole {Name = "Project_Edit" },
                new DBRole {Name = "ProjectEvent_View" },new DBRole {Name = "ProjectEvent_Edit" },
                
                new DBRole {Name = "Document_View" },new DBRole {Name = "Document_Edit" },
                new DBRole {Name = "DocumentType_View" },new DBRole {Name = "DocumentType_Edit" },
                
                new DBRole {Name = "Location_View" },new DBRole {Name = "Location_Edit" }, 
                new DBRole {Name = "LocationType_View" },new DBRole {Name = "LocationType_Edit" }, 
                
                new DBRole {Name = "Assembly_View" },new DBRole {Name = "Assembly_Edit" },
                new DBRole {Name = "AssemblyType_View" },new DBRole {Name = "AssemblyType_Edit" },
                new DBRole {Name = "AssemblyModel_View" },new DBRole {Name = "AssemblyModel_Edit" },
                new DBRole {Name = "AssemblyStatus_View" },new DBRole {Name = "AssemblyStatus_Edit" },
                
                new DBRole {Name = "Component_View" },new DBRole {Name = "Component_Edit" },
                new DBRole {Name = "ComponentType_View" },new DBRole {Name = "ComponentType_Edit" },
                new DBRole {Name = "ComponentModel_View" },new DBRole {Name = "ComponentModel_Edit" },
                new DBRole {Name = "ComponentStatus_View" },new DBRole {Name = "ComponentStatus_Edit" },
            };

            var roleManager = DependencyResolver.Current.GetService<IAppRoleManager>();

            foreach (var role in dbRoles)
            {
                if (!roleManager.RoleExistsAsync(role.Name).Result)
                {
                    var identityResult = roleManager.CreateAsync(role).Result;
                }
            }
        }
    }
}