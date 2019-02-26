using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaskScheduler;

namespace Win8InstallTool
{
	public static class TaskShcdulerManager
	{
		static TaskSchedulerClass taskScheduler;

		static TaskShcdulerManager()
		{
			taskScheduler = new TaskSchedulerClass();
			taskScheduler.Connect();
			Root = taskScheduler.GetFolder(@"\");
		}

		public static ITaskFolder Root { get; }

		public static bool ExistFolder(string path)
		{
			try
			{
				Root.GetFolder(path);
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			return true;
		}

		internal static void DeleteDirectory(string path)
		{
			var folder = Root.GetFolder(path);

			folder.GetTasks((int)_TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN)
				.Cast<IRegisteredTask>()
				.ForEach(t => folder.DeleteTask(t.Name, 0));

			folder.GetFolders(0)
				.Cast<ITaskFolder>()
				.ForEach(f => DeleteDirectory(f.Path));

			Root.DeleteFolder(path, 0);
		}

		internal static void DeleteTask(string dir, string name)
		{
			var folder = taskScheduler.GetFolder(dir);
			folder.DeleteTask(name, 0);
		}

		internal static void ChangeEnable(string dir, string name, bool enable)
		{
			var task = taskScheduler.GetFolder(dir)?.GetTask(name);
			if (task != null)
			{
				task.Enabled = enable;
			}
		}

		internal static bool? GetEnable(string dir, string name)
		{
			try
			{
				return taskScheduler.GetFolder(dir).GetTask(name)?.Enabled;
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
