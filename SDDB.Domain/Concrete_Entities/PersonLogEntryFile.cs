using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

using SDDB.Domain.Abstract;
using SDDB.Domain.Infrastructure;

namespace SDDB.Domain.Entities
{
    [Table("PersonLogEntryFiles")]
    public class PersonLogEntryFile : IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required(ErrorMessage = "File Name field is required")]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required(ErrorMessage = "File Type field is required")]
        [StringLength(255)]
        public string FileType { get; set; }

        [Required(ErrorMessage = "File Size field is required")]
        public int FileSize { get; set; }

        [Required(ErrorMessage = "File DateTime field is required")]
        [DBIsDateTimeISO] 
        public DateTime FileDateTime { get; set; }

        [Required(ErrorMessage = "Person Log Entry Id field is required")]
        [StringLength(40)]
        [ForeignKey("AssignedToPersonLogEntry")]
        public string AssignedToPersonLogEntry_Id { get; set; }

        [Column(TypeName = "text")] [StringLength(65535)]
        public string Comments { get; set; }
    
        //TSP Column Wireup ----------------------------------------------------------------
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //LastSavedByPerson_Id -------------------------------------------------------------
        //[Required(ErrorMessage = "LastSavedByPerson Id field is required")]
        [StringLength(40)]
        [ForeignKey("LastSavedByPerson")]
        public string LastSavedByPerson_Id { get; set; }
        //Navigation Property
        public virtual Person LastSavedByPerson { get; set; }

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //one to one
        
        //one to many
        public virtual PersonLogEntry AssignedToPersonLogEntry { get; set; }

        [InverseProperty("PersonLogEntryFile")]
        public virtual ICollection<PersonLogEntryFileData> PersonLogEntryFileDatas { get; set; }

        //many to many


        //Non-persistent Properties--------------------------------------------------------------------------------------------//

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

        [NotMapped]
        public MemoryStream FileData { get; set; }

        [NotMapped]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }


        //Constructors---------------------------------------------------------------------------------------------------------//

        public PersonLogEntryFile()
        {
            this.PersonLogEntryFileDatas = new HashSet<PersonLogEntryFileData>();
            this.FileData = new MemoryStream();
        }
        
    }
  



}
