using System;
using System.Reflection;
using System.Windows.Forms;

namespace Win8InstallTool;

public partial class AboutWindow : Form
{
	public AboutWindow()
	{
		InitializeComponent();

		var version = Assembly.GetExecutingAssembly().GetName().Version;
		versionLabel.Text = $"版本 {version.ToString(3)}，更新于 2021-11-11";
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.OK;
	}
}
