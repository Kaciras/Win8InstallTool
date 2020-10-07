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
			Console.WriteLine($"Kaciras 的 Win8 优化工具 v{version.ToString(3)}");

			var osType = CheckOSSupport();

			
			var rule = new ExplorerFolderRule();
			if (rule.Check())
			{
				rule.Optimize();
			}

			Console.WriteLine("优化完毕！按任意键结束程序");
			Console.ReadKey();
		}

		/// <summary>
		/// 检查本程序是否支持所在的系统，如果不支持则会终止程序并返回退出码1，支持则返回系统类型。
		/// </summary>
		/// <returns>支持的系统类型</returns>
		static SupportedOS CheckOSSupport()
		{
			var os = Environment.OSVersion;
			var version = os.Version;

			if (!Environment.Is64BitOperatingSystem)
			{
				Console.Error.WriteLine("本程序仅支持64位系统，请运行32位的版本");
				Environment.Exit(1);
			}

			if (os.Platform != PlatformID.Win32NT)
			{
				Console.Error.WriteLine("本程序不支持该系统");
				Environment.Exit(1);
			}

			if (version.Major == 10)
			{
				return SupportedOS.Windows10;
			}
			if (version.Major == 6 && version.Minor == 3)
			{
				return SupportedOS.Windows8_1;
			}

			Console.Error.WriteLine("本程序不支持该系统");
			Environment.Exit(1);
			throw new InvalidProgramException("前面一句已经结束了程序");
		}
	}
}
