using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("Benchmark")]
namespace Win8InstallTool;

/*
 * 为了实现的简洁，本程序假设用户的系统满足以下条件：
 * 1）没有手动修改过注册表等底层数据，只用系统提供的控制中心调整配置，这避免了各种边界情况。
 * 2）在扫描和优化两个操作之间不对系统设置做修改，这避免了不一致的状态。
 */
static class Program
{
	[STAThread]
	static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		if (!CheckOSSupport())
		{
			MessageBox.Show(
				"本程序仅支持 64 位 Windows8.1",
				"无法启动",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}
		else
		{
			var elevated = Utils.CheckIsAdministrator();
			var provider = new RuleProvider(elevated);
			provider.Initialize();

			Application.Idle += CaptureSyncContext;
			Application.Run(new MainWindow(provider));
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

	/// <summary>
	/// 初始化 STAExecutor，直接复用 WinForm 的同步上下文。
	/// </summary>
	private static void CaptureSyncContext(object sender, EventArgs e)
	{
		Application.Idle -= CaptureSyncContext;
		STAExecutor.SetSyncContext(SynchronizationContext.Current);
	}
}
