using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace Win8InstallTool.Test
{
    [TestClass]
	public sealed class RegistryHelperTest
	{
		[TestMethod]
		public void GetCLSIDValue()
		{
			var value = RegistryHelper.GetCLSIDValue("{C7657C4A-9F68-40fa-A4DF-96BC08EB3551}");
			Assert.AreEqual("Photo Thumbnail Provider", value);
		}

		[TestMethod]
		public void ContainsKey()
		{
			Assert.IsTrue(Registry.LocalMachine.ContainsSubKey(@"SOFTWARE\Microsoft"));
			Assert.IsFalse(Registry.LocalMachine.ContainsSubKey(@"SOFTWARE\Xicrosoft"));
		}

		[TestMethod]
		public void Import()
		{
			RegistryHelper.Import(@"Resources\ImportTest.reg");

			try
			{
				var value = Registry.GetValue(@"HKEY_CURRENT_USER\Environment\Test_Win8Tool", "StringValue", null);
				Assert.AreEqual("foobar", value);
			}
			finally
			{
				Registry.CurrentUser.DeleteSubKeyTree(@"Environment\Test_Win8Tool", false);
			}
		}

		[TestMethod]
		public void Export()
		{
			using (var key = Registry.CurrentUser.CreateSubKey(@"Environment\Test_Win8Tool"))
			{
				key.SetValue("StringValue", "foobar");
			}
			try
			{
				RegistryHelper.Export("ExportTest.reg", @"HKEY_CURRENT_USER\Environment\Test_Win8Tool");

				var actual = File.ReadAllBytes("ExportTest.reg");
				var expect = File.ReadAllBytes(@"Resources\ImportTest.reg");
				CollectionAssert.AreEqual(expect, actual);
			}
			finally
			{
				Registry.CurrentUser.DeleteSubKeyTree(@"Environment\Test_Win8Tool", false);
			}
		}
	}
}
