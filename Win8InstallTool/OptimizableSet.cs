using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool
{
	public interface OptimizableSet
	{
		string Name { get; }

		IEnumerable<Optimizable> Scan();
	}
}
