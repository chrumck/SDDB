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
    public class PersonActivityTypeService : BaseDbService<PersonActivityType>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonActivityTypeService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual Task<List<PersonActivityType>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonActivityTypes.Where(x => x.IsActive_bl == getActive).ToListAsync();
            }
        }

        //get by ids
        public virtual Task<List<PersonActivityType>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonActivityTypes.Where(x => x.IsActive_bl == getActive && ids.Contains(x.Id)).ToListAsync();
            }
        }


        //lookup by query
        public virtual async Task<List<PersonActivityType>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.PersonActivityTypes.Where(x =>
                        x.ActivityTypeName.Contains(query) &&
                        x.IsActive_bl == getActive
                    )
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []  - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService
        // see overriden checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes LocationModel ids array
        //protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        //{
        //    for (int i = 0; i < ids.Length; i++)
        //    {
        //        var currentId = ids[i];
        //        var assignedLogEntriesCount = await dbContext.PersonLogEntrys
        //            .CountAsync(x => x.IsActive_bl && x.PersonActivityType_Id == currentId).ConfigureAwait(false);
        //        if (assignedLogEntriesCount > 0)
        //        {
        //            var dbEntry = await dbContext.PersonActivityTypes.FindAsync(currentId).ConfigureAwait(false);
        //            throw new DbBadRequestException(
        //                string.Format("Some Person Log Entries have the type {0} assigned to it.\nDelete aborted.", dbEntry.ActivityTypeName));
        //        }
        //    }
        //}

        #endregion
    }
}
