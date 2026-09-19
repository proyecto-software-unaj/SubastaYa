using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auctions
{
    public class PlaceBidRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser positivo.")]
        public decimal Amount { get; set; }
    }
}
