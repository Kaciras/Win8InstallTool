using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
    public class ContextMenuRule : Rule
    {
        private readonly string @class;

        public string Description => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public ContextMenuRule(string @class)
        {
            this.@class = @class;
        }

        public bool Check()
        {
            return RegistryHelper.ContainsSubKey(Registry.ClassesRoot, @class);
        }

        public void Optimize()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(@class, false);
        }
    }
}
