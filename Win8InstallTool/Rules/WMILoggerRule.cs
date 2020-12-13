using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Win8InstallTool.Rules
{
	public class WMILoggerRule : ImutatableRule
	{
		public override string Name { get; }

		public override string Description { get; }

		private readonly string key;

		private readonly bool? cycle;
		private readonly int? maxFileSize;

		private int? fileModeTarget;
		private int? fileSizeTarget;

		public WMILoggerRule(string name, string description, string key, bool? cycle, int? maxFileSize)
		{
			Name = name;
			Description = description;
			this.key = key;
			this.cycle = cycle;
			this.maxFileSize = maxFileSize;
		}

		protected override bool Check()
		{
			if (cycle.HasValue)
			{
				var mode = (int)Registry.GetValue(key, "LogFileMode", 0);
				if ((mode & 2) == 0)
				{
					fileModeTarget = mode | 2;
				}
			}

			if (maxFileSize.HasValue)
			{
				var size = (int)Registry.GetValue(key, "MaxFileSize", 0);
				if(maxFileSize.Value > size)
				{
					fileSizeTarget = maxFileSize;
				}
			}

			return fileModeTarget.HasValue || fileSizeTarget.HasValue;
		}

		public override void Optimize()
		{
			if (fileModeTarget.HasValue)
			{
				Registry.SetValue(key, "LogFileMode", fileModeTarget, RegistryValueKind.DWord);
			}
			if (fileSizeTarget.HasValue)
			{
				Registry.SetValue(key, "MaxFileSize", fileSizeTarget, RegistryValueKind.DWord);
			}
		}
	}
}
