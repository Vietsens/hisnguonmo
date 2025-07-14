using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Common.WebApiClient;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using System.Resources;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisIcdSkinPathology.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Security.Cryptography;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.HisIcdSkinPathology.HisIcdSkinPathology
{
    public partial class frmHisIcdSkinpathology : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string,int> dicOrderTabIndexControl = new Dictionary<string,int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_ICD_SKIN_PATHOLOGY currentData;

        public frmHisIcdSkinpathology(Inventec.Desktop.Common.Modules.Module moduleData) : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                pagingGrid = new PagingGrid();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmHisIcdSkinpathology_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void Show()
        {
            FillDataToControl();
            SetDefaultFocus();
            EnableControlChanged(this.ActionType);
            SetDefaultValue();
            InitTabIndex();
            SetResource();
            ValidateForm();
            btnAdd.Enabled = true;
        }

        private void ValidateForm()
        {
            try
            {
                ValidationControlTextEditCode();
                ValidationControlTextEditName();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditName()
        {
            try
            {
                HIS.Desktop.Plugins.HisIcdSkinPathology.Validation.ValidationControlTextEditName validRule = new HIS.Desktop.Plugins.HisIcdSkinPathology.Validation.ValidationControlTextEditName();
                validRule.txtTen = txtTen;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTen, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditCode()
        {
            try
            {
                HIS.Desktop.Plugins.HisIcdSkinPathology.Validation.ValidationControlTextEditCode validRule = new HIS.Desktop.Plugins.HisIcdSkinPathology.Validation.ValidationControlTextEditCode();
                validRule.txtMa = txtMa;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtMa, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetResource()
        {
            try
            {
                Resources.ResourcesIcdSkinPathologyManager.Resource = new ResourceManager("HIS.Desktop.Plugins.HisIcdSkinPathology.Resources.Lang", typeof(HIS.Desktop.Plugins.HisIcdSkinPathology.HisIcdSkinPathology.frmHisIcdSkinpathology).Assembly);
                //layout
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.LayoutControl1.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.LayoutControl2.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.lcEditorInfo.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                //button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.btnAdd.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.btnEdit.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.btnReset.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.btnSearch.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                //bar button
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.bbtnAdd.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.bbtnEdit.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.bbtnReset.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.bbtnSearch.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.barButtonItem6.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.barButtonItem6.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());

                //column
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridColumn1.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridColumn2.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridMa.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridMa.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridTen.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridTen.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridColumn5.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridColumn6.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridColumn7.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridColumn8.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.gridColumn9.Caption", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                //input box
                this.txtMa.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.txtMa.Properties.NullValuePrompt", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.txtTen.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.txtTen.Properties.NullValuePrompt", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.txtSearch.Properties.NullValuePrompt", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisIcdSkinpathology.Text", Resources.ResourcesIcdSkinPathologyManager.Resource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return;
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
                LogSystem.Warn(ex);
            }
            return;
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtSearch.Text = string.Empty;
                txtMa.Text = string.Empty;
                txtTen.Text = string.Empty;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
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
                LogSystem.Debug(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if(ucPaging1.pagingGrid != null)
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
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(Object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY>> apiResult = null;
                HisIcdSkinPathologyFilter filter = new HisIcdSkinPathologyFilter();
                SetFilterNavbar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginDataUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY>>(HisIcdSkinPathologyRequestUriStore.HIS_ICD_SKIN_PATHOLOGY_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if(apiResult != null && apiResult.Data != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY>)apiResult.Data;
                    if(data!= null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();
                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void SetFilterNavbar(ref HisIcdSkinPathologyFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY)gridView1.GetFocusedRow();
                if (rowData != null)                 
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY data)
        {
            try
            {
                if (data != null)
                {
                    FillDataEditorControl(data);
                    //txtMa = currentData.ICD_SKIN_PATHOLOGY_CODE;
                    //txtTen = currentData.ICD_SKIN_PATHOLOGY_NAME;
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY data)
        {
            try
            {
                if (data != null)
                {
                    txtMa.Text = data.ICD_SKIN_PATHOLOGY_CODE;
                    txtTen.Text = data.ICD_SKIN_PATHOLOGY_NAME;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY)gridView1.GetFocusedRow();
                    if(rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if(e.RowHandle >= 0)
                {
                    HIS_ICD_SKIN_PATHOLOGY data = (HIS_ICD_SKIN_PATHOLOGY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if(e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnlock);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if(e.RowHandle >= 0)
            {
                HIS_ICD_SKIN_PATHOLOGY data = (HIS_ICD_SKIN_PATHOLOGY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if(e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
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
                LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if(!btnEdit.Enabled && !btnAdd.Enabled)
                {
                    return;
                }
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_ICD_SKIN_PATHOLOGY updateDTO = new HIS_ICD_SKIN_PATHOLOGY();
                if(this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY>(HisIcdSkinPathologyRequestUriStore.HIS_ICD_SKIN_PATHOLOGY_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY>(HisIcdSkinPathologyRequestUriStore.HIS_ICD_SKIN_PATHOLOGY_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
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
                MessageManager.Show(this,param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach(DevExpress.XtraLayout.LayoutControlItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            formatFrm.ResetText();
                            formatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
                txtMa.Text = "";
                txtTen.Text = "";
                txtSearch.Text = "";
                txtMa.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_ICD_SKIN_PATHOLOGY currentDTO)
        {
            try
            {
                currentDTO.ICD_SKIN_PATHOLOGY_CODE = txtMa.Text.Trim();
                currentDTO.ICD_SKIN_PATHOLOGY_NAME = txtTen.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void LoadCurrent(long currentID, ref HIS_ICD_SKIN_PATHOLOGY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisIcdSkinPathologyFilter filter = new HisIcdSkinPathologyFilter();
                filter.ID = currentID;
                currentDTO = new BackendAdapter(param).Get<List<HIS_ICD_SKIN_PATHOLOGY>>(HisIcdSkinPathologyRequestUriStore.HIS_ICD_SKIN_PATHOLOGY_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex); 
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
                LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if(e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY)gridView1.GetFocusedRow();
                    if(rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if(e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_ICD_SKIN_PATHOLOGY)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_ICD_SKIN_PATHOLOGY pdata = (HIS_ICD_SKIN_PATHOLOGY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pdata.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if(e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Error(ex);
                        }
                    }
                    else if(e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pdata.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pdata.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Error(ex);
                        }
                    }
                }
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if(this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }

        private void txtMa_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    txtTen.Focus();
                    txtTen.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_ICD_SKIN_PATHOLOGY success = new HIS_ICD_SKIN_PATHOLOGY();
            bool notHandler = false;
            try
            {
                HIS_ICD_SKIN_PATHOLOGY data = (HIS_ICD_SKIN_PATHOLOGY)gridView1.GetFocusedRow();
                if(MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_ICD_SKIN_PATHOLOGY>(HisIcdSkinPathologyRequestUriStore.HIS_ICD_SKIN_PATHOLOGY_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
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
                LogSystem.Error(ex);

            }
        }
        private void btnGUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_ICD_SKIN_PATHOLOGY success = new HIS_ICD_SKIN_PATHOLOGY();
            bool notHandler = false;
            try
            {
                HIS_ICD_SKIN_PATHOLOGY data = (HIS_ICD_SKIN_PATHOLOGY)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_ICD_SKIN_PATHOLOGY>(HisIcdSkinPathologyRequestUriStore.HIS_ICD_SKIN_PATHOLOGY_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
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
                LogSystem.Error(ex);
            }
        }

    }
}
