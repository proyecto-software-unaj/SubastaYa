using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);

            builder.HasIndex(w => w.UserId).IsUnique();

            builder.Property(w => w.TotalBalance)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(w => w.HeldBalance)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Ignore(w => w.AvailableBalance);

            builder.Property(w => w.RowVersion).IsRowVersion();

            builder.HasMany(w => w.LedgerTransactions)
                   .WithOne(t => t.Wallet)
                   .HasForeignKey(t => t.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
