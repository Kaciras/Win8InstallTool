using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace Win8InstallTool.Rules
{
	public abstract class TaskShcdulerRule : Rule
	{
		public string Path { get; set; }

		public string Description { get; set; }

        public abstract bool Check();

        public abstract void Optimize();
    }
}
