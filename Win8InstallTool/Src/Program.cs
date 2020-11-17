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
			var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine($"Kaciras 的系统优化工具 v{appVersion.ToString(3)}");

			var clrVersion = Environment.Version;
			Console.WriteLine($".Net Version: {clrVersion.Major}.{clrVersion.Minor}");

			CheckOSSupport();

            //HandleRuleGroup(InternalRuleList.ContextMenuRules());
            //HandleRuleGroup(InternalRuleList.SystemServiceRules());
            HandleRuleGroup(InternalRuleList.RegistryTaskRules());
            HandleRuleGroup(InternalRuleList.OtherRules());

			Console.WriteLine("优化完毕！可能需要重启下系统哦。");
			Console.ReadKey();
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
			
			throw new InvalidProgramException("前面一句已经结束了程序");
		}

		static void  HandleRuleGroup(List<Rule> rules)
        {
			foreach (var rule in rules)
			{
				if (!rule.Check())
				{
					continue;
				}
				Console.WriteLine(rule.Description);

				var invalidInput = true;
				while (invalidInput)
				{
					Console.Write("是否执行该优化？[Y/n]");
					invalidInput = false;

					switch (Console.ReadLine().ToLower())
					{
						case "":
						case "y":
							rule.Optimize();
							break;
						case "n":
							break;
						default:
							invalidInput = true;
							Console.WriteLine("无效的输入，请重选");
							break;
					}
				}
			}
		}
	}
}
