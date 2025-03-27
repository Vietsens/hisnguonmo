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
using HIS.UC.UCOtherServiceReqInfo.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;

namespace HIS.UC.UCOtherServiceReqInfo
{
    public partial class UCOtherServiceReqInfo : UserControlBase
    {
        #region Get - Set Data

        private HisPatientSDO  patientSdo { get; set; }
        private HIS_TREATMENT TreatmentByPatientSdo { get; set; }

        public void DisposeControl()
        {
            try
            {

                dataClassify = null;
                dataOtherPayTemp = null;
                IsChangeFromClassify = false;
                hasDataAutoCheckPriority = false;
                _IsAutoSetOweType = false;
                _BranchTimes = null;
                _IsUserBranchTime = false;
                workingPatientType = null;
                _HisTreatment = null;
                dlgPriorityNumberChanged = null;
                dlgFocusNextUserControl = null;
                _PatientName = null;
                this.txtGuaranteeReason.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtGuaranteeReason_PreviewKeyDown);
                this.cboGuaranteeUsername.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboGuaranteeUsername_Closed);
                this.cboGuaranteeUsername.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboGuaranteeUsername_ButtonClick);
                this.cboGuaranteeUsername.EditValueChanged -= new System.EventHandler(this.cboGuaranteeUsername_EditValueChanged);
                this.cboGuaranteeUsername.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboGuaranteeUsername_PreviewKeyDown);
                this.txtGuaranteeLoginname.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtGuaranteeLoginname_PreviewKeyDown);
                this.cboPatientClassify.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPatientClassify_ButtonClick);
                this.cboPatientClassify.EditValueChanged -= new System.EventHandler(this.cboPatientClassify_EditValueChanged);
                this.txtIncode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIncode_PreviewKeyDown);
                this.cboOtherPaySource.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboOtherPaySource_ButtonClick);
                this.cboOtherPaySource.EditValueChanged -= new System.EventHandler(this.cboOtherPaySource_EditValueChanged);
                this.cboPriorityType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPriorityType_Closed);
                this.cboPriorityType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPriorityType_ButtonClick);
                this.cboPriorityType.EditValueChanged -= new System.EventHandler(this.cboPriorityType_EditValueChanged);
                this.cboPriorityType.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboPriorityType_KeyDown);
                this.cboPriorityType.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPriorityType_KeyUp);
                this.cboPriorityType.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboPriorityType_PreviewKeyDown);
                this.txtTreatmentOrder.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtTreatmentOrder_KeyPress);
                this.txtSTTPriority.EditValueChanged -= new System.EventHandler(this.txtSTTPriority_EditValueChanged);
                this.btnAddCTT.Click -= new System.EventHandler(this.btnAddCTT_Click);
                this.cboCTT.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCTT_Closed);
                this.cboCTT.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboCTT_ButtonClick);
                this.txtIntructionTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtIntructionTime_ButtonClick);
                this.txtIntructionTime.EditValueChanged -= new System.EventHandler(this.txtIntructionTime_EditValueChanged);
                this.txtIntructionTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtIntructionTime_KeyDown);
                this.txtIntructionTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIntructionTime_PreviewKeyDown);
                this.dtIntructionTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtIntructionTime_Closed);
                this.dtIntructionTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtIntructionTime_KeyDown);
                this.dtIntructionTime.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.dtIntructionTime_KeyPress);
                this.chkIsChronic.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.chkIsChronic_KeyDown);
                this.cboOweType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboOweType_Closed);
                this.cboTreatmentType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTreatmentType_Closed);
                this.cboTreatmentType.EditValueChanged -= new System.EventHandler(this.cboTreatmentType_EditValueChanged);
                this.chkIsNotRequireFee.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkIsNotRequireFee_PreviewKeyDown);
                this.chkPriority.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkPriority_PreviewKeyDown);
                this.chkEmergency.EditValueChanged -= new System.EventHandler(this.chkEmergency_EditValueChanged);
                this.chkEmergency.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEmergency_PreviewKeyDown);
                this.cboEmergencyTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEmergencyTime_Closed);
                this.Load -= new System.EventHandler(this.UCOtherServiceReqInfo_Load);
                gridView3.GridControl.DataSource = null;
                gridView2.GridControl.DataSource = null;

                gridView1.GridControl.DataSource = null;

                gridLookUpEdit1View.GridControl.DataSource = null;

                layoutControlItem4 = null;
                txtNote = null;
                toolTipItem2 = null;
                toolTipItem1 = null;
                //emptySpaceItem2 = null;
                lciTuberculosis = null;
                chkTuberculosis = null;
                lciGuaranteeReason = null;
                txtGuaranteeReason = null;
                lciGuaranteeUsername = null;
                gridView3 = null;
                cboGuaranteeUsername = null;
                lciGuaranteeLoginname = null;
                txtGuaranteeLoginname = null;
                lciPatientClassify = null;
                gridView2 = null;
                cboPatientClassify = null;
                lciFortxtIncode = null;
                txtIncode = null;
                layoutControlItem3 = null;
                gridView1 = null;
                cboOtherPaySource = null;
                lciFortxtMaMS = null;
                lciForchkCapMaMS = null;
                chkCapMaMS = null;
                txtMaMS = null;
                layoutControlItem1 = null;
                cboPriorityType = null;
                lciTreatmentOrder = null;
                txtTreatmentOrder = null;
                timerInitForm = null;
                lciFortxtSTTPriority = null;
                txtSTTPriority = null;
                layoutControlItem2 = null;
                lciCboCTT = null;
                gridLookUpEdit1View = null;
                cboCTT = null;
                btnAddCTT = null;
                dxErrorProviderControl = null;
                dxValidationUCOtherReqInfo = null;
                dtIntructionTime = null;
                txtIntructionTime = null;
                lciIntructionTime = null;
                panel1 = null;
                lciEmergencyTime = null;
                lciEmergency = null;
                lciPriority = null;
                lciIsNotRequireFee = null;
                lciTreatmentType = null;
                lciOweType = null;
                lciIsChronic = null;
                cboEmergencyTime = null;
                chkEmergency = null;
                chkPriority = null;
                chkIsNotRequireFee = null;
                cboTreatmentType = null;
                cboOweType = null;
                chkIsChronic = null;
                lcgOtherRequest = null;
                lcUCOtherServiceReqInfo = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCServiceReqInfoADO GetValue()
        {
            UCServiceReqInfoADO dataServiceReqInfoADO = new UCServiceReqInfoADO();
            try
            {
                dataServiceReqInfoADO.HospitalizationReason = !string.IsNullOrEmpty(txtHosReason.Text.Trim()) ? txtHosReason.Text.Trim() : null;
                dataServiceReqInfoADO.IntructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtIntructionTime.Text).Value.ToString("yyyyMMddHHmm") + "00");
                if (chkCapMaMS.Checked)
                    dataServiceReqInfoADO.IsCapMaMS = true;
                else
                    dataServiceReqInfoADO.IsCapMaMS = false;
                if (chkEmergency.Checked)
                    dataServiceReqInfoADO.IsEmergency = true;
                else
                    dataServiceReqInfoADO.IsEmergency = false;
                if (chkIsChronic.Checked)
                    dataServiceReqInfoADO.IsChronic = true;
                else
                    dataServiceReqInfoADO.IsChronic = false;
                if (chkIsNotRequireFee.Checked)
                    dataServiceReqInfoADO.IsNotRequireFee = true;
                else
                    dataServiceReqInfoADO.IsNotRequireFee = false;
                if (chkPriority.Checked)
                    dataServiceReqInfoADO.IsPriority = true;
                else
                    dataServiceReqInfoADO.IsPriority = false;
                if (this.cboOweType.EditValue != null)
                {
                    dataServiceReqInfoADO.OweType_ID = (long)this.cboOweType.EditValue;
                    var dataCHeck = BackendDataWorker.Get<HIS_OWE_TYPE>().FirstOrDefault(p => p.IS_ACTIVE == 1 && p.ID == dataServiceReqInfoADO.OweType_ID);
                    if (dataCHeck == null || dataCHeck.ID <= 0)
                        dataServiceReqInfoADO.OweType_ID = 0;

                }
                if (this.cboTreatmentType.EditValue != null)
                    dataServiceReqInfoADO.TreatmentType_ID = (long)this.cboTreatmentType.EditValue;
                if (this.cboOtherPaySource.EditValue != null)
                    dataServiceReqInfoADO.OTHER_PAY_SOURCE_ID = (long)this.cboOtherPaySource.EditValue;
                if (this.cboPriorityType.EditValue != null)
                    dataServiceReqInfoADO.PriorityType = (long)this.cboPriorityType.EditValue;
                else dataServiceReqInfoADO.PriorityType = null;
                if (this.cboEmergencyTime.EditValue != null)
                    dataServiceReqInfoADO.EmergencyTime_ID = (long)this.cboEmergencyTime.EditValue;
                if (this.txtSTTPriority.EditValue != null)
                    dataServiceReqInfoADO.PriorityNumber = (long)this.txtSTTPriority.Value;
                if (this.cboCTT.EditValue != null && this._HisTreatment != null)
                {
                    dataServiceReqInfoADO.FUND_ID = (long)this.cboCTT.EditValue;
                    if (this._HisTreatment.FUND_BUDGET > 0)
                        dataServiceReqInfoADO.FUND_BUDGET = this._HisTreatment.FUND_BUDGET ?? 0;
                    else
                        dataServiceReqInfoADO.FUND_BUDGET = null;
                    dataServiceReqInfoADO.FUND_COMPANY_NAME = this._HisTreatment.FUND_COMPANY_NAME;
                    if (this._HisTreatment.FUND_FROM_TIME > 0)
                        dataServiceReqInfoADO.FUND_FROM_TIME = this._HisTreatment.FUND_FROM_TIME ?? 0;
                    else
                        dataServiceReqInfoADO.FUND_FROM_TIME = null;
                    if (this._HisTreatment.FUND_TO_TIME > 0)
                        dataServiceReqInfoADO.FUND_TO_TIME = this._HisTreatment.FUND_TO_TIME ?? 0;
                    else
                        dataServiceReqInfoADO.FUND_TO_TIME = null;
                    dataServiceReqInfoADO.FUND_ISSUE_TIME = this._HisTreatment.FUND_ISSUE_TIME;
                    dataServiceReqInfoADO.FUND_NUMBER = this._HisTreatment.FUND_NUMBER;
                    dataServiceReqInfoADO.FUND_TYPE_NAME = this._HisTreatment.FUND_TYPE_NAME;
                    dataServiceReqInfoADO.FUND_CUSTOMER_NAME = this._HisTreatment.FUND_CUSTOMER_NAME;
                }
                dataServiceReqInfoADO.IN_CODE = txtIncode.Text;
                dataServiceReqInfoADO.MaMS = txtMaMS.Text;
                if (!String.IsNullOrWhiteSpace(txtTreatmentOrder.Text))
                {
                    dataServiceReqInfoADO.TreatmentOrder = Convert.ToInt64(txtTreatmentOrder.Text.Trim());
                }
                else
                {
                    dataServiceReqInfoADO.TreatmentOrder = null;
                }

                if (this.cboPatientClassify.EditValue != null)
                    dataServiceReqInfoADO.PATIENT_CLASSIFY_ID = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientClassify.EditValue.ToString());

                if (this.cboGuaranteeUsername.EditValue != null)
                {
                    dataServiceReqInfoADO.GUARANTEE_LOGINNAME = this.txtGuaranteeLoginname.Text.Trim();
                    dataServiceReqInfoADO.GUARANTEE_USERNAME = this.cboGuaranteeUsername.Text.Trim();
                }

                dataServiceReqInfoADO.GUARANTEE_REASON = this.txtGuaranteeReason.Text.Trim();
                dataServiceReqInfoADO.NOTE = this.txtNote.Text.Trim();
                dataServiceReqInfoADO.IsTuberCulosis = this.chkTuberculosis.Checked;
                dataServiceReqInfoADO.IsWarningForNext = this.chkWNext.Checked;
                dataServiceReqInfoADO.IsHiv = this.chkIsHiv.Checked;
                if (cboHosReason.EditValue != null)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.ID == Int64.Parse(cboHosReason.EditValue.ToString()));
                    if (data != null && data.HOSPITALIZE_REASON_NAME == txtHosReasonNt.Text.Trim())
                    {
                        HospitalizeReasonCode = data.HOSPITALIZE_REASON_CODE;
                        HospitalizeReasonName = data.HOSPITALIZE_REASON_NAME;
                    }
                    else
                    {
                        HospitalizeReasonCode = null;
                        HospitalizeReasonName = txtHosReasonNt.Text.Trim();
                    }
                }
                else
                {
                    HospitalizeReasonCode = null;
                    HospitalizeReasonName = txtHosReasonNt.Text.Trim();
                }
                dataServiceReqInfoADO.HospitalizeReasonCode = HospitalizeReasonCode;
                dataServiceReqInfoADO.HospitalizeReasonName = HospitalizeReasonName;
                dataServiceReqInfoADO.IsExamOnline = this.chkExamOnline.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                dataServiceReqInfoADO = null;
            }
            return dataServiceReqInfoADO;
        }

        public void SetValue(UCServiceReqInfoADO dataServiceReqInfoADO)
        {
            try
            {
                if (dataServiceReqInfoADO._FocusNextUserControl != null)
                    this.dlgFocusNextUserControl = dataServiceReqInfoADO._FocusNextUserControl;
                if (dataServiceReqInfoADO != null)
                {
                    txtHosReason.Text = dataServiceReqInfoADO.HospitalizationReason;
                    if (dataServiceReqInfoADO.IsCapMaMS)
                        chkCapMaMS.Checked = true;
                    else
                        chkCapMaMS.Checked = false;
                    if (dataServiceReqInfoADO.IsChronic)
                        chkIsChronic.Checked = true;
                    else
                        chkIsChronic.Checked = false;
                    if (dataServiceReqInfoADO.IsNotRequireFee)
                        chkIsNotRequireFee.Checked = true;
                    else
                        chkIsNotRequireFee.Checked = false;
                    if (dataServiceReqInfoADO.IsPriority)
                    {
                        chkPriority.Checked = true;
                        lciPriority.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    }
                    else
                    {
                        chkPriority.Checked = false;
                        lciPriority.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    }
                    if (dataServiceReqInfoADO.IsEmergency)
                        chkEmergency.Checked = true;
                    else
                        chkEmergency.Checked = false;
                    this.txtMaMS.Text = dataServiceReqInfoADO.MaMS;
                    this.txtIntructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataServiceReqInfoADO.IntructionTime);
                    if (dataServiceReqInfoADO.TreatmentType_ID > 0)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE treatmentType = null;
                        treatmentType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == dataServiceReqInfoADO.TreatmentType_ID);
                        if (treatmentType != null)
                            this.cboTreatmentType.EditValue = treatmentType.ID;
                    }
                    else
                        this.cboTreatmentType.EditValue = null;
                    if (dataServiceReqInfoADO.PriorityType.HasValue && dataServiceReqInfoADO.PriorityType > 0)
                    {
                        MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE priorityType = null;
                        priorityType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE>().FirstOrDefault(o => o.ID == dataServiceReqInfoADO.PriorityType);
                        if (priorityType != null)
                            this.cboPriorityType.EditValue = priorityType.ID;
                    }
                    else
                        this.cboPriorityType.EditValue = null;

                    if (dataServiceReqInfoADO.EmergencyTime_ID > 0)
                    {
                        var _emrgencyTime = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>().FirstOrDefault(o => o.ID == dataServiceReqInfoADO.EmergencyTime_ID);
                        if (_emrgencyTime != null)
                            this.cboEmergencyTime.EditValue = _emrgencyTime.EMERGENCY_WTIME_NAME;
                    }
                    else
                        this.cboEmergencyTime.EditValue = null;

                    if (dataServiceReqInfoADO.OweType_ID > 0)
                    {
                        var _owenType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>().FirstOrDefault(o => o.ID == dataServiceReqInfoADO.OweType_ID);
                        if (_owenType != null)
                            this.cboOweType.EditValue = _owenType.OWE_TYPE_NAME;
                    }
                    else
                        this.cboOweType.EditValue = null;
                    if (dataServiceReqInfoADO.OTHER_PAY_SOURCE_ID > 0)
                    {
                        this.cboOtherPaySource.EditValue = dataServiceReqInfoADO.OTHER_PAY_SOURCE_ID;
                    }
                    else
                        this.cboOtherPaySource.EditValue = null;

                    if (dataServiceReqInfoADO.PriorityNumber.HasValue)
                    {
                        this.txtSTTPriority.EditValue = dataServiceReqInfoADO.PriorityNumber;
                    }
                    else
                        this.txtSTTPriority.EditValue = null;

                    this.txtIncode.Text = dataServiceReqInfoADO.IN_CODE;

                    this.cboPatientClassify.EditValue = dataServiceReqInfoADO.PATIENT_CLASSIFY_ID;

                    if (!String.IsNullOrEmpty(dataServiceReqInfoADO.GUARANTEE_LOGINNAME))
                    {
                        this.cboGuaranteeUsername.EditValue = dataServiceReqInfoADO.GUARANTEE_LOGINNAME;
                    }
                    else
                        this.cboGuaranteeUsername.EditValue = null;

                    this.txtGuaranteeReason.Text = dataServiceReqInfoADO.GUARANTEE_REASON;
                    this.txtNote.Text = dataServiceReqInfoADO.NOTE;
                    this.chkIsHiv.Checked = dataServiceReqInfoADO.IsHiv;
                    if (dataServiceReqInfoADO.IsTuberCulosis)
                        chkTuberculosis.Checked = true;
                    else
                        chkTuberculosis.Checked = false;

                    if (!string.IsNullOrEmpty(dataServiceReqInfoADO.HospitalizeReasonCode))
                    {
                        var lst = cboHosReason.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>;
                        if(lst != null && lst.Count > 0 && lst.FirstOrDefault(o=>o.HOSPITALIZE_REASON_CODE == dataServiceReqInfoADO.HospitalizeReasonCode) != null)
                        {
                            cboHosReason.EditValue = lst.FirstOrDefault(o => o.HOSPITALIZE_REASON_CODE == dataServiceReqInfoADO.HospitalizeReasonCode).ID;
                        }
                        else
                        {
                            txtHosReasonNt.Text = dataServiceReqInfoADO.HospitalizeReasonName;
                        }
                    }
                    else
                    {
                        txtHosReasonNt.Text = dataServiceReqInfoADO.HospitalizeReasonName;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValueIncode(string _inCode)
        {
            try
            {
                this.txtIncode.Text = _inCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValueChronic(bool _isChronic)
        {
            try
            {
                this.chkIsChronic.Checked = _isChronic;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValueByPatientInfo(HisPatientSDO data)
        {
            try
            {
                this.patientSdo = data;
                this.chkIsChronic.Checked = data.IS_CHRONIC == 1;
                this.chkTuberculosis.Checked = data.IS_TUBERCULOSIS == 1;
                this.cboPatientClassify.EditValue = null;
                this.cboPatientClassify.EditValue = data.PATIENT_CLASSIFY_ID;
                this.chkIsHiv.Checked = data.IS_HIV == 1;
                GetTreatment();
                cboTreatmentType_EditValueChanged(null, null);
                //this.AutoCheckPriorityByPriorityType(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetTreatment()
        {

            try
            {
                if(patientSdo == null || !patientSdo.TreatmentId.HasValue)
                {
                    TreatmentByPatientSdo = null;
                    return;
                }

                try
                {
                    if (patientSdo.LastTreatmentFee != null && patientSdo.LastTreatmentFee.ID > 0)
                        TreatmentByPatientSdo = new HIS_TREATMENT() { HOSPITALIZATION_REASON = patientSdo.LastTreatmentFee.HOSPITALIZATION_REASON, ICD_NAME = patientSdo.LastTreatmentFee.ICD_NAME, IS_CHRONIC = patientSdo.LastTreatmentFee.IS_CHRONIC };
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = patientSdo.TreatmentId;
                TreatmentByPatientSdo = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void SetMaMS(string msCode)
        {
            this.txtMaMS.Text = msCode;
        }

        public void SetCapMaMsLayout(bool isEnable)
        {
            try
            {
                chkCapMaMS.Enabled = isEnable;
                chkCapMaMS.ReadOnly = !isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SetEnableChkExamOnline(bool IsEnable)
        {
            chkExamOnline.Enabled = IsEnable;
            if (!IsEnable)
                chkExamOnline.Checked = false;
            chkExamOnline.ReadOnly = !IsEnable;
        }
        #endregion

        #region Refresh UserControl

        public void RefreshUserControl()
        {
            try
            {
                this.patientSdo = null;
                this.dtIntructionTime.EditValue = DateTime.Now;
                this.cboTreatmentType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                this.cboEmergencyTime.EditValue = null;
                this.cboOweType.EditValue = null;
                this.chkIsChronic.Checked = false;
                this.chkEmergency.Checked = false;
                this.chkIsNotRequireFee.Checked = false;
                string option = ConfigApplicationWorker.Get<string>("CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE");
                if (this.workingPatientType != null)
                {
                    if (!String.IsNullOrEmpty(option))
                    {
                        var lstKey = option.Split(',').ToList();
                        this.chkIsNotRequireFee.Checked = (lstKey != null && lstKey.Count > 0 && lstKey.Contains(this.workingPatientType.PATIENT_TYPE_CODE));
                    }
                }
                this.chkPriority.Checked = false;
                this.cboPriorityType.EditValue = null;
                this.cboOtherPaySource.EditValue = null;
                this.cboEmergencyTime.Enabled = false;
                this.chkIsHiv.Checked = false;
                this.cboHosReason.EditValue = null;
                this.FillDataOweTypeDefault();
                this.txtIntructionTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                this.cboCTT.EditValue = null;
                this.txtSTTPriority.EditValue = null;
                this._HisTreatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                this._PatientName = "";
                this.txtTreatmentOrder.Text = "";
                this.txtMaMS.Text = "";
                this.chkCapMaMS.Checked = false;
                this.chkExamOnline.Checked = false;
                this.txtIncode.Text = "";
                lciPriority.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.cboPatientClassify.EditValue = null;
                this.cboGuaranteeUsername.EditValue = null;
                this.txtGuaranteeReason.Text = "";
                this.txtNote.Text = "";
                this.chkTuberculosis.Checked = false;
                this.txtHosReason.Text = null;
                this.txtHosReasonNt.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
