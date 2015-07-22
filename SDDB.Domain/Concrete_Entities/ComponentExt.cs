using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("ComponentExts")]
    public class ComponentExt : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Required]
        [DBIsUnique]
        [Key, ForeignKey("Component")]
        [StringLength(40)]
        public string Id { get; set; }

        [StringLength(255)]
        public string Attr01 { get; set; }

        [StringLength(255)]
        public string Attr02 { get; set; }

        [StringLength(255)]
        public string Attr03 { get; set; }

        [StringLength(255)]
        public string Attr04 { get; set; }

        [StringLength(255)]
        public string Attr05 { get; set; }

        [StringLength(255)]
        public string Attr06 { get; set; }

        [StringLength(255)]
        public string Attr07 { get; set; }

        [StringLength(255)]
        public string Attr08 { get; set; }

        [StringLength(255)]
        public string Attr09 { get; set; }

        [StringLength(255)]
        public string Attr10 { get; set; }

        [StringLength(255)]
        public string Attr11 { get; set; }

        [StringLength(255)]
        public string Attr12 { get; set; }

        [StringLength(255)]
        public string Attr13 { get; set; }

        [StringLength(255)]
        public string Attr14 { get; set; }

        [StringLength(255)]
        public string Attr15 { get; set; }

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

        //one to one
        public virtual Component Component { get; set; }

        //one to many


        //many to many
        

        //Constructors---------------------------------------------------------------------------------------------------------//

        
        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

        [NotMapped]
        public bool IsActive_bl { get; set; }

    }
  



}
