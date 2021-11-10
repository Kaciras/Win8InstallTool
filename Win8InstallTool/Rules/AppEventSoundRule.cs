namespace Win8InstallTool.Rules;

/// <summary>
/// 优化 控制面板/硬件和声音/更改系统声音/声音方案 的规则，如果当前方案与目标不同则修改为目标方案。
/// </summary>
internal sealed class AppEventSoundRule : Rule
{
	const string ROOT = @"HKCU\AppEvents\Schemes";

	public string Name { get; }

	public string Description => "将 Windows 系统声音设为指定的方案";

	readonly string target;

	/// <summary>
	/// 创建规则的实例，将声音方案调整为制定的项，方案名是注册表里的键名。
	/// </summary>
	/// <param name="target">方案名</param>
	public AppEventSoundRule(string target)
	{
		this.target = target;

		using var schemes = RegistryHelper.OpenKey(@$"{ROOT}\Names\{target}");
		Name = "设置系统音效为：" + schemes.GetValue("");
	}

	public bool Check()
	{
		using var schemes = RegistryHelper.OpenKey(ROOT);
		return !schemes.GetValue("").Equals(target);
	}

	public void Optimize()
	{
		using var schemes = RegistryHelper.OpenKey(ROOT, true);

		schemes.SetValue("", target);

		using var apps = schemes.OpenSubKey("Apps");
		foreach (var appName in apps.GetSubKeyNames())
		{
			using var app = apps.OpenSubKey(appName);
			foreach (var item in app.GetSubKeyNames())
			{
				using var key = app.OpenSubKey(item + @"\" + target);
				if (key == null) continue;

				using var current = app.OpenSubKey(item + @"\.Current", true);
				current.SetValue("", key.GetValue(""));
			}
		}
	}
}
