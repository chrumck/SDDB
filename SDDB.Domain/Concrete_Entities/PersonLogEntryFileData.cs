using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("PersonLogEntryFileDatas")]
    public class PersonLogEntryFileData : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required(ErrorMessage = "ChunkNumber field is required")]
        public int ChunkNumber { get; set; }

        [Required(ErrorMessage = "Data field is required")]
        [Column(TypeName = "BLOB")]
        public byte[] Data { get; set; }

        [Required(ErrorMessage = "Person Log Entry File Id field is required")]
        [StringLength(40)]
        [ForeignKey("PersonLogEntryFile")]
        public string PersonLogEntryFile_Id { get; set; }

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
        public virtual PersonLogEntryFile PersonLogEntryFile { get; set; }

        //many to many

        //Constructors---------------------------------------------------------------------------------------------------------//
        
        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

        [NotMapped]
        public const int DataLength = 65535;

        [NotMapped]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }

    }
  



}
