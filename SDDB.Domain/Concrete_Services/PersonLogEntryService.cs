using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
    public class PersonLogEntryService : BaseDbService<PersonLogEntry>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }
        
        
        //Methods--------------------------------------------------------------------------------------------------------------//

        //get by PersonLogEntry ids
        public virtual async Task<List<PersonLogEntry>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntrys
                    .Where(x => 
                        x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Include(x => x.EnteredByPerson)
                    .Include(x => x.PersonActivityType)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .Include(x => x.AssignedToProjectEvent)
                    .Include(x => x.PersonLogEntryFiles)
                    .ToListAsync().ConfigureAwait(false);
                
                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by PersonLogEntry ids - no filters on person Id and getActive
        //used by PersonLogEntrySrvController.isUserActivity(string[] ids)
        public virtual async Task<List<PersonLogEntry>> GetAllAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntrys
                    .Where(x => ids.Contains(x.Id))
                    .Include(x => x.EnteredByPerson)
                    .Include(x => x.PersonActivityType)
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.AssignedToLocation)
                    .Include(x => x.AssignedToProjectEvent)
                    .Include(x => x.PersonLogEntryFiles)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by personIds, projectIds, typeIds, startDate, endDate
        public virtual async Task<List<PersonLogEntry>> GetByAltIdsAsync(string[] personIds, string[] projectIds, 
            string[] assyIds, string[] typeIds, DateTime? startDate, DateTime? endDate, bool getActive = true)
        {
            personIds = personIds ?? new string[] { };
            projectIds = projectIds ?? new string[] { };
            assyIds = assyIds ?? new string[] { };
            typeIds = typeIds ?? new string[] { };

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.PersonLogEntrys
                       .Where(x => 
                           x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) && 
                           (personIds.Count() == 0 || x.PrsLogEntryPersons.Any(y => personIds.Contains(y.Id)) ||
                                personIds.Contains(x.EnteredByPerson_Id) ) &&
                           (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                           (assyIds.Count() == 0 || x.PrsLogEntryAssemblyDbs.Any(y => assyIds.Contains(y.Id))) &&
                           (typeIds.Count() == 0 || typeIds.Contains(x.PersonActivityType_Id)) &&
                           (startDate == null || x.LogEntryDateTime >= startDate) &&
                           (endDate == null || x.LogEntryDateTime <= endDate) &&
                           x.IsActive_bl == getActive
                           ) 
                       .Include(x => x.EnteredByPerson)
                       .Include(x => x.PersonActivityType)
                       .Include(x => x.AssignedToProject)
                       .Include(x => x.AssignedToLocation)
                       .Include(x => x.AssignedToProjectEvent)
                       .Include(x => x.PersonLogEntryFiles)
                       .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        ////get by personIds, projectIds, typeIds, startDate, endDate
        //public virtual async Task<List<PersonLogEntry>> GetForTimesheetAsync(string personId, DateTime startDate, DateTime endDate, bool getActive = true)
        //{
        //    if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

        //    using (var dbContextScope = contextScopeFac.CreateReadOnly())
        //    {
        //        var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

        //        var records = await dbContext.PersonLogEntrys
        //               .GroupBy(x => x.AssignedToProject_Id,)
        //               .Include(x => x.EnteredByPerson)
        //               .Include(x => x.PersonActivityType)
        //               .Include(x => x.AssignedToProject)
        //               .Include(x => x.AssignedToLocation)
        //               .Include(x => x.AssignedToProjectEvent)
        //               .Include(x => x.PersonLogEntryFiles)
        //               .ToListAsync().ConfigureAwait(false);

        //        records.FillRelatedIfNull();
        //        return records;
        //    }
        //}
                
        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in [] - same as BaseDbService
        // See overriden checkBeforeEditHelperAsync(EFDbContext dbContext, PersonLogEntry record)


        // Delete records by their Ids - same as BaseDbService

        //-----------------------------------------------------------------------------------------------------------------------

        //get all person log entry assemblies
        public virtual Task<List<AssemblyDb>> GetPrsLogEntryAssysAsync(string logEntryId)
        {
            if (String.IsNullOrEmpty(logEntryId)) { throw new ArgumentNullException("logEntryId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                return dbContext.AssemblyDbs
                    .Where(x => x.AssemblyDbPrsLogEntrys.Any(y => y.Id == logEntryId) && x.IsActive_bl)
                    .ToListAsync();
            }
        }

        //get all active assemblies from the location not assigned to log entry
        public virtual Task<List<AssemblyDb>> GetPrsLogEntryAssysNotAsync(string logEntryId, string locId)
        {
            if (String.IsNullOrEmpty(logEntryId)) { throw new ArgumentNullException("logEntryId"); }
            
            locId = locId ?? String.Empty;

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                return dbContext.AssemblyDbs
                    .Where(x =>
                        !x.AssemblyDbPrsLogEntrys.Any(y => y.Id == logEntryId) &&
                        x.IsActive_bl && 
                        (locId == String.Empty || x.AssignedToLocation_Id == locId)
                    )
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Assemblies to Person Log Entry 
        //use generic version AddRemoveRelated from BaseDbService to replace EditPrsLogEntryAssysAsync

        //-----------------------------------------------------------------------------------------------------------------------
        
        //get all person log entry persons
        public virtual Task<List<Person>> GetPrsLogEntryPersonsAsync(string logEntryId)
        {
            if (String.IsNullOrEmpty(logEntryId)) { throw new ArgumentNullException("logEntryId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Persons
                    .Where(x => x.PersonPrsLogEntrys.Any(y => y.Id == logEntryId) && x.IsActive_bl)
                    .ToListAsync();
            }
        }

        //get all active persons managed by user and not assigned to log entry
        public virtual Task<List<Person>> GetPrsLogEntryPersonsNotAsync(string userId, string logEntryId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("userId"); }
            if (String.IsNullOrEmpty(logEntryId)) { throw new ArgumentNullException("logEntryId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.PersonGroups
                    .Where(x => x.GroupManagers.Any(y => y.Id == userId)).SelectMany(x => x.GroupPersons).Distinct()
                    .Where(x => x.IsActive_bl && !x.PersonPrsLogEntrys.Any(y => y.Id == logEntryId))
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Persons to Person Log Entry
        //use generic version AddRemoveRelated from BaseDbService to replace EditPrsLogEntryPersonsAsync

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //checking if log entry, event and location belong to the same project - overriden from BaseDbService
        protected override async Task checkBeforeEditHelperAsync(EFDbContext dbContext, PersonLogEntry record)
        {
                var projEvent = await dbContext.ProjectEvents.FindAsync(record.AssignedToProjectEvent_Id).ConfigureAwait(false);
                var location = await dbContext.Locations.FindAsync(record.AssignedToLocation_Id).ConfigureAwait(false);

                if (record.AssignedToProjectEvent_Id != null && projEvent.AssignedToProject_Id != record.AssignedToProject_Id)
                { throw new DbBadRequestException("Log Entry and Project Event do not belong to the same project.\n Entry(ies) not saved."); }
                
                if (record.AssignedToLocation_Id != null && location.AssignedToProject_Id != record.AssignedToProject_Id)
                { throw new DbBadRequestException("Log Entry and Location do not belong to the same project.\n Entry(ies) not saved."); }
        }

        
        #endregion
    }
}
