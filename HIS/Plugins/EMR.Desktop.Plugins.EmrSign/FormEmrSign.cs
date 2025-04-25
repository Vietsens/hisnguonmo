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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using EMR.Desktop.Plugins.EmrSign.ADO;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.EmrConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.EmrSign
{
    public partial class FormEmrSign : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private System.Globalization.CultureInfo cultureLang;

        private long DocumentId;
        private string LoginName;

        private long MaxOrder;
        private long MinOrder;

        List<EMR_SIGNER> ListSigner;

        V_EMR_DOCUMENT Document;

        List<SignADO> ListDataSign;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        bool? isEdit = null;
        DocumentUpdateStateForIntegrateSystem documentUpdateStateForIntegrateSystem;

        public FormEmrSign()
        {
            InitializeComponent();
        }

        public FormEmrSign(Inventec.Desktop.Common.Modules.Module module, long documentId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.ModuleData = module;
                this.DocumentId = documentId;
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim();
                this.Text = module.text;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormEmrSign_Load(object sender, EventArgs e)
        {
            try
            {

                LoadKeysFromlanguage();
                controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                documentUpdateStateForIntegrateSystem = new DocumentUpdateStateForIntegrateSystem();
                SetDefaultData();

                InitDataCombo();

                FillDataToControl();

                EnableSetting();
                AddPatientByKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddPatientByKey()
        {
            try
            {
                EMR.Filter.EmrDocumentTypeFilter documentTypeFilter = new Filter.EmrDocumentTypeFilter();
                documentTypeFilter.ID = this.Document.DOCUMENT_TYPE_ID;
                var apiTypeData = new BackendAdapter(new CommonParam()).Get<List<EMR_DOCUMENT_TYPE>>("api/EmrDocumentType/Get", ApiConsumers.EmrConsumer, documentTypeFilter, SessionManager.ActionLostToken, null);
                if (apiTypeData != null && apiTypeData.Count > 0)
                {
                    var IsAddPatientSign = apiTypeData.FirstOrDefault().PATIENT_MUST_SIGN == 1;
                    if (IsAddPatientSign)
                    {
                        BtnAddPatient_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCombo()
        {
            try
            {
                InitDataComboSigner();
                InitDataComboDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataComboDepartment()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemCboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataComboSigner()
        {
            try
            {
                repositoryItemCboSigner.DataSource = ListSigner;
                repositoryItemCboSigner.DisplayMember = "USERNAME";
                repositoryItemCboSigner.ValueMember = "LOGINNAME";
                repositoryItemCboSigner.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemCboSigner.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemCboSigner.ImmediatePopup = true;
                repositoryItemCboSigner.View.Columns.Clear();
                repositoryItemCboSigner.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = repositoryItemCboSigner.View.Columns.AddField("USERNAME");
                aColumnCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_SINGER");
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 150;

                GridColumn aColumnTitle = repositoryItemCboSigner.View.Columns.AddField("TITLE");
                aColumnTitle.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_TITLE");
                aColumnTitle.Visible = true;
                aColumnTitle.VisibleIndex = 2;
                aColumnTitle.Width = 80;

                GridColumn aColumnDepartment = repositoryItemCboSigner.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_DEPARTMENT");
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 3;
                aColumnDepartment.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                LblTitle.Text = "";

                EMR.Filter.EmrSignerFilter filter = new Filter.EmrSignerFilter();
                //filter.LOGINNAME = this.LoginName;
                filter.IS_ACTIVE = 1;
                ListSigner = new BackendAdapter(new CommonParam()).Get<List<EMR_SIGNER>>(EMR.URI.EmrSigner.GET, ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                if (ListSigner != null && ListSigner.Count > 0)
                {
                    var signer = ListSigner.FirstOrDefault(o => o.LOGINNAME == this.LoginName);
                    if (signer != null)
                    {
                        LblTitle.Text = String.Format("{0}-{1}-{2}-{3}", signer.LOGINNAME, signer.USERNAME, signer.TITLE, signer.DEPARTMENT_NAME);
                    }
                }

                EMR.Filter.EmrDocumentViewFilter documentFilter = new Filter.EmrDocumentViewFilter();
                documentFilter.ID = this.DocumentId;
                var apiData = new BackendAdapter(new CommonParam()).Get<List<V_EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET_VIEW, ApiConsumers.EmrConsumer, documentFilter, SessionManager.ActionLostToken, null);
                if (apiData != null && apiData.Count > 0)
                {
                    this.Document = apiData.FirstOrDefault();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableSetting()
        {
            try
            {
                var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == this.LoginName);

                if (acsUser != null)
                {
                    var acsRoleUserList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_ROLE_USER>().Where(o => o.USER_ID == acsUser.ID).ToList();
                    var controlSetting = controlAcs != null && controlAcs.Count > 0 ? controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnSetting) : null;
                    if (acsRoleUserList != null && acsRoleUserList.Count > 0 && controlSetting != null)
                    {
                        isEdit = null;
                    }
                    else
                    {
                        isEdit = false;
                        gridControlSign.Enabled = false;
                        BtnAddPatient.Enabled = false;
                        BtnSave.Enabled = false;
                    }
                }
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
                ListDataSign = new List<SignADO>();
                SignADO ado = new SignADO();
                ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                ado.NUM_ORDER = MaxOrder + 100;
                ado.IdRow = ado.NUM_ORDER;
                ado.DOCUMENT_ID = this.DocumentId;

                MinOrder = ado.NUM_ORDER;
                MaxOrder = ado.NUM_ORDER;

                this.ListDataSign.Add(ado);

                CommonParam param = new CommonParam();
                EmrSignViewFilter filter = new EmrSignViewFilter();
                filter.DOCUMENT_ID = this.DocumentId;
                var apiressult = new BackendAdapter(param).Get<List<V_EMR_SIGN>>(EMR.URI.EmrSign.GET_VIEW, ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, param);
                if (apiressult != null && apiressult.Count > 0)
                {
                    ListDataSign = new List<SignADO>();

                    foreach (var item in apiressult)
                    {
                        SignADO itemRs = new SignADO(item);
                        itemRs.IdRow = itemRs.NUM_ORDER;
                        ListDataSign.Add(itemRs);
                    }

                    if (ListDataSign != null && ListDataSign.Count > 0)
                    {
                        MaxOrder = ListDataSign.Max(o => o.IdRow);
                        MinOrder = MaxOrder;
                        var lstmin = ListDataSign.Where(o => !o.SIGN_TIME.HasValue && !o.REJECT_TIME.HasValue).ToList();
                        if (lstmin != null && lstmin.Count > 0)
                        {
                            MinOrder = lstmin.Min(m => m.IdRow);
                        }

                        ListDataSign = ListDataSign.OrderBy(o => o.IdRow).ToList();
                        ListDataSign.First().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    }
                }

                WaitingManager.Hide();
                gridControlSign.BeginUpdate();
                gridControlSign.DataSource = null;
                gridControlSign.DataSource = ListDataSign;
                gridControlSign.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.Gc_Add.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_ADD");
                this.Gc_Delete.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_DELETE");
                this.Gc_Department.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_DEPARTMENT");
                this.Gc_Down.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_DOWN");
                this.Gc_RejectReason.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_REJECT_REASON");
                this.Gc_RejectTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_REJECT_TIME");
                this.Gc_Signer.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_SINGER");
                this.Gc_SignTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_SIGN_TIME");
                this.Gc_Stt.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_STT");
                this.Gc_Title.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_TITLE");
                this.Gc_Up.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__GC_UP");
                this.repositoryItemButtonAdd.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__RP_BTN_ADD");
                this.repositoryItemButtonDelete.Buttons[0].ToolTip = this.repositoryItemButtonDeleteDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__RP_BTN_DELETE");
                this.repositoryItemButtonDown.Buttons[0].ToolTip = this.repositoryItemButtonDownDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__RP_BTN_DOWN");
                this.repositoryItemButtonRemove.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__RP_BTN_REMOVE");
                this.repositoryItemButtonUp.Buttons[0].ToolTip = this.repositoryItemButtonUpDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__RP_BTN_UP");
                this.BtnSave.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__BTN_SAVE");
                this.BtnAddPatient.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_EMR_SIGN__BTN_ADD_PATIENT");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void gridViewSign_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((View.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    long signTime = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "SIGN_TIME") ?? "").ToString());
                    long rejectTime = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "REJECT_TIME") ?? "").ToString());
                    string loginname = (View.GetRowCellValue(e.RowHandle, "LOGINNAME") ?? "").ToString();
                    bool IsPatient = Inventec.Common.TypeConvert.Parse.ToBoolean((View.GetRowCellValue(e.RowHandle, "IsPatient") ?? "").ToString());
                    long flowId = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "FLOW_ID") ?? "").ToString());

                    if (e.Column.FieldName == "ADD")
                    {
                        if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd && !this.isEdit.HasValue)
                        {
                            e.RepositoryItem = repositoryItemButtonAdd;
                        }
                        else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit && !this.isEdit.HasValue)
                        {
                            e.RepositoryItem = repositoryItemButtonRemove;
                        }
                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        if ((signTime > 0 || rejectTime > 0) || this.isEdit.HasValue)// || loginname.ToLower() != this.LoginName.ToLower())
                        {
                            e.RepositoryItem = repositoryItemButtonDeleteDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDelete;
                        }
                    }
                    else if (e.Column.FieldName == "UP")
                    {
                        if (signTime > 0 || rejectTime > 0 || this.isEdit.HasValue)
                        {
                            e.RepositoryItem = repositoryItemButtonUpDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonUp;
                        }
                    }
                    else if (e.Column.FieldName == "DOWN")
                    {
                        if (signTime > 0 || rejectTime > 0 || this.isEdit.HasValue)
                        {
                            e.RepositoryItem = repositoryItemButtonDownDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDown;
                        }
                    }
                    else if (e.Column.FieldName == "Signer")
                    {
                        if (IsPatient || flowId > 0)
                        {
                            e.RepositoryItem = repositoryItemText;
                        }
                        else
                        {
                            if (this.isEdit.HasValue && this.isEdit.Value == true)
                            {
                                if (loginname == this.LoginName)
                                {
                                    e.RepositoryItem = repositoryItemCboSigner;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemText;
                                }
                            }
                            else
                                e.RepositoryItem = repositoryItemCboSigner;

                        }
                    }
                    else if (e.Column.FieldName == "TITLE")
                    {
                        if (IsPatient || flowId > 0 || this.isEdit.HasValue)
                        {
                            e.RepositoryItem = repositoryItemText;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemText_Enable;
                        }
                    }
                    else if (e.Column.FieldName == "DEPARTMENT_ID")
                    {
                        if (IsPatient || flowId > 0 || this.isEdit.HasValue)
                        {
                            e.RepositoryItem = repositoryItemText;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCboDepartment;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSign_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (SignADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SignADO)gridViewSign.GetFocusedRow();
                if (row != null)
                {
                    if (row.REJECT_TIME.HasValue || row.SIGN_TIME.HasValue || row.ID <= 0) return;

                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceLanguageManager.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                        Resources.ResourceLanguageManager.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        var success = new BackendAdapter(param).Post<bool>(EMR.URI.EmrSign.DELETE, ApiConsumers.EmrConsumer, row.ID, SessionManager.ActionLostToken, param);
                        if (success)
                        {
                            FillDataToControl();
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        #endregion
                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SignADO)gridViewSign.GetFocusedRow();
                if (row != null)
                {
                    //chưa ký thì luôn ở dưới đã ký nên kiểm tra để tránh đẩy lên trên dòng đã ký
                    if (row.REJECT_TIME.HasValue || row.SIGN_TIME.HasValue || row.IdRow < MinOrder + 100) return;

                    var changeRow = ListDataSign.LastOrDefault(o => o.IdRow < row.IdRow);
                    if (changeRow != null && !changeRow.REJECT_TIME.HasValue && !changeRow.SIGN_TIME.HasValue)
                    {
                        UpdateSigner(changeRow, row);
                    }

                    ListDataSign = ListDataSign.OrderBy(o => o.IdRow).ToList();

                    gridControlSign.BeginUpdate();
                    gridControlSign.DataSource = null;
                    gridControlSign.DataSource = ListDataSign;
                    gridControlSign.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSigner(SignADO update, SignADO original)
        {
            try
            {
                var loginname = update.LOGINNAME;
                var signer = update.Signer;
                var title = update.TITLE;
                var departmentId = update.DEPARTMENT_ID;
                var signerTime = update.SIGN_TIME;
                var regectTime = update.REJECT_TIME;
                var rejectReason = update.REJECT_REASON;
                var isPatient = update.IsPatient;
                var flowId = update.FLOW_ID;
                var userName = update.USERNAME;
                var patientCode = update.PATIENT_CODE;
                var departmentCode = update.DEPARTMENT_CODE;
                var departmentName = update.DEPARTMENT_NAME;

                update.LOGINNAME = original.LOGINNAME;
                update.Signer = original.Signer;
                update.TITLE = original.TITLE;
                update.DEPARTMENT_ID = original.DEPARTMENT_ID;
                update.SIGN_TIME = original.SIGN_TIME;
                update.SIGN_TIME_STR = update.SIGN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(update.SIGN_TIME.Value) : "";
                update.REJECT_TIME = original.REJECT_TIME;
                update.REJECT_TIME_STR = update.REJECT_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(update.REJECT_TIME.Value) : "";
                update.REJECT_REASON = original.REJECT_REASON;
                update.IsPatient = original.IsPatient;
                update.FLOW_ID = original.FLOW_ID;
                update.USERNAME = original.USERNAME;
                update.PATIENT_CODE = original.PATIENT_CODE;
                update.DEPARTMENT_CODE = original.DEPARTMENT_CODE;
                update.DEPARTMENT_NAME = original.DEPARTMENT_NAME;

                original.DEPARTMENT_ID = departmentId;
                original.TITLE = title;
                original.Signer = signer;
                original.LOGINNAME = loginname;
                original.SIGN_TIME = signerTime;
                original.SIGN_TIME_STR = update.SIGN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(update.SIGN_TIME.Value) : "";
                original.REJECT_TIME = regectTime;
                original.REJECT_TIME_STR = update.REJECT_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(update.REJECT_TIME.Value) : "";
                original.REJECT_REASON = rejectReason;
                original.IsPatient = isPatient;
                original.FLOW_ID = flowId;
                original.USERNAME = userName;
                original.PATIENT_CODE = patientCode;
                original.DEPARTMENT_CODE = departmentCode;
                original.DEPARTMENT_NAME = departmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDown_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SignADO)gridViewSign.GetFocusedRow();
                if (row != null)
                {
                    //dòng cuối thì không thay đổi
                    if (row.REJECT_TIME.HasValue || row.SIGN_TIME.HasValue || row.IdRow >= MaxOrder) return;

                    var changeRow = ListDataSign.FirstOrDefault(o => o.IdRow > row.IdRow);
                    if (changeRow != null && !changeRow.REJECT_TIME.HasValue && !changeRow.SIGN_TIME.HasValue)
                    {
                        UpdateSigner(changeRow, row);
                    }

                    ListDataSign = ListDataSign.OrderBy(o => o.IdRow).ToList();

                    gridControlSign.BeginUpdate();
                    gridControlSign.DataSource = null;
                    gridControlSign.DataSource = ListDataSign;
                    gridControlSign.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                SignADO ado = new SignADO();
                ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                ado.NUM_ORDER = MaxOrder + 100;
                ado.IdRow = ado.NUM_ORDER;
                ado.DOCUMENT_ID = this.DocumentId;

                MaxOrder = ado.IdRow;

                this.ListDataSign.Add(ado);

                gridControlSign.BeginUpdate();
                gridControlSign.DataSource = null;
                gridControlSign.DataSource = ListDataSign;
                gridControlSign.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonRemove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SignADO)gridViewSign.GetFocusedRow();
                if (row != null)
                {
                    if (row.REJECT_TIME.HasValue || row.SIGN_TIME.HasValue || row.ID > 0) return;

                    gridControlSign.BeginUpdate();
                    gridViewSign.DeleteRow(gridViewSign.FocusedRowHandle);
                    gridControlSign.RefreshDataSource();
                    gridControlSign.EndUpdate();

                    if (row.IdRow == MaxOrder)
                    {
                        MaxOrder = ListDataSign.Max(m => m.IdRow);
                    }

                    if (row.IdRow == MinOrder)
                    {
                        var lstmin = ListDataSign.Where(o => !o.SIGN_TIME.HasValue && !o.REJECT_TIME.HasValue).ToList();
                        if (lstmin != null && lstmin.Count > 0)
                        {
                            MinOrder = lstmin.Min(m => m.IdRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboSigner_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as DevExpress.XtraEditors.GridLookUpEdit;
                    if (cbo.EditValue != null)
                    {
                        if (cbo.EditValue != cbo.OldEditValue)
                        {
                            var row = (SignADO)gridViewSign.GetFocusedRow();
                            if (row != null)
                            {
                                var signer = ListSigner.FirstOrDefault(o => o.LOGINNAME == cbo.EditValue.ToString());
                                if (signer != null)
                                {
                                    row.TITLE = signer.TITLE;
                                    row.LOGINNAME = signer.LOGINNAME;
                                    row.USERNAME = signer.USERNAME;
                                    row.PCA_SERIAL = signer.PCA_SERIAL;
                                    var department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE == signer.DEPARTMENT_CODE);
                                    if (department != null)
                                    {
                                        row.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                                        row.DEPARTMENT_ID = department.ID;
                                        row.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                                    }
                                }
                            }
                            gridViewSign.RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListDataSign != null && ListDataSign.Count > 0 && ListDataSign.Exists(o => !o.SIGN_TIME.HasValue))
                {
                    if (ListDataSign.Any(a => a.REJECT_TIME.HasValue))
                    {
                        XtraMessageBox.Show("Văn bản đã bị từ chối ký", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                    CommonParam param = new CommonParam();
                    bool success = false;

                    string doubleName = "";
                    var groupSign = ListDataSign.GroupBy(o => o.LOGINNAME).ToList();
                    foreach (var gr in groupSign)
                    {
                        if (gr.Count() > 1)
                        {
                            doubleName = gr.First().USERNAME;
                            break;
                        }
                    }

                    if (String.IsNullOrEmpty(doubleName))
                    {
                        EmrSignUpdateSDO sdo = new EmrSignUpdateSDO();
                        sdo.DocumentId = this.DocumentId;
                        var updateData = ListDataSign.Where(o => !o.SIGN_TIME.HasValue && !o.REJECT_TIME.HasValue && o.ID > 0).ToList();
                        var createData = ListDataSign.Where(o => o.ID <= 0 && (!String.IsNullOrWhiteSpace(o.LOGINNAME) || !String.IsNullOrWhiteSpace(o.PATIENT_CODE))).ToList();
                        if ((updateData == null || updateData.Count == 0) && (createData == null || createData.Count == 0))
                        {
                            if (ListDataSign.Any(a => a.REJECT_TIME.HasValue))
                            {
                                XtraMessageBox.Show("Không có dữ liệu thay đổi", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                                return;
                            }
                        }
                        Mapper.CreateMap<SignADO, EMR_SIGN>();
                        if (updateData != null && updateData.Count > 0)
                        {
                            sdo.Updates = Mapper.Map<List<EMR_SIGN>>(updateData);
                        }
                        if (createData != null && createData.Count > 0)
                        {
                            sdo.Creates = Mapper.Map<List<EMR_SIGN>>(createData);
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("______" + Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                        var apiresult = new BackendAdapter(param).Post<List<EMR_SIGN>>(EMR.URI.EmrSign.UPDATE_SDO, ApiConsumers.EmrConsumer, sdo, SessionManager.ActionLostToken, param);
                        if (apiresult != null && apiresult.Count > 0)
                        {
                            success = true;
                        }
                        //}
                        Inventec.Common.Logging.LogSystem.Debug("EMR.Desktop.Plugins.EmrSign____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                        if (success)
                        {
                             documentUpdateStateForIntegrateSystem.UpdateStateIGSys(Document, SignStateCode.DOCUMENT_HAS_SIGN_CONFIG);

                            FillDataToControl();
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Resources.ResourceLanguageManager.TrungNguoiKy, doubleName));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdateSign(SignADO row)
        {
            try
            {
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    EMR_SIGN data = new EFMODEL.DataModels.EMR_SIGN();
                    Inventec.Common.Mapper.DataObjectMapper.Map<EMR_SIGN>(data, row);

                    var apiresult = new BackendAdapter(param).Post<List<EMR_SIGN>>(EMR.URI.EmrSign.UPDATE, ApiConsumers.EmrConsumer, data, SessionManager.ActionLostToken, param);
                    if (apiresult == null || apiresult.Count <= 0)
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboSigner_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var cbo = sender as DevExpress.XtraEditors.GridLookUpEdit;
                    if (cbo.EditValue != null)
                    {
                        if (cbo.EditValue != cbo.OldEditValue)
                        {
                            var row = (SignADO)gridViewSign.GetFocusedRow();
                            if (row != null)
                            {
                                var signer = ListSigner.FirstOrDefault(o => o.LOGINNAME == cbo.EditValue.ToString());
                                if (signer != null)
                                {
                                    row.TITLE = signer.TITLE;
                                    row.LOGINNAME = signer.LOGINNAME;
                                    row.USERNAME = signer.USERNAME;
                                    var department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE == signer.DEPARTMENT_CODE);
                                    if (department != null)
                                    {
                                        row.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                                        row.DEPARTMENT_ID = department.ID;
                                        row.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                                    }
                                }
                            }
                            gridViewSign.RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void BtnAddPatient_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ListDataSign.Exists(o => o.IsPatient) && this.Document != null)
                {
                    SignADO adop = new SignADO();
                    adop.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    if (GlobalStore.EmrConfigs.Exists(o => o.KEY == "EMR.EMR_DOCUMENT.PATIENT_SIGN_FIRST.OPTION")  && GlobalStore.EmrConfigs.FirstOrDefault(o=>o.KEY == "EMR.EMR_DOCUMENT.PATIENT_SIGN_FIRST.OPTION").VALUE == "1")
                    {
                        adop.NUM_ORDER = MaxOrder - 1;
                    }
                    else 
                        adop.NUM_ORDER = MaxOrder + 100;
                    adop.IdRow = adop.NUM_ORDER;
                    adop.DOCUMENT_ID = this.DocumentId;
                    adop.PATIENT_CODE = this.Document.PATIENT_CODE;
                    adop.FIRST_NAME = this.Document.FIRST_NAME;
                    adop.LAST_NAME = this.Document.LAST_NAME;
                    adop.Signer = this.Document.VIR_PATIENT_NAME;
                    adop.TITLE = Resources.ResourceLanguageManager.BenhNhan;
                    adop.IsPatient = true;
                    this.ListDataSign.Add(adop);
                }

                MaxOrder = ListDataSign.Max(o => o.IdRow);
                WaitingManager.Hide();
                gridControlSign.BeginUpdate();
                gridControlSign.DataSource = null;
                ListDataSign = ListDataSign.OrderBy(o => o.IdRow).ToList();
                ListDataSign.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                ListDataSign.First().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                gridControlSign.DataSource = ListDataSign;
                gridControlSign.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
