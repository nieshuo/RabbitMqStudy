using Microservice.Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Inventory.Api.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p=>p.Id).ValueGeneratedNever();//取消自增

            builder.Property(p => p.Name).HasMaxLength(256).IsRequired(true);
            builder.Property(p => p.Description).HasMaxLength(512).IsRequired(false);
        }
    }
}
