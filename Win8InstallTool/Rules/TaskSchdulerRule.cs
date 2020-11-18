using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler;

namespace Win8InstallTool.Rules
{
    public class TaskSchdulerRule
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

        public Optimizable Check()
        {
            var folder = TaskShcdulerManager.Root.GetFolder(path);
            if (folder != null)
            {
                return new FolderOptimizeItem(folder, description);
            }

            var task = TaskShcdulerManager.Root.GetTask(path);
            if (task == null)
            {
                return null;
            }
            if(keep && !task.Enabled)
            {
                return null;
            }
            return new TaskOptimizeItem(task, keep, description);
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
