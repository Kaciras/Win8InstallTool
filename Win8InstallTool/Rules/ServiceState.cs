using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
	public enum ServiceState
	{
		/// <summary>
		/// 启动类型：自动
		/// </summary>
		Automatic = 2,

		/// <summary>
		/// 启动类型：手动
		/// </summary>
		Manual = 3,

		/// <summary>
		/// 启动类型：禁用
		/// </summary>
		Disabled = 4,

		/// <summary>
		/// 启动类型：延迟启动
		/// </summary>
		LazyStart = -1,

		/// <summary>
		/// 直接删除服务，眼不见心不烦
		/// </summary>
		Deleted = -2,
	}
}
