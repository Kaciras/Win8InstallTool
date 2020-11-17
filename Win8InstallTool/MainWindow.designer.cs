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
			this.tabPanel = new System.Windows.Forms.TabControl();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.btnOptimize = new System.Windows.Forms.Button();
			this.backupBox = new System.Windows.Forms.CheckBox();
			this.btnSelectAll = new System.Windows.Forms.Button();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.buttonRestore = new System.Windows.Forms.Button();
			this.descBox = new System.Windows.Forms.TextBox();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPanel
			// 
			this.tabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabPanel.Location = new System.Drawing.Point(0, 0);
			this.tabPanel.Margin = new System.Windows.Forms.Padding(0);
			this.tabPanel.Name = "tabPanel";
			this.tabPanel.SelectedIndex = 0;
			this.tabPanel.Size = new System.Drawing.Size(399, 361);
			this.tabPanel.TabIndex = 0;
			this.tabPanel.SelectedIndexChanged += new System.EventHandler(this.Tab_SelectedIndexChanged);
			// 
			// btnRefresh
			// 
			this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRefresh.Location = new System.Drawing.Point(417, 321);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(80, 28);
			this.btnRefresh.TabIndex = 2;
			this.btnRefresh.Text = "刷新";
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
			// 
			// btnOptimize
			// 
			this.btnOptimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOptimize.Location = new System.Drawing.Point(503, 321);
			this.btnOptimize.Name = "btnOptimize";
			this.btnOptimize.Size = new System.Drawing.Size(80, 28);
			this.btnOptimize.TabIndex = 3;
			this.btnOptimize.Text = "优化";
			this.btnOptimize.UseVisualStyleBackColor = true;
			this.btnOptimize.Click += new System.EventHandler(this.BtnOptimize_Click);
			// 
			// backupBox
			// 
			this.backupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.backupBox.AutoSize = true;
			this.backupBox.Checked = true;
			this.backupBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.backupBox.Location = new System.Drawing.Point(417, 254);
			this.backupBox.Name = "backupBox";
			this.backupBox.Size = new System.Drawing.Size(72, 16);
			this.backupBox.TabIndex = 0;
			this.backupBox.Text = "自动备份";
			this.backupBox.UseVisualStyleBackColor = true;
			// 
			// btnSelectAll
			// 
			this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectAll.Location = new System.Drawing.Point(417, 281);
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
			this.btnClearAll.Location = new System.Drawing.Point(503, 281);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Size = new System.Drawing.Size(80, 28);
			this.btnClearAll.TabIndex = 5;
			this.btnClearAll.Text = "全不选";
			this.btnClearAll.UseVisualStyleBackColor = true;
			this.btnClearAll.Click += new System.EventHandler(this.BtnClearAll_Click);
			// 
			// buttonRestore
			// 
			this.buttonRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRestore.Location = new System.Drawing.Point(503, 247);
			this.buttonRestore.Name = "buttonRestore";
			this.buttonRestore.Size = new System.Drawing.Size(80, 28);
			this.buttonRestore.TabIndex = 4;
			this.buttonRestore.Text = "还原";
			this.buttonRestore.UseVisualStyleBackColor = true;
			this.buttonRestore.Click += new System.EventHandler(this.ButtonRestore_Click);
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			groupBox1.Controls.Add(this.descBox);
			groupBox1.Location = new System.Drawing.Point(417, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(166, 229);
			groupBox1.TabIndex = 6;
			groupBox1.TabStop = false;
			groupBox1.Text = "描述";
			// 
			// descBox
			// 
			this.descBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.descBox.Location = new System.Drawing.Point(8, 22);
			this.descBox.Margin = new System.Windows.Forms.Padding(5);
			this.descBox.Multiline = true;
			this.descBox.Name = "descBox";
			this.descBox.ReadOnly = true;
			this.descBox.Size = new System.Drawing.Size(150, 199);
			this.descBox.TabIndex = 0;
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(595, 361);
			this.Controls.Add(groupBox1);
			this.Controls.Add(this.btnOptimize);
			this.Controls.Add(this.btnRefresh);
			this.Controls.Add(this.btnClearAll);
			this.Controls.Add(this.backupBox);
			this.Controls.Add(this.buttonRestore);
			this.Controls.Add(this.btnSelectAll);
			this.Controls.Add(this.tabPanel);
			this.MinimumSize = new System.Drawing.Size(500, 300);
			this.Name = "MainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabPanel;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.Button btnOptimize;
		private System.Windows.Forms.CheckBox backupBox;
		private System.Windows.Forms.Button btnSelectAll;
		private System.Windows.Forms.Button btnClearAll;
		private System.Windows.Forms.Button buttonRestore;
		private System.Windows.Forms.TextBox descBox;
	}
}

