using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Abstractions.Exceptions;
public class InvalidCodeVerifierException : Exception
{
    public InvalidCodeVerifierException()
    {
    }

    public InvalidCodeVerifierException(string message) : base(message)
    {
    }

    public InvalidCodeVerifierException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
