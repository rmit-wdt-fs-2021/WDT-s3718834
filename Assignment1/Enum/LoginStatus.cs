using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment1
{
    public enum LoginStatus
    {
        Initial,
        IncorrectID,
        IncorrectPassword,
        MaxAttempts,
        Success
    }
}
