using SDDB.Domain.Entities;
using System.Threading.Tasks;

namespace SDDB.Domain.Abstract
{
    public interface ILogger
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        //Methods--------------------------------------------------------------------------------------------------------------//

        void LogResult(DBResult result);

    }
}
