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

		public static bool FolderExists(string path)
		{
			try
			{
				Root.GetFolder(path);
				return true;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}

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

		public static void SetEnable(string path, bool enable)
		{
			var task = Root.GetTask(path);
			if (task != null)
			{
				task.Enabled = enable;
			}
            else
            {
				throw new FileNotFoundException("找不到任务项");
            }
		}

		public static bool? GetEnable(string path)
		{
			try
			{
				return Root.GetTask(path).Enabled;
			}
			catch(IOException ex)
			when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
			{
				return null;
			}
		}

		public static void Import(string dir, string name, string xml)
		{
			taskScheduler.GetFolder(dir).RegisterTask(
				"asd",
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
