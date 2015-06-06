using Microsoft.AspNet.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using SDDB.Domain.Entities;

namespace SDDB.Domain.Abstract
{
    public interface IAppRoleManager
    {
        //Fields and Properties---------------------------------------------------

        IQueryable<DBRole> Roles { get; }

        //Methods------------------------------------------------------------------

        Task<IdentityResult> CreateAsync(DBRole role);
        Task<bool> RoleExistsAsync(string roleName);
    }
}
