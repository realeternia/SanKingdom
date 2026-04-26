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

        private ConfigData currentConfig;
        private string currentFilePath;
        private DataTable dataTable;
        private Dictionary<string, ColorInfo> colorMap;

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
            LoadColorMap(filePath);
            BuildDataTable();
            ApplyColors();
        }

        private void BuildDataTable()
        {
            dataTable = new DataTable();

            foreach (var field in currentConfig.Fields)
            {
                var col = dataTable.Columns.Add(field.Name, typeof(string));
            }

            foreach (var row in currentConfig.Rows)
            {
                var dataRow = dataTable.NewRow();
                foreach (var field in currentConfig.Fields)
                {
                    string val = row.ContainsKey(field.Name) ? row[field.Name] : "";
                    dataRow[field.Name] = val;
                }
                dataTable.Rows.Add(dataRow);
            }

            dataGridView1.DataSource = dataTable;

            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                if (i < dataGridView1.Columns.Count)
                {
                    string tip = currentConfig.Fields[i].Comment;
                    if (!string.IsNullOrEmpty(tip))
                    {
                        dataGridView1.Columns[i].HeaderText = currentConfig.Fields[i].Name;
                        dataGridView1.Columns[i].ToolTipText = tip;
                    }
                }
            }
        }

        private void SyncDataTableToConfig()
        {
            if (currentConfig == null || dataTable == null) return;

            currentConfig.Rows.Clear();
            foreach (DataRow dr in dataTable.Rows)
            {
                var row = new Dictionary<string, string>();
                foreach (var field in currentConfig.Fields)
                {
                    row[field.Name] = dr[field.Name] == DBNull.Value ? "" : dr[field.Name].ToString();
                }
                currentConfig.Rows.Add(row);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || string.IsNullOrEmpty(currentFilePath)) return;

            SyncDataTableToConfig();
            string newSource = currentConfig.GenerateSource();
            File.WriteAllText(currentFilePath, newSource, Encoding.UTF8);
            SaveColorMap(currentFilePath);
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNewRow_Click(object sender, EventArgs e)
        {
            if (dataTable == null) return;
            var row = dataTable.NewRow();
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
                if (!selRow.IsNewRow)
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
                if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0 && cell.ColumnIndex < currentConfig.Fields.Count)
                {
                    string fieldType = currentConfig.Fields[cell.ColumnIndex].Type;
                    if (!ValidateInput(input, fieldType))
                    {
                        MessageBox.Show(string.Format("值\"{0}\"不符合字段\"{1}\"的类型({2})", input, currentConfig.Fields[cell.ColumnIndex].Name, fieldType), "类型错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0)
                {
                    cell.Value = input;
                }
            }
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
                cell.Style.ForeColor = colorDialog1.Color;
                SetCellColor(cell, true, colorDialog1.Color);
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
                cell.Style.BackColor = colorDialog1.Color;
                SetCellColor(cell, false, colorDialog1.Color);
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
                cell.Style.ForeColor = Color.Empty;
                cell.Style.BackColor = Color.Empty;
                string key = cell.RowIndex + "," + cell.ColumnIndex;
                if (colorMap != null) colorMap.Remove(key);
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

        private void SetCellColor(DataGridViewCell cell, bool isFore, Color color)
        {
            if (colorMap == null) colorMap = new Dictionary<string, ColorInfo>();
            string key = cell.RowIndex + "," + cell.ColumnIndex;
            ColorInfo ci;
            if (colorMap.ContainsKey(key))
                ci = colorMap[key];
            else
                ci = new ColorInfo();

            if (isFore)
                ci.ForeColor = color;
            else
                ci.BackColor = color;

            colorMap[key] = ci;
        }

        private void LoadColorMap(string csFilePath)
        {
            colorMap = new Dictionary<string, ColorInfo>();
            string colorFile = Path.ChangeExtension(csFilePath, ".color");
            if (!File.Exists(colorFile)) return;

            try
            {
                string[] lines = File.ReadAllLines(colorFile, Encoding.UTF8);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string key = parts[0];
                    var ci = new ColorInfo();

                    if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
                    {
                        ci.ForeColor = Color.FromArgb(int.Parse(parts[1]));
                    }
                    if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
                    {
                        ci.BackColor = Color.FromArgb(int.Parse(parts[2]));
                    }

                    colorMap[key] = ci;
                }
            }
            catch { }
        }

        private void SaveColorMap(string csFilePath)
        {
            if (colorMap == null || colorMap.Count == 0) return;

            string colorFile = Path.ChangeExtension(csFilePath, ".color");
            var sb = new StringBuilder();
            foreach (var kv in colorMap)
            {
                sb.Append(kv.Key);
                sb.Append('\t');
                sb.Append(kv.Value.ForeColor.HasValue ? kv.Value.ForeColor.Value.ToArgb().ToString() : "");
                sb.Append('\t');
                sb.Append(kv.Value.BackColor.HasValue ? kv.Value.BackColor.Value.ToArgb().ToString() : "");
                sb.AppendLine();
            }
            File.WriteAllText(colorFile, sb.ToString(), Encoding.UTF8);
        }

        private void ApplyColors()
        {
            if (colorMap == null || colorMap.Count == 0) return;

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dataTable;

            for (int colIdx = 0; colIdx < currentConfig.Fields.Count; colIdx++)
            {
                if (colIdx < dataGridView1.Columns.Count)
                {
                    string tip = currentConfig.Fields[colIdx].Comment;
                    if (!string.IsNullOrEmpty(tip))
                    {
                        dataGridView1.Columns[colIdx].HeaderText = currentConfig.Fields[colIdx].Name;
                        dataGridView1.Columns[colIdx].ToolTipText = tip;
                    }
                }
            }

            for (int rowIdx = 0; rowIdx < dataGridView1.Rows.Count; rowIdx++)
            {
                for (int colIdx = 0; colIdx < dataGridView1.Columns.Count; colIdx++)
                {
                    string key = rowIdx + "," + colIdx;
                    if (colorMap.ContainsKey(key))
                    {
                        var ci = colorMap[key];
                        if (ci.ForeColor.HasValue)
                            dataGridView1.Rows[rowIdx].Cells[colIdx].Style.ForeColor = ci.ForeColor.Value;
                        if (ci.BackColor.HasValue)
                            dataGridView1.Rows[rowIdx].Cells[colIdx].Style.BackColor = ci.BackColor.Value;
                    }
                }
            }
        }

        private class ColorInfo
        {
            public Color? ForeColor;
            public Color? BackColor;
        }
    }
}
