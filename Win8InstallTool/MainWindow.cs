using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win8InstallTool
{
    public sealed partial class MainWindow : Form
	{
		readonly RuleProvider provider;

		public MainWindow(RuleProvider provider)
		{
			this.provider = provider;
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;

			var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
			Text = $"Kaciras 的 Win8 优化工具 v{appVersion.ToString(3)}";
		}

		/// <summary>
		/// 实现 TreeViewItem 的选项联动，可惜 WinForms 不自带三态复选框
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			var current = e.Node;
			current.TreeView.AfterCheck -= TreeView_AfterCheck;

			foreach (TreeNode node in current.Nodes)
			{
				node.Checked = current.Checked;
			}

			var parent = current.Parent;
			if (parent != null)
			{
				var setNodes = parent.Nodes;
				parent.Checked = setNodes.Cast<TreeNode>().All(n => n.Checked);
			}

			current.TreeView.AfterCheck += TreeView_AfterCheck;
		}

		void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Parent != null)
			{
				var item = (Optimizable)e.Node.Tag;
				descBox.Text = item.Description;
				//descBox.Text += $"\r\n\r\n当前状态: {item.CurrentState}";
			}
		}

		async void BtnOptimize_Click(object sender, EventArgs e)
		{
			var checkedNodes = treeView.Nodes
				.Cast<TreeNode>()
				.SelectMany(t => t.Nodes.Cast<TreeNode>())
				.Where(item => item.Checked)
				.ToList();

			progressBar.Maximum = checkedNodes.Count;
			progressBar.Value = 0;

			treeView.Enabled = false;
			try
			{
				await Task.Run(() => RunOptimize(checkedNodes));
			}
			catch(Exception ex)
            {
				MessageBox.Show(ex.Message, "优化时出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Debugger.Break();
            }
			treeView.Enabled = true;
		}

		void RunOptimize(IEnumerable<TreeNode> nodes) 
		{
			foreach (var node in nodes)
			{
				((Optimizable)node.Tag).Optimize();
				progressBar.Value++;

				var parent = node.Parent;
				node.Remove();

				if (parent.Nodes.Count == 0)
				{
					parent.Remove();
				}
				else
				{
					parent.Checked = parent.Nodes
						.Cast<TreeNode>()
						.All(n => n.Checked);
				}
			}
		}

		void BtnSelectAll_Click(object sender, EventArgs e) => ChangeAllChecked(_ => true);

		void BtnClearAll_Click(object sender, EventArgs e) => ChangeAllChecked(_ => false);

		void ChangeAllChecked(Func<bool, bool> func)
		{
			treeView.BeginUpdate();
			foreach (TreeNode item in treeView.Nodes)
			{
				item.Checked = func(item.Checked);
			}
			treeView.EndUpdate();
		}

		async void ScanButton_Click(object sender, EventArgs e)
		{
			btnClearAll.Enabled = false;
			btnOptimize.Enabled = false;
			btnSelectAll.Enabled = false;

			progressBar.Maximum = provider.ProgressMax;
			progressBar.Value = 0;

			provider.OnProgress += Provider_OnProgress;
			await Task.Run(FindOptimizable);
			provider.OnProgress -= Provider_OnProgress;

			btnClearAll.Enabled = true;
			btnOptimize.Enabled = true;
			btnSelectAll.Enabled = true;
		}

		void Provider_OnProgress(object sender, int value)
		{
			progressBar.Value = value;
		}

		void FindOptimizable()
		{
			treeView.BeginUpdate();
			treeView.Nodes.Clear();

			foreach (var set in provider.Scan())
			{
				var setNode = new TreeNode(set.Name);

				foreach (var item in set.Items)
				{
					var node = new TreeNode(item.Name);
					node.Tag = item;
					Invoke(new Action(() => setNode.Nodes.Add(node)));
				}

				if (setNode.Nodes.Count == 0)
				{
					continue;
				}
				Invoke(new Action(() => treeView.Nodes.Add(setNode)));
			}

			treeView.EndUpdate();
		}
	}
}
