﻿using System;
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
        
        public PersonLogEntrySrvController(PersonLogEntryService personLogEntryService,
            PersonLogEntryFileService personLogEntryFileService)
        {
            this.personLogEntryService = personLogEntryService;
            this.personLogEntryFileService = personLogEntryFileService;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//
        
        // POST: /PersonLogEntrySrv/GetByIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetAsync";
            var records = await personLogEntryService.GetAsync(ids, getActive, true).ConfigureAwait(false);
            return DbJsonDateTime(records);
        }

        // POST: /PersonLogEntrySrv/GetByAltIds
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetByAltIds(string[] personIds, string[] projectIds, string[] assyIds, string[] typeIds,
            DateTime? startDate, DateTime? endDate, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetByAltIdsAsync";
            var records = await personLogEntryService
                .GetByAltIdsAsync(personIds, projectIds, assyIds, typeIds, startDate, endDate, getActive, true)
                .ConfigureAwait(false);
            return DbJsonDateTime(records);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonLogEntrySrv/GetYourByIds
        [HttpPost]
        [DBSrvAuth("YourActivity_View")]
        public async Task<ActionResult> GetYourByIds(string[] ids, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetAsync";
            var records = await personLogEntryService.GetAsync(ids, getActive, false).ConfigureAwait(false);
            return DbJsonDateTime(records);
        }

        // POST: /PersonLogEntrySrv/GetYourByAltIds
        [HttpPost]
        [DBSrvAuth("YourActivity_View")]
        public async Task<ActionResult> GetYourByAltIds(string[] personIds, string[] projectIds, string[] assyIds, string[] typeIds,
            DateTime? startDate, DateTime? endDate, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetByAltIdsAsync";
            var records = await personLogEntryService
                .GetByAltIdsAsync(personIds, projectIds, assyIds, typeIds, startDate, endDate, getActive, false)
                .ConfigureAwait(false);
            return DbJsonDateTime(records);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonLogEntrySrv/GetActivitySummaries
        [HttpPost]
        [DBSrvAuth("YourActivity_View,ActivitySummary_ViewOthers")]
        public async Task<ActionResult> GetActivitySummaries(string personId,
            DateTime startDate, DateTime endDate, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetActivitySummariesAsync";
            if (!User.IsInRole("ActivitySummary_ViewOthers") && personId != UserId) { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetActivitySummariesAsync(personId, startDate, endDate, getActive)
                .ConfigureAwait(false);
            return DbJsonDate(records);
        }

        // POST: /PersonLogEntrySrv/GetLastEntrySummaries
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View")]
        public async Task<ActionResult> GetLastEntrySummaries(string activityTypeId,
            DateTime startDate, DateTime endDate, bool getActive = true)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetLastEntrySummariesAsync";
            var records = await personLogEntryService.GetLastEntrySummariesAsync(activityTypeId, startDate, endDate, getActive)
                .ConfigureAwait(false);
            return DbJsonDateTime(records);
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

        // POST: /PersonLogEntrySrv/QcLogEntries
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Qc")]
        public async Task<ActionResult> QcLogEntries(string[] ids)
        {
            ViewBag.ServiceName = "PersonLogEntryService.QcLogEntriesAsync";
            await personLogEntryService.QcLogEntriesAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // POST: /PersonLogEntrySrv/GetPrsLogEntryAssys
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssys(string[] ids)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(ids).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryAssysAsync(ids).ConfigureAwait(false);
            return DbJson(filterForJsonAssys(records));
        }

        // POST: /PersonLogEntrySrv/GetPrsLogEntryAssysNot
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Assembly_View")]
        public async Task<ActionResult> GetPrsLogEntryAssysNot(string[] ids, string locId = null)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryAssysNotAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(ids).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryAssysNotAsync(ids, locId).ConfigureAwait(false);
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

        // POST: /PersonLogEntrySrv/GetPrsLogEntryPersons
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersons(string[] ids)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(ids).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryPersonsAsync(ids).ConfigureAwait(false);
            return DbJson(filterForJsonPersons(records));
        }

        // POST: /PersonLogEntrySrv/GetPrsLogEntryPersonsNot
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View,Person_View")]
        public async Task<ActionResult> GetPrsLogEntryPersonsNot(string[] ids)
        {
            ViewBag.ServiceName = "PersonLogEntryService.GetPrsLogEntryPersonsNotAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(ids).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryService.GetPrsLogEntryPersonsNotAsync(ids).ConfigureAwait(false);
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
        public async Task<ActionResult> ListFiles(string id)
        {
            ViewBag.ServiceName = "PersonLogEntryFileService.ListAsync";
            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { id }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var records = await personLogEntryFileService.ListAsync(id).ConfigureAwait(false);
            return DbJsonDateTime(filterForJsonFiles(records));
        }

        // POST: /PersonLogEntrySrv/UploadFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit")]
        public async Task<ActionResult> UploadFiles(string id)
        {
            ViewBag.ServiceName = "fileRepoService.UploadFilesAsync";
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(new[] { id }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            var files = await returnFileListFromRequestHelper(id).ConfigureAwait(false);
            var newEntryIds = await personLogEntryFileService.UploadFilesAsync(files).ConfigureAwait(false);
            return DbJson(new { Success = "True", newEntryIds = newEntryIds });
        }

        // POST: /PersonLogEntrySrv/DownloadFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_View,YourActivity_View", false)]
        public async Task<ActionResult> DownloadFiles(long dlToken, string id, string[] fileIds)
        {
            ViewBag.ServiceName = "personLogEntryFileService.DownloadAsync";

            var tokenCookie = new HttpCookie("dlToken", dlToken.ToString());
            Response.Cookies.Set(tokenCookie);

            if (!User.IsInRole("PersonLogEntry_View") && !(await isUserActivity(new[] { id }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }

            var file = await personLogEntryFileService.DownloadAsync(fileIds).ConfigureAwait(false);
            var fileData = file.FileData.ToArray();
            file.FileData.Dispose();
            return File(fileData, file.FileType, file.FileName);
        }
        
        // POST: /PersonLogEntrySrv/DeleteFiles
        [HttpPost]
        [DBSrvAuth("PersonLogEntry_Edit,YourActivity_Edit")]
        public async Task<ActionResult> DeleteFiles(string id, string[] ids)
        {
            ViewBag.ServiceName = "personLogEntryFileService.DeleteAsync";
            if (!User.IsInRole("PersonLogEntry_Edit") && !(await isUserActivity(new[] { id }).ConfigureAwait(false)))
                { return JsonResponseForNoRights(); }
            await personLogEntryFileService.DeleteAsync(ids).ConfigureAwait(false);
            return DbJson(new { Success = "True" });
        }

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers
                
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

        //-----------------------------------------------------------------------------------------------------------------------

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
            return (await isUserActivity(ids).ConfigureAwait(false) && records.All(x => x.EnteredByPerson_Id == UserId));
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