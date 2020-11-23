using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Environment;

namespace Win8InstallTool.Test
{
	[TestClass]
	public class UtilsTest
	{
		[DataRow("Resources/Calculator.lnk", @"\system32\calc.exe")]
		[DataRow("Resources/Excel 2016.lnk", @"\Installer\{90160000-0011-0000-1000-0000000FF1CE}\xlicons.exe")]
		[DataTestMethod]
		public void GetShortcutTarget(string lnk, string subpath)
		{
			var windir = GetFolderPath(SpecialFolder.Windows);
			var target = Utils.GetShortcutTarget(lnk);
			Assert.AreEqual(windir + subpath, target);
		}

		[DataRow("not/exists/folder")]
		[DataRow("Resources/Calculator")]
		[DataTestMethod]
		public void GetShortcutTargetException(string file)
		{
			Assert.ThrowsException<FileNotFoundException>(() => Utils.GetShortcutTarget(file));
		}

		[Ignore("总是出现莫名其妙的错误 2 or 1008")]
		[TestMethod]
		public void ExtractStringFromDLL()
		{
			var text = Utils.ExtractStringFromDLL(@"shell32.dll", 51330);
			Assert.AreEqual("解决所选项的同步错误", text);
		}

		[TestMethod]
		public void CreateTempFile()
		{
			Utils.TempFileSession file;

			using (file = Utils.CreateTempFile())
			{
				Assert.IsTrue(File.Exists(file.Path));
			}
			Assert.IsFalse(File.Exists(file.Path));
		}
	}
}
