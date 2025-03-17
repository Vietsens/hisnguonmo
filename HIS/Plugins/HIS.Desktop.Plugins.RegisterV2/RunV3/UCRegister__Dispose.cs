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
using System.Threading;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System.Reflection;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Core;
using SDA.SDO;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
	public partial class UCRegister : UserControlBase
	{
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                lstService = null;
                IsEmergency = false;
                baseNameControl = null;
                txtNumberPer = null;
                numTotal = null;
                numSttNow = null;
                roomId = 0;
                isSaveWithRoomHasConfigAllowNotChooseService = false;
                _IsDungTuyenCapCuuByTime = false;
                isNotCheckTT = false;
                isAlertTreatmentEndInDay = false;
                CallConfigString = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                isNotLoadWhileChangeControlStateInFirst = false;
                isResetForm = false;
                _isPatientAppointmentCode = false;
                _TreatmnetIdByAppointmentCode = 0;
                IsPresentAndAppointment = false;
                IsPresent = false;
                ValidatedTTCT = false;
                appointmentCode = null;
                isReadQrCode = false;
                isPrintNow = false;
                actionType = 0;
                isNotPatientDayDob = false;
                ResultDataADO = null;
                _HeinCardData = null;
                resultHisPatientProfileSDO = null;
                currentHisExamServiceReqResultSDO = null;
                frm = null;
                clienttManager = null;
                dataPatientRaw = null;
                dataAddressPatient = null;
                roomExamServiceProcessor = null;
                mainHeinProcessor = null;
                ucHeinBHYT = null;
                currentPatientTypeAllowByPatientType = null;
                serviceReqPrintIds = null;
                isShowMess = false;
                registerNumber = 0;
                ucKskContract = null;
                kskContractProcessor = null;
                currentModule = null;
                serviceReqDetailSDOs = null;
                transPatiADO = null;
                cardSearch = null;
                currentPatientSDO = null;
                gateName = null;
                gateCode = null;
                RegisterGateId = 0;
                KEY_SINGLE = null;
                dicRegisterReq = null;
                RegisterReqIds = null;
                bTo = 0;
                bFrom = 0;
                nTo = 0;
                nFrom = 0;
                callPatientFormName = null;
                count = 0;
                _RegisterGates = null;
                lstPreviousDebtTreatmentsRegister = null;
                menu = null;
                barManager = null;
                isPrintNowBL = false;
                ServiceReqList = null;
                treatmentTypeID = 0;
                EmergencyBol = false;
                lst = null;
                lstModuleLinkApply = null;
                this.chkAutoPay.CheckedChanged -= new System.EventHandler(this.chkAutoPaid_CheckedChanged);
                this.chkAutoDeposit.CheckedChanged -= new System.EventHandler(this.chkAutoDeposit_CheckedChanged);
                this.chkAssignDoctor.CheckedChanged -= new System.EventHandler(this.chkAssignDoctor_CheckedChanged);
                this.chkPrintExam.CheckedChanged -= new System.EventHandler(this.chkPrintExam_CheckedChanged);
                this.chkAutoCreateBill.CheckedChanged -= new System.EventHandler(this.chkAutoCreateBill_CheckedChanged);
                this.chkPrintPatientCard.CheckedChanged -= new System.EventHandler(this.chkPrintPatientCard_CheckedChanged);
                this.btnTTChuyenTuyen.Click -= new System.EventHandler(this.btnTTChuyenTuyen_Click);
                this.txtTo.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtTo_KeyPress);
                this.txtFrom.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtFrom_KeyPress);
                this.btnGiayTo.Click -= new System.EventHandler(this.btnGiayTo_Click);
                this.btnDepositRequest.Click -= new System.EventHandler(this.btnDepositRequest_Click);
                this.dropDownButton__Other.Click -= new System.EventHandler(this.dropDownButton__Other_Click);
                this.btnRecallPatient.Click -= new System.EventHandler(this.btnRecallPatient_Click);
                this.btnCallPatient.Click -= new System.EventHandler(this.btnCallPatient_Click);
                this.txtGateNumber.Leave -= new System.EventHandler(this.txtGateNumber_Leave);
                this.txtGateNumber.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtGateNumber_ButtonClick);
                this.txtStepNumber.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtStepNumber_KeyPress);
                this.txtStepNumber.Leave -= new System.EventHandler(this.txtStepNumber_Leave);
                this.btnSaveAndPrint.Click -= new System.EventHandler(this.btnSaveAndPrint_Click);
                this.btnPatientNew.Click -= new System.EventHandler(this.btnPatientNew_Click);
                this.btnSaveAndAssain.Click -= new System.EventHandler(this.btnSaveAndAssain_Click);
                this.btnDepositDetail.Click -= new System.EventHandler(this.btnDepositDetail_Click);
                this.btnTreatmentBedRoom.Click -= new System.EventHandler(this.btnTreatmentBedRoom_Click);
                this.btnNewContinue.Click -= new System.EventHandler(this.btnNewContinue_Click);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.timerRefeshAutoCreateBill.Tick -= TimerRefeshAutoCreateBill_Tick;
                this.Load -= new System.EventHandler(this.UCRegister_Load);

                layoutControlItem30 = null;
                layoutControlItem29 = null;
                txtFrom = null;
                txtTo = null;
                layoutControlItem28 = null;
                btnGiayTo = null;
                layoutControlItem17 = null;
                chkAutoPay = null;
                chkAutoDeposit = null;
                lciAutoDeposit = null;
                timerRefeshAutoCreateBill = null;
                layoutControlItem27 = null;
                chkAssignDoctor = null;
                layoutControlItem25 = null;
                layoutControlItem26 = null;
                layoutControlItem24 = null;
                layoutControlItem22 = null;
                layoutControlGroup3 = null;
                chkPrintExam = null;
                layoutControl4 = null;
                chkAutoCreateBill = null;
                chkPrintPatientCard = null;
                layoutControlItem19 = null;
                //if (ucCheckTT1 != null)//TODO phuongdt comment
                //    ucCheckTT1.DisposeControl();//TODO phuongdt comment
                ucCheckTT1 = null;
                lcibtnDepositRequest = null;
                btnDepositRequest = null;
                timerInitForm = null;
                lciRegisterNumOrder = null;
                lblRegisterNumOrder = null;
                layoutControlItem12 = null;
                dropDownButton__Other = null;
                emptySpaceItem1 = null;
                ucPlusInfo1 = null;
                layoutControlItem10 = null;
                layoutControlItem7 = null;
                btnTTChuyenTuyen = null;
                lciUCServiceRoomInfo = null;
                ucServiceRoomInfo1 = null;
                layoutControlItem23 = null;
                ucImageInfo1 = null;
                layoutControlItem21 = null;
                //if (ucRelativeInfo1 != null)//TODO phuongdt comment
                //    ucRelativeInfo1.DisposeControl();//TODO phuongdt comment
                ucRelativeInfo1 = null;
                layoutControlItem20 = null;
                //if (ucOtherServiceReqInfo1 != null)//TODO phuongdt comment
                //    ucOtherServiceReqInfo1.DisposeControl();//TODO phuongdt comment
                ucOtherServiceReqInfo1 = null;
                lciUCHeinInfo = null;
                layoutControlItem18 = null;
                //if (ucAddressCombo1 != null)//TODO phuongdt comment
                //    ucAddressCombo1.DisposeControl();//TODO phuongdt comment
                ucAddressCombo1 = null;
                ucHeinInfo1 = null;
                pnlServiceRoomInfomation = null;
                //if (ucPatientRaw1 != null)//TODO phuongdt comment
                //    ucPatientRaw1.DisposeControl();//TODO phuongdt comment
                ucPatientRaw1 = null;
                layoutControlItem16 = null;
                layoutControlItem15 = null;
                layoutControlItem14 = null;
                layoutControlItem13 = null;
                lcibtnDepositDetail = null;
                layoutControlItem11 = null;
                lcibtnPatientNewInfo = null;
                layoutControlItem9 = null;
                layoutControlItem8 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                btnSave = null;
                btnPrint = null;
                btnNewContinue = null;
                btnTreatmentBedRoom = null;
                btnDepositDetail = null;
                btnSaveAndAssain = null;
                btnPatientNew = null;
                btnSaveAndPrint = null;
                cboCashierRoom = null;
                txtStepNumber = null;
                txtGateNumber = null;
                btnCallPatient = null;
                btnRecallPatient = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup2 = null;
                Root = null;
                layoutControl3 = null;
                layoutControl2 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
