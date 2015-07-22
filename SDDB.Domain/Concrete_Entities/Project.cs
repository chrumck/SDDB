using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("Projects")]
    public class Project : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string ProjectName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string ProjectAltName { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string ProjectCode { get; set; }

        [Required( ErrorMessage = "Project Manager field is required")]
        [StringLength(40)]
        [ForeignKey("ProjectManager")]
        public string ProjectManager_Id { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Comments { get; set; }
        
        [Required]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }
        
        //TSP Column Wireup ---------------------------------------------------------------------------------------------------//
        [ConcurrencyCheck]
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //one to many
        public virtual Person ProjectManager { get; set; }

        [InverseProperty("AssignedToProject")]
        public virtual ICollection<Document> ProjectDocuments { get; set; }

        [InverseProperty("AssignedToProject")]
        public virtual ICollection<Location> ProjectLocations { get; set; }

        //many to many
        [InverseProperty("PersonProjects")]
        public virtual ICollection<Person> ProjectPersons { get; set; }

        //Constructors---------------------------------------------------------------------------------------------------------//
        
        public Project()
        {
            this.ProjectDocuments = new HashSet<Document>();
            this.ProjectLocations = new HashSet<Location>();
            
            this.ProjectPersons = new HashSet<Person>();
        }

        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }


}
