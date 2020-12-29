using System;
using System.Collections.Generic;
using System.Linq;

namespace Win8InstallTool.Rules
{
	/// <summary>
	/// 一个简单的 OptimizableSet 实现，手动设置名字并提供返回规则列表的委托，
	/// 在扫描时调用每条规则的 Check() 方法。
	/// </summary>
	public class RuleList : OptimizableSet
	{
		readonly Func<IEnumerable<Rule>> factory;

		public string Name { get; }

		public RuleList(string name, Func<IEnumerable<Rule>> factory)
		{
			Name = name;
			this.factory = factory;
		}

		public IEnumerable<Optimizable> Scan()
		{
			return factory().Where(rule => rule.Check());
		}
	}
}
