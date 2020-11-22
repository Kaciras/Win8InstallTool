using System;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Windows.Forms;

[assembly: InternalsVisibleTo("Test")]
namespace Win8InstallTool
{
    static class Program
    {
        internal static bool IsElevated { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (CheckOSSupport())
            {
                StartProgram();
            }
            else
            {
                MessageBox.Show(
                    "本程序仅支持 64 位 Windows8.1",
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

        
        static void StartProgram()
		{
            /*
             * 获取当前用户，并检测是否具有管理员权限。
             * https://stackoverflow.com/a/5953294/7065321
             */
            string currentUser;

            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                currentUser = identity.Name;
                IsElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            var provider = new RuleProvider(currentUser, IsElevated);
            provider.Initialize();

            Application.Run(new MainWindow(provider));
        }
	}
}
