using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
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
        private string userId;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryFileService(IDbContextScopeFactory contextScopeFac, string userId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("userId"); }

            this.contextScopeFac = contextScopeFac;
            this.userId = userId;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //list files by PersonLogEntry_Id 
        public virtual async Task<List<PersonLogEntryFile>> ListAsync(string logEntryId)
        {
            if (String.IsNullOrEmpty(logEntryId)) { throw new ArgumentNullException("logEntryId"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.PersonLogEntryFiles
                    .Where(x => 
                        x.AssignedToPersonLogEntry.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        x.AssignedToPersonLogEntry_Id == logEntryId)
                    .ToListAsync().ConfigureAwait(false);

                return records;
            }
        }

        //get by PersonLogEntryFile ids
        public virtual async Task<List<PersonLogEntryFile>> ListAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                var records = await dbContext.PersonLogEntryFiles
                    .Where(x => 
                        x.AssignedToPersonLogEntry.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        ids.Contains(x.Id))
                    .ToListAsync().ConfigureAwait(false);

                return records;
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------------------

        ////upload file
        //public virtual async Task UploadAsync(PersonLogEntryFile file)
        //{

        //}


        ////DownloadAsync - download log entry files from database. Return file if only one or .zip if many
        //public virtual async Task<byte[]> DownloadAsync(string id, string[] names)
        //{ 
        //}


        ////DeleteAsync - upload received files to ftp
        //public virtual async Task DeleteAsync(string id)
        //{

        //}


        

        //-----------------------------------------------------------------------------------------------------------------------

        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
