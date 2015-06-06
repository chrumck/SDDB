using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mehdime.Entity;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using SDDB.Domain.Infrastructure;
using SDDB.Domain.DbContexts;

namespace SDDB.Domain.Services
{
    public class DBUserService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private bool ldapAuthenticationEnabled;
        private IAppUserManager appUserManager;
        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DBUserService(IAppUserManager appUserManager, IDbContextScopeFactory contextScopeFac, bool ldapAuthenticationEnabled)
        {
            this.ldapAuthenticationEnabled = ldapAuthenticationEnabled;
            this.appUserManager = appUserManager;
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //Wiring up IAppUserManager.FindByNameAsync
        public virtual Task<DBUser> FindByNameAsync(string userName)
        {
            return appUserManager.FindByNameAsync(userName);
        }

        // Logging in user and returning identity result
        public virtual async Task<ClaimsIdentity> LoginAsync(string userName, string password)
        {
            var dbEntry = await appUserManager.FindAsync(userName, password).ConfigureAwait(false);
            if (dbEntry == null) return null;
            else return await appUserManager.CreateIdentityAsync(dbEntry, DefaultAuthenticationTypes.ApplicationCookie).ConfigureAwait(false);
        }


        //-----------------------------------------------------------------------------------------------------------------------

        //get all users
        public virtual Task<List<DBUser>> GetAsync()
        {
            return Task.FromResult(appUserManager.Users.Include( x => x.Person).ToList());
        }

        // Create and Update DBUsers given in DBUser[]
        public virtual async Task<DBResult> EditAsync(DBUser[] records)
        {
            var errorMessage = "";
            foreach (var record in records)
            {
                if (!record.LDAPAuthenticated && record.PropIsModified(x => x.LDAPAuthenticated) && String.IsNullOrEmpty(record.Password))
                {
                    errorMessage += String.Format("User {0}: Password is required if not LDAP authenticated\n", record.UserName); continue;
                }

                if (record.LDAPAuthenticated) { record.Password = Guid.NewGuid().ToString(); record.PasswordConf = record.Password; }

                var dbEntry = await appUserManager.FindByIdAsync(record.Id).ConfigureAwait(false);
                if (dbEntry == null)
                {
                    var identityResult = await appUserManager.CreateAsync(record, record.Password).ConfigureAwait(false);
                    errorMessage += (identityResult.Succeeded) ? "" : String.Format("User {0}:{1}\n", record.UserName, getErrorsFromIdResult(identityResult));
                }
                else
                {
                    if (record.PropIsModified(x => x.UserName)) dbEntry.UserName = record.UserName;
                    if (record.PropIsModified(x => x.Email)) dbEntry.Email = record.Email;
                    if (record.PropIsModified(x => x.LDAPAuthenticated)) dbEntry.LDAPAuthenticated = record.LDAPAuthenticated;
                    if (record.PropIsModified(x => x.Password)) dbEntry.PasswordHash = appUserManager.HashPassword(record.Password);

                    var identityResult = await appUserManager.UpdateAsync(dbEntry).ConfigureAwait(false);
                    errorMessage += (identityResult.Succeeded) ? "" : String.Format("User {0}:{1}\n", record.UserName, getErrorsFromIdResult(identityResult));
                }
            }
            if (errorMessage == "") return new DBResult { StatusCode = HttpStatusCode.OK };
            else return new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = "Errors editing records:\n" + errorMessage };
        }

        // Delete DBUsers by their IDs
        public virtual async Task<DBResult> DeleteAsync(string[] ids)
        {
            var errorMessage = "";
            foreach (var id in ids)
            {
                DBUser user = await appUserManager.FindByIdAsync(id).ConfigureAwait(false);
                if (user != null)
                {
                    IdentityResult result = await appUserManager.DeleteAsync(user).ConfigureAwait(false);
                    if (!result.Succeeded) { errorMessage += string.Format("Id={0}: {1}\n", id, getErrorsFromIdResult(result)); }
                }
            }
            if (errorMessage == "") return new DBResult { StatusCode = HttpStatusCode.OK };
            else return new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = "Errors deleting records:\n" + errorMessage };
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //get all DBRoles
        public virtual Task<List<string>> GetAllRolesAsync()
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Roles.Select(x => x.Name).ToListAsync();
            }
        }

        //get particular DBUser Roles
        public virtual async Task<List<string>> GetUserRolesAsync(string id)
        {
            return (await appUserManager.GetRolesAsync(id).ConfigureAwait(false)).ToList();
        }

        //get roles not assigned to DBUser
        public virtual async Task<List<string>> GetUserRolesNotAsync(string id)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var userRoles = await appUserManager.GetRolesAsync(id).ConfigureAwait(false);
                return dbContext.Roles.Select(x => x.Name).ToList().Except(userRoles).ToList();
            }
        }

        //Add (or Remove  when set isAdd to false) roles to DBUSer
        public virtual async Task<DBResult> EditRolesAsync(string[] ids, string[] dbRoles, bool isAdd)
        {
            var errorMessage = ""; var identityResult = IdentityResult.Success;
            foreach (var id in ids)
            {
                var dbEntry = await appUserManager.FindByIdAsync(id).ConfigureAwait(false);
                if (dbEntry == null) errorMessage += String.Format("User with Id={0} not found.\n", id);
                else
                {
                    var userRoles = await appUserManager.GetRolesAsync(id).ConfigureAwait(false);
                    foreach (var dbRole in dbRoles)
                    {
                        if (isAdd)
                        {
                            if (!userRoles.Contains(dbRole))
                            {
                                identityResult = await appUserManager.AddToRoleAsync(id, dbRole).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            if (userRoles.Contains(dbRole))
                            {
                                identityResult = await appUserManager.RemoveFromRoleAsync(id, dbRole).ConfigureAwait(false);
                            }
                        }
                        errorMessage += (identityResult.Succeeded) ? "" : String.Format("User Id={0}:{1}\n", id, getErrorsFromIdResult(identityResult));
                    }
                }
            }
            if (errorMessage == "") return new DBResult { StatusCode = HttpStatusCode.OK };
            else return new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = "Errors editing records:\n" + errorMessage };
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        private string getErrorsFromIdResult(IdentityResult identityResult)
        {
            string errorsFromResult = null;
            foreach (string error in identityResult.Errors)
            {
                errorsFromResult += error + "; ";
            }
            return errorsFromResult;
        }

        #endregion
    }
}
