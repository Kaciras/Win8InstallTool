using System.IO;
using static System.Environment;

namespace Win8InstallTool.Rules
{
	public class SendToRule : ImutatableRule
	{
		public override string Name { get; }

		public override string Description { get; }

		private readonly string path;

		public SendToRule(string name, string description)
		{
			var appRoaming = GetFolderPath(SpecialFolder.ApplicationData);
			var folder = Path.Combine(appRoaming, @"Microsoft\Windows\SendTo");

			var desktopIni = new SimpleIniFile(Path.Combine(folder, "desktop.ini"));
			var localized = desktopIni.Read("LocalizedFileNames", name, name);
			if (localized[0] == '@')
			{
				localized = Utils.ExtractStringFromDLL(localized);
			}

			path = Path.Combine(folder, name);
			Name = localized;
			Description = description;
		}

		protected override bool Check() => File.Exists(path);

		public override void Optimize() => File.Delete(path);
	}
}
