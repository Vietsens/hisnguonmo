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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Save
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
        protected string IcdTranditionalName { get; set; }
        protected string IcdTranditionalCode { get; set; }
        protected string IcdTranditionalText { get; set; }
        protected string IcdTranditionalSubCode { get; set; }
        protected long SoNgay { get; set; }
        protected bool IsAutoTreatmentEnd { get; set; }
        protected long EndTime { get; set; }
        protected long? TreatmentEndTypeExtId { get; set; }
        protected long TreatmentEndTypeId { get; set; }
        protected long AppointmentTime { get; set; }
        protected string AdviseFinish { get; set; }
        public long? SurgeryAppointmentTime { get; set; }
        public string AppointmentSurgery { get; set; }
        protected string SickHeinCardNumber { get; set; }
        protected string SickLoginname { get; set; }
        protected string SickUsername { get; set; }
        protected long? SickLeaveTo { get; set; }
        protected long? SickLeaveFrom { get; set; }
        protected decimal? SickLeaveDay { get; set; }
        protected long? SickWorkplaceId { get; set; }
        protected long? NumOrderBlockId { get; set; }
        protected long? DocumentBookId { get; set; }
        protected List<long> AppointmentNextRoomIds { get; set; }
        protected bool IsHomePres { get; set; }
        protected bool IsKidney { get; set; }
        protected long? KidneyTimes { get; set; }
        protected bool IsExecuteKidneyPres { get; set; }
        protected long? DrugStoreId { get; set; }
        protected string ShowIcdCode { get; set; }
        protected string ShowIcdName { get; set; }
        protected string ShowIcdSubCode { get; set; }
        protected string ShowIcdText { get; set; }
        protected bool IsExpXml4210Collinear { get; set; }
        protected long? CareerId { get; set; }
        protected string InteractionReason { get; set; }
        protected long? ExpMestReasonId { get; set; }
        public string EndDeptSubsHeadLoginname { get; set; }
        public string EndDeptSubsHeadUsername { get; set; }
        public string HospSubsDirectorLoginname { get; set; }
        public string HospSubsDirectorUsername { get; set; }
        public string TranPatiHospitalLoginname { get; set; }
        public string TranPatiHospitalUsername { get; set; }
        public long? TranPatiReasonId { get; set; }
        public long? TranPatiFormId { get; set; }
        public long? TranPatiTechId { get; set; }
        public string TransferOutMediOrgCode { get; set; }
        public string TransferOutMediOrgName { get; set; }
        public string ClinicalNote { get; set; }
        public string SubclinicalResult { get; set; }
        public string PatientCondition { get; set; }
        public string TransportVehicle { get; set; }
        public string TransporterLoginnames { get; set; }
        public string Transporter { get; set; }
        public string TreatmentMethod { get; set; }
        public string TreatmentDirection { get; set; }
        public string MainCause { get; set; }
        public string Surgery { get; set; }
        public long? DeathTime { get; set; }
        public short? IsHasAupopsy { get; set; }
        public long? DeathCauseId { get; set; }
        public long? DeathWithinId { get; set; }
        public string EndTypeExtNote { get; set; }
        public string MotherName { get; set; }
        public string FatherName { get; set; }
        public string HospitalizeReasonName { get; set; }
        public string HospitalizeReasonCode { get; set; }
        protected frmAssignPrescription frmAssignPrescription { get; set; }
        protected bool IsMultiDate { get; set; }
        protected List<long> InstructionTimes { get; set; }
        protected List<long> UseTimes { get; set; }
        protected List<PresMedicineSDO> OutPatientPresMedicineSDOs { get; set; }
        protected List<PresMaterialSDO> OutPatientPresMaterialSDOs { get; set; }
        protected List<PresMedicineADO> OutPatientPresMedicineADOs { get; set; }
        protected List<PresMaterialADO> OutPatientPresMaterialADOs { get; set; }
        protected List<PresMedicineSDO> InPatientPresMedicineSDOs { get; set; }
        protected List<PresMaterialSDO> InPatientPresMaterialSDOs { get; set; }
        protected List<PresOutStockMatySDO> PresOutStockMatySDOs { get; set; }
        protected List<PresOutStockMetySDO> PresOutStockMetySDOs { get; set; }

        protected List<PresMaterialBySerialNumberSDO> PatientPresMaterialBySerialNumberSDOs { get; set; }
        protected HisTreatmentFinishSDO TreatmentFinishSDO { get; set; }
        protected long prescriptionTypeId { get; set; }
        protected string ProvisionalDiagnosis { get; set; }
        protected bool CreateOutPatientMediRecord { get; set; }
        protected string StoreCode { get; set; }
        protected long? ProgramId { get; set; }
        protected short? IsTemporaryPres { get; set; }
        protected long? PrescriptionPhaseNum { get; set; }
        protected List<HisGfrAlertLogSDO> AlertLogSDOs { get; set; }
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
            this.IsHomePres = frmAssignPrescription.chkHomePres.Checked;
            this.IsKidney = frmAssignPrescription.chkPreKidneyShift.Checked;
            if (frmAssignPrescription.chkPreKidneyShift.Checked && frmAssignPrescription.spinKidneyCount.Value > 0)
                this.KidneyTimes = (long)frmAssignPrescription.spinKidneyCount.Value;
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
            this.InteractionReason = frmAssignPrescription.txtInteractionReason.Text;

            if (frmAssignPrescription.currentMediStockNhaThuocSelecteds != null && frmAssignPrescription.currentMediStockNhaThuocSelecteds.Count > 0 && frmAssignPrescription.GetSelectedOpionGroup() == 2 && (HisConfigCFG.IsAutoCreateSaleExpMest || HisConfigCFG.IsDrugStoreComboboxOption))
                this.DrugStoreId = (long)frmAssignPrescription.currentMediStockNhaThuocSelecteds.FirstOrDefault().ID;
            this.ProvisionalDiagnosis = frmAssignPrescription.txtProvisionalDiagnosis.Text;
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

            var icdCauseValue = frmAssignPrescription.UcIcdCauseGetValue() as IcdInputADO;
            if (icdCauseValue != null)
            {
                this.IcdCauseCode = icdCauseValue.ICD_CODE;
                this.IcdCauseName = icdCauseValue.ICD_NAME;
            }

            var subIcd = frmAssignPrescription.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
            if (subIcd != null)
            {
                this.IcdSubCode = subIcd.ICD_SUB_CODE;
                this.IcdText = subIcd.ICD_TEXT;
            }
            if (frmAssignPrescription.ucIcdYhct != null)
            {
                var icdTranditional = frmAssignPrescription.icdYhctProcessor.GetValue(frmAssignPrescription.ucIcdYhct);
                if (icdTranditional != null && icdTranditional is IcdInputADO)
                {
                    this.IcdTranditionalCode = ((IcdInputADO)icdTranditional).ICD_CODE;
                    this.IcdTranditionalName = ((IcdInputADO)icdTranditional).ICD_NAME;
                }
            }
            if (frmAssignPrescription.ucSecondaryIcdYhct != null)
            {
                var subIcdTranditional = frmAssignPrescription.subIcdYhctProcessor.GetValue(frmAssignPrescription.ucSecondaryIcdYhct);
                if (subIcdTranditional != null && subIcdTranditional is SecondaryIcdDataADO)
                {
                    this.IcdTranditionalSubCode = ((SecondaryIcdDataADO)subIcdTranditional).ICD_SUB_CODE;
                    this.IcdTranditionalText = ((SecondaryIcdDataADO)subIcdTranditional).ICD_TEXT;
                }
            }

            if (frmAssignPrescription.chkTemporayPres.Checked)
                this.IsTemporaryPres = 1;
            if (frmAssignPrescription.spnPresPhaseNum.EditValue != null && frmAssignPrescription.spnPresPhaseNum.Value > 0)
                this.PrescriptionPhaseNum = (long)frmAssignPrescription.spnPresPhaseNum.Value;
            if (frmAssignPrescription.treatmentFinishProcessor != null && frmAssignPrescription.ucTreatmentFinish != null)
            {
                var treatDT = frmAssignPrescription.treatmentFinishProcessor.GetDataOutput(frmAssignPrescription.ucTreatmentFinish);
                if (treatDT != null)
                {
                    this.IsAutoTreatmentEnd = treatDT.IsAutoTreatmentFinish;
                    if (treatDT.dtEndTime != null && treatDT.dtEndTime != DateTime.MinValue)
                        this.EndTime = Inventec.Common.TypeConvert.Parse.ToInt64((treatDT.dtEndTime.ToString("yyyyMMddHHmmss")).ToString());
                    else
                        this.EndTime = Inventec.Common.TypeConvert.Parse.ToInt64((DateTime.Now.ToString("yyyyMMddHHmmss")).ToString());
                    this.TreatmentEndTypeId = treatDT.TreatmentEndTypeId;
                    this.TreatmentEndTypeExtId = treatDT.TreatmentEndTypeExtId;
                    this.SickHeinCardNumber = treatDT.SickHeinCardNumber;
                    this.SickLoginname = treatDT.SickLoginname;
                    this.SickUsername = treatDT.SickUsername;
                    this.SickLeaveTo = treatDT.SickLeaveTo;
                    this.SickLeaveFrom = treatDT.SickLeaveFrom;
                    this.SickLeaveDay = treatDT.SickLeaveDay;
                    this.SickWorkplaceId = treatDT.SickWorkplaceId;
                    this.NumOrderBlockId = treatDT.NumOrderBlockId;
                    this.DocumentBookId = treatDT.DocumentBookId;
                    if (treatDT.dtAppointmentTime != null && treatDT.dtAppointmentTime != DateTime.MinValue)
                    {
                        this.AppointmentTime = Inventec.Common.TypeConvert.Parse.ToInt64((treatDT.dtAppointmentTime.ToString("yyyyMMddHHmm") + "00").ToString());
                    }
                    this.AdviseFinish = treatDT.Advise;
                    this.SurgeryAppointmentTime = treatDT.SurgeryAppointmentTime;
                    this.AppointmentSurgery = treatDT.AppointmentSurgery;
                    this.AppointmentNextRoomIds = treatDT.AppointmentNextRoomIds;
                    this.CreateOutPatientMediRecord = treatDT.IsIssueOutPatientStoreCode;
                    this.StoreCode = treatDT.StoreCode;
                    this.ProgramId = treatDT.ProgramId;

                    this.ShowIcdCode = treatDT.icdCode;
                    this.ShowIcdName = treatDT.icdName;
                    this.ShowIcdSubCode = treatDT.icdSubCode;
                    this.ShowIcdText = treatDT.icdText;
                    this.IsExpXml4210Collinear = treatDT.IsExpXml4210Collinear;
                    this.CareerId = treatDT.CareeId;
                    this.EndDeptSubsHeadLoginname = treatDT.EndDeptSubsHeadLoginname;
                    this.EndDeptSubsHeadUsername = treatDT.EndDeptSubsHeadUsername;
                    this.HospSubsDirectorLoginname = treatDT.HospSubsDirectorLoginname;
                    this.HospSubsDirectorUsername = treatDT.HospSubsDirectorUsername;
                    this.TranPatiHospitalLoginname = treatDT.TranPatiHospitalLoginname;
                    this.TranPatiHospitalUsername = treatDT.TranPatiHospitalUsername;
                    this.TranPatiReasonId = treatDT.TranPatiReasonId;
                    this.TranPatiFormId = treatDT.TranPatiFormId;
                    this.TranPatiTechId = treatDT.TranPatiTechId;
                    this.TransferOutMediOrgCode = treatDT.TransferOutMediOrgCode;
                    this.TransferOutMediOrgName = treatDT.TransferOutMediOrgName;
                    this.ClinicalNote = treatDT.ClinicalNote;
                    this.SubclinicalResult = treatDT.SubclinicalResult;
                    this.PatientCondition = treatDT.PatientCondition;
                    this.TransportVehicle = treatDT.TransportVehicle;
                    this.TransporterLoginnames = treatDT.TransporterLoginnames;
                    this.Transporter = treatDT.Transporter;
                    this.TreatmentMethod = treatDT.TreatmentMethod;
                    this.TreatmentDirection = treatDT.TreatmentDirection;
                    this.MainCause = treatDT.MainCause;
                    this.Surgery = treatDT.Surgery;
                    this.DeathTime = treatDT.DeathTime;
                    this.IsHasAupopsy = treatDT.IsHasAupopsy;
                    this.DeathCauseId = treatDT.DeathCauseId;
                    this.DeathWithinId = treatDT.DeathWithinId;
                    this.EndTypeExtNote = treatDT.EndTypeExtNote;
                    this.MotherName = treatDT.MotherName;
                    this.FatherName = treatDT.FatherName;
                    this.HospitalizeReasonName = treatDT.HospitalizeReasonName;
                    this.HospitalizeReasonCode = treatDT.HospitalizeReasonCode;

                }
                this.TreatmentFinishSDO = frmAssignPrescription.treatmentFinishProcessor.GetData(frmAssignPrescription.ucTreatmentFinish);
                if (this.TreatmentFinishSDO != null && !string.IsNullOrEmpty(this.TreatmentFinishSDO.TreatmentMethod))
                    TreatmentFinishSDO.TreatmentMethod = frmAssignPrescription.TreatmentMethod;
            }

            this.IsMultiDate = frmAssignPrescription.isMultiDateState;
            if (frmAssignPrescription.cboExpMestReason.EditValue != null && frmAssignPrescription.cboExpMestReason.EditValue != "" && frmAssignPrescription.lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            {
                this.ExpMestReasonId = (long?)frmAssignPrescription.cboExpMestReason.EditValue;
            }
            AutoMapper.Mapper.CreateMap<AlertLogADO, HisGfrAlertLogSDO>();
            AlertLogSDOs = AutoMapper.Mapper.Map<List<HisGfrAlertLogSDO>>(frmAssignPrescription.AlertLogsSdo);
        }

        protected void InitBase()
        {
            if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                this.InGenerateListMediMaty();
            else
                this.OutGenerateListMediMaty();
        }

        long? GetMeMaId(MediMatyTypeADO pres)
        {
            return ((pres.IsAssignPackage.HasValue && pres.IsAssignPackage.Value) ? pres.MAME_ID : null);
        }

        private void ProcessMergeDuplicateData()
        {
            if (this.MediMatyTypeADOs != null && this.MediMatyTypeADOs.Count > 0)
            {
                List<MediMatyTypeADO> mediMatyTypeADOMerges = new List<MediMatyTypeADO>();
                foreach (var pres in this.MediMatyTypeADOs)
                {
                    if (pres.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    {
                        //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                        //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                        var checkPresExists = mediMatyTypeADOMerges
                                                            .FirstOrDefault(o =>
                                                                o.ID == pres.ID
                                                                && ((GetMeMaId(o) == null && GetMeMaId(pres) == null) || (GetMeMaId(o) == GetMeMaId(pres)))
                                                                && o.MEDI_STOCK_ID == pres.MEDI_STOCK_ID
                                                                && o.PATIENT_TYPE_ID == pres.PATIENT_TYPE_ID
                                                                && o.IsExpend == pres.IsExpend
                                                                && o.IsExpendType == pres.IsExpendType
                                                                && o.IsOutParentFee == pres.IsOutParentFee
                                                                && o.SereServParentId == pres.SereServParentId
                                                                && o.MIXED_INFUSION == pres.MIXED_INFUSION
                                                                && o.TUTORIAL == pres.TUTORIAL
                                                                && o.EXP_MEST_REASON_ID == pres.EXP_MEST_REASON_ID
                                                                && o.DataType == pres.DataType
                                                            //&& o.EmbedPatientTypeId == pres.EmbedPatientTypeId
                                                            //&& o.NumOfDays == pres.NumOfDays
                                                            );

                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT += pres.AMOUNT;
                            if (checkPresExists.RAW_AMOUNT.HasValue && pres.RAW_AMOUNT.HasValue)
                                checkPresExists.RAW_AMOUNT += pres.RAW_AMOUNT;
                            if (pres.IntructionTimeSelecteds != null && pres.IntructionTimeSelecteds.Count > 0)
                            {
                                if (checkPresExists.IntructionTimeSelecteds == null) checkPresExists.IntructionTimeSelecteds = new List<long>();
                                checkPresExists.IntructionTimeSelecteds.AddRange(pres.IntructionTimeSelecteds);
                            }
                            if (!string.IsNullOrEmpty(pres.OVER_KIDNEY_REASON))
                                checkPresExists.OVER_KIDNEY_REASON = pres.OVER_KIDNEY_REASON;
                            if (!string.IsNullOrEmpty(pres.OVER_RESULT_TEST_REASON))
                                checkPresExists.OVER_RESULT_TEST_REASON = pres.OVER_RESULT_TEST_REASON;
                            if (checkPresExists.dicTreatmentOverKidneyReason == null || checkPresExists.dicTreatmentOverKidneyReason.Count == 0)
                                checkPresExists.dicTreatmentOverKidneyReason = pres.dicTreatmentOverKidneyReason;
                            else if (pres.dicTreatmentOverKidneyReason != null && pres.dicTreatmentOverKidneyReason.Count > 0)
                            {
                                foreach (var item in pres.dicTreatmentOverKidneyReason)
                                {
                                    if (!checkPresExists.dicTreatmentOverKidneyReason.ContainsKey(item.Key))
                                    {
                                        checkPresExists.dicTreatmentOverKidneyReason[item.Key] = item.Value;
                                    }
                                    else
                                    {
                                        checkPresExists.dicTreatmentOverKidneyReason[item.Key].AddRange(item.Value);
                                    }
                                }
                            }

                            if (checkPresExists.dicTreatmentOverResultTestReason == null || checkPresExists.dicTreatmentOverResultTestReason.Count == 0)
                                checkPresExists.dicTreatmentOverResultTestReason = pres.dicTreatmentOverResultTestReason;
                            else if (pres.dicTreatmentOverResultTestReason != null && pres.dicTreatmentOverResultTestReason.Count > 0)
                            {
                                foreach (var item in pres.dicTreatmentOverResultTestReason)
                                {
                                    if (!checkPresExists.dicTreatmentOverResultTestReason.ContainsKey(item.Key))
                                    {
                                        checkPresExists.dicTreatmentOverResultTestReason[item.Key] = item.Value;
                                    }
                                    else
                                    {
                                        checkPresExists.dicTreatmentOverResultTestReason[item.Key].AddRange(item.Value);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MediMatyTypeADO mediMatyTypeADOAdd = new MediMatyTypeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOAdd, pres);
                            mediMatyTypeADOAdd.dicTreatmentOverKidneyReason = pres.dicTreatmentOverKidneyReason;
                            mediMatyTypeADOAdd.dicTreatmentOverResultTestReason = pres.dicTreatmentOverResultTestReason;
                            mediMatyTypeADOAdd.HTU_IDs = pres.HTU_IDs;
                            mediMatyTypeADOMerges.Add(mediMatyTypeADOAdd);
                        }
                    }
                    else if (pres.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    {
                        //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                        //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                        var checkPresExists = mediMatyTypeADOMerges
                          .FirstOrDefault(o =>
                              o.ID == pres.ID
                              && ((GetMeMaId(o) == null && GetMeMaId(pres) == null) || (GetMeMaId(o) == GetMeMaId(pres)))
                              && o.MEDI_STOCK_ID == pres.MEDI_STOCK_ID
                              && o.PATIENT_TYPE_ID == pres.PATIENT_TYPE_ID
                              && o.IsExpend == pres.IsExpend
                              && o.IsExpendType == pres.IsExpendType
                              && o.IsOutParentFee == pres.IsOutParentFee
                              && o.EXP_MEST_REASON_ID == pres.EXP_MEST_REASON_ID
                              && o.DataType == pres.DataType
                          );

                        if (checkPresExists != null && checkPresExists.ID > 0 && pres.IsStent == false)
                        {
                            checkPresExists.AMOUNT += pres.AMOUNT;
                            if (checkPresExists.RAW_AMOUNT.HasValue && pres.RAW_AMOUNT.HasValue)
                                checkPresExists.RAW_AMOUNT += pres.RAW_AMOUNT;

                            if (pres.IntructionTimeSelecteds != null && pres.IntructionTimeSelecteds.Count > 0)
                            {
                                if (checkPresExists.IntructionTimeSelecteds == null) checkPresExists.IntructionTimeSelecteds = new List<long>();
                                checkPresExists.IntructionTimeSelecteds.AddRange(pres.IntructionTimeSelecteds);
                            }
                        }
                        else
                        {
                            MediMatyTypeADO mediMatyTypeADOAdd = new MediMatyTypeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOAdd, pres);
                            mediMatyTypeADOMerges.Add(mediMatyTypeADOAdd);
                        }
                    }
                    else if (pres.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                    {
                        var checkPresExists = mediMatyTypeADOMerges
                        .FirstOrDefault(o => o.ID == pres.ID
                            && o.MEDICINE_USE_FORM_ID == pres.MEDICINE_USE_FORM_ID
                            && o.TUTORIAL == pres.TUTORIAL
                            && o.EXP_MEST_REASON_ID == pres.EXP_MEST_REASON_ID
                            && o.DataType == pres.DataType
                        );
                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT += pres.AMOUNT;
                            if (pres.IntructionTimeSelecteds != null && pres.IntructionTimeSelecteds.Count > 0)
                            {
                                if (checkPresExists.IntructionTimeSelecteds == null) checkPresExists.IntructionTimeSelecteds = new List<long>();
                                checkPresExists.IntructionTimeSelecteds.AddRange(pres.IntructionTimeSelecteds);
                            }
                            if (!string.IsNullOrEmpty(pres.OVER_KIDNEY_REASON))
                                checkPresExists.OVER_KIDNEY_REASON = pres.OVER_KIDNEY_REASON;
                            if (!string.IsNullOrEmpty(pres.OVER_RESULT_TEST_REASON))
                                checkPresExists.OVER_RESULT_TEST_REASON = pres.OVER_RESULT_TEST_REASON;
                            if (checkPresExists.dicTreatmentOverKidneyReason == null || checkPresExists.dicTreatmentOverKidneyReason.Count == 0)
                                checkPresExists.dicTreatmentOverKidneyReason = pres.dicTreatmentOverKidneyReason;
                            else if (pres.dicTreatmentOverKidneyReason != null && pres.dicTreatmentOverKidneyReason.Count > 0)
                            {
                                foreach (var item in pres.dicTreatmentOverKidneyReason)
                                {
                                    if (!checkPresExists.dicTreatmentOverKidneyReason.ContainsKey(item.Key))
                                    {
                                        checkPresExists.dicTreatmentOverKidneyReason[item.Key] = item.Value;
                                    }
                                    else
                                    {
                                        checkPresExists.dicTreatmentOverKidneyReason[item.Key].AddRange(item.Value);
                                    }
                                }
                            }

                            if (checkPresExists.dicTreatmentOverResultTestReason == null || checkPresExists.dicTreatmentOverResultTestReason.Count == 0)
                                checkPresExists.dicTreatmentOverResultTestReason = pres.dicTreatmentOverResultTestReason;
                            else if (pres.dicTreatmentOverResultTestReason != null && pres.dicTreatmentOverResultTestReason.Count > 0)
                            {
                                foreach (var item in pres.dicTreatmentOverResultTestReason)
                                {
                                    if (!checkPresExists.dicTreatmentOverResultTestReason.ContainsKey(item.Key))
                                    {
                                        checkPresExists.dicTreatmentOverResultTestReason[item.Key] = item.Value;
                                    }
                                    else
                                    {
                                        checkPresExists.dicTreatmentOverResultTestReason[item.Key].AddRange(item.Value);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MediMatyTypeADO mediMatyTypeADOAdd = new MediMatyTypeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOAdd, pres);
                            mediMatyTypeADOAdd.dicTreatmentOverKidneyReason = pres.dicTreatmentOverKidneyReason;
                            mediMatyTypeADOAdd.dicTreatmentOverResultTestReason = pres.dicTreatmentOverResultTestReason;
                            mediMatyTypeADOAdd.HTU_IDs = pres.HTU_IDs;
                            mediMatyTypeADOMerges.Add(mediMatyTypeADOAdd);
                        }
                    }
                    else if (pres.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                    {
                        var checkPresExists = mediMatyTypeADOMerges
                       .FirstOrDefault(o => o.ID == pres.ID
                           && o.TUTORIAL == pres.TUTORIAL
                           && o.EXP_MEST_REASON_ID == pres.EXP_MEST_REASON_ID
                           && o.DataType == pres.DataType
                       );
                        if (checkPresExists != null && checkPresExists.ID > 0)
                        {
                            checkPresExists.AMOUNT += pres.AMOUNT;
                            if (pres.IntructionTimeSelecteds != null && pres.IntructionTimeSelecteds.Count > 0)
                            {
                                if (checkPresExists.IntructionTimeSelecteds == null) checkPresExists.IntructionTimeSelecteds = new List<long>();
                                checkPresExists.IntructionTimeSelecteds.AddRange(pres.IntructionTimeSelecteds);
                            }
                        }
                        else
                        {
                            MediMatyTypeADO mediMatyTypeADOAdd = new MediMatyTypeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOAdd, pres);
                            mediMatyTypeADOMerges.Add(mediMatyTypeADOAdd);
                        }
                    }
                    else if (pres.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        var checkPresExists = mediMatyTypeADOMerges
                        .FirstOrDefault(o => o.MEDICINE_TYPE_NAME == pres.MEDICINE_TYPE_NAME
                            && o.MEDICINE_USE_FORM_ID == pres.MEDICINE_USE_FORM_ID
                            && o.TUTORIAL == pres.TUTORIAL
                            && o.EXP_MEST_REASON_ID == pres.EXP_MEST_REASON_ID
                            && o.DataType == pres.DataType
                        );
                        if (checkPresExists != null && String.IsNullOrEmpty(checkPresExists.MEDICINE_TYPE_NAME))
                        {
                            checkPresExists.AMOUNT += pres.AMOUNT;
                            if (pres.IntructionTimeSelecteds != null && pres.IntructionTimeSelecteds.Count > 0)
                            {
                                if (checkPresExists.IntructionTimeSelecteds == null) checkPresExists.IntructionTimeSelecteds = new List<long>();
                                checkPresExists.IntructionTimeSelecteds.AddRange(pres.IntructionTimeSelecteds);
                            }
                        }
                        else
                        {
                            MediMatyTypeADO mediMatyTypeADOAdd = new MediMatyTypeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOAdd, pres);
                            mediMatyTypeADOAdd.dicTreatmentOverKidneyReason = pres.dicTreatmentOverKidneyReason;
                            mediMatyTypeADOAdd.dicTreatmentOverResultTestReason = pres.dicTreatmentOverResultTestReason;
                            mediMatyTypeADOAdd.HTU_IDs = pres.HTU_IDs;
                            mediMatyTypeADOMerges.Add(mediMatyTypeADOAdd);
                        }
                    }
                    else
                    {
                        MediMatyTypeADO mediMatyTypeADOAdd = new MediMatyTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOAdd, pres);
                        mediMatyTypeADOAdd.HTU_IDs = pres.HTU_IDs;
                        mediMatyTypeADOMerges.Add(mediMatyTypeADOAdd);
                    }
                }

                this.MediMatyTypeADOs = new List<MediMatyTypeADO>();
                this.MediMatyTypeADOs.AddRange(mediMatyTypeADOMerges);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOMerges), mediMatyTypeADOMerges)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => MediMatyTypeADOs), MediMatyTypeADOs));
            }
        }

        private void OutGenerateListMediMaty()
        {
            //this.OutPatientPresMedicineSDOs = new List<PresMedicineSDO>();
            this.OutPatientPresMedicineADOs = new List<PresMedicineADO>();
            //this.OutPatientPresMaterialSDOs = new List<PresMaterialSDO>();
            this.OutPatientPresMaterialADOs = new List<PresMaterialADO>();
            this.PresOutStockMatySDOs = new List<PresOutStockMatySDO>();
            this.PresOutStockMetySDOs = new List<PresOutStockMetySDO>();

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.lstOutPatientPres), frmAssignPrescription.lstOutPatientPres));

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => MediMatyTypeADOs.Select(o => o.DataType).ToList()), MediMatyTypeADOs.Select(o => o.DataType).ToList()));
            foreach (var item in this.MediMatyTypeADOs)
            {
                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC ||
                    item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU ||
                    item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                {
                    if (frmAssignPrescription.UseTimeSelecteds != null && frmAssignPrescription.UseTimeSelecteds.Count > 0)
                    {
                        foreach (var ut in frmAssignPrescription.UseTimeSelecteds)
                        {
                            var ado = frmAssignPrescription.lstOutPatientPres.FirstOrDefault(o => o.PrimaryKey == item.PrimaryKey && o.MEDI_MATE_ID == item.ID && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID && o.USE_TIME == ut);
                            if (frmAssignPrescription.actionType != GlobalVariables.ActionEdit)
                            {
                                if (ado != null)
                                    ProcessOutPatientPresMedicineSDOs(item, ado.TAKE_BEAN_ID, ut);
                                else
                                    ProcessOutPatientPresMedicineSDOs(item, null, ut);
                            }
                            else if (frmAssignPrescription.actionType == GlobalVariables.ActionEdit)
                            {
                                List<long> lstIds = new List<long>();
                                if (item.MedicineBean1Result != null || item.MaterialBean1Result != null)
                                    lstIds = item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC ? item.MedicineBean1Result.Select(o => o.ID).ToList() : item.MaterialBean1Result.Select(o => o.ID).ToList();

                                ProcessOutPatientPresMedicineSDOs(item, ado != null ? ado.TAKE_BEAN_ID : lstIds, ut);
                            }
                        }
                    }
                    else
                    {
                        List<long> lstIds = null;
                        if (frmAssignPrescription.lstOutPatientPres != null && frmAssignPrescription.lstOutPatientPres.Count > 0 && frmAssignPrescription.actionType != GlobalVariables.ActionEdit)
                            lstIds = frmAssignPrescription.lstOutPatientPres.FirstOrDefault(o => o.PrimaryKey == item.PrimaryKey && o.MEDI_MATE_ID == item.ID && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID && o.USE_TIME == 0) != null ? frmAssignPrescription.lstOutPatientPres.FirstOrDefault(o => o.PrimaryKey == item.PrimaryKey && o.MEDI_MATE_ID == item.ID && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID && o.USE_TIME == 0).TAKE_BEAN_ID : null;
                        else if (frmAssignPrescription.actionType == GlobalVariables.ActionEdit && (item.MedicineBean1Result != null || item.MaterialBean1Result != null))
                            lstIds = item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC ? item.MedicineBean1Result.Select(o => o.ID).ToList() : item.MaterialBean1Result.Select(o => o.ID).ToList();
                        ProcessOutPatientPresMedicineSDOs(item, lstIds);

                    }
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)
                {
                    PresOutStockMetySDO mety = new PresOutStockMetySDO();
                    mety.MedicineInfoSdos = new List<MedicineInfoSDO>();
                    if (frmAssignPrescription.IsSaveOverResultReasonTest)
                    {
                        foreach (var intructionTime in frmAssignPrescription.intructionTimeSelecteds)
                        {
                            if (item.dicTreatmentOverResultTestReason != null && item.dicTreatmentOverResultTestReason.Count > 0 && item.dicTreatmentOverResultTestReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverResultTestReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverResultTestReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                            }
                            if (item.dicTreatmentOverKidneyReason != null && item.dicTreatmentOverKidneyReason.Count > 0 && item.dicTreatmentOverKidneyReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverKidneyReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverKidneyReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverKidneyReason = item.OVER_KIDNEY_REASON });
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                            mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                        if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                            mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_KIDNEY_REASON });
                    }
                    mety.HtuText = item.HTU_TEXT;
                    mety.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    mety.PresAmount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.PRES_AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.PRES_AMOUNT ?? 0);
                    mety.MedicineTypeId = item.ID;
                    mety.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    mety.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    mety.UnitName = item.SERVICE_UNIT_NAME;
                    mety.NumOrder = item.NUM_ORDER;
                    mety.Tutorial = item.TUTORIAL;
                    mety.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    mety.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if (item.ALERT_MAX_IN_TREATMENT.HasValue && item.IsAlertInTreatPresciption)
                        mety.ExceedLimitInTreatmentReason = item.EXCEED_LIMIT_IN_BATCH_REASON;

                    mety.OddPresReason = item.ODD_PRES_REASON;
                    mety.UseTimeTo = item.UseTimeTo;
                    var sangTemp = frmAssignPrescription.GetValueSpinNew(item.Sang);
                    if (!String.IsNullOrEmpty(sangTemp))
                        mety.Morning = sangTemp;
                    var truaTemp = frmAssignPrescription.GetValueSpinNew(item.Trua);
                    if (!String.IsNullOrEmpty(truaTemp))
                        mety.Noon = truaTemp;
                    var chieuTemp = frmAssignPrescription.GetValueSpinNew(item.Chieu);
                    if (!String.IsNullOrEmpty(chieuTemp))
                        mety.Afternoon = chieuTemp;
                    var toiTemp = frmAssignPrescription.GetValueSpinNew(item.Toi);
                    if (!String.IsNullOrEmpty(toiTemp))
                        mety.Evening = toiTemp;
                    mety.HtuId = item.HTU_IDs != null && item.HTU_IDs.Count > 0 ? (long?)item.HTU_IDs[0] : null; // 
                    mety.Price = item.PRICE ?? 0;
                    //if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                    //    mety.ServiceConditionId = item.SERVICE_CONDITION_ID;
                    mety.PreviousUsingCount = item.PREVIOUS_USING_COUNT;
                    if (this.ExpMestReasonId == null)
                    {
                        mety.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        mety.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    var checkPresExists = this.PresOutStockMetySDOs
                        .FirstOrDefault(o => o.MedicineTypeId == mety.MedicineTypeId
                            && o.MedicineUseFormId == mety.MedicineUseFormId
                            && o.Tutorial == mety.Tutorial
                            && o.ExpMestReasonId == mety.ExpMestReasonId
                        );
                    if (checkPresExists != null && checkPresExists.MedicineTypeId > 0)
                    {
                        checkPresExists.Amount += mety.Amount;
                        if (mety.MedicineInfoSdos != null && mety.MedicineInfoSdos.Count > 0)
                        {
                            checkPresExists.MedicineInfoSdos = mety.MedicineInfoSdos;
                        }
                    }
                    else
                        this.PresOutStockMetySDOs.Add(mety);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    PresOutStockMatySDO maty = new PresOutStockMatySDO();
                    maty.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    maty.PresAmount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.PRES_AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.PRES_AMOUNT ?? 0);
                    maty.MaterialTypeId = item.ID;
                    maty.Tutorial = item.TUTORIAL;
                    maty.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    maty.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    maty.MaterialTypeName = item.MEDICINE_TYPE_NAME;
                    maty.UnitName = item.SERVICE_UNIT_NAME;
                    maty.NumOrder = item.NUM_ORDER;
                    //maty.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    //maty.ID = item.SERVICE_REQ_METY_MATY_ID;
                    maty.Price = item.PRICE ?? 0;
                    //if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                    //    maty.ServiceConditionId = item.SERVICE_CONDITION_ID;

                    maty.HtuText = item.HTU_TEXT;
                    if (this.ExpMestReasonId == null)
                    {
                        maty.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        maty.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    var checkPresExists = this.PresOutStockMatySDOs
                        .FirstOrDefault(o => o.MaterialTypeId == maty.MaterialTypeId
                            && o.ExpMestReasonId == maty.ExpMestReasonId
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
                    orty.PresAmount = item.PRES_AMOUNT;
                    orty.NumOrder = item.NUM_ORDER;
                    orty.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    orty.UnitName = item.SERVICE_UNIT_NAME;
                    orty.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    orty.Tutorial = item.TUTORIAL;
                    orty.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    orty.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if (item.ALERT_MAX_IN_TREATMENT.HasValue && item.IsAlertInTreatPresciption)
                        orty.ExceedLimitInTreatmentReason = item.EXCEED_LIMIT_IN_BATCH_REASON;
                    orty.HtuText = item.HTU_TEXT;
                    orty.OddPresReason = item.ODD_PRES_REASON;
                    var sangTemp = frmAssignPrescription.GetValueSpinNew(item.Sang);
                    if (!String.IsNullOrEmpty(sangTemp))
                        orty.Morning = sangTemp;
                    var truaTemp = frmAssignPrescription.GetValueSpinNew(item.Trua);
                    if (!String.IsNullOrEmpty(truaTemp))
                        orty.Noon = truaTemp;
                    var chieuTemp = frmAssignPrescription.GetValueSpinNew(item.Chieu);
                    if (!String.IsNullOrEmpty(chieuTemp))
                        orty.Afternoon = chieuTemp;
                    var toiTemp = frmAssignPrescription.GetValueSpinNew(item.Toi);
                    if (!String.IsNullOrEmpty(toiTemp))
                        orty.Evening = toiTemp;
                    orty.HtuId = item.HTU_IDs != null && item.HTU_IDs.Count > 0 ? (long?)item.HTU_IDs[0] : null; // 
                    //orty.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    orty.UseTimeTo = item.UseTimeTo;
                    orty.Price = item.PRICE ?? 0;

                    if (this.ExpMestReasonId == null)
                    {
                        orty.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        orty.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    orty.PreviousUsingCount = item.PREVIOUS_USING_COUNT;
                    var checkPresExists = this.PresOutStockMetySDOs
                        .FirstOrDefault(o => o.MedicineTypeName == orty.MedicineTypeName
                            && o.MedicineUseFormId == orty.MedicineUseFormId
                            && o.Tutorial == orty.Tutorial
                            && o.ExpMestReasonId == orty.ExpMestReasonId
                        );
                    if (checkPresExists != null && String.IsNullOrEmpty(checkPresExists.MedicineTypeName))
                        checkPresExists.Amount += orty.Amount;
                    else
                        this.PresOutStockMetySDOs.Add(orty);
                }
            }
        }

        private void ProcessOutPatientPresMedicineSDOs(MediMatyTypeADO item, List<long> beanIds, long useTime = 0)
        {
            try
            {
                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                {
                    PresMedicineADO pres = new PresMedicineADO();
                    pres.MedicineInfoSdos = new List<MedicineInfoSDO>();
                    if (frmAssignPrescription.IsSaveOverResultReasonTest)
                    {
                        foreach (var intructionTime in frmAssignPrescription.intructionTimeSelecteds)
                        {
                            if (item.dicTreatmentOverResultTestReason != null && item.dicTreatmentOverResultTestReason.Count > 0 && item.dicTreatmentOverResultTestReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverResultTestReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverResultTestReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                            }
                            if (item.dicTreatmentOverKidneyReason != null && item.dicTreatmentOverKidneyReason.Count > 0 && item.dicTreatmentOverKidneyReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverKidneyReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverKidneyReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverKidneyReason = item.OVER_KIDNEY_REASON });
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                            pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                        if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                            pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverKidneyReason = item.OVER_KIDNEY_REASON });
                    }
                    pres.HtuText = item.HTU_TEXT;
                    pres.UseOriginalUnitForPres = (item.IsUseOrginalUnitForPres ?? false);
                    pres.MedicineId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    if (item.IS_SUB_PRES != 1)
                    {
                        if (item.MedicineBean1Result != null && item.MedicineBean1Result.Count > 0)
                            pres.MedicineBeanIds = beanIds;
                        else if (item.BeanIds != null && item.BeanIds.Count > 0)
                        {
                            pres.MedicineBeanIds = item.BeanIds;
                        }
                    }
                    else
                    {
                        pres.MedicineBeanIds = null;
                    }
                    #region
                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.PresAmount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.PRES_AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.PRES_AMOUNT ?? 0);
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
                    pres.IsBedExpend = item.IsExpendType;
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;
                    var sangTemp = frmAssignPrescription.GetValueSpinNew(item.Sang);
                    if (!String.IsNullOrEmpty(sangTemp))
                        pres.Morning = sangTemp;
                    var truaTemp = frmAssignPrescription.GetValueSpinNew(item.Trua);
                    if (!String.IsNullOrEmpty(truaTemp))
                        pres.Noon = truaTemp;
                    var chieuTemp = frmAssignPrescription.GetValueSpinNew(item.Chieu);
                    if (!String.IsNullOrEmpty(chieuTemp))
                        pres.Afternoon = chieuTemp;
                    var toiTemp = frmAssignPrescription.GetValueSpinNew(item.Toi);
                    if (!String.IsNullOrEmpty(toiTemp))
                        pres.Evening = toiTemp;
                    var breathSpeedTemp = frmAssignPrescription.GetValueSpinNew(item.BREATH_SPEED);
                    if (!String.IsNullOrEmpty(breathSpeedTemp))
                        pres.BreathSpeed = breathSpeedTemp;
                    var breathTimeTemp = frmAssignPrescription.GetValueSpinNew(item.BREATH_TIME);
                    if (!String.IsNullOrEmpty(breathTimeTemp))
                        pres.BreathTime = breathTimeTemp;
                    pres.HtuId = item.HTU_ID_NOT_CHECK_ACIN_INTERACTIVE;
                    if (item.HTU_IDs != null && item.HTU_IDs.Count > 0)
                        pres.HtuIds = string.Join(",", item.HTU_IDs);

                    pres.PreviousUsingCount = item.PREVIOUS_USING_COUNT;
                    if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                        pres.ServiceConditionId = item.SERVICE_CONDITION_ID;
                    pres.SereServParentId = item.SereServParentId;

                    pres.MixedInfusion = item.MIXED_INFUSION;
                    pres.IsMixedMain = item.IS_MIXED_MAIN;
                    pres.TutorialInfusion = item.TUTORIAL_INFUSION;
                    pres.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    pres.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if (item.ALERT_MAX_IN_TREATMENT.HasValue && item.IsAlertInTreatPresciption)
                        pres.ExceedLimitInTreatmentReason = item.EXCEED_LIMIT_IN_BATCH_REASON;
                    pres.OddPresReason = item.ODD_PRES_REASON;

                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    pres.useTime = useTime;

                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = OutPatientPresMedicineADOs
                            .FirstOrDefault(
                            o => o.MedicineTypeId == pres.MedicineTypeId
                                && ((o.MedicineId == null && pres.MedicineId == null) || (o.MedicineId == pres.MedicineId))
                                && o.MediStockId == pres.MediStockId
                                && o.PatientTypeId == pres.PatientTypeId
                                && o.IsExpend == pres.IsExpend
                                && o.IsBedExpend == pres.IsBedExpend
                                && o.IsOutParentFee == pres.IsOutParentFee
                                && o.SereServParentId == pres.SereServParentId
                                && o.MixedInfusion == pres.MixedInfusion
                                && o.Tutorial == pres.Tutorial
                                && o.ExpMestReasonId == pres.ExpMestReasonId
                                && o.useTime == useTime
                            );
                    #endregion
                    if (checkPresExists != null && checkPresExists.MedicineTypeId > 0)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.MedicineBeanIds != null && pres.MedicineBeanIds.Count > 0)
                        {
                            if (checkPresExists.MedicineBeanIds == null) checkPresExists.MedicineBeanIds = new List<long>();
                            checkPresExists.MedicineBeanIds.AddRange(pres.MedicineBeanIds);

                            checkPresExists.MedicineBeanIds = checkPresExists.MedicineBeanIds.Distinct().ToList();
                        }
                        if (pres.MedicineInfoSdos != null && pres.MedicineInfoSdos.Count > 0)
                        {
                            checkPresExists.MedicineInfoSdos = pres.MedicineInfoSdos;
                        }
                    }
                    else
                    {
                        OutPatientPresMedicineADOs.Add(pres);
                    }

                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                {
                    PresMaterialADO pres = new PresMaterialADO();
                    pres.MaterialId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    if (item.IS_SUB_PRES != 1)
                    {
                        if (item.MaterialBean1Result != null && item.MaterialBean1Result.Count > 0)
                            pres.MaterialBeanIds = beanIds;
                        else if (item.BeanIds != null && item.BeanIds.Count > 0)
                        {
                            pres.MaterialBeanIds = item.BeanIds;
                        }
                    }
                    else
                    {
                        pres.MaterialBeanIds = null;
                    }

                    pres.HtuText = item.HTU_TEXT;
                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.PresAmount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.PRES_AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.PRES_AMOUNT ?? 0);
                    pres.MaterialTypeId = item.ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.IsBedExpend = item.IsExpendType;
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    pres.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;
                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }
                    pres.SereServParentId = item.SereServParentId;
                    if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                        pres.ServiceConditionId = item.SERVICE_CONDITION_ID;

                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }


                    pres.useTime = useTime;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.OutPatientPresMaterialADOs
                       .FirstOrDefault(o =>
                           o.MaterialTypeId == pres.MaterialTypeId
                           && ((o.MaterialId == null && pres.MaterialId == null) || (o.MaterialId == pres.MaterialId))
                           && o.MediStockId == pres.MediStockId
                           && o.PatientTypeId == pres.PatientTypeId
                           && o.IsExpend == pres.IsExpend
                           && o.IsBedExpend == pres.IsBedExpend
                           && o.IsOutParentFee == pres.IsOutParentFee
                           && o.EquipmentSetId == pres.EquipmentSetId
                           && o.ExpMestReasonId == pres.ExpMestReasonId
                           && o.useTime == useTime
                       );

                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0 && item.IsStent == false)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.MaterialBeanIds != null && pres.MaterialBeanIds.Count > 0)
                        {
                            if (checkPresExists.MaterialBeanIds == null) checkPresExists.MaterialBeanIds = new List<long>();
                            checkPresExists.MaterialBeanIds.AddRange(pres.MaterialBeanIds);
                            checkPresExists.MaterialBeanIds = checkPresExists.MaterialBeanIds.Distinct().ToList();
                        }
                    }
                    else
                        this.OutPatientPresMaterialADOs.Add(pres);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                {
                    PresMaterialADO pres = new PresMaterialADO();
                    if (item.IS_SUB_PRES != 1)
                    {
                        if (item.MaterialBean1Result != null && item.MaterialBean1Result.Count > 0)
                            pres.MaterialBeanIds = beanIds;
                        else if (item.BeanIds != null && item.BeanIds.Count > 0)
                        {
                            pres.MaterialBeanIds = item.BeanIds;
                        }
                    }
                    else
                    {
                        pres.MaterialBeanIds = null;
                    }

                    pres.Amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.AMOUNT ?? 0);
                    pres.PresAmount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.PRES_AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.PRES_AMOUNT ?? 0);
                    pres.MaterialTypeId = item.ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.IsBedExpend = item.IsExpendType;
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    pres.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }
                    pres.SereServParentId = item.SereServParentId;
                    if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                        pres.ServiceConditionId = item.SERVICE_CONDITION_ID;
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

                    pres.useTime = useTime;
                    pres.SerialNumber = item.SERIAL_NUMBER;
                    //đối tượng thanh toán, hao phí... khiến cho 2 dòng có cùng loại thuốc nhưng các trường thông tin khác không giống nhau hoàn toàn thì vẫn gửi lên server như bình thường hiện tại.
                    //Còn trường hợp 2 dòng giống hệt nhau thì client khi gửi lên server nên gộp lại thành 1 dòng số lượng.
                    var checkPresExists = this.OutPatientPresMaterialADOs
                    .FirstOrDefault(o => o.MaterialTypeId == pres.MaterialTypeId
                    && o.MediStockId == pres.MediStockId
                    && o.PatientTypeId == pres.PatientTypeId
                    && o.IsExpend == pres.IsExpend
                    && o.IsBedExpend == pres.IsBedExpend
                    && o.IsOutParentFee == pres.IsOutParentFee
                    && o.EquipmentSetId == pres.EquipmentSetId
                    && o.ExpMestReasonId == pres.ExpMestReasonId
                    && o.useTime == useTime
                    && o.SerialNumber == pres.SerialNumber
                    );
                    if (checkPresExists != null && checkPresExists.MaterialTypeId > 0 && item.IsStent == false)
                    {
                        checkPresExists.Amount += pres.Amount;
                        if (pres.MaterialBeanIds != null && pres.MaterialBeanIds.Count > 0)
                        {
                            if (checkPresExists.MaterialBeanIds == null) checkPresExists.MaterialBeanIds = new List<long>();
                            checkPresExists.MaterialBeanIds.AddRange(pres.MaterialBeanIds);

                            checkPresExists.MaterialBeanIds = checkPresExists.MaterialBeanIds.Distinct().ToList();
                        }
                    }
                    else
                        this.OutPatientPresMaterialADOs.Add(pres);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InGenerateListMediMaty()
        {
            this.InPatientPresMedicineSDOs = new List<PresMedicineSDO>();
            this.InPatientPresMaterialSDOs = new List<PresMaterialSDO>();
            this.PresOutStockMatySDOs = new List<PresOutStockMatySDO>();
            this.PresOutStockMetySDOs = new List<PresOutStockMetySDO>();
            this.PatientPresMaterialBySerialNumberSDOs = new List<PresMaterialBySerialNumberSDO>();

            this.ProcessMergeDuplicateData();

            foreach (var item in this.MediMatyTypeADOs)
            {
                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                {
                    PresMedicineSDO pres = new PresMedicineSDO();
                    pres.InstructionTimes = item.IntructionTimeSelecteds;
                    pres.MedicineInfoSdos = new List<MedicineInfoSDO>();
                    if (frmAssignPrescription.IsSaveOverResultReasonTest)
                    {
                        foreach (var intructionTime in frmAssignPrescription.intructionTimeSelecteds)
                        {
                            if (item.dicTreatmentOverResultTestReason != null && item.dicTreatmentOverResultTestReason.Count > 0 && item.dicTreatmentOverResultTestReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverResultTestReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverResultTestReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                            }
                            if (item.dicTreatmentOverKidneyReason != null && item.dicTreatmentOverKidneyReason.Count > 0 && item.dicTreatmentOverKidneyReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverKidneyReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverKidneyReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                                    pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverKidneyReason = item.OVER_KIDNEY_REASON });
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                            pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                        if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                            pres.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverKidneyReason = item.OVER_KIDNEY_REASON });
                    }
                    pres.UseOriginalUnitForPres = (item.IsUseOrginalUnitForPres ?? false);
                    pres.MedicineId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    pres.Amount = item.CalculateAmount(item, this.ActionType);
                    pres.PresAmount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.PRES_AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1) : (item.PRES_AMOUNT ?? 0);
                    pres.MedicineTypeId = item.ID;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    if (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value)
                        pres.IsOutParentFee = true;
                    pres.IsBedExpend = item.IsExpendType;
                    pres.Speed = item.Speed;
                    pres.HtuText = item.HTU_TEXT;
                    var sangTemp = frmAssignPrescription.GetValueSpinNew(item.Sang);
                    if (!String.IsNullOrEmpty(sangTemp))
                        pres.Morning = sangTemp;
                    var truaTemp = frmAssignPrescription.GetValueSpinNew(item.Trua);
                    if (!String.IsNullOrEmpty(truaTemp))
                        pres.Noon = truaTemp;
                    var chieuTemp = frmAssignPrescription.GetValueSpinNew(item.Chieu);
                    if (!String.IsNullOrEmpty(chieuTemp))
                        pres.Afternoon = chieuTemp;
                    var toiTemp = frmAssignPrescription.GetValueSpinNew(item.Toi);
                    if (!String.IsNullOrEmpty(toiTemp))
                        pres.Evening = toiTemp;
                    var breathSpeedTemp = frmAssignPrescription.GetValueSpinNew(item.BREATH_SPEED);
                    if (!String.IsNullOrEmpty(breathSpeedTemp))
                        pres.BreathSpeed = breathSpeedTemp;
                    var breathTimeTemp = frmAssignPrescription.GetValueSpinNew(item.BREATH_TIME);
                    if (!String.IsNullOrEmpty(breathTimeTemp))
                        pres.BreathTime = breathTimeTemp;
                    pres.HtuId = item.HTU_ID_NOT_CHECK_ACIN_INTERACTIVE;
                    if (item.HTU_IDs != null && item.HTU_IDs.Count > 0)
                        pres.HtuIds = string.Join(",", item.HTU_IDs);
                    pres.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.NumOfDays = GetNumOfDays(item);
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.SereServParentId = item.SereServParentId;
                    pres.PreviousUsingCount = item.PREVIOUS_USING_COUNT;
                    if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                        pres.ServiceConditionId = item.SERVICE_CONDITION_ID;
                    if (item.IsKidneyShift.HasValue && item.IsKidneyShift.Value)
                    {
                        //pres.IsKidneyShift = item.IsKidneyShift;//TODO
                        //pres.KidneyShiftCount = item.KidneyShiftCount;//TODO
                    }
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;

                    pres.MixedInfusion = item.MIXED_INFUSION;
                    pres.IsMixedMain = item.IS_MIXED_MAIN;
                    pres.TutorialInfusion = item.TUTORIAL_INFUSION;
                    pres.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    pres.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if ((item.ALERT_MAX_IN_TREATMENT.HasValue && item.IsAlertInTreatPresciption) && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment != null && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.Count > 0 && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.Exists(o => o.PrimaryKey == item.PrimaryKey && o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[1] == TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE))
                    {
                        pres.ExceedLimitInTreatmentReason = frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.LastOrDefault(o => o.PrimaryKey == item.PrimaryKey && o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[1] == TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE).EXCEED_LIMIT_IN_BATCH_REASON;
                    }


                    pres.OddPresReason = item.ODD_PRES_REASON;

                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    this.InPatientPresMedicineSDOs.Add(pres);

                    if (
                        (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (item.IS_SPLIT_COMPENSATION.HasValue && item.IS_SPLIT_COMPENSATION == 1))
                        && ((item.IS_ALLOW_ODD ?? 0) != 1)
                        && (item.RAW_AMOUNT ?? 0) > 0
                        && (item.RAW_AMOUNT != (int)item.RAW_AMOUNT))
                    {
                        PresMedicineSDO presMedicinePlus = new PresMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<PresMedicineSDO>(presMedicinePlus, pres);
                        presMedicinePlus.InstructionTimes = item.IntructionTimeSelecteds;
                        presMedicinePlus.Amount = item.CalculateAmountNoPres(item, this.ActionType);
                        presMedicinePlus.IsNotPres = true;

                        if (presMedicinePlus.Amount > 0)
                            this.InPatientPresMedicineSDOs.Add(presMedicinePlus);
                    }
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                {
                    PresMaterialSDO pres = new PresMaterialSDO();
                    pres.InstructionTimes = item.IntructionTimeSelecteds;
                    pres.MaterialId = ((item.IsAssignPackage.HasValue && item.IsAssignPackage.Value) ? item.MAME_ID : null);
                    pres.Amount = new MediMatyTypeADO().CalculateAmount(item, this.ActionType);
                    pres.PresAmount = item.PRES_AMOUNT;
                    pres.MaterialTypeId = item.ID;
                    pres.Tutorial = item.TUTORIAL;
                    pres.PatientTypeId = item.PATIENT_TYPE_ID ?? 0;
                    pres.IsExpend = item.IsExpend;
                    pres.IsOutParentFee = (item.IsOutKtcFee.HasValue && item.IsOutKtcFee.Value);
                    pres.IsBedExpend = item.IsExpendType;
                    pres.NumOrder = item.NUM_ORDER;
                    pres.MediStockId = item.MEDI_STOCK_ID ?? 0;
                    pres.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    pres.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    pres.SereServParentId = item.SereServParentId;
                    if (this.ExpMestReasonId == null)
                    {
                        pres.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        pres.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    pres.HtuText = item.HTU_TEXT;
                    if (item.EQUIPMENT_SET_ID.HasValue && item.EQUIPMENT_SET_ID.Value > 0)
                    {
                        pres.EquipmentSetId = item.EQUIPMENT_SET_ID ?? 0;
                    }
                    if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                        pres.ServiceConditionId = item.SERVICE_CONDITION_ID;
                    if (item.OTHER_PAY_SOURCE_ID.HasValue && item.OTHER_PAY_SOURCE_ID.Value > 0)
                        pres.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;

                    this.InPatientPresMaterialSDOs.Add(pres);

                    if (
                        (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (item.IS_SPLIT_COMPENSATION.HasValue && item.IS_SPLIT_COMPENSATION == 1))
                        && ((item.IS_ALLOW_ODD ?? 0) != 1)
                        && (item.RAW_AMOUNT ?? 0) > 0
                        && (item.RAW_AMOUNT != (int)item.RAW_AMOUNT))
                    {
                        PresMaterialSDO presMaterialPlus = new PresMaterialSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<PresMaterialSDO>(presMaterialPlus, pres);

                        presMaterialPlus.Amount = item.CalculateAmountNoPres(item, this.ActionType);
                        presMaterialPlus.IsNotPres = true;
                        presMaterialPlus.InstructionTimes = item.IntructionTimeSelecteds;
                        if (presMaterialPlus.Amount > 0)
                            this.InPatientPresMaterialSDOs.Add(presMaterialPlus);
                    }
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
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
                    mety.MedicineInfoSdos = new List<MedicineInfoSDO>();
                    if (frmAssignPrescription.IsSaveOverResultReasonTest)
                    {
                        foreach (var intructionTime in frmAssignPrescription.intructionTimeSelecteds)
                        {
                            if (item.dicTreatmentOverResultTestReason != null && item.dicTreatmentOverResultTestReason.Count > 0 && item.dicTreatmentOverResultTestReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverResultTestReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverResultTestReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                            }
                            if (item.dicTreatmentOverKidneyReason != null && item.dicTreatmentOverKidneyReason.Count > 0 && item.dicTreatmentOverKidneyReason.ContainsKey(intructionTime))
                            {
                                var ListData = item.dicTreatmentOverKidneyReason[intructionTime];
                                if (ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId) != null)
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = intructionTime, IsNoPrescription = item.IsNoPrescription, OverKidneyReason = ListData.LastOrDefault(o => o.treatmentId == this.TreatmentId).overReason });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                                    mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_KIDNEY_REASON });
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                            mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_RESULT_TEST_REASON });
                        if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                            mety.MedicineInfoSdos.Add(new MedicineInfoSDO() { IntructionTime = frmAssignPrescription.InstructionTime, IsNoPrescription = false, OverResultTestReason = item.OVER_KIDNEY_REASON });
                    }
                    mety.HtuText = item.HTU_TEXT;
                    mety.Amount = (item.AMOUNT ?? 0);
                    mety.PresAmount = item.PRES_AMOUNT;
                    mety.MedicineTypeId = item.ID;
                    mety.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    mety.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    mety.UnitName = item.SERVICE_UNIT_NAME;
                    mety.NumOrder = item.NUM_ORDER;
                    mety.Tutorial = item.TUTORIAL;
                    mety.UseTimeTo = item.UseTimeTo;
                    mety.Price = item.PRICE ?? 0;
                    mety.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    mety.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if ((item.ALERT_MAX_IN_TREATMENT.HasValue && item.IsAlertInTreatPresciption) && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment != null && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.Count > 0 && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.Exists(o => o.PrimaryKey == item.PrimaryKey && o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[1] == TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE))
                    {
                        mety.ExceedLimitInTreatmentReason = frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.LastOrDefault(o => o.PrimaryKey == item.PrimaryKey && o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[1] == TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE).EXCEED_LIMIT_IN_BATCH_REASON;
                    }
                    mety.OddPresReason = item.ODD_PRES_REASON;
                    var sangTemp = frmAssignPrescription.GetValueSpinNew(item.Sang);
                    if (!String.IsNullOrEmpty(sangTemp))
                        mety.Morning = sangTemp;
                    var truaTemp = frmAssignPrescription.GetValueSpinNew(item.Trua);
                    if (!String.IsNullOrEmpty(truaTemp))
                        mety.Noon = truaTemp;
                    var chieuTemp = frmAssignPrescription.GetValueSpinNew(item.Chieu);
                    if (!String.IsNullOrEmpty(chieuTemp))
                        mety.Afternoon = chieuTemp;
                    var toiTemp = frmAssignPrescription.GetValueSpinNew(item.Toi);
                    if (!String.IsNullOrEmpty(toiTemp))
                        mety.Evening = toiTemp;
                    mety.HtuId = item.HTU_IDs != null && item.HTU_IDs.Count > 0 ? (long?)item.HTU_IDs[0] : null;

                    //if (item.SERVICE_CONDITION_ID.HasValue && item.SERVICE_CONDITION_ID.Value > 0)
                    //    mety.ServiceConditionId = item.SERVICE_CONDITION_ID;
                    mety.PreviousUsingCount = item.PREVIOUS_USING_COUNT;
                    if (this.ExpMestReasonId == null)
                    {
                        mety.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        mety.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    this.PresOutStockMetySDOs.Add(mety);
                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    PresOutStockMatySDO maty = new PresOutStockMatySDO();
                    maty.InstructionTimes = item.IntructionTimeSelecteds;
                    maty.Amount = (item.AMOUNT ?? 0);
                    maty.PresAmount = item.PRES_AMOUNT;
                    maty.MaterialTypeId = item.ID;
                    maty.MaterialTypeName = item.MEDICINE_TYPE_NAME;
                    maty.UnitName = item.SERVICE_UNIT_NAME;
                    maty.NumOrder = item.NUM_ORDER;
                    maty.Tutorial = item.TUTORIAL;
                    maty.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    maty.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    maty.Price = item.PRICE ?? 0;

                    maty.HtuText = item.HTU_TEXT;
                    if (this.ExpMestReasonId == null)
                    {
                        maty.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        maty.ExpMestReasonId = this.ExpMestReasonId;
                    }

                    this.PresOutStockMatySDOs.Add(maty);

                }
                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                {
                    PresOutStockMetySDO orty = new PresOutStockMetySDO();
                    orty.InstructionTimes = item.IntructionTimeSelecteds;
                    orty.Amount = (item.AMOUNT ?? 0);
                    orty.PresAmount = item.PRES_AMOUNT;
                    orty.NumOrder = item.NUM_ORDER;
                    orty.MedicineTypeName = item.MEDICINE_TYPE_NAME;
                    orty.UnitName = item.SERVICE_UNIT_NAME;
                    orty.MedicineUseFormId = item.MEDICINE_USE_FORM_ID;
                    orty.Tutorial = item.TUTORIAL;
                    orty.ExceedLimitInPresReason = item.EXCEED_LIMIT_IN_PRES_REASON;
                    orty.ExceedLimitInDayReason = item.EXCEED_LIMIT_IN_DAY_REASON;
                    if ((item.ALERT_MAX_IN_TREATMENT.HasValue && item.IsAlertInTreatPresciption) && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment != null && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.Count > 0 && frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.Exists(o => o.PrimaryKey == item.PrimaryKey && o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[1] == TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE))
                    {
                        orty.ExceedLimitInTreatmentReason = frmAssignPrescription.mediMatyTypeADOsAlertInTreatment.LastOrDefault(o => o.PrimaryKey == item.PrimaryKey && o.PATIENT_NAME_BY_TREATMENT_CODE.Split('_')[1] == TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE).EXCEED_LIMIT_IN_BATCH_REASON;
                    }
                    orty.HtuText = item.HTU_TEXT;
                    orty.OddPresReason = item.ODD_PRES_REASON;
                    orty.UseTimeTo = item.UseTimeTo;
                    orty.Price = item.PRICE ?? 0;
                    var sangTemp = frmAssignPrescription.GetValueSpinNew(item.Sang);
                    if (!String.IsNullOrEmpty(sangTemp))
                        orty.Morning = sangTemp;
                    var truaTemp = frmAssignPrescription.GetValueSpinNew(item.Trua);
                    if (!String.IsNullOrEmpty(truaTemp))
                        orty.Noon = truaTemp;
                    var chieuTemp = frmAssignPrescription.GetValueSpinNew(item.Chieu);
                    if (!String.IsNullOrEmpty(chieuTemp))
                        orty.Afternoon = chieuTemp;
                    var toiTemp = frmAssignPrescription.GetValueSpinNew(item.Toi);
                    if (!String.IsNullOrEmpty(toiTemp))
                        orty.Evening = toiTemp;
                    orty.HtuId = item.HTU_IDs != null && item.HTU_IDs.Count > 0 ? (long?)item.HTU_IDs[0] : null; // item.HTU_ID_NOT_CHECK_ACIN_INTERACTIVE;
                    orty.MedicineTypeId = null;

                    if (this.ExpMestReasonId == null)
                    {
                        orty.ExpMestReasonId = item.EXP_MEST_REASON_ID;
                    }
                    else
                    {
                        orty.ExpMestReasonId = this.ExpMestReasonId;
                    }

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
                            System.DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(UseTimes != null && UseTimes.Count > 0 ? this.UseTimes.FirstOrDefault() : this.InstructionTimes.FirstOrDefault()).Value;
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
                List<string> paramMessageErrorOther = new List<string>(), paramMessageErrorEmpty = new List<string>();
                if (frmAssignPrescription.treatmentFinishProcessor != null && !frmAssignPrescription.treatmentFinishProcessor.GetValidateWithMessage(frmAssignPrescription.ucTreatmentFinish, paramMessageErrorEmpty, paramMessageErrorOther))
                {
                    if (paramMessageErrorOther.Count > 0)
                    {
                        this.Param.Messages.AddRange(paramMessageErrorOther);
                    }
                    if (paramMessageErrorEmpty.Count > 0)
                    {
                        this.Param.Messages.Add(String.Join(", ", paramMessageErrorEmpty) + " " + Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
                    }

                    if (this.Param.Messages.Count > 0)
                    {
                        this.Param.Messages = this.Param.Messages.Distinct().ToList();
                    }
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
                        string messageErr = "";
                        string name = "";
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                        {
                            name = String.Format(ResourceMessage.CanhBaoThuoc, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item.MEDICINE_TYPE_NAME, System.Drawing.FontStyle.Bold));
                        }
                        else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        {
                            name = String.Format(ResourceMessage.CanhBaoVatTu, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item.MEDICINE_TYPE_NAME, System.Drawing.FontStyle.Bold));
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

                            if (!HisConfigCFG.IsNotAutoGenerateTutorial && (item.TUTORIAL == null || String.IsNullOrEmpty(item.TUTORIAL.Trim())))
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
                        if (!string.IsNullOrEmpty(messageErr.Trim()))
                            messageErr = name + messageErr;
                        //Check takebean thất bại
                        //if (item.IsNotTakeBean == true)
                        //{
                        //    //messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, System.Drawing.Color.Maroon));
                        //    valid = false;
                        //    Inventec.Common.Logging.LogSystem.Warn("Take bean thất bại");
                        //}

                        if (!valid && !string.IsNullOrEmpty(messageErr.Trim()))
                        {
                            param.Messages.Add(messageErr);
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
                Inventec.Common.Logging.LogSystem.Debug("CheckValidHeinServicePrice.1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.bIsSelectMultiPatientProcessing), frmAssignPrescription.bIsSelectMultiPatientProcessing));
                if (frmAssignPrescription.bIsSelectMultiPatientProcessing)
                {
                    return valid;
                }
                Inventec.Common.Logging.LogSystem.Debug("CheckValidHeinServicePrice.2");
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    decimal limitHeinMedicinePrice__RightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg;
                    decimal limitHeinMedicinePrice__NotRightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => limitHeinMedicinePrice__RightMediOrg), limitHeinMedicinePrice__RightMediOrg) +
                            Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => limitHeinMedicinePrice__NotRightMediOrg), limitHeinMedicinePrice__NotRightMediOrg) +
                            Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => frmAssignPrescription.currentHisPatientTypeAlter), frmAssignPrescription.currentHisPatientTypeAlter) +
                            Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => limitHeinMedicinePrice__RightMediOrg), limitHeinMedicinePrice__RightMediOrg));

                    //Kiểm tra, nếu có key cấu hình "check giới hạn thuốc", thì khi người dùng nhấn nút "Lưu" mới lấy thông tin hồ sơ điều trị để check
                    if ((limitHeinMedicinePrice__RightMediOrg > 0
                            || limitHeinMedicinePrice__NotRightMediOrg > 0)
                        && frmAssignPrescription.currentHisPatientTypeAlter != null && (frmAssignPrescription.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                            || frmAssignPrescription.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                        )
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE), this.TreatmentWithPatientTypeInfoSDO.TREATMENT_CODE) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP), this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.PatientTypeAlter.TREATMENT_TYPE_CODE), this.PatientTypeAlter.TREATMENT_TYPE_CODE));

                        //Nếu hồ sơ điều trị cấu hình trường IS_NOT_CHECK_LHMP = 1 thì bỏ qua không check, return true
                        //Hoặc đối tượng điều trị là điều trị nội/ngoại trú thì bỏ qua không check
                        //Sửa lại chỉ tính trần bhyt theo đơn phòng khám. (theo 2 cấu hình mức trần bn đúng tuyến đúng cskcb và đúng tuyến chuyển tuyến)
                        //không tính đơn tủ trực
                        if ((this.TreatmentWithPatientTypeInfoSDO != null
                        && this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP.HasValue
                        && (this.TreatmentWithPatientTypeInfoSDO.IS_NOT_CHECK_LHMP ?? 0) == 1)
                        || this.PatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatIn
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
                Inventec.Common.Logging.LogSystem.Debug("CheckValidHeinServicePrice.3");
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
