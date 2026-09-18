using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string IconUrl { get; set; } = string.Empty;

        public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();
    }
}
