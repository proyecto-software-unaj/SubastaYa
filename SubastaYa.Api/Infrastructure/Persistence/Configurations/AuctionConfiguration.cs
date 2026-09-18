using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(a => a.Description)
                   .HasMaxLength(2000);

            builder.Property(a => a.ImageUrl)
                   .HasMaxLength(500);

            builder.Property(a => a.BasePrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.MinimumIncrement)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(a => a.RowVersion)
                   .IsRowVersion();

            builder.HasOne(a => a.Seller)
                   .WithMany(u => u.Auctions)
                   .HasForeignKey(a => a.SellerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Category)
                   .WithMany(c => c.Auctions)
                   .HasForeignKey(a => a.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.Status);
            builder.HasIndex(a => a.EndDate);
            builder.HasIndex(a => a.CategoryId);
        }
    }
}
