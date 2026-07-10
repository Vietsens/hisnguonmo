using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// F1 trên các ô "Nếu có đề nghị ghi rõ tên bệnh" (tab trên-18, lái xe) → mở frmSubIcd chọn nhiều mã bệnh,
    /// điền CHỈ mã ICD (nối ";") vào ô đang focus. Wiring KeyUp + NullValuePrompt đặt trong Designer.
    /// </summary>
    public partial class frmEnterKskInfomantionVer2
    {
        private BaseEdit currentIcdF1Edit;

        private void IcdF1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.F1) return;
                BaseEdit edit = sender as BaseEdit;
                if (edit == null || !edit.Enabled) return;
                this.currentIcdF1Edit = edit;

                string curCodes = edit.Text ?? string.Empty;
                int pageSize = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;

                frmSubIcd frm = new frmSubIcd(new DelegateRefeshIcdChandoanphu(ReceiveIcdCodes),
                    curCodes, string.Empty, pageSize, new List<HIS_ICD>());
                frm.ShowDialog();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Chỉ điền MÃ bệnh (nối ";") vào ô đang mở F1.</summary>
        private void ReceiveIcdCodes(string icdCodes, string icdNames)
        {
            try
            {
                if (this.currentIcdF1Edit == null) return;
                this.currentIcdF1Edit.Text = string.IsNullOrEmpty(icdCodes) ? null : icdCodes;
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }
}
