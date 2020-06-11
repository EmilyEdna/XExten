using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XExten.ProfileTest.Controllers
{
    public class SugarContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=106.58.174.64;Initial Catalog=ZSY;uid=zsy;pwd=zsy123456!@#$!");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WarnInfo>().HasNoKey();
        }

        public DbSet<WarnInfo> warnInfos { get; set; }
    }
}
