﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Win8InstallTool.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Win8InstallTool.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to shellex\ContextMenuHandlers\Compatibility
        ///执行文件 - 兼容性疑难解答
        ///出了问题不去谷歌，看着破解答有屁用
        ///exefile
        ///
        ///shellex\ContextMenuHandlers\Library Location
        ///文件夹 - 包含到库中
        ///我现在还没用过这个系统自带的各种库文件夹，删了吧
        ///Folder
        ///
        ///shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}
        ///所有文件 - 固定到任务栏
        ///直接往任务栏上拖即可
        ///*
        ///
        ///shell\AnyCode
        ///文件夹及其背景 - Open with Visual Studio
        ///没啥用纯看着碍眼
        ///Directory
        ///Directory\background
        ///
        ///shell\edit
        ///各种纯文本类文件 - 编辑
        ///跟&quot;记事本打开&quot;的功能重复了，删之
        ///cmdfile
        ///batfile
        ///xmlfile
        ///regfile
        ///SystemFileAssociations\text
        ///
        ///Shell\setdesktopwallpaper
        ///各种媒体文件 - 设为桌面背景
        ///一般在播放器里都 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ContextMenuRules {
            get {
                return ResourceManager.GetString("ContextMenuRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Ext
        ///DisableAddonLoadTimePerformanceNotifications
        ///1
        ///关闭IE加载项性能通知
        ///防止出现打开IE浏览器状态栏提示关闭加载项来提高浏览器运行速度的提示
        ///
        ///HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Internet Explorer\Suggested Sites
        ///Enabled
        ///0
        ///IE打开建议的网站
        ///建议你妈逼，老子看啥要你指点？
        ///
        ///HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Internet Explorer\Main
        ///DisableFirstRunCustomize
        ///1
        ///禁用首次运行向导
        ///全是没卵用的配置，浪费启动时间
        ///
        ///HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer
        ///NoDriveTypeAutoRun
        ///255
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GroupPolicyRules {
            get {
                return ResourceManager.GetString("GroupPolicyRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # 关闭系统崩溃时的内存转储功能，一般人用不上，还会在日志里报错。
        ///# 该项值与面板中的项的对应如下：
        ///#	0 - 无
        ///#	1 - 完全内存转储
        ///#	2 - 核心内存转储
        ///#	3 - 小内存转储(256KB)
        ///#	7 - 自动
        ///# https://docs.microsoft.com/zh-cn/windows/client-management/system-failure-recovery-options
        ///
        ///禁止系统失败后写入调试信息
        ///不做系统相关开发的话这些调试信息鸟用没有，如果禁用了虚拟内存还会在事件日志里报错
        ///[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CrashControl]
        ///&quot;CrashDumpEnabled&quot;=dword:00000000
        ///
        ///禁用性能计数器
        ///统计系统底层性能的东西，一般人用不到，还老是在日志里报错 “无法打开服务器服务性能对象……”
        ///[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\PerfNet\Performance]
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RegistryRules {
            get {
                return ResourceManager.GetString("RegistryRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # 发传真的那个关闭系统功能后会自动删除。
        ///
        ///Compressed (zipped) Folder.ZFSendToTarget
        ///有专门压缩软件来做了
        ///
        ///Mail Recipient.MAPIMail
        ///邮件这么正式的东西还是不用这么快捷的方法
        ///
        ///文档.mydocs
        ///就是个直接复制到用户的文档目录，可惜我不用它
        ///.
        /// </summary>
        internal static string SendToRules {
            get {
                return ResourceManager.GetString("SendToRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BFE
        ///Windows安全平台的核心服务，防火墙和杀毒软件都需要，如果不用它们倒是可以关了
        ///
        ///hidserv
        ///不用特殊输入设备的不需要，该服务还容易被攻击者利用
        ///
        ///ShellHWDetection
        ///这年头谁还玩光盘自动播放，该服务还容易被攻击者利用
        ///
        ///LanmanServer
        ///服务器文件共享才需要，家用机可以关掉
        ///
        ///LanmanWorkstation
        ///用SMB协议做文件共享的服务，办公室可能需要，不用的话可以关掉
        ///
        ///lmhosts
        ///办公室远程控制打印机、文件才需要
        ///
        ///WSearch
        ///系统自带的搜索功能，并不是每个人都需要，还耗磁盘
        ///
        ///PcaSvc
        ///啥卵用都没有的兼容性助手
        ///
        ///IKEEXT
        ///某些协议的VPN需要这个服务，不用的话可以关掉
        ///
        ///DPS
        ///出问题不谷歌，要你来诊断?这个关了后另外两个也不会启动。注意禁用此服务后事件日志里会经常出现DCOM没有注册的错误。
        ///
        ///WPDBusEnum
        ///反正我是没用过什么可移动大容量存储设备
        ///
        ///TrkWks
        ///没几个人会把NTFS文件链接到远程计算机
        ///
        ///PolicyAgent
        ///关掉后防火墙不能使用IPSec策略，个人机一般用不着
        ///
        ///SSDPSRV
        ///Upnp设备个人机不常用
        ///
        ///ip [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ServiceRules {
            get {
                return ResourceManager.GetString("ServiceRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WinRAR
        ///Microsoft Office 2016 工具
        ///Windows Kits
        ///Music, Photos and Videos
        ///Visual Studio 2019
        ///.
        /// </summary>
        internal static string StartupRules {
            get {
                return ResourceManager.GetString("StartupRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Microsoft\Windows\Application Experience
        ///客户体验改善计划相关的任务，没同意的可以删了
        ///
        ///Microsoft\Windows\Autochk
        ///客户体验改善计划相关的任务，没同意的可以删了
        ///
        ///Microsoft\Windows\Customer Experience Improvement Program
        ///全是客户体验改善计划，不参加的直接删光光
        ///
        ///Microsoft\Windows\Application Experience\StartupAppTask
        ///启动项应该自己注意，而不是靠它来扫描
        ///:DISABLE
        ///
        ///Microsoft\Windows\DiskCleanup\SilentCleanup
        ///什么傻逼任务，磁盘空间不足还用你来提示？？？
        ///:DISABLE
        ///
        ///Microsoft\Windows\SkyDrive
        ///SkyDrive 目录里两个同步任务是万恶之源，老在日志里报错，不用就删掉
        ///
        ///Microsoft\Windows\UPnP\UPnPHostConfig
        ///用不用 UPnp 服务不是你说了算，还敢在后台偷偷改
        ///:DISABLE
        ///
        ///Microsoft\Offi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TaskSchdulerRules {
            get {
                return ResourceManager.GetString("TaskSchdulerRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 增加 ReadyBoot 日志文件大小
        ///增加日志文件上限到40MB，解决事件日志里 Circular Kernel Context Logger 停止的问题
        ///HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\WMI\Autologger\ReadyBoot
        ///null
        ///40
        ///
        ///启用 AirSpaceChannel 日志文件循环选项
        ///解决日志文件溢满导致 EventLog-AirSpaceChannel 已停止的问题
        ///HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\WMI\Autologger\EventLog-AirSpaceChannel
        ///true
        ///null
        ///.
        /// </summary>
        internal static string WMILoggerRules {
            get {
                return ResourceManager.GetString("WMILoggerRules", resourceCulture);
            }
        }
    }
}
