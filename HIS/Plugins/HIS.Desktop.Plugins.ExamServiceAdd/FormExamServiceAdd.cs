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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceAdd
{
    public partial class FormExamServiceAdd : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        System.Globalization.CultureInfo cultureLang;
        Inventec.Desktop.Common.Modules.Module module;
        long sereServId = 0;
        long serviceReqId = 0;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentServiceReq { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> currentExecuteRoom;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> currentService;
        MOS.EFMODEL.DataModels.HIS_SERE_SERV sereServ;
        string specialityCode;
        int positionHandle = -1;
        HIS.Desktop.Common.DelegateReturnSuccess delegateSuccess = null;

        #endregion

        #region Construct
        public FormExamServiceAdd(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormExamServiceAdd(Inventec.Desktop.Common.Modules.Module module, long id)
            : this(module)
        {
            try
            {
                this.module = module;
                this.serviceReqId = id;
                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormExamServiceAdd(Inventec.Desktop.Common.Modules.Module module, long id, HIS.Desktop.Common.DelegateReturnSuccess success)
            : this(module)
        {
            try
            {
                this.module = module;
                this.serviceReqId = id;
                this.Text = module.text;
                this.delegateSuccess = success;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormExamServiceAdd_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                LoadKeysFromlanguage();

                SetDefaultValueControl();

                LoadServiceReq(serviceReqId);

                FillDataToCbo();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region Load
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
                    "IVT_LANGUAGE_KEY__FORM_EXAM_SERVICE_ADD__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciExecuteRoom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAM_SERVICE_ADD__LCI_EXECUTE_ROOM",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciService.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAM_SERVICE_ADD__LCI_SERVICE",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciChangeDepartment.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAM_SERVICE_ADD__LCI_CHANGE_DEPARTMENT",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciChkIsPrimary.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAM_SERVICE_ADD__LCI_PRIMARY",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
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
                var workPlace = LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.Where(o => o.RoomId == module.RoomId && o.RoomTypeId == module.RoomTypeId).FirstOrDefault();
                if (workPlace != null)
                {
                    this.currentExecuteRoom = LoadExecuteRoom().Where(o => o.BRANCH_ID == workPlace.BranchId).ToList();
                }
                else
                    this.currentExecuteRoom = LoadExecuteRoom();
                txtExecuteRoom.Focus();
                this.cboExecuteRoom.EditValue = "";
                this.cboExamService.EditValue = "";
                this.txtExecuteRoom.Text = "";
                this.txtService.Text = "";
                dxValidationProvider.RemoveControlError(txtExecuteRoom);
                dxValidationProvider.RemoveControlError(txtService);
                this.currentService = LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).ToList();

                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> LoadExecuteRoom()
        {
            List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> result = null;
            try
            {
                result = LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == (short)1).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadServiceReq(long serviceReqId)
        {
            try
            {
                if (serviceReqId > 0)
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = serviceReqId;
                    var ServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, null);
                    if (ServiceReq != null && ServiceReq.Count == 1)
                    {
                        currentServiceReq = ServiceReq[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCbo()
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                LoadDataSpecialityCode();

                LoadDataToCbocboExecuteRoom();

                LoadDataToCboExamService();
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }

        private void LoadDataSpecialityCode()
        {
            try
            {
                if (this.serviceReqId != 0)
                {
                    MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                    filter.SERVICE_REQ_ID = this.serviceReqId;
                    var result = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(RequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (result != null && result.Count == 1)
                    {
                        this.sereServ = result[0];
                        this.sereServId = sereServ.ID;

                        var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                        if (service != null)
                        {
                            this.specialityCode = service.SPECIALITY_CODE;
                        }

                        var listServicePaty = LocalStorage.BackendData.BackendDataWorker.Get
                            <MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                            .Where(o => o.PATIENT_TYPE_ID == sereServ.PATIENT_TYPE_ID).ToList();

                        if (listServicePaty != null && listServicePaty.Count > 0)
                        {
                            List<long> listId = listServicePaty.Select(o => o.SERVICE_ID).ToList();
                            this.currentService = this.currentService.Where(o => listId.Contains(o.SERVICE_ID)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCbocboExecuteRoom()
        {
            try
            {
                cboExecuteRoom.Properties.DataSource = currentExecuteRoom;
                cboExecuteRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboExecuteRoom.Properties.ValueMember = "ROOM_ID";
                cboExecuteRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboExecuteRoom.Properties.ImmediatePopup = true;
                cboExecuteRoom.ForceInitialize();
                cboExecuteRoom.Properties.View.Columns.Clear();
                cboExecuteRoom.Properties.PopupFormSize = new Size(350, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 0;
                aColumnCode.Width = 70;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 1;
                aColumnName.Width = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboExamService()
        {
            try
            {
                cboExamService.Properties.DisplayMember = "SERVICE_NAME";
                cboExamService.Properties.ValueMember = "SERVICE_ID";
                cboExamService.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboExamService.Properties.ImmediatePopup = true;
                cboExamService.ForceInitialize();
                cboExamService.Properties.View.Columns.Clear();
                cboExamService.Properties.PopupFormSize = new Size(350, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboExamService.Properties.View.Columns.AddField("SERVICE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 0;
                aColumnCode.Width = 70;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboExamService.Properties.View.Columns.AddField("SERVICE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 1;
                aColumnName.Width = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region validation
        private void ValidateForm()
        {
            try
            {
                ValidateGridLookupWithTextEdit(cboExamService, txtService, dxValidationProvider);
                ValidateGridLookupWithTextEdit(cboExecuteRoom, txtExecuteRoom, dxValidationProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
        #endregion

        #region Enter
        private void LoadDataCboExamService(long id)
        {
            try
            {
                if (id > 0)
                {
                    var data = currentService.Where(o => o.ROOM_ID == id).ToList();
                    cboExamService.Properties.DataSource = data;
                    if (specialityCode != null)
                    {
                        var service = data.Where(o => o.SPECIALITY_CODE.Equals(specialityCode)).FirstOrDefault();
                        if (service != null)
                        {
                            cboExamService.EditValue = service.SERVICE_ID;
                            txtService.Text = service.SERVICE_CODE;
                        }
                        else if (data != null && data.Count > 0)
                        {
                            cboExamService.EditValue = data[0].SERVICE_ID;
                            txtService.Text = data[0].SERVICE_CODE;
                        }
                        else
                        {
                            cboExamService.EditValue = null;
                            txtService.Text = "";
                        }
                    }
                    else if (data != null && data.Count > 0)
                    {
                        cboExamService.EditValue = data[0].SERVICE_ID;
                        txtService.Text = data[0].SERVICE_CODE;
                    }
                    else
                    {
                        cboExamService.EditValue = null;
                        txtService.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExecuteRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        bool showCbo = true;
            //        if (!String.IsNullOrEmpty(txtExecuteRoom.Text.Trim()))
            //        {
            //            txtService.Text = "";
            //            cboExamService.EditValue = null;
            //            string code = txtExecuteRoom.Text.Trim();
            //            var listData = currentExecuteRoom.Where(o => o.EXECUTE_ROOM_CODE.Equals(code)).FirstOrDefault();
            //            if (listData != null)
            //            {
            //                showCbo = false;
            //                txtExecuteRoom.Text = listData.EXECUTE_ROOM_CODE;
            //                cboExecuteRoom.EditValue = listData.ROOM_ID;
            //                LoadDataCboExamService(listData.ROOM_ID);
            //                if (cboExamService.EditValue != null)
            //                {
            //                    btnSave.Focus();

            //                }
            //                txtService.Focus();
            //                txtService.SelectAll();
            //            }
            //        }
            //        if (showCbo)
            //        {
            //            cboExecuteRoom.Focus();
            //            cboExecuteRoom.ShowPopup();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void txtExecuteRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtExecuteRoom.Text.Trim()))
                    {
                        txtService.Text = "";
                        cboExamService.EditValue = null;
                        string code = txtExecuteRoom.Text.Trim();
                        var listData = currentExecuteRoom.Where(o => o.EXECUTE_ROOM_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.EXECUTE_ROOM_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtExecuteRoom.Text = result.First().EXECUTE_ROOM_CODE;
                            cboExecuteRoom.EditValue = result.First().ROOM_ID;
                            LoadDataCboExamService(result.First().ROOM_ID);
                            if (cboExamService.EditValue != null)
                            {
                                btnSave.Focus();
                                e.Handled = true;
                            }
                            txtService.Focus();
                            txtService.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                        cboExamService.EditValue = null;
                        var room = currentExecuteRoom.FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString()));
                        if (room != null)
                        {
                            txtExecuteRoom.Text = room.EXECUTE_ROOM_CODE;
                            LoadDataCboExamService(room.ROOM_ID);
                            txtService.Focus();
                            txtService.SelectAll();
                            if (cboExamService.EditValue != null)
                            {
                                SendKeys.Send("{TAB}");
                                SendKeys.Send("{TAB}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool focus = true;
                    if (!String.IsNullOrEmpty(cboExecuteRoom.EditValue.ToString()))
                    {
                        txtService.Text = "";
                        cboExamService.EditValue = null;
                        var room = currentExecuteRoom.FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString()));
                        if (room != null)
                        {
                            txtExecuteRoom.Text = room.EXECUTE_ROOM_CODE;
                            LoadDataCboExamService(room.ROOM_ID);
                            if (cboExamService.EditValue != null)
                            {
                                focus = false;
                                btnSave.Focus();
                                e.Handled = true;
                            }
                        }
                    }

                    if (focus)
                    {
                        txtService.Focus();
                        txtService.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboExecuteRoom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtService.Text.Trim()))
                    {
                        string code = txtService.Text.Trim();
                        var dataSource = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>)cboExamService.Properties.DataSource;
                        var listData = dataSource.Where(o => o.SERVICE_CODE.Equals(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SERVICE_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtService.Text = result.First().SERVICE_CODE;
                            cboExamService.EditValue = result.First().SERVICE_ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (showCbo)
                    {
                        cboExamService.Focus();
                        cboExamService.ShowPopup();
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
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExamService.EditValue != null)
                    {
                        var dataSource = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>)cboExamService.Properties.DataSource;
                        var service = dataSource.FirstOrDefault(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExamService.EditValue ?? 0).ToString()));
                        if (service != null)
                        {
                            txtService.Text = service.SERVICE_CODE;
                            SendKeys.Send("{TAB}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboExamService.EditValue != null)
                    {
                        var dataSource = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>)cboExamService.Properties.DataSource;
                        var service = dataSource.FirstOrDefault(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExamService.EditValue ?? 0).ToString()));
                        if (service != null)
                        {
                            txtService.Text = service.SERVICE_CODE;
                        }
                    }
                    SendKeys.Send("{TAB}");
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboExamService.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                MOS.SDO.HisServiceReqExamAdditionSDO examAdd = new MOS.SDO.HisServiceReqExamAdditionSDO();
                if (!btnSave.Enabled) return;
                positionHandle = -1;
                if (!dxValidationProvider.Validate()) return;

                Inventec.Desktop.Common.Message.WaitingManager.Show();
                examAdd.AdditionRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString());
                examAdd.AdditionServiceId = Inventec.Common.TypeConvert.Parse.ToInt64((cboExamService.EditValue ?? 0).ToString());
                examAdd.CurrentSereServId = this.sereServId;
                examAdd.RequestRoomId = module.RoomId;
                if (chkIsPrimary.Checked) examAdd.IsPrimary = true;
                if (chkChangeDepartment.Checked) examAdd.IsChangeDepartment = true;

                ServiceReqPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>(RequestUriStore.HIS_SERVICE_REQ__EXAM_ADDITION, ApiConsumer.ApiConsumers.MosConsumer, examAdd, param);
                if (ServiceReqPrint != null)
                {
                    success = true;
                    //this.Close();
                    btnPrint.Enabled = true;
                    btnSave.Enabled = false;
                    if (delegateSuccess != null)
                    {
                        delegateSuccess(success);
                    }
                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                string printTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                richEditorMain.RunPrintTemplate(printTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (ServiceReqPrint != null)
                {
                    WaitingManager.Show();

                    ADO.ThreadDataADO data = new ADO.ThreadDataADO(ServiceReqPrint);
                    CreateThreadLoadDataForService(data);

                    V_HIS_SERE_SERV sereServExamSerivceReq = data.VHisSereServ;

                    List<MPS.Processor.Mps000071.PDO.ExeSereServSdo> sereServExamServiceReqs = new List<MPS.Processor.Mps000071.PDO.ExeSereServSdo>();
                    var aSereSrev = new MPS.Processor.Mps000071.PDO.ExeSereServSdo(data.VHisSereServ, this.ServiceReqPrint);
                    sereServExamServiceReqs.Add(aSereSrev);

                    //Mức hưởng BHYT

                    MPS.Processor.Mps000071.PDO.PatientADO Patient = new MPS.Processor.Mps000071.PDO.PatientADO(data.VHisPatient);

                    MPS.Processor.Mps000071.PDO.Mps000071PDO mps000071RDO = new MPS.Processor.Mps000071.PDO.Mps000071PDO(
                        Patient,
                        data.VHisPatientTypeAlter,
                        sereServExamServiceReqs,
                        sereServExamSerivceReq,
                        ServiceReqPrint,
                        data.FirstExamRoom,
                        data.Ratio
                     );

                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000071RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000071RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void CreateThreadLoadDataForService(ADO.ThreadDataADO data)
        {
            Thread threadTreatment = new Thread(new ParameterizedThreadStart(LoadDataTreatment));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadDataSereServ));
            try
            {

                threadTreatment.Start(data);
                threadSereServ.Start(data);

                threadTreatment.Join();
                threadSereServ.Join();
            }
            catch (Exception ex)
            {
                threadTreatment.Abort();
                threadSereServ.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServ(object obj)
        {
            try
            {
                LoadThreadDataSereServ((ADO.ThreadDataADO)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServ(ADO.ThreadDataADO data)
        {
            try
            {
                if (data != null && data.VHisServiceReq_print != null)
                {
                    data.VHisPatientTypeAlter = getPatientTypeAlter(data.VHisServiceReq_print.TREATMENT_ID, 0);

                    if (data.VHisPatientTypeAlter != null && !String.IsNullOrEmpty(data.VHisPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        data.Ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(
                            data.VHisPatientTypeAlter.HEIN_CARD_NUMBER,
                            data.VHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE,
                            data.VHisPatientTypeAlter.LEVEL_CODE,
                            data.VHisPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0;
                    }

                    MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                    sereServFilter.SERVICE_REQ_ID = data.VHisServiceReq_print.ID;
                    var sereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>(RequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, sereServFilter, null);
                    if (sereServ != null && sereServ.Count > 0)
                    {
                        var asereServ = sereServ.FirstOrDefault();
                        V_HIS_SERE_SERV ss = new V_HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(ss, sereServ.FirstOrDefault());
                        var service = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == ss.SERVICE_ID);
                        if (service != null)
                        {
                            ss.TDL_SERVICE_CODE = service.SERVICE_CODE;
                            ss.TDL_SERVICE_NAME = service.SERVICE_NAME;
                            ss.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            ss.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            ss.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            ss.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            ss.HEIN_SERVICE_TYPE_CODE = service.HEIN_SERVICE_TYPE_CODE;
                            ss.HEIN_SERVICE_TYPE_NAME = service.HEIN_SERVICE_TYPE_NAME;
                            ss.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;
                        }

                        var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == ss.TDL_EXECUTE_ROOM_ID);
                        if (executeRoom != null)
                        {
                            ss.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                            ss.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                            ss.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                            ss.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                        }

                        var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == ss.TDL_REQUEST_ROOM_ID);
                        if (reqRoom != null)
                        {
                            ss.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                            ss.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                            ss.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                            ss.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                        }

                        var patientTpye = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == ss.PATIENT_TYPE_ID);
                        if (patientTpye != null)
                        {
                            ss.PATIENT_TYPE_CODE = patientTpye.PATIENT_TYPE_CODE;
                            ss.PATIENT_TYPE_NAME = patientTpye.PATIENT_TYPE_NAME;
                        }
                        data.VHisSereServ = ss;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment(object obj)
        {
            try
            {
                LoadThreadDataTreatment((ADO.ThreadDataADO)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataTreatment(ADO.ThreadDataADO data)
        {
            try
            {
                if (data != null && data.VHisServiceReq_print != null)
                {
                    data.VHisPatient = PrintGlobalStore.GetPatientById(data.VHisServiceReq_print.TDL_PATIENT_ID);

                    HIS_TREATMENT treatment = null;
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                    treatmentFilter.ID = data.VHisServiceReq_print.TREATMENT_ID;
                    var treatmentlst = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam());
                    if (treatmentlst != null && treatmentlst.Count > 0)
                    {
                        treatment = treatmentlst.FirstOrDefault();

                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == (treatment.TDL_FIRST_EXAM_ROOM_ID ?? 0));
                        if (room != null)
                        {
                            data.FirstExamRoom = room.ROOM_NAME;
                        }
                    }

                    if (ServiceReqPrint.ICD_ID == null && String.IsNullOrEmpty(ServiceReqPrint.ICD_NAME) && treatment != null)
                    {
                        if (treatment.ICD_ID != null)
                        {
                            var icd = BackendDataWorker.Get<HIS_ICD>().Where(w => w.ID == treatment.ICD_ID).FirstOrDefault();
                            if (icd != null)
                            {
                                ServiceReqPrint.ICD_CODE = icd.ICD_CODE;
                                ServiceReqPrint.ICD_ID = icd.ID;
                                ServiceReqPrint.ICD_NAME = icd.ICD_NAME;
                            }
                        }
                        else
                        {
                            ServiceReqPrint.ICD_CODE = treatment.ICD_CODE;
                            ServiceReqPrint.ICD_NAME = treatment.ICD_NAME;
                        }

                        if (String.IsNullOrEmpty(ServiceReqPrint.ICD_SUB_CODE))
                        {
                            ServiceReqPrint.ICD_SUB_CODE = treatment.ICD_SUB_CODE;
                        }
                        if (String.IsNullOrEmpty(ServiceReqPrint.ICD_TEXT))
                        {
                            ServiceReqPrint.ICD_TEXT = treatment.ICD_TEXT;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_PATIENT_TYPE_ALTER getPatientTypeAlter(long treatmentId, long instructTime)
        {
            Inventec.Common.Logging.LogSystem.Info("Begin get HispatientTypeAlter");
            CommonParam param = new CommonParam();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = treatmentId;
                if (instructTime > 0)
                    hisPTAlterFilter.InstructionTime = instructTime;
                else
                    hisPTAlterFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                currentHispatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, param);
            }
            catch (Exception ex)
            {
                currentHispatientTypeAlter = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Info("End get HispatientTypeAlter");
            return currentHispatientTypeAlter;
        }
        #endregion

        #region shotcut
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

        private void barButtonItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
    }
}
