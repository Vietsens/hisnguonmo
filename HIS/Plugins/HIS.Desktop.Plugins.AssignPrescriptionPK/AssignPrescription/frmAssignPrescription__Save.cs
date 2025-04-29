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
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Save;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Utility;
using HIS.UC.Icd.ADO;
using HIS.UC.MenuPrint.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void UpdateAutoRoundUpByConvertUnitRatioDataMediMaty()
        {
            try
            {
                var instructionTimeMedis = GetInstructionTimeMedi();
                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (item.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD && item.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        item.UpdateAutoRoundUpByConvertUnitRatioInDataRow(item, VHistreatment);
                    }

                    if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC) && item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (item.MEDICINE_USE_FORM_ID ?? 0) <= 0)
                    {
                        item.ErrorMessageMedicineUseForm = ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
                        item.ErrorTypeMedicineUseForm = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessageMedicineUseForm = "";
                        item.ErrorTypeMedicineUseForm = ErrorType.None;
                    }

                    if (item.PATIENT_TYPE_ID <= 0)
                    {
                        item.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        item.ErrorTypePatientTypeId = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessagePatientTypeId = "";
                        item.ErrorTypePatientTypeId = ErrorType.None;
                    }

                    if (item.AMOUNT <= 0 && (currentMediStockNhaThuocSelecteds == null || currentMediStockNhaThuocSelecteds.Count == 0 || (currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0 && item.IS_OUT_HOSPITAL != GlobalVariables.CommonNumberTrue)))
                    {
                        item.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        item.ErrorTypeAmount = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessageAmount = "";
                        item.ErrorTypeAmount = ErrorType.None;
                    }

                    item.ErrorMessageIsAssignDay = "";
                    item.ErrorTypeIsAssignDay = ErrorType.None;
                    if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                    {
                        HIS_SERE_SERV sereServ = null;
                        if (item.IsEdit)
                        {
                            sereServ = this.sereServWithTreatment.FirstOrDefault(o =>
                            o.SERVICE_ID == item.SERVICE_ID
                            && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == (instructionTimeMedis.OrderByDescending(t => t).First()).ToString().Substring(0, 8)
                            && o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID);
                        }
                        else
                        {
                            sereServ = this.sereServWithTreatment.FirstOrDefault(o =>
                            o.SERVICE_ID == item.SERVICE_ID
                            && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == (instructionTimeMedis.OrderByDescending(t => t).First()).ToString().Substring(0, 8));
                        }

                        if (sereServ != null)
                        {
                            if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                    || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                    || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                            {
                                item.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                                item.ErrorTypeIsAssignDay = ErrorType.Warning;
                            }
                        }

                        //Inventec.Common.Logging.LogSystem.Debug("UpdateAutoRoundUpByConvertUnitRatioDataMediMaty____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServWithTreatment), sereServWithTreatment) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServ), sereServ) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidDataMediMaty()
        {
            try
            {
                var instructionTimeMedis = GetInstructionTimeMedi();
                if (!PrescriptionPrevious)
                    GetOverReason(ref this.mediMatyTypeADOs, new List<long>() { treatmentId }, instructionTimeMedis, false);
                PrescriptionPrevious = false;
                foreach (var item in this.mediMatyTypeADOs)
                {
                    SereServInDay = new List<HIS_SERE_SERV>();
                    if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC) && item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (item.MEDICINE_USE_FORM_ID ?? 0) <= 0)
                    {
                        item.ErrorMessageMedicineUseForm = ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
                        item.ErrorTypeMedicineUseForm = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessageMedicineUseForm = "";
                        item.ErrorTypeMedicineUseForm = ErrorType.None;
                    }

                    if (item.PATIENT_TYPE_ID <= 0)
                    {
                        item.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        item.ErrorTypePatientTypeId = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessagePatientTypeId = "";
                        item.ErrorTypePatientTypeId = ErrorType.None;
                    }

                    if (item.AMOUNT <= 0 && (currentMediStockNhaThuocSelecteds == null || currentMediStockNhaThuocSelecteds.Count == 0 || (currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0 && item.IS_OUT_HOSPITAL != GlobalVariables.CommonNumberTrue)))
                    {
                        item.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        item.ErrorTypeAmount = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessageAmount = "";
                        item.ErrorTypeAmount = ErrorType.None;
                    }
                    if (!String.IsNullOrEmpty(item.ODD_WARNING_CONTENT) && String.IsNullOrWhiteSpace(item.ODD_PRES_REASON) && item.AMOUNT != (int)item.AMOUNT)
                    {
                        item.ErrorMessageOddPres = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                        item.ErrorTypeOddPres = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessageOddPres = "";
                        item.ErrorTypeOddPres = ErrorType.None;
                    }
                    item.ErrorMessageIsAssignDay = "";
                    item.ErrorTypeIsAssignDay = ErrorType.None;
                    if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                    {
                        if (item.IsEdit)
                        {
                            SereServInDay = this.sereServWithTreatment.Where(o =>
                            o.SERVICE_ID == item.SERVICE_ID
                            && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == (instructionTimeMedis.OrderByDescending(t => t).First()).ToString().Substring(0, 8)
                            && o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID).ToList();
                        }
                        else
                        {
                            SereServInDay = this.sereServWithTreatment.Where(o =>
                            o.SERVICE_ID == item.SERVICE_ID
                            && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == (instructionTimeMedis.OrderByDescending(t => t).First()).ToString().Substring(0, 8)).ToList();
                        }
                        if (SereServInDay != null && SereServInDay.Count > 0)
                        {
                            if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                    || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                    || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                            {
                                item.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                                item.ErrorTypeIsAssignDay = ErrorType.Warning;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                        item.IsEditOverResultTestReason = true;
                    if (item.IsEditOverResultTestReason && string.IsNullOrEmpty(item.OVER_RESULT_TEST_REASON))
                    {
                        item.ErrorMessageOverResultTestReason = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                        item.ErrorTypeOverResultTestReason = ErrorType.Warning;
                    }
                    if (!string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                        item.IsEditOverKidneyReason = true;
                    if (item.IsEditOverKidneyReason && string.IsNullOrEmpty(item.OVER_KIDNEY_REASON))
                    {
                        item.ErrorMessageOverKidneyReason = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                        item.ErrorTypeOverKidneyReason = ErrorType.Warning;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AlertOutofMaxExpend()
        {
            try
            {
                string errMessage = "";
                string errMedicineDetail = "";
                decimal totalExpend = 0;
                foreach (var item in this.mediMatyTypeADOs)
                {
                    //Khi người dùng chỉ định dịch vụ trong màn hình xử lý phẫu thuật thủ thuật, phần mềm kiểm tra xem trường max_expend có khác null không, 
                    //nếu khác null thì check tổng số tiền hao phí đã vượt quá số tiền định mức chưa, 
                    //nếu đã vượt thì ko check "hao phí" với các dịch vụ bổ sung khiến tổng tiền bị vượt
                    if (this.Service__Main != null && (this.Service__Main.MAX_EXPEND ?? 0) > 0)
                    {
                        if ((item.PATIENT_TYPE_ID ?? 0) > 0 && item.IsExpend == true)
                        {
                            decimal priceCal = CalculatePrice(item);
                            if (priceCal > 0)
                            {
                                totalExpend += priceCal;
                                item.IsExpend = false;
                            }

                            //var dataServicePrice = servicePatyAllows[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                            //if (dataServicePrice != null && dataServicePrice.Count > 0)
                            //{
                            //    totalExpend += ((item.AMOUNT ?? 0) * (dataServicePrice[0].PRICE * (1 + dataServicePrice[0].VAT_RATIO)));
                            //    item.IsExpend = false;
                            //}
                        }

                        if ((totalExpend + this.currentExpendInServicePackage) > Service__Main.MAX_EXPEND.Value)
                        {
                            errMedicineDetail += (item.MEDICINE_TYPE_CODE + " - " + item.MEDICINE_TYPE_NAME + ";");
                        }
                    }
                }
                if (!String.IsNullOrEmpty(errMedicineDetail))
                {
                    errMessage += String.Format(ResourceMessage.TongTienHaoPhiVuotQuaDinhMucHaoPhiKhongTheChonDichVuNay, Inventec.Common.Number.Convert.NumberToString((totalExpend + this.currentExpendInServicePackage), 0), Inventec.Common.Number.Convert.NumberToString(Service__Main.MAX_EXPEND.Value, 0), Service__Main.SERVICE_NAME, "\r\n" + errMedicineDetail);
                    //MessageManager.Show(errMessage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleButton(int action)
        {
            try
            {
                if (action == GlobalVariables.ActionAdd)
                    this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnAdd.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                else
                    this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnAdd.Text.Update", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                    return;

                this.btnSave.Enabled = isCheckAssignServiceSimultaneityOption ? false : isLock;
                this.btnSaveAndPrint.Enabled = isCheckAssignServiceSimultaneityOption ? false : isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool IsNotReloadTreatmentFinish = false;
        private void UpdateTotalPriceInRow(List<HIS_SERE_SERV> SereServs)
        {
            try
            {
                if (SereServs == null || SereServs.Count == 0)
                    return;
                foreach (var item in mediMatyTypeADOs)
                {
                    var SereServItem = SereServs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID);
                    if (SereServItem != null)
                    {
                        item.TotalPrice = (SereServItem.VIR_PRICE ?? 0) * SereServItem.AMOUNT;
                    }
                }
                IsNotReloadTreatmentFinish = true;
                gridViewServiceProcess.GridControl.DataSource = null;
                gridViewServiceProcess.GridControl.DataSource = mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                IsNotReloadTreatmentFinish = false;
                this.SetTotalPrice__TrongDon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckIcd()
        {
            bool valid = true;
            try
            {
                string messErr = null;
                if (HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2")
                {
                    InitCheckIcdManager();
                    if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr, HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2", true))
                    {
                        if (HisConfigCFG.CheckIcdWhenSave == "1")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(currentTreatment.TREATMENT_CODE + ": " + messErr + ". Bạn có muốn tiếp tục?",
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.YesNo) == DialogResult.No) valid = false;
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(currentTreatment.TREATMENT_CODE + ": " + messErr,
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.OK);
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private decimal? GetPrice()
        {
            decimal? result = null;
            try
            {
                result = transferTotal + totalPriceNotBHYT;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE saveType)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.1");
                IsValidForSave = true;
                bool valid = true;
                ListExpMestMedicineAntibioticRequired = null;
                this.positionHandleControl = -1;
                this.resultDataPrescription = null;
                List<HIS_SERVICE_REQ> lstserviceReqResult = null;

                //if (this.gridViewServiceProcess.IsEditing)
                //    this.gridViewServiceProcess.CloseEditor();

                //if (this.gridViewServiceProcess.FocusedRowModified)
                //    this.gridViewServiceProcess.UpdateCurrentRow();

                if (!(GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) && !string.IsNullOrEmpty(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                {
                    LoadVServiceReq();
                    if (vServiceReq != null && vServiceReq.START_TIME.HasValue && Inventec.Common.DateTime.Calculation.DifferenceTime(vServiceReq.START_TIME.Value, InstructionTime, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.SECOND) < Int32.Parse(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian chỉ định {0} phải cách thời gian bắt đầu khám {1} là {2} giây mới được phép chỉ định", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(InstructionTime), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vServiceReq.START_TIME.Value), HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return;
                    }
                }

                this.paramCommon = new CommonParam();
                string validFolow = "", warning = "";

                //this.mediMatyTypeADOs = this.gridViewServiceProcess.DataSource as List<MediMatyTypeADO>;
                valid = this.dxValidationProviderControl.Validate() && valid;
                if (ucIcdYhct != null)
                    valid = valid && (bool)icdYhctProcessor.ValidationIcd(ucIcdYhct);
                if (ucSecondaryIcdYhct != null)
                    valid = valid && subIcdYhctProcessor.GetValidate(ucSecondaryIcdYhct);
                string codeIcd = "";
                string nameIcd = "";
                var icdValue = UcIcdGetValue() as UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    codeIcd = icdValue.ICD_CODE;
                    nameIcd = icdValue.ICD_NAME;
                }

                var subIcd = UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    codeIcd += subIcd.ICD_SUB_CODE;
                    nameIcd += subIcd.ICD_TEXT;
                }
                if (!string.IsNullOrEmpty(codeIcd) && codeIcd.Length > 100)
                {
                    IsValidForSave = false;
                    XtraMessageBox.Show(string.Format("Mã chẩn đoán phụ nhập quá {0} ký tự", 100));
                    return;
                }

                if (!string.IsNullOrEmpty(nameIcd) && Encoding.UTF8.GetByteCount(nameIcd) > 1500)
                {
                    IsValidForSave = false;
                    XtraMessageBox.Show(string.Format("Tên chẩn đoán phụ nhập quá {0} ký tự", 1500));
                    return;
                }

                string codeIcdYhct = "";
                if (ucIcdYhct != null)
                {
                    var icdTranditional = icdYhctProcessor.GetValue(ucIcdYhct);
                    if (icdTranditional != null && icdTranditional is UC.Icd.ADO.IcdInputADO)
                    {
                        codeIcdYhct = ((UC.Icd.ADO.IcdInputADO)icdTranditional).ICD_CODE;
                    }
                }
                if (ucSecondaryIcdYhct != null)
                {
                    var subIcdTranditional = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                    if (subIcdTranditional != null && subIcdTranditional is SecondaryIcdDataADO)
                    {
                        codeIcdYhct += ((SecondaryIcdDataADO)subIcdTranditional).ICD_SUB_CODE;
                    }
                }
                if (!string.IsNullOrEmpty(codeIcdYhct) && codeIcdYhct.Length > 255)
                {
                    IsValidForSave = false;
                    XtraMessageBox.Show(string.Format("Mã chẩn đoán YHCT phụ nhập quá {0} ký tự", 255));
                    return;
                }

                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.2");
                bool isHasUcTreatmentFinish = ((!GlobalStore.IsTreatmentIn) && this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null);
                var treatUC = isHasUcTreatmentFinish ? treatmentFinishProcessor.GetDataOutput(this.ucTreatmentFinish) : null;
                bool isHasTreatmentFinishChecked = (treatUC != null && treatUC.IsAutoTreatmentFinish);
                if (isHasTreatmentFinishChecked && treatUC != null)
                {
                    if (subIcd != null && !string.IsNullOrEmpty(subIcd.ICD_SUB_CODE))
                    {
                        var subIcdList = subIcd.ICD_SUB_CODE.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (subIcdList != null && subIcdList.Count > 12)
                        {
                            if ((HisConfigCFG.IsCheckSubIcdExceedLimit == "1" && DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Vui lòng kiểm tra lại",
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.OK) == DialogResult.OK) || (HisConfigCFG.IsCheckSubIcdExceedLimit == "2" && DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Bạn có muốn tiếp tục?",
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.YesNo) == DialogResult.No))
                            {
                                return;
                            }
                        }
                    }
                    if ((string.IsNullOrEmpty(currentTreatment.TREATMENT_METHOD) || string.IsNullOrEmpty(treatUC.TreatmentMethod)) && ((HisConfigCFG.RequiredTreatmentMethodOption == "1" && currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU && (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV)) || (HisConfigCFG.RequiredTreatmentMethodOption == "2" && (treatUC.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM || ((currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) && (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV))))))
                    {
                        frmTreatmentMethod frm = new frmTreatmentMethod(TreatmentMethod, treatmentMethod =>
                        {
                            TreatmentMethod = treatmentMethod;
                        });
                        frm.ShowDialog();
                        if (string.IsNullOrEmpty(TreatmentMethod))
                            return;
                    }
                    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID);
                    var price = GetPrice();
                    if (treatmentType != null && price != null && price > 0)
                    {
                        if (treatmentType.FEE_DEBT_OPTION == 2)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Bệnh nhân đang thiếu viện phí {0} đồng", Inventec.Common.Number.Convert.NumberToString(Math.Abs(price ?? 0), ConfigApplications.NumberSeperator)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            return;
                        }
                        else if (treatmentType.FEE_DEBT_OPTION == 1)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Bệnh nhân đang thiếu viện phí {0} đồng", Inventec.Common.Number.Convert.NumberToString(Math.Abs(price ?? 0), ConfigApplications.NumberSeperator)) + ". Bạn có muốn tiếp tục?",
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                        }
                    }

                    if ((HisConfigCFG.OptionTreatmentEndTypeIsTransfer == "1" || HisConfigCFG.OptionTreatmentEndTypeIsTransfer == "2") && treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        HisExpMestFilter filter = new HisExpMestFilter();
                        filter.TDL_TREATMENT_ID = treatmentId;
                        filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK;
                        var expMestPK = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, null);
                        if (expMestPK != null && expMestPK.Count > 0 && XtraMessageBox.Show(string.Format("Bệnh nhân tồn tại đơn phòng khám, {0}", HisConfigCFG.OptionTreatmentEndTypeIsTransfer == "1" ? "không cho phép chuyển viện" : "bạn có muốn cho bệnh nhân chuyển viện không?"), "Thông báo", HisConfigCFG.OptionTreatmentEndTypeIsTransfer == "1" ? MessageBoxButtons.OK : MessageBoxButtons.YesNo) == (HisConfigCFG.OptionTreatmentEndTypeIsTransfer == "1" ? DialogResult.OK : DialogResult.No))
                        {
                            return;
                        }
                    }

                }
                if (valid)
                {
                    string errorMessages = "";
                    string errorMessagesBlock = "";
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        if (item.ErrorTypeAmount == ErrorType.Warning)
                        {
                            if (item.IsNotShowMessage)
                            {
                                valid = false;
                            }
                            else
                            {
                                errorMessages += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessageAmount + "; ";
                            }
                        }
                        if (item.ErrorTypeIsAssignDay == ErrorType.Warning)
                        {
                            errorMessages += item.MEDICINE_TYPE_NAME + " " + ResourceMessage.CanhBaoDaKeTrongNgay + "; ";
                        }
                        if (item.ErrorTypePatientTypeId == ErrorType.Warning)
                        {
                            errorMessages += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessagePatientTypeId + "; ";
                        }
                        if (item.ErrorTypeOddPres == ErrorType.Warning)
                        {
                            errorMessagesBlock += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessageOddPres + "; ";
                        }
                        if (item.ErrorTypeOverResultTestReason == ErrorType.Warning)
                        {
                            errorMessagesBlock += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessageOverResultTestReason + "; ";
                        }
                        if (item.ErrorTypeOverKidneyReason == ErrorType.Warning)
                        {
                            errorMessagesBlock += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessageOverKidneyReason + ".";
                        }
                    }
                    if (!String.IsNullOrEmpty(errorMessagesBlock))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(errorMessagesBlock, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return;
                    }
                    if (!String.IsNullOrEmpty(errorMessages))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(errorMessages + ". Bạn có muốn tiếp tục?",
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) valid = false;
                    }
                }
                else
                {
                    if (this.ModuleControls == null || this.ModuleControls.Count == 0)
                    {
                        ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                        this.ModuleControls = controlProcess.GetControls(this);
                    }

                    GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new Utility.GetMessageErrorControlInvalidProcess();
                    getMessageErrorControlInvalidProcess.Run(this, this.dxValidationProviderControl, this.ModuleControls, this.paramCommon);

                    warning = this.paramCommon.GetMessage();

                    if (!String.IsNullOrEmpty(warning))
                        MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                validFolow += "valid.0=" + valid + ";";
                valid = valid && this.CheckIcd();
                validFolow += "valid.1=" + valid + ";";
                valid = valid && this.CheckTuberCulosis();
                validFolow += "valid.2=" + valid + ";";
                valid = valid && this.CheckMaxExpend();
                validFolow += "valid.3=" + valid + ";";
                valid = valid && this.CheckTreatmentFinish();
                validFolow += "valid.4=" + valid + ";";
                valid = valid && this.CheckICDService();//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.5=" + valid + ";";
                valid = valid && this.CheckUseDayAndExpTimeBHYT();
                validFolow += "valid.6=" + valid + ";";
                valid = valid && this.CheckWarringIntructionUseDayNum();
                validFolow += "valid.7=" + valid + ";";
                valid = valid && this.CheckThuocKhangSinhTrongNgay();//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.8=" + valid + ";";
                valid = valid && this.CheckCungHoatChat();
                validFolow += "valid.9=" + valid + ";";
                valid = valid && this.CheckMediStockWhenEditPrescription();
                validFolow += "valid.10=" + valid + ";";
                valid = valid && this.CheckPrescriptionSplitOutMediStock();
                validFolow += "valid.11=" + valid + ";";
                valid = valid && this.CheckAmoutWarringInStock();
                validFolow += "valid.12=" + valid + ";";
                valid = valid && this.CheckAmoutWarringNumber();
                validFolow += "valid.13=" + valid + ";";
                valid = valid && this.ProcessValidMedicineTypeAge();//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.14=" + valid + ";";
                valid = valid && ValidAcinInteractiveWorker.ValidConsultationReqiured(this.actionType, this.oldServiceReq, this.currentModule, this.requestRoom, this.treatmentId, this.mediMatyTypeADOs);//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.15=" + valid + ";";
                valid = valid && this.CheckOverlapWarningOption();
                validFolow += "valid.16=" + valid + ";";
                valid = valid && this.CheckChongChiDinhWarring();//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.17=" + valid + ";";
                valid = valid && this.WarningAlertWarningFeeProcess();//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.18=" + valid + ";";
                valid = valid && this.ValidSereServWithCondition();//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.19=" + valid + ";";
                valid = valid && this.CheckICDServiceForContraindicaterWarningOption(this.mediMatyTypeADOs, true);//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.20=" + valid + ";";
                valid = valid && this.ValidSereServWithOtherPaySource(this.mediMatyTypeADOs);//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.21=" + valid + ";";
                valid = valid && this.CheckMaxInPrescriptionWhenSave(this.mediMatyTypeADOs);//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.22=" + valid + ";";
                valid = valid && this.CheckMaxInPrescriptionInDayWhenSave(this.mediMatyTypeADOs);//TODO cần check với TH chọn nhiều BN kê
                validFolow += "valid.23=" + valid + ";";
                if (!(GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                    valid = valid && this.CheckMaxInPrescriptionInBatchWhenSave(this.mediMatyTypeADOs);
                validFolow += "valid.23=" + valid + ";";
                if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign && currentWorkPlace.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                {
                    valid = valid && CheckTimeInDepartment();
                    validFolow += "valid.22=" + valid + ";";
                }

                valid = valid && CheckMultiIntructionTime();
                validFolow += "valid.23=" + valid + ";";

                valid = valid && CheckUseTime();
                validFolow += "valid.24=" + valid + ";";

                valid = valid && CheckReasonRequied(); //kiểm tra bắt buộc nhập lý do xuất
                validFolow += "valid.25=" + valid + ";";

                valid = valid && CheckPayICD(); //kiểm tra đối tượng thanh toán theo chẩn đoán
                validFolow += "valid.26=" + valid + ";";

                Inventec.Common.Logging.LogSystem.Debug(validFolow + "____" + Inventec.Common.Logging.LogUtil.TraceData("frmAssignPrescription.valid", valid));
                IsValidForSave = valid;
                if (!valid)
                {
                    if (treatmentFinishProcessor != null && this.ucTreatmentFinish != null && isHasTreatmentFinishChecked && treatUC != null && treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN && treatUC.dtAppointmentTime == DateTime.MinValue)
                    {
                        treatmentFinishProcessor.ShowPopupAppointmentControl(this.ucTreatmentFinish);
                    }
                    return;
                }
                else
                {
                    if (!isHasTreatmentFinishChecked)
                    {
                        bool validNotFinishingIncaseOfOutPatient = this.WarningWhenNotFinishingIncaseOfOutPatientProcess(isHasTreatmentFinishChecked);
                        if (validNotFinishingIncaseOfOutPatient)
                        {
                            if (treatmentFinishProcessor != null && this.ucTreatmentFinish != null && treatUC != null)
                            {
                                treatmentFinishProcessor.ShowPopupWhenNotFinishingIncaseOfOutPatient(this.ucTreatmentFinish);
                                IsValidForSave = false;
                                return;
                            }
                        }
                    }
                }

                if (treatUC != null)
                {
                    var patientProgram = BackendDataWorker.Get<HIS_PROGRAM>().FirstOrDefault(o => o.ID == treatUC.ProgramId);
                    if (patientProgram != null && treatUC.IsIssueOutPatientStoreCode && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU && patientProgram.AUTO_CHANGE_TO_OUT_PATIENT == 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.ChuongTrinh, patientProgram.PROGRAM_NAME),
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK);
                        IsValidForSave = false;
                    }
                    if ((HisConfigCFG.MustChooseSeviceExamOption == "1" || (isHasTreatmentFinishChecked && HisConfigCFG.MustChooseSeviceExamOption == "2")) && !this.CheckMustChooseSeviceExamOption(HisConfigCFG.MustChooseSeviceExamOption))
                        return;
                }

                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.3");


                bool isSaveAndPrint = (saveType == SAVETYPE.SAVE_PRINT_NOW);
                //Tạm khóa các button lưu && lưu in lại khi đang xử lý
                this.ChangeLockButtonWhileProcess(false);
                bool success = false;
                WaitingManager.Show();
                msgTuVong = "";
                //List<MediMatyTypeADO> mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                //AutoMapper.Mapper.CreateMap<MediMatyTypeADO, MediMatyTypeADO>();
                //mediMatyTypeADOTemps = AutoMapper.Mapper.Map<List<MediMatyTypeADO>>(this.mediMatyTypeADOs);
                //mediMatyTypeADOTemps.AddRange(this.mediMatyTypeADOs);

                ISave isave = SaveFactory.MakeISave(
                    paramCommon,
                    mediMatyTypeADOs,
                    this,
                    this.actionType,
                    isSaveAndPrint,
                    this.serviceReqParentId ?? 0,
                    this.GetSereServInKip()
                    );

                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.4");
                this.resultDataPrescription = (isave != null ? isave.Run() : null);
                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.5");
                if (this.resultDataPrescription != null)
                {
                    HIS_SERVICE_REQ serviceReqResult = null;
                    HIS_EXP_MEST expMestResult = null;
                    ListExpMestResult = null;
                    List<HIS_SERE_SERV> lstSereServObeyCantraindi = new List<HIS_SERE_SERV>();
                    List<HIS_SERE_SERV> lstSereServ = new List<HIS_SERE_SERV>();
                    List<HIS_EXP_MEST_MEDICINE> Medicines = new List<HIS_EXP_MEST_MEDICINE>();
                    List<HIS_EXP_MEST_MATERIAL> Materials = new List<HIS_EXP_MEST_MATERIAL>();
                    if (this.resultDataPrescription.GetType() == typeof(InPatientPresResultSDO))
                    {
                        InPatientPresResultSDO patientPresResultSDO = this.resultDataPrescription as InPatientPresResultSDO;
                        serviceReqResult = patientPresResultSDO.ServiceReqs.FirstOrDefault();
                        expMestResult = (patientPresResultSDO.ExpMests != null && patientPresResultSDO.ExpMests.Count > 0 ? patientPresResultSDO.ExpMests.FirstOrDefault() : null);
                        ListExpMestResult = patientPresResultSDO.ExpMests;
                        lstserviceReqResult = new List<HIS_SERVICE_REQ>();
                        lstserviceReqResult.AddRange(patientPresResultSDO.ServiceReqs);
                        if (patientPresResultSDO.SereServs != null)
                        {
                            lstSereServObeyCantraindi = new List<HIS_SERE_SERV>();
                            lstSereServObeyCantraindi.AddRange(patientPresResultSDO.SereServs);
                        }
                        Medicines = patientPresResultSDO.Medicines;
                        Materials = patientPresResultSDO.Materials;
                    }
                    else if (this.resultDataPrescription.GetType() == typeof(OutPatientPresResultSDO))
                    {
                        OutPatientPresResultSDO outPatientPresResultSDO = this.resultDataPrescription as OutPatientPresResultSDO;
                        serviceReqResult = outPatientPresResultSDO.ServiceReqs.FirstOrDefault();
                        expMestResult = (outPatientPresResultSDO.ExpMests != null && outPatientPresResultSDO.ExpMests.Count > 0 ? outPatientPresResultSDO.ExpMests.FirstOrDefault() : null);
                        ListExpMestResult = outPatientPresResultSDO.ExpMests;
                        lstserviceReqResult = new List<HIS_SERVICE_REQ>();
                        lstserviceReqResult.AddRange(outPatientPresResultSDO.ServiceReqs);
                        Medicines = outPatientPresResultSDO.Medicines;
                        Materials = outPatientPresResultSDO.Materials;
                        if (outPatientPresResultSDO.SereServs != null)
                        {
                            lstSereServObeyCantraindi = new List<HIS_SERE_SERV>();
                            lstSereServObeyCantraindi.AddRange(outPatientPresResultSDO.SereServs);
                        }
                    }
                    TaskUpdateObeyCantraindi(lstSereServObeyCantraindi, ListExpMestResult, lstserviceReqResult);
                    List<long> mediTypeIds = new List<long>();
                    List<long> mateTypeIds = new List<long>();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListExpMestResult), ListExpMestResult));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Medicines), Medicines));
                    if (Medicines != null && Medicines.Count > 0)
                    {
                        if (ListExpMestResult != null && ListExpMestResult.Count > 0 && ListExpMestResult.FirstOrDefault(o => o.ANTIBIOTIC_REQUEST_ID == null) != null)
                        {
                            ListExpMestMedicineAntibioticRequired = Medicines.Where(o => ListExpMestResult.Where(p => p.ANTIBIOTIC_REQUEST_ID == null).Select(p => p.ID).ToList().Exists(p => p == o.EXP_MEST_ID)).ToList();
                        }
                        mediTypeIds.AddRange(Medicines.Select(o => o.TDL_MEDICINE_TYPE_ID ?? 0).ToList());
                        var mediType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().Where(o => mediTypeIds.Exists(p => p == o.ID)).ToList();
                        foreach (var item in Medicines)
                        {
                            HIS_SERE_SERV ss = new HIS_SERE_SERV();
                            var medi = mediType.FirstOrDefault(o => o.ID == item.TDL_MEDICINE_TYPE_ID);
                            if (medi != null)
                            {
                                ss.SERVICE_ID = medi.SERVICE_ID;
                                ss.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID ?? 0;
                                ss.VIR_PRICE = item.VIR_PRICE;
                                ss.AMOUNT = item.AMOUNT;
                                lstSereServ.Add(ss);
                            }
                        }
                    }
                    if (Materials != null && Materials.Count > 0)
                    {
                        mateTypeIds.AddRange(Materials.Select(o => o.TDL_MATERIAL_TYPE_ID ?? 0).ToList());
                        var mateType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>().Where(o => mateTypeIds.Exists(p => p == o.ID)).ToList();
                        foreach (var item in Materials)
                        {
                            HIS_SERE_SERV ss = new HIS_SERE_SERV();
                            var mate = mateType.FirstOrDefault(o => o.ID == item.TDL_MATERIAL_TYPE_ID);
                            if (mate != null)
                            {
                                ss.SERVICE_ID = mate.SERVICE_ID;
                                ss.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID ?? 0;
                                ss.VIR_PRICE = item.VIR_PRICE;
                                ss.AMOUNT = item.AMOUNT;
                                lstSereServ.Add(ss);
                            }
                        }
                    }

                    UpdateTotalPriceInRow(lstSereServ);
                    this.oldServiceReq = serviceReqResult;
                    this.oldExpMest = expMestResult;
                    this.oldExpMestId = this.oldExpMest != null ? this.oldExpMest.ID : 0;
                    if (assignPrescriptionEditADO != null && assignPrescriptionEditADO.DgRefeshData != null)
                    {
                        assignPrescriptionEditADO.DgRefeshData(this.oldServiceReq);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(msgTuVong)) paramCommon.Messages.Add(msgTuVong);
                }
                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.6");
                success = (this.resultDataPrescription != null) ? ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ?
                    ProcessAfterSaveForIn(isave, isSaveAndPrint, (InPatientPresResultSDO)this.resultDataPrescription)
                    : ProcessAfterSaveForOut(isave, isSaveAndPrint, (OutPatientPresResultSDO)this.resultDataPrescription)) : false;

                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.7");
                if (success)
                {
                    //Tạo phiếu yêu cầu
                    CreateRequestTicket(this.actionType);

                    gridControlTutorial.DataSource = null;


                    this.actionBosung = GlobalVariables.ActionAdd;

                    //Mở khóa các button lưu && lưu in khi đã xử lý xong
                    isCheckAssignServiceSimultaneityOption = false;
                    if (lstserviceReqResult != null && lstserviceReqResult.Count == 1)
                    {
                        this.actionType = GlobalVariables.ActionEdit;
                        this.SetEnableButtonControl(this.actionType);
                        this.ChangeLockButtonWhileProcess(true);
                        this.NoEdit = true;
                        this.btnNew.Enabled = true;
                        this.InitComboNhaThuoc();
                    }
                    else
                    {
                        this.actionType = GlobalVariables.ActionView;
                        this.SetEnableButtonControl(this.actionType);
                        this.ChangeLockButtonWhileProcess(false);
                    }

                    if (isHasTreatmentFinishChecked)
                    {
                        this.layoutControl6.BeginUpdate();
                        this.layoutControlPrintAssignPrescription.Root.Clear();

                        this.lciPrintAssignPrescription.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                        this.lciPrintAssignPrescription.MinSize = new System.Drawing.Size(1, 1);
                        this.lciPrintAssignPrescription.MaxSize = new System.Drawing.Size(2, 40);

                        //this.lciPrintAssignPrescriptionExt.MinSize = new System.Drawing.Size(2, 1);
                        //this.lciPrintAssignPrescriptionExt.MaxSize = new System.Drawing.Size(2, 40);
                        this.lciPrintAssignPrescriptionExt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lciPrintAssignPrescription.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.InitMenuToButtonPrint();//TODO
                        this.layoutControl6.EndUpdate();

                        if (this.processWhileAutoTreatmentEnd != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("processWhileAutoTreatmentEnd");
                            this.processWhileAutoTreatmentEnd();
                        }
                    }
                    EnableButtonConfig();
                    // Sửa lại nghiệp vụ các nút "Ký", "In", "Xem trước in" để nhất quán với các chức năng khác. Cụ thể:
                    //- Khi checkbox "Xem trước in" được check thì tự động bỏ check checkbox "In"
                    //- Khi checkbox "In" được check thì tự động bỏ check checkbox "Xem trước In"

                    //- Nếu checkbox "In" được check thì xử lý:
                    //+ Nếu checkbox "Ký" được check, thì thực hiện tự động in luôn (chứ không hiển thị màn hình print-preview) ra văn bản sau khi ký (văn bản do EMR trả về).
                    //+ Nếu checkbox "Ký" không được check, thì thực hiện tự động in ra đơn thuốc trên HIS.

                    //- Nếu checkbox "Xem trước khi in" được check thì xử lý:
                    //+ Nếu checkbox "Ký" được check, thì thực hiện tự động hiển thị màn hình print-preview ra văn bản sau khi ký (văn bản do EMR trả về).
                    //+ Nếu checkbox "Ký" không được check, thì thực hiện tự động hiển thị màn hình print preview đơn thuốc trên HIS
                    MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null;
                    bool printNow = (isSaveAndPrint || chkPrint.Checked);
                    switch (saveType)
                    {
                        case SAVETYPE.SAVE:
                            if ((lciForchkSignForDDT.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkSignForDDT.Checked) || (lciForchkSignForDPK.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkSignForDPK.Checked) || (lciForchkSignForDTT.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkSignForDTT.Checked))
                            {
                                if (printNow)
                                {    
                                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

                                    var allowedModules = HisConfigCFG.MODULELINKS.Split(',');

                                    if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS) && moduleData != null && allowedModules.Contains(moduleData.ModuleLink))
                                    {
                                        if (allowedModules.Contains(moduleData.ModuleLink))
                                        {
                                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                                        }
                                    }
                                    else
                                    {
                                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                                    }
                                }
                                else if (chkPreviewBeforePrint.Checked)
                                {
                                    previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;
                                }
                                else
                                    previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                            }
                            else if (printNow)
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

                                var allowedModules = HisConfigCFG.MODULELINKS.Split(',');

                                if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS) && moduleData != null && allowedModules.Contains(moduleData.ModuleLink))
                                {
                                    if (allowedModules.Contains(moduleData.ModuleLink))
                                    {
                                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                                    }
                                }
                                else
                                {
                                    previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                                }

                            }
                            else if (chkPreviewBeforePrint.Checked)
                            {
                                previewType = MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;
                            }

                            if (previewType != null)
                            {
                                this.PrescriptionSavePrintShowHasClickSave(printNow ? "" : MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode, printNow, previewType);
                            }
                            break;
                        case SAVETYPE.SAVE_PRINT_NOW:
                            this.PrescriptionSavePrintShowHasClickSave("", true, null);
                            break;
                        case SAVETYPE.SAVE_SHOW_PRINT_PREVIEW:
                            this.PrescriptionSavePrintShowHasClickSave(MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode, false);
                            break;
                    }


                    Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8");

                    //Nếu người dùng tích chọn kết thúc điều trị > Chọn "Lưu" = Lưu toa thuốc + kết thúc điều trị (tự động in giấy hẹn khám, bảng kê nếu tích chọn) + Tự động close form kê toa + xử lý khám (có option theo user).
                    if (isHasTreatmentFinishChecked)
                    {
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.1");
                        this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(this.treatmentId, GetInstructionTimeMedi().OrderByDescending(o => o).First());
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.2");
                        if (((treatUC.IsAutoPrintGHK || treatUC.IsSignGHK) && treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN) || treatUC.TreatmentEndTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                        {
                            this.ProcessAndPrintTreatmentEnd();
                        }
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.3");
                        if (currentTreatmentWithPatientType.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                        {
                            btnDichVuHenKham.Enabled = true;
                        }

                        if (currentTreatmentWithPatientType.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                        {
                            ReLoadPrintTreatmentEndTypeExt(PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI, printNow);
                        }
                        IsSignExamTreatmentFn = treatUC.IsSignExam;
                        IsPrintExamTreatmentFn = treatUC.IsPrintExam;
                        if (IsPrintExamTreatmentFn || IsSignExamTreatmentFn)
                        {
                            this.ProcessAndPrintExam();
                        }
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.4");
                        if (treatUC.IsAutoBK || treatUC.IsSignBK)
                        {
                            this.ProcessAndPrintBK(treatUC);
                        }
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.5");
                        if (treatUC.IsAutoPrintTL || treatUC.IsSignTL)
                        {
                            //in trích lục
                            this.ProcessAndPrintTL(treatUC);
                        }
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.6");
                        if (treatUC.IsAutoBANT)
                        {
                            this.ProcessAndPrintBANT();
                        }
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.7");

                        if (treatUC.IsPrintBHXH || treatUC.IsSignBHXH)
                        {
                            this.ProcessAndPrintBHXH(treatUC);
                        }

                        if (HisConfigCFG.HisPatientProgramNotHasEmrCoverTypeWarningOption == GlobalVariables.CommonStringTrue)
                        {
                            this.ProcessUpdateUIForHisPatientProgramNotHasEmrCoverTypeWarningOption(this.resultDataPrescription);
                        }

                        if (HisConfigCFG.IsAutoCloseFormWithAutoConfigTreatmentFinish)//TODO cần check với TH chọn nhiều BN kê
                        {
                            WaitingManager.Hide();
                            this.Close();
                            return;
                        }
                        Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.8.8");
                    }
                    Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.9");
                    if (GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                    {
                        this.patientSelectProcessor.ReloadStatePrescriptionPerious(this.ucPatientSelect);
                        this.OpionGroupSelectedChangedAsync();
                    }

                    this.FillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);

                    if (this.oldServiceReq != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        var logDatail = String.Join("||||", this.mediMatyTypeADOs.Select(o => o.MEDICINE_TYPE_NAME + "(" + o.MEDICINE_TYPE_CODE + ") -  Amount=" + o.AMOUNT + " - IsExpend = " + o.IsExpend + " - IsExpendType=" + o.IsExpendType));
                        Inventec.Common.Logging.LogSystem.Debug(String.Format("Tai khoan {0} da tao/sua don thuoc(ServiceReqCode ={1})   thanh cong, log chi tiet: {2}", loginName, this.oldServiceReq.SERVICE_REQ_CODE, logDatail));
                    }
                    Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.10");
                }
                else
                    this.ChangeLockButtonWhileProcess(true);

                WaitingManager.Hide();

                #region Show message
                this.successSaveList = successSaveList && success;
                if (this.bIsSelectMultiPatientProcessing && GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    if (!success)
                    {
                        if (paramSaveList == null) paramSaveList = new CommonParam();
                        if (paramSaveList.Messages == null) paramSaveList.Messages = new List<string>();

                        string mess = paramCommon.GetMessage();
                        if (!String.IsNullOrEmpty(paramCommon.GetBugCode()))
                        {
                            mess += "(" + paramCommon.GetBugCode() + ")";
                        }
                        if (!String.IsNullOrEmpty(mess))
                        {
                            paramSaveList.Messages.Add(String.Format("Bệnh nhân {0} - {1}", this.patientName, mess));
                            paramSaveList.Messages.Add("<br>");
                        }
                    }
                }
                else
                    MessageManager.Show(this, paramCommon, success);

                #endregion

                if (treatmentFinishProcessor != null && this.ucTreatmentFinish != null && isHasTreatmentFinishChecked && treatUC != null && treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN && treatUC.dtAppointmentTime == DateTime.MinValue)
                {
                    treatmentFinishProcessor.ShowPopupAppointmentControl(this.ucTreatmentFinish);
                }

                if (success)
                {
                    LockByKeyConfig();
                    Thread PortI3 = new Thread(CallPortI3);
                    PortI3.Start();
                    PortI3.Join();
                }
                Inventec.Common.Logging.LogSystem.Info("frmAssignPrescription.ProcessSaveData.11");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.ChangeLockButtonWhileProcess(true);
                WaitingManager.Hide();
                MessageManager.Show(this, paramCommon, false);
            }
        }
        private void LockByKeyConfig()
        {

            try
            {
                if (resultDataPrescription != null && HisConfigCFG.IsSaveButtonOption != "1")
                {
                    ChangeLockButtonWhileProcess(false);
                    btnAdd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private bool CheckMustChooseSeviceExamOption(string KeyMustChooseSeviceExam)
        {
            bool rs = true;
            try
            {
                HisServiceReqFilter srFilter = new HisServiceReqFilter();
                srFilter.TREATMENT_ID = currentTreatment.ID;
                srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                var serviceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);
                if (serviceReqs != null && serviceReqs.Count > 0)
                {
                    serviceReqs = serviceReqs.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_DELETE != 1 && string.IsNullOrEmpty(o.TDL_SERVICE_IDS)).ToList();
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        if ((KeyMustChooseSeviceExam == "1" && XtraMessageBox.Show(String.Format("Y lệnh {0} thiếu dịch vụ khám. Bạn có muốn tiếp tục", String.Join(", ", serviceReqs.Select(o => o.SERVICE_REQ_CODE))), "Thông báo", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes) || (KeyMustChooseSeviceExam == "2" && XtraMessageBox.Show(String.Format("Y lệnh {0} thiếu dịch vụ khám. Không cho phép kết thúc điều trị", String.Join(", ", serviceReqs.Select(o => o.SERVICE_REQ_CODE))), "Thông báo") == System.Windows.Forms.DialogResult.OK))
                            rs = false;
                    }
                }
            }
            catch (Exception ex)
            {
                rs = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }
        private async Task TaskUpdateObeyCantraindi(List<HIS_SERE_SERV> sereServ, List<HIS_EXP_MEST> expMests, List<HIS_SERVICE_REQ> serviceReq)
        {
            try
            {
                List<HIS_OBEY_CONTRAINDI> lstSend = new List<HIS_OBEY_CONTRAINDI>();
                if (ObeyContraindiSave == null || ObeyContraindiSave.Count == 0)
                    ObeyContraindiSave = new List<HIS_OBEY_CONTRAINDI>();
                if (ObeyContraindiEdit != null && ObeyContraindiEdit.Count > 0)
                {
                    ObeyContraindiSave.AddRange(ObeyContraindiEdit.Where(o => !ObeyContraindiSave.Exists(p => p.ID == o.ID)));
                }
                ObeyContraindiSave.ForEach(o =>
                {
                    if (sereServ != null && sereServ.Count > 0)
                    {
                        var serviceReqHasObey = sereServ.Where(p => p.SERVICE_ID == o.SERVICE_ID && serviceReq.Exists(k => k.ID == p.SERVICE_REQ_ID)).ToList();
                        if (serviceReqHasObey != null && serviceReqHasObey.Count() > 0)
                        {
                            if (string.IsNullOrEmpty(o.SERVICE_REQ_CODES))
                            {
                                o.SERVICE_REQ_CODES = String.Join(",", serviceReqHasObey.Select(p => p.TDL_SERVICE_REQ_CODE).Distinct());
                                if (expMests != null && expMests.Count > 0 && expMests.FirstOrDefault(p => serviceReqHasObey.Exists(k => k.SERVICE_REQ_ID == p.SERVICE_REQ_ID)) != null)
                                    o.EXP_MEST_CODES = String.Join(",", expMests.Where(p => serviceReqHasObey.Exists(k => k.SERVICE_REQ_ID == p.SERVICE_REQ_ID)).Select(p => p.EXP_MEST_CODE).Distinct());
                                lstSend.Add(o);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(o.SERVICE_REQ_CODES))
                            {
                                if (expMests != null && expMests.Count > 0 && expMests.FirstOrDefault(p => sereServ.Exists(k => k.SERVICE_REQ_ID == p.SERVICE_REQ_ID)) != null)
                                {
                                    o.EXP_MEST_CODES = String.Join(",", o.EXP_MEST_CODES.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Where(k => !expMests.Where(p => sereServ.Exists(i => i.SERVICE_REQ_ID == p.SERVICE_REQ_ID)).Select(p => p.EXP_MEST_CODE).ToList().Exists(p => p.Equals(k))).Distinct());
                                    o.SERVICE_REQ_CODES = String.Join(",", o.SERVICE_REQ_CODES.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Where(k => !expMests.Where(p => sereServ.Exists(i => i.SERVICE_REQ_ID == p.SERVICE_REQ_ID)).Select(p => p.TDL_SERVICE_REQ_CODE).ToList().Exists(p => p.Equals(k))).Distinct());
                                }
                                lstSend.Add(o);
                            }
                        }
                    }
                });
                if (lstSend != null && lstSend.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSend), lstSend));
                    ObeyContraindiEdit = new BackendAdapter(param).Post<List<HIS_OBEY_CONTRAINDI>>("api/HisObeyContraindi/UpdateList", ApiConsumers.MosConsumer, lstSend, ProcessLostToken, param);
                    MessageManager.ShowAlert(this, param, ObeyContraindiEdit != null && ObeyContraindiEdit.Count > 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CheckUseTime()
        {
            bool result = true;
            try
            {
                if (this.UseTimeSelecteds != null && this.UseTimeSelecteds.Count > 0 && this.InstructionTime != null)
                {
                    long intructionDateSelectedProcess = 0;

                    string intructionDate = InstructionTime.ToString().Substring(0, 8) + "000000";

                    if (long.TryParse(intructionDate, out intructionDateSelectedProcess))
                    {
                        foreach (var item in this.UseTimeSelecteds)
                        {
                            if (item < intructionDateSelectedProcess)
                            {
                                XtraMessageBox.Show(ResourceMessage.NgayDuTruKhongDuocNhoHonThoiGianChiDinh);
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckReasonRequied()
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.IsReasonRequired)
                {
                    if (actionType == GlobalVariables.ActionAdd)
                    {
                        if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                        {
                            var datareasion = this.mediMatyTypeADOs.Where(o => o.IsNotOutStock && o.EXP_MEST_REASON_ID == null).Select(p => p.MEDICINE_TYPE_NAME).ToList();

                            if (datareasion != null && datareasion.Count > 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.ThuocVatTuChuaNhapLyDoXuat, string.Join(", ", datareasion)),
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                    else if (actionType == GlobalVariables.ActionEdit)
                    {
                        if (this.lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            if (this.cboExpMestReason.EditValue == null && this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && this.mediMatyTypeADOs.Exists(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaNhapLyDoXuat,
                             HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                             MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.cboExpMestReason.Focus();
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckPayICD()
        {
            bool result = true;
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;

                if ((subIcd == null || (subIcd != null && string.IsNullOrEmpty(subIcd.ICD_SUB_CODE))) && (icdValue != null && !string.IsNullOrEmpty(icdValue.ICD_CODE)))
                {
                    if (!String.IsNullOrEmpty(HisConfigCFG.IcdCodeToApplyRestrictPatientTypeByOtherSourcePaid))
                    {
                        var IcdCodes = HisConfigCFG.IcdCodeToApplyRestrictPatientTypeByOtherSourcePaid.Split(',').ToList();
                        if (IcdCodes != null && IcdCodes.Count > 0)
                        {
                            if (IcdCodes.Contains(icdValue.ICD_CODE))
                            {
                                var checkData = (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0) ? this.mediMatyTypeADOs.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && o.OTHER_PAY_SOURCE_ID == null).Select(p => p.MEDICINE_TYPE_NAME).ToList() : null;
                                if (checkData != null && checkData.Count > 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BoSungThongTinChanDoanPhuHoacDoiDoiTuongThanhToan, string.Join(", ", checkData)),
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CreateRequestTicket(int actionType)
        {
            try
            {
                List<long> medicineTypeIds = this.mediMatyTypeADOs.Select(o => o.ID).ToList();

                var medicineTypeAcinByMety = ValidAcinInteractiveWorker.GetMedicineTypeAcinByMedicineType(medicineTypeIds);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListExpMestMedicineAntibioticRequired), ListExpMestMedicineAntibioticRequired));
                if (medicineTypeAcinByMety != null && medicineTypeAcinByMety.Count > 0 && ListExpMestMedicineAntibioticRequired != null && ListExpMestMedicineAntibioticRequired.Count > 0)
                {
                    var AcinIds = medicineTypeAcinByMety.Where(o => ListExpMestMedicineAntibioticRequired.Exists(p => p.TDL_MEDICINE_TYPE_ID == o.MEDICINE_TYPE_ID)).Select(o => o.ACTIVE_INGREDIENT_ID).ToList();
                    var acinIngredients = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>().Where(o => AcinIds.Contains(o.ID)).Where(o => o.IS_APPROVAL_REQUIRED == GlobalVariables.CommonNumberTrue).ToList();

                    if (acinIngredients != null && acinIngredients.Count > 0)
                    {
                        var ActiveIngredientName = acinIngredients.Select(o => o.ACTIVE_INGREDIENT_NAME).ToList();
                        if (XtraMessageBox.Show(string.Format(ResourceMessage.BanCoMuonTaoYeuCauKhong, string.Join(", ", ActiveIngredientName)), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AntibioticRequest").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AntibioticRequest'");
                            if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AntibioticRequest' is not plugins");

                            AntibioticRequestADO ado = new AntibioticRequestADO();

                            List<HIS_ANTIBIOTIC_NEW_REG> NewRegimen = new List<HIS_ANTIBIOTIC_NEW_REG>();
                            var acinIngredientID = acinIngredients.Select(o => o.ID).ToList();
                            var TypeAcinMedicineType = medicineTypeAcinByMety.Where(o => acinIngredientID.Contains(o.ACTIVE_INGREDIENT_ID)).Select(p => p.MEDICINE_TYPE_ID).ToList();
                            var medicineType = this.mediMatyTypeADOs.Where(o => TypeAcinMedicineType.Contains(o.ID)).ToList();
                            var ExpMestId = ListExpMestMedicineAntibioticRequired.Select(o => o.EXP_MEST_ID).Distinct().ToList();
                            foreach (var em in ExpMestId)
                            {
                                var medicineTypeExpMest = medicineType.Where(o => ListExpMestMedicineAntibioticRequired.Where(p => p.EXP_MEST_ID == em).ToList().Exists(p => p.TDL_MEDICINE_TYPE_ID == o.ID)).ToList();
                                foreach (var item in medicineTypeExpMest)
                                {
                                    var acinIngredient = medicineTypeAcinByMety.Where(o => o.MEDICINE_TYPE_ID == item.ID).ToList();

                                    foreach (var iAcin in acinIngredient)
                                    {
                                        HIS_ANTIBIOTIC_NEW_REG NewReg = new HIS_ANTIBIOTIC_NEW_REG();
                                        NewReg.DOSAGE = item.TUTORIAL;
                                        NewReg.USE_FORM = item.MEDICINE_USE_FORM_NAME;
                                        NewReg.USE_DAY = item.UseDays;
                                        NewReg.ACTIVE_INGREDIENT_ID = iAcin != null ? iAcin.ACTIVE_INGREDIENT_ID : 0;
                                        NewReg.CONCENTRA = item.CONCENTRA;

                                        NewRegimen.Add(NewReg);
                                    }
                                }

                                if (actionType == GlobalVariables.ActionAdd)
                                {
                                    ado.PatientCode = currentTreatmentWithPatientType.TDL_PATIENT_CODE;
                                    ado.PatientName = currentTreatmentWithPatientType.TDL_PATIENT_NAME;
                                    ado.Dob = currentTreatmentWithPatientType.TDL_PATIENT_DOB;
                                    ado.IsHasNotDayDob = currentTreatmentWithPatientType.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1;
                                    ado.GenderName = currentTreatmentWithPatientType.TDL_PATIENT_GENDER_NAME;
                                    ado.Temperature = (decimal?)spinTemperature.EditValue;
                                    ado.Weight = (decimal?)spinWeight.EditValue;
                                    ado.Height = (decimal?)spinHeight.EditValue;
                                    ado.IcdSubCode = txtIcdSubCode.Text;
                                    ado.IcdText = txtIcdText.Text;
                                    ado.NewRegimen = new List<HIS_ANTIBIOTIC_NEW_REG>();
                                    ado.NewRegimen = NewRegimen;
                                    ado.ExpMestId = em ?? 0;
                                    ado.processType = HIS.Desktop.ADO.AntibioticRequestADO.ProcessType.Request;
                                    ado.InstructionDate = this.intructionTimeSelecteds.OrderByDescending(o => o).Last();

                                }
                                else if (actionType == GlobalVariables.ActionEdit)
                                {
                                    V_HIS_ANTIBIOTIC_REQUEST AntibioticRequest = new V_HIS_ANTIBIOTIC_REQUEST();
                                    if (em != null)
                                    {
                                        CommonParam param = new CommonParam();
                                        HisAntibioticRequestViewFilter filter = new HisAntibioticRequestViewFilter();
                                        filter.ID = ListExpMestResult.FirstOrDefault(o => o.ID == (em ?? 0)).ANTIBIOTIC_REQUEST_ID;
                                        AntibioticRequest = new BackendAdapter(param).Get<List<V_HIS_ANTIBIOTIC_REQUEST>>("api/HisAntibioticRequest/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                                    }

                                    ado.AntibioticRequest = new V_HIS_ANTIBIOTIC_REQUEST();
                                    ado.AntibioticRequest = AntibioticRequest;
                                    ado.NewRegimen = new List<HIS_ANTIBIOTIC_NEW_REG>();
                                    ado.NewRegimen = NewRegimen;
                                    ado.ExpMestId = em ?? 0;
                                    ado.processType = HIS.Desktop.ADO.AntibioticRequestADO.ProcessType.Request;
                                    ado.InstructionDate = this.intructionTimeSelecteds.OrderByDescending(o => o).Last();
                                }

                                List<object> listArgs = new List<object>();
                                listArgs.Add(ado);
                                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                                ((Form)extenceInstance).ShowDialog();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallPortI3()
        {
            try
            {
                HIS.Desktop.Plugins.Library.DrugInterventionInfo.DrugInterventionInfoProcessor CallI3 = new Library.DrugInterventionInfo.DrugInterventionInfoProcessor(HisConfigCFG.ConnectionInfo, VHistreatment);
                CallI3.ReleasePrescription(this.inPrescriptionResultSDOs, this.outPrescriptionResultSDOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// - Khi nhấn nút "Lưu", nếu tồn tại dịch vụ chưa chọn "Nguồn khác" thì hiển thị cảnh báo: "Bạn chưa chọn "Nguồn khác" đối với dịch vụ XXX, YYY", không cho phép lưu.
        //Trong đó: XXX, YYY là tên các dịch vụ chưa chọn nguồn khác và có Đối tượng thanh toán có cấu hình "Nguồn khác"
        //- Khi lưu, gửi thông tin nguồn khác vào OtherPaySourceId vào SDO để xử lý lưu thông tin vào server
        /// </summary>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool ValidSereServWithOtherPaySource(List<MediMatyTypeADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    string sereServOtherpaysourceStr = "";
                    foreach (var item in serviceCheckeds__Send)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == item.PATIENT_TYPE_ID).FirstOrDefault();
                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS) && (item.OTHER_PAY_SOURCE_ID ?? 0) <= 0)
                        {
                            sereServOtherpaysourceStr += item.MEDICINE_TYPE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServOtherpaysourceStr))
                    {
                        sereServOtherpaysourceStr = sereServOtherpaysourceStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SereServOtherpaySourceAlert__DVChuaDuocNhapNguonChiTra, sereServOtherpaysourceStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("ValidSereServWithOtherPaySource: valid = false_____" + sereServOtherpaysourceStr);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        /// <summary>
        /// - Khi người dùng nhấn "Lưu", nếu kiểm tra tồn tại 1 dịch vụ được check chọn là dịch vụ điều kiện (HIS_SERVICE có IS_CONDITIONED = 1) nhưng người dùng không chọn điều kiện (cột "Điều kiện" không chọn giá trị) thì hiển thị cảnh báo và không cho lưu:
        ///"Dịch vụ XXXX, YYYY chưa được nhập điều kiện" (trong đó XXXX, YYYY là tên các dịch vụ điều kiện chưa được nhập điều kiện)
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        private bool ValidSereServWithCondition()
        {
            bool valid = true;
            try
            {
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    string sereServConditionStr = "";
                    foreach (var item in mediMatyTypeADOs)
                    {
                        long patientTypeId = (item.PATIENT_TYPE_ID > 0 ? item.PATIENT_TYPE_ID ?? 0 : this.currentHisPatientTypeAlter != null ? this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : 0);
                        var dataCondition = (workingServiceConditions != null && patientTypeId == HisConfigCFG.PatientTypeId__BHYT) ? workingServiceConditions.Where(o => o.SERVICE_ID == item.SERVICE_ID).ToList() : null;
                        if (dataCondition != null && dataCondition.Count > 0)
                        {
                            List<HIS_SERVICE_CONDITION> dataConditionTmps = new List<HIS_SERVICE_CONDITION>();
                            foreach (var itemCon in dataCondition)
                            {
                                if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == itemCon.SERVICE_CONDITION_NAME && t.HEIN_RATIO == itemCon.HEIN_RATIO))
                                {
                                    dataConditionTmps.Add(itemCon);
                                }
                            }
                            dataCondition.Clear();
                            dataCondition.AddRange(dataConditionTmps);

                        }
                        else
                        {
                            dataCondition = null;
                        }
                        if (dataCondition != null && dataCondition.Count > 0 && (item.SERVICE_CONDITION_ID ?? 0) <= 0)
                        {
                            sereServConditionStr += item.MEDICINE_TYPE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServConditionStr))
                    {
                        sereServConditionStr = sereServConditionStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SereServConditionAlert__DVChuaDuocNhapDieuKien, sereServConditionStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("ValidSereServWithCondition: valid = false_____" + sereServConditionStr);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }


        //4. Sửa chức năng "Kê đơn": Khi click vào checkbox "Kết thúc điều trị":
        //a. Sau khi lưu xử trí Kết thúc điều trị thành công:
        //- Nếu key cấu hình HIS.DESKTOP.HIS_PATIENT_PROGRAM.NOT_HAS_EMR_COVER_TYPE.WARNING_OPTION có giá trị 1 và hồ sơ có thông tin chương trình (PROGRAM_ID trong HIS_TREATMENT có giá trị) và chưa có thông tin Vỏ bệnh án (EMR_COVER_TYPE_ID trong HIS_TREATMENT không có giá trị) thì show cảnh báo: "Bệnh nhân chương trình chưa được tạo Vỏ bệnh án".
        //b. Bổ sung button "Tạo vỏ bệnh án". Button này được enable khi thỏa mãn các điều kiện:
        //- Sau khi lưu kê đơn kết thúc điều trị thành công.
        //- Hồ sơ có thông tin chương trình (PROGRAM_ID trong HIS_TREATMENT có giá trị).
        //- Hồ sơ chưa có thông tin loại vỏ bệnh án (EMR_COVER_TYPE_ID trong HIS_TREATMENT không có giá trị).
        //- Khi click nút thì xử lý như sau (Xử lý giống nút "Tạo vỏ bệnh án" ở màn hình "Xử lý khám"):
        //+ Lấy tất cả dữ liệu HIS_EMR_COVER_CONFIG thỏa mãn ROOM_ID theo phòng đang làm việc, TREATMENT_TYPE_ID theo diện điều trị của hồ sơ.
        //++ Trường hợp chỉ có 1 bản ghi thì truyền vào thư viện generate menu VBA loại vỏ bệnh án (EmrCoverTypeId) như hiện tại.
        //++ Trường hợp có nhiều hơn 1 bản ghi thì truyền vào thư viện generate menu VBA danh sách loại vỏ bệnh án lấy được đấy.
        //+ Trường hợp không có dữ liệu thì tiếp tục lấy HIS_EMR_COVER_CONFIG thỏa mãn DEPARTMENT_ID theo khoa đang làm việc, TREATMENT_TYPE_ID theo diện điều trị của hồ sơ.
        //++ Trường hợp chỉ có 1 bản ghi thì truyền vào thư viện generate menu VBA loại vỏ bệnh án (EmrCoverTypeId) như hiện tại.
        //++ Trường hợp có nhiều hơn 1 bản ghi thì truyền vào thư viện generate menu VBA danh sách loại vỏ bệnh án lấy được đấy.
        private void ProcessUpdateUIForHisPatientProgramNotHasEmrCoverTypeWarningOption(object rsData)
        {
            try
            {
                if (rsData != null)
                {
                    LoadEmrCoverConfig();

                    if (this.treatmentData != null && this.treatmentData.PROGRAM_ID.HasValue && this.treatmentData.PROGRAM_ID.Value > 0 && (this.treatmentData.EMR_COVER_TYPE_ID ?? 0) <= 0)
                    {
                        if (HisConfigCFG.NotHasEmrCoverTypeWarningOption == "1")
                        {
                            DialogResult myResult;
                            myResult = XtraMessageBox.Show(ResourceMessage.BenhNhanChuongTrinhChuaDuocTaoVoBenhAnBanCoMuonTaoVBDHayKhong, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True);
                            if (myResult == DialogResult.Yes)
                            {
                                CreateEMRVBAOnClick();
                            }
                            Inventec.Common.Logging.LogSystem.Debug(ResourceMessage.BenhNhanChuongTrinhChuaDuocTaoVoBenhAnBanCoMuonTaoVBDHayKhong);
                        }
                        else
                        {
                            MessageManager.Show(ResourceMessage.BenhNhanChuongTrinhChuaDuocTaoVoBenhAn);
                            Inventec.Common.Logging.LogSystem.Debug(ResourceMessage.BenhNhanChuongTrinhChuaDuocTaoVoBenhAn);
                        }

                        btnCreateVBA.Enabled = true;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.HisPatientProgramNotHasEmrCoverTypeWarningOption), HisConfigCFG.HisPatientProgramNotHasEmrCoverTypeWarningOption)
                   );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CreateEMRVBAOnClick()
        {
            try
            {
                if (this.resultDataPrescription != null)
                {
                    if (this._Menu == null)
                        this._Menu = new PopupMenu(this.barManager1);
                    this._Menu.ItemLinks.Clear();
                    if (this.emrMenuPopupProcessor == null) this.emrMenuPopupProcessor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();

                    HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdoCreateVBA = new Library.FormMedicalRecord.Base.EmrInputADO();
                    emrInputAdoCreateVBA.TreatmentId = this.treatmentId;
                    emrInputAdoCreateVBA.PatientId = this.currentTreatmentWithPatientType.PATIENT_ID;
                    if (LstEmrCoverConfig != null && LstEmrCoverConfig.Count > 0)
                    {
                        if (LstEmrCoverConfig.Count == 1)
                        {
                            emrInputAdoCreateVBA.EmrCoverTypeId = LstEmrCoverConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                        }
                        else
                        {
                            emrInputAdoCreateVBA.lstEmrCoverTypeId = new List<long>();
                            emrInputAdoCreateVBA.lstEmrCoverTypeId = LstEmrCoverConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                        }
                    }
                    else
                    {
                        if (LstEmrCoverConfigDepartment != null && LstEmrCoverConfigDepartment.Count > 0)
                        {
                            if (LstEmrCoverConfigDepartment.Count == 1)
                            {
                                emrInputAdoCreateVBA.EmrCoverTypeId = LstEmrCoverConfigDepartment.FirstOrDefault().EMR_COVER_TYPE_ID;
                            }
                            else
                            {
                                emrInputAdoCreateVBA.lstEmrCoverTypeId = new List<long>();
                                emrInputAdoCreateVBA.lstEmrCoverTypeId = LstEmrCoverConfigDepartment.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                            }
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Info("emrInputAdoCreateVBA: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputAdoCreateVBA), emrInputAdoCreateVBA));

                    this.emrMenuPopupProcessor.InitMenuButton(this._Menu, this.barManager1, emrInputAdoCreateVBA);
                    this._Menu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadEmrCoverConfig()
        {
            try
            {
                LstEmrCoverConfig = new List<HIS_EMR_COVER_CONFIG>();
                LstEmrCoverConfigDepartment = new List<HIS_EMR_COVER_CONFIG>();

                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = this.treatmentId;
                this.treatmentData = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                LstEmrCoverConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.ROOM_ID == currentModule.RoomId && o.TREATMENT_TYPE_ID == this.currentTreatmentWithPatientType.TDL_TREATMENT_TYPE_ID
                    ).ToList();
                Inventec.Common.Logging.LogSystem.Info("LstEmrCoverConfig: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => LstEmrCoverConfig), LstEmrCoverConfig));

                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModule.RoomId).DepartmentId;

                LstEmrCoverConfigDepartment = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == this.currentTreatmentWithPatientType.TDL_TREATMENT_TYPE_ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool CheckSignStateByPresType()
        {
            bool ischecked = false;
            try
            {
                if ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || GlobalStore.IsExecutePTTT)
                {
                    ischecked = chkSignForDDT.Checked;
                }
                else
                {
                    if (GlobalStore.IsCabinet)
                    {
                        ischecked = chkSignForDTT.Checked;
                    }
                    else
                    {
                        ischecked = chkSignForDPK.Checked;
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return ischecked;
        }

        List<string> GetExcludePrintTypeByBussiness()
        {
            List<string> results = new List<string>();
            try
            {
                var treatUC = ((this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null) ? this.treatmentFinishProcessor.GetDataOutput(this.ucTreatmentFinish) : null);
                if (treatUC != null && treatUC.IsAutoTreatmentFinish)
                {
                    if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        results.Add(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010);
                    }
                    else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)//Hen kham
                    {
                        results.Add(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008);
                        results.Add(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011);
                    }
                    else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)//ra vien
                    {
                        results.Add(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010);
                        results.Add(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }

        private async Task ReLoadPrintTreatmentEndTypeExt(PrintTreatmentEndTypeExPrintType.TYPE exPrintType, bool printNow = false)
        {
            try
            {
                PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatmentId, ReloadMenuTreatmentEndTypeExt, CreateMenu.TYPE.DYNAMIC, currentModule != null ? currentModule.RoomId : 0);

                printTreatmentEndTypeExtProcessor.Print(exPrintType,
                    HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.PRINT__INIT_MENU, printNow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadMenuTreatmentEndTypeExt(object data)
        {
            try
            {

                MenuPrintADO menuPrintADO = data as MenuPrintADO;
                if (menuPrintADO == null)
                {
                    return;
                }

                if (this.menuPrintADOs == null)
                    this.menuPrintADOs = new List<MenuPrintADO>();
                HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                menuPrintADOs.Add(menuPrintADO);

                HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>());
                menuPrintInitADO.MinSizeHeight = this.lcibtnSave.MinSize.Height;
                menuPrintInitADO.MaxSizeHeight = this.lcibtnSave.MaxSize.Height;
                menuPrintInitADO.ControlContainer = this.layoutControlPrintAssignPrescription;
                var menuResultADO = menuPrintProcessor.Run(menuPrintInitADO) as MenuPrintResultADO;
                if (menuResultADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                }
                //this.layoutControlPrintAssignPrescription.Update();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessAfterSaveForIn(ISave isave, bool isSaveAndPrint, InPatientPresResultSDO rsIn)
        {
            bool success = false;
            this.inPrescriptionResultSDOs = (rsIn as InPatientPresResultSDO);
            if (this.inPrescriptionResultSDOs != null
                && this.inPrescriptionResultSDOs.ServiceReqs != null && this.inPrescriptionResultSDOs.ServiceReqs.Count > 0
                && ((this.inPrescriptionResultSDOs.ServiceReqMaties != null && this.inPrescriptionResultSDOs.ServiceReqMaties.Count > 0)
                    || (this.inPrescriptionResultSDOs.ServiceReqMeties != null && this.inPrescriptionResultSDOs.ServiceReqMeties.Count > 0)
                    || (this.inPrescriptionResultSDOs.Materials != null && this.inPrescriptionResultSDOs.Materials.Count > 0)
                    || (this.inPrescriptionResultSDOs.Medicines != null && this.inPrescriptionResultSDOs.Medicines.Count > 0))
                )
            {
                try
                {
                    if (this.processDataResult != null)
                        this.processDataResult(this.inPrescriptionResultSDOs);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Goi ham cap nhat du lieu tu delegate module goi vao that bai (call processDataResult fail)", ex);
                }

                if (this.processRefeshIcd != null)
                    this.processRefeshIcd(this.inPrescriptionResultSDOs.ServiceReqs[0].ICD_CODE, this.inPrescriptionResultSDOs.ServiceReqs[0].ICD_NAME, this.inPrescriptionResultSDOs.ServiceReqs[0].ICD_SUB_CODE, this.inPrescriptionResultSDOs.ServiceReqs[0].ICD_TEXT);
                success = true;
            }
            return success;
        }

        private bool ProcessAfterSaveForOut(ISave isave, bool isSaveAndPrint, OutPatientPresResultSDO rsOut)
        {
            bool success = false;
            this.outPrescriptionResultSDOs = (rsOut as OutPatientPresResultSDO);
            if (this.outPrescriptionResultSDOs != null
                && this.outPrescriptionResultSDOs.ServiceReqs != null && this.outPrescriptionResultSDOs.ServiceReqs.Count > 0
                && ((this.outPrescriptionResultSDOs.ServiceReqMaties != null && this.outPrescriptionResultSDOs.ServiceReqMaties.Count > 0)
                    || (this.outPrescriptionResultSDOs.ServiceReqMeties != null && this.outPrescriptionResultSDOs.ServiceReqMeties.Count > 0)
                    || (this.outPrescriptionResultSDOs.Materials != null && this.outPrescriptionResultSDOs.Materials.Count > 0)
                    || (this.outPrescriptionResultSDOs.Medicines != null && this.outPrescriptionResultSDOs.Medicines.Count > 0))
                )
            {
                try
                {
                    if (this.processDataResult != null)
                        this.processDataResult(this.outPrescriptionResultSDOs);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Goi ham cap nhat du lieu tu delegate module goi vao that bai (call processDataResult fail)", ex);
                }

                if (this.processRefeshIcd != null)
                    this.processRefeshIcd(this.outPrescriptionResultSDOs.ServiceReqs[0].ICD_CODE, this.outPrescriptionResultSDOs.ServiceReqs[0].ICD_NAME, this.outPrescriptionResultSDOs.ServiceReqs[0].ICD_SUB_CODE, this.outPrescriptionResultSDOs.ServiceReqs[0].ICD_TEXT);
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Nếu người dùng tích chọn kết thúc điều trị -> Chọn "Lưu" = Lưu toa thuốc + kết thúc điều trị (tự động in giấy hẹn khám, bảng kê nếu tích chọn) + Tự động close form kê toa + xử lý khám (có option theo user).
        /// </summary>
        private void ProcessAndPrintTreatmentEnd()
        {
            try
            {
                HIS_TREATMENT treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

                PrintTreatmentFinishProcessor printTreatmentFinishProcessor = null;
                var treatUC = treatmentFinishProcessor.GetDataOutput(ucTreatmentFinish);
                if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                {
                    MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null;

                    if (treatUC.IsSignGHK)
                    {
                        if (treatUC.IsAutoPrintGHK)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                        }
                        else
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else if (treatUC.IsAutoPrintGHK)
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                    }

                    if (previewType != null)
                    {
                        printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, (currentModule != null ? currentModule.RoomId : 0), previewType.Value);
                    }
                    else
                        printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
                    printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode);
                }
                else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                {
                    printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, LoadServiceReq(treatment.ID), currentModule != null ? currentModule.RoomId : 0);
                    printTreatmentFinishProcessor.Print(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode);
                }
                else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                {
                    //Nothing
                }
                else if ((currentTreatmentWithPatientType.TDL_TREATMENT_TYPE_ID ?? 0) != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
                    printTreatmentFinishProcessor.Print(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HIS_SERVICE_REQ LoadServiceReq(long id)
        {
            HIS_SERVICE_REQ serviceReqExamEndType = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.IS_ACTIVE = 1;
                filter.TREATMENT_ID = id;
                var dt = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                if (dt != null && dt.Count > 0 && dt.FirstOrDefault(o => o.EXAM_END_TYPE == 3) != null)
                    serviceReqExamEndType = dt.FirstOrDefault(o => o.EXAM_END_TYPE == 3);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return serviceReqExamEndType;
        }
        private void ProcessAndPrintBANT()
        {
            try
            {
                HIS_TREATMENT treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

                PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
                printTreatmentFinishProcessor.Print(PrintEnum.IN_BANT__MPS000174);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessAndPrintExam()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000007", DelegateRunPrinterPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAndPrintBK(HIS.UC.TreatmentFinish.ADO.DataOutputADO dataOutputADO)
        {
            try
            {
                V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, this.currentTreatmentWithPatientType);

                BordereauInitData bordereauInitData = new BordereauInitData();
                bordereauInitData.Treatment = treatment;

                Library.PrintBordereau.Base.PrintOption.Value vlPrintOption = Library.PrintBordereau.Base.PrintOption.Value.PRINT_NOW;
                if (dataOutputADO.IsSignBK)
                {
                    if (dataOutputADO.IsAutoBK)
                    {
                        vlPrintOption = Library.PrintBordereau.Base.PrintOption.Value.PRINT_NOW_AND_EMR_SIGN_NOW;
                    }
                    else
                        vlPrintOption = Library.PrintBordereau.Base.PrintOption.Value.EMR_SIGN_NOW;
                }
                else if (dataOutputADO.IsAutoBK)
                {
                    vlPrintOption = Library.PrintBordereau.Base.PrintOption.Value.PRINT_NOW;
                }

                PrintBordereauProcessor printBordereauProcessor = new PrintBordereauProcessor(this.currentTreatmentWithPatientType.ID, this.currentTreatmentWithPatientType.PATIENT_ID, bordereauInitData, null);
                printBordereauProcessor.Print(vlPrintOption);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ProcessAndPrintTL(UC.TreatmentFinish.ADO.DataOutputADO dataOutputADO)
        {
            try
            {
                //HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                //CommonParam param = new CommonParam();
                //HisServiceReqFilter filter = new HisServiceReqFilter();
                //filter.TREATMENT_ID = this.treatmentId;
                //var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                //if (serviceReqs != null && serviceReqs.Count > 0)
                //{
                //    req = serviceReqs.FirstOrDefault();
                //}

                var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(this.currentModule.RoomId, this.treatmentId);
                Inventec.Common.Logging.LogSystem.Fatal("Phiếu Trích Phụ Lục MPS000316 ( UC_TREATMENT_FINISH) _____ 2");
                if (dataOutputADO.IsAutoPrintTL && !dataOutputADO.IsSignTL)
                {
                    printTest.Print("Mps000316", MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow);
                }
                else if (dataOutputADO.IsAutoPrintTL && dataOutputADO.IsSignTL)
                {
                    printTest.Print("Mps000316", MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow);

                }
                else if (!dataOutputADO.IsAutoPrintTL && dataOutputADO.IsSignTL)
                {
                    printTest.Print("Mps000316", MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow);
                }
                Inventec.Common.Logging.LogSystem.Fatal("Phiếu Trích Phụ Lục MPS000316 ( UC_TREATMENT_FINISH) _____ 3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ValidAddRow(object data)
        {
            bool valid = true;
            try
            {
                if (this.actionBosung == GlobalVariables.ActionView) return false;

                this.positionHandle = -1;
                if (!this.dxValidProviderBoXung.Validate())
                {
                    Inventec.Common.Logging.LogSystem.Debug("validate add row fail");
                    valid = false;
                }
                if (data == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ValidAddRow -> data is null");
                    MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc));
                    valid = false;
                }
                if (this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
                {
                    MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, Inventec.Common.DateTime.Convert.TimeNumberToDateString(InstructionTime)));
                    valid = false;
                    this.UcDateFocusControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        void ProcessAndPrintBHXH(HIS.UC.TreatmentFinish.ADO.DataOutputADO dataOutputADO)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                if (dataOutputADO.IsPrintBHXH && !dataOutputADO.IsSignBHXH)
                {
                    richEditorMain.RunPrintTemplate("Mps000298", DelegateRunPrinterPrint);
                }
                else if (!dataOutputADO.IsPrintBHXH && dataOutputADO.IsSignBHXH)
                {
                    richEditorMain.RunPrintTemplate("Mps000298", DelegateRunPrinterSign);
                }
                else if (dataOutputADO.IsPrintBHXH && dataOutputADO.IsSignBHXH)
                {
                    richEditorMain.RunPrintTemplate("Mps000298", DelegateRunPrinterSignAndPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000298":
                        Mps000298(1, printTypeCode, fileName, ref result);
                        break;
                    case "Mps000007":
                        Mps000007(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private V_HIS_PATIENT_TYPE_ALTER LoadPatientTypeAlter()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = currentTreatmentWithPatientType.ID;
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.LOG_TIME).FirstOrDefault();

                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == (currentTreatmentWithPatientType.IN_TREATMENT_TYPE_ID ?? 0));
                if (treatmentType != null && patientTypeAlter != null)
                {
                    patientTypeAlter.TREATMENT_TYPE_ID = treatmentType.ID;
                    patientTypeAlter.TREATMENT_TYPE_CODE = treatmentType.TREATMENT_TYPE_CODE;
                    patientTypeAlter.TREATMENT_TYPE_NAME = treatmentType.TREATMENT_TYPE_NAME;
                }
                return patientTypeAlter;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> ClsSereServ;
        private void LoadClsSereServ()
        {
            try
            {
                WaitingManager.Show();
                HisSereServFilter ClsSereServFilter = new HisSereServFilter();
                ClsSereServFilter.TREATMENT_ID = currentTreatmentWithPatientType.ID;
                ClsSereServFilter.TDL_SERVICE_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                };
                ClsSereServFilter.HAS_EXECUTE = true;
                CommonParam param = new CommonParam();
                this.ClsSereServ = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ClsSereServFilter, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans;
        private void LoadDepartmentTran()
        {
            try
            {
                CommonParam param = new CommonParam();
                //Lấy thông tin chuyển khoa
                HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = treatmentId;
                departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        V_HIS_SERVICE_REQ vServiceReq;
        private void LoadVServiceReq()
        {
            try
            {
                if (serviceReqParentId == null)
                    return;
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = serviceReqParentId;
                vServiceReq = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Mps000007(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                List<Action> methods = new List<Action>();
                methods.Add(LoadClsSereServ);
                methods.Add(LoadDepartmentTran);
                methods.Add(LoadVServiceReq);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                WaitingManager.Hide();


                CommonParam param = new CommonParam();
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = VHistreatment.PATIENT_ID;
                var patients = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param);

                V_HIS_PATIENT patient = patients.FirstOrDefault();
                var patientTypeAlter = LoadPatientTypeAlter();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                string executeRoomName = "";
                string executeDepartmentName = "";
                string hospitalizeDepartmentCode = "";
                string hospitalizeDepartmentName = "";

                executeRoomName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).RoomName;

                var executeDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == this.oldServiceReq.EXECUTE_DEPARTMENT_ID);
                var hospitalizeDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == VHistreatment.HOSPITALIZE_DEPARTMENT_ID);
                if (executeDepartment != null)
                {
                    executeDepartmentName = executeDepartment.DEPARTMENT_NAME;
                }
                if (hospitalizeDepartment != null)
                {
                    hospitalizeDepartmentCode = hospitalizeDepartment.DEPARTMENT_CODE;
                    hospitalizeDepartmentName = hospitalizeDepartment.DEPARTMENT_NAME;
                }
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                string ratio_text = "";
                if (patientTypeAlter != null)
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);

                MPS.Processor.Mps000007.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000007.PDO.SingleKeyValue();
                singleKeyValue.ExecuteRoomName = executeRoomName;
                singleKeyValue.ExecuteDepartmentName = executeDepartmentName;
                singleKeyValue.RatioText = ratio_text;
                singleKeyValue.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKeyValue.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                singleKeyValue.HospitalizeDepartmentCode = hospitalizeDepartmentCode;
                singleKeyValue.HospitalizeDepartmentName = hospitalizeDepartmentName;
                if (VHistreatment.ICD_NAME != null)
                {
                    singleKeyValue.Icd_Name = VHistreatment.ICD_NAME;
                }

                var ExamRoomList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == 1).ToList();
                List<V_HIS_EXP_MEST_BLOOD> ExpMestBloodList = new List<V_HIS_EXP_MEST_BLOOD>();
                List<V_HIS_EXP_MEST_BLTY_REQ> ExpMestBltyReqList = new List<V_HIS_EXP_MEST_BLTY_REQ>();
                List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicineList = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterialList = new List<V_HIS_EXP_MEST_MATERIAL>();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.REQ_ROOM_IDs = ExamRoomList.Select(o => o.ROOM_ID).Distinct().ToList();
                expMestFilter.TDL_TREATMENT_ID = VHistreatment.ID;
                expMestFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                var expMestList = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, null);
                if (expMestList != null && expMestList.Count > 0)
                {
                    MOS.Filter.HisExpMestBloodViewFilter expMestBloodFilter = new HisExpMestBloodViewFilter();
                    expMestBloodFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestBloodList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestBloodFilter, null);

                    MOS.Filter.HisExpMestBltyReqViewFilter expMestBltyReqFilter = new HisExpMestBltyReqViewFilter();
                    expMestBltyReqFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestBltyReqList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestBltyReqFilter, null);

                    MOS.Filter.HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestMedicineList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, null);

                    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_IDs = expMestList.Select(o => o.ID).ToList();
                    ExpMestMaterialList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialFilter, null);
                }
                MPS.Processor.Mps000007.PDO.Mps000007PDO rdo = new MPS.Processor.Mps000007.PDO.Mps000007PDO(
                    patient,
                    patientTypeAlter,
                    departmentTrans,
                    vServiceReq,
                    dhst,
                    VHistreatment,
                    ClsSereServ,
                    singleKeyValue,
                    ExpMestBloodList,
                    ExpMestBltyReqList,
                    ExpMestMedicineList,
                    ExpMestMaterialList
                    );

                MPS.ProcessorBase.PrintConfig.PreviewType PreviewType;
                if (this.IsSignExamTreatmentFn && this.IsPrintExamTreatmentFn)
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                }
                else if (this.IsSignExamTreatmentFn)
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                }
                else
                {
                    PreviewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(VHistreatment != null ? VHistreatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, PreviewType, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }


        bool DelegateRunPrinterSign(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Mps000298(2, printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool DelegateRunPrinterSignAndPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Mps000298(3, printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// PrintOrSign = 1: in
        /// PrintOrSign = 2: ký
        /// PrintOrSign = 3: in và ký
        /// </summary>
        /// <param name="PrintOrSign"></param>
        /// <param name="printTypeCode"></param>
        /// <param name="fileName"></param>
        /// <param name="result"></param>
        private void Mps000298(long PrintOrSign, string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, this.currentTreatmentWithPatientType);

                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = treatment.ID;
                var lstHisSereServ = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);

                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatment.ID;
                var lstPatientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>(RequestUriStore.HIS_PATIENT_TYPE_ALTER__GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);

                HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                HIS_PATIENT Patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                HIS_SERE_SERV HisSereServ = new HIS_SERE_SERV();
                V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                if (lstHisSereServ != null && lstHisSereServ.Count > 0)
                {
                    HisSereServ = lstHisSereServ.FirstOrDefault();
                }

                if (lstPatientTypeAlter != null && lstPatientTypeAlter.Count > 0)
                {
                    PatientTypeAlter = lstPatientTypeAlter.FirstOrDefault();
                }

                Inventec.Common.Logging.LogSystem.Info("treatment.TREATMENT_CODE: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment.TREATMENT_CODE), treatment.TREATMENT_CODE));

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, (this.currentModule != null ? this.currentModule.RoomId : 0));

                Inventec.Common.Logging.LogSystem.Info("inputADO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO), inputADO));
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.Processor.Mps000298.PDO.Mps000298PDO mps000298RDO = new MPS.Processor.Mps000298.PDO.Mps000298PDO(
                    treatment,
                    PatientTypeAlter,
                    Patient,
                    HisSereServ
                    );

                WaitingManager.Hide();

                if (PrintOrSign == 1)
                {
                    Inventec.Common.Logging.LogSystem.Warn(" PrintOrSign == 1 in ");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000298RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else if (PrintOrSign == 2)
                {
                    Inventec.Common.Logging.LogSystem.Warn(" PrintOrSign == 2 ký ");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000298RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName) { EmrInputADO = inputADO });
                }
                else if (PrintOrSign == 3)
                {
                    Inventec.Common.Logging.LogSystem.Warn(" PrintOrSign == 3 ký và in ");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000298RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, printerName) { EmrInputADO = inputADO });
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
