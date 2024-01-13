using Newtonsoft.Json;

using System.ComponentModel;

namespace Speakeasy.Messages;

public class SubscriptionMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public SubscriptionMessage()
	{
		EventName = string.Empty;
	}

	public SubscriptionMessage(string eventName)
	{
		EventName = eventName;
	}

	[JsonProperty("eventName")]
	public string EventName;
}
