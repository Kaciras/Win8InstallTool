using System;
using System.Reflection;
using System.Windows.Forms;

namespace Win8InstallTool
{
	public partial class AboutWindow : Form
	{
		public AboutWindow()
		{
			InitializeComponent();

			var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
			versionLabel.Text = $"版本 {appVersion.ToString(3)}，更新于 2020-12-21";
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}
	}
}
