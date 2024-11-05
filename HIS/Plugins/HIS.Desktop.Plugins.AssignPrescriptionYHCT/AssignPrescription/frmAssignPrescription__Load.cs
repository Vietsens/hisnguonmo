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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Utility;
using HIS.UC.Icd.ADO;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
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

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workingPatientType), workingPatientType)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataOtherPaySourceTmps), dataOtherPaySourceTmps)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_ID), currentRowSereServADO.OTHER_PAY_SOURCE_ID)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_NAME), currentRowSereServADO.OTHER_PAY_SOURCE_NAME));
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("FillDataOtherPaySourceDataRow.2____");
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
                if (medicineTypeSDO != null && this.actionType == GlobalVariables.ActionAdd)
                {
                    medicineTypeSDO.EXP_MEST_REASON_ID = null;
                    medicineTypeSDO.EXP_MEST_REASON_CODE = "";
                    medicineTypeSDO.EXP_MEST_REASON_NAME = "";

                    var dataExmeReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXME_REASON_CFG>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue
                            && o.PATIENT_CLASSIFY_ID == this.Histreatment.TDL_PATIENT_CLASSIFY_ID && o.TREATMENT_TYPE_ID == this.Histreatment.TDL_TREATMENT_TYPE_ID && (o.PATIENT_TYPE_ID == null || o.PATIENT_TYPE_ID == medicineTypeSDO.PATIENT_TYPE_ID)
                            && (o.OTHER_PAY_SOURCE_ID == null || o.OTHER_PAY_SOURCE_ID == medicineTypeSDO.OTHER_PAY_SOURCE_ID)).ToList();

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

        private void LoadDonThuocTruocDay()
        {
            try
            {
                Thread threadLoadDonThuoc = new Thread(ThreadLoadDonThuocCu);
                threadLoadDonThuoc.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAllergenic(long patientId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisAllergenicFilter filter = new HisAllergenicFilter();
                filter.TDL_PATIENT_ID = patientId;
                allergenics = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_ALLERGENIC>>("api/HisAllergenic/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private async Task LoadDataToLocal()
        {
            try
            {
                Task.Run(() => LoadViewMedicineTypeTut());
                Task t1 = LoadViewMediStock();
                Task t2 = LoadViewRoom();
                Task t3 = LoadViewMestRoom();
                Task t4 = LoadViewMedicineUseForm();
                Task t5 = LoadMestPatientType();
                Task t6 = LoadExpMestTemplate();
                Task t7 = LoadViewAcsUser();
                Task t8 = InitPatientAgeInfo();
                await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7, t8);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadViewMedicineType()
        {
            List<V_HIS_MEDICINE_TYPE> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<V_HIS_MEDICINE_TYPE>();

        }

        private void LoadMedicineMaterialTypeComboADO()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("t2");
                List<MedicineMaterialTypeComboADO> mediMateTypeComboADOs = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true);
                Inventec.Common.Logging.LogSystem.Error("t2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadViewAcsUser()
        {
            List<ACS_USER> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<ACS_USER>();

        }

        private async Task LoadMestPatientType()
        {
            List<HIS_MEST_PATIENT_TYPE> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<HIS_MEST_PATIENT_TYPE>();

        }
        private async Task LoadExpMestTemplate()
        {
            List<HIS_EXP_MEST_TEMPLATE> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<HIS_EXP_MEST_TEMPLATE>();

        }

        private async Task LoadViewMaterialType()
        {
            List<V_HIS_MATERIAL_TYPE> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<V_HIS_MATERIAL_TYPE>();

        }

        private async Task LoadViewMediStock()
        {
            List<V_HIS_MEDI_STOCK> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<V_HIS_MEDI_STOCK>();

        }

        private async Task LoadViewRoom()
        {
            List<V_HIS_ROOM> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<V_HIS_ROOM>();

        }

        private async Task LoadViewMestRoom()
        {
            List<V_HIS_MEST_ROOM> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<V_HIS_MEST_ROOM>();

        }
        private async Task LoadViewMedicineUseForm()
        {
            List<HIS_MEDICINE_USE_FORM> data = await HIS.Desktop.LocalStorage.BackendData.BackendDataWorkerAsync.Get<HIS_MEDICINE_USE_FORM>();

        }

        private void LoadViewMedicineTypeTut()
        {
            Inventec.Common.Logging.LogSystem.Error("t1");
            List<HIS_MEDICINE_TYPE_TUT> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE_TUT>();
            Inventec.Common.Logging.LogSystem.Error("t1");
        }



        private void ThreadLoadDonThuocCu()
        {
            try
            {

                //Neu la in gop don thuoc thi moi load
                string savePrintMpsDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.SAVE_PRINT_MPS_DEFAULT);
                if (savePrintMpsDefault != "Mps000234")
                    return;

                CommonParam param = new CommonParam();
                //Load đơn phòng khám
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this.treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                serviceReqPrints = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                if (serviceReqPrints == null || serviceReqPrints.Count == 0)
                    return;
                //Load expmest
                serviceReqPrints = serviceReqPrints.Where(o => o.ID != this.serviceReqParentId).ToList();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = serviceReqPrints.Select(o => o.ID).ToList();
                expMestPrints = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                if (expMestPrints == null || expMestPrints.Count == 0)
                    return;

                //Laays thuoc va tu trong kho

                HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMestPrints.Select(o => o.ID).ToList();
                expMestMedicinePrints = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMestPrints.Select(o => o.ID).ToList();
                expMestMaterialPrints = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

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
                AssignPrescriptionWorker.Instance.MediMatyCreateWorker = new MediMatyCreateWorker(GetDataAmountOutOfStock, SetDefaultMediStockForData, ChoosePatientTypeDefaultlService, GetPatientTypeId, GetNumRow, SetNumRow, GetMediMatyTypeADOs, GetIsAutoCheckExpend);
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
        private void LoadDefaultSoNgayHoaDonFromAppointmentTimeDefault()
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
                var appointmentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, ProcessLostToken, param).SingleOrDefault();
                if (appointmentTreatment != null && appointmentTreatment.ID > 0)
                {
                    //Kiểm tra thời gian hẹn khám trong thông tin hẹn khám (tương ứng với appointment_id trong treament) là ngày nào.
                    //So sánh ngày đó với ngày hiện tại. Lúc đó, số ngày điền sẵn vào đơn sẽ theo công thức:
                    //Số ngày trên đơn = Số ngày hẹn khám mặc định - MIN ((ngày hẹn khám - ngày hiện tại), 0)
                    System.DateTime dtAppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTreatment.APPOINTMENT_TIME ?? 0).Value;
                    TimeSpan diff__hour = (System.DateTime.Now.Date - dtAppointmentTime.Date);
                    double totaldays = diff__hour.TotalDays;

                    long songaytrendon = (songayHKmacDinh - Math.Min((long)totaldays, 0));
                    this.spinSoNgay.EditValue = songaytrendon;
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
                long roomTypeId = GetRoomTypeId();
                //this.repositoryItemSpinAmount__MedicinePage.BeginUpdate();
                if ((roomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG || GlobalStore.IsCabinet) && ((mediMatyADO.IsAllowOdd.HasValue && mediMatyADO.IsAllowOdd.Value == true) || (mediMatyADO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)))
                {
                    this.spinAmount.Properties.DisplayFormat.FormatString = "#,##0.00";
                    this.spinAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.spinAmount.Properties.EditFormat.FormatString = "#,##0.00";
                    this.spinAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.None;
                    this.spinAmount.Properties.Mask.EditMask = "n";
                    this.spinAmount.Properties.Mask.UseMaskAsDisplayFormat = true;

                    //this.repositoryItemSpinAmount__MedicinePage.Properties.DisplayFormat.FormatString = "#,##0.00";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.EditFormat.FormatString = "#,##0.00";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.None;
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.EditMask = "n";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.UseMaskAsDisplayFormat = true;
                }
                else
                {
                    this.spinAmount.Properties.Mask.EditMask = "d";
                    this.spinAmount.Properties.Mask.UseMaskAsDisplayFormat = true;

                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.EditMask = "d";
                    //this.repositoryItemSpinAmount__MedicinePage.Properties.Mask.UseMaskAsDisplayFormat = true;
                }
                this.spinAmount.Update();
                //this.repositoryItemSpinAmount__MedicinePage.EndUpdate();
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
                    grcIsOutKtcFee__TabMedicine.Visible = false;
                }

                //An hien cot hao phi
                long isVisibleColumnHaoPhi = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI);
                if (isVisibleColumnHaoPhi == 1)
                {
                    grcExpend__TabMedicine.Visible = false;
                }

                if (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && this.GetSereServInKip() <= 0)
                {
                    this.grcExpend__TabMedicine.Visible = false;
                }

                //if (GlobalStore.IsTreatmentIn)
                //{
                //    grcTotalPrice__TabMedicine.Visible = false;
                //}
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboRepositoryPatientType(repositoryItemcboPatientType_TabMedicine_GridLookUp, currentPatientTypeWithPatientTypeAlter);
                LogSystem.Debug("FillDataToControlsForm => InitComboRepositoryPatientType");
                InitComboRepositoryPatientType(repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());

                //InitComboRepositoryEquipmentSet(repositoryItemGridLookUpEditEquipmentSet__Enabled, HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>(false, true));

                //InitComboRepositoryEquipmentSet(repositoryItemGridLookUpEditEquipmentSet__Disabled, HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>(false, true));

                LogSystem.Debug("FillDataToControlsForm => InitComboRepositoryPatientType");
                LogSystem.Debug("FillDataToControlsForm => InitComboHtu");
                InitComboUser();
                LogSystem.Debug("FillDataToControlsForm => InitComboUser");
                InitComboMedicineUseForm(cboMedicineUseForm, null);
                LogSystem.Debug("FillDataToControlsForm => InitComboMedicineUseForm");
                InitComboMedicineUseForm(repositoryItemcboMedicineUseForm, null);
                LogSystem.Debug("FillDataToControlsForm => InitComboMedicineUserepositoryItemForm");
                InitComboExpMestTemplate();
                LogSystem.Debug("FillDataToControlsForm => InitComboExpMestTemplate");
                InitComboMediStockAllow(0);
                LogSystem.Debug("FillDataToControlsForm => InitComboMediStockAllow");
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
                this.FillAllPatientInfoSelectedInForm();
                this.cboPhieuDieuTri.EditValue = null;
                this.cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                this.cboPhieuDieuTri.Properties.DataSource = null;
                this.trackingADOs = new List<TrackingADO>();
                this.LoadDataTracking();
                this.InitComboTracking(cboPhieuDieuTri);
                Inventec.Common.Logging.LogSystem.Debug("trackingADOs.count=" + trackingADOs != null ? trackingADOs.Count + "" : 0 + "");
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
                if (GlobalStore.IsExecutePTTT
                    || (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null
                    && this.assignPrescriptionEditADO.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && IsRoomTypeIsExecuteRoom(this.assignPrescriptionEditADO.ServiceReq.REQUEST_ROOM_ID)))
                {
                    this.FillAllPatientInfoSelectedInFormAtExecutePTTT();
                }
                else
                {
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        this.patientSelectProcessor.Load(this.ucPatientSelect);
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

        private void FillAllPatientInfoSelectedInFormWithThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.FillAllPatientInfoSelectedInForm(); }));
                }
                else
                {
                    this.FillAllPatientInfoSelectedInForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToPatientInfoWithThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadDataToPatientInfo(); }));
                }
                else
                {
                    this.LoadDataToPatientInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillAllPatientInfoSelectedInForm()
        {
            try
            {

                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(this.treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First());

                this.PatientTypeWithTreatmentView7();
                LogSystem.Debug("Loaded PatientTypeWithTreatmentView7 info");
                Task.Run(() => CheckWarningOverTotalPatientPrice());
                this.CreateThreadLoadDataSereServWithTreatment(this.currentTreatmentWithPatientType);
                LogSystem.Debug("Loaded CreateThreadLoadDataSereServWithTreatment (Truy van danh sach cac loai dich vu da chi dinh trong ngày, lay tu view v_his_sere_serv_8)");

                this.LoadSereServTotalHeinPriceWithTreatment(treatmentId);
                LogSystem.Debug("Loaded LoadDSereServ1WithTreatment (Truy vấn tổng số tiền thuốc BHYT đã kê trong hồ sơ điều trị Lấy thông tin từ d_his_sere_serv_1 theo tdl_treatment_id và patient_type_id (bhyt))");


                this.FillTreatmentInfo__PatientType();//tinh toan va hien thi thong tin ve tong tien tat ca cac dich vu dang chi dinh
                LogSystem.Debug("Loaded FillTreatmentInfo__PatientType (Gan du lieu luy ke tien thuoc lay tu DSereServ1)");

                this.LoadIcdDefault();
                LogSystem.Debug("Loaded LoadIcdDefault");

                this.LoadDefaultSoNgayHoaDonFromAppointmentTimeDefault();
                LogSystem.Debug("Loaded LoadAppointmentTimeDefault");

                LogSystem.Debug("Begin FillDataToComboPriviousExpMest");
                this.FillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);
                LogSystem.Debug("End FillDataToComboPriviousExpMest");

                this.InitWorker();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillAllPatientInfoSelectedInFormAtExecutePTTT()
        {
            try
            {

                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(this.treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First());

                this.PatientTypeWithTreatmentView7();
                LogSystem.Debug("Loaded PatientTypeWithTreatmentView7 info");
                Task.Run(() => CheckWarningOverTotalPatientPrice());
                this.CreateThreadLoadDataSereServWithTreatment(this.currentTreatmentWithPatientType);
                LogSystem.Debug("Loaded CreateThreadLoadDataSereServWithTreatment (Truy van danh sach cac loai dich vu da chi dinh trong ngày, lay tu view v_his_sere_serv_8)");


                this.LoadSereServTotalHeinPriceWithTreatment(treatmentId);
                LogSystem.Debug("Loaded LoadDSereServ1WithTreatment (Truy vấn tổng số tiền thuốc BHYT đã kê trong hồ sơ điều trị Lấy thông tin từ d_his_sere_serv_1 theo tdl_treatment_id và patient_type_id (bhyt))");

                this.FillTreatmentInfo__PatientType();//tinh toan va hien thi thong tin ve tong tien tat ca cac dich vu dang chi dinh
                LogSystem.Debug("Loaded FillTreatmentInfo__PatientType (Gan du lieu luy ke tien thuoc lay tu DSereServ1)");

                this.LoadIcdDefault();
                LogSystem.Debug("Loaded LoadIcdDefault");

                LogSystem.Debug("Begin FillDataToComboPriviousExpMest");
                this.CreateThreadFillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);
                LogSystem.Debug("End FillDataToComboPriviousExpMest");

                this.InitWorker();
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
        private void CheckWarningOverTotalPatientPrice()
        {
            try
            {
                //Kiem tra cau hinh
                if (!HisConfigCFG.IsWarningOverTotalPatientPrice || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.actionType != GlobalVariables.ActionAdd)
                    return;

                if ((!GlobalStore.IsTreatmentIn
                    && !GlobalStore.IsCabinet
                    && !GlobalStore.IsExecutePTTT))
                    return;

                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = treatmentId;
                var treatmentFees = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param);

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
                totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                exemption = treatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan

                decimal warningOverTotalPatientPrice = HisConfigCFG.WarningOverTotalPatientPrice;
                if (transfer > warningOverTotalPatientPrice)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(this, String.Format("Bệnh nhân đang thiếu viện phí ({0} đồng). Bạn có muốn tiếp tục?", Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator)), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult != DialogResult.OK)
                    {
                        this.Close();
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

        internal void ReloadDataAvaiableMediBeanInCombo()
        {
            try
            {
                var extMediBean = this.mediMatyTypeADOs.Any(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                    || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
                if (extMediBean)
                {
                    if (rdOpionGroup.SelectedIndex == 0)
                    {
                        if (this.treatmentFinishProcessor != null)
                            this.treatmentFinishProcessor.Reload(this.ucTreatmentFinish, this.GetDateADO());

                        this.InitDataMetyMatyTypeInStockD();
                        this.RebuildMediMatyWithInControlContainer(GetDataMediMatyInStock());
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
                if (HisConfigCFG.IsloadIcdFromExamServiceExecute && this.icdExam != null)
                {

                    IcdInputADO icd = new IcdInputADO();
                    icd.ICD_CODE = this.icdExam.ICD_CODE;
                    icd.ICD_NAME = this.icdExam.ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    IcdInputADO icdCause = new IcdInputADO();
                    icdCause.ICD_CODE = this.icdExam.ICD_CAUSE_CODE;
                    icdCause.ICD_NAME = this.icdExam.ICD_CAUSE_NAME;
                    if (ucIcdCause != null)
                    {
                        icdCauseProcessor.Reload(ucIcdCause, icdCause);
                    }
                    
                    var icdCaus = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE == this.icdExam.ICD_CODE);
                    if (icdCaus != null)
                    {
                        this.icdCauseProcessor.SetRequired(this.ucIcdCause, (icdCaus.IS_REQUIRE_CAUSE == 1));
                    }

                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = this.icdExam.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = this.icdExam.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                }
                else if (this.currentTreatmentWithPatientType != null)
                {
                    IcdInputADO icd = new IcdInputADO();
                    icd.ICD_CODE = currentTreatmentWithPatientType.ICD_CODE;
                    icd.ICD_NAME = currentTreatmentWithPatientType.ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    IcdInputADO icdCause = new IcdInputADO();
                    icdCause.ICD_CODE = currentTreatmentWithPatientType.ICD_CAUSE_CODE;
                    icdCause.ICD_NAME = currentTreatmentWithPatientType.ICD_CAUSE_NAME;
                    if (ucIcdCause != null)
                    {
                        icdCauseProcessor.Reload(ucIcdCause, icdCause);
                    }
                    
                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = currentTreatmentWithPatientType.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = currentTreatmentWithPatientType.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                }
                LogSystem.Debug("cau hinh HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute: " + HisConfigCFG.IsloadIcdFromExamServiceExecute);
                if(this.icdExam != null &&
                    (HisConfigCFG.IsloadIcdFromExamServiceExecute || string.IsNullOrEmpty(this.currentTreatmentWithPatientType.TRADITIONAL_ICD_CODE)))
                {
                    LogSystem.Debug("Ho so dieu tri khong co YHCT -> load tu xlkham ");
                    //yhct
                    if (!string.IsNullOrEmpty(this.icdExam.TRADITIONAL_ICD_CODE))
                    {
                        IcdInputADO icdYHCT = new IcdInputADO();
                        icdYHCT.ICD_CODE = this.icdExam.TRADITIONAL_ICD_CODE;
                        icdYHCT.ICD_NAME = this.icdExam.TRADITIONAL_ICD_NAME;
                        if (ucIcdYHCT != null)
                        {
                            icdProcessorYHCT.Reload(ucIcdYHCT, icdYHCT);
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(this.icdExam.TRADITIONAL_ICD_SUB_CODE))
                    {
                        SecondaryIcdDataADO subIcdYHCT = new SecondaryIcdDataADO();
                        subIcdYHCT.ICD_SUB_CODE = this.icdExam.TRADITIONAL_ICD_SUB_CODE;
                        subIcdYHCT.ICD_TEXT = this.icdExam.TRADITIONAL_ICD_TEXT;
                        if (ucSecondaryIcdYHCT != null)
                        {
                            subIcdProcessorYHCT.Reload(ucSecondaryIcdYHCT, subIcdYHCT);
                        }
                    }
                    
                    //
                }
                else if(!string.IsNullOrEmpty(this.currentTreatmentWithPatientType.TRADITIONAL_ICD_CODE) 
                    && !string.IsNullOrEmpty(this.currentTreatmentWithPatientType.TRADITIONAL_ICD_SUB_CODE))
                {
                    LogSystem.Debug("Ho so dieu tri co YHCT -> load tu treatment ");
                    //yhct
                    if (!string.IsNullOrEmpty(this.currentTreatmentWithPatientType.TRADITIONAL_ICD_CODE))
                    {
                        IcdInputADO icdYHCT = new IcdInputADO();
                        icdYHCT.ICD_CODE = this.currentTreatmentWithPatientType.TRADITIONAL_ICD_CODE;
                        icdYHCT.ICD_NAME = this.currentTreatmentWithPatientType.TRADITIONAL_ICD_NAME;
                        if (ucIcdYHCT != null)
                        {
                            icdProcessorYHCT.Reload(ucIcdYHCT, icdYHCT);
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(this.currentTreatmentWithPatientType.TRADITIONAL_ICD_SUB_CODE))
                    {
                        SecondaryIcdDataADO subIcdYHCT = new SecondaryIcdDataADO();
                        subIcdYHCT.ICD_SUB_CODE = this.currentTreatmentWithPatientType.TRADITIONAL_ICD_SUB_CODE;
                        subIcdYHCT.ICD_TEXT = this.currentTreatmentWithPatientType.TRADITIONAL_ICD_TEXT;
                        if (ucSecondaryIcdYHCT != null)
                        {
                            subIcdProcessorYHCT.Reload(ucSecondaryIcdYHCT, subIcdYHCT);
                        }
                    }
                    
                    //
                }
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
        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlServiceOther(long patientTypeId, MediMatyTypeADO medimaty)
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
                return (result ?? new HIS_PATIENT_TYPE());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// - 1 thuốc được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã hoạt chất BHYT (ACTIVE_INGR_BHYT_CODE), số đăng ký (REGISTER_NUMBER), và nhóm BHYT thuộc 1 trong các loại: "Thuốc trong danh mục", "Thuốc thanh toán theo tỷ lệ" hoặc "Thuốc ung thư, chống thải ghép"
        /// - 1 vật tư được coi là "Có đủ thông tin BHYT" khi thỏa mãn:
        /// Khai báo đủ các thông tin: mã BHYT (HEIN_SERVICE_BHYT_CODE), tên BHYT (HEIN_SERVICE_BHYT_NAME), và nhóm BHYT thuộc 1 trong các loại: "Vật tư thay thế", "Vật tư trong danh mục", "Vật tư thanh toán theo tỷ lệ"
        /// </summary>
        /// <returns></returns>
        private bool IsFullHeinInfo(MediMatyTypeADO medimaty)
        {
            bool valid = false;
            try
            {
                valid = (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC &&
                            !String.IsNullOrEmpty(medimaty.ACTIVE_INGR_BHYT_CODE)
                    //&& !String.IsNullOrEmpty(medimaty.REGISTER_NUMBER)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT))
                        ||
                        (medimaty.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT &&
                            !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_CODE)
                            && !String.IsNullOrEmpty(medimaty.HEIN_SERVICE_BHYT_NAME)
                            && (medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL
                            || medimaty.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, long serviceTypeId)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                MediMatyTypeADO mediMatyTypeADO = null;
                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
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

                return this.ChoosePatientTypeDefaultlServiceOther(patientTypeId, mediMatyTypeADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ LoadDataToCurrentServiceReqData(long Id)
        {
            MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serq = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqViewFilter filter = new MOS.Filter.HisServiceReqViewFilter();
                filter.ID = Id;

                var listSerq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (listSerq != null && listSerq.Count > 0)
                {
                    serq = listSerq[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serq;
        }

        private HisTreatmentWithPatientTypeInfoSDO LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
        {
            HisTreatmentWithPatientTypeInfoSDO treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.INTRUCTION_TIME = intructionTime;
                treatment = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumers.MosConsumer, filter, ProcessLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                filter.InstructionTime = ((intructionTime > 0) ? intructionTime : Inventec.Common.DateTime.Get.Now() ?? 0);
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeWithTreatmentView7()
        {
            try
            {
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
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

                var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).Distinct().ToList();
                //Lấy tất cả chính sách giá của tất cả các đối tượng tt và đối tượng chuyển đổi của bệnh nhân
                var servicePatyTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
                    .ToList();
                //this.servicePatyAllows = servicePatyTemps.GroupBy(o => o.SERVICE_ID)
                //    .ToDictionary(o => o.Key, o => o.ToList());

                //Lọc các đối tượng thanh toán không có chính sách giá
                //var patientHasSetyIds = servicePatyTemps.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                //this.currentPatientTypeWithPatientTypeAlter = this.currentPatientTypeWithPatientTypeAlter.Where(o => patientHasSetyIds.Contains(o.ID)).ToList();
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

        private void LoadTotalSereServByHeinWithTreatment(long treatmentId)
        {
            try
            {
                LogSystem.Debug("LoadTotalSereServByHeinWithTreatment => start");
                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = treatmentId;
                hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                var sereServByHeinWithTreatments = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, param);
                //Nếu sửa đơn thuốc thì lấy tổng tiền bảo hiểm hồ sơ trừ đi đơn đang sửa
                if (this.assignPrescriptionEditADO != null && this.assignPrescriptionEditADO.ServiceReq != null && this.assignPrescriptionEditADO.ServiceReq.ID > 0)
                {
                    this.totalHeinByTreatment = sereServByHeinWithTreatments.Where(o => o.SERVICE_REQ_ID != this.assignPrescriptionEditADO.ServiceReq.ID).Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0);
                }
                //Ngược lại lấy tất cả tổng tiền bảo hiểm trong hồ sơ
                else
                {
                    this.totalHeinByTreatment = sereServByHeinWithTreatments.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0);
                }
                LogSystem.Debug("LoadTotalSereServByHeinWithTreatment => end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServTotalHeinPriceWithTreatment(long treatmentId)
        {
            try
            {
                LogSystem.Debug("Load LoadSereServTotalHeinPriceWithTreatment start");
                CommonParam param = new CommonParam();
                HisSereServView7Filter sereServFilter = new HisSereServView7Filter();
                sereServFilter.TDL_TREATMENT_ID = treatmentId;
                sereServFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                sereServFilter.TDL_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                sereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                var sereServTotalHeinPriceWithTreatments = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_7>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_7, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, param);

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

                LogSystem.Debug("Loaded LoadSereServTotalHeinPriceWithTreatment end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadDataSereServWithTreatment(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataSereServWithTreatmentNewThread));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadDataSereServWithTreatmentNewThread(object param)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadDataSereServWithTreatment((HisTreatmentWithPatientTypeInfoSDO)param, 0); }));
                }
                else
                {
                    this.LoadDataSereServWithTreatment((HisTreatmentWithPatientTypeInfoSDO)param, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataSereServWithTreatment(HisTreatmentWithPatientTypeInfoSDO treatment, long intructionTime)
        {
            try
            {
                if (treatment != null)
                {
                    LogSystem.Debug("Load LoadDataSereServWithTreatment start");
                    CommonParam param = new CommonParam();
                    HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
                    hisSereServFilter.TREATMENT_ID = treatment.ID;
                    //if (intructionTime > 0)
                    //{
                    //    hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64((intructionTime.ToString().Substring(8) + "000000"));
                    //    hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.ToString().Substring(8) + "235959");
                    //}
                    //else
                    //{
                    //    hisSereServFilter.INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                    //    hisSereServFilter.INTRUCTION_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
                    //}
                    List<long> setyAllowsIds = new List<long>();
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                    hisSereServFilter.SERVICE_TYPE_IDs = setyAllowsIds;
                    this.sereServWithTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_1, ApiConsumers.MosConsumerNoStore, hisSereServFilter, ProcessLostToken, param);

                    LogSystem.Debug("Loaded LoadDataSereServWithTreatment end");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCurrentMestRoom()
        {
            try
            {
                var mediIds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Select(o => o.ID).ToList();
                var roomIds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Select(o => o.ID).ToList();
                this.currentMestRoomByRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o =>
                    o.ROOM_ID == WorkPlace.GetRoomId()
                    && (mediIds != null && mediIds.Contains(o.MEDI_STOCK_ID))
                    && (roomIds != null && roomIds.Contains(o.ROOM_ID))
                    ).ToList();

                var departmerts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o => WorkPlace.GetBranchId() == 0 || o.BRANCH_ID == WorkPlace.GetBranchId()).ToList();
                if (departmerts == null || departmerts.Count == 0)
                    throw new ArgumentNullException("departmerts is null");

                var departmentIds = departmerts.Select(o => o.ID).Distinct().ToArray();
                this.currentMestRoomByRooms = this.currentMestRoomByRooms.Where(o => departmentIds != null && departmentIds.Contains(o.DEPARTMENT_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> GetSereServ8ByTreatmentId(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServView7Filter hisSereServFilter = new HisSereServView7Filter();
                hisSereServFilter.TDL_TREATMENT_ID = this.currentTreatmentWithPatientType.ID;
                return new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_7, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<HIS_MEDICINE_BEAN> GetMedicineBeanByExpMestMedicine(List<long> expMestMedicineIds)
        {
            List<HIS_MEDICINE_BEAN> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineBeanFilter hismedicineBeanFilter = new HisMedicineBeanFilter();
                hismedicineBeanFilter.EXP_MEST_MEDICINE_IDs = expMestMedicineIds;
                result = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>(RequestUriStore.HIS_MEDICINE_BEAN__GET, ApiConsumers.MosConsumer, hismedicineBeanFilter, ProcessLostToken, param);
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

        private List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN_1> GetMedicineBeanByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineBeanView1Filter medicineBeanFilter = new HisMedicineBeanView1Filter();
                //medicineBeanFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_MEDICINE_BEAN_1>>("api/HisMedicineBean/GetView1", ApiConsumers.MosConsumer, medicineBeanFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1> GetMaterialBeanByExpMestId(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMaterialBeanView1Filter materialBeanFilter = new HisMaterialBeanView1Filter();
                //materialBeanFilter.EXP_MEST_ID = expMestId;
                return new BackendAdapter(param).Get<List<V_HIS_MATERIAL_BEAN_1>>("api/HisMaterialBean/GetView1", ApiConsumers.MosConsumer, materialBeanFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void LoadDataTracking()
        {
            try
            {
                //Init Control
                CommonParam param = new CommonParam();
                if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && !HisConfigCFG.IsServiceReqIcdOption)
                {
                    lciPhieuDieuTri.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    return;
                }

                if (GlobalStore.IsCabinet)
                {
                    //Check đối tượng nội trú hoặc ngoại trú
                    HisPatientTypeAlterViewAppliedFilter filterPatientTypeAlter = new HisPatientTypeAlterViewAppliedFilter();
                    filterPatientTypeAlter.TreatmentId = this.treatmentId;
                    filterPatientTypeAlter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filterPatientTypeAlter, param);
                    if (patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                        && patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    {
                        lciPhieuDieuTri.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        return;
                    }
                }

                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = this.treatmentId;
                List<HIS_TRACKING> trackings = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);
                if (trackings == null || trackings.Count == 0)
                    return;

                trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();

                List<long> intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                if (intructionTimeSelecteds == null)
                    return;

                List<string> intructionDateSelecteds = new List<string>();
                foreach (var item in intructionTimeSelecteds)
                {
                    string intructionDate = item.ToString().Substring(0, 8);
                    intructionDateSelecteds.Add(intructionDate);
                }

                // long dateTimeNowInt64 = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                var trackingTemps = trackings.Where(o => intructionDateSelecteds.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                    && o.DEPARTMENT_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId))
                    .OrderByDescending(o => o.TRACKING_TIME).ToList();

                bool isShowDefault = true;
                if (trackingTemps == null || trackingTemps.Count == 0)
                {
                    isShowDefault = false;
                }

                trackingADOs = new List<TrackingADO>();
                foreach (var item in trackings)
                {
                    TrackingADO tracking = new TrackingADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TrackingADO>(tracking, item);
                    tracking.TrackingTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                    trackingADOs.Add(tracking);
                }
                trackingADOs = trackingADOs.OrderByDescending(o => o.TRACKING_TIME).ToList();

                if (this.tracking != null)// && !isChangeData
                {
                    Inventec.Common.Logging.LogSystem.Debug("Ben ngoai truyen vao to dieu tri____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tracking), tracking));
                    if (this.ucDateProcessor.GetChkMultiDateState(this.ucDate) == false)
                    {
                        cboPhieuDieuTri.EditValue = this.tracking.ID;
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                        if (HisConfigCFG.IsServiceReqIcdOption)
                        {
                            if (ucIcd != null)
                            {
                                IcdInputADO icd = new IcdInputADO();
                                icd.ICD_CODE = this.tracking.ICD_CODE;
                                icd.ICD_NAME = this.tracking.ICD_NAME;
                                if (ucIcd != null)
                                {
                                    icdProcessor.Reload(ucIcd, icd);
                                }
                            }

                            if (ucIcd != null)
                            {
                                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                                subIcd.ICD_SUB_CODE = this.tracking.ICD_SUB_CODE;
                                subIcd.ICD_TEXT = this.tracking.ICD_TEXT;
                                if (ucSecondaryIcd != null)
                                {
                                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                                }
                            }
                            //yhct
                            if (ucIcdYHCT != null)
                            {
                                IcdInputADO icd = new IcdInputADO();
                                icd.ICD_CODE = this.tracking.TRADITIONAL_ICD_CODE;
                                icd.ICD_NAME = this.tracking.TRADITIONAL_ICD_NAME;
                                if (ucIcdYHCT != null)
                                {
                                    icdProcessorYHCT.Reload(ucIcdYHCT, icd);
                                }
                            }

                            if (ucSecondaryIcdYHCT != null)
                            {
                                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                                subIcd.ICD_SUB_CODE = this.tracking.TRADITIONAL_ICD_SUB_CODE;
                                subIcd.ICD_TEXT = this.tracking.TRADITIONAL_ICD_TEXT;
                                if (ucSecondaryIcdYHCT != null)
                                {
                                    subIcdProcessorYHCT.Reload(ucSecondaryIcdYHCT, subIcd);
                                }
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Info("Truong hop co tracking ben ngoai truyen vao, gan vao doi tuong tracking de phuc vu cho nghiep vu load cac du lieu: + Mã bệnh chỉnh,+ Tên bệnh chính,+ Mã bệnh phụ,+ Tên bệnh phụ,+ Mã bệnh YHCT chính,+ Tên bệnh YHCT chính,+ Mã bệnh YHCT phụ,+ Tên bệnh YHCT phụ tu ban ghi tracking do");
                    }
                }
                else if (HisConfigCFG.IsDefaultTracking && isShowDefault)
                {
                    if (this.ucDateProcessor.GetChkMultiDateState(this.ucDate) == false)
                    {
                        cboPhieuDieuTri.EditValue = trackingTemps[0].ID;
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHisTreatment()
        {
            try
            {
                this.Histreatment = new HIS_TREATMENT();
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = this.treatmentId;
                this.Histreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void GetListExpMestMedicine()
        {
            try
            {
                this.LstExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                if (HisConfigCFG.AcinInteractiveOption != "1" && HisConfigCFG.AcinInteractiveOption != "2")
                    return;
                CommonParam param = new CommonParam();
                if (HisConfigCFG.AcinInteractiveOption == "1")
                {
                    foreach (var item in this.intructionTimeSelecteds)
                    {
                        HisExpMestMedicineViewFilter ExpMestMediFilter = new HisExpMestMedicineViewFilter();
                        ExpMestMediFilter.TDL_INTRUCTION_TIME_FROM = Int64.Parse(item.ToString().Substring(0, 8) + "000000");
                        ExpMestMediFilter.TDL_INTRUCTION_TIME_TO = Int64.Parse(item.ToString().Substring(0, 8) + "235959");
                        ExpMestMediFilter.TDL_TREATMENT_ID = treatmentId;
                        var ExpMestMedis = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, ExpMestMediFilter, ProcessLostToken, param);
                        if (ExpMestMedis != null && ExpMestMedis.Count > 0)
                            this.LstExpMestMedicine.AddRange(ExpMestMedis);
                    }
                }
                else
                {
                    HisExpMestMedicineViewFilter ExpMestMediFilter = new HisExpMestMedicineViewFilter();
                    ExpMestMediFilter.USE_TIME_TO_FROM = Int64.Parse(this.intructionTimeSelecteds.OrderByDescending(o => o).Last().ToString().Substring(0, 8) + "000000");
                    ExpMestMediFilter.TDL_PATIENT_ID = currentTreatmentWithPatientType.PATIENT_ID;
                    var ExpMestMedis = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, ExpMestMediFilter, ProcessLostToken, param);
                    if (ExpMestMedis != null && ExpMestMedis.Count > 0)
                        this.LstExpMestMedicine.AddRange(ExpMestMedis);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
