using System.IO;
using System.Linq;
using TaskScheduler;

namespace Win8InstallTool
{
    /// <summary>
    /// 对 TaskScheduler 的封装，提供了一些快捷方法。
    /// </summary>
    public static class TaskShcdulerManager
	{
		static readonly TaskSchedulerClass taskScheduler;

		static TaskShcdulerManager()
		{
			taskScheduler = new TaskSchedulerClass();
			taskScheduler.Connect();
			Root = taskScheduler.GetFolder(@"\");
		}

		public static ITaskFolder Root { get; }

		public static void DeleteFolder(string path)
		{
			var folder = Root.GetFolder(path);

			folder.GetTasks((int)_TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN)
				.Cast<IRegisteredTask>()
				.ForEach(t => folder.DeleteTask(t.Name, 0));

			folder.GetFolders(0)
				.Cast<ITaskFolder>()
				.ForEach(f => DeleteFolder(f.Path));

			Root.DeleteFolder(path, 0);
		}

		public static void DeleteTask(string path)
		{
			var folder = Path.GetDirectoryName(path);
			var name = Path.GetFileName(path);
			taskScheduler.GetFolder(folder).DeleteTask(name, 0);
		}

		public static void Import(string dir, string name, string xml)
		{
			taskScheduler.GetFolder(dir).RegisterTask(
				name,
				xml,
				(int) _TASK_CREATION.TASK_CREATE,
				null,
				null,
				_TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN);
		}

		public static string Export(string dir, string name)
		{
			return taskScheduler.GetFolder(dir).GetTask(name).Xml;
		}
	}
}
