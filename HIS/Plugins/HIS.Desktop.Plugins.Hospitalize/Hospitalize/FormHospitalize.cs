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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using HIS.Desktop.Common;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Print;
using HIS.Desktop.Plugins.Hospitalize.Resources;

namespace HIS.Desktop.Plugins.Hospitalize.Hospitalize
{
    public partial class FormHospitalize : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        DelegateReturnSuccess returnSuccess;
        List<HIS_TREATMENT_TYPE> listTreatmentType = new List<HIS_TREATMENT_TYPE>();
        List<V_HIS_BED_ROOM> listBedRoom = new List<V_HIS_BED_ROOM>();

        List<V_HIS_DEPARTMENT_1> listDepartment = new List<V_HIS_DEPARTMENT_1>();
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_SERVICE_REQ vserviceReq;
        HisDepartmentTranHospitalizeResultSDO resultSdo;
        bool checkLoad = false;
        bool hasDon = false;

        long treatmentId;
        int positionHandleControl = -1;
        long departmentId;
        List<V_HIS_ROOM> listRoom = new List<V_HIS_ROOM>();
        List<HIS_ICD> hisICD;
        #endregion

        #region Construct

        public FormHospitalize(Inventec.Desktop.Common.Modules.Module module, long treatmentid, V_HIS_SERVICE_REQ Servicereq, DelegateReturnSuccess rsreturnSuccess)
		:base(module)
        {
            InitializeComponent();
            this.currentModule = module;
            this.treatmentId = treatmentid;
            this.returnSuccess = rsreturnSuccess;
            this.vserviceReq = Servicereq;

        }

