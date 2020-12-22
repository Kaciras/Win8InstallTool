using System;
using System.Threading;
using System.Windows.Forms;

namespace Win8InstallTool
{
	/// <summary>
	/// 一些 COM 组件要求在 STA 线程访问，但 .NET 的线程池不是 STA，故需要一个切换线程的工具。
	/// <br/>
	/// 代码参考了：<seealso cref="https://stackoverflow.com/a/21684059"/>
	/// </summary>
	public static class STAExecutor
	{
		static SynchronizationContext context;

		/// <summary>
		/// 初始化，请在 Application.Run 之前调用此函数。
		/// <br/>
		/// 因为本程序使用 WinForm，所以底层的消息循环线程就是 STA，这里直接使用它就不用创建新线程了。
		/// </summary>
		public static void Initialize()
		{
			Application.Idle += Application_Idle;
		}

		static void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= Application_Idle;
			context = SynchronizationContext.Current;
		}

		/// <summary>
		/// 在 STA 线程中执行一个无返回值函数，同步等待执行完毕。
		/// </summary>
		/// <param name="action">要执行的函数</param>
		public static void Run(Action action)
		{
			context.Send(_ => action(), null);
		}

		/// <summary>
		/// 在 STA 线程中执行一个函数，同步等待执行完成并返回结果。
		/// </summary>
		/// <typeparam name="R">返回值类型</typeparam>
		/// <param name="function">要执行的函数</param>
		/// <returns>函数返回的结果</returns>
		public static R Run<R>(Func<R> function)
		{
			R returnValue = default;
			context.Send(_ => returnValue = function(), null);
			return returnValue;
		}
	}
}
