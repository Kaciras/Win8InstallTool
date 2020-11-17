using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win8InstallTool
{
	public sealed partial class MainWindow : Form
	{
		const int WINDOW_INIT_WIDTH = 1000;
		const int WINDOW_INIT_HEIGHT = 600;

		readonly Lazy<RestoreWindow> restoreWindow = new Lazy<RestoreWindow>(() => new RestoreWindow());

		public MainWindow()
		{
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;

			foreach (var item in Program.Rules)
			{
				CreatePanel(item.Key, item.Value);
			}

			Text = "SystemOptimizer";
			Width = WINDOW_INIT_WIDTH;
			Height = WINDOW_INIT_HEIGHT;
			Tab_SelectedIndexChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// 根据已注册的优化分类创建相应的面板
		/// </summary>
		/// <param name="type">分类名</param>
		/// <param name="rules">包含的规则集</param>
		void CreatePanel(string type, IEnumerable<IRuleSet> rules)
		{
			var treeView = new TreeView
			{
				CheckBoxes = true,
				Dock = DockStyle.Fill,
				FullRowSelect = true,
				BorderStyle = BorderStyle.None,
				Font = new Font("微软雅黑", 9.5F),
			};
			treeView.NodeMouseClick += TreeView_NodeMouseClick;
			treeView.AfterCheck += TreeView_AfterCheck;
			var page = new TabPage
			{
				Text = type,
				UseVisualStyleBackColor = true
			};
			page.Controls.Add(treeView);
			tabPanel.Controls.Add(page);
		}

		/// <summary>
		/// 实现TreeViewItem的选项联动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			e.Node.TreeView.AfterCheck -= TreeView_AfterCheck;
			foreach (TreeNode node in e.Node.Nodes)
			{
				node.Checked = e.Node.Checked;
			}
			var parent = e.Node.Parent;
			if (e.Node.Parent != null)
			{
				var setNodes = parent.Nodes;
				parent.Checked = setNodes.Cast<TreeNode>().All(n => n.Checked);
			}
			e.Node.TreeView.AfterCheck += TreeView_AfterCheck;
		}

		private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if(e.Node.Parent != null)
			{
				var item = (IOptimizeItem)e.Node.Tag;
				descBox.Text = item.Description;
				descBox.Text += $"\r\n\r\n当前状态: {item.CurrentState}";
			}
		}

		private void Tab_SelectedIndexChanged(object sender, EventArgs e)
		{
			var tp = tabPanel.TabPages[tabPanel.SelectedIndex];
			if (tp.Tag == null)
			{
				Task.Factory.StartNew(FindOptimizable);
				tp.Tag = true;
			}
		}

		private void BtnRefresh_Click(object sender, EventArgs e)
		{
			Task.Factory.StartNew(FindOptimizable);
		}

		TreeView GetCurrentList()
		{
			return (TreeView)tabPanel.TabPages[tabPanel.SelectedIndex].Controls[0];
		}

		void FindOptimizable()
		{
			var page = tabPanel.TabPages[tabPanel.SelectedIndex];
			var tree = (TreeView)page.Controls[0];
			var rSets = Program.Rules[page.Text];

			tabPanel.Enabled = false;
			tree.Nodes.Clear();
			tree.BeginUpdate();

			foreach (var set in rSets)
			{
				var setNode = tree.Nodes[set.Name] ?? new TreeNode(set.Name);
				foreach (var item in set.FindOptimizeItems())
				{
					var itemView = new TreeNode($"[{item.Target}] {item.Name}");
					itemView.Tag = item;
					Invoke(new Action(() => setNode.Nodes.Add(itemView)));
				}
				if(setNode.Nodes.Count > 0)
				{
					Invoke(new Action(() => tree.Nodes.Add(setNode)));
				}
			}
			tree.EndUpdate();
			tabPanel.Enabled = true;
		}

		private void BtnOptimize_Click(object sender, EventArgs e)
		{
			var page = tabPanel.TabPages[tabPanel.SelectedIndex];

			var checkeds = from TreeNode gp in GetCurrentList().Nodes
						   from TreeNode item in gp.Nodes
						   where item.Checked
						   select item;

			var r = new OptimizeResult(page.Text);
			foreach (var vItem in checkeds)
			{
				var oItem = (IOptimizeItem)vItem.Tag;
				var cmd = oItem.Command;
				try
				{
					if (backupBox.Checked)
						Program.AddRecord(page.Text, vItem.Parent.Text, oItem.Name, cmd);
					cmd.Execute();
					r.Success.Add(oItem);
				}
				catch (SecurityException)
				{
					r.SecurtyFailure.Add(oItem);
				}
			}
			FindOptimizable();
			new ReportDialog(r).ShowDialog(this);
		}

		private void BtnSelectAll_Click(object sender, EventArgs e)
			=> GetCurrentList().ChangeAllChecked(ch => true);

		private void BtnClearAll_Click(object sender, EventArgs e)
			=> GetCurrentList().ChangeAllChecked(ch => false);

		private void ButtonRestore_Click(object sender, EventArgs e)
		{
			var window = restoreWindow.Value;
			window.LoadBackups();
			window.ShowDialog(this);
		}
	}
}
