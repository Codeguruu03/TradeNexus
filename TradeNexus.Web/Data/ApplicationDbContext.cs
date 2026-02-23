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
    }
}