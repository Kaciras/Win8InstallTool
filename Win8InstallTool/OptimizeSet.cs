using System.Collections.Generic;

namespace Win8InstallTool
{
    public struct OptimizeSet
    {
        public readonly string Name;

        public readonly IEnumerable<Optimizable> Items;

        public OptimizeSet(string name, IEnumerable<Optimizable> items)
        {
            Name = name;
            Items = items;
        }
    }
}
