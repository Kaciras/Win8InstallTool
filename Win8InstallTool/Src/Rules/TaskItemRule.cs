using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
    public sealed class TaskItemRule : TaskShcdulerRule
    {
        public bool ShouldDelete { get; set; } = true;

        public override bool Check()
        {
            return TaskShcdulerManager.GetEnable(Path) == true;
        }

        public override void Optimize()
        {
            if (ShouldDelete)
            {
                TaskShcdulerManager.DeleteTask(Path);
            }
            else
            {
                TaskShcdulerManager.SetEnable(Path, false);
            }
        }
    }
}
