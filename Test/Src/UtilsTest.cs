using System;
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
	}
}
