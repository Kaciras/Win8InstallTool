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
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine($"Kaciras 的 Windows8 优化工具 v{version.ToString(3)}");

			try
			{
				CheckSupportedOS();
			}
			catch (PlatformNotSupportedException e)
			{
				Console.WriteLine(e.Message);
				Environment.Exit(1);
			}

			var rule = new ExplorerFolderRule();
			if (rule.Check())
			{
				rule.Optimize();
			}
			Console.WriteLine("优化完毕，按任意键结束程序");
			Console.ReadKey();
		}

		private static void CheckSupportedOS()
		{
			var os = Environment.OSVersion;
			var version = os.Version;

			var isWin81 = os.Platform == PlatformID.Win32NT && version.Major * 10 == 6 && version.Minor == 1;
			if (!isWin81)
			{
				throw new PlatformNotSupportedException("本程序仅支持 Windows8.1");
			}

			if (!Environment.Is64BitOperatingSystem)
			{
				throw new PlatformNotSupportedException("本程序仅支持64位系统，请运行32位的版本");
			}
		}
	}
}
