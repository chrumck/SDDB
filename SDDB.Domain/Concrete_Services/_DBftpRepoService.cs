using System;
using System.IO;
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
using SDDB.Domain.Abstract;

namespace SDDB.Domain.Services
{
    public class DBftpRepoService : IFileRepoService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private string ftpAddress;
        private string ftpUserName;
        private string ftpPwd;
        private bool ftpIsSSL;
        private bool ftpIsPassive;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DBftpRepoService(string ftpAddress = "", string ftpUserName = "", string ftpPwd = "",
            bool ftpIsSSL = false, bool ftpIsPassive = true)
        {
            this.ftpAddress = ftpAddress;
            this.ftpUserName = ftpUserName;
            this.ftpPwd = ftpPwd;
            this.ftpIsSSL = ftpIsSSL;
            this.ftpIsPassive = ftpIsPassive;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get files assigned to PersonLogEntry 
        
        public virtual async Task<List<string>> Get(string logEntryId = null)
        {
            if (String.IsNullOrEmpty(ftpAddress) || String.IsNullOrEmpty(logEntryId)) return new List<string>() ;
            
            var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpAddress + "/" + logEntryId);
            ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPwd);
            ftpRequest.EnableSsl = ftpIsSSL; ftpRequest.UsePassive = ftpIsPassive; ftpRequest.Timeout = 60000;
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

            var ftpResponse = (FtpWebResponse)(await ftpRequest.GetResponseAsync());

            var ftpNames = await (new StreamReader(ftpResponse.GetResponseStream()).ReadToEndAsync());

            return ftpNames.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        
        //-----------------------------------------------------------------------------------------------------------------------




        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }
}
