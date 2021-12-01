using Microsoft.Win32;

namespace Win8InstallTool.Rules;

public sealed class GroupPolicyRule : Rule
{
	readonly string key;
	readonly string item;
	readonly string value;

	public string Name { get; }

	public string Description { get; }

	public GroupPolicyRule(string key, string item, string value, string name, string description)
	{
		this.key = key;
		this.item = item;
		this.value = value;
		Name = name;
		Description = description;
	}

	public bool NeedOptimize()
	{
		return GroupPolicy.GetPolicySetting(key, item)?.ToString() != value;
	}

	public void Optimize()
	{
		GroupPolicy.SetPolicySetting(key, item, value, RegistryValueKind.DWord);
	}
}
