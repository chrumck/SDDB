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

        IAppRoleManager appRoleManager;
        IAppUserManager appUserManager;
        bool ldapAuthenticationEnabled;

        //Constructors---------------------------------------------------------------------------------------------------------//
        public DBUserService(IAppRoleManager appRoleManager, IAppUserManager appUserManager, bool ldapAuthenticationEnabled)
        {
            this.appRoleManager = appRoleManager;
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
        public virtual async Task<List<string>> EditAsync(DBUser[] records)
        {
            if (records == null || records.Length == 0) { throw new ArgumentNullException("records"); }

            checkDbUserBeforeEditHelper(records);
            return await editDbUserHelperAsync(records).ConfigureAwait(false);
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
            return appRoleManager.Roles.Select(x => x.Name).ToListAsync();
        }

        //get particular DBUser Roles
        public virtual async Task<List<string>> GetUserRolesAsync(string[] userIds)
        {
            if (userIds == null || userIds.Length == 0) { throw new ArgumentNullException("userIds"); }

            return await appRoleManager.Roles
                .Where(x => x.Users.Any(y => userIds.Contains(y.UserId)))
                .Select(x => x.Name)
                .ToListAsync().ConfigureAwait(false);
        }

        //get roles not assigned to DBUser
        public virtual async Task<List<string>> GetUserRolesNotAsync(string[] userIds)
        {
            if (userIds == null || userIds.Length == 0) { throw new ArgumentNullException("userIds"); }
                        
            return await appRoleManager.Roles
                .Where(x => !userIds.All(y => x.Users.Select(z => z.UserId).Contains(y)))
                .Select(x => x.Name)
                .ToListAsync().ConfigureAwait(false);
        }

        //Add (or Remove  when set isAdd to false) roles to DBUSer
        public virtual async Task AddRemoveRolesAsync(string[] ids, string[] roleNames, bool isAdd)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }
            if (roleNames == null || roleNames.Length == 0) { throw new ArgumentNullException("roleNames"); }

            var users = await GetAsync(ids).ConfigureAwait(false);
            if (users.Count != ids.Length) { throw new DbBadRequestException("Error adding/removing roles. User(s) not found."); }

            foreach (var user in users)
            {
                foreach (var roleName in roleNames)
                {
                    var userIsInRole = await appUserManager.IsInRoleAsync(user.Id, roleName).ConfigureAwait(false);
                    var addRemoveResult = IdentityResult.Success;
                    if (isAdd && !userIsInRole) 
                    { 
                        addRemoveResult = await appUserManager.AddToRoleAsync(user.Id, roleName).ConfigureAwait(false);
                    }
                    if (!isAdd && userIsInRole) {
                        addRemoveResult = await appUserManager.RemoveFromRoleAsync(user.Id, roleName).ConfigureAwait(false);
                    }
                    if (!addRemoveResult.Succeeded)
                    {
                        throw new DbBadRequestException(string.Format("Error adding role {0} to user {1}: {2}",
                            roleName, user.UserName, getErrorsFromIdResult(addRemoveResult)));
                    }
                }
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
        private async Task<string> editDbUserHelperAsync(DBUser record)
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
                return record.Id;
            }

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

            return null;
        }

        //editDbUserHelperAsync - override for array of records
        private async Task<List<string>> editDbUserHelperAsync(DBUser[] records)
        {
            var newEntryIds = new List<string>();

            for (int i = 0; i < records.Length; i++)
            {
                var newEntryId =  await editDbUserHelperAsync(records[i]).ConfigureAwait(false);
                if (!String.IsNullOrEmpty(newEntryId)) { newEntryIds.Add(newEntryId); }
            }
            return newEntryIds;
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
