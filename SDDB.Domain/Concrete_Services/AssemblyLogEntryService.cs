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
    public class AssemblyLogEntryService : BaseDbService<AssemblyLogEntry>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        public AssemblyLogEntryService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//
        
        //get by ids
        public virtual async Task<List<AssemblyLogEntry>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.AssemblyLogEntrys
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssemblyDb)
                    .Include(x => x.EnteredByPerson)
                    .Include(x => x.AssemblyStatus)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
                
        //get by projectIds and componentIds
        public virtual async Task<List<AssemblyLogEntry>> GetByAltIdsAsync( string[] projectIds, string[] assemblyIds,
            string[] personIds, DateTime? startDate, DateTime? endDate, bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };
            assemblyIds = assemblyIds ?? new string[] { };
            personIds = personIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.AssemblyLogEntrys
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (assemblyIds.Count() == 0 || assemblyIds.Contains(x.AssemblyDb_Id)) &&
                        (personIds.Count() == 0 || personIds.Contains(x.EnteredByPerson_Id)) &&
                        (startDate == null || x.LogEntryDateTime >= startDate) &&
                        (endDate == null || x.LogEntryDateTime <= endDate) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssemblyDb)
                    .Include(x => x.EnteredByPerson)
                    .Include(x => x.AssemblyStatus)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        // See overriden editHelperAsync(EFDbContext dbContext, AssmeblyLogEntry record)

        // Delete records by their Ids - same as BaseDbService


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - editing single Db Entry - overriden from BaseDbService
        protected override async Task<string> editHelperAsync(EFDbContext dbContext, AssemblyLogEntry record)
        {
            var dbEntry = await dbContext.AssemblyLogEntrys.FindAsync(record.Id).ConfigureAwait(false);
            if (dbEntry == null)
            {
                record.Id = Guid.NewGuid().ToString();
                record.EnteredByPerson_Id = record.EnteredByPerson_Id ?? userId;
                dbContext.AssemblyLogEntrys.Add(record);
                return record.Id;
            }
            dbEntry.CopyModifiedProps(record);
            return null;
        }

        #endregion
    }
}
