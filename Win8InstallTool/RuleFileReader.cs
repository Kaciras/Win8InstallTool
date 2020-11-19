using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool
{
    /// <summary>
    /// 一大片规则卸载代码里看着不输入，所以就单独提出来一个文本文件放在资源里。
    /// <br/>
    /// 本类专门用于读取这些文本文件。
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

        public string Read()
        {
            var j = i;

            for (; i < content.Length; i++)
            {
                switch (content[i])
                {
                    case '\r':
                        ThrowCR();
                        break;
                    case '\n':
                        goto SearchEnd;
                }
            }

        SearchEnd:
            return content.Substring(j, i - j);
        }
    }
}
