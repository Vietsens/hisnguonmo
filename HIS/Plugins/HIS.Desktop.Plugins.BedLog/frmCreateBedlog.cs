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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BedLog.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.BedLog;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedLog
{
    public partial class frmCreateBedlog : HIS.Desktop.Utility.FormBase
    {
        internal HisBedLogAdo hisBedLogAdo { get; set; }
        internal V_HIS_TREATMENT_BED_ROOM hisTreatmentBedRoom { get; set; }
        internal List<V_HIS_SERE_SERV> lstSereServPttts { get; set; }
        internal List<V_HIS_SERE_SERV> DsSereServPttts { get; set; }
        internal V_HIS_SERE_SERV_PTTT hisSereServPttt { get; set; }
        internal List<HIS_DEPARTMENT> lstHisDepartments { get; set; }
        internal List<HIS_PTTT_GROUP> hisPtttGroup { get; set; }
        internal List<V_HIS_BED_SERVICE_TYPE> VHisBedServiceTypes { get; set; }
        int positionHandleControlBedInfo = -1;
        RefeshData refeshData;
        internal V_HIS_BED_LOG dataVhisBedLog { get; set; }
        internal int action = -1;
        bool is_cboBed = false;
        bool is_cboBedServiceType = false;

        public frmCreateBedlog(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            InitializeComponent();
        }

        public frmCreateBedlog(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_BED_ROOM hisTreatmentBedRoom, RefeshData refeshData)
		:base(module)
        {
            InitializeComponent();
            this.hisTreatmentBedRoom = hisTreatmentBedRoom;
            this.refeshData = refeshData;
            this.action = GlobalVariables.ActionAdd;
        }

        public frmCreateBedlog(Inventec.Desktop.Common.Modules.Module module, V_HIS_BED_LOG data, RefeshData refeshData)
		:base(module)
        {
            InitializeComponent();
            this.dataVhisBedLog = data;
            this.refeshData = refeshData;
            this.action = GlobalVariables.ActionEdit;
        }

        private void frmBedLog_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ValidateBedForm();
                dtFromTime.EditValue = DateTime.Now;
                dtToTime.EditValue = DateTime.Now;
                LoadDataBedServiceType();
                LoadDataBed();

                if (this.action == GlobalVariables.ActionEdit)
                {
                    LoadDataToComboByEdit();
                }
                else
                {
                    //Check cấu hình khoa ngoại
                    CheckCardConfigCCC();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboByEdit()
        {
            try
            {
                dtFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataVhisBedLog.START_TIME);
                if (dataVhisBedLog.FINISH_TIME != null)
                {
                    dtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)dataVhisBedLog.FINISH_TIME);
                }
                else
                {
                    dtToTime.EditValue = DateTime.Now;
                }
                if (dataVhisBedLog.BED_SERVICE_TYPE_ID != null)
                {
                    // var serviceId = VHisBedServiceTypes.SingleOrDefault(p => p.ID == dataVhisBedLog.BED_SERVICE_TYPE_ID).SERVICE_ID;
                    LoadDataToCboBedServiceType(dataVhisBedLog.BED_ID);
                    cboBedServiceType.EditValue = dataVhisBedLog.SERVICE_ID;
                    LoadDataToCboBed(dataVhisBedLog.SERVICE_ID ?? 0);
                    //List<V_HIS_BED> hisBeds = BackendDataWorker.Get<V_HIS_BED>();
                    //LoadDataCboBed(this.cboBed, hisBeds);
                    cboBed.EditValue = dataVhisBedLog.BED_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckCardConfigCCC()
        {
            try
            {
                var departmentCodes = Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>(HIS.Desktop.LocalStorage.SdaConfigKey.ExtensionConfigKey.EXE_CREATE_BED_LOG_DEPARTMENT_CODES);//cấu hình

                string pattern = ",";
                Regex myRegex = new Regex(pattern);
                string[] Codes = myRegex.Split(departmentCodes);

                var departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault().DepartmentId;
                lstHisDepartments = new List<HIS_DEPARTMENT>();
                lstHisDepartments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(p => p.ID == departmentId && Codes.Contains(p.DEPARTMENT_CODE)).ToList();
                if (lstHisDepartments != null && lstHisDepartments.Count > 0)//Kiểm tra cấu hình khoa
                {
                    KiemTraXemCoPTTT();
                    SSVsFromTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void KiemTraXemCoPTTT()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new MOS.Filter.HisServiceReqViewFilter();
                serviceReqFilter.TREATMENT_ID = hisTreatmentBedRoom.TREATMENT_ID;

                var serviceReqTypeCode = Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_SERVICE_REQ_TYPE__SERVICE_REQ_TYPE_CODE__SURG);

                var currentServiceReqs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).Where(p => p.SERVICE_REQ_TYPE_CODE == serviceReqTypeCode).ToList();
                List<long> serviceReqIds = new List<long>();
                if (currentServiceReqs != null && currentServiceReqs.Count > 0)
                {
                    serviceReqIds = currentServiceReqs.Select(p => p.ID).ToList();
                }


                MOS.Filter.HisSereServViewFilter sereServFilter = new MOS.Filter.HisSereServViewFilter();
                sereServFilter.SERVICE_REQ_IDs = serviceReqIds;
                lstSereServPttts = new List<V_HIS_SERE_SERV>();
                lstSereServPttts = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, param).Where(p => p.FINISH_TIME != null).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SSVsFromTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (lstSereServPttts != null && lstSereServPttts.Count > 0)
                {
                    DsSereServPttts = new List<V_HIS_SERE_SERV>();
                    foreach (var item in lstSereServPttts)
                    {
                        DateTime finishTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.FINISH_TIME ?? 0);
                        DateTime fromTime = (DateTime)dtFromTime.DateTime;
                        if (fromTime < finishTime)
                        {
                            MessageBox.Show("Ngày vào giường không được nhỏ hơn ngày kết thúc PTTT  " + finishTime.ToString(), "Thông báo");
                            return;
                        }
                        TimeSpan diff = fromTime - finishTime;
                        if (diff.Days <= 10)
                        {
                            DsSereServPttts.Add(item);
                        }
                    }
                }
                if (DsSereServPttts != null && DsSereServPttts.Count > 0)
                {
                    DsSereServPttts = DsSereServPttts.OrderByDescending(p => p.FINISH_TIME).ToList();
                    var SereServ = DsSereServPttts.FirstOrDefault();
                    MOS.Filter.HisSereServPtttViewFilter sSPtttFilter = new MOS.Filter.HisSereServPtttViewFilter();
                    sSPtttFilter.SERE_SERV_ID = SereServ.ID;
                    hisSereServPttt = new V_HIS_SERE_SERV_PTTT();
                    hisSereServPttt = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, sSPtttFilter, param).FirstOrDefault();
                    if (hisSereServPttt != null)
                    {
                        LoadPTTTGroup(hisSereServPttt);
                    }
                }
                else
                {
                    cboBedServiceType.EditValue = null;
                    cboBed.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPTTTGroup(V_HIS_SERE_SERV_PTTT sereServPttt)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPtttGroupFilter filter = new MOS.Filter.HisPtttGroupFilter();
                filter.ID = sereServPttt.PTTT_GROUP_ID;
                hisPtttGroup = new List<HIS_PTTT_GROUP>();
                hisPtttGroup = new BackendAdapter(param).Get<List<HIS_PTTT_GROUP>>(HisRequestUriStore.HIS_PTTT_GROUP_GET, ApiConsumers.MosConsumer, filter, param);
                if (hisPtttGroup != null && hisPtttGroup.Count > 0)
                {
                    var serviceId = VHisBedServiceTypes.SingleOrDefault(p => p.ID == hisPtttGroup[0].BED_SERVICE_TYPE_ID).SERVICE_ID;
                    cboBedServiceType.EditValue = serviceId;
                    is_cboBedServiceType = true;
                    if (this.cboBedServiceType.EditValue != null)
                    {
                        LoadDataToCboBed(serviceId);
                    }
                    else
                    {
                        LoadDataToCboBed(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event cboBedServiceType
        private void LoadDataBedServiceType()
        {
            try
            {
                CommonParam param = new CommonParam();

                //Load BedServiceType
                MOS.Filter.HisBedServiceTypeViewFilter bedServiceTypefilter = new MOS.Filter.HisBedServiceTypeViewFilter();
                VHisBedServiceTypes = new BackendAdapter(param).Get<List<V_HIS_BED_SERVICE_TYPE>>(HisRequestUriStore.HIS_BED_SERVICE_TYPE_GETVIEW, ApiConsumers.MosConsumer, bedServiceTypefilter, param);

                var serviceTypeBedCode = Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_SERVICE_TYPE__SERVICE_TYPE_CODE__BED);//cấu hình

                //Load theo servieRoom lấy ra các dịch vụ giường của room OK
                var currentServiceBedByRoom = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault().RoomId && o.SERVICE_TYPE_CODE == serviceTypeBedCode).ToList();


                FillDataCboBedServiceType(this.cboBedServiceType, currentServiceBedByRoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboBedServiceType(long bedId)
        {
            try
            {
                CommonParam param = new CommonParam();
                this.cboBedServiceType.EditValue = null;

                MOS.Filter.HisBedBstyViewFilter bedBstyFilter = new MOS.Filter.HisBedBstyViewFilter();
                bedBstyFilter.BED_ID = bedId;
                var lstBedServiceTypes = new BackendAdapter(param).Get<List<V_HIS_BED_BSTY>>(HisRequestUriStore.HIS_BED_BSTY_GETVIEW, ApiConsumers.MosConsumer, bedBstyFilter, param);
                List<long> bedServiceTypeIds = new List<long>();
                if (lstBedServiceTypes != null && lstBedServiceTypes.Count > 0)
                {
                    bedServiceTypeIds = lstBedServiceTypes.Select(p => p.BED_SERVICE_TYPE_ID).ToList();
                }

                var lstBedServiceTypeByBedId = VHisBedServiceTypes.Where(p => bedServiceTypeIds.Contains(p.ID)).ToList();
                List<long> serviceIds = new List<long>();
                if (lstBedServiceTypeByBedId != null && lstBedServiceTypeByBedId.Count > 0)
                {
                    serviceIds = lstBedServiceTypeByBedId.Select(p => p.SERVICE_ID).ToList();
                }
                var currentServiceTypeByBeds = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(p => serviceIds.Contains(p.SERVICE_ID)).ToList();

                FillDataCboBedServiceType(this.cboBedServiceType, currentServiceTypeByBeds);
                if (currentServiceTypeByBeds != null && currentServiceTypeByBeds.Count > 0)
                {
                    this.cboBedServiceType.EditValue = currentServiceTypeByBeds[0].SERVICE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataCboBedServiceType(DevExpress.XtraEditors.LookUpEdit cboBedServiceType, object data)
        {
            try
            {
                cboBedServiceType.Properties.DataSource = data;
                cboBedServiceType.Properties.DisplayMember = "SERVICE_NAME";
                cboBedServiceType.Properties.ValueMember = "SERVICE_ID";
                cboBedServiceType.Properties.ForceInitialize();
                cboBedServiceType.Properties.Columns.Clear();
                cboBedServiceType.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_CODE", "", 50));
                cboBedServiceType.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_NAME", "", 250));
                cboBedServiceType.Properties.ShowHeader = false;
                cboBedServiceType.Properties.ImmediatePopup = true;
                cboBedServiceType.Properties.DropDownRows = 10;
                cboBedServiceType.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedServiceType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBedServiceType.EditValue != null && cboBed.EditValue == null)
                    {
                        long ServiceId = (long)cboBedServiceType.EditValue;
                        LoadDataToCboBed(ServiceId);
                        cboBed.Focus();
                        is_cboBedServiceType = true;
                    }
                    else if (is_cboBedServiceType)
                    {
                        cboBed.EditValue = null;
                        long ServiceId = (long)cboBedServiceType.EditValue;
                        LoadDataToCboBed(ServiceId);
                        cboBed.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBed.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event cboBed
        private void LoadDataBed()
        {
            try
            {
                var currentBeds = BackendDataWorker.Get<V_HIS_BED>().Where(o => o.BED_ROOM_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault().BedRoomId).ToList();
                LoadDataCboBed(this.cboBed, currentBeds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboBed(long serviceId)
        {
            try
            {
                CommonParam param = new CommonParam();
                this.cboBed.EditValue = null;

                List<V_HIS_BED> hisBeds = BackendDataWorker.Get<V_HIS_BED>();

                MOS.Filter.HisBedBstyViewFilter bedBstyfilter = new MOS.Filter.HisBedBstyViewFilter();
                List<V_HIS_BED_BSTY> hisBedBstys = new BackendAdapter(param).Get<List<V_HIS_BED_BSTY>>(HisRequestUriStore.HIS_BED_BSTY_GETVIEW, ApiConsumers.MosConsumer, bedBstyfilter, param);
                List<long> lstBedIds = new List<long>();
                if (serviceId != 0)
                {
                    lstBedIds = hisBedBstys.Where(p => p.SERVICE_ID == serviceId).Select(p => p.BED_ID).ToList();
                }
                var lstBeds = hisBeds.Where(p => lstBedIds.Contains(p.ID)).ToList();
                if (lstBeds != null && lstBeds.Count > 0)
                {
                    LoadDataCboBed(this.cboBed, lstBeds);
                    this.cboBed.EditValue = lstBeds[0].ID;
                    // txtBedName.Text = cboBed.EditValue.ToString();
                }
                else
                {
                    LoadDataCboBed(this.cboBed, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboBed(DevExpress.XtraEditors.LookUpEdit cboBedName, object data)
        {
            try
            {
                cboBedName.Properties.DataSource = data;
                cboBedName.Properties.DisplayMember = "BED_NAME";
                cboBedName.Properties.ValueMember = "ID";
                cboBedName.Properties.ForceInitialize();
                cboBedName.Properties.Columns.Clear();
                cboBedName.Properties.Columns.Add(new LookUpColumnInfo("BED_NAME", "", 200));
                cboBedName.Properties.ShowHeader = false;
                cboBedName.Properties.ImmediatePopup = true;
                cboBedName.Properties.DropDownRows = 10;
                cboBedName.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBed_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBed.EditValue != null && cboBedServiceType.EditValue == null)
                    {
                        long bedId = (long)cboBed.EditValue;
                        LoadDataToCboBedServiceType(bedId);
                        cboBedServiceType.Focus();
                        is_cboBed = true;
                    }
                    else if (is_cboBed)
                    {
                        cboBedServiceType.EditValue = null;
                        long bedId = (long)cboBed.EditValue;
                        LoadDataToCboBedServiceType(bedId);
                        cboBedServiceType.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBedServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Validation Bed
        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlBedInfo == -1)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlBedInfo > edit.TabIndex)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateBedForm()
        {

            ValidateLookupWithTextEdit(cboBed);
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                //validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cbo, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event DateTime
        private void dtFromTime_MouseLeave(object sender, EventArgs e)
        {
            // SSVsFromTime();
        }

        private void dtToTime_CloseUp(object sender, CloseUpEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBed.Text != null)
                    {
                        btnSave_Create.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtFromTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstHisDepartments != null && lstHisDepartments.Count > 0)
                {
                    SSVsFromTime();
                    //MessageBox.Show("Thời gian thay đổi");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void dtFromTime_EditValueChanging(object sender, ChangingEventArgs e)
        {
            //if (lstHisDepartments != null && lstHisDepartments.Count > 0)
            //{
            //    SSVsFromTime();
            //}
        }

        private void dtFromTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (lstHisDepartments != null && lstHisDepartments.Count > 0)
                    {
                        SSVsFromTime();
                    }
                    cboBedServiceType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event Save
        private void btnSave_Create_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControlBedInfo = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_BED_LOG inPut = new MOS.EFMODEL.DataModels.HIS_BED_LOG();
                HIS_BED_LOG outPut = new HIS_BED_LOG();
                //Ktra thời gian kết thúc
                DateTime finishTime = (DateTime)dtToTime.DateTime;
                DateTime fromTime = (DateTime)dtFromTime.DateTime;
                if (finishTime < fromTime)
                {
                    MessageManager.Show("Ngày kết thúc giường không được nhỏ hơn ngày bắt đầu vào  " + fromTime.ToString());
                    return;
                }
                if (this.action == GlobalVariables.ActionAdd)
                {
                    inPut.TREATMENT_BED_ROOM_ID = hisTreatmentBedRoom.ID;
                    inPut.BED_ID = (long)cboBed.EditValue;
                    inPut.BED_SERVICE_TYPE_ID = VHisBedServiceTypes.SingleOrDefault(p => p.SERVICE_ID == (long)cboBedServiceType.EditValue).ID;
                    inPut.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime) ?? 0;
                    inPut.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime) ?? 0;
                    outPut = new BackendAdapter(param).Post<HIS_BED_LOG>(HisRequestUriStore.HIS_BED_LOG_CREATE, ApiConsumers.MosConsumer, inPut, param);
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_BED_LOG, MOS.EFMODEL.DataModels.HIS_BED_LOG>();
                    inPut = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_BED_LOG, MOS.EFMODEL.DataModels.HIS_BED_LOG>(dataVhisBedLog);
                    inPut.BED_ID = (long)cboBed.EditValue;
                    inPut.BED_SERVICE_TYPE_ID = VHisBedServiceTypes.SingleOrDefault(p => p.SERVICE_ID == (long)cboBedServiceType.EditValue).ID;
                    inPut.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime) ?? 0;
                    inPut.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime) ?? 0;
                    outPut = new BackendAdapter(param).Post<HIS_BED_LOG>(HisRequestUriStore.HIS_BED_LOG_UPDATE, ApiConsumers.MosConsumer, inPut, param);
                }
                // string messenger = "";
                if (outPut != null)
                {
                    success = true;
                    // messenger = "Xử lý thành công" + "\r\n";
                    this.refeshData();
                    this.Close();
                }
                #region Show message
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Create_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event Refesh
        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                cboBed.EditValue = null;
                cboBedServiceType.EditValue = null;
                //dtFromTime.EditValue = DateTime.Now;
                dtToTime.EditValue = DateTime.Now;
                LoadDataBedServiceType();
                LoadDataBed();
                is_cboBed = false;
                is_cboBedServiceType = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI_Refesh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefesh_Click(null, null);
        }
        #endregion
    }
}
