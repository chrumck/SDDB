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
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive)
                    .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Component).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual async Task<List<Component>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive && ids.Contains(x.Id))
                    .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Component).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual async Task<List<Component>> GetByProjectAsync(string userId, string[] projectIds = null, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? await dbContext.Projects.Where(x => x.ProjectPersons.Select(y => y.Id).Contains(userId)).Select(x => x.Id)
                    .ToArrayAsync().ConfigureAwait(false);

                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                        && projectIds.Contains(x.AssignedToProject_Id))
                    .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Component).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual async Task<List<Component>> GetByModelAsync(string userId, string[] projectIds = null, string[] modelIds = null, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? await dbContext.Projects.Where(x => x.ProjectPersons.Select(y => y.Id).Contains(userId)).Select(x => x.Id)
                    .ToArrayAsync().ConfigureAwait(false);

                modelIds = modelIds ?? await dbContext.ComponentModels.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

                var records = await dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                        && projectIds.Contains(x.AssignedToProject_Id) && modelIds.Contains(x.ComponentModel_Id))
                    .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Component).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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

        //get by projectIds, typeIds and assyIds
        public virtual async Task<List<Component>> GetByTypeAssyAsync(string userId,
            string[] projectIds = null, string[] typeIds = null, string[] assyIds = null, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? await dbContext.Projects.Where(x => x.ProjectPersons.Select(y => y.Id).Contains(userId)).Select(x => x.Id)
                    .ToArrayAsync().ConfigureAwait(false);

                typeIds = typeIds ?? await dbContext.ComponentTypes.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

                List<Component> records = null;
                if (assyIds == null || assyIds.Length == 0)
                {
                    records = await dbContext.Components
                        .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                            && projectIds.Contains(x.AssignedToProject_Id) && typeIds.Contains(x.ComponentType_Id))
                        .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                        .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                        .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    records = await dbContext.Components
                        .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                            && projectIds.Contains(x.AssignedToProject_Id) && typeIds.Contains(x.ComponentType_Id) && assyIds.Contains(x.AssignedToAssemblyDb_Id))
                        .Include(x => x.ComponentType).Include(x => x.ComponentStatus).Include(x => x.ComponentModel).Include(x => x.AssignedToProject)
                        .Include(x => x.AssignedToAssemblyDb).Include(x => x.ComponentExt)
                        .ToListAsync().ConfigureAwait(false);
                }

                

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(Component).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual Task<List<Component>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Components
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive &&
                    (x.CompName.Contains(query) || x.CompAltName.Contains(query) || x.CompAltName2.Contains(query))).ToListAsync();
            }
        }

        //find by query and project
        public virtual async Task<List<Component>> LookupByProjAsync(string userId, string[] projectIds = null, string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                if (projectIds == null || projectIds.Length == 0) projectIds = await dbContext.Projects.Where(x => x.ProjectPersons
                    .Select(y => y.Id).Contains(userId)).Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

                var records =  await dbContext.Components.Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) &&
                    x.IsActive == getActive && projectIds.Contains(x.AssignedToProject_Id) &&
                    (x.CompName.Contains(query) || x.CompAltName.Contains(query) || x.CompAltName2.Contains(query))).ToListAsync();
                
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(Component[] records)
        {
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var newEntries = 1;  foreach (var record in records)
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
                                    property.SetValue(record, property.GetValue(record) + String.Format("_{0:HHmm}_{1:D3}",DateTime.Now,newEntries));
                                }
                            }
                            dbContext.Components.Add(record); newEntries++;
                        }
                        else
                        {
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(Component).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
                        var dbEntry = await dbContext.Components.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            dbEntry.IsActive = false;
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
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(ComponentExt).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
