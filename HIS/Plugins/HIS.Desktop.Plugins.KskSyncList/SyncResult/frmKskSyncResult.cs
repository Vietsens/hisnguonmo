/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Plugins.KskSyncList.ADO;

namespace HIS.Desktop.Plugins.KskSyncList.SyncResult
{
    public partial class frmKskSyncResult : DevExpress.XtraEditors.XtraForm
    {
        private readonly List<KskSyncResultADO> results;

        public frmKskSyncResult(List<KskSyncResultADO> results)
        {
            InitializeComponent();
            this.results = results ?? new List<KskSyncResultADO>();
        }

        private void frmKskSyncResult_Load(object sender, EventArgs e)
        {
            try
            {
                int total = results.Count;
                int success = results.Count(o => o.IsSuccess);
                int fail = total - success;

                lblTotalValue.Text = total.ToString();
                lblSuccessValue.Text = success.ToString();
                lblFailValue.Text = fail.ToString();

                gridControl1.DataSource = results;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column != colResult) return;
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                KskSyncResultADO row = view.GetRow(e.RowHandle) as KskSyncResultADO;
                if (row == null) return;
                e.Appearance.Options.UseForeColor = true;
                e.Appearance.ForeColor = row.IsSuccess
                    ? System.Drawing.Color.FromArgb(0, 150, 60)
                    : System.Drawing.Color.FromArgb(210, 40, 40);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try { this.Close(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
    }
}
