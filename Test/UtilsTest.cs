﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win8InstallTool.Test
{
	[TestClass]
	public class UtilsTest
	{
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
