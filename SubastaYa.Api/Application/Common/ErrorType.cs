using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    public enum ErrorType
    {
        None,
        Validation,        
        NotFound,          
        Conflict,          
        InsufficientFunds  
    }
}
