using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

[assembly: InternalsVisibleTo("Test")]
namespace Win8InstallTool
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (CheckOSSupport())
            {
                var provider = new RuleProvider();
                provider.Initialize();
                Application.Run(new MainWindow(provider));
            }
            else
            {
                MessageBox.Show(
                    "本程序仅支持64位Windows8.1",
                    "无法启动",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 检查所在的系统是否支持本程序，如果不支持则不应继续运行。
        /// </summary>
        static bool CheckOSSupport()
		{
			var os = Environment.OSVersion;
			var version = os.Version;

			return Environment.Is64BitOperatingSystem
				&& os.Platform == PlatformID.Win32NT
				&& version.Major == 6 && version.Minor == 3;
		}
	}
}
