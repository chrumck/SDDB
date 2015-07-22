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
    public class LocationService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public LocationService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<Location>> GetAsync(string userId, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Locations
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive)
                    .Include(x => x.LocationType).Include(x => x.AssignedToProject).Include(x => x.ContactPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by ids
        public virtual async Task<List<Location>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Locations
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive && ids.Contains(x.Id))
                    .Include(x => x.LocationType).Include(x => x.AssignedToProject).Include(x => x.ContactPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }

        //get by projectIds and TypeIds
        public virtual async Task<List<Location>> GetByAltIdsAsync(string userId, string[] projectIds = null, string[] typeIds = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { }; typeIds = typeIds ?? new string[] { };

                var records = await dbContext.Locations
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (typeIds.Count() == 0 || typeIds.Contains(x.LocationType_Id)))
                    .Include(x => x.LocationType).Include(x => x.AssignedToProject).Include(x => x.ContactPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records;
            }
        }
        
        //lookup by query
        public virtual Task<List<Location>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Locations
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (x.LocName.Contains(query) || x.LocAltName.Contains(query) || x.AssignedToProject.ProjectCode.Contains(query)))
                    .Include(x => x.AssignedToProject).ToListAsync();
            }
        }

        //lookup by query and project
        public virtual async Task<List<Location>> LookupByProjAsync(string userId, string[] projectIds = null, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { };

                var records = await dbContext.Locations
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (x.LocName.Contains(query) || x.LocAltName.Contains(query) || x.AssignedToProject.ProjectCode.Contains(query)))
                    .Include(x => x.AssignedToProject).ToListAsync();

                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(Location[] records)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var newEntries = 1;  foreach (var record in records)
                    {
                        var dbEntry = await dbContext.Locations.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();

                            if (newEntries > 1)
                            {
                                var excludedProperties = new string[] { "Id", "TSP" };
                                var properties = typeof(Location).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                                foreach (var property in properties)
                                {
                                    if (property.GetCustomAttributes(typeof(DBIsUniqueAttribute), false).FirstOrDefault() == null) continue;
                                    property.SetValue(record, property.GetValue(record) + String.Format("_{0:D3}", newEntries));
                                }
                            }
                            dbContext.Locations.Add(record); newEntries++;
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
                        var dbEntry = await dbContext.Locations.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            //tasks prior to desactivating: check if assemblies assigned to location
                            if ((await dbContext.AssemblyDbs.Where(x => x.IsActive_bl == true && x.AssignedToLocation_Id == id).CountAsync().ConfigureAwait(false)) > 0)
                                errorMessage += string.Format("Location {0} not deleted, it has assemblies assigned to it\n", dbEntry.LocName);
                            else
                            {
                            dbEntry.IsActive_bl = false;
                            }
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


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
