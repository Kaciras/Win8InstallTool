using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win8InstallTool.Test
{
	[TestClass]
	public class SimpleIniFileTest
	{
		[TestMethod]
		public void ReadNonExists()
		{
			var file = new SimpleIniFile("Resources/Config.ini");
			Assert.AreEqual(string.Empty, file.Read(null, "non-exists", null));
		}

		[TestMethod]
		public void Read()
		{
			var file = new SimpleIniFile("Resources/Config.ini");
			Assert.AreEqual("hello world", file.Read("server", "key", null));
		}
	}
}
