using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Win8InstallTool.Rules
{
    public sealed class ContextMenuRule : ImutatableRule
    {
        private readonly ICollection<string> keys;

        public override string Name { get; }

        public override string Description { get; }

        public ContextMenuRule(ICollection<string> keys, string name, string description)
        {
            this.keys = keys;
            Name = name;
            Description = description;
        }

        protected override bool Check()
        {
            return keys.Any(Registry.ClassesRoot.ContainsSubKey);
        }
        
        // TODO: 删除空的 shell 和 shellex 项？
        public override void Optimize()
        {
            keys.ForEach(key => Registry.ClassesRoot.DeleteSubKeyTree(key, false));
        }
    }
}
