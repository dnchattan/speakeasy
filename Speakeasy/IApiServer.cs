using Speakeasy.Messages;

namespace Speakeasy;

public interface IApiServer<T>
{
	bool SupportsContract(string contract);
	void HandleMessage(SocketMessage message, IApiSession<T> session);
}
