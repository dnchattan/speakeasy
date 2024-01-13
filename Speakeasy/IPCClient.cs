using System.IO.Pipes;
using System.Security.Principal;

namespace Speakeasy;

public class IPCClient<T> : IPCSession<T>, IPipeSession<T>, IDisposable
{
	public IPCClient(string pipeName, IPCMessageSerializer<T> serializer)
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
