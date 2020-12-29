using System.ServiceProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Win8InstallTool.Rules;

namespace Win8InstallTool.Test
{
	[TestClass]
	public sealed class ServiceRuleTest
	{
		[TestInitialize]
		public void Init()
		{
			new WinSvcApi("Win8InstallToolTest", @"C:\foobar.exe").Install();
		}

		[TestCleanup]
		public void Cleanup()
		{
			WinSvcApi.Uninstall("Win8InstallToolTest");
		}

		[TestMethod]
		public void Optimize()
		{
			var rule = new ServiceRule("Win8InstallToolTest", "descr", ServiceState.Disabled);
			Assert.IsTrue(rule.Check());

			rule.Optimize();
			using var controller = new ServiceController("Win8InstallToolTest");
			Assert.AreEqual(controller.StartType, ServiceStartMode.Disabled);
		}
	}
}
