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

        [TestMethod]
        public void GetEnabled()
        {
			var task = TaskShcdulerManager.Root.GetTask(@"Microsoft\Windows\Chkdsk\ProactiveScan");
			Assert.IsTrue(task.Enabled);
		}

		[TestMethod]
		public void DeleteFolder()
		{
			var folder = TaskShcdulerManager.Root.CreateFolder("Test");
			folder.CreateFolder("SubFolder");
			TaskShcdulerManager.DeleteFolder("Test");
		}
	}
}
