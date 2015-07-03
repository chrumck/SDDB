using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;

using SDDB.Domain.Entities;
using SDDB.Domain.Abstract;

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
                using (var ftpResponse = (FtpWebResponse)(await ftpRequest.GetResponseAsync().ConfigureAwait(false)))
                using (var respStream = ftpResponse.GetResponseStream())
                using (var reader = new StreamReader(respStream))
                    ftpRespStreamText = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                if (!message.Contains("550") && !message.Contains("450")) throw;
            }
            var ftpRespList = new List<FtpFileDetail>();

            var patternName = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+\d+\s+\w{3}\s+\d{1,2}\s+(?:\d\d:\d\d|\d{4})\s+([\w|\W]+)";
            var patternModified = @"-[rwx-]{9}\s+\d+\s+\w+\s+\w+\s+\d+\s+(\w{3}\s+\d{1,2}\s+(?:\d\d:\d\d|\d{4}))";
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
                        Modified = DateTime.ParseExact(Regex.Replace(matchModified.Groups[1].Value,@"\s+"," "),
                            formats,CultureInfo.InvariantCulture, DateTimeStyles.None)
                    };
                    ftpRespList.Add(fileDetail);
                }
                else throw new InvalidOperationException("Failed to parse ftp LIST output string");
            }
            return ftpRespList;
        }

        //DownloadAsync - download log entry files from FTP. Return file if only one or .zip if many
        public virtual async Task<byte[]> DownloadAsync(string id, string[] names)
        {
            switch (names.Length)
            {
                case 0:
                    return null;
                case 1:
                    using (var fileMemStream = await getfileMemStreamAsync(id, names[0]).ConfigureAwait(false))
                    {
                        return fileMemStream.ToArray();
                    }
                default:
                    using (var zipStream = new MemoryStream())
                    {
                        using (var zip = new ZipArchive(zipStream,ZipArchiveMode.Create,true))
                        {
                            var i = 0; var downloadedCount = 0; var dlTasks = new List<Task<fileMemStream>>();
                            while (downloadedCount < names.Length)
                            {
                                if (dlTasks.Count <= maxDls && i < names.Length)
                                {
                                    var dlTask = getfileMemStreamAsync(id, names[i]);
                                    dlTasks.Add(dlTask);
                                    i++; continue;
                                }

                                var finishedTask = await Task.WhenAny(dlTasks).ConfigureAwait(false);
                                dlTasks.Remove(finishedTask);
                                
                                using (var fileMemStream = await finishedTask.ConfigureAwait(false))
                                {
                                    var newZipEntryStream = zip.CreateEntry(fileMemStream.FileName).Open();
                                    fileMemStream.WriteTo(newZipEntryStream);
                                    newZipEntryStream.Close();
                                }                                                                                                
                                
                                downloadedCount++;
                            }   
                        }
                        return zipStream.ToArray(); 
                    }
            }
        }
        

        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //gets single file through ftp - async
        private async Task<fileMemStream> getfileMemStreamAsync(string id, string name)
        {
            var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpAddress + @"/" + id + @"/" + name);
            ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPwd);
            ftpRequest.EnableSsl = ftpIsSSL;
            ftpRequest.UsePassive = ftpIsPassive;
            ftpRequest.Timeout = 60000;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            var memoryStream = new fileMemStream();

            try
            {
                using (var ftpResponse = (FtpWebResponse)(await ftpRequest.GetResponseAsync().ConfigureAwait(false)))
                using (var respStream = ftpResponse.GetResponseStream())
                {
                    await respStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                if (!message.Contains("550") && !message.Contains("450")) throw;
            }

            memoryStream.FileName = name;
            return memoryStream;
        }

        private class fileMemStream : MemoryStream
        {
            public string FileName { get; set; }
        }

        ////gets single file through ftp - async
        //private async Task<byte[]> getFtpFile(string id, string name)
        //{
        //    var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpAddress + @"/" + id + @"/" + name);
        //    ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPwd);
        //    ftpRequest.EnableSsl = ftpIsSSL;
        //    ftpRequest.UsePassive = ftpIsPassive;
        //    ftpRequest.Timeout = 60000;
        //    ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

        //    byte[] outputFile = null;
        //    try
        //    {
        //        using (var ftpResponse = (FtpWebResponse)(await ftpRequest.GetResponseAsync().ConfigureAwait(false)))
        //        using (var respStream = ftpResponse.GetResponseStream())
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await respStream.CopyToAsync(memoryStream).ConfigureAwait(false);
        //            outputFile = memoryStream.ToArray();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        var message = e.GetBaseException().Message;
        //        if (!message.Contains("550") && !message.Contains("450")) throw;
        //    }

        //    return outputFile;
        //}



        #endregion
    }

    
}
