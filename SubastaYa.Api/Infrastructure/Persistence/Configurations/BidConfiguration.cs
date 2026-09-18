using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class BidConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(b => b.BidDate)
                   .IsRequired();

            builder.HasIndex(b => new { b.AuctionId, b.BidDate });

        }
    }
}
