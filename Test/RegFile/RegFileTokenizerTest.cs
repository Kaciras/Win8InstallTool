using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Win8InstallTool.RegFile;

[TestClass]
public sealed class RegFileTokenizerTest
{
	[ExpectedException(typeof(FormatException))]
	[TestMethod]
	public void RequireVersion()
	{
		new RegFileTokenizer(string.Empty).Read();
	}

	[ExpectedException(typeof(FormatException))]
	[TestMethod]
	public void InvalidVersion()
	{
		new RegFileTokenizer("invalid version line\r\n").Read();
	}

	[ExpectedException(typeof(FormatException))]
	[TestMethod]
	public void InvalidTopLevelToken()
	{
		var tokenizer = new RegFileTokenizer("Windows Registry Editor Version 5.00\r\nfoobar");

		Assert.IsTrue(tokenizer.Read());

		tokenizer.Read(); // throw FormatException
	}
}
