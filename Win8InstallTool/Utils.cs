using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Win8InstallTool
{
    internal static class Utils
    {
        /// <summary>
        /// 这么常用的函数标准库竟然不自带。
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> ienum, Action<T> action)
        {
            foreach (var item in ienum) action(item);
        }

        /// <summary>
        /// 创建临时文件，搭配 using 语句使用，在销毁时删除。
        /// </summary>
        public static TempFileSession CreateTempFile()
        {
            var file = Path.GetTempFileName();
            return new TempFileSession(file);
        }

        /// <summary>
        /// 解析类似 "@shell32.dll,-1#immutable1" 这样的字符串，并读取其引用的DLL中的字符串资源。
        /// </summary>
        /// <param name="string">字符串引用</param>
        /// <returns>字符串资源</returns>
        public static string ExtractStringFromDLL(string @string)
        {
            if (@string[0] == '@')
            {
                @string = @string.Substring(1);
            }
            var splited = @string.Split(',');
            var num = Math.Abs(int.Parse(splited[1]));
            return ExtractStringFromDLL(splited[0], num);
        }

        /// <summary>
        /// 从 Windows 动态链接库文件里读取字符串资源。
        /// </summary>
        /// <param name="file">DLL文件</param>
        /// <param name="number">资源索引，不能是负数</param>
        /// <returns>字符串资源</returns>
        public static string ExtractStringFromDLL(string file, int number)
        {
            file = Environment.ExpandEnvironmentVariables(file);
            var lib = LoadLibrary(file);

            var code = Marshal.GetLastWin32Error();
            if (code != 0)
            {
                throw new SystemException($"无法加载{file}，错误代码:{code}");
            }

            var buffer = new StringBuilder(2048);
            LoadString(lib, number, buffer, buffer.Capacity);
            FreeLibrary(lib);
            return buffer.ToString();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        static extern int LoadString(IntPtr hInstance, int ID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        public sealed class TempFileSession : IDisposable
        {
            public readonly string Path;

            internal TempFileSession(string path)
            {
                Path = path;
            }

            public void Dispose() => File.Delete(Path);
        }
    }
}
