using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Mehdime.Entity;

using SDDB.Domain.Entities;
using SDDB.Domain.DbContexts;

namespace SDDB.Domain.Services
{
    public class PersonLogEntryFileService :BaseDbService<PersonLogEntryFile>
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private int dataChunkLength;
        private List<string> newFileNames;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryFileService(IDbContextScopeFactory contextScopeFac, string userId) 
            : base(contextScopeFac, userId) 
        {
            this.dataChunkLength = PersonLogEntryFileData.DataChunkLength;
            this.newFileNames = new List<string>();
        }

        //Methods--------------------------------------------------------------------------------------------------------------//


        //list files by PersonLogEntry_Id 
        public virtual async Task<List<PersonLogEntryFile>> ListAsync(string id)
        {
            if (String.IsNullOrEmpty(id)) { throw new ArgumentNullException("id"); }

            using (var dbContextScope = contextScopeFac.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<EFDbContext>();
                
                var records = await dbContext.PersonLogEntryFiles
                    .Where(x => 
                        x.AssignedToPersonLogEntry.AssignedToProject.ProjectPersons.Any(y => y.Id == userId) &&
                        x.AssignedToPersonLogEntry_Id == id
                    )
                    .Include(x => x.LastSavedByPerson)
                    .ToListAsync().ConfigureAwait(false);
                return records;
            }
        }
                
        //-----------------------------------------------------------------------------------------------------------------------

        //Create and Update records given in [] - same as BaseDbService

