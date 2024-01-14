namespace Speakeasy;

public class ApiException : Exception
{
    public ApiError Error { get; }
    public ApiException(ApiError error, string message) : base(message)
    {
        Error = error;
    }

    public ApiException(ApiError error, string message, Exception innerException) : base(message, innerException)
    {
        Error = error;
    }
}