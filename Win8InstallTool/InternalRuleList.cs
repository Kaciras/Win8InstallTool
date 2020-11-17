using System.Collections.Generic;
using System.IO;
using System.Linq;
using Win8InstallTool.Rules;

namespace Win8InstallTool
{
	/// <summary>
	/// 服务规则集列表
	/// </summary>
	internal static class InternalRuleList
	{
		public static List<Rule> SystemServiceRules() => new List<Rule>
			{
				new ServiceRule
				{
					Key = "HomeGroupProvider",
					Description = "不使用家庭组功能的可以禁用",
				},
				new ServiceRule
				{
					Key = "hidserv",
					Description = "不用特殊输入设备的不需要,该服务还容易被攻击者利用",
				},
				new ServiceRule
				{
					Key = "ShellHWDetection",
					Description = "这年头谁还用光盘自动播放,该服务还容易被攻击者利用",
				},
				new ServiceRule
                {
					Key = "LanmanServer",
					Description = "服务器文件共享才需要,家用机关掉",
				},
				new ServiceRule
				{
					Key = "LanmanWorkstation",
					Description = "服务器文件共享才需要,家用机关掉",
				},
				new ServiceRule
				{
					Key = "lmhosts",
					Description = "办公室远程控制打印机、文件才需要",
				},
				new ServiceRule
				{
					Key = "WSearch",
					Description = "系统自带的搜索功能,并不是每个人都需要,还耗磁盘",
				},
				new ServiceRule
				{
					Key = "AeLookupSvc",
					Description = "我也不知道这服务有啥用,反正关了也没出事",
				},
				new ServiceRule
				{
					Key = "Spooler",
					Description = "打印机的服务,没必要一直启动",
					TargetState = ServiceState.Manual,
				},
				new ServiceRule
				{
					Key = "PcaSvc",
					Description = "啥卵用都没有的兼容性助手",
				},
				new ServiceRule
				{
					Key = "IKEEXT",
					Description = "[注意] VPN需要这个服务",
				},
				new ServiceRule
				{
					Key = "DPS",
					Description = "出问题不百度,要你来诊断?",
				},
				new ServiceRule
				{
					Key = "WdiServiceHost",
					Description = "出问题不百度,要你来诊断?",
				},
				new ServiceRule
				{
					Key = "DiagTrack",
					Description = "出问题不百度,要你来诊断?",
				},
				new ServiceRule
				{
					Key = "WPDBusEnum",
					Description = "反正我是没用过什么可移动大容量存储设备",
				},
				new ServiceRule
				{
					Key = "TrkWks",
					Description = "没几个人会把NTFS文件链接到远程计算机",
				},
				new ServiceRule
				{
					Key = "PolicyAgent",
					Description = "关掉后防火墙不能使用IPSec策略,不过个人机一般用不着",
				},
				new ServiceRule
				{
					Key = "SSDPSRV",
					Description = "Upnp设备个人机不常用",
				},
				new ServiceRule
				{
					Key = "iphlpsvc",
					Description = "提供通知的服务,没啥卵用,而且此服务依赖IP Helper,在禁用IP Helper后会报错",
				},
				new ServiceRule
				{
					Key = "WinHttpAutoProxySvc",
					Description = "该服务使应用程序支持WPAD协议的应用,因为大多数的情况下不会用到.建议关闭",
				},
				new ServiceRule
				{
					Key = "SCardSvr",
					Description = "智能卡这东西一般用户用不到,你要没有就禁掉",
				}
			};

		public static List<Rule> OtherRules() => new List<Rule>
			{
				new SchannelRule(),
				new OpenWithNotepadRule(),
			};

		public static List<Rule> ContextMenuRules()
		{
			var result = new List<Rule> {
			// 兼容性疑难解答
			new ContextMenuRule(@"exefile\shellex\ContextMenuHandlers\Compatibility"),

			// 固定到任务栏
			new ContextMenuRule(@"*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}"),

			// Open in Visual Studio
			new ContextMenuRule(@"Directory\shell\AnyCode"),
			new ContextMenuRule(@"Directory\background\shell\AnyCode"),

			// 包含到库中
			new ContextMenuRule(@"Folder\shellex\ContextMenuHandlers\Library Location"),

			// 各种文件的 "编辑"，因为已经搞了记事本打开所以它多余了
			new ContextMenuRule(@"cmdfile\shell\edit"),
			new ContextMenuRule(@"batfile\shell\edit"),
			new ContextMenuRule(@"xmlfile\shell\edit"),
			new ContextMenuRule(@"regfile\shell\edit"),
			new ContextMenuRule(@"SystemFileAssociations\text\shell\edit"),
		};
			// 设为桌面背景
			var wallpapers = RegSearch(@"HKEY_CLASSES_ROOT\SystemFileAssociations", @"Shell\setdesktopwallpaper");
			foreach (var item in wallpapers)
			{
				result.Add(new ContextMenuRule(@"SystemFileAssociations\" + item));
			}

			return result;
		}

		// 只搜一层
		public static List<string> RegSearch(string root, string key)
		{
			using var rootKey = RegistryHelper.OpenKey(root);
			return rootKey.GetSubKeyNames()
				.Select(name => Path.Combine(name, key))
				.Where(path => rootKey.ContainsSubKey(path))
				.ToList();
		}

		public static List<Rule> RegistryTaskRules()
		{
			return new List<Rule>
			{
				// 用户体验改善计划相关
				new TaskItemRule
				{
					Path = @"\Microsoft\Windows\Application Experience\AitAgent",
					Description = "没同意客户体验改善计划的可以删了",
				},
				new TaskItemRule
				{
					Path = @"\Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser",
					Description = "微软收集用户电脑性能信息的任务",
				},
				new TaskItemRule
				{
					Path = @"\Microsoft\Windows\Application Experience\ProgramDataUpdater",
					Description = "没同意客户体验改善计划的可以删了",
				},
				new TaskItemRule
				{
					Path = @"\Microsoft\Windows\Application Experience\StartupAppTask",
					Description = "启动项应该自己注意,而不是靠它来扫描",
					ShouldDelete = false,
				},
				new TaskDirectoryRule
				{
					Path = @"\Microsoft\Windows\Autochk",
					//Name = "Proxy",
					Description = "没同意客户体验改善计划的可以删了",
				},
				new TaskDirectoryRule // 删不掉？
				{
					Path = @"Microsoft\Windows\Customer Experience Improvement Program",
					Description = "全是客户体验改善计划,不参加的直接删",
				},


				new TaskItemRule
				{
					Path = @"\Microsoft\Windows\DiskCleanup\SilentCleanup",
					Description = "磁盘空间不足还用你来提示?",
					ShouldDelete = false,
				},
				new TaskDirectoryRule
				{
					Path = @"\Microsoft\Windows\SkyDrive",
					Description = "SkyDrive目录，里头两个同步任务是万恶之源,老在日志里报错",
				},
				new TaskItemRule
				{
					Path = @"\Microsoft\Windows\UPnP\UPnPHostConfig",
					Description = "用不用UPnp不是你说了算,还敢在后台偷偷改",
					ShouldDelete = false,
				},
				new TaskItemRule
				{
					Path = @"\Microsoft\Office\Office 15 Subscription Heartbeat",
					Description = "如果你没有订阅Office就可以删除此任务",
				}
			};
		}
	}
}
