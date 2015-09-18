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
    public class AssemblyDbService : BaseDbService<AssemblyDb>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IReadOnlyList<string> modifiedPropsForStatusChange = new List<string> { "AssemblyStatus_Id" };

        //Constructors---------------------------------------------------------------------------------------------------------//

        public AssemblyDbService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) 
        {
            loggedProperties = new List<string> {"AssemblyStatus_Id", "AssignedToLocation_Id", "AssyGlobalX",
                "AssyGlobalY", "AssyGlobalZ", "AssyLocalXDesign", "AssyLocalYDesign", "AssyLocalZDesign",
                "AssyLocalXAsBuilt", "AssyLocalYAsBuilt", "AssyLocalZAsBuilt", "AssyStationing", "AssyLength","Comments"};
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get by ids
        public virtual async Task<List<AssemblyDb>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.AssemblyDbs
                    .Where(x => 
                        x.AssignedToLocation.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssemblyType)
                    .Include(x => x.AssemblyStatus)
                    .Include(x => x.AssignedToLocation.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .Include(x => x.AssignedToLocation.LocationType)
                    .Include(x => x.AssemblyExt)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by projectIds, typeIds and locIds
        public virtual async Task<List<AssemblyDb>> GetByAltIdsAsync(string[] projectIds, string[] typeIds, string[] locIds,
            bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };
            typeIds = typeIds ?? new string[] { };
            locIds = locIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.AssemblyDbs
                        .Where(x =>
                            x.AssignedToLocation.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                            (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToLocation.AssignedToProject_Id)) &&
                            (typeIds.Count() == 0 || typeIds.Contains(x.AssemblyType_Id)) &&
                            (locIds.Count() == 0 || locIds.Contains(x.AssignedToLocation_Id)) &&
                            x.IsActive_bl == getActive
                            )
                        .Include(x => x.AssemblyType)
                        .Include(x => x.AssemblyStatus)
                        .Include(x => x.AssignedToLocation.AssignedToProject)
                        .Include(x => x.AssignedToLocation)
                        .Include(x => x.AssignedToLocation.LocationType)
                        .Include(x => x.AssemblyExt)
                        .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
        
        //lookup by query
        public virtual async Task<List<AssemblyDb>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.AssemblyDbs
                    .Where(x =>
                        x.AssignedToLocation.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (x.AssyName.Contains(query) || x.AssignedToLocation.AssignedToProject.ProjectName.Contains(query)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToLocation.AssignedToProject)
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //lookup by query and project
        public virtual async Task<List<AssemblyDb>> LookupByProjAsync(string[] projectIds, string query = "",
            bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.AssemblyDbs
                    .Where(x =>
                        x.AssignedToLocation.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToLocation.AssignedToProject_Id)) &&
                        (x.AssyName.Contains(query) || x.AssignedToLocation.AssignedToProject.ProjectName.Contains(query)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToLocation.AssignedToProject)
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //lookup by location
        public virtual async Task<List<AssemblyDb>> LookupByLocAsync(string locId = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.AssemblyDbs
                    .Where(x =>
                        x.AssignedToLocation.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (locId == "" || x.AssignedToLocation_Id == locId) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToLocation.AssignedToProject)
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in [] - same as BaseDbService
        // See overriden addLogEntryHelper(EFDbContext dbContext, AssemblyDb dbEntry)

        // Create and Update AssemblyExt records given in [] - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService
        // See overriden checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)


        // change status given as statusId of records given as recordIds[]
        public virtual async Task EditStatusAsync(string[] ids, string statusId)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }
            if (String.IsNullOrEmpty(statusId)) { throw new ArgumentNullException("statusId"); }

            var dbEntries = await GetAsync(ids).ConfigureAwait(false);
            if (ids.Length != dbEntries.Count) { throw new DbBadRequestException("Edit status failed, entry(ies) not found."); }

            foreach (var dbEntry in dbEntries)
            {
                if (dbEntry.AssemblyStatus_Id != statusId)
                {
                    dbEntry.AssemblyStatus_Id = statusId;
                    dbEntry.ModifiedProperties = modifiedPropsForStatusChange.ToArray();
                }
            }
            await EditAsync(dbEntries.ToArray()).ConfigureAwait(false);
        }

               
        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes AssemblyDb ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var currentId = ids[i];
                var assignedCompsCount = await dbContext.Components
                    .CountAsync(x => x.IsActive_bl && x.AssignedToAssemblyDb_Id == currentId).ConfigureAwait(false);
                if (assignedCompsCount > 0)
                {
                    var dbEntry = await dbContext.AssemblyDbs.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Some components are assigned to the assembly {0}.\nDelete aborted.", dbEntry.AssyName));
                }
            }
        }
        
        //adds log entry to the dbContext based on assembly data and user Id - override
        protected override void addLogEntryHelper(EFDbContext dbContext, AssemblyDb dbEntry)
        {
            dbContext.AssemblyLogEntrys.Add(
                new AssemblyLogEntry
                {
                    Id = Guid.NewGuid().ToString(),
                    LogEntryDateTime = DateTime.Now,
                    AssemblyDb_Id = dbEntry.Id,
                    LastSavedByPerson_Id = userId,
                    AssemblyStatus_Id = dbEntry.AssemblyStatus_Id,
                    AssignedToLocation_Id = dbEntry.AssignedToLocation_Id,
                    AssyGlobalX = dbEntry.AssyGlobalX,
                    AssyGlobalY = dbEntry.AssyGlobalY,
                    AssyGlobalZ = dbEntry.AssyGlobalZ,
                    AssyLocalXDesign = dbEntry.AssyLocalXDesign,
                    AssyLocalYDesign = dbEntry.AssyLocalYDesign,
                    AssyLocalZDesign = dbEntry.AssyLocalZDesign,
                    AssyLocalXAsBuilt = dbEntry.AssyLocalXAsBuilt,
                    AssyLocalYAsBuilt = dbEntry.AssyLocalYAsBuilt,
                    AssyLocalZAsBuilt = dbEntry.AssyLocalZAsBuilt,
                    AssyStationing = dbEntry.AssyStationing,
                    AssyLength = dbEntry.AssyLength,
                    Comments = dbEntry.Comments,
                    IsActive_bl = true
                }
            );
        }


        #endregion
    }
}
