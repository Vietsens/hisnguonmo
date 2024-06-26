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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.UpdateExamServiceReq.Base;
using HIS.Desktop.Plugins.UpdateExamServiceReq.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.UpdateExamServiceReq
{
    public partial class frmUpdateExamServiceReq : HIS.Desktop.Utility.FormBase
    {
        internal long treatmentId;
        internal List<V_HIS_SERE_SERV_6> sereServExams { get; set; }
        int positionHandleControlLeft = -1;
        internal HIS_SERVICE_REQ serviceReq { get; set; }
        List<V_HIS_SERVICE_ROOM> serviceRooms { get; set; }
        internal HIS_TREATMENT treatment { get; set; }
        internal List<HIS_PATIENT_TYPE> PatientTypes { get; set; }
        internal List<HIS_PATIENT_TYPE> PatientTypes_cboPrimaryPatientTypes { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter { get; set; }
        internal List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter { get; set; }
        internal List<HIS_SERE_SERV_BILL> SereServBills { get; set; }
        internal List<HIS_SERE_SERV_DEPOSIT> SereServDeposits { get; set; }
        internal ModuleExecuteType.TYPE ModuleExeType { get; set; }
        internal HisServiceReqResultSDO hisServiceReqResultSDO { get; set; }
        long roomId;
        long roomTypeId;
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isSave = true;

        bool _isAllowEnableCboPrimaryPatientTypes = true;

        public frmUpdateExamServiceReq()
        {
            InitializeComponent();
            HisConfigCFG.LoadConfig();
            SetCaptionByLanguageKey();
        }

        public frmUpdateExamServiceReq(Inventec.Desktop.Common.Modules.Module moduledata)
            : base(moduledata)
        {
            InitializeComponent();
            try
            {
                this.roomId = moduledata.RoomId;
                this.roomTypeId = moduledata.RoomTypeId;
                HisConfigCFG.LoadConfig();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmUpdateExamServiceReq(Inventec.Desktop.Common.Modules.Module moduledata, long servciceReqId, bool isExecuteRoom)
            : base(moduledata)
        {
            InitializeComponent();
            try
            {
                this.roomId = moduledata.RoomId;
                this.roomTypeId = moduledata.RoomTypeId;
                HisConfigCFG.LoadConfig();
                this.serviceReq = this.GetViewServiceReqFromId(servciceReqId);
                if (servciceReqId > 0)
                {

                    ModuleExeType = ModuleExecuteType.TYPE.FROM_MODULE_OTHER;
                    if (isExecuteRoom)
                        this.ModuleExeType = ModuleExecuteType.TYPE.FROM_EXECUTE_ROOM;
                }
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_SERVICE_REQ GetViewServiceReqFromId(long serviceReqId)
        {
            HIS_SERVICE_REQ result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ID = serviceReqId;
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                var apiResult = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    result = apiResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.UpdateExamServiceReq.Resources.Lang", typeof(HIS.Desktop.Plugins.UpdateExamServiceReq.frmUpdateExamServiceReq).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrioritize.Properties.Caption = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.chkPrioritize.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCopyExamOldContent.Properties.Caption = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.chkCopyExamOldContent.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamServiceReq.Properties.NullText = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.cboExamServiceReq.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.cboRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmUpdateExamServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception examServiceReq)
            {
                Inventec.Common.Logging.LogSystem.Error(examServiceReq);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToConTrol();
                    //DateEditValidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                ResetDataToDefault(false);
                FillDataToConTrol();
                //DateEditValidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmUpdateExamServiceReq_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                this.InitControlState();
                LoadDataFromServiceReq(this.serviceReq, false);
                LoadDataToControl();
                InitValidate();
                InitEnableControl();
                //if (sereServExams != null && sereServExams.Count > 0)
                //{
                //    cboPrimaryPatientType.EditValue = sereServExams[0].PRIMARY_PATIENT_TYPE_ID;
                //}
                //else
                //{
                //    cboPrimaryPatientType.EditValue = null;
                //}

                ListHisSerVice_ = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1).ToList();



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                LoadExamRoom(strValue, false);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            LoadComboServiceRoom(data.ROOM_ID);
                            txtRoomCode.Text = data.EXECUTE_ROOM_CODE;
                            txtServiceCode.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (!isSave || !btnSave.Enabled || !dxValidationProvider1.Validate())
                    return;
                long time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0;
                if (treatment != null && time < treatment.IN_TIME)
                {
                    MessageBox.Show(string.Format("Không cho phép nhập thời gian yêu cầu nhỏ hơn thời gian vào viện {0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(treatment.IN_TIME)), "Thông báo");
                    dtInstructionTime.Focus();
                    return;
                }

                List<long> serviceIds = new List<long> { Inventec.Common.TypeConvert.Parse.ToInt64(cboExamServiceReq.EditValue.ToString()) };
                List<HIS_SERE_SERV> sereServWithMinDurations = this.GetSereServWithMinDuration(serviceReq.TDL_PATIENT_ID, serviceIds);
                if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
                {
                    string sereServMinDurationStr = "";
                    foreach (var item in sereServWithMinDurations)
                    {
                        sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                    }

                    if (MessageBox.Show(string.Format("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép: {0} .Bạn có muốn tiếp tục?", sereServMinDurationStr), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }

                // #9680
                List<long> _RoomIds = new List<long> { Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString()) };
                string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
                var PATIENT_TYPE_CODE = HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                var PATIENT_TYPE_ID = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == PATIENT_TYPE_CODE);
                string _configKey = HisConfigs.Get<string>("HIS.Desktop.WarningOverExamBhyt");
                if (_RoomIds != null && _RoomIds.Count > 0 && _configKey == "1"
                    && Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()) == PATIENT_TYPE_ID.ID
                && this.serviceReq.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    HisSereServBhytOutpatientExamFilter _HisSereServBhytOutpatientExamFilter = new HisSereServBhytOutpatientExamFilter();
                    _HisSereServBhytOutpatientExamFilter.ROOM_IDs = _RoomIds;
                    long _intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0;
                    _HisSereServBhytOutpatientExamFilter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(_intructionTime.ToString().Substring(0, 8) + "000000");
                    var dataSereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetSereServBhytOutpatientExam", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, _HisSereServBhytOutpatientExamFilter, null);
                    if (dataSereServs != null && dataSereServs.Count > 0)
                    {
                        MOS.Filter.HisExecuteRoomFilter executeRoomFilter = new HisExecuteRoomFilter();
                        executeRoomFilter.ROOM_IDs = _RoomIds;
                        var dataExecuteRooms = new BackendAdapter(new CommonParam()).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, executeRoomFilter, null);
                        if (dataExecuteRooms != null && dataExecuteRooms.Count > 0)
                        {
                            foreach (var itemRoom in dataExecuteRooms)
                            {
                                var coutSS = dataSereServs.Count(p => p.TDL_EXECUTE_ROOM_ID == itemRoom.ROOM_ID);
                                if (itemRoom.MAX_REQ_BHYT_BY_DAY.HasValue && coutSS >= itemRoom.MAX_REQ_BHYT_BY_DAY)
                                {
                                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                    string.Format("Phòng {0} đã vượt quá {1} lượt khám BHYT trong ngày. Bạn có muốn thực hiện không?", itemRoom.EXECUTE_ROOM_NAME, itemRoom.MAX_REQ_BHYT_BY_DAY),
                                    "Thông báo",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                WaitingManager.Show();
                HisServiceReqExamChangeSDO examServiceReqChangeSDO = new HisServiceReqExamChangeSDO();
                examServiceReqChangeSDO = SetDataToExamServiceReqChangeSDO();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisServiceReqExamChangeSDO  Input:", examServiceReqChangeSDO));
                var result = new BackendAdapter(param)
                    .Post<HisServiceReqResultSDO>("api/HisServiceReq/ExamChange", ApiConsumers.MosConsumer, examServiceReqChangeSDO, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    hisServiceReqResultSDO = result;
                }

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> GetSereServWithMinDuration(long patientId, List<long> serviceIds)
        {
            List<HIS_SERE_SERV> results = new List<HIS_SERE_SERV>();
            try
            {
                if (serviceIds == null || serviceIds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong truyen danh sach serviceids");
                    return null;
                }

                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>()
                    .Where(o => serviceIds.Contains(o.ID) && o.MIN_DURATION.HasValue).ToList();
                if (services == null || services.Count == 0)
                    return null;

                List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                foreach (var item in services)
                {
                    ServiceDuration sd = new ServiceDuration();
                    sd.MinDuration = item.MIN_DURATION.Value;
                    sd.ServiceId = item.ID;
                    serviceDurations.Add(sd);
                }

                CommonParam param = new CommonParam();
                HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                hisSereServMinDurationFilter.ServiceDurations = serviceDurations;
                hisSereServMinDurationFilter.PatientId = patientId;
                hisSereServMinDurationFilter.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                results = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);

                if (results != null && results.Count > 0)
                {
                    var listSereServResultTemp = from SereServResult in results
                                                 group SereServResult by SereServResult.SERVICE_ID into g
                                                 orderby g.Key
                                                 select g.FirstOrDefault();
                    results = listSereServResultTemp.ToList();
                }
            }
            catch (Exception ex)
            {
                results = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return results;
        }

        private void txtServiceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadService(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamServiceReq_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExamServiceReq.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM data = serviceRooms.FirstOrDefault(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceReq.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtServiceCode.Text = data.SERVICE_CODE;
                            LoadPatientType(data.SERVICE_ID);
                            txtPatientTypeCode.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetDataToDefault();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefesh_Click(null, null);
        }

        private void txtPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    SearchPatientType(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data = PatientTypes.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPatientTypeCode.Text = data.PATIENT_TYPE_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRoomCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Print

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (hisServiceReqResultSDO != null)
                {
                    CommonParam param = new CommonParam();
                    List<V_HIS_SERVICE_REQ> listServiceReq = new List<V_HIS_SERVICE_REQ>();
                    listServiceReq.Add(hisServiceReqResultSDO.ServiceReq);

                    HisServiceReqListResultSDO sdo = new HisServiceReqListResultSDO();
                    sdo.SereServs = hisServiceReqResultSDO.SereServs;
                    sdo.ServiceReqs = listServiceReq;

                    List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();

                    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    bedLogFilter.TREATMENT_ID = treatment.ID;
                    var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                    if (resultBedlog != null)
                    {
                        listBedLogs = resultBedlog;
                    }

                    HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, treatment);

                    if (this.currentPatientTypeAlter != null)
                    {
                        HisTreatment.PATIENT_TYPE_CODE = this.currentPatientTypeAlter.PATIENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_FROM_TIME = this.currentPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                        HisTreatment.HEIN_CARD_NUMBER = this.currentPatientTypeAlter.HEIN_CARD_NUMBER;
                        HisTreatment.HEIN_CARD_TO_TIME = this.currentPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                        HisTreatment.HEIN_MEDI_ORG_CODE = this.currentPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                        HisTreatment.LEVEL_CODE = this.currentPatientTypeAlter.LEVEL_CODE;
                        HisTreatment.RIGHT_ROUTE_CODE = this.currentPatientTypeAlter.RIGHT_ROUTE_CODE;
                        HisTreatment.RIGHT_ROUTE_TYPE_CODE = this.currentPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        HisTreatment.TREATMENT_TYPE_CODE = this.currentPatientTypeAlter.TREATMENT_TYPE_CODE;
                    }

                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(sdo, HisTreatment, listBedLogs);
                    PrintServiceReqProcessor.Print("Mps000001", false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        #endregion

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkCopyExam)
                        {
                            chkCopyExamOldContent.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCopyExamOldContent_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkCopyExam && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkCopyExamOldContent.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkCopyExam;
                    csAddOrUpdate.VALUE = (chkCopyExamOldContent.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool check_ = false;
        private void cboExamServiceReq_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                check_ = true;
                LoadPrimaryPatientType();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                check_ = true;
                LoadPrimaryPatientType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrimaryPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPrimaryPatientType.EditValue != null)
                    {
                        dtInstructionTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPrimaryPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    SearchPrimaryPatientType(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPrimaryPatientType.EditValue != null)
                {
                    HIS_PATIENT_TYPE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                                            .FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString()));
                    if (data != null)
                    {
                        txtPrimaryPatientTypeCode.Text = data.PATIENT_TYPE_CODE;
                    }
                    else
                    {
                        txtPrimaryPatientTypeCode.Text = "";
                    }
                }
                else
                {
                    txtPrimaryPatientTypeCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExamServiceReq.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM data = serviceRooms.FirstOrDefault(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceReq.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtServiceCode.Text = data.SERVICE_CODE;
                        LoadPatientType(data.SERVICE_ID);
                        txtPatientTypeCode.Focus();
                        if (sereServExams != null && sereServExams.Count > 0 && cboPatientType.EditValue == null) {
                            HIS_PATIENT_TYPE patientType = PatientTypes.FirstOrDefault(o => o.ID == sereServExams[0].PATIENT_TYPE_ID);
                            if (patientType != null)
                            {
                                txtPatientTypeCode.Text = patientType.PATIENT_TYPE_CODE;
                                cboPatientType.EditValue = patientType.ID;
                            }
                        }
                    }
                    else
                    {
                        cboPatientType.EditValue = null;
                        txtPatientTypeCode.Text = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrimaryPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                cboPrimaryPatientType.EditValue = null;
                txtPrimaryPatientTypeCode.Text = "";
            }
        }
    }
}
