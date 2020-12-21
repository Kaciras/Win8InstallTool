using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

			if (Program.IsElevated)
			{
				roleLabel.ForeColor = Color.DeepPink;
				roleLabel.Text = "管理员";
			}
			else
			{
				var tooltip = new ToolTip();
				tooltip.AutomaticDelay = 500;
				tooltip.SetToolTip(roleLabel, "以管理员运行才能扫描系统优化项");
			}
		}

		/// <summary>
		/// 实现 TreeViewItem 的选项联动，可惜 WinForms 不自带三态复选框。
		/// <br/>
		/// TODO: 点太快会导致冲突。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			// 跳过非交互事件，避免自身触发导致死循环
			if (e.Action != TreeViewAction.Unknown)
			{
				treeView.BeginUpdate();
				var current = e.Node;

				current.Nodes.Cast<TreeNode>().ForEach(n => n.Checked = current.Checked);

				var parent = current.Parent;
				if (parent != null)
				{
					parent.Checked = parent.Nodes.Cast<TreeNode>().All(n => n.Checked);
				}

				treeView.EndUpdate();
			}
		}

		void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Parent != null)
			{
				var item = (Optimizable)e.Node.Tag;
				textBox.Text = item.Description;
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
			catch (Exception ex)
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

		private void collapseButton_Click(object sender, EventArgs e)
		{
			treeView.CollapseAll();
		}

		private void aboutButton_Click(object sender, EventArgs e)
		{
			new AboutWindow().ShowDialog(this);
		}

		void BtnSelectAll_Click(object sender, EventArgs e) => ChangeAllChecked(_ => true);

		void BtnClearAll_Click(object sender, EventArgs e) => ChangeAllChecked(_ => false);

		void ChangeAllChecked(Func<bool, bool> func)
		{
			var queue = new Queue<TreeNode>();
			treeView.Nodes.Cast<TreeNode>().ForEach(queue.Enqueue);

			treeView.BeginUpdate();
			while (queue.Count > 0)
			{
				var node = queue.Dequeue();
				node.Nodes.Cast<TreeNode>().ForEach(queue.Enqueue);
				node.Checked = func(node.Checked);
			}
			treeView.EndUpdate();
		}

		async void ScanButton_Click(object sender, EventArgs e)
		{
			collapseButton.Enabled = false;
			scanButton.Enabled = false;
			btnClearAll.Enabled = false;
			btnOptimize.Enabled = false;
			btnSelectAll.Enabled = false;

			progressBar.Maximum = provider.RuleSets.Count;
			progressBar.Value = 0;

			await Task.Run(FindOptimizable);

			collapseButton.Enabled = true;
			scanButton.Enabled = true;
			btnClearAll.Enabled = true;
			btnOptimize.Enabled = true;
			btnSelectAll.Enabled = true;
		}

		void FindOptimizable()
		{
			treeView.Nodes.Clear();
			treeView.BeginUpdate();

			foreach (var set in provider.RuleSets)
			{
				var setNode = new TreeNode(set.Name);

				foreach (var item in set.Scan())
				{
					var node = new TreeNode(item.Name);
					node.Tag = item;
					Invoke(new Action(() => setNode.Nodes.Add(node)));
				}
				progressBar.Value++;

				if (setNode.Nodes.Count == 0)
				{
					continue;
				}
				Invoke(new Action(() => treeView.Nodes.Add(setNode)));
			}

			treeView.ExpandAll();
			treeView.EndUpdate();
		}
	}
}
