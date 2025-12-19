using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Form Event Handlers

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FillDataToForm();
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FillDataToForm();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            FillDataToForm();
        }

        private void bbiFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnFind.Enabled)
                btnFind.PerformClick();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave.PerformClick();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ProcessSave();
        }

        #endregion
    }
}
