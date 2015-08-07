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
    public class ComponentService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public ComponentService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<Component>> GetAsync(string userId, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive)
                    .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by ids
        public virtual async Task<List<Component>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive && ids.Contains(x.Id))
                    .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }
                
        //get by projectIds and modelIds
        public virtual async Task<List<Component>> GetByAltIdsAsync(string userId, string[] projectIds = null, string[] modelIds = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { }; modelIds = modelIds ?? new string[] { }; 

                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (modelIds.Count() == 0 || modelIds.Contains(x.ComponentModel_Id)))
                    .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by projectIds, typeIds and assyIds
        public virtual async Task<List<Component>> GetByAltIdsAsync(string userId,
            string[] projectIds = null, string[] typeIds = null, string[] assyIds = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { }; typeIds = typeIds ?? new string[] { }; assyIds = assyIds ?? new string[] { };

                var records = await dbContext.Components
                   .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (typeIds.Count() == 0 || typeIds.Contains(x.ComponentType_Id)) &&
                        (assyIds.Count() == 0 || assyIds.Contains(x.AssignedToAssemblyDb_Id)))
                   .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                   .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                   .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }
                        
        //lookup by query
        public virtual Task<List<Component>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (x.CompName.Contains(query) || x.CompAltName.Contains(query) || x.CompAltName2.Contains(query)))
                    .ToListAsync();
            }
        }

        //lookup by query and project
        public virtual async Task<List<Component>> LookupByProjAsync(string userId, string[] projectIds = null, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { };

                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (x.CompName.Contains(query) || x.CompAltName.Contains(query) || x.CompAltName2.Contains(query)))
                    .ToListAsync();
                
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(string userId, Component[] records)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            var errorMessage = "";
            var newEntries = 1;
            var loggedProperties = new string[] { "ComponentStatus_Id", "AssignedToProject_Id", "AssignedToAssemblyDb_Id", "LastCalibrationDate" }; 

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.Components.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();

                            if (newEntries > 1)
                            {
                                var excludedProperties = new string[] { "Id", "TSP" };
                                var properties = typeof(Component).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                                foreach (var property in properties)
                                {
                                    if (property.GetCustomAttributes(typeof(DBIsUniqueAttribute), false).FirstOrDefault() == null) continue;
                                    property.SetValue(record, property.GetValue(record) + String.Format("_{0:D3}",newEntries));
                                }
                            }
                            dbContext.Components.Add(record); newEntries++;
                        }
                        else
                        {
                            dbEntry.CopyModifiedProps(record);
                            if (record.LoggedPropsModified(loggedProperties)) { addLogEntry(dbContext, dbEntry, userId); }
                        }
                    }
                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
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
                        var dbEntry = await dbContext.Components.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            dbEntry.IsActive_bl = false;
                        }
                        else
                        {
                            errorMessage += string.Format("Record with Id={0} not found\n", id);
                        }
                    }
                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
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

        //-----------------------------------------------------------------------------------------------------------------------
        
        // Create and Update records given in [] - ComponentExt
        public virtual async Task<DBResult> EditExtAsync(ComponentExt[] records)
        {
            var errorMessage = "";
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.ComponentExts.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            if (await dbContext.Components.FindAsync(record.Id).ConfigureAwait(false) == null)
                                errorMessage += String.Format("Component with id={0} not found.\n", record.Id);
                            else
                                dbContext.ComponentExts.Add(record);
                        }
                        else
                        {
                            dbEntry.CopyModifiedProps(record);
                        }
                    }
                    await dbContext.SaveChangesWithRetryAsync().ConfigureAwait(false);
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


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //adds log entry to the dbContext based on assembly data and user Id
        private void addLogEntry(EFDbContext dbContext, Component dbEntry, string userId)
        {
            dbContext.ComponentLogEntrys.Add(
                new ComponentLogEntry
                {
                    Id = Guid.NewGuid().ToString(),
                    Component_Id = dbEntry.Id,
                    LastSavedByPerson_Id = userId,
                    LogEntryDateTime = DateTime.Now,
                    ComponentStatus_Id = dbEntry.ComponentStatus_Id,
                    AssignedToProject_Id = dbEntry.AssignedToProject_Id,
                    AssignedToAssemblyDb_Id = dbEntry.AssignedToAssemblyDb_Id,
                    LastCalibrationDate = dbEntry.LastCalibrationDate,
                    Comments = dbEntry.Comments,
                    IsActive_bl = true
                }
            );
        }


        #endregion
    }
}
