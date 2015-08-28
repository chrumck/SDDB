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

        IDbContextScopeFactory contextScopeFac;
        private IAppUserManager appUserManager;
        private bool ldapAuthenticationEnabled;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DBUserService(IDbContextScopeFactory contextScopeFac, IAppUserManager appUserManager, bool ldapAuthenticationEnabled)
        {
            this.contextScopeFac = contextScopeFac;
            this.appUserManager = appUserManager;
            this.ldapAuthenticationEnabled = ldapAuthenticationEnabled;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all users
        public virtual Task<List<DBUser>> GetAsync()
        {
            return appUserManager.Users
                .Include( x => x.Person)
                .ToListAsync();
        }

        //get users by ids
        public virtual Task<List<DBUser>> GetAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            return appUserManager.Users
                .Where(x => ids.Contains((x.Id)))
                .Include(x => x.Person)
                .ToListAsync();
        }

        //Wiring up IAppUserManager.FindByNameAsync
        public virtual Task<DBUser> FindByNameAsync(string userName)
        {
            return appUserManager.FindByNameAsync(userName);
        }
        
        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task EditAsync(DBUser[] records)
        {
            if (records == null || records.Length == 0) { throw new ArgumentNullException("records"); }

            checkDbUserBeforeEditHelper(records);
            await editDbUserHelperAsync(records).ConfigureAwait(false);
        }

        // Delete records by their Ids
        public virtual async Task DeleteAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            await deleteDbUserHelperAsync(ids).ConfigureAwait(false);
        }
        
        // Logging in user and returning identity result
        public virtual async Task<ClaimsIdentity> LoginAsync(string userName, string password)
        {
            var dbEntry = await appUserManager.FindAsync(userName, password).ConfigureAwait(false);
            if (dbEntry == null) { return null; }
            return await appUserManager.CreateIdentityAsync(dbEntry, DefaultAuthenticationTypes.ApplicationCookie)
                .ConfigureAwait(false);
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
        public virtual async Task<List<string>> GetUserRolesAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            return (await appUserManager.GetRolesAsync(userId).ConfigureAwait(false)).ToList();
        }

        //get roles not assigned to DBUser
        public virtual async Task<List<string>> GetUserRolesNotAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var userRoles = await appUserManager.GetRolesAsync(userId).ConfigureAwait(false);
                return dbContext.Roles.Select(x => x.Name).ToList().Except(userRoles).ToList();
            }
        }

        //Add (or Remove  when set isAdd to false) roles to DBUSer
        public virtual async Task AddRemoveRolesAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }
            if (idsAddRem == null || idsAddRem.Length == 0) { throw new ArgumentNullException("roleNames"); }

            var users = await GetAsync(ids).ConfigureAwait(false);
            if (users.Count != ids.Length) { throw new DbBadRequestException("Error adding/removing roles. User(s) not found."); }

            foreach (var user in users)
            {
                await addRemoveRolesHelper(user, idsAddRem, isAdd).ConfigureAwait(false);
            }
        }
        
        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //checkDbUserBeforeEditHelperAsync - single record
        private void checkDbUserBeforeEditHelper(DBUser record)
        {
            if (record.PropIsModified(x => x.LDAPAuthenticated_bl) && !record.LDAPAuthenticated_bl && String.IsNullOrEmpty(record.Password))
            {
                throw new DbBadRequestException(
                    String.Format("User {0}: Password is required if not LDAP authenticated\n", record.UserName));
            }
        }

        //checkDbUserBeforeEditHelperAsync - override for array of records
        private void checkDbUserBeforeEditHelper(DBUser[] records)
        {
            for (int i = 0; i < records.Length; i++)
            {
                checkDbUserBeforeEditHelper(records[i]);
            }
        }

        //editDbUserHelperAsync - single record
        private async Task editDbUserHelperAsync(DBUser record)
        {
            if (record.LDAPAuthenticated_bl)
            { record.Password = Guid.NewGuid().ToString(); record.PasswordConf = record.Password; }

            var dbEntry = await appUserManager.FindByIdAsync(record.Id).ConfigureAwait(false);
            if (dbEntry == null)
            {
                var createResult = await appUserManager.CreateAsync(record, record.Password).ConfigureAwait(false);
                if (!createResult.Succeeded)
                {
                    throw new DbBadRequestException(
                        String.Format("Error creating user {0}:{1}\n", record.UserName, getErrorsFromIdResult(createResult)));
                }
            }
            else
            {
                if (record.PropIsModified(x => x.UserName)) dbEntry.UserName = record.UserName;
                if (record.PropIsModified(x => x.Email)) dbEntry.Email = record.Email;
                if (record.PropIsModified(x => x.LDAPAuthenticated_bl)) dbEntry.LDAPAuthenticated_bl = record.LDAPAuthenticated_bl;
                if (record.PropIsModified(x => x.Password)) dbEntry.PasswordHash = appUserManager.HashPassword(record.Password);
                var updateResult = await appUserManager.UpdateAsync(dbEntry).ConfigureAwait(false);
                if (!updateResult.Succeeded)
                {
                    throw new DbBadRequestException(
                        String.Format("Error editing user {0}:{1}\n", record.UserName, getErrorsFromIdResult(updateResult)));
                }
            }
        }

        //editDbUserHelperAsync - override for array of records
        private async Task editDbUserHelperAsync(DBUser[] records)
        {
            for (int i = 0; i < records.Length; i++)
            {
                await editDbUserHelperAsync(records[i]).ConfigureAwait(false);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //deleteDbUserHelperAsync - single record
        private async Task deleteDbUserHelperAsync(string id)
        {
            var user = await appUserManager.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null) { throw new DbBadRequestException(string.Format("DB User with Id={0} not found", id)); }

            var deleteResult = await appUserManager.DeleteAsync(user).ConfigureAwait(false);
            if (!deleteResult.Succeeded)
            {
                throw new DbBadRequestException(
                    string.Format("Error Deleting User {0}: {1}\n", user.UserName, getErrorsFromIdResult(deleteResult)));
            }
        }

        //deleteDbUserHelperAsync - override for array of records
        private async Task deleteDbUserHelperAsync(string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                await deleteDbUserHelperAsync(ids[i]).ConfigureAwait(false);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //helper - Add (or Remove  when set isAdd to false) roles to dbUser 
        //taking taking single DBUser + rolenames and isAdd
        private async Task addRemoveRolesHelper(DBUser user, string[] roleNames, bool isAdd)
        {
            var userRoles = await appUserManager.GetRolesAsync(user.Id).ConfigureAwait(false);
            foreach (var dbRoleName in roleNames)
            {
                await addRemoveRoleHelper(user, userRoles, dbRoleName, isAdd).ConfigureAwait(false);
            }
        }

        //helper - Add (or Remove  when set isAdd to false) role to dbUser 
        //taking taking single roleName and dbUser + userRoles and isAdd
        private async Task addRemoveRoleHelper(DBUser user, IList<string> userRoles, string roleName, bool isAdd)
        {
            if (isAdd && !userRoles.Contains(roleName))
            {
                var addResult = await appUserManager.AddToRoleAsync(user.Id, roleName).ConfigureAwait(false);
                if (!addResult.Succeeded)
                {
                    throw new DbBadRequestException(string.Format("Error adding role {0} to user {1}: {2}",
                        roleName, user.UserName, getErrorsFromIdResult(addResult)));
                }
            }
            if (!isAdd && userRoles.Contains(roleName))
            {
                var removeResult = await appUserManager.RemoveFromRoleAsync(user.Id, roleName).ConfigureAwait(false);
                if (!removeResult.Succeeded)
                {
                    throw new DbBadRequestException(string.Format("Error removing role {0} from user {1}: {2}",
                        roleName, user.UserName, getErrorsFromIdResult(removeResult)));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //getErrorsFromIdResult
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
