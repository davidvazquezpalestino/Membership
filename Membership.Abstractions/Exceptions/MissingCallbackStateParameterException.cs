namespace Membership.Abstractions.Exceptions;
public class MissingCallbackStateParameterException : Exception
{
    public MissingCallbackStateParameterException()
    {
    }

    public MissingCallbackStateParameterException(string message) : base(message)
    {
    }

    public MissingCallbackStateParameterException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
