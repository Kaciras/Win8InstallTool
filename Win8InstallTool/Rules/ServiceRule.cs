using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public class ServiceRule : Rule
	{
		private const string SERVICE_DIR = @"SYSTEM\CurrentControlSet\Services\";

		private readonly string name;
		private readonly ServiceState state;

		public ServiceRule(string name, ServiceState state)
		{
			this.name = name;
			this.state = state;
		}

		public bool Optimizable()
		{
			using (var config = Registry.LocalMachine.OpenSubKey(SERVICE_DIR + name))
			{
				if (config == null)
				{
					return false; // 不存在的服务没法优化
				}
				var current = (ServiceState)config.GetValue("Start");
				if (current == ServiceState.Automatic && (int)config.GetValue("DelayedAutostart") == 1)
				{
					return current == ServiceState.LazyStart;
				}
				return current == state;
			}
		}

		public void Execute()
		{
			if (state == ServiceState.Deleted)
			{
				Registry.LocalMachine.DeleteSubKeyTree(SERVICE_DIR + name);
			}

			using (var config = Registry.LocalMachine.OpenSubKey(SERVICE_DIR + name))
			{
				var startValue = (int)state;

				if (state == ServiceState.LazyStart)
				{
					config.SetValue("DelayedAutostart", 1, RegistryValueKind.DWord);
					startValue = 2;
				}
				else
				{
					config.DeleteValue("DelayedAutostart");
					config.SetValue("Start", startValue, RegistryValueKind.DWord);
				}
			}
		}
	}
}
