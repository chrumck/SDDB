using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("ComponentModels")]
    public class ComponentModel : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string CompModelName { get; set; }

        [DBIsUnique] [Index(IsUnique = true)]
        [StringLength(255)]
        public string CompModelAltName { get; set; }
                
        public MAttrTypes Attr01Type { get; set; }

        [StringLength(64)]
        public string Attr01Desc { get; set; }

        public MAttrTypes Attr02Type { get; set; }

        [StringLength(64)]
        public string Attr02Desc { get; set; }

        public MAttrTypes Attr03Type { get; set; }

        [StringLength(64)]
        public string Attr03Desc { get; set; }

        public MAttrTypes Attr04Type { get; set; }

        [StringLength(64)]
        public string Attr04Desc { get; set; }

        public MAttrTypes Attr05Type { get; set; }

        [StringLength(64)]
        public string Attr05Desc { get; set; }

        public MAttrTypes Attr06Type { get; set; }

        [StringLength(64)]
        public string Attr06Desc { get; set; }

        public MAttrTypes Attr07Type { get; set; }

        [StringLength(64)]
        public string Attr07Desc { get; set; }

        public MAttrTypes Attr08Type { get; set; }

        [StringLength(64)]
        public string Attr08Desc { get; set; }

        public MAttrTypes Attr09Type { get; set; }

        [StringLength(64)]
        public string Attr09Desc { get; set; }

        public MAttrTypes Attr10Type { get; set; }

        [StringLength(64)]
        public string Attr10Desc { get; set; }

        public MAttrTypes Attr11Type { get; set; }

        [StringLength(64)]
        public string Attr11Desc { get; set; }

        public MAttrTypes Attr12Type { get; set; }

        [StringLength(64)]
        public string Attr12Desc { get; set; }

        public MAttrTypes Attr13Type { get; set; }

        [StringLength(64)]
        public string Attr13Desc { get; set; }

        public MAttrTypes Attr14Type { get; set; }

        [StringLength(64)]
        public string Attr14Desc { get; set; }

        public MAttrTypes Attr15Type { get; set; }

        [StringLength(64)]
        public string Attr15Desc { get; set; }

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


        //Constructors---------------------------------------------------------------------------------------------------------//

        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

    }

}
