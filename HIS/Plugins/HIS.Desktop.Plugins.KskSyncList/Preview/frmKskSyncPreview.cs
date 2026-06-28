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
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.KskSyncList.Preview
{
    public partial class frmKskSyncPreview : DevExpress.XtraEditors.XtraForm
    {
        private readonly V_HIS_KSK_SYNC data;
        private readonly string content;

        public frmKskSyncPreview(V_HIS_KSK_SYNC data, string content)
        {
            InitializeComponent();
            this.data = data;
            this.content = content;
        }

        private void frmKskSyncPreview_Load(object sender, EventArgs e)
        {
            try
            {
                lblKskTypeValue.Text = SafeGet("KSK_TYPE_NAME");
                lblPatientValue.Text = string.Format("{0} - {1}", SafeGet("TDL_PATIENT_NAME"), SafeGet("TDL_PATIENT_CODE"));
                lblConclusionTimeValue.Text = FormatTime("CONCLUSION_TIME");
                lblStatusValue.Text = GetStatusText();
                memoContent.Text = content;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetStatusText()
        {
            try
            {
                int t = 0;
                try { t = Convert.ToInt32(data.SYNC_RESULT_TYPE); }
                catch { t = 0; }
                if (t == 0) t = 1;
                switch (t)
                {
                    case 2: return "Đã đồng bộ";
                    case 3: return "Thất bại";
                    case 4: return "Có chỉnh sửa";
                    default: return "Chưa đồng bộ";
                }
            }
            catch { return "Chưa đồng bộ"; }
        }

        private string SafeGet(string prop)
        {
            try
            {
                if (data == null) return "";
                var p = data.GetType().GetProperty(prop);
                var v = p != null ? p.GetValue(data, null) : null;
                return v == null ? "" : v.ToString();
            }
            catch { return ""; }
        }

        private string FormatTime(string prop)
        {
            try { return Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(SafeGet(prop))); }
            catch { return ""; }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try { this.Close(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
    }
}
