using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public class ServiceRule : Optimizable
	{
		private const string SERVICE_DIR = @"SYSTEM\CurrentControlSet\Services\";

		/// <summary>
		/// 服务在注册表里的目录名。
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// 对此服务的简单介绍，以及需要优化的原因。
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// 服务的显示名，改名称可读性较强。
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// 此服务应当被优化为什么状态，默认禁用。
		/// </summary>
		public ServiceState TargetState { get; set; } = ServiceState.Disabled;

        public ServiceRule(string key, string description)
        {
            Key = key;
            Description = description;
        }

        public bool Check()
		{
			using var config = Registry.LocalMachine.OpenSubKey(SERVICE_DIR + Key);
			if (config == null)
			{
				return false; // 不存在的服务没法优化
			}

			Name = (string)config.GetValue("DisplayName", Key);
			if (Name.StartsWith("@"))
            {
				Name = Utils.ExtractStringFromDLL(Name);
            }

			var state = (ServiceState)config.GetValue("Start");
			var delayed = (int)config.GetValue("DelayedAutostart", -1) == 1;

			if (state == ServiceState.Automatic && delayed)
			{
				state = ServiceState.LazyStart;
			}

			return state == TargetState;
		}

		public void Optimize()
		{
			if (TargetState == ServiceState.Deleted)
			{
				Registry.LocalMachine.DeleteSubKeyTree(SERVICE_DIR + Key);
			}

			using var config = Registry.LocalMachine.OpenSubKey(SERVICE_DIR + Key);
			var startValue = (int)TargetState;

			if (TargetState == ServiceState.LazyStart)
			{
				config.SetValue("DelayedAutostart", 1, RegistryValueKind.DWord);
				startValue = 2;
			}
			else
			{
				config.DeleteValue("DelayedAutostart");
			}

			config.SetValue("Start", startValue, RegistryValueKind.DWord);
		}
	}
}
