using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.IO;

namespace Win8InstallTool;

[TestClass]
public sealed class RegHelperTest
{
	[TestCleanup]
	public void Cleanup()
	{
		Registry.CurrentUser.DeleteSubKeyTree("_Test_Import", false);
		Registry.CurrentUser.DeleteSubKeyTree("_Test_AutoConvert", false);
	}

	[DataRow(@"INVALID\Sub")]
	[DataRow(@"INVALID")]
	[DataTestMethod]
	public void OpenKeyNonExists(string path)
	{
		Assert.IsNull(RegHelper.OpenKey(path));
	}

	[ExpectedException(typeof(DirectoryNotFoundException))]
	[TestMethod]
	public void GetCLSIDValueException()
	{
		RegHelper.GetCLSIDValue("{66666666-0000-0000-6666-000000000000}");
	}

	[TestMethod]
	public void GetCLSIDValue()
	{
		var value = RegHelper.GetCLSIDValue("{C7657C4A-9F68-40fa-A4DF-96BC08EB3551}");
		Assert.AreEqual("Photo Thumbnail Provider", value);
	}

	[DataRow(@"INVALID\Sub", false)]
	[DataRow(@"INVALID", false)]
	[DataRow(@"HKCR\CLSID", true)]
	[DataTestMethod]
	public void KeyExists(string path, bool expected)
	{
		Assert.AreEqual(expected, RegHelper.KeyExists(path));
	}

	[TestMethod]
	public void ContainsSubKey()
	{
		Assert.IsTrue(Registry.LocalMachine.ContainsSubKey(@"SOFTWARE\Microsoft"));
		Assert.IsFalse(Registry.LocalMachine.ContainsSubKey(@"SOFTWARE\Xicrosoft"));
	}

	[TestMethod]
	public void Import()
	{
		RegHelper.Import(@"Resources\ImportTest.reg");

		var value = Registry.GetValue(@"HKEY_CURRENT_USER\_Test_Import\Key", "StringValue", null);
		Assert.AreEqual("中文内容", value);
	}

	[TestMethod]
	public void Export()
	{
		using (var key = Registry.CurrentUser.CreateSubKey(@"_Test_Import\Key"))
		{
			key.SetValue("StringValue", "中文内容");
		}
		RegHelper.Export("ExportTest.reg", @"HKEY_CURRENT_USER\_Test_Import\Key");

		var actual = File.ReadAllBytes("ExportTest.reg");
		var expect = File.ReadAllBytes(@"Resources\ImportTest.reg");
		CollectionAssert.AreEqual(expect, actual);
	}

	/// <summary>
	/// 验证 Registry.SetValue() 无法直接接受 .reg 文件里的值格式，必须要先转换。
	/// </summary>
	[ExpectedException(typeof(ArgumentException))]
	[TestMethod]
	public void AutoConvertOnSetValue()
	{
		var key = @"HKEY_CURRENT_USER\_Test_AutoConvert";
		var text = "50,2d,02,09,60,d1,d6,01";
		var kind = RegistryValueKind.QWord;
		Registry.SetValue(key, "name", text, kind);
	}
}
