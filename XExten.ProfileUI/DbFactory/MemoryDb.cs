using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.ProfileUI.Model.EFModel;

namespace XExten.ProfileUI.DbFactory
{
    public class MemoryDb : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TraceUIDB");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TraceModel>().HasKey(t => t.Id);
            modelBuilder.Entity<TraceModel>().Property(t => t.CreateTime).HasConversion<DateTime>();
            modelBuilder.Entity<TraceModel>().Property(t => t.Id).ValueGeneratedOnAdd();
        }

        public DbSet<TraceModel> Traces { get; set; }
    }
}
