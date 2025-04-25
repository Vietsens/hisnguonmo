using HIS.Desktop.DelegateRegister;
using HIS.Desktop.LocalStorage.LocalData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCCheckBoxCCCD : UserControl
    {
        #region Declare

        DelegateNextControl dlgFocusNextUserControl;
        DelegateReloadData delegateReloadData;
        int positionHandle = -1;
        #endregion
        public UCCheckBoxCCCD(DelegateReloadData delegateReloadData)
        {
            InitializeComponent();
            this.chkNoCCCD.TabIndex = this.TabIndex;
            this.delegateReloadData = delegateReloadData;
        }
        internal bool GetValue()
        {
            return chkNoCCCD.Checked;
        }

        #region Focus

        internal void FocusNextControl(DelegateNextControl _dlgFocusNextControl)
        {
            try
            {
                if (_dlgFocusNextControl != null)
                    this.dlgFocusNextUserControl = _dlgFocusNextControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        internal bool ValidateRequiredField()
        {
            bool result = true;
            try
            {
                positionHandle = -1;
                result = dxValidationProvider1.Validate();
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        internal void ResetRequiredField()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            
        }

        private void UcCheckBoxCCCD_Load(object sender, EventArgs e)
        {
            try
            { 
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    var controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                    chkNoCCCD.Enabled = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "HIS000046") != null;
                    chkNoCCCD_CheckedChanged(null, null);
                }
            }
            catch (Exception ex)  
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkNoCCCD_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if((HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "2")) {
                    delegateReloadData(chkNoCCCD.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
