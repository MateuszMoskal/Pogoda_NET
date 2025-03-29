using Microsoft.EntityFrameworkCore;
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
            string sqlServerName = "DESKTOP-H0MGPLN";
            //string sqlServerName = "Twoja nazwa SQL Servera";
            string dataBaseName = "WeatherService";

            optionsBuilder.UseSqlServer($"Server={sqlServerName};Database={dataBaseName};Trusted_Connection=true;TrustServerCertificate=true;");
        }

        public DbSet<User> Users { get; set; }

        public DbSet<City> Cities { get; set; }
    }
}
