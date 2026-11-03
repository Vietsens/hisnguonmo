/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.FundAllocation.ADO;
using HIS.Desktop.Plugins.FundAllocation.Base;
using HIS.Desktop.Plugins.FundAllocation.Resources;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.FundAllocation
{
    /// <summary>
    /// Allocation of a treatment's costs between the commercial guarantee insurer (fund) and the patient
    /// </summary>
    public partial class frmFundAllocation : FormBase
    {
        private const short IS_TRUE = 1;

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long treatmentId;
        private RefeshReference refreshReference;

        private HIS_TREATMENT treatment;
        private HIS_FUND fund;
        private V_HIS_FUND_ALLOCATION allocation;
        private List<FundAllocationLineADO> lines = new List<FundAllocationLineADO>();
        private string loginName;
        private bool isDirty = false;
        private bool isBinding = false;

        public frmFundAllocation()
        {
            InitializeComponent();
        }

        public frmFundAllocation(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId, RefeshReference refreshReference)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.treatmentId = treatmentId;
                this.refreshReference = refreshReference;
                if (moduleData != null && !String.IsNullOrWhiteSpace(moduleData.text))
                {
                    this.Text = moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmFundAllocation_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                this.SetCaptionByLanguageKey();
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.InitGridFund();
                this.InitGridPatient();
                this.InitGridHistory();
                this.LoadData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.S))
                {
                    if (this.btnSave.Enabled && this.lciSave.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        this.btnSave_Click(null, null);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lciTreatmentCode.Text = ResourceLanguageManager.Get("frmFundAllocation.lciTreatmentCode.Text");
                this.lciPatient.Text = ResourceLanguageManager.Get("frmFundAllocation.lciPatient.Text");
                this.lciFund.Text = ResourceLanguageManager.Get("frmFundAllocation.lciFund.Text");
                this.lciFundNumber.Text = ResourceLanguageManager.Get("frmFundAllocation.lciFundNumber.Text");
                this.lciFundTime.Text = ResourceLanguageManager.Get("frmFundAllocation.lciFundTime.Text");
                this.lciStatus.Text = ResourceLanguageManager.Get("frmFundAllocation.lciStatus.Text");
                this.lciVersion.Text = ResourceLanguageManager.Get("frmFundAllocation.lciVersion.Text");
                this.lciFundBudget.Text = ResourceLanguageManager.Get("frmFundAllocation.lciFundBudget.Text");
                this.lciSurchargeRatio.Text = ResourceLanguageManager.Get("frmFundAllocation.lciSurchargeRatio.Text");
                this.tabPageAllocation.Text = ResourceLanguageManager.Get("frmFundAllocation.tabPageAllocation.Text");
                this.tabPageHistory.Text = ResourceLanguageManager.Get("frmFundAllocation.tabPageHistory.Text");
                this.lblFundCaption.Text = ResourceLanguageManager.Get("frmFundAllocation.lblFundCaption.Text");
                this.lblPatientCaption.Text = ResourceLanguageManager.Get("frmFundAllocation.lblPatientCaption.Text");
                this.btnMoveToFund.Text = ResourceLanguageManager.Get("frmFundAllocation.btnMoveToFund.Text");
                this.btnMoveToPatient.Text = ResourceLanguageManager.Get("frmFundAllocation.btnMoveToPatient.Text");
                this.lblMoveHint.Text = ResourceLanguageManager.Get("frmFundAllocation.lblMoveHint.Text");
                this.btnSave.Text = ResourceLanguageManager.Get("frmFundAllocation.btnSave.Text");
                this.btnFinish.Text = ResourceLanguageManager.Get("frmFundAllocation.btnFinish.Text");
                this.btnReopen.Text = ResourceLanguageManager.Get("frmFundAllocation.btnReopen.Text");
                this.btnCancelAllocation.Text = ResourceLanguageManager.Get("frmFundAllocation.btnCancelAllocation.Text");
                this.btnMoveToFund.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.btnMoveToFund.ToolTip");
                this.btnMoveToPatient.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.btnMoveToPatient.ToolTip");
                this.btnSave.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.btnSave.ToolTip");
                this.btnFinish.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.btnFinish.ToolTip");
                this.btnReopen.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.btnReopen.ToolTip");
                this.btnCancelAllocation.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.btnCancelAllocation.ToolTip");
                this.spinFundBudget.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.spinFundBudget.ToolTip");
                this.spinSurchargeRatio.ToolTip = ResourceLanguageManager.Get("frmFundAllocation.spinSurchargeRatio.ToolTip");
                if (this.moduleData == null || String.IsNullOrWhiteSpace(this.moduleData.text))
                {
                    this.Text = ResourceLanguageManager.Get("frmFundAllocation.Text");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool HasControl(string controlCode)
        {
            try
            {
                return GlobalVariables.AcsAuthorizeSDO != null
                    && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null
                    && GlobalVariables.AcsAuthorizeSDO.ControlInRoles.Any(o => o.CONTROL_CODE == controlCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private bool IsFinished()
        {
            return this.allocation != null && this.allocation.IS_FINISH == IS_TRUE;
        }

        private string GetStatusText(V_HIS_FUND_ALLOCATION data)
        {
            if (data == null)
            {
                return ResourceLanguageManager.Get("frmFundAllocation.Status.ChuaPhanBo");
            }
            string text;
            if (data.IS_CANCEL == IS_TRUE)
            {
                text = ResourceLanguageManager.Get("frmFundAllocation.Status.DaHuy");
            }
            else if (data.IS_FINISH == IS_TRUE)
            {
                text = ResourceLanguageManager.Get("frmFundAllocation.Status.DaChot");
            }
            else
            {
                text = ResourceLanguageManager.Get("frmFundAllocation.Status.DaLuu");
            }
            if (data.IS_CURRENT == IS_TRUE && data.IS_COST_CHANGED == IS_TRUE)
            {
                text += " - " + ResourceLanguageManager.Get("frmFundAllocation.Status.CoThayDoiChiPhi");
            }
            return text;
        }

        /// <summary>
        /// Enable/visible state of editors and buttons by allocation state and permission
        /// </summary>
        private void ApplyState()
        {
            try
            {
                bool hasFund = this.treatment != null && this.treatment.FUND_ID.HasValue;
                bool finished = this.IsFinished();
                bool editable = hasFund && !finished;

                this.gridViewFund.OptionsBehavior.Editable = editable;
                this.gridViewPatient.OptionsBehavior.Editable = editable;
                this.btnMoveToFund.Enabled = editable;
                this.btnMoveToPatient.Enabled = editable;
                this.btnSave.Enabled = editable;
                this.btnFinish.Enabled = editable && this.allocation != null;

                bool canEditBudget = editable && this.HasControl(ControlCode.EDIT_BUDGET_AND_RATIO);
                this.spinFundBudget.Properties.ReadOnly = !canEditBudget;
                this.spinSurchargeRatio.Properties.ReadOnly = !canEditBudget;

                this.lciReopen.Visibility = finished && String.Equals(this.allocation.FINISH_LOGINNAME, this.loginName, StringComparison.OrdinalIgnoreCase)
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                this.lciCancelAllocation.Visibility = this.allocation != null && this.HasControl(ControlCode.CANCEL_ALLOCATION)
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
