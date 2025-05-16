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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.ServiceRoom.ADO;
using Inventec.Core;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.UC.UCServiceRoomInfo.ClassServiceRoomInfo;
using HIS.UC.ServiceRoom;
using HIS.Desktop.DelegateRegister;
using HIS.UC.UCServiceRoomInfo.ADO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.UCServiceRoomInfo
{
    public partial class UCServiceRoomInfo : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare

        RoomExamServiceProcessor roomExamServiceProcessor;
        UserControl ucRoomExamService = null;
        int roomExamServiceNumber = 0;
        private bool isFocusCombo;
        DelegateFocusNextUserControl dlgFocusNextUserControl;
        dlgGetPatientTypeId dlgGetPatientTypeId;
        Action registerPatientWithRightRouteBHYT;
        Action changeRoomNotEmergency;
        long _RoomID = 0;
        System.Windows.Forms.Timer timer;
        DelegateGetIntructionTime dlgGetIntructionTime;
        #endregion

        #region Constructor - Load
        public UCServiceRoomInfo()
            : base("HIS.Desktop.Plugins.RegisterV2", "UCServiceRoomInfo")
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCServiceRoomInfo .1");
                InitializeComponent();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceRoomInfo .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCServiceRoomInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCServiceRoomInfo_Load .1");
                this.CreateExamServiceRoomInfoPanel();
                InitFieldFromAsync();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceRoomInfo_Load .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public async Task InitFieldFromAsync()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCServiceRoomInfo.InitFieldFromAsync .1");
                SetCaptionByLanguageKey();
                LoadComboPatientType();
                LoadComboPatientTypePrimary();
                SetDefaultComboPatientType();
                LoadExecuteRoomProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("UCServiceRoomInfo.InitFieldFromAsync .2");
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCServiceRoomInfo
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCServiceRoomInfo.Resources.Lang", typeof(UCServiceRoomInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcUCServiceRoomInfo.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lcUCServiceRoomInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboPatientTypePrimary.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.CboPatientTypePrimary.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControlGroup1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientType.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientTypePhuThu.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientTypePhuThu.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientTypePhuThu.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientTypePhuThu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Public method
        public void InitForm(ServiceRoomInitADO serviceRoomInitADO)
        {
            try
            {
                this._RoomID = serviceRoomInitADO.RoomId;
                this.dlgGetPatientTypeId = serviceRoomInitADO.dlgGetPatientTypeId;
                this.dlgFocusNextUserControl = serviceRoomInitADO.DelegateFocusNextUserControl;
                this.registerPatientWithRightRouteBHYT = serviceRoomInitADO.RegisterPatientWithRightRouteBHYT;
                this.changeRoomNotEmergency = serviceRoomInitADO.ChangeRoomNotEmergency;
                this.isFocusCombo = serviceRoomInitADO.IsFocusCombo;
                this.dlgGetIntructionTime = serviceRoomInitADO.dlgGetIntructionTime;
                this.InitExamServiceRoom(true, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitComboRoom(List<L_HIS_ROOM_COUNTER_2> executeRooms)
        {
            try
            {
                if (layoutControl2.Root.Items != null && layoutControl2.Root.Items.Count > 0)
                {
                    foreach (LayoutControlItem item in layoutControl2.Root.Items)
                    {
                        if (item != null && (item.Control is UserControl || item.Control is XtraUserControl))
                        {
                            this.roomExamServiceProcessor.InitComboRoom(item.Control, executeRooms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitComboRoom(List<L_HIS_ROOM_COUNTER_2> executeRooms, bool isSync)
        {
            try
            {
                if (layoutControl2.Root.Items != null && layoutControl2.Root.Items.Count > 0)
                {
                    foreach (LayoutControlItem item in layoutControl2.Root.Items)
                    {
                        if (item != null && (item.Control is UserControl || item.Control is XtraUserControl))
                        {
                            this.roomExamServiceProcessor.InitComboRoom(item.Control, executeRooms, isSync);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// b. Yêu cầu sửa lại:
        ///- Nếu người dùng chọn phòng khám và không chọn dịch vụ khám và phòng khám đó có cấu hình "cho phép không chọn dịch vụ" (ALLOW_NOT_CHOOSE_SERVICE = 1) thì khi thực hiện lưu sẽ xử lý:
        ///+ KHÔNG hiển thị thông báo confirm
        ///+ Gọi đến API đăng ký khám (thay vì gọi đến api tạo hồ sơ điều trị)
        ///+ Lưu ý: nếu người dùng không chọn phòng lẫn dịch vụ thì vẫn hiển thị thông báo xác nhận như cũ.
        ///- Nếu người dùng chọn phòng khám và không chọn dịch vụ khám và phòng khám không có cấu hình "cho phép không chọn dịch vụ" (ALLOW_NOT_CHOOSE_SERVICE khác 1) thì xử lý như cũ. Cụ thể:
        ///+ Hiển thị thông báo confirm
        ///+ Nếu người dùng đồng ý thì gọi api để tạo hồ sơ điều trị.
        /// </summary>
        /// <returns></returns>
        public bool ValidateRequiredField()
        {
            return true;
        }

        public void SetPatientClassify(long? patientClassifyId)
        {
            try
            {
                if (this.roomExamServiceProcessor != null && this.ucRoomExamService != null)
                {
                    this.roomExamServiceProcessor.SetPatientClassifyId(this.ucRoomExamService, patientClassifyId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void RefreshUserControl()
        {
            try
            {
                this.layoutControl2.BeginUpdate();
                this.layoutControl2.Clear();
                this.layoutControl2.Root.Clear();
                this.layoutControl2.HiddenItems.Clear();
                this.layoutControl2.Controls.Clear();
                this.layoutControl2.EndUpdate();
                this.roomExamServiceNumber = 0;
                this.CboPatientTypePrimary.EditValue = null;
                this.InitExamServiceRoom(true, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void PriorityChanged(long? priorityNumber)
        {
            try
            {
                bool enable = !(priorityNumber.HasValue && priorityNumber > 0);
                btnAddRow.Enabled = enable;
                if (!enable)
                {
                    if (layoutControl2.Root.Items != null && layoutControl2.Root.Items.Count > 1)
                    {
                        RefreshUserControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitExamServiceRoom(bool isInit, V_HIS_SERE_SERV sereServExam)
        {
            try
            {
                SetDefaultComboPatientType();
                SetDefaultComboPatientTypePrimary(sereServExam);
                if (this.roomExamServiceProcessor != null)
                {
                    RoomExamServiceInitADO roomExamServiceData = CreateInitADO(isInit, sereServExam);
                    this.ucRoomExamService = this.roomExamServiceProcessor.Run(roomExamServiceData) as UserControl;
                    if (this.ucRoomExamService != null)
                    {
                        this.ucRoomExamService.Name = roomExamServiceNumber + "";
                        this.ucRoomExamService.Dock = DockStyle.Fill;
                        this.ucRoomExamService.AutoSize = true;
                        var itemRoomExamService = LayoutControlUtil.AddToLayout(ucRoomExamService, false, new Size(410, 48), new Size(70, 20), SizeConstraintsType.Custom, new System.Drawing.Size(410, 56), new System.Drawing.Size(400, 48));
                        itemRoomExamService.TextVisible = false;
                        layoutControl2.Root.Add(itemRoomExamService);
                        if (layoutControl2.Root.Items.Count > 1)
                        {
                            LayoutControlUtil.Move(itemRoomExamService, layoutControl2.Root.Items[layoutControl2.Root.Items.Count - 1] as LayoutControlItem, DevExpress.XtraLayout.Utils.InsertType.Bottom);
                        }

                        ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(roomExamServiceData);
                    }
                    //Inventec.Common.Logging.LogSystem.Info("roomExamServiceData____" + Newtonsoft.Json.JsonConvert.SerializeObject(roomExamServiceData.HisExecuteRooms));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitExamServiceRoomByAppoiment(V_HIS_SERE_SERV sereServByAppoiment, MOS.SDO.HisPatientSDO patientSDO = null)
        {
            try
            {
                this.layoutControl2.BeginUpdate();
                this.layoutControl2.Clear();
                this.layoutControl2.Root.Clear();
                this.layoutControl2.HiddenItems.Clear();
                this.layoutControl2.Controls.Clear();
                this.layoutControl2.EndUpdate();
                this.roomExamServiceNumber = 0;
                if (this.roomExamServiceProcessor != null)
                {
                    RoomExamServiceInitADO roomExamServiceData = CreateInitADO(true, sereServByAppoiment, patientSDO);
                    this.ucRoomExamService = this.roomExamServiceProcessor.Run(roomExamServiceData) as UserControl;
                    if (this.ucRoomExamService != null)
                    {
                        this.ucRoomExamService.Name = roomExamServiceNumber + "";
                        this.ucRoomExamService.Dock = DockStyle.Fill;
                        this.ucRoomExamService.AutoSize = true;
                        var itemRoomExamService = LayoutControlUtil.AddToLayout(ucRoomExamService, false, new Size(410, 48), new Size(70, 20), SizeConstraintsType.Custom, new System.Drawing.Size(410, 56), new System.Drawing.Size(400, 48));
                        itemRoomExamService.TextVisible = false;
                        layoutControl2.Root.Add(itemRoomExamService);
                        if (layoutControl2.Root.Items.Count > 1)
                        {
                            LayoutControlUtil.Move(itemRoomExamService, layoutControl2.Root.Items[layoutControl2.Root.Items.Count - 1] as LayoutControlItem, DevExpress.XtraLayout.Utils.InsertType.Bottom);
                        }

                        ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(roomExamServiceData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadExamServiceRoom(ServiceRoomInitADO serviceRoomInitADO)
        {
            try
            {
                this._RoomID = serviceRoomInitADO.RoomId;
                this.dlgGetPatientTypeId = serviceRoomInitADO.dlgGetPatientTypeId;
                this.dlgFocusNextUserControl = serviceRoomInitADO.DelegateFocusNextUserControl;
                this.registerPatientWithRightRouteBHYT = serviceRoomInitADO.RegisterPatientWithRightRouteBHYT;
                this.dlgGetIntructionTime = serviceRoomInitADO.dlgGetIntructionTime;

                SetDefaultComboPatientType();

                if (this.roomExamServiceProcessor != null)
                {
                    if (this.ucRoomExamService != null)
                    {
                        ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(CreateInitADO(true, null));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValueExamServiceRoom(V_HIS_PATIENT vPatient)
        {
            try
            {
                if (this.roomExamServiceProcessor != null && this.ucRoomExamService != null)
                {
                    this.roomExamServiceProcessor.SetValueByPatient(this.ucRoomExamService, vPatient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<long> GetPatientTypeRoom1(long _patientTypeId)
        {
            List<long> _roomIdByPatientTypeRooms = new List<long>();
            try
            {
                MOS.Filter.HisPatientTypeRoomFilter _Filter = new MOS.Filter.HisPatientTypeRoomFilter();
                _Filter.PATIENT_TYPE_ID = _patientTypeId;
                _Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, _Filter, null);
                if (datas != null && datas.Count > 0)
                {
                    _roomIdByPatientTypeRooms = datas.Select(p => p.ROOM_ID).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return _roomIdByPatientTypeRooms;
        }

        public async Task<List<long>> GetPatientTypeRoom1Async(long _patientTypeId)
        {
            List<long> _roomIdByPatientTypeRooms = new List<long>();
            try
            {
                MOS.Filter.HisPatientTypeRoomFilter _Filter = new MOS.Filter.HisPatientTypeRoomFilter();
                _Filter.PATIENT_TYPE_ID = _patientTypeId;
                _Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var datas = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, _Filter, null);
                if (datas != null && datas.Count > 0)
                {
                    _roomIdByPatientTypeRooms = datas.Select(p => p.ROOM_ID).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return _roomIdByPatientTypeRooms;
        }

        #endregion

        #region private Method

        #region Event Control

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.btnAddRow.Enabled) return;

                this.roomExamServiceNumber += 1;
                this.InitExamServiceRoom(false, null);
                //this.roomExamServiceProcessor.FocusAndShow(this.ucRoomExamService);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void CreateExamServiceRoomInfoPanel()
        {
            try
            {
                CommonParam commonParam = new Inventec.Core.CommonParam();
                this.roomExamServiceProcessor = new RoomExamServiceProcessor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        RoomExamServiceInitADO CreateInitADO(bool isInit, V_HIS_SERE_SERV sereServExam, MOS.SDO.HisPatientSDO patientSDO = null)
        {
            RoomExamServiceInitADO roomExamServiceData = new RoomExamServiceInitADO();
            try
            {
                roomExamServiceData.TemplateDesign = TemplateDesign.T20;
                roomExamServiceData.RemoveUC = this.DeleteServiceRoomInfo;
                roomExamServiceData.FocusOutUC = this.dlgFocusNextUserControl;
                roomExamServiceData.ChangeServiceProcessPrimaryPatientType = this.ProcessPrimaryPatientTypeChangeService;
                roomExamServiceData.ChangeDisablePrimaryPatientType = this.ProcessDisablePatientTypeChangeService;
                roomExamServiceData.GetIntructionTime = this.dlgGetIntructionTime;

                List<long> _roomIdByPatientTypeRooms = new List<long>();//#15492
                long _patientTypeId = 0;
                if (this.cboPatientType.EditValue != null)
                {
                    _patientTypeId = (long)this.cboPatientType.EditValue;
                }
                else if(dlgGetPatientTypeId != null)
                {
                    _patientTypeId = this.dlgGetPatientTypeId();
                }
                if (_patientTypeId > 0)
                {
                    roomExamServiceData.CurrentPatientTypeAlter = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
                    roomExamServiceData.CurrentPatientTypeAlter.PATIENT_TYPE_ID = _patientTypeId;

                    if (_patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                        roomExamServiceData.RegisterPatientWithRightRouteBHYT = this.registerPatientWithRightRouteBHYT;
                    roomExamServiceData.ChangeRoomNotEmergency = this.changeRoomNotEmergency;

                    _roomIdByPatientTypeRooms = GetPatientTypeRoom1(_patientTypeId);
                }

                roomExamServiceData.CurrentCulture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                var dataExecuteRooms = new ExecuteRoomGet1().GetLCounter1();//#10548

				
                if (dataExecuteRooms != null && dataExecuteRooms.Count > 0)
                {
                    var roomIdActivesV2 = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => (p.IS_RESTRICT_PATIENT_TYPE == null
                        || _roomIdByPatientTypeRooms.Contains(p.ID))).Select(p => p.ID).ToList();

                    //dataExecuteRooms = dataExecuteRooms.Where(p => p.IS_PAUSE_ENCLITIC != 1 && roomIdActivesV2.Contains(p.ROOM_ID)).ToList();
                    dataExecuteRooms = dataExecuteRooms.Where(p => roomIdActivesV2.Contains(p.ROOM_ID)).ToList();
                    var roomWorking = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == this._RoomID);

                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsShowingExamRoomInDepartment && roomWorking != null)
                    {
                        List<long> _RoomIdByDepartments = new List<long>();//20939
                        _RoomIdByDepartments = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.DEPARTMENT_ID == roomWorking.DEPARTMENT_ID).Select(p => p.ID).ToList();
                        dataExecuteRooms = dataExecuteRooms.Where(p => _RoomIdByDepartments.Contains(p.ROOM_ID)
                        ).ToList();

                        Inventec.Common.Logging.LogSystem.Debug("Co cau hinh IsShowingExamRoomInDepartment, loc ra cac phong cung khoa voi phong nguoi dung dang lam viec" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._RoomID), this._RoomID) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomWorking.DEPARTMENT_ID), roomWorking.DEPARTMENT_ID) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _RoomIdByDepartments), _RoomIdByDepartments));
                    }

                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsShowingExamRoomInArea && roomWorking != null && roomWorking.AREA_ID.HasValue && roomWorking.AREA_ID > 0)
                    {
                        var _RoomIdSameAreas = BackendDataWorker.Get<HIS_ROOM>().Where(p => p.AREA_ID == null || p.AREA_ID == roomWorking.AREA_ID).Select(p => p.ID).ToList();
                        dataExecuteRooms = dataExecuteRooms.Where(p => _RoomIdSameAreas.Contains(p.ROOM_ID)
                        ).ToList();

                        Inventec.Common.Logging.LogSystem.Debug("Co cau hinh IsShowingExamRoomInArea, loc ra cac phong cung AREA_ID voi phong nguoi dung dang lam viec hoac co AREA_ID = null" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._RoomID), this._RoomID) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomWorking.AREA_ID), roomWorking.AREA_ID) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _RoomIdSameAreas), _RoomIdSameAreas));
                    }
                }

                roomExamServiceData.LHisRoomCounters = dataExecuteRooms;
                roomExamServiceData.HisServiceRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o => o.BRANCH_ID == WorkPlace.GetBranchId() && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && o.IS_ACTIVE == 1).ToList();
                //Lấy danh sách các service & room đang hoạt động (IS_ACTIVE = 1), sau đó lọc các HisServiceRooms
                var roomIdActives = roomExamServiceData.LHisRoomCounters.Select(o => o.ROOM_ID).ToList();

                var serviceIdActives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && o.IS_ACTIVE == 1).Select(o => o.ID).ToList();
                roomExamServiceData.HisServiceRooms = roomExamServiceData.HisServiceRooms.Where(o =>
                    roomIdActives != null && roomIdActives.Contains(o.ROOM_ID)
                    && serviceIdActives != null
                    && serviceIdActives.Contains(o.SERVICE_ID)).ToList();
                roomExamServiceData.UcName = roomExamServiceNumber + "";
                roomExamServiceData.IsInit = isInit;
                if (sereServExam != null)
                    roomExamServiceData.SereServExam = sereServExam;
                roomExamServiceData.CurrentPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1 && o.ID == _patientTypeId).ToList();
                roomExamServiceData.IsFocusCombo = this.isFocusCombo;
                roomExamServiceData.patientSDO = patientSDO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomExamServiceData;
        }

        private void ProcessDisablePatientTypeChangeService(bool IsDisable)
		{
			try
			{
                CboPatientTypePrimary.Enabled = IsDisable;
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}
        private void ProcessPrimaryPatientTypeChangeService(long billPatientTypeId)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = null;
                //CboPatientTypePrimary.EditValue = null;
                if (billPatientTypeId > 0)
                {
                    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1 && o.ID == billPatientTypeId).FirstOrDefault();
                    if (patientType != null && patientType.IS_ADDITION == 1 && (cboPatientType.EditValue == null || Int64.Parse(cboPatientType.EditValue.ToString()) != billPatientTypeId))
                        //&& cboPatientType.EditValue != null && billPatientTypeId != Int64.Parse(cboPatientType.EditValue.ToString()))
                    {
                        CboPatientTypePrimary.EditValue = billPatientTypeId;
                    }
                    else
                    {
                        CboPatientTypePrimary.EditValue = null;
                    }
                }			
                Inventec.Common.Logging.LogSystem.Debug("ProcessPrimaryPatientTypeChangeService:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => billPatientTypeId), billPatientTypeId) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientType), patientType));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExecuteRoomProcess()
        {
            try
            {
                timer = new System.Windows.Forms.Timer();
                int tgian = 300000;
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonThoiGianLoadDanhSachPhongKham > 0)
                {
                    tgian = (int)HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonThoiGianLoadDanhSachPhongKham;
                }
                timer.Interval = Convert.ToInt32(tgian);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void timer_Tick (object sender, System.EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { ProcessTimerSyncRoomCounter(); }));
                }
                else
                {
                    ProcessTimerSyncRoomCounter();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        async Task ProcessTimerSyncRoomCounter()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessTimerSyncRoomCounter.1");
                List<long> _roomIdByPatientTypeRooms = new List<long>();
                long _patientTypeId = 0;
                if (this.cboPatientType.EditValue != null)
                {
                    _patientTypeId = (long)this.cboPatientType.EditValue;
                }
                else if (dlgGetPatientTypeId != null)
                {
                    _patientTypeId = this.dlgGetPatientTypeId();
                }

                if (_patientTypeId > 0)
                {
                    //roomExamServiceData.CurrentPatientTypeAlter = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
                    //roomExamServiceData.CurrentPatientTypeAlter.PATIENT_TYPE_ID = _patientTypeId;

                    //if (_patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                    //    roomExamServiceData.RegisterPatientWithRightRouteBHYT = this.registerPatientWithRightRouteBHYT;
                    //roomExamServiceData.ChangeRoomNotEmergency = this.changeRoomNotEmergency;

                    _roomIdByPatientTypeRooms = await GetPatientTypeRoom1Async(_patientTypeId);
                }

                var dataExecuteRooms = await new ExecuteRoomGet1().GetLCounter1Async();//#10548
                if (dataExecuteRooms != null && dataExecuteRooms.Count > 0)
                {
                    var roomIdActivesV2 = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => (p.IS_RESTRICT_PATIENT_TYPE == null
                        || _roomIdByPatientTypeRooms.Contains(p.ID))).Select(p => p.ID).ToList();

                    //dataExecuteRooms = dataExecuteRooms.Where(p => p.IS_PAUSE_ENCLITIC != 1 && roomIdActivesV2.Contains(p.ROOM_ID)).ToList();
                    dataExecuteRooms = dataExecuteRooms.Where(p => roomIdActivesV2.Contains(p.ROOM_ID)).ToList();

                    var roomWorking = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == this._RoomID);

                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsShowingExamRoomInDepartment && roomWorking != null)
                    {
                        List<long> _RoomIdByDepartments = new List<long>();
                        _RoomIdByDepartments = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.DEPARTMENT_ID == roomWorking.DEPARTMENT_ID).Select(p => p.ID).ToList();
                        dataExecuteRooms = dataExecuteRooms.Where(p => _RoomIdByDepartments.Contains(p.ROOM_ID)
                        ).ToList();
                    }

                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsShowingExamRoomInArea && roomWorking != null && roomWorking.AREA_ID.HasValue && roomWorking.AREA_ID > 0)
                    {
                        var _RoomIdSameAreas = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.AREA_ID == null || p.AREA_ID == roomWorking.AREA_ID).Select(p => p.ID).ToList();
                        dataExecuteRooms = dataExecuteRooms.Where(p => _RoomIdSameAreas.Contains(p.ROOM_ID)
                        ).ToList();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("ProcessTimerSyncRoomCounter.2");
                InitComboRoom(dataExecuteRooms, true);
                Inventec.Common.Logging.LogSystem.Debug("ProcessTimerSyncRoomCounter.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DeleteServiceRoomInfo(object LayoutControlItemName)
        {
            bool result = false;
            try
            {
                if (LayoutControlItemName != null)
                {
                    //layoutControl2.GetItemByControl(layoutControl2.GetControlByName((LayoutControlItemName as Control).Name)).Visibility = LayoutVisibility.Never;

                    for (int i = layoutControl2.Root.Items.Count - 1; i >= 0; i--)
                    {
                        LayoutControlItem item = layoutControl2.Root.Items[i] as LayoutControlItem;
                        UserControl itemDelete = (UserControl)LayoutControlItemName;
                        if (item != null && (item.Control is UserControl || item.Control is XtraUserControl) && item.Control.Name == itemDelete.Name)
                        {
                            Control c = item.Control;
                            layoutControl2.Root.RemoveAt(i);
                            c.Dispose();
                            layoutControl2.Refresh();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void SetDefaultComboPatientTypePrimary(V_HIS_SERE_SERV sereServExam)
        {
            try
            {
                if (sereServExam != null && sereServExam.PRIMARY_PATIENT_TYPE_ID.HasValue)
                {
                    var paties = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.IS_ADDITION.HasValue && p.IS_ADDITION == 1 && p.ID == sereServExam.PRIMARY_PATIENT_TYPE_ID).FirstOrDefault();
                    if (paties != null)
                    {
                        CboPatientTypePrimary.EditValue = sereServExam.PRIMARY_PATIENT_TYPE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultComboPatientType()
        {
            try
            {
                //this.txtPatientTypeCode.Text = "";
                this.cboPatientType.EditValue = null;
                //if (this.dlgGetPatientTypeId() > 0)
                //{
                //    LoadComboPatientType();
                //}
                if (this._RoomID > 0)
                {
                    var _room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == this._RoomID);
                    if (_room != null
                        && HisConfigCFG.RoomCodeChoosePatientType != null
                        && HisConfigCFG.RoomCodeChoosePatientType.Count > 0
                        && HisConfigCFG.RoomCodeChoosePatientType.FirstOrDefault(p => p.Trim() == _room.ROOM_CODE) != null)
                    {
                        //lciPatientTypeCode.Visibility = LayoutVisibility.Always;
                        lciCboPatientType.Visibility = LayoutVisibility.Always;

                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == _room.DEPARTMENT_ID && p.IS_ACTIVE == 1);
                        if (department != null && department.DEFAULT_INSTR_PATIENT_TYPE_ID > 0)
                        {
                            var pt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.ID == department.DEFAULT_INSTR_PATIENT_TYPE_ID && p.IS_ACTIVE == 1);
                            if (pt != null)
                            {
                                //this.txtPatientTypeCode.Text = pt.PATIENT_TYPE_CODE;
                                this.cboPatientType.EditValue = pt.ID;
                            }
                        }
                        else
                        {
                            var pt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.ID == this.dlgGetPatientTypeId() && p.IS_ACTIVE == 1);
                            if (pt != null)
                            {
                                //this.txtPatientTypeCode.Text = pt.PATIENT_TYPE_CODE;
                                this.cboPatientType.EditValue = pt.ID;
                            }
                        }
                    }
                    else
                    {
                        var pt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.ID == this.dlgGetPatientTypeId() && p.IS_ACTIVE == 1);
                        if (pt != null)
                        {
                            //this.txtPatientTypeCode.Text = pt.PATIENT_TYPE_CODE;
                            this.cboPatientType.EditValue = pt.ID;
                        }
                        //lciPatientTypeCode.Visibility = LayoutVisibility.Never;
                        lciCboPatientType.Visibility = LayoutVisibility.Never;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboPatientType()
        {
            try
            {
                var paties = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList();
                //if (this.dlgGetPatientTypeId() > 0)
                //{

                //}
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 60, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 280, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 340);
                ControlEditorLoader.Load(this.cboPatientType, paties, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboPatientTypePrimary()
        {
            try
            {
                string cfgPT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE");
                if (cfgPT == "1" || cfgPT == "2")
                {
                    lciCboPatientTypePhuThu.Visibility = LayoutVisibility.Always;

                    var paties = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.IS_ADDITION == 1).ToList();

                    List<ColumnInfo> columnInfosPT = new List<ColumnInfo>();
                    columnInfosPT.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 60, 1));
                    columnInfosPT.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 280, 2));
                    ControlEditorADO controlEditorADOPT = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfosPT, false, 340);
                    ControlEditorLoader.Load(this.CboPatientTypePrimary, paties, controlEditorADOPT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadData(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadData(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPatientType.Focus();
                    cboPatientType.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o =>
                        o.IS_ACTIVE == 1 && o.PATIENT_TYPE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPatientType.EditValue = data[0].ID;
                            cboPatientType.Properties.Buttons[1].Visible = true;


                            //TODO focus
                            ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).FocusUserControl();

                            if (this.ucRoomExamService != null)
                            {
                                ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(CreateInitADO(true, null));
                            }
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PATIENT_TYPE_CODE == searchCode);
                            if (search != null)
                            {
                                cboPatientType.EditValue = search.ID;
                                cboPatientType.Properties.Buttons[1].Visible = true;


                                //TODO focus
                                ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).FocusUserControl();
                                if (this.ucRoomExamService != null)
                                {
                                    ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(CreateInitADO(true, null));
                                }
                            }
                            else
                            {
                                cboPatientType.EditValue = null;
                                cboPatientType.Focus();
                                cboPatientType.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPatientType.EditValue = null;
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientType.Properties.Buttons[1].Visible = false;
                    cboPatientType.EditValue = null;

                    if (this.ucRoomExamService != null)
                    {
                        ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(CreateInitADO(true, null));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()));
                        {
                            cboPatientType.Properties.Buttons[1].Visible = true;


                            //TODO focus
                            ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).FocusUserControl();
                            if (this.ucRoomExamService != null)
                            {
                                ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(CreateInitADO(true, null));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboPatientType.Properties.Buttons[1].Visible = true;

                            //TODO focus
                            ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).FocusUserControl();
                            if (this.ucRoomExamService != null)
                            {
                                ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).InitLoad(CreateInitADO(true, null));
                            }
                        }
                    }
                }
                else
                {
                    cboPatientType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientTypePrimary_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CboPatientTypePrimary.Properties.Buttons[1].Visible = CboPatientTypePrimary.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientTypePrimary_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboPatientTypePrimary.Properties.Buttons[1].Visible = false;
                    CboPatientTypePrimary.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientTypePrimary_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (CboPatientTypePrimary.EditValue != null)
                    {
                        ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).FocusUserControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientTypePrimary_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboPatientTypePrimary.EditValue != null)
                    {
                        ((UC.ServiceRoom.UCRoomExamService)ucRoomExamService).FocusUserControl();
                    }
                }
                else
                {
                    CboPatientTypePrimary.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion


    }
}
