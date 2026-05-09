/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BidCreate.Forms
{
    public partial class frmTransferMediOrgSelect : Form
    {
        /// <summary>Default prefix khi ghép vào TextEdit preview theo thiết kế.</summary>
        private const string DEFAULT_PREFIX = "C.";

        private List<HIS_MEDI_ORG> allMediOrgs = new List<HIS_MEDI_ORG>();

        /// <summary>Cờ chặn FocusedRowChanged ghi đè preview khi popup mới load.</summary>
        private bool suppressAutoFillPreview = true;

        /// <summary>Giá trị cuối cùng (đã ghép, đã edit prefix nếu có) sau khi user chọn.</summary>
        public string SelectedTransferCode { get; private set; }

        public frmTransferMediOrgSelect()
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(
                    System.Windows.Forms.Application.StartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTransferMediOrgSelect_Load(object sender, EventArgs e)
        {
            try
            {
                // Load danh mục HIS_MEDI_ORG từ RAM cache (IS_ACTIVE=1, IS_DELETE!=1)
                allMediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                             && o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                    .OrderBy(o => o.MEDI_ORG_CODE)
                    .ToList();

                FillGrid(allMediOrgs);
                txtTransferCodePreview.Text = "";
                suppressAutoFillPreview = false; // cho phép FocusedRowChanged hoạt động sau khi load xong
                txtKeyword.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillGrid(List<HIS_MEDI_ORG> source)
        {
            try
            {
                gridViewMediOrg.BeginUpdate();
                gridControlMediOrg.DataSource = source;
                gridViewMediOrg.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = (txtKeyword.Text ?? "").Trim().ToLower();
                if (string.IsNullOrEmpty(keyword))
                {
                    FillGrid(allMediOrgs);
                    return;
                }

                var filtered = allMediOrgs.Where(o =>
                        (!string.IsNullOrEmpty(o.MEDI_ORG_CODE) && o.MEDI_ORG_CODE.ToLower().Contains(keyword))
                     || (!string.IsNullOrEmpty(o.MEDI_ORG_NAME) && o.MEDI_ORG_NAME.ToLower().Contains(keyword)))
                    .ToList();
                FillGrid(filtered);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewMediOrg.GridControl.Focus();
                    if (gridViewMediOrg.RowCount > 0)
                    {
                        gridViewMediOrg.FocusedRowHandle = 0;
                    }
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    AcceptCurrentSelection();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khi user click 1 dòng grid (focused row đổi) → tự ghép "C." + MEDI_ORG_CODE vào preview.
        /// User có thể sửa prefix "C." trong textedit này trước khi nhấn Chọn.
        /// </summary>
        private void gridViewMediOrg_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                if (suppressAutoFillPreview) return;
                FillPreviewFromFocusedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillPreviewFromFocusedRow()
        {
            var row = gridViewMediOrg.GetFocusedRow() as HIS_MEDI_ORG;
            if (row == null || string.IsNullOrWhiteSpace(row.MEDI_ORG_CODE))
            {
                txtTransferCodePreview.Text = "";
                return;
            }
            txtTransferCodePreview.Text = DEFAULT_PREFIX + row.MEDI_ORG_CODE;
        }

        /// <summary>
        /// Double-click 1 dòng → ghép giá trị {C. + MEDI_ORG_CODE} và đóng popup luôn (không qua bước chỉnh prefix).
        /// </summary>
        private void gridViewMediOrg_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var row = gridViewMediOrg.GetFocusedRow() as HIS_MEDI_ORG;
                if (row == null || string.IsNullOrWhiteSpace(row.MEDI_ORG_CODE)) return;

                this.SelectedTransferCode = DEFAULT_PREFIX + row.MEDI_ORG_CODE;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediOrg_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    AcceptCurrentSelection();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                AcceptCurrentSelection();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTransferMediOrgSelect_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.S && e.Control)
                {
                    AcceptCurrentSelection();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy giá trị từ TextEdit preview (đã được auto-fill từ FocusedRowChanged và user có thể đã sửa prefix).
        /// </summary>
        private void AcceptCurrentSelection()
        {
            try
            {
                string val = (txtTransferCodePreview.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(val)) return; // chưa chọn dòng nào

                this.SelectedTransferCode = val;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
