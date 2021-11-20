using System;
using System.Text;

namespace Win8InstallTool.RegFile;

/// <summary>
/// 解析注册表导出文件（.reg）的类，属于最前端的分词器。我觉得应该有开源库读取 reg 文件的，
/// 但是找了一圈也没找到，只能自己撸了。
/// <br/>
/// 注意本分词器要求文件末尾必须有换行。
/// <seealso cref="https://support.microsoft.com/en-us/help/310516/how-to-add-modify-or-delete-registry-subkeys-and-values-by-using-a-reg"/>
/// </summary>
public ref struct RegFileTokenizer
{
	readonly string content;

	int i;
	bool hasMoreLines;

	public RegTokenType TokenType { get; private set; }

	public string Value { get; private set; }

	public RegFileTokenizer(string content)
	{
		this.content = content;

		Value = null;
		i = 0;
		hasMoreLines = false;
		TokenType = RegTokenType.None;
	}

	/// <summary>
	/// 读取下一个 Token，如果已经读完则返回false，否则返回true
	/// </summary>
	public bool Read()
	{
		if (hasMoreLines)
		{
			ConsumeNextPart();
			return true;
		}
		switch (TokenType)
		{
			case RegTokenType.None:
				ConsumeVersion();
				break;
			case RegTokenType.Name:
				ConsumeKindOrString();
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
				ConsumeName();
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

	// 旧版的 REGEDIT4 就不支持了，Win 7 以上都是 5.0 的了。
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
		TokenType = RegTokenType.Name;
		Value = string.Empty;
	}

	void ConsumeName()
	{
		i += 1;
		TokenType = RegTokenType.Name;
		Value = ReadTo('"');
	}

	// 字符串值也放在这了，因为已经读了一个引号，免得回看。
	void ConsumeKindOrString()
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
				Value = ReadQuoted();
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

	void ConsumeNextPart()
	{
		SkipBlankLines();

		if (content[i] == ';')
		{
			ConsumeComment();
		}
		else
		{
			ConsumeValue();
		}
	}

	// 类型后面必须立即跟着值，不能是注释然后把值写到下一行。
	void ConsumeValue()
	{
		var j = i;

		for (; i < content.Length; i++)
		{
			switch (content[i])
			{
				case '\\':
					TokenType = RegTokenType.ValuePart;
					hasMoreLines = true;
					Value = content.Substring(j, i - j);
					i += 1;
					return;
				case ';':
				case '\r':
					goto SearchEnd;
			}
		}

	SearchEnd:

		TokenType = RegTokenType.Value;
		hasMoreLines = false;
		Value = content.Substring(j, i - j);
	}

	/// <summary>
	/// 读取被双引号包围的内容，自动处理转义，内容不能有换行。
	/// </summary>
	string ReadQuoted()
	{
		var buffer = new StringBuilder();
		var j = i;

		for (; i < content.Length; i++)
		{
			switch (content[i])
			{
				case '\\':
					buffer.Append(content, j, i - j);
					j = i += 1;
					break;
				case '"':
					buffer.Append(content, j, i - j);
					i += 1;
					return buffer.ToString();
				case '\r':
					throw new FormatException("字符串不能换行");
			}
		}

		throw new FormatException("数据不完整");
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
