using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Win8InstallTool;

[TestClass]
public sealed class RegFileTokenizerTest
{
    [TestMethod]
    public void RequireVersion()
    {
        var tokenizer = new RegFileTokenizer(string.Empty);
        Assert.ThrowsException<FormatException>(() => tokenizer.Read());
    }

    [TestMethod]
    public void InvalidVersion()
    {
        var tokenizer = new RegFileTokenizer("invalid version line\r\n");
        Assert.ThrowsException<FormatException>(() => tokenizer.Read());
    }

    [TestMethod]
    public void InvalidTopLevelToken()
    {
        var tokenizer = new RegFileTokenizer("Windows Registry Editor Version 5.00\r\nfoobar");

        Assert.IsTrue(tokenizer.Read());
        Assert.ThrowsException<FormatException>(() => tokenizer.Read());
    }
}
