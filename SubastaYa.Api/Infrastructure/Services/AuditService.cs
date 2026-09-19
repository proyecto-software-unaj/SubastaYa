using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context) => _context = context;

        public void Register(string entity, int entityId, string action, int? userId, object? details = null)
        {
            var log = new AuditLog
            {
                Entity = entity,
                EntityId = entityId,
                Action = action,
                UserId = userId,
                DetailsJson = details is null ? "{}" : JsonSerializer.Serialize(details),
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
        }
    }
}
