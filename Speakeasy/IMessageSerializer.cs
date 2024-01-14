namespace Speakeasy;

public interface IMessageSerializer<T>
{
	Task<T?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default);
	Task<int> WriteMessageAsync(Stream stream, T message, CancellationToken cancellation = default);
}
