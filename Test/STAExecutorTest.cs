using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win8InstallTool
{
	[TestClass]
	public sealed class STAExecutorTest
	{
		// STAExecutor 是全局的，如果该测试失败可能影响到其他测试。
		[TestMethod]
		public void SetSyncContext()
		{
			Assert.ThrowsException<ArgumentException>(() => STAExecutor.SetSyncContext(new ThreadSyncContext()));
		}

		class ThreadSyncContext : SynchronizationContext
		{
			public override void Send(SendOrPostCallback action, object state)
			{
				TestHelper.RunInNewThread(() => action(state));
			}
		}
	}
}
