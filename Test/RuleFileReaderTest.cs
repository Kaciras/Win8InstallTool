using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Test
{
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
        public void ThrowOnInvalidLineEnd()
        {
            var reader = new RuleFileReader("\r\n");
            Assert.ThrowsException<ArgumentException>(() => reader.Read());
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
    }
}
