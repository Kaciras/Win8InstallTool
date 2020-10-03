using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Win8InstallTool;

namespace Test
{
	[TestClass]
	public class UtilsTest
	{
		[TestMethod]
		public void ExtractStringFromDLL()
		{
			var text = Utils.ExtractStringFromDLL(@"shell32.dll", -51330);
			Assert.AreEqual("s", text);
		}
	}
}
