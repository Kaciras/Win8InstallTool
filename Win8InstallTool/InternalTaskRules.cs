using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win8InstallTool.Rules;

namespace Win8InstallTool
{
	internal static class InternalTaskRules
	{
		public static void RegistryTaskRules()
		{
			var TaskRules = new List<TaskShcdulerRule>();

			// 用户体验改善计划相关
			TaskRules.Add(new TaskItemRule
			{
				Path = @"\Microsoft\Windows\Application Experience\AitAgent",
				Description = "没同意客户体验改善计划的可以删了",
			});
			TaskRules.Add(new TaskItemRule
			{
				Path = @"\Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser",
				Description = "微软收集用户电脑性能信息的任务",
			});
			TaskRules.Add(new TaskItemRule
			{
				Path = @"\Microsoft\Windows\Application Experience\StartupAppTask",
				Description = "启动项应该自己注意,而不是靠它来扫描",
				Delete = false,
			});
			TaskRules.Add(new TaskDirectoryRule
			{
				Path = @"\Microsoft\Windows\Autochk",
				//Name = "Proxy",
				Description = "没同意客户体验改善计划的可以删了",
			});
			TaskRules.Add(new TaskDirectoryRule // 删不掉？
			{
				Path = @"Microsoft\Windows\Customer Experience Improvement Program",
				Description = "全是客户体验改善计划,不参加的直接删",
			});


			TaskRules.Add(new TaskItemRule
			{
				Path = @"\Microsoft\Windows\DiskCleanup\SilentCleanup",
				Description = "磁盘空间不足还用你来提示?",
				Delete = false,
			});
			TaskRules.Add(new TaskDirectoryRule
			{
				Path = @"\Microsoft\Windows\SkyDrive",
				Description = "SkyDrive目录，里头两个同步任务是万恶之源,老在日志里报错",
			});
			TaskRules.Add(new TaskItemRule
			{
				Path = @"\Microsoft\Windows\UPnP\UPnPHostConfig",
				Description = "用不用UPnp不是你说了算,还敢在后台偷偷改",
				Delete = false,
			});
			TaskRules.Add(new TaskItemRule
			{
				Path = @"\Microsoft\Office\Office 15 Subscription Heartbeat",
				Description = "如果你没有订阅Office就可以删除此任务",
			});
		}
	}
}
