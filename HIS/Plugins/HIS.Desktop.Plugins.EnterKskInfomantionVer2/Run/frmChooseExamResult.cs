using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// Form chon ket qua kham lam sang (>=18 tuoi) de dien vao o "Bệnh tật".
    /// Grid 3 cot: Tên, Kết quả (memo nhieu dong), Chọn (checkbox). Nut "Chọn" gom noi dung cac dong
    /// tich chon -> <see cref="SelectedText"/>.
    /// </summary>
    public partial class frmChooseExamResult : DevExpress.XtraEditors.XtraForm
    {
        private readonly List<KskExamResultADO> items;

        /// <summary>Noi dung da chon (dat khi bam "Chọn"); null neu huy.</summary>
        public string SelectedText { get; private set; }

        public frmChooseExamResult(List<KskExamResultADO> data)
        {
            InitializeComponent();
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            this.items = data ?? new List<KskExamResultADO>();
            this.SelectedText = null;
            this.gridControl1.DataSource = this.items;
        }

        private void btnChon_Click(object sender, EventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();

                var chosen = items.Where(o => o != null && o.Chon).ToList();
                if (chosen.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng tích chọn ít nhất một dòng.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var sb = new StringBuilder();
                foreach (var c in chosen)
                {
                    string kq = (c.KetQua ?? "").Trim();
                    if (string.IsNullOrEmpty(kq)) continue;
                    if (sb.Length > 0) sb.Append(Environment.NewLine);
                    sb.Append(kq);   // chỉ lấy nội dung Kết quả, KHÔNG nối tên vùng
                }
                this.SelectedText = sb.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
    }
}
