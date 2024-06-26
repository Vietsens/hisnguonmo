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
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutrition.ADO;
using HIS.Desktop.Plugins.AssignNutrition.Config;
using HIS.Desktop.Plugins.AssignNutrition.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extentions;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignNutrition.AssignNutrition
{
    public partial class frmAssignNutrition : HIS.Desktop.Utility.FormBase
    {
        private void ProcessSaveData(bool isSaveAndPrint)
        {
            try
            {
                if (!(subIcdProcessor.GetValidate(ucSecondaryIcd) && (bool)icdProcessor.ValidationIcd(ucIcd)))
                    return;
                if (this.gridViewService.IsEditing)
                    this.gridViewService.CloseEditor();

                if (this.gridViewService.FocusedRowModified)
                    this.gridViewService.UpdateCurrentRow();


                this.ChangeLockButtonWhileProcess(true);

                HisRationServiceReqSDO serviceReqSDO = new HisRationServiceReqSDO();
                serviceReqSDO.RationServices = new List<RationServiceSDO>();
                serviceReqSDO.TrackingId = (this.tracking != null && this.tracking.ID > 0) ? this.tracking.ID : (long?)null;
                serviceReqSDO.HalfInFirstDay = lciHalfInFirstDay.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkHalfInFirstDay.Checked;
                serviceReqSDO.IsForAutoCreateRation = layoutControlItem18.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always &&
                    chkAutoEat.Checked;
                serviceReqSDO.IsForHomie = chkIsForHomie.Checked;
                var datas = (List<SSServiceADO>)gridViewService.DataSource;
                List<SSServiceADO> dataChecks = new List<SSServiceADO>();
                this.treatmentBedRoomSelecteds = new List<V_HIS_TREATMENT_BED_ROOM>();
                if (ValidData(datas, ref dataChecks, ref  treatmentBedRoomSelecteds)
                    && ValidSereServWithOtherPaySource(dataChecks))
                {
                    foreach (var item in dataChecks)
                    {
                        if (item.RationTimeIds != null && item.RationTimeIds.Count > 0)
                        {
                            continue;
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn bữa ăn với Suất ăn " + "'" + item.SERVICE_NAME + "'", "Thông báo");
                            return;
                        }
                    }
                    this.ProcessServiceReqSDO(serviceReqSDO, dataChecks);
                    this.ProcessServiceReqSDOForIcd(serviceReqSDO);
                    this.SaveServiceReqCombo(serviceReqSDO, isSaveAndPrint);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thiếu trường thông tin bắt buộc", "Thông báo");
                    Inventec.Common.Logging.LogSystem.Debug("Thiếu trường thông tin bắt buộc____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentBedRoomSelecteds), treatmentBedRoomSelecteds) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataChecks), dataChecks));
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool ValidData(List<SSServiceADO> datas, ref List<SSServiceADO> dataChecks, ref List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRoomSelecteds)
        {
            bool valid = false;
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    dataChecks = datas.Where(p => p.IsChecked).ToList();
                }
                valid = (dataChecks != null && dataChecks.Count > 0);

                int[] rowHandles = gridViewTreatmentBedRoom.GetSelectedRows();
                valid = valid && (rowHandles != null && rowHandles.Length > 0);

                if (valid)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(i);
                        if (row != null)
                        {
                            treatmentBedRoomSelecteds.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        public void getDataFromOtherFormDelegate(object data)
        {
            try
            {
                if (data != null && data is bool)
                {
                    isYes = (bool)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_SERE_SERV> getSereServWithMinDuration(List<SereServADO> serviceCheckeds)
        {
            List<HIS_SERE_SERV> listSereServResult = null;
            try
            {
                if (serviceCheckeds != null && serviceCheckeds.Count > 0)
                {
                    List<SereServADO> sereServADOExistMinDUration = serviceCheckeds.Where(o => o.MIN_DURATION != null).ToList();
                    if (sereServADOExistMinDUration != null && sereServADOExistMinDUration.Count > 0)
                    {
                        List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                        foreach (var item in sereServADOExistMinDUration)
                        {
                            ServiceDuration serviceDuration = new ServiceDuration();
                            serviceDuration.ServiceId = item.SERVICE_ID;
                            serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                            serviceDurations.Add(serviceDuration);
                        }
                        // gọi api để lấy về thông báo
                        CommonParam param = new CommonParam();
                        HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                        hisSereServMinDurationFilter.ServiceDurations = serviceDurations;

                        if (this.isMultiDateState)
                            hisSereServMinDurationFilter.InstructionTime = intructionTimeSelecteds.First();//TODO
                        else
                            hisSereServMinDurationFilter.InstructionTime = intructionTimeSelecteds.First();

                        hisSereServMinDurationFilter.PatientId = this.currentHisTreatment.PATIENT_ID;
                        Inventec.Common.Logging.LogSystem.Debug("du lieu dau vao khi goi api HisSereServ/GetExceedMinDuration: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisSereServMinDurationFilter), hisSereServMinDurationFilter));
                        listSereServResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);
                        Inventec.Common.Logging.LogSystem.Debug("ket qua tra ve khi goi api HisSereServ/GetExceedMinDuration: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSereServResult), listSereServResult));
                        var listSereServResultTemp = from SereServResult in listSereServResult
                                                     group SereServResult by SereServResult.SERVICE_ID into g
                                                     orderby g.Key
                                                     select g.FirstOrDefault();
                        listSereServResult = listSereServResultTemp.ToList();

                    }
                    else
                    {
                        listSereServResult = null;
                    }
                }
                else
                {
                    listSereServResult = null;
                }
            }
            catch (Exception ex)
            {
                listSereServResult = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return listSereServResult;
        }

        private bool ValidSereServWithOtherPaySource(List<SSServiceADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0 && gridColumnPRIMARY_PATIENT_TYPE_ID.Visible == true)
                {
                    string sereServOtherpaysourceStr = "";
                    foreach (var item in serviceCheckeds__Send)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == item.PATIENT_TYPE_ID).FirstOrDefault();
                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS) && (item.OTHER_PAY_SOURCE_ID ?? 0) <= 0)
                        {
                            sereServOtherpaysourceStr += item.SERVICE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServOtherpaysourceStr))
                    {
                        sereServOtherpaysourceStr = sereServOtherpaysourceStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho, sereServOtherpaysourceStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
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

        private void ServiceAttachForServicePrimary(ref AssignServiceSDO result)
        {
            try
            {
                List<V_HIS_SERVICE_FOLLOW> serviceFollows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>();
                if (result.ServiceReqDetails != null && result.ServiceReqDetails.Count > 0
                    && serviceFollows != null && serviceFollows.Count > 0)
                {
                    List<long> serviceIds = result.ServiceReqDetails.Select(o => o.ServiceId).Distinct().ToList();
                    long defaultPatientTypeId = this.currentHisPatientTypeAlter.PATIENT_TYPE_ID;
                    List<long> allowPatientTypeIds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>() != null ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>()
                        .Where(o => o.PATIENT_TYPE_ID == defaultPatientTypeId)
                        .Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : null;

                    List<ServiceReqDetailSDO> serviceReqDetailSDOTemp = new List<ServiceReqDetailSDO>();
                    foreach (ServiceReqDetailSDO sdo in result.ServiceReqDetails)
                    {
                        List<V_HIS_SERVICE_FOLLOW> follows = serviceFollows.Where(o => o.SERVICE_ID == sdo.ServiceId).ToList();
                        if (follows != null && follows.Count > 0)
                        {
                            foreach (V_HIS_SERVICE_FOLLOW f in follows)
                            {
                                V_HIS_SERVICE_PATY servicePaty = this.servicePatyInBranchs[f.FOLLOW_ID]
                                    .FirstOrDefault(o => o.PATIENT_TYPE_ID == defaultPatientTypeId);
                                long? patientTypeId = null;
                                if (servicePaty != null)
                                {
                                    patientTypeId = defaultPatientTypeId;
                                }
                                else
                                {
                                    V_HIS_SERVICE_PATY otherServicePaty = this.servicePatyInBranchs[f.FOLLOW_ID]
                                    .Where(o => allowPatientTypeIds.Contains(o.PATIENT_TYPE_ID)).FirstOrDefault();
                                    patientTypeId = otherServicePaty != null ? new Nullable<long>(otherServicePaty.PATIENT_TYPE_ID) : null;
                                }

                                if (patientTypeId.HasValue)
                                {
                                    ServiceReqDetailSDO attach = new ServiceReqDetailSDO();
                                    attach.ServiceId = f.FOLLOW_ID;
                                    attach.Amount = f.AMOUNT;
                                    attach.PatientTypeId = patientTypeId.Value;
                                    serviceReqDetailSDOTemp.Add(attach);
                                }
                            }
                        }
                    }

                    if (serviceReqDetailSDOTemp != null && serviceReqDetailSDOTemp.Count > 0)
                    {
                        result.ServiceReqDetails.AddRange(serviceReqDetailSDOTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessServiceReqSDO(HisRationServiceReqSDO serviceReqSDO, List<SSServiceADO> dataSereServModel)
        {
            try
            {
                if (this.txtDescription.Text != "")
                    serviceReqSDO.Description = this.txtDescription.Text.Trim();

                if (this.cboUser.EditValue != null)
                {
                    serviceReqSDO.RequestLoginName = this.cboUser.EditValue.ToString();
                    serviceReqSDO.RequestUserName = this.cboUser.Text;
                }

                if (dataSereServModel != null && dataSereServModel.Count > 0)
                {
                    foreach (var item in dataSereServModel)
                    {
                        RationServiceSDO sdo = new RationServiceSDO();
                        sdo.Amount = item.AMOUNT;
                        sdo.PatientTypeId = item.PATIENT_TYPE_ID;
                        sdo.RoomId = item.ROOM_ID;
                        sdo.ServiceId = item.ID;
                        sdo.InstructionNote = item.NOTE;
                        sdo.RationTimeIds = item.RationTimeIds;
                        if (item.OTHER_PAY_SOURCE_ID.HasValue)
                        {
                            //    sdo.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;//TODO
                        }
                        if (HisConfigCFG.IsSetPrimaryPatientType == commonString__true
                            || HisConfigCFG.IsSetPrimaryPatientType == "2")
                        {
                            //    sdo.PrimaryPatientTypeId = item.PRIMARY_PATIENT_TYPE_ID;//TODO
                        }

                        serviceReqSDO.RationServices.Add(sdo);
                    }
                }

                serviceReqSDO.RequestRoomId = GetRoomId();
                if (this.treatmentBedRoomSelecteds != null && this.treatmentBedRoomSelecteds.Count > 0)
                {
                    //TODO
                    serviceReqSDO.TreatmentIds = this.treatmentBedRoomSelecteds.Select(o => o.TREATMENT_ID).Distinct().ToList();
                }
                //else if (this.currentHisTreatment != null)
                //{
                //    serviceReqSDO.TreatmentIds = new List<long>();
                //    serviceReqSDO.TreatmentIds.Add(this.currentHisTreatment.ID);
                //}

                if (serviceReqSDO.RequestRoomId == 0)
                    Inventec.Common.Logging.LogSystem.Warn("Khong xac dinh du lieu phong lam viec trong module, chuc nang goi module nay khong truyen vao phong lam viec. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule), currentModule));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessServiceReqSDOForIcd(HisRationServiceReqSDO serviceReqSDO)
        {
            try
            {
                if (this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
                        serviceReqSDO.IcdCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        if (!string.IsNullOrEmpty(((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE))
                        {
                            serviceReqSDO.IcdCode = (((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE);
                        }
                        serviceReqSDO.IcdName = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (this.ucSecondaryIcd != null)
                {
                    var subIcd = this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                    {
                        serviceReqSDO.IcdSubCode = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        serviceReqSDO.IcdText = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateIcdToTreatment(HisTreatmentWithPatientTypeInfoSDO hisTreatmentWithPatientTypeInfoSDO)
        {
            try
            {
                if (this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(ucIcd);
                    if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
                        hisTreatmentWithPatientTypeInfoSDO.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        if (!string.IsNullOrEmpty(((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE))
                        {
                            hisTreatmentWithPatientTypeInfoSDO.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        }
                        hisTreatmentWithPatientTypeInfoSDO.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (this.ucSecondaryIcd != null)
                {
                    var subIcd = this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                    {
                        hisTreatmentWithPatientTypeInfoSDO.ICD_SUB_CODE = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        hisTreatmentWithPatientTypeInfoSDO.ICD_TEXT = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveServiceReqCombo(HisRationServiceReqSDO serviceReqSDO, bool issaveandprint)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                if (this.isMultiDateState)
                    serviceReqSDO.InstructionTimes = intructionTimeSelecteds;//TODO
                else
                    serviceReqSDO.InstructionTimes = intructionTimeSelecteds;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqSDO), serviceReqSDO));
                //Gọi api chỉ định dv
                this.serviceReqComboResultSDO = new BackendAdapter(param).Post<HisServiceReqListResultSDO>(RequestUriStore.HIS_SERVICE_REQ__ASSIGN_SERVICE, ApiConsumers.MosConsumer, serviceReqSDO, ProcessLostToken, param);
                if (this.serviceReqComboResultSDO != null)
                {
                    this.actionType = GlobalVariables.ActionView;
                    success = true;
                    this.SetEnableButtonControl(this.actionType);
                    this.isSaveAndPrint = issaveandprint;
                    this.btnPrint.Enabled = true;
                    this.btnShowDetail.Enabled = true;

                    //Nếu click nút lưu in => tự động gọi hàm xử lý in ngay
                    if (this.isSaveAndPrint)
                    {
                        UpdateIcdToTreatment(this.currentHisTreatment);
                        //InPhieuYeuCauDichVu();
                    }
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void UpdateIcdToCurrentHisTreatment()
        {
            try
            {
                if (String.IsNullOrEmpty(currentHisTreatment.ICD_NAME) && (string.IsNullOrEmpty(currentHisTreatment.ICD_CODE)))
                {
                    HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                    icd.ICD_CODE = currentHisTreatment.PREVIOUS_ICD_CODE;
                    icd.ICD_NAME = this.currentHisTreatment.PREVIOUS_ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = this.currentHisTreatment.PREVIOUS_ICD_SUB_CODE;
                    subIcd.ICD_TEXT = this.currentHisTreatment.PREVIOUS_ICD_NAME;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
