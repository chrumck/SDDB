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
using System.Diagnostics;

namespace SDDB.Domain.Services
{
    public class PersonService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;
        private DBUserService dbUserService;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonService(IDbContextScopeFactory contextScopeFac, DBUserService dbUserService)
        {
            this.contextScopeFac = contextScopeFac;
            this.dbUserService = dbUserService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<Person>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x => x.IsActive == getActive).ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Person).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual async Task<List<Person>> GetAsync(string[] ids, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x => x.IsActive == getActive && ids.Contains(x.Id))
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Person).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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

        //get persons without DBUser
        public virtual async Task<List<Person>> GetWoDBUserAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x => x.DBUser == null && x.IsActive == getActive).ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Person).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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

        //find all persons by query
        public virtual Task<List<Person>> LookupAllAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons.Where(x => x.IsActive == getActive &&
                    (x.LastName.Contains(query) || x.FirstName.Contains(query) || x.Initials.Contains(query))).ToListAsync();
            }
        }

        //find managed persons by query
        public virtual Task<List<Person>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.PersonGroups
                    .Where(x => x.GroupManagers.Any(y => y.Id == userId)).SelectMany(x => x.GroupPersons)
                    .Distinct().Where(x => x.IsActive == getActive &&
                        (x.LastName.Contains(query) || x.FirstName.Contains(query) || x.Initials.Contains(query))).ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(Person[] records)
        {
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.Persons.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();
                            dbContext.Persons.Add(record);
                        }
                        else
                        {
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(Person).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
                //running dbUserService and deleting the SDDB accounts
                serviceResult = await dbUserService.DeleteAsync(ids).ConfigureAwait(false);
                //removing persons from PersonGroups
                var groupIds = await dbContext.PersonGroups.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);
                if (serviceResult.StatusCode == HttpStatusCode.OK)
                    serviceResult = await EditPersonGroupsAsync(ids, groupIds, false).ConfigureAwait(false);
                if (serviceResult.StatusCode == HttpStatusCode.OK)
                    serviceResult = await EditManagedGroupsAsync(ids, groupIds, false).ConfigureAwait(false);
                //removing persons from PersonProjects
                var projectIds = await dbContext.Projects.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);
                if (serviceResult.StatusCode == HttpStatusCode.OK)
                    serviceResult = await EditPersonProjectsAsync(ids, projectIds, false).ConfigureAwait(false);

                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (serviceResult.StatusCode == HttpStatusCode.OK)
                    {
                        foreach (var id in ids)
                        {
                            var dbEntry = await dbContext.Persons.FindAsync(id).ConfigureAwait(false);
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

        //get all person projects
        public virtual Task<List<Project>> GetPersonProjectsAsync(string personId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                return dbContext.Projects
                    .Where(x => x.ProjectPersons.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }

        //get all active projects not assigned to person
        public virtual Task<List<Project>> GetPersonProjectsNotAsync(string personId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Projects
                    .Where(x => !x.ProjectPersons.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) projects to Person
        public virtual async Task<DBResult> EditPersonProjectsAsync(string[] personIds, string[] projectIds, bool isAdd)
        {
            var errorMessage = ""; var serviceResult = new DBResult();
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var persons = await dbContext.Persons.Include(x => x.PersonProjects).Where(x => personIds.Contains(x.Id))
                       .ToListAsync().ConfigureAwait(false);

                    var projects = await dbContext.Projects.Where(x => projectIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                    foreach (var person in persons)
                    {
                        foreach (var project in projects)
                        {
                            if (isAdd) { if (!person.PersonProjects.Contains(project)) person.PersonProjects.Add(project); }
                            else { if (person.PersonProjects.Contains(project)) person.PersonProjects.Remove(project); }
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
                    trans.Complete();
                }
            }

            if (errorMessage == "" && serviceResult.StatusCode == HttpStatusCode.OK) return serviceResult;
            else return new DBResult { StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage + serviceResult.StatusDescription };
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //get all person groups
        public virtual Task<List<PersonGroup>> GetPersonGroupsAsync(string personId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.PersonGroups
                    .Where(x => x.GroupPersons.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }

        //get all active groups not assigned to person
        public virtual Task<List<PersonGroup>> GetPersonGroupsNotAsync(string personId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.PersonGroups
                    .Where(x => !x.GroupPersons.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }


        //Add (or Remove  when set isAdd to false) Groups to Person
        public virtual async Task<DBResult> EditPersonGroupsAsync(string[] personIds, string[] groupIds, bool isAdd)
        {
            var errorMessage = ""; var serviceResult = new DBResult();
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var persons = await dbContext.Persons.Include(x => x.PersonGroups).Where(x => personIds.Contains(x.Id))
                        .ToListAsync().ConfigureAwait(false);

                    var groups = await dbContext.PersonGroups.Where(x => groupIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                    foreach (var person in persons)
                    {
                        foreach (var group in groups)
                        {
                            if (isAdd) { if (!person.PersonGroups.Contains(group)) person.PersonGroups.Add(group); }
                            else { if (person.PersonGroups.Contains(group)) person.PersonGroups.Remove(group); }
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
                    trans.Complete();
                }
            }

            if (errorMessage == "" && serviceResult.StatusCode == HttpStatusCode.OK) return serviceResult;
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage + serviceResult.StatusDescription
            };
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //get all managed groups
        public virtual Task<List<PersonGroup>> GetManagedGroupsAsync(string personId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups
                    .Where(x => x.GroupManagers.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }

        //get all active groups not assigned to person
        public virtual Task<List<PersonGroup>> GetManagedGroupsNotAsync(string personId)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.PersonGroups
                    .Where(x => !x.GroupManagers.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) managed Groups to Person
        public virtual async Task<DBResult> EditManagedGroupsAsync(string[] personIds, string[] groupIds, bool isAdd)
        {
            var errorMessage = ""; var serviceResult = new DBResult();
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var persons = await dbContext.Persons.Include(x => x.ManagedGroups).Where(x => personIds.Contains(x.Id))
                                        .ToListAsync().ConfigureAwait(false);

                    var groups = await dbContext.PersonGroups.Where(x => groupIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                    foreach (var person in persons)
                    {
                        foreach (var group in groups)
                        {
                            if (isAdd) { if (!person.ManagedGroups.Contains(group)) person.ManagedGroups.Add(group); }
                            else { if (person.ManagedGroups.Contains(group)) person.ManagedGroups.Remove(group); }
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
                    trans.Complete();
                }
            }

            if (errorMessage == "" && serviceResult.StatusCode == HttpStatusCode.OK) return serviceResult;
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage + serviceResult.StatusDescription
            };
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion


    }
}
