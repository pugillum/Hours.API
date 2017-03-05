using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoursApi.Entities
{
    public class HoursApiContext : DbContext
    {
        public HoursApiContext(DbContextOptions<HoursApiContext> options)
           : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Project> Projects { get; set; }

        
    }
}
