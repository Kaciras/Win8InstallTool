using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	// TODO: 跟 CrashDumpRule、SchannelRule 一样都是一个简单的注册表项，你不能抽象一下？
	public sealed class PerfCounterRule : ImutatableRule
	{
		const string KEY = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\PerfNet\Performance";

		public override string Name => "禁用性能计数器";

		public override string Description => "统计系统底层性能的东西，一般人用不到，还老是报错 “无法打开服务器服务性能对象……”";

		public override void Optimize()
		{
			Registry.SetValue(KEY, "Disable Performance Counters", 1, RegistryValueKind.DWord);
		}

		protected override bool Check()
		{
			return !Registry.GetValue(KEY, "Disable Performance Counters", 0).Equals(1);
		}
	}
}
