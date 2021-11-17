using System;
using System.Text;

namespace Win8InstallTool.RegFile;

/// <summary>
/// 解析注册表导出文件（.reg）的类，属于最前端的分词器。
/// <br/>
/// 我觉得应该有开源库读取 reg 文件的，但是找了一圈也没找到，只能自己撸了。
/// <br/>
/// <seealso cref="https://support.microsoft.com/en-us/help/310516/how-to-add-modify-or-delete-registry-subkeys-and-values-by-using-a-reg"/>
/// </summary>
public ref struct RegFileTokenizer
{
	readonly string content;

	int i;

	public RegTokenType TokenType { get; private set; }

	public string Value { get; private set; }

	public RegFileTokenizer(string content)
	{
		this.content = content;

		Value = null;
		i = 0;
		TokenType = RegTokenType.None;
	}

	/// <summary>
	/// 读取下一个 Token，如果已经读完则返回false，否则返回true
	/// </summary>
	public bool Read()
	{
		switch (TokenType)
		{
			case RegTokenType.None:
				ConsumeVersion();
				break;
			case RegTokenType.ValueName:
				ConsumeKindOrValue();
				break;
			case RegTokenType.Kind:
				ConsumeValue();
				break;
			default:
				SkipBlankLines();
				return ConsumeTopLevel();
		}

		return true;
	}

	bool ConsumeTopLevel()
	{
		if (i >= content.Length)
		{
			return false;
		}
		switch (content[i])
		{
			case '@':
				ConsumeDefaultName();
				break;
			case '"':
				ConsumeValueName();
				break;
			case '[':
				ConsumeKey();
				break;
			case ';':
				ConsumeComment();
				break;
			default:
				throw new FormatException("Invalid Token: " + content[i]);
		}
		return true;
	}

	void ConsumeComment()
	{
		TokenType = RegTokenType.Comment;
		var j = i + 1;
		i = content.IndexOf('\r', j);
		Value = content.Substring(j, i - j);
	}

	void ConsumeVersion()
	{
		const string VER_LINE = "Windows Registry Editor Version 5.00";

		var j = i;
		i = content.IndexOf('\r', j);
		if (i == -1)
		{
			throw new FormatException("数据不完整");
		}

		TokenType = RegTokenType.Version;
		Value = content.Substring(j, i - j);

		if (Value != VER_LINE)
		{
			throw new FormatException("Invalid version: " + Value);
		}
	}

	void ConsumeKey()
	{
		if (content[++i] == '-')
		{
			TokenType = RegTokenType.DeleteKey;
			i += 1;
		}
		else
		{
			TokenType = RegTokenType.CreateKey;
		}

		Value = ReadTo(']');
	}

	void ConsumeDefaultName()
	{
		i += 1;
		TokenType = RegTokenType.ValueName;
		Value = string.Empty;
	}

	void ConsumeValueName()
	{
		i += 1;
		TokenType = RegTokenType.ValueName;
		Value = ReadTo('"');
	}

	void ConsumeKindOrValue()
	{
		if (content[i++] != '=')
		{
			throw new FormatException("值名后面必须紧跟等号");
		}

		switch (content[i])
		{
			case '"':
				TokenType = RegTokenType.Value;
				i += 1;
				Value = ReadTo('"');
				break;
			case '-':
				i += 1;
				TokenType = RegTokenType.DeleteValue;
				break;
			default:
				Value = ReadTo(':');
				TokenType = RegTokenType.Kind;
				break;
		}
	}

	string ReadTo(char ch)
	{
		var j = i;
		i = content.IndexOf(ch, j) + 1;
		if (i == 0)
		{
			throw new FormatException("数据不完整");
		}
		return content.Substring(j, i - j - 1);
	}

	void ConsumeValue()
	{
		var buffer = new StringBuilder();
		var hasMoreLine = true;

		while (hasMoreLine)
		{
			SkipWhiteSpaces();

			var j = i;
			var k = content.IndexOf('\r', j);
			i = k + 2;

			if (content[k - 1] == '\\')
			{
				k -= 1;
				hasMoreLine = true;
			}
			else
			{
				hasMoreLine = false;
			}

			buffer.Append(content, j, k - j);
		}

		TokenType = RegTokenType.Value;
		Value = buffer.ToString();
	}

	void SkipWhiteSpaces()
	{
		for (; i < content.Length; i++)
		{
			switch (content[i])
			{
				case ' ':
				case '\t':
					break;
				default:
					return;
			}
		}
	}

	void SkipBlankLines()
	{
		for (; i < content.Length; i++)
		{
			switch (content[i])
			{
				case '\r':
				case '\n':
				case ' ':
				case '\t':
					break;
				default:
					return;
			}
		}
	}
}
