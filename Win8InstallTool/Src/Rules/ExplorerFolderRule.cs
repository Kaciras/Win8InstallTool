using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public class ExplorerFolderRule : Rule
	{
		// HKEY_LOCAL_MACHINE
		private const string KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace";

		private readonly string[] names = {
			"{1CF1260C-4DD0-4ebb-811F-33C572699FDE}",
			"{374DE290-123F-4565-9164-39C4925E467B}",
			"{3ADD1653-EB32-4cb0-BBD7-DFA0ABB5ACCA}",
			"{A0953C92-50DC-43bf-BE83-3742FED03C9C}",
			"{A8CDFF1C-4878-43be-B5FD-F8091C1C60D0}",
			"{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}",
		};

		public string Name = "清除我的电脑界面上的6个文件夹";
		public string Discription = "这些可以从个人文件夹里打开,放在我的电脑里只会占位置";

		public bool Optimizable()
		{
			using (var nameSpage = Registry.LocalMachine.OpenSubKey(KEY))
			{
				return names.Any(k => nameSpage.ContainsSubKey(k));
			}
		}

		// 找不到子项，但是在Regeditor里却存在
		public void Optimize()
		{
			using (var nameSpage = Registry.LocalMachine.OpenSubKey(KEY, true))
			{
				names.ForEach(name => nameSpage.DeleteSubKey(name));
			}
		}
	}
}
