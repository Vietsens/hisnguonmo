/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Dialog tổng hợp kết quả đẩy hàng loạt lên cổng ECDS (mô hình frmKskSyncResult).
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.SyncResult
{
    public class frmEcdsSyncResult : DevExpress.XtraEditors.XtraForm
    {
        private LabelControl lblSummary;
        private GridControl grd;
        private GridView gv;
        private SimpleButton btnClose;

        public frmEcdsSyncResult(List<EcdsSyncResultADO> results)
        {
            try
            {
                BuildUi();
                if (results == null) results = new List<EcdsSyncResultADO>();
                int ok = results.Count(o => o.Success);
                int fail = results.Count - ok;
                lblSummary.Text = string.Format("Tổng: {0}   ·   Đã đẩy: {1}   ·   Lỗi: {2}",
                    results.Count, ok, fail);
                grd.DataSource = results;
                SetIcon();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void BuildUi()
        {
            this.Text = "Kết quả đồng bộ ca bệnh lên cổng ECDS";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(720, 440);
            this.MinimizeBox = false; this.MaximizeBox = false;

            lblSummary = new LabelControl();
            lblSummary.Dock = DockStyle.Top; lblSummary.Height = 26;
            lblSummary.Padding = new Padding(8, 6, 0, 0);
            lblSummary.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            grd = new GridControl(); grd.Dock = DockStyle.Fill;
            gv = new GridView(grd); grd.MainView = gv;
            gv.OptionsBehavior.Editable = false;
            gv.OptionsView.ShowGroupPanel = false;
            gv.OptionsView.ColumnAutoWidth = false;

            AddCol("Stt", "STT", 45);
            AddCol("TreatmentCode", "Mã điều trị", 120);
            AddCol("PatientName", "Bệnh nhân", 160);
            AddCol("IcdCode", "ICD", 60);
            AddCol("StatusText", "Trạng thái", 80);
            AddCol("MaCaBenh", "Mã ca bệnh ECDS", 130);
            AddCol("Message", "Thông điệp", 260);

            gv.RowCellStyle += (s, e) =>
            {
                try
                {
                    var row = gv.GetRow(e.RowHandle) as EcdsSyncResultADO;
                    if (row != null && !row.Success) e.Appearance.ForeColor = Color.Firebrick;
                }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            };

            var pnlBottom = new PanelControl();
            pnlBottom.Dock = DockStyle.Bottom; pnlBottom.Height = 40;
            pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btnClose = new SimpleButton() { Text = "Đóng", Size = new Size(90, 26), Location = new Point(620, 7) };
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Click += (s, e) => this.Close();
            pnlBottom.Controls.Add(btnClose);

            this.Controls.Add(grd);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(lblSummary);
        }

        private void AddCol(string field, string caption, int width)
        {
            var col = gv.Columns.AddVisible(field, caption);
            col.Width = width; col.OptionsColumn.AllowEdit = false;
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
    }
}
