using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System.IO;
using static System.Environment;

namespace Win8InstallTool.Rules;

[TestClass]
public sealed class RegFileRuleTest
{
	[TestInitialize]
	public void ImportTestData()
	{
		RegHelper.Import(@"Resources\Kinds.reg");
	}

	[TestCleanup]
	public void Cleanup()
	{
		Registry.CurrentUser.DeleteSubKeyTree("_Test_Kinds", false);
		Registry.CurrentUser.DeleteSubKeyTree("_Test_Import", false);
	}

	[TestMethod]
	public void CheckNoNeeded()
	{
		var content = File.ReadAllText(@"Resources\Kinds.reg");
		var rule = new RegFileRule("test", "test", content);
		Assert.IsFalse(rule.Check());
	}

	[TestMethod]
	public void Check()
	{
		var content = File.ReadAllText(@"Resources\ImportTest.reg");
		var rule = new RegFileRule("test", "test", content);
		Assert.IsTrue(rule.Check());
	}

	[TestMethod]
	public void Optimize()
	{
		var content = File.ReadAllText(@"Resources\ImportTest.reg");
		var rule = new RegFileRule("test", "test", content);

		rule.Optimize();

		var value = Registry.GetValue(@"HKEY_CURRENT_USER\_Test_Import\Key", "StringValue", null);
		Assert.AreEqual("中文内容", value);
	}
}
