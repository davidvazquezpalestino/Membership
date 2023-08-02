using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Abstractions.Exceptions;
public class InvalidScopeActionException : Exception
{
    public InvalidScopeActionException()
    {
    }

    public InvalidScopeActionException(string message) : base(message)
    {
    }

    public InvalidScopeActionException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
