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
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive)
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
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive && ids.Contains(x.Id))
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
        public virtual async Task<List<AssemblyDb>> GetByAltIdsAsync(string userId, string[] projectIds = null, string[] modelIds = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { }; modelIds = modelIds ?? new string[] { };

                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (modelIds.Count() == 0 || modelIds.Contains(x.AssemblyModel_Id)))
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
        public virtual async Task<List<AssemblyDb>> GetByAltIdsAsync(string userId,
            string[] projectIds = null, string[] typeIds = null, string[] locIds = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { }; typeIds = typeIds ?? new string[] { }; locIds = locIds ?? new string[] { };

                var records = await dbContext.AssemblyDbs
                        .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                            (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                            (typeIds.Count() == 0 || typeIds.Contains(x.AssemblyType_Id)) &&
                            (locIds.Count() == 0 || locIds.Contains(x.AssignedToLocation_Id)))
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

        //lookup by query
        public virtual Task<List<AssemblyDb>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (x.AssyName.Contains(query) || x.AssyAltName.Contains(query) || x.AssyAltName2.Contains(query) ||
                        x.AssignedToProject.ProjectCode.Contains(query)))
                    .Include(x => x.AssignedToProject).ToListAsync();
            }
        }

        //lookup by query and project
        public virtual async Task<List<AssemblyDb>> LookupByProjAsync(string userId, string[] projectIds = null, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { };

                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (x.AssyName.Contains(query) || x.AssyAltName.Contains(query) || x.AssyAltName2.Contains(query)))
                    .Include(x => x.AssignedToProject).ToListAsync();

                return records;
            }
        }

        //lookup by location
        public virtual async Task<List<AssemblyDb>> LookupByLocAsync(string userId, string locId = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                locId = locId ?? "";

                var records = await dbContext.AssemblyDbs
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                        (locId == "" || x.AssignedToLocation_Id == locId))
                    .Include(x => x.AssignedToProject).ToListAsync();

                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(string userId, AssemblyDb[] records)
        {
            if (string.IsNullOrEmpty(userId))
                return new DBResult { StatusCode = HttpStatusCode.BadRequest, StatusDescription = "arguments missing" };

            var errorMessage = "";
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var newEntries = 1; foreach (var record in records)
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
                                    property.SetValue(record, property.GetValue(record) + String.Format("_{0:D3}", newEntries));
                                }
                            }
                            dbContext.AssemblyDbs.Add(record); newEntries++;
                        }
                        else
                        {
                            var loggedProperties = new string[] { "AssemblyStatus_Id", "AssignedToProject_Id", "AssignedToLocation_Id",
                                "AssyGlobalX", "AssyGlobalY", "AssyGlobalZ", "AssyLocalXDesign", "AssyLocalYDesign", "AssyLocalZDesign",
                                "AssyLocalXAsBuilt", "AssyLocalYAsBuilt", "AssyLocalZAsBuilt", "AssyStationing", "AssyLength"};
                            var logChanges = false;
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(AssemblyDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                            foreach (var property in properties)
                            {
                                if (property.GetMethod.IsVirtual) continue;
                                if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                                if (excludedProperties.Contains(property.Name)) continue;

                                if (record.PropIsModified(property.Name)) property.SetValue(dbEntry, property.GetValue(record));

                                if (record.PropIsModified(property.Name) && loggedProperties.Contains(property.Name)) logChanges = true;
                            }
                            if (logChanges) addLogEntry(dbContext, dbEntry, userId);
                        }
                    }
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);

                    trans.Complete();
                }
            }
            if (errorMessage == "") return new DBResult();
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage
            };
        }

        // change status given as statusId of records given as recordIds[]
        public virtual async Task<DBResult> EditStatusAsync(string userId, string[] ids, string statusId)
        {
            if (ids == null || ids.Length == 0 || string.IsNullOrEmpty(statusId) || string.IsNullOrEmpty(userId))
                return new DBResult { StatusCode = HttpStatusCode.BadRequest, StatusDescription = "arguments missing" };

            var errorMessage = "";
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var recordId in ids)
                    {
                        var dbEntry = await dbContext.AssemblyDbs.FindAsync(recordId).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            if (dbEntry.AssemblyStatus_Id != statusId)
                            {
                                dbEntry.AssemblyStatus_Id = statusId;
                                addLogEntry(dbContext, dbEntry, userId);
                            }
                        }
                        else errorMessage += string.Format("Record with id:{0} not found\n", recordId);
                    }
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);

                    trans.Complete();
                }
            }
            if (errorMessage == "") return new DBResult();
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage
            };
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
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);

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
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);

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

        //adds log entry to the dbContext based on assembly data and user Id
        private void addLogEntry(EFDbContext dbContext, AssemblyDb assy, string userId)
        {
            dbContext.AssemblyLogEntrys.Add(
                new AssemblyLogEntry
                {
                    Id = Guid.NewGuid().ToString(),
                    LogEntryDateTime = DateTime.Now,
                    AssemblyDb_Id = assy.Id,
                    EnteredByPerson_Id = userId,
                    AssemblyStatus_Id = assy.AssemblyStatus_Id,
                    AssignedToProject_Id = assy.AssignedToProject_Id,
                    AssignedToLocation_Id = assy.AssignedToLocation_Id,
                    AssyGlobalX = assy.AssyGlobalX,
                    AssyGlobalY = assy.AssyGlobalY,
                    AssyGlobalZ = assy.AssyGlobalZ,
                    AssyLocalXDesign = assy.AssyLocalXDesign,
                    AssyLocalYDesign = assy.AssyLocalYDesign,
                    AssyLocalZDesign = assy.AssyLocalZDesign,
                    AssyLocalXAsBuilt = assy.AssyLocalXAsBuilt,
                    AssyLocalYAsBuilt = assy.AssyLocalYAsBuilt,
                    AssyLocalZAsBuilt = assy.AssyLocalZAsBuilt,
                    AssyStationing = assy.AssyStationing,
                    AssyLength = assy.AssyLength,
                    Comments = assy.Comments,
                    IsActive_bl = true
                }
            );
        }

        #endregion
    }
}
