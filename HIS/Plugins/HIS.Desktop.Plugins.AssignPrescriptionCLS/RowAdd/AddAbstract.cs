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
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Add
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
        protected bool? IsCheckFilm { get; set; }
        protected decimal? FilmNumber { get; set; }

        protected ValidAddRow ValidAddRow { get; set; }
        //protected GetPatientTypeBySeTy GetPatientTypeBySeTy { get; set; }
        internal HIS.Desktop.Plugins.AssignPrescriptionCLS.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService;
        internal HIS.Desktop.Plugins.AssignPrescriptionCLS.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther;
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
        protected string HtuText { get; set; }
        protected bool IsExpend { get; set; }
        protected decimal? UseDays { get; set; }
        protected object DataRow { get; set; }
        protected string SeriNumber { get; set; }
        protected long? UseCount { get; set; }
        protected long? UseRemainCount { get; set; }
        protected long? MaxReuseCount { get; set; }

        protected string PackageNumber { get; set; }
        protected long? ExpiredDate { get; set; }
        protected bool? IsAssignPackage { get; set; }
        protected long? MAME_ID { get; set; }
        protected bool IsMultiDateState { get; set; }
        protected List<long> IntructionTimeSelecteds { get; set; }

        public OptionChonThuocThayThe ChonThuocThayThe { get; set; }
        public EnumOptionChonVatTuThayThe ChonVTThayThe { get; set; }
        protected MediMatyTypeADO medicineTypeSDO__Category__SameMediAcin;

        internal AddAbstract(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            HIS.Desktop.Plugins.AssignPrescriptionCLS.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService,
            HIS.Desktop.Plugins.AssignPrescriptionCLS.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther,
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
            this.Tutorial = frmAssignPrescription.txtTutorial.Text.Trim();
            this.HtuText = frmAssignPrescription.txtHtu.Text.Trim();
            this.UseDays = 1;

            this.IsCheckFilm = frmAssignPrescription.chkPhimHong.Enabled && frmAssignPrescription.chkPhimHong.Checked;
            this.FilmNumber = this.IsCheckFilm.Value ? (decimal?)frmAssignPrescription.spinSoPhimHong.Value : null;

            this.Amount = frmAssignPrescription.GetAmount();
            this.NumOrder = frmAssignPrescription.idRow;

            this.ValidAddRow = validAddRow;
            this.choosePatientTypeDefaultlService = choosePatientTypeDefaultlService;
            this.choosePatientTypeDefaultlServiceOther = choosePatientTypeDefaultlServiceOther;
            this.CalulateUseTimeTo = calulateUseTimeTo;
            this.ExistsAssianInDay = existsAssianInDay;

            this.DataRow = dataRow;

            if (HisConfigCFG.ManyDayPrescriptionOption == 2 && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
            {
                this.IsMultiDateState = frmAssignPrescription.isMultiDateState;
                this.IntructionTimeSelecteds = frmAssignPrescription.intructionTimeSelecteds;
            }
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
            medicineTypeSDO.FilmNumber = this.FilmNumber;
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
            medicineTypeSDO.ATC_CODES = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_CODES : null;
            medicineTypeSDO.ATC_GROUP_CODES = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_GROUP_CODES : null;
            if (this.Sang > 0)
                medicineTypeSDO.Sang = this.Sang;
            if (this.Trua > 0)
                medicineTypeSDO.Trua = this.Trua;
            if (this.Chieu > 0)
                medicineTypeSDO.Chieu = this.Chieu;
            if (this.Toi > 0)
                medicineTypeSDO.Toi = this.Toi;
            medicineTypeSDO.TUTORIAL = this.Tutorial;
            medicineTypeSDO.HTU_TEXT = this.HtuText;
            medicineTypeSDO.IsExpend = this.IsExpend;
            if (this.MedicineUseFormId > 0)
                medicineTypeSDO.MEDICINE_USE_FORM_ID = this.MedicineUseFormId;
            if (this.HtuId > 0)
                medicineTypeSDO.HTU_ID = this.HtuId;
            //if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
            //    || this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
            //{
            if (frmAssignPrescription.currentMedicineTypeADOForEdit != null)
            {
                medicineTypeSDO.PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE;
                medicineTypeSDO.TotalPrice = (this.Amount * (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE ?? 0)) * (1 + (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_VAT_RATIO ?? 0));
            }
            //}
            //else if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC && frmAssignPrescription.spinPrice.EditValue != null)
            //{
            //    medicineTypeSDO.PRICE = frmAssignPrescription.spinPrice.Value;
            //    medicineTypeSDO.TotalPrice = (this.Amount * (medicineTypeSDO.PRICE ?? 0));
            //}
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

                patientTypeSelected = this.choosePatientTypeDefaultlServiceOther(patientTypeId, medicineTypeSDO.SERVICE_ID, medicineTypeSDO.SERVICE_TYPE_ID);

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
                        Inventec.Common.Logging.LogSystem.Debug("ValidThuocDaKeTrongNgay: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
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

        ///// <summary>
        ///// #20013
        ///// Khi bật key cấu hình hệ thống tự động tạo phiếu xuất bán (MOS.HIS_SERVICE_REQ.IS_AUTO_CREATE_SALE_EXP_MEST) 
        ///// thì ko cho bổ sung thuốc/vật tư vượt quá số lượng khả dụng của nhà thuốc (tương tự như kê thuốc/vật tư trong kho)
        ///// </summary>
        ///// <returns></returns>      
        //protected bool ValidKhaDungThuocTrongNhaThuoc()
        //{
        //    bool valid = true;
        //    CommonParam paramWarn = new CommonParam();
        //    try
        //    {
        //        try
        //        {
        //            if (HisConfigCFG.IsAutoCreateSaleExpMest
        //                && (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
        //                && this.frmAssignPrescription.cboNhaThuoc.EditValue != null)
        //            {
        //                //Lay thuoc trong kho va kiem tra thuoc co con trong kho khong
        //                decimal damount = AmountOutOfStock(this.ServiceId, (this.frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_ID ?? 0));
        //                if (damount <= 0)
        //                {
        //                    paramWarn.Messages.Add(ResourceMessage.ThuocKhongCoTrongKho);
        //                    throw new ArgumentNullException("medicinetypeStockSDO is null");
        //                }
        //                decimal amountAdded = 0;
        //                if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
        //                {
        //                    amountAdded = ((frmAssignPrescription.mediMatyTypeADOs != null && frmAssignPrescription.mediMatyTypeADOs.Count > 0) ? frmAssignPrescription.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM).Sum(o => o.AMOUNT) ?? 0 : 0);
        //                }
        //                else if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
        //                {
        //                    amountAdded = ((frmAssignPrescription.mediMatyTypeADOs != null && frmAssignPrescription.mediMatyTypeADOs.Count > 0) ? frmAssignPrescription.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM).Sum(o => o.AMOUNT) ?? 0 : 0);
        //                }
        //                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.Amount), this.Amount)
        //                    + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amountAdded), amountAdded)
        //                    + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => damount), damount)
        //                     + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT), frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT)
        //                    );
        //                Rectangle buttonBounds = new Rectangle(frmAssignPrescription.txtMediMatyForPrescription.Bounds.X, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Y, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Width, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Height);
        //                if ((this.Amount + amountAdded) > (frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT ?? 0))
        //                {
        //                    //if (GlobalStore.IsCabinet)//TODO
        //                    //{
        //                    MessageBox.Show("Thuốc vật tư trong kho không đủ khả dụng");
        //                    return false;
        //                    //}

        //                    if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
        //                    {
        //                        var medicineTypeAcin__SameAcinBhyt = GetDataByActiveIngrBhyt();
        //                        frmMessageBoxChooseAcinBhyt form = new frmMessageBoxChooseAcinBhyt(ChonThuocTrongKhoCungHoatChat);
        //                        form.ShowDialog();

        //                        switch (this.ChonThuocThayThe)
        //                        {
        //                            case OptionChonThuocThayThe.None:
        //                                frmAssignPrescription.spinAmount.SelectAll();
        //                                frmAssignPrescription.spinAmount.Focus();
        //                                valid = false;
        //                                break;
        //                            case OptionChonThuocThayThe.ThuocCungHoatChat:
        //                                //thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
        //                                frmAssignPrescription.txtMediMatyForPrescription.Text = medicineTypeAcin__SameAcinBhyt;
        //                                frmAssignPrescription.gridViewMediMaty.ActiveFilterString = " [ACTIVE_INGR_BHYT_NAME] Like '%" + frmAssignPrescription.txtMediMatyForPrescription.Text + "%'";
        //                                //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
        //                                //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
        //                                frmAssignPrescription.gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
        //                                frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
        //                                frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
        //                                frmAssignPrescription.gridViewMediMaty.FocusedRowHandle = 0;
        //                                frmAssignPrescription.gridViewMediMaty.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
        //                                frmAssignPrescription.gridViewMediMaty.OptionsFind.HighlightFindResults = true;

        //                                frmAssignPrescription.popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
        //                                frmAssignPrescription.txtMediMatyForPrescription.Focus();
        //                                frmAssignPrescription.txtMediMatyForPrescription.SelectAll();
        //                                valid = false;
        //                                break;
        //                            case OptionChonThuocThayThe.ThuocNgoaiKho:
        //                                //Trong trường hợp, số lượng vượt quá tồn, mà trong kho cũng không có thuốc nào cùng hoạt chất đang còn tồn thì hiển thị thông báo kiêu 
        //                                //"Thuốc đã chọn và các thuốc cùng hoạt chất khác trong kho không đủ để kê. Bạn có muốn kê thuốc ngoài kho không". 
        //                                //Nếu chọn ok thì lấy thuốc ngoài kho, nếu ko thì ko xử lý j cả
        //                                if (frmAssignPrescription.currentMedicineTypes == null || frmAssignPrescription.currentMedicineTypes.Count == 0)
        //                                {
        //                                    frmAssignPrescription.currentMedicineTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
        //                                    long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
        //                                    if (isOnlyDisplayMediMateIsBusiness == 1 && frmAssignPrescription.currentMedicineTypes != null && frmAssignPrescription.currentMedicineTypes.Count > 0)
        //                                        frmAssignPrescription.currentMedicineTypes = frmAssignPrescription.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();
        //                                }

        //                                V_HIS_MEDICINE_TYPE medicineType = null;
        //                                medicineType = frmAssignPrescription.currentMedicineTypes.Where(o => o.MEDICINE_TYPE_NAME == this.Name)
        //                                    .OrderBy(o => Math.Abs(o.SERVICE_ID - this.ServiceId)).FirstOrDefault();
        //                                //Nếu không tìm được thuốc ngoài kho nào thì tự động chuyển sang thuốc khác (tự mua)
        //                                if (medicineType == null)
        //                                {
        //                                    AddMedicineTypeCategoryByOtherMedi();
        //                                }
        //                                //Nếu tìm thấy thuốc ngoài kho thì lấy luôn thuốc ngoài kho đó
        //                                else
        //                                {
        //                                    AddMedicineTypeCategoryBySameMediAcin(medicineType);
        //                                }
        //                                break;
        //                        }
        //                    }
        //                    else if (this.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
        //                    {
        //                        frmMessageBoxChooseVT form = new frmMessageBoxChooseVT(ChonVatTu);
        //                        form.ShowDialog();

        //                        switch (this.ChonVTThayThe)
        //                        {
        //                            case EnumOptionChonVatTuThayThe.None:
        //                                frmAssignPrescription.spinAmount.SelectAll();
        //                                frmAssignPrescription.spinAmount.Focus();
        //                                valid = false;
        //                                break;
        //                            case EnumOptionChonVatTuThayThe.VatTuNgoaiKho:

        //                                List<V_HIS_MATERIAL_TYPE> materialTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();

        //                                var materialType = materialTypes.FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
        //                                if (materialType == null)
        //                                    throw new ArgumentNullException("Khong tim thay medicineType SERVICE_ID = " + this.ServiceId + " tu danh muc thuoc.");

        //                                AddMaterialTypeCategoryBySameMediAcin(materialType);
        //                                break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (ArgumentNullException ex)
        //        {
        //            valid = false;
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        catch (Exception ex)
        //        {
        //            valid = false;
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }

        //        if (!String.IsNullOrEmpty(paramWarn.GetMessage()))
        //            MessageManager.Show(paramWarn.GetMessage());
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //    return valid;
        //}

        protected void UpdateMedicinePackageInfoInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (this.IsAssignPackage.HasValue && this.IsAssignPackage.Value)
                {
                    medicineTypeSDO.MAME_ID = this.MAME_ID;
                    medicineTypeSDO.IsAssignPackage = true;
                    medicineTypeSDO.TDL_PACKAGE_NUMBER = this.PackageNumber;
                    medicineTypeSDO.EXPIRED_DATE = this.ExpiredDate;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void UpdateMediMatyByMedicineTypeCategory(MediMatyTypeADO addMedicineTypeADO)
        {
            try
            {
                if (addMedicineTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                this.medicineTypeSDO = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.medicineTypeSDO, addMedicineTypeADO);
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
            {
                //frmAssignPrescription.currentHisPatientTypeAlter.PATIENT_TYPE_ID
                //var patientTypeSelected = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
                //patientTypeSelected = this.ChoosePatientTypeDefaultlService(this.PatientTypeAlter.PATIENT_TYPE_ID, this.ServiceId);

                if (
                    //patientTypeSelected != null
                    //&& patientTypeSelected.ID == HisConfigCFG.PatientTypeId__BHYT &&
                     this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    //Get patientType
                    if (String.IsNullOrEmpty(this.Tutorial))
                    {
                        MessageBox.Show(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        valid = false;
                        frmAssignPrescription.txtTutorial.Focus();
                    }
                    if (String.IsNullOrEmpty(this.HtuText))
                    {
                        DialogResult result = XtraMessageBox.Show("Bắt buộc phải nhập cách dùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                        frmAssignPrescription.txtHtu.Focus();
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
                var patientTypeSelected = this.choosePatientTypeDefaultlServiceOther(this.PatientTypeAlter.PATIENT_TYPE_ID, this.ServiceId, this.ServiceTypeId);
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
