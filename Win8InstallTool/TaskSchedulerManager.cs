using System.IO;
using System.Linq;
using TaskScheduler;

namespace Win8InstallTool
{
    /// <summary>
    /// 对 TaskScheduler 的封装，提供了一些快捷方法。
    /// </summary>
    public static class TaskSchedulerManager
	{
		static readonly TaskScheduler.TaskScheduler taskScheduler;

		static TaskSchedulerManager()
		{
			// 必须把 Class 后缀去掉，或是使用外置 Interop Types
			// https://stackoverflow.com/a/4553402/7065321
			taskScheduler = new TaskScheduler.TaskScheduler();
			taskScheduler.Connect();
			Root = taskScheduler.GetFolder(@"\");
		}

		public static TaskScheduler.TaskScheduler Instance => taskScheduler;

		public static ITaskFolder Root { get; }

		/// <summary>
		/// 清空目录中的所有任务，考虑到有些目录无法删除所有把文件夹留下了。
		/// </summary>
		/// <param name="path">目录路径</param>
		public static void ClearFolder(string path)
		{
			var folder = Root.GetFolder(path);

			folder.GetTasks((int)_TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN)
				.Cast<IRegisteredTask>()
				.ForEach(t => folder.DeleteTask(t.Name, 0));

			folder.GetFolders(0)
				.Cast<ITaskFolder>()
				.ForEach(f => ClearFolder(f.Path));
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
