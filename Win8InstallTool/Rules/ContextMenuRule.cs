using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Win8InstallTool.Rules
{
    public sealed class ContextMenuRule : ImutatableRule
    {
        private readonly string item;
        private readonly IEnumerable<string> folders;

        public override string Name { get; }

        public override string Description { get; }

        public ContextMenuRule(string item, IEnumerable<string> folders, string name, string description)
        {
            this.item = item;
            this.folders = folders;
            Name = name;
            Description = description;
        }

        protected override bool Check()
        {
            return folders
                .Select(folder => Path.Combine(folder, item))
                .Any(Registry.ClassesRoot.ContainsSubKey);
        }
        
        public override void Optimize()
        {
            folders.Select(folder => Path.Combine(folder, item))
                .ForEach(key => Registry.ClassesRoot.DeleteSubKeyTree(key, false));
        }
    }
}
