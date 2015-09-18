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
    public class ComponentService : BaseDbService<Component>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        public ComponentService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) 
        {
            loggedProperties = new List<string> { "ComponentStatus_Id", "AssignedToProject_Id","AssignedToAssemblyDb_Id",
                "LastCalibrationDate", "Comments" };
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get by ids
        public virtual async Task<List<Component>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.Components
                    .Where(x =>
                        (x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) ||
                            (x.AssignedToAssemblyDb_Id != null &&
                                 x.AssignedToAssemblyDb.AssignedToLocation
                                    .AssignedToProject.ProjectPersons.Any(y => y.Id == userId))) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.ComponentType)
                    .Include(x => x.ComponentStatus)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb)
                    .Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
                
        //get by projectIds, typeIds and assyIds
        public virtual async Task<List<Component>> GetByAltIdsAsync(string[] projectIds, string[] typeIds, string[] assyIds,
            bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };
            typeIds = typeIds ?? new string[] { };
            assyIds = assyIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.Components
                   .Where(x =>
                        (x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) ||
                            (x.AssignedToAssemblyDb_Id != null &&
                                x.AssignedToAssemblyDb.AssignedToLocation
                                    .AssignedToProject.ProjectPersons.Any(y => y.Id == userId))) &&
                       (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id) ||
                            (x.AssignedToAssemblyDb_Id != null && 
                                projectIds.Contains(x.AssignedToAssemblyDb.AssignedToLocation.AssignedToProject_Id))) &&
                       (typeIds.Count() == 0 || typeIds.Contains(x.ComponentType_Id)) &&
                       (assyIds.Count() == 0 || assyIds.Contains(x.AssignedToAssemblyDb_Id)) &&
                       x.IsActive_bl == getActive
                       )
                   .Include(x => x.ComponentType)
                   .Include(x => x.ComponentStatus)
                   .Include(x => x.AssignedToProject)
                   .Include(x => x.AssignedToAssemblyDb)
                   .Include(x => x.ComponentExt)
                   .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
                        
        //lookup by query
        public virtual async Task<List<Component>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.Components
                    .Where(x =>
                        (x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) ||
                            (x.AssignedToAssemblyDb_Id != null &&
                                 x.AssignedToAssemblyDb.AssignedToLocation
                                    .AssignedToProject.ProjectPersons.Any(y => y.Id == userId))) &&
                        (x.CompName.Contains(query) || x.AssignedToProject.ProjectName.Contains(query)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToProject)
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //lookup by query and project
        public virtual async Task<List<Component>> LookupByProjAsync(string[] projectIds, string query = "",
            bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.Components
                    .Where(x =>
                        (x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) ||
                            (x.AssignedToAssemblyDb_Id != null &&
                                 x.AssignedToAssemblyDb.AssignedToLocation
                                    .AssignedToProject.ProjectPersons.Any(y => y.Id == userId))) &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id) ||
                            (x.AssignedToAssemblyDb_Id != null &&
                                projectIds.Contains(x.AssignedToAssemblyDb.AssignedToLocation.AssignedToProject_Id))) &&
                        (x.CompName.Contains(query) || x.AssignedToProject.ProjectName.Contains(query)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToProject)
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in [] - same as BaseDbService
        // See overriden addLogEntryHelper(EFDbContext dbContext, AssemblyDb dbEntry)

        // Create and Update ComponentExt records given in [] - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService

        //-----------------------------------------------------------------------------------------------------------------------
        
        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //adds log entry to the dbContext based on assembly data and user Id - override
        protected override void addLogEntryHelper(EFDbContext dbContext, Component dbEntry)
        {
            dbContext.ComponentLogEntrys.Add(
                new ComponentLogEntry
                {
                    Id = Guid.NewGuid().ToString(),
                    Component_Id = dbEntry.Id,
                    LastSavedByPerson_Id = userId,
                    LogEntryDateTime = DateTime.Now,
                    ComponentStatus_Id = dbEntry.ComponentStatus_Id,
                    AssignedToProject_Id = dbEntry.AssignedToProject_Id,
                    AssignedToAssemblyDb_Id = dbEntry.AssignedToAssemblyDb_Id,
                    LastCalibrationDate = dbEntry.LastCalibrationDate,
                    Comments = dbEntry.Comments,
                    IsActive_bl = true
                }
            );
        }



        #endregion
    }
}
