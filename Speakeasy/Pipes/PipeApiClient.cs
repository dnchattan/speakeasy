using Speakeasy.Messages;

namespace Speakeasy.Pipes;

public class PipeApiClient : ApiClientBase
{
	private static readonly ApiMessageSerializer Serializer = new();
	private readonly PipeClient<SocketMessage> Pipe;
	public PipeApiClient(string pipeName) : base()
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
