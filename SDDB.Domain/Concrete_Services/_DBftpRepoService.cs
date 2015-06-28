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

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DbFtpRepoService(string ftpAddress = "", string ftpUserName = "", string ftpPwd = "",
            bool ftpIsSSL = false, bool ftpIsPassive = true)
        {
            this.ftpAddress = ftpAddress;
            this.ftpUserName = ftpUserName;
            this.ftpPwd = ftpPwd;
            this.ftpIsSSL = ftpIsSSL;
            this.ftpIsPassive = ftpIsPassive;
        }

        //Methods--------------------------------------------------------------------------------------------------------------//

        //get files assigned to id 

        public virtual async Task<List<FtpFilesDetail>> Get(string id = null)
        {
            if (String.IsNullOrEmpty(ftpAddress) || String.IsNullOrEmpty(id)) return new List<string>() ;
            
            var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpAddress + "/" + id);
            ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPwd);
            ftpRequest.EnableSsl = ftpIsSSL; ftpRequest.UsePassive = ftpIsPassive; ftpRequest.Timeout = 60000;
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            FtpWebResponse ftpResponse = null; var ftpRespStream = "";
            try
            {
                ftpResponse = (FtpWebResponse)(await ftpRequest.GetResponseAsync());
                ftpRespStream = await (new StreamReader(ftpResponse.GetResponseStream()).ReadToEndAsync());
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                if (!message.Contains("550") && !message.Contains("450")) throw;
            }
            if (ftpRespStream != "")
            {
                var patternName = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+\d+\s+\w{3}\s\d{1,2}\s(?:\d\d:\d\d|\d{4})\s+([\w|\W]+)";
                var patternModified = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+\d+\s+(\w{3}\s\d{1,2}\s(?:\d\d:\d\d|\d{4}))";
                var patternSize = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+(\d+)";
                var matchName = new Match(); var matchSize = new Match(); var matchModified = new Match();

                var ftpRespArray = ftpRespStream.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var ftpRespList = new List<FtpFilesDetail>();

                foreach (var line in ftpRespArray)
                {
                    matchName = Regex.Match(line, patternName);
                    matchSize = Regex.Match(line, patternSize);
                    matchModified = Regex.Match(line, patternModified);

                    if (matchName.Success && matchModified.Success && matchSize.Success)
                    {
                        var fileDetail = new FtpFilesDetail
                        {
                            Name = matchName.Value,
                            Size = double.Parse(matchSize.Value) / 1024,
                            Modified = DateTime.Parse(matchModified.Value)
                        };
                        ftpRespList.Add(fileDetail);
                    }
                    else throw new InvalidOperationException("Failed to parse ftp LIST output string");
                }

                return ftpRespStream.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
                    .Select(x =>
                    {
                        matchName = Regex.Match(x, patternName);  
                        matchSize = Regex.Match(x, patternSize);
                        matchModified = Regex.Match(x, patternModified);
      
                        if (matchName.Success && matchModified.Success && matchSize.Success)
                        {
                            return new FtpFilesDetail { 
                                Name = matchName.Value,
                                Size = double.Parse(matchSize.Value) / 1024,
                                Modified = DateTime.Parse(matchModified.Value) 
                            };
                        }
                        else throw new InvalidOperationException("Failed to parse ftp LIST output string");
                    }).ToList();


            }
            else
            {
                return new List<FtpFilesDetail>();
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------------------




        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        #endregion
    }

    
}
