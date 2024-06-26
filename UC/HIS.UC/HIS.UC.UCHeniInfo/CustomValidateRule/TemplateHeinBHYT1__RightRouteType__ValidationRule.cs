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
using HIS.Desktop.Plugins.Library.RegisterConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCHeniInfo.CustomValidateRule
{
    class TemplateHeinBHYT1__RightRouteType__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.LookUpEdit cboHeinRightRoute;
        internal DevExpress.XtraEditors.LookUpEdit cboNoiSong;
        internal DevExpress.XtraEditors.TextEdit txtMaDKKCBBD;
        internal bool IsTempQN;
        internal DevExpress.XtraEditors.CheckEdit chkTempQN;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboHeinRightRoute == null || cboHeinRightRoute.EditValue == null) return valid;
                if (HisConfigCFG.IsNotRequiredRightTypeInCaseOfHavingAreaCode && cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE && ((cboNoiSong.EditValue ?? "").ToString() == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || (cboNoiSong.EditValue ?? "").ToString() == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || (cboNoiSong.EditValue ?? "").ToString() == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3))
                {
                    return true;
                }

                if (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                       || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)
                {
                    if ((!String.IsNullOrEmpty(txtMaDKKCBBD.Text) && HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT != txtMaDKKCBBD.Text)

                        && (cboHeinRightRoute.EditValue.ToString() != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT
                        && cboHeinRightRoute.EditValue.ToString() != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY
                        && cboHeinRightRoute.EditValue.ToString() != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                        && !(this.IsTempQN && chkTempQN.Checked))
                    {
                        valid = false;
                        this.ErrorText = ResourceMessage.BatBuocPhaiChonTruongHop;
                    }
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }

    class UCHeinInfo__RightRouteType__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.LookUpEdit cboHeinRightRoute;
        internal bool IsTempQN;
        internal DevExpress.XtraEditors.CheckEdit chkTempQN;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboHeinRightRoute == null || cboHeinRightRoute.EditValue == null) return valid;
                if (cboHeinRightRoute.Enabled == false)
                {
                    valid = true;
                    return valid;
                }
                else
                {
                    if (IsTempQN == true && chkTempQN.Checked == true && cboHeinRightRoute.EditValue == null)
                        return valid;
                    else if (IsTempQN == true && chkTempQN.Checked == true && cboHeinRightRoute.EditValue != null || cboHeinRightRoute.EditValue != null)
                    {
                        valid = true;
                        return valid;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
