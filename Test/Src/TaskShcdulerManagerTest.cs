using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win8InstallTool.Test
{
	[TestClass]
	public sealed class TaskShcdulerManagerTest
	{
		[TestMethod]
		public void InitRoot()
		{
			var windows = TaskShcdulerManager.Root.GetFolder(@"Microsoft\Windows");
			Assert.IsTrue(windows.GetFolders(0).Count > 50);
		}
	}
}
