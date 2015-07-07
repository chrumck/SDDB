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
    public class PersonLogEntryService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<PersonLogEntry>> GetAsync(string userId, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.PersonLogEntrys
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive)
                    .Include(x => x.EnteredByPerson).Include(x => x.PersonActivityType)
                    .Include(x => x.AssignedToProject).Include(x => x.AssignedToLocation).Include(x => x.AssignedToProjectEvent)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(PersonLogEntry).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual async Task<List<PersonLogEntry>> GetAsync(string userId, string[] ids, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0 ) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntrys
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive && ids.Contains(x.Id))
                    .Include(x => x.EnteredByPerson).Include(x => x.PersonActivityType)
                    .Include(x => x.AssignedToProject).Include(x => x.AssignedToLocation).Include(x => x.AssignedToProjectEvent)
                    .ToListAsync().ConfigureAwait(false);
                
                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(PersonLogEntry).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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

        //get by personIds, projectIds, typeIds, startDate, endDate
        public virtual async Task<List<PersonLogEntry>> GetByAltIdsAsync(string userId, string[] personIds = null, string[] projectIds = null,
            string[] typeIds = null, DateTime? startDate = null, DateTime? endDate = null, bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                personIds = personIds ?? new string[] { }; projectIds = projectIds ?? new string[] { }; typeIds = typeIds ?? new string[] { };
                
                var records = await dbContext.PersonLogEntrys
                       .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                            (personIds.Count() == 0 || x.PrsLogEntryPersons.Any(y => personIds.Contains(y.Id)) ||
                                personIds.Contains(x.EnteredByPerson_Id)) &&
                            (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                            (typeIds.Count() == 0 || typeIds.Contains(x.PersonActivityType_Id)) &&
                            (startDate == null || x.LogEntryDateTime >= startDate) &&
                            (endDate == null || x.LogEntryDateTime <= endDate)  )
                       .Include(x => x.EnteredByPerson).Include(x => x.PersonActivityType)
                       .Include(x => x.AssignedToProject).Include(x => x.AssignedToLocation).Include(x => x.AssignedToProjectEvent)
                       .ToListAsync().ConfigureAwait(false);

                foreach (var record in records)
                {
                    var excludedProperties = new string[] { "Id", "TSP" };
                    var properties = typeof(PersonLogEntry).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
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
        public virtual Task<List<PersonLogEntry>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.PersonLogEntrys
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (x.EnteredByPerson.Initials.Contains(query) || x.EnteredByPerson.LastName.Contains(query) || x.Comments.Contains(query)))
                    .Include(x => x.EnteredByPerson).ToListAsync();
            }
        }

        //lookup by query and project
        public virtual async Task<List<PersonLogEntry>> LookupByProjAsync(string userId, string[] projectIds = null, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                projectIds = projectIds ?? new string[] { };
                
                var records = await dbContext.PersonLogEntrys
                    .Where(x => x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && x.IsActive_bl == getActive &&
                        (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                        (x.EnteredByPerson.Initials.Contains(query) || x.EnteredByPerson.LastName.Contains(query) || x.Comments.Contains(query)))
                    .Include(x => x.EnteredByPerson).ToListAsync();
                
                return records;
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(string userId, PersonLogEntry[] records)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            var errorMessage = ""; var serviceResult = new DBResult();
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var projEvent = await dbContext.ProjectEvents.FindAsync(record.AssignedToProjectEvent_Id).ConfigureAwait(false);
                        var location = await dbContext.Locations.FindAsync(record.AssignedToLocation_Id).ConfigureAwait(false);

                        if (record.PropIsModified(x => x.AssignedToProjectEvent_Id) && record.AssignedToProjectEvent_Id != null &&
                            projEvent.AssignedToProject_Id != record.AssignedToProject_Id )
                        {
                            errorMessage += "Log Entry and Project Event do not belong to the same project. Entry not saved.";
                        }
                        else if (record.PropIsModified(x => x.AssignedToLocation_Id) && record.AssignedToLocation_Id != null &&
                            location.AssignedToProject_Id != record.AssignedToProject_Id)
                        {
                            errorMessage += "Log Entry and Location do not belong to the same project. Entry not saved.";
                        }
                        else
                        {
                            var dbEntry = await dbContext.PersonLogEntrys.FindAsync(record.Id).ConfigureAwait(false);
                            if (dbEntry == null)
                            {
                                record.Id = Guid.NewGuid().ToString();
                                record.EnteredByPerson_Id = record.EnteredByPerson_Id ?? userId;
                                dbContext.PersonLogEntrys.Add(record);
                                serviceResult.ReturnIds.Add(record.Id);

                            }
                            else
                            {
                                var excludedProperties = new string[] { "Id", "TSP" };
                                var properties = typeof(PersonLogEntry).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
                                foreach (var property in properties)
                                {
                                    if (property.GetMethod.IsVirtual) continue;
                                    if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                                    if (excludedProperties.Contains(property.Name)) continue;

                                    if (record.PropIsModified(property.Name)) property.SetValue(dbEntry, property.GetValue(record));
                                }
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
            if (errorMessage == "") return serviceResult;
            else
            {
                serviceResult.StatusCode = HttpStatusCode.Conflict; serviceResult.StatusDescription = "Errors editing records:\n" + errorMessage;
                return serviceResult;
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
                        var dbEntry = await dbContext.PersonLogEntrys.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            dbEntry.IsActive_bl = false;
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

        //get all person log entry assemblies
        public virtual Task<List<AssemblyDb>> GetPrsLogEntryAssysAsync(string logEntryId)
        {
            if (String.IsNullOrEmpty(logEntryId)) throw new ArgumentNullException("logEntryId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.AssemblyDbs
                    .Where(x => x.AssemblyDbPrsLogEntrys.Any(y => y.Id == logEntryId) && x.IsActive == true).ToListAsync();
            }
        }

        //get all active assemblies from the location not assigned to log entry
        public virtual Task<List<AssemblyDb>> GetPrsLogEntryAssysNotAsync(string logEntryId, string locId = null)
        {
            if (String.IsNullOrEmpty(logEntryId)) throw new ArgumentNullException("logEntryId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                locId = locId ?? "";

                return dbContext.AssemblyDbs
                    .Where(x => !x.AssemblyDbPrsLogEntrys.Any(y => y.Id == logEntryId) && x.IsActive == true && 
                        (locId == "" || x.AssignedToLocation_Id == locId))
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Assemblies to Person Log Entry
        public virtual async Task<DBResult> EditPrsLogEntryAssysAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            var errorMessage = ""; var serviceResult = new DBResult();
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var logEntrys = await dbContext.PersonLogEntrys.Include(x => x.PrsLogEntryAssemblyDbs).Where(x => ids.Contains(x.Id))
                       .ToListAsync().ConfigureAwait(false);

                    var assys = await dbContext.AssemblyDbs.Where(x => idsAddRem.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                    foreach (var logEntry in logEntrys)
                    {
                        foreach (var assy in assys)
                        {
                            if (isAdd) { if (!logEntry.PrsLogEntryAssemblyDbs.Contains(assy)) logEntry.PrsLogEntryAssemblyDbs.Add(assy); }
                            else {
                                if (logEntry.PrsLogEntryAssemblyDbs.Contains(assy)) logEntry.PrsLogEntryAssemblyDbs.Remove(assy); 
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
                                errorMessage += string.Format("Error Saving changes: {0}\n", message);
                                break;
                            }
                        }
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                    trans.Complete();
                }
            }

            if (errorMessage == "" && serviceResult.StatusCode == HttpStatusCode.OK) return serviceResult;
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage + serviceResult.StatusDescription
            };
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //get all person log entry persons
        public virtual Task<List<Person>> GetPrsLogEntryPersonsAsync(string logEntryId)
        {
            if (String.IsNullOrEmpty(logEntryId)) throw new ArgumentNullException("logEntryId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Persons
                    .Where(x => x.PersonPrsLogEntrys.Any(y => y.Id == logEntryId) && x.IsActive == true).ToListAsync();
            }
        }

        //get all active persons managed by user and not assigned to log entry
        public virtual Task<List<Person>> GetPrsLogEntryPersonsNotAsync(string userId, string logEntryId)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (String.IsNullOrEmpty(logEntryId)) throw new ArgumentNullException("logEntryId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.PersonGroups.Where(x => x.GroupManagers.Any(y => y.Id == userId)).SelectMany(x => x.GroupPersons)
                    .Distinct().Where(x => x.IsActive == true && !x.PersonPrsLogEntrys.Any(y => y.Id == logEntryId))
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Persons to Person Log Entry
        public virtual async Task<DBResult> EditPrsLogEntryPersonsAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            var errorMessage = ""; var serviceResult = new DBResult();
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var logEntrys = await dbContext.PersonLogEntrys.Include(x => x.PrsLogEntryPersons).Where(x => ids.Contains(x.Id))
                       .ToListAsync().ConfigureAwait(false);

                    var persons = await dbContext.Persons.Where(x => idsAddRem.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                    foreach (var logEntry in logEntrys)
                    {
                        foreach (var person in persons)
                        {
                            if (isAdd) { if (!logEntry.PrsLogEntryPersons.Contains(person)) logEntry.PrsLogEntryPersons.Add(person); }
                            else
                            {
                                if (logEntry.PrsLogEntryPersons.Contains(person)) logEntry.PrsLogEntryPersons.Remove(person);
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
                                errorMessage += string.Format("Error Saving changes: {0}\n", message);
                                break;
                            }
                        }
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                    trans.Complete();
                }
            }

            if (errorMessage == "" && serviceResult.StatusCode == HttpStatusCode.OK) return serviceResult;
            else return new DBResult
            {
                StatusCode = HttpStatusCode.Conflict,
                StatusDescription = "Errors editing records:\n" + errorMessage + serviceResult.StatusDescription
            };
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
