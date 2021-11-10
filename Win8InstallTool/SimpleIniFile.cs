using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Win8InstallTool;

/// <summary>
/// 使用 Windows 自带的底层 API 读取 INI 配置文件。
/// </summary>
/// <seealso cref="https://stackoverflow.com/a/14906422/7065321"/>
sealed class SimpleIniFile
{
	readonly string EXE = Assembly.GetExecutingAssembly().GetName().Name;

	readonly string path;

	public SimpleIniFile(string path)
	{
		this.path = path;
	}

	// 好像没法读取无 Section 的键
	public string Read(string section, string key, string @default)
	{
		var RetVal = new StringBuilder(255);
		GetPrivateProfileString(section ?? EXE, key, @default, RetVal, 255, path);
		return RetVal.ToString();
	}

	[DllImport("kernel32", CharSet = CharSet.Unicode)]
	static extern int GetPrivateProfileString(
		string appName,
		string keyName,
		string @default,
		StringBuilder reurnString,
		int nSize, string filename);
}
