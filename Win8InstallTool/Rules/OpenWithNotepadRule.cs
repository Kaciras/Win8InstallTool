﻿using System.IO;
using System.Text;
using Microsoft.Win32;
using Win8InstallTool.Properties;

namespace Win8InstallTool.Rules
{
	public sealed class OpenWithNotepadRule : Rule
	{
		public string Name => "把用记事本打开添加到右键菜单";

		public string Description => "很常用的功能，稍微会点电脑的都懂。";

		public bool Check()
		{
			return Registry.ClassesRoot.OpenSubKey(@"*\shell\OpenWithNotepad") == null;
		}

		public void Optimize()
		{
			using var file = Utils.CreateTempFile();
			File.WriteAllText(file.Path, Resources.OpenWithNotepad, Encoding.Unicode);
			RegistryHelper.Import(file.Path);
		}
	}
}
