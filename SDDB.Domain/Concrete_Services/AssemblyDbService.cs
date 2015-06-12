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
    public class AssemblyDbService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public AssemblyDbService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<AssemblyDb>> GetAsync(string userId, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive)
                    .Include(x => x.AssemblyType).Include(x => x.AssemblyStatus).Include(x => x.AssemblyModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation).Include(x => x.AssignedToLocation.LocationType).Include(x => x.AssemblyExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                    foreach (var property in properties)
                    {
                        if (!property.GetMethod.IsVirtual) continue;
                        if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                        if (excludedProperties.Contains(property.Name)) continue;

                        if (property.GetValue(record) == null) property.SetValue(record, Activator.CreateInstance(property.PropertyType));
                    }
                }

                return records;
            }
        }

        //get by ids
        public virtual async Task<List<AssemblyDb>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive && ids.Contains(x.Id))
                    .Include(x => x.AssemblyType).Include(x => x.AssemblyStatus).Include(x => x.AssemblyModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation).Include(x => x.AssignedToLocation.LocationType).Include(x => x.AssemblyExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                    foreach (var property in properties)
                    {
                        if (!property.GetMethod.IsVirtual) continue;
                        if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                        if (excludedProperties.Contains(property.Name)) continue;

                        if (property.GetValue(record) == null) property.SetValue(record, Activator.CreateInstance(property.PropertyType));
                    }
                }

                return records;
            }
        }

        //get by projectIds 
        public virtual async Task<List<AssemblyDb>> GetByProjectAsync(string userId, string[] projectIds = null, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? await dbContext.Projects.Where(x => x.ProjectPersons.Select(y => y.Id).Contains(userId)).Select(x => x.Id)
                    .ToArrayAsync().ConfigureAwait(false);

                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                        && projectIds.Contains(x.AssignedToProject_Id))
                    .Include(x => x.AssemblyType).Include(x => x.AssemblyStatus).Include(x => x.AssemblyModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation).Include(x => x.AssignedToLocation.LocationType).Include(x => x.AssemblyExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                    foreach (var property in properties)
                    {
                        if (!property.GetMethod.IsVirtual) continue;
                        if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                        if (excludedProperties.Contains(property.Name)) continue;

                        if (property.GetValue(record) == null) property.SetValue(record, Activator.CreateInstance(property.PropertyType));
                    }
                }

                return records;
            }
        }

        //get by projectIds and modelIds
        public virtual async Task<List<AssemblyDb>> GetByModelAsync(string userId, string[] projectIds = null, string[] modelIds = null, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? await dbContext.Projects.Where(x => x.ProjectPersons.Select(y => y.Id).Contains(userId)).Select(x => x.Id)
                    .ToArrayAsync().ConfigureAwait(false);

                modelIds = modelIds ?? await dbContext.AssemblyModels.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                        && projectIds.Contains(x.AssignedToProject_Id) && modelIds.Contains(x.AssemblyModel_Id))
                    .Include(x => x.AssemblyType).Include(x => x.AssemblyStatus).Include(x => x.AssemblyModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation).Include(x => x.AssignedToLocation.LocationType).Include(x => x.AssemblyExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                    foreach (var property in properties)
                    {
                        if (!property.GetMethod.IsVirtual) continue;
                        if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                        if (excludedProperties.Contains(property.Name)) continue;

                        if (property.GetValue(record) == null) property.SetValue(record, Activator.CreateInstance(property.PropertyType));
                    }
                }

                return records;
            }
        }

        //get by projectIds, typeIds and locIds
        public virtual async Task<List<AssemblyDb>> GetByTypeLocAsync(string userId,
            string[] projectIds = null, string[] typeIds = null, string[] locIds = null, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? await dbContext.Projects.Where(x => x.ProjectPersons.Select(y => y.Id).Contains(userId)).Select(x => x.Id)
                    .ToArrayAsync().ConfigureAwait(false);

                typeIds = typeIds ?? await dbContext.AssemblyTypes.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

                List<AssemblyDb> records = null;
                if (locIds == null || locIds.Length == 0)
                {
                    records = await dbContext.AssemblyDbs
                        .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                            && projectIds.Contains(x.AssignedToProject_Id) && typeIds.Contains(x.AssemblyType_Id))
                        .Include(x => x.AssemblyType).Include(x => x.AssemblyStatus).Include(x => x.AssemblyModel).Include(x => x.AssignedToProject)
                        .Include(x => x.AssignedToLocation).Include(x => x.AssignedToLocation.LocationType).Include(x => x.AssemblyExt)
                        .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    records = await dbContext.AssemblyDbs
                        .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                            && projectIds.Contains(x.AssignedToProject_Id) && typeIds.Contains(x.AssemblyType_Id) && locIds.Contains(x.AssignedToLocation_Id))
                        .Include(x => x.AssemblyType).Include(x => x.AssemblyStatus).Include(x => x.AssemblyModel).Include(x => x.AssignedToProject)
                        .Include(x => x.AssignedToLocation).Include(x => x.AssignedToLocation.LocationType).Include(x => x.AssemblyExt)
                        .ToListAsync().ConfigureAwait(false);
                }

                

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                    foreach (var property in properties)
                    {
                        if (!property.GetMethod.IsVirtual) continue;
                        if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                        if (excludedProperties.Contains(property.Name)) continue;

                        if (property.GetValue(record) == null) property.SetValue(record, Activator.CreateInstance(property.PropertyType));
                    }
                }

                return records;
            }
        }
        
        //find by query
        public virtual Task<List<AssemblyDb>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive &&
                    (x.AssyName.Contains(query) || x.AssyAltName.Contains(query) || x.AssyAltName2.Contains(query)))
                    .Include(x => x.AssignedToProject).ToListAsync();
            }
        }

        //find by query and project
        public virtual async Task<List<AssemblyDb>> LookupByProjAsync(string userId, string[] projectIds = null, string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                if (projectIds == null || projectIds.Length == 0) projectIds = await dbContext.Projects.Where(x => x.ProjectPersons
                    .Select(y => y.Id).Contains(userId)).Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);
                
                var records = await dbContext.AssemblyDbs.Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) &&
                    x.IsActive == getActive && projectIds.Contains(x.AssignedToProject_Id) &&
                    (x.AssyName.Contains(query) || x.AssyAltName.Contains(query) || x.AssyAltName2.Contains(query)))
                    .Include(x => x.AssignedToProject).ToListAsync();
                
                return records;
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(AssemblyDb[] records)
        {
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var newEntries = 1;  foreach (var record in records)
                    {
                        var dbEntry = await dbContext.AssemblyDbs.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();

                            if (newEntries > 1)
                            {
                                var excludedProperties = new string[] { "Id", "TSP" };
                                var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                                foreach (var property in properties)
                                {
                                    if (property.GetCustomAttributes(typeof(DBIsUniqueAttribute), false).FirstOrDefault() == null) continue;
                                    property.SetValue(record, property.GetValue(record) + String.Format("_{0:HHmm}_{1:D3}",DateTime.Now,newEntries));
                                }
                            }
                            dbContext.AssemblyDbs.Add(record); newEntries++;
                        }
                        else
                        {
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                            foreach (var property in properties)
                            {
                                if (property.GetMethod.IsVirtual) continue;
                                if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                                if (excludedProperties.Contains(property.Name)) continue;

                                if (record.PropIsModified(property.Name)) property.SetValue(dbEntry, property.GetValue(record));
                            }
                        }
                    }
                    for (int i = 1; i <= 10; i++)
                    {
                        try
                        {
                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                            break;
                        }
                        catch (Exception e)
                        {
                            var message = e.GetBaseException().Message;
                            if (i == 10 || !message.Contains("Deadlock found when trying to get lock"))
                            {
                                return new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = message };
                            }
                        }
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                    trans.Complete();
                }
            }
            return new DBResult { StatusCode = HttpStatusCode.OK };
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
                        var dbEntry = await dbContext.AssemblyDbs.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            //tasks prior to desactivating: check if components assigned to assembly
                            if ((await dbContext.Components.Where(x => x.IsActive == true && x.AssignedToAssemblyDb_Id == id).CountAsync().ConfigureAwait(false)) > 0)
                                errorMessage += string.Format("Assembly {0} not deleted, it has components assigned to it\n", dbEntry.AssyName);
                            else
                            {
                                dbEntry.IsActive = false;
                            }
                        }
                        else
                        {
                            errorMessage += string.Format("Record with Id={0} not found\n", id);
                        }
                    }
                    for (int i = 1; i <= 10; i++)
                    {
                        try
                        {
                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                            break;
                        }
                        catch (Exception e)
                        {
                            var message = e.GetBaseException().Message;
                            if (i == 10 || !message.Contains("Deadlock found when trying to get lock"))
                            {
                                errorMessage += string.Format("Error Saving changes: {0}\n", message);
                                break;
                            }
                        }
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                    trans.Complete();
                }
            }
            if (errorMessage == "") return new DBResult();
            else return new DBResult {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors deleting records:\n" + errorMessage
            };
        }

        //-----------------------------------------------------------------------------------------------------------------------
        
        // Create and Update records given in [] - AssemblyExt
        public virtual async Task<DBResult> EditExtAsync(AssemblyExt[] records)
        {
            var errorMessage = "";
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.AssemblyExts.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            if (await dbContext.AssemblyDbs.FindAsync(record.Id).ConfigureAwait(false) == null)
                                errorMessage += String.Format("Assembly with id={0} not found.\n", record.Id);
                            else
                                dbContext.AssemblyExts.Add(record);
                        }
                        else
                        {
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(AssemblyExt).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                            foreach (var property in properties)
                            {
                                if (property.GetMethod.IsVirtual) continue;
                                if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                                if (excludedProperties.Contains(property.Name)) continue;

                                if (record.PropIsModified(property.Name)) property.SetValue(dbEntry, property.GetValue(record));
                            }
                        }
                    }
                    for (int i = 1; i <= 10; i++)
                    {
                        try
                        {
                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                            break;
                        }
                        catch (Exception e)
                        {
                            var message = e.GetBaseException().Message;
                            if (i == 10 || !message.Contains("Deadlock found when trying to get lock"))
                            {
                                errorMessage += String.Format("Error saving records: {0}\n", message);
                                break;
                            }
                        }
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                    trans.Complete();
                }
            }
            if (errorMessage == "") return new DBResult();
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors deleting records:\n" + errorMessage
            };
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
