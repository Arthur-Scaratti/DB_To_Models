using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnections
{
    public class MariaDBContext(DbContextOptions<MariaDBContext> options) : DbContext(options)
    {https://github.com/Arthur-Scaratti/DB_To_Models/tree/main
        public void ConfigureContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new System.Version(8, 0, 21)));
            OnConfiguring(optionsBuilder);
        }

        public DbSet<MARIAColumns> Columns {get; set; }
        public DbSet<MARIATables> Tables {get; set; }


         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MARIAColumns>().HasNoKey();
            modelBuilder.Entity<MARIATables>().HasNoKey();
        }

        // Não é mais necessário
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     // Default configuration
        //     optionsBuilder.UseMySql(stringConnection, new MySqlServerVersion(new Version(8, 0, 21)));
        // }
    }
}
