namespace Speakeasy;

public class IPCException : Exception
{
    public IPCException(string message) : base(message)
    {
    }

    public IPCException(string message, Exception innerException) : base(message, innerException)
    {
    }
}