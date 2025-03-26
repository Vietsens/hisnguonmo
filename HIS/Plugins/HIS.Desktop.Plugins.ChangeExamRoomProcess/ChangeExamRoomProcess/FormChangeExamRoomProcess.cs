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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MPS;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using HIS.Desktop.Plugins.ChangeExamRoomProcess.Resources;
using MPS.Processor.Mps000001.PDO;
using HIS.Desktop.LocalStorage.HisConfig;
namespace HIS.Desktop.Plugins.ChangeExamRoomProcess.ChangeExamRoomProcess
{
    public partial class FormChangeExamRoomProcess : HIS.Desktop.Utility.FormBase
    {

        #region Declare

        int positionHandle = -1;
        L_HIS_SERVICE_REQ vhissServiceReq { get; set; }
        List<MOS.EFMODEL.DataModels.V_HIS_ROOM> rooms;
        Inventec.Desktop.Common.Modules.Module Module;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentHisServiceReq { get; set; }

        #endregion

        #region Contructor

        public FormChangeExamRoomProcess(Inventec.Desktop.Common.Modules.Module module, L_HIS_SERVICE_REQ serviceReq)
            : base(module)
        {
            InitializeComponent();
            this.vhissServiceReq = serviceReq;
            this.Module = module;
            ValidateForm();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            if (this.Module != null && !String.IsNullOrEmpty(this.Module.text))
            {
                this.Text = this.Module.text;
            }
        }
        #endregion

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ChangeExamRoomProcess.Resources.Lang", typeof(HIS.Desktop.Plugins.ChangeExamRoomProcess.ChangeExamRoomProcess.FormChangeExamRoomProcess).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPriority.Properties.Caption = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.chkPriority.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.cboExecuteRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("FormChangeExamRoomProcess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_ROOM> GetRoom(List<long> serviceIds)
        {

            //Nghiệp vụ
            //Chọn ra những phòng chứa tất dịch vụ đã được chỉ định để hiển thị lên combo 

            //Lấy dịch vụ phòng
            List<V_HIS_SERVICE_ROOM> serviceRooms = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>();

            //Lấy phòng xử lý
            HIS_EXECUTE_ROOM executeRoom = new HIS_EXECUTE_ROOM();
            var listExecuteRooms = BackendDataWorker.Get<HIS_EXECUTE_ROOM>();

            var listPauseEncliticExecuteRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.IS_PAUSE_ENCLITIC == 1);

            if (listExecuteRooms != null && listExecuteRooms.Count > 0)
            {
                executeRoom = listExecuteRooms.FirstOrDefault(o => o.ROOM_ID == this.vhissServiceReq.EXECUTE_ROOM_ID);
            }


            if (serviceRooms != null && serviceRooms.Count > 0)
            {

                //Lấy phòng được cấu hình dịch vụ phòng và không phải phòng hiện tại
                List<V_HIS_ROOM> rooms = serviceRooms.Select(o => new V_HIS_ROOM
                {
                    ROOM_CODE = o.ROOM_CODE,
                    ROOM_NAME = o.ROOM_NAME,
                    ID = o.ROOM_ID,
                    ROOM_TYPE_ID = o.ROOM_TYPE_ID
                })
                .Where(o => o.ID != executeRoom.ROOM_ID) //ko lay phong hien tai
                .Distinct().ToList();

                List<V_HIS_ROOM> listRooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => rooms.Select(p => p.ID).Contains(o.ID) && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL && o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()).Distinct().ToList();

