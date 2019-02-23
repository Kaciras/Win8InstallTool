using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public class ContextMenuRule
	{
		public string Class { get; set; }

		public ContextMenuRule(string @class)
		{
			Class = @class;
		}

		public void Execute()
		{
			try
			{
				Registry.ClassesRoot.DeleteSubKeyTree(Class);
			}
			catch(ArgumentException)
			{
				Console.WriteLine($"{Class}不存在，可能已经删除");
			}
		}
	}
}
