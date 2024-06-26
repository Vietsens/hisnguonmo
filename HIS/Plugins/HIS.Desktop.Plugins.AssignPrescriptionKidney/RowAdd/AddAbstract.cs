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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Add
{
    public abstract class AddAbstract : EntityBase
    {
        protected long Id { get; set; }
        protected decimal Amount { get; set; }
        protected decimal AmountAvaiable { get; set; }
        protected int DataType { get; set; }
        protected string Code { get; set; }
        protected string Name { get; set; }
        protected string ManuFacturerName { get; set; }
        protected string ServiceUnitName { get; set; }
        protected string NationalName { get; set; }
        protected long ServiceId { get; set; }
        protected string Concentra { get; set; }
        protected long? MediStockId { get; set; }
        protected string MediStockCode { get; set; }
        protected string MediStockName { get; set; }
        protected decimal? Price { get; set; }
        protected long? HeinServiceTypeId { get; set; }
        protected long ServiceTypeId { get; set; }
        protected long? NumOrder { get; set; }
        protected string ActiveIngrBhytCode { get; set; }
        protected string ActiveIngrBhytName { get; set; }
        protected double? Sang { get; set; }
        protected double? Trua { get; set; }
        protected double? Chieu { get; set; }
        protected double? Toi { get; set; }
        protected bool? IsStent { get; set; }
        protected bool? IsAllowOdd { get; set; }
        protected decimal? Speed { get; set; }
        protected bool? IsKidneyShift { get; set; }
        protected decimal? KidneyShiftCount { get; set; }

        protected ValidAddRow ValidAddRow { get; set; }
        protected GetPatientTypeBySeTy GetPatientTypeBySeTy { get; set; }
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

        protected CommonParam Param { get; set; }
        protected MediMatyTypeADO medicineTypeSDO { get; set; }

        protected long HtuId { get; set; }
        protected long MedicineUseFormId { get; set; }
        protected string Tutorial { get; set; }
        protected bool IsExpend { get; set; }
        protected decimal? UseDays { get; set; }
        protected object DataRow { get; set; }
        protected string SeriNumber { get; set; }
        protected long? UseCount { get; set; }
        protected long? UseRemainCount { get; set; }
        protected long? MaxReuseCount { get; set; }
        protected bool IsMultiDateState { get; set; }
        protected List<long> IntructionTimeSelecteds { get; set; }

        public OptionChonThuocThayThe ChonThuocThayThe { get; set; }
        public EnumOptionChonVatTuThayThe ChonVTThayThe { get; set; }

        protected AddAbstract(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            GetPatientTypeBySeTy getPatientTypeBySeTy,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow
            )
            : base()
        {
            if (param == null)
                this.Param = new CommonParam();
            else
                this.Param = param;
            this.TreatmentId = frmAssignPrescription.currentTreatmentWithPatientType.ID;
            this.PatientId = frmAssignPrescription.currentTreatmentWithPatientType.PATIENT_ID;
            this.TreatmentWithPatientTypeInfoSDO = frmAssignPrescription.currentTreatmentWithPatientType;
            this.frmAssignPrescription = frmAssignPrescription;
            this.PatientTypeAlter = frmAssignPrescription.currentHisPatientTypeAlter;
            this.MediMatyTypeADOs = frmAssignPrescription.mediMatyTypeADOs;
            this.MediStockD1SDOs = frmAssignPrescription.mediStockD1ADOs;
            this.MedicineUseFormId = Inventec.Common.TypeConvert.Parse.ToInt64((frmAssignPrescription.cboMedicineUseForm.EditValue ?? "0").ToString());
            this.HtuId = Inventec.Common.TypeConvert.Parse.ToInt64((frmAssignPrescription.cboHtu.EditValue ?? "0").ToString());
            this.Tutorial = frmAssignPrescription.txtTutorial.Text.Trim();
            this.UseDays = frmAssignPrescription.spinSoLuongNgay.Value;

            this.Sang = frmAssignPrescription.GetValueSpin(frmAssignPrescription.spinSang.Text);
            this.Trua = frmAssignPrescription.GetValueSpin(frmAssignPrescription.spinTrua.Text);
            this.Chieu = frmAssignPrescription.GetValueSpin(frmAssignPrescription.spinChieu.Text);
            this.Toi = frmAssignPrescription.GetValueSpin(frmAssignPrescription.spinToi.Text);
            this.Amount = frmAssignPrescription.GetAmount();
            this.NumOrder = frmAssignPrescription.idRow;

            this.ValidAddRow = validAddRow;
            this.GetPatientTypeBySeTy = getPatientTypeBySeTy;
            this.CalulateUseTimeTo = calulateUseTimeTo;
            this.ExistsAssianInDay = existsAssianInDay;
            if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn))
            {
                this.IsMultiDateState = frmAssignPrescription.isMultiDateState;
                this.IntructionTimeSelecteds = frmAssignPrescription.intructionTimeSelecteds;
            }
            this.DataRow = dataRow;
        }

        protected void CreateADO()
        {
            medicineTypeSDO = new MediMatyTypeADO();
            medicineTypeSDO.ID = this.Id;
            medicineTypeSDO.SERIAL_NUMBER = this.SeriNumber;
            medicineTypeSDO.MAX_REUSE_COUNT = this.MaxReuseCount;//TODO
            medicineTypeSDO.USE_COUNT = this.UseCount;//TODO
            medicineTypeSDO.USE_REMAIN_COUNT = this.UseRemainCount;//TODO
            medicineTypeSDO.AMOUNT = this.Amount;
            medicineTypeSDO.BK_AMOUNT = this.Amount;
            medicineTypeSDO.DataType = this.DataType;
            medicineTypeSDO.MEDICINE_TYPE_CODE = this.Code;
            medicineTypeSDO.MEDICINE_TYPE_NAME = this.Name;
            medicineTypeSDO.MANUFACTURER_NAME = this.ManuFacturerName;
            medicineTypeSDO.SERVICE_UNIT_NAME = this.ServiceUnitName;
            medicineTypeSDO.NATIONAL_NAME = this.NationalName;
            medicineTypeSDO.SERVICE_ID = this.ServiceId;
            medicineTypeSDO.CONCENTRA = this.Concentra;
            medicineTypeSDO.MEDI_STOCK_ID = this.MediStockId;
            medicineTypeSDO.MEDI_STOCK_CODE = this.MediStockCode;
            medicineTypeSDO.MEDI_STOCK_NAME = this.MediStockName;
            medicineTypeSDO.HEIN_SERVICE_TYPE_ID = this.HeinServiceTypeId;
            medicineTypeSDO.SERVICE_TYPE_ID = this.ServiceTypeId;
            medicineTypeSDO.NUM_ORDER = this.NumOrder;
            medicineTypeSDO.ACTIVE_INGR_BHYT_CODE = this.ActiveIngrBhytCode;
            medicineTypeSDO.ACTIVE_INGR_BHYT_NAME = this.ActiveIngrBhytName;
            medicineTypeSDO.IsOutKtcFee = this.IsOutKtcFee;
            medicineTypeSDO.IS_STAR_MARK = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_STAR_MARK : null;
            medicineTypeSDO.IsStent = this.IsStent;
            medicineTypeSDO.IsAllowOdd = this.IsAllowOdd;
            medicineTypeSDO.ALERT_MAX_IN_PRESCRIPTION = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ALERT_MAX_IN_PRESCRIPTION : null;
            medicineTypeSDO.TDL_GENDER_ID = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.TDL_GENDER_ID : null;
            if (this.Sang > 0)
                medicineTypeSDO.Sang = this.Sang;
            if (this.Trua > 0)
                medicineTypeSDO.Trua = this.Trua;
            if (this.Chieu > 0)
                medicineTypeSDO.Chieu = this.Chieu;
            if (this.Toi > 0)
                medicineTypeSDO.Toi = this.Toi;
            medicineTypeSDO.TUTORIAL = this.Tutorial;
            medicineTypeSDO.IsExpend = this.IsExpend;
            if (this.MedicineUseFormId > 0)
                medicineTypeSDO.MEDICINE_USE_FORM_ID = this.MedicineUseFormId;
            if (this.HtuId > 0)
                medicineTypeSDO.HTU_ID = this.HtuId;
            if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                || this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
            {
                medicineTypeSDO.PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE : null;
                medicineTypeSDO.TotalPrice = (this.Amount * (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE ?? 0)) * (1 + (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_VAT_RATIO ?? 0));
            }
            else if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC && frmAssignPrescription.spinPrice.EditValue != null)
            {
                medicineTypeSDO.PRICE = frmAssignPrescription.spinPrice.Value;
                medicineTypeSDO.TotalPrice = (this.Amount * (medicineTypeSDO.PRICE ?? 0));
            }
            medicineTypeSDO.Speed = this.Speed;
            medicineTypeSDO.IsKidneyShift = this.IsKidneyShift;
            medicineTypeSDO.KidneyShiftCount = this.KidneyShiftCount;
            medicineTypeSDO.CONVERT_RATIO = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO : null;
            medicineTypeSDO.CONVERT_UNIT_CODE = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_UNIT_CODE : "";
            medicineTypeSDO.CONVERT_UNIT_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : "";
            medicineTypeSDO.IntructionTimeSelecteds = this.IntructionTimeSelecteds;
            medicineTypeSDO.IsMultiDateState = this.IsMultiDateState;
        }

        protected void SaveDataAndRefesh(MediMatyTypeADO mediMatyADO)
        {
            frmAssignPrescription.mediMatyTypeADOs.Add(mediMatyADO);
            frmAssignPrescription.idRow += frmAssignPrescription.stepRow;

            frmAssignPrescription.gridViewServiceProcess.BeginUpdate();
            frmAssignPrescription.gridViewServiceProcess.GridControl.DataSource = frmAssignPrescription.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
            frmAssignPrescription.gridViewServiceProcess.EndUpdate();

            frmAssignPrescription.ReSetDataInputAfterAdd__MedicinePage();
            frmAssignPrescription.SetEnableButtonControl(frmAssignPrescription.actionType);
            frmAssignPrescription.ResetFocusMediMaty(true);
            frmAssignPrescription.SetTotalPrice__TrongDon();
            frmAssignPrescription.gridControlTutorial.DataSource = null;
        }

        protected void UpdatePatientTypeInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                //Lay doi tuong mac dinh
                var patientTypeSelected = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();

                long patientTypeId = 0;
                if (frmAssignPrescription.cboPatientType.EditValue != null)
                {
                    patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.cboPatientType.EditValue.ToString());
                }
                else
                {
                    patientTypeId = this.PatientTypeAlter.PATIENT_TYPE_ID;
                }

                patientTypeSelected = this.ChoosePatientTypeDefaultlService(patientTypeId, medicineTypeSDO.SERVICE_ID, medicineTypeSDO.SERVICE_TYPE_ID);

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

        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, long serviceTypeId)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                if (this.GetPatientTypeBySeTy != null)
                {
                    return this.GetPatientTypeBySeTy(patientTypeId, serviceId, serviceTypeId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        protected object GetDataMediMatyInStock()
        {
            object result = null;
            try
            {
                //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                //{
                //    result = this.MediStockD2SDOs;
                //}
                //else
                //{
                result = this.MediStockD1SDOs;
                //}
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
                //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                //{
                //    var medi2 = checkMatyInStock as D_HIS_MEDI_STOCK_2;
                //    if (medi2 != null)
                //    {
                //        result = (medi2.AMOUNT ?? 0);
                //    }
                //}
                //else
                //{
                var medi1 = checkMatyInStock as DMediStock1ADO;
                if (medi1 != null)
                {
                    result = (medi1.AMOUNT ?? 0);
                }
                //}
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
                //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                //{
                //    var result2 = this.MediStockD2SDOs.FirstOrDefault(o => o.SERVICE_ID == serviceId && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId));
                //    if (result2 != null && this.Amount > (result2.AMOUNT ?? 0))
                //    {
                //        //model.AMOUNT = result2.AMOUNT;
                //        //model.AmountAlert = result2.AMOUNT;
                //    }
                //    result = result2;
                //}
                //else
                //{
                var result1 = this.MediStockD1SDOs.FirstOrDefault(o => o.SERVICE_ID == serviceId && (meidStockId == 0 || o.MEDI_STOCK_ID == meidStockId));
                if (result1 != null && this.Amount > (result1.AMOUNT ?? 0))
                {
                    //model.AMOUNT = result1.AMOUNT;
                    //model.AmountAlert = result1.AMOUNT;
                }
                result = result1;
                //}
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
                //    var rs = this.MediStockD2SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && !String.IsNullOrEmpty(o.ACTIVE_INGR_BHYT_NAME) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount && ((o.SERVICE_ID == this.ServiceId && o.MEDI_STOCK_ID != this.MediStockId) || (o.SERVICE_ID != this.ServiceId)));
                //    if (rs != null)
                //    {
                //        result = rs.ACTIVE_INGR_BHYT_NAME;
                //    }
                //}
                //else
                //{
                var rs = this.MediStockD1SDOs.FirstOrDefault(o =>
                    !String.IsNullOrEmpty(this.ActiveIngrBhytName)
                    && !String.IsNullOrEmpty(o.ACTIVE_INGR_BHYT_NAME)
                    && (o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName))
                    && o.AMOUNT >= this.Amount
                    && ((o.SERVICE_ID == this.ServiceId && o.MEDI_STOCK_ID != this.MediStockId) || (o.SERVICE_ID != this.ServiceId)));
                if (rs != null)
                {
                    result = this.ActiveIngrBhytName;
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
                //if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                //{
                //    result = this.MediStockD2SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount);
                //}
                //else
                //{
                result = this.MediStockD1SDOs.FirstOrDefault(o => !String.IsNullOrEmpty(this.ActiveIngrBhytName) && o.ACTIVE_INGR_BHYT_NAME.Contains(this.ActiveIngrBhytName) && o.AMOUNT >= this.Amount);
                //}
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
                if (this.ExistsAssianInDay != null && this.ExistsAssianInDay(this.ServiceId))
                {
                    if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        medicineTypeSDO.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                        medicineTypeSDO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
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

        /// <summary>
        /// Cap nhat 11/09/2018
        /// Bat buoc nhap lieu dung voi tat ca cac doi tuong
        /// </summary>
        /// <returns></returns>
        protected bool ValidTutorialAndUseForm()
        {
            bool valid = true;
            try
            { //frmAssignPrescription.currentHisPatientTypeAlter.PATIENT_TYPE_ID
                var patientTypeSelected = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                patientTypeSelected = this.ChoosePatientTypeDefaultlService(this.PatientTypeAlter.PATIENT_TYPE_ID, this.ServiceId, this.ServiceTypeId);

                if (patientTypeSelected != null
                    //&& patientTypeSelected.ID == HisConfigCFG.PatientTypeId__BHYT
                    && this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    //Get patientType
                    if (String.IsNullOrEmpty(this.Tutorial))
                    {
                        MessageBox.Show(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        valid = false;
                        frmAssignPrescription.txtTutorial.Focus();
                    }

                    if (this.MedicineUseFormId <= 0)
                    {
                        MessageBox.Show(ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        valid = false;
                        frmAssignPrescription.txtMediMatyForPrescription.Focus();
                    }
                }
                else
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong thanh toan ,Service id" + this.ServiceId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        public bool CheckPatientTypeHasValue()
        {
            bool valid = true;
            try
            {
                var patientTypeSelected = this.ChoosePatientTypeDefaultlService(this.PatientTypeAlter.PATIENT_TYPE_ID, this.ServiceId, this.ServiceTypeId);
                if (patientTypeSelected == null || patientTypeSelected.ID == 0)
                {
                    MessageBox.Show(ResourceMessage.KhongTimThayChinhSachGiaCuaDichVu, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
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
                    medicineTypeSDO.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
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
                    medicineTypeSDO.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
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

    }
}
