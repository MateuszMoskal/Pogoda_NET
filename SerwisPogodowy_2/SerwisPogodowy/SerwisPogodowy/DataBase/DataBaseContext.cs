﻿using Microsoft.EntityFrameworkCore;
using SerwisPogodowy.Models;
using System.Threading.Tasks;

namespace SerwisPogodowy.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //string sqlServerName = "Twoja nazwa SQL Servera";
            string sqlServerName = "LAPTOP-3840J1OF";
            string dataBaseName = "WeatherService";

            optionsBuilder.UseSqlServer($"Server={sqlServerName};Database={dataBaseName};Trusted_Connection=true;TrustServerCertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasOne(s => s.User)
                .WithMany(u => u.Cities)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<City> Cities { get; set; }
    }
}
