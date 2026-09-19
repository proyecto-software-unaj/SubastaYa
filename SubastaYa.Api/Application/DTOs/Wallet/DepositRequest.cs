using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Wallet
{
    public class DepositRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto a depositar debe ser positivo.")]
        public decimal Amount { get; set; }
    }
}
