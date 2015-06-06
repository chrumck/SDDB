using Microsoft.AspNet.Identity.EntityFramework;

namespace SDDB.Domain.Entities
{
       public class DBRole : IdentityRole
    {
        public DBRole() : base() { }
        public DBRole(string name) : base(name) { }
    }
}