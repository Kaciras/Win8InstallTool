using System.Collections.Generic;
using System.IO;
using System.Linq;
using Win8InstallTool.Properties;
using Win8InstallTool.Rules;

namespace Win8InstallTool
{
    /// <summary>
    /// 服务规则集列表
    /// </summary>
    internal static class InternalRuleList
    {
        public static IEnumerable<RuleSet> Scan()
        {
            yield return ScanSystemService();
            yield return OtherRules();
        }

        public static RuleSet ScanSystemService()
        {
            var rules = new List<ServiceRule>
            {
                new ServiceRule("HomeGroupProvider", "不使用家庭组功能的可以禁用"),
                new ServiceRule("hidserv", "不用特殊输入设备的不需要,该服务还容易被攻击者利用"),
                new ServiceRule("ShellHWDetection", "这年头谁还用光盘自动播放,该服务还容易被攻击者利用"),
                new ServiceRule("LanmanServer","服务器文件共享才需要,家用机关掉"),
                new ServiceRule("LanmanWorkstation","服务器文件共享才需要,家用机可以关掉"),
                new ServiceRule("lmhosts","办公室远程控制打印机、文件才需要"),
                new ServiceRule("WSearch","系统自带的搜索功能,并不是每个人都需要,还耗磁盘"),
                new ServiceRule("AeLookupSvc","我也不知道这服务有啥用,反正关了也没出事"),
                new ServiceRule( "PcaSvc","啥卵用都没有的兼容性助手"),
                new ServiceRule( "IKEEXT","[注意] VPN需要这个服务"),
                new ServiceRule("DPS","出问题不谷歌,要你来诊断?这个关了后另外两个也不会启动"),
                new ServiceRule( "WPDBusEnum", "反正我是没用过什么可移动大容量存储设备"),
                new ServiceRule("TrkWks","没几个人会把NTFS文件链接到远程计算机"),
                new ServiceRule("PolicyAgent","关掉后防火墙不能使用IPSec策略,不过个人机一般用不着"),
                new ServiceRule("SSDPSRV","Upnp设备个人机不常用"),
                new ServiceRule("iphlpsvc","我从没用过它说的这些功能"),
                new ServiceRule("WinHttpAutoProxySvc","该服务使应用程序支持WPAD协议的应用,因为大多数的情况下不会用到.建议关闭"),
                new ServiceRule("SCardSvr","智能卡这东西一般用户用不到,你要没有就禁掉"),
            };

            return new RuleSet { Name = "系统服务", Items = rules.Where(r => r.Check()) };
        }

        public static RuleSet OtherRules()
        {
            var rules = new List<Rule>
            {
                new SchannelRule(),
                new OpenWithNotepadRule(),
            };

            return new RuleSet { Name = "其他优化项", Items = rules.Where(r => r.Check()) };
        }

        public static RuleSet StartupRules()
        {
            var rules = new List<Rule>
            {
                new StartupMenuRule("WinRAR", "都是些从来不用的垃圾"),
                new StartupMenuRule("Microsoft Office 2016 工具", "都是些从来不用的垃圾"),
                new StartupMenuRule("Windows Kits", "都是些从来不用的垃圾"),
                new StartupMenuRule("Music, Photos and Videos", "都是些从来不用的垃圾"),
                new StartupMenuRule("Visual Studio 2019", "都是些从来不用的垃圾"),
            };

            return new RuleSet { Name = "开始菜单", Items = rules.Where(r => r.Check()) };
        }

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
            var reader = new RuleFileReader(Resources.TaskSchdulerRules);
            var rules = new List<TaskSchdulerRule>();

            while (reader.MoveNext())
            {
                var first = reader.Read();
                var disable = first == ":DISABLE";

                if(disable)
                {
                    rules.Add(new TaskSchdulerRule(reader.Read(), reader.Read(), true));
                }
                else
                {
                    rules.Add(new TaskSchdulerRule(first, reader.Read(), false));
                }
            }


        }
    }
}
