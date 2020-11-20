﻿using System.Linq;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
    public class ExplorerFolderRule : ImutatableRule
	{
		// HKEY_LOCAL_MACHINE
		private const string KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace";

		private readonly string[] clsids = {
			"{1CF1260C-4DD0-4ebb-811F-33C572699FDE}",
			"{374DE290-123F-4565-9164-39C4925E467B}",
			"{3ADD1653-EB32-4cb0-BBD7-DFA0ABB5ACCA}",
			"{A0953C92-50DC-43bf-BE83-3742FED03C9C}",
			"{A8CDFF1C-4878-43be-B5FD-F8091C1C60D0}",
			"{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}",
		};

		public override string Name => "删除我的电脑界面上的6个文件夹";

		public override string Description => "他们可以从个人文件夹里打开,放在我的电脑里只会占位置，而且我不咋用";

		protected override bool Check()
		{
			using var nameSpage = Registry.LocalMachine.OpenSubKey(KEY, RegistryRights.FullControl);
			return clsids.Any(k => nameSpage.ContainsSubKey(k));
		}

		// 注册表 32 跟 64 位存储是分开的，系统自带的注册表编辑器能同时操作两者，但C#的API不能。
		// 如果架构对不上，则会出现找不到 key 的情况。
		public override void Optimize()
		{
			using var nameSpage = Registry.LocalMachine.OpenSubKey(KEY, true);
			clsids.ForEach(name => nameSpage.DeleteSubKeyTree(name));
		}
	}
}
