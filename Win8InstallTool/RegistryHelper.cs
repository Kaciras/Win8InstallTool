using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;

namespace Win8InstallTool
{
	public static class RegistryHelper
	{
		/// <summary>
		/// 从 .NET 标准库里抄的快捷方法，增加了根键的缩写，为什么微软不直接提供？
		/// </summary>
		public static RegistryKey OpenKey(string path, bool wirte = false)
		{
			if (path == null)
			{
				throw new ArgumentNullException("keyName");
			}

			var basekeyName = path;
			var i = path.IndexOf('\\');
			if (i != -1)
			{
				basekeyName = path.Substring(0, i);
			}
			var basekey = GetBaseKey(basekeyName);

			if (i == -1 || i == path.Length)
			{
				return basekey;
			}
			else
			{
				var pathRemain = path.Substring(i + 1, path.Length - i - 1);
				return basekey.OpenSubKey(pathRemain, wirte);
			}
		}

		static RegistryKey GetBaseKey(string name) => name.ToUpper() switch
		{
			"HKEY_CURRENT_USER" or "HKCU" => Registry.CurrentUser,
			"HKEY_LOCAL_MACHINE" or "HKLM" => Registry.LocalMachine,
			"HKEY_CLASSES_ROOT" or "HKCR" => Registry.ClassesRoot,
			"HKEY_USERS" or "HKU" => Registry.Users,
			"HKEY_CURRENT_CONFIG" or "HKCC" => Registry.CurrentConfig,
			"HKEY_PERFORMANCE_DATA" => Registry.PerformanceData,
			"HKEY_DYN_DATA" => RegistryKey.OpenBaseKey(RegistryHive.DynData, RegistryView.Default),
			_ => throw new ArgumentException("InvalidKeyName: " + name),
		};

		public static bool KeyExists(string path)
		{
			var i = path.IndexOf('\\');
			if (i == -1)
			{
				GetBaseKey(path);
			}
			var basekeyName = path.Substring(0, i);
			var remain = path.Substring(i + 1, path.Length - i - 1);

			var basekey = GetBaseKey(basekeyName);
			using var target = basekey.OpenSubKey(remain);
			return target != null;
		}

		/// <summary>
		/// 导出注册表键，相当于注册表编辑器里右键 -> 导出。
		/// </summary>
		/// <param name="file">保存的文件</param>
		/// <param name="path">注册表键</param>
		public static void Export(string file, string path) => Utils.Execute("regedit", $"/e {file} {path}");

		// 必须用 regedit.exe，如果用 regedt32 可能出错，上面的一样
		public static void Import(string file) => Utils.Execute("regedit", $"/s {file}");

		// CLSDI 格式 {8-4-4-4-12}
		public static string GetCLSIDValue(string clsid)
		{
			using var key = OpenKey(@"HKEY_CLASSES_ROOT\CLSID\" + clsid);
			if (key == null)
			{
				throw new DirectoryNotFoundException("CLSID记录不存在");
			}
			return (string)key.GetValue(string.Empty);
		}

		/// <summary>
		/// 在指定的目录中搜索含有某个路径的项，只搜一层。
		/// <br/>
		/// 因为 rootKey 会销毁，必须在离开作用域前遍历完，所以返回IList。
		/// </summary>
		/// <param name="root">在此目录中搜索</param>
		/// <param name="key">要搜索的键路径</param>
		/// <returns>子项名字列表</returns>
		public static IList<string> Search(string root, string key)
		{
			using var rootKey = OpenKey(root);
			return rootKey.GetSubKeyNames()
				.Where(name => rootKey.ContainsSubKey(Path.Combine(name, key)))
				.ToList();
		}

		public static bool ContainsSubKey(this RegistryKey key, string name)
		{
			using var subKey = key.OpenSubKey(name);
			return subKey != null;
		}
	}
}
