using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using static System.Environment;

namespace Win8InstallTool.RegFile;

[TestClass]
public sealed class RegFileReaderTest
{
	[TestMethod]
	public void ReadKey()
	{
		var content = File.ReadAllText("Resources/ImportTest.reg");
		var reader = new RegFileReader(content);

		Assert.IsTrue(reader.Read());
		Assert.IsTrue(reader.IsKey);
		Assert.AreEqual(@"HKEY_CURRENT_USER\_Test_Import\Key", reader.Key);
	}

	[TestMethod]
	public void Multipart()
	{
		var content = File.ReadAllText("Resources/ValueParts.reg");
		var reader = new RegFileReader(content);

		Assert.IsTrue(reader.Read());
		Assert.IsTrue(reader.Read());

		var expected = new string[] { "Str0", "Str1" };
		CollectionAssert.AreEqual(expected, (string[])reader.Value);
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
