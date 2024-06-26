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
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.HisHeinServiceBhyt.HisHeinServiceBhyt
{
    public partial class frmHisHeinServiceBhyt : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT heinServiceBhyt;
        DelegateSelectData delegateSelect;
        ToolTipControlInfo lastInfoForImp = null;
        DevExpress.XtraGrid.Columns.GridColumn lastColumnForImp = null;
        int lastRowHandleForImp = -1;
        #endregion

        #region Construct
        public frmHisHeinServiceBhyt(Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelect)
		:base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;
                if (_delegateSelect != null)
                {
                    this.delegateSelect = _delegateSelect;
                }

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
        private void frmHisHeinServiceBhyt_Load(object sender, EventArgs e)
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
            Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisHeinServiceBhyt.Resources.Lang", typeof(HIS.Desktop.Plugins.HisHeinServiceBhyt.HisHeinServiceBhyt.frmHisHeinServiceBhyt).Assembly);

            ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColHeinServiceBhytCode.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColHeinServiceBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColHeinServiceBhytCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColHeinServiceBhytCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColHeinServiceBhytName.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColHeinServiceBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColHeinServiceBhytName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColHeinServiceBhytName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColHeinOrder.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColHeinOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColHeinOrder.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColHeinOrder.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColIsInKtcFee.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColIsInKtcFee.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColIsInKtcFee.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColIsInKtcFee.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColAtc.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColAtc.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColAtc.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColAtc.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lkHeinServiceId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.lkHeinServiceId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciHeinServiceId.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.lciHeinServiceId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciHeinServiceBhytCode.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.lciHeinServiceBhytCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciHeinServiceBhytName.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.lciHeinServiceBhytName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.lciHeinOrder.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.lciHeinOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.Text = Inventec.Common.Resource.Get.Value("frmHisHeinServiceBhyt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                dicOrderTabIndexControl.Add("lkHeinServiceId", 0);
                dicOrderTabIndexControl.Add("txtHeinServiceBhytCode", 1);
                dicOrderTabIndexControl.Add("txtHeinServiceBhytName", 2);
                dicOrderTabIndexControl.Add("txtHeinOrder", 3);
                dicOrderTabIndexControl.Add("chkIsInKtcFee", 4);
                dicOrderTabIndexControl.Add("txtAtc", 5);


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
                InitComboHeinServiceId();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboHeinServiceId()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisHeinServiceViewFilter filter = new HisHeinServiceViewFilter();
                Inventec.Common.Logging.LogSystem.Warn("bat dau goi api HisHeinService/GetView");
                var heinServices = new BackendAdapter(param).Get<List<V_HIS_HEIN_SERVICE>>("/api/HisHeinService/GetView", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Warn("ket thuc goi api HisHeinService/GetView");
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkHeinServiceId, heinServices, controlEditorADO);
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
                Inventec.Common.Logging.LogSystem.Warn("bat dau khoi tao gridControl");
                LoadPaging(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn("ket thuc khoi tao gridControl");
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>> apiResult = null;
                HisHeinServiceBhytFilter filter = new HisHeinServiceBhytFilter();
                SetFilterNavBar(ref filter);
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>>(HisRequestUriStore.MOSHIS_HEIN_SERVICE_BHYT_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
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

        private void SetFilterNavBar(ref HisHeinServiceBhytFilter filter)
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT pData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "IS_IN_KTC_FEE_DISPLAY")
                    {
                        e.Value = imageCollection1.Images[0];
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)gridviewFormList.GetFocusedRow();
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
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT data)
        {
            try
            {
                if (data != null)
                {
                    lkHeinServiceId.EditValue = data.HEIN_SERVICE_ID;
                    txtHeinServiceBhytCode.Text = data.HEIN_SERVICE_BHYT_CODE;
                    txtHeinServiceBhytName.Text = data.HEIN_SERVICE_BHYT_NAME;
                    txtHeinOrder.Text = data.HEIN_ORDER;
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
                txtHeinServiceBhytCode.Focus();
                txtHeinServiceBhytCode.SelectAll();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHeinServiceBhytFilter filter = new HisHeinServiceBhytFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>>(HisRequestUriStore.MOSHIS_HEIN_SERVICE_BHYT_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_HEIN_SERVICE_BHYT_DELETE, ApiConsumers.MosConsumer, rowData, param);
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
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
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
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT updateDTO = new MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    heinServiceBhyt = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>(HisRequestUriStore.MOSHIS_HEIN_SERVICE_BHYT_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (heinServiceBhyt != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                        RefeshDataAfterSave();
                    }
                }
                else
                {
                    heinServiceBhyt = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>(HisRequestUriStore.MOSHIS_HEIN_SERVICE_BHYT_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (heinServiceBhyt != null)
                    {
                        success = true;
                        UpdateRowDataAfterEdit(heinServiceBhyt);
                        RefeshDataAfterSave();
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

        void RefeshDataAfterSave()
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(heinServiceBhyt);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT currentDTO)
        {
            try
            {
                if (lkHeinServiceId.EditValue != null) currentDTO.HEIN_SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkHeinServiceId.EditValue ?? "0").ToString());
                currentDTO.HEIN_SERVICE_BHYT_CODE = txtHeinServiceBhytCode.Text.Trim();
                currentDTO.HEIN_SERVICE_BHYT_NAME = txtHeinServiceBhytName.Text.Trim();
                currentDTO.HEIN_ORDER = txtHeinOrder.Text.Trim();
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
                ValidationSingleControl(lkHeinServiceId);
                ValidationSingleControl(txtHeinServiceBhytCode);
                ValidationSingleControl(txtHeinServiceBhytName);
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
                try
                {
                    if (e.Info == null && e.SelectedControl == gridControlFormList)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = gridControlFormList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                        GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                        if (info.InRowCell)
                        {
                            if (lastRowHandleForImp != info.RowHandle || lastColumnForImp != info.Column)
                            {
                                lastColumnForImp = info.Column;
                                lastRowHandleForImp = info.RowHandle;

                                string text = "";
                                if (info.Column.FieldName == "IS_IN_KTC_FEE_DISPLAY")
                                {
                                    text = "KTC";
                                }
                                lastInfoForImp = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                            }
                            e.Info = lastInfoForImp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
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
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

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
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
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
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
