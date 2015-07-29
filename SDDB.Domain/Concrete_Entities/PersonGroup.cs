using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("PersonGroups")]
    public class PersonGroup : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string PrsGroupName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string PrsGroupAltName { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }
                
        //TSP Column Wireup ---------------------------------------------------------------------------------------------------//
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //one to one

        //one to many

        //many to many

        [InverseProperty("PersonGroups")]
        public virtual ICollection<Person> GroupPersons { get; set; }

        [InverseProperty("ManagedGroups")]
        public virtual ICollection<Person> GroupManagers { get; set; }

        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonGroup()
        {
            this.GroupPersons = new HashSet<Person>();
            this.GroupManagers = new HashSet<Person>();
        }

        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }
  

}
