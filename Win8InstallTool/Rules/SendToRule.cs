using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			var appData = GetFolderPath(SpecialFolder.ApplicationData);
			path = Path.Combine(appData, @"Microsoft\Windows\SendTo", name);
			Name = name;
			Description = description;
		}

		protected override bool Check() => File.Exists(path);

		public override void Optimize() => File.Delete(path);
	}
}
