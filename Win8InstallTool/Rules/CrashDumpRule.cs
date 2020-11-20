using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
    /// <summary>
    /// 关闭系统崩溃时的内存转储功能，一般人用不上，还会在日志里报错。
    /// <br/>
    /// 该项值与面板中的项的对应如下：
    /// <list type="">
    ///     <item>0 - 无</item>
    ///     <item>1 - 完全内存转储</item>
    ///     <item>2 - 核心内存转储</item>
    ///     <item>3 - 小内存转储(256KB)</item>
    ///     <item>7 - 自动</item>
    /// </list>
    /// <seealso cref="https://docs.microsoft.com/zh-cn/windows/client-management/system-failure-recovery-options"/>
    /// </summary>
    public sealed class CrashDumpRule : ImutatableRule
    {
        const string KEY = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CrashControl";

        public override string Name => "系统失败 -> 写入调试信息";

        public override string Description => "不做系统相关开发的话" +
            "这些调试信息鸟用没有，如果禁用了虚拟内存还会在事件日志里报错";

        public override void Optimize()
        {
            Registry.SetValue(KEY, "CrashDumpEnabled", 0);
        }

        protected override bool Check()
        {
            return !Registry.GetValue(KEY, "CrashDumpEnabled", 0).Equals(0);
        }
    }
}
