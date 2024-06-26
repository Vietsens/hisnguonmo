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
using HIS.UC.RoomExamService.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using MOS.SDO;
using System.Resources;
using System.Globalization;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.UC.RoomExamService
{
    public partial class UCRoomExamService2 : UserControl
    {
        public V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter { get; set; }
        public List<HIS_PATIENT_TYPE> currentPatientTypes { get; set; }
        public List<V_HIS_SERVICE_ROOM> hisServiceRooms { get; set; }
        public List<V_HIS_EXECUTE_ROOM> hisExecuteRooms { get; set; }

        public string layoutRoomName { get; set; }
        public string layoutExamServiceName { get; set; }
        public bool isInit { get; set; }
        public string userControlItemName { get; set; }
        public V_HIS_SERE_SERV sereServExam { get; set; }

        public RemoveRoomExamService removeUC { get; set; }
        public FocusMoveOutRoomExamService focusOutUC { get; set; }
        Action registerPatientWithRightRouteBHYT;
        Action changeRoomNotEmergency { get; set; }

        Dictionary<long, HIS_PATIENT_TYPE> dicPatientType = new Dictionary<long, HIS_PATIENT_TYPE>();
        Dictionary<long, List<V_HIS_SERVICE_ROOM>> dicRoomService = new Dictionary<long, List<V_HIS_SERVICE_ROOM>>();
        Dictionary<long, V_HIS_EXECUTE_ROOM> dicExecuteRoom = new Dictionary<long, V_HIS_EXECUTE_ROOM>();
        CultureInfo currentCulture;
        List<V_HIS_SERVICE_ROOM> currentServiceRooms = new List<V_HIS_SERVICE_ROOM>();

        string ucName;
        long? serviceId = null;

        public UCRoomExamService2(RoomExamServiceInitADO ado)
        {
            InitializeComponent();
            try
            {
                if (ado != null)
                {
                    this.currentPatientTypeAlter = ado.CurrentPatientTypeAlter;
                    this.currentPatientTypes = ado.CurrentPatientTypes;
                    this.hisExecuteRooms = ado.HisExecuteRooms;
                    this.hisServiceRooms = ado.HisServiceRooms;

                    this.isInit = ado.IsInit;
                    this.layoutExamServiceName = ado.LciExamServiceName;
                    this.layoutRoomName = ado.LciRoomName;
                    this.sereServExam = ado.SereServExam;
                    this.removeUC = ado.RemoveUC;
                    this.focusOutUC = ado.FocusOutUC;
                    this.ucName = ado.UcName;
                    this.currentCulture = ado.CurrentCulture;
                    this.registerPatientWithRightRouteBHYT = ado.RegisterPatientWithRightRouteBHYT;
                    this.changeRoomNotEmergency = ado.ChangeRoomNotEmergency;
                    if (ado.TextSize != null)
                    {
                        this.lciExamService.TextSize = ado.TextSize;
                        this.lciRoom.TextSize = ado.TextSize;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRoomExamService_Load(object sender, EventArgs e)
        {
            try
            {
                this.VisibilityControl();
                this.InitComboRoom();
                this.InitComboExamService();
                this.InitData();
                this.Init();
                this.SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void VisibilityControl()
        {
            try
            {
                if (this.isInit)
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExamService()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 120, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 420);
                ControlEditorLoader.Load(this.cboExamService, this.currentServiceRooms, controlEditorADO);
                cboExamService.Properties.ImmediatePopup = true;
                cboExamService.Properties.PopupFormSize = new System.Drawing.Size(420, cboExamService.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetDataSourceCboExamService()
        {
            try
            {
                this.cboExamService.Properties.DataSource = this.currentServiceRooms;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboRoom()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboRoom, this.hisExecuteRooms.OrderByDescending(o => o.NUM_ORDER).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitData()
        {
            try
            {
                this.dicPatientType = new Dictionary<long, HIS_PATIENT_TYPE>();
                this.dicExecuteRoom = new Dictionary<long, V_HIS_EXECUTE_ROOM>();
                this.dicRoomService = new Dictionary<long, List<V_HIS_SERVICE_ROOM>>();

                if (this.currentPatientTypes != null && this.currentPatientTypes.Count > 0)
                {
                    this.dicPatientType = this.currentPatientTypes.ToDictionary(o => o.ID, o => o);
                }

                if (this.hisExecuteRooms != null && this.hisExecuteRooms.Count > 0)
                {
                    this.dicExecuteRoom = this.hisExecuteRooms.ToDictionary(o => o.ROOM_ID, o => o);
                }

                //if (this.hisServicePatys != null && this.hisServicePatys.Count > 0)
                //{
                //    this.dicService = this.hisServicePatys
                //    .Where(o => this.dicPatientType != null && this.dicPatientType.ContainsKey(o.PATIENT_TYPE_ID))
                //    .GroupBy(o => o.SERVICE_ID)
                //    .ToDictionary(o => o.Key, o => o.ToList());
                //}

                if (this.hisServiceRooms != null && this.hisServiceRooms.Count > 0)
                {
                    this.dicRoomService = this.hisServiceRooms
                    .Where(o => BranchDataWorker.HasServicePatyWithListPatientType(o.SERVICE_ID, this.dicPatientType.Select(t => t.Key).ToList()))
                    .GroupBy(o => o.ROOM_ID)
                    .ToDictionary(o => o.Key, o => o.ToList());

                    //this.dicRoomService = this.hisServiceRooms
                    //.Where(o => this.dicService != null && this.dicService.ContainsKey(o.SERVICE_ID))
                    //.GroupBy(o => o.ROOM_ID)
                    //.ToDictionary(o => o.Key, o => o.ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.RoomExamService.Resources.Lang", typeof(HIS.UC.RoomExamService.UCRoomExamService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.cboExamService.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomExamService.cboExamService.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.cboRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomExamService.cboRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciCboRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciCboRoom.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciExamService.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciExamService.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciRoom.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Init()
        {
            try
            {
                if (this.sereServExam != null)
                {
                    if (!this.dicExecuteRoom.ContainsKey(this.sereServExam.TDL_EXECUTE_ROOM_ID))
                        return;
                    if (!this.dicRoomService.ContainsKey(this.sereServExam.TDL_EXECUTE_ROOM_ID))
                        return;
                    var room = this.dicExecuteRoom[this.sereServExam.TDL_EXECUTE_ROOM_ID];
                    var service = this.dicRoomService[this.sereServExam.TDL_EXECUTE_ROOM_ID].FirstOrDefault(o => o.SERVICE_ID == this.sereServExam.SERVICE_ID);
                    if (service != null)
                    {
                        this.serviceId = service.SERVICE_ID;
                    }
                    this.cboRoom.EditValue = room.ROOM_ID;

                    this.sereServExam = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(this.txtRoomCode.Text))
                    {
                        string key = this.txtRoomCode.Text.ToLower();
                        var listData = this.hisExecuteRooms.Where(o => o.EXECUTE_ROOM_CODE.ToLower().Contains(key)).ToList();
                        var searchResult = (listData != null && listData.Count > 0) ? (listData.Count == 1 ? listData : listData.Where(o => o.EXECUTE_ROOM_CODE.ToUpper() == txtRoomCode.Text.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            valid = true;
                            this.cboRoom.EditValue = searchResult.First().ROOM_ID;
                            if (cboExamService.EditValue == null)
                            {
                                this.txtExamServiceCode.Focus();
                                this.txtExamServiceCode.SelectAll();
                            }
                            else
                            {
                                if (this.focusOutUC != null)
                                    this.focusOutUC(this);
                            }
                        }
                    }
                    if (!valid)
                    {
                        this.cboRoom.Focus();
                        this.cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.txtExamServiceCode.Focus();
                    this.txtExamServiceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtExamServiceCode.Focus();
                    this.txtExamServiceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboExamService.EditValue = null;
                this.txtExamServiceCode.Text = "";
                this.txtRoomCode.Text = "";
                this.currentServiceRooms = new List<V_HIS_SERVICE_ROOM>();
                this.cboRoom.Properties.Buttons[1].Visible = false;
                if (this.cboRoom.EditValue != null)
                {
                    this.cboRoom.Properties.Buttons[1].Visible = true;
                    var room = this.hisExecuteRooms.FirstOrDefault(o => o.ROOM_ID == Convert.ToInt64(this.cboRoom.EditValue));
                    if (room != null)
                    {
                        this.txtRoomCode.Text = room.EXECUTE_ROOM_CODE;
                    }
                    if (this.dicRoomService.ContainsKey(Convert.ToInt64(this.cboRoom.EditValue)))
                        this.currentServiceRooms = dicRoomService[Convert.ToInt64(this.cboRoom.EditValue)];

                    //Khi người dùng đăng kí vào phòng khám là phòng cấp cứu, và đối tượng BN là BHYT thì PM tự động đổi đối tượng BN thành Đúng tuyến Cấp cứu. Giá trị hiển thị mặc định ở combobox và vẫn cho phép người dùng sửa (xử lý tại issue 8981)
                    //Bên ngoài module sử dụng uc này sẽ phải kiểm tra nếu là đối tượng BHYT thì mới gán giá trị cho action registerPatientWithRightRouteBHYT, ngược lại set null.
                    if (room.IS_EMERGENCY == 1
                        && this.registerPatientWithRightRouteBHYT != null)
                    {
                        this.registerPatientWithRightRouteBHYT();
                    }
                    //else //thaovtb xác nhận chỉ xl khi phòng là phòng cấp cứu, còn k thì k xl
                    //{
                    //    if (changeRoomNotEmergency != null)
                    //    {
                    //        changeRoomNotEmergency();
                    //        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("changeRoomNotEmergency", changeRoomNotEmergency));
                    //    }
                    //}
                }
                this.SetDataSourceCboExamService();
                if (this.serviceId.HasValue)
                {
                    this.cboExamService.EditValue = this.serviceId.Value;
                    this.serviceId = null;
                }
                else if (this.currentServiceRooms != null && this.currentServiceRooms.Count == 1)
                {
                    this.cboExamService.EditValue = this.currentServiceRooms.FirstOrDefault().SERVICE_ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboRoom.Properties.Buttons[1].Visible = false;
                    this.cboRoom.EditValue = null;
                    this.txtRoomCode.Text = "";
                    this.txtRoomCode.Focus();
                    this.txtRoomCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExamServiceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(this.txtExamServiceCode.Text) && this.currentServiceRooms != null && this.currentServiceRooms.Count > 0)
                    {
                        string key = this.txtExamServiceCode.Text.ToLower();
                        var listData = this.currentServiceRooms.Where(o => o.SERVICE_CODE.ToLower().Contains(key)).ToList();
                        var searchResult = (listData != null && listData.Count > 0) ? (listData.Count == 1 ? listData : listData.Where(o => o.SERVICE_CODE.ToUpper() == key.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            valid = true;
                            this.cboExamService.EditValue = searchResult.First().SERVICE_ID;
                            if (this.focusOutUC != null)
                                this.focusOutUC(this);
                        }
                        else
                        {
                            this.txtExamServiceCode.Text = "";
                            this.cboExamService.EditValue = null;
                        }
                    }
                    if (!valid)
                    {
                        this.cboExamService.Focus();
                        this.cboExamService.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (this.cboExamService.EditValue != null)
                    {
                        if (this.focusOutUC != null)
                            this.focusOutUC(this);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboExamService.EditValue != null)
                    {
                        if (this.focusOutUC != null)
                            this.focusOutUC(this);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtExamServiceCode.Text = "";
                this.cboExamService.Properties.Buttons[1].Visible = false;
                if (this.cboExamService.EditValue != null)
                {
                    var service = this.currentServiceRooms.FirstOrDefault(o => o.SERVICE_ID == Convert.ToInt64(this.cboExamService.EditValue));
                    if (service != null)
                    {
                        this.txtExamServiceCode.Text = service.SERVICE_CODE;
                        this.cboExamService.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboExamService.Properties.Buttons[1].Visible = false;
                    this.cboExamService.EditValue = null;
                    this.txtExamServiceCode.Text = "";
                    this.txtExamServiceCode.Focus();
                    this.txtExamServiceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.btnDelete.Enabled)
                    return;
                if (this.removeUC != null)
                    this.removeUC(this.ucName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal List<ServiceReqDetailSDO> GetDetailSDO()
        {
            List<ServiceReqDetailSDO> datas = new List<ServiceReqDetailSDO>();
            try
            {
                ServiceReqDetailSDO detail = new ServiceReqDetailSDO();
                long? roomId = null;
                long? serviceId = null;
                if (this.cboRoom.EditValue != null)
                {
                    roomId = Convert.ToInt64(this.cboRoom.EditValue);
                }
                if (this.cboExamService.EditValue != null)
                {
                    serviceId = Convert.ToInt64(this.cboExamService.EditValue);
                }
                //if (roomId.HasValue && serviceId.HasValue && roomId.Value > 0 && serviceId.Value > 0)
                //nếu chỉ chọn phòng & không chọn dịch vụ thì vẫn thêm vào
                //-> Trường hợp này chỗ sử dụng uc này sẽ đưa ra cảnh báo với người dùng, nếu vẫn muốn tiếp tục thì chỉ tạo hồ sơ, ngược lại sẽ đưa ra cảnh báo lỗi
                if (roomId.HasValue && roomId.Value > 0)
                {
                    detail.Amount = 1;
                    detail.ServiceId = (serviceId.HasValue ? serviceId.Value : 0);
                    detail.RoomId = roomId.Value;
                    detail.PatientTypeId = this.currentPatientTypeAlter.PATIENT_TYPE_ID;
                    datas.Add(detail);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                datas = new List<ServiceReqDetailSDO>();
            }
            return datas;
        }

        internal void FocusAndShowControl()
        {
            try
            {
                this.txtRoomCode.Focus();
                this.txtRoomCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusServiceControl()
        {
            try
            {
                this.txtExamServiceCode.Focus();
                this.txtExamServiceCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetValueByPatient(V_HIS_PATIENT patient)
        {
            try
            {
                this.serviceId = null;
                this.cboRoom.EditValue = null;
                if (patient != null && patient.RECENT_ROOM_ID.HasValue)
                {
                    if (!this.dicExecuteRoom.ContainsKey(patient.RECENT_ROOM_ID.Value))
                        return;
                    this.serviceId = patient.RECENT_SERVICE_ID;
                    this.cboRoom.EditValue = patient.RECENT_ROOM_ID.Value;
                }
                //else
                //{
                //    this.serviceId = null;
                //    this.cboRoom.EditValue = null;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
