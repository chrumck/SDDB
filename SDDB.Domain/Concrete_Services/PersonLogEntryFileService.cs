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
    public class PersonLogEntryFileService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryFileService(IDbContextScopeFactory contextScopeFac)
        {
            this.contextScopeFac = contextScopeFac;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //list files by PersonLogEntry_Id 
        public virtual async Task<List<PersonLogEntryFile>> ListAsync(string userId, string logEntryId)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (String.IsNullOrEmpty(logEntryId)) throw new ArgumentNullException("logEntryId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.PersonLogEntryFiles
                    .Where(x => x.AssignedToPersonLogEntry.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        x.AssignedToPersonLogEntry_Id == logEntryId).ToListAsync().ConfigureAwait(false);

                return records;
            }
        }

        //get by ids
        public virtual async Task<List<PersonLogEntryFile>> ListAsync(string userId, string[] ids)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            if (ids == null || ids.Length == 0 ) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntryFiles
                    .Where(x => x.AssignedToPersonLogEntry.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                return records;
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------------------

        //upload file
        public virtual async Task UploadAsync(PersonLogEntryFile file)
        {

        }

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(string userId, PersonLogEntryFile[] records)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            var errorMessage = ""; 
            var serviceResult = new DBResult();

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        //checking if log entry, event and location belong to the same project
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
                            var dbEntry = await dbContext.PersonLogEntryFiles.FindAsync(record.Id).ConfigureAwait(false);
                            if (dbEntry == null)
                            {
                                record.Id = Guid.NewGuid().ToString();
                                record.EnteredByPerson_Id = record.EnteredByPerson_Id ?? userId;
                                dbContext.PersonLogEntryFiles.Add(record);
                                serviceResult.ReturnIds.Add(record.Id);
                            }
                            else
                            {
                                dbEntry.CopyModifiedProps(record);
                            }
                        }
                    }
                    errorMessage += await DbHelpers.SaveChangesAsync(dbContext).ConfigureAwait(false);
                    trans.Complete();
                }
            }
            if (errorMessage == "") { return serviceResult; }
            else
            {
                serviceResult.StatusCode = HttpStatusCode.Conflict;
                serviceResult.StatusDescription = "Errors editing records:\n" + errorMessage;
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
                        var dbEntry = await dbContext.PersonLogEntryFiles.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            dbEntry.IsActive_bl = false;
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
                locId = locId ?? "";

                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.AssemblyDbs
                    .Where(x => !x.AssemblyDbPrsLogEntrys.Any(y => y.Id == logEntryId) && x.IsActive == true && 
                        (locId == "" || x.AssignedToLocation_Id == locId))
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Assemblies to Person Log Entry
        public virtual async Task<DBResult> EditPrsLogEntryAssysAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            if (ids == null || ids.Length == 0 || idsAddRem == null || idsAddRem.Length == 0)
                return new DBResult { StatusCode = HttpStatusCode.BadRequest, StatusDescription = "arguments missing" };

            var errorMessage = ""; 

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var logEntrys = await dbContext.PersonLogEntryFiles.Include(x => x.PrsLogEntryAssemblyDbs).Where(x => ids.Contains(x.Id))
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

        //-----------------------------------------------------------------------------------------------------------------------

        //get all person log entry persons
        public virtual Task<List<Person>> GetPrsLogEntryPersonsAsync(string logEntryId)
        {
            if (String.IsNullOrEmpty(logEntryId)) throw new ArgumentNullException("logEntryId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons
                    .Where(x => x.PersonPrsLogEntrys.Any(y => y.Id == logEntryId) && x.IsActive_bl == true).ToListAsync();
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
                    .Distinct().Where(x => x.IsActive_bl == true && !x.PersonPrsLogEntrys.Any(y => y.Id == logEntryId))
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Persons to Person Log Entry
        public virtual async Task<DBResult> EditPrsLogEntryPersonsAsync(string[] ids, string[] idsAddRem, bool isAdd)
        {
            if (ids == null || ids.Length == 0 || idsAddRem == null || idsAddRem.Length == 0)
                return new DBResult { StatusCode = HttpStatusCode.BadRequest, StatusDescription = "arguments missing" };

            var errorMessage = "";
            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var logEntrys = await dbContext.PersonLogEntryFiles.Include(x => x.PrsLogEntryPersons).Where(x => ids.Contains(x.Id))
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


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
