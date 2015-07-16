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

        //get all : no by-group filtering
        public virtual async Task<List<Person>> GetAllAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x => x.IsActive_bl == getActive).ToListAsync().ConfigureAwait(false);
                return records; 
            }
        }

        //get by ids : no by-group filtering
        public virtual async Task<List<Person>> GetAllAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x => x.IsActive_bl == getActive && ids.Contains(x.Id))
                    .ToListAsync().ConfigureAwait(false);
                return records; 
            }
        }

        //get all : WITH by-group filtering
        public virtual async Task<List<Person>> GetAsync(string userId, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.PersonGroups
                    .Where(x => x.GroupManagers.Any(y => y.Id == userId)).SelectMany(x => x.GroupPersons).Distinct()
                    .Where(x => x.IsActive_bl == getActive).ToListAsync().ConfigureAwait(false);
                return records;
            }
        }
        
        //get persons without DBUser
        public virtual async Task<List<Person>> GetWoDBUserAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x => x.DBUser == null && x.IsActive_bl == getActive).ToListAsync().ConfigureAwait(false);
                return records; 
            }
        }

        //find all persons by query
        public virtual Task<List<Person>> LookupAllAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons.Where(x => x.IsActive_bl == getActive &&
                    (x.LastName.Contains(query) || x.FirstName.Contains(query) || x.Initials.Contains(query))).ToListAsync();
            }
        }

        //find managed persons by query
        public virtual Task<List<Person>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups
                    .Where(x => x.GroupManagers.Any(y => y.Id == userId)).SelectMany(x => x.GroupPersons).Distinct()
                    .Where(x => x.IsActive_bl == getActive && 
                        (x.LastName.Contains(query) || x.FirstName.Contains(query) || x.Initials.Contains(query))).ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(Person[] records)
        {
            var errorMessage = ""; 

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
                //running dbUserService and deleting the SDDB accounts
                var serviceResult = await dbUserService.DeleteAsync(ids).ConfigureAwait(false);
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
                            var dbEntry = await dbContext.Persons.FindAsync(id).ConfigureAwait(false);
                            if (dbEntry != null)
                            {
                                dbEntry.IsActive_bl = false;
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

        //get all person projects
        public virtual Task<List<Project>> GetPersonProjectsAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) throw new ArgumentNullException("personId");

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
            if (String.IsNullOrEmpty(personId)) throw new ArgumentNullException("personId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Projects
                    .Where(x => !x.ProjectPersons.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) projects to Person
        public virtual async Task<DBResult> EditPersonProjectsAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            if (ids == null || ids.Length == 0 || idsAddRem == null || idsAddRem.Length == 0)
                return new DBResult { StatusCode = HttpStatusCode.BadRequest, StatusDescription = "arguments missing" };

            var errorMessage = "";
            
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var persons = await dbContext.Persons.Include(x => x.PersonProjects).Where(x => ids.Contains(x.Id))
                       .ToListAsync().ConfigureAwait(false);
                    var projects = await dbContext.Projects.Where(x => idsAddRem.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                    
                    foreach (var person in persons)
                    {
                        foreach (var project in projects)
                        {
                            if (isAdd) { if (!person.PersonProjects.Contains(project)) person.PersonProjects.Add(project); }
                            else { if (person.PersonProjects.Contains(project)) person.PersonProjects.Remove(project); }
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

        //-----------------------------------------------------------------------------------------------------------------------

        //get all person groups
        public virtual Task<List<PersonGroup>> GetPersonGroupsAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) throw new ArgumentNullException("personId");

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
            if (String.IsNullOrEmpty(personId)) throw new ArgumentNullException("personId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups
                    .Where(x => !x.GroupPersons.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }


        //Add (or Remove  when set isAdd to false) Groups to Person
        public virtual async Task<DBResult> EditPersonGroupsAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            if (ids == null || ids.Length == 0 || idsAddRem == null || idsAddRem.Length == 0)
                return new DBResult { StatusCode = HttpStatusCode.BadRequest, StatusDescription = "arguments missing" };

            var errorMessage = "";
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var persons = await dbContext.Persons.Include(x => x.PersonGroups).Where(x => ids.Contains(x.Id))
                        .ToListAsync().ConfigureAwait(false);
                    var groups = await dbContext.PersonGroups.Where(x => idsAddRem.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                    
                    foreach (var person in persons)
                    {
                        foreach (var group in groups)
                        {
                            if (isAdd) { if (!person.PersonGroups.Contains(group)) person.PersonGroups.Add(group); }
                            else { if (person.PersonGroups.Contains(group)) person.PersonGroups.Remove(group); }
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

        //-----------------------------------------------------------------------------------------------------------------------

        //get all managed groups
        public virtual Task<List<PersonGroup>> GetManagedGroupsAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) throw new ArgumentNullException("personId");

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
            if (String.IsNullOrEmpty(personId)) throw new ArgumentNullException("personId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups
                    .Where(x => !x.GroupManagers.Any(y => y.Id == personId) && x.IsActive == true).ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) managed Groups to Person
        public virtual async Task<DBResult> EditManagedGroupsAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            if (ids == null || ids.Length == 0 || idsAddRem == null || idsAddRem.Length == 0)
                return new DBResult { StatusCode = HttpStatusCode.BadRequest, StatusDescription = "arguments missing" };

            var errorMessage = "";
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var persons = await dbContext.Persons.Include(x => x.ManagedGroups).Where(x => ids.Contains(x.Id))
                                        .ToListAsync().ConfigureAwait(false);
                    var groups = await dbContext.PersonGroups.Where(x => idsAddRem.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                    foreach (var person in persons)
                    {
                        foreach (var group in groups)
                        {
                            if (isAdd) { if (!person.ManagedGroups.Contains(group)) person.ManagedGroups.Add(group); }
                            else { if (person.ManagedGroups.Contains(group)) person.ManagedGroups.Remove(group); }
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


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion


    }
}
