using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
	public class TaskRule : Rule
	{
		public string Directory { get; set; }
		public string Namae { get; set; }

		public void Optimize()
		{
			throw new NotImplementedException();
		}

		public bool Optimizable()
		{
			throw new NotImplementedException();
		}
	}
}
