using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RuleFileReader
    {
        private readonly string content;

        private int i;

        public RuleFileReader(string content)
        {
            this.content = content;
        }

        public bool MoveNext()
        {
            for (; i < content.Length; i++)
            {
                switch (content[i])
                {
                    case '\r':
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
            var k = j;

            for (; k < content.Length; k++)
            {
                switch (content[k])
                {
                    case '\r':
                    case '\n':
                        goto SearchEnd;
                }
            }

        SearchEnd:

            i = k;

            // 跳过剩余的换行符
            switch (content[k + 1])
            {
                case '\r':
                case '\n':
                    i += 2;
                    break;
                default:
                    i += 1;
                    break;
            }

            return content.Substring(j, k - j);
        }
    }
}
