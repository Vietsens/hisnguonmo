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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraGrid.Views.Grid;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.HisServiceReqMaty.Resources;

namespace HIS.Desktop.Plugins.HisServiceReqMaty.HisServiceReqMaty
{
    public partial class frmHisServiceReqMaty : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        internal List<MateriTypeADO> lstMateriTypelADO = new List<MateriTypeADO>();
        internal List<MateriTypeADO> lstMateriTypelEmpt = new List<MateriTypeADO>();
        internal long sereServId;
        #endregion

        #region Construct
        public frmHisServiceReqMaty(Inventec.Desktop.Common.Modules.Module moduleData, long _sereServId)
		:base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.sereServId = _sereServId;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisServiceReqMaty_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceReqMaty.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServiceReqMaty.HisServiceReqMaty.frmHisServiceReqMaty).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LookupVattu.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.LookupVattu.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboVatTu.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.cboVatTu.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqId.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.grdColServiceReqId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.grdColServiceReqId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaterialTypeId.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.grdColMaterialTypeId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaterialTypeId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.grdColMaterialTypeId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAmount.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.grdColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.grdColAmount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisServiceReqMaty.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("lkServiceReqId", 0);
                dicOrderTabIndexControl.Add("lkMaterialTypeId", 1);
                dicOrderTabIndexControl.Add("spAmount", 2);


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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboMaterialTypeId();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboServiceReqId()
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("SERVICE_REQ_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("SERVICE_REQ_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_REQ_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(lkServiceReqId, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboMaterialTypeId()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisSereServMatyViewFilter filter = new HisSereServMatyViewFilter();
                filter.SERE_SERV_ID = sereServId;
                var ListFormMaterialType = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>>(HisRequestUriStore.MOSHIS_SERV_SERE_MATY_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                //if (ListFormMaterialType != null && ListFormMaterialType.Count > 0)
                //{
                //var ListFormMaterialType = gridviewFormList.DataSource as List<V_HIS_SERE_SERV_MATY>;
                List<long> materialTypeIds = ListFormMaterialType.Select(o => o.MATERIAL_TYPE_ID).ToList();
                var MaterialType = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => !materialTypeIds.Contains(o.ID)).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(LookupVattu, MaterialType, controlEditorADO);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE"); ;

                LoadPaging(new CommonParam(0, pagingSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pagingSize,this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>> apiResult = null;
                HisSereServMatyViewFilter filter = new HisSereServMatyViewFilter();
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "DESC";
                filter.SERE_SERV_ID = sereServId;
                SetFilterNavBar(ref filter);
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>>(HisRequestUriStore.MOSHIS_SERV_SERE_MATY_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>)apiResult.Data;
                    if (data != null)
                    {
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisSereServMatyViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY pData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        if (e.ListSourceRowIndex != null)
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage + ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                        }
                        else
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }

                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
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

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
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

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    this.currentData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>)[dnNavigation.Position];
            //    if (this.currentData != null)
            //    {
            //        ChangedDataRow(this.currentData);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    ////Disable nút sửa nếu dữ liệu đã bị khóa
                    //btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY data)
        {
            try
            {
                if (data != null)
                {
                    //                    lkServiceReqId.EditValue = data.SERVICE_REQ_ID;
                    //lkMaterialTypeId.EditValue = data.MATERIAL_TYPE_ID;
                    //spAmount.EditValue = data.AMOUNT;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
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

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServMatyViewFilter filter = new HisSereServMatyViewFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>>(HisRequestUriStore.MOSHIS_SERV_SERE_MATY_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Button handler
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
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
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_SERV_SERE_MATY_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                //this.ActionType = GlobalVariables.ActionAdd;
                //EnableControlChanged(this.ActionType);
                //positionHandle = -1;
                //Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                //ResetFormData();
                //SetFocusEditor();

                //if (gridControlRegMaty.DataSource == null)
                //{
                //    List<MateriTypeADO> maty = new List<MateriTypeADO>();
                //    CommonParam param = new CommonParam();
                //    HisServiceReqMatyFilter ReqmatyFilter = new HisServiceReqMatyFilter();
                //    MateriTypeADO matyTemps = new MateriTypeADO();
                //    var data = new BackendAdapter(param)
                //            .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, ReqmatyFilter, param);
                //    maty.Add(matyTemps);
                //    gridControlRegMaty.DataSource = maty;
                //}
                lstMateriTypelEmpt = new List<MateriTypeADO>();
                List<MateriTypeADO> maty = new List<MateriTypeADO>();
                MateriTypeADO matyTemps = new MateriTypeADO();
                lstMateriTypelEmpt.Add(matyTemps);
                gridControlRegMaty.DataSource = lstMateriTypelEmpt;
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
                //if (!btnEdit.Enabled && !btnAdd.Enabled)
                //    return;
                gridviewReqMaty.PostEditor();
                //if (gridviewReqMaty.VisibleColumns[0] == null)
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn loại vật tư. Vui lòng kiểm tra lại", "Thông báo");
                //    return;
                //}
                GetError(gridviewReqMaty.FocusedRowHandle, gridviewReqMaty.FocusedColumn);
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();

                List<MateriTypeADO> sereServMatyFromGrids = gridControlRegMaty.DataSource as List<MateriTypeADO>;



                if (sereServMatyFromGrids != null && sereServMatyFromGrids.Count > 0)
                {
                    var Check = sereServMatyFromGrids.Where(o => o.MATERIAL_TYPE_ID == 0 || o.AMOUNT < 1).ToList();
                    if (Check != null && Check.Count > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapDuThongTin);
                        return;
                    }
                    var serviceReqMatyGroups = sereServMatyFromGrids.Where(o => o.MATERIAL_TYPE_ID != 0 && o.AMOUNT >= 1)
                        .GroupBy(o => o.MATERIAL_TYPE_ID).ToList();
                    List<V_HIS_SERE_SERV_MATY> serviceReqMatys = new List<V_HIS_SERE_SERV_MATY>();
                    foreach (var serviceReqMatyGroup in serviceReqMatyGroups)
                    {
                        V_HIS_SERE_SERV_MATY serviceReqMaty = new V_HIS_SERE_SERV_MATY();
                        serviceReqMaty.MATERIAL_TYPE_ID = serviceReqMatyGroup.First().MATERIAL_TYPE_ID;
                        serviceReqMaty.AMOUNT = serviceReqMatyGroup.Sum(o => o.AMOUNT);
                        serviceReqMaty.SERE_SERV_ID = sereServId;
                        serviceReqMatys.Add(serviceReqMaty);
                    }
                    if (serviceReqMatys.FirstOrDefault().MATERIAL_TYPE_ID == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaChonLoaiVatTu);
                        return;
                    }
                    if (serviceReqMatys != null && serviceReqMatys.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<List<V_HIS_SERE_SERV_MATY>>("api/HisSereServMaty/CreateList", ApiConsumers.MosConsumer, serviceReqMatys, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            //ResetFormData();
                            btnCancel_Click(null, null);
                            InitComboMaterialTypeId();
                        }
                    }
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY currentDTO)
        {
            try
            {
                //                

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                //               ValidationSingleControl(lkServiceReqId);
                //ValidationSingleControl(lkMaterialTypeId);
                //ValidationSingleControl(spAmount);

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
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void ValidateGridLookupWithSpinEdit(GridLookUpEdit cbo, SpinEdit textEdit)
        //{
        //    try
        //    {
        //        ControlEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
        //        validRule.txtTextEdit = textEdit;
        //        validRule.cbo = cbo;
        //        validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        validRule.ErrorType = ErrorType.Warning;
        //        dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToGridControl();
                initGridRegMaty();
                FillDataToControlsForm();

                //Load du lieu



                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                //InitTabIndex();

                //Set validate rule
                // ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSearch.Enabled)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
            //    {
            //        btnEdit_Click(null, null);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnCancel.Enabled)
                    btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (txtKeyword.Enabled)
                {
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void initGridRegMaty()
        {
            lstMateriTypelEmpt = new List<MateriTypeADO>();
            MateriTypeADO matyTemps = new MateriTypeADO();
            matyTemps.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
            //var data = new BackendAdapter(param)
            //        .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, ReqmatyFilter, param);
            lstMateriTypelEmpt.Add(matyTemps);
            gridControlRegMaty.DataSource = lstMateriTypelEmpt;
        }

        private void gridviewReqMaty_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "HIS_MATERIAL_TYPE")
                {
                    gridviewReqMaty.ShowEditor();
                    ((LookUpEdit)gridviewReqMaty.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboVatTu_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                gridviewReqMaty.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                gridviewReqMaty.FocusedColumn = gridviewReqMaty.VisibleColumns[1];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewReqMaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MateriTypeADO data_ServiceSDO = (MateriTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data_ServiceSDO != null)
                    {
                        //if (e.Column.FieldName == "PRICE_DISPLAY")
                        //{
                        //    if (data_ServiceSDO.PATIENT_TYPE_ID != 0)
                        //    {
                        //        var data_ServicePrice = EXE.APP.GlobalStore.ListServicePaty.Where(o => o.SERVICE_ID == data_ServiceSDO.SERVICE_ID && o.PATIENT_TYPE_ID == data_ServiceSDO.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                        //        if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                        //        {
                        //            e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound((data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO)));
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewReqMaty_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MateriTypeADO data = null;
                if (e.RowHandle > 0)
                {
                    data = (MateriTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.Column.FieldName == "THEM")
                {
                    if (data.Action == GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = btnThem;
                    }
                    else if (data.Action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = btnDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewReqMaty_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MateriTypeADO data = view.GetFocusedRow() as MateriTypeADO;
                if (view.FocusedColumn.FieldName == "HIS_MATERIAL_ID" && view.ActiveEditor is LookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        //FillDataIntoUserCombo(data, editor);
                        editor.EditValue = data.MATERIAL_TYPE_ID;

                        if (editor.EditValue == null)//xemlai...
                        {

                        }
                        editor.ShowPopup();
                    }
                    string error = GetError(gridviewReqMaty.FocusedRowHandle, gridviewReqMaty.FocusedColumn);
                    if (error == string.Empty) return;
                    gridviewReqMaty.SetColumnError(gridviewReqMaty.FocusedColumn, error);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnThem_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MateriTypeADO metiryTemp = new MateriTypeADO();
                metiryTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                lstMateriTypelEmpt.Add(metiryTemp);
                gridControlRegMaty.DataSource = null;
                gridControlRegMaty.DataSource = lstMateriTypelEmpt;
                //gridviewReqMaty.FocusedColumn = gridviewReqMaty.VisibleColumns[0];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var currentReqMaty = (MateriTypeADO)gridviewReqMaty.GetFocusedRow();
                var reqMatys = gridviewReqMaty.DataSource as List<MateriTypeADO>;
                if (currentReqMaty != null)
                {

                    if (reqMatys.Count > 0)
                    {
                        reqMatys.Remove(currentReqMaty);
                        gridControlRegMaty.DataSource = null;
                        gridControlRegMaty.DataSource = reqMatys;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spEdit_Enter(object sender, EventArgs e)
        {

        }

        private void spEdit_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridviewReqMaty.FocusedColumn = gridviewReqMaty.VisibleColumns[2];
                    btnThem_ButtonClick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnThem_Enter(object sender, EventArgs e)
        {

        }

        private void LookupVattu_Click(object sender, EventArgs e)
        {

        }

        private void LookupVattu_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                gridviewReqMaty.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                gridviewReqMaty.FocusedColumn = gridviewReqMaty.VisibleColumns[1];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LookupVattu_Popup(object sender, EventArgs e)
        {
            //GridLookUpEdit edit = sender as GridLookUpEdit;
            //GridView gridView = edit.Properties.View as GridView;
            //FieldInfo fi = gridView.GetType().GetField("extraFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            //BinaryOperator op1 = new BinaryOperator("MATERIAL_TYPE_CODE", edit.AutoSearchText + "%", BinaryOperatorType.Like);
            //BinaryOperator op2 = new BinaryOperator("MATERIAL_TYPE_NAME", edit.AutoSearchText + "%", BinaryOperatorType.Like);
            //string filterCondition = new GroupOperator(GroupOperatorType.Or, new CriteriaOperator[] { op1, op2 }).ToString();
            //fi.SetValue(gridView, filterCondition);

            //MethodInfo mi = gridView.GetType().GetMethod("ApplyColumnsFilterEx", BindingFlags.NonPublic | BindingFlags.Instance);
            //mi.Invoke(gridView, null);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LookupVattu_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridviewReqMaty.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                    gridviewReqMaty.FocusedColumn = gridviewReqMaty.VisibleColumns[1];
                }
                else
                {
                    LookupVattu.View.ShowEditForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LookupVattu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridviewReqMaty.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                    gridviewReqMaty.FocusedColumn = gridviewReqMaty.VisibleColumns[1];
                }
                else
                {
                    //gridviewReqMaty_ShowingEditor(sender,e);
                    //LookupVattu.View.();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewReqMaty_Click(object sender, EventArgs e)
        {

        }

        private void gridControlRegMaty_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (gridviewReqMaty.FocusedColumn.RealColumnEdit is RepositoryItemGridLookUpEdit)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        GridLookUpEdit edit = gridviewReqMaty.ActiveEditor as GridLookUpEdit;
                        if (edit == null)
                        {
                            gridviewReqMaty.ShowEditor();
                            edit = gridviewReqMaty.ActiveEditor as GridLookUpEdit;
                        }

                        edit.ShowPopup();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridviewReqMaty_RowCellClick(object sender, RowCellClickEventArgs e)
        {

        }

        private void LookupVattu_Click_1(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                if (edit != null)
                {
                    edit.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public string GetError(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                HIS.Desktop.Plugins.HisServiceReqMaty.MateriTypeADO data = (HIS.Desktop.Plugins.HisServiceReqMaty.MateriTypeADO)gridviewReqMaty.GetRow(rowHandle);
                if (column.FieldName == "AMOUNT")
                {

                    if (data == null)
                        return string.Empty;
                    if (data.AMOUNT < 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không cho phép nhập nhỏ hơn 1", "Thông báo");
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
        }
    }
}
