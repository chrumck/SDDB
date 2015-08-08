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
    public class AssemblyModelService : BaseDbService<AssemblyModel>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        public AssemblyModelService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual Task<List<AssemblyModel>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.AssemblyModels.Where(x => x.IsActive_bl == getActive).ToListAsync();
            }
        }

        //get by ids
        public virtual Task<List<AssemblyModel>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.AssemblyModels.Where(x => x.IsActive_bl == getActive && ids.Contains(x.Id)).ToListAsync();
            }
        }
        
        //lookup by query
        public virtual Task<List<AssemblyModel>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.AssemblyModels
                    .Where(x =>
                        (x.AssyModelName.Contains(query) || x.AssyModelAltName.Contains(query) ) &&
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

        //helper - check before deleting records, takes AssemblyModel ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var currentId = ids[i];
                var assignedAssysCount = await dbContext.AssemblyDbs
                    .CountAsync(x => x.IsActive_bl && x.AssemblyModel_Id == currentId).ConfigureAwait(false);
                if (assignedAssysCount > 0)
                {
                    var dbEntry = await dbContext.AssemblyModels.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Some assemblies have the model {0} assigned to it.\nDelete aborted.", dbEntry.AssyModelName));
                }
            }
        }

        #endregion
    }
}
