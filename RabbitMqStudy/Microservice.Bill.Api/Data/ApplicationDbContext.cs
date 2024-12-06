using Microservice.Bill.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Microservice.Bill.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Outbill> Outbills { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
