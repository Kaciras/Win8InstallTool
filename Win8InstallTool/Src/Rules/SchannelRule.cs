using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8InstallTool.Rules
{
    /// <summary>
    /// 一些系统程序出现了TLS连接错误时会产生一个事件记录，但是连接失败很正常，
    /// 比如网络卡或是服务端证书有问题，故忽略此错误。
    /// 
    /// "从远程终点接收到一个严重警告。TLS 协议所定义的严重警告代码为 xx。"
    /// 
    /// 【警告代码】
    /// https://docs.microsoft.com/en-us/archive/blogs/kaushal/ssltls-alert-protocol-the-alert-codes
    /// 
    /// 【注册表值】
    /// https://support.microsoft.com/zh-cn/help/260729/how-to-enable-schannel-event-logging-in-windows-and-windows-server
    /// 
    /// 【参考】
    /// https://mp.weixin.qq.com/s?__biz=MjM5Nzc2MTU0MQ==&mid=2454671613&idx=2&sn=4dda0985adf0341bea436dac825eba71&chksm=b16ef79786197e81f63b1a2f8ed31ac04a5f451b710a0bade32ece9d3381ea4b8208cd36693f#rd
    /// </summary>
    public sealed class SchannelRule : Rule
    {
        const string KEY = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\SecurityProviders\SCHANNEL";

        public string Description => "屏蔽系统日志里的 '从远程终点接收到一个严重警告' 错误";

        public void Optimize()
        {
            Registry.SetValue(KEY, "EventLogging", 0x0000);
        }

        public bool Check()
        {
            return !Registry.GetValue(KEY, "EventLogging", null).Equals(0x0000);
        }
    }
}
