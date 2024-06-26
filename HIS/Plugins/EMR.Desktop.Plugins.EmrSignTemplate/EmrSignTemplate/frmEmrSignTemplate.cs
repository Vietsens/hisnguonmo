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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using EMR.Desktop.Plugins.EmrSignTemplate.Resources;
using EMR.Desktop.Plugins.EmrSignTemplate.Validtion;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.EmrSignTemplate.EmrSignTemplate
{
    public partial class frmEmrSignTemplate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;

        EMR_SIGN_TEMP currentData;

        List<EMR_SIGN_TEMP> EmrSignTemp;
        List<EMR_SIGNER> EmrSigner;
        List<EMR_SIGNER> GridSigner;

        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmEmrSignTemplate(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void frmEmrSignTemplate_Load(object sender, EventArgs e)
        {
            SetDefaultValue();
            //Focus default
            SetDefaultFocus();

            //LoadCombobox
            InitComboServiceType();

            EnableControlChanged(this.ActionType);

            FillDataToControl();

            // kiem tra du lieu nhap vao
            ValidateForm();
            //set ngon ngu
            SetCaptionByLanguagekey();

            //Set tabindex control
            InitTabIndex();
        }

        private void InitComboServiceType()
        {
            try
            {
                EmrSigner = new List<EMR_SIGNER>();
                CommonParam param = new CommonParam();

                EmrSignerFilter filter = new EmrSignerFilter();
                filter.IS_ACTIVE = 1;

                EmrSigner = new BackendAdapter(param).Get<List<EMR_SIGNER>>(EmrRequestUriStore.EMR_SIGNER_GET, ApiConsumers.EmrConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboUserName, EmrSigner, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();

                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<EMR_SIGN_TEMP>> apiResult = null;

                EmrSignTemp = new List<EMR_SIGN_TEMP>();

                EmrSignTempFilter filter = new EmrSignTempFilter();

                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<EMR_SIGN_TEMP>>(EmrRequestUriStore.EMR_SIGN_TEMP_GET, ApiConsumers.EmrConsumer, filter, paramCommon);

                if (apiResult != null)
                {
                    EmrSignTemp = (List<EMR_SIGN_TEMP>)apiResult.Data;
                    if (EmrSignTemp != null)
                    {
                        gridView1.GridControl.DataSource = EmrSignTemp;
                        rowCount = (EmrSignTemp == null ? 0 : EmrSignTemp.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void SetFilterNavBar(ref EmrSignTempFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                filter.CREATOR = loginName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("EMR.Desktop.Plugins.EmrSignTemplate.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrSignTemplate.EmrSignTemplate.frmEmrSignTemplate).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt

                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdditional.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.btnAdditional.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPatientSign.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.btnPatientSign.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdditional.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.bbtnAdditional.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColum10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmEmrSignTemplate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void InitTabIndex()
        {
            try
            {
                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl4);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        #region validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtSignTempCode, dxValidationProvider1);
                ValidationSingleControl(txtSignTempName, dxValidationProvider1);
                ValidationVoiceText();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void ValidationVoiceText()
        {
            try
            {
                ValidationSignedBy validRule = new ValidationSignedBy();
                validRule.gridview = gridView2;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtLoginName, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void EnableControlChanged(int action)
        {
            try
            {
                
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;

                GridSigner = new List<EMR_SIGNER>();
                txtSearch.Text = "";
                txtSignTempCode.Text = "";
                txtSignTempName.Text = "";
                txtLoginName.Text = "";
                cboUserName.EditValue = null;
                ResetFormData();

                gridView2.BeginUpdate();
                gridView2.GridControl.DataSource = null;
                gridView2.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl4.IsInitialized) return;
                layoutControl4.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl4.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                    txtSignTempCode.Focus();
                    txtSignTempCode.SelectAll();

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl4.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPatientSign_Click(object sender, EventArgs e)
        {
            try
            {
                var check = GridSigner.FirstOrDefault(o => o.USERNAME.ToUpper() == ResourceMessage.BenhNhanKy.ToUpper());

                if (check != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DaTonTaiNguoiKy, check.USERNAME));
                }
                else 
                {
                    EMR_SIGNER row = new EMR_SIGNER();
                    row.USERNAME = ResourceMessage.BenhNhanKy;
                    row.LOGINNAME = "1";

                    row.NUM_ORDER = GridSigner.Count + 1;

                    GridSigner.Add(row);

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = GridSigner;
                    gridView2.EndUpdate();
                }

                txtLoginName.Text = "";
                cboUserName.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnAdditional_Click(object sender, EventArgs e)
        {
            try
            {
                List<EMR_SIGNER> lstcheck = new List<EMR_SIGNER>();
                if (GridSigner != null && GridSigner.Count > 0)
                {
                    lstcheck = GridSigner.Where(o => o.LOGINNAME != null).ToList();
                }

                if (cboUserName.EditValue != null)
                {
                    var search = lstcheck.FirstOrDefault(o => o.LOGINNAME.ToUpper() == cboUserName.EditValue.ToString().ToUpper());

                    if (search != null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DaTonTaiNguoiKy, search.LOGINNAME + "-" + search.USERNAME));
                    }
                    else
                    {
                        var data = EmrSigner.FirstOrDefault(o => o.LOGINNAME.ToUpper() == cboUserName.EditValue.ToString().ToUpper());

                        data.NUM_ORDER = GridSigner.Count + 1;

                        GridSigner.Add(data);

                        gridView2.BeginUpdate();
                        gridView2.GridControl.DataSource = GridSigner;
                        gridView2.EndUpdate();
                    }
                }
                txtLoginName.Text = "";
                cboUserName.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (EMR_SIGN_TEMP)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;

                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(EMR_SIGN_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);

                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(EMR_SIGN_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    txtSignTempCode.Text = data.SIGN_TEMP_CODE;
                    txtSignTempName.Text = data.SIGN_TEMP_NAME;

                    CommonParam param = new CommonParam();
                    EmrSignOrderFilter filter = new EmrSignOrderFilter();
                    filter.IS_ACTIVE = 1;
                    filter.SIGN_TEMP_ID = data.ID;
                    filter.ORDER_DIRECTION = "ASC";
                    filter.ORDER_FIELD = "NUM_ORDER";

                    var LstSignOrder = new BackendAdapter(param).Get<List<EMR_SIGN_ORDER>>(EmrRequestUriStore.EMR_SIGN_ORDER_GET, ApiConsumers.EmrConsumer, filter, param);

                    Inventec.Common.Logging.LogSystem.Info("LstSignOrder: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => LstSignOrder), LstSignOrder));

                    GridSigner = new List<EMR_SIGNER>();

                    if (LstSignOrder != null && LstSignOrder.Count >0)
                    {
                        foreach (var item in LstSignOrder)
                        {
                            EMR_SIGNER Signer = new EMR_SIGNER();
                            if (item.SIGNER_ID != null && item.IS_PATIENT_SIGN == null)
                            {
                                var check = EmrSigner.FirstOrDefault(o => o.ID == item.SIGNER_ID);
                                if (check != null)
                                {
                                    Signer = check;
                                }
                            }
                            else if (item.SIGNER_ID == null && item.IS_PATIENT_SIGN == 1)
                            {
                                Signer.USERNAME = ResourceMessage.BenhNhanKy;
                            }

                            Signer.NUM_ORDER = item.NUM_ORDER;

                            GridSigner.Add(Signer);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Info("GridSigner: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GridSigner), GridSigner));

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = GridSigner;
                    gridView2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    EMR_SIGN_TEMP data = (EMR_SIGN_TEMP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnlock);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : btnGEnable);

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EMR_SIGN_TEMP pData = (EMR_SIGN_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                       try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (EMR_SIGN_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;

                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            EMR_SIGN_TEMP success = new EMR_SIGN_TEMP();
            bool notHandler = false;
            try
            {

                EMR_SIGN_TEMP data = (EMR_SIGN_TEMP)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //EMR_SIGN_TEMP data1 = new EMR_SIGN_TEMP();
                    //data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_SIGN_TEMP>(EmrRequestUriStore.EMR_SIGN_TEMP_UN_LOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            EMR_SIGN_TEMP success = new EMR_SIGN_TEMP();
            bool notHandler = false;
            try
            {
                EMR_SIGN_TEMP data = (EMR_SIGN_TEMP)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //EMR_SIGN_TEMP data1 = new EMR_SIGN_TEMP();
                    //data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_SIGN_TEMP>(EmrRequestUriStore.EMR_SIGN_TEMP_LOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (EMR_SIGN_TEMP)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(EmrRequestUriStore.EMR_SIGN_TEMP_DELETE, ApiConsumers.EmrConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();

                            currentData = null;

                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();

            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                EmrSignTempSDO updateDTO = new EmrSignTempSDO();

                bool check = true;

                UpdateDTOFromDataForm(ref updateDTO, ref check);

                Inventec.Common.Logging.LogSystem.Info("dữ liệu đầu vào updateDTO:  "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                if (!check)
                {
                    return;
                }
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<EMR_SIGN_TEMP>(EmrRequestUriStore.EMR_SIGN_TEMP_CREATE_BY_SDO, ApiConsumers.EmrConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        SetDefaultValue();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<EMR_SIGN_TEMP>(EmrRequestUriStore.EMR_SIGN_TEMP_UPDATE_BY_SDO, ApiConsumers.EmrConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                    }
                }

                if (success)
                {
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void UpdateDTOFromDataForm(ref EmrSignTempSDO updateDTO, ref bool check)
        {
            try
            {
                updateDTO.SignTempId = currentData != null ? (long?)currentData.ID : null;
                updateDTO.SignTempCode = txtSignTempCode.Text;
                updateDTO.SignTempName = txtSignTempName.Text;
                updateDTO.SignOrders = new List<EmrSignOrderSDO>();

                if (GridSigner != null && GridSigner.Count >0)
                {
                    foreach (var item in GridSigner)
                    {
                        EmrSignOrderSDO SignOrderSDO = new EmrSignOrderSDO();
                        SignOrderSDO.IsPatientSign = item.ID > 0 ? false : true;
                        SignOrderSDO.NumOrder = item.NUM_ORDER.Value;
                        SignOrderSDO.SignerId = item.ID > 0 ? (long?)item.ID : null;
                        updateDTO.SignOrders.Add(SignOrderSDO);
                    }
                    check = true;
                }
                else 
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongCoDulieuThutuKy);
                    check = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider1, dxErrorProvider1);
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnAdditional_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdditional_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { 
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnGXoa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (EMR_SIGNER)gridView2.GetFocusedRow();

                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GridSigner.Remove(rowData);

                    if (GridSigner != null && GridSigner.Count >0)
                    {
                        foreach (var item in GridSigner)
                        {
                            item.NUM_ORDER = GridSigner.IndexOf(item) + 1;
                        }
                    }

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = GridSigner;
                    gridView2.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    EMR_SIGNER data = (EMR_SIGNER)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "UP")
                    {
                        e.RepositoryItem = btnGUp;

                    }

                    if (e.Column.FieldName == "DOWN")
                    {
                        e.RepositoryItem = btnGDown;

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void btnGUp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EMR_SIGNER)gridView2.GetFocusedRow();
                if (row != null)
                {
                    if (row.NUM_ORDER <= 1) return;

                    var changeRow = GridSigner.LastOrDefault(o => o.NUM_ORDER < row.NUM_ORDER);
                    if (changeRow != null)
                    {
                        UpdateSigner(changeRow, row);
                    }

                    GridSigner = GridSigner.OrderBy(o => o.NUM_ORDER).ToList();

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = null;
                    gridView2.GridControl.DataSource = GridSigner;
                    gridView2.EndUpdate();

                    InitComboServiceType();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSigner(EMR_SIGNER update, EMR_SIGNER original)
        {
            try
            {
                var loginname = update.LOGINNAME;
                var title = update.TITLE;
                var userName = update.USERNAME;
                var departmentCode = update.DEPARTMENT_CODE;
                var departmentName = update.DEPARTMENT_NAME;
                var Id = update.ID;

                update.ID = original.ID;
                update.LOGINNAME = original.LOGINNAME;
                update.TITLE = original.TITLE;
                update.USERNAME = original.USERNAME;
                update.DEPARTMENT_CODE = original.DEPARTMENT_CODE;
                update.DEPARTMENT_NAME = original.DEPARTMENT_NAME;

                original.ID = Id;
                original.TITLE = title;
                original.USERNAME = userName;
                original.LOGINNAME = loginname;
                original.DEPARTMENT_CODE = departmentCode;
                original.DEPARTMENT_NAME = departmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGDown_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EMR_SIGNER)gridView2.GetFocusedRow();
                if (row != null)
                {
                    //dòng cuối thì không thay đổi
                    if (row.NUM_ORDER >= GridSigner.Count) return;

                    var changeRow = GridSigner.FirstOrDefault(o => o.NUM_ORDER > row.NUM_ORDER);
                    if (changeRow != null)
                    {
                        UpdateSigner(changeRow, row);
                    }

                    GridSigner = GridSigner.OrderBy(o => o.NUM_ORDER).ToList();

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = null;
                    gridView2.GridControl.DataSource = GridSigner;
                    gridView2.EndUpdate();

                    InitComboServiceType();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboUserName.EditValue != null)
                    {
                        var data = EmrSigner.FirstOrDefault(o => o.LOGINNAME.ToUpper() == cboUserName.EditValue.ToString().ToUpper());
                        if (data != null)
                        {
                            txtLoginName.Text = data.LOGINNAME;
                            cboUserName.Properties.Buttons[1].Visible = true;

                            btnAdditional.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EMR_SIGNER pData = (EMR_SIGNER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "SignedBy")
                    {
                        if (pData.LOGINNAME == "1")
                        {
                            e.Value = pData.USERNAME;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(pData.LOGINNAME) && !string.IsNullOrEmpty(pData.USERNAME))
                            {
                                e.Value = pData.LOGINNAME + " - " + pData.USERNAME;
                            }
                            else
                            {
                                e.Value = pData.LOGINNAME + pData.USERNAME;
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

        private void cboUserName_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboUserName.EditValue = null;
                    txtLoginName.Text = "";
                    cboUserName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void txtLoginName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtLoginName.Text))
                    {
                        List<EMR_SIGNER> searchs = null;

                        var check = EmrSigner.Where(o => o.LOGINNAME.ToUpper().Contains(txtLoginName.Text.ToUpper())).ToList();

                        if (check != null && check.Count > 0)
                        {
                            searchs = (check.Count == 1) ? check : (check.Where(o => o.LOGINNAME.ToUpper() == txtLoginName.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtLoginName.Text = searchs[0].LOGINNAME;
                            cboUserName.EditValue = searchs[0].LOGINNAME;

                            btnAdditional.Focus();
                            e.Handled = true;
                        }
                        else
                        {
                            cboUserName.Focus();
                            cboUserName.ShowPopup();
                        }
                    }
                    else
                    {
                        cboUserName.Focus();
                        cboUserName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboUserName.EditValue != null)
                    {
                        var data = this.EmrSigner.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboUserName.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtLoginName.Text = data.LOGINNAME;
                            cboUserName.Properties.Buttons[1].Visible = true;

                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboUserName.ShowPopup();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (EMR_SIGN_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void txtSignTempName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSignTempCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSignTempName.Focus();
                    txtSignTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }




    }
}
