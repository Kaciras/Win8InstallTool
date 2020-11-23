using System.Collections.Generic;

namespace Win8InstallTool
{
	// TODO: 纯值对象的设计不好，还是应该以规则组作为核心
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
