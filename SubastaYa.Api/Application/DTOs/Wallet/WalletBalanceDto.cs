using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Wallet
{
    public class WalletBalanceDto
    {
        public decimal TotalBalance { get; set; }
        public decimal HeldBalance { get; set; }
        public decimal AvailableBalance { get; set; }
    }
}
