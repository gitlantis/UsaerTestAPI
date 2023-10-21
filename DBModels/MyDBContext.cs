using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace UserTestMonnitorAPI.DBModels
{
    public class MyDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();  
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();  
            modelBuilder.Entity<User>().HasKey(u => u.UserGuid).HasName("PK_Users");
        }
    }
}
