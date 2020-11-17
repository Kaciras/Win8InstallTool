using System;
using System.Collections.Generic;
using System.IO;
using Win8InstallTool.Properties;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public sealed class OpenWithNotepadRule : Rule
	{
		public string Description => "把用记事本打开添加到右键菜单";

        public bool Check()
		{
			return Registry.ClassesRoot.OpenSubKey(@"*\shell\OpenWithNotepad") == null;
		}

		public void Optimize()
		{
			RegistryHelper.Import("OpenWithNotepad.reg");
		}
	}
}
