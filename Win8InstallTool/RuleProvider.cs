using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Win8InstallTool.Properties;
using Win8InstallTool.Rules;

// TODO: 本来想加个清理无效快捷方式的，Utils.GetShortcutTarget 都写好了，但Win8时代用不上所以算了。
// TODO: 开始菜单清除用户跟系统重复的项。
namespace Win8InstallTool
{
	internal struct RuleSet
	{
		public string Name;

		public IList<Rule> Rules;
	}

	/// <summary>
	/// 优化方案的提供者，调用 Scan() 方法检测所有可优化的地方。
	/// </summary>
	public sealed class RuleProvider
	{
		private readonly ICollection<RuleSet> ruleSets = new List<RuleSet>();

		private readonly bool includeSystem;

		public RuleProvider(bool includeSystem)
		{
			this.includeSystem = includeSystem;
		}

		public int ProgressMax { get; private set; }

		// 在新版 .NET 里事件参数无需继承 EventArg，这里也就不继承了
		public event EventHandler<int> OnProgress;

		internal void Initialize()
		{
			LoadRuleFile("开始菜单（用户）", Resources.StartupRules, r => new StartupMenuRule(false, r.Read()));
			LoadRuleFile("右键菜单 - 发送到", Resources.SendToRules, ReadSendTo);

			if (includeSystem)
			{
				// 这是什么奇怪的写法，好像内部属性跟外层平级了
				ruleSets.Add(new RuleSet
				{
					Name = "其他优化项",
					Rules = new List<Rule>
					{
						new LLDPServiceSecurityRule(),
						new CrashDumpRule(),
						new PerfCounterRule(),
						new SchannelRule(),
						new OpenWithNotepadRule(),
					}
				});

				LoadRuleFile("性能数据收集器", Resources.WMILoggerRules, ReadWmiLogger);
				LoadRuleFile("组策略", Resources.GroupPolicyRules, ReadGroupPolicy);
				LoadRuleFile("右键菜单清理", Resources.ContextMenuRules, ReadContextMenu);
				LoadRuleFile("系统服务", Resources.ServiceRules, ReadService);
				LoadRuleFile("任务计划程序", Resources.TaskSchdulerRules, ReadTask);
				LoadRuleFile("开始菜单（系统）", Resources.StartupRules, r => new StartupMenuRule(true, r.Read()));
			}

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

		// 下面是各种规则的加载逻辑，为了省点字把 Rule 后缀去掉了（ReadTaskRule -> ReadTask）

		static Rule ReadService(RuleFileReader reader)
		{
			return new ServiceRule(reader.Read(), reader.Read());
		}

		static Rule ReadSendTo(RuleFileReader reader)
		{
			return new SendToRule(reader.Read(), reader.Read());
		}

		static Rule ReadTask(RuleFileReader reader)
		{
			return new TaskSchedulerRule(reader.Read(), reader.Read(), reader.Read() == ":DISABLE");
		}

		static Rule ReadContextMenu(RuleFileReader reader)
		{
			var item = reader.Read();
			var name = reader.Read();
			var description = reader.Read();
			var directive = reader.Read();

			IList<string> folders;

			if (directive == ":SEARCH")
			{
				var folder = reader.Read();
				var root = Path.Combine("HKEY_CLASSES_ROOT", folder);

				folders = RegistryHelper.Search(root, item)
					.Select(name => Path.Combine(folder, name))
					.ToList();
			}
			else
			{
				folders = reader.Drain().ToList();
				folders.Add(directive);
			}

			return new ContextMenuRule(item, folders, name, description);
		}

		static Rule ReadGroupPolicy(RuleFileReader reader)
		{
			return new GroupPolicyRule(reader.Read(), reader.Read(), reader.Read(), reader.Read(), reader.Read());
		}

		static Rule ReadWmiLogger(RuleFileReader reader)
		{
			var name = reader.Read();
			var description = reader.Read();
			var key = reader.Read();

			var cy = reader.Read();
			bool? cycle = cy == "null" ? null : bool.Parse(cy);

			var fs = reader.Read();
			int? fileSize = fs == "null" ? null : int.Parse(fs);

			return new WMILoggerRule(name, description, key, cycle, fileSize);
		}
	}
}
