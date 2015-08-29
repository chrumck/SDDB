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
    public class DocumentService : BaseDbService<Document>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DocumentService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }
        

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get by ids
        public virtual async Task<List<Document>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.Documents
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.DocumentType)
                    .Include(x => x.AuthorPerson)
                    .Include(x => x.ReviewerPerson)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.RelatesToAssyType)
                    .Include(x => x.RelatesToCompType)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by projectIds and typeIds
        public virtual async Task<List<Document>> GetByAltIdsAsync(string[] projectIds, string[] typeIds, bool getActive = true)
        {
            projectIds = projectIds ?? new string[] { };
            typeIds = typeIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.Documents
                    .Where(x => 
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (typeIds.Count() == 0 || typeIds.Contains(x.DocumentType_Id)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.DocumentType)
                    .Include(x => x.AuthorPerson)
                    .Include(x => x.ReviewerPerson)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.RelatesToAssyType)
                    .Include(x => x.RelatesToCompType)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }
        
        //lookup by query
        public virtual async Task<List<Document>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Documents
                    .Where(x =>
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        (x.DocName.Contains(query) || x.AssignedToProject.ProjectName.Contains(query)) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.AssignedToProject)
                    .Take(maxRecordsFromLookup)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

                

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
