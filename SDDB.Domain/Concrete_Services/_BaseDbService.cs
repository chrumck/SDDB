using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNet.Identity.EntityFramework;
using Mehdime.Entity;

using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;
using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;

namespace SDDB.Domain.Services
{
    public abstract class BaseDbService<T> where T: class, IDbEntity
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        protected IDbContextScopeFactory contextScopeFac;
        protected string userId;
        protected List<IdentityRole> userRoles;
        protected const int maxRecordsFromLookup = 100;

        protected IReadOnlyList<string> loggedProperties = new List<string> { };

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

        // Create and Update extended table records given in [] (meant at this point for AssemblyExt and ComponentExt)
        public virtual async Task EditExtendedAsync<TExtended>(TExtended[] records) where TExtended: class, IDbEntity
        {
            if (records == null || records.Length == 0) { throw new ArgumentNullException("records"); }

            await dbScopeHelperAsync(async dbContext =>
            {
                await editExtendedHelperAsync(dbContext, records).ConfigureAwait(false);
                return default(int);
            })
            .ConfigureAwait(false);
        }


        // Delete records by their Ids
        public virtual async Task DeleteAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            await dbScopeHelperAsync(async dbContext =>
            {
                await checkBeforeDeleteHelperAsync(dbContext, ids).ConfigureAwait(false);
                await deleteHelperAsync(dbContext, ids).ConfigureAwait(false);
                return default(int);
            })
            .ConfigureAwait(false);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedCollectionName' of entity T 
        public virtual async Task AddRemoveRelated<TAddRem> (string[] ids, string[] idsAddRem, 
            string relatedCollectionName, bool isAdd = true) where TAddRem: class, IDbEntity
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }
            if (idsAddRem == null || idsAddRem.Length == 0) { throw new ArgumentNullException("idsAddRem"); }
            if (String.IsNullOrEmpty(relatedCollectionName)) { throw new ArgumentNullException("relatedCollectionName"); }

            await dbScopeHelperAsync(async dbContext => 
            {
                await checkBeforeAddRemoveHelperAsync<TAddRem>(dbContext, ids, idsAddRem, isAdd);

                var dbEntries = await getEntriesFromContextHelperAsync<T>(dbContext, ids, relatedCollectionName)
                    .ConfigureAwait(false);

                var dbEntriesAddRem = await getEntriesFromContextHelperAsync<TAddRem>(dbContext, idsAddRem)
                    .ConfigureAwait(false);
                
                await addRemoveRelatedHelper(dbEntries, dbEntriesAddRem, relatedCollectionName, isAdd).ConfigureAwait(false);

                return default(int);
            })
            .ConfigureAwait(false);
        }

        //Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedCollectionName' of entity T
        //overload taking lamdba expression
        public virtual async Task AddRemoveRelated<TAddRem>(string[] ids, string[] idsAddRem,
            Expression<Func<T, ICollection<TAddRem>>> lambda, bool isAdd = true) where TAddRem : class, IDbEntity
        {
            string relatedCollectionName = getRelatedCollectionNameFromLambdaHelper<TAddRem>(lambda);
            await AddRemoveRelated<TAddRem>(ids, idsAddRem, relatedCollectionName, isAdd).ConfigureAwait(false);
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
            var dbEntry = await dbContext.Set<T>().FindAsync(record.Id).ConfigureAwait(false);
            if (dbEntry == null)
            {
                record.Id = Guid.NewGuid().ToString();
                record.LastSavedByPerson_Id = userId;
                dbContext.Set<T>().Add(record);
                addLogEntryHelper(dbContext, record);
                return record.Id;
            }
            dbEntry.CopyModifiedProps(record);
            dbEntry.LastSavedByPerson_Id = userId;
            if (record.LoggedPropsModified(loggedProperties.ToArray())) { addLogEntryHelper(dbContext, dbEntry); }
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

        //helper - check before deleting records, takes T ids array
        //stub method for derived classes to implement
        protected virtual Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            return Task.FromResult(default(int));
        }
                
        //helper -  deleting db entries (setting isActive_bl to false) 
        protected virtual async Task deleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            var dbEntries = await getEntriesFromContextHelperAsync<T>(dbContext, ids).ConfigureAwait(false);
            dbEntries.ForEach(x => x.IsActive_bl = false);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //checkBeforeAddRemoveHelperAsync
        //stub method for derived classes to implement
        protected virtual Task checkBeforeAddRemoveHelperAsync<TAddRem>(EFDbContext dbContext,
            string[] ids, string[] idsAddRem, bool isAdd)
        {
            return Task.FromResult(default(int));
        }

        //helper - Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedPropName' of entity T 
        //taking taking single T and TAddRem
        protected virtual Task<int> addRemoveRelatedHelper<TAddRem>(T dbEntry, TAddRem dbEntryAddRem,
            string relatedCollectionName, bool isAdd) where TAddRem : class, IDbEntity
        {
            var relatedCollection = (ICollection<TAddRem>)typeof(T).GetProperty(relatedCollectionName).GetValue(dbEntry);

            if (isAdd && !relatedCollection.Contains(dbEntryAddRem)) { relatedCollection.Add(dbEntryAddRem); }
            if (!isAdd && relatedCollection.Contains(dbEntryAddRem)) { relatedCollection.Remove(dbEntryAddRem); }    

            return Task.FromResult(default(int));
        }

        //Add (or Remove  when set isAdd to false) related entities TAddRem to collection 'relatedPropName' of entity T 
        //overload taking lists of T and TAddRem
        protected virtual async Task<int> addRemoveRelatedHelper<TAddRem>(List<T> dbEntries, List<TAddRem> dbEntriesAddRem,
            string relatedCollectionName, bool isAdd) where TAddRem : class, IDbEntity
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

        //editExtHelper - takes single record and adds or updates the dbEntry
        protected virtual async Task editExtendedHelperAsync<TExtended>(EFDbContext dbContext, TExtended record) 
            where TExtended : class, IDbEntity
        {
            var dbEntry = await dbContext.Set<TExtended>().FindAsync(record.Id).ConfigureAwait(false);
            if (dbEntry == null)
            {
                if (await dbContext.Set<T>().FindAsync(record.Id).ConfigureAwait(false) == null)
                {
                    throw new DbBadRequestException(
                        String.Format("Entity {0} with id={1} not found.\n", typeof(TExtended).Name ,record.Id));
                }
                record.LastSavedByPerson_Id = userId;
                dbContext.Set<TExtended>().Add(record);
            }
            else
            {
                dbEntry.CopyModifiedProps(record);
                dbEntry.LastSavedByPerson_Id = userId;
            }
        }

        //editExtHelper - overload for array of T records
        protected virtual async Task editExtendedHelperAsync<TExtended>(EFDbContext dbContext, TExtended[] records) 
            where TExtended : class, IDbEntity
        {
            for (int i = 0; i < records.Length; i++)
            {
                await editExtendedHelperAsync<TExtended>(dbContext, records[i]).ConfigureAwait(false);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //helper -  getEntriesFromContextAsync
        protected virtual async Task<List<TOut>> getEntriesFromContextHelperAsync<TOut>(EFDbContext dbContext, string[] ids) 
            where TOut: class, IDbEntity
        {
            var dbEntries = await dbContext.Set<TOut>()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync<TOut>().ConfigureAwait(false);
            
            if (dbEntries.Count != ids.Length) { throw new DbBadRequestException("Entry(ies) not found"); }

            return dbEntries;
        }

        //helper -  getEntriesFromContextAsync
        // overload with relatedCollectionName
        protected virtual async Task<List<TOut>> getEntriesFromContextHelperAsync<TOut>(EFDbContext dbContext, string[] ids,
            string relatedCollectionName) where TOut : class, IDbEntity
        {
            var dbEntries = await dbContext.Set<TOut>().Where(x => ids.Contains(x.Id))
                .Include(relatedCollectionName)
                .ToListAsync<TOut>().ConfigureAwait(false);

            if (dbEntries.Count != ids.Length)
            {
                throw new DbBadRequestException( String.Format("Entry(ies) in collection {0} not found", relatedCollectionName));
            }
            return dbEntries;

        }
        
        //helper - gets relatedCollectionName from expression
        protected string getRelatedCollectionNameFromLambdaHelper<TOut>(Expression<Func<T, ICollection<TOut>>> lambda) 
            where TOut: class, IDbEntity
        {
            var body = (MemberExpression)lambda.Body;
            if (body == null)
            {
                throw new ArgumentException(
                    string.Format("Expression '{0}' refers to a method, not a property.", lambda.ToString()));
            }
            return body.Member.Name;
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //adds log entry to the dbContext based on record data and user Id
        protected virtual void addLogEntryHelper(EFDbContext dbContext, T dbEntry)
        {
            //stub method for derived classes to implement
        }



        #endregion
    }
}
