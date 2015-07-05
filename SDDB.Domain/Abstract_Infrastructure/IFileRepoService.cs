using SDDB.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SDDB.Domain.Abstract
{
    public interface IFileRepoService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        Task<List<FtpFileDetail>> GetAsync(string id);
        Task<byte[]> DownloadAsync(string id, string[] names);
        Task UploadAsync(string id, FileMemStream[] files);
        Task DeleteAsync(string id, string[] names);
        
    }
}
