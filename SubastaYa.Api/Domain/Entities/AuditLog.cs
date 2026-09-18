using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string Entity { get; set; } = string.Empty;

        public int EntityId { get; set; }

        public string Action { get; set; } = string.Empty;

        public int? UserId { get; set; }

        public string DetailsJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
