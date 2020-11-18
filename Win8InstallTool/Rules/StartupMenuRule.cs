using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace Win8InstallTool.Rules
{
    /// <summary>
    /// 开始菜单清理规则，有些八百年用不到的程序看着就烦，安装时还不能取消创建。
    /// </summary>
    public class StartupMenuRule : Rule
    {
        public string Name { get; }

        public string Description { get; }

        public StartupMenuRule(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public bool Check()
        {
            var path = Path.Combine(GetFolderPath(SpecialFolder.Startup), Name);
            return Directory.Exists(path);
        }

        public void Optimize()
        {
            throw new NotImplementedException();
        }
    }
}
