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

        // 在新版 .NET 里事件参数无需继承 EventArg，这里也就不继承了
        public event EventHandler<int> OnProgress;

        internal void Initialize()
        {
            LoadRuleFile("右键菜单清理", Resources.ContextMenuRules, ReadContextMenuRules);
            LoadRuleFile("系统服务", Resources.ServiceRules, ReadServiceRules);
            LoadRuleFile("任务计划程序", Resources.TaskSchdulerRules, ReadTaskRules);
            LoadRuleFile("开始菜单", Resources.StartupRules, r => new StartupMenuRule(r.Read()));

            // 这是什么奇怪的写法，好像内部属性跟外层平级了
            ruleSets.Add(new RuleSet
            {
                Name = "其他优化项",
                Rules = new List<Rule>
                {
                    new CrashDumpRule(),
                    new SchannelRule(),
                    new OpenWithNotepadRule(),
                }
            });


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

        void LoadRuleFile(string name, string content, Func<RuleFileReader, Rule> func)
        {
            var reader = new RuleFileReader(content);
            var rules = new List<Rule>();
            while (reader.MoveNext())
            {
                rules.Add(func(reader));
            }
            ruleSets.Add(new RuleSet { Name = name, Rules = rules });
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
                return new TaskSchedulerRule(reader.Read(), reader.Read(), true);
            }
            else
            {
                return new TaskSchedulerRule(first, reader.Read(), false);
            }
        }

        static Rule ReadContextMenuRules(RuleFileReader reader)
        {
            var item = reader.Read();
            var context = reader.Read();
            var description = reader.Read();

            var directive = reader.Read();
            IList<string> folders;

            if (directive[0] == ':')
            {
                var root = Path.Combine("HKEY_CLASSES_ROOT", reader.Read());
                folders = RegistryHelper.Search(root, item);
            }
            else
            {
                folders = reader.Drain().ToList();
                folders.Add(directive);
            }

            return new ContextMenuRule(item, folders, context, description);
        }
    }
}
