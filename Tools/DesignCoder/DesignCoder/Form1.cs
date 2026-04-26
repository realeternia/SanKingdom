using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DesignCoder
{
    public partial class Form1 : Form
    {
        private const string ConfigDir = @"D:\U3dPrj\SanKingdom\Assets\Resources\Scripts\Configs";
        private const int HeaderRowCount = 3;

        private ConfigData currentConfig;
        private string currentFilePath;
        private DataTable dataTable;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void RefreshFileList()
        {
            listView1.Items.Clear();
            if (!Directory.Exists(ConfigDir)) return;

            var files = Directory.GetFiles(ConfigDir, "*_s.cs");
            foreach (var f in files)
            {
                string name = Path.GetFileNameWithoutExtension(f);
                if (name.EndsWith("_s")) name = name.Substring(0, name.Length - 2);
                listView1.Items.Add(name);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            string selectedName = listView1.SelectedItems[0].Text;
            string filePath = Path.Combine(ConfigDir, selectedName + "_s.cs");

            if (!File.Exists(filePath)) return;

            currentFilePath = filePath;
            LoadConfigFile(filePath);
        }

        private void LoadConfigFile(string filePath)
        {
            string source = File.ReadAllText(filePath, Encoding.UTF8);
            currentConfig = ConfigData.Parse(source);
            BuildDataTable();
            SetupDataGridView();
            ApplyColors();
        }

        private void BuildDataTable()
        {
            dataTable = new DataTable();

            dataTable.Columns.Add("_RowTag_", typeof(string));
            foreach (var field in currentConfig.Fields)
            {
                dataTable.Columns.Add(field.Name, typeof(string));
            }

            var fieldNameRow = dataTable.NewRow();
            fieldNameRow["_RowTag_"] = "FieldName";
            var chineseNameRow = dataTable.NewRow();
            chineseNameRow["_RowTag_"] = "ChineseName";
            var typeRow = dataTable.NewRow();
            typeRow["_RowTag_"] = "Type";

            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                var field = currentConfig.Fields[i];
                fieldNameRow[field.Name] = field.Name;
                chineseNameRow[field.Name] = field.ChineseName ?? field.Comment ?? "";
                typeRow[field.Name] = field.Type;
            }

            dataTable.Rows.Add(fieldNameRow);
            dataTable.Rows.Add(chineseNameRow);
            dataTable.Rows.Add(typeRow);

            foreach (var row in currentConfig.Rows)
            {
                var dataRow = dataTable.NewRow();
                dataRow["_RowTag_"] = "Data";
                foreach (var field in currentConfig.Fields)
                {
                    string val = row.ContainsKey(field.Name) ? row[field.Name] : "";
                    dataRow[field.Name] = val;
                }
                dataTable.Rows.Add(dataRow);
            }

            dataGridView1.DataSource = dataTable;
        }

        private void SetupDataGridView()
        {
            dataGridView1.Columns["_RowTag_"].Visible = false;

            typeof(DataGridView).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic, 
                null, dataGridView1, new object[] { true });

            dataGridView1.SuspendLayout();

            Font dataFont = new Font("微软雅黑", 10F);
            Font headerFont = new Font("微软雅黑", 10F, FontStyle.Bold);
            Color darkBg = Color.FromArgb(45, 45, 48);
            Color darkRow = Color.FromArgb(37, 37, 38);
            Color darkAltRow = Color.FromArgb(42, 42, 44);
            Color darkSelection = Color.FromArgb(0, 122, 204);
            Color textColor = Color.FromArgb(220, 220, 220);
            Color deepMorandiBlue = Color.FromArgb(70, 90, 115);
            Color idColumnBg = Color.FromArgb(35, 60, 95);
            Color idColumnFg = Color.FromArgb(140, 190, 255);

            dataGridView1.BackgroundColor = darkBg;
            dataGridView1.DefaultCellStyle.Font = dataFont;
            dataGridView1.DefaultCellStyle.BackColor = darkRow;
            dataGridView1.DefaultCellStyle.ForeColor = textColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = darkSelection;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = darkAltRow;

            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersHeight = 28;
            dataGridView1.RowTemplate.Height = 26;

            int firstDataColIdx = -1;
            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                string colName = currentConfig.Fields[i].Name;
                if (dataGridView1.Columns.Contains(colName))
                {
                    dataGridView1.Columns[colName].HeaderText = colName;
                    dataGridView1.Columns[colName].DefaultCellStyle.Font = dataFont;
                    dataGridView1.Columns[colName].DefaultCellStyle.BackColor = darkRow;
                    dataGridView1.Columns[colName].DefaultCellStyle.ForeColor = textColor;

                    if (colName == "Id" && firstDataColIdx < 0)
                    {
                        firstDataColIdx = dataGridView1.Columns[colName].Index;
                    }
                }
            }

            if (firstDataColIdx >= 0)
            {
                dataGridView1.Columns[firstDataColIdx].DefaultCellStyle.BackColor = idColumnBg;
                dataGridView1.Columns[firstDataColIdx].DefaultCellStyle.Font = headerFont;
                dataGridView1.Columns[firstDataColIdx].DefaultCellStyle.ForeColor = idColumnFg;
            }

            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;

            for (int i = 0; i < HeaderRowCount && i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Frozen = true;
                dataGridView1.Rows[i].ReadOnly = true;
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = deepMorandiBlue;
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                dataGridView1.Rows[i].DefaultCellStyle.Font = headerFont;
                dataGridView1.Rows[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Rows[i].DefaultCellStyle.SelectionBackColor = deepMorandiBlue;
                dataGridView1.Rows[i].DefaultCellStyle.SelectionForeColor = Color.White;
            }

            dataGridView1.GridColor = Color.FromArgb(60, 60, 65);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.BorderStyle = BorderStyle.None;

            dataGridView1.ResumeLayout(false);

            dataGridView1.FirstDisplayedScrollingRowIndex = HeaderRowCount;
        }

        private void SyncDataTableToConfig()
        {
            if (currentConfig == null || dataTable == null) return;

            currentConfig.Rows.Clear();
            foreach (DataRow dr in dataTable.Rows)
            {
                string tag = dr["_RowTag_"] as string;
                if (tag != "Data") continue;

                var row = new Dictionary<string, string>();
                foreach (var field in currentConfig.Fields)
                {
                    row[field.Name] = dr[field.Name] == DBNull.Value ? "" : dr[field.Name].ToString();
                }
                currentConfig.Rows.Add(row);
            }
        }

        private void SyncCellMetasToConfig()
        {
            if (currentConfig == null) return;
            currentConfig.CellMetas.Clear();

            for (int rowIdx = HeaderRowCount; rowIdx < dataGridView1.Rows.Count; rowIdx++)
            {
                for (int colIdx = 0; colIdx < dataGridView1.Columns.Count; colIdx++)
                {
                    if (!dataGridView1.Columns[colIdx].Visible) continue;

                    var cell = dataGridView1.Rows[rowIdx].Cells[colIdx];
                    bool hasForeColor = cell.Style.ForeColor != Color.Empty && cell.Style.ForeColor != dataGridView1.DefaultCellStyle.ForeColor;
                    bool hasBackColor = cell.Style.BackColor != Color.Empty && cell.Style.BackColor != dataGridView1.DefaultCellStyle.BackColor;

                    if (hasForeColor || hasBackColor)
                    {
                        var cm = new CellMeta();
                        cm.Row = rowIdx - HeaderRowCount;
                        cm.Col = colIdx;
                        if (hasForeColor) cm.ForeColor = cell.Style.ForeColor.ToArgb();
                        if (hasBackColor) cm.BackColor = cell.Style.BackColor.ToArgb();
                        currentConfig.CellMetas.Add(cm);
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || string.IsNullOrEmpty(currentFilePath)) return;

            SyncDataTableToConfig();
            SyncCellMetasToConfig();
            string newSource = currentConfig.GenerateSource();
            File.WriteAllText(currentFilePath, newSource, Encoding.UTF8);
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNewRow_Click(object sender, EventArgs e)
        {
            if (dataTable == null) return;
            var row = dataTable.NewRow();
            row["_RowTag_"] = "Data";
            foreach (var field in currentConfig.Fields)
            {
                row[field.Name] = "";
            }
            dataTable.Rows.Add(row);
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedRows.Count == 0) return;

            foreach (DataGridViewRow selRow in dataGridView1.SelectedRows)
            {
                if (selRow.Index >= HeaderRowCount && !selRow.IsNewRow)
                    dataGridView1.Rows.Remove(selRow);
            }
        }

        private void btnBatchFill_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要填充的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string input = ShowInputDialog("批量填充", "请输入要填充的值：");
            if (input == null) return;

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex < HeaderRowCount) continue;
                if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0)
                {
                    int fieldIdx = GetFieldIndexFromColumnIndex(cell.ColumnIndex);
                    if (fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count)
                    {
                        string fieldType = currentConfig.Fields[fieldIdx].Type;
                        if (!ValidateInput(input, fieldType))
                        {
                            MessageBox.Show(string.Format("值\"{0}\"不符合字段\"{1}\"的类型({2})", input, currentConfig.Fields[fieldIdx].Name, fieldType), "类型错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount && cell.ColumnIndex >= 0)
                {
                    cell.Value = input;
                }
            }
        }

        private int GetFieldIndexFromColumnIndex(int colIdx)
        {
            if (colIdx <= 0) return -1;
            string colName = dataGridView1.Columns[colIdx].Name;
            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                if (currentConfig.Fields[i].Name == colName)
                    return i;
            }
            return -1;
        }

        private bool ValidateInput(string input, string type)
        {
            if (string.IsNullOrEmpty(input)) return true;

            if (type == "int")
            {
                int v;
                return int.TryParse(input, out v);
            }
            if (type == "float")
            {
                string s = input.TrimEnd('f', 'F');
                float v;
                return float.TryParse(s, out v);
            }
            if (type == "bool")
            {
                return input == "true" || input == "false" || input == "True" || input == "False";
            }
            return true;
        }

        private void btnForeColor_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要设置的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (colorDialog1.ShowDialog() != DialogResult.OK) return;

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount)
                    cell.Style.ForeColor = colorDialog1.Color;
            }
        }

        private void btnBackColor_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要设置的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (colorDialog1.ShowDialog() != DialogResult.OK) return;

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount)
                    cell.Style.BackColor = colorDialog1.Color;
            }
        }

        private void btnClearColors_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要清除颜色的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount)
                {
                    cell.Style.ForeColor = Color.Empty;
                    cell.Style.BackColor = Color.Empty;
                }
            }
        }

        private string ShowInputDialog(string title, string prompt)
        {
            Form inputForm = new Form();
            inputForm.Text = title;
            inputForm.Size = new Size(300, 150);
            inputForm.StartPosition = FormStartPosition.CenterParent;
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.MaximizeBox = false;
            inputForm.MinimizeBox = false;

            Label lbl = new Label();
            lbl.Text = prompt;
            lbl.Location = new Point(10, 10);
            lbl.AutoSize = true;
            inputForm.Controls.Add(lbl);

            TextBox txt = new TextBox();
            txt.Location = new Point(10, 35);
            txt.Size = new Size(260, 22);
            inputForm.Controls.Add(txt);

            Button btnOk = new Button();
            btnOk.Text = "确定";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(100, 70);
            inputForm.Controls.Add(btnOk);

            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(190, 70);
            inputForm.Controls.Add(btnCancel);

            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            if (inputForm.ShowDialog() == DialogResult.OK)
                return txt.Text;
            return null;
        }

        private void ApplyColors()
        {
            if (currentConfig == null || currentConfig.CellMetas == null || currentConfig.CellMetas.Count == 0) return;

            foreach (var cm in currentConfig.CellMetas)
            {
                int rowIdx = cm.Row + HeaderRowCount;
                int colIdx = cm.Col;

                if (rowIdx >= HeaderRowCount && rowIdx < dataGridView1.Rows.Count &&
                    colIdx >= 0 && colIdx < dataGridView1.Columns.Count)
                {
                    var cell = dataGridView1.Rows[rowIdx].Cells[colIdx];
                    if (cm.ForeColor.HasValue)
                        cell.Style.ForeColor = Color.FromArgb(cm.ForeColor.Value);
                    if (cm.BackColor.HasValue)
                        cell.Style.BackColor = Color.FromArgb(cm.BackColor.Value);
                }
            }
        }
    }
}
