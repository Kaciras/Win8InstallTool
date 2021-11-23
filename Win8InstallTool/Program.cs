﻿using System;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

[assembly: InternalsVisibleTo("Test")]
[assembly: InternalsVisibleTo("Benchmark")]
namespace Win8InstallTool;

static class Program
{
	/// <summary>
	/// 程序是否具有管理员权限
	/// </summary>
	internal static bool IsElevated { get; private set; }

	[STAThread]
	static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		if (CheckOSSupport())
		{
			IsElevated = Utils.CheckIsAdministrator();
			var provider = new RuleProvider(IsElevated);
			provider.Initialize();

			Application.Idle += CaptureSyncContext;
			Application.Run(new MainWindow(provider));
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

	/// <summary>
	/// 初始化 STAExecutor，直接复用 WinForm 的同步上下文。
	/// </summary>
	private static void CaptureSyncContext(object sender, EventArgs e)
	{
		Application.Idle -= CaptureSyncContext;
		STAExecutor.SetSyncContext(SynchronizationContext.Current);
	}
}
