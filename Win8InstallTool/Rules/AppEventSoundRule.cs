using Microsoft.Win32;

namespace Win8InstallTool.Rules;

internal sealed class AppEventSoundRule : Rule
{
	public string Name { get; }

	public string Description => "将 Windows 系统声音设为指定的方案";

	readonly string target;

	public AppEventSoundRule(string target)
	{
		this.target = target;

		using var schemes = Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes\Names\" + target);
		Name = "设置系统音效为：" + schemes.GetValue("");
	}

	public bool Check()
	{
		using var schemes = Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes");
		return !schemes.GetValue("").Equals(target);
	}

	public void Optimize()
	{
		using var schemes = Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes", true);

		using var apps = schemes.OpenSubKey("Apps");
		foreach (var appName in apps.GetSubKeyNames())
		{
			using var app = apps.OpenSubKey(appName);
			foreach (var item in app.GetSubKeyNames())
			{
				using var key = app.OpenSubKey(item + @"\" + target);
				if (key == null) continue;

				var value = key.GetValue("");

				using var current = app.OpenSubKey(item + @"\.Current", true);
				current.SetValue("", value);
			}
		}

		schemes.SetValue("", target);
	}
}
