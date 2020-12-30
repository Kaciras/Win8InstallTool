using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Win8InstallTool
{
	static class TestHelper
	{
		public static void RunInNewThread(Action action)
		{
			Exception exception = null;
			var thread = new Thread(() =>
			{
				try
				{
					action();
				}
				catch (Exception e)
				{
					exception = e;
				}
			});
			thread.Start();
			thread.Join();
			if (exception != null) throw exception;
		}
	}
}
