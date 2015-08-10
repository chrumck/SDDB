using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Mehdime.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.Abstract;
using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace SDDB.Domain.Services
{
    public class ComponentStatusService : BaseDbService<ComponentStatus>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public ComponentStatusService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual Task<List<ComponentStatus>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ComponentStatuss.Where(x => x.IsActive_bl == getActive).ToListAsync();
            }
        }

        //get by ids
        public virtual Task<List<ComponentStatus>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ComponentStatuss.Where(x => x.IsActive_bl == getActive && ids.Contains(x.Id)).ToListAsync();
            }
        }

        //lookup by query
        public virtual Task<List<ComponentStatus>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ComponentStatuss
                    .Where(x =>
                        (x.CompStatusName.Contains(query) || x.CompStatusAltName.Contains(query)) &&
                        x.IsActive_bl == getActive
                    )
                    .ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []  - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService
        // See overriden checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes ComponentModel ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var currentId = ids[i];
                var assignedCompsCount = await dbContext.Components
                    .CountAsync(x => x.IsActive_bl && x.ComponentStatus_Id == currentId).ConfigureAwait(false);
                if (assignedCompsCount > 0)
                {
                    var dbEntry = await dbContext.ComponentStatuss.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Some components have the status {0} assigned to it.\nDelete aborted.", dbEntry.CompStatusName));
                }
            }
        }

        #endregion
    }
}
