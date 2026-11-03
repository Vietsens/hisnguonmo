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
using HIS.Desktop.Plugins.Library.KhoDuLieuYTe.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe
{
    /// <summary>Popup kết quả gửi Kho dữ liệu SYT, dùng chung cho màn trẻ sơ sinh và màn tử vong.</summary>
    public class frmKdlResult : XtraForm
    {
        private readonly List<KdlResultADO> results;
        private GridControl gridControl;
        private GridView gridView;

        public frmKdlResult(List<KdlResultADO> results, string certCaption)
        {
            this.results = results ?? new List<KdlResultADO>();
            InitControls(certCaption);
        }

        private void InitControls(string certCaption)
        {
            this.Text = "Kết quả gửi Kho dữ liệu ngành Y tế (SYT)";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(900, 480);
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            LabelControl lblSummary = new LabelControl();
            lblSummary.Dock = DockStyle.Top;
            lblSummary.Padding = new Padding(8, 8, 8, 8);
            lblSummary.AutoSizeMode = LabelAutoSizeMode.None;
            lblSummary.Height = 32;
            lblSummary.Text = string.Format("Đã chọn: {0}    Thành công: {1}    Đã có trên kho: {2}    Lỗi: {3}    Bỏ qua: {4}",
                this.results.Count,
                this.results.Count(o => o.Status == KdlResultADO.STATUS__SUCCESS),
                this.results.Count(o => o.Status == KdlResultADO.STATUS__EXISTED),
                this.results.Count(o => o.Status == KdlResultADO.STATUS__FAILED),
                this.results.Count(o => o.Status == KdlResultADO.STATUS__SKIPPED));

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
            AddColumn(certCaption, "CertCode", 140);
            AddColumn("Kết quả", "StatusText", 90);
            AddColumn("Chi tiết", "Message", 360);
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
                KdlResultADO row = gridView.GetRow(e.RowHandle) as KdlResultADO;
                if (row == null) return;
                if (row.Status == KdlResultADO.STATUS__FAILED)
                    e.Appearance.ForeColor = Color.Red;
                else if (row.Status == KdlResultADO.STATUS__SUCCESS || row.Status == KdlResultADO.STATUS__EXISTED)
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
                    .Where(o => o.Status == KdlResultADO.STATUS__FAILED)
                    .Select(o => string.Format("{0}\t{1}\t{2}", o.TreatmentCode, o.CertCode, o.Message))
                    .ToList();
                if (lines.Count == 0)
                {
                    XtraMessageBox.Show("Không có bản ghi lỗi.", "Thông báo");
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
