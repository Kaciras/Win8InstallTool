﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Environment;

namespace Win8InstallTool.Test;

[TestClass]
public sealed class UtilsTest
{
	// 本来应该用个可控的程序来测试的，但我懒得搞于是就用系统自带的 sc.exe 了。
	[TestMethod]
	public void ExecuteFailed()
	{
		Assert.ThrowsException<SystemException>(() => Utils.Execute("sc", "create x x"));
	}

	[DataRow("Resources/Calculator.lnk", @"\system32\calc.exe")]
	[DataRow("Resources/Excel 2016.lnk", @"\Installer\{90160000-0011-0000-1000-0000000FF1CE}\xlicons.exe")]
	[DataTestMethod]
	public void GetShortcutTarget(string lnk, string subpath)
	{
		var windir = GetFolderPath(SpecialFolder.Windows);
		var target = Utils.GetShortcutTarget(lnk);
		Assert.AreEqual(windir + subpath, target);
	}

	/*
	 * 这个快捷方式在新装 Win8.1 中存在，文件内容全是0，完全没有任何意义。
	 * 但 FolderItem.IsLink 却返回 true，故本项目也认为它是一个快捷方式，目标为 null。
	 */
	[TestMethod]
	public void GetShortcutTargetInvalid()
	{
		Assert.IsNull(Utils.GetShortcutTarget("Resources/Fax Recipient.lnk"));
	}

	[DataRow("not/exists/folder")]
	[DataRow("Resources/Calculator")]
	[DataTestMethod]
	public void GetShortcutTargetFileNotExists(string file)
	{
		Assert.ThrowsException<FileNotFoundException>(() => Utils.GetShortcutTarget(file));
	}

	[TestMethod]
	public void GetShortcutTargetWhenFileIsNotALink()
	{
		Assert.ThrowsException<InvalidOperationException>(() => Utils.GetShortcutTarget("Resources/config.ini"));
	}

	[DataRow("INVALID_VALUE")]
	[DataRow("shell32.dll,INVALID")]
	[DataRow("")]
	[ExpectedException(typeof(FormatException))]
	[DataTestMethod]
	public void ExtractStringWithInvalidText(string value)
	{
		Utils.ExtractStringFromDLL(value);
	}

	[Ignore("总是出现莫名其妙的错误 6")]
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
