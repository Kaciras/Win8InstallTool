using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win8InstallTool
{
	public sealed partial class MainWindow : Form
	{
		private readonly bool isElevated = Utils.IsElevated();

		public MainWindow()
		{
			CheckForIllegalCrossThreadCalls = false;
			InitializeComponent();

			var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
			Text = $"Kaciras的Win8优化工具 v{appVersion.ToString(3)}";
		}

		/// <summary>
		/// 实现 TreeViewItem 的选项联动
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
				var item = (Optimizable) e.Node.Tag;
				descBox.Text = item.Description;
				//descBox.Text += $"\r\n\r\n当前状态: {item.CurrentState}";
			}
		}

		private void BtnOptimize_Click(object sender, EventArgs e)
		{
			var checkeds = from TreeNode gp in treeView.Nodes
						   from TreeNode item in gp.Nodes
						   where item.Checked
						   select item;

			foreach (var vItem in checkeds)
			{
				var optimizable = (Optimizable)vItem.Tag;
				try
				{
					optimizable.Optimize();
				}
				catch (SecurityException)
				{
					break;
				}
			}
		}

		private void BtnSelectAll_Click(object sender, EventArgs e) => ChangeAllChecked(_ => true);

		private void BtnClearAll_Click(object sender, EventArgs e) => ChangeAllChecked(_ => false);

		private void ChangeAllChecked(Func<bool, bool> func)
		{
			treeView.BeginUpdate();
			foreach (TreeNode item in treeView.Nodes)
			{
				item.Checked = func(item.Checked);
			}
			treeView.EndUpdate();
		}

		private async void ScanButton_Click(object sender, EventArgs e)
        {
			btnClearAll.Enabled = false;
			btnOptimize.Enabled = false;
			btnSelectAll.Enabled = false;

			await Task.Run(FindOptimizable);

			btnClearAll.Enabled = true;
			btnOptimize.Enabled = true;
			btnSelectAll.Enabled = true;
		}
		
		void FindOptimizable()
		{
			treeView.BeginUpdate();
			treeView.Nodes.Clear();

			foreach (var set in InternalRuleList.Scan(isElevated))
			{
				var setNode = new TreeNode(set.Name);

				foreach (var item in set.Items)
				{
					var itemView = new TreeNode(item.Name);
					itemView.Tag = item;
					Invoke(new Action(() => setNode.Nodes.Add(itemView)));
				}

				Invoke(new Action(() => treeView.Nodes.Add(setNode)));
			}

			treeView.EndUpdate();
		}
    }
}
