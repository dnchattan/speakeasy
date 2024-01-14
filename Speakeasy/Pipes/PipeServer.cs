using System.Collections.Concurrent;
using System.IO.Pipes;

namespace Speakeasy.Pipes;

public class PipeServer<T> : IDisposable
{
	private readonly CancellationTokenSource Cancellation;
	private readonly ConcurrentBag<IPipeSession<T>> Connections = new();
	private readonly string PipeName;
	private readonly IMessageSerializer<T> Serializer;
	private bool IsDisposed;

	public event EventHandler<T>? MessageReceived;

	public PipeServer(string pipeName, IMessageSerializer<T> serializer)
	{
		PipeName = pipeName;
		Serializer = serializer;
		Cancellation = new();
	}

	public void Start()
	{
		CreatePendingConnection();
	}

	public void Stop()
	{
		Dispose();
	}

	public void CreatePendingConnection(object? _state = null)
	{
		PipeServerConnection connection = new(PipeName, Serializer);
		connection.MessageReceived += Connection_MessageReceived;
		Connections.Add(connection);
		connection.WaitForConnection().ContinueWith(CreatePendingConnection, Cancellation.Token);
	}

	private void Connection_MessageReceived(object? sender, T e)
	{
		MessageReceived?.Invoke(sender, e);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				Cancellation.Cancel();
				foreach (IPipeSession<T> connection in Connections)
				{
					connection.Dispose();
				}
			}

			IsDisposed = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private class PipeServerConnection : PipeSession<T>
	{
		public PipeServerConnection(string pipeName, IMessageSerializer<T> serializer)
			: base(new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances), serializer)
		{ }

		public async Task WaitForConnection()
		{
			if (Pipe is not NamedPipeServerStream server)
				throw new InvalidCastException(nameof(Pipe));

			await server.WaitForConnectionAsync();
			StartListening();
		}
	}
}
