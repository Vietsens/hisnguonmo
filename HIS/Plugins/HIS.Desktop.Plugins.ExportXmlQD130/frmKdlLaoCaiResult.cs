/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>Dong ket qua gui Kho du lieu dung chung tinh Lao Cai cua 1 ho so.</summary>
    public class KdlLaoCaiResultADO
    {
        public const int STATUS__SUCCESS = 1;
        public const int STATUS__FAILED = 2;
        public const int STATUS__SKIPPED = 3;

        public string TreatmentCode { get; set; }
        public string PatientName { get; set; }
        public string IdBanGhi { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }

        public string StatusText
        {
            get
            {
                if (this.Status == STATUS__SUCCESS) return "Thành công";
                if (this.Status == STATUS__FAILED) return "Lỗi";
                return "Bỏ qua";
            }
        }
    }

    /// <summary>Popup ket qua gui Kho du lieu dung chung tinh Lao Cai (viec 62218/62313).</summary>
    public class frmKdlLaoCaiResult : XtraForm
    {
        private readonly List<KdlLaoCaiResultADO> results;
        private GridControl gridControl;
        private GridView gridView;

        public frmKdlLaoCaiResult(List<KdlLaoCaiResultADO> results)
        {
            this.results = results ?? new List<KdlLaoCaiResultADO>();
            InitControls();
        }

        private void InitControls()
        {
            this.Text = "Kết quả gửi Kho dữ liệu dùng chung tỉnh Lào Cai";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(960, 480);
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            LabelControl lblSummary = new LabelControl();
            lblSummary.Dock = DockStyle.Top;
            lblSummary.Padding = new Padding(8, 8, 8, 8);
            lblSummary.AutoSizeMode = LabelAutoSizeMode.None;
            lblSummary.Height = 32;
            lblSummary.Text = string.Format("Đã chọn: {0}    Thành công: {1}    Lỗi: {2}    Bỏ qua: {3}",
                this.results.Count,
                this.results.Count(o => o.Status == KdlLaoCaiResultADO.STATUS__SUCCESS),
                this.results.Count(o => o.Status == KdlLaoCaiResultADO.STATUS__FAILED),
                this.results.Count(o => o.Status == KdlLaoCaiResultADO.STATUS__SKIPPED));

            gridControl = new GridControl();
            gridView = new GridView(gridControl);
            gridControl.MainView = gridView;
            gridControl.Dock = DockStyle.Fill;
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsView.ColumnAutoWidth = false;
            gridView.OptionsView.RowAutoHeight = true;
            AddColumn("Mã điều trị", "TreatmentCode", 100);
            AddColumn("Tên bệnh nhân", "PatientName", 170);
            AddColumn("Mã bản ghi trên Kho", "IdBanGhi", 150);
            AddColumn("Kết quả", "StatusText", 80);
            AddColumn("Chi tiết", "Message", 400);
            gridView.RowCellStyle += gridView_RowCellStyle;
            gridControl.DataSource = this.results;

            PanelControl pnlBottom = new PanelControl();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 40;
            pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            SimpleButton btnClose = new SimpleButton();
            btnClose.Text = "Đóng";
            btnClose.Size = new Size(90, 27);
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Location = new Point(pnlBottom.Width - 100, 6);
            btnClose.Click += (s, e) => this.Close();
            SimpleButton btnCopy = new SimpleButton();
            btnCopy.Text = "Sao chép lỗi";
            btnCopy.Size = new Size(110, 27);
            btnCopy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopy.Location = new Point(pnlBottom.Width - 216, 6);
            btnCopy.Click += btnCopy_Click;
            pnlBottom.Controls.Add(btnCopy);
            pnlBottom.Controls.Add(btnClose);

            this.Controls.Add(gridControl);
            this.Controls.Add(lblSummary);
            this.Controls.Add(pnlBottom);
            this.CancelButton = btnClose;
        }

        private void AddColumn(string caption, string fieldName, int width)
        {
            GridColumn col = gridView.Columns.AddVisible(fieldName, caption);
            col.Width = width;
            if (fieldName == "Message")
                col.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
        }

        private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "StatusText") return;
                KdlLaoCaiResultADO row = gridView.GetRow(e.RowHandle) as KdlLaoCaiResultADO;
                if (row == null) return;
                if (row.Status == KdlLaoCaiResultADO.STATUS__FAILED)
                    e.Appearance.ForeColor = Color.Red;
                else if (row.Status == KdlLaoCaiResultADO.STATUS__SUCCESS)
                    e.Appearance.ForeColor = Color.Green;
                else
                    e.Appearance.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> lines = this.results
                    .Where(o => o.Status == KdlLaoCaiResultADO.STATUS__FAILED)
                    .Select(o => string.Format("{0}\t{1}\t{2}", o.TreatmentCode, o.IdBanGhi, o.Message))
                    .ToList();
                if (lines.Count == 0)
                {
                    XtraMessageBox.Show("Không có hồ sơ lỗi.", "Thông báo");
                    return;
                }
                Clipboard.SetText(string.Join(Environment.NewLine, lines));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
