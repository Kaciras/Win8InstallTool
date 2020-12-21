using System;
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
		readonly string filename;

		public string Name { get; }

		public string Description { get; }

		public RegFileRule(string name, string description, string filename)
		{
			this.filename = filename;
			Name = name;
			Description = description;
		}

		Stream OpenStream()
		{
			var name = $"Win8InstallTool.RegFiles.{filename}.reg";
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		}

		/// <summary>
		/// 对比 reg 文件和注册表，判断是否存在不同，如果不同表示需要优化。
		/// </summary>
		public bool Check()
		{
			using var regFile = new StreamReader(OpenStream());

			var tokenilizer = new RegFileTokenizer(regFile.ReadToEnd());
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
						var valueInDB = Registry.GetValue(key, valueName, null);
						expected = ParseValue(tokenilizer.Value, kind).Equals(valueInDB);
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

		object ParseValue(string text, RegistryValueKind kind)
		{
			byte[] ToBytes() => text.Split(',').Select(byte.Parse).ToArray();

			return kind switch
			{
				RegistryValueKind.ExpandString or RegistryValueKind.MultiString => Encoding.Unicode.GetString(ToBytes()),
				RegistryValueKind.Binary => ToBytes(),
				RegistryValueKind.DWord => int.Parse(text, NumberStyles.HexNumber),
				RegistryValueKind.QWord => long.Parse(text.Replace(",", ""), NumberStyles.HexNumber),
				RegistryValueKind.String => text,
				_ => throw new Exception("无效的值类型: " + Enum.GetName(typeof(RegistryValueKind), kind)),
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
			using (var output = File.OpenWrite(file.Path))
			{
				OpenStream().CopyTo(output);
			}
			RegistryHelper.Import(file.Path);
		}
	}
}
