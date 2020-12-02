using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			return ComputerGroupPolicyObject.GetPolicySetting(key, item)?.ToString() != value;
		}

		public override void Optimize()
		{
			ComputerGroupPolicyObject.SetPolicySetting(key, item, value, RegistryValueKind.DWord);
		}
	}
}
