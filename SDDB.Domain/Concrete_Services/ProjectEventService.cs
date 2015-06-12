﻿using System;
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
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive)
                    .Include( x => x.AssignedToProject).Include( x => x.CreatedByPerson).Include( x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(ProjectEvent).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual async Task<List<ProjectEvent>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive && ids.Contains(x.Id))
                    .Include(x => x.AssignedToProject).Include(x => x.CreatedByPerson).Include(x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(ProjectEvent).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual async Task<List<ProjectEvent>> GetByProjectAsync(string userId, string[] projectIds = null, bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? await dbContext.Projects.Where(x => x.ProjectPersons.Select(y => y.Id).Contains(userId)).Select(x => x.Id)
                    .ToArrayAsync().ConfigureAwait(false);

                var records = await dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive
                        && projectIds.Contains(x.AssignedToProject_Id))
                    .Include(x => x.AssignedToProject).Include(x => x.CreatedByPerson).Include(x => x.ClosedByPerson)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(ProjectEvent).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual Task<List<ProjectEvent>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.ProjectEvents
                    .Where(x => x.AssignedToProject.ProjectPersons.Select(y => y.Id).Contains(userId) && x.IsActive == getActive &&
                    (x.EventName.Contains(query) || x.EventAltName.Contains(query))).ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(ProjectEvent[] records)
        {
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
                            var excludedProperties = new string[] { "Id", "TSP" };
                            var properties = typeof(ProjectEvent).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
                        var dbEntry = await dbContext.ProjectEvents.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            if (String.IsNullOrEmpty(dbEntry.ClosedByPerson_Id) || String.IsNullOrEmpty(dbEntry.EventClosed))
                                errorMessage += string.Format("Event {0} not deleted. Please close before deleting\n", dbEntry.EventName);
                            else
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


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}