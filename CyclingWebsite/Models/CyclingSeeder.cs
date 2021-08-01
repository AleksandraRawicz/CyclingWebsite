using CyclingWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Models
{
    public class CyclingSeeder
    {
        private readonly WebsiteDbContext _context;
        public CyclingSeeder(WebsiteDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if(!_context.Roles.Any())
            {
                var roles = new List<Role>()
                {
                    new Role()
                    {
                        Name= "User"
                    },
                    new Role()
                    {
                        Name="Admin"
                    }
                };

                _context.AddRange(roles);
                _context.SaveChanges();
            }
        }
    }
}
