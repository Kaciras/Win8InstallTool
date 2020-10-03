using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
			var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			Console.WriteLine($"Kaciras 的 Windows8 优化工具 v{version}");
			Console.ReadKey();

			//new ExplorerFolderRule().Optimize();
		}
	}
}
