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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Save;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HIS.Desktop.LibraryMessage;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using System.Text;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void ValidDataMediMaty()
        {
            try
            {
                foreach (var item in this.mediMatyTypeADOs)
                {
                    if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC) && item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (item.MEDICINE_USE_FORM_ID ?? 0) <= 0 && (item.DO_NOT_REQUIRED_USE_FORM ?? -1) != RequiredUseFormCFG.DO_NOT_REQUIRED)
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
                DevExpress.XtraLayout.Utils.LayoutVisibility always = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                DevExpress.XtraLayout.Utils.LayoutVisibility never = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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

        private void PatientSelectedChange(V_HIS_TREATMENT_BED_ROOM data, bool isResetData)
        {
            try
            {
                if (!this.isNotLoadWhileChangeInstructionTimeInFirst && this.treatmentCode == data.TREATMENT_CODE)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Goi ham thay doi benh nhan nhung kiem tra ma dieu tri cu van nhu ma dieu tri hien tai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.treatmentCode), this.treatmentCode)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.TREATMENT_CODE), data.TREATMENT_CODE));
                    return;
                }
                this.treatmentCode = data.TREATMENT_CODE;
                this.treatmentId = data.TREATMENT_ID;
                this.patientDob = data.TDL_PATIENT_DOB;
                this.patientName = data.TDL_PATIENT_NAME;
                this.genderName = data.TDL_PATIENT_GENDER_NAME;

                LogSystem.Debug("PatientSelectedChange => 1");
                if (isResetData)
                {
                    this.SetDefaultData();
                    this.ReSetDataInputAfterAdd__MedicinePage();
                }

                this.cboPhieuDieuTri.EditValue = null;
                this.cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                this.cboPhieuDieuTri.Properties.DataSource = null;
                this.trackingADOs = new List<TrackingADO>();
                this.FillAllPatientInfoSelectedInForm();
                this.LoadDataTracking();
                if (isResetData)
                    this.FillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);
                LogSystem.Debug("PatientSelectedChange => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, MediMatyTypeADO medimaty)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                bool isFullHeinInfoData = IsFullHeinInfo(medimaty);
                if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT && isFullHeinInfoData)
                {
                    result = currentPatientTypeWithPatientTypeAlter.Where(o => o.ID == HisConfigCFG.PatientTypeId__BHYT).FirstOrDefault();
                }
                else if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT && !isFullHeinInfoData)
                {
                    result = currentPatientTypeWithPatientTypeAlter.Where(o => o.ID == HisConfigCFG.PatientTypeId__VP).FirstOrDefault();
                }
                else
                {
                    result = currentPatientTypeWithPatientTypeAlter.Where(o => o.ID == patientTypeId).FirstOrDefault();
                }
                if (result == null || result.ID == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeWithPatientTypeAlter), currentPatientTypeWithPatientTypeAlter) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.PatientTypeId__BHYT), HisConfigCFG.PatientTypeId__BHYT) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.PatientTypeId__VP), HisConfigCFG.PatientTypeId__VP));
                }
                return (result ?? new HIS_PATIENT_TYPE());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlServiceOther(long patientTypeId, long serviceId, long serviceTypeId)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                MediMatyTypeADO mediMatyTypeADO = null;
                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    //1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
                    //Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE) và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
                    //(bỏ, ko check "số đăng ký")
                    var sv = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    mediMatyTypeADO = new MediMatyTypeADO()
                    {
                        SERVICE_ID = sv.SERVICE_ID,
                        SERVICE_TYPE_ID = sv.SERVICE_TYPE_ID,
                        ACTIVE_INGR_BHYT_CODE = sv.ACTIVE_INGR_BHYT_CODE,
                        //REGISTER_NUMBER = sv.REGISTER_NUMBER,
                        HEIN_SERVICE_TYPE_ID = sv.HEIN_SERVICE_TYPE_ID,
                        HEIN_SERVICE_TYPE_CODE = sv.HEIN_SERVICE_TYPE_CODE,
                        HEIN_SERVICE_BHYT_CODE = sv.HEIN_SERVICE_BHYT_CODE,
                        HEIN_SERVICE_BHYT_NAME = sv.HEIN_SERVICE_BHYT_NAME,
                        DO_NOT_REQUIRED_USE_FORM = sv.DO_NOT_REQUIRED_USE_FORM,
                    };
                }
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    var sv = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    mediMatyTypeADO = new MediMatyTypeADO()
                    {
                        SERVICE_ID = sv.SERVICE_ID,
                        SERVICE_TYPE_ID = sv.SERVICE_TYPE_ID,
                        HEIN_SERVICE_TYPE_ID = sv.HEIN_SERVICE_TYPE_ID,
                        HEIN_SERVICE_TYPE_CODE = sv.HEIN_SERVICE_TYPE_CODE,
                        HEIN_SERVICE_BHYT_CODE = sv.HEIN_SERVICE_BHYT_CODE,
                        HEIN_SERVICE_BHYT_NAME = sv.HEIN_SERVICE_BHYT_NAME,
                    };
                }

                return this.ChoosePatientTypeDefaultlService(patientTypeId, mediMatyTypeADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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
        private void ProcessSaveForListSelect(bool isSaveAndPrint) 
        {
            try
            {
                 this.bIsSelectMultiPatientProcessing = false;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.mediMatyTypeADOs = this.gridViewServiceProcess.DataSource as List<MediMatyTypeADO>;
                if (!string.IsNullOrEmpty(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                {
                    LoadVServiceReq();
                    if (vServiceReq != null && vServiceReq.START_TIME.HasValue && Inventec.Common.DateTime.Calculation.DifferenceTime(vServiceReq.START_TIME.Value, InstructionTime, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.SECOND) < Int32.Parse(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian chỉ định {0} phải cách thời gian bắt đầu khám {1} là {2} giây mới được phép chỉ định", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(InstructionTime), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vServiceReq.START_TIME.Value), HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return;
                    }
                }

                if (GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    var listPatientSelecteds = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (listPatientSelecteds == null || listPatientSelecteds.Count < 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonBenhNhan, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (listPatientSelecteds != null && listPatientSelecteds.Count > 1)
                    {
                        var myResult = DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BanDangChonKeDonChoNBenhNhanBanCoChacMuonThucHien, listPatientSelecteds.Count), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Trường hợp kê đơn nội trú chọn nhiều bệnh nhân để kê, người dùng chọn không kê=> dừng không xử lý gì tiếp____" + Inventec.Common.Logging.LogUtil.TraceData("listPatientSelecteds.Count", listPatientSelecteds.Count));
                            return;
                        }

                        this.bIsSelectMultiPatientProcessing = true;
                        this.paramSaveList = new CommonParam();
                        this.paramSaveList.Messages = new List<string>();
                        this.successSaveList = true;

                        this.ProcessSaveData(isSaveAndPrint);

                        this.actionType = GlobalVariables.ActionAdd;
                        this.actionBosung = GlobalVariables.ActionAdd;
                        this.SetEnableButtonControl(this.actionType);

                        var listProcessPatientSelecteds = listPatientSelecteds.Where(o => o.TREATMENT_ID != this.treatmentId).ToList();

                        foreach (var item in listProcessPatientSelecteds)
                        {
                            PatientSelectedChange(item, false);
                            this.ChangeLockButtonWhileProcess(true);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.TREATMENT_ID), item.TREATMENT_ID));
                            //- Nguồn khác
                            //- Kết hợp BH
                            //- Kiểm tra điều kiện DV(HIS_SERVICE_CONDITION)
                            foreach (var medi in this.mediMatyTypeADOs)
                            {
                                medi.AmountAlert = null;
                                medi.ErrorMessageAmount = "";
                                medi.ErrorMessageAmountAlert = "";
                                medi.ErrorMessageIsAssignDay = "";
                                medi.ErrorMessageMedicineUseForm = "";
                                medi.ErrorMessageMediMatyBean = "";
                                medi.ErrorMessagePatientTypeId = "";
                                medi.ErrorMessageTutorial = "";
                                medi.ErrorTypeAmount = ErrorType.None;
                                medi.ErrorTypeAmountAlert = ErrorType.None;
                                medi.ErrorTypeIsAssignDay = ErrorType.None;
                                medi.ErrorTypeMedicineUseForm = ErrorType.None;
                                medi.ErrorTypeMediMatyBean = ErrorType.None;
                                medi.ErrorTypePatientTypeId = ErrorType.None;
                                medi.ErrorTypeTutorial = ErrorType.None;
                                medi.IsKHBHYT = false;
                                medi.OTHER_PAY_SOURCE_ID = null;
                                medi.OTHER_PAY_SOURCE_CODE = null;
                                medi.OTHER_PAY_SOURCE_NAME = null;

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentHisPatientTypeAlter.PATIENT_TYPE_ID", currentHisPatientTypeAlter.PATIENT_TYPE_ID)
                                    + Inventec.Common.Logging.LogUtil.TraceData("item.TREATMENT_ID", item.TREATMENT_ID));
                                HIS_PATIENT_TYPE patientTypeDefault = ChoosePatientTypeDefaultlServiceOther(currentHisPatientTypeAlter.PATIENT_TYPE_ID, medi.SERVICE_ID, medi.SERVICE_TYPE_ID);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeDefault), patientTypeDefault)
                                    + Inventec.Common.Logging.LogUtil.TraceData("medi.PATIENT_TYPE_ID", medi.PATIENT_TYPE_ID));
                                if (patientTypeDefault != null)
                                {
                                    medi.PATIENT_TYPE_ID = patientTypeDefault.ID;
                                    FillDataOtherPaySourceDataRow(medi);
                                }
                                else
                                {
                                    medi.PATIENT_TYPE_ID = null;
                                }

                                UpdateExpMestReasonInDataRow(medi);
                            }

                            gridViewServiceProcess.BeginUpdate();
                            gridViewServiceProcess.GridControl.DataSource = this.mediMatyTypeADOs;
                            gridViewServiceProcess.EndUpdate();
                            treatmentId = item.TREATMENT_ID;
                            LoadHisTreatment();
                            this.ProcessSaveData(isSaveAndPrint);

                            this.actionType = GlobalVariables.ActionAdd;
                            this.actionBosung = GlobalVariables.ActionAdd;
                            this.SetEnableButtonControl(this.actionType);
                        }
                        this.actionType = GlobalVariables.ActionView;
                        this.actionBosung = GlobalVariables.ActionAdd;
                        this.SetEnableButtonControl(this.actionType);
                        string message = "";
                        if (!this.successSaveList)
                        {
                            message += ResourceMessage.CacBenhNhanSaukeDonThatBai;
                            message += "<br>";
                            message += String.Join("", this.paramSaveList.Messages);

                            if (!String.IsNullOrEmpty(message))
                            {
                                MessageManager.Show(message);
                            }
                        }
                        else
                        {
                            message = MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong);
                            MessageManager.Show(this, new CommonParam(), true);
                        }
                    }
                    else
                        this.ProcessSaveData(isSaveAndPrint);
                }
                else
                    this.ProcessSaveData(isSaveAndPrint);

                this.bIsSelectMultiPatientProcessing = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }
        private bool CheckICDPreSave()
        {
            bool valid = true;
            try
            {

                var icdValue = (IcdInputADO)this.icdProcessor.GetValue(this.ucIcd);
                var icdValueSecond = (SecondaryIcdDataADO)this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                var icdYHCT = (IcdInputADO)this.icdProcessorYHCT.GetValue(this.ucIcdYHCT);
                var icdSecondYHCT = (SecondaryIcdDataADO)subIcdProcessorYHCT.GetValue(this.ucSecondaryIcdYHCT);
                var config = HisConfigs.Get<string>("HIS.Desktop.Plugins.CheckIcdWhenSave");
                if (this.icdProcessor != null && this.ucIcd != null || (this.subIcdProcessor != null && this.ucSecondaryIcd != null))
                {
                    
                    

                    //if ( !(bool)icdProcessor.ValidationIcd(ucIcd) || !(bool)icdProcessorYHCT.ValidationIcd(ucIcdYHCT) || !subIcdProcessor.GetValidate(this.ucSecondaryIcd) || !subIcdProcessorYHCT.GetValidate(ucSecondaryIcdYHCT)) return false;
                    string mess = "";
                    var mainCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    var mainCodeSecond = ((SecondaryIcdDataADO)icdValueSecond).ICD_SUB_CODE;
                    List<string> listicd = new List<string>();
                    string icd_code_error = "";
                    if (!string.IsNullOrEmpty(mainCode) || !string.IsNullOrEmpty(mainCodeSecond))
                    {
                        List<string> listSubCode = new List<string>();

                        if (!string.IsNullOrEmpty(mainCodeSecond))
                        {
                            listSubCode = mainCodeSecond.Split(';').ToList();
                        }
                        if (CheckICD(mainCode, mainCodeSecond, ref mess, ref icd_code_error))
                        {
                            var param_error = icd_code_error.Split(';').ToList();
                            if (!string.IsNullOrEmpty(mess))
                            {
                               
                                if(config == "1")
                                {
                                    if (MessageBox.Show(this, mess, "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        valid = true;
                                    }
                                    else valid = false;
                                }
                                else if (config == "2")
                                {
                                    MessageBox.Show(this, mess, "Thông báo", MessageBoxButtons.OK);

                                    valid = false;
                                }
                                
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        private void ProcessSaveData(bool isSaveAndPrint)
        {
            try
            {
                bool valid = true;
                string warning = "", errorMessages = "";
                this.positionHandleControl = -1;
                this.paramCommon = new CommonParam();
                List<string> paramMessageErrorOther = new List<string>();
                List<string> paramMessageErrorEmpty = new List<string>();

                

                valid = (bool)this.icdProcessor.ValidationIcdWithMessage(this.ucIcd, paramMessageErrorEmpty, paramMessageErrorOther) && valid;
                valid = (bool)this.icdCauseProcessor.ValidationIcdWithMessage(this.ucIcdCause, paramMessageErrorEmpty, paramMessageErrorOther) && valid;
                valid = (bool)this.subIcdProcessor.GetValidateWithMessage(this.ucSecondaryIcd, paramMessageErrorEmpty, paramMessageErrorOther) && valid;
                valid = (bool)this.ucDateProcessor.ValidationFormWithMessage(this.ucDate, paramMessageErrorEmpty, paramMessageErrorOther) && valid;
                valid = this.dxValidationProviderControl.Validate() && valid;
                valid = valid && CheckReasonRequied(); //kiểm tra bắt buộc nhập lý do xuất
                valid = valid && CheckPayICD(); //kiểm tra đối tượng thanh toán theo chẩn đoán
                
                
                valid = valid &&CheckICDPreSave() ;
                if (valid)
                {
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        if (item.ErrorTypeAmount == ErrorType.Warning)
                        {
                            errorMessages += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessageAmount + "; ";
                        }
                        if (item.ErrorTypeIsAssignDay == ErrorType.Warning)
                        {
                            errorMessages += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessageIsAssignDay + "; ";
                        }
                        if (item.ErrorTypePatientTypeId == ErrorType.Warning)
                        {
                            errorMessages += item.MEDICINE_TYPE_NAME + " " + item.ErrorMessagePatientTypeId + "; ";
                        }
                    }
                    if (!String.IsNullOrEmpty(errorMessages))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(errorMessages + ". Bạn có muốn tiếp tục?",
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) valid = false;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => errorMessages), errorMessages));
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
                    getMessageErrorControlInvalidProcess.Run(this, this.dxValidationProviderControl, this.ModuleControls, paramMessageErrorEmpty, paramMessageErrorOther);

                    if (paramMessageErrorOther.Count > 0)
                    {
                        paramMessageErrorOther = paramMessageErrorOther.Distinct().ToList();
                        paramCommon.Messages.AddRange(paramMessageErrorOther);
                    }
                    if (paramMessageErrorEmpty.Count > 0)
                    {
                        paramMessageErrorEmpty = paramMessageErrorEmpty.Distinct().ToList();
                        paramCommon.Messages.Add(String.Join(", ", paramMessageErrorEmpty) + " " + Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
                    }
                    warning = this.paramCommon.GetMessage();
                    if (!String.IsNullOrEmpty(warning))
                    {
                        MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => warning), warning));
                    }
                }

                valid = valid && this.CheckTreatmentFinish();
                valid = valid && this.CheckICDService();
                valid = valid && this.CheckUseDayAndExpTimeBHYT();
                valid = valid && this.ProcessValidMedicineTypeAge();
                valid = valid && this.ValidSereServWithOtherPaySource(this.mediMatyTypeADOs);
                valid = valid && this.CheckOverlapWarningOption();
                bool isValid = true;
                //kiem tra thoi gian du tru va thoi gian y lenh
                if (this.USE_TIME != null && this.USE_TIME.Count > 0)
                {
                    foreach (var time in this.USE_TIME)
                    {
                        var exits = this.intructionTimeSelecteds

                                     .Where(s => s > time);

                        if (exits.Any())
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
                if (!isValid)
                {
                    MessageBox.Show(this, "Ngày dự trù không được nhỏ hơn thời gian chỉ định", "Thông Báo");
                    return;
                }

                if (this.TIME_TO != null && this.TIME_TO > 0)
                {
                    var exits = this.intructionTimeSelecteds.Where(s => Int64.Parse(s.ToString().Substring(0,8)) > Int64.Parse(this.TIME_TO.ToString().Substring(0,8)));
                    if (exits.Any())
                    {
                        valid = false;
                        MessageBox.Show(this, "Ngày Chỉ định đến không được nhỏ hơn thời gian chỉ định", "Thông Báo");
                        return;
                        
                    }
                }

                string codeIcd = "";
                string nameIcd = "";
                var icdValue = (IcdInputADO)this.icdProcessor.GetValue(this.ucIcd);
                var icdValueSecond = (SecondaryIcdDataADO)this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                var icdYHCT = (IcdInputADO)this.icdProcessorYHCT.GetValue(this.ucIcdYHCT);
                var icdSecondYHCT = (SecondaryIcdDataADO)subIcdProcessorYHCT.GetValue(this.ucSecondaryIcdYHCT);
                if (icdValue != null)
                {
                    codeIcd = icdValue.ICD_CODE;
                    nameIcd = icdValue.ICD_NAME;
                }
                if (icdValueSecond != null)
                {
                    codeIcd += icdValueSecond.ICD_SUB_CODE;
                    nameIcd += icdValueSecond.ICD_TEXT;
                }
                if (!string.IsNullOrEmpty(codeIcd) && codeIcd.Length > 100)
                {
                    isValid = false;
                    XtraMessageBox.Show(string.Format("Mã chẩn đoán phụ nhập quá {0} ký tự", 100));
                    return;
                }

                if (!string.IsNullOrEmpty(nameIcd) && Encoding.UTF8.GetByteCount(nameIcd) > 1500)
                {
                    isValid = false;
                    XtraMessageBox.Show(string.Format("Tên chẩn đoán phụ nhập quá {0} ký tự", 1500));
                    return;
                }

                string codeIcdYhct = "";
                if (icdYHCT != null)
                {
                    codeIcdYhct = icdYHCT.ICD_CODE;
                }
                if (icdSecondYHCT != null)
                {
                    codeIcdYhct += icdSecondYHCT.ICD_SUB_CODE;
                }
                if (!string.IsNullOrEmpty(codeIcdYhct) && codeIcdYhct.Length > 255)
                {
                    isValid = false;
                    XtraMessageBox.Show(string.Format("Mã chẩn đoán YHCT phụ nhập quá {0} ký tự", 255));
                    return;
                }
                valid = valid && isValid;

                if (valid)
                    ProcessUpdateTutorialForSave();

                if (!valid)
                    return;

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
                msgTuVong = "";

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
                else
                {
                    if (!string.IsNullOrEmpty(msgTuVong)) paramCommon.Messages.Add(msgTuVong);
                }

                LogSystem.Debug("Du lieu luu, delegate lai chuc nang. " + LogUtil.TraceData("rsData", rsData));
                success = ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ?
                    ProcessAfterSaveForIn(isave, isSaveAndPrint, (InPatientPresResultSDO)rsData)
                    : ProcessAfterSaveForOut(isave, isSaveAndPrint, (OutPatientPresResultSDO)rsData));


                if (success)
                {
                    if (HisConfigCFG.IsAutoCreateSaleExpMest || HisConfigCFG.DrugStoreComboboxOption)
                    {
                            cboNhaThuoc.Enabled = false;
                    }

                    this.actionType = GlobalVariables.ActionView;
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.isCheckAssignServiceSimultaneityOption = false;
                    this.SetEnableButtonControl(this.actionType);
                    //Mở khóa các button lưu && lưu in khi đã xử lý xong
                    this.ChangeLockButtonWhileProcess(false);
                    //dangth
                    if (isSaveAndPrint)
                        this.PrescriptionPrintNow();

                    if ((!GlobalStore.IsTreatmentIn) && this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null)
                    {// || GlobalStore.IsCabinet
                        var treatUC = treatmentFinishProcessor.GetDataOutput(this.ucTreatmentFinish);
                        //Nếu người dùng tích chọn kết thúc điều trị > Chọn "Lưu" = Lưu toa thuốc + kết thúc điều trị (tự động in giấy hẹn khám, bảng kê nếu tích chọn) + Tự động close form kê toa + xử lý khám (có option theo user).
                        if (treatUC != null && treatUC.IsAutoTreatmentFinish)
                        {
                            this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(this.treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First());
                            if (treatUC.IsAutoPrintGHK)
                            {
                                this.ProcessAndPrintTreatmentEnd();
                            }
                            if (treatUC.IsAutoBK)
                            {
                                this.ProcessAndPrintBK();
                            }

                            if (currentTreatmentWithPatientType.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                            {
                                btnDichVuHenKham.Enabled = true;
                            }

                            if (HisConfigCFG.IsAutoCloseFormWithAutoConfigTreatmentFinish)
                            {
                                WaitingManager.Hide();
                                this.Close();
                                return;
                            }

                            this.InitMenuToButtonPrint();
                        }
                    }

                    if (GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)// && !GlobalStore.IsCabinet
                        this.patientSelectProcessor.ReloadStatePrescriptionPerious(this.ucPatientSelect);

                    this.ReSetDataInputAfterAdd__MedicinePage();

                    LogSystem.Debug("Begin FillDataToComboPriviousExpMest");
                    this.CreateThreadFillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);
                    LogSystem.Debug("End FillDataToComboPriviousExpMest");
                }
                else
                {
                    this.ChangeLockButtonWhileProcess(true);
                }

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

            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                WaitingManager.Hide();
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
                if (this.processWhileAutoTreatmentEnd != null)
                    this.processWhileAutoTreatmentEnd();

                HIS_TREATMENT treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

                PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
                var treatUC = treatmentFinishProcessor.GetDataOutput(ucTreatmentFinish);
                if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                {
                    printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode);
                }
                else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                {
                    printTreatmentFinishProcessor.Print(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode);
                }
                else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                {
                    //Nothing
                }
                else
                {
                    printTreatmentFinishProcessor.Print(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private bool CheckReasonRequied()
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.IsReasonRequired)
                {
                    if (this.lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        if (this.cboExpMestReason.EditValue == null)
                        {
                            if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                            {
                                var dataCheck = this.mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU).ToList();

                                if (dataCheck != null && dataCheck.Count > 0)
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

                    if (this.lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                        {
                            var datareasion = this.mediMatyTypeADOs.Where(o => o.EXP_MEST_REASON_ID == null && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)).Select(p => p.MEDICINE_TYPE_NAME).ToList();

                            if (datareasion != null && datareasion.Count > 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.ThuocVatTuChuaNhapLyDoXuat, string.Join(", ", datareasion)),
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                var icdValue = this.icdProcessor.GetValue(this.ucIcd) as HIS.UC.Icd.ADO.IcdInputADO;

                var subIcd = this.ucSecondaryIcd != null ? this.subIcdProcessor.GetValue(this.ucSecondaryIcd) as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO : null;

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

    }
}
