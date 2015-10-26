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
    public class PersonService : BaseDbService<Person>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

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
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons
                    .Where(x => 
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                    )
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //get persons without DBUser, no by-group filtering
        public virtual async Task<List<Person>> GetWoDBUserAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons
                    .Where(x =>
                            x.DBUser == null && 
                            x.IsActive_bl == getActive
                        ).ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //look up all persons by query, no by-group filtering
        public virtual async Task<List<Person>> LookupAllAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x =>
                        (x.LastName.Contains(query) || x.FirstName.Contains(query) || x.Initials.Contains(query)) &&
                        x.IsActive_bl == getActive
                    )
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //get: WITH by-group filtering
        public virtual async Task<List<Person>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x =>
                        x.PersonGroups.Any(y => y.GroupManagers.Any(z => z.Id == userId)) &&
                        x.IsActive_bl == getActive
                    )
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //look up managed persons by query, WITH by-group filtering
        public async virtual Task<List<Person>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x =>
                        x.PersonGroups.Any(y => y.GroupManagers.Any(z => z.Id == userId)) &&
                        (x.LastName.Contains(query) || x.FirstName.Contains(query) || x.Initials.Contains(query)) &&
                        x.IsActive_bl == getActive
                    )
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //look up project persons by query, WITH BY-PROJECT filtering
        public async virtual Task<List<Person>> LookupFromProjectAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Persons.Where(x =>
                        x.PersonProjects.Any(y => y.ProjectPersons.Any(z => z.Id == userId)) &&
                        (x.LastName.Contains(query) || x.FirstName.Contains(query) || x.Initials.Contains(query)) &&
                        x.IsActive_bl == getActive
                    )
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []  - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService
        // See overriden checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)

        //-----------------------------------------------------------------------------------------------------------------------

        //get all person projects
        public virtual Task<List<Project>> GetPersonProjectsAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Projects.Where(x =>
                        x.ProjectPersons.Any(y => y.Id == personId) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }

        //get all active projects not assigned to person
        public virtual Task<List<Project>> GetPersonProjectsNotAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Projects.Where(x =>
                        !x.ProjectPersons.Any(y => y.Id == personId) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) projects to Person
        //use generic version AddRemoveRelated from BaseDbService 
        
        //-----------------------------------------------------------------------------------------------------------------------

        //get all person groups
        public virtual Task<List<PersonGroup>> GetPersonGroupsAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where(x =>
                        x.GroupPersons.Any(y => y.Id == personId) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }

        //get all active groups not assigned to person
        public virtual Task<List<PersonGroup>> GetPersonGroupsNotAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where(x => 
                        !x.GroupPersons.Any(y => y.Id == personId) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Groups to Person
        //use generic version AddRemoveRelated from BaseDbService 

        //-----------------------------------------------------------------------------------------------------------------------

        //get all managed groups
        public virtual Task<List<PersonGroup>> GetManagedGroupsAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where(x =>
                        x.GroupManagers.Any(y => y.Id == personId) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }

        //get all active groups not assigned to person
        public virtual Task<List<PersonGroup>> GetManagedGroupsNotAsync(string personId)
        {
            if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where(x =>
                        !x.GroupManagers.Any(y => y.Id == personId) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) managed Groups to Person
        //use generic version AddRemoveRelated from BaseDbService 


        //-----------------------------------------------------------------------------------------------------------------------

        //get all group managers
        public virtual Task<List<Person>> GetGroupManagersAsync(string groupId)
        {
            if (String.IsNullOrEmpty(groupId)) { throw new ArgumentNullException("groupId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons
                    .Where(x => x.ManagedGroups.Select(y => y.Id).Contains(groupId) && x.IsActive_bl)
                    .ToListAsync();
            }
        }

        //get all active persons not assigned to group managers
        public virtual Task<List<Person>> GetGroupManagersNotAsync(string groupId)
        {
            if (String.IsNullOrEmpty(groupId)) { throw new ArgumentNullException("groupId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Persons
                    .Where(x => !x.ManagedGroups.Select(y => y.Id).Contains(groupId) && x.IsActive_bl)
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) managed Groups to Person
        //use generic version AddRemoveRelated from BaseDbService 

        
        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes LocationModel ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var currentId = ids[i];
                if (await dbContext.Persons.AnyAsync(x => x.Id == currentId && x.DBUser != null).ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Persons.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Person {0} {1} has a SDDB User account assigned to it.\nDelete aborted.",
                            dbEntry.FirstName, dbEntry.LastName));
                }
                if (await dbContext.Persons.AnyAsync(x => x.Id == currentId && x.PersonGroups.Count > 0).ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Persons.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Person {0} {1} is assigned to person groups.\nDelete aborted.",
                            dbEntry.FirstName, dbEntry.LastName));
                }
                if (await dbContext.Persons.AnyAsync(x => x.Id == currentId && x.ManagedGroups.Count > 0).ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Persons.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Person {0} {1} is a manager of person groups.\nDelete aborted.",
                            dbEntry.FirstName, dbEntry.LastName));
                }
                if (await dbContext.Persons.AnyAsync(x => x.Id == currentId && x.PersonProjects.Count > 0).ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Persons.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Person {0} {1} is assigned to projects.\nDelete aborted.",
                            dbEntry.FirstName, dbEntry.LastName));
                }
            }
        }


        #endregion
    }
}
