using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Win8InstallTool;

[TestClass]
public class RuleFileReaderTest
{
	[TestMethod]
	public void EmptyContent()
	{
		var reader = new RuleFileReader(string.Empty);

		Assert.IsFalse(reader.MoveNext());
		Assert.AreEqual(string.Empty, reader.Read());
	}

	[TestMethod]
	public void ThrowOnReadInvalidLineEnd()
	{
		var reader = new RuleFileReader("\r\n");
		Assert.ThrowsException<ArgumentException>(() => reader.Read());
	}

	[TestMethod]
	public void ThrowOnMoveNextInvalidLineEnd()
	{
		var reader = new RuleFileReader("\r\n");
		Assert.ThrowsException<ArgumentException>(() => reader.MoveNext());
	}

	[TestMethod]
	public void SkipComment()
	{
		var text = "#foo\n#bar\n\nFirst";
		var reader = new RuleFileReader(text);
		Assert.IsTrue(reader.MoveNext());
		Assert.AreEqual("First", reader.Read());
	}

	[TestMethod]
	public void RepeatReadEnpty()
	{
		var reader = new RuleFileReader("\n");
		Assert.AreEqual(string.Empty, reader.Read());
		Assert.AreEqual(string.Empty, reader.Read());
		Assert.AreEqual(string.Empty, reader.Read());
		Assert.AreEqual(string.Empty, reader.Read());
	}

	[DataRow("First\nSecond")]
	[DataRow("\nFirst\nSecond\n")]
	[DataRow("First\nSecond\n\n")]
	[DataTestMethod]
	public void ReadLines(string text)
	{
		var reader = new RuleFileReader(text);

		Assert.IsTrue(reader.MoveNext());
		Assert.AreEqual("First", reader.Read());
		Assert.AreEqual("Second", reader.Read());

		Assert.IsFalse(reader.MoveNext());
	}

	[TestMethod]
	public void Drain()
	{
		var text = "我\n好\n他\n妈\n帅\n\n(误)";
		var reader = new RuleFileReader(text);

		var value = string.Join("", reader.Drain());
		Assert.AreEqual("我好他妈帅", value);
	}

	[TestMethod]
	public void Iter()
	{
		var text = "\n\n\n\nFirst\n\n";
		var enumerable = RuleFileReader.Iter(text);

		var enumerator = enumerable.GetEnumerator();
		Assert.IsTrue(enumerator.MoveNext());
		Assert.AreEqual("First", enumerator.Current.Read());
		Assert.IsFalse(enumerator.MoveNext());

		enumerator.Reset();
		Assert.IsTrue(enumerator.MoveNext());
		Assert.AreEqual("First", enumerator.Current.Read());
	}
}
