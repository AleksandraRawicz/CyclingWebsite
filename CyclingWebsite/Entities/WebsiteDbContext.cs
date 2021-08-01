using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Entities
{
    public class WebsiteDbContext:DbContext
    {
        private readonly string _connectionString = @"Server=(LocalDb)\MSSQLLocalDB;Database=CyclingDb;Trusted_Connection=true;";

        public DbSet<Tour> Tours { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }
    }
}
