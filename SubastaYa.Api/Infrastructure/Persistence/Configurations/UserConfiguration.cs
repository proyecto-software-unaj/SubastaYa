using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                   .HasMaxLength(256)
                   .IsRequired();
           
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(u => u.PasswordHash)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(u => u.RegisteredAt)
                   .IsRequired();

            builder.HasOne(u => u.Wallet)
                   .WithOne(w => w.User)
                   .HasForeignKey<Wallet>(w => w.UserId);

            builder.HasMany(u => u.Auctions)
                   .WithOne(a => a.Seller)
                   .HasForeignKey(a => a.SellerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Bids)
                   .WithOne(b => b.User)
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.AuditLogs)
                   .WithOne(l => l.User)
                   .HasForeignKey(l => l.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
