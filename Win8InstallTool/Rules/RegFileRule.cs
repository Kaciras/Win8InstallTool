﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	/// <summary>
	/// 注册表规则，能够判断是否需要导入内置的注册表文件（.reg），并在优化时将其导入。
	/// <br/>
	/// 注册表文件需要放在 RegFiles 目录下，并设置为 Embedded Resource.
	/// <br/>
	/// 导入注册表文件一条命令即可，但没有现成的方法可以比较其跟注册表是否一致，故自己实现了这功能。
	/// </summary>
	public class RegFileRule : Rule
	{
		public string Name { get; }

		public string Description { get; }

		readonly string content;

		public RegFileRule(string name, string description, string content)
		{
			this.content = content;
			Name = name;
			Description = description;
		}

		/// <summary>
		/// 对比 reg 文件和注册表，判断是否存在不同，如果不同表示需要优化。
		/// </summary>
		public bool Check()
		{
			var tokenilizer = new RegFileTokenizer(content);
			var expected = true;

			var key = string.Empty;
			var valueName = string.Empty;
			var kind = RegistryValueKind.None;

			while (expected && tokenilizer.Read())
			{
				switch (tokenilizer.TokenType)
				{
					case RegFileTokenType.DeleteKey:
						expected = !RegistryHelper.KeyExists(tokenilizer.Value);
						break;
					case RegFileTokenType.CreateKey:
						key = tokenilizer.Value;
						expected = RegistryHelper.KeyExists(key);
						break;
					case RegFileTokenType.ValueName:
						kind = RegistryValueKind.String;
						valueName = tokenilizer.Value;
						break;
					case RegFileTokenType.Value:
						expected = CheckValueInDB(key, valueName, tokenilizer.Value, kind);
						break;
					case RegFileTokenType.Kind:
						kind = ParseKind(tokenilizer.Value);
						break;
					case RegFileTokenType.DeleteValue:
						expected = Registry.GetValue(key, valueName, null) == null;
						break;
				}
			}

			return !expected;
		}

		bool CheckValueInDB(string key, string name, string valueStr, RegistryValueKind kind)
		{
			using var keyObj = RegistryHelper.OpenKey(key);
			var valueInDB = keyObj.GetValue(name, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
			var expected = ParseValue(valueStr, kind);

			bool ConvertAndCheck<T>()
			{
				if (!(valueInDB is T[]) || valueInDB == null)
				{
					return false;
				}
				return ((T[])expected).SequenceEqual((T[])valueInDB);
			}

			return kind switch
			{
				RegistryValueKind.Unknown or RegistryValueKind.None => throw new Exception("无效的值类型"),
				RegistryValueKind.Binary => ConvertAndCheck<byte>(),
				RegistryValueKind.MultiString => ConvertAndCheck<string>(),
				_ => expected.Equals(valueInDB),
			};
		}

		object ParseValue(string text, RegistryValueKind kind)
		{
			byte[] ToBytes() => text.Split(',')
				.Select(x => byte.Parse(x, NumberStyles.HexNumber))
				.ToArray();

			string ToString() => Encoding.Unicode.GetString(ToBytes()).Trim('\0');

			return kind switch
			{
				RegistryValueKind.ExpandString => ToString(),
				RegistryValueKind.MultiString => ToString().Split('\0'),
				RegistryValueKind.Binary => ToBytes(),
				RegistryValueKind.DWord => int.Parse(text, NumberStyles.HexNumber),
				RegistryValueKind.QWord => BitConverter.ToInt64(ToBytes(), 0),
				RegistryValueKind.String => text,
				_ => throw new Exception("无效的值类型"),
			};
		}

		RegistryValueKind ParseKind(string value) => value switch
		{
			"dword" => RegistryValueKind.DWord,
			"hex" => RegistryValueKind.Binary,
			"hex(2)" => RegistryValueKind.ExpandString,
			"hex(7)" => RegistryValueKind.MultiString,
			"hex(b)" => RegistryValueKind.QWord,
			_ => throw new FormatException("Unknown kind: " + value),
		};

		public void Optimize()
		{
			using var file = Utils.CreateTempFile();
			File.WriteAllText(file.Path, content);
			RegistryHelper.Import(file.Path);
		}
	}
}
