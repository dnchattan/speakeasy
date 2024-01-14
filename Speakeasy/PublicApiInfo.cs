using System.Reflection;

namespace Speakeasy;

internal record ApiMemberDefinition(string Contract, MemberInfo MemberInfo, PublicApiAttribute Attribute)
{
	public string Name => MemberInfo.Name;
	public string PublicName => Attribute.Name;
}

internal class PublicApiInfo<T>
{
	public readonly string Name;
	protected readonly IReadOnlyDictionary<string, ApiMemberDefinition> PublicApiByPublicName;
	protected readonly IReadOnlyDictionary<string, ApiMemberDefinition> PublicApiByMemberName;

	public PublicApiInfo()
	{
		Name = typeof(T).GetCustomAttribute<PublicApiAttribute>()?.Name ?? throw new InvalidOperationException("Missing PublicApiAttribute");
		var memberDefinitions = typeof(T).GetMembers()
				.Where(member => member.GetCustomAttribute<PublicApiAttribute>() != null)
				.Select(member =>
					new ApiMemberDefinition(
						Name,
						member,
						member.GetCustomAttribute<PublicApiAttribute>() ?? throw new InvalidOperationException("Missing PublicApiAttribute"))
					)
				.Where(member => member.Attribute != null);
		PublicApiByPublicName = memberDefinitions.ToDictionary(entry => entry.PublicName);
		PublicApiByMemberName = memberDefinitions.ToDictionary(entry => entry.MemberInfo.Name);
	}

	public U GetPublicApi<U>(string name, out string contractName) where U : MemberInfo
	{
		if (PublicApiByPublicName.TryGetValue(name, out ApiMemberDefinition? member) && member.MemberInfo is U result)
		{
			contractName = member.Contract;
			return result;
		}
		throw new ApiException(ApiError.UnknownContract, $"Unknown contract {member?.Contract}.{name}");
	}

	public ApiMemberDefinition GetMember<U>(string memberName) where U : MemberInfo
	{
		return PublicApiByMemberName.TryGetValue(memberName, out ApiMemberDefinition? member) && member.MemberInfo is U
			? member
			: throw new ApiException(ApiError.UnknownContract, $"Unknown contract {member?.Contract}.{memberName}");
	}
}
