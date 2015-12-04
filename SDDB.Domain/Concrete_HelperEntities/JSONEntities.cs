using System;
using System.Collections.Generic;

namespace SDDB.Domain.Entities
{
    //ProjectJSON
    public class ProjectJSON
    {
        public string Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectAltName { get; set; }
        public string ProjectCode { get; set; }
    }

    //ProjectJSON
    public class ProjectEventJSON
    {
        public string Id { get; set; }
        public string EventName { get; set; }
    }

    //LocationJSON
    public class LocationJSON
    {
        public string Id { get; set; }
        public string LocName { get; set; }
        public string ProjectName { get; set; }
    }
    
    //PersonJSON
    public class PersonJSON 
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initials { get; set; }
    }

    //PersonActivityTypeJSON
    public class PersonActivityTypeJSON
    {
        public string Id { get; set; }
        public string ActivityTypeName { get; set; }
    }

    //PersonLogEntryJSON
    public class PersonLogEntryJSON
    {
        public string Id { get; set; }

        public DateTime LogEntryDateTime { get; set; }

        public string EnteredByPerson_Id { get; set; }
        public PersonJSON EnteredByPerson_ { get; set; }

        public string PersonActivityType_Id { get; set; }
        public PersonActivityTypeJSON PersonActivityType_ { get; set; }

        public decimal ManHours { get; set; }

        public string AssignedToProject_Id { get; set; }
        public ProjectJSON AssignedToProject_ { get; set; }

        public string AssignedToLocation_Id { get; set; }
        public LocationJSON AssignedToLocation_ { get; set; }

        public string AssignedToProjectEvent_Id { get; set; }
        public ProjectEventJSON AssignedToProjectEvent_ { get; set; }

        public string QcdByPerson_Id { get; set; }
        public PersonJSON QcdByPerson_ { get; set; }

        public DateTime? QcdDateTime { get; set; }

        public string Comments { get; set; }

        public int? PrsLogEntryFilesCount { get; set; }
        public int? PrsLogEntryAssysCount { get; set; }
        public string PrsLogEntryPersonsInitials { get; set; }
        public bool IsActive_bl { get; set; }
        
        public List<PersonJSON> PrsLogEntryPersons { get; set; }
    }



}
