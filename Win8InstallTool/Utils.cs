using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shell32;

namespace Win8InstallTool;

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
    /// 执行命令并等待完成，检查退出码，已经设置来重定向了输入和禁止显示窗口。
    /// <br/>
    /// 如果命令以非零值退出，则会抛出异常，异常信息使用 stderr 或 stdout，请保证命令的输出不要太长。
    /// </summary>
    /// <param name="file">文件名</param>
    /// <param name="args">参数</param>
    /// <returns>进程对象</returns>
    /// <exception cref="SystemException">如果命令执行失败</exception>
    public static Process Execute(string file, string args = "")
    {
        var startInfo = new ProcessStartInfo(file)
        {
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        var process = Process.Start(startInfo);

        process.WaitForExit();
        if (process.ExitCode == 0)
        {
            return process;
        }

        var message = process.StandardError.ReadToEnd();
        if (string.IsNullOrEmpty(message))
        {
            message = process.StandardOutput.ReadToEnd();
        }
        throw new SystemException($"命令执行失败 - {process.ExitCode}：{message}");
    }

    /// <summary>
    /// 获取快捷方式所指向的文件路径。
    /// </summary>
    /// <seealso cref="https://stackoverflow.com/a/9414495/7065321"/>
    /// <param name="filename">快捷方式文件</param>
    /// <returns>目标文件的路径</returns>
    /// <exception cref="InvalidOperationException">文件不是快捷方式</exception>
    /// <exception cref="UnauthorizedAccessException">无权限，或者是特殊的链接，比如开始菜单里的桌面</exception>
    /// <exception cref="FileNotFoundException">如果所给的快捷方式不存在</exception>
    public static string GetShortcutTarget(string filename)
    {
        return STAExecutor.Run(() =>
        {
            var pathOnly = Path.GetDirectoryName(filename);
            var filenameOnly = Path.GetFileName(filename);
            var shell = new Shell();

            var folderItem = shell.NameSpace(Path.GetFullPath(pathOnly))
                ?.ParseName(filenameOnly)
                ?? throw new FileNotFoundException("File not exists", filename);

                // 如果不是链接 GetLink 会抛 NotImplementedException
                if (!folderItem.IsLink)
            {
                throw new InvalidOperationException("File is not a link");
            }

            return ((ShellLinkObject)folderItem.GetLink).Path;
        });
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
        var lib = LoadLibraryEx(file, IntPtr.Zero, 0x00000020 | 0x00000002);

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

    // 使用 LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE + LOAD_LIBRARY_AS_IMAGE_RESOURCE 两个标志，
    // 避免加载多余的依赖，防止出现 12 错误码。
    // https://docs.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryexw

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, int flags);

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
