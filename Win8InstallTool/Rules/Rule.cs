using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
    /// <summary>
    /// 表示一条优化规则，调用 Scan() 来检测是否可优化。
    /// <br/>
    /// 不知道这么做是否多余，使用可变对象好像也没有什么问题。
    /// </summary>
	public interface Rule
	{
        /// <summary>
        /// 检测该规则是否可优化，如果是返回优化项，否则返回null
        /// </summary>
		Optimizable Scan();
	}

    /// <summary>
    /// 简单的规则同时也是优化项，该规则的状态必须在多次扫描中保持一致。
    /// </summary>
	public abstract class ImutatableRule : Rule, Optimizable
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract void Optimize();

        protected abstract bool Check();

        public Optimizable Scan() => Check() ? this : null;
    }
}
