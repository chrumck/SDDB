using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using SDDB.Domain.DbContexts;

namespace SDDB.Domain.Services
{
    public class AppUserManager : UserManager<DBUser>, IAppUserManager
    {
        //Constructors---------------------------------------------------------------------------------------------------------//
        public AppUserManager(IUserStore<DBUser> store, IdentityFactoryOptions<AppUserManager> options)
            : base(store)
        {
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = false
            };
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //Wiring up PasswordHasher.HashPassword
        public string HashPassword(string password)
        {
            return PasswordHasher.HashPassword(password);
        }
    }

    public class AppUserStore : UserStore<DBUser>
    {
        //Constructors---------------------------------------------------------------------------------------------------------//
        public AppUserStore(EFDbContext context) : base(context) { }
    }
}
