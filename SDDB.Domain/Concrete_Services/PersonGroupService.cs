using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
using Mehdime.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Services
{
    public class PersonGroupService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;
        private PersonService personService;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonGroupService(IDbContextScopeFactory contextScopeFac, PersonService personService)
        {
            this.contextScopeFac = contextScopeFac;
            this.personService = personService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<PersonGroup>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.PersonGroups.Where(x => x.IsActive == getActive).ToListAsync().ConfigureAwait(false);
                return records; 
            }
        }

        //get by ids
        public virtual async Task<List<PersonGroup>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.PersonGroups.Where(x => x.IsActive == getActive && ids.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                return records; 
            }
        }

        //find managed froups by query
        public virtual Task<List<PersonGroup>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where(x => x.GroupManagers.Any(y => y.Id == userId) && x.IsActive == getActive &&
                    (x.PrsGroupName.Contains(query) || x.PrsGroupAltName.Contains(query))).ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(PersonGroup[] records)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.PersonGroups.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();
                            dbContext.PersonGroups.Add(record);
                        }
                        else
                        {
                            dbEntry.CopyModifiedProps(record);
                        }
                    }
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);
                    trans.Complete();
                }
            }
            if (errorMessage == "") { return new DBResult(); }
            else
            {
                return new DBResult
                {
                    StatusCode = HttpStatusCode.Conflict,
                    StatusDescription = "Errors editing records:\n" + errorMessage
                };
            }
        }

        // Delete records by their Ids
        public virtual async Task<DBResult> DeleteAsync(string[] ids)
        {
            var errorMessage = "";
            
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                //tasks prior to desactivating:
                //running PersonService and deleting Persons from GroupPersons and GroupManagers
                var personIds = await dbContext.Persons.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);
                var serviceResult = await personService.EditPersonGroupsAsync(personIds, ids, false).ConfigureAwait(false);
                if (serviceResult.StatusCode == HttpStatusCode.OK)
                { serviceResult = await personService.EditManagedGroupsAsync(personIds, ids, false).ConfigureAwait(false); }

                if (serviceResult.StatusCode != HttpStatusCode.OK) 
                { 
                    errorMessage += "Error running tasks before deleting:\n" + serviceResult.StatusDescription;
                }
                else
                {
                    using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var id in ids)
                        {
                            var dbEntry = await dbContext.PersonGroups.FindAsync(id).ConfigureAwait(false);
                            if (dbEntry != null)
                            {
                                dbEntry.IsActive = false;
                            }
                            else
                            {
                                errorMessage += string.Format("Record with Id={0} not found\n", id);
                            }
                        }
                        errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);
                        trans.Complete();
                    }                    
                }
            }
            if (errorMessage == "") { return new DBResult(); }
            else
            {
                return new DBResult
                {
                    StatusCode = HttpStatusCode.Conflict,
                    StatusDescription = "Errors deleting records:\n" + errorMessage
                };
            }
        }


        //-----------------------------------------------------------------------------------------------------------------------

        //get all group managers
        public virtual Task<List<Person>> GetGroupManagersAsync(string groupId)
        {
            if (String.IsNullOrEmpty(groupId)) throw new ArgumentNullException("groupId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons
                    .Where(x => x.ManagedGroups.Select(y => y.Id).Contains(groupId) && x.IsActive_bl == true).ToListAsync();
            }
        }

        //get all active persons not assigned to group managers
        public virtual Task<List<Person>> GetGroupManagersNotAsync(string groupId)
        {
            if (String.IsNullOrEmpty(groupId)) throw new ArgumentNullException("groupId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Persons
                    .Where(x => !x.ManagedGroups.Select(y => y.Id).Contains(groupId) && x.IsActive_bl == true).ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) managed Groups to Person
        //already implemented in PersonService.EditManagedGroupsAsync

             

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
