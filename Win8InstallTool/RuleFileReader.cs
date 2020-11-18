using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool
{
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
            var k = i;
            for (; i < content.Length; i++)
            {
                switch (content[i])
                {
                    case '\r':
                    case '\n':
                        goto SearchEnd;
                }
            }

        SearchEnd:
            return content.Substring(k, i - k);
        }
    }
}
