using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler;

namespace Win8InstallTool.Rules
{
    public class TaskSchdulerRule : Rule
    {
        private readonly string path;
        private readonly string description;
        private readonly bool keep;

        public TaskSchdulerRule(string path, string description, bool keep)
        {
            this.path = path;
            this.description = description;
            this.keep = keep;
        }

        public Optimizable Scan()
        {
            try
            {
                var folder = TaskShcdulerManager.Root.GetFolder(path);
                return new FolderOptimizeItem(folder, description);
            }
            catch (IOException e)
            when (e is DirectoryNotFoundException || e is FileNotFoundException)
            {
                // Ignore, maybe path is a task.
            }

            try
            {
                var task = TaskShcdulerManager.Root.GetTask(path);
                if (keep && !task.Enabled)
                {
                    return null;
                }
                return new TaskOptimizeItem(task, keep, description);
            }
            catch (IOException e)
            when (e is DirectoryNotFoundException || e is FileNotFoundException)
            {
                return null; // Task not found, cannot optimize
            }
        }

        internal sealed class FolderOptimizeItem : Optimizable
        {
            private readonly ITaskFolder folder;

            public string Name => folder.Name;

            public string Description { get; }

            public FolderOptimizeItem(ITaskFolder folder, string description)
            {
                this.folder = folder;
                Description = description;
            }

            public void Optimize()
            {
                TaskShcdulerManager.DeleteFolder(folder.Path);
            }
        }

        internal sealed class TaskOptimizeItem : Optimizable
        {
            private readonly IRegisteredTask task;
            private readonly bool keep;

            public string Name => task.Name;

            public string Description { get; }

            public TaskOptimizeItem(IRegisteredTask task, bool keep, string description)
            {
                this.task = task;
                this.keep = keep;
                Description = description;
            }

            public void Optimize()
            {
                if (keep)
                {
                    task.Enabled = false;
                }
                else
                {
                    TaskShcdulerManager.DeleteTask(task.Path);
                }
            }
        }
    }
}
