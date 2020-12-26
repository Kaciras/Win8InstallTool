using System;
using System.Collections.Generic;
using System.Linq;

namespace Win8InstallTool.Rules
{
	/// <summary>
	/// 表示一条固定的优化规则，调用 Check() 来检测是否可优化。
	/// <br/>
	/// 规则的状态不可共享，在每次扫描时都必须创建新的实例。
	/// </summary>
	public interface Rule : Optimizable
	{
		/// <summary>
		/// 检测该规则是否可优化，该方法一定会在 Optimize() 之前调用，可以在该方法里改变内部状态。
		/// </summary>
		bool Check();
	}
}
