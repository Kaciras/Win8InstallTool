using System;
using System.Text;

namespace Win8InstallTool
{
	/// <summary>
	/// 
	/// <seealso cref="https://support.microsoft.com/en-us/help/310516/how-to-add-modify-or-delete-registry-subkeys-and-values-by-using-a-reg"/>
	/// </summary>
	public class RegFileTokenizer
	{
		private readonly string content;
		private int i;

		public RegFileTokenType TokenType { get; private set; }

		public string Value { get; private set; }

		public RegFileTokenizer(string content)
		{
			this.content = content;
		}

		public bool Read()
		{
			switch (TokenType)
			{
				case RegFileTokenType.None:
					ConsumeVersion();
					break;
				case RegFileTokenType.ValueName:
					ConsumeKindOrValue();
					break;
				case RegFileTokenType.Kind:
					ConsumeValue();
					break;
				default:
					SkipBlankLines();
					return ConsumeTopLevelToken();
			}

			return true;
		}

		bool ConsumeTopLevelToken()
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
			TokenType = RegFileTokenType.Comment;
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

			TokenType = RegFileTokenType.Version;
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
				TokenType = RegFileTokenType.DeleteKey;
				i += 1;
			}
			else
			{
				TokenType = RegFileTokenType.CreateKey;
			}

			Value = ReadTo(']');
		}

		void ConsumeDefaultName()
		{
			i += 1;
			TokenType = RegFileTokenType.ValueName;
			Value = string.Empty;
		}

		void ConsumeValueName()
		{
			i += 1;
			TokenType = RegFileTokenType.ValueName;
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
					TokenType = RegFileTokenType.Value;
					i += 1;
					Value = ReadTo('"');
					break;
				case '-':
					i += 1;
					TokenType = RegFileTokenType.DeleteValue;
					break;
				default:
					Value = ReadTo(':');
					TokenType = RegFileTokenType.Kind;
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

			TokenType = RegFileTokenType.Value;
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
}