                //Lấy các phòng có dịch vụ bao gồm các dịch vụ được chỉ định để return
                List<V_HIS_ROOM> result = new List<V_HIS_ROOM>();
                foreach (V_HIS_ROOM room in listRooms)
                {
                    long count = serviceRooms
                        .Where(o => o.ROOM_ID == room.ID && serviceIds.Contains(o.SERVICE_ID))
                        .Select(o => o.SERVICE_ID)
                        .Distinct().Count();

                    if (count == serviceIds.Count)
                    {
                        result.Add(room);
                    }
                }
                //KHÔNG hiển thị các phòng đang tạm ngừng chỉ định 
                if (listPauseEncliticExecuteRoom != null && listPauseEncliticExecuteRoom.Count() > 0 && result != null && result.Count > 0)
                {
                    result = result.Distinct().Where(o => !listPauseEncliticExecuteRoom.Select(p => p.ROOM_ID).Contains(o.ID)).ToList();
                }
                return result;
            }
            return null;
        }

        private void LoadDataExamRoomFromDbToLocal(MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                //Lấy các dịch vụ đã được chỉ định từ yêu cầu
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter hisSereServFilter = new MOS.Filter.HisSereServFilter();
                hisSereServFilter.SERVICE_REQ_ID = serviceReq.ID;
                List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> lstSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, hisSereServFilter, null);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSereServ), lstSereServ));
                if (lstSereServ != null && lstSereServ.Count > 0)
                {
                    List<long> serviceIds = lstSereServ.Select(o => o.SERVICE_ID).ToList();
                    this.rooms = this.GetRoom(serviceIds);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        internal void LoadDataToComboExamRoom(FormChangeExamRoomProcess control)
        {
            try
            {
                control.cboExecuteRoom.Properties.DataSource = this.rooms;
                control.cboExecuteRoom.Properties.DisplayMember = "ROOM_NAME";
                control.cboExecuteRoom.Properties.ValueMember = "ID";
                control.cboExecuteRoom.Properties.ForceInitialize();
                control.cboExecuteRoom.Properties.Columns.Clear();
                control.cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_CODE", "", 100));
                control.cboExecuteRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_NAME", "", 200));
                control.cboExecuteRoom.Properties.ShowHeader = false;
                control.cboExecuteRoom.Properties.ImmediatePopup = true;
                control.cboExecuteRoom.Properties.DropDownRows = 10;
                control.cboExecuteRoom.Properties.PopupWidth = 300;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadExamRoomCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboExecuteRoom.EditValue = null;
                    cboExecuteRoom.Focus();
                    cboExecuteRoom.ShowPopup();
                }
                else
                {
                    List<V_HIS_ROOM> searchs = null;
                    var listDataRoom = this.rooms.Where(o => o.ROOM_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();
                    if (listDataRoom != null && listDataRoom.Count > 0)
                    {
                        searchs = (listDataRoom.Count == 1) ? listDataRoom : (listDataRoom.Where(o => o.ROOM_CODE.ToUpper() == searchCode.ToUpper()).ToList());
                    }
                    if (searchs != null && searchs.Count == 1)
                    {
                        txtExecuteRoomCode.Text = searchs[0].ROOM_CODE;
                        cboExecuteRoom.EditValue = searchs[0].ID;
                    }
                    else
                    {
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckOverExamBhyt(HIS_SERVICE_REQ input, ref V_HIS_EXECUTE_ROOM data)
        {
            bool rs = false;
            try
            {
                if (input.EXECUTE_ROOM_ID > 0)
                {
                    var executeRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == input.EXECUTE_ROOM_ID);
                    if (executeRoom != null)
                    {
                        data = executeRoom;
                        CommonParam param = new CommonParam();
                        HisSereServBhytOutpatientExamFilter filter = new HisSereServBhytOutpatientExamFilter();
                        long now = Inventec.Common.DateTime.Get.Now() ?? 0;
                        if (input.INTRUCTION_TIME > 0)
                            filter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64((input.INTRUCTION_TIME.ToString().Substring(0, 8) + "000000"));
                        filter.ROOM_IDs = new List<long>();
                        filter.ROOM_IDs.Add(executeRoom.ROOM_ID);

                        var rsApi = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetSereServBhytOutpatientExam", ApiConsumers.MosConsumer, filter, param);
                        if (rsApi != null && rsApi.Count >= (executeRoom.MAX_REQ_BHYT_BY_DAY ?? 0))
                        {
                            rs = true;
                        }
                    }
                    else
                    {
                        data = new V_HIS_EXECUTE_ROOM();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return rs;
        }

        private HIS_TREATMENT GetTreatmentById(long id)
        {
            HIS_TREATMENT rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = id;

                var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                if (rsApi != null && rsApi.Count > 0)
                {
                    rs = rsApi.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        #endregion

        #region Validdate

        private void dxValidationProviderControl_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandle == -1)
                {
                    this.positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (this.positionHandle > edit.TabIndex)
                {
                    this.positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(dtTime);
                ValidateLookupWithTextEdit(cboExecuteRoom, txtExecuteRoomCode);
                //ValidationSingleControl(txtMedicineUseFormName);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void txtExecuteRoomCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadExamRoomCombo(txtExecuteRoomCode.Text);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExecuteRoom.EditValue != null)
                    {
                        var data = this.rooms.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            txtExecuteRoomCode.Text = data.ROOM_CODE;
                            dtTime.Update();
                            dtTime.Focus();
                            //e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboExecuteRoom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_ROOM data = this.rooms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtExecuteRoomCode.Text = data.ROOM_CODE;
                            dtTime.Update();
                            dtTime.Focus();
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExecuteRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadExamRoomCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPriority.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPriority_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateData(ref HIS_SERVICE_REQ hisServiceReq)
        {
            try
            {
                hisServiceReq.ID = this.vhissServiceReq.ID;
                if (chkPriority.Checked == true)
                {
                    hisServiceReq.PRIORITY = GlobalVariables.HAS_PRIORITY;
                }
                hisServiceReq.EXECUTE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboExecuteRoom.EditValue.ToString());
                hisServiceReq.REQUEST_ROOM_ID = Module.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                MOS.EFMODEL.DataModels.HIS_SERVICE_REQ hisServiceReq = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();

                UpdateData(ref hisServiceReq);

                String dt = Inventec.Common.TypeConvert.Parse.ToDateTime(dtTime.Text).ToString("yyyyMMddHHmm") + "00";
                hisServiceReq.INTRUCTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dt);

                var treatment = GetTreatmentById(this.vhissServiceReq.TREATMENT_ID);
                if (treatment != null && hisServiceReq.INTRUCTION_TIME >= treatment.IN_TIME)
                {
                    if (hisServiceReq != null
                           && treatment != null
                           && treatment.TDL_HEIN_CARD_NUMBER != null
                           && treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                           && treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    {
                        V_HIS_EXECUTE_ROOM executeRoom = new V_HIS_EXECUTE_ROOM();
                        if (HisConfigs.Get<string>("HIS.Desktop.WarningOverExamBhyt") == "1")
                        {
                            if (CheckOverExamBhyt(hisServiceReq, ref executeRoom) && executeRoom.MAX_REQ_BHYT_BY_DAY.HasValue)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(executeRoom.EXECUTE_ROOM_NAME + " đã vượt quá " + executeRoom.MAX_REQ_BHYT_BY_DAY + " lượt khám BHYT trong ngày. Bạn có muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                    return;
                            }
                        }
                    }

                    WaitingManager.Show();
                    this.currentHisServiceReq = new HIS_SERVICE_REQ();
                    this.currentHisServiceReq = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_CHANGEROOM, ApiConsumer.ApiConsumers.MosConsumer, hisServiceReq, param);
                    if (this.currentHisServiceReq != null)
                    {
                        success = true;
                        lblNumOrder.Text = this.currentHisServiceReq.NUM_ORDER != null ? this.currentHisServiceReq.NUM_ORDER.ToString() : "";
                    }

                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian chuyển phòng không được nhỏ hơn thời gian vào viện", ResourceMessage.TieuDeThongBao);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void FormChangeExamRoomProcess_Load(object sender, EventArgs e)
        {
            
            try
            {
                SetCaptionByLanguageKey();
                LoadDataExamRoomFromDbToLocal(this.vhissServiceReq);
                LoadDataToComboExamRoom(this);
                LoadDefaultForm();
                
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void LoadDefaultForm()
        {
            try
            {

                if (vhissServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && vhissServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    dtTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                }
                else if (vhissServiceReq.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH || (vhissServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && vhissServiceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL))
                {
                    dtTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vhissServiceReq.INTRUCTION_TIME );
                }
                dtTime.Update();
                chkPriority.Checked = this.vhissServiceReq.PRIORITY == 1 ? true : false;
                lblNumOrder.Text = this.vhissServiceReq.NUM_ORDER.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Print

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate(PrintTypeCode.MPS000001, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.currentHisServiceReq == null && this.vhissServiceReq != null)
                {
                    this.currentHisServiceReq = new HIS_SERVICE_REQ();
                    AutoMapper.Mapper.CreateMap<L_HIS_SERVICE_REQ, HIS_SERVICE_REQ>();
                    this.currentHisServiceReq = AutoMapper.Mapper.Map<HIS_SERVICE_REQ>(this.vhissServiceReq);
                }
                if (this.currentHisServiceReq == null)
                    throw new ArgumentNullException("this.currentHisServiceReq sau khi luu is null");
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                V_HIS_SERVICE_REQ serviceReq;
                HIS_TREATMENT currentTreatment;
                List<V_HIS_SERE_SERV> sereServ;
                V_HIS_PATIENT_TYPE_ALTER HisPatyAlterBhyt;
                V_HIS_PATIENT currenPatient = new V_HIS_PATIENT();
                MPS.Processor.Mps000001.PDO.Mps000001ADO mps000001Ado;
                UpdateData(out serviceReq, out currentTreatment, out sereServ, out HisPatyAlterBhyt, out mps000001Ado);

                if (currentTreatment != null)
                {
                    HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                    patientViewFilter.ID = currentTreatment.PATIENT_ID;
                    currenPatient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientViewFilter, param).FirstOrDefault();
                }

                List<Mps000001_ListSereServs> listSereServs = new List<Mps000001_ListSereServs>();
                foreach (var item in sereServ)
                {
                    var data = new Mps000001_ListSereServs(item);
                    listSereServs.Add(data);
                }

                MPS.Processor.Mps000001.PDO.Mps000001PDO mps000001RDO = new MPS.Processor.Mps000001.PDO.Mps000001PDO(
                    serviceReq,
                    HisPatyAlterBhyt,
                    currenPatient,
                    listSereServs,
                    currentTreatment,
                    mps000001Ado
                    );

                MPS.ProcessorBase.PrintConfig.PreviewType printType;

                string printerName = ((GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode])) ? GlobalVariables.dicPrinter[printTypeCode] : "");
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                }
                else
                {
                    printType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                }
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001RDO, printType, printerName));

                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void UpdateData(out V_HIS_SERVICE_REQ serviceReq, out HIS_TREATMENT currentTreatment, out List<V_HIS_SERE_SERV> sereServ, out V_HIS_PATIENT_TYPE_ALTER HisPatyAlterBhyt, out MPS.Processor.Mps000001.PDO.Mps000001ADO mps000001Ado)
        {
            CommonParam param = new CommonParam();
            HIS_TREATMENT hisTreatment = new HIS_TREATMENT();


            //Lấy thông tin yêu cầu
            MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new MOS.Filter.HisServiceReqViewFilter();
            serviceReqFilter.ID = this.currentHisServiceReq.ID;
            serviceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();
            if (serviceReq == null)
                throw new ArgumentNullException("serviceReq is null");


            //Lấy thông tin khoa xử lý
            var department = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == this.currentHisServiceReq.EXECUTE_DEPARTMENT_ID);
            if (department != null)
            {
                serviceReq.EXECUTE_DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                serviceReq.EXECUTE_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
            }

            //Lấy thông tin HSDT
            MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
            hisTreatmentFilter.ID = serviceReq.TREATMENT_ID;
            currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, hisTreatmentFilter, param).FirstOrDefault();
            hisTreatment = currentTreatment;

            //Lấy thông tin chỉ định
            HisSereServViewFilter sereServFiler = new HisSereServViewFilter();
            sereServFiler.SERVICE_REQ_ID = this.currentHisServiceReq.ID;
            sereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFiler, null);


            HisPatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();

            MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
            hisPTAlterFilter.TreatmentId = serviceReq.TREATMENT_ID;
            hisPTAlterFilter.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00");
            var currentHispatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, param);
            if (currentHispatientTypeAlter != null)
            {
                var patientTypeCFG = HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                if (currentHispatientTypeAlter.PATIENT_TYPE_CODE == patientTypeCFG)
                {
                    HisPatyAlterBhyt = currentHispatientTypeAlter;
                }
            }

            //Mức hưởng BHYT
            string ratio_text = "";

            if (HisPatyAlterBhyt != null)
            {
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                ratio_text = GetDefaultHeinRatioForView(HisPatyAlterBhyt.HEIN_CARD_NUMBER, HisPatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, levelCode, HisPatyAlterBhyt.RIGHT_ROUTE_CODE);
            }

            var tranpatiData = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == hisTreatment.TRAN_PATI_REASON_ID);
            var tranpatiData1 = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == hisTreatment.TRANSFER_IN_REASON_ID);
            var roomData = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == hisTreatment.IN_ROOM_ID);
            var departmentData = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == hisTreatment.IN_DEPARTMENT_ID);

            mps000001Ado = new MPS.Processor.Mps000001.PDO.Mps000001ADO();
            mps000001Ado.ratio_text = ratio_text;
            mps000001Ado.IN_DEPARTMENT_NAME = departmentData != null ? departmentData.DEPARTMENT_NAME : "";
            mps000001Ado.IN_ROOM_NAME = roomData != null ? roomData.ROOM_NAME : "";
            //mps000001Ado.TRAN_PATI_REASON_NAME = tranpatiData != null ? tranpatiData.TRAN_PATI_REASON_NAME : "";
            mps000001Ado.firstExamRoomName = GetFirstExamRoom(serviceReq.TREATMENT_ID);
            //mps000001Ado.PHONE = patient.PHONE;
            mps000001Ado.TRANSFER_IN_REASON_NAME = tranpatiData1 != null ? tranpatiData1.TRAN_PATI_REASON_NAME : "";
        }

        private string GetFirstExamRoom(long treatmentId)
        {
            string result = "";
            try
            {
                //Lấy phòng khám đầu tiên
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                serviceReqFilter.TREATMENT_ID = treatmentId;
                var listServiceReq = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, paramCommon);
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    var firstExam = listServiceReq.OrderBy(o => o.CREATE_TIME).FirstOrDefault();
                    if (firstExam != null)
                    {
                        var roomname = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == firstExam.EXECUTE_ROOM_ID).FirstOrDefault();
                        if (roomname != null)
                        {
                            result = roomname.ROOM_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void barButtonItem_Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void lblNumOrder_Click(object sender, EventArgs e)
        {

        }

    }
}
