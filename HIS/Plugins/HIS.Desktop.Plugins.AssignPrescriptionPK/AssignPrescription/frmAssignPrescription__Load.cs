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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseMediStock;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.Library.AlertWarningFee;
using HIS.Desktop.Print;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl.CustomGrid;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal void FillDataOtherPaySourceDataRow(MediMatyTypeADO currentRowSereServADO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FillDataOtherPaySourceDataRow.1____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO), currentRowSereServADO));
                if (currentRowSereServADO.PATIENT_TYPE_ID > 0)
                {
                    var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
                    dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                    if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                        {
                            dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();

                            if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count == 1)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = dataOtherPaySourceTmps[0].ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else
                        {
                            dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
                        }

                        if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null
                            && currentTreatmentWithPatientType != null && currentTreatmentWithPatientType.OTHER_PAY_SOURCE_ID.HasValue && currentTreatmentWithPatientType.OTHER_PAY_SOURCE_ID.Value > 0
                            && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Exists(k => k.ID == currentTreatmentWithPatientType.OTHER_PAY_SOURCE_ID.Value))
                        {
                            var otherPaysourceByTreatment = dataOtherPaySourceTmps.Where(k => k.ID == currentTreatmentWithPatientType.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
                            if (otherPaysourceByTreatment != null)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByTreatment.ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByTreatment.OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByTreatment.OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null)
                        {
                            IcdInputADO icdData = UcIcdGetValue() as IcdInputADO;
                            var serviceTemp = lstService.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == currentRowSereServADO.SERVICE_ID).FirstOrDefault();
                            if (serviceTemp != null && serviceTemp.OTHER_PAY_SOURCE_ID.HasValue && dataOtherPaySourceTmps.Exists(k =>
                               k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value))
                            {

                                if (((String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) && string.IsNullOrEmpty(serviceTemp.BHYT_WHITELIST_CODES)) || (icdData != null && !String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) && !String.IsNullOrEmpty(icdData.ICD_CODE) && ("," + serviceTemp.OTHER_PAY_SOURCE_ICDS.ToLower() + ",").Contains("," + icdData.ICD_CODE.ToLower() + ","))) || ((currentRowSereServADO.OTHER_PAY_SOURCE_ID == null || String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS)) && (!string.IsNullOrEmpty(serviceTemp.BHYT_WHITELIST_CODES) && (!string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && ("," + serviceTemp.BHYT_WHITELIST_CODES + ",").Contains(currentTreatmentWithPatientType.HEIN_CARD_NUMBER.Substring(0, 2))))))
                                {
                                    var otherPaysourceByService = dataOtherPaySourceTmps.Where(k => k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
                                    if (otherPaysourceByService != null)
                                    {
                                        currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByService.ID;
                                        currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByService.OTHER_PAY_SOURCE_CODE;
                                        currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByService.OTHER_PAY_SOURCE_NAME;
                                    }
                                }
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceTemp), serviceTemp)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdData), icdData));
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workingPatientType), workingPatientType)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataOtherPaySourceTmps), dataOtherPaySourceTmps)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_ID), currentRowSereServADO.OTHER_PAY_SOURCE_ID)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_NAME), currentRowSereServADO.OTHER_PAY_SOURCE_NAME));
                    }
                    Inventec.Common.Logging.LogSystem.Debug("FillDataOtherPaySourceDataRow.2____");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UpdateExpMestReasonInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (medicineTypeSDO != null && this.lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {

                    medicineTypeSDO.EXP_MEST_REASON_ID = null;
                    medicineTypeSDO.EXP_MEST_REASON_CODE = "";
                    medicineTypeSDO.EXP_MEST_REASON_NAME = "";

                    var dataExmeReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXME_REASON_CFG>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue
                            && o.PATIENT_CLASSIFY_ID == this.VHistreatment.TDL_PATIENT_CLASSIFY_ID && o.TREATMENT_TYPE_ID == this.VHistreatment.TDL_TREATMENT_TYPE_ID && (o.PATIENT_TYPE_ID == null || o.PATIENT_TYPE_ID == medicineTypeSDO.PATIENT_TYPE_ID)).ToList();
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
                        var data = (this.lstExpMestReasons != null && this.lstExpMestReasons.Count > 0) ? this.lstExpMestReasons.Where(o => o.ID == dataExmeReasons[0].EXP_MEST_REASON_ID).ToList() : null;

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

        private void LoadDataServiceReqById(long id)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadDataServiceReqById. 1: id = " + id);
                CommonParam paramCommon = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ID = id;
                filter.ColumnParams = new List<string>()
                    {
                        "ID",
                        "IS_EMERGENCY",
                        "IS_NOT_USE_BHYT",
                        "IS_NOT_REQUIRE_FEE",
                        "SERVICE_REQ_CODE",
                        "IS_MAIN_EXAM",
                        "IS_ANTIBIOTIC_RESISTANCE",
                        "SERVICE_REQ_TYPE_ID",
                        "REQUEST_ROOM_ID",
                        "INTRUCTION_TIME",
                        "IS_SUB_PRES",
                        "EXECUTE_ROOM_ID",
                        "TDL_PATIENT_TYPE_ID",
                        "IS_WAIT_CHILD",
                    };
                filter.ColumnParams = filter.ColumnParams.Distinct().ToList();
                var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>((RequestUriStore.HIS_SERVICE_REQ_GET_DYNAMIC), ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (serviceReqs != null && serviceReqs.Count > 0)
                {
                    this.serviceReqMain = serviceReqs.FirstOrDefault();
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqMain), serviceReqMain));
                Inventec.Common.Logging.LogSystem.Debug("LoadDataServiceReqById. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadAllergenic(long patientId)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadAllergenic. 1");
                CommonParam param = new CommonParam();
                HisAllergenicFilter filter = new HisAllergenicFilter();
                filter.TDL_PATIENT_ID = patientId;
                this.allergenics = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_ALLERGENIC>>("api/HisAllergenic/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug("LoadAllergenic. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ThreadLoadDonThuocCu(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> _serviceReqPrintAlls = null)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadDonThuocCu. 1");
                //Neu la in gop don thuoc thi moi load
                string savePrintMpsDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.SAVE_PRINT_MPS_DEFAULT);
                if (savePrintMpsDefault != "Mps000234")
                    return;

                CommonParam param = new CommonParam();
                //if (_serviceReqPrintAlls == null)
                {
                    
                    //Load đơn phòng khám
                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.TREATMENT_ID = this.treatmentId;
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT };
                    serviceReqPrintAlls = await new BackendAdapter(param)
                          .GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);
                }
                //else
                //{
                //    serviceReqPrintAlls = _serviceReqPrintAlls;
                //}

                if (serviceReqPrintAlls == null || serviceReqPrintAlls.Count == 0)
                    return;
                //Load expmest
                this.serviceReqPrints = serviceReqPrintAlls.Where(o => o.ID != this.serviceReqParentId).ToList();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqParentId), serviceReqParentId));

                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = serviceReqPrints.Select(o => o.ID).ToList();
                expMestPrints = await new BackendAdapter(param)
                     .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                if (expMestPrints == null || expMestPrints.Count == 0)
                    return;

                //Laays thuoc va tu trong kho

                HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMestPrints.Select(o => o.ID).ToList();
                expMestMedicinePrints = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMestPrints.Select(o => o.ID).ToList();
                expMestMaterialPrints = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadDonThuocCu. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool? IsFromTypeOut()
        {
            bool? typeFormOut = null;
            try
            {
                //Trường hợp sửa đơn thuốc sẽ load giao diện tương ứng với loại đơn phòng khám hay tủ trực hay đơn nội trú
                if (this.assignPrescriptionEditADO != null)
                {
                    if (this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID > 0)
                    {
                        if (this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK || this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        {
                            typeFormOut = true;
                        }
                        else if (this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                        {
                            typeFormOut = false;
                        }
                        else
                        {
                            typeFormOut = null;
                            Inventec.Common.Logging.LogSystem.Debug("Loai don thuoc khong hop le, chi cho phep sua don voi cac loai: don phong kham, don tu truc, don noi tru____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.assignPrescriptionEditADO), this.assignPrescriptionEditADO));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                typeFormOut = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return typeFormOut;
        }

        private bool? IsFromTypeTTByEditor()
        {
            bool? typeFormTT = null;
            try
            {
                //Trường hợp sửa đơn thuốc sẽ load giao diện tương ứng với loại đơn phòng khám hay tủ trực hay đơn nội trú
                if (this.assignPrescriptionEditADO != null)
                {
                    if (this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID > 0)
                    {
                        if (this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        {
                            typeFormTT = true;
                        }
                        else
                        {
                            typeFormTT = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                typeFormTT = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return typeFormTT;
        }

        private void InitWorker()
        {
            try
            {
                AssignPrescriptionWorker.CreateInstance();
                AssignPrescriptionWorker.Instance.MediMatyCreateWorker = new MediMatyCreateWorker(GetDataAmountOutOfStock, SetDefaultMediStockForData, ChoosePatientTypeDefaultlService, ChoosePatientTypeDefaultlServiceOther, GetPatientTypeId, GetNumRow, SetNumRow, GetMediMatyTypeADOs, GetIsAutoCheckExpend);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MediMatyTypeADO> GetMediMatyTypeADOs()
        {
            try
            {
                return mediMatyTypeADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return new List<MediMatyTypeADO>();
        }

        private bool GetIsAutoCheckExpend()
        {
            try
            {
                return isAutoCheckExpend;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private long GetPatientTypeId()
        {
            try
            {
                return currentHisPatientTypeAlter.PATIENT_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        private int GetNumRow()
        {
            try
            {
                return idRow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 1;
        }

        private void SetNumRow()
        {
            try
            {
                idRow += stepRow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nếu có thẻ cấu hình số ngày hẹn khám mặc định (EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT) và kiểm tra xem hồ sơ điều trị hiện tại có phải đến theo diện hẹn khám hay không (trong his_treatment trường appointment_id có khác null hay ko).
        /// Nếu ko thì xử lý như cũ (ko tự động điền số ngày)
        /// Nếu có thì tiếp tục theo luồng sau:
        /// Kiểm tra thời gian hẹn khám trong thông tin hẹn khám (tương ứng với appointment_id trong treament) là ngày nào. So sánh ngày đó với ngày hiện tại. Lúc đó, số ngày điền sẵn vào đơn sẽ theo công thức:
        /// Số ngày trên đơn = Số ngày hẹn khám mặc định - MIN ((ngày hẹn khám - ngày hiện tại), 0)
        /// </summary>
        private async Task LoadDefaultSoNgayHoaDonFromAppointmentTimeDefault()
        {
            try
            {
                if (String.IsNullOrEmpty(HisConfigCFG.AppointmentTimeDefault)) return;
                if (String.IsNullOrEmpty(this.currentTreatmentWithPatientType.APPOINTMENT_CODE)) return;
                long songayHKmacDinh = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigCFG.AppointmentTimeDefault);
                if (songayHKmacDinh <= 0) return;

                CommonParam param = new CommonParam();
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.TREATMENT_CODE__EXACT = this.currentTreatmentWithPatientType.APPOINTMENT_CODE;
                var appointmentTreatments = await new BackendAdapter(param).GetAsync<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, ProcessLostToken, param);
                if (appointmentTreatments != null && appointmentTreatments.Count > 0)
                {
                    //Kiểm tra thời gian hẹn khám trong thông tin hẹn khám (tương ứng với appointment_id trong treament) là ngày nào.
                    //So sánh ngày đó với ngày hiện tại. Lúc đó, số ngày điền sẵn vào đơn sẽ theo công thức:
                    //Số ngày trên đơn = Số ngày hẹn khám mặc định - MIN ((ngày hẹn khám - ngày hiện tại), 0)
                    System.DateTime dtAppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTreatments[0].APPOINTMENT_TIME ?? 0).Value;
                    TimeSpan diff__hour = (dtAppointmentTime.Date - System.DateTime.Now.Date);
                    double totaldays = diff__hour.TotalDays;
                    long songaytrendon = 0;
                    if (totaldays > 0)
                    {
                        songaytrendon = (long)(songayHKmacDinh - (long)totaldays);
                        this.spinSoNgay.EditValue = songaytrendon;
                    }
                    else
                    {
                        songaytrendon = (long)songayHKmacDinh;
                        this.spinSoNgay.EditValue = songaytrendon;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("Truong hop co cau hinh su dung so ngay hen kham mac dinh(EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT) va ho so hien tai là den hen kham --> tinh so ngay tren don  gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => totaldays), totaldays) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => songaytrendon), songaytrendon) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => songayHKmacDinh), songayHKmacDinh));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nếu phòng đang làm việc là buồng bệnh -> cho phép nhập số lượng nguyên, thập phân, phân số
        /// ô số lượng nhập số: 1; 1.15; 1/2
        /// số lượng trên grid chi tiết nhập số lượng: nguyên, thập phân vd: 1; 1.15
        /// </summary>
        private void SetControlSoLuongNgayNhapChanLe(MediMatyTypeADO mediMatyADO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetControlSoLuongNgayNhapChanLe____MEDICINE_TYPE_CODE:" + mediMatyADO.MEDICINE_TYPE_CODE + ", IsAllowOdd:" + mediMatyADO.IsAllowOdd + ", IsAllowOddAndExportOdd:" + mediMatyADO.IsAllowOddAndExportOdd);
                if ((mediMatyADO.IsAllowOdd.HasValue && mediMatyADO.IsAllowOdd.Value == true))
                {
                    if (HisConfigCFG.AmountDecimalNumber > 0)
                    {
                        numberDisplaySeperateFormatAmount = HisConfigCFG.AmountDecimalNumber;
                    }
                    else
                    {
                        numberDisplaySeperateFormatAmount = 2;
                    }
                }
                else
                {
                    numberDisplaySeperateFormatAmount = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra giá trị của các cấu hình ẩn hiện ô số thang
        /// Ẩn hiện các control theo cấu hình
        /// </summary>
        private void InitControlByConfig()
        {
            try
            {
                string configVisibleTempateMedi = HisConfigCFG.IsVisilbleTemplateMedicine;
                if (!String.IsNullOrEmpty(configVisibleTempateMedi))
                {
                    if (configVisibleTempateMedi == GlobalVariables.CommonStringTrue)
                    {
                        //lciExpMestTemplate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        //lciTemplateMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
                        //lciExpMestTemplate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        //lciTemplateMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra nếu thiết lập cấu hình phần mềm (key = HIS.Desktop.Plugins.Assign.IsExecuteGroup = 1) 
        /// và (loại phòng làm việc của người dùng là phòng xử lý dv và là phòng phẫu thuật) hoặc (loại phòng làm việc của người dùng là buồng bệnh là phòng phẫu thuật)
        /// thì hiển thị control nhóm xử lý. Mặc định ẩn control nhóm xử lý
        /// </summary>
        private void VisibleExecuteGroupByConfig()
        {
            try
            {
                //if (HisConfigCFG.IsVisilbleExecuteGroup == GlobalVariables.CommonStringTrue && currentModule != null && currentModule.RoomId > 0)
                //{
                //    var executeRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId);
                //    var bedRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId);

                //    lciExecuteGroup.Visibility = ((executeRoom != null && executeRoom.IS_SURGERY == GlobalVariables.CommonNumberTrue)
                //                                || (bedRoom != null && bedRoom.IS_SURGERY == GlobalVariables.CommonNumberTrue))
                //        ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra cấu hình ẩn hiện cột chi phí ngoài gói và hao phí trong grid danh sách thuốc/vật tư đã chọn
        /// </summary>
        private void VisibleColumnInGridControlService()
        {
            try
            {
                //An hien cot cp ngoai goi
                long isVisibleColumnCPNgoaiGoi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI);
                if (isVisibleColumnCPNgoaiGoi == 1)
                {
                    this.grcIsOutKtcFee__TabMedicine.Visible = false;
                }

                //An hien cot hao phi
                long isVisibleColumnHaoPhi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI);
                if (isVisibleColumnHaoPhi == 1)
                {
                    this.grcExpend__TabMedicine.Visible = false;
                    this.grcIsExpendType.Visible = false;
                }

                if (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && this.GetSereServInKip() <= 0)
                {
                    this.grcExpend__TabMedicine.Visible = false;
                    this.grcIsExpendType.Visible = false;
                }
                //if (GlobalStore.IsTreatmentIn)
                //{
                //    this.grcTotalPrice__TabMedicine.Visible = false;
                //}

                if (actionType == GlobalVariables.ActionEdit)
                {
                    this.gridColumnEXP_MEST_REASON.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///3. Sửa chức năng "Kê đơn" (phòng khám) và "Kê đơn YHCT":
        ///- Nếu hồ sơ có diện điều trị là "Khám", và đối tượng bệnh nhân không được check "Mặc định hiển thị thuốc/vật tư mua ngoài khi kê đơn 
        ///  phòng khám" (HIS_PATIENT_TYPE có IS_SHOWING_OUT_STOCK_BY_DEF = 1), thì mặc định check hiển thị "Thuốc, vật tư mua ngoài"
        /// </summary>
        private void LoadDefaultTabpageMedicine()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisPatientTypeAlter), currentHisPatientTypeAlter));
                //- Nếu giá trị của key = 1: Với đối tượng bệnh nhân không phải là BHYT và đối tượng điều trị là Khám thì tự động focus vào tab "Thuốc - vật tư mua ngoài".

                var patienttype = (this.currentHisPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0) ? currentPatientTypeWithPatientTypeAlter.FirstOrDefault(o => o.ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID) : null;
                if (
                    patienttype != null && patienttype.IS_SHOWING_OUT_STOCK_BY_DEF == 1
                    && this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__Exam
                    && !GlobalStore.IsCabinet && !GlobalStore.IsTreatmentIn
                    )
                {
                    Inventec.Common.Logging.LogSystem.Info("Hồ sơ có diện điều trị là \"Khám\", và đối tượng bệnh nhân không được check \"Mặc định hiển thị thuốc/vật tư mua ngoài khi kê đơn phòng khám\" (HIS_PATIENT_TYPE có IS_SHOWING_OUT_STOCK_BY_DEF = 1), thì mặc định check hiển thị \"Thuốc, vật tư mua ngoài");
                    rdOpionGroup.SelectedIndex = 1;
                }
                //- Khác 2 giá trị trên thì focus vào tab "Thuốc - vật tư trong kho"
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadDefaultTabpageMedicine. SelectedIndex = 0");
                    rdOpionGroup.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task FillDataToControlsForm()
        {
            try
            {
                this.InitComboRepositoryPatientType(repositoryItemcboPatientType_TabMedicine_GridLookUp, currentPatientTypeWithPatientTypeAlter);
                this.InitComboRepositoryPatientType(repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());

                this.InitComboUser();
                this.InitComboHtu(null);
                this.InitComboMedicineUseForm(null);
                await InitComboMediStockAllow(0);

                LoadSubPrescription();
                this.InitPatientAgeInfo();
                this.FillSomePatientInfoSelectedInFormGeneralAfterLoad();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillDataToControlsFormRebuild()
        {
            try
            {
                this.InitComboRepositoryPatientType(repositoryItemcboPatientType_TabMedicine_GridLookUp, currentPatientTypeWithPatientTypeAlter);
                this.InitComboMediStockAllow(0);
                this.InitPatientAgeInfo();
                this.FillSomePatientInfoSelectedInFormGeneralAfterLoad();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitMedicineTypeAcinInfo()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<V_HIS_MEDICINE_TYPE_ACIN>())
                {
                    ValidAcinInteractiveWorker.currentMedicineTypeAcins = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE_ACIN>();
                }
                else
                {
                    CommonParam param = new CommonParam();
                    HisMedicineTypeAcinViewFilter medicineTypeAcinFilter = new HisMedicineTypeAcinViewFilter();
                    ValidAcinInteractiveWorker.currentMedicineTypeAcins = await new BackendAdapter(param)
                    .GetAsync<List<V_HIS_MEDICINE_TYPE_ACIN>>("api/HisMedicineTypeAcin/GetView", ApiConsumers.MosConsumer, medicineTypeAcinFilter, param);
                }

                //TODO            
                //API hoạt chất
                CommonParam param1 = new CommonParam();
                HisExpMestMedicineLView1Filter expMestMedicineLView1Filter = new HisExpMestMedicineLView1Filter()
                {
                    INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTime).Value.ToString("yyyyMMdd") + "000000"),
                    TDL_PATIENT_ID = this.currentTreatmentWithPatientType.PATIENT_ID,
                    TDL_SERVICE_REQ_ID__NOT_EQUAL = (this.oldServiceReq != null && this.oldServiceReq.ID > 0) ? (long?)this.oldServiceReq.ID : null
                };
                ValidAcinInteractiveWorker.CurrentLExpMestMedicine1s = await new BackendAdapter(param1)
                .GetAsync<List<L_HIS_EXP_MEST_MEDICINE_1>>("api/HisExpMestMedicine/GetLView1", ApiConsumers.MosConsumer, expMestMedicineLView1Filter, param1);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestMedicineLView1Filter), expMestMedicineLView1Filter));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ValidAcinInteractiveWorker.CurrentLExpMestMedicine1s), ValidAcinInteractiveWorker.CurrentLExpMestMedicine1s));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitPatientAgeInfo()
        {
            try
            {
                if (this.patientDob <= 0)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = this.treatmentId;
                    var treatments = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (treatments != null && treatments.Count > 0)
                    {
                        this.patientDob = treatments[0].TDL_PATIENT_DOB;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientSelectedChange(V_HIS_TREATMENT_BED_ROOM data)
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
                this.SetDefaultData();
                this.ReSetDataInputAfterAdd__MedicinePage();
                this.ReSetChongCHiDinhInfoControl__MedicinePage();
                this.cboPhieuDieuTri.EditValue = null;
                this.cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                this.cboPhieuDieuTri.Properties.DataSource = null;
                this.trackingADOs = new List<TrackingADO>();
                this.FillAllPatientInfoSelectedInForm();
                if (!this.isInitTracking)
                    this.LoadDataTracking();
                this.FillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);
                LogSystem.Debug("PatientSelectedChange => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                    this.ReSetChongCHiDinhInfoControl__MedicinePage();
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

        private void LoadDataToPatientInfo()
        {
            try
            {
                if (this.serviceReqParentId.HasValue && this.serviceReqParentId > 0)
                    LoadDataServiceReqById(this.serviceReqParentId.Value);

                if (GlobalStore.IsExecutePTTT
                    || (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null
                    && this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && IsRoomTypeIsExecuteRoom(this.assignPrescriptionEditADO.ServiceReq.REQUEST_ROOM_ID)))
                {
                    this.FillAllPatientInfoSelectedInFormGeneral();
                }
                else
                {
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        this.patientSelectProcessor.LoadWithFilter(this.ucPatientSelect, this.treatmentBedRoomLViewFilterInput);
                    }
                    else
                    {
                        this.FillAllPatientInfoSelectedInForm();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsRoomTypeIsExecuteRoom(long roomId)
        {
            bool result = false;
            try
            {
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (room != null && room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillAllPatientInfoSelectedInForm()
        {
            try
            {
                this.FillAllPatientInfoSelectedInFormGeneral();
                this.LoadDefaultSoNgayHoaDonFromAppointmentTimeDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillAllPatientInfoSelectedInFormGeneral()
        {
            try
            {
                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(this.treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First());
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(currentTreatment, currentTreatmentWithPatientType);
                LogSystem.Debug(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentWithPatientType), this.currentTreatmentWithPatientType)
                    + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.treatmentId), this.treatmentId)
                    + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), this.intructionTimeSelecteds));
                this.LoadDataSereServWithTreatment(currentTreatmentWithPatientType, 0);
                this.PatientTypeWithTreatmentView7();
                this.FillTreatmentInfo__PatientType();//tinh toan va hien thi thong tin ve tong tien tat ca cac dich vu dang chi dinh
                this.CheckWarningOverTotalPatientPrice();

                this.LoadIcdDefault();
                this.InitWorker();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillSomePatientInfoSelectedInFormGeneralAfterLoad()
        {
            try
            {
                this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, 0);
                this.LoadTotalSereServByHeinWithTreatment();
                //this.CheckWarningOverTotalPatientPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra no vien phi
        /// Mức tiền cảnh báo nợ viện phí đối với BN nội trú và tủ trực
        /// </summary>
        private async Task CheckWarningOverTotalPatientPrice()
        {
            try
            {
                if (this.bIsSelectMultiPatientProcessing)
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = treatmentId;
                var treatmentFees = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param);

                //So tien benh nhan can thu
                if (treatmentFees == null || treatmentFees.Count == 0)
                    return;

                decimal totalPrice = 0;
                decimal totalHeinPrice = 0;
                decimal totalPatientPrice = 0;
                decimal totalDeposit = 0;
                decimal totalBill = 0;
                decimal totalBillTransferAmount = 0;
                decimal totalRepay = 0;
                decimal exemption = 0;
                decimal total_obtained_price = 0;
                totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                decimal totalDebtAmount = treatmentFees[0].TOTAL_DEBT_AMOUNT ?? 0;
                totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                exemption = treatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                transferTotal = totalPatientPrice - totalDebtAmount - total_obtained_price;//Phai thu benh nhan
                //transferTotal = Inventec.Common.Number.Convert.NumberToNumberRoundAuto(transferTotal, ConfigApplications.NumberSeperator);
                //- Bổ sung thông tin viện phí lấy theo dữ liệu trong V_HIS_TREATMENT_FEE_1, cụ thể:
                //+ "Tổng tiền" --> sửa lại thành "Phát sinh" (chính là số tiền tương ứng với chỉ định bs đang kê)
                //+ Tổng chi phí BN phải trả = TOTAL_PATIENT_PRICE
                //+ Đã đóng: = TOTAL_DEPOSIT_AMOUNT + TOTAL_BILL_AMOUNT - TOTAL_BILL_TRANSFER_AMOUNT - TOTAL_REPAY_AMOUNT
                //+ X = Đã đóng - Tổng chi phí BN phải trả
                //Nếu X >= 0, thì hiển thị label là "Còn thừa" và "Còn thừa" = X
                //Nếu X < 0, thì hiển thị label là "Còn thiếu", hiển thị màu đỏ, và "Còn thiếu" = abs(X)
                //* Lưu ý: cần xử lý gọi api bất đồng bộ để tránh làm tăng thời gian mở form kê đơn

                lblChiPhiBNPhaiTra.Text = Inventec.Common.Number.Convert.NumberToString(totalPatientPrice, ConfigApplications.NumberSeperator);
                lblDaDong.Text = Inventec.Common.Number.Convert.NumberToString(total_obtained_price, ConfigApplications.NumberSeperator);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transferTotal), transferTotal));
                if (transferTotal > 0)
                {
                    lblConThua.Text = Inventec.Common.Number.Convert.NumberToString(Math.Abs(transferTotal), ConfigApplications.NumberSeperator);
                    lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForlblConThieu.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciForlblConThua.AppearanceItemCaption.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblConThua.Text = Inventec.Common.Number.Convert.NumberToString(Math.Abs(transferTotal), ConfigApplications.NumberSeperator);
                    this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }

                //Trong trường hợp ĐỐI TƯỢNG BỆNH NHÂN  được không  check "Không cho phép kê đơn nếu thiếu tiền" (HIS_PATIENT_TYPE có IS_CHECK_FEE_WHEN_PRES = 1) và hồ sơ là "Khám" (HIS_TREATMENT có TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) thì kiểm tra:

                //Nếu hồ sơ đang không thiếu tiền "Còn thiếu" = 0 thì thực hiện cho phép kê đơn  

                //Nếu hồ sơ đang thiếu tiền ("Còn thiếu" > 0), thỏa mãn đồng thời các điều kiện sau:
                //- MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT = 1
                //- Phòng xử lý khám được cấu hình "phòng thu ngân" (trong danh mục "Phòng khám/cls/pttt" có khai báo thông tin "Phòng thu ngân")
                //- Bệnh nhân có thông tin thẻ khám chữa bệnh thông minh trên hệ thống (và chưa bị khóa)
                //- Số dư trong thẻ lớn hơn hoặc bằng số tiền “còn thiếu
                //- Dịch vụ là dịch vụ khám

                //thì hiển thị thông báo: “Bệnh nhân chưa thanh toán tiền công khám. Bạn có muốn thanh toán tiền công khám  không”.
                HIS_CARD PatientCard = null;
                decimal SoDuTaiKhoan = 0;
                if (treatmentFees[0].TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                && (HisConfigCFG.IsUsingExecuteRoomPayment == "1" || HisConfigCFG.ExecuteRoomPaymentOption == "1" || HisConfigCFG.ExecuteRoomPaymentOption == "2"))
                {
                    HisCardFilter CardFilter = new HisCardFilter();
                    CardFilter.PATIENT_ID = treatmentFees[0].PATIENT_ID;
                    CardFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var PatientCards = new BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, CardFilter, param);

                    if (PatientCards != null && PatientCards.Count > 0)
                    {
                        PatientCard = PatientCards.FirstOrDefault();
                    }

                    var balance = await new BackendAdapter(param).GetAsync<decimal?>(RequestUriStore.HIS_PATIENT__GET_CARD_BALANCE, ApiConsumers.MosConsumer, treatmentFees[0].PATIENT_ID, ProcessLostToken, param);
                    SoDuTaiKhoan = (balance.HasValue ? Inventec.Common.Number.Convert.NumberToNumberRoundAuto(balance.Value, ConfigApplications.NumberSeperator) : 0);
                }
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);

                var patientTypeByPT = (currentHisPatientTypeAlter != null && currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == currentHisPatientTypeAlter.PATIENT_TYPE_ID).FirstOrDefault() : null;

                if (patientTypeByPT != null && patientTypeByPT.IS_CHECK_FINISH_CLS_WHEN_PRES == 1
                        && this.VHistreatment != null && this.VHistreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                        && this.serviceReqMain != null && this.serviceReqMain.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && this.serviceReqMain.IS_WAIT_CHILD == 1)
                {
                    MessageBox.Show(this, ResourceMessage.ChuaThucHienXongDichVuCLS, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    this.Close();
                    return;
                }
                transferTotal = transferTotal < 1 ? 0 : transferTotal;
                if (patientTypeByPT != null && patientTypeByPT.IS_CHECK_FEE_WHEN_PRES == 1
                    && treatmentFees[0].TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                    && transferTotal > 0
                    && HisConfigCFG.IsUsingExecuteRoomPayment == "1"
                    && room != null && room.DEFAULT_CASHIER_ROOM_ID != null
                    && PatientCard != null
                    && SoDuTaiKhoan >= transferTotal
                    && this.serviceReqMain != null && this.serviceReqMain.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                    )
                {
                    bool success = false;
                    DialogResult myResult = MessageBox.Show(ResourceMessage.BenhNhanChuaThanhToanTienCongKham, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (myResult == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        epaymentDepositResultSDO = new EpaymentDepositResultSDO();

                        EpaymentDepositSD EpaymentDepositSDO = new EpaymentDepositSD();
                        EpaymentDepositSDO.ServiceReqIds = new List<long>() { this.serviceReqMain.ID };
                        EpaymentDepositSDO.RequestRoomId = this.serviceReqMain.REQUEST_ROOM_ID;

                        epaymentDepositResultSDO = new BackendAdapter(param).Post<EpaymentDepositResultSDO>("api/HisTransaction/EpaymentDeposit", ApiConsumers.MosConsumer, EpaymentDepositSDO, param);

                        if (epaymentDepositResultSDO != null)
                        {
                            success = true;
                            //tạm ứng thành công thì in bảng kê tạm ứng 
                            WaitingManager.Hide();
                            Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, ProcessPrintMps000102);

                            HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                            filterTreatmentFee.ID = treatmentId;
                            treatmentFees = await new BackendAdapter(param)
                                .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filterTreatmentFee, param);

                            totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                            totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                            totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                            totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                            totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                            totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                            exemption = treatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                            totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                            totalDebtAmount = treatmentFees[0].TOTAL_DEBT_AMOUNT ?? 0;
                            total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                            transferTotal = totalPatientPrice - totalDebtAmount - total_obtained_price;//Phai thu benh nhan

                        }
                        WaitingManager.Hide();
                        #region Hien thi message thong bao
                        MessageManager.Show(this, param, success);
                        #endregion
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }

                //Nếu có cấu hình và kê đơn phòng khám thì mới xử lý tiếp
                if (HisConfigCFG.DoNotAllowPresOutPatietInCaseOfHavingDebt == GlobalVariables.CommonStringTrue
                    && (!GlobalStore.IsTreatmentIn
                        && !GlobalStore.IsCabinet
                        && !GlobalStore.IsExecutePTTT)
                    && transferTotal > 0
                    )
                {
                    MessageBox.Show(this, String.Format(ResourceMessage.HoSoDangNoTienKhongChoPhepKeDon, Inventec.Common.Number.Convert.NumberToString(transferTotal, ConfigApplications.NumberSeperator)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    Inventec.Common.Logging.LogSystem.Warn("co cau hinh DoNotAllowPresOutPatietInCaseOfHavingDebt va ke don phong kham ==>" + ResourceMessage.HoSoDangNoTienKhongChoPhepKeDon);
                    this.Close();
                    return;
                }

                // Trong trường hợp ĐỐI TƯỢNG BỆNH NHÂN được check "Không cho phép chỉ định dịch vụ nếu thiếu tiền" (HIS_PATIENT_TYPE có IS_CHECK_FEE_WHEN_ASSIGN = 1) và hồ sơ là "Khám" (HIS_TREATMENT có TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) thì kiểm tra:
                //+ Nếu hồ sơ đang không thừa tiền ("Còn thừa" = 0 hoặc hiển thị "Còn thiếu") thì hiển thị thông báo "Bệnh nhân đang nợ tiền, không cho phép chỉ định dịch vụ", người dùng nhấn "Đồng ý" thì tắt form kê đơn.
                if ((!GlobalStore.IsTreatmentIn
                        && !GlobalStore.IsCabinet
                        && !GlobalStore.IsExecutePTTT)
                        && patientTypeByPT != null && patientTypeByPT.IS_CHECK_FEE_WHEN_PRES == 1
                        && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                        && transferTotal > 0
                    )
                {
                    HisSereServView17Filter filterSs = new HisSereServView17Filter();
                    filterSs.TDL_TREATMENT_ID = treatmentId;
                    var SereSerView17 = await new BackendAdapter(param).GetAsync<List<V_HIS_SERE_SERV_17>>("api/HisSereServ/GetView17", ApiConsumer.ApiConsumers.MosConsumer, filterSs, param);
                    frmDetailsSereServ frm = new frmDetailsSereServ(SereSerView17.ToList(), (RefeshReference)this.Close);
                    frm.ShowDialog();
                    return;
                }

                //Kiem tra cau hinh
                if (!HisConfigCFG.IsWarningOverTotalPatientPrice || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.actionType != GlobalVariables.ActionAdd)
                    return;

                if ((!GlobalStore.IsTreatmentIn
                    && !GlobalStore.IsCabinet
                    && !GlobalStore.IsExecutePTTT))
                    return;

                decimal warningOverTotalPatientPrice = HisConfigCFG.WarningOverTotalPatientPrice;
                if (transferTotal > warningOverTotalPatientPrice)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(this, String.Format(ResourceMessage.BenhNhanDangThieuVienPhi, Inventec.Common.Number.Convert.NumberToString(transferTotal, ConfigApplications.NumberSeperator)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (myResult != DialogResult.Yes)
                    {
                        this.Close();
                        return;
                    }

                    txtMediMatyForPrescription.Focus();
                    txtMediMatyForPrescription.SelectAll();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessPrintMps000102(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = treatmentId;
                var treatmentFees = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                HisPatientViewFilter filterPatient = new HisPatientViewFilter();
                filterPatient.ID = treatmentFees != null ? treatmentFees.PATIENT_ID : 0;
                var patientPrint = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filterPatient, param).FirstOrDefault();



                if (this.epaymentDepositResultSDO != null && this.epaymentDepositResultSDO.SereServDeposit != null && this.epaymentDepositResultSDO.SereServDeposit.Count > 0 && this.epaymentDepositResultSDO.Transaction != null)
                {
                    V_HIS_TRANSACTION transactionPrint = new V_HIS_TRANSACTION();
                    List<HIS_SERE_SERV_DEPOSIT> ssDepositPrint = new List<HIS_SERE_SERV_DEPOSIT>();
                    if (this.epaymentDepositResultSDO.Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                    {
                        transactionPrint = this.epaymentDepositResultSDO.Transaction;
                    }
                    if (transactionPrint == null)
                        return result;

                    ssDepositPrint = this.epaymentDepositResultSDO.SereServDeposit.Where(o => o.DEPOSIT_ID == transactionPrint.ID).ToList();

                    //chỉ định chưa có thời gian ra viện nên chưa cso số ngày điều trị
                    long? totalDay = null;
                    string departmentName = "";

                    //sử dụng SereServs để hiển thị thêm dịch vụ thanh toán cha
                    List<V_HIS_SERE_SERV> sereServs = new List<V_HIS_SERE_SERV>();
                    if (this.epaymentDepositResultSDO.SereServs != null && this.epaymentDepositResultSDO.SereServs.Count > 0)
                    {
                        sereServs = this.epaymentDepositResultSDO.SereServs.Where(o => ssDepositPrint.Exists(e => e.SERE_SERV_ID == o.ID)).ToList();
                    }

                    var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;

                    var sereServHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                    var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);

                    //các sereServ trong nhóm vật tư
                    var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                    var sereServVTTTs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                    var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);

                    var sereServNotHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();

                    var servicePatyPrpos = lstService;

                    //Cộng các sereServ trong gói vào dv ktc
                    foreach (var sereServHitech in sereServHitechADOs)
                    {
                        List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                        var sereServVTTTInKtcs = sereServs.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                        sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                        if (sereServHitech.PRICE_POLICY != null)
                        {
                            var servicePatyPrpo = servicePatyPrpos.Where(o => o.ID == sereServHitech.SERVICE_ID && o.BILL_PATIENT_TYPE_ID == sereServHitech.PATIENT_TYPE_ID && o.PACKAGE_PRICE == sereServHitech.PRICE_POLICY).ToList();
                            if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                            {
                                sereServHitech.VIR_PRICE = sereServHitech.PRICE;
                            }
                        }
                        else
                            sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);

                        sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                        sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                        decimal totalHeinPrice = 0;
                        foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                        {
                            totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                        }
                        sereServHitech.PRICE_BHYT += totalHeinPrice;
                        sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                        sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        sereServHitech.VIR_TOTAL_PATIENT_PRICE = sereServHitech.VIR_TOTAL_PRICE - sereServHitech.VIR_TOTAL_HEIN_PRICE;
                        sereServHitech.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sereServHitech.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;
                    }

                    //Lọc các sereServ không nằm trong dịch vụ ktc và vật tư thay thế
                    //
                    var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    foreach (var sereServVTTTADO in sereServVTTTADOs)
                    {
                        var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                        if (sereServADODelete.Count == 0)
                        {
                            sereServDeleteADOs.Add(sereServVTTTADO);
                        }
                    }

                    foreach (var sereServDelete in sereServDeleteADOs)
                    {
                        sereServVTTTADOs.Remove(sereServDelete);
                    }
                    var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                    sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                    var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);

                    string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentHisPatientTypeAlter.HEIN_CARD_NUMBER, currentHisPatientTypeAlter.LEVEL_CODE, currentHisPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                    MPS.Processor.Mps000102.PDO.PatientADO patientAdo = new MPS.Processor.Mps000102.PDO.PatientADO(patientPrint);

                    if (sereServNotHitechADOs != null && sereServNotHitechADOs.Count > 0)
                    {
                        sereServNotHitechADOs = sereServNotHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServHitechADOs != null && sereServHitechADOs.Count > 0)
                    {
                        sereServHitechADOs = sereServHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServVTTTADOs != null && sereServVTTTADOs.Count > 0)
                    {
                        sereServVTTTADOs = sereServVTTTADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    V_HIS_SERVICE_REQ firsExamRoom = new V_HIS_SERVICE_REQ();
                    if (treatmentFees.TDL_FIRST_EXAM_ROOM_ID.HasValue)
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatmentFees.TDL_FIRST_EXAM_ROOM_ID);
                        if (room != null)
                        {
                            firsExamRoom.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        }
                    }

                    MPS.Processor.Mps000102.PDO.Mps000102PDO mps000102RDO = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                            patientAdo,
                            currentHisPatientTypeAlter,
                            departmentName,

                            sereServNotHitechADOs,
                            sereServHitechADOs,
                            sereServVTTTADOs,

                            null,//bản tin chuyển khoa, mps lấy ramdom thời gian vào khoa khi chỉ định tạm thời chưa cần
                            treatmentFees,

                            BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                            transactionPrint,
                            ssDepositPrint,
                            totalDay,
                            ratio_text,
                            firsExamRoom
                            );
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentFees != null ? treatmentFees.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
            try
            {
                foreach (var item in sereServs)
                {
                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

                    if (sereServADO.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }


        internal int GetSelectedOpionGroup()
        {
            int selectedOpionGroup = 1;
            try
            {
                int iSelectedIndex = this.rdOpionGroup.SelectedIndex;
                iSelectedIndex = iSelectedIndex == -1 ? 0 : iSelectedIndex;
                selectedOpionGroup = (int)this.rdOpionGroup.Properties.Items[iSelectedIndex].Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdOpionGroup.SelectedIndex), rdOpionGroup.SelectedIndex) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selectedOpionGroup), selectedOpionGroup) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.rdOpionGroup.Properties.Items.Count), this.rdOpionGroup.Properties.Items.Count), ex);
            }

            return selectedOpionGroup;
        }

        internal void ReloadDataAvaiableMediBeanInCombo()
        {
            try
            {
                var selectedOpionGroup = GetSelectedOpionGroup();
                var extMediBean = this.mediMatyTypeADOs.Any(o =>
                    o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                    || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD);
                if (extMediBean)
                {
                    if (selectedOpionGroup == 1 || selectedOpionGroup == 3)
                    {
                        if (this.treatmentFinishProcessor != null)
                            this.treatmentFinishProcessor.Reload(this.ucTreatmentFinish, this.GetDateADO());
                        this.RebuildMediMatyWithInControlContainer(selectedOpionGroup == 3);
                    }
                }

                this.SetEnableButtonControl(this.actionType);
                this.SetTotalPrice__TrongDon();

                if (this.gridViewServiceProcess.DataRowCount == 0)
                    this.idRow = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay Chan doan mac dinh: Lay chan doan cuoi cung trong cac xu ly dich vu Kham benh
        /// </summary>
        private void LoadIcdDefault()
        {
            try
            {
                LogSystem.Debug("Load LoadIcdDefault .1");
                this.isNotProcessWhileChangedTextSubIcd = true;
                //#20126
                //"Lấy thông tin chẩn đoán từ y/c khám nếu kê đơn từ màn hình xử lý khám" 
                //--> Nếu bật cấu hình này, thì nếu kê đơn từ màn hình xử lý khám, sẽ tự động lấy thông tin chẩn đoán từ y/c khám chứ ko lấy chẩn đoán từ hồ sơ điều trị

                HIS_TRACKING tracking = new HIS_TRACKING();
                tracking = (this.Listtrackings != null && this.Listtrackings.Count > 0) ? this.Listtrackings.OrderByDescending(o => o.TRACKING_TIME).FirstOrDefault() : null;


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdExam), icdExam));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tracking), tracking));

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTreatmentWithPatientType), currentTreatmentWithPatientType));
                if (tracking != null && !String.IsNullOrEmpty(tracking.TRADITIONAL_ICD_CODE) && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                {
                    this.LoadIcdTranditionalToControl(tracking.TRADITIONAL_ICD_CODE, tracking.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(tracking.TRADITIONAL_ICD_SUB_CODE, tracking.TRADITIONAL_ICD_TEXT);
                }
                else if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentTreatmentWithPatientType != null && String.IsNullOrEmpty(currentTreatmentWithPatientType.TRADITIONAL_ICD_CODE))) && this.icdExam != null)
                {
                    this.LoadIcdTranditionalToControl(icdExam.TRADITIONAL_ICD_CODE, icdExam.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(icdExam.TRADITIONAL_ICD_SUB_CODE, icdExam.TRADITIONAL_ICD_TEXT);
                }
                else if (this.currentTreatmentWithPatientType != null)
                {
                    this.LoadIcdTranditionalToControl(currentTreatmentWithPatientType.TRADITIONAL_ICD_CODE, currentTreatmentWithPatientType.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(currentTreatmentWithPatientType.TRADITIONAL_ICD_SUB_CODE, currentTreatmentWithPatientType.TRADITIONAL_ICD_TEXT);
                }

                if (tracking != null && !String.IsNullOrEmpty(tracking.ICD_CODE) && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                {
                    this.LoadIcdToControl(tracking.ICD_CODE, tracking.ICD_NAME);

                    if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentTreatmentWithPatientType != null && String.IsNullOrEmpty(currentTreatmentWithPatientType.ICD_CODE))) && this.icdExam != null)
                    {
                        this.LoadIcdCauseToControl(this.icdExam.ICD_CAUSE_CODE, this.icdExam.ICD_CAUSE_NAME);
                    }
                    else if (this.currentTreatmentWithPatientType != null)
                    {
                        this.LoadIcdCauseToControl(this.currentTreatmentWithPatientType.ICD_CAUSE_CODE, this.currentTreatmentWithPatientType.ICD_CAUSE_NAME);
                    }

                    var icdCaus = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == tracking.ICD_CODE);
                    if (icdCaus != null)
                    {
                        LoadRequiredCause((icdCaus.IS_REQUIRE_CAUSE == 1));
                    }

                    this.LoadDataToIcdSub(tracking.ICD_SUB_CODE, tracking.ICD_TEXT);
                }
                else if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentTreatmentWithPatientType != null && String.IsNullOrEmpty(currentTreatmentWithPatientType.ICD_CODE))) && this.icdExam != null)
                {
                    this.LoadIcdToControl(this.icdExam.ICD_CODE, this.icdExam.ICD_NAME);
                    this.LoadIcdCauseToControl(this.icdExam.ICD_CAUSE_CODE, this.icdExam.ICD_CAUSE_NAME);

                    var icdCaus = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.icdExam.ICD_CODE);
                    if (icdCaus != null)
                    {
                        LoadRequiredCause((icdCaus.IS_REQUIRE_CAUSE == 1));
                    }

                    this.LoadDataToIcdSub(this.icdExam.ICD_SUB_CODE, this.icdExam.ICD_TEXT);
                }
                else if (this.currentTreatmentWithPatientType != null)
                {
                    this.LoadIcdToControl(this.currentTreatmentWithPatientType.ICD_CODE, this.currentTreatmentWithPatientType.ICD_NAME);
                    this.LoadIcdCauseToControl(this.currentTreatmentWithPatientType.ICD_CAUSE_CODE, this.currentTreatmentWithPatientType.ICD_CAUSE_NAME);

                    var icdCaus = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.currentTreatmentWithPatientType.ICD_CODE);
                    if (icdCaus != null)
                    {
                        LoadRequiredCause((icdCaus.IS_REQUIRE_CAUSE == 1));
                    }

                    this.LoadDataToIcdSub(this.currentTreatmentWithPatientType.ICD_SUB_CODE, this.currentTreatmentWithPatientType.ICD_TEXT);
                }


                string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                this.icdSubcodeAdoChecks = (from m in this.currentIcds.ToList() select new ADO.IcdADO(m, codes)).ToList();

                customGridViewSubIcdName.BeginUpdate();
                customGridViewSubIcdName.GridControl.DataSource = this.icdSubcodeAdoChecks;
                customGridViewSubIcdName.EndUpdate();
                this.isNotProcessWhileChangedTextSubIcd = false;
                LogSystem.Debug("Load LoadIcdDefault .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bổ sung: trong trường hợp đối tượng BN là BHYT và chưa đến ngày hiệu lực 
        /// hoặc đã hết hạn sử dụng (thời gian y lệnh ko nằm trong khoảng [từ ngày - đến ngày] của thẻ BHYT), 
        /// thì hiển thị đối tượng thanh toán mặc định là đối tượng viện phí
        /// Ngược lại xử lý như hiện tại: ưu tiên lấy theo đối tượng Bn trước, không có sẽ lấy mặc định theo đối tượng chấp nhận TT đầu tiên tìm thấy

        ///#22112 ----------- 
        ///- Ở màn hình tồn thuốc, bỏ điều kiện lọc chính sách giá dịch vụ khi hiển thị các thuốc/vật tư.
        ///- Khi chọn thuốc, hệ thống căn cứ vào đối tượng BN và "có đủ thông tin BHYT hay không" để tự động điền "Đối tượng thanh toán" mặc định. Cụ thể:
        ///+ Nếu đối tượng BN là BHYT, và loại thuốc/vật tư đó CÓ ĐỦ thông tin BHYT thì mặc định điền ĐTTT là BHYT
        ///+ Nếu đối tượng BN là BHYT, và loại thuốc/vật tư đó KHÔNG CÓ ĐỦ thông tin BHYT thì mặc định điền ĐTTT là Viện phí
        ///+ Nếu đối tượng BN ko phải là BHYT thì mặc định điền ĐTTT là đối tượng của BN

        ///1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        ///Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE), số đăng ký (REGISTER_NUMBER), và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
        ///1 vật tư được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        ///Khai báo đủ các thông tin: mã BHYT (HEIN_SERVICE_BHYT_CODE), tên BHYT (HEIN_SERVICE_BHYT_NAME), và nhóm BHYT thuộc 1 trong các loại: "Vật tư thay thế", "Vật tư trong danh mục", "Vật tư thanh toán theo tỷ lệ"
        ///------------
        /// </summary>
        /// <param name="patientTypeId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// - 1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE), và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
        /// - 1 vật tư được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã BHYT (HEIN_SERVICE_BHYT_CODE), tên BHYT (HEIN_SERVICE_BHYT_NAME), và nhóm BHYT thuộc 1 trong các loại: "Vật tư thay thế", "Vật tư trong danh mục", "Vật tư thanh toán theo tỷ lệ"
        /// </summary>
        /// <returns>true/false</returns>
        private bool IsFullHeinInfo(MediMatyTypeADO medimaty)
        {
            bool valid = false;
            try
            {
                valid = (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC &&
                            (!String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE) || !string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)))
                            //&& !String.IsNullOrEmpty(medimaty.REGISTER_NUMBER)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT)
                            || (!string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)) && medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM && !String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE)))
                        ||
                        (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT &&
                            !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_CODE)
                            && !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_NAME)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM)
                            || (!string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)) && medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// - 1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE), số đăng ký (REGISTER_NUMBER), và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
        /// - 1 vật tư được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã BHYT (HEIN_SERVICE_BHYT_CODE), tên BHYT (HEIN_SERVICE_BHYT_NAME), và nhóm BHYT thuộc 1 trong các loại: "Vật tư thay thế", "Vật tư trong danh mục", "Vật tư thanh toán theo tỷ lệ"
        /// </summary>
        /// <returns>true/false</returns>
        private bool IsFullHeinInfo(DMediStock1ADO medimaty)
        {
            bool valid = false;
            try
            {
                valid = (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC &&
                              (!String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE) || !string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)))
                              //&& !String.IsNullOrEmpty(medimaty.REGISTER_NUMBER)
                              && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
                              || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL
                              || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT)
                              || (!string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)) && medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM && !String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE)))
                          ||
                          (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT &&
                              !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_CODE)
                              && !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_NAME)
                              && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                              || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL
                              || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM)
                              || (!string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)) && medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// - 1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE), số đăng ký (REGISTER_NUMBER), và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
        /// - 1 vật tư được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã BHYT (HEIN_SERVICE_BHYT_CODE), tên BHYT (HEIN_SERVICE_BHYT_NAME), và nhóm BHYT thuộc 1 trong các loại: "Vật tư thay thế", "Vật tư trong danh mục", "Vật tư thanh toán theo tỷ lệ"
        /// </summary>
        /// <returns>true/false</returns>
        private bool IsFullHeinInfo(HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO medimaty)
        {
            bool valid = false;
            try
            {
                valid = (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC &&
                            (!String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE) || !string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)))
                            //&& !String.IsNullOrEmpty(medimaty.REGISTER_NUMBER)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT)
                            || (!string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)) && medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM && !String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE)))
                        ||
                        (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT &&
                            !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_CODE)
                            && !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_NAME)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM)
                            || (!string.IsNullOrEmpty(HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix) && !string.IsNullOrEmpty(currentTreatmentWithPatientType.HEIN_CARD_NUMBER) && HisConfigCFG.AllowAssignOffListMedicineMaterialHeinCardNumberPrefix.Split(',').ToList().Exists(o => currentTreatmentWithPatientType.HEIN_CARD_NUMBER.StartsWith(o)) && medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private HisTreatmentWithPatientTypeInfoSDO LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
        {
            HisTreatmentWithPatientTypeInfoSDO treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                if (this.pnlUCDate.Enabled)
                    filter.INTRUCTION_TIME = intructionTime;
                treatment = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private void PatientTypeWithTreatmentView7()
        {
            try
            {
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (patientTypeAllows != null && patientTypeAllows.Count > 0 && patientTypes != null)
                {
                    if (this.currentTreatmentWithPatientType != null && !String.IsNullOrEmpty(this.currentTreatmentWithPatientType.PATIENT_TYPE_CODE))
                    {
                        var patientType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.currentTreatmentWithPatientType.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

                        this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.currentTreatmentWithPatientType.TREATMENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.currentTreatmentWithPatientType.HEIN_MEDI_ORG_CODE;
                        this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.currentTreatmentWithPatientType.HEIN_CARD_FROM_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.currentTreatmentWithPatientType.HEIN_CARD_TO_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.currentTreatmentWithPatientType.HEIN_CARD_NUMBER;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.currentTreatmentWithPatientType.RIGHT_ROUTE_TYPE_CODE;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.currentTreatmentWithPatientType.RIGHT_ROUTE_CODE;
                        var tt = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == this.currentTreatmentWithPatientType.TREATMENT_TYPE_CODE);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID = (tt != null ? tt.ID : 0);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME = (tt != null ? tt.TREATMENT_TYPE_NAME : "");

                        var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                            this.currentPatientTypeWithPatientTypeAlter = patientTypes
                                .Where(o => patientTypeAllow.Contains(o.ID)).ToList();
                    }
                    else
                        throw new AggregateException("currentHisTreatment.PATIENT_TYPE_CODE is null");
                }
                else
                    throw new AggregateException("patientTypeAllows is null");
            }
            catch (AggregateException ex)
            {
                WaitingManager.Hide();
                this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                this.currentPatientTypeWithPatientTypeAlter = new List<HIS_PATIENT_TYPE>();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                LogSystem.Warn(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentWithPatientType), this.currentTreatmentWithPatientType));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshSereServInTreatmentData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = this.treatmentId;
                this.sereServsInTreatmentRaw = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private async Task LoadTotalSereServByHeinWithTreatment()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadTotalSereServByHeinWithTreatment.1");
                List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServByHeinWithTreatments = null;
                //if (this.sereServsInTreatmentRaw == null || this.sereServsInTreatmentRaw.Count == 0)
                //{
                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = this.treatmentId;
                hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                sereServByHeinWithTreatments = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, param);
                //}
                //else
                //{
                //    sereServByHeinWithTreatments = this.sereServsInTreatmentRaw.Where(o => o.TDL_TREATMENT_ID == this.treatmentId && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).ToList();
                //}

                //Nếu sửa đơn thuốc thì lấy tổng tiền bảo hiểm hồ sơ trừ đi đơn đang sửa
                if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0)
                {
                    this.totalHeinByTreatment = sereServByHeinWithTreatments != null ? sereServByHeinWithTreatments.Where(o => o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID).Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0) : 0;
                }
                //Ngược lại lấy tất cả tổng tiền bảo hiểm trong hồ sơ
                else
                {
                    this.totalHeinByTreatment = sereServByHeinWithTreatments != null ? sereServByHeinWithTreatments.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0) : 0;
                }
                this.totalHeinPriceByTreatment = sereServByHeinWithTreatments != null ? sereServByHeinWithTreatments.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0) : 0;

                string messageErr = "";
                AlertWarningFeeManager alertWarningFeeManager = new AlertWarningFeeManager();
                if (!alertWarningFeeManager.RunOption(treatmentId, currentHisPatientTypeAlter.PATIENT_TYPE_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_ID, currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE, HisConfigCFG.PatientTypeId__BHYT, totalHeinPriceByTreatment, HisConfigCFG.IsUsingWarningHeinFee, 0, ref messageErr, true))
                {
                    this.Close();
                }

                var sereServTotalHeinPriceWithTreatments = sereServByHeinWithTreatments != null ? sereServByHeinWithTreatments.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK).ToList() : null;

                //Nếu sửa đơn thuốc thì lấy tổng tiền thuốc bảo hiểm hồ sơ trừ đi đơn đang sửa
                if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0)
                {
                    this.totalPriceBHYT = sereServTotalHeinPriceWithTreatments != null ? sereServTotalHeinPriceWithTreatments.Where(o => o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID).Sum(o => o.VIR_TOTAL_PRICE ?? 0) : 0;
                }
                //Ngược lại lấy tất cả tổng tiền thuốc bảo hiểm trong hồ sơ
                else
                {
                    this.totalPriceBHYT = sereServTotalHeinPriceWithTreatments != null ? sereServTotalHeinPriceWithTreatments.Sum(o => o.VIR_TOTAL_PRICE ?? 0) : 0;
                }
                Inventec.Common.Logging.LogSystem.Debug("LoadTotalSereServByHeinWithTreatment.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadSereServTotalHeinPriceWithTreatment(long treatmentId)
        {
            try
            {
                List<HIS_SERE_SERV> sereServTotalHeinPriceWithTreatments = null;
                List<long> SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                if (this.sereServsInTreatmentRaw == null || this.sereServsInTreatmentRaw.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServView7Filter sereServFilter = new HisSereServView7Filter();
                    sereServFilter.TDL_TREATMENT_ID = treatmentId;
                    sereServFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                    sereServFilter.TDL_SERVICE_REQ_TYPE_IDs = SERVICE_REQ_TYPE_IDs;
                    sereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                    sereServTotalHeinPriceWithTreatments = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_7, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);
                }
                else
                {
                    sereServTotalHeinPriceWithTreatments = this.sereServsInTreatmentRaw.Where(o =>
                        o.TDL_TREATMENT_ID == treatmentId
                        && o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        && SERVICE_REQ_TYPE_IDs.Contains(o.TDL_SERVICE_REQ_TYPE_ID)
                        && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        ).ToList();
                }
                //Nếu sửa đơn thuốc thì lấy tổng tiền thuốc bảo hiểm hồ sơ trừ đi đơn đang sửa
                if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0)
                {
                    this.totalPriceBHYT = sereServTotalHeinPriceWithTreatments.Where(o => o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID).Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                }
                //Ngược lại lấy tất cả tổng tiền thuốc bảo hiểm trong hồ sơ
                else
                {
                    this.totalPriceBHYT = sereServTotalHeinPriceWithTreatments.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataSereServWithTreatment(HisTreatmentWithPatientTypeInfoSDO treatment, long intructionTime)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithTreatment.1");
                if (treatment != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithTreatment.2");
                    CommonParam param = new CommonParam();

                    List<long> setyAllowsIds = new List<long>();
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                    //if (this.sereServsInTreatmentRaw == null || this.sereServsInTreatmentRaw.Count == 0)
                    //{
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithTreatment.3");
                    HisSereServFilter hisSereServFilter = new HisSereServFilter();
                    hisSereServFilter.TREATMENT_ID = treatment.ID;
                    hisSereServFilter.TDL_SERVICE_TYPE_IDs = setyAllowsIds;
                    this.sereServWithTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumerNoStore, hisSereServFilter, ProcessLostToken, param);
                    //}
                    //else
                    //{
                    //    Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithTreatment.4");
                    //    this.sereServWithTreatment = this.sereServsInTreatmentRaw.Where(o => o.TDL_TREATMENT_ID == treatment.ID && setyAllowsIds.Contains(o.TDL_SERVICE_TYPE_ID)).ToList();
                    //}
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithTreatment.5");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_MEDICINE_BEAN_1> GetMedicineBeanByExpMestMedicine(List<long> expMestMedicineIds)
        {
            List<V_HIS_MEDICINE_BEAN_1> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineBeanFilter hismedicineBeanFilter = new HisMedicineBeanFilter();
                hismedicineBeanFilter.EXP_MEST_MEDICINE_IDs = expMestMedicineIds;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN_1>>(RequestUriStore.HIS_MEDICINE_BEAN__GETVIEW, ApiConsumers.MosConsumer, hismedicineBeanFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_MATERIAL_BEAN_1> GetMaterialBeanByExpMestMedicine1(List<long> expMestMaterialIds)
        {
            List<V_HIS_MATERIAL_BEAN_1> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisMaterialBeanViewFilter hismaterialBeanFilter = new HisMaterialBeanViewFilter();
                hismaterialBeanFilter.EXP_MEST_MATERIAL_IDs = expMestMaterialIds;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1>>(RequestUriStore.HIS_MATERIAL_BEAN__GETVIEW, ApiConsumers.MosConsumer, hismaterialBeanFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<HIS_MATERIAL_BEAN> GetMaterialBeanByExpMestMedicine(List<long> expMestMaterialIds)
        {
            List<HIS_MATERIAL_BEAN> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisMaterialBeanFilter hismaterialBeanFilter = new HisMaterialBeanFilter();
                hismaterialBeanFilter.EXP_MEST_MATERIAL_IDs = expMestMaterialIds;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>(RequestUriStore.HIS_MATERIAL_BEAN__GET, ApiConsumers.MosConsumer, hismaterialBeanFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> GetExpMestMedicineByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter searchMedicineFilter = new HisExpMestMedicineViewFilter();
                searchMedicineFilter.ORDER_DIRECTION = "ASC";
                searchMedicineFilter.ORDER_FIELD = "ID";
                searchMedicineFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, searchMedicineFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> GetExpMestMaterialByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMaterialViewFilter searchMaterialFilter = new HisExpMestMaterialViewFilter();
                searchMaterialFilter.ORDER_DIRECTION = "ASC";
                searchMaterialFilter.ORDER_FIELD = "ID";
                searchMaterialFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, searchMaterialFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY> GetServiceReqMetyByServiceReqId(long serviceReqId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqMetyFilter expMestMetyFilter = new HisServiceReqMetyFilter();
                expMestMetyFilter.SERVICE_REQ_ID = serviceReqId;
                return new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>(RequestUriStore.HIS_SERVICE_REQ_METY__GET, ApiConsumers.MosConsumer, expMestMetyFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_METY_REQ> GetExpMestMetyReqByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMetyReqFilter filter = new HisExpMestMetyReqFilter();
                filter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATY_REQ> GetExpMestMatyReqByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMatyReqFilter filter = new HisExpMestMatyReqFilter();
                filter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY> GetServiceReqMatyByServiceReqId(long serviceReqId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqMatyFilter expMestMatyFilter = new HisServiceReqMatyFilter();
                expMestMatyFilter.SERVICE_REQ_ID = serviceReqId;
                return new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUriStore.HIS_SERVICE_REQ_MATY__GET, ApiConsumers.MosConsumer, expMestMatyFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE GetAcinInteractiveByCode(string activeIngrBhytCode, string activeIngrBhytConflicCode)
        {
            MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE>()
                    .FirstOrDefault(o => o.ACTIVE_INGREDIENT_CODE == activeIngrBhytCode
                        && o.CONFLICT_CODE == activeIngrBhytConflicCode
                    );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcinByMedicineType(List<long> medicineTypeIds)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeIds.Contains(o.MEDICINE_TYPE_ID)
                    ).ToList();

                var medis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EMTE_MEDICINE_TYPE> GetEmteMedicineTypeByExpMestId(long expMestTemplateId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmteMedicineTypeViewFilter filter = new HisEmteMedicineTypeViewFilter();
                filter.EXP_MEST_TEMPLATE_ID = expMestTemplateId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                return new BackendAdapter(param).Get<List<V_HIS_EMTE_MEDICINE_TYPE>>(HisRequestUriStore.HIS_EMTE_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EMTE_MATERIAL_TYPE> GetEmteMaterialTypeByExpMestId(long expMestTemplateId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmteMaterialTypeViewFilter filter = new HisEmteMaterialTypeViewFilter();
                filter.EXP_MEST_TEMPLATE_ID = expMestTemplateId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                return new BackendAdapter(param).Get<List<V_HIS_EMTE_MATERIAL_TYPE>>(HisRequestUriStore.HIS_EMTE_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EQUIPMENT_SET_MATY> GetMaterialTypeByEquipmentSetId(long equipmentSetId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEquipmentSetMatyViewFilter filter = new HisEquipmentSetMatyViewFilter();
                filter.EQUIPMENT_SET_ID = equipmentSetId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";
                return new BackendAdapter(param).Get<List<V_HIS_EQUIPMENT_SET_MATY>>(RequestUriStore.HIS_EQUIPMENT_SET_MATY__GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private async Task LoadDataTracking(bool isChangeData = true)
        {
            try
            {
                // Neu truyen vao tracking thi hien thi mac dinh tracking nay #23109
                // Set giá trị mặc định cho tờ điều trị ở chức năng tủ trực #24587
                // + nếu tờ điều trị đã được lưu, thì chọn mặc định tờ điều trị trên form kê đơn là tờ điều trị đang được mở.
                // + nếu tờ điều trị chưa được lưu, thì hiển thị mặc định theo cấu hình hệ thống "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking"

                LogSystem.Debug("LoadDataTracking => 1");
                //Init Control
                CommonParam param = new CommonParam();
                if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                {
                    this.isInitTracking = false;
                    lciPhieuDieuTri.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    return;
                }

                if (GlobalStore.IsCabinet)
                {
                    //Check đối tượng nội trú hoặc ngoại trú
                    HisPatientTypeAlterViewAppliedFilter filterPatientTypeAlter = new HisPatientTypeAlterViewAppliedFilter();
                    filterPatientTypeAlter.TreatmentId = this.treatmentId;
                    filterPatientTypeAlter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = await new BackendAdapter(param).GetAsync<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filterPatientTypeAlter, param);
                    if (patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                        && patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    {
                        this.isInitTracking = false;
                        lciPhieuDieuTri.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        return;
                    }
                }

                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = this.treatmentId;
                List<HIS_TRACKING> trackings = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);


                if (trackings == null || trackings.Count == 0)
                {
                    this.isInitTracking = false;
                    return;
                }

                trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();

                List<string> intructionDateSelectedProcess = new List<string>();
                foreach (var item in this.intructionTimeSelecteds)
                {
                    string intructionDate = item.ToString().Substring(0, 8);
                    intructionDateSelectedProcess.Add(intructionDate);
                }

                this.trackingADOs = new List<TrackingADO>();
                foreach (var item in trackings)
                {
                    TrackingADO tracking = new TrackingADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TrackingADO>(tracking, item);
                    tracking.TrackingTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                    this.trackingADOs.Add(tracking);
                }
                trackingADOs = trackingADOs.OrderByDescending(o => o.TRACKING_TIME).ToList();
                long trackIdSet = 0;
                List<HIS_TRACKING> trackingTemps = new List<HIS_TRACKING>();
                if (this.Listtrackings != null && this.Listtrackings.Count > 0 && !isChangeData)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Ben ngoai truyen vao to dieu tri____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Listtrackings), Listtrackings));
                    if (chkMultiIntructionTime.Checked == false)
                    {
                        trackingTemps = this.Listtrackings;
                        trackIdSet = this.Listtrackings[0].ID;
                        Inventec.Common.Logging.LogSystem.Info("Truong hop co tracking ben ngoai truyen vao, gan vao doi tuong tracking de phuc vu cho nghiep vu load cac du lieu: + Mã bệnh chỉnh,+ Tên bệnh chính,+ Mã bệnh phụ,+ Tên bệnh phụ,+ Mã bệnh YHCT chính,+ Tên bệnh YHCT chính,+ Mã bệnh YHCT phụ,+ Tên bệnh YHCT phụ tu ban ghi tracking do____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackIdSet), trackIdSet));
                    }
                }
                else
                {
                    trackingTemps = trackings.Where(o => intructionDateSelectedProcess.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                        //&& o.DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId))
                        && o.CREATOR.ToUpper() == this.txtLoginName.Text.ToUpper())
                        .OrderByDescending(o => o.TRACKING_TIME).ToList();

                    if (trackingTemps != null && trackingTemps.Count > 0
                        && HisConfigCFG.IsDefaultTracking
                        && chkMultiIntructionTime.Checked == false
                        )
                    {
                        Inventec.Common.Logging.LogSystem.Info("Truong hop khong co tracking ben ngoai truyen vao, nhung co cau hình HisConfigCFG.IsDefaultTracking =" + HisConfigCFG.IsDefaultTracking + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackIdSet), trackIdSet));
                        trackIdSet = trackingTemps[0].ID;
                        this.Listtrackings = new List<HIS_TRACKING>();
                        this.Listtrackings.Add(trackingTemps[0]);
                    }
                }
                this.InitComboTracking(cboPhieuDieuTri);
                if (trackIdSet > 0)
                {
                    cboPhieuDieuTri.EditValue = trackIdSet;
                    cboPhieuDieuTri.Properties.Buttons[1].Visible = true;

                    if (HisConfigCFG.IsServiceReqIcdOption)
                    {
                        this.LoadIcdToControl(trackingTemps[0].ICD_CODE, trackingTemps[0].ICD_NAME);
                        isNotProcessWhileChangedTextSubIcd = true;
                        this.LoadDataToIcdSub(trackingTemps[0].ICD_SUB_CODE, trackingTemps[0].ICD_TEXT);
                        isNotProcessWhileChangedTextSubIcd = false;
                    }
                    else
                    {
                        if (HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                            this.LoadIcdDefault();//TODO
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelecteds), intructionTimeSelecteds)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isChangeData), isChangeData)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isInitTracking), isInitTracking)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackIdSet), trackIdSet)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Listtrackings), Listtrackings));
                this.isInitTracking = false;

                LogSystem.Debug("LoadDataTracking => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
