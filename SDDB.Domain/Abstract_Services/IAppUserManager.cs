using Microsoft.AspNet.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using SDDB.Domain.Entities;

namespace SDDB.Domain.Abstract
{
    public interface IAppUserManager
    {
        //Fields and Properties---------------------------------------------------

        IQueryable<DBUser> Users { get; }

        //Methods------------------------------------------------------------------

        Task<DBUser> FindByIdAsync(string userId);
        Task<DBUser> FindByNameAsync(string userName);
        Task<IdentityResult> CreateAsync(DBUser user, string password);
        Task<IdentityResult> UpdateAsync(DBUser user);
        Task<IdentityResult> DeleteAsync(DBUser user);

        Task<DBUser> FindAsync(string userName, string password);
        Task<ClaimsIdentity> CreateIdentityAsync(DBUser user, string p);

        Task<IList<string>> GetRolesAsync(string Id);
        Task<IdentityResult> AddToRolesAsync(string userId, params string[] roles);
        Task<IdentityResult> AddToRoleAsync(string userId, string dbRole);
        Task<IdentityResult> RemoveFromRolesAsync(string userId, params string[] roles);
        Task<IdentityResult> RemoveFromRoleAsync(string userId, string dbRole);
        Task<bool> IsInRoleAsync(string userId, string role);


        string HashPassword(string password);

    }
}
