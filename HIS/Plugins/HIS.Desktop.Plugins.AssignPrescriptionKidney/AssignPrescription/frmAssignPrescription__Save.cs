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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Save;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void ValidDataMediMaty()
        {
            try
            {
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
                        V_HIS_SERE_SERV_1 sereServ = null;
                        if (item.IsEdit)
                        {
                            sereServ = this.sereServWithTreatment.FirstOrDefault(o =>
                            o.SERVICE_ID == item.SERVICE_ID
                            && o.INTRUCTION_TIME.ToString().Substring(0, 8) == (this.intructionTimeSelecteds.OrderByDescending(t => t).First()).ToString().Substring(0, 8)
                            && o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID);
                        }
                        else
                        {
                            sereServ = this.sereServWithTreatment.FirstOrDefault(o =>
                            o.SERVICE_ID == item.SERVICE_ID
                            && o.INTRUCTION_TIME.ToString().Substring(0, 8) == (this.intructionTimeSelecteds.OrderByDescending(t => t).First()).ToString().Substring(0, 8));
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSaveData(bool isSaveAndPrint)
        {
            try
            {
                bool valid = true;
                this.positionHandleControl = -1;
                valid = (bool)this.icdProcessor.ValidationIcd(this.ucIcd) && valid;
                valid = (bool)this.icdCauseProcessor.ValidationIcd(this.ucIcdCause) && valid;
                valid = (bool)this.subIcdProcessor.GetValidate(this.ucSecondaryIcd) && valid;
                valid = this.dxValidationProviderControl.Validate() && valid;
                valid = valid && this.CheckICDService();
                valid = valid && this.CheckUseDayAndExpTimeBHYT();
                valid = valid && this.CheckWarringIntructionUseDayNum();
                valid = valid && this.CheckThuocKhangSinhTrongNgay();
                valid = valid && this.CheckCungHoatChat();
                valid = valid && this.CheckMediStockWhenEditPrescription();
                valid = valid && this.CheckPrescriptionSplitOutMediStock();
                valid = valid && this.CheckAmoutWarringInStock();
                valid = valid && this.CheckAmoutWarringNumber();
                valid = valid && this.ProcessValidMedicineTypeAge();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("frmAssignPrescription.valid", valid));
                if (!valid) return;

                //Tạm khóa các button lưu && lưu in lại khi đang xử lý
                this.ChangeLockButtonWhileProcess(false);
                bool success = false;
                WaitingManager.Show();
                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                paramCommon = new CommonParam();
                this.mediMatyTypeADOs = this.gridViewServiceProcess.DataSource as List<MediMatyTypeADO>;

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
                    else if (rsData.GetType() == typeof(OutPatientPresResultSDO))
                    {
                        OutPatientPresResultSDO outPatientPresResultSDO = rsData as OutPatientPresResultSDO;
                        serviceReqResult = outPatientPresResultSDO.ServiceReqs.FirstOrDefault();
                    }

                    this.oldServiceReq = serviceReqResult;
                    if (assignPrescriptionEditADO != null && assignPrescriptionEditADO.DgRefeshData != null)
                    {
                        assignPrescriptionEditADO.DgRefeshData(this.oldServiceReq);
                    }
                }

                success = ProcessAfterSaveForIn(isave, isSaveAndPrint, (InPatientPresResultSDO)rsData);

                if (success)
                {
                    gridControlTutorial.DataSource = null;
                    this.actionType = GlobalVariables.ActionView;
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.SetEnableButtonControl(this.actionType);
                    //Mở khóa các button lưu && lưu in khi đã xử lý xong
                    this.ChangeLockButtonWhileProcess(false);
                    if (isSaveAndPrint)
                        this.PrescriptionPrintNow();

                    if (this.oldServiceReq != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        var logDatail = String.Join("||||", this.mediMatyTypeADOs.Select(o => o.MEDICINE_TYPE_NAME + "(" + o.MEDICINE_TYPE_CODE + ") - IsExpend = " + o.IsExpend + " - IsExpendType=" + o.IsExpendType));
                        Inventec.Common.Logging.LogSystem.Debug(String.Format("Tai khoan {0} da sua don thuoc(ServiceReqCode ={1})   thanh cong, log chi tiet: {2}", loginName, this.oldServiceReq.SERVICE_REQ_CODE, logDatail));
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

                success = true;
            }
            return success;
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
                    MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.ucDateProcessor.GetValue(this.ucDate).First())));
                    valid = false;
                    this.ucDateProcessor.FocusControl(this.ucDate);
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
