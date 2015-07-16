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
    public class DocumentService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DocumentService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<Document>> GetAsync(string userId, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.Documents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive)
                    .Include(x => x.DocumentType).Include(x => x.AuthorPerson).Include(x => x.ReviewerPerson).Include(x => x.AssignedToProject)
                    .Include(x => x.RelatesToAssyType).Include(x => x.RelatesToCompType)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by ids
        public virtual async Task<List<Document>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.Documents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive && ids.Contains(x.Id))
                    .Include(x => x.DocumentType).Include(x => x.AuthorPerson).Include(x => x.ReviewerPerson).Include(x => x.AssignedToProject)
                    .Include(x => x.RelatesToAssyType).Include(x => x.RelatesToCompType)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by projectIds and typeIds
        public virtual async Task<List<Document>> GetByTypeAsync(string userId, string[] projectIds = null, string[] typeIds = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { }; typeIds = typeIds ?? new string[] { };

                var records = await dbContext.Documents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (typeIds.Count() == 0 || typeIds.Contains(x.DocumentType_Id)))
                    .Include(x => x.DocumentType).Include(x => x.AuthorPerson).Include(x => x.ReviewerPerson).Include(x => x.AssignedToProject)
                    .Include(x => x.RelatesToAssyType).Include(x => x.RelatesToCompType)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }
        
        //lookup by query
        public virtual Task<List<Document>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Documents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (x.DocName.Contains(query) || x.DocAltName.Contains(query) || x.DocFilePath.Contains(query)))
                    .ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(Document[] records)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.Documents.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();
                            dbContext.Documents.Add(record);
                        }
                        else
                        {
                            dbEntry.CopyModifiedProps(record);
                        }
                    }
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);
                    trans.Complete();
                }
            }
            if (errorMessage == "") { return new DBResult(); }
            else
            {
                return new DBResult
                {
                    StatusCode = HttpStatusCode.Conflict,
                    StatusDescription = "Errors editing records:\n" + errorMessage
                };
            }
        }

        // Delete records by their Ids
        public virtual async Task<DBResult> DeleteAsync(string[] ids)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var id in ids)
                    {
                        var dbEntry = await dbContext.Documents.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            dbEntry.IsActive = false;
                        }
                        else
                        {
                            errorMessage += string.Format("Record with Id={0} not found\n", id);
                        }
                    }
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);
                    trans.Complete();
                }
            }
            if (errorMessage == "") { return new DBResult(); }
            else
            {
                return new DBResult
                {
                    StatusCode = HttpStatusCode.Conflict,
                    StatusDescription = "Errors deleting records:\n" + errorMessage
                };
            }
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
