using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: InternalsVisibleTo("Test")]
namespace Win8InstallTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"Kaciras 的系统优化工具 v{appVersion.ToString(3)}");

            var clrVersion = Environment.Version;
            Console.WriteLine($".Net Version: {clrVersion.Major}.{clrVersion.Minor}");

            CheckOSSupport();

            Application.Run(new MainWindow());
        }

		/// <summary>
		/// 检查本程序是否支持所在的系统，如果不支持则会终止程序并返回退出码1，支持则返回系统类型。
		/// </summary>
		/// <returns>支持的系统类型</returns>
		static void CheckOSSupport()
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
				Console.Error.WriteLine("本程序仅支持Windows8");
				Environment.Exit(1);
			}

			if (version.Major != 6 && version.Minor != 3)
			{
				Console.Error.WriteLine("本程序仅支持Windows8");
				Environment.Exit(1);
			}
		}
	}
}
