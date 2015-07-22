using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
using SDDB.Domain.Abstract;

namespace SDDB.Domain.Services
{
    public abstract class BaseDbService<T> where T: IDbEntity
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        protected IDbContextScopeFactory contextScopeFac;
        protected string userId;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public BaseDbService(IDbContextScopeFactory contextScopeFac, string userId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("userId"); }

            this.contextScopeFac = contextScopeFac;
            this.userId = userId;
        }

        //Main Methods---------------------------------------------------------------------------------------------------------//

        // Create and Update records given in []
        public virtual async Task<List<string>> EditAsync(T[] records)
        {
            var newEntryIds = await executeOnDbScope(async (dbContext) =>
            {
                return await editHelperAsync(dbContext, records).ConfigureAwait(false);
            })
            .ConfigureAwait(false);

            return newEntryIds;
        }

        // Delete records by their Ids
        public virtual async Task DeleteAsync(string[] ids)
        {
            await executeOnDbScope<Task>((dbContext) =>
            {
                return Task.FromResult(deleteHelperAsync(dbContext, ids));
            })
            .ConfigureAwait(false);

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dbEntries = await dbContext.PersonLogEntrys.Where(x => ids.Contains(x.Id))
                        .ToListAsync().ConfigureAwait(false);
                    if (dbEntries.Count() == 0) throw new ArgumentException("Entry(ies) not found");
                    dbEntries.ForEach(x => x.IsActive_bl = false);
                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
                    trans.Complete();
                }
            }
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - editing single Db Entry
        protected async Task<string> editHelperAsync(EFDbContext dbContext, T record)
        {
            if (record == null) { throw new ArgumentNullException("record"); }

            var dbEntry = (T)(await dbContext.Set(typeof(T)).FindAsync(record.Id).ConfigureAwait(false));
            if (dbEntry == null)
            {
                record.Id = Guid.NewGuid().ToString();
                dbContext.Set(typeof(T)).Add(record);
                return record.Id;
            }
            dbEntry.CopyProperties(record);
            return null;
        }

        //helper - editing Db Entries in array
        protected async Task<List<string>> editHelperAsync(EFDbContext dbContext, T[] records)
        {
            if (records == null || records.Length == 0) { throw new ArgumentNullException("records"); }

            var newEntryIds = new List<string>();

            for (int i = 0; i < records.Length; i++)
            {
                var newEntryId = await editHelperAsync(dbContext, records[0]).ConfigureAwait(false);
                if (!String.IsNullOrEmpty(newEntryId)) { newEntryIds.Add(newEntryId); }
            }
            return newEntryIds;
        }

        //helper deleting db entries (setting isActive_bl to false) 
        protected async Task deleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            var dbEntries = await ((IQueryable<T>)dbContext.Set(typeof(T)).AsQueryable()).Where(x => ids.Contains(x.Id))
                .ToListAsync().ConfigureAwait(false);
            if (dbEntries.Count() == 0) throw new ArgumentException("Entry(ies) not found");
            dbEntries.ForEach(x => x.IsActive_bl = false);
        }
        
        //wrapper for dbcontext scope factory - version for Write
        protected async Task<TResult> executeOnDbScope<TResult>(Func<EFDbContext,Task<TResult>> innerFunction)
        {
            TResult result = default(TResult);
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    result = await innerFunction(dbContext).ConfigureAwait(false);

                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
                    trans.Complete();
                }
            }
            return result;
        }


        #endregion
    }
}
