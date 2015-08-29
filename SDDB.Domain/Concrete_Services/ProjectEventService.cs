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
    public class ProjectEventService: BaseDbService<ProjectEvent>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public ProjectEventService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }
        
        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<ProjectEvent>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.ProjectEvents
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.CreatedByPerson)
                    .Include(x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by ids
        public virtual async Task<List<ProjectEvent>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.ProjectEvents
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.CreatedByPerson)
                    .Include(x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by projectIds 
        public virtual async Task<List<ProjectEvent>> GetByProjectAsync(string[] projectIds, bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.ProjectEvents
                   .Where(x =>
                       x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                       (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                       x.IsActive_bl == getActive
                       )
                   .Include(x => x.AssignedToProject)
                   .Include(x => x.CreatedByPerson)
                   .Include(x => x.ClosedByPerson)
                   .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
                        
        //lookup by query
        public virtual async Task<List<ProjectEvent>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.ProjectEvents
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        x.EventName.Contains(query) &&
                        x.IsActive_bl == getActive
                    )
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //lookup by query and project
        public virtual async Task<List<ProjectEvent>> LookupByProjAsync(string[] projectIds = null, string query = "", bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.ProjectEvents
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        x.EventName.Contains(query) &&
                        x.IsActive_bl == getActive
                    )
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in [] - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService
        // See overriden checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes record ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var currentId = ids[i];
                var dbEntry = await dbContext.ProjectEvents.FindAsync(currentId).ConfigureAwait(false);
                if (String.IsNullOrEmpty(dbEntry.ClosedByPerson_Id) || dbEntry.EventClosed == null)
                {
                    throw new DbBadRequestException(
                        string.Format("Event {0} not closed and/or no closed-by value specified.\nDelete aborted.",
                            dbEntry.EventName));
                }
            }
        }

        #endregion
    }
}
