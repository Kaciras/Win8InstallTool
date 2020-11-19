using Microsoft.Win32;
using System.IO;
using System.Text;
using Win8InstallTool.Properties;

namespace Win8InstallTool.Rules
{
    public sealed class OpenWithNotepadRule : ImutatableRule
	{
        public override string Name => "把用记事本打开添加到右键菜单";

		public override string Description => "很常用的功能，稍微会点电脑的都懂。";

		protected override bool Check()
		{
			return Registry.ClassesRoot.OpenSubKey(@"*\shell\OpenWithNotepad") == null;
		}

		public override void Optimize()
		{
			using var file = Utils.CreateTempFile();
			File.WriteAllText(file.Path, Resources.OpenWithNotepad, Encoding.Unicode);
            RegistryHelper.Import(file.Path);
        }
	}
}
