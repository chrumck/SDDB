using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mehdime.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;
using SDDB.Domain.Abstract;

namespace SDDB.Domain.Services
{
    public class PersonLogEntryService : BaseDbService<PersonLogEntry>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IAppUserManager appUserManager;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryService(IDbContextScopeFactory contextScopeFac, string userId, IAppUserManager appUserManager) 
            : base(contextScopeFac, userId) 
        {
            this.appUserManager = appUserManager;
        }
        
        
        //Methods--------------------------------------------------------------------------------------------------------------//

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
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by PersonLogEntry ids
        public virtual async Task<List<PersonLogEntryJSON>> GetAsync(string[] ids, bool getActive = true,
            bool filterForPLEView = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntrys
                    .Where(x =>
                        (!filterForPLEView || x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId)) &&
                        (filterForPLEView || 
                            x.EnteredByPerson_Id == userId || x.PrsLogEntryPersons.Any(y => y.Id == userId)) &&
                        ids.Contains(x.Id) &&
                        x.IsActive_bl == getActive
                        )
                    .Select(x => new PersonLogEntryJSON
                    {
                        Id = x.Id,
                        LogEntryDateTime = x.LogEntryDateTime,
                        EnteredByPerson_Id = x.EnteredByPerson_Id,
                        EnteredByPerson_ = new PersonJSON
                        {
                            Id = x.EnteredByPerson_Id,
                            FirstName = x.EnteredByPerson.FirstName,
                            LastName = x.EnteredByPerson.LastName,
                            Initials = x.EnteredByPerson.Initials
                        },
                        PersonActivityType_Id = x.PersonActivityType_Id,
                        PersonActivityType_ = new PersonActivityTypeJSON
                        {
                            Id = x.PersonActivityType_Id,
                            ActivityTypeName = x.PersonActivityType.ActivityTypeName
                        },
                        ManHours = x.ManHours,
                        AssignedToProject_Id = x.AssignedToProject_Id,
                        AssignedToProject_ = new ProjectJSON
                        {
                            Id = x.AssignedToProject_Id,
                            ProjectName = x.AssignedToProject.ProjectName,
                            ProjectCode = x.AssignedToProject.ProjectCode,
                        },
                        AssignedToLocation_Id = x.AssignedToLocation_Id,
                        AssignedToLocation_ = new LocationJSON
                        {
                            Id = x.AssignedToLocation_Id,
                            LocName = x.AssignedToLocation.LocName
                        },
                        AssignedToProjectEvent_Id = x.AssignedToProjectEvent_Id,
                        AssignedToProjectEvent_ = new ProjectEventJSON
                        {
                            Id = x.AssignedToProjectEvent_Id,
                            EventName = x.AssignedToProjectEvent.EventName,
                        },
                        QcdByPerson_Id = x.QcdByPerson_Id,
                        QcdByPerson_ = new PersonJSON
                        {
                            Id = x.QcdByPerson_Id,
                            FirstName = x.QcdByPerson.FirstName,
                            LastName = x.QcdByPerson.LastName,
                            Initials = x.QcdByPerson.Initials
                        },
                        QcdDateTime = x.QcdDateTime,
                        Comments = x.Comments,
                        PrsLogEntryFilesCount = x.PersonLogEntryFiles.Count,
                        PrsLogEntryAssysCount = x.PrsLogEntryAssemblyDbs.Count,
                        PrsLogEntryPersonsInitials = x.PrsLogEntryPersons.Count.ToString(),
                        PrsLogEntryPersons = x.PrsLogEntryPersons.Select(y => new PersonJSON
                        {
                            Id = y.Id,
                            FirstName = y.FirstName,
                            LastName = y.LastName,
                            Initials = y.Initials
                        }).ToList(),
                        IsActive_bl = x.IsActive_bl
                    })
                    .ToListAsync().ConfigureAwait(false);

