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
		RegistryHelper.Import(@"Resources\Kinds.reg");
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

	/// <summary>
	/// 该测试不对应代码，仅用于验证 Registry.GetValue 返回值的类型。
	/// </summary>
	[TestMethod]
	public void MyTestMethod()
	{
		using var key = Registry.CurrentUser.OpenSubKey("_Test_Kinds");

		Assert.AreEqual("文字文字", key.GetValue(""));

		Assert.AreEqual(0x123, key.GetValue("Dword"));

		Assert.AreEqual(0x666888L, key.GetValue("Qword"));

		CollectionAssert.AreEqual(new string[] { "Str0", "Str1" }, (string[])key.GetValue("Multi"));

		Assert.AreEqual(GetFolderPath(SpecialFolder.UserProfile), key.GetValue("Expand"));

		CollectionAssert.AreEqual(new byte[] { 0xFA, 0x51, 0x6F, 0x89 }, (byte[])key.GetValue("Binary"));
	}
}
