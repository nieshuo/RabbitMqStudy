using Microservice.Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Inventory.Api.Data.Configurations
{
    public class WareInventoryConfiguration : IEntityTypeConfiguration<WareInventory>
    {
        public void Configure(EntityTypeBuilder<WareInventory> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p=>p.Id).ValueGeneratedNever();//取消自增
        }
    }
}
