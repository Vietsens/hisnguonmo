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
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.Save
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
        protected string IcdNameYHCT { get; set; }
        protected string IcdCode { get; set; }
        protected string IcdCodeYHCT { get; set; }
        protected string IcdCauseName { get; set; }
        protected string IcdCauseCode { get; set; }
        protected string IcdText { get; set; }
        protected string IcdTextYHCT { get; set; }
        protected string IcdSubCode { get; set; }
        protected string IcdSubCodeYHCT { get; set; }
        protected long? TIME_TO { get; set; }
        protected List<long> USE_TIMES { get; set; }
        protected long SoNgay { get; set; }
        protected bool IsAutoTreatmentEnd { get; set; }
        protected long EndTime { get; set; }
        protected long TreatmentEndTypeId { get; set; }
        protected long AppointmentTime { get; set; }
        protected frmAssignPrescription frmAssignPrescription { get; set; }
        protected bool IsMultiDate { get; set; }
        protected List<long> InstructionTimes { get; set; }
        protected List<PresMedicineSDO> OutPatientPresMedicineSDOs { get; set; }
        protected List<PresMaterialSDO> OutPatientPresMaterialSDOs { get; set; }
        protected List<PresMedicineSDO> InPatientPresMedicineSDOs { get; set; }
        protected List<PresMaterialSDO> InPatientPresMaterialSDOs { get; set; }
        protected List<PresOutStockMatySDO> ServiceReqMaties { get; set; }
        protected List<PresOutStockMetySDO> ServiceReqMeties { get; set; }
        protected HisTreatmentFinishSDO TreatmentFinishSDO { get; set; }
        protected long prescriptionTypeId { get; set; }
        protected long? DrugStoreId { get; set; }
        protected long? ExpMestReasonId { get; set; }
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
            this.SoNgay = (long)frmAssignPrescription.spinSoNgay.Value;
            this.RemedyCount = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtLadder.Text);
            this.Advise = frmAssignPrescription.txtAdvise.Text;
            this.prescriptionTypeId = frmAssignPrescription.prescriptionTypeId;


            if (frmAssignPrescription.currentMediStockNhaThuocSelecteds != null && frmAssignPrescription.currentMediStockNhaThuocSelecteds.Count > 0 && frmAssignPrescription.rdOpionGroup.SelectedIndex == 1 && (HisConfigCFG.IsAutoCreateSaleExpMest || HisConfigCFG.DrugStoreComboboxOption))
                this.DrugStoreId = (long)frmAssignPrescription.currentMediStockNhaThuocSelecteds.FirstOrDefault().ID;

            if (frmAssignPrescription.cboUser.EditValue != null)
            {
                this.RequestLoginname = frmAssignPrescription.cboUser.EditValue.ToString();
                this.RequestUserName = frmAssignPrescription.cboUser.Text;
            }
            this.ParentServiceReqId = parentServiceReqId;
            this.SereServParentId = sereServParentId;

            if (frmAssignPrescription.ucIcd != null)
            {
                var icdValue = frmAssignPrescription.icdProcessor.GetValue(frmAssignPrescription.ucIcd);
                if (icdValue != null && icdValue is IcdInputADO)
                {
                    this.IcdCode = ((IcdInputADO)icdValue).ICD_CODE;
                    this.IcdName = ((IcdInputADO)icdValue).ICD_NAME;
                }
            }
            if (frmAssignPrescription.ucIcdYHCT != null)
            {
                var icdValue = frmAssignPrescription.icdProcessorYHCT.GetValue(frmAssignPrescription.ucIcdYHCT);
                if (icdValue != null && icdValue is IcdInputADO)
                {
                    this.IcdCodeYHCT = ((IcdInputADO)icdValue).ICD_CODE;
                    this.IcdNameYHCT = ((IcdInputADO)icdValue).ICD_NAME;
                }
            }
            if (frmAssignPrescription.ucIcdCause != null)
            {
                var icdValue = frmAssignPrescription.icdCauseProcessor.GetValue(frmAssignPrescription.ucIcdCause);
                if (icdValue != null && icdValue is IcdInputADO)
                {
                    this.IcdCauseCode = ((IcdInputADO)icdValue).ICD_CODE;
                    this.IcdCauseName = ((IcdInputADO)icdValue).ICD_NAME;
                }
            }
            if (frmAssignPrescription.ucSecondaryIcd != null)
            {
                var subIcd = frmAssignPrescription.subIcdProcessor.GetValue(frmAssignPrescription.ucSecondaryIcd);
                if (subIcd != null && subIcd is SecondaryIcdDataADO)
                {
                    this.IcdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                    this.IcdText = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                }
            }
            if (frmAssignPrescription.ucSecondaryIcdYHCT != null)
            {
                var subIcd = frmAssignPrescription.subIcdProcessorYHCT.GetValue(frmAssignPrescription.ucSecondaryIcdYHCT);
                if (subIcd != null && subIcd is SecondaryIcdDataADO)
                {
                    this.IcdSubCodeYHCT = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                    this.IcdTextYHCT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                }
            }
            if(frmAssignPrescription.TIME_TO != null && frmAssignPrescription.TIME_TO > 0)
            {
                TIME_TO = frmAssignPrescription.TIME_TO;
            }
            if (frmAssignPrescription.USE_TIME != null && frmAssignPrescription.USE_TIME.Count > 0)
            {
                USE_TIMES = frmAssignPrescription.USE_TIME;
            }
            Inventec.Common.Logging.LogSystem.Debug("TIME_TO " + frmAssignPrescription.TIME_TO + " USE_TIMES " + LogUtil.TraceData("frmAssignPrescription.USE_TIME", frmAssignPrescription.USE_TIME));
            if (frmAssignPrescription.treatmentFinishProcessor != null && frmAssignPrescription.ucTreatmentFinish != null)
            {
                var treatDT = frmAssignPrescription.treatmentFinishProcessor.GetDataOutput(frmAssignPrescription.ucTreatmentFinish);
                if (treatDT != null)
                {
                    this.IsAutoTreatmentEnd = treatDT.IsAutoTreatmentFinish;
                    if (treatDT.dtEndTime != null && treatDT.dtEndTime != DateTime.MinValue)
                        this.EndTime = Inventec.Common.TypeConvert.Parse.ToInt64((treatDT.dtEndTime.ToString("yyyyMMddHHmm") + "00").ToString());
                    else
                        this.EndTime = Inventec.Common.TypeConvert.Parse.ToInt64((DateTime.Now.ToString("yyyyMMddHHmm") + "00").ToString());
                    this.TreatmentEndTypeId = treatDT.TreatmentEndTypeId;
                    if (treatDT.dtAppointmentTime != null && treatDT.dtAppointmentTime != DateTime.MinValue)
                        this.AppointmentTime = Inventec.Common.TypeConvert.Parse.ToInt64((treatDT.dtAppointmentTime.ToString("yyyyMMddHHmm") + "00").ToString());
                }
                this.TreatmentFinishSDO = frmAssignPrescription.treatmentFinishProcessor.GetData(frmAssignPrescription.ucTreatmentFinish);
                if (this.TreatmentFinishSDO != null)
                    TreatmentFinishSDO.TreatmentMethod = frmAssignPrescription.TreatmentMethod;

            }

            if (frmAssignPrescription.cboExpMestReason.EditValue != null && frmAssignPrescription.cboExpMestReason.EditValue != "" && frmAssignPrescription.lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
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
            this.ServiceReqMaties = new List<PresOutStockMatySDO>();
            this.ServiceReqMeties = new List<PresOutStockMetySDO>();


            foreach (var item in this.MediMatyTypeADOs)
            {
                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                {
                    PresMedicineSDO pres = new PresMedicineSDO();
                    if (item.MedicineBean1Result != null && item.MedicineBean1Result.Count > 0)
                        pres.MedicineBeanIds = item.MedicineBean1Result.Select(o => o.ID).ToList();
                    else if (item.BeanIds != null && item.BeanIds.Count > 0)
                    {
                        pres.MedicineBeanIds = item.BeanIds;
                    }
                    pres.Amount = item.AMOUNT ?? 0;
                    pres.MedicineTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    if (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value)
                        pres.IsOutParentFee = true;
                    pres.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.NumOfDays = GetNumOfDays(item);
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.SereServParentId = item.SereServParentId;
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;
                    pres.HtuText = item.HTU_TEXT;
                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.OutPatientPresMedicineSDOs
                        .FirstOrDefault(o => o.MedicineTypeId == pres.MedicineTypeId
                        && o.MediStockId == pres.MediStockId
                        && o.PatientTypeId == pres.PatientTypeId
                        && o.IsExpend == pres.IsExpend
                        && o.IsOutParentFee == pres.IsOutParentFee
                        && o.SereServParentId == pres.SereServParentId
                        && o.ExpMestReasonId == pres.ExpMestReasonId
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
                    if (item.MaterialBean1Result != null && item.MaterialBean1Result.Count > 0)
                        pres.MaterialBeanIds = item.MaterialBean1Result.Select(o => o.ID).ToList();
                    else if (item.BeanIds != null && item.BeanIds.Count > 0)
                    {
                        pres.MaterialBeanIds = item.BeanIds;
                    }
                    pres.Amount = item.AMOUNT ?? 0;
                    pres.MaterialTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.Tutorial = item.TUTORIAL;
                    pres.HtuText = item.HTU_TEXT;
                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;
                    pres.SereServParentId = this.SereServParentId;
                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.OutPatientPresMaterialSDOs
                        .FirstOrDefault(o => o.MaterialTypeId == pres.MaterialTypeId
                        && o.MediStockId == pres.MediStockId
                        && o.PatientTypeId == pres.PatientTypeId
                        && o.IsExpend == pres.IsExpend
                        && o.IsOutParentFee == pres.IsOutParentFee
                        && o.EquipmentSetId == pres.EquipmentSetId
                        && o.ExpMestReasonId == pres.ExpMestReasonId
                        );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0 && item.IsStent == false)
                    {
                        checkPresExists.Amount += pres.Amount;
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
                    mety.Amount = item.AMOUNT ?? 0;
                    mety.MedicineTypeId = item.ID;
                    mety.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    mety.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    mety.UnitName = item.SERVICE_UNIT_NAME;
                    mety.NumOrder = item.NUM_ORDER;
                    mety.Tutorial = item.TUTORIAL;
                    mety.UseTimeTo = item.UseTimeTo;
                    //mety.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    //mety.ID = item.SERVICE_REQ_METY_MATY_ID;

                    mety.HtuText = item.HTU_TEXT;
                    var checkPresExists = this.ServiceReqMeties
                        .FirstOrDefault(o => o.MedicineTypeId == mety.MedicineTypeId
                            && o.MedicineUseFormId == mety.MedicineUseFormId
                            && o.ExpMestReasonId == mety.ExpMestReasonId
                        );
                    if (checkPresExists != null && checkPresExists.MedicineTypeId > 0)
                        checkPresExists.Amount += mety.Amount;
                    else
                        this.ServiceReqMeties.Add(mety);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    PresOutStockMatySDO maty = new PresOutStockMatySDO();
                    maty.Amount = item.AMOUNT ?? 0;
                    maty.MaterialTypeId = item.ID;
                    maty.MaterialTypeName = item.MEDICINE_TYPE_NAME;
                    maty.UnitName = item.SERVICE_UNIT_NAME;
                    maty.NumOrder = item.NUM_ORDER;
                    //maty.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    //maty.ID = item.SERVICE_REQ_METY_MATY_ID;

                    maty.Tutorial = item.TUTORIAL;
                    maty.HtuText = item.HTU_TEXT;
                    var checkPresExists = this.ServiceReqMaties
                        .FirstOrDefault(o => o.MaterialTypeId == maty.MaterialTypeId
                            && o.ExpMestReasonId == maty.ExpMestReasonId
                        );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0)
                        checkPresExists.Amount += maty.Amount;
                    else
                        this.ServiceReqMaties.Add(maty);
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
                    //orty.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    orty.UseTimeTo = item.UseTimeTo;

                    orty.HtuText = item.HTU_TEXT;
                    var checkPresExists = this.ServiceReqMeties
                        .FirstOrDefault(o => o.MedicineTypeName == orty.MedicineTypeName
                            && o.MedicineUseFormId == orty.MedicineUseFormId
                            && o.ExpMestReasonId == orty.ExpMestReasonId
                        );
                    if (checkPresExists != null && String.IsNullOrEmpty(checkPresExists.MedicineTypeName))
                        checkPresExists.Amount += orty.Amount;
                    else
                        this.ServiceReqMeties.Add(orty);
                }
            }
        }

        private void InGenerateListMediMaty()
        {
            this.InPatientPresMedicineSDOs = new List<PresMedicineSDO>();
            this.InPatientPresMaterialSDOs = new List<PresMaterialSDO>();
            this.ServiceReqMaties = new List<PresOutStockMatySDO>();
            this.ServiceReqMeties = new List<PresOutStockMetySDO>();

            foreach (var item in this.MediMatyTypeADOs)
            {
                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                {
                    PresMedicineSDO pres = new PresMedicineSDO();
                    pres.InstructionTimes = item.IntructionTimeSelecteds;
                    pres.Amount = item.AMOUNT ?? 0;
                    pres.MedicineTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    if (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value)
                        pres.IsOutParentFee = true;
                    pres.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.NumOfDays = GetNumOfDays(item);
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.SereServParentId = item.SereServParentId;
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;

                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }
                    pres.HtuText = item.HTU_TEXT;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.InPatientPresMedicineSDOs
                        .FirstOrDefault(o => o.MedicineTypeId == pres.MedicineTypeId
                        && o.MediStockId == pres.MediStockId
                        && o.PatientTypeId == pres.PatientTypeId
                        && o.IsExpend == pres.IsExpend
                        && o.IsOutParentFee == pres.IsOutParentFee
                        && o.SereServParentId == pres.SereServParentId
                        && o.ExpMestReasonId == pres.ExpMestReasonId
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
                    pres.Amount = item.AMOUNT ?? 0;
                    pres.MaterialTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.SereServParentId = item.SereServParentId;
                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;

                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    pres.Tutorial = item.TUTORIAL;
                    pres.HtuText = item.HTU_TEXT;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.InPatientPresMaterialSDOs
                        .FirstOrDefault(o => o.MaterialTypeId == pres.MaterialTypeId
                        && o.MediStockId == pres.MediStockId
                        && o.PatientTypeId == pres.PatientTypeId
                        && o.IsExpend == pres.IsExpend
                        && o.IsOutParentFee == pres.IsOutParentFee
                        && o.ExpMestReasonId == pres.ExpMestReasonId
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
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                {
                    PresOutStockMetySDO mety = new PresOutStockMetySDO();
                    mety.InstructionTimes = item.IntructionTimeSelecteds;
                    mety.Amount = item.AMOUNT ?? 0;
                    mety.MedicineTypeId = item.ID;
                    mety.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    mety.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    mety.UnitName = item.SERVICE_UNIT_NAME;
                    mety.NumOrder = item.NUM_ORDER;
                    mety.Tutorial = item.TUTORIAL;
                    mety.UseTimeTo = item.UseTimeTo;

                    mety.HtuText = item.HTU_TEXT;
                    var checkPresExists = this.ServiceReqMeties
                        .FirstOrDefault(o => o.MedicineTypeId == mety.MedicineTypeId
                            && o.MedicineUseFormId == mety.MedicineUseFormId
                            && o.ExpMestReasonId == mety.ExpMestReasonId
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
                        this.ServiceReqMeties.Add(mety);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    PresOutStockMatySDO maty = new PresOutStockMatySDO();
                    maty.InstructionTimes = item.IntructionTimeSelecteds;
                    maty.Amount = item.AMOUNT ?? 0;
                    maty.MaterialTypeId = item.ID;
                    maty.MaterialTypeName = item.MEDICINE_TYPE_NAME;
                    maty.UnitName = item.SERVICE_UNIT_NAME;
                    maty.NumOrder = item.NUM_ORDER;
                    maty.Tutorial = item.TUTORIAL;
                    maty.HtuText = item.HTU_TEXT;
                    var checkPresExists = this.ServiceReqMaties
                        .FirstOrDefault(o => o.MaterialTypeId == maty.MaterialTypeId
                            && o.ExpMestReasonId == maty.ExpMestReasonId
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
                        this.ServiceReqMaties.Add(maty);
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
                    orty.UseTimeTo = item.UseTimeTo;
                    orty.HtuText = item.HTU_TEXT;
                    var checkPresExists = this.ServiceReqMeties
                        .FirstOrDefault(o => o.MedicineTypeName == orty.MedicineTypeName
                            && o.MedicineUseFormId == orty.MedicineUseFormId
                            && o.ExpMestReasonId == orty.ExpMestReasonId
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
                        this.ServiceReqMeties.Add(orty);
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
                            TimeSpan diff__Day = (dtUseTimeTo - dtUseTime);
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
            //Param = (Param ?? new CommonParam());
            bool valid = true;
            try
            {
                //this.frmAssignPrescription.positionHandleControl = -1;
                //valid = valid && (bool)frmAssignPrescription.icdProcessor.ValidationIcd(frmAssignPrescription.ucIcd);
                //valid = valid && (this.frmAssignPrescription.dxValidationProviderControl.Validate());
                valid = valid && this.CheckValidDataInGridService(Param, this.MediMatyTypeADOs);
                valid = valid && this.CheckValidHeinServicePrice(Param, this.MediMatyTypeADOs);
                valid = valid && this.ValidUseAndRemedyCountForm(Param);
                valid = valid && (this.RequestRoomId > 0);
                valid = valid && this.CheckTreatmentFinish();
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

        private bool CheckTreatmentFinish()
        {
            bool result = true;
            try
            {
                if (frmAssignPrescription.treatmentFinishProcessor != null && !frmAssignPrescription.treatmentFinishProcessor.GetValidate(frmAssignPrescription.ucTreatmentFinish))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }



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
                            if ((item.MEDICINE_USE_FORM_ID ?? 0) == 0 && item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (item.DO_NOT_REQUIRED_USE_FORM ?? -1) != RequiredUseFormCFG.DO_NOT_REQUIRED)
                            {
                                messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung, System.Drawing.Color.Maroon));
                                valid = false;
                                Inventec.Common.Logging.LogSystem.Warn("Doi tuong thanh toan bhyt bat buoc phai nhap thong tin duong dung cua thuoc.");
                            }

                            //if (String.IsNullOrEmpty(item.TUTORIAL))
                            //{
                            //    messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, System.Drawing.Color.Maroon));
                            //    valid = false;
                            //    Inventec.Common.Logging.LogSystem.Warn("Doi tuong thanh toan bhyt bat buoc phai nhap thong tin duong dan su dung cua thuoc.");
                            //}
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
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE), this.TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP), this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.PatientTypeAlter.TREATMENT_TYPE_CODE), this.PatientTypeAlter.TREATMENT_TYPE_CODE));

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

        private bool ValidUseAndRemedyCountForm(CommonParam param)
        {
            bool valid = true;
            try
            { //frmAssignPrescription.currentHisPatientTypeAlter.PATIENT_TYPE_ID

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => MediMatyTypeADOs), MediMatyTypeADOs));
                string messageError = "";
                foreach (var item in this.MediMatyTypeADOs)
                {
                    if (
                        item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        && (String.IsNullOrEmpty(item.TUTORIAL) || (item.RemedyCount ?? 0) <= 0)
                        )
                    {
                        messageError += item.MEDICINE_TYPE_NAME + ";";
                        valid = false;
                    }
                }
                if (!valid)
                {
                    param.Messages.Add(String.Format("{0}: {1}", ResourceMessage.CacThuocDaKeThieuThongTinBatBuoc, messageError));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
