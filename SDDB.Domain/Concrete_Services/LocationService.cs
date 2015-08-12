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
    public class LocationService : BaseDbService<Location>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        
        //Constructors---------------------------------------------------------------------------------------------------------//

        public LocationService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get by ids
        public virtual async Task<List<Location>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.Locations
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.LocationType)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.ContactPerson)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
                
        //get by projectIds and TypeIds
        public virtual async Task<List<Location>> GetByAltIdsAsync(string[] projectIds, string[] typeIds, bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };
            typeIds = typeIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.Locations
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (typeIds.Count() == 0 || typeIds.Contains(x.LocationType_Id)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.LocationType)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.ContactPerson)
                    .ToListAsync().ConfigureAwait(false);
                
                records.FillRelatedIfNull();
                return records;
            }
        }
        
        //lookup by query
        public virtual async Task<List<Location>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Locations
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (x.LocName.Contains(query) || x.AssignedToProject.ProjectName.Contains(query)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToProject)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //lookup by query and project
        public virtual async Task<List<Location>> LookupByProjAsync(string[] projectIds, string query = "", bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.Locations
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (x.LocName.Contains(query) || x.AssignedToProject.ProjectName.Contains(query)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToProject)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in [] - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService

        //-----------------------------------------------------------------------------------------------------------------------


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes record ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var currentId = ids[i];
                var assignedAssysCount = await dbContext.AssemblyDbs
                    .CountAsync(x => x.IsActive_bl && x.AssignedToLocation_Id == currentId).ConfigureAwait(false);
                if (assignedAssysCount > 0)
                {
                    var dbEntry = await dbContext.Locations.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Some assemblies are assigned to the location {0}.\nDelete aborted.", dbEntry.LocName));
                }
            }
        }
        


        #endregion
    }
}
