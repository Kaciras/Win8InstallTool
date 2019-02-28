using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool
{
	public static class RegistryHelper
	{
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

			RegistryKey basekey = null;

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
					throw new ArgumentException("InvalidKeyName");
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

		private static void InvokeRegeditor(string args)
		{
			var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			var startInfo = new ProcessStartInfo(Path.Combine(windir, "regedit.exe"), args);
			var process = new Process { StartInfo = startInfo };
			process.Start();
			process.WaitForExit();

			if (process.ExitCode != 0)
			{
				throw new SystemException("注册表操作失败,返回码:" + process.ExitCode);
			}
		}

		public static void Export(string file, string path) => InvokeRegeditor($"/e {file} {path}");

		public static void Import(string file) => InvokeRegeditor($"/s {file}");

		// CLSDI 格式 {8-4-4-4-12}
		public static string GetCLSIDValue(string clsid)
		{
			using (var key = OpenKey(@"HKEY_CLASSES_ROOT\CLSID\" + clsid))
			{
				if (clsid == null)
				{
					throw new DirectoryNotFoundException("CLSID记录不存在");
				}
				return (string)key.GetValue(string.Empty);
			}
		}

		/// <summary>
		/// 尽管程序要求以管理员身份运行，但有些注册表键仍然没有修改权限，故需要添加一下权限。
		/// 可以使用using语法来自动还原权限：<code>using (RegistryHelper.ElevatePermission(key)) { ... }</code>
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

		// 扩展方法

		public static bool ContainsSubKey(this RegistryKey key, string name)
		{
			using (var subKey = key.OpenSubKey(name)) return subKey != null;
		}
	}
}
