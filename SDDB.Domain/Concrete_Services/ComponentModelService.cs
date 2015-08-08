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
    public class ComponentModelService : BaseDbService<ComponentModel>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        public ComponentModelService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual Task<List<ComponentModel>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ComponentModels.Where(x => x.IsActive_bl == getActive).ToListAsync();
            }
        }

        //get by ids
        public virtual Task<List<ComponentModel>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ComponentModels.Where(x => x.IsActive_bl == getActive && ids.Contains(x.Id)).ToListAsync();
            }
        }

        //lookup by query
        public virtual Task<List<ComponentModel>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ComponentModels
                    .Where(x =>
                        (x.CompModelName.Contains(query) || x.CompModelAltName.Contains(query)) &&
                        x.IsActive_bl == getActive
                    ).ToListAsync();
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
                    .CountAsync(x => x.IsActive_bl && x.ComponentModel_Id == currentId).ConfigureAwait(false);
                if (assignedCompsCount > 0)
                {
                    var dbEntry = await dbContext.ComponentModels.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Some components have the model {0} assigned to it.\nDelete aborted.", dbEntry.CompModelName));
                }
            }
        }

        #endregion
    }
}
