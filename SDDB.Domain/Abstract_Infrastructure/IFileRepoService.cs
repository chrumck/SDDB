using SDDB.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDDB.Domain.Abstract
{
    public interface IFileRepoService
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        Task<List<string>> Get(string logEntryId);
        
    }
}
