using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Models
{
    public class UserEditDto
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