        #endregion

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Hospitalize.Resources.Lang", typeof(HIS.Desktop.Plugins.Hospitalize.Hospitalize.FormHospitalize).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("FormHospitalize.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBedRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHospitalize.cboBedRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHospitalize.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHospitalize.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("FormHospitalize.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSavePrint.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.btnSavePrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.GiuongKeHoach", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.GiuongThucKe", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.BenhChinh", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("FormHospitalize.BenhPhu", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Lấy khoa hiện tại
        private void GetDepartmentId()
        {
            try
            {
                var wp = WorkPlace.GetWorkPlace(this.currentModule);
                if (wp != null)
                {
                    this.departmentId = wp.DepartmentId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        #endregion

        #region Load data to combo

        private void LoadDataToDepartmentCombo(DevExpress.XtraEditors.LookUpEdit cboDepartment)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDepartmentView1Filter filter = new HisDepartmentView1Filter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                this.listDepartment = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_1>>("api/HisDepartment/GetView1", ApiConsumers.MosConsumer, filter, param);
                cboDepartment.Properties.DataSource = this.listDepartment;
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.ForceInitialize();
                cboDepartment.Properties.Columns.Clear();
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "", 50));
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "", 100));
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

        private void LoadDataToDepartmentComboExecute(DevExpress.XtraEditors.LookUpEdit cboDepartment, long departmentId)
        {
            try
            {
                //Lấy các khoa là khoa cận lâm sàng IS_CLINICAL
                CommonParam param = new CommonParam();
                MOS.Filter.HisDepartmentView1Filter filter = new HisDepartmentView1Filter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_CLINICAL = true;

                this.listDepartment = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_1>>("api/HisDepartment/GetView1", ApiConsumers.MosConsumer, filter, param);
                cboDepartment.Properties.DataSource = this.listDepartment;
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.ForceInitialize();
                cboDepartment.Properties.Columns.Clear();
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "", 50));
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "", 100));
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

        private void SetEnableButton()
        {
            try
            {
                var treatmentTypeExam = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;

                //Nếu diện điều trị không phải là khám thì enable nút lưu
                btnSave.Enabled = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue).ToString()) != treatmentTypeExam ? true : false;
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
                // Lấy giường dựa trên khoa đã được chọn
                CommonParam param = new CommonParam();
                HisBedRoomFilter filter = new HisBedRoomFilter();
                filter.IS_ACTIVE = 1;
                this.listBedRoom = new BackendAdapter(param).Get<List<V_HIS_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                this.listBedRoom = this.listBedRoom.Where(o => o.DEPARTMENT_ID == this.departmentId).ToList();

                cboBedRoom.Properties.DataSource = this.listBedRoom;
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

        private void LoadDataToComboTreatmentType(DevExpress.XtraEditors.LookUpEdit cbo)
        {
            try
            {
                this.listTreatmentType = new List<HIS_TREATMENT_TYPE>();
                this.listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).ToList();

                cbo.Properties.DataSource = this.listTreatmentType;
                cbo.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.ForceInitialize();
                cbo.Properties.Columns.Clear();
                cbo.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_CODE", "", 50));
                cbo.Properties.Columns.Add(new LookUpColumnInfo("TREATMENT_TYPE_NAME", "", 100));
                cbo.Properties.ShowHeader = false;
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.DropDownRows = 10;
                cbo.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate

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

        private void ValidateForm()
        {
            try
            {
                ValidateLookupWithTextEdit(cboDepartment, txtDepartmentCode, dxValidationProvider1);
                ValidateLookupWithTextEdit(cboTreatmentType, txtTreatmentTypeCode, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void FormHospitalize_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.checkLoad = true;
                SetCaptionByLanguageKey();
                this.CheckTreatmentHasDon(this.treatmentId);
                this.GetDepartmentId();
                LoadDataToDepartmentComboExecute(cboDepartment, this.departmentId);
                LoaddataToComboChuanDoanTD();
                LoadDataToComboTreatmentType(cboTreatmentType);
                LoadDefaultLoadForm();
                ValidateForm();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void LoadDefaultLoadForm()
        {
            try
            {
                btnPrint.Enabled = false;
                dtLogTime.EditValue = DateTime.Now;
                dtLogTime.Update();
                dtLogTime.Focus();
                LoadInfoLoadForm();
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
                if (btnSave.Enabled == true)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void CheckTreatmentHasDon(long treatmentID)
        {
            //Kiểm tra xem HSDT có kê đơn thuốc không
            CommonParam param = new CommonParam();
            try
            {
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = treatmentID;
                //test
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DON;
                var check = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);
                if (check != null && check.Count > 0)
                {
                    this.hasDon = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateSdo(ref HisDepartmentTranHospitalizeSDO hospitalize)
        {
            try
            {
                hospitalize.Time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                hospitalize.TreatmentId = this.treatmentId;
                if (cboDepartment.EditValue != null)
                {
                    hospitalize.DepartmentId = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue).ToString());
                }
                hospitalize.RequestRoomId = this.currentModule.RoomId;
                if (cboBedRoom.EditValue != null)
                {
                    hospitalize.BedRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue).ToString());
                }

                if (cboIcds.EditValue != null)
                {
                    var icdData = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ID == (long)cboIcds.EditValue);
                    hospitalize.IcdId = (long)cboIcds.EditValue;
                    if (icdData != null)
                    {
                        hospitalize.IcdCode = icdData.ICD_CODE;
                    }
                    if (chkIcds.Checked == true)
                    {
                        hospitalize.IcdName = txtIcds.Text;
                    }
                    else
                        hospitalize.IcdName = icdData.ICD_NAME;
                }

                hospitalize.IcdSubCode = txtSubCode.Text;
                hospitalize.IcdText = txtIcdText.Text;
                hospitalize.TreatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "0").ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void saveProcess(bool checkPrint)
        {
            bool rs = false;
            CommonParam param = new CommonParam();
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();

                HisDepartmentTranHospitalizeSDO hospitalize = new HisDepartmentTranHospitalizeSDO();
                //Update dữ liệu gửi lên server
                UpdateSdo(ref hospitalize);

                this.resultSdo = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisDepartmentTranHospitalizeResultSDO>("api/HisDepartmentTran/Hospitalize", ApiConsumers.MosConsumer, hospitalize, null);
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData("Du lieu tra ve: ", this.resultSdo));
                if (this.resultSdo != null)
                {
                    if (this.returnSuccess != null)
                    {
                        this.returnSuccess(true);
                    }

                    rs = true;
                    this.btnPrint.Enabled = true;

                    if (checkPrint)
                    {
                        onClickPrintHospitalize();
                    }

                    btnSave.Enabled = false;
                    btnSavePrint.Enabled = false;
                    lblNumHospitalize.Text = this.resultSdo.Treatment.IN_CODE;

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
            MessageManager.Show(this, param, rs);
            #endregion

            #region Process has exception
            SessionManager.ProcessTokenLost(param);
            #endregion

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Check có đơn hay không. Nếu có đưa ra cảnh báo. nhấn yes để tiếp tục no để dừng lại
                if (this.hasDon)
                {
                    DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân đã được kê thuốc. Bạn có muốn cho bệnh nhân nhập viện không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.No)
                        return;
                }
                saveProcess(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtDepartmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //Code mẫu tối ưu xử lý tìm kiếm theo mã
                    //Thuật toán:
                    //--> Tìm kiếm like theo mã tìm kiếm
                    //--------> Nếu tìm thấy 1 => Fill dữ liệu
                    //--------> Nếu tìm thấy nhiều hơn 1 => Tìm kiếm chính xác theo mã => thấy 1 => Fill dữ liệu
                    //--------> Ngược lại => show popup combo

                    if (string.IsNullOrEmpty(txtDepartmentCode.Text))
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_DEPARTMENT_1> searchs = null;
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

        private void SetShowControl(bool set)
        {
            try
            {
                //Set ẩn hiện combo buồng
                if (set)
                {
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //Khi chọn khoa xong. Sẽ lấy số giường thực kê và số giường kế hoạch của khoa đó để hiển thị lên label 
        private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    var data = this.listDepartment.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue).ToString()) == this.departmentId)
                        {

                            SetShowControl(true);
                            LoadDataToBedRoomCombo(cboBedRoom, this.departmentId);
                        }
                        else
                        {
                            this.listBedRoom = null;
                            SetShowControl(false);
                        }
                        txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                        txtIcdCode.Focus();
                        txtIcdCode.SelectAll();
                        LoadBedCount(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBedCount(V_HIS_DEPARTMENT_1 data)
        {
            try
            {
                if (data != null)
                {
                    lblGiuongKeHoach.Text = ((long)(data.THEORY_PATIENT_COUNT == null ? 0 : data.THEORY_PATIENT_COUNT)).ToString();
                    lblGiuongThucKe.Text = ((long)(data.PATIENT_COUNT == null ? 0 : data.PATIENT_COUNT)).ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtBedRoomCode.Text = "";
                cboBedRoom.EditValue = null;
                var data = this.listDepartment.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString()));
                if (data != null)
                {
                    LoadBedCount(data);
                    LoadDataToBedRoomCombo(this.cboBedRoom, data.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
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
                            LoadBedCount(data);
                            txtIcdCode.Focus();
                            txtIcdCode.SelectAll();
                        }
                    }
                    else
                    {
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBedRoomCode_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtBedRoomCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtBedRoomCode.Text))
                    {
                        cboBedRoom.EditValue = null;
                        cboBedRoom.Focus();
                        cboBedRoom.ShowPopup();
                    }
                    else
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    var data = this.listBedRoom.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        txtBedRoomCode.Text = data.BED_ROOM_CODE;
                    }
                    btnSave.Focus();
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
                        var data = this.listBedRoom.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtBedRoomCode.Text = data.BED_ROOM_CODE;
                        }
                        btnSave.Focus();
                    }
                    else
                    {
                        cboBedRoom.Focus();
                        cboBedRoom.ShowPopup();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtTreatmentTypeCode.Text))
                    {
                        cboTreatmentType.EditValue = null;
                        cboTreatmentType.Focus();
                        cboTreatmentType.ShowPopup();
                    }
                    else
                    {
                        List<HIS_TREATMENT_TYPE> searchs = null;
                        var listData1 = this.listTreatmentType.Where(o => o.TREATMENT_TYPE_CODE.ToUpper().Contains(txtTreatmentTypeCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.TREATMENT_TYPE_CODE.ToUpper() == txtTreatmentTypeCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtTreatmentTypeCode.Text = searchs[0].TREATMENT_TYPE_CODE;
                            cboTreatmentType.EditValue = searchs[0].ID;
                        }
                        else
                        {
                            cboTreatmentType.Focus();
                            cboTreatmentType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentType.Properties.Buttons[1].Visible = false;
                    cboTreatmentType.EditValue = null;
                    txtTreatmentTypeCode.Text = "";
                    txtTreatmentTypeCode.Focus();
                    txtTreatmentTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        var data = this.listTreatmentType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtTreatmentTypeCode.Text = data.TREATMENT_TYPE_CODE;
                        }
                        txtDepartmentCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        var data = this.listTreatmentType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            txtTreatmentTypeCode.Text = data.TREATMENT_TYPE_CODE;
                        }
                        txtDepartmentCode.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtLogTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                txtTreatmentTypeCode.Focus();
                txtTreatmentTypeCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void dtLogTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentTypeCode.Focus();
                    txtTreatmentTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIcds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSubCode.Focus();
                    txtSubCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtIcdText_EditValueChanged(object sender, EventArgs e)
        {

        }

        #region ICD

        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadIcdCombo(strValue, false, this);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoaddataToComboChuanDoanTD()
        {

            try
            {
                this.hisICD = new List<HIS_ICD>();
                this.hisICD = BackendDataWorker.Get<HIS_ICD>();
                cboIcds.Properties.DataSource = this.hisICD;
                cboIcds.Properties.DisplayMember = "ICD_NAME";
                cboIcds.Properties.ValueMember = "ID";
                cboIcds.Properties.ForceInitialize();
                cboIcds.Properties.Columns.Clear();
                cboIcds.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ICD_CODE", "", 50));
                cboIcds.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ICD_NAME", "", 100));
                cboIcds.Properties.ShowHeader = false;
                cboIcds.Properties.ImmediatePopup = true;
                cboIcds.Properties.DropDownRows = 10;
                cboIcds.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCombo(string searchCode, bool isExpand, FormHospitalize control)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    if (!chkIcds.Checked)
                    {
                        control.cboIcds.Focus();
                        control.cboIcds.ShowPopup();
                    }
                    else
                    {
                        control.txtIcds.Text = "";
                        control.txtIcds.Focus();
                        control.txtIcds.SelectAll();
                    }
                }
                else
                {
                    var data = this.hisICD.Where(o => o.ICD_CODE.ToUpper().Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            if (!chkIcds.Checked)
                            {
                                control.cboIcds.EditValue = data[0].ID;
                                control.txtIcdCode.Text = data[0].ICD_CODE;
                                control.chkIcds.Focus();
                            }
                            else
                            {
                                control.cboIcds.EditValue = data[0].ID;
                                control.txtIcds.Text = data[0].ICD_NAME;
                                control.txtIcds.Focus();
                                control.txtIcds.SelectAll();
                            }
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ICD_CODE.ToUpper() == searchCode);
                            if (search != null)
                            {
                                if (!chkIcds.Checked)
                                {
                                    control.cboIcds.EditValue = search.ID;
                                    control.txtIcdCode.Text = search.ICD_CODE;
                                    control.chkIcds.Focus();
                                }
                                else
                                {
                                    control.cboIcds.EditValue = search.ID;
                                    control.txtIcds.Text = search.ICD_NAME;
                                    control.txtIcds.Focus();
                                    control.txtIcds.SelectAll();
                                }
                            }
                            else
                            {
                                if (!chkIcds.Checked)
                                {
                                    control.cboIcds.Focus();
                                    control.cboIcds.ShowPopup();
                                }
                                else
                                {
                                    control.txtIcds.Text = "";
                                    control.txtIcds.Focus();
                                    control.txtIcds.SelectAll();
                                }
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

        private void cboIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboIcds.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ICD data = this.hisICD.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtIcdCode.Text = data.ICD_CODE;
                            chkIcds.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboIcds.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ICD data = this.hisICD.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtIcdCode.Text = data.ICD_CODE;
                            chkIcds.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIcds.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIcds_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcds.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcds.Visible = true;
                    if (!this.checkLoad)
                    {
                        txtIcds.Text = cboIcds.Text;
                    }
                    txtIcds.Focus();
                    txtIcds.SelectAll();
                }
                else if (chkIcds.Checked == false)
                {
                    txtIcdCode.Focus();
                    txtIcds.Visible = false;
                    cboIcds.Visible = true;
                    txtIcds.Text = null;
                }
                this.checkLoad = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSubCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSubCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSubCode_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtSubCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtSubCode.Focus();
                            txtSubCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtSubCode.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
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

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");

                    HIS.Desktop.ADO.SecondaryIcdADO sereservInTreatmentADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtSubCode.Text, txtIcdText.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(sereservInTreatmentADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtIcdText.Text = delegateIcdNames;
                }
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtSubCode.Text = delegateIcdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (layoutControlItem3.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always &&
                       layoutControlItem6.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        txtBedRoomCode.Focus();
                        txtBedRoomCode.SelectAll();
                    }
                    else
                    {
                        //e.Handled = true;
                        btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadInfoLoadForm()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = this.treatmentId;
                var data = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                if (data != null)
                {
                    txtIcdText.Text = data.ICD_TEXT;
                    txtIcdCode.Text = data.ICD_CODE;
                    txtSubCode.Text = data.ICD_SUB_CODE;
                    if (data.ICD_ID != null)
                        cboIcds.EditValue = data.ICD_ID;
                    if (BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ID == data.ICD_ID).ICD_NAME != data.ICD_NAME)
                    {
                        txtIcds.Text = data.ICD_NAME;
                        chkIcds.Checked = true;
                    }
                    else
                    {
                        chkIcds.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #endregion

        #region Print

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                //Check có đơn hay không. Nếu có đưa ra cảnh báo. nhấn yes để tiếp tục no để dừng lại
                if (this.hasDon)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân đã được kê thuốc. Bạn có muốn cho bệnh nhân nhập viện không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        saveProcess(true);
                    }
                }
                else
                    saveProcess(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        void onClickPrintHospitalize()
        {
            try
            {
                PrintTypeCare type = new PrintTypeCare();
                type = PrintTypeCare.IN_PHIEU_VAO_VIEN;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintTypeCare
        {
            IN_PHIEU_VAO_VIEN,
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_PHIEU_VAO_VIEN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_YEU_CAU_VAO_VIEN_MPS000007, DelegateRunPrinterCare);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_YEU_CAU_VAO_VIEN_MPS000007:
                        LoadBieuMauPhieuVaoVien(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauPhieuVaoVien(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                if (this.resultSdo != null)
                {
                    CommonParam param = new CommonParam();

                    //DepartmentTran
                    var _departmentTrans = this.resultSdo.DepartmentTran;


                    //Treatment
                    var currentTreatment = this.resultSdo.Treatment;
                    var treatment = new V_HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, currentTreatment);
                    //treatment.ICD_NAME = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ID == treatment.ICD_ID).ICD_NAME;


                    //execute Room NAme
                    var executeRoomName = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId).RoomName;

                    //executeDepartmentName
                    var executeDepartmentName = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName;

                    //Patient type alter
                    V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    currentHispatientTypeAlter = this.resultSdo.PatientTypeAlter;

                    //user NAme
                    string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                    //Mức hưởng BHYT
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                    string ratio_text = "";
                    if (currentHispatientTypeAlter != null)
                    {
                        ratio_text = GetDefaultHeinRatioForView(currentHispatientTypeAlter.HEIN_CARD_NUMBER, currentHispatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, currentHispatientTypeAlter.RIGHT_ROUTE_CODE);
                    }

                    var dhst = new HIS_DHST();
                    HisDhstFilter hisDhstFilter = new MOS.Filter.HisDhstFilter();
                    hisDhstFilter.TREATMENT_ID = treatmentId;
                    dhst = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, hisDhstFilter, param).FirstOrDefault();

                    MPS.Processor.Mps000007.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000007.PDO.SingleKeyValue();
                    singleKeyValue.ExecuteDepartmentName = executeDepartmentName;
                    singleKeyValue.ExecuteRoomName = executeRoomName;
                    singleKeyValue.RatioText = ratio_text;
                    singleKeyValue.Username = userName;
                    singleKeyValue.Icd_Name = _departmentTrans.ICD_NAME;

                    var currentPatient = this.resultSdo.Patient;

                    MPS.Processor.Mps000007.PDO.Mps000007PDO mps000007RDO = new MPS.Processor.Mps000007.PDO.Mps000007PDO(
                        currentPatient,
                    currentHispatientTypeAlter,
                    _departmentTrans,
                    this.vserviceReq,
                    dhst,
                    currentTreatment,
                    singleKeyValue
                        );

                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000007RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000007RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                WaitingManager.Hide();
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickPrintHospitalize();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        #endregion

    }
}
