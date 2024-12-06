using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microservice.Bill.Api.Entities;

namespace Microservice.Bill.Api.Data.Configurations
{
    public class OutbillConfiguration : IEntityTypeConfiguration<Outbill>
    {
        public void Configure(EntityTypeBuilder<Outbill> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();//取消自增

            builder.Property(p => p.ProductId).IsRequired(true);
            builder.Property(p => p.Quantity).IsRequired(true);
        }
    }
}
