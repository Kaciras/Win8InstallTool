namespace Win8InstallTool
{
	partial class MainWindow
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.GroupBox groupBox1;
			this.textBox = new System.Windows.Forms.RichTextBox();
			this.scanButton = new System.Windows.Forms.Button();
			this.btnOptimize = new System.Windows.Forms.Button();
			this.btnSelectAll = new System.Windows.Forms.Button();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.treeView = new System.Windows.Forms.TreeView();
			this.roleLabel = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			groupBox1.Controls.Add(this.textBox);
			groupBox1.Location = new System.Drawing.Point(500, 47);
			groupBox1.Margin = new System.Windows.Forms.Padding(5);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(170, 244);
			groupBox1.TabIndex = 6;
			groupBox1.TabStop = false;
			groupBox1.Text = "描述";
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.BackColor = System.Drawing.SystemColors.Control;
			this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox.Location = new System.Drawing.Point(6, 20);
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.Size = new System.Drawing.Size(158, 218);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "";
			// 
			// scanButton
			// 
			this.scanButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.scanButton.Location = new System.Drawing.Point(500, 339);
			this.scanButton.Margin = new System.Windows.Forms.Padding(5);
			this.scanButton.Name = "scanButton";
			this.scanButton.Size = new System.Drawing.Size(80, 28);
			this.scanButton.TabIndex = 2;
			this.scanButton.Text = "扫描";
			this.scanButton.UseVisualStyleBackColor = true;
			this.scanButton.Click += new System.EventHandler(this.ScanButton_Click);
			// 
			// btnOptimize
			// 
			this.btnOptimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOptimize.Enabled = false;
			this.btnOptimize.Location = new System.Drawing.Point(590, 339);
			this.btnOptimize.Margin = new System.Windows.Forms.Padding(5);
			this.btnOptimize.Name = "btnOptimize";
			this.btnOptimize.Size = new System.Drawing.Size(80, 28);
			this.btnOptimize.TabIndex = 3;
			this.btnOptimize.Text = "优化";
			this.btnOptimize.UseVisualStyleBackColor = true;
			this.btnOptimize.Click += new System.EventHandler(this.BtnOptimize_Click);
			// 
			// btnSelectAll
			// 
			this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectAll.Enabled = false;
			this.btnSelectAll.Location = new System.Drawing.Point(500, 301);
			this.btnSelectAll.Margin = new System.Windows.Forms.Padding(5);
			this.btnSelectAll.Name = "btnSelectAll";
			this.btnSelectAll.Size = new System.Drawing.Size(80, 28);
			this.btnSelectAll.TabIndex = 4;
			this.btnSelectAll.Text = "全选";
			this.btnSelectAll.UseVisualStyleBackColor = true;
			this.btnSelectAll.Click += new System.EventHandler(this.BtnSelectAll_Click);
			// 
			// btnClearAll
			// 
			this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearAll.Enabled = false;
			this.btnClearAll.Location = new System.Drawing.Point(590, 301);
			this.btnClearAll.Margin = new System.Windows.Forms.Padding(5);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Size = new System.Drawing.Size(80, 28);
			this.btnClearAll.TabIndex = 5;
			this.btnClearAll.Text = "全不选";
			this.btnClearAll.UseVisualStyleBackColor = true;
			this.btnClearAll.Click += new System.EventHandler(this.BtnClearAll_Click);
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(14, 375);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(656, 24);
			this.progressBar.TabIndex = 7;
			// 
			// treeView
			// 
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeView.CheckBoxes = true;
			this.treeView.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.treeView.FullRowSelect = true;
			this.treeView.Location = new System.Drawing.Point(14, 14);
			this.treeView.Margin = new System.Windows.Forms.Padding(5);
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(476, 353);
			this.treeView.TabIndex = 8;
			this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterCheck);
			this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseClick);
			// 
			// roleLabel
			// 
			this.roleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.roleLabel.BackColor = System.Drawing.SystemColors.Control;
			this.roleLabel.Font = new System.Drawing.Font("Microsoft YaHei Light", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.roleLabel.ForeColor = System.Drawing.Color.Green;
			this.roleLabel.Location = new System.Drawing.Point(508, 14);
			this.roleLabel.Margin = new System.Windows.Forms.Padding(5);
			this.roleLabel.Name = "roleLabel";
			this.roleLabel.Size = new System.Drawing.Size(156, 23);
			this.roleLabel.TabIndex = 9;
			this.roleLabel.Text = "标准权限";
			this.roleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 411);
			this.Controls.Add(this.roleLabel);
			this.Controls.Add(this.treeView);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(groupBox1);
			this.Controls.Add(this.btnOptimize);
			this.Controls.Add(this.scanButton);
			this.Controls.Add(this.btnClearAll);
			this.Controls.Add(this.btnSelectAll);
			this.MinimumSize = new System.Drawing.Size(600, 450);
			this.Name = "MainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button scanButton;
		private System.Windows.Forms.Button btnOptimize;
		private System.Windows.Forms.Button btnSelectAll;
		private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Label roleLabel;
		private System.Windows.Forms.RichTextBox textBox;
	}
}

