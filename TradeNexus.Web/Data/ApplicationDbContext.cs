using Microsoft.EntityFrameworkCore;
using TradeNexus.Web.Models;

namespace TradeNexus.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SubBroker> SubBrokers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Trade> Trades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Trade model to TRADE_NEXUS_MASTER table
            modelBuilder.Entity<Trade>()
                .ToTable("TRADE_NEXUS_MASTER")
                .HasKey(t => t.TradeId);

            modelBuilder.Entity<Trade>()
                .Property(t => t.TradeId)
                .HasColumnName("TradeId");
        }
    }
}