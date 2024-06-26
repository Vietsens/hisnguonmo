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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedAssign
{
    public partial class FormBedAssign : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        System.Globalization.CultureInfo cultureLang;
        MOS.SDO.WorkPlaceSDO _WorkPlaceSDO;
        int action = -1;
        int positionHandle = -1;
        bool is_cboBed = false;
        int _IsChoose = 0;
        bool is_cboBedServiceType = false;
        V_HIS_TREATMENT_BED_ROOM hisTreatmentBedRoom { get; set; }
        V_HIS_BED_LOG dataVhisBedLog { get; set; }
        internal List<HIS_DEPARTMENT> _Departments { get; set; }
        internal List<V_HIS_SERVICE> VHisBedServiceTypes { get; set; }
        internal List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        internal List<V_HIS_BED> _vHisBeds { get; set; }
        internal List<HisBedADO> dataBedADOs { get; set; }
        internal List<V_HIS_SERVICE_ROOM> _ServiceBedByRooms { get; set; }
        #endregion

        #region Construct
        public FormBedAssign(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                _WorkPlaceSDO = new MOS.SDO.WorkPlaceSDO();
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormBedAssign(V_HIS_TREATMENT_BED_ROOM data, Inventec.Desktop.Common.Modules.Module module)
            : this(module)
        {
            try
            {
                this.action = GlobalVariables.ActionAdd;
                this.hisTreatmentBedRoom = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormBedAssign(V_HIS_BED_LOG data, Inventec.Desktop.Common.Modules.Module module)
            : this(module)
        {
            try
            {
                this.action = GlobalVariables.ActionEdit;
                this.dataVhisBedLog = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormBedAssign_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                    this._WorkPlaceSDO = this.currentModule.RoomId > 0 ? LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId) : LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault();
                }

                this._vHisBeds = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED>().Where(o => o.BED_ROOM_ID == _WorkPlaceSDO.BedRoomId && o.IS_ACTIVE == 1).ToList();

                LoadKeysFromlanguage();

                SetDefaultValueControl();

                FillDataToControl();

                ValidateBedForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormBedAssign,
                    cultureLang);
                this.lciBed.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__LCI_BED",
                    Resources.ResourceLanguageManager.LanguageFormBedAssign,
                    cultureLang);
                this.lciBedServiceType.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__LCI_BED_SERVICE",
                    Resources.ResourceLanguageManager.LanguageFormBedAssign,
                    cultureLang);
                this.lciFromTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__LCI_FROM_TIME",
                    Resources.ResourceLanguageManager.LanguageFormBedAssign,
                    cultureLang);
                this.lciToTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__LCI_TO_TIME",
                    Resources.ResourceLanguageManager.LanguageFormBedAssign,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                dtFromTime.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                dtToTime.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();

                //LoadDataBed();

                LoadDataBedServiceType();

                if (this.action == GlobalVariables.ActionEdit && dataVhisBedLog != null)
                {
                    LoadDataToComboByEdit();
                }
                else
                {
                    CheckCardConfigCCC();  //Check cấu hình khoa ngoại
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Load du lieu cac giuong theo phong lam viec
        /// </summary>
        private void LoadDataBed()
        {
            try
            {
                LoadDataCboBed(this.cboBed, this._vHisBeds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboBed(DevExpress.XtraEditors.GridLookUpEdit cboBedName, List<V_HIS_BED> datas)
        {
            try
            {
                dataBedADOs = new List<HisBedADO>();
                if (datas != null && datas.Count > 0)
                {
                    dataBedADOs.AddRange((from r in datas select new HisBedADO(r)).ToList());

                    List<long> bedIds = datas.Select(p => p.ID).Distinct().ToList();
                    MOS.Filter.HisBedLogFilter filter = new MOS.Filter.HisBedLogFilter();
                    filter.BED_IDs = bedIds;
                    if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                    {
                        filter.START_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime) ?? 0;
                        filter.FINISH_TIME_FROM__OR__NULL = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime) ?? 0;
                    }
                    CommonParam param = new CommonParam();
                    var dataBedLogs = new BackendAdapter(param).Get<List<HIS_BED_LOG>>("api/HisBedLog/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dataBedLogs != null && dataBedLogs.Count > 0)
                    {
                        var dataBedLogGroups = dataBedLogs.GroupBy(p => p.BED_ID).Select(p => p.ToList()).ToList();
                        foreach (var itemADO in dataBedADOs)
                        {
                            var dataByBedLogs = dataBedLogs.Where(p => p.BED_ID == itemADO.ID).ToList();
                            if (dataByBedLogs != null && dataByBedLogs.Count > 0)
                            {
                                if (itemADO.MAX_CAPACITY.HasValue)
                                {
                                    if (dataByBedLogs.Count >= itemADO.MAX_CAPACITY)
                                        itemADO.IsKey = 2;
                                    else
                                        itemADO.IsKey = 1;
                                }
                                itemADO.AMOUNT = dataByBedLogs.Count + "/" + itemADO.MAX_CAPACITY;
                            }
                        }
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("BED_NAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("AMOUNT", "", 50, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBedName, dataBedADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBedServiceType()
        {
            try
            {
                CommonParam param = new CommonParam();

                //Load BedServiceType MOS.Filter.HisBedServiceTypeViewFilter
                MOS.Filter.HisServiceViewFilter _serviceViewFilter = new MOS.Filter.HisServiceViewFilter();
                if (this._WorkPlaceSDO != null)
                {
                    _serviceViewFilter.DATA_DOMAIN_FILTER = true;
                    _serviceViewFilter.WORKING_ROOM_ID = _WorkPlaceSDO.RoomId;
                    _serviceViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                }
                _serviceViewFilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G;
                this.VHisBedServiceTypes = new BackendAdapter(param).Get<List<V_HIS_SERVICE>>(Base.GlobalStore.HIS_SERVICE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, _serviceViewFilter, param);

                //Load theo servieRoom lấy ra các dịch vụ giường của room OK
                this._ServiceBedByRooms = new List<V_HIS_SERVICE_ROOM>();
                _ServiceBedByRooms = Base.GlobalStore.listServiceRoom.Where(o =>
                    o.ROOM_ID == _WorkPlaceSDO.RoomId
                    && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                    && o.IS_ACTIVE == 1).ToList();

                if (_ServiceBedByRooms != null
                    && _ServiceBedByRooms.Count > 0
                    && this.VHisBedServiceTypes != null
                    && this.VHisBedServiceTypes.Count > 0)
                {
                    _ServiceBedByRooms = _ServiceBedByRooms.Where(p => this.VHisBedServiceTypes.Select(o => o.ID).ToList().Contains(p.SERVICE_ID)).ToList();
                    _ServiceBedByRooms = _ServiceBedByRooms.OrderBy(p => p.SERVICE_CODE).ToList();
                }

                FillDataCboBedServiceType(this.cboBedServiceType, _ServiceBedByRooms);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboByEdit()
        {
            try
            {
                dtFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.dataVhisBedLog.START_TIME);
                if (dataVhisBedLog.FINISH_TIME != null)
                {
                    dtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64((dataVhisBedLog.FINISH_TIME ?? 0).ToString()));
                }
                else
                {
                    dtToTime.EditValue = DateTime.Now;
                }
                if (dataVhisBedLog.BED_SERVICE_TYPE_ID != null)
                {
                    LoadDataToCboBedServiceType(dataVhisBedLog.BED_ID);
                    cboBedServiceType.EditValue = dataVhisBedLog.BED_SERVICE_TYPE_ID ?? 0;

                    RefeshDataToCboBed(dataVhisBedLog.BED_SERVICE_TYPE_ID ?? 0);
                }
                cboBed.EditValue = dataVhisBedLog.BED_ID;
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
                bedBstyFilter.ROOM_ID = this._WorkPlaceSDO.RoomId;
                bedBstyFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var lstBedServiceTypes = new BackendAdapter(param).Get<List<V_HIS_BED_BSTY>>(Base.GlobalStore.HIS_BED_BSTY_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bedBstyFilter, param);
                List<long> bedServiceTypeIds = new List<long>();
                if (lstBedServiceTypes != null && lstBedServiceTypes.Count > 0)
                {
                    bedServiceTypeIds = lstBedServiceTypes.Select(p => p.BED_SERVICE_TYPE_ID).ToList();
                }

                var lstBedServiceTypeByBedId = VHisBedServiceTypes.Where(p => bedServiceTypeIds.Contains(p.ID)).ToList();
                List<long> serviceIds = new List<long>();
                if (lstBedServiceTypeByBedId != null && lstBedServiceTypeByBedId.Count > 0)
                {
                    serviceIds = lstBedServiceTypeByBedId.Select(p => p.ID).ToList();
                }
                var currentServiceTypeByBeds = Base.GlobalStore.listServiceRoom.Where(p =>
                    serviceIds.Contains(p.SERVICE_ID)
                    && p.ROOM_ID == this._WorkPlaceSDO.RoomId
                    && p.IS_ACTIVE == 1)
                    .ToList();
                if (currentServiceTypeByBeds != null && currentServiceTypeByBeds.Count > 0)
                {
                    currentServiceTypeByBeds = currentServiceTypeByBeds.OrderBy(p => p.SERVICE_CODE).ToList();
                }
                FillDataCboBedServiceType(this.cboBedServiceType, currentServiceTypeByBeds);
                if (currentServiceTypeByBeds != null && currentServiceTypeByBeds.Count > 0)
                {
                    this.cboBedServiceType.EditValue = currentServiceTypeByBeds[0].SERVICE_ID;
                    cboBedServiceType.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load lai data cbo Bed
        /// </summary>
        /// <param name="serviceId"></param>
        private void RefeshDataToCboBed(long serviceId)
        {
            try
            {
                CommonParam param = new CommonParam();
                this.cboBed.EditValue = null;

                List<V_HIS_BED> hisBeds = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED>();

                MOS.Filter.HisBedBstyViewFilter bedBstyfilter = new MOS.Filter.HisBedBstyViewFilter();
                //bedBstyfilter.ROOM_ID = this.WorkPlaceSDO.RoomId;
                List<V_HIS_BED_BSTY> hisBedBstys = new BackendAdapter(param).Get<List<V_HIS_BED_BSTY>>(Base.GlobalStore.HIS_BED_BSTY_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bedBstyfilter, param);
                List<long> lstBedIds = new List<long>();
                if (serviceId != 0)
                {
                    lstBedIds = hisBedBstys.Where(p => p.BED_SERVICE_TYPE_ID == serviceId).Select(p => p.BED_ID).ToList();
                }
                var lstBeds = hisBeds.Where(p => lstBedIds.Contains(p.ID)).ToList();
                if (lstBeds != null && lstBeds.Count > 0)
                {
                    LoadDataCboBed(this.cboBed, lstBeds);
                    this.cboBed.EditValue = lstBeds[0].ID;
                    cboBed.Properties.Buttons[1].Visible = true;
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


        /// <summary>
        /// Kiem tra cau hinh PTTT
        /// </summary>
        private void CheckCardConfigCCC()
        {
            try
            {
                var departmentCodes = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.EXE_CREATE_BED_LOG_DEPARTMENT_CODES);//cấu hình

                string pattern = ",";
                System.Text.RegularExpressions.Regex myRegex = new System.Text.RegularExpressions.Regex(pattern);
                string[] Codes = myRegex.Split(departmentCodes);

                long departmentId = 0;
                if (this._WorkPlaceSDO != null)
                {
                    departmentId = this._WorkPlaceSDO.DepartmentId;
                }

                _Departments = new List<HIS_DEPARTMENT>();
                _Departments = LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.ID == departmentId && Codes.Contains(p.DEPARTMENT_CODE)).ToList();
                if (_Departments != null && _Departments.Count > 0)//Kiểm tra cấu hình khoa
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
                this._ServiceReqs = new List<HIS_SERVICE_REQ>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = hisTreatmentBedRoom.TREATMENT_ID;
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT;
                serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this._ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param).Where(p => p.FINISH_TIME != null).ToList();
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
                if (this._ServiceReqs != null && this._ServiceReqs.Count > 0)
                {
                    MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                    sereServFilter.SERVICE_REQ_IDs = _ServiceReqs.Select(p => p.ID).ToList();
                    var _SereServPttts = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(Base.GlobalStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, param).ToList();

                    List<HIS_SERE_SERV> DsSereServPttts = new List<HIS_SERE_SERV>();

                    foreach (var item in _ServiceReqs)
                    {
                        DateTime finishTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.FINISH_TIME ?? 0);
                        // dtFromTime.DateTime = finishTime;
                        DateTime fromTime = (DateTime)dtFromTime.DateTime;
                        if (fromTime < finishTime)
                        {
                            MessageBox.Show(ResourceMessage.ERROR_FINISH_TIME_PTTT + finishTime.ToString(),
                                ResourceMessage.ThongBao);
                            return;
                        }
                        TimeSpan diff = fromTime - finishTime;
                        if (diff.Days <= 10)
                        {
                            DsSereServPttts.Add(_SereServPttts.FirstOrDefault(p => p.SERVICE_REQ_ID == item.ID));
                        }
                    }
                    if (DsSereServPttts != null && DsSereServPttts.Count > 0)
                    {
                        // MessageBox.Show("Check lai");
                        var SereServ = DsSereServPttts.FirstOrDefault();
                        MOS.Filter.HisSereServPtttViewFilter sSPtttFilter = new MOS.Filter.HisSereServPtttViewFilter();
                        sSPtttFilter.SERE_SERV_ID = SereServ.ID;
                        V_HIS_SERE_SERV_PTTT hisSereServPttt = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>(Base.GlobalStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, sSPtttFilter, param).FirstOrDefault();
                        if (hisSereServPttt != null)
                        {
                            LoadPTTTGroup(hisSereServPttt);
                        }
                    }
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
                List<HIS_PTTT_GROUP> _PtttGroups = new BackendAdapter(param).Get<List<HIS_PTTT_GROUP>>(Base.GlobalStore.HIS_PTTT_GROUP_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (_PtttGroups != null && _PtttGroups.Count > 0)
                {
                    var serviceId = this.VHisBedServiceTypes.SingleOrDefault(p => p.ID == _PtttGroups[0].BED_SERVICE_TYPE_ID).ID;
                    cboBedServiceType.EditValue = serviceId;
                    is_cboBedServiceType = true;
                    if (this.cboBedServiceType.EditValue != null)
                    {
                        RefeshDataToCboBed(serviceId);
                    }
                    else
                    {
                        RefeshDataToCboBed(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validation
        private void ValidateBedForm()
        {
            try
            {
                Validation.CboValidationRule validRule = new Validation.CboValidationRule();
                validRule.cbo = this.cboBed;
                validRule.ErrorText = ResourceMessage.ThieuTruongDuLieuBatBuoc;
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboBed, validRule);

                Validation.DateFromValidationRule dateValidRule = new Validation.DateFromValidationRule();
                dateValidRule.dt = this.dtFromTime;
                dateValidRule.ErrorText = ResourceMessage.BatBuocNhapThoiGianDeLocGiuong;
                dateValidRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.dtFromTime, dateValidRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                try
                {
                    DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                    if (edit == null)
                        return;

                    DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                    if (viewInfo == null)
                        return;

                    if (positionHandle == -1)
                    {
                        positionHandle = edit.TabIndex;
                        if (edit.Visible)
                        {
                            edit.SelectAll();
                            edit.Focus();
                        }
                    }
                    if (positionHandle > edit.TabIndex)
                    {
                        positionHandle = edit.TabIndex;
                        if (edit.Visible)
                        {
                            edit.SelectAll();
                            edit.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event enter
        private void cboBed_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboBed.EditValue != null)
                    {
                        long bedId = (long)cboBed.EditValue;
                        var dataBed = this.dataBedADOs.FirstOrDefault(p => p.ID == bedId);
                        if (dataBed != null)
                        {
                            if (dataBed.IsKey == 1)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.GiuongDaCoBenhNhanNam, dataBed.BED_NAME + " - " + dataBed.AMOUNT), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    cboBed.EditValue = null;
                                    cboBed.ShowPopup();
                                    return;
                                }
                            }
                            else if (dataBed.IsKey == 2)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.GiuongVuotQuaSoLuong, dataBed.BED_NAME + " - " + dataBed.AMOUNT), ResourceMessage.ThongBao);
                                cboBed.EditValue = null;
                                cboBed.ShowPopup();
                                return;
                            }
                        }
                    }
                    if (cboBed.EditValue != null && _IsChoose == 0)
                    {
                        long bedId = (long)cboBed.EditValue;
                        LoadDataToCboBedServiceType(bedId);
                        cboBedServiceType.Focus();
                        _IsChoose = 1;
                    }
                    else if (cboBed.EditValue != null && _IsChoose == 1)
                    {
                        long bedId = (long)cboBed.EditValue;
                        LoadDataToCboBedServiceType(bedId);
                        cboBedServiceType.Focus();
                    }
                    if (cboBed.EditValue != null)
                    {
                        cboBed.Properties.Buttons[1].Visible = true;
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
                    cboBedServiceType.Focus();
                    cboBedServiceType.ShowPopup();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboBed.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBed.EditValue = null;
                    cboBed.Properties.Buttons[1].Visible = false;
                    cboBed.EditValue = null;
                    cboBedServiceType.EditValue = null;
                    if (_IsChoose == 2)
                        LoadDataBed();
                    else if (_IsChoose == 1)
                        //LoadDataBedServiceType();
                        FillDataCboBedServiceType(this.cboBedServiceType, _ServiceBedByRooms);
                    _IsChoose = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedServiceType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboBedServiceType.EditValue != null && _IsChoose == 0)
                    {
                        cboBed.EditValue = null;
                        long ServiceId = (long)cboBedServiceType.EditValue;
                        RefeshDataToCboBed(ServiceId);
                        cboBed.Focus();
                        _IsChoose = 2;
                    }
                    else if (cboBedServiceType.EditValue != null && _IsChoose == 2)
                    {
                        cboBed.EditValue = null;
                        long ServiceId = (long)cboBedServiceType.EditValue;
                        RefeshDataToCboBed(ServiceId);
                        cboBed.Focus();
                    }

                    //if (cboBedServiceType.EditValue != null && cboBed.EditValue == null)
                    //{
                    //    long ServiceId = (long)cboBedServiceType.EditValue;
                    //    RefeshDataToCboBed(ServiceId);
                    //    cboBed.Focus();
                    //    is_cboBedServiceType = true;
                    //}
                    //else if (is_cboBedServiceType)
                    //{
                    //    cboBed.EditValue = null;
                    //    long ServiceId = (long)cboBedServiceType.EditValue;
                    //    RefeshDataToCboBed(ServiceId);
                    //    cboBed.Focus();
                    //}
                    if (cboBedServiceType.EditValue != null)
                    {
                        cboBedServiceType.Properties.Buttons[1].Visible = true;
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
                    dtFromTime.Focus();
                    dtFromTime.ShowPopup();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboBedServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedServiceType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBedServiceType.EditValue = null;
                    cboBedServiceType.Properties.Buttons[1].Visible = false;
                    cboBed.EditValue = null;
                    cboBedServiceType.EditValue = null;
                    if (_IsChoose == 2)
                        LoadDataBed();
                    else if (_IsChoose == 1)
                        FillDataCboBedServiceType(this.cboBedServiceType, _ServiceBedByRooms);// LoadDataBedServiceType();
                    _IsChoose = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtFromTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtFromTime.EditValue != null)
                    {

                        if (_Departments != null && _Departments.Count > 0)
                        {
                            SSVsFromTime();
                        }
                        dtToTime.Focus();
                        dtToTime.ShowPopup();
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
                dxValidationProvider1.SetValidationRule(this.cboBed, null);
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    LoadDataCboBed(this.cboBed, null);
                    FillDataCboBedServiceType(this.cboBedServiceType, null);
                    return;
                }
                else
                {
                    Validation.CboValidationRule validRule = new Validation.CboValidationRule();
                    validRule.cbo = this.cboBed;
                    validRule.ErrorText = ResourceMessage.ThieuTruongDuLieuBatBuoc;
                    validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(this.cboBed, validRule);

                    LoadDataCboBed(this.cboBed, this._vHisBeds);
                    FillDataCboBedServiceType(this.cboBedServiceType, _ServiceBedByRooms);
                }
                if (_Departments != null && _Departments.Count > 0)
                {
                    SSVsFromTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboBed.Text != null)
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event save
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                bool success = false;
                CommonParam param = new CommonParam();
                HIS_BED_LOG inPut = new HIS_BED_LOG();
                HIS_BED_LOG outPut = new HIS_BED_LOG();
                //Ktra thời gian kết thúc
                DateTime finishTime = (DateTime)dtToTime.DateTime;
                DateTime fromTime = (DateTime)dtFromTime.DateTime;
                if (finishTime != DateTime.MinValue && finishTime < fromTime)
                {
                    MessageManager.Show(ResourceMessage.ERROR_FROM_TO_TIME + fromTime.ToString());
                    return;
                }

                if (this.action == GlobalVariables.ActionAdd)
                {
                    inPut.TREATMENT_BED_ROOM_ID = hisTreatmentBedRoom.ID;

                    SaveData(ref inPut);

                    outPut = new BackendAdapter(param).Post<HIS_BED_LOG>(Base.GlobalStore.HIS_BED_LOG_CREATE, ApiConsumer.ApiConsumers.MosConsumer, inPut, param);
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_BED_LOG, HIS_BED_LOG>();
                    inPut = AutoMapper.Mapper.Map<V_HIS_BED_LOG, HIS_BED_LOG>(dataVhisBedLog);
                    SaveData(ref inPut);

                    outPut = new BackendAdapter(param).Post<HIS_BED_LOG>(Base.GlobalStore.HIS_BED_LOG_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, inPut, param);
                }
                if (outPut != null)
                {
                    success = true;
                    this.Close();
                }

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveData(ref HIS_BED_LOG inPut)
        {
            try
            {
                inPut.BED_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBed.EditValue.ToString());
                if (cboBedServiceType.EditValue != null)
                {
                    inPut.BED_SERVICE_TYPE_ID = VHisBedServiceTypes.SingleOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBedServiceType.EditValue.ToString())).ID;

                }
                else inPut.BED_SERVICE_TYPE_ID = null;
                inPut.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime) ?? 0;
                if (dtToTime.DateTime != DateTime.MinValue)
                {
                    inPut.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime) ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEditView_Bed_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    long IsKey = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IsKey") ?? "0").ToString());
                    //int v1 = Convert.ToInt32(View.GetRowCellValue(e.RowHandle, View.Columns[0]));
                    //int v2 = Convert.ToInt32(View.GetRowCellValue(e.RowHandle, View.Columns[1]));
                    //if (v1 > v2)
                    if (IsKey == 1)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (IsKey == 2)
                    {
                        e.Appearance.ForeColor = Color.Red;
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
