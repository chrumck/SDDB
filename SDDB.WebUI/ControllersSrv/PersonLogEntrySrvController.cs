using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.Domain.Abstract;
using SDDB.WebUI.Infrastructure;
using System.Web;
using System.Collections.Generic;

namespace SDDB.WebUI.ControllersSrv
{
    public class PersonLogEntrySrvController : BaseSrvCtrl
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private PersonLogEntryService personLogEntryService;
        private PersonLogEntryFileService personLogEntryFileService;

        //Constructors---------------------------------------------------------------------------------------------------------//
        
        public PersonLogEntrySrvController(PersonLogEntryService personLogEntryService, PersonLogEntryFileService personLogEntryFileService)
        {
            this.personLogEntryService = personLogEntryService;
            this.personLogEntryFileService = personLogEntryFileService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//
        
        // POST: /PersonLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetAsync";
            var records = await personLogEntryService.GetAsync(ids, getActive).ConfigureAwait(false);
            if (!User.IsInRole("PersonLogEntry_View")) { records = records.Where(x => x.EnteredByPerson_Id == UserId).ToList(); }
            return DbJsonDateTime(filterForJsonFull(records));
        }

        // POST: /PersonLogEntrySrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View")]
        public async Task<ActionResult> GetByAltIds(string[] personIds, string[] projectIds, string[] assyIds, string[] typeIds,
            DateTime? startDate, DateTime? endDate, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetByAltIdsAsync";
            var records = await personLogEntryService.GetByAltIdsAsync(personIds, projectIds, assyIds, typeIds, startDate, endDate, getActive)
                .ConfigureAwait(false);
            if (!User.IsInRole("PersonLogEntry_View")) { records = records.Where(x => x.EnteredByPerson_Id == UserId).ToList(); }
            return DbJsonDateTime(filterForJsonFull(records));
        }
        
        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonLogEntrySrv/Edit
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit")]
        public async Task<ActionResult> Edit(PersonLogEntry[] records)
        {
            ViewBag.ServiceName = "PersonLogEntryService.EditAsync";
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(records).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var newEntryIds = await personLogEntryService.EditAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /PersonLogEntrySrv/Delete
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit")]
        public async Task<ActionResult> Delete(string[] ids)
        {
            ViewBag.ServiceName = "PersonLogEntryService.DeleteAsync";
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(ids).ConfigureAwait(false)))
            { return JsonResponseForNoRights(); }
            await personLogEntryService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/GetPrsLogEntryAssys
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssys(string logEntryId)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryAssysAsync(logEntryId).ConfigureAwait(false);
            return DbJson(filterForJsonAssys(records));
        }

        // GET: /PersonLogEntrySrv/GetPrsLogEntryAssysNot
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssysNot(string logEntryId, string locId = null)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysNotAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryAssysNotAsync(logEntryId, locId).ConfigureAwait(false);
            return DbJson(filterForJsonAssys(records));
        }

        // POST: /PersonLogEntrySrv/EditPrsLogEntryAssys
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit,Assembly_Edit")]
        public async Task<ActionResult> EditPrsLogEntryAssys(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "PersonLogEntryService.AddRemoveRelated";
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(ids).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            await personLogEntryService.AddRemoveRelated(ids, idsAddRem, x => x.PrsLogEntryAssemblyDbs, isAdd).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/GetPrsLogEntryPersons
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersons(string logEntryId)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryPersonsAsync(logEntryId).ConfigureAwait(false);
            return DbJson(filterForJsonPersons(records));
        }

        // GET: /PersonLogEntrySrv/GetPrsLogEntryPersonsNot
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersonsNot(string logEntryId)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsNotAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryPersonsNotAsync(UserId, logEntryId).ConfigureAwait(false);
            return DbJson(filterForJsonPersons(records));
        }

        // POST: /PersonLogEntrySrv/EditPrsLogEntryPersons
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit,Person_Edit")]
        public async Task<ActionResult> EditPrsLogEntryPersons(string[] ids, string[] idsAddRem, bool isAdd)
        {
            ViewBag.ServiceName = "PersonLogEntryService.AddRemoveRelated";
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(ids).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            await personLogEntryService.AddRemoveRelated(ids, idsAddRem, x => x.PrsLogEntryPersons, isAdd).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // GET: /PersonLogEntrySrv/ListFiles
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View")]
        public async Task<ActionResult> ListFiles(string logEntryId)
        {
            ViewBag.ServiceName = "PersonLogEntryFileService.ListAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            List<PersonLogEntryFile> records = await personLogEntryFileService.ListAsync(logEntryId).ConfigureAwait(false);
            return DbJsonDateTime(filterForJsonFiles(records));
        }

        // POST: /PersonLogEntrySrv/UploadFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit")]
        public async Task<ActionResult> UploadFiles(string logEntryId)
        {
            ViewBag.ServiceName = "fileRepoService.UploadFilesAsync";
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            List<PersonLogEntryFile> records = await returnFileListFromRequestHelper(logEntryId).ConfigureAwait(false);
            List<string> newEntryIds = await personLogEntryFileService.UploadFilesAsync(records).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /PersonLogEntrySrv/DownloadFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View", false)]
        public async Task<ActionResult> DownloadFiles(long DlToken, string logEntryId, string[] fileIds)
        {
            ViewBag.ServiceName = "personLogEntryFileService.DownloadAsync";

            var tokenCookie = new HttpCookie("DlToken", DlToken.ToString());
            Response.Cookies.Set(tokenCookie);

            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }

            PersonLogEntryFile file = await personLogEntryFileService.DownloadAsync(fileIds).ConfigureAwait(false);
            byte[] fileData = file.FileData.ToArray();
            file.FileData.Dispose();
            return File(fileData, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
        }
        
        // POST: /PersonLogEntrySrv/DeleteFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit")]
        public async Task<ActionResult> DeleteFiles(string logEntryId, string[] ids)
        {
            ViewBag.ServiceName = "personLogEntryFileService.DeleteAsync";
        
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(new[] { logEntryId }).ConfigureAwait(false)))
            { return JsonResponseForNoRights(); }

            await personLogEntryFileService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //take data and select fields for JSON - full data set
        private object filterForJsonFull(List<PersonLogEntry> records) 
        {
            return records.Select(x => new {
                x.Id,
                x.LogEntryDateTime,
                EnteredByPerson_ = new
                {
                    x.EnteredByPerson.FirstName,
                    x.EnteredByPerson.LastName,
                    x.EnteredByPerson.Initials
                },
                PersonActivityType_ = new
                {
                    x.PersonActivityType.ActivityTypeName
                },
                x.ManHours,
                AssignedToProject_ = new
                {
                    x.AssignedToProject.ProjectCode,
                    x.AssignedToProject.ProjectName
                },
                AssignedToLocation_ = new
                {
                    x.AssignedToLocation.LocName,
                    x.AssignedToLocation.LocAltName
                },
                AssignedToProjectEvent_ = new
                {
                    x.AssignedToProjectEvent.EventName
                },
                x.Comments,
                PersonLogEntryFilesCount = x.PersonLogEntryFiles.Count,
                x.IsActive_bl,
                x.EnteredByPerson_Id,
                x.PersonActivityType_Id,
                x.AssignedToProject_Id,
                x.AssignedToLocation_Id,
                x.AssignedToProjectEvent_Id
            }).ToList();
        }

        //filterForJsonPersons - filter data from service to be passed as response
        private object filterForJsonPersons(List<Person> records)
        {
            return records
                .Select(x => new {
                    x.Id,
                    x.LastName,
                    x.FirstName,
                    x.Initials
                });
        }

        //filterForJsonAssys - filter data from service to be passed as response
        private object filterForJsonAssys(List<AssemblyDb> records)
        {
            return records
                .Select(x => new {
                    x.Id,
                    x.AssyName,
                    x.AssyAltName
                });
        }

        //filterForJsonPersons - filter data from service to be passed as response
        private object filterForJsonFiles(List<PersonLogEntryFile> records)
        {
            return records
                .Select(x => new
                {
                    x.Id,
                    x.FileName,
                    x.FileType,
                    FileSize = x.FileSize / 1024,
                    x.FileDateTime,
                    LastSavedByPerson_ = new
                    {
                        x.LastSavedByPerson.FirstName,
                        x.LastSavedByPerson.LastName,
                        x.LastSavedByPerson.Initials
                    }
                });
        }

        //check if person log entry belongs to user - overload for single id
        private async Task<bool> isUserActivity(string id)
        {
            var ids = new[] { id };
            return await isUserActivity(ids).ConfigureAwait(false);
        }
        
        //check if person log entry belongs to user
        private async Task<bool> isUserActivity(string[] ids)
        {
            var dbEntries = await personLogEntryService.GetAllAsync(ids).ConfigureAwait(false);
            if (dbEntries.Count() == 0) { return true; }
            return dbEntries.All(x => x.EnteredByPerson_Id == UserId);
        }

        //check if person log entry belongs to user - overload for PersonLogEntry[]
        private async Task<bool> isUserActivity(PersonLogEntry[] records)
        {
            var ids = records.Select(x => x.Id).ToArray();
            return (await isUserActivity(ids) && records.All(x => x.EnteredByPerson_Id == UserId));
        }

        //set viewbag, response http code and return JSON if user does not have PersonLogEntry_ rights
        private JsonResult JsonResponseForNoRights()
        {
            ViewBag.StatusCode = HttpStatusCode.BadRequest;
            ViewBag.StatusDescription = "You have insufficient privileges to edit or view other person's entries.";
            Response.StatusCode = (int)ViewBag.StatusCode;
            return DbJson(new { Success = "False", responseText = ViewBag.StatusDescription }); 
        }

        //returnFileListFromRequestHelper
        private async Task<List<PersonLogEntryFile>> returnFileListFromRequestHelper(string logEntryId)
        {
            var filesList = new List<PersonLogEntryFile>();
            foreach (string fileName in Request.Files)
            {
                var fileContent = Request.Files[fileName];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var personLogEntryFile = new PersonLogEntryFile();
                    personLogEntryFile.FileName = fileContent.FileName;
                    personLogEntryFile.FileType = fileContent.ContentType;
                    personLogEntryFile.FileDateTime = DateTime.Now;
                    personLogEntryFile.AssignedToPersonLogEntry_Id = logEntryId;
                    await fileContent.InputStream.CopyToAsync(personLogEntryFile.FileData).ConfigureAwait(false);
                    personLogEntryFile.FileSize = (int)personLogEntryFile.FileData.Length;
                    filesList.Add(personLogEntryFile);
                }
            }
            return filesList;
        }

        




        #endregion
    }
}