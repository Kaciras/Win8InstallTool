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
}
