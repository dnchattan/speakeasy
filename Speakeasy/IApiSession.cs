namespace Speakeasy;

public interface IApiSession<T>
{
	string Id { get; }
	bool Connected { get; }
	Task SendAsync(T message, CancellationToken token = default);
}
