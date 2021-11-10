using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace Win8InstallTool;

[TestClass]
public sealed class RegistryHelperTest
{
    [TestCleanup]
    public void Cleanup()
    {
        Registry.CurrentUser.DeleteSubKeyTree("_Test_Import", false);
        Registry.CurrentUser.DeleteSubKeyTree("_Test_AutoConvert", false);
    }

    [TestMethod]
    public void GetCLSIDValueException()
    {
        var clsid = "{66666666-0000-0000-6666-000000000000}";
        Assert.ThrowsException<DirectoryNotFoundException>(() => RegistryHelper.GetCLSIDValue(clsid));
    }

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

        var value = Registry.GetValue(@"HKEY_CURRENT_USER\_Test_Import\Key", "StringValue", null);
        Assert.AreEqual("foobar", value);
    }

    [TestMethod]
    public void Export()
    {
        using (var key = Registry.CurrentUser.CreateSubKey(@"_Test_Import\Key"))
        {
            key.SetValue("StringValue", "foobar");
        }
        RegistryHelper.Export("ExportTest.reg", @"HKEY_CURRENT_USER\_Test_Import\Key");

        var actual = File.ReadAllBytes("ExportTest.reg");
        var expect = File.ReadAllBytes(@"Resources\ImportTest.reg");
        CollectionAssert.AreEqual(expect, actual);
    }

    /// <summary>
    /// 验证 Registry.SetValue() 无法直接接受 .reg 文件里的值格式，必须要先转换。
    /// </summary>
    [TestMethod]
    public void AutoConvertOnSetValue()
    {
        var key = @"HKEY_CURRENT_USER\_Test_AutoConvert";
        var text = "50,2d,02,09,60,d1,d6,01";
        var kind = RegistryValueKind.QWord;
        Assert.ThrowsException<ArgumentException>(() => Registry.SetValue(key, "a", text, kind));
    }
}
