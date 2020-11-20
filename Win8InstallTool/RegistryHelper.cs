using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Win8InstallTool
{
    public static class RegistryHelper
	{
		/// <summary>
		/// 从 .NET 标准库里抄的快捷方法，为什么微软不直接提供？
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

			RegistryKey basekey;

			switch (basekeyName.ToUpper())
			{
				case "HKEY_CURRENT_USER":
					basekey = Registry.CurrentUser;
					break;
				case "HKEY_LOCAL_MACHINE":
					basekey = Registry.LocalMachine;
					break;
				case "HKEY_CLASSES_ROOT":
					basekey = Registry.ClassesRoot;
					break;
				case "HKEY_USERS":
					basekey = Registry.Users;
					break;
				case "HKEY_PERFORMANCE_DATA":
					basekey = Registry.PerformanceData;
					break;
				case "HKEY_CURRENT_CONFIG":
					basekey = Registry.CurrentConfig;
					break;
				case "HKEY_DYN_DATA":
					basekey = RegistryKey.OpenBaseKey(RegistryHive.DynData, RegistryView.Default);
					break;
				default:
					throw new ArgumentException("InvalidKeyName: " + basekeyName);
			}

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

		/// <summary>
		/// 调用 Regedit.exe 程序，用来做一些 Win32 库没有提供的功能，比如导入导出。
		/// </summary>
		/// <param name="args">运行参数</param>
		private static void InvokeRegeditor(string args)
		{
			// 如果用 regedt32 可能无法导入
            var startInfo = new ProcessStartInfo("regedit.exe", args)
            {
                UseShellExecute = false,
                RedirectStandardError = true,
            };

            var process = Process.Start(startInfo);
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				var stderr = process.StandardError.ReadToEnd();
				throw new SystemException($"注册表操作失败({process.ExitCode}):{stderr}");
			}
		}

		/// <summary>
		/// 导出注册表键，相当于注册表编辑器里右键 -> 导出
		/// </summary>
		/// <param name="file">保存的文件</param>
		/// <param name="path">注册表键</param>
		public static void Export(string file, string path) => InvokeRegeditor($"/e {file} {path}");

		public static void Import(string file) => InvokeRegeditor($"/s {file}");

		// CLSDI 格式 {8-4-4-4-12}
		public static string GetCLSIDValue(string clsid)
		{
			using var key = OpenKey(@"HKEY_CLASSES_ROOT\CLSID\" + clsid);
			if (clsid == null)
			{
				throw new DirectoryNotFoundException("CLSID记录不存在");
			}
			return (string)key.GetValue(string.Empty);
		}

		/// <summary>
		/// 在指定的目录中搜索含有某个路径的项，只搜一层。
		/// </summary>
		/// <param name="root">在此目录中搜索</param>
		/// <param name="key">要搜索的键路径</param>
		/// <returns>路径列表</returns>
		public static IList<string> Search(string root, string key)
		{
			using var rootKey = OpenKey(root);
			return rootKey.GetSubKeyNames()
				.Select(name => Path.Combine(name, key))
				.Where(path => rootKey.ContainsSubKey(path))
				.ToList(); // rootKey 会销毁，必须全部遍历完
		}

		public static bool ContainsSubKey(this RegistryKey key, string name)
		{
			using var subKey = key.OpenSubKey(name);
			return subKey != null;
		}

		/// <summary>
		/// 尽管程序要求以管理员身份运行，但有些注册表键仍然没有修改权限，故需要添加一下权限。
		/// 可以使用using语法来自动还原权限：
		/// <code>
		///		using (RegistryHelper.ElevatePermission(key)) { ... }
		/// </code>
		/// </summary>
		/// <param name="key">键</param>
		/// <returns>一个可销毁对象，在销毁时还原键的权限</returns>
		public static IDisposable ElevatePermission(RegistryKey key)
		{
			var old = key.GetAccessControl();

			var accessControl = key.GetAccessControl();
			var rule = new RegistryAccessRule(WindowsIdentity.GetCurrent().User, RegistryRights.FullControl, AccessControlType.Allow);
			accessControl.AddAccessRule(rule);
			key.SetAccessControl(accessControl);

			return new TemporaryElevateSession(key, old);
		}

		internal readonly struct TemporaryElevateSession : IDisposable
		{
			private readonly RegistryKey key;
			private readonly RegistrySecurity accessControl;

			public TemporaryElevateSession(RegistryKey key, RegistrySecurity accessControl)
			{
				this.key = key;
				this.accessControl = accessControl;
			}

			public void Dispose()
			{
				// TODO: key被删了咋办
				key.SetAccessControl(accessControl);
			}
		}		
	}
}
