using System;

namespace Win8InstallTool
{
    /// <summary>
    /// 我用的两个Windows系统，其他的不关心。
    /// </summary>
    [Flags]
	public enum SupportedOS
	{
		Windows8_1 = 1, Windows10 = 2,
	}
}
