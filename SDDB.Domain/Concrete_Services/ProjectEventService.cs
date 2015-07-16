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
    public class ProjectEventService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public ProjectEventService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<ProjectEvent>> GetAsync(string userId, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive)
                    .Include( x => x.AssignedToProject).Include( x => x.CreatedByPerson).Include( x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by ids
        public virtual async Task<List<ProjectEvent>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive && ids.Contains(x.Id))
                    .Include(x => x.AssignedToProject).Include(x => x.CreatedByPerson).Include(x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by projectIds 
        public virtual async Task<List<ProjectEvent>> GetByProjectAsync(string userId, string[] projectIds = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                projectIds = projectIds ?? new string[] { };
                
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) )
                    .Include(x => x.AssignedToProject).Include(x => x.CreatedByPerson).Include(x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }
                        
        //lookup by query
        public virtual Task<List<ProjectEvent>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                    (x.EventName.Contains(query) || x.EventAltName.Contains(query))).ToListAsync();
            }
        }

        //lookup by query and project
        public virtual async Task<List<ProjectEvent>> LookupByProjAsync(string userId, string[] projectIds = null, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                projectIds = projectIds ?? new string[] { };

                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (x.EventName.Contains(query) || x.EventAltName.Contains(query))).ToListAsync();

                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(ProjectEvent[] records)
        {
            var errorMessage = ""; 

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.ProjectEvents.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();
                            dbContext.ProjectEvents.Add(record);
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
                        var dbEntry = await dbContext.ProjectEvents.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            if (String.IsNullOrEmpty(dbEntry.ClosedByPerson_Id) || dbEntry.EventClosed == null)
                                errorMessage += string.Format("Event {0} not deleted. Please close with date and person before deleting\n", dbEntry.EventName);
                            else
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
