﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win8InstallTool;

sealed partial class MainWindow : Form
{
	readonly RuleProvider provider;

	public MainWindow(RuleProvider provider)
	{
		this.provider = provider;
		InitializeComponent();
		CheckForIllegalCrossThreadCalls = false;

		if (provider.AdminMode)
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
	void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
	{
		// 跳过非交互事件，避免自身触发导致死循环
		if (e.Action != TreeViewAction.Unknown)
		{
			treeView.BeginUpdate();
			var current = e.Node;

			foreach (var node in current.Nodes)
			{
				((TreeNode)node).Checked = current.Checked;
			}

			var parent = current.Parent;
			if (parent != null)
			{
				var nodes = parent.Nodes.Cast<TreeNode>();
				parent.Checked = nodes.All(n => n.Checked);
			}

			treeView.EndUpdate();
			UpdateButtonsEnabled();
		}
	}

	void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Node.Parent == null)
		{
			textBox.Text = string.Empty;
		}
		else
		{
			var item = (Optimizable)e.Node.Tag;
			textBox.Text = item.Description;
		}
	}

	void CollapseButton_Click(object sender, EventArgs e)
	{
		treeView.CollapseAll();
	}

	void AboutButton_Click(object sender, EventArgs e)
	{
		new AboutWindow().ShowDialog(this);
	}

	void SelectAllButton_Click(object sender, EventArgs e) => ChangeAllChecked(true);

	void ClearAllButton_Click(object sender, EventArgs e) => ChangeAllChecked(false);

	void ChangeAllChecked(bool value)
	{
		var queue = new Queue<TreeNode>();
		treeView.Nodes.Cast<TreeNode>().ForEach(queue.Enqueue);

		treeView.BeginUpdate();
		while (queue.Count > 0)
		{
			var node = queue.Dequeue();
			node.Nodes.Cast<TreeNode>().ForEach(queue.Enqueue);
			node.Checked = value;
		}
		treeView.EndUpdate();
		UpdateButtonsEnabled();
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

		UpdateButtonsEnabled();
		scanButton.Enabled = true;
	}

	/// <summary>
	/// 刷新所有按钮的禁用状态，逻辑如下：
	/// <list>
	/// <item>扫描和关于永远是启用的。</item>
	/// <item>选择和折叠在 TreeView 有节点的情况下启用。</item>
	/// <item>优化在选中了优化项后启用。</item>
	/// </list>
	/// </summary>
	void UpdateButtonsEnabled()
	{
		var hasNode = treeView.Nodes.Count > 0;
		btnSelectAll.Enabled = hasNode;
		btnClearAll.Enabled = hasNode;
		collapseButton.Enabled = hasNode;

		btnOptimize.Enabled = GetCheckedNodes().Count() > 0;
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
				Invoke(() => setNode.Nodes.Add(node));
			}
			progressBar.Value++;

			if (setNode.Nodes.Count == 0)
			{
				continue;
			}
			Invoke(() => treeView.Nodes.Add(setNode));
		}

		treeView.ExpandAll();
		treeView.EndUpdate();
	}

	async void OptimizeButton_Click(object sender, EventArgs e)
	{
		var checkedNodes = GetCheckedNodes().ToList();

		progressBar.Maximum = checkedNodes.Count;
		progressBar.Value = 0;
		treeView.Enabled = false;

#if DEBUG
		await Task.Run(() => RunOptimize(checkedNodes));
#else
		try
		{
			await Task.Run(() => RunOptimize(checkedNodes));
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "优化时出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
#endif

		treeView.Enabled = true;
		textBox.Text = string.Empty;
	}

	/// <summary>
	/// 获取 TreeView 里所有被选中的二级节点。
	/// </summary>
	IEnumerable<TreeNode> GetCheckedNodes()
	{
		return treeView.Nodes
			.Cast<TreeNode>()
			.SelectMany(p => p.Nodes.Cast<TreeNode>())
			.Where(item => item.Checked);
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
}
