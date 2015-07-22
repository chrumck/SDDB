using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using SDDB.Domain.Infrastructure;
using SDDB.Domain.Abstract;


namespace SDDB.Domain.Entities
{
    public class DBUser : IdentityUser, IDbEntity
    {
        //Entity Framework Properties------------------------------------------------------------------------------------------//

        [Required]
        [DBIsUnique]
        [Key, ForeignKey("Person")]
        [StringLength(40)]
        public override string Id
        { get { return base.Id; } set { base.Id = value; } }

        [DBIsUnique]
        [Required]
        public override string UserName
        { get { return base.UserName; } set { base.UserName = value; } }

        public override string Email
        { get { return base.Email; } set { base.Email = value; } }

        [Required]
        public bool LDAPAuthenticated { get; set; }

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
        public virtual Person Person { get; set; }

        //one to many

        //many to many

        //Constructors---------------------------------------------------------------------------------------------------------//

        public DBUser() : base() { }

        //Non-persisten Properties---------------------------------------------------------------------------------------------//

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string PasswordConf { get; set; }

        [NotMapped]
        public string[] ModifiedProperties { get; set; }

        [NotMapped]
        public bool IsActive_bl { get; set; }


    }
}