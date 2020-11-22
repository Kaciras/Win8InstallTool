using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public class ServiceRule : ImutatableRule
	{
		private const string SERVICE_DIR = @"SYSTEM\CurrentControlSet\Services\";

		// override 属性不能增加 setter，傻逼C#总有东西来恶心老子。
		private string name;

		/// <summary>
		/// 服务在注册表里的键，不同于显示名。
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// 对此服务的简单介绍，以及需要优化的原因。
		/// </summary>
		public override string Description { get; }

		/// <summary>
		/// 服务的显示名，该名称可读性较强。
		/// </summary>
		public override string Name => name;

		/// <summary>
		/// 此服务应当被优化为什么状态，默认禁用。
		/// </summary>
		public ServiceState TargetState { get; set; } = ServiceState.Disabled;

		public ServiceRule(string key, string description)
		{
			Key = key;
			Description = description;
		}

		protected override bool Check()
		{
			using var config = Registry.LocalMachine.OpenSubKey(SERVICE_DIR + Key);
			if (config == null)
			{
				return false; // 服务不存在
			}

			if (name == null)
			{
				name = (string)config.GetValue("DisplayName", Key);
				if (name.StartsWith("@"))
				{
					name = Utils.ExtractStringFromDLL(name);
				}
			}

			var state = (ServiceState)config.GetValue("Start");
			var delayed = (int)config.GetValue("DelayedAutostart", -1) == 1;

			if (state == ServiceState.Automatic && delayed)
			{
				state = ServiceState.LazyStart;
			}

			return state != TargetState;
		}

		public override void Optimize()
		{
			if (TargetState == ServiceState.Deleted)
			{
				Registry.LocalMachine.DeleteSubKeyTree(SERVICE_DIR + Key);
			}

			using var config = Registry.LocalMachine.OpenSubKey(SERVICE_DIR + Key, true);
			var startValue = TargetState;

			if (TargetState == ServiceState.LazyStart)
			{
				config.SetValue("DelayedAutostart", 1, RegistryValueKind.DWord);
				startValue = ServiceState.Automatic;
			}
			else
			{
				// 默认不存在会报错，需要添加第二个参数
				config.DeleteValue("DelayedAutostart", false);
			}

			config.SetValue("Start", (int)startValue, RegistryValueKind.DWord);
		}
	}
}
