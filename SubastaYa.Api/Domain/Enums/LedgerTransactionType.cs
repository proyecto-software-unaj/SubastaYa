using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum LedgerTransactionType
    {
        Deposit,
        Hold,
        Release,
        Payment,
        Collection
    }
}
