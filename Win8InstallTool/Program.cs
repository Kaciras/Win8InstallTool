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
                Application.Run(new MainWindow());
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
		/// 检查本程序是否支持所在的系统，如果不支持则会终止程序并返回退出码1，支持则返回系统类型。
		/// </summary>
		/// <returns>支持的系统类型</returns>
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
