using Speakeasy.Messages;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Speakeasy;

public class ApiCallerBase<T>
{
	protected readonly ApiClientBase Client;
	internal readonly PublicApiInfo<T> Api = new();
	private readonly Dictionary<string, List<Delegate>> EventHandlers = new();

	protected ApiCallerBase(ApiClientBase client)
	{
		Client = client;
		Client.EventReceived += Client_EventReceived;
	}

	private void Client_EventReceived(object? sender, ApiMessageEventArgs e)
	{
		HandleEvent(e.Message);
	}


	protected void AddHandler(string memberName, Delegate handler)
	{
		if (!EventHandlers.TryGetValue(memberName, out List<Delegate>? handlers))
		{
			EventHandlers.Add(memberName, new List<Delegate>() { handler });
			Subscribe(memberName);
			return;
		}
		handlers.Add(handler);
	}

	protected void RemoveHandler(string memberName, Delegate handler)
	{
		if (!EventHandlers.TryGetValue(memberName, out List<Delegate>? handlers))
			return;
		handlers.Remove(handler);
		if (handlers.Count == 0)
		{
			EventHandlers.Remove(memberName);
			Unsubscribe(memberName);
		}
	}

	private void HandleEvent(SendEventMessage message)
	{
		var def = Api.GetMember<EventInfo>(message.EventName);
		if (def.MemberInfo is not EventInfo eventInfo || !EventHandlers.TryGetValue(message.EventName, out List<Delegate>? handlers))
			return;
		Type eventArgsType = eventInfo!.EventHandlerType!.GetMethod("Invoke")!.GetParameters()[1].ParameterType;
		object[] eventArgsValues = eventArgsType.GetConstructors()
			.First(ctor => ctor.GetParameters().Length == message.Payload.Count)
			.GetParameters()
			.Select(param => param.ParameterType)
			.Zip(message.Payload, (paramType, arg) => arg.ToObject(paramType)!)
			.ToArray();
		object eventArgs = Activator.CreateInstance(eventArgsType, eventArgsValues)!;
		foreach (Delegate handler in handlers)
			handler.DynamicInvoke(this, eventArgs);
	}

	protected Task<U> CallMethod<U>(object[] args, [CallerMemberName] string? methodName = null)
	{
		if (string.IsNullOrEmpty(methodName))
			throw new ArgumentNullException(nameof(methodName));

		var def = Api.GetMember<MethodInfo>(methodName);
		return Client.Call<U>(def.Contract, def.Attribute.Name, args);
	}

	protected Task CallMethod(object[] args, [CallerMemberName] string? methodName = null)
	{
		if (string.IsNullOrEmpty(methodName))
			throw new ArgumentNullException(nameof(methodName));

		var def = Api.GetMember<MethodInfo>(methodName);
		return Client.Call(def.Contract, def.Attribute.Name, args);
	}

	protected void Subscribe([CallerMemberName] string? eventName = null)
	{
		if (string.IsNullOrEmpty(eventName))
			throw new ArgumentNullException(nameof(eventName));
		var def = Api.GetMember<EventInfo>(eventName);
		Client.Subscribe(def.Contract, def.Attribute.Name);
	}

	protected void Unsubscribe([CallerMemberName] string? eventName = null)
	{
		if (string.IsNullOrEmpty(eventName))
			throw new ArgumentNullException(nameof(eventName));
		var def = Api.GetMember<EventInfo>(eventName);
		Client.Unsubscribe(def.Contract, def.Attribute.Name);
	}
}
