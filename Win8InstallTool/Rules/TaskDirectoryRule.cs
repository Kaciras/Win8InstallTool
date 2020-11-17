using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
    public sealed class TaskDirectoryRule : TaskShcdulerRule
    {
        public override bool Check()
        {
            return TaskShcdulerManager.FolderExists(Path);
        }

        public override void Optimize()
        {
            TaskShcdulerManager.DeleteFolder(Path);
        }
    }
}
