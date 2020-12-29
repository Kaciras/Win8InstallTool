using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win8InstallTool
{
	[TestClass]
	public class GroupPolicyTest
	{
		// 单元测试的线程也是STA，需要手动创建新的
		[TestMethod]
		public void NonSTAThread()
		{
			Exception e = null;

			var thread = new Thread(() =>
			{
				try { new ComputerGroupPolicyObject(); }
				catch (Exception ex) { e = ex; }
			});
			thread.Start();
			thread.Join();

			Assert.IsNotNull(e);
			Assert.AreEqual("GPO can only be accessed in STA thread", e.Message);
		}

		// 其他方法不好测，都依赖系统环境或有副作用
	}
}
