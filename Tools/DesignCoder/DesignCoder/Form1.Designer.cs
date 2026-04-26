namespace DesignCoder
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnNewRow = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteRow = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnBatchFill = new System.Windows.Forms.ToolStripButton();
            this.btnForeColor = new System.Windows.Forms.ToolStripButton();
            this.btnBackColor = new System.Windows.Forms.ToolStripButton();
            this.btnClearColors = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuBatchFill = new System.Windows.Forms.ToolStripMenuItem();
            this.menuForeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearColors = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // toolStrip1
            //
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh,
            this.toolStripSeparator1,
            this.btnSave,
            this.btnNewRow,
            this.btnDeleteRow,
            this.toolStripSeparator2,
            this.btnBatchFill,
            this.btnForeColor,
            this.btnBackColor,
            this.btnClearColors});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1399, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            //
            // btnRefresh
            //
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(36, 22);
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            //
            // btnSave
            //
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(36, 22);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnNewRow
            //
            this.btnNewRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNewRow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewRow.Name = "btnNewRow";
            this.btnNewRow.Size = new System.Drawing.Size(48, 22);
            this.btnNewRow.Text = "新增行";
            this.btnNewRow.Click += new System.EventHandler(this.btnNewRow_Click);
            //
            // btnDeleteRow
            //
            this.btnDeleteRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDeleteRow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(48, 22);
            this.btnDeleteRow.Text = "删除行";
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            //
            // btnBatchFill
            //
            this.btnBatchFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBatchFill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBatchFill.Name = "btnBatchFill";
            this.btnBatchFill.Size = new System.Drawing.Size(60, 22);
            this.btnBatchFill.Text = "批量填充";
            this.btnBatchFill.Click += new System.EventHandler(this.btnBatchFill_Click);
            //
            // btnForeColor
            //
            this.btnForeColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnForeColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.Size = new System.Drawing.Size(60, 22);
            this.btnForeColor.Text = "前景色";
            this.btnForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            //
            // btnBackColor
            //
            this.btnBackColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBackColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBackColor.Name = "btnBackColor";
            this.btnBackColor.Size = new System.Drawing.Size(60, 22);
            this.btnBackColor.Text = "背景色";
            this.btnBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            //
            // btnClearColors
            //
            this.btnClearColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClearColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearColors.Name = "btnClearColors";
            this.btnClearColors.Size = new System.Drawing.Size(60, 22);
            this.btnClearColors.Text = "清除颜色";
            this.btnClearColors.Click += new System.EventHandler(this.btnClearColors_Click);
            //
            // splitContainer1
            //
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            //
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1399, 976);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 0;
            //
            // listView1
            //
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(199, 976);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            //
            // columnHeader1
            //
            this.columnHeader1.Text = "配置表";
            this.columnHeader1.Width = 180;
            //
            // dataGridView1
            //
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = true;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1196, 976);
            this.dataGridView1.TabIndex = 0;
            //
            // contextMenuStrip1
            //
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBatchFill,
            this.menuForeColor,
            this.menuBackColor,
            this.menuClearColors});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            //
            // menuBatchFill
            //
            this.menuBatchFill.Name = "menuBatchFill";
            this.menuBatchFill.Size = new System.Drawing.Size(152, 22);
            this.menuBatchFill.Text = "批量填充";
            this.menuBatchFill.Click += new System.EventHandler(this.btnBatchFill_Click);
            //
            // menuForeColor
            //
            this.menuForeColor.Name = "menuForeColor";
            this.menuForeColor.Size = new System.Drawing.Size(152, 22);
            this.menuForeColor.Text = "设置前景色";
            this.menuForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            //
            // menuBackColor
            //
            this.menuBackColor.Name = "menuBackColor";
            this.menuBackColor.Size = new System.Drawing.Size(152, 22);
            this.menuBackColor.Text = "设置背景色";
            this.menuBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            //
            // menuClearColors
            //
            this.menuClearColors.Name = "menuClearColors";
            this.menuClearColors.Size = new System.Drawing.Size(152, 22);
            this.menuClearColors.Text = "清除颜色";
            this.menuClearColors.Click += new System.EventHandler(this.btnClearColors_Click);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1399, 1001);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "ConfigCoder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnNewRow;
        private System.Windows.Forms.ToolStripButton btnDeleteRow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnBatchFill;
        private System.Windows.Forms.ToolStripButton btnForeColor;
        private System.Windows.Forms.ToolStripButton btnBackColor;
        private System.Windows.Forms.ToolStripButton btnClearColors;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuBatchFill;
        private System.Windows.Forms.ToolStripMenuItem menuForeColor;
        private System.Windows.Forms.ToolStripMenuItem menuBackColor;
        private System.Windows.Forms.ToolStripMenuItem menuClearColors;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}
