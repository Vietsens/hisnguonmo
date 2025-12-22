using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region License and Disable Controls

        private bool CheckMchLicense()
        {
            bool result = true;
            try
            {
                string branchCode = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId()).BRANCH_CODE;
                if (string.IsNullOrEmpty(branchCode))
                {
                    Inventec.Common.Logging.LogSystem.Warn("Branch code is empty");
                    return false;
                }

                CommonParam param = new CommonParam();
                result = new BackendAdapter(param)
                    .Post<bool>("api/MchLicense/Check", ApiConsumers.MchConsumer, branchCode, param);

                if (!result)
                {
                    Inventec.Common.Logging.LogSystem.Info("MCH License check failed for branch: " + branchCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void DisableAllControls()
        {
            try
            {
                foreach (Control control in this.Controls)
                {
                    DisableControlRecursive(control);
                }

                if (bbiFind != null) bbiFind.Enabled = false;
                if (bbiSave != null) bbiSave.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisableControlRecursive(Control control)
        {
            try
            {
                control.Enabled = false;

                if (control.HasChildren)
                {
                    foreach (Control childControl in control.Controls)
                    {
                        DisableControlRecursive(childControl);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
