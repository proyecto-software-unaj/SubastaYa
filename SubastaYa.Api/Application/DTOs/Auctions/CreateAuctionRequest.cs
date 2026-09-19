using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auctions
{
    public class CreateAuctionRequest
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio base debe ser positivo.")]
        public decimal BasePrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El incremento mínimo debe ser positivo.")]
        public decimal MinimumIncrement { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
