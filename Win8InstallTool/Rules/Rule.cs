using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
	public interface Rule
	{
		bool Optimizable();

		void Execute();
	}
}
