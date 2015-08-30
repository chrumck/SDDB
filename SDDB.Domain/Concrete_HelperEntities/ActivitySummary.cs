using System;
using System.Collections.Generic;

namespace SDDB.Domain.Entities
{
    public class ActivitySummary 
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public string PersonId { get; private set; }
        public DateTime SummaryDay { get; private set; }
        public decimal TotalManHours { get; private set; }
        public IReadOnlyCollection<ActivitySummaryDetail> SummaryDetails { get; private set; }
        
        //Constructors---------------------------------------------------------------------------------------------------------//

        public ActivitySummary(string PersonId, DateTime SummaryDay, decimal TotalManHours,
            IReadOnlyCollection<ActivitySummaryDetail> SummaryDetails)
        {
            this.PersonId = PersonId;
            this.SummaryDay = SummaryDay;
            this.TotalManHours = TotalManHours;
            this.SummaryDetails = SummaryDetails;
        }
                
    }

    public class ActivitySummaryDetail
    {
        //Fields and Properties------------------------------------------------------------------------------------------------//

        public string ProjectId { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectCode { get; private set; }
        public decimal ManHours { get; private set; }

        //Constructors---------------------------------------------------------------------------------------------------------//

        public ActivitySummaryDetail(string ProjectId, string ProjectName, string ProjectCode, decimal ManHours)
        {
            this.ProjectId = ProjectId;
            this.ProjectName = ProjectName;
            this.ProjectCode = ProjectCode;
            this.ManHours = ManHours;
        }
    }


}
