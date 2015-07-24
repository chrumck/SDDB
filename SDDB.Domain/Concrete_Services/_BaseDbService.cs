using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Mehdime.Entity;

using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;
using SDDB.Domain.Abstract;
using System.Diagnostics;

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
            if (records == null || records.Length == 0) { throw new ArgumentNullException("records"); }

            var newEntryIds = await dbScopeHelperAsync(async dbContext =>
            {
                await checkBeforeEditHelperAsync(dbContext, records).ConfigureAwait(false);
                return await editHelperAsync(dbContext, records).ConfigureAwait(false);
            })
                .ConfigureAwait(false);

            return newEntryIds;
        }

        // Delete records by their Ids
        public virtual async Task DeleteAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            await dbScopeHelperAsync(async dbContext =>
                {
                    await checkBeforeDeleteHelperAsync(dbContext, ids).ConfigureAwait(false);
                    return await deleteHelperAsync(dbContext, ids).ConfigureAwait(false);
                })
                .ConfigureAwait(false);
        }
                
        //Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedPropName' of entity T 
        public virtual async Task AddRemoveRelated<TAddRem> (string[] ids, string[] idsAddRem, 
            string relatedCollectionName, bool isAdd = true) where TAddRem: IDbEntity
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }
            if (idsAddRem == null || idsAddRem.Length == 0) { throw new ArgumentNullException("idsAddRem"); }
            if (String.IsNullOrEmpty(relatedCollectionName)) { throw new ArgumentNullException("relatedCollectionName"); }

            await dbScopeHelperAsync(async dbContext => 
            {
                var dbEntries = await getEntriesFromContextHelperAsync<T>(dbContext, ids, relatedCollectionName)
                    .ConfigureAwait(false);
                if (dbEntries.Count != ids.Length)
                { 
                    throw new DbBadRequestException(
                        String.Format("Add/Remove to {0} failed, entry(ies) not found",relatedCollectionName));
                }

                var dbEntriesAddRem = await getEntriesFromContextHelperAsync<TAddRem>(dbContext, idsAddRem)
                    .ConfigureAwait(false);
                if (dbEntriesAddRem.Count != idsAddRem.Length) {
                    throw new DbBadRequestException(
                        String.Format("Add/Remove to {0} failed, related entry(ies) not found", relatedCollectionName)); 
                }

                await addRemoveRelatedHelper(dbEntries, dbEntriesAddRem, relatedCollectionName, isAdd).ConfigureAwait(false);

                return default(int);
            })
            .ConfigureAwait(false);
        }

        //Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedPropName' of entity T
        //overload taking lamdba expression
        public virtual async Task AddRemoveRelated<TAddRem>(string[] ids, string[] idsAddRem,
            Expression<Func<T, ICollection<TAddRem>>> lambda, bool isAdd = true) where TAddRem : IDbEntity
        {
            var body = (MemberExpression)lambda.Body;
            if (body == null)
            {
                throw new ArgumentException(
                    string.Format("Expression '{0}' refers to a method, not a property.", lambda.ToString()));
            }
            await AddRemoveRelated<TAddRem>(ids, idsAddRem, body.Member.Name, isAdd).ConfigureAwait(false);
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //wrapper for dbcontext scope factory - version for Write
        protected async Task<TResult> dbScopeHelperAsync<TResult>(Func<EFDbContext, Task<TResult>> innerDelegate)
        {
            TResult result = default(TResult);
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    result = await innerDelegate(dbContext).ConfigureAwait(false);

                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
                    trans.Complete();
                }
            }
            return result;
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //helper - check before editing records, takes single T record
        //stub method for derived classes to implement
        protected virtual Task checkBeforeEditHelperAsync(EFDbContext dbContext, T record)
        {
            return Task.FromResult(default(int));
        }

        //helper - check before editing records, overload - takes array of T record
        protected virtual async Task checkBeforeEditHelperAsync(EFDbContext dbContext, T[] records)
        {
            for (int i = 0; i < records.Length; i++)
            {
                await checkBeforeEditHelperAsync(dbContext, records[i]).ConfigureAwait(false);
            }
        }

        //helper - editing records, takes single T record
        protected virtual async Task<string> editHelperAsync(EFDbContext dbContext, T record)
        {
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

        //helper - editing records, overload - takes array of T record
        protected virtual async Task<List<string>> editHelperAsync(EFDbContext dbContext, T[] records)
        {
            var newEntryIds = new List<string>();

            for (int i = 0; i < records.Length; i++)
            {
                var newEntryId = await editHelperAsync(dbContext, records[i]).ConfigureAwait(false);
                if (!String.IsNullOrEmpty(newEntryId)) { newEntryIds.Add(newEntryId); }
            }
            return newEntryIds;
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //helper - check before deleting records, takes single T record
        //stub method for derived classes to implement
        protected virtual Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            return Task.FromResult(default(int));
        }
                
        //helper -  deleting db entries (setting isActive_bl to false) 
        protected virtual async Task<int> deleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            var dbEntries = await getEntriesFromContextHelperAsync<T>(dbContext, ids).ConfigureAwait(false);
            if (dbEntries.Count() != ids.Length) { throw new DbBadRequestException("Delete failed, entry(ies) not found"); }

            dbEntries.ForEach(x => x.IsActive_bl = false);
            
            return default(int);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //helper - Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedPropName' of entity T 
        //taking taking single T and TAddRem
        protected virtual async Task<int> addRemoveRelatedHelper<TAddRem>(T dbEntry, TAddRem dbEntryAddRem,
            string relatedCollectionName, bool isAdd) where TAddRem : IDbEntity
        {
            await Task.Run(() =>
            {
                var relatedCollection = (List<TAddRem>)typeof(T).GetProperty(relatedCollectionName).GetValue(dbEntry);

                if (isAdd && !relatedCollection.Contains(dbEntryAddRem)) { relatedCollection.Add(dbEntryAddRem); }
                if (!isAdd && relatedCollection.Contains(dbEntryAddRem)) { relatedCollection.Remove(dbEntryAddRem); }    
            }).ConfigureAwait(false);

            return default(int);
        }

        //Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedPropName' of entity T 
        //overload taking lists of T and TAddRem
        protected virtual async Task<int> addRemoveRelatedHelper<TAddRem>(List<T> dbEntries, List<TAddRem> dbEntriesAddRem,
            string relatedCollectionName, bool isAdd) where TAddRem : IDbEntity
        {
            foreach (var dbEntry in dbEntries)
            {
                foreach (var dbEntryAddRem in dbEntriesAddRem)
                {
                    await addRemoveRelatedHelper(dbEntry, dbEntryAddRem, relatedCollectionName, isAdd).ConfigureAwait(false);
                }
            }
            return default(int);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //helper -  getEntriesFromContextAsync
        protected virtual async Task<List<TOut>> getEntriesFromContextHelperAsync<TOut>(EFDbContext dbContext, string[] ids) 
            where TOut: IDbEntity
        {
            return await ((IQueryable<TOut>)dbContext.Set(typeof(TOut)).AsQueryable())
                .Where(x => ids.Contains(x.Id))
                .ToListAsync().ConfigureAwait(false);
        }

        //helper -  getEntriesFromContextAsync - overload with relatedCollectionName
        protected virtual async Task<List<TOut>> getEntriesFromContextHelperAsync<TOut>(EFDbContext dbContext, string[] ids,
            string relatedCollectionName) where TOut: IDbEntity
        {
            return await ((IQueryable<TOut>)dbContext.Set(typeof(TOut)).AsQueryable())
                .Where(x => ids.Contains(x.Id))
                .Include(relatedCollectionName)
                .ToListAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
