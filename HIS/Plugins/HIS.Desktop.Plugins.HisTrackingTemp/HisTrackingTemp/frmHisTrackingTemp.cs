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
using HIS.Desktop.ADO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisTrackingTemp.HisTrackingTemp
{
    public partial class frmHisTrackingTemp : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP currentData;
        TrackingTempADO trackingTempADO;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_EMPLOYEE employ = new HIS_EMPLOYEE();
        #endregion

        #region Construct
        public frmHisTrackingTemp(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public frmHisTrackingTemp(Inventec.Desktop.Common.Modules.Module moduleData, HIS_TRACKING_TEMP HIS_TRACKING_TEMP)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.trackingTempADO = new TrackingTempADO();
                this.trackingTempADO.HIS_TRACKING_TEMP = HIS_TRACKING_TEMP;
                //this.trackingTempADO.DelegateSelectData = (DelegateSelectData)NullSelectData;
                this.trackingTempADO.IsCreatorOrPublic = true;
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

        private void NullSelectData(object hisTrackingTemp)
        {

        }

        public frmHisTrackingTemp(Inventec.Desktop.Common.Modules.Module moduleData, TrackingTempADO trackingTempADO)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.trackingTempADO = trackingTempADO;
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
        private void frmHisTrackingTemp_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisTrackingTemp.Resources.Lang", typeof(HIS.Desktop.Plugins.HisTrackingTemp.HisTrackingTemp.frmHisTrackingTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.gridColumnDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcolGSelect.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.gcolGSelect.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTrackingTempName.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColTrackingTempCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTrackingTempName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColTrackingTempCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTrackingTempName.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColTrackingTempName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTrackingTempName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColTrackingTempName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColContent.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColContent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColContent.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColContent.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMedicalInstruction.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColMedicalInstruction.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMedicalInstruction.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColMedicalInstruction.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkGIsPublic.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.chkGIsPublic.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSelect.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.btnSelect.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBMI.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.lciBMI.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBSA.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.lciBSA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTrackingTempCode.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.lciTrackingTempCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTrackingTempName.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.lciTrackingTempName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsPublic.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.chkIsPublic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsInDepartment.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingTemp.chkIsInDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                if (this.moduleData.RoomId != 0)
                {
                    chkIsInDepartment.Enabled = true;
                }
                else 
                {
                    chkIsInDepartment.Enabled = false;
                    chkIsInDepartment.ToolTip = "Để sử dụng tính năng này, bạn cần mở chức năng 'Tờ điều trị mẫu' từ phòng làm việc";
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
                dicOrderTabIndexControl.Add("txtTrackingTempCode", 0);
                dicOrderTabIndexControl.Add("chkIsPublic", 1);
                dicOrderTabIndexControl.Add("chkIsInDepartment", 2);
                dicOrderTabIndexControl.Add("txtTrackingTempName", 3);
                dicOrderTabIndexControl.Add("meContent", 4);
                dicOrderTabIndexControl.Add("meMedicalInstruction", 5);
                dicOrderTabIndexControl.Add("spinPulse", 6);
                dicOrderTabIndexControl.Add("spinBloodPressureMax", 7);
                dicOrderTabIndexControl.Add("spinBloodPressureMin", 8);
                dicOrderTabIndexControl.Add("spinTemperature", 9);
                dicOrderTabIndexControl.Add("spinBreathRate", 10);
                dicOrderTabIndexControl.Add("spinHeight", 11);
                dicOrderTabIndexControl.Add("spinWeight", 12);
                dicOrderTabIndexControl.Add("spinChest", 13);
                dicOrderTabIndexControl.Add("spinBelly", 14);

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
                if (this.trackingTempADO.HIS_TRACKING_TEMP != null)
                {
                    FillDataToEditorControl(this.trackingTempADO.HIS_TRACKING_TEMP);
                    if (this.trackingTempADO.HIS_TRACKING_TEMP.ID != 0)
                    {
                        this.ActionType = GlobalVariables.ActionEdit;
                    }
                    else
                    {
                        this.ActionType = GlobalVariables.ActionAdd;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>> apiResult = null;
                HisTrackingTempFilter filter = new HisTrackingTempFilter();
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "ACS";
                SetFilterNavBar(ref filter);
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>>(RequestUriStore.HIS_TRACKING_TEMP_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>)apiResult.Data;
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

        private void SetFilterNavBar(ref HisTrackingTempFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                if (!IsAdmin())
                {
                    if (this.trackingTempADO != null && this.trackingTempADO.IsCreatorOrPublic.HasValue && this.trackingTempADO.IsCreatorOrPublic.Value)
                    {
                        //filter.IS_PUBLIC = 1;
                        filter.DATA_DOMAIN_FILTER = true;
                    }
                }
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP pData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_PUBLIC_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PUBLIC == 1 ? "1" : "0";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi khi lay HIS_TRACKING_TEMP.IS_PUBLIC", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_IN_DEPARTMENT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PUBLIC_IN_DEPARTMENT == 1 ? "1" : "0";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi khi lay HIS_TRACKING_TEMP.IS_IN_DEPARTMENT_STR", ex);
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
        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                short IS_ACTIVE = short.Parse((view.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "0").ToString());
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnlock);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                if (rowData != null &&
                    (this.currentData == null || (this.currentData != null && rowData.ID == this.currentData.ID)))
                {
                    this.currentData = rowData;
                    ChangedDataRow(this.currentData);

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
                    this.currentData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                    if (this.currentData != null)
                    {
                        ChangedDataRow(this.currentData);

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
                this.currentData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>)[dnNavigation.Position];
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP data)
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    txtTrackingTempCode.Text = data.TRACKING_TEMP_CODE;
                    txtTrackingTempName.Text = data.TRACKING_TEMP_NAME;
                    chkIsPublic.Checked = data.IS_PUBLIC == 1 ? true : false;
                    meContent.Text = data.CONTENT;
                    meMedicalInstruction.Text = data.MEDICAL_INSTRUCTION;
                    spinPulse.EditValue = data.PULSE == 0 ? null : data.PULSE;
                    spinTemperature.EditValue = data.TEMPERATURE == 0 ? null : data.TEMPERATURE;
                    spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX == 0 ? null : data.BLOOD_PRESSURE_MAX;
                    spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN == 0 ? null : data.BLOOD_PRESSURE_MIN;
                    spinBreathRate.EditValue = data.BREATH_RATE == 0 ? null : data.BREATH_RATE;
                    spinWeight.EditValue = data.WEIGHT == 0 ? null : data.WEIGHT;
                    spinHeight.EditValue = data.HEIGHT == 0 ? null : data.HEIGHT;
                    spinChest.EditValue = data.CHEST == 0 ? null : data.CHEST;
                    spinBelly.EditValue = data.BELLY == 0 ? null : data.BELLY;
                    chkIsInDepartment.Checked = data.IS_PUBLIC_IN_DEPARTMENT == 1 ? true : false;
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
                txtTrackingTempCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ResetControlText()
        {
            try
            {
                txtTrackingTempCode.Text = "";
                txtTrackingTempName.Text = "";
                chkIsPublic.Checked = false;
                chkIsInDepartment.Checked = false;
                meContent.Text = "";
                meMedicalInstruction.Text = "";
                spinPulse.EditValue = null;
                spinBloodPressureMax.EditValue = null;
                spinBloodPressureMin.EditValue = null;
                spinTemperature.EditValue = null;
                spinBreathRate.EditValue = null;
                spinHeight.EditValue = null;
                spinWeight.EditValue = null;
                spinChest.EditValue = null;
                spinBelly.EditValue = null;
                lcBMI.Text = "";
                lcBSA.Text = "";
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    chkIsPublic.Text = "Công khai toàn viện";
                    chkIsInDepartment.Text = "Công khai trong khoa";
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTrackingTempFilter filter = new HisTrackingTempFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>>(RequestUriStore.HIS_TRACKING_TEMP_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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

        private void btnGSelect_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    ProcessSlected(rowData);
                    this.Close();
                }
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

                CommonParam param = new CommonParam();
                if (MessageBox.Show("Bạn có muốn bỏ xóa dữ liệu không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        if (IsAllowEdit(rowData.CREATOR))
                        {

                            success = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_TRACKING_TEMP_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                            if (success)
                            {
                                FillDataToGridControl();
                            }
                        }
                        else
                        {
                            param.Messages.Add(" - Không được phép xóa. Chưa có quyền! ");
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                this.currentData = null;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                ResetControlText();
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                UpdateDTOFromDataForm(ref currentData);
                ProcessSlected(this.currentData);
                WaitingManager.Hide();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSlected(MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP data)
        {
            try
            {
                //if (this.selectData != null)
                //{
                //    this.selectData(data);
                //}
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
                MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP updateDTO = new MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                Inventec.Common.Logging.LogSystem.Info("updateDTO: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>(RequestUriStore.HIS_TRACKING_TEMP_CREATE, ApiConsumers.MosConsumer, updateDTO, param);

                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetControlText();
                        ResetFormData();
                    }
                }
                else
                {
                    if (IsAllowEdit(updateDTO.CREATOR))
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>(RequestUriStore.HIS_TRACKING_TEMP_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            UpdateRowDataAfterEdit(resultData);
                        }
                    }
                    else
                    {
                        param.Messages.Add(" - Không được phép sửa. Chưa có quyền! ");
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

        private bool IsAllowEdit(string creator)
        {
            bool result = false;
            try
            {
                if (this.employ != null)
                {
                    if (this.employ.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        return true;
                    }
                    else
                    {
                        return creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }
                }
                else
                {
                    return creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
            return result;
        }




        private bool IsAdmin()
        {
            bool result = false;
            try
            {
                if (this.employ != null && this.employ.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    return true;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_TRACKING_TEMP currentDTO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.employ), this.employ));
                currentDTO.TRACKING_TEMP_CODE = txtTrackingTempCode.Text.Trim();

                currentDTO.IS_PUBLIC = (short)(this.chkIsPublic.Checked ? 1 : 0);
                currentDTO.IS_PUBLIC_IN_DEPARTMENT = (short)(this.chkIsInDepartment.Checked ? 1 : 0);
                currentDTO.TRACKING_TEMP_NAME = txtTrackingTempName.Text.Trim();
                currentDTO.CONTENT = meContent.Text.Trim();
                currentDTO.MEDICAL_INSTRUCTION = meMedicalInstruction.Text.Trim();

                if (!string.IsNullOrEmpty(spinPulse.Text))
                {
                    currentDTO.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.Value.ToString());
                }
                else
                {
                    currentDTO.PULSE = null;
                }

                if (!string.IsNullOrEmpty(spinBloodPressureMax.Text))
                {
                    currentDTO.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.Value.ToString());
                }
                else
                {
                    currentDTO.BLOOD_PRESSURE_MAX = null;
                }

                if (!string.IsNullOrEmpty(spinBloodPressureMin.Text))
                {
                    currentDTO.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.Value.ToString());
                }
                else
                {
                    currentDTO.BLOOD_PRESSURE_MIN = null;
                }

                if (!string.IsNullOrEmpty(spinTemperature.Text))
                {
                    currentDTO.TEMPERATURE = Inventec.Common.TypeConvert.Parse.ToDecimal(spinTemperature.Value.ToString());
                }
                else 
                {
                    currentDTO.TEMPERATURE = null;
                }

                if (!string.IsNullOrEmpty(spinBreathRate.Text))
                {
                    currentDTO.BREATH_RATE = Inventec.Common.TypeConvert.Parse.ToDecimal(spinBreathRate.Value.ToString());
                }
                else 
                {
                    currentDTO.BREATH_RATE = null;
                }

                if (!string.IsNullOrEmpty(spinHeight.Text))
                {
                    currentDTO.HEIGHT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinHeight.Value.ToString());
                }
                else 
                {
                    currentDTO.HEIGHT = null;
                }

                if (!string.IsNullOrEmpty(spinWeight.Text))
                {
                    currentDTO.WEIGHT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinWeight.Value.ToString());
                }
                else 
                {
                    currentDTO.WEIGHT = null;
                }

                if (!string.IsNullOrEmpty(spinChest.Text))
                {
                    currentDTO.CHEST = Inventec.Common.TypeConvert.Parse.ToDecimal(spinChest.Value.ToString());
                }
                else 
                {
                    currentDTO.CHEST = null;
                }

                if (!string.IsNullOrEmpty(spinBelly.Text))
                {
                    currentDTO.BELLY = Inventec.Common.TypeConvert.Parse.ToDecimal(spinBelly.Value.ToString());
                }
                else 
                {
                    currentDTO.BELLY = null;
                }

                var roomId = this.moduleData.RoomId;
                if (roomId != 0)
                {
                    currentDTO.DEPARTMENT_ID = BackendDataWorker.Get<HIS_ROOM>().Where(o => o.ID == roomId).FirstOrDefault().DEPARTMENT_ID;
                }
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
                ValidationControlMaxLength(meContent, 4000);
                ValidationControlMaxLength(meMedicalInstruction, 4000);
                ValidationSingleControl(txtTrackingTempCode,6);
                ValidationSingleControl(txtTrackingTempName,100);
                ValidSpin(this.spinPulse);
                ValidSpin(this.spinBloodPressureMax);
                ValidSpin(this.spinBloodPressureMin);
                ValidSpin(this.spinTemperature);
                ValidSpin(this.spinBreathRate);
                ValidSpin(this.spinChest);
                ValidSpin(this.spinBelly);
                ValidSpin(this.spinHeight);
                ValidSpin(this.spinWeight);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidSpin(SpinEdit spinEdit)
        {
            SpinEditValidationRule spin = new SpinEditValidationRule();
            spin.spinEdit = spinEdit;
            spin.ErrorText = "Trường dữ liệu không được phép âm";
            spin.ErrorType = ErrorType.Warning;
            this.dxValidationProviderEditorInfo.SetValidationRule(spinEdit, spin);
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = "Trường nhập quá dài";
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
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

        private void ValidationSingleControl(BaseEdit control, int maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = true;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);

                //ControlEditValidationRule validRule = new ControlEditValidationRule();
                //validRule.editor = control;
                //validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                //validRule.ErrorType = ErrorType.Warning;
                //dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
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

                //Kiem tra co yeu cau su dung du lieu hien tại khong
                CheckRequestData();

                //Get du lieu thong tin user hien tai
                GetEmployee();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //ResetControl
                ResetControlText();

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

        private void GetEmployee()
        {
            try
            {
                CommonParam paramGet = new CommonParam();
                HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.employ = new BackendAdapter(paramGet).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>(RequestUriStore.HIS_EMPLOYEE_GET, ApiConsumers.MosConsumer, filter, paramGet).FirstOrDefault();
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

        private void bbtnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSelect_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void CheckRequestData()
        {
            try
            {
                if (this.trackingTempADO.DelegateSelectData != null)
                {
                    gcolGSelect.Visible = true;
                    btnSelect.Enabled = true;
                    this.layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    gcolGSelect.Visible = false;
                    btnSelect.Enabled = false;
                    this.layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                decimal height = Inventec.Common.TypeConvert.Parse.ToDecimal(spinHeight.Value.ToString());
                decimal weight = Inventec.Common.TypeConvert.Parse.ToDecimal(spinWeight.Value.ToString());
                //CASE  WHEN ("WEIGHT" IS NULL OR "HEIGHT" IS NULL OR "WEIGHT"=0 OR "HEIGHT"=0) THEN NULL ELSE ROUND("WEIGHT"/POWER("HEIGHT"/100,2),2) END 
                if (height > 0 && weight > 0)
                {

                    var bmi = Math.Round(((double)weight / Math.Pow((double)(height / 100), 2)), 2);
                    lcBMI.Text = bmi.ToString();
                }
                else lcBMI.Text = null;
                //ROUND(0.007184*POWER("HEIGHT",0.725)*POWER("WEIGHT",0.425),2)
                if (height > 0 && weight > 0)
                {
                    var BSA = Math.Round(0.007184 * Math.Pow((double)height, 0.725) * Math.Pow((double)weight, 0.425), 2);
                    lcBSA.Text = BSA.ToString();
                }
                else lcBSA.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                decimal height = Inventec.Common.TypeConvert.Parse.ToDecimal(spinHeight.Value.ToString());
                decimal weight = Inventec.Common.TypeConvert.Parse.ToDecimal(spinWeight.Value.ToString());
                //CASE  WHEN ("WEIGHT" IS NULL OR "HEIGHT" IS NULL OR "WEIGHT"=0 OR "HEIGHT"=0) THEN NULL ELSE ROUND("WEIGHT"/POWER("HEIGHT"/100,2),2) END 
                if (height > 0 && weight > 0)
                {

                    var bmi = Math.Round(((double)weight / Math.Pow((double)(height / 100), 2)), 2);
                    lcBMI.Text = bmi.ToString();
                }
                else lcBMI.Text = null;
                //ROUND(0.007184*POWER("HEIGHT",0.725)*POWER("WEIGHT",0.425),2)
                if (height > 0 && weight > 0)
                {
                    var BSA = Math.Round(0.007184 * Math.Pow((double)height, 0.725) * Math.Pow((double)weight, 0.425), 2);
                    lcBSA.Text = BSA.ToString();
                }
                else lcBSA.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    ChangeLock(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HIS_TRACKING_TEMP)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    ChangeLock(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ChangeLock(HIS_TRACKING_TEMP row)
        {
            try
            {
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    if (IsAllowEdit(row.CREATOR))
                    {
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                              (param).Post<HIS_TRACKING_TEMP>
                              ("api/HisTrackingTemp/ChangeLock", ApiConsumers.MosConsumer, row.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;

                            FillDataToGridControl();
                        }
                    }

                    else
                    {
                        param.Messages.Add(" - Không được phép sửa khóa. Chưa có quyền! ");
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsInDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTrackingTempName.Focus();
                    txtTrackingTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



    }
}
