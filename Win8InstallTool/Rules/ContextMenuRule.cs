using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
    public sealed class ContextMenuRule : ImutatableRule
    {
        private readonly string @class;

        // override 属性不能增加 setter，傻逼C#总有东西来恶心老子。
        private string name;

        public override string Description => "";

        public override string Name => name;

        public ContextMenuRule(string @class)
        {
            this.@class = @class;
        }

        protected override bool Check()
        {
            using var key = Registry.ClassesRoot.OpenSubKey(@class);
            if (key == null)
            {
                return false;
            }
            if (name == null)
            {
                name = key.GetValue(null, @class).ToString();
                if (name.StartsWith("@"))
                {
                    name = Utils.ExtractStringFromDLL(name);
                }
            }
            return true;
        }

        public override void Optimize()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(@class, false);
        }
    }
}
