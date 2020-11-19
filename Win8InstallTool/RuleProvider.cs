using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Win8InstallTool.Properties;
using Win8InstallTool.Rules;

namespace Win8InstallTool
{
    internal struct RuleSet
    {
        public string Name;

        public IList<Rule> Rules;
    }

    public sealed class RuleProvider
    {
        private readonly ICollection<RuleSet> ruleSets = new List<RuleSet>();

        public int ProgressMax { get; private set; }

        public event EventHandler<int> OnProgress;

        internal void Initialize()
        {
            var rules = LoadRuleFile(Resources.ServiceRules, ReadServiceRules);
            ruleSets.Add(new RuleSet { Name = "系统服务", Rules = rules });

            rules = LoadRuleFile(Resources.TaskSchdulerRules, ReadTaskRules);
            ruleSets.Add(new RuleSet { Name = "任务计划程序", Rules = rules });

            rules = LoadRuleFile(Resources.StartupRules, r => new StartupMenuRule(r.Read()));
            ruleSets.Add(new RuleSet { Name = "开始菜单", Rules = rules });

            // 这是什么奇怪的写法，好像内部属性跟外层平级了
            ruleSets.Add(new RuleSet
            {
                Name = "其他优化项",
                Rules = new List<Rule>
            {
                new SchannelRule(),
                new OpenWithNotepadRule(),
            }
            });

            ruleSets.Add(ContextMenuRules());

            ProgressMax = ruleSets.Sum(set => set.Rules.Count);
        }

        public IEnumerable<OptimizeSet> Scan()
        {
            var progress = 0;

            Optimizable DoScan(Rule rule)
            {
                var value = rule.Scan();
                OnProgress?.Invoke(this, ++progress);
                return value;
            }

            foreach (var ruleSet in ruleSets)
            {
                var items = ruleSet.Rules
                    .Select(rule => DoScan(rule))
                    .Where(o => o != null);

                yield return new OptimizeSet(ruleSet.Name, items);
            }
        }

        static RuleSet ContextMenuRules()
        {
            var result = new List<Rule> {
			// 兼容性疑难解答
			new ContextMenuRule(@"exefile\shellex\ContextMenuHandlers\Compatibility"),

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
            var wallpapers = RegistryHelper.Search(@"HKEY_CLASSES_ROOT\SystemFileAssociations", @"Shell\setdesktopwallpaper");
            foreach (var item in wallpapers)
            {
                result.Add(new ContextMenuRule(@"SystemFileAssociations\" + item));
            }

            return new RuleSet { Name = "右键菜单清理", Rules = result };
        }

        static List<Rule> LoadRuleFile(string content, Func<RuleFileReader, Rule> func)
        {
            var reader = new RuleFileReader(content);
            var rules = new List<Rule>();
            while (reader.MoveNext())
            {
                rules.Add(func(reader));
            }
            return rules;
        }

        static Rule ReadServiceRules(RuleFileReader reader)
        {
            return new ServiceRule(reader.Read(), reader.Read());
        }

        static Rule ReadTaskRules(RuleFileReader reader)
        {
            var first = reader.Read();
            var disable = first == ":DISABLE";

            if (disable)
            {
                return new TaskSchdulerRule(reader.Read(), reader.Read(), true);
            }
            else
            {
                return new TaskSchdulerRule(first, reader.Read(), false);
            }
        }
    }
}
