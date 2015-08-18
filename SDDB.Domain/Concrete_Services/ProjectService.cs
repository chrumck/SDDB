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
    public class ProjectService : BaseDbService<Project>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public ProjectService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<Project>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Projects
                    .Where(x => x.IsActive_bl == getActive)
                    .Include(x => x.ProjectManager)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //get by ids
        public virtual async Task<List<Project>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Projects
                    .Where(x => 
                            ids.Contains(x.Id) &&
                            x.IsActive_bl == getActive
                        )
                    .Include(x => x.ProjectManager)
                    .ToListAsync().ConfigureAwait(false);

                records.FillRelatedIfNull();
                return records;
            }
        }

        //lookup by query - returns only projects the person is assigned to
        public virtual async Task<List<Project>> LookupAsync(string query = "", bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Projects
                    .Where(x =>
                            x.ProjectPersons.Any(y => y.Id == userId) &&
                           (x.ProjectName.Contains(query) || x.ProjectCode.Contains(query)) &&
                            x.IsActive_bl == getActive
                        )
                        .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []  - same as BaseDbService

        // Delete records by their Ids - same as BaseDbService
        // See overriden checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)

        //-----------------------------------------------------------------------------------------------------------------------

        //get all project persons
        public virtual Task<List<Person>> GetProjectPersonsAsync(string Id)
        {
            if (String.IsNullOrEmpty(Id)) { throw new ArgumentNullException("Id"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons.Where(x =>
                        x.PersonProjects.Any(y => y.Id == Id) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }

        //get all project persons
        public virtual Task<List<Person>> GetProjectPersonsNotAsync(string Id)
        {
            if (String.IsNullOrEmpty(Id)) { throw new ArgumentNullException("Id"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                return dbContext.Persons.Where(x =>
                        !x.PersonProjects.Any(y => y.Id == Id) &&
                        x.IsActive_bl == true
                    )
                    .ToListAsync();
            }
        }
        
        //Add (or Remove  when set isAdd to false) persons to Project
        //use generic version AddRemoveRelated from BaseDbService 

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //helper - check before deleting records, takes LocationModel ids array
        protected override async Task checkBeforeDeleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var currentId = ids[i];
                if (await dbContext.Documents.AnyAsync(x => x.IsActive_bl && x.AssignedToProject_Id == currentId)
                    .ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Projects.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Project {0} has documents assigned to it.\nDelete aborted.", dbEntry.ProjectName));
                }
                if (await dbContext.Locations.AnyAsync(x => x.IsActive_bl && x.AssignedToProject_Id == currentId)
                    .ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Projects.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Project {0} has locations assigned to it.\nDelete aborted.", dbEntry.ProjectName));
                }
                if (await dbContext.AssemblyDbs.AnyAsync(x => x.IsActive_bl && x.AssignedToProject_Id == currentId)
                    .ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Projects.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Project {0} has assemblies assigned to it.\nDelete aborted.", dbEntry.ProjectName));
                }
                if (await dbContext.Components.AnyAsync(x => x.IsActive_bl && x.AssignedToProject_Id == currentId)
                    .ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Projects.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Project {0} has components assigned to it.\nDelete aborted.", dbEntry.ProjectName));
                }
                if (await dbContext.Projects.AnyAsync(x => x.Id == currentId && x.ProjectPersons.Count > 0).ConfigureAwait(false))
                {
                    var dbEntry = await dbContext.Projects.FindAsync(currentId).ConfigureAwait(false);
                    throw new DbBadRequestException(
                        string.Format("Project {0} has persons assigned to it.\nDelete aborted.", dbEntry.ProjectName));
                }
            }
        }


        #endregion
    }
}
