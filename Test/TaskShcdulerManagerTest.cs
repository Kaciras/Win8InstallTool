using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TaskScheduler;

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
		public void ClearFolder()
		{
			var task = TaskShcdulerManager.Instance.NewTask(0);
			var action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
			action.Id = "id";
			action.Path = "cmd.exe";

			// 目录会自动创建
			TaskShcdulerManager.Root.RegisterTaskDefinition(
				@"Test\SubFolder\测试任务", 
				task,
				(int)_TASK_CREATION.TASK_CREATE,
				null,
				null, 
				_TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN); 

			TaskShcdulerManager.ClearFolder("Test");

            try
            {
				TaskShcdulerManager.Root.GetTask(@"Test\SubFolder\测试任务");
				Assert.Fail("Expect to throw exception");
			} 
			catch(IOException e)
			when (e is DirectoryNotFoundException || e is FileNotFoundException)
			{
				// Expect task is not exists.
			}
            finally
            {
				TaskShcdulerManager.Root.DeleteFolder(@"Test\SubFolder", 0);
				TaskShcdulerManager.Root.DeleteFolder(@"Test", 0);
			}
		}
	}
}
