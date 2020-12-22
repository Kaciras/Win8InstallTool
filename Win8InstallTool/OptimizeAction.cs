using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool
{
	/// <summary>
	/// 函数式的优化项写法，在优化时调用指定的函数。
	/// </summary>
	public sealed class OptimizeAction : Optimizable
	{
		readonly Action optimizeAction;

		public string Name { get; }

		public string Description { get; }

		public OptimizeAction(string name, string description, Action optimizeAction)
		{
			this.optimizeAction = optimizeAction;
			Name = name;
			Description = description;
		}

		public void Optimize() => optimizeAction();
	}
}
