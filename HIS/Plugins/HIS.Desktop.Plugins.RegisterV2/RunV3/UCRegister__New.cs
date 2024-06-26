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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void RefreshUserControl()
        {
            try
            {
                this.currentHisExamServiceReqResultSDO = null;
                this.serviceReqDetailSDOs = null;
                this.resultHisPatientProfileSDO = null;
                this.dataAddressPatient = new UC.AddressCombo.ADO.UCAddressADO();
                this.ucHeinInfo1.RefreshUserControl();
                this.ucPatientRaw1.RefreshUserControl();
                this.ucAddressCombo1.RefreshUserControl();
                this.ucImageInfo1.RefreshUserControl();
                this.ucOtherServiceReqInfo1.RefreshUserControl();
                this.ucRelativeInfo1.RefreshUserControl();
                this.ucPlusInfo1.RefreshUserControl();
                this.SetPatientSearchPanel(false);
                this.EnableControl(true);
                this.ucCheckTT1.ResetData();
                this.ucServiceRoomInfo1.RefreshUserControl();
                this.transPatiADO = null;
                this.actionType = GlobalVariables.ActionAdd;
                this.frm = null;
                this.ValidatedTTCT = false;
                this.ResetVariableUCAddress(false);
                this._TreatmnetIdByAppointmentCode = 0;
                this.cardSearch = null;

                this.ucHeinInfo1.RefreshUserControl();
                this.ucPatientRaw1.FocusUserControl();

                if (this.ucPatientRaw1 != null && this.ucPatientRaw1.GetValue().PATIENTTYPE_ID > 0)
                {
                    //if (AppConfigs.PatientIdIsNotRequireExamFee != null
                    //    && AppConfigs.PatientIdIsNotRequireExamFee.Count > 0
                    //    && AppConfigs.PatientIdIsNotRequireExamFee.Contains(this.ucPatientRaw1.GetValue().PATIENTTYPE_ID))
                    //{
                    //    this.AutoSetDataForOtherServiceReqInfo(true, this.ucPatientRaw1.GetValue().PATIENTTYPE_ID);
                    //}
                    this.ucOtherServiceReqInfo1.ChangePatientType(this.ucPatientRaw1.GetValue().PATIENTTYPE_ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetVariableUCAddress(bool isTrue)
        {
            try
            {
                this.ucAddressCombo1.isReadCard = isTrue;
                this.ucAddressCombo1.isPatientBHYT = isTrue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPatientSearchPanel(bool isFinded)
        {
            try
            {
                if (isFinded)
                {
                    this.lcibtnPatientNewInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    this.currentPatientSDO = null;
                    this.lcibtnPatientNewInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                Inventec.Common.Logging.LogSystem.Debug("SetPatientSearchPanel");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControl(bool _isEnable)
        {
            try
            {
                this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = this.btnTTChuyenTuyen.Enabled  = _isEnable;
                this.dropDownButton__Other.Enabled = this.btnDepositDetail.Enabled = this.btnDepositRequest.Enabled = btnGiayTo.Enabled = this.btnPrint.Enabled = this.btnSaveAndAssain.Enabled = !_isEnable;
                HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter = null;

                //resultHisPatientProfileSDO,currentHisExamServiceReqResultSDO = null khi bam nut. Va chi ton tai 1 bien co gia tri. Yen tam di
                if (currentHisExamServiceReqResultSDO != null && currentHisExamServiceReqResultSDO.HisPatientProfile != null && currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter != null)
                {
                    hisPatientTypeAlter = currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter;
                }

                if (resultHisPatientProfileSDO != null && resultHisPatientProfileSDO.HisPatientTypeAlter != null)
                {
                    hisPatientTypeAlter = resultHisPatientProfileSDO.HisPatientTypeAlter;
                }

                if (hisPatientTypeAlter != null)
                {
                    if (hisPatientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        this.btnTreatmentBedRoom.Enabled = !_isEnable;
                    }
                    else
                    {
                        this.btnTreatmentBedRoom.Enabled = false;
                    }
                }
                else
                {
                    this.btnTreatmentBedRoom.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
