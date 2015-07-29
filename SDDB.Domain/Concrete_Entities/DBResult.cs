using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

using SDDB.Domain.Abstract;
using System.Collections.Generic;

namespace SDDB.Domain.Entities
{
    public class DBResult : IDbEntity
    {
        //EntityFramework Properties----------------------------------

        [Key]
        [StringLength(40)]
        public string Id { get; set; }

        [Required]
        public DateTime DtStart { get; set; }
        
        [Required]
        public DateTime DtEnd { get; set; }

        [Required]
        [StringLength(64)]
        public string ActionName { get; set; }

        [Required]
        [StringLength(64)]
        public string ControllerName { get; set; }

        [Required]
        [StringLength(64)]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        public string UserHostAddress { get; set; }

        [Required]
        [StringLength(64)]
        public string ServiceName { get; set; }

        [Required]
        [DefaultValue(0)]
        public HttpStatusCode StatusCode { get; set; }

        [Column(TypeName = "TEXT")]
        public string StatusDescription { get; set; }

        //TSP Column Wireup ------------------------------------
        [Column(TypeName = "timestamp")]
        public DateTime TSP
        {
            get { return this.tsp.HasValue ? this.tsp.Value : DateTime.Now; }
            set { tsp = value; }
        }
        [NotMapped]
        private DateTime? tsp;

        //EF Navigation Properties---------------------------------------------------------------------------------------------//

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DBResult()
        {
            this.StatusCode = HttpStatusCode.OK;
        }

        //Non-persistent Properties----------------------------------------

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

        [NotMapped]
        [DefaultValue(true)]
        public bool IsActive_bl { get; set; }

    }

}
