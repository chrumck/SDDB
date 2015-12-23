using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using SDDB.Domain.DbContexts;


namespace SDDB.Domain.Services
{
    public class AppRoleManager : RoleManager<DBRole>, IDisposable, IAppRoleManager
    {
        //Constructors---------------------------------------------------------------------------------------------------------//
        public AppRoleManager(AppRoleStore roleStore, IdentityFactoryOptions<AppRoleManager> roleOptions)
            : base(roleStore) { }
    }

    public class AppRoleStore : RoleStore<DBRole>
    {
        //Constructors---------------------------------------------------------------------------------------------------------//
        public AppRoleStore(EFDbContext context)
            : base(context)
        {
        }
    }

}
