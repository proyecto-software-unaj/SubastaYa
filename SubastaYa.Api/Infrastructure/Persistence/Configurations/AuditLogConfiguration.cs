using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Entity)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(l => l.Action)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(l => l.DetailsJson)
                   .HasColumnType("nvarchar(max)");

            builder.Property(l => l.CreatedAt)
                   .IsRequired();

            builder.HasIndex(l => new { l.Entity, l.EntityId });
            builder.HasIndex(l => l.CreatedAt);

            builder.HasOne(l => l.User)
                   .WithMany(u => u.AuditLogs)
                   .HasForeignKey(l => l.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
