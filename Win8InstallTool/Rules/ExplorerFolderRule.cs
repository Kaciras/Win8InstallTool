using Microsoft.Win32;
using System.Linq;
using System.Security.AccessControl;

namespace Win8InstallTool.Rules;

public sealed class ExplorerFolderRule : Rule
{
	// HKEY_LOCAL_MACHINE
	const string KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace";

	readonly string[] clsids = {
			"{1CF1260C-4DD0-4ebb-811F-33C572699FDE}",
			"{374DE290-123F-4565-9164-39C4925E467B}",
			"{3ADD1653-EB32-4cb0-BBD7-DFA0ABB5ACCA}",
			"{A0953C92-50DC-43bf-BE83-3742FED03C9C}",
			"{A8CDFF1C-4878-43be-B5FD-F8091C1C60D0}",
			"{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}",
		};

	public string Name => "删除我的电脑界面上的6个文件夹";

	public string Description => "就是视频、文档、链接这些，它们可以从用户目录里打开,放在我的电脑里只会占位置，而且我不咋用";

	public bool Check()
	{
		using var nameSpage = Registry.LocalMachine.OpenSubKey(KEY, RegistryRights.FullControl);
		return clsids.Any(k => nameSpage.ContainsSubKey(k));
	}

	// 注册表 32 跟 64 位存储是分开的，系统自带的注册表编辑器能同时操作两者，但C#的API不能。
	// 如果架构对不上，则会出现找不到 key 的情况。
	public void Optimize()
	{
		using var nameSpage = Registry.LocalMachine.OpenSubKey(KEY, true);
		clsids.ForEach(name => nameSpage.DeleteSubKeyTree(name));
	}
}
