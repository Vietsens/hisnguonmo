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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Add
{
    public abstract class AddAbstract : EntityBase
    {
        protected long Id { get; set; }
        protected decimal Amount { get; set; }
        protected decimal RawAmount { get; set; }
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
        protected string Sang { get; set; }
        protected string Trua { get; set; }
        protected string Chieu { get; set; }
        protected string Toi { get; set; }
        protected string BreathSpeed { get; set; }
        protected string BreathTime { get; set; }
        protected bool? IsStent { get; set; }
        protected bool? IsAllowOdd { get; set; }
        protected bool? IsAllowOddAndExportOdd { get; set; }
        protected decimal? Speed { get; set; }
        protected bool? IsKidneyShift { get; set; }
        protected decimal? KidneyShiftCount { get; set; }
        protected V_HIS_TREATMENT treatmentView { get; set; }
        protected ValidAddRow ValidAddRow { get; set; }
        //protected GetPatientTypeBySeTy GetPatientTypeBySeTy { get; set; }
        internal HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService;
        internal HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther;
        protected CalulateUseTimeTo CalulateUseTimeTo { get; set; }
        protected ExistsAssianInDay ExistsAssianInDay { get; set; }

        protected List<DMediStock1ADO> MediStockD1SDOs { get; set; }
        protected bool? IsOutKtcFee { get; set; }
        protected long TreatmentId { get; set; }
        protected MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        protected long PatientId { get; set; }
        protected long RequestRoomId { get; set; }

        protected List<MediMatyTypeADO> MediMatyTypeADOs { get; set; }
        protected int IdRow = 1;
        protected HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfoSDO { get; set; }
        protected frmAssignPrescription frmAssignPrescription { get; set; }

        protected CommonParam Param { get; set; }
        protected MediMatyTypeADO medicineTypeSDO { get; set; }

        protected List<long> HtuIds { get; set; }
        protected long HtuIdNotCheckAcinInteractive { get; set; }
        protected long MedicineUseFormId { get; set; }
        protected string Tutorial { get; set; }
        protected string HtuText { get; set; }
        protected bool IsExpend { get; set; }
        protected bool IsDisableExpend { get; set; }
        protected decimal? UseDays { get; set; }
        protected object DataRow { get; set; }
        protected string SeriNumber { get; set; }
        protected long? UseCount { get; set; }
        protected long? UseRemainCount { get; set; }
        protected long? MaxReuseCount { get; set; }
        public bool IsNotOutStock { get; set; }
        protected string PackageNumber { get; set; }
        protected long? ExpiredDate { get; set; }
        protected bool? IsAssignPackage { get; set; }
        protected short? IS_SPLIT_COMPENSATION { get; set; }
        protected short? IS_OUT_HOSPITAL { get; set; }
        protected long? MAME_ID { get; set; }
        protected long? PREVIOUS_USING_COUNT { get; set; }
        protected bool IsMultiDateState { get; set; }
        protected string ATC_CODES { get; set; }
        protected string ATC_GROUP_CODES { get; set; }
        protected string CONTRAINDICATION { get; set; }
        protected string DESCRIPTION { get; set; }
        protected string CONTRAINDICATION_IDS { get; set; }
        protected List<long> IntructionTimeSelecteds { get; set; }
        protected long? SERVICE_CONDITION_ID { get; set; }
        protected string SERVICE_CONDITION_NAME { get; set; }

        protected string ExceedLimitInPresReason { get; set; }
        protected string ExceedLimitInDayReason { get; set; }
        protected string OddPresReason { get; set; }
        protected string IcdCode { get; set; }
        protected string IcdCauseCode { get; set; }
        protected string IcdSubCode { get; set; }
        public OptionChonThuocThayThe ChonThuocThayThe { get; set; }
        public EnumOptionChonVatTuTuongDuong ChonVatTuTuongDuong { get; set; }
        public EnumOptionChonVatTuThayThe ChonVTThayThe { get; set; }

        protected MediMatyTypeADO medicineTypeSDO__Category__SameMediAcin;

        internal AddAbstract(CommonParam param,
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
            this.HtuIds = frmAssignPrescription.DataHtuList.Exists(o=>o.IsChecked) ? frmAssignPrescription.DataHtuList.Where(o => o.IsChecked).Select(o=>o.ID).ToList() : null;
            this.HtuIdNotCheckAcinInteractive = frmAssignPrescription.DataHtuList.Exists(o => o.IsChecked && o.CHECK_ACIN_INTERACTIVE != 1) ? frmAssignPrescription.DataHtuList.Where(o => o.IsChecked && o.CHECK_ACIN_INTERACTIVE != 1).OrderBy(o => o.NUM_ORDER).ToList()[0].ID : 0;
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
            this.NumOrder = frmAssignPrescription.idRow;

            this.ValidAddRow = validAddRow;
            this.choosePatientTypeDefaultlService = choosePatientTypeDefaultlService;
            this.choosePatientTypeDefaultlServiceOther = choosePatientTypeDefaultlServiceOther;
            this.CalulateUseTimeTo = calulateUseTimeTo;
            this.ExistsAssianInDay = existsAssianInDay;
            this.IsNotOutStock = frmAssignPrescription.GetSelectedOpionGroup() == 1;
            this.DataRow = dataRow;
            this.ExceedLimitInPresReason = frmAssignPrescription.reasonMaxPrescription;
            this.ExceedLimitInDayReason = frmAssignPrescription.reasonMaxPrescriptionDay;

            this.OddPresReason = frmAssignPrescription.reasonOddPrescription;
            if (HisConfigCFG.ManyDayPrescriptionOption == 2 && GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
            {
                this.IsMultiDateState = frmAssignPrescription.isMultiDateState;
                if (this.IsMultiDateState && frmAssignPrescription.UcDateGetValueForMedi() != null
                    && frmAssignPrescription.UcDateGetValueForMedi().Count > 0)
                {
                    this.IntructionTimeSelecteds = frmAssignPrescription.UcDateGetValueForMedi();
                }
                else
                {
                    this.IntructionTimeSelecteds = frmAssignPrescription.intructionTimeSelecteds;
                }
                Inventec.Common.Logging.LogSystem.Debug("AddAbstract____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IntructionTimeSelecteds), IntructionTimeSelecteds));
            }
            if (frmAssignPrescription.currentMedicineTypeADOForEdit != null)
            {
                this.ATC_CODES =frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_CODES;
                this.ATC_GROUP_CODES = frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_GROUP_CODES;
            }
        }

        protected void CreateADO()
        {
            medicineTypeSDO = new MediMatyTypeADO();
            medicineTypeSDO.IdRow = this.IdRow;
            medicineTypeSDO.ID = this.Id;
            medicineTypeSDO.SERIAL_NUMBER = this.SeriNumber;
            medicineTypeSDO.MAX_REUSE_COUNT = this.MaxReuseCount;
            medicineTypeSDO.USE_COUNT = this.UseCount;
            medicineTypeSDO.USE_REMAIN_COUNT = this.UseRemainCount;
            medicineTypeSDO.AMOUNT = this.Amount;
            medicineTypeSDO.PRES_AMOUNT = this.frmAssignPrescription.PresAmount;
            medicineTypeSDO.RAW_AMOUNT = this.RawAmount;
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
            medicineTypeSDO.IsAllowOddAndExportOdd = this.IsAllowOddAndExportOdd;
            medicineTypeSDO.ALERT_MAX_IN_PRESCRIPTION = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ALERT_MAX_IN_PRESCRIPTION : null;
            medicineTypeSDO.ALERT_MAX_IN_DAY = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ALERT_MAX_IN_DAY : null;
            medicineTypeSDO.IS_BLOCK_MAX_IN_PRESCRIPTION = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_BLOCK_MAX_IN_PRESCRIPTION : null;
            medicineTypeSDO.IS_BLOCK_MAX_IN_DAY = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_BLOCK_MAX_IN_DAY : null;
            medicineTypeSDO.TDL_GENDER_ID = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.TDL_GENDER_ID : null;
            if (!String.IsNullOrEmpty(this.Sang))
                medicineTypeSDO.Sang = this.Sang;
            if (!String.IsNullOrEmpty(this.Trua))
                medicineTypeSDO.Trua = this.Trua;
            if (!String.IsNullOrEmpty(this.Chieu))
                medicineTypeSDO.Chieu = this.Chieu;
            if (!String.IsNullOrEmpty(this.Toi))
                medicineTypeSDO.Toi = this.Toi;
            if (!String.IsNullOrEmpty(this.BreathSpeed))
                medicineTypeSDO.BREATH_SPEED = this.BreathSpeed;
            if (!String.IsNullOrEmpty(this.BreathTime))
                medicineTypeSDO.BREATH_TIME = this.BreathTime;
            medicineTypeSDO.TUTORIAL = this.Tutorial;
            medicineTypeSDO.HTU_TEXT = this.HtuText;
            medicineTypeSDO.IsExpend = this.IsExpend;
            medicineTypeSDO.IsDisableExpend = this.IsDisableExpend;
            if (this.MedicineUseFormId > 0)
                medicineTypeSDO.MEDICINE_USE_FORM_ID = this.MedicineUseFormId;
            if (this.HtuIds != null && this.HtuIds.Count > 0)
                medicineTypeSDO.HTU_IDs = this.HtuIds;
            if(this.HtuIdNotCheckAcinInteractive > 0)
                medicineTypeSDO.HTU_ID_NOT_CHECK_ACIN_INTERACTIVE = this.HtuIdNotCheckAcinInteractive;
            if (frmAssignPrescription.currentMedicineTypeADOForEdit != null)
            {
                if (frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE.HasValue || frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_VAT_RATIO.HasValue)
                {
                    medicineTypeSDO.LAST_EXP_PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE;
                    medicineTypeSDO.LAST_EXP_VAT_RATIO = frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_VAT_RATIO;
                    decimal? priceRaw = this.Amount * (frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE ?? 0) * (1 + (frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_VAT_RATIO ?? 0));
                    priceRaw = (frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO.HasValue && frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO > 0) ? priceRaw / frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO.Value : priceRaw;

                    medicineTypeSDO.TotalPrice = priceRaw ?? 0;
                    medicineTypeSDO.PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit.LAST_EXP_PRICE;
                }
                else
                {
                    medicineTypeSDO.PRICE = frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE;
                    medicineTypeSDO.TotalPrice = (this.Amount * (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_PRICE ?? 0)) * (1 + (frmAssignPrescription.currentMedicineTypeADOForEdit.IMP_VAT_RATIO ?? 0));
                }

            }
            medicineTypeSDO.Speed = this.Speed;
            medicineTypeSDO.IsKidneyShift = this.IsKidneyShift;
            medicineTypeSDO.KidneyShiftCount = this.KidneyShiftCount;
            medicineTypeSDO.CONVERT_RATIO = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_RATIO : null;
            medicineTypeSDO.CONVERT_UNIT_CODE = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_UNIT_CODE : "";
            medicineTypeSDO.CONVERT_UNIT_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.CONVERT_UNIT_NAME : "";

            medicineTypeSDO.OTHER_PAY_SOURCE_ID = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.OTHER_PAY_SOURCE_ID : null;
            medicineTypeSDO.OTHER_PAY_SOURCE_CODE = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.OTHER_PAY_SOURCE_CODE : "";
            medicineTypeSDO.OTHER_PAY_SOURCE_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.OTHER_PAY_SOURCE_NAME : "";

            medicineTypeSDO.IntructionTimeSelecteds = this.IntructionTimeSelecteds;
            medicineTypeSDO.IsMultiDateState = this.IsMultiDateState;
            medicineTypeSDO.IS_SPLIT_COMPENSATION = this.IS_SPLIT_COMPENSATION;
            medicineTypeSDO.ATC_CODES = this.ATC_CODES;
            medicineTypeSDO.ATC_GROUP_CODES = this.ATC_GROUP_CODES;
            medicineTypeSDO.CONTRAINDICATION = this.CONTRAINDICATION;
            medicineTypeSDO.DESCRIPTION = this.DESCRIPTION;
            medicineTypeSDO.CONTRAINDICATION_IDS = this.CONTRAINDICATION_IDS;
            medicineTypeSDO.PREVIOUS_USING_COUNT = this.PREVIOUS_USING_COUNT;
            medicineTypeSDO.SERVICE_CONDITION_ID = this.SERVICE_CONDITION_ID;
            medicineTypeSDO.SERVICE_CONDITION_NAME = this.SERVICE_CONDITION_NAME;
            //medicineTypeSDO.PrimaryKey = (this.ServiceId + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
            medicineTypeSDO.IS_SUB_PRES = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_SUB_PRES : null;
            medicineTypeSDO.IsNotOutStock = this.IsNotOutStock;
            medicineTypeSDO.IcdsWarning = this.frmAssignPrescription.icdsWarning;
            medicineTypeSDO.EXCEED_LIMIT_IN_PRES_REASON = this.ExceedLimitInPresReason;
            medicineTypeSDO.EXCEED_LIMIT_IN_DAY_REASON = this.ExceedLimitInDayReason;
            medicineTypeSDO.ODD_PRES_REASON = this.OddPresReason;
            medicineTypeSDO.IsWarned = this.frmAssignPrescription.IsWarned;
            medicineTypeSDO.ODD_WARNING_CONTENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ODD_WARNING_CONTENT : null;
            medicineTypeSDO.IdRowPopupGrid = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IdRow : 0;
            medicineTypeSDO.IntructionTime = frmAssignPrescription.InstructionTime;
            medicineTypeSDO.IS_IDENTITY_MANAGEMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_IDENTITY_MANAGEMENT : null;
            medicineTypeSDO.IS_REUSABLE = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_REUSABLE : null;
            medicineTypeSDO.ALERT_MAX_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.ALERT_MAX_IN_TREATMENT : null;
            medicineTypeSDO.IS_BLOCK_MAX_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.IS_BLOCK_MAX_IN_TREATMENT : null;
            medicineTypeSDO.NUMBER_EXCEED_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.NUMBER_EXCEED_IN_TREATMENT : null;
            medicineTypeSDO.NUMBER_PRESCIPTION_IN_TREATMENT = frmAssignPrescription.currentMedicineTypeADOForEdit != null ? frmAssignPrescription.currentMedicineTypeADOForEdit.NUMBER_PRESCIPTION_IN_TREATMENT : null;
            var amountPres = (medicineTypeSDO.NUMBER_PRESCIPTION_IN_TREATMENT ?? 0) + medicineTypeSDO.AMOUNT + frmAssignPrescription.mediMatyTypeADOs.Where(o => o.ID == medicineTypeSDO.ID && o.PrimaryKey != medicineTypeSDO.PrimaryKey).Sum(o => o.UseDays != 0 ? o.AMOUNT / o.UseDays : o.AMOUNT);
            if (amountPres > medicineTypeSDO.ALERT_MAX_IN_TREATMENT)
                medicineTypeSDO.IsAlertInTreatPresciption = true;
        }

        protected void SaveDataAndRefesh(MediMatyTypeADO mediMatyADO)
        {
            frmAssignPrescription.mediMatyTypeADOs.Add(mediMatyADO);
            frmAssignPrescription.idRow += frmAssignPrescription.stepRow;
            frmAssignPrescription.ProcessMediStock(frmAssignPrescription.mediMatyTypeADOs);
            frmAssignPrescription.gridViewServiceProcess.GridControl.DataSource = null;
            frmAssignPrescription.gridViewServiceProcess.GridControl.DataSource = frmAssignPrescription.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
            int rowHandlerActive = 0;
            for (int i = 0; i < frmAssignPrescription.mediMatyTypeADOs.Count; i++)
            {
                if (frmAssignPrescription.mediMatyTypeADOs[i].SERVICE_ID == mediMatyADO.SERVICE_ID
                    && frmAssignPrescription.mediMatyTypeADOs[i].DataType == mediMatyADO.DataType
                    && frmAssignPrescription.mediMatyTypeADOs[i].NUM_ORDER == mediMatyADO.NUM_ORDER
                    && frmAssignPrescription.mediMatyTypeADOs[i].PrimaryKey == mediMatyADO.PrimaryKey
                    && frmAssignPrescription.mediMatyTypeADOs[i].SERVICE_CONDITION_ID == mediMatyADO.SERVICE_CONDITION_ID)
                {
                    rowHandlerActive = i;
                    break;
                }
            }

            frmAssignPrescription.gridViewServiceProcess.FocusedRowHandle = rowHandlerActive;

            Inventec.Common.Logging.LogSystem.Debug("SaveDataAndRefesh__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowHandlerActive), rowHandlerActive));

            frmAssignPrescription.ReSetDataInputAfterAdd__MedicinePage();
            frmAssignPrescription.ReSetChongCHiDinhInfoControl__MedicinePage();
            frmAssignPrescription.SetEnableButtonControl(frmAssignPrescription.actionType);
            frmAssignPrescription.ResetFocusMediMaty(true);
            frmAssignPrescription.SetTotalPrice__TrongDon();
            frmAssignPrescription.gridControlTutorial.DataSource = null;
            Inventec.Common.Logging.LogSystem.Info(
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.SplitOffset), HisConfigCFG.SplitOffset) + "____" +
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsTreatmentIn), GlobalStore.IsTreatmentIn) + "____" +
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.mediMatyTypeADOs), frmAssignPrescription.mediMatyTypeADOs));

            frmAssignPrescription.CheckConditionService(mediMatyADO);
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
                frmAssignPrescription.FillDataOtherPaySourceDataRow(medicineTypeSDO);
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
                if (medicineTypeSDO != null && frmAssignPrescription.lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
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

        protected bool ValidThuocDaKeTrongNgay()
        {
            bool valid = true;
            try
            {
                IdRow = 1;
                var medicinetypeStockExists = this.MediMatyTypeADOs
                     .FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
                if (medicinetypeStockExists != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThuocDaduocKe, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == DialogResult.No)
                    {
                        valid = false;
                        Inventec.Common.Logging.LogSystem.Debug("ValidThuocDaKeTrongNgay: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                    }
                    IdRow = this.MediMatyTypeADOs.Where(o => o.SERVICE_ID == this.ServiceId).Count() + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        protected bool ValidVatTuKeTrongNgay()
        {
            bool valid = true;
            try
            {
                var medicinetypeStockExists = this.MediMatyTypeADOs
                     .FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
                if (medicinetypeStockExists != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.VatTuduocKe, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True) == DialogResult.No)
                    {
                        valid = false;
                        Inventec.Common.Logging.LogSystem.Debug("ValidVatTuKeTrongNgay: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
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

                //var medis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();

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

        protected DMediStock1ADO GetDataByMaterialTypeMap()
        {
            DMediStock1ADO result = null;
            try
            {
                result = this.MediStockD1SDOs.FirstOrDefault(o =>
                    (o.MATERIAL_TYPE_MAP_ID == frmAssignPrescription.currentMedicineTypeADOForEdit.MATERIAL_TYPE_MAP_ID)
                    && o.AMOUNT >= this.Amount
                    && ((o.SERVICE_ID == this.ServiceId && o.MEDI_STOCK_ID != this.MediStockId) || (o.SERVICE_ID != this.ServiceId)));
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

        /// <summary>
        /// #20013
        /// Khi bật key cấu hình hệ thống tự động tạo phiếu xuất bán (MOS.HIS_SERVICE_REQ.IS_AUTO_CREATE_SALE_EXP_MEST) 
        /// thì ko cho bổ sung thuốc/vật tư vượt quá số lượng khả dụng của nhà thuốc (tương tự như kê thuốc/vật tư trong kho)
        /// </summary>
        /// <returns></returns>      
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
                        if ((this.Amount *  (frmAssignPrescription.intructionTimeSelecteds != null && frmAssignPrescription.intructionTimeSelecteds.Count > 0 ?frmAssignPrescription.intructionTimeSelecteds.Count() : 1) + amountAdded) > (frmAssignPrescription.currentMedicineTypeADOForEdit.AMOUNT ?? 0))
                        {
                            MessageBox.Show("Thuốc vật tư trong kho không đủ khả dụng");
                            return false;
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
                medicineTypeSDO__Category__SameMediAcin.IsDisableExpend = this.IsDisableExpend;
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
                medicineTypeSDO__Category__SameMediAcin.IsDisableExpend = this.IsDisableExpend;
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

        protected void ChonVatTuTrongKhoTuongDuong(EnumOptionChonVatTuTuongDuong optionChonVatTuTuongDuong)
        {
            try
            {
                this.ChonVatTuTuongDuong = optionChonVatTuTuongDuong;
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
                if (this.ExistsAssianInDay != null && this.ExistsAssianInDay(medicineTypeSDO))
                {
                    medicineTypeSDO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        medicineTypeSDO.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                    }
                    else if (medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                                    || medicineTypeSDO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                        medicineTypeSDO.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoVatTuDaKeTrongNgay;
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
                    if (String.IsNullOrEmpty(this.HtuText))
                    {
                        DialogResult result = XtraMessageBox.Show("Bắt buộc phải nhập cách dùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                        frmAssignPrescription.memHtu.Focus();
                    }

                    if (!HisConfigCFG.IsNotAutoGenerateTutorial && String.IsNullOrEmpty(this.Tutorial))
                    {
                        MessageBox.Show(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        valid = false;
                        frmAssignPrescription.txtTutorial.Focus();
                    }

                    if (this.MedicineUseFormId <= 0)
                    {
                        MessageBox.Show(ResourceMessage.BatBuocPhaiNhapDuongDung, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
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


    }
}
