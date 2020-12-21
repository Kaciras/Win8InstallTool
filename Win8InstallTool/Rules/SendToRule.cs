using System.IO;
using static System.Environment;

namespace Win8InstallTool.Rules
{
	public class SendToRule : Rule
	{
		public string Name { get; }

		public string Description { get; }

		readonly string path;

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

		public bool Check() => File.Exists(path);

		public void Optimize() => File.Delete(path);
	}
}
