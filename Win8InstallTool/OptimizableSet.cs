using System.Collections.Generic;

namespace Win8InstallTool
{
	public interface OptimizableSet
	{
		string Name { get; }

		IEnumerable<Optimizable> Scan();
	}
}
