using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;
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

        private int dataChunkLength;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryFileService(IDbContextScopeFactory contextScopeFac, string userId) 
            : base(contextScopeFac, userId) 
        {
            this.dataChunkLength = PersonLogEntryFileData.DataChunkLength;
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
                        x.AssignedToPersonLogEntry_Id == logEntryId
                    )
                    .Include(x => x.LastSavedByPerson)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }
                
        //-----------------------------------------------------------------------------------------------------------------------

        // Create and Update records given in []
        public override Task<List<string>> EditAsync(PersonLogEntryFile[] records)
        {
            throw new NotImplementedException();
        }

        //upload file - overload for PersonLogEntryFile[]
        public virtual async Task<List<String>> UploadFilesAsync(List<PersonLogEntryFile> records)
        {
            if (records == null || records.Count == 0) { throw new ArgumentNullException("records"); }

            var newEntryIds = await dbScopeHelperAsync(dbContext =>
            {
                return addPersonLogEntryFile(dbContext, records);
            })
            .ConfigureAwait(false);
            return newEntryIds;
        }

        //DownloadAsync - download log entry file from database. 
        public virtual async Task<PersonLogEntryFile> DownloadAsync(string[] fileIds)
        {
            if (fileIds == null || fileIds.Length == 0) { throw new ArgumentNullException("fileIds"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();

                if (fileIds.Length == 1) { return await getPersonLogEntryFile(dbContext, fileIds[0]); }

                return await getZipFileFromEntryFiles(dbContext, fileIds);
            }
        }

        // Delete records by their Ids
        public override async Task DeleteAsync(string[] ids)
        {
            if (ids == null || ids.Length == 0) { throw new ArgumentNullException("ids"); }

            var filesToDelete = new List<PersonLogEntryFile>();
            for (int i = 0; i < ids.Length; i++)
            {
                filesToDelete.Add(new PersonLogEntryFile { Id = ids[i] });
            }

            await dbScopeHelperAsync(dbContext =>
            {
                foreach (var file in filesToDelete)
                {
                    dbContext.PersonLogEntryFiles.Attach(file);
                }
                dbContext.PersonLogEntryFiles.RemoveRange(filesToDelete);
                return Task.FromResult(default(int));
            })
            .ConfigureAwait(false);
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //upload (save to database) PersonLogEntryFile - overload for PersonLogEntryFile[]
        private async Task<List<string>> addPersonLogEntryFile(EFDbContext dbContext, List<PersonLogEntryFile> records)
        {
            var newEntryIds = new List<string>();
            foreach (PersonLogEntryFile record in records)
            {
                var newEntryId = await addPersonLogEntryFile(dbContext, record).ConfigureAwait(false);
                newEntryIds.Add(newEntryId);
            }
            return newEntryIds;
        }

        //upload (save to database) PersonLogEntryFile
        private async Task<string> addPersonLogEntryFile(EFDbContext dbContext, PersonLogEntryFile record)
        {
            if (record == null) { throw new ArgumentNullException("record"); }
            
            record.FileName = await getNewFileNameIfDuplicate(dbContext, record.AssignedToPersonLogEntry_Id, record.FileName);
            record.Id = Guid.NewGuid().ToString();
            record.LastSavedByPerson_Id = userId;
            dbContext.PersonLogEntryFiles.Add(record);

            await addPersonLogEntryFileData(dbContext, record).ConfigureAwait(false);
            return record.Id;
        }

        //add data from PersonLogEntryFile.FileData to PersonLogEntryFileDatas
        private async Task addPersonLogEntryFileData(EFDbContext dbContext, PersonLogEntryFile record)
        {
            int lastChunkLength = record.FileSize % dataChunkLength;
            int noOfChunks = (lastChunkLength == 0) ? record.FileSize / dataChunkLength : record.FileSize / dataChunkLength + 1;

            record.FileData.Position = 0;
            for (int i = 0; i < noOfChunks; i++)
            {
                byte[] chunkDataBuffer = new byte[dataChunkLength];
                if (i == noOfChunks - 1 && lastChunkLength > 0) { chunkDataBuffer = new byte[lastChunkLength]; }
                await record.FileData.ReadAsync(chunkDataBuffer, 0, chunkDataBuffer.Length).ConfigureAwait(false);
                dbContext.PersonLogEntryFileDatas.Add(new PersonLogEntryFileData
                {
                    Id = Guid.NewGuid().ToString(),
                    ChunkNumber = i,
                    Data = chunkDataBuffer,
                    PersonLogEntryFile_Id = record.Id
                });
            }
            record.FileData.Dispose();
        }

        //getPersonLogEntryFile - get all data chunks and merge them into getPersonLogEntryFile.FileData 
        private async Task<PersonLogEntryFile> getPersonLogEntryFile(EFDbContext dbContext, string fileId)
        {
            PersonLogEntryFile record = await dbContext.PersonLogEntryFiles.FirstOrDefaultAsync(x => x.Id == fileId)
                .ConfigureAwait(false);
            if (record == null)
                { throw new DbBadRequestException( String.Format("Log Entry File with Id={0} not found", fileId)); }

            int noOfChunks = (record.FileSize % dataChunkLength == 0) ?
                record.FileSize / dataChunkLength : record.FileSize / dataChunkLength + 1;

            for (int i = 0; i < noOfChunks; i++)
            {
                PersonLogEntryFileData fileDataChunk = await dbContext.PersonLogEntryFileDatas.FirstOrDefaultAsync(x =>
                        x.PersonLogEntryFile_Id == fileId && x.ChunkNumber == i).ConfigureAwait(false);
                if (fileDataChunk == null)
                {
                    record.FileData.Dispose();
                    throw new DbBadRequestException(
                        String.Format("Data Chunk for Log Entry File {0} not found", record.FileName)); 
                }
                await record.FileData.WriteAsync(fileDataChunk.Data, 0, fileDataChunk.Data.Length);
            }
            return record;
        }

        //getZipFileFromEntryFiles - get all files from fileIds and zip them into getPersonLogEntryFile.FileData
        private async Task<PersonLogEntryFile> getZipFileFromEntryFiles(EFDbContext dbContext, string[] fileIds)
        {
            var zipFile = new PersonLogEntryFile();
            zipFile.FileName = "SDDBFiles_" + String.Format("_{0:yyyyMMdd_HHmm}", DateTime.Now) + ".zip";

            using (ZipArchive zip = new ZipArchive(zipFile.FileData, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < fileIds.Length; i++)
                {
                    PersonLogEntryFile record =  await getPersonLogEntryFile(dbContext, fileIds[i]); 
                    Stream newZipEntryStream = zip.CreateEntry(record.FileName).Open();
                    record.FileData.WriteTo(newZipEntryStream);
                    record.FileData.Dispose();
                    newZipEntryStream.Close();
                }
            }
            return zipFile;
        }

        //changeFileNameIfDuplicate - checks if file with given name exists and is assigned to log entry and returns new file name
        private async Task<string> getNewFileNameIfDuplicate(EFDbContext dbContext, string logEntryId, string currentFileName)
        {
            string[] existingFileNames = await dbContext.PersonLogEntryFiles
                    .Where(x => x.AssignedToPersonLogEntry_Id == logEntryId)
                    .Select(x => x.FileName)
                    .ToArrayAsync().ConfigureAwait(false);

            var i = 1;
            while (existingFileNames.Contains(currentFileName))
            {
                currentFileName = currentFileName.TrimEnd(new[] { ')' });
                currentFileName = currentFileName.TrimEnd(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                currentFileName = currentFileName.TrimEnd(new[] { '(' });
                currentFileName += "(" + i + ")";
                i++;
            }
            return currentFileName;
        }

        
        #endregion
    }
}
