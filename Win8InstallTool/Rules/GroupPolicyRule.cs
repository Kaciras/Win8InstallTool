using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public sealed class GroupPolicyRule : ImutatableRule
	{
		private readonly string key;
		private readonly string item;
		private readonly string value;

		public override string Name { get; }

		public override string Description { get; }

		public GroupPolicyRule(string key, string item, string value, string name, string description)
		{
			this.key = key;
			this.item = item;
			this.value = value;
			Name = name;
			Description = description;
		}

		protected override bool Check()
		{
			return GroupPolicy.GetPolicySetting(key, item)?.ToString() != value;
		}

		public override void Optimize()
		{
			GroupPolicy.SetPolicySetting(key, item, value, RegistryValueKind.DWord);
		}
	}
}
