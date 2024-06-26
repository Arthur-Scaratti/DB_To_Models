using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnections
{
    public class OracleDBContext(DbContextOptions<OracleDBContext> options) : DbContext(options)
    {

        public void ConfigureContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseOracle(connectionString);
            OnConfiguring(optionsBuilder);
        }

        public DbSet<ORACLE_ALL_TABLES> ALL_TABLES { get; set; }
        public DbSet<ORACLE_ALL_TAB_COLUMNS> ALL_TAB_COLUMNS { get; set; }
        public DbSet<ORACLE_ALL_CONS_COLUMNS> ALL_CONS_COLUMNS { get; set; }
        public DbSet<ORACLE_ALL_CONSTRAINTS> ALL_CONSTRAINTS { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ORACLE_ALL_TABLES>().HasNoKey();
            modelBuilder.Entity<ORACLE_ALL_TAB_COLUMNS>().HasNoKey();
            modelBuilder.Entity<ORACLE_ALL_CONS_COLUMNS>().HasNoKey();
            modelBuilder.Entity<ORACLE_ALL_CONSTRAINTS>().HasNoKey();
        }

        // Não é mais necessário
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     // Default configuration
        //     optionsBuilder.UseOracle("User Id=system;Password=oracle;Data Source=localhost:1521/ORCL");
        // }
    }
}
