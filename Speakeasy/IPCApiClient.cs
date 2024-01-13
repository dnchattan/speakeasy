using Speakeasy.Messages;

namespace Speakeasy;

public class IPCApiClient : ApiClientBase
{
	private static readonly ApiMessageSerializer Serializer = new();
	private readonly IPCClient<SocketMessage> Pipe;
	public IPCApiClient(string pipeName) : base()
	{
		Pipe = new(pipeName, Serializer);
		Pipe.MessageReceived += (s, e) => HandleMessage(e);
	}

	public override Task ConnectAsync(CancellationToken token = default)
	{
		return Pipe.ConnectAsync(token);
	}

	public override void Disconnect()
	{
		Pipe.Close();
	}

	protected override Task SendAsync(SocketMessage message, CancellationToken cancellation = default)
	{
		return Pipe.SendAsync(message, cancellation);
	}
}
