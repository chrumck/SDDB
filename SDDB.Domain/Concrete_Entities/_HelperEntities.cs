using System;
using System.IO;

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

    //MemoryStream + file metadata
    public class FileMemStream : MemoryStream
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Int64 FileSize { get; set; }
        public DateTime FileDateTime { get; set; }

    }
}
