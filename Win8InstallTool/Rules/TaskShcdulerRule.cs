using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
	public abstract class TaskShcdulerRule : Rule
	{
		public string Path { get; set; }
		public string Description { get; set; }

		public bool Delete { get; set; } = true;

		public bool Optimizable()
		{
			throw new NotImplementedException();
		}

		public void Optimize()
		{
			throw new NotImplementedException();
		}
	}
}
