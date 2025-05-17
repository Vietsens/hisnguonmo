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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Save;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void ValidDataMediMaty()
        {
            try
            {
                var instructionTimeMedis = GetInstructionTimeMedi();
                foreach (var item in this.mediMatyTypeADOs)
                {
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
                        item.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                        item.ErrorTypePatientTypeId = ErrorType.Warning;
                    }
                    else
                    {
                        item.ErrorMessagePatientTypeId = "";
                        item.ErrorTypePatientTypeId = ErrorType.None;
                    }

                    if (item.AMOUNT <= 0)
                    {
                        item.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
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

                this.btnSave.Enabled = isLock;
                this.btnSaveAndPrint.Enabled = isLock;
                this.btnSaveAndShowPrintPreview.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionCLS.SAVETYPE saveType)
        {
            try
            {
                bool valid = true;
                this.positionHandleControl = -1;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                paramCommon = new CommonParam();
                this.mediMatyTypeADOs = this.gridViewServiceProcess.DataSource as List<MediMatyTypeADO>;

                //Inventec.Common.Logging.LogSystem.Debug("Du lieu thuoc/vat tu da chon de ke truoc khi xu ly luu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.mediMatyTypeADOs), this.mediMatyTypeADOs));
                string validFolow = "";
                //valid = (bool)this.icdProcessor.ValidationIcd(this.ucIcd) && valid;
                //valid = (bool)this.icdCauseProcessor.ValidationIcd(this.ucIcdCause) && valid;
                //valid = (bool)this.subIcdProcessor.GetValidate(this.ucSecondaryIcd) && valid;
                valid = this.dxValidationProviderControl.Validate() && valid;
                validFolow += "valid.1=" + valid + ";";
                //valid = valid && this.CheckSunSatAppointment();
                //validFolow += "valid.2=" + valid + ";";
                valid = valid && this.CheckMaxExpend();
                //validFolow += "valid.3=" + valid + ";";
                //valid = valid && this.CheckTreatmentFinish();
                validFolow += "valid.4=" + valid + ";";
                valid = valid && this.CheckICDService();
                validFolow += "valid.5=" + valid + ";";
                valid = valid && this.CheckUseDayAndExpTimeBHYT();
                validFolow += "valid.6=" + valid + ";";
                valid = valid && this.CheckWarringIntructionUseDayNum();
                validFolow += "valid.7=" + valid + ";";
                valid = valid && this.CheckThuocKhangSinhTrongNgay();
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
                valid = valid && this.ProcessValidMedicineTypeAge();
                validFolow += "valid.14=" + valid + ";";
                valid = valid && this.CheckOverlapWarningOption();
                validFolow += "valid.15=" + valid + ";";
                valid = valid && ValidAcinInteractiveWorker.ValidConsultationReqiured(this.actionType, this.oldServiceReq, this.currentModule, this.treatmentId, this.mediMatyTypeADOs);
                validFolow += "valid.16=" + valid + ";";
                Inventec.Common.Logging.LogSystem.Debug(validFolow + "____" + Inventec.Common.Logging.LogUtil.TraceData("frmAssignPrescription.valid", valid));
                if (!valid) return;

                bool isSaveAndPrint = (saveType == SAVETYPE.SAVE_PRINT_NOW);
                //Tạm khóa các button lưu && lưu in lại khi đang xử lý
                this.ChangeLockButtonWhileProcess(false);
                bool success = false;
                WaitingManager.Show();

                ISave isave = SaveFactory.MakeISave(
                    paramCommon,
                    this.mediMatyTypeADOs,
                    this,
                    this.actionType,
                    isSaveAndPrint,
                    this.serviceReqParentId ?? 0,
                    this.GetSereServInKip()
                    );

                var rsData = (isave != null ? isave.Run() : null);
                if (rsData != null)
                {
                    HIS_SERVICE_REQ serviceReqResult = null;
                    if (rsData.GetType() == typeof(InPatientPresResultSDO))
                    {
                        InPatientPresResultSDO patientPresResultSDO = rsData as InPatientPresResultSDO;
                        serviceReqResult = patientPresResultSDO.ServiceReqs.FirstOrDefault();
                    }
                    else if (rsData.GetType() == typeof(SubclinicalPresResultSDO))
                    {
                        SubclinicalPresResultSDO outPatientPresResultSDO = rsData as SubclinicalPresResultSDO;
                        serviceReqResult = outPatientPresResultSDO.ServiceReqs.FirstOrDefault();
                    }

                    this.oldServiceReq = serviceReqResult;
                    if (assignPrescriptionEditADO != null && assignPrescriptionEditADO.DgRefeshData != null)
                    {
                        assignPrescriptionEditADO.DgRefeshData(this.oldServiceReq);
                    }
                }
                success = ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ?
                    ProcessAfterSaveForIn(isave, isSaveAndPrint, (InPatientPresResultSDO)rsData)
                    : ProcessAfterSaveForOut(isave, isSaveAndPrint, (SubclinicalPresResultSDO)rsData));

                if (success)
                {
                    gridControlTutorial.DataSource = null;
                    this.actionType = GlobalVariables.ActionView;
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.SetEnableButtonControl(this.actionType);
                    //Mở khóa các button lưu && lưu in khi đã xử lý xong
                    this.ChangeLockButtonWhileProcess(false);
                    switch (saveType)
                    {
                        case SAVETYPE.SAVE:
                            break;
                        case SAVETYPE.SAVE_PRINT_NOW:
                            this.PrescriptionPrintNow();
                            break;
                        case SAVETYPE.SAVE_SHOW_PRINT_PREVIEW:
                            this.PrescriptionPrintShow(PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338, false);
                            break;
                    }

                    if (this.oldServiceReq != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        var logDatail = String.Join("||||", this.mediMatyTypeADOs.Select(o => o.MEDICINE_TYPE_NAME + "(" + o.MEDICINE_TYPE_CODE + ") - IsExpend = " + o.IsExpend + " - IsExpendType=" + o.IsExpendType));
                        Inventec.Common.Logging.LogSystem.Debug(String.Format("Tai khoan {0} da tao/sua don thuoc(ServiceReqCode ={1})   thanh cong, log chi tiet: {2}", loginName, this.oldServiceReq.SERVICE_REQ_CODE, logDatail));
                    }
                }
                else
                {
                    this.ChangeLockButtonWhileProcess(true);
                }

                WaitingManager.Hide();


                #region Show message

                //if (!success
                //    && (paramCommon.Messages == null || paramCommon.Messages.Count == 0)
                //    && (paramCommon.BugCodes == null || paramCommon.BugCodes.Count == 0)
                //    )
                //    return;
                MessageManager.Show(this, paramCommon, success);
                #endregion
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                WaitingManager.Hide();
                MessageManager.Show(this, paramCommon, false);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ReLoadPrintTreatmentEndTypeExt(PrintTreatmentEndTypeExPrintType.TYPE exPrintType)
        {
            try
            {
                PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatmentId, ReloadMenuTreatmentEndTypeExt, CreateMenu.TYPE.DYNAMIC, currentModule != null ? currentModule.RoomId : 0);

                printTreatmentEndTypeExtProcessor.Print(exPrintType,
                    HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.PRINT__INIT_MENU);
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
                this.lciPrintAssignPrescription.Update();
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

        private bool ProcessAfterSaveForOut(ISave isave, bool isSaveAndPrint, SubclinicalPresResultSDO rsOut)
        {
            bool success = false;
            this.outPrescriptionResultSDOs = (rsOut as SubclinicalPresResultSDO);
            if (this.outPrescriptionResultSDOs != null
                && this.outPrescriptionResultSDOs.ServiceReqs != null && this.outPrescriptionResultSDOs.ServiceReqs.Count > 0
                && (
                     //(this.outPrescriptionResultSDOs.ServiceReqMaties != null && this.outPrescriptionResultSDOs.ServiceReqMaties.Count > 0)
                     //|| (this.outPrescriptionResultSDOs.ServiceReqMeties != null && this.outPrescriptionResultSDOs.ServiceReqMeties.Count > 0)
                     (this.outPrescriptionResultSDOs.Materials != null && this.outPrescriptionResultSDOs.Materials.Count > 0)
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

        private void ProcessAndPrintBK()
        {
            try
            {
                if (this.processWhileAutoTreatmentEnd != null)
                    this.processWhileAutoTreatmentEnd();

                V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, this.currentTreatmentWithPatientType);

                BordereauInitData bordereauInitData = new BordereauInitData();
                bordereauInitData.Treatment = treatment;

                PrintBordereauProcessor printBordereauProcessor = new PrintBordereauProcessor(this.currentTreatmentWithPatientType.ID, this.currentTreatmentWithPatientType.PATIENT_ID, bordereauInitData, null);
                printBordereauProcessor.Print(Library.PrintBordereau.Base.PrintOption.Value.PRINT_NOW);
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
                    MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
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

    }
}
