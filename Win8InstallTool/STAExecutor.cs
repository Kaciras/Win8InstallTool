﻿using System;
using System.Threading;

namespace Win8InstallTool
{
	/// <summary>
	/// 一些 COM 组件要求在 STA 线程访问，但 .NET 的线程池不是 STA，故做了一个切换线程的工具。
	/// <br/>
	/// 代码参考了：<seealso cref="https://stackoverflow.com/a/21684059"/>
	/// </summary>
	public static class STAExecutor
	{
		static SynchronizationContext context;

		/// <summary>
		/// 在调用 Run 方法前必须先调用此方法，设置使用 STA 线程的同步上下文。
		/// </summary>
		public static void SetSyncContext(SynchronizationContext context)
		{
			var apartment = ApartmentState.Unknown;
			context.Send(_ => apartment = Thread.CurrentThread.GetApartmentState(), null);
			if (apartment != ApartmentState.STA)
			{
				throw new ArgumentException("同步上下文必须使用 STA 线程", nameof(context));
			}

			STAExecutor.context = context;
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
