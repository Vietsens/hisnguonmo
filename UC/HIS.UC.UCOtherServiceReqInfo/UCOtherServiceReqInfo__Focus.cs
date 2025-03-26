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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;

namespace HIS.UC.UCOtherServiceReqInfo
{
    public partial class UCOtherServiceReqInfo : UserControlBase
    {
        #region Outside Focus UserControl

        public void FocusUserControl()
        {
            try
            {
                this.txtIntructionTime.Focus();
                if (txtIntructionTime.Text.Trim().Length >= 4)
                    txtIntructionTime.SelectAll();
                else
                    txtIntructionTime_ButtonClick(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusNextUserControl(Action<object> _dlgFocusNextUserControl)
        {
            try
            {
                if (_dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl = _dlgFocusNextUserControl;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusGuarantee()
        {
            try
            {
                this.txtGuaranteeLoginname.Focus();
                this.txtGuaranteeLoginname.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Inside Focus Control

        private void FocusToPriorityType()
        {
            try
            {
                this.FocusShowpopup(this.cboPriorityType, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToTreatmentType()
        {
            try
            {
                this.FocusShowpopup(this.cboTreatmentType, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToEmergencyTime()
        {
            try
            {
                this.FocusShowpopup(this.cboEmergencyTime, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToIncode()
        {
            try
            {
                txtIncode.Focus();
                txtIncode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusToOweType()
        {
            try
            {
                this.FocusShowpopup(this.cboOweType, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusTochkPriority()
        {
            try
            {
                this.chkPriority.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusTochkEmergency()
        {
            try
            {
                this.chkEmergency.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void FocusTochkIsNotRequireFee()
        {
            try
            {
                this.chkIsNotRequireFee.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusTochkIsChronic()
        {
            try
            {
                this.chkIsChronic.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        public void SetEnableControl(bool isEnable)
        {
            try
            {
                this.chkEmergency.Checked = isEnable;
                this.lciEmergencyTime.Enabled = isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateHeinRightRouteType(Action<bool> isOutTime)
        {
            try
            {
                if (isOutTime != null)
                    this.dlgHeinRightRouteType = isOutTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegatePriorityNumberChanged(Action<long?> priorityNumberChanged)
        {
            try
            {
                if (priorityNumberChanged != null)
                    this.dlgPriorityNumberChanged = priorityNumberChanged;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetBranchTime(List<MOS.EFMODEL.DataModels.HIS_BRANCH_TIME> _branchTimes, bool _isUserBranchTime)
        {
            try
            {
                this._BranchTimes = _branchTimes;
                this._IsUserBranchTime = _isUserBranchTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string _PatientName = "";
        public void SetPatientName(string _patientName)
        {
            try
            {
                this._PatientName = _patientName;
                this._HisTreatment.FUND_CUSTOMER_NAME = _patientName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
