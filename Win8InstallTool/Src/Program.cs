using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Win8InstallTool.Rules;

[assembly: InternalsVisibleTo("Test")]
namespace Win8InstallTool
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			new ExplorerFolderRule().Optimize();
		}
	}
}
