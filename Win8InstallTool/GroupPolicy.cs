﻿using System;
using Microsoft.Win32;

namespace Win8InstallTool
{
	public static class GroupPolicy
	{
		public static void SetPolicySetting(string key, string item, object value, RegistryValueKind kind)
		{
			// C# 不像 Python 那样有方便的 AOP，没法用注解来切STA线程，还得缩进一层挺难看的。
			STAExecutor.Run(() =>
			{
				var gpo = new ComputerGroupPolicyObject();
				var section = Key(key, out string subkey);

				using (var root = gpo.GetRootRegistryKey(section))
				{
					// Data can't be null so we can use this value to indicate key must be delete
					if (value == null)
					{
						using var subKey = root.OpenSubKey(subkey, true);
						if (subKey != null)
						{
							subKey.DeleteValue(item);
						}
					}
					else
					{
						using var subKey = root.CreateSubKey(subkey);
						subKey.SetValue(item, value, kind);
					}
				}

				gpo.Save();
			});
		}

		public static object GetPolicySetting(string key, string item)
		{
			return STAExecutor.Run(() =>
			{
				var gpo = new ComputerGroupPolicyObject();
				var section = Key(key, out string subkey);

				using var root = gpo.GetRootRegistryKey(section);
				using var subKey = root.OpenSubKey(subkey, true);
				return subKey?.GetValue(item);
			});
		}

		private static GroupPolicySection Key(string path, out string subkey)
		{
			var i = path.IndexOf('\\');
			var hive = path.Substring(0, i);
			subkey = path.Substring(i + 1);

			return hive.ToUpper() switch
			{
				"HKEY_LOCAL_MACHINE" or "HKLM" => GroupPolicySection.Machine,
				"HKEY_CURRENT_USER" or "HKCU" => GroupPolicySection.User,
				_ => throw new Exception($"错误的注册表 Root key: {hive}"),
			};
		}
	}
}
