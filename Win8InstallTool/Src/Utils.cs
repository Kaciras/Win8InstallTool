using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Win8InstallTool
{
	internal static class Utils
	{
		public static void ForEach<T>(this IEnumerable<T> ienum, Action<T> action)
		{
			foreach (var item in ienum) action(item);
		}

		internal static string GetDescription(Enum obj)
		{
			var mem = obj.GetType().GetField(obj.ToString());
			var attr=Attribute.GetCustomAttribute(mem, typeof(DescriptionAttribute));
			return (attr as DescriptionAttribute)?.Description;
		}

		internal static void FocusAnother()
		{
			var current = Process.GetCurrentProcess();
			var other = Process.GetProcessesByName(current.ProcessName)
				.Where(p => p.Id != current.Id).First();
			SwitchToThisWindow(other.MainWindowHandle, true);
		}

		[DllImport("user32.dll")]
		static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

		/// <summary>
		/// 解析类似 "@shell32.dll,-1#immutable1" 这样的字符串，并读取其引用的DLL中的字符串资源。
		/// </summary>
		/// <param name="string">字符串引用</param>
		/// <returns>字符串资源</returns>
		public static string ExtractStringFromDLL(string @string)
		{
			if(@string[0] == '@')
			{
				@string = @string.Substring(1);
			}
			var splited = @string.Split(',');
			var num = Math.Abs(int.Parse(splited[1]));
			return ExtractStringFromDLL(splited[0], num);
		}

		/// <summary>
		/// 从Windows动态链接库文件里读取字符串资源。
		/// </summary>
		/// <param name="file">DLL文件</param>
		/// <param name="number">资源索引</param>
		/// <returns>字符串资源</returns>
		public static string ExtractStringFromDLL(string file, int number)
		{
			var lib = LoadLibrary(file);

			var returnCode = Marshal.GetLastWin32Error();
			if (returnCode != 0)
			{
				throw new SystemException("错误代码:" + returnCode);
			}

			var result = new StringBuilder(2048);
			LoadString(lib, number, result, result.Capacity);
			FreeLibrary(lib);
			return result.ToString();
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		[DllImport("user32.dll")]
		static extern int LoadString(IntPtr hInstance, int ID, StringBuilder lpBuffer, int nBufferMax);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeLibrary(IntPtr hModule);
	}
}