                records.ForEach(x =>
                {
                    x.PrsLogEntryPersonsInitials = x.PrsLogEntryPersons.Aggregate("", (initials, person) =>
                        initials += String.IsNullOrEmpty(initials) ? person.Initials : " " + person.Initials);
                });

                return records;
            }
        }

        //get by personIds, projectIds, typeIds, startDate, endDate
        public virtual async Task<List<PersonLogEntryJSON>> GetByAltIdsAsync(string[] personIds, string[] projectIds,
            string[] assyIds, string[] typeIds, DateTime? startDate, DateTime? endDate, bool getActive = true,
            bool filterForPLEView = true)
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
                           (!filterForPLEView || x.AssignedToProject.ProjectPersons.Any(y => y.Id == userId)) &&
                           (filterForPLEView ||
                            x.EnteredByPerson_Id == userId || x.PrsLogEntryPersons.Any(y => y.Id == userId)) &&
                           (personIds.Count() == 0 || x.PrsLogEntryPersons.Any(y => personIds.Contains(y.Id)) ||
                                personIds.Contains(x.EnteredByPerson_Id)) &&
                           (projectIds.Count() == 0 || projectIds.Contains(x.AssignedToProject_Id)) &&
                           (assyIds.Count() == 0 || x.PrsLogEntryAssemblyDbs.Any(y => assyIds.Contains(y.Id))) &&
                           (typeIds.Count() == 0 || typeIds.Contains(x.PersonActivityType_Id)) &&
                           (startDate == null || x.LogEntryDateTime >= startDate) &&
                           (endDate == null || x.LogEntryDateTime <= endDate) &&
                           x.IsActive_bl == getActive
                           )
                        .Select(x => new PersonLogEntryJSON
                        {
                            Id = x.Id,
                            LogEntryDateTime = x.LogEntryDateTime,
                            EnteredByPerson_Id = x.EnteredByPerson_Id,
                            EnteredByPerson_ = new PersonJSON
                            {
                                Id = x.EnteredByPerson_Id,
                                FirstName = x.EnteredByPerson.FirstName,
                                LastName = x.EnteredByPerson.LastName,
                                Initials = x.EnteredByPerson.Initials
                            },
                            PersonActivityType_Id = x.PersonActivityType_Id,
                            PersonActivityType_ = new PersonActivityTypeJSON {
                                Id = x.PersonActivityType_Id,
                                ActivityTypeName = x.PersonActivityType.ActivityTypeName
                            },
                            ManHours = x.ManHours,
                            AssignedToProject_Id = x.AssignedToProject_Id,
                            AssignedToProject_ = new ProjectJSON {
                                Id = x.AssignedToProject_Id,
                                ProjectName = x.AssignedToProject.ProjectName,
                                ProjectCode = x.AssignedToProject.ProjectCode,
                            },
                            AssignedToLocation_Id = x.AssignedToLocation_Id,
                            AssignedToLocation_ = new LocationJSON {
                                Id = x.AssignedToLocation_Id,
                                LocName = x.AssignedToLocation.LocName
                            },
                            AssignedToProjectEvent_Id = x.AssignedToProjectEvent_Id,
                            AssignedToProjectEvent_ = new ProjectEventJSON {
                                Id = x.AssignedToProjectEvent_Id,
                                EventName = x.AssignedToProjectEvent.EventName,
                            },
                            QcdByPerson_Id = x.QcdByPerson_Id,
                            QcdByPerson_ = new PersonJSON
                            {
                                Id = x.QcdByPerson_Id,
                                FirstName = x.QcdByPerson.FirstName,
                                LastName = x.QcdByPerson.LastName,
                                Initials = x.QcdByPerson.Initials
                            },
                            QcdDateTime = x.QcdDateTime,
                            Comments = x.Comments,
                            PrsLogEntryFilesCount = x.PersonLogEntryFiles.Count,
                            PrsLogEntryAssysCount = x.PrsLogEntryAssemblyDbs.Count,
                            PrsLogEntryPersonsInitials = x.PrsLogEntryPersons.Count.ToString(),
                            PrsLogEntryPersons = x.PrsLogEntryPersons.Select(y => new PersonJSON
                            {
                                Id = y.Id,
                                FirstName = y.FirstName,
                                LastName = y.LastName,
                                Initials = y.Initials
                            }).ToList(),
                            IsActive_bl = x.IsActive_bl
                        })
                       .ToListAsync().ConfigureAwait(false);
                                
                records.ForEach(x =>
                {
                    x.PrsLogEntryPersonsInitials = x.PrsLogEntryPersons.Aggregate("",(initials, person) =>
                        initials += String.IsNullOrEmpty(initials) ? person.Initials : " " + person.Initials);
                });
                
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //GetActivitySummariesAsync
        public virtual async Task<List<ActivitySummary>> GetActivitySummariesAsync(string personId, 
            DateTime startDate, DateTime endDate, bool getActive = true)
        {
            if (String.IsNullOrEmpty(personId)) { throw new ArgumentNullException("personId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntrys
                    .Where(x =>
                           (x.PrsLogEntryPersons.Any(y => personId == y.Id) || personId == x.EnteredByPerson_Id) &&
                           (x.LogEntryDateTime >= startDate) &&
                           (x.LogEntryDateTime <= endDate) &&
                           x.IsActive_bl == getActive
                           )
                    .Include(x => x.AssignedToProject)
                    .ToListAsync().ConfigureAwait(false);
                var recordGroupsByDay = records.GroupBy(x => x.LogEntryDateTime.Date).ToList();
                return getSummariesFromGroups(personId, recordGroupsByDay);
            }
        }

        //GetLastEntrySummaryAsync
        public virtual async Task<List<LastEntrySummary>> GetLastEntrySummariesAsync(string activityTypeId,
            DateTime startDate, DateTime endDate, bool getActive = true)
        {
            if (String.IsNullOrEmpty(activityTypeId)) { throw new ArgumentNullException("activityTypeId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntrys
                    .Where(x =>
                           (activityTypeId == x.PersonActivityType_Id) &&
                           (x.LogEntryDateTime >= startDate) &&
                           (x.LogEntryDateTime <= endDate) &&
                           x.IsActive_bl == getActive
                           )
                    .Include(x => x.AssignedToProject)
                    .Include(x => x.EnteredByPerson)
                    .ToListAsync().ConfigureAwait(false);

                return records.GroupBy(x => new { project = x.AssignedToProject, date = x.LogEntryDateTime.Date})
                    .Select(x => {
                        var lastEntry = x.OrderBy(y => y.LogEntryDateTime).Last();
                        return new LastEntrySummary(
                                    x.Key.date,
                                    x.Key.project.Id,
                                    x.Key.project.ProjectName,
                                    x.Key.project.ProjectCode,
                                    x.Count(),
                                    lastEntry.LogEntryDateTime,
                                    lastEntry.EnteredByPerson.Initials,
                                    lastEntry.Comments
                            );
                        })
                    .ToList();
            }
        }
                
        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in [] - same as BaseDbService
        // See overriden checkBeforeEditHelperAsync(EFDbContext dbContext, PersonLogEntry record)


        // Delete records by their Ids - same as BaseDbService

        //QcLogEntries find Person Log Entries by ids and ad Quality Control data based on userId and DateTime.Now
        public virtual async Task QcLogEntriesAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            await dbScopeHelperAsync(async dbContext =>
            {
                List<PersonLogEntry> dbEntrys = await dbContext.PersonLogEntrys
                    .Where(x => ids.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

                if (dbEntrys.Count != ids.Length)
                { throw new DbBadRequestException("Some or all DB Entries not found"); }

                foreach (var dbEntry in dbEntrys)
                {
                    await checkBeforeQcHelperAsync(dbEntry);
                    dbEntry.QcdByPerson_Id = userId;
                    dbEntry.QcdDateTime = DateTime.Now;
                }
                return default(int);
            })
            .ConfigureAwait(false);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        //get all person log entry assemblies
        public virtual Task<List<AssemblyDb>> GetPrsLogEntryAssysAsync(string[] logEntryIds)
        {
            if (logEntryIds == null || logEntryIds.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.AssemblyDbs
                    .Where(x => 
                        x.AssemblyDbPrsLogEntrys.Any(y => logEntryIds.Contains(y.Id)) &&
                        x.IsActive_bl
                    )
                    .ToListAsync();
            }
        }

        //get all active assemblies from the location not assigned to log entry
        public virtual Task<List<AssemblyDb>> GetPrsLogEntryAssysNotAsync(string[] logEntryIds, string locId)
        {
            if (logEntryIds == null || logEntryIds.Length == 0) { throw new ArgumentNullException("ids"); }
            locId = locId ?? String.Empty;

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.AssemblyDbs
                    .Where(x =>
                        x.AssignedToLocation.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        !logEntryIds.All(y => x.AssemblyDbPrsLogEntrys.Select(z => z.Id).Contains(y)) &&
                        (locId == String.Empty || x.AssignedToLocation_Id == locId) &&
                        x.IsActive_bl
                    )
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Assemblies to Person Log Entry 
        //use generic version AddRemoveRelated from BaseDbService to replace EditPrsLogEntryAssysAsync
        // see overriden checkBeforeAddRemoveHelperAsync

        //-----------------------------------------------------------------------------------------------------------------------
        
        //get all person log entry persons
        public virtual Task<List<Person>> GetPrsLogEntryPersonsAsync(string[] logEntryIds)
        {
            if (logEntryIds == null || logEntryIds.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Persons
                    .Where(x => 
                        x.PersonPrsLogEntrys.Any(y => logEntryIds.Contains(y.Id)) &&
                        x.IsActive_bl
                    )
                    .ToListAsync();
            }
        }

        //get all active persons managed by user and not assigned to log entry
        public virtual Task<List<Person>> GetPrsLogEntryPersonsNotAsync(string[] logEntryIds)
        {
            if (logEntryIds == null || logEntryIds.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                //return dbContext.PersonGroups
                //    .Where(x => x.GroupManagers.Any(y => y.Id == userId)).SelectMany(x => x.GroupPersons).Distinct()
                //    .Where(x => !x.PersonPrsLogEntrys.Any(y => logEntryIds.Contains(y.Id)) && x.IsActive_bl)
                //    .ToListAsync();

                return dbContext.Persons
                    .Where(x => 
                        x.PersonGroups.Any(y => y.GroupManagers.Any(z => z.Id == userId)) &&
                        !logEntryIds.All(y => x.PersonPrsLogEntrys.Select(z => z.Id).Contains(y)) &&
                        x.IsActive_bl)
                    .ToListAsync();
            }
        }

        //Add (or Remove  when set isAdd to false) Persons to Person Log Entry
        //use generic version AddRemoveRelated from BaseDbService to replace EditPrsLogEntryPersonsAsync

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //returning List<ActivitySummary> from List<IGrouping<DateTime,PersonLogEntry>>
        private List<ActivitySummary> getSummariesFromGroups(string personId, 
            List<IGrouping<DateTime,PersonLogEntry>> groups)
        {
            var activitySummaries = new List<ActivitySummary>();

            foreach (var group in groups)
            {
                List<ActivitySummaryDetail> summaryDetails = group.GroupBy(
                    x => x.AssignedToProject_Id,
                    (projectId, logEntries) => new ActivitySummaryDetail(
                        projectId,
                        logEntries.First().AssignedToProject.ProjectName,
                        logEntries.First().AssignedToProject.ProjectCode,
                        logEntries.Sum(logEntry => logEntry.ManHours)
                        )
                    ).ToList();
                var activitySummary = new ActivitySummary(personId, group.Key, group.Sum(x => x.ManHours), summaryDetails);
                activitySummaries.Add(activitySummary);
            }
            return activitySummaries;
        }

        //-----------------------------------------------------------------------------------------------------------------------
        
        //checking if log entry, event and location belong to the same project - overriden from BaseDbService
        protected override async Task checkBeforeEditHelperAsync(EFDbContext dbContext, PersonLogEntry record)
        {
            if (record.PropIsModified(x => x.QcdByPerson_Id)) { await checkBeforeQcHelperAsync(record); }
            
            var projEvent = await dbContext.ProjectEvents.FindAsync(record.AssignedToProjectEvent_Id).ConfigureAwait(false);
            if (record.AssignedToProjectEvent_Id != null && projEvent.AssignedToProject_Id != record.AssignedToProject_Id)
            { throw new DbBadRequestException("Log Entry and Project Event do not belong to the same project.\n Entry(ies) not saved."); }

            var location = await dbContext.Locations.FindAsync(record.AssignedToLocation_Id).ConfigureAwait(false);
            if (record.AssignedToLocation_Id != null && location.AssignedToProject_Id != record.AssignedToProject_Id)
            { throw new DbBadRequestException("Log Entry and Location do not belong to the same project.\n Entry(ies) not saved."); }

            var recordProject = await dbContext.Projects.Where(x => x.Id == record.AssignedToProject_Id)
                .Include(x => x.ProjectPersons).FirstOrDefaultAsync().ConfigureAwait(false);
            if (recordProject.ProjectPersons.All(x => x.Id != userId))
            { throw new DbBadRequestException("Log Entry assigned to project which is not managed by you.\nEntry(ies) not saved."); }

            var logEntry = await dbContext.PersonLogEntrys.Where(x => x.Id == record.Id)
                .Include(x => x.AssignedToProject.ProjectPersons).FirstOrDefaultAsync().ConfigureAwait(false);
            if (logEntry != null && logEntry.AssignedToProject.ProjectPersons.All(x => x.Id != userId))
            { throw new DbBadRequestException("Log Entry you try to modify is assigned to project which is not managed by you.\nEntry(ies) not saved."); }

        }

        //checkBeforeQcHelperAsync
        private async Task checkBeforeQcHelperAsync(PersonLogEntry record)
        {
            if (!(await appUserManager.IsInRoleAsync(userId, "PersonLogEntry_Qc").ConfigureAwait(false)))
            { throw new DbBadRequestException("You do not have sufficient rights to QC Person Entries."); }

            if (record.EnteredByPerson_Id == userId) 
            { throw new DbBadRequestException("You cannot QC your own entry."); }
        }

        
        //-----------------------------------------------------------------------------------------------------------------------

        //checkBeforeAddRemoveHelperAsync - overriden from BaseDbService
        protected override async Task checkBeforeAddRemoveHelperAsync<TAddRem>(EFDbContext dbContext,
            string[] ids, string[] idsAddRem, bool isAdd)
        {
            if (!isAdd || typeof(TAddRem) != typeof(AssemblyDb)) { return; }

            var AddProjectIds = await dbContext.AssemblyDbs
                .Where(x => idsAddRem.Contains(x.Id))
                .Select(x => x.AssignedToLocation.AssignedToProject_Id)
                .ToListAsync().ConfigureAwait(false);

            if (AddProjectIds.Any(x => x != AddProjectIds[0]))
            { throw new DbBadRequestException(
                    "Assemblies do not belong to the same Project.\n Assemblies not added to Log Entry."); }

            var dbEntries = await dbContext.PersonLogEntrys
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.AssignedToLocation)
                .ToListAsync().ConfigureAwait(false);

            foreach (var dbEntry in dbEntries)
            {
                if (dbEntry.AssignedToLocation.AssignedToProject_Id != AddProjectIds[0])
                { throw new DbBadRequestException(
                    "Assembly(ies) and Person Log Entry do not belong to the same Project.\n" + 
                    "Assembly(ies) not added to Log Entry."); }
            }
        }
        
        #endregion
    }
}
