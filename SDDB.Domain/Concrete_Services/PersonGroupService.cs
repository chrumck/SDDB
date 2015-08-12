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
    public class PersonGroupService : BaseDbService<PersonGroup>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonGroupService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual Task<List<PersonGroup>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where(x => x.IsActive_bl == getActive).ToListAsync();
            }
        }

        //get by ids
        public virtual Task<List<PersonGroup>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups.Where(x => x.IsActive_bl == getActive && ids.Contains(x.Id)).ToListAsync();
            }
        }


        //lookup by query
        public virtual async Task<List<PersonGroup>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.PersonGroups.Where(x =>
                        x.GroupManagers.Any(y => y.Id == userId) &&    
                        x.PrsGroupName.Contains(query) &&
                        x.IsActive_bl == getActive
                    )
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }


        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []  - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService
        // See overriden checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)

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

        //-----------------------------------------------------------------------------------------------------------------------

        //get all group persons
        public virtual Task<List<Person>> GetGroupPersonsAsync(string groupId)
        {
            if (String.IsNullOrEmpty(groupId)) { throw new ArgumentNullException("groupId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons
                    .Where(x => x.PersonGroups.Select(y => y.Id).Contains(groupId) && x.IsActive_bl)
                    .ToListAsync();
            }
        }

        //get all active persons not assigned to group persons
        public virtual Task<List<Person>> GetGroupPersonsNotAsync(string groupId)
        {
            if (String.IsNullOrEmpty(groupId)) { throw new ArgumentNullException("groupId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Persons
                    .Where(x => !x.PersonGroups.Select(y => y.Id).Contains(groupId) && x.IsActive_bl)
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) person Groups to Person
        //use generic version AddRemoveRelated from BaseDbService 

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes LocationModel ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var dbEntry = await dbContext.PersonGroups.FindAsync(ids[i]).ConfigureAwait(false);
                var GroupPersonsCount  = dbEntry.GroupPersons.Count;
                var GroupManagersCount = dbEntry.GroupManagers.Count;
                if (GroupPersonsCount + GroupManagersCount > 0)
                {
                    throw new DbBadRequestException(
                        string.Format("There are {0} persons and {1} managers assigned to the group {2}.\nDelete aborted.",
                            GroupPersonsCount, GroupManagersCount, dbEntry.PrsGroupName));
                }
            }
        }


        #endregion
    }
}