        //Upload PersonLogEntryFile[]
        public virtual async Task<List<String>> UploadFilesAsync(List<PersonLogEntryFile> files)
        {
            if (files == null || files.Count == 0) { throw new ArgumentNullException("files"); }

            var newEntryIds = await dbScopeHelperAsync(dbContext =>
            {
                return addPersonLogEntryFile(dbContext, files);
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

                if (fileIds.Length == 1) 
                    { return await getPersonLogEntryFile(dbContext, fileIds[0]).ConfigureAwait(false); }

                return await getZipFileFromEntryFiles(dbContext, fileIds).ConfigureAwait(false);
            }
        }

        // Delete records by their Ids
        // same as BaseDbService - see overriden deleteHelperAsync(dbContext, ids);
        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //upload (save to database) PersonLogEntryFile - overload for PersonLogEntryFile[]
        private async Task<List<string>> addPersonLogEntryFile(EFDbContext dbContext, List<PersonLogEntryFile> files)
        {
            var newEntryIds = new List<string>();
            foreach (var file in files)
            {
                var newEntryId = await addPersonLogEntryFile(dbContext, file).ConfigureAwait(false);
                newEntryIds.Add(newEntryId);
            }
            return newEntryIds;
        }

        //upload (save to database) PersonLogEntryFile
        private async Task<string> addPersonLogEntryFile(EFDbContext dbContext, PersonLogEntryFile file)
        {
            if (file == null) { throw new ArgumentNullException("file"); }
            
            file.FileName = await getNewFileNameIfDuplicate(dbContext,
                file.AssignedToPersonLogEntry_Id, file.FileName).ConfigureAwait(false);
            file.Id = Guid.NewGuid().ToString();
            file.LastSavedByPerson_Id = userId;
            dbContext.PersonLogEntryFiles.Add(file);

            await addPersonLogEntryFileData(dbContext, file).ConfigureAwait(false);
            return file.Id;
        }

        //add data from PersonLogEntryFile.FileData to PersonLogEntryFileDatas
        private async Task addPersonLogEntryFileData(EFDbContext dbContext, PersonLogEntryFile file)
        {
            int lastChunkLength = file.FileSize % dataChunkLength;
            int noOfChunks = (lastChunkLength == 0) ? file.FileSize / dataChunkLength : file.FileSize / dataChunkLength + 1;

            file.FileData.Position = 0;
            for (int i = 0; i < noOfChunks; i++)
            {
                var chunkDataBuffer = new byte[dataChunkLength];
                if (i == noOfChunks - 1 && lastChunkLength > 0) { chunkDataBuffer = new byte[lastChunkLength]; }
                await file.FileData.ReadAsync(chunkDataBuffer, 0, chunkDataBuffer.Length).ConfigureAwait(false);
                dbContext.PersonLogEntryFileDatas.Add(new PersonLogEntryFileData
                {
                    Id = Guid.NewGuid().ToString(),
                    ChunkNumber = i,
                    Data = chunkDataBuffer,
                    PersonLogEntryFile_Id = file.Id
                });
            }
            file.FileData.Dispose();
        }

        //getPersonLogEntryFile - get all data chunks and merge them into getPersonLogEntryFile.FileData 
        private async Task<PersonLogEntryFile> getPersonLogEntryFile(EFDbContext dbContext, string fileId)
        {
            var file = await dbContext.PersonLogEntryFiles.FirstOrDefaultAsync(x => x.Id == fileId)
                .ConfigureAwait(false);
            if (file == null)
                { throw new DbBadRequestException( String.Format("Log Entry File with Id={0} not found", fileId)); }

            int noOfChunks = (file.FileSize % dataChunkLength == 0) ?
                file.FileSize / dataChunkLength : file.FileSize / dataChunkLength + 1;

            for (int i = 0; i < noOfChunks; i++)
            {
                var fileDataChunk = await dbContext.PersonLogEntryFileDatas.FirstOrDefaultAsync(x =>
                        x.PersonLogEntryFile_Id == fileId && x.ChunkNumber == i).ConfigureAwait(false);
                if (fileDataChunk == null)
                {
                    file.FileData.Dispose();
                    throw new DbBadRequestException(
                        String.Format("Data Chunk for Log Entry File {0} not found", file.FileName)); 
                }
                await file.FileData.WriteAsync(fileDataChunk.Data, 0, fileDataChunk.Data.Length).ConfigureAwait(false);
            }
            return file;
        }

        //getZipFileFromEntryFiles - get all files from fileIds and zip them into getPersonLogEntryFile.FileData
        private async Task<PersonLogEntryFile> getZipFileFromEntryFiles(EFDbContext dbContext, string[] fileIds)
        {
            var zipFile = new PersonLogEntryFile();
            zipFile.FileType = System.Net.Mime.MediaTypeNames.Application.Octet;
            zipFile.FileName = "SDDBFiles_" + String.Format("_{0:yyyyMMdd_HHmm}", DateTime.Now) + ".zip";

            using (var zip = new ZipArchive(zipFile.FileData, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < fileIds.Length; i++)
                {
                    var file =  await getPersonLogEntryFile(dbContext, fileIds[i]).ConfigureAwait(false); 
                    Stream newZipEntryStream = zip.CreateEntry(file.FileName).Open();
                    file.FileData.WriteTo(newZipEntryStream);
                    file.FileData.Dispose();
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
            while (existingFileNames.Contains(currentFileName) || newFileNames.Contains(currentFileName))
            {
                var extension = System.IO.Path.GetExtension(currentFileName);
                var currentFileNameWoExt = currentFileName.Substring(0, currentFileName.Length - extension.Length);

                if (currentFileNameWoExt.Substring(currentFileNameWoExt.Length - 1) == ")")
                {
                    currentFileNameWoExt = currentFileNameWoExt.TrimEnd(new[] { ')' });
                    currentFileNameWoExt = currentFileNameWoExt.TrimEnd
                        (new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                    currentFileNameWoExt = currentFileNameWoExt.TrimEnd(new[] { '(' });
                }

                currentFileName = currentFileNameWoExt + "(" + i + ")" + extension;
                i++;
            }
            newFileNames.Add(currentFileName);
            return currentFileName;
        }

        //helper -  deleting db entries - overriden fron BaseDbService
        protected override async Task deleteHelperAsync(EFDbContext dbContext, string[] ids)
        {
            var dbEntries = await getEntriesFromContextHelperAsync<PersonLogEntryFile>(dbContext, ids).ConfigureAwait(false);
            dbContext.PersonLogEntryFiles.RemoveRange(dbEntries);
        }

        
        #endregion
    }
}
