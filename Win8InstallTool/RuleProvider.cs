using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Win8InstallTool.Properties;
using Win8InstallTool.Rules;

namespace Win8InstallTool;

/// <summary>
/// 优化方案的提供者，调用 Scan() 方法检测所有可优化的地方。
/// </summary>
public sealed class RuleProvider
{
	public ICollection<OptimizableSet> RuleSets { get; } = new List<OptimizableSet>();

	readonly bool includeSystem;

	public RuleProvider(bool includeSystem)
	{
		this.includeSystem = includeSystem;
	}

	internal void Initialize()
	{
		LoadRuleFile("系统设置（用户）", Resources.UserRegistryRules, ReadRegistry);
		LoadRuleFile("开始菜单（用户）", Resources.StartupRules, r => new StartupMenuRule(false, r.Read(), r.Read()));
		RuleSets.Add(new SendToRuleSet());

		var others = new List<Rule>
			{
				new AppEventSoundRule(".None"),
			};

		if (includeSystem)
		{
			others.Add(new ExplorerFolderRule());
			others.Add(new LLDPSecurityRule());
			others.Add(new RegFileRule("把用记事本打开添加到右键菜单", "很常用的功能，不解释", GetEmbeddedRegFile("OpenWithNotepad")));

			RuleSets.Add(new TaskSchedulerOptimizeSet());

			LoadRuleFile("性能数据收集器", Resources.WMILoggerRules, ReadWmiLogger);
			LoadRuleFile("组策略", Resources.GroupPolicyRules, ReadGroupPolicy);
			LoadRuleFile("右键菜单清理", Resources.ContextMenuRules, ReadContextMenu);
			LoadRuleFile("服务", Resources.ServiceRules, ReadService);
			LoadRuleFile("开始菜单", Resources.StartupRules, r => new StartupMenuRule(true, r.Read(), r.Read()));
			LoadRuleFile("系统设置", Resources.RegistryRules, ReadRegistry);
		}

		RuleSets.Add(new RuleList("其它优化项", () => others));
	}

	void LoadRuleFile(string name, string content, Func<RuleFileReader, Rule> func)
	{
		IEnumerable<Rule> factory() => RuleFileReader.Iter(content).Select(func).ToList();
		RuleSets.Add(new RuleList(name, factory));
	}

	// 下面是各种规则的加载逻辑，为了省点字把 Rule 后缀去掉了（ReadTaskRule -> ReadTask）

	static Rule ReadRegistry(RuleFileReader reader)
	{
		return new RegFileRule(reader.Read(), reader.Read(), GetEmbeddedRegFile(reader.Read()));
	}

	static string GetEmbeddedRegFile(string name)
	{
		name = $"Win8InstallTool.RegFiles.{name}.reg";
		var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}

	static Rule ReadService(RuleFileReader reader)
	{
		return new ServiceRule(reader.Read(), reader.Read(),
			(ServiceState)Enum.Parse(typeof(ServiceState), reader.Read()));
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
