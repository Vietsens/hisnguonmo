using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepartmentExpeMaty.DepartmentExpeMaty
{
    public partial class frmDepartmentExpeMaty : HIS.Desktop.Utility.FormBase
    {
        private void InitSpin(SpinEdit spin)
        {
            spin.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            spin.Properties.Mask.EditMask = "[0-9]{0,}";
            spin.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            spin.Properties.Mask.ShowPlaceHolders = false;
            spin.Properties.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(this.spinMaxExpend_Properties_Spin);
            spin.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinMaxExpend_PreviewKeyDown);

        }
        private void spinMaxExpend_Properties_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            try
            {
                if (!e.IsSpinUp)
                {
                    var spin = sender as SpinEdit;
                    if (spin.Value <= 0)
                    {
                        e.IsSpinUp = true;
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

        }

        private void spinMaxExpend_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    var editor = sender as DevExpress.XtraEditors.SpinEdit;
                    if (editor == null) return;
                    editor.EditValue = null;
                    e.IsInputKey = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
