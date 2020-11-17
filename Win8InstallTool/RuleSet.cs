using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool
{
    public sealed class RuleSet
    {
        public string Name { get; set; }

        public IEnumerable<Optimizable> Items { get; set; }
    }
}
