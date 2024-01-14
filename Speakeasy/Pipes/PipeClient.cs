using System.IO.Pipes;
using System.Security.Principal;

namespace Speakeasy.Pipes;

public class PipeClient<T> : PipeSession<T>, IPipeSession<T>, IDisposable
{
	public PipeClient(string pipeName, IMessageSerializer<T> serializer)
		: base(new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation), serializer)
	{
	}

	public async Task ConnectAsync(CancellationToken token = default)
	{
		if (Pipe is not NamedPipeClientStream client)
			throw new InvalidCastException(nameof(Pipe));

		await client.ConnectAsync(token);
		StartListening(CancellationToken.None);
	}
}
