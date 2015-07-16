using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Mehdime.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.Abstract;
using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace SDDB.Domain.Services
{
    public class ProjectService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private IDbContextScopeFactory contextScopeFac;
        private PersonService personService;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public ProjectService(IDbContextScopeFactory contextScopeFac, PersonService personService)
        {
            this.contextScopeFac = contextScopeFac;
            this.personService = personService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get all 
        public virtual async Task<List<Project>> GetAsync(bool getActive = true)
        {
            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Projects.Where(x => x.IsActive == getActive)
                    .Include(x => x.ProjectManager).ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records; 
            }
        }

        //get by ids
        public virtual async Task<List<Project>> GetAsync(string[] ids, bool getActive = true)
        {
            if (ids == null || ids.Length == 0) throw new ArgumentNullException("ids");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                var records = await dbContext.Projects.Where(x => x.IsActive == getActive && ids.Contains(x.Id))
                    .Include(x => x.ProjectManager).ToListAsync().ConfigureAwait(false);

                foreach (var record in records) { record.FillRelatedIfNull(); }

                return records; 
            }
        }

        //lookup by query - returns only projects the person is assigned to
        public virtual Task<List<Project>> LookupAsync(string userId, string query = "", bool getActive = true)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                return dbContext.Projects.Where(x => x.ProjectPersons.Any(y => y.Id == userId) && x.IsActive == getActive &&
                    (x.ProjectName.Contains(query) || x.ProjectAltName.Contains(query) || x.ProjectCode.Contains(query))).ToListAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public virtual async Task<DBResult> EditAsync(Project[] records)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var record in records)
                    {
                        var dbEntry = await dbContext.Projects.FindAsync(record.Id).ConfigureAwait(false);
                        if (dbEntry == null)
                        {
                            record.Id = Guid.NewGuid().ToString();
                            dbContext.Projects.Add(record);
                        }
                        else
                        {
                            dbEntry.CopyModifiedProps(record);
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

        // Delete records by their Ids
        public virtual async Task<DBResult> DeleteAsync(string[] ids)
        {
            var errorMessage = "";

            using (var dbContextScope = contextScopeFac.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var personIds = await dbContext.Persons.Select(x => x.Id).ToArrayAsync().ConfigureAwait(false);

                    foreach (var id in ids)
                    {
                        var dbEntry = await dbContext.Projects.FindAsync(id).ConfigureAwait(false);
                        if (dbEntry != null)
                        {
                            //tasks prior to desactivating: check if documents, locations, assemblies, components assigned to projects
                            if ((await dbContext.Documents.Where(x => x.IsActive == true && x.AssignedToProject_Id == id).CountAsync().ConfigureAwait(false)) > 0)
                                errorMessage += string.Format("Project {0} not deleted, it has documents assigned to it\n", dbEntry.ProjectName);
                            else if ((await dbContext.Locations.Where(x => x.IsActive == true && x.AssignedToProject_Id == id).CountAsync().ConfigureAwait(false)) > 0)
                                errorMessage += string.Format("Project {0} not deleted, it has locations assigned to it\n", dbEntry.ProjectName);
                            else if ((await dbContext.AssemblyDbs.Where(x => x.IsActive == true && x.AssignedToProject_Id == id).CountAsync().ConfigureAwait(false)) > 0)
                                errorMessage += string.Format("Project {0} not deleted, it has assemblies assigned to it\n", dbEntry.ProjectName);
                            else if ((await dbContext.Components.Where(x => x.IsActive == true && x.AssignedToProject_Id == id).CountAsync().ConfigureAwait(false)) > 0)
                                errorMessage += string.Format("Project {0} not deleted, it has components assigned to it\n", dbEntry.ProjectName);
                            else
                            {
                                //tasks prior to desactivating: running PersonService and deleting ManagedProjects
                                var serviceResult = await personService.EditPersonProjectsAsync(personIds, new string[] { id }, false).ConfigureAwait(false);
                                if (serviceResult.StatusCode != HttpStatusCode.OK)
                                    errorMessage += string.Format("Error Removing Persons from Project {0}: {1} \n", dbEntry.ProjectName, serviceResult.StatusDescription);
                                else
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
