using System;
using System.Collections.Generic;
using System.Linq;

namespace Win8InstallTool.Rules
{
	/// <summary>
	/// 表示一条优化规则，调用 Check() 来检测是否可优化。
	/// <br/>
	/// 不知道这么做是否多余，使用可变对象好像也没有什么问题。
	/// </summary>
	public interface Rule : Optimizable
	{
		/// <summary>
		/// 检测该规则是否可优化，该方法一定会在 Optimize() 之前调用。
		/// </summary>
		bool Check();
	}

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
