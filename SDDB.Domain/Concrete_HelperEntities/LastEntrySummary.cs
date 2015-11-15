using System;
using System.Collections.Generic;

namespace SDDB.Domain.Entities
{
    public class LastEntrySummary
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public DateTime SummaryDay { get; private set; }
        public string ProjectId { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectCode { get; private set; }
        public int TotalEntries { get; private set; }
        public DateTime LastEntryDateTime { get; private set; }
        public string LastEntryPersonInitials { get; private set; }
        public string LastEntryComments { get; private set; }
        
        //Constructors---------------------------------------------------------------------------------------------------------//

        public LastEntrySummary(DateTime SummaryDay, string ProjectId, string ProjectName, string ProjectCode, 
            int TotalEntries, DateTime LastEntryDateTime, string LastEntryPersonInitials, string LastEntryComments)
        {
            this.SummaryDay = SummaryDay;
            this.ProjectId = ProjectId;
            this.ProjectName = ProjectName;
            this.ProjectCode = ProjectCode;
            this.TotalEntries = TotalEntries;
            this.LastEntryDateTime = LastEntryDateTime;
            this.LastEntryPersonInitials = LastEntryPersonInitials;
            this.LastEntryComments = LastEntryComments;
        }
    }
}
