
using System;
namespace SDDB.Domain.Abstract
{
    public interface IDbEntity
    {
        string Id { get; set; }        

        DateTime TSP { get; set; }

        string[] ModifiedProperties { get; set; }

    }
}
