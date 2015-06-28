using System;

namespace SDDB.Domain.Entities
{
    public class SessionSettings
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//


        //Constructors---------------------------------------------------------------------------------------------------------//

        public SessionSettings()
        {
        }
    }

    public class FtpFileDetail
    {
        public string Name { get; set; }
        public double Size { get; set; }
        public DateTime Modified { get; set; }
    }
}
