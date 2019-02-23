using System;
using System.Collections.Generic;
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
		/// <summary>
		/// 尽管程序要求以管理员身份运行，但有些注册表键仍然没有修改权限，故需要添加一下权限。
		/// 可以使用using语法来自动还原权限：<code>using (RegistryHelper.ElevatePermission(key)) { ... }</code>
		/// </summary>
		/// <param name="key">键</param>
		/// <returns>一个可销毁对象，在销毁时还原键的权限</returns>
		public static TemporaryElevateSession ElevatePermission(RegistryKey key)
		{
			var old = key.GetAccessControl();

			var accessControl = key.GetAccessControl();
			var rule = new RegistryAccessRule(WindowsIdentity.GetCurrent().User, RegistryRights.FullControl, AccessControlType.Allow);
			accessControl.AddAccessRule(rule);
			key.SetAccessControl(accessControl);

			return new TemporaryElevateSession(key, old);
		}

		public readonly struct TemporaryElevateSession : IDisposable
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
