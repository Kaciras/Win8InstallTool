using System;
using System.Collections.Generic;

namespace Win8InstallTool
{
	/// <summary>
	/// 一大片规则卸载代码里看着不输入，所以就单独提出来一个文本文件放在资源里，本类专门用于读取这些文本文件。
	/// <br/>
	/// 规则文件是一种以行为单位，低冗余的紧凑格式，具有以下特点：
	/// <list>
	/// <item>只能用LF换行符</item>
	/// <item>使用空行分隔项目，项目内不允许空行</item>
	/// <item>一行是一个属性，上层怎么解析随意</item>
	/// </list>
	/// </summary>
	public sealed class RuleFileReader
	{
		private readonly string content;

		private int i;

		public RuleFileReader(string content)
		{
			this.content = content;
		}

		/// <summary>
		/// 反正都读取的是预定义的资源，限制死换行符可以避免一些的麻烦。
		/// </summary>
		void ThrowCR() => throw new ArgumentException("规则文件只能用LF换行");

		/// <summary>
		/// 跳过空白和注释行，准备读取新的条目。
		/// </summary>
		/// <returns>如果读完则为false，否则返回true</returns>
		public bool MoveNext()
		{
			for (; i < content.Length; i++)
			{
				switch (content[i])
				{
					case '\r':
						ThrowCR();
						break;
					case '#':
						i = content.IndexOf('\n', i);
						break;
					case '\n':
					case '\t':
					case ' ':
						break;
					default:
						return true;
				}
			}
			return false; // 文件读完了
		}

		// 可以搞成扩展方法
		public IEnumerable<string> Drain()
		{
			var line = Read();
			while (line != string.Empty)
			{
				yield return line;
				line = Read();
			}
		}

		/// <summary>
		/// 读取一行，请先使用 MoveNext() 确保读取位置在有效行上。
		/// </summary>
		/// <returns>一行内容</returns>
		public string Read()
		{
			var j = i;
			var k = j;

			for (; k < content.Length; k++)
			{
				switch (content[k])
				{
					case '\r':
						ThrowCR();
						break;
					case '\n':
						goto SearchEnd;
				}
			}

		SearchEnd:

			if (k > content.Length)
			{
				return string.Empty;
			}

			i = k + 1;
			return content.Substring(j, k - j);
		}
	}
}
