using System.ComponentModel;

namespace Speakeasy.Messages;

public abstract class SerializableEventArgs : EventArgs
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public SerializableEventArgs()
	{
		EventArguments = Array.Empty<string>();
	}
	public SerializableEventArgs(params string[] arguments)
	{
		EventArguments = arguments;
	}
	public SerializableEventArgs(params object[] arguments)
	{
		EventArguments = arguments;
	}
	public object[] EventArguments { get; set; }
}
