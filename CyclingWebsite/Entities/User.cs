using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string HashedPassword { get; set; }
        public virtual IEnumerable<Tour> Tours { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role {get;set;}
        public string SecurityToken { get; set; }
    }
}
