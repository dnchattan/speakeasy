using System.Text;
using Newtonsoft.Json;

namespace Speakeasy.Messages;

public class ApiStringSerializer : IMessageSerializer<string>
{
	private readonly UnicodeEncoding StreamEncoding = new();

	public async Task<string?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default)
	{
		using StreamReader sr = new(stream, StreamEncoding, false, 1024, true);
		return await sr.ReadLineAsync(cancellation);
	}

	public async Task<int> WriteMessageAsync(Stream stream, string outString, CancellationToken cancellation = default)
	{
		using StreamWriter sw = new(stream, StreamEncoding, 1024, true);
		await sw.WriteLineAsync(outString);
		return outString.Length + 2;
	}
}

public class ApiMessageSerializer : IMessageSerializer<SocketMessage>
{
	private static readonly ApiStringSerializer StringSerializer = new();
	public async Task<SocketMessage?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default)
	{
		string? message = await StringSerializer.ReadMessageAsync(stream, cancellation);
		if (message == null)
			return null;
		return JsonConvert.DeserializeObject<SocketMessage>(message);
	}

	public Task<int> WriteMessageAsync(Stream stream, SocketMessage message, CancellationToken cancellation = default)
	{
		string serializedMessage = JsonConvert.SerializeObject(message);
		return StringSerializer.WriteMessageAsync(stream, serializedMessage, cancellation);
	}
}
