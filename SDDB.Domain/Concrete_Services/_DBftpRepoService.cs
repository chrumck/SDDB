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
using System.Text.RegularExpressions;
using System.Globalization;

namespace SDDB.Domain.Services
{
    public class DbFtpRepoService : IFileRepoService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        private string ftpAddress;
        private string ftpUserName;
        private string ftpPwd;
        private bool ftpIsSSL;
        private bool ftpIsPassive;
        private int maxDls;

        private FtpWebRequest ftpReuest;

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DbFtpRepoService(string ftpAddress = "", string ftpUserName = "", string ftpPwd = "",
            bool ftpIsSSL = false, bool ftpIsPassive = true, int maxDls = 3)
        {
            this.ftpAddress = ftpAddress;
            this.ftpUserName = ftpUserName;
            this.ftpPwd = ftpPwd;
            this.ftpIsSSL = ftpIsSSL;
            this.ftpIsPassive = ftpIsPassive;
            this.maxDls = maxDls;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get files assigned to log entry id 

        public virtual async Task<List<FtpFileDetail>> GetAsync(string id = null)
        {
            if (String.IsNullOrEmpty(ftpAddress) || String.IsNullOrEmpty(id)) return new List<FtpFileDetail>() ;
            
            var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpAddress + @"/" + id);
            ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPwd);
            ftpRequest.EnableSsl = ftpIsSSL;
            ftpRequest.UsePassive = ftpIsPassive;
            ftpRequest.Timeout = 60000;
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            var ftpRespStreamText = "";
            try
            {
                using (var ftpResponse = (FtpWebResponse)(await ftpRequest.GetResponseAsync()))
                using (var respStream = ftpResponse.GetResponseStream())
                using (var reader = new StreamReader(respStream))
                    ftpRespStreamText = await reader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                if (!message.Contains("550") && !message.Contains("450")) throw;
            }
            var ftpRespList = new List<FtpFileDetail>();

            var patternName = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+\d+\s+\w{3}\s\d{1,2}\s(?:\d\d:\d\d|\d{4})\s+([\w|\W]+)";
            var patternModified = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+\d+\s+(\w{3}\s\d{1,2}\s(?:\d\d:\d\d|\d{4}))";
            var patternSize = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+(\d+)";
            Match matchName = null; Match matchSize = null; Match matchModified = null;
            var formats = new string[] { "MMM dd HH:mm","MMM dd H:mm", "MMM d HH:mm","MMM d H:mm","MMM dd yyyy","MMM d yyyy"};

            var ftpRespArray = ftpRespStreamText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in ftpRespArray)
            {
                matchName = Regex.Match(line, patternName);
                matchSize = Regex.Match(line, patternSize);
                matchModified = Regex.Match(line, patternModified);

                if (matchName.Success && matchModified.Success && matchSize.Success)
                {
                    var fileDetail = new FtpFileDetail
                    {
                        Name = matchName.Groups[1].Value,
                        Size = int.Parse(matchSize.Groups[1].Value) / 1024,
                        Modified = DateTime.ParseExact(matchModified.Groups[1].Value,
                            formats,CultureInfo.InvariantCulture, DateTimeStyles.None)
                    };
                    ftpRespList.Add(fileDetail);
                }
                else throw new InvalidOperationException("Failed to parse ftp LIST output string");
            }
            return ftpRespList;
        }

        //public virtual async Task<byte[]> DownloadAsync(string id, string[] names)
        //{
        //    foreach (var name in names)
        //    {

        //    }
        //}

        public virtual async Task<byte[]> DownloadAsync(string id, string[] names)
        {
            var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpAddress + @"/" + id + @"/" + names[0]);
            ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPwd);
            ftpRequest.EnableSsl = ftpIsSSL;
            ftpRequest.UsePassive = ftpIsPassive;
            ftpRequest.Timeout = 60000;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            byte[] outputFile = null;
            try
            {
                using (var ftpResponse = (FtpWebResponse)(await ftpRequest.GetResponseAsync()))
                using (var respStream = ftpResponse.GetResponseStream())
                using (var memoryStream = new MemoryStream())
                {
                    await respStream.CopyToAsync(memoryStream);
                    outputFile = memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                if (!message.Contains("550") && !message.Contains("450")) throw;
            }

            return outputFile;
        }
        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }

    
}
