using HIS.Desktop.Plugins.TreatmentList.ADO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList.Popup
{
    public partial class frmCheckBHYT : Form
    {
        private List<BatchCheckResult> _results;

        public frmCheckBHYT(List<BatchCheckResult> results)
        {
            InitializeComponent();
            _results = results;
        }

        private void frmCheckBHYT_Load(object sender, EventArgs e)
        {
            SetupGrid();
            BindData();
            BestFitGrid();
            AddButtons();
        }

        private void SetupGrid()
        {
            gridColumn1.FieldName = "ROWNUM";
            gridColumn1.Width = 50;

            gridColumn2.FieldName = "TREATMENT_CODE";
            gridColumn2.Width = 120;

            gridColumn3.FieldName = "TDL_PATIENT_NAME";
            gridColumn3.Width = 150;

            gridColumn4.Caption = "Ngày sinh";
            gridColumn4.FieldName = "TDL_PATIENT_DOB";  
            gridColumn4.Width = 100;

            gridColumn5.FieldName = "TDL_HEIN_CARD_NUMBER";
            gridColumn5.Width = 150;

            gridColumn6.FieldName = "Message";
            gridColumn6.Width = 200;

            gridColumn7.FieldName = "Note";
            gridColumn7.Width = 350;
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewBHYT.Columns)
            {
                col.BestFit();
            }
        }

        private void BindData()
        {
            gridControlBHYT.DataSource = _results;
        }

        private void AddButtons()
        {
            this.Text = "Kết quả kiểm tra BHYT";
            this.Size = new System.Drawing.Size(900, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;

            Panel pnl = new Panel { Dock = DockStyle.Bottom, Height = 46 };
            Button btnExport = new Button { Text = "Xuất Excel", Width = 110, Height = 30, Left = 10, Top = 8 };
            Button btnClose = new Button { Text = "Đóng", Width = 80, Height = 30, Left = 130, Top = 8 };

            btnExport.Click += (s, e) => ExportCsv();
            btnClose.Click += (s, e) => this.Close();

            pnl.Controls.Add(btnExport);
            pnl.Controls.Add(btnClose);
            this.Controls.Add(pnl);
        }

        private void ExportCsv()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV file (*.csv)|*.csv";
                sfd.FileName = "KQ_KiemTra_BHYT.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var sw = new StreamWriter(sfd.FileName, false, new System.Text.UTF8Encoding(true)))
                    {
                        sw.WriteLine("STT,Mã điều trị,Họ tên,Ngày sinh,Số thẻ BHYT,Kết quả,Ghi chú");
                        foreach (var r in _results)
                        {
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                                r.ROWNUM,
                                r.TREATMENT_CODE?.Replace("\"", "\"\""),
                                r.TDL_PATIENT_NAME?.Replace("\"", "\"\""),
                                r.TDL_PATIENT_DOB?.Replace("\"", "\"\""),
                                r.TDL_HEIN_CARD_NUMBER?.Replace("\"", "\"\""),
                                r.Message?.Replace("\"", "\"\""),
                                r.Note?.Replace("\"", "\"\"")));
                        }
                    }
                    DevExpress.XtraEditors.XtraMessageBox.Show("Xuất thành công", "Thông báo", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất thất bại: " + ex.Message);
            }
        }
        private void BestFitGrid()
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewBHYT.Columns)
            {
                if (col != gridColumn6)
                    col.BestFit();
            }

        }
    }
}
