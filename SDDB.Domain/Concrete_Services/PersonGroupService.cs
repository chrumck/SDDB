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

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(PersonGroup).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                    foreach (var property in properties)
                    {
                        if (!property.GetMethod.IsVirtual) continue;
                        if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                        if (excludedProperties.Contains(property.Name)) continue;

                        if (property.GetValue(record) == null) property.SetValue(record, Activator.CreateInstance(property.PropertyType));
                    }
                }

                return records; 
            }
        }

        //get by ids
        public virtual async Task<List<PersonGroup>> GetAsync(string[] ids, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.PersonGroups.Where(x => x.IsActive == getActive && ids.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(PersonGroup).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                    foreach (var property in properties)
                    {
                        if (!property.GetMethod.IsVirtual) continue;
                        if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                        if (excludedProperties.Contains(property.Name)) continue;

                        if (property.GetValue(record) == null) property.SetValue(record, Activator.CreateInstance(property.PropertyType));
                    }
                }

                return records; 
            }
        }

        //find managed froups by query
        public virtual Task<List<PersonGroup>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where( x => x.GroupManagers.Select( y => y.Id).Contains(userId) && x.IsActive == getActive &&
                    (x.PrsGroupName.Contains(query) || x.PrsGroupAltName.Contains(query))).ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(PersonGroup[] records)
        {
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
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(PersonGroup).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                            foreach (var property in properties)
                            {
                                if (property.GetMethod.IsVirtual) continue;
                                if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                                if (excludedProperties.Contains(property.Name)) continue;

                                if (record.PropIsModified(property.Name)) property.SetValue(dbEntry, property.GetValue(record));
                            }
                        }
                    }
                    for (int i = 1; i <= 10; i++)
                    {
                        try
                        {
                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                            break;
                        }
                        catch (Exception e)
                        {
                            var message = e.GetBaseException().Message;
                            if (i == 10 || !message.Contains("Deadlock found when trying to get lock"))
                            {
                                return new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = message };
                            }
                        }
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                    trans.Complete();
                }
            }
            return new DBResult { StatusCode = HttpStatusCode.OK };
        }

        // Delete records by their Ids
        public virtual async Task<DBResult> DeleteAsync(string[] ids)
        {
            var errorMessage = ""; var serviceResult = new DBResult();
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                //tasks prior to desactivating:
                //running PersonService and deleting Persons from GroupPersons and GroupManagers
                var personIds = await dbContext.Persons.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);
                serviceResult = await personService.EditPersonGroupsAsync(personIds, ids, false).ConfigureAwait(false);
                if (serviceResult.StatusCode == HttpStatusCode.OK)
                    serviceResult = await personService.EditManagedGroupsAsync(personIds, ids, false).ConfigureAwait(false);

                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (serviceResult.StatusCode == HttpStatusCode.OK)
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
                        for (int i = 1; i <= 10; i++)
                        {
                            try
                            {
                                await dbContext.SaveChangesAsync().ConfigureAwait(false);
                                break;
                            }
                            catch (Exception e)
                            {
                                var message = e.GetBaseException().Message;
                                if (i == 10 || !message.Contains("Deadlock found when trying to get lock"))
                                {
                                    errorMessage += string.Format("Error Saving changes: {0}\n", message);
                                    break;
                                }
                            }
                            await Task.Delay(200).ConfigureAwait(false);
                        }
                    }
                    trans.Complete();
                }
            }
            if (errorMessage == "" && serviceResult.StatusCode == HttpStatusCode.OK) return serviceResult;
            else return new DBResult { StatusCode = HttpStatusCode.Conflict, 
                StatusDescription = "Errors deleting records:\n" + errorMessage + serviceResult.StatusDescription };
        }


        //-----------------------------------------------------------------------------------------------------------------------

        //get all group managers
        public virtual Task<List<Person>> GetGroupManagersAsync(string groupId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons
                    .Where(x => x.ManagedGroups.Select(y => y.Id).Contains(groupId) && x.IsActive == true).ToListAsync();
            }
        }

        //get all active persons not assigned to group managers
        public virtual Task<List<Person>> GetGroupManagersNotAsync(string groupId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Persons
                    .Where(x => !x.ManagedGroups.Select(y => y.Id).Contains(groupId) && x.IsActive == true).ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) managed Groups to Person
        //already implemented in PersonService.EditManagedGroupsAsync

             

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
