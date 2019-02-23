using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Win8InstallTool.Rules;

[assembly: InternalsVisibleTo("Test")]
namespace Win8InstallTool
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			// 兼容性疑难解答
			new ContextMenuRule(@"exefile\shellex\ContextMenuHandlers\Compatibility").Execute();

			// 固定到开始屏幕
			//new ContextMenuRule(@"exefile\shellex\ContextMenuHandlers\PintoStartScreen").Execute();
			//new ContextMenuRule(@"Folder\shellex\ContextMenuHandlers\PintoStartScreen").Execute();

			// 固定到任务栏
			new ContextMenuRule(@"*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}").Execute();

			// Open in Visual Studio
			new ContextMenuRule(@"Directory\shell\AnyCode").Execute();
			new ContextMenuRule(@"Directory\background\shell\AnyCode").Execute();

			// 包含到库中
			new ContextMenuRule(@"Folder\shellex\ContextMenuHandlers\Library Location").Execute();

			// 各种文件的 "编辑"，因为已经搞了记事本打开所以它多余了
			new ContextMenuRule(@"cmdfile\shell\edit").Execute();
			new ContextMenuRule(@"batfile\shell\edit").Execute();
			new ContextMenuRule(@"xmlfile\shell\edit").Execute();
			new ContextMenuRule(@"regfile\shell\edit").Execute();
			new ContextMenuRule(@"SystemFileAssociations\text\shell\edit").Execute();

			// 打印
			//new ContextMenuRule(@"batfile\shell\print").Execute();
			//new ContextMenuRule(@"txtfile\shell\print").Execute();
			//new ContextMenuRule(@"SystemFileAssociations\image\shell\print").Execute();
		}
	}
}
