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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Edit
{
    public abstract class EditAbstract : EntityBase
    {
        protected long Id { get; set; }
        protected decimal Amount { get; set; }
        protected decimal? RawAmount { get; set; }
        protected decimal AmountAvaiable { get; set; }
        protected int DataType { get; set; }
        protected string Code { get; set; }
        protected string Name { get; set; }
        protected string ManuFacturerName { get; set; }
        protected string ServiceUnitName { get; set; }
        protected decimal? Price { get; set; }
        protected string NationalName { get; set; }
        protected long ServiceId { get; set; }
        protected string Concentra { get; set; }
        protected long? MediStockId { get; set; }
        protected string MediStockCode { get; set; }
        protected string MediStockName { get; set; }
        protected long? HeinServiceTypeId { get; set; }
        protected long ServiceTypeId { get; set; }
        protected long? NumOrder { get; set; }
        protected string ActiveIngrBhytCode { get; set; }
        protected string ActiveIngrBhytName { get; set; }
        protected string Sang { get; set; }
        protected string Trua { get; set; }
        protected string Chieu { get; set; }
        protected string Toi { get; set; }
        protected string BreathSpeed { get; set; }
        protected string BreathTime { get; set; }
        protected bool? IsUseOrginalUnitForPres { get; set; }
        protected bool? IsStent { get; set; }

        protected ValidAddRow ValidAddRow { get; set; }
        internal HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService { get; set; }
        internal HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther { get; set; }
        protected CalulateUseTimeTo CalulateUseTimeTo { get; set; }
        protected ExistsAssianInDay ExistsAssianInDay { get; set; }

        protected List<DMediStock1ADO> MediStockD1SDOs { get; set; }
        protected bool? IsOutKtcFee { get; set; }
        protected long TreatmentId { get; set; }
        protected MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        protected long PatientId { get; set; }
        protected long RequestRoomId { get; set; }

        protected List<MediMatyTypeADO> MediMatyTypeADOs { get; set; }
        protected HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfoSDO { get; set; }
        protected frmAssignPrescription frmAssignPrescription { get; set; }
        protected MediMatyTypeADO currentMedicineTypeADOForEdit;
        protected CommonParam Param { get; set; }
        protected MediMatyTypeADO medicineTypeSDO { get; set; }

        protected decimal? Speed { get; set; }
        protected List<long> HtuIds { get; set; }
        protected long? HtuIdNotCheckAcinInteractive { get; set; }
        protected long? MedicineUseFormId { get; set; }
        protected string Tutorial { get; set; }
        protected string HtuText { get; set; }
        protected bool IsExpend { get; set; }
        protected bool IsDisableExpend { get; set; }
        protected decimal? UseDays { get; set; }
        protected object DataRow { get; set; }
        protected string PrimaryKey { get; set; }
        protected string SeriNumber { get; set; }
        protected long? UseCount { get; set; }
        protected long? UseRemainCount { get; set; }
        protected long? MaxReuseCount { get; set; }
        protected bool? IsKidneyShift { get; set; }
        protected decimal? KidneyShiftCount { get; set; }

        public string PackageNumber { get; set; }
        public long? ExpiredDate { get; set; }
        public bool? IsAssignPackage { get; set; }
        public long? MAME_ID { get; set; }
        public long? PREVIOUS_USING_COUNT { get; set; }
        public short? IS_SPLIT_COMPENSATION { get; set; }
        protected short? IS_OUT_HOSPITAL { get; set; }
        public bool IsMultiDateState { get; set; }
        public List<long> IntructionTimeSelecteds { get; set; }
        protected long? SERVICE_CONDITION_ID { get; set; }
        protected string SERVICE_CONDITION_NAME { get; set; }
        protected short? IS_SUB_PRES { get; set; }
        public bool IsNotOutStock { get; set; }
        protected string ExceedLimitInPresReason { get; set; }
        protected string ExceedLimitInDayReason { get; set; }
        protected string ExceedLimitInBatchReason { get; set; }
        protected string OddPresReason { get; set; }
        public OptionChonThuocThayThe ChonThuocThayThe { get; set; }
        public EnumOptionChonVatTuThayThe ChonVTThayThe { get; set; }
        protected MediMatyTypeADO medicineTypeSDO__Category__SameMediAcin;

        internal EditAbstract(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow
            )
            : base()
        {
            this.Param = param;
            this.currentMedicineTypeADOForEdit = (dataRow as MediMatyTypeADO);
            this.PrimaryKey = this.currentMedicineTypeADOForEdit.PrimaryKey;
            this.TreatmentId = frmAssignPrescription.currentTreatmentWithPatientType.ID;
            this.PatientId = frmAssignPrescription.currentTreatmentWithPatientType.PATIENT_ID;
            this.TreatmentWithPatientTypeInfoSDO = frmAssignPrescription.currentTreatmentWithPatientType;
            this.frmAssignPrescription = frmAssignPrescription;
            this.PatientTypeAlter = frmAssignPrescription.currentHisPatientTypeAlter;
            this.MediMatyTypeADOs = frmAssignPrescription.mediMatyTypeADOs;
            this.MediStockD1SDOs = frmAssignPrescription.mediStockD1ADOs;
            if (frmAssignPrescription.cboMedicineUseForm.EditValue != null)
                this.MedicineUseFormId = Inventec.Common.TypeConvert.Parse.ToInt64((frmAssignPrescription.cboMedicineUseForm.EditValue ?? "0").ToString());
            this.HtuIds = frmAssignPrescription.DataHtuList.Exists(o => o.IsChecked) ? frmAssignPrescription.DataHtuList.Where(o => o.IsChecked).Select(o => o.ID).ToList() : null;
            this.HtuIdNotCheckAcinInteractive = frmAssignPrescription.DataHtuList.Exists(o => o.IsChecked && o.CHECK_ACIN_INTERACTIVE != 1) ? (long?)frmAssignPrescription.DataHtuList.Where(o => o.IsChecked && o.CHECK_ACIN_INTERACTIVE != 1).OrderBy(o => o.NUM_ORDER).ToList()[0].ID : null;
            this.Tutorial = frmAssignPrescription.txtTutorial.Text.Trim();
            this.HtuText = frmAssignPrescription.memHtu.Text.Trim();
            this.UseDays = frmAssignPrescription.spinSoLuongNgay.Value;
            if (!String.IsNullOrEmpty(frmAssignPrescription.spinSang.Text))
                this.Sang = frmAssignPrescription.spinSang.Text;
            if (!String.IsNullOrEmpty(frmAssignPrescription.spinTrua.Text))
                this.Trua = frmAssignPrescription.spinTrua.Text;
            if (!String.IsNullOrEmpty(frmAssignPrescription.spinChieu.Text))
                this.Chieu = frmAssignPrescription.spinChieu.Text;
            if (!String.IsNullOrEmpty(frmAssignPrescription.spinToi.Text))
                this.Toi = frmAssignPrescription.spinToi.Text;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtTocDoTho.Text))
                this.BreathSpeed = frmAssignPrescription.txtTocDoTho.Text;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtThoiGianTho.Text))
                this.BreathTime = frmAssignPrescription.txtThoiGianTho.Text;
            this.Amount = frmAssignPrescription.GetAmount();
            this.RawAmount = frmAssignPrescription.GetRawAmount();
            this.NumOrder = this.currentMedicineTypeADOForEdit.NUM_ORDER;
            this.IsStent = this.currentMedicineTypeADOForEdit.IsStent;
            this.IsNotOutStock = frmAssignPrescription.GetSelectedOpionGroup() == 1;
            this.ValidAddRow = validAddRow;
            this.choosePatientTypeDefaultlService = choosePatientTypeDefaultlService;
            this.choosePatientTypeDefaultlServiceOther = choosePatientTypeDefaultlServiceOther;
            this.CalulateUseTimeTo = calulateUseTimeTo;
            this.ExistsAssianInDay = existsAssianInDay;

            this.DataRow = dataRow;
            this.ExceedLimitInPresReason = frmAssignPrescription.reasonMaxPrescription;
            this.ExceedLimitInDayReason = frmAssignPrescription.reasonMaxPrescriptionDay;
            if (!(GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                this.ExceedLimitInBatchReason = frmAssignPrescription.reasonMaxPrescriptionBatch;
            this.OddPresReason = frmAssignPrescription.reasonOddPrescription;

            if (HisConfigCFG.ManyDayPrescriptionOption == 2 && ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || frmAssignPrescription.VHistreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU))
            {
                this.IsMultiDateState = frmAssignPrescription.isMultiDateState;
                if (frmAssignPrescription.UcDateGetValueForMedi() != null
                    && frmAssignPrescription.UcDateGetValueForMedi().Count > 0)
                {
                    this.IntructionTimeSelecteds = frmAssignPrescription.UcDateGetValueForMedi();
                }
                else
                {
                    this.IntructionTimeSelecteds = frmAssignPrescription.intructionTimeSelecteds;
                }
            }
        }

        protected void CreateADO()
        {
            try
            {
                CreateADO(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void CreateADO(bool isUpdate)
        {
            if (medicineTypeSDO == null)
                medicineTypeSDO = new MediMatyTypeADO();
            if (isUpdate)
            {
                if (this.RawAmount.HasValue && this.RawAmount > 0)
                    medicineTypeSDO.RAW_AMOUNT = this.RawAmount;
                medicineTypeSDO.AMOUNT = this.Amount;
                medicineTypeSDO.PRES_AMOUNT = this.frmAssignPrescription.PresAmount;
                medicineTypeSDO.BK_AMOUNT = this.Amount;
                if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                || this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                || this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                {
                    if (medicineTypeSDO.LAST_EXP_PRICE.HasValue || medicineTypeSDO.LAST_EXP_VAT_RATIO.HasValue)
                    {
                        decimal? priceRaw = this.Amount * (medicineTypeSDO.LAST_EXP_PRICE ?? 0) * (1 + (medicineTypeSDO.LAST_EXP_VAT_RATIO ?? 0));
                        priceRaw = (medicineTypeSDO.CONVERT_RATIO.HasValue && medicineTypeSDO.CONVERT_RATIO > 0) ? priceRaw / medicineTypeSDO.CONVERT_RATIO.Value : priceRaw;

                        medicineTypeSDO.TotalPrice = priceRaw ?? 0;
                    }
                    else
                    {
                        medicineTypeSDO.TotalPrice = (this.Amount * (medicineTypeSDO.PRICE ?? 0)) * (1 + (medicineTypeSDO.IMP_VAT_RATIO ?? 0));
                    }
                }
                medicineTypeSDO.SERIAL_NUMBER = this.SeriNumber;
                medicineTypeSDO.MAX_REUSE_COUNT = this.MaxReuseCount;//TODO
                medicineTypeSDO.USE_COUNT = this.UseCount;//TODO
                medicineTypeSDO.USE_REMAIN_COUNT = this.UseRemainCount;//TODO
                medicineTypeSDO.NUM_ORDER = this.NumOrder;
                medicineTypeSDO.IsOutKtcFee = this.IsOutKtcFee;
                medicineTypeSDO.Sang = this.Sang;
                medicineTypeSDO.Trua = this.Trua;
                medicineTypeSDO.Chieu = this.Chieu;
                medicineTypeSDO.Toi = this.Toi;
                medicineTypeSDO.BREATH_SPEED = this.BreathSpeed;
                medicineTypeSDO.BREATH_TIME = this.BreathTime;
                medicineTypeSDO.TUTORIAL = this.Tutorial;
                medicineTypeSDO.HTU_TEXT = this.HtuText;
                medicineTypeSDO.IsExpend = this.IsExpend;
                medicineTypeSDO.MEDICINE_USE_FORM_ID = this.MedicineUseFormId;
                medicineTypeSDO.HTU_IDs = this.HtuIds;
                medicineTypeSDO.HTU_ID_NOT_CHECK_ACIN_INTERACTIVE = this.HtuIdNotCheckAcinInteractive;
                medicineTypeSDO.Speed = this.Speed;
                medicineTypeSDO.IsKidneyShift = this.IsKidneyShift;
                medicineTypeSDO.KidneyShiftCount = this.KidneyShiftCount;
                medicineTypeSDO.IsUseOrginalUnitForPres = this.IsUseOrginalUnitForPres;
                medicineTypeSDO.IntructionTimeSelecteds = this.IntructionTimeSelecteds;
                medicineTypeSDO.IsMultiDateState = this.IsMultiDateState;
                medicineTypeSDO.IS_SPLIT_COMPENSATION = this.IS_SPLIT_COMPENSATION;
                medicineTypeSDO.PREVIOUS_USING_COUNT = this.PREVIOUS_USING_COUNT;
                medicineTypeSDO.SERVICE_CONDITION_ID = this.SERVICE_CONDITION_ID;
                medicineTypeSDO.SERVICE_CONDITION_NAME = this.SERVICE_CONDITION_NAME;
                medicineTypeSDO.IS_SUB_PRES = this.IS_SUB_PRES;
                medicineTypeSDO.IsNotOutStock = this.IsNotOutStock;
                medicineTypeSDO.IsWarned = this.frmAssignPrescription.IsWarned;
                medicineTypeSDO.IcdsWarning = this.frmAssignPrescription.icdsWarning;
                medicineTypeSDO.EXCEED_LIMIT_IN_PRES_REASON = this.ExceedLimitInPresReason;
                medicineTypeSDO.EXCEED_LIMIT_IN_DAY_REASON = this.ExceedLimitInDayReason;
                medicineTypeSDO.EXCEED_LIMIT_IN_BATCH_REASON = this.ExceedLimitInBatchReason;
                medicineTypeSDO.ODD_PRES_REASON = this.OddPresReason;
                medicineTypeSDO.ALERT_MAX_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ALERT_MAX_IN_TREATMENT : null;
                medicineTypeSDO.IS_BLOCK_MAX_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_BLOCK_MAX_IN_TREATMENT : null; 
                medicineTypeSDO.NUMBER_EXCEED_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.NUMBER_EXCEED_IN_TREATMENT : null; 
                medicineTypeSDO.NUMBER_PRESCIPTION_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.NUMBER_PRESCIPTION_IN_TREATMENT : null;
                var amountPres = (medicineTypeSDO.NUMBER_PRESCIPTION_IN_TREATMENT ?? 0) + medicineTypeSDO.AMOUNT + frmAssignPrescription.mediMatyTypeADOs.Where(o => o.ID == medicineTypeSDO.ID && o.PrimaryKey != medicineTypeSDO.PrimaryKey).Sum(o => o.UseDays != 0 ? o.AMOUNT / o.UseDays : o.AMOUNT);
                if (amountPres > medicineTypeSDO.ALERT_MAX_IN_TREATMENT)
                    medicineTypeSDO.IsAlertInTreatPresciption = true;
            }
        }

        protected void SaveDataAndRefesh()
        {

            frmAssignPrescription.gridViewServiceProcess.BeginUpdate();
            frmAssignPrescription.gridViewServiceProcess.GridControl.DataSource = frmAssignPrescription.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
            frmAssignPrescription.gridViewServiceProcess.EndUpdate();

            frmAssignPrescription.ReSetDataInputAfterAdd__MedicinePage();
            frmAssignPrescription.ReSetChongCHiDinhInfoControl__MedicinePage();
            frmAssignPrescription.SetEnableButtonControl(frmAssignPrescription.actionType);
            frmAssignPrescription.ResetFocusMediMaty(true);
            frmAssignPrescription.SetTotalPrice__TrongDon();
            //frmAssignPrescription.VerifyWarningOverCeiling();
            frmAssignPrescription.gridControlTutorial.DataSource = null;
        }

        protected void UpdatePatientTypeInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                //TienNV
                if (medicineTypeSDO == null)
                    throw new Exception("Sua thuoc khong tim thay medicineTypeSDO");
                if (medicineTypeSDO.PATIENT_TYPE_ID.HasValue && medicineTypeSDO.PATIENT_TYPE_ID > 0)
                    return;
                //

                //Lay doi tuong mac dinh
                var patientTypeSelected = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                patientTypeSelected = this.choosePatientTypeDefaultlServiceOther(this.PatientTypeAlter.PATIENT_TYPE_ID, medicineTypeSDO.SERVICE_ID, medicineTypeSDO.SERVICE_TYPE_ID);

                if (patientTypeSelected != null && patientTypeSelected.ID > 0)
                {
                    medicineTypeSDO.PATIENT_TYPE_ID = patientTypeSelected.ID;
                    medicineTypeSDO.PATIENT_TYPE_CODE = patientTypeSelected.PATIENT_TYPE_CODE;
                    medicineTypeSDO.PATIENT_TYPE_NAME = patientTypeSelected.PATIENT_TYPE_NAME;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void UpdateExpMestReasonInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (medicineTypeSDO != null && frmAssignPrescription.actionType == GlobalVariables.ActionAdd)
                {
                    medicineTypeSDO.EXP_MEST_REASON_ID = null;
                    medicineTypeSDO.EXP_MEST_REASON_CODE = "";
                    medicineTypeSDO.EXP_MEST_REASON_NAME = "";

                    var dataExmeReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXME_REASON_CFG>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue
                            && o.PATIENT_CLASSIFY_ID == frmAssignPrescription.VHistreatment.TDL_PATIENT_CLASSIFY_ID && o.TREATMENT_TYPE_ID == frmAssignPrescription.VHistreatment.TDL_TREATMENT_TYPE_ID && (o.PATIENT_TYPE_ID == null || o.PATIENT_TYPE_ID == medicineTypeSDO.PATIENT_TYPE_ID)).ToList();
                    if (medicineTypeSDO.OTHER_PAY_SOURCE_ID != null)
                    {
                        dataExmeReasons = dataExmeReasons.Where(o => o.OTHER_PAY_SOURCE_ID == medicineTypeSDO.OTHER_PAY_SOURCE_ID).ToList();
                    }
                    else
                    {
                        dataExmeReasons = dataExmeReasons.Where(o => o.OTHER_PAY_SOURCE_ID == null).ToList();
                    }
                    if (dataExmeReasons != null && dataExmeReasons.Count > 0)
                    {
                        var data = (frmAssignPrescription.lstExpMestReasons != null && frmAssignPrescription.lstExpMestReasons.Count > 0) ? frmAssignPrescription.lstExpMestReasons.Where(o => o.ID == dataExmeReasons[0].EXP_MEST_REASON_ID).ToList() : null;

                        if (data != null && data.Count > 0)
                        {
                            medicineTypeSDO.EXP_MEST_REASON_ID = data[0].ID;
                            medicineTypeSDO.EXP_MEST_REASON_CODE = data[0].EXP_MEST_REASON_CODE;
                            medicineTypeSDO.EXP_MEST_REASON_NAME = data[0].EXP_MEST_REASON_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void UpdateMedicineUseFormInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                //Duong dung, HDSD:
                if (this.MedicineUseFormId > 0 && medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM data_dd = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == this.MedicineUseFormId);
                    if (data_dd != null)
                    {
                        medicineTypeSDO.MEDICINE_USE_FORM_ID = data_dd.ID;
                        medicineTypeSDO.MEDICINE_USE_FORM_CODE = data_dd.MEDICINE_USE_FORM_CODE;
                        medicineTypeSDO.MEDICINE_USE_FORM_NAME = data_dd.MEDICINE_USE_FORM_NAME;
                    }
                }
                else
                {
                    medicineTypeSDO.MEDICINE_USE_FORM_ID = null;
                    medicineTypeSDO.MEDICINE_USE_FORM_CODE = "";
                    medicineTypeSDO.MEDICINE_USE_FORM_NAME = "";
                    medicineTypeSDO.ErrorMessageMedicineUseForm = "";
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void UpdateUseTimeInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (this.CalulateUseTimeTo != null)
                {
                    long? useTimeTo = this.CalulateUseTimeTo();
                    if ((useTimeTo ?? 0) > 0)
                    {
                        medicineTypeSDO.UseTimeTo = useTimeTo;
                        medicineTypeSDO.UseDays = this.UseDays;
                    }
                    else
                    {
                        medicineTypeSDO.UseTimeTo = null;
                        medicineTypeSDO.UseDays = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected object GetDataMediMatyInStock()
        {
            object result = null;
            try
            {
                result = this.MediStockD1SDOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected decimal AmountOutOfStock(long serviceId, long meidStockId)
        {
            decimal result = 0;
            try
            {
                var checkMatyInStock = GetDataAmountOutOfStock(serviceId, meidStockId);
                var medi1 = checkMatyInStock as DMediStock1ADO;
                if (medi1 != null)
                {
                    result = (medi1.AMOUNT ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected object GetDataAmountOutOfStock(long serviceId, long meidStockId)
        {
            object result = null;
            try
            {
                var result1 = this.MediStockD1SDOs.FirstOrDefault(o => o.SERVICE_ID == serviceId && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId));
                if (result1 != null && this.Amount > (result1.AMOUNT ?? 0))
                {
                    //model.AMOUNT = result1.AMOUNT;
                    //model.AmountAlert = result1.AMOUNT;
                }
                result = result1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected string GetDataByActiveIngrBhyt()
        {
            string result = "";
            try
            {
                //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                //{
                //    var rs = this.MediStockD2SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && !String.IsNullOrEmpty(o.ACTIVE_INGR_BHYT_NAME) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount && (o.SERVICE_ID ?? 0) != this.ServiceId);
                //    if (rs != null)
                //    {
                //        result = rs.ACTIVE_INGR_BHYT_NAME;
                //    }
                //}
                //else
                //{
                var rs = this.MediStockD1SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && !String.IsNullOrEmpty(o.ACTIVE_INGR_BHYT_NAME) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount && (o.SERVICE_ID ?? 0) != this.ServiceId);
                if (rs != null)
                {
                    result = rs.ACTIVE_INGR_BHYT_NAME;
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected object GetDataByActiveIngrBhytConstain()
        {
            object result = null;
            try
            {
                result = this.MediStockD1SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// 11/09/2018
        /// Bat buoc nhap huong dan su dung voi tat ca cac doi tuong
        /// </summary>
        /// <returns></returns>
        protected bool ValidTutorial()
        {
            bool valid = true;
            try
            {    
                if (String.IsNullOrEmpty(this.HtuText))
                {
                    DialogResult result = XtraMessageBox.Show("Bắt buộc phải nhập cách dùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    valid = false;
                    frmAssignPrescription.memHtu.Focus();
                }

                if (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    && String.IsNullOrEmpty(this.Tutorial) && !HisConfigCFG.IsNotAutoGenerateTutorial) //frmAssignPrescription.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                {
                    MessageBox.Show(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    valid = false;
                    frmAssignPrescription.txtTutorial.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        protected bool CheckValidPre()
        {
            bool valid = true;
            try
            {
                if (this.ValidAddRow != null)
                {
                    valid = this.ValidAddRow(this.DataRow);
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        protected bool ValidThuocDaKeTrongNgay()
        {
            bool valid = true;
            try
            {
                var medicinetypeStockExists = this.MediMatyTypeADOs
                     .FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
                if (medicinetypeStockExists != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThuocDaduocKe, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == DialogResult.No)
                    {
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcinByMedicineType(List<long> medicineTypeIds)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeIds.Contains(o.MEDICINE_TYPE_ID)).ToList();

                var medis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected void SetValidError()
        {
            try
            {
                SetValidAssianInDayError();
                SetValidMedicineUseFormError();
                SetValidPatientTypeError();
                SetValidAmountError();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetValidAssianInDayError()
        {
            try
            {
                if (this.ExistsAssianInDay != null && this.ExistsAssianInDay(medicineTypeSDO))
                {
                    //if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    //                || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                    //                || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    //{
                    medicineTypeSDO.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                    medicineTypeSDO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    //}
                }
                else
                {
                    medicineTypeSDO.ErrorMessageIsAssignDay = "";
                    medicineTypeSDO.ErrorTypeIsAssignDay = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetValidMedicineUseFormError()
        {
            try
            {
                if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    && medicineTypeSDO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && (medicineTypeSDO.MEDICINE_USE_FORM_ID ?? 0) <= 0)
                {
                    medicineTypeSDO.ErrorMessageMedicineUseForm = ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.Warning;
                }
                else
                {
                    medicineTypeSDO.ErrorMessageMedicineUseForm = "";
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetValidPatientTypeError()
        {
            try
            {
                if (medicineTypeSDO.PATIENT_TYPE_ID <= 0)
                {
                    medicineTypeSDO.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    medicineTypeSDO.ErrorTypePatientTypeId = ErrorType.Warning;
                }
                else
                {
                    medicineTypeSDO.ErrorMessagePatientTypeId = "";
                    medicineTypeSDO.ErrorTypePatientTypeId = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void SetValidAmountError()
        {
            try
            {
                if (medicineTypeSDO.AMOUNT <= 0)
                {
                    medicineTypeSDO.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    medicineTypeSDO.ErrorTypeAmount = ErrorType.Warning;
                }
                else
                {
                    medicineTypeSDO.ErrorMessageAmount = "";
                    medicineTypeSDO.ErrorTypeAmount = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected bool ValidKhaDungThuocTrongNhaThuoc()
        {
            bool valid = true;
            CommonParam paramWarn = new CommonParam();
            try
            {
                try
                {
                    if (HisConfigCFG.IsAutoCreateSaleExpMest
                        && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                        && this.frmAssignPrescription.cboNhaThuoc.EditValue != null
                        && this.IS_OUT_HOSPITAL != 1)
                    {
                        //Lay thuoc trong kho va kiem tra thuoc co con trong kho khong
                        decimal damount = AmountOutOfStock(this.ServiceId, (this.frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_ID ?? 0));
                        if (damount <= 0)
                        {
                            paramWarn.Messages.Add(ResourceMessage.ThuocKhongCoTrongKho);
                            throw new ArgumentNullException("medicinetypeStockSDO is null");
                        }
                        decimal amountAdded = 0;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.Amount), this.Amount)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amountAdded), amountAdded)
                            + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => damount), damount)
                             + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT), frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT)
                            );
                        Rectangle buttonBounds = new Rectangle(frmAssignPrescription.txtMediMatyForPrescription.Bounds.X, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Y, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Width, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Height);
                        if (this.Amount > frmAssignPrescription.currentMedicineTypeADOForEdit.BK_AMOUNT && (this.Amount - frmAssignPrescription.currentMedicineTypeADOForEdit.BK_AMOUNT + amountAdded) > (frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT ?? 0))
                        {
                            MessageBox.Show("Thuốc vật tư trong kho không đủ khả dụng");
                            return false;

                            if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                            {
                                var medicineTypeAcin__SameAcinBhyt = GetDataByActiveIngrBhyt();
                                frmMessageBoxChooseAcinBhyt form = new frmMessageBoxChooseAcinBhyt(ChonThuocTrongKhoCungHoatChat);
                                form.ShowDialog();

                                switch (this.ChonThuocThayThe)
                                {
                                    case OptionChonThuocThayThe.None:
                                        frmAssignPrescription.spinAmount.SelectAll();
                                        frmAssignPrescription.spinAmount.Focus();
                                        valid = false;
                                        break;
                                    case OptionChonThuocThayThe.ThuocCungHoatChat:
                                        //thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
                                        frmAssignPrescription.txtMediMatyForPrescription.Text = medicineTypeAcin__SameAcinBhyt;
                                        frmAssignPrescription.gridViewMediMaty.ActiveFilterString = " [ACTIVE_INGR_BHYT_NAME] Like '%" + frmAssignPrescription.txtMediMatyForPrescription.Text + "%'";
                                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                                        frmAssignPrescription.gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                                        frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                                        frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                                        frmAssignPrescription.gridViewMediMaty.FocusedRowHandle = 0;
                                        frmAssignPrescription.gridViewMediMaty.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                                        frmAssignPrescription.gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                                        frmAssignPrescription.popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                                        frmAssignPrescription.txtMediMatyForPrescription.Focus();
                                        frmAssignPrescription.txtMediMatyForPrescription.SelectAll();
                                        valid = false;
                                        break;
                                    case OptionChonThuocThayThe.ThuocNgoaiKho:
                                        //Trong trường hợp, số lượng vượt quá tồn, mà trong kho cũng không có thuốc nào cùng hoạt chất đang còn tồn thì hiển thị thông báo kiêu 
                                        //"Thuốc đã chọn và các thuốc cùng hoạt chất khác trong kho không đủ để kê. Bạn có muốn kê thuốc ngoài kho không". 
                                        //Nếu chọn ok thì lấy thuốc ngoài kho, nếu ko thì ko xử lý j cả
                                        if (frmAssignPrescription.currentMedicineTypes == null || frmAssignPrescription.currentMedicineTypes.Count == 0)
                                        {
                                            frmAssignPrescription.currentMedicineTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                                            long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                                            if (isOnlyDisplayMediMateIsBusiness == 1 && frmAssignPrescription.currentMedicineTypes != null && frmAssignPrescription.currentMedicineTypes.Count > 0)
                                                frmAssignPrescription.currentMedicineTypes = frmAssignPrescription.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();
                                        }

                                        V_HIS_MEDICINE_TYPE medicineType = null;
                                        medicineType = frmAssignPrescription.currentMedicineTypes.Where(o => o.MEDICINE_TYPE_NAME == this.Name)
                                            .OrderBy(o => Math.Abs(o.SERVICE_ID - this.ServiceId)).FirstOrDefault();
                                        //Nếu không tìm được thuốc ngoài kho nào thì tự động chuyển sang thuốc khác (tự mua)
                                        if (medicineType == null)
                                        {
                                            AddMedicineTypeCategoryByOtherMedi();
                                        }
                                        //Nếu tìm thấy thuốc ngoài kho thì lấy luôn thuốc ngoài kho đó
                                        else
                                        {
                                            AddMedicineTypeCategoryBySameMediAcin(medicineType);
                                        }
                                        break;
                                }
                            }
                            else if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                            {
                                frmMessageBoxChooseVT form = new frmMessageBoxChooseVT(ChonVatTu);
                                form.ShowDialog();

                                switch (this.ChonVTThayThe)
                                {
                                    case EnumOptionChonVatTuThayThe.None:
                                        frmAssignPrescription.spinAmount.SelectAll();
                                        frmAssignPrescription.spinAmount.Focus();
                                        valid = false;
                                        break;
                                    case EnumOptionChonVatTuThayThe.VatTuNgoaiKho:

                                        List<V_HIS_MATERIAL_TYPE> materialTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();

                                        var materialType = materialTypes.FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
                                        if (materialType == null)
                                            throw new ArgumentNullException("Khong tim thay medicineType SERVICE_ID = " + this.ServiceId + " tu danh muc thuoc.");

                                        AddMaterialTypeCategoryBySameMediAcin(materialType);
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                catch (Exception ex)
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                if (!String.IsNullOrEmpty(paramWarn.GetMessage()))
                    MessageManager.Show(paramWarn.GetMessage());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        protected void ChonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe chonThuocThayThe)
        {
            try
            {
                this.ChonThuocThayThe = chonThuocThayThe;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void AddMedicineTypeCategoryByOtherMedi()
        {
            try
            {
                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.BK_AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                medicineTypeSDO__Category__SameMediAcin.SERVICE_UNIT_NAME = this.ServiceUnitName;
                medicineTypeSDO__Category__SameMediAcin.MEDICINE_TYPE_NAME = this.Name;
                medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                medicineTypeSDO__Category__SameMediAcin.HTU_TEXT = this.HtuText;
                //UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                //medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                //medicineTypeSDO__Category__SameMediAcin.IsStent = this.IsStent;
                //medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
                //medicineTypeSDO__Category__SameMediAcin.UseDays = this.UseDays;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_ID = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_NAME = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_ID = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_NAME = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void AddMedicineTypeCategoryBySameMediAcin(V_HIS_MEDICINE_TYPE addMedicineTypeADO)
        {
            try
            {
                if (addMedicineTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(medicineTypeSDO__Category__SameMediAcin, addMedicineTypeADO);
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.BK_AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                medicineTypeSDO__Category__SameMediAcin.HTU_TEXT = this.HtuText;
                UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                medicineTypeSDO__Category__SameMediAcin.IsStent = this.IsStent;
                medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
                medicineTypeSDO__Category__SameMediAcin.UseDays = this.UseDays;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_ID = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_NAME = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_ID = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_NAME = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void AddMaterialTypeCategoryBySameMediAcin(V_HIS_MATERIAL_TYPE addMaterialTypeADO)
        {
            try
            {
                if (addMaterialTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                medicineTypeSDO__Category__SameMediAcin.MEDICINE_TYPE_NAME = addMaterialTypeADO.MATERIAL_TYPE_NAME;
                medicineTypeSDO__Category__SameMediAcin.MEDICINE_TYPE_CODE = addMaterialTypeADO.MATERIAL_TYPE_CODE;
                medicineTypeSDO__Category__SameMediAcin.ID = addMaterialTypeADO.ID;
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.BK_AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
                medicineTypeSDO__Category__SameMediAcin.TUTORIAL = this.Tutorial;
                medicineTypeSDO__Category__SameMediAcin.HTU_TEXT = this.HtuText;
                UpdateUseTimeInDataRow(medicineTypeSDO__Category__SameMediAcin);
                medicineTypeSDO__Category__SameMediAcin.IsOutKtcFee = this.IsOutKtcFee;
                medicineTypeSDO__Category__SameMediAcin.IsStent = this.IsStent;
                medicineTypeSDO__Category__SameMediAcin.IsExpend = this.IsExpend;
                medicineTypeSDO__Category__SameMediAcin.UseDays = this.UseDays;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_ID = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.PATIENT_TYPE_NAME = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_ID = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_CODE = null;
                medicineTypeSDO__Category__SameMediAcin.MEDI_STOCK_NAME = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ChonVatTu(EnumOptionChonVatTuThayThe chonVTThayThe)
        {
            try
            {
                this.ChonVTThayThe = chonVTThayThe;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected bool ValidThuocWithContraindicaterWarningOption()
        {
            bool valid = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOAdds = new List<MediMatyTypeADO>();
                mediMatyTypeADOAdds.Add(new MediMatyTypeADO() { SERVICE_ID = this.ServiceId, DataType = this.DataType, ID = this.Id, MEDICINE_TYPE_NAME = this.Name, AMOUNT = this.Amount, MEDICINE_TYPE_CODE = this.Code });
                valid = valid && frmAssignPrescription.CheckICDServiceForContraindicaterWarningOption(mediMatyTypeADOAdds);
            }
            catch (Exception ex)
            {
                //valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }
    }
}
