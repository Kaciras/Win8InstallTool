using System.IO;
using static System.Environment;

namespace Win8InstallTool.Rules
{
	/// <summary>
	/// 开始菜单清理规则，有些八百年用不到的程序看着就烦，安装时还不能取消创建。
	/// </summary>
	public sealed class StartupMenuRule : Rule
	{
		private readonly string path;

		public  string Name { get; }

		public  string Description => "都是些从来不用的垃圾";

		public StartupMenuRule(bool isSystem, string name)
		{
			Name = name;
			var folder = isSystem ? SpecialFolder.CommonStartMenu : SpecialFolder.StartMenu;
			path = Path.Combine(GetFolderPath(folder), "Programs", name);
		}

		public bool Check()
		{
			return Directory.Exists(path);
		}

		public  void Optimize()
		{
			Directory.Delete(path, true);
		}
	}
}
