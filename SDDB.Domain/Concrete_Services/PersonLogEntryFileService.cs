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
    public class PersonLogEntryFileService :BaseDbService<PersonLogEntryFile>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryFileService(IDbContextScopeFactory contextScopeFac, string userId) 
            : base(contextScopeFac, userId) { }

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
                
        //-----------------------------------------------------------------------------------------------------------------------


        //upload file - overload for PersonLogEntryFile[]
        public virtual async Task<List<String>> AddFilesAsync(PersonLogEntryFile[] records)
        {
            if (records == null || records.Length == 0) { throw new ArgumentNullException("files"); }

            var newEntryIds = await dbScopeHelperAsync(dbContext =>
            {
                return addToPersonLogEntryFilesHelper(dbContext, records);
            })
            .ConfigureAwait(false);

            return newEntryIds;
        }


        ////DownloadAsync - download log entry files from database. Return file if only one or .zip if many
        //public virtual async Task<byte[]> DownloadAsync(string id, string[] names)
        //{ 
        //}


        ////DeleteAsync - upload received files to ftp
        //public virtual async Task DeleteAsync(string id)
        //{

        //}


                
        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //upload (save to database) - overload for PersonLogEntryFile[]
        private async Task<List<string>> addToPersonLogEntryFilesHelper(EFDbContext dbContext, PersonLogEntryFile[] records)
        {
            var newEntryIds = new List<string>();
            for (int i = 0; i < records.Length; i++)
            {
                var newEntryId = await addToPersonLogEntryFilesHelper(dbContext, records[i]).ConfigureAwait(false);
                newEntryIds.Add(newEntryId);
            }
            return newEntryIds;
        }

        //upload (save to database) single PersonLogEntryFile
        private async Task<string> addToPersonLogEntryFilesHelper(EFDbContext dbContext, PersonLogEntryFile record)
        {
            if (record == null) { throw new ArgumentNullException("record"); }

            record.Id = Guid.NewGuid().ToString();
            record.LastSavedByPerson_Id = userId;
            dbContext.PersonLogEntryFiles.Add(record);

            await addFileDataToDbHelper(dbContext, record).ConfigureAwait(false);

            return record.Id;
        }

        //add data from PersonLogEntryFile.fileData to PersonLogEntryFileDatas
        private async Task addFileDataToDbHelper(EFDbContext dbContext, PersonLogEntryFile record)
        {
            var dataLength = PersonLogEntryFileData.DataChunkLength;
            int noOfChunks = record.FileSize / dataLength + 1;
            var chunkDataBuffer = new byte[dataLength];

            for (int i = 0; i < noOfChunks; i++)
            {
                await record.FileData.ReadAsync(chunkDataBuffer, 0, dataLength).ConfigureAwait(false);
                dbContext.PersonLogEntryFileDatas.Add(new PersonLogEntryFileData
                {
                    Id = Guid.NewGuid().ToString(),
                    ChunkNumber = i,
                    Data = chunkDataBuffer,
                    PersonLogEntryFile_Id = record.Id
                });
            }
        }


        #endregion
    }
}
