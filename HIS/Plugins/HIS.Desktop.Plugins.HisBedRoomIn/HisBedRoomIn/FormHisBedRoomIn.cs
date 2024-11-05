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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Token;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using MOS.SDO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisBedRoomIn.ADO;
using HIS.Desktop.Plugins.HisBedRoomIn.Resources;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.HisBedRoomIn
{
    public partial class FormHisBedRoomIn : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        List<V_HIS_BED_ROOM> listBedRoom = new List<V_HIS_BED_ROOM>();
        List<V_HIS_BED> listBed = new List<V_HIS_BED>();
        List<HIS_DEPARTMENT> listDepartment = new List<HIS_DEPARTMENT>();
        private HIS_TREATMENT_TYPE currentTreatmentType;
        Inventec.Desktop.Common.Modules.Module currentModule;

        long roomTypeID;
        long treatmentId;
        int positionHandleControl = -1;
        long roomID;
        long departmentID;
        List<V_HIS_ROOM> listRoom = new List<V_HIS_ROOM>();
        private List<HisBedADO> dataBedADOs;
        private Common.RefeshReference RefreshRef;
        List<HIS_HOSPITALIZE_REASON> lstHopitalReason;
        List<HIS_HOSPITALIZE_REASON> lstHopitalReasonSave;
        #endregion

        #region Contructor
        public FormHisBedRoomIn(Inventec.Desktop.Common.Modules.Module module, long treatmentId, Common.RefeshReference refreshRef)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = treatmentId;
                this.currentModule = module;
                this.roomID = module.RoomId;
                this.roomTypeID = module.RoomTypeId;
                this.RefreshRef = refreshRef;
                WorkPlaceSDO = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModule.RoomId);
                VisibilityControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
        private void FormHisBedRoomIn_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                SetCaptionByLanguageKey();
                LoadDataToDepartmentCombo(cboDepartment);
                LoadDataToBedRoomCombo(cboBedRoom);
                //LoadDataToBedCombo(cboBed);
                ProcessThreadLoadData();
                LoadDefautForm();
                InitDataCboPatientType();
                InitHospitalizeReason();
                ValidateForm();
                if (WorkPlace.GetWorkPlace(currentModule).DepartmentId != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ GetWorkPlace(currentModule).RoomId  ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => WorkPlace.GetWorkPlace(currentModule).RoomId), WorkPlace.GetWorkPlace(currentModule).DepartmentId));
                    cboDepartment.EditValue = WorkPlace.GetWorkPlace(currentModule).DepartmentId;
                    txtDepartmentCode.Text = this.listDepartment.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString())).DEPARTMENT_CODE;
                }

                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitHospitalizeReason()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisHospitalizeReasonFilter filter = new MOS.Filter.HisHospitalizeReasonFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>> apiResult = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_HOSPITALIZE_REASON>>("/api/HisHospitalizeReason/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                lstHopitalReason = apiResult.Data;

                LoadComboEditor(cboHospitalizeReason, "HOSPITALIZE_REASON_CODE", "HOSPITALIZE_REASON_NAME", "HOSPITALIZE_REASON_NAME", lstHopitalReason);
                if (TreatmentWithPaTyInfo != null)
                {
                    btnHospitalizeReason.Text = TreatmentWithPaTyInfo.HOSPITALIZE_REASON_NAME;
                }
                _timer.Interval = _delayTime;
                _timer.Tick += new EventHandler(_timer_Tick);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefautForm()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBedFilter filter = new HisBedFilter();
                filter.IS_ACTIVE = 1;
                this.listBed = new BackendAdapter(param).Get<List<V_HIS_BED>>(ApiConsumer.HisRequestUriStore.HIS_BED_GETVIEW, ApiConsumers.MosConsumer, filter, param);

                dtLogTime.EditValue = DateTime.Now;
                dtLogTime.Update();
                dtLogTime.Focus();
                SpNamGhep.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBedRoomIn.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBedRoomIn.FormHisBedRoomIn).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnSave.Caption = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.barbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboPrimaryPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.CboPrimaryPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.CboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboBedService.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.CboBedService.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoOutBed.Properties.Caption = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.chkAutoOutBed.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBed.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.cboBed.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBedRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.cboBedRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //    this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAutoOut.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.lciAutoOut.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciBedService.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.LciBedService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.LciPatientType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciPatientType.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.LciPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciPrimaryPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.LciPrimaryPatientType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciPrimaryPatientType.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.LciPrimaryPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciPrice.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.LciPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciNamGhep.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.LciNamGhep.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormHisBedRoomIn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void dtLogTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                txtDepartmentCode.Focus();
                txtDepartmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void UpdateSdo(ref HisTreatmentBedRoomSDO treatmentBR)
        {
            try
            {
                if (treatmentBR.REMOVE_TIME != null)
                {
                    treatmentBR.REMOVE_TIME = null;
                    treatmentBR.REMOVE_USERNAME = null;
                    treatmentBR.REMOVE_LOGINNAME = null;
                }

                treatmentBR.TREATMENT_ID = this.treatmentId;
                treatmentBR.RequestRoomId = this.currentModule.RoomId;
                treatmentBR.ADD_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                treatmentBR.BED_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue).ToString());

                if (cboBed.EditValue != null)
                {
                    treatmentBR.BED_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboBed.EditValue ?? "").ToString());

                    if (CboBedService.EditValue != null)
                    {
                        if (CboBedService.EditValue != null)
                        {
                            treatmentBR.BedServiceTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((CboBedService.EditValue ?? "").ToString());
                        }

                        if (CboPatientType.EditValue != null)
                        {
                            treatmentBR.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((CboPatientType.EditValue ?? "").ToString());
                        }

                        if (CboPrimaryPatientType.EditValue != null)
                        {
                            treatmentBR.PrimaryPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((CboPrimaryPatientType.EditValue ?? "").ToString());
                        }

                        if (SpNamGhep.EditValue != null)
                        {
                            treatmentBR.ShareCount = long.Parse((SpNamGhep.EditValue ?? "").ToString());
                        }
                    }
                }
                else
                {
                    treatmentBR.BED_ID = null;
                }
                treatmentBR.IsAutoRemove = this.chkAutoOutBed.Checked;
                if (lstHopitalReasonSave != null && lstHopitalReasonSave.Count() == 1)
                {
                    treatmentBR.HospitalizeReasonCode = lstHopitalReasonSave.FirstOrDefault().HOSPITALIZE_REASON_CODE;
                    treatmentBR.HospitalizeReasonName = lstHopitalReasonSave.FirstOrDefault().HOSPITALIZE_REASON_NAME;
                }
                else
                {
                    treatmentBR.HospitalizeReasonCode = null;
                    treatmentBR.HospitalizeReasonName = btnHospitalizeReason.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HisTreatmentBedRoomSDO treatmentBR = new HisTreatmentBedRoomSDO();

                //Cập nhật thông tin gửi lên server
                UpdateSdo(ref treatmentBR);

                if (Config.IsUsingBedTemp == "1" && cboBed.EditValue != null && (CboBedService.EditValue == null || CboPatientType.EditValue == null))
                {
                    if (CboBedService.EditValue == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bắt buộc chọn dịch vụ giường khi đã chọn giường");
                        CboBedService.Focus();
                        CboBedService.ShowPopup();
                    }
                    else if (CboPatientType.EditValue == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bắt buộc chọn đối tượng thanh toán khi chọn giường");
                        CboPatientType.Focus();
                        CboPatientType.ShowPopup();
                    }

                    return;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisTreatmentBedRoomSDO", treatmentBR));
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisTreatmentBedRoomSDO>("api/HisTreatmentBedRoom/CreateSdo", ApiConsumers.MosConsumer, treatmentBR, null);

                if (rs != null)
                {
                    success = true;
                    if (RefreshRef != null)
                    {
                        RefreshRef();
                    }
                    this.Close();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                Inventec.Desktop.Common.LibraryMessage.MessageUtil.SetMessage(param, Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }
            #region Show message
            MessageManager.Show(this, param, success);
            #endregion

            #region Process has exception
            SessionManager.ProcessTokenLost(param);
            #endregion

            if (success)
            {
                this.Close();
            }
        }

        private void cboDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var data = this.listDepartment.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {

                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                            txtBedRoomCode.Focus();
                            txtBedRoomCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var data = this.listDepartment.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                            txtBedRoomCode.Focus();
                            txtBedRoomCode.SelectAll();
                        }

                        e.Handled = true;

                    }
                    else
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBedRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboBedRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBedRoom.Properties.Buttons[1].Visible = false;
                    cboBedRoom.EditValue = null;
                    txtBedRoomCode.Text = "";
                    txtBedRoomCode.Focus();
                    txtBedRoomCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedRoom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    var data = this.listBedRoom.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        txtBedRoomCode.Text = data.BED_ROOM_CODE;
                        cboBed.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBedRoom.EditValue != null)
                    {
                        var data = this.listBedRoom.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtBedRoomCode.Text = data.BED_ROOM_CODE;
                        }
                        cboBed.Focus();
                        e.Handled = true;
                    }
                    else
                    {
                        cboBedRoom.Focus();
                        cboBedRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboBed.EditValue = null;
                var data = this.listBedRoom.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                if (data != null)
                {
                    this.ListServiceBedByRooms = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o =>
                        o.ROOM_ID == data.ROOM_ID &&
                        o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && o.IS_ACTIVE == 1).ToList();
                    LoadDataToBedCombo(this.cboBed, data.ID);
                    LoadDefautBedServicePatient();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void cboBed_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //cboBed.Properties.Buttons[1].Visible = false;
                    cboBed.EditValue = null;
                    //txtBedCode.Text = "";
                    //txtBedCode.Focus();
                    //txtBedCode.SelectAll();
                }
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
                    if (cboBed.EditValue != null)
                    {
                        CboBedService.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDepartmentCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtDepartmentCode.Text))
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        List<HIS_DEPARTMENT> searchs = null;
                        var listData1 = this.listDepartment.Where(o => o.DEPARTMENT_CODE.ToUpper().Contains(txtDepartmentCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.DEPARTMENT_CODE.ToUpper() == txtDepartmentCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtDepartmentCode.Text = searchs[0].DEPARTMENT_CODE;
                            cboDepartment.EditValue = searchs[0].ID;
                        }
                        else
                        {
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtBedRoomCode.Text = "";
                cboBedRoom.EditValue = null;
                var data = this.listDepartment.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                if (data != null)
                {
                    LoadDataToBedRoomCombo(this.cboBedRoom, data.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void txtBedRoomCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    List<V_HIS_BED_ROOM> searchs = null;
                    var listData1 = this.listBedRoom.Where(o => o.BED_ROOM_CODE.ToUpper().Contains(txtBedRoomCode.Text.ToUpper())).ToList();
                    if (listData1 != null && listData1.Count > 0)
                    {
                        searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.BED_ROOM_CODE.ToUpper() == txtBedRoomCode.Text.ToUpper()).ToList());
                    }
                    if (searchs != null && searchs.Count == 1)
                    {
                        txtBedRoomCode.Text = searchs[0].BED_ROOM_CODE;
                        cboBedRoom.EditValue = searchs[0].ID;
                    }
                    else
                    {
                        cboBedRoom.Focus();
                        cboBedRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtLogTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtLogTime.EditValue != null)
                    {
                        txtDepartmentCode.Focus();
                        txtDepartmentCode.SelectAll();
                        e.Handled = true;
                    }
                    else
                    {
                        dtLogTime.Focus();
                        dtLogTime.ShowPopup();
                    }
                }
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

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
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
                ValidationSingleControl(dtLogTime, dxValidationProvider1);
                ValidateLookupWithTextEdit(cboBedRoom, txtBedRoomCode, dxValidationProvider1);
                ValidateLookupWithTextEdit(cboDepartment, txtDepartmentCode, dxValidationProvider1);
                ValidationSingleControl(btnHospitalizeReason, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load data to combo

        private void LoadDataToDepartmentCombo(DevExpress.XtraEditors.LookUpEdit cboDepartment)
        {
            try
            {
                //Lọc những khoa là khoa cận lâm sàng IS_CLINICAL

                this.listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                this.listDepartment = this.listDepartment.Where(o => o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList();//.Where(o => o.ID == WorkPlace.GetDepartmentId()).ToList();
                var departments = this.listDepartment.Where(o => o.BRANCH_ID == WorkPlace.GetBranchId()).ToList();
                cboDepartment.Properties.DataSource = departments;
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.ForceInitialize();
                cboDepartment.Properties.Columns.Clear();
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "", 50));
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "", 350));
                cboDepartment.Properties.ShowHeader = false;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.Properties.DropDownRows = 10;
                cboDepartment.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToBedRoomCombo(DevExpress.XtraEditors.LookUpEdit cboBedRoom)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBedRoomFilter filter = new HisBedRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.listBedRoom = new BackendAdapter(param).Get<List<V_HIS_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                cboBedRoom.Properties.DataSource = this.listBedRoom;
                cboBedRoom.Properties.DisplayMember = "BED_ROOM_NAME";
                cboBedRoom.Properties.ValueMember = "ID";
                cboBedRoom.Properties.ForceInitialize();
                cboBedRoom.Properties.Columns.Clear();
                cboBedRoom.Properties.Columns.Add(new LookUpColumnInfo("BED_ROOM_CODE", "", 50));
                cboBedRoom.Properties.Columns.Add(new LookUpColumnInfo("BED_ROOM_NAME", "", 350));
                cboBedRoom.Properties.ShowHeader = false;
                cboBedRoom.Properties.ImmediatePopup = true;
                cboBedRoom.Properties.DropDownRows = 10;
                cboBedRoom.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToBedRoomCombo(DevExpress.XtraEditors.LookUpEdit cboBedRoom, long departmentId)
        {
            try
            {
                long? treatmentTypeID = null;
                if (this.TreatmentWithPaTyInfo != null)
                {
                    treatmentTypeID = this.TreatmentWithPaTyInfo.TDL_TREATMENT_TYPE_ID;
                }
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToBedRoomCombo 1: __treatmentTypeID: " + treatmentTypeID ?? "null");
                //Khi chọn khoa load lại datasource combo bedroom
                CommonParam param = new CommonParam();
                HisBedRoomFilter filter = new HisBedRoomFilter();
                filter.IS_ACTIVE = 1;
                this.listBedRoom = new BackendAdapter(param).Get<List<V_HIS_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                if (this.listBedRoom == null)
                    this.listBedRoom = new List<V_HIS_BED_ROOM>();
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToBedRoomCombo 2");
                List<V_HIS_BED_ROOM> bedRoom_AllowTreatmentType = new List<V_HIS_BED_ROOM>();
                if (treatmentTypeID != null && treatmentTypeID > 0)
                {
                    currentTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == treatmentTypeID);
                    foreach (var item in this.listBedRoom)
                    {
                        string treatmentTypeIDs = item.TREATMENT_TYPE_IDS;
                        //Inventec.Common.Logging.LogSystem.Debug(item.BED_ROOM_NAME + "_treatmentTypeIDs: " + treatmentTypeIDs ?? "null");
                        if (treatmentTypeIDs == null)
                        {
                            bedRoom_AllowTreatmentType.Add(item);
                            continue;
                        }

                        List<long> listId = new List<long>();
                        try
                        {
                            listId = item.TREATMENT_TYPE_IDS.Split(',').Select(long.Parse).ToList() ?? new List<long>();
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }

                        if (listId.Contains(treatmentTypeID ?? -1))
                        {
                            bedRoom_AllowTreatmentType.Add(item);
                        }
                    }
                }
                else
                {
                    bedRoom_AllowTreatmentType = this.listBedRoom;
                }

                var bedRooms = bedRoom_AllowTreatmentType.Where(o => o.DEPARTMENT_ID == departmentId).ToList();
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToBedRoomCombo 3");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("bedRooms", bedRooms));
                cboBedRoom.Properties.DataSource = bedRooms;
                cboBedRoom.Properties.DisplayMember = "BED_ROOM_NAME";
                cboBedRoom.Properties.ValueMember = "ID";
                cboBedRoom.Properties.ForceInitialize();
                cboBedRoom.Properties.Columns.Clear();
                cboBedRoom.Properties.Columns.Add(new LookUpColumnInfo("BED_ROOM_CODE", "", 50));
                cboBedRoom.Properties.Columns.Add(new LookUpColumnInfo("BED_ROOM_NAME", "", 150));
                cboBedRoom.Properties.ShowHeader = false;
                cboBedRoom.Properties.ImmediatePopup = true;
                cboBedRoom.Properties.DropDownRows = 10;
                cboBedRoom.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToBedCombo(DevExpress.XtraEditors.LookUpEdit cboBed)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBedFilter filter = new HisBedFilter();
                filter.IS_ACTIVE = 1;
                this.listBed = new BackendAdapter(param).Get<List<V_HIS_BED>>(ApiConsumer.HisRequestUriStore.HIS_BED_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                cboBed.Properties.DataSource = this.listBed;
                cboBed.Properties.DisplayMember = "BED_NAME";
                cboBed.Properties.ValueMember = "ID";
                cboBed.Properties.ForceInitialize();
                cboBed.Properties.Columns.Clear();
                cboBed.Properties.Columns.Add(new LookUpColumnInfo("BED_NAME", "", 150));
                cboBed.Properties.ShowHeader = false;
                cboBed.Properties.ImmediatePopup = true;
                cboBed.Properties.DropDownRows = 10;
                cboBed.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToBedCombo(DevExpress.XtraEditors.GridLookUpEdit cboBed, long bedRoomId)
        {
            try
            {
                var beds = this.listBed.Where(o => o.BED_ROOM_ID == bedRoomId && o.IS_ACTIVE == 1).ToList();
                dataBedADOs = ProcessDataBedAdo(beds);

                dataBedADOs = dataBedADOs.OrderBy(o => o.IsKey).ThenBy(o => o.BED_CODE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_NAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("AMOUNT_STR", "", 50, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBed, dataBedADOs, controlEditorADO);
                //  cboBed.Properties.ImmediatePopup= true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboBed_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBed.EditValue != null)
                    {
                        CboBedService.Focus();
                    }
                    else
                    {
                        cboBed.Focus();
                        cboBed.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboBedService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboBedService.EditValue = null;
                    CboPatientType.EditValue = null;
                    CboPrimaryPatientType.EditValue = null;
                    CboPrimaryPatientType.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBedService_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    CboPatientType.Focus();
                    CboPatientType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBedService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboBedService.EditValue != null)
                    {
                        CboPatientType.Focus();
                        CboPatientType.SelectAll();
                    }
                    else
                    {
                        CboBedService.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboPatientType.EditValue = null;
                    CboPrimaryPatientType.EditValue = null;
                    CboPrimaryPatientType.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    CboPrimaryPatientType.Focus();
                    CboPrimaryPatientType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboPatientType.EditValue != null)
                    {
                        CboPrimaryPatientType.Focus();
                        CboPrimaryPatientType.SelectAll();
                    }
                    else
                    {
                        CboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete && !CboPrimaryPatientType.ReadOnly)
                {
                    CboPrimaryPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    chkAutoOutBed.Focus();
                    chkAutoOutBed.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboPrimaryPatientType.EditValue != null)
                    {
                        chkAutoOutBed.Focus();
                        chkAutoOutBed.SelectAll();
                    }
                    else
                    {
                        CboPrimaryPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoOutBed_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBedService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboBedService.EditValue != null)
                {
                    ReloadPatientType();
                    ReloadPrimaryPatientType();

                    HisBedADO row = dataBedADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBed.EditValue ?? "").ToString()));
                    if (row != null)
                    {
                        var bedType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((CboBedService.EditValue ?? "").ToString()));
                        row.BILL_PATIENT_TYPE_ID = bedType != null ? bedType.BILL_PATIENT_TYPE_ID : null;
                        ChoosePatientTypeDefaultlService(TreatmentWithPaTyInfo.TDL_PATIENT_TYPE_ID ?? 0, Inventec.Common.TypeConvert.Parse.ToInt64((CboBedService.EditValue ?? "").ToString()), row);
                    }
                }
                ProcessPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboPatientType.EditValue != null)
                {
                    ReloadPrimaryPatientType();
                }
                ProcessPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ProcessPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo != null && dataBedADOs != null && dataBedADOs.Count > 0)
                {
                    HisBedADO row = dataBedADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? "").ToString()));
                    if (row != null)
                    {
                        this.SpNamGhep.EditValue = null;
                        if (row.IsKey == 1)
                        {
                            if (currentTreatmentType != null && currentTreatmentType.IS_NOT_ALLOW_SHARE_BED == 1)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.KhongDuocPhepNamGhep, currentTreatmentType.TREATMENT_TYPE_NAME), ResourceMessage.ThongBao);
                                return;
                            }
                            else
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.GiuongDaCoBenhNhanNam, ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    cboBed.EditValue = null;
                                    cboBed.ShowPopup();
                                    return;
                                }

                                this.SpNamGhep.Value = row.AMOUNT + 1;
                                LoadDataToCboBedServiceType(row);
                            }
                        }
                        else if (row.IsKey == 2)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.GiuongVuotQuaSoLuong, ResourceMessage.ThongBao);
                            cboBed.EditValue = null;
                            cboBed.Focus();
                            cboBed.ShowPopup();
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("____________________________1");
                            LoadDataToCboBedServiceType(row);
                        }

                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("____________________________2");
                        CboBedService.EditValue = null;
                        CboPatientType.EditValue = null;
                        CboPrimaryPatientType.EditValue = null;
                    }
                }
                else
                {
                    CboBedService.EditValue = null;
                    CboPatientType.EditValue = null;
                    CboPrimaryPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    long IsKey = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IsKey") ?? "0").ToString());
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

        private void btnHospitalizeReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    btnHospitalizeReason.EditValue = null;
                    btnHospitalizeReason.Properties.Buttons[1].Visible = false;
                }

                if (e.Button.Kind == ButtonPredefines.Combo)
                {
                    cboHospitalizeReason.Properties.DataSource = lstHopitalReason;
                    cboHospitalizeReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private int _delayTime = 1500;
        bool firstStart = true;
        private void btnHospitalizeReason_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //nhập combobox tìm kiếm (_timer_Tick)
                _timer.Stop();
                if (firstStart == false)
                {
                    _timer.Start();
                }
                firstStart = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                string findReasonName = btnHospitalizeReason.Text;
                if (!string.IsNullOrEmpty(findReasonName))
                {
                    btnHospitalizeReason.Properties.Buttons[1].Visible = true;
                    var ketQua = lstHopitalReason.Where(i => i.HOSPITALIZE_REASON_NAME.Contains(findReasonName) || i.HOSPITALIZE_REASON_CODE.Contains(findReasonName)).ToList();
                    lstHopitalReasonSave = ketQua;
                    if (ketQua == null || ketQua.Count() == 0)
                    {
                        cboHospitalizeReason.ClosePopup();
                    }
                    else
                    {
                        cboHospitalizeReason.Properties.DataSource = ketQua;
                        cboHospitalizeReason.ShowPopup();
                    }
                }
                else
                {
                    cboHospitalizeReason.Properties.DataSource = lstHopitalReason;
                }
                _timer.Stop();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHospitalizeReason_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                btnHospitalizeReason.Text = cboHospitalizeReason.EditValue.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

