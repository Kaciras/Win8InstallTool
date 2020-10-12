﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Win8InstallTool;

namespace Test
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
				Registry.CurrentUser.DeleteSubKeyTree(@"Environment\Test_Win8Tool");
			}
		}
	}
}
