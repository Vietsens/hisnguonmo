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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Save
{
    abstract class SaveAbstract : EntityBase
    {
        protected List<MediMatyTypeADO> MediMatyTypeADOs { get; set; }
        protected HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeInfoSDO { get; set; }
        protected int ActionType { get; set; }
        protected long TreatmentId { get; set; }
        protected V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        protected long PatientId { get; set; }
        protected bool IsSaveAndPrint { get; set; }
        protected long RequestRoomId { get; set; }
        protected long SereServParentId { get; set; }
        protected long RemedyCount { get; set; }
        protected string Advise { get; set; }
        protected string RequestLoginname { get; set; }
        protected string RequestUserName { get; set; }
        protected long ExecuteGroupId { get; set; }
        protected long? ParentServiceReqId { get; set; }
        protected DateTime DtInstructionTime { get; set; }
        protected DateTime TimeSelested { get; set; }
        protected string IcdName { get; set; }
        protected string IcdCode { get; set; }
        protected string IcdCauseName { get; set; }
        protected string IcdCauseCode { get; set; }
        protected string IcdText { get; set; }
        protected string IcdSubCode { get; set; }
        protected long SoNgay { get; set; }
        protected bool IsAutoTreatmentEnd { get; set; }
        protected long EndTime { get; set; }
        protected long TreatmentEndTypeId { get; set; }
        protected long AppointmentTime { get; set; }
        protected string AdviseFinish { get; set; }
        protected List<long> AppointmentNextRoomIds { get; set; }
        protected bool IsHomePres { get; set; }
        protected bool IsKidney { get; set; }
        protected long? KidneyTimes { get; set; }
        protected bool IsExecuteKidneyPres { get; set; }
        protected long? DrugStoreId { get; set; }
        protected long? ExpMestReasonId { get; set; }

        protected frmAssignPrescription frmAssignPrescription { get; set; }
        protected bool IsMultiDate { get; set; }
        protected List<long> InstructionTimes { get; set; }
        protected List<PresMedicineSDO> OutPatientPresMedicineSDOs { get; set; }
        protected List<PresMaterialSDO> OutPatientPresMaterialSDOs { get; set; }
        protected List<PresMedicineSDO> InPatientPresMedicineSDOs { get; set; }
        protected List<PresMaterialSDO> InPatientPresMaterialSDOs { get; set; }
        protected List<PresOutStockMatySDO> PresOutStockMatySDOs { get; set; }
        protected List<PresOutStockMetySDO> PresOutStockMetySDOs { get; set; }
        protected List<PresMaterialBySerialNumberSDO> PatientPresMaterialBySerialNumberSDOs { get; set; }
        //protected List<HIS_SERVICE_REQ_MATY> ServiceReqMaties { get; set; }
        //protected List<HIS_SERVICE_REQ_METY> ServiceReqMeties { get; set; }
        protected HisTreatmentFinishSDO TreatmentFinishSDO { get; set; }
        protected long prescriptionTypeId { get; set; }
        protected string ProvisionalDiagnosis { get; set; }
        protected CommonParam Param { get; set; }

        protected SaveAbstract(CommonParam param,
            List<MediMatyTypeADO> mediMatyTypeADOs,
            frmAssignPrescription frmAssignPrescription,
            int actionType,
            bool isSaveAndPrint,
            long parentServiceReqId,
            long sereServParentId
            )
            : base()
        {
            this.Param = param;
            this.ActionType = frmAssignPrescription.actionType;
            this.TreatmentId = frmAssignPrescription.currentTreatmentWithPatientType.ID;
            this.PatientId = frmAssignPrescription.currentTreatmentWithPatientType.PATIENT_ID;
            this.TreatmentWithPatientTypeInfoSDO = frmAssignPrescription.currentTreatmentWithPatientType;
            this.frmAssignPrescription = frmAssignPrescription;
            this.PatientTypeAlter = frmAssignPrescription.currentHisPatientTypeAlter;
            this.MediMatyTypeADOs = mediMatyTypeADOs;
            this.IsSaveAndPrint = isSaveAndPrint;
            this.SoNgay = 1;
            this.RequestLoginname = frmAssignPrescription.txtLoginName.Text;
            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            this.RequestLoginname = String.IsNullOrEmpty(this.RequestLoginname) ? loginName : this.RequestLoginname;
            var data = !String.IsNullOrEmpty(this.RequestLoginname) ? HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Equals(RequestLoginname.ToUpper())).ToList() : null;
            this.RequestUserName = !String.IsNullOrEmpty(frmAssignPrescription.cboUser.Text) ? frmAssignPrescription.cboUser.Text : (data != null && data.Count > 0 ? data[0].USERNAME : "");

            this.IsExecuteKidneyPres = frmAssignPrescription.oldServiceReq != null && frmAssignPrescription.oldServiceReq.IS_EXECUTE_KIDNEY_PRES == 1;
            this.ParentServiceReqId = parentServiceReqId;
            this.SereServParentId = sereServParentId;

            var icdValue = frmAssignPrescription.UcIcdGetValue() as IcdInputADO;
            if (icdValue != null)
            {
                this.IcdCode = icdValue.ICD_CODE;
                this.IcdName = icdValue.ICD_NAME;
            }

            var subIcd = frmAssignPrescription.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
            if (subIcd != null)
            {
                this.IcdSubCode = subIcd.ICD_SUB_CODE;
                this.IcdText = subIcd.ICD_TEXT;
            }

            this.IsMultiDate = frmAssignPrescription.isMultiDateState;
            if (frmAssignPrescription.cboExpMestReason.EditValue != null && frmAssignPrescription.cboExpMestReason.EditValue != "")
            {
                this.ExpMestReasonId = (long?)frmAssignPrescription.cboExpMestReason.EditValue;
            }
        }

        protected void InitBase()
        {
            if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                this.InGenerateListMediMaty();
            else
                this.OutGenerateListMediMaty();
        }

        private void OutGenerateListMediMaty()
        {
            this.OutPatientPresMedicineSDOs = new List<PresMedicineSDO>();
            this.OutPatientPresMaterialSDOs = new List<PresMaterialSDO>();
            this.PresOutStockMatySDOs = new List<PresOutStockMatySDO>();
            this.PresOutStockMetySDOs = new List<PresOutStockMetySDO>();

            foreach (var item in this.MediMatyTypeADOs)
            {
                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                {
                    PresMedicineSDO pres = new PresMedicineSDO();
                    pres.UseOriginalUnitForPres = (item.IsUseOrginalUnitForPres ?? false);
                    pres.MedicineId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    if (item.MedicineBean1Result != null && item.MedicineBean1Result.Count > 0)
                        pres.MedicineBeanIds = item.MedicineBean1Result.Select(o => o.ID).ToList();
                    else if (item.BeanIds != null && item.BeanIds.Count > 0)
                    {
                        pres.MedicineBeanIds = item.BeanIds;
                    }
                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.MedicineTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    if (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value)
                        pres.IsOutParentFee = true;
                    pres.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.HtuText = item.HTU_TEXT;
                    pres.NumOfDays = GetNumOfDays(item);
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.IsBedExpend = item.IsExpendType;

                    if (this.SereServParentId > 0)
                        pres.SereServParentId = this.SereServParentId;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.OutPatientPresMedicineSDOs
                        .FirstOrDefault(
                        o => o.MedicineTypeId == pres.MedicineTypeId
                            && ((o.MedicineId == null && pres.MedicineId == null) || (o.MedicineId == pres.MedicineId))
                            && o.MediStockId == pres.MediStockId
                            && o.PatientTypeId == pres.PatientTypeId
                            && o.IsExpend == pres.IsExpend
                            && o.IsBedExpend == pres.IsBedExpend
                            && o.IsOutParentFee == pres.IsOutParentFee
                            && o.SereServParentId == pres.SereServParentId
                        );
                    if (checkPresExists != null && checkPresExists.MedicineTypeId > 0)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.MedicineBeanIds != null && pres.MedicineBeanIds.Count > 0)
                        {
                            if (checkPresExists.MedicineBeanIds == null) checkPresExists.MedicineBeanIds = new List<long>();
                            checkPresExists.MedicineBeanIds.AddRange(pres.MedicineBeanIds);
                        }
                    }
                    else
                    {
                        this.OutPatientPresMedicineSDOs.Add(pres);
                    }
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                {
                    PresMaterialSDO pres = new PresMaterialSDO();
                    pres.MaterialId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    if (item.MaterialBean1Result != null && item.MaterialBean1Result.Count > 0)
                        pres.MaterialBeanIds = item.MaterialBean1Result.Select(o => o.ID).ToList();
                    else if (item.BeanIds != null && item.BeanIds.Count > 0)
                    {
                        pres.MaterialBeanIds = item.BeanIds;
                    }
                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.FailedAmount = item.FilmNumber.HasValue ? ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.FilmNumber) / (item.CONVERT_RATIO ?? 1) : (item.FilmNumber) : null;
                    pres.MaterialTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.IsBedExpend = item.IsExpendType;
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;

                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }
                    if (this.SereServParentId > 0)
                        pres.SereServParentId = this.SereServParentId;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.OutPatientPresMaterialSDOs
                        .FirstOrDefault(o =>
                            o.MaterialTypeId == pres.MaterialTypeId
                            && ((o.MaterialId == null && pres.MaterialId == null) || (o.MaterialId == pres.MaterialId))
                            && o.MediStockId == pres.MediStockId
                            && o.PatientTypeId == pres.PatientTypeId
                            && o.IsExpend == pres.IsExpend
                            && o.IsBedExpend == pres.IsBedExpend
                            && o.IsOutParentFee == pres.IsOutParentFee
                            && o.EquipmentSetId == pres.EquipmentSetId
                        );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0 && item.IsStent == false)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.FailedAmount.HasValue)
                            checkPresExists.FailedAmount += pres.FailedAmount;
                        if (pres.MaterialBeanIds != null && pres.MaterialBeanIds.Count > 0)
                        {
                            if (checkPresExists.MaterialBeanIds == null) checkPresExists.MaterialBeanIds = new List<long>();
                            checkPresExists.MaterialBeanIds.AddRange(pres.MaterialBeanIds);
                        }
                    }
                    else
                        this.OutPatientPresMaterialSDOs.Add(pres);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                {
                    PresMaterialSDO pres = new PresMaterialSDO();
                    if (item.MaterialBean1Result != null && item.MaterialBean1Result.Count > 0)
                        pres.MaterialBeanIds = item.MaterialBean1Result.Select(o => o.ID).ToList();
                    else if (item.BeanIds != null && item.BeanIds.Count > 0)
                    {
                        pres.MaterialBeanIds = item.BeanIds;
                    }
                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.FailedAmount = item.FilmNumber.HasValue ? ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.FilmNumber) / (item.CONVERT_RATIO ?? 1) : (item.FilmNumber) : null;
                    pres.MaterialTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.IsBedExpend = item.IsExpendType;
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;

                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }
                    if (this.SereServParentId > 0)
                        pres.SereServParentId = this.SereServParentId;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.OutPatientPresMaterialSDOs
                        .FirstOrDefault(o => o.MaterialTypeId == pres.MaterialTypeId
                        && o.MediStockId == pres.MediStockId
                        && o.PatientTypeId == pres.PatientTypeId
                        && o.IsExpend == pres.IsExpend
                        && o.IsBedExpend == pres.IsBedExpend
                        && o.IsOutParentFee == pres.IsOutParentFee
                        && o.EquipmentSetId == pres.EquipmentSetId
                        );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.FailedAmount.HasValue)
                            checkPresExists.FailedAmount += pres.FailedAmount;
                        if (pres.MaterialBeanIds != null && pres.MaterialBeanIds.Count > 0)
                        {
                            if (checkPresExists.MaterialBeanIds == null) checkPresExists.MaterialBeanIds = new List<long>();
                            checkPresExists.MaterialBeanIds.AddRange(pres.MaterialBeanIds);
                        }
                    }
                    else
                        this.OutPatientPresMaterialSDOs.Add(pres);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                {
                    PresOutStockMetySDO mety = new PresOutStockMetySDO();
                    mety.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    mety.MedicineTypeId = item.ID;
                    mety.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    mety.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    mety.UnitName = item.SERVICE_UNIT_NAME;
                    mety.NumOrder = item.NUM_ORDER;
                    mety.Tutorial = item.TUTORIAL;
                    mety.HtuText = item.HTU_TEXT;
                    mety.UseTimeTo = item.UseTimeTo;
                    //mety.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    //mety.ID = item.SERVICE_REQ_METY_MATY_ID;
                    mety.Price = item.PRICE ?? 0;

                    var checkPresExists = this.PresOutStockMetySDOs
                        .FirstOrDefault(o => o.MedicineTypeId == mety.MedicineTypeId
                            && o.MedicineUseFormId == mety.MedicineUseFormId
                        );
                    if (checkPresExists != null && checkPresExists.MedicineTypeId > 0)
                        checkPresExists.Amount += mety.Amount;
                    else
                        this.PresOutStockMetySDOs.Add(mety);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    PresOutStockMatySDO maty = new PresOutStockMatySDO();
                    maty.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    maty.MaterialTypeId = item.ID;
                    maty.MaterialTypeName = item.MEDICINE_TYPE_NAME;
                    maty.UnitName = item.SERVICE_UNIT_NAME;
                    maty.NumOrder = item.NUM_ORDER;
                    //maty.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    //maty.ID = item.SERVICE_REQ_METY_MATY_ID;
                    maty.Price = item.PRICE ?? 0;

                    var checkPresExists = this.PresOutStockMatySDOs
                        .FirstOrDefault(o => o.MaterialTypeId == maty.MaterialTypeId
                        );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0)
                        checkPresExists.Amount += maty.Amount;
                    else
                        this.PresOutStockMatySDOs.Add(maty);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                {
                    PresOutStockMetySDO orty = new PresOutStockMetySDO();
                    orty.Amount = item.AMOUNT ?? 0;
                    orty.NumOrder = item.NUM_ORDER;
                    orty.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    orty.UnitName = item.SERVICE_UNIT_NAME;
                    orty.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    orty.Tutorial = item.TUTORIAL;
                    orty.HtuText = item.HTU_TEXT;
                    //orty.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    orty.UseTimeTo = item.UseTimeTo;
                    orty.Price = item.PRICE ?? 0;

                    var checkPresExists = this.PresOutStockMetySDOs
                        .FirstOrDefault(o => o.MedicineTypeName == orty.MedicineTypeName
                            && o.MedicineUseFormId == orty.MedicineUseFormId
                        );
                    if (checkPresExists != null && String.IsNullOrEmpty(checkPresExists.MedicineTypeName))
                        checkPresExists.Amount += orty.Amount;
                    else
                        this.PresOutStockMetySDOs.Add(orty);
                }
            }
        }

        private void InGenerateListMediMaty()
        {
            this.InPatientPresMedicineSDOs = new List<PresMedicineSDO>();
            this.InPatientPresMaterialSDOs = new List<PresMaterialSDO>();
            this.PresOutStockMatySDOs = new List<PresOutStockMatySDO>();
            this.PresOutStockMetySDOs = new List<PresOutStockMetySDO>();
            this.PatientPresMaterialBySerialNumberSDOs = new List<PresMaterialBySerialNumberSDO>();

            foreach (var item in this.MediMatyTypeADOs)
            {
                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                {
                    PresMedicineSDO pres = new PresMedicineSDO();
                    pres.InstructionTimes = item.IntructionTimeSelecteds;
                    pres.UseOriginalUnitForPres = (item.IsUseOrginalUnitForPres ?? false);
                    pres.MedicineId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.MedicineTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    if (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value)
                        pres.IsOutParentFee = true;
                    pres.IsBedExpend = item.IsExpendType;
                    pres.Speed = item.Speed;//TODO
                    //if (item.IsKHBHYT == true)
                    //    pres.EmbedPatientTypeId = item.PATIENT_TYPE_ID;
                    //Duong dung, huong dan su dung
                    pres.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.HtuText = item.HTU_TEXT;
                    pres.NumOfDays = GetNumOfDays(item);
                    //pres.UseTimeTo = item.UseTimeTo;
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.SereServParentId = item.SereServParentId;
                    if (item.IsKidneyShift.HasValue && item.IsKidneyShift.Value)
                    {
                        //pres.IsKidneyShift = item.IsKidneyShift;//TODO
                        //pres.KidneyShiftCount = item.KidneyShiftCount;//TODO
                    }
                    //if (this.SereServParentId > 0)
                    //    pres.SereServParentId = this.SereServParentId;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.InPatientPresMedicineSDOs
                        .FirstOrDefault(o =>
                            o.MedicineTypeId == pres.MedicineTypeId
                            && ((o.MedicineId == null && pres.MedicineId == null) || (o.MedicineId == pres.MedicineId))
                            && o.MediStockId == pres.MediStockId
                            && o.PatientTypeId == pres.PatientTypeId
                            && o.IsExpend == pres.IsExpend
                            && o.IsBedExpend == pres.IsBedExpend
                            && o.IsOutParentFee == pres.IsOutParentFee
                            && o.SereServParentId == pres.SereServParentId
                        //&& o.EmbedPatientTypeId == pres.EmbedPatientTypeId
                        //&& o.NumOfDays == pres.NumOfDays
                        );
                    if (checkPresExists != null && checkPresExists.MedicineTypeId > 0)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.InstructionTimes != null && pres.InstructionTimes.Count > 0)
                        {
                            if (checkPresExists.InstructionTimes == null) checkPresExists.InstructionTimes = new List<long>();
                            checkPresExists.InstructionTimes.AddRange(pres.InstructionTimes);
                        }
                    }
                    else
                    {
                        this.InPatientPresMedicineSDOs.Add(pres);
                    }
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                {
                    PresMaterialSDO pres = new PresMaterialSDO();
                    pres.InstructionTimes = item.IntructionTimeSelecteds;
                    pres.MaterialId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.MaterialTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.IsBedExpend = item.IsExpendType;
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.SereServParentId = item.SereServParentId;

                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }

                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.InPatientPresMaterialSDOs
                        .FirstOrDefault(o =>
                            o.MaterialTypeId == pres.MaterialTypeId
                            && ((o.MaterialId == null && pres.MaterialId == null) || (o.MaterialId == pres.MaterialId))
                            && o.MediStockId == pres.MediStockId
                            && o.PatientTypeId == pres.PatientTypeId
                            && o.IsExpend == pres.IsExpend
                            && o.IsBedExpend == pres.IsBedExpend
                            && o.IsOutParentFee == pres.IsOutParentFee
                        );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0 && item.IsStent == false)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.InstructionTimes != null && pres.InstructionTimes.Count > 0)
                        {
                            if (checkPresExists.InstructionTimes == null) checkPresExists.InstructionTimes = new List<long>();
                            checkPresExists.InstructionTimes.AddRange(pres.InstructionTimes);
                        }
                    }
                    else
                        this.InPatientPresMaterialSDOs.Add(pres);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)//TODO
                {
                    PresMaterialBySerialNumberSDO pres = new PresMaterialBySerialNumberSDO();
                    pres.InstructionTimes = item.IntructionTimeSelecteds;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.NumOrder = item.NUM_ORDER;
                    pres.SereServParentId = item.SereServParentId;
                    pres.SerialNumber = item.SERIAL_NUMBER;
                    pres.IsBedExpend = item.IsExpendType;

                    this.PatientPresMaterialBySerialNumberSDOs.Add(pres);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                {
                    PresOutStockMetySDO mety = new PresOutStockMetySDO();
                    mety.InstructionTimes = item.IntructionTimeSelecteds;
                    mety.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    mety.MedicineTypeId = item.ID;
                    mety.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    mety.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    mety.UnitName = item.SERVICE_UNIT_NAME;
                    mety.NumOrder = item.NUM_ORDER;
                    mety.Tutorial = item.TUTORIAL;
                    mety.HtuText = item.HTU_TEXT;
                    mety.UseTimeTo = item.UseTimeTo;
                    mety.Price = item.PRICE ?? 0;

                    var checkPresExists = this.PresOutStockMetySDOs
                        .FirstOrDefault(o => o.MedicineTypeId == mety.MedicineTypeId
                            && o.MedicineUseFormId == mety.MedicineUseFormId
                        );
                    if (checkPresExists != null && checkPresExists.MedicineTypeId > 0)
                    {
                        checkPresExists.Amount += mety.Amount;
                        if (mety.InstructionTimes != null && mety.InstructionTimes.Count > 0)
                        {
                            if (checkPresExists.InstructionTimes == null) checkPresExists.InstructionTimes = new List<long>();
                            checkPresExists.InstructionTimes.AddRange(mety.InstructionTimes);
                        }
                    }
                    else
                        this.PresOutStockMetySDOs.Add(mety);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    PresOutStockMatySDO maty = new PresOutStockMatySDO();
                    maty.InstructionTimes = item.IntructionTimeSelecteds;
                    maty.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    maty.MaterialTypeId = item.ID;
                    maty.MaterialTypeName = item.MEDICINE_TYPE_NAME;
                    maty.UnitName = item.SERVICE_UNIT_NAME;
                    maty.NumOrder = item.NUM_ORDER;
                    maty.Price = item.PRICE ?? 0;

                    var checkPresExists = this.PresOutStockMatySDOs
                        .FirstOrDefault(o => o.MaterialTypeId == maty.MaterialTypeId
                        );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0)
                    {
                        checkPresExists.Amount += maty.Amount;
                        if (maty.InstructionTimes != null && maty.InstructionTimes.Count > 0)
                        {
                            if (checkPresExists.InstructionTimes == null) checkPresExists.InstructionTimes = new List<long>();
                            checkPresExists.InstructionTimes.AddRange(maty.InstructionTimes);
                        }
                    }
                    else
                        this.PresOutStockMatySDOs.Add(maty);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                {
                    PresOutStockMetySDO orty = new PresOutStockMetySDO();
                    orty.InstructionTimes = item.IntructionTimeSelecteds;
                    orty.Amount = item.AMOUNT ?? 0;
                    orty.NumOrder = item.NUM_ORDER;
                    orty.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    orty.UnitName = item.SERVICE_UNIT_NAME;
                    orty.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    orty.Tutorial = item.TUTORIAL;
                    orty.HtuText = item.HTU_TEXT;
                    orty.UseTimeTo = item.UseTimeTo;
                    orty.Price = item.PRICE ?? 0;

                    var checkPresExists = this.PresOutStockMetySDOs
                        .FirstOrDefault(o => o.MedicineTypeName == orty.MedicineTypeName
                            && o.MedicineUseFormId == orty.MedicineUseFormId
                        );
                    if (checkPresExists != null && String.IsNullOrEmpty(checkPresExists.MedicineTypeName))
                    {
                        checkPresExists.Amount += orty.Amount;
                        if (orty.InstructionTimes != null && orty.InstructionTimes.Count > 0)
                        {
                            if (checkPresExists.InstructionTimes == null) checkPresExists.InstructionTimes = new List<long>();
                            checkPresExists.InstructionTimes.AddRange(orty.InstructionTimes);
                        }
                    }
                    else
                        this.PresOutStockMetySDOs.Add(orty);
                }
            }
        }

        public long? GetNumOfDays(MediMatyTypeADO item)
        {
            long? numOfDays = null;
            try
            {
                if (item.UseDays.HasValue)
                    numOfDays = (long)(item.UseDays);
                else
                {
                    if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        numOfDays = this.SoNgay;
                    }
                    else
                    {
                        if ((item.UseTimeTo ?? 0) > 0)
                        {
                            System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.UseTimeTo ?? 0).Value;
                            System.DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTimes.FirstOrDefault()).Value;
                            TimeSpan diff__Day = (dtUseTimeTo.Date - dtUseTime.Date);
                            numOfDays = diff__Day.Days + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                numOfDays = null;
            }

            return numOfDays;
        }

        protected bool CheckValid()
        {
            bool valid = true;
            try
            {
                valid = valid && this.CheckValidHeinServicePrice(Param, this.MediMatyTypeADOs);
                valid = valid && this.CheckValidDataInGridService(Param, this.MediMatyTypeADOs);//TODO
                valid = valid && (this.RequestRoomId > 0);
                //valid = valid && this.CheckTreatmentFinish();
                if (this.RequestRoomId <= 0)
                    Inventec.Common.Logging.LogSystem.Warn("Khong xac dinh du lieu phong yeu cau. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.RequestRoomId), this.RequestRoomId));
                //if (!valid && Param.Messages != null && Param.Messages.Count > 0)
                //{
                //    MessageManager.Show(Param, null);
                //}
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        //private bool CheckTreatmentFinish()
        //{
        //    bool result = true;
        //    try
        //    {
        //        if (frmAssignPrescription.treatmentFinishProcessor != null && !frmAssignPrescription.treatmentFinishProcessor.GetValidate(frmAssignPrescription.ucTreatmentFinish))
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = true;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        private bool CheckValidDataInGridService(CommonParam param, List<MediMatyTypeADO> serviceCheckeds__Send)
        {
            bool valid = true;
            int validCount = 0;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    foreach (var item in serviceCheckeds__Send)
                    {
                        valid = true;
                        string messageErr = "";
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                        {
                            messageErr = String.Format(ResourceMessage.CanhBaoThuoc, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item.MEDICINE_TYPE_NAME, System.Drawing.FontStyle.Bold));
                        }
                        else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        {
                            messageErr = String.Format(ResourceMessage.CanhBaoVatTu, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item.MEDICINE_TYPE_NAME, System.Drawing.FontStyle.Bold));
                        }

                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && item.PATIENT_TYPE_ID <= 0)
                        {
                            valid = false;
                            validCount++;
                            messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.KhongCoDoiTuongThanhToan, System.Drawing.Color.Maroon));
                            Inventec.Common.Logging.LogSystem.Warn("Thuoc/Vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " khong co doi tuong thanh toan.");
                        }
                        if (item.AMOUNT <= 0 || item.AMOUNT == null)
                        {
                            valid = false;
                            validCount++;
                            messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.KhongNhapSoLuong, System.Drawing.Color.Maroon));
                            Inventec.Common.Logging.LogSystem.Warn("Thuoc/vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " khong co so luong.");
                        }
                        if ((item.AmountAlert ?? 0) > 0)
                        {
                            valid = false;
                            validCount++;
                            messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho, System.Drawing.Color.Maroon));
                            Inventec.Common.Logging.LogSystem.Warn("Thuoc/vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " co so luong lon hon so luong kha dung trong kho.");
                        }
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                        {

                            if ((item.MEDICINE_USE_FORM_ID ?? 0) == 0 && item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung, System.Drawing.Color.Maroon));
                                valid = false;
                                Inventec.Common.Logging.LogSystem.Warn("Doi tuong thanh toan bhyt bat buoc phai nhap thong tin duong dung cua thuoc.");
                            }

                            if (String.IsNullOrEmpty(item.TUTORIAL))
                            {
                                messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, System.Drawing.Color.Maroon));
                                valid = false;
                                Inventec.Common.Logging.LogSystem.Warn("Doi tuong thanh toan bhyt bat buoc phai nhap thong tin duong dan su dung cua thuoc.");
                            }
                        }

                        if (!String.IsNullOrEmpty(item.TUTORIAL) && Encoding.UTF8.GetByteCount(item.TUTORIAL) > 1000)
                        {
                            messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.HDSDVuotQuaKyTu, System.Drawing.Color.Maroon));
                            valid = false;
                        }
                        //Check takebean thất bại
                        //if (item.IsNotTakeBean == true)
                        //{
                        //    //messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, System.Drawing.Color.Maroon));
                        //    valid = false;
                        //    Inventec.Common.Logging.LogSystem.Warn("Take bean thất bại");
                        //}

                        if (!valid)
                        {
                            param.Messages.Add(messageErr + ";");
                        }
                        if (validCount > 0) valid = false;
                    }
                    var existsMameType = serviceCheckeds__Send.Any(o => !(o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
                    var existsMame = serviceCheckeds__Send.Any(o => (o.IsAssignPackage.HasValue && o.IsAssignPackage.Value) && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
                    if (existsMameType && existsMame)
                    {
                        param.Messages.Add(ResourceMessage.Trong1LanKeDonChiDuocKeTheoLoHoacTheoLoai);
                        valid = false;
                    }
                }
                else
                {
                    Inventec.Desktop.Common.LibraryMessage.MessageUtil.SetParam(param, Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// Hàm kiểm tra số tiền đã kê có vượt quá cấu hình trần bảo hiểm không
        /// Trường hợp trần được cấu hình trong hồ sơ điều trị thì lấy từ hsdt
        /// Trường hợp hsdt không cấu hình trần thì lấy trần từ cấu hình ccc ra
        /// Chỉ check trần số tiền bhyt khi có cấu hình trần
        /// </summary>
        /// <param name="param"></param>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool CheckValidHeinServicePrice(CommonParam param, List<MediMatyTypeADO> serviceCheckeds__Send)
        {
            bool valid = true;
            decimal tongtienThuocPhatSinh = 0;
            string messageErr = "";
            try
            {

                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    decimal limitHeinMedicinePrice__RightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg;
                    decimal limitHeinMedicinePrice__NotRightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg;

                    //Kiểm tra, nếu có key cấu hình "check giới hạn thuốc", thì khi người dùng nhấn nút "Lưu" mới lấy thông tin hồ sơ điều trị để check
                    if (limitHeinMedicinePrice__RightMediOrg > 0
                        || limitHeinMedicinePrice__NotRightMediOrg > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE), this.TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP), this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.PatientTypeAlter.TREATMENT_TYPE_CODE), this.PatientTypeAlter.TREATMENT_TYPE_CODE));

                        //Nếu hồ sơ điều trị cấu hình trường IS_NOT_CHECK_LHMP = 1 thì bỏ qua không check, return true
                        //Hoặc đối tượng điều tị là điều trị nội/ngoại trú thì bỏ qua không check
                        //Sửa lại chỉ tính trần bhyt theo đơn phòng khám. (theo 2 cấu hình mức trần bn đúng tuyến đúng cskcb và đúng tuyến chuyển tuyến)
                        //không tính đơn tủ trực
                        if ((this.TreatmentWithPatientTypeInfoSDO != null
                        && this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP.HasValue
                        && (this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP ?? 0) == 1)
                        || this.PatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatIn
                        || this.PatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatOut
                        || GlobalStore.IsCabinet
                            )
                        {
                            return true;
                        }

                        frmAssignPrescription.limitHeinMedicinePrice = frmAssignPrescription.IsLimitHeinMedicinePrice(this.TreatmentId);

                        var bhyt__Exists = serviceCheckeds__Send
                            .Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && !o.IsExpend).ToList();
                        //Kiểm tra tiền bhyt đã kê vượt mức giới hạn chưa
                        if (frmAssignPrescription.limitHeinMedicinePrice == true)
                        {
                            valid = false;
                            messageErr = String.Format(ResourceMessage.TienBHYTDaKeVuotQuaMucGioiHan);
                        }
                        else if (bhyt__Exists != null
                            && bhyt__Exists.Count > 0
                            && this.PatientTypeAlter != null
                            && (limitHeinMedicinePrice__RightMediOrg > 0 || limitHeinMedicinePrice__NotRightMediOrg > 0)
                            )
                        {
                            foreach (var item in bhyt__Exists)
                            {
                                tongtienThuocPhatSinh += (item.TotalPrice);
                            }

                            //Đối với bệnh nhân đúng tuyến KCB
                            if (limitHeinMedicinePrice__RightMediOrg > 0
                                && this.PatientTypeAlter.HEIN_MEDI_ORG_CODE == HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT)
                            {
                                if (tongtienThuocPhatSinh + frmAssignPrescription.totalPriceBHYT > limitHeinMedicinePrice__RightMediOrg)
                                {
                                    messageErr = String.Format(ResourceMessage.SoTienDaKeChoBHYTDaVuotquaMucGioiHan, Inventec.Common.Number.Convert.NumberToString(tongtienThuocPhatSinh + frmAssignPrescription.totalPriceBHYT, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToString(HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg));
                                    valid = false;
                                }
                            }

                            //Đối với bệnh nhân chuyển tuyến
                            if (limitHeinMedicinePrice__NotRightMediOrg > 0
                                && this.PatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE
                                && this.PatientTypeAlter.HEIN_MEDI_ORG_CODE != HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT)
                            {
                                if (tongtienThuocPhatSinh + frmAssignPrescription.totalPriceBHYT > limitHeinMedicinePrice__NotRightMediOrg)
                                {
                                    messageErr = String.Format(ResourceMessage.SoTienDaKeChoBHYTDaVuotquaMucGioiHan, Inventec.Common.Number.Convert.NumberToString(tongtienThuocPhatSinh + frmAssignPrescription.totalPriceBHYT, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToString(HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg, ConfigApplications.NumberSeperator));
                                    valid = false;
                                }
                            }
                        }

                        if (!valid)
                        {
                            param.Messages.Add(messageErr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }


    }
}
