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
    public class ComponentLogEntryService : BaseDbService<ComponentLogEntry>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public ComponentLogEntryService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get by ids
        public virtual async Task<List<ComponentLogEntry>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.ComponentLogEntrys
                    .Where(x =>
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.Component)
                    .Include(x => x.LastSavedByPerson)
                    .Include(x => x.ComponentStatus)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
                
        //get by projectIds and componentIds
        public virtual async Task<List<ComponentLogEntry>> GetByAltIdsAsync(string[] projectIds, string[] componentIds, 
            string[] compTypeIds, string[] personIds, DateTime? startDate, DateTime? endDate , bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };
            componentIds = componentIds ?? new string[] { };
            compTypeIds = compTypeIds ?? new string[] { };
            personIds = personIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.ComponentLogEntrys
                    .Where(x =>
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (componentIds.Count() == 0 || componentIds.Contains(x.Component_Id)) &&
                        (compTypeIds.Count() == 0 || compTypeIds.Contains(x.Component.ComponentType.Id)) &&
                        (personIds.Count() == 0 || personIds.Contains(x.LastSavedByPerson_Id)) &&
                        (startDate == null || x.LogEntryDateTime >= startDate) &&
                        (endDate == null || x.LogEntryDateTime <= endDate) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.Component)
                    .Include(x => x.LastSavedByPerson)
                    .Include(x => x.ComponentStatus)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []  - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
