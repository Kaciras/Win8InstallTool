using System.IO;
using static System.Environment;

namespace Win8InstallTool.Rules
{
    /// <summary>
    /// 开始菜单清理规则，有些八百年用不到的程序看着就烦，安装时还不能取消创建。
    /// </summary>
    public class StartupMenuRule : ImutatableRule
    {
        public override string Name { get; }

        public override string Description => "都是些从来不用的垃圾";

        public StartupMenuRule(string name)
        {
            Name = name;
        }

        protected override bool Check()
        {
            var startmenu = GetFolderPath(SpecialFolder.CommonStartMenu);
            return Directory.Exists(Path.Combine(startmenu, "Programs", Name));
        }

        public override void Optimize()
        {
            var startmenu = GetFolderPath(SpecialFolder.CommonStartMenu);
            Directory.Delete(Path.Combine(startmenu, "Programs", Name), true);
        }
    }
}
