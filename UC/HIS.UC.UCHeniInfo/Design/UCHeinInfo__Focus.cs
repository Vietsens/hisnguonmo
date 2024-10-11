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
using HIS.UC.UCHeniInfo.ADO;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using HIS.UC.UCHeniInfo.Data;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.UCHeniInfo
{
    public partial class UCHeinInfo : UserControlBase
    {
        #region Outside Focus UsreControl

        public void FocusUserControl()
        {
            try
            {
                if (lciHasDobCertificate.Enabled == true)
                {
                    this.chkHasCardTemp.Focus();
                }
                else
                {
                    this.txtSoThe.Focus();
                    this.txtSoThe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusNextUserControl(DelegateFocusNextUserControl _dlgFocusNextUserControl)
        {
            try
            {
                if (_dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl = _dlgFocusNextUserControl;
                }
                else
                    this.dlgFocusNextUserControl = this.SendTABToNextUserControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SendTABToNextUserControl()
        {
            try
            {
                SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusUserByLiveAreaCode()
        {
            try
            {
                this.cboNoiSong.Focus();
                this.cboNoiSong.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ShowCheckWorkingLetter(bool isShow)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ShowCheckSS(bool En)
        {
            try
            {
                chkSs.Enabled = En;
                if (!En)
                {
                    chkSs.Checked = false;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Inside Focus UserControl

        private void FocusTotxtSoThe()
        {
            try
            {
                txtSoThe.Focus();
                txtSoThe.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusTodtHeinCardFromTime()
        {
            try
            {
                this.txtHeinCardFromTime.Focus();
                this.txtHeinCardFromTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusTotxtHeinCardToTime()
        {
            try
            {
                this.dtHeinCardToTime.Focus();
                this.dtHeinCardToTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusTotxtAddress()
        {
            try
            {
                this.txtAddress.Focus();
                this.txtAddress.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusTotxtMaDKKCBBD()
        {
            try
            {
                this.txtMaDKKCBBD.Focus();
                this.txtMaDKKCBBD.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Set Visible Disable

        public void SetDisableHasCardTemp(bool _isDisable)
        {
            try
            {
                if (_isDisable == true)
                {
                    this.chkHasCardTemp.Enabled = true;
                }
                else
                    this.chkHasCardTemp.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetTypeCardTemp(long patientTypeID)
        {
            try
            {
                if (patientTypeID > 0 && patientTypeID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN)
                    this.isTempQN = true;
                else
                    this.isTempQN = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SetEnableChkSS(bool isEnable)
        {
            try
            {
                chkSs.Enabled = isEnable;
                if (!isEnable) chkSs.Checked = isEnable;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SetEnableControlEmergency(DelegateVisible _dlgEnableEmergency)
        {
            try
            {
                if (_dlgEnableEmergency != null)
                    this.dlgIsEnableEmergency = _dlgEnableEmergency;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetInformationPatientRawFromUC(HIS.UC.UCPatientRaw.UCPatientRaw uc)
        {
            try
            {
                if (uc != null)
                {
                    this.currentPatientRaw = uc;
                    

                }    
                    
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetShowThongTinChuyenTuyen(DelegateVisible _dlgShowThongTinChuyenTuyen)
        {
            try
            {
                if (_dlgShowThongTinChuyenTuyen != null)
                    this.dlgShowThongTinChuyenTuyen = _dlgShowThongTinChuyenTuyen;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDelegateDisableBtnTTCT(DelegateVisible _dlgDisableBtnTTCT)
        {
            try
            {
                if (_dlgDisableBtnTTCT != null)
                    this.dlgDisableBtnTTCT = _dlgDisableBtnTTCT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetCareerByHeinCardNumber(DelegateSetCareerByHeinCardNumber _dlgSetCarerrByHeinCardNumber)
        {
            try
            {
                if (_dlgSetCarerrByHeinCardNumber != null)
                    this.dlgSetCareerByHeinCardNumber = _dlgSetCarerrByHeinCardNumber;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Send3WBhytCode(DelegateSend3WBhytCode _dlg)
        {
            try
            {
                if (_dlg != null)
                    this.dlgSend3WBhytCode = _dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetCurrentModule(Inventec.Desktop.Common.Modules.Module module)
        {
            try
            {
                if (module != null)
                    this.module = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetTreatmentId(long tid)
        {
            try
            {
                if (tid > 0)
                    this.treatmentId = tid;
                else
                    this.treatmentId = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        public void RightRouteEmergencyWhenRegisterOutTime(bool IsDungTuyenCapCuu)
        {
            try
            {
                this.IsDungTuyenCapCuuByTime = IsDungTuyenCapCuu;
                if (IsDungTuyenCapCuu)
                {
                    cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                }
                else if (this.cboHeinRightRoute.EditValue == null
                    || (this.cboHeinRightRoute.EditValue != null
                        && (string)this.cboHeinRightRoute.EditValue != MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE))
                {
                    HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO mediorg = MediOrgDataWorker.MediOrgADOs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                    if (mediorg != null
                        && !string.IsNullOrEmpty(mediorg.MEDI_ORG_CODE)
                        && (HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT == mediorg.MEDI_ORG_CODE
              || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
              || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT))
                    // || this.IsMediOrgRightRouteByCurrent(mediorg.MEDI_ORG_CODE)))
                    {
                        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                    }
                    else if (mediorg != null)
                    {
                        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                    }
                    else
                    {
                        this.cboHeinRightRoute.EditValue = null;
                        this.cboHeinRightRoute.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
