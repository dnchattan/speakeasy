namespace Speakeasy;

public interface IPCMessageSerializer<T>
{
	Task<T?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default);
	Task<int> WriteMessageAsync(Stream stream, T message, CancellationToken cancellation = default);
}
