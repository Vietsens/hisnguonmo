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
using MOS.SDO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.HisDataStore.HisDataStore
{
    public partial class frmHisDataStore : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        HisDataStoreADO currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        DelegateSelectData delegateSelect = null;
        MOS.EFMODEL.DataModels.HIS_DATA_STORE resultData;
        List<MOS.EFMODEL.DataModels.HIS_DATA_STORE> dataStoreList;
        List<V_HIS_ROOM> storeRoomList;
        #endregion

        #region Construct
        public frmHisDataStore(Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelect)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                delegateSelect = _delegateSelect;
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
        private void frmHisDataStore_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
                treeList1.CollapseAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RefeshDataAfterSave()
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(resultData);
                }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDataStore.Resources.Lang", typeof(frmHisDataStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.clDelete.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.clDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.clLock.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.clLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.clDataStore.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.clDataStore.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.clName.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.clName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_STORED_ROOM_NAME.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.treeListColumn_STORED_ROOM_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_STORED_DEPARTMENT_NAME.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.treeListColumn_STORED_DEPARTMENT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnTreatmentType.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.treeListColumnTreatmentType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisDataStore.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentTypeIDS.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDataStore.cboTreatmentTypeIDS.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisDataStore.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDataStore.cboTreatmentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStoreRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDataStore.cboStoreRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStoreDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDataStore.cboStoreDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDataStore.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDataStore.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineUseFormCode.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciMedicineUseFormCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineUseFormName.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciMedicineUseFormName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDataStore.layoutControlItem6.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboStoreDepartment.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciCboStoreDepartment.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboStoreDepartment.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciCboStoreDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboStoreRoom.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciCboStoreRoom.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboStoreRoom.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciCboStoreRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciTreatmentType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentTypeIDS.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciTreatmentTypeIDS.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentTypeIDS.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.lciTreatmentTypeIDS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisDataStore.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !string.IsNullOrEmpty(moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
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
                cboTreatmentType.EditValue = null;
                cboTreatmentTypeIDS.EditValue = null;
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

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtMedicineUseFormCode", 0);
                dicOrderTabIndexControl.Add("txtMedicineUseFormName", 1);


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
                LoadComboDepartment();
                LoadComboStoreDepartment();
                LoadComboStoreRoom();
                LoadComboParent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboParent()
        {
            CommonParam param = new CommonParam();
            HisDataStoreFilter filter = new HisDataStoreFilter();
            filter.IS_ACTIVE = 1;

            dataStoreList = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DATA_STORE>>("api/HisDataStore/Get", ApiConsumers.MosConsumer, filter, param);
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("DATA_STORE_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("DATA_STORE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("DATA_STORE_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cboParent, dataStoreList, controlEditorADO);
        }

        private void LoadComboDepartment()
        {
            var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
        }

        private void LoadComboStoreDepartment()
        {
            var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(this.cboStoreDepartment, data, controlEditorADO);
        }

        private void LoadComboStoreRoom()
        {
            storeRoomList = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).ToList();
            if (cboStoreDepartment.EditValue != null)
            {
                storeRoomList = storeRoomList.Where(o => o.DEPARTMENT_ID == (long)cboStoreDepartment.EditValue).ToList();
            }
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(this.cboStoreRoom, storeRoomList, controlEditorADO);
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
                LoadPaging(new CommonParam(0, 0));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, 0, this.treeList1);
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
                CommonParam paramCommon = new CommonParam();
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_DATA_STORE>> apiResult = null;
                HisDataStoreViewFilter filter = new HisDataStoreViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                treeList1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_DATA_STORE>>(HisRequestUriStore.MOSHIS_DATA_STORE_GET_VIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    List<HisDataStoreADO> dataStoreList = new List<HisDataStoreADO>();
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_DATA_STORE>)apiResult.Data;
                    foreach (var item in data)
                    {
                        HisDataStoreADO store = new HisDataStoreADO(item);
                        dataStoreList.Add(store);
                    }
                    if (dataStoreList != null)
                    {
                        treeList1.KeyFieldName = "ID";
                        treeList1.ParentFieldName = "PARENT_ID";
                        treeList1.DataSource = dataStoreList;
                        rowCount = (data == null ? 0 : dataStoreList.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                treeList1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisDataStoreViewFilter filter)
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
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void treeList1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        //{
        //    try
        //    {
        //        if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
        //        {
        //            MOS.EFMODEL.DataModels.V_HIS_DATA_STORE pData = (MOS.EFMODEL.DataModels.V_HIS_DATA_STORE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
        //            if (e.Column.FieldName == "STT")
        //            {
        //                e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
        //            }
        //            else if (e.Column.FieldName == "CREATE_TIME_STR")
        //            {
        //                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
        //            }
        //            else if (e.Column.FieldName == "MODIFY_TIME_STR")
        //            {
        //                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
        //            }
        //            treeList1.RefreshDataSource();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void treeList1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void treeList1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    this.currentData = data as HisDataStoreADO;
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



        private void ChangedDataRow(HisDataStoreADO data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HisDataStoreADO data)
        {
            try
            {
                if (data != null)
                {
                    GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                    }
                    if (!String.IsNullOrWhiteSpace(data.TREATMENT_END_TYPE_IDS) && gridCheckMark != null)
                    {
                        ProcessSelectTreatmentType(data, gridCheckMark);
                    }


                    GridCheckMarksSelection gridCheckMark2 = cboTreatmentTypeIDS.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark2 != null)
                    {
                        gridCheckMark2.ClearSelection(cboTreatmentTypeIDS.Properties.View);
                    }
                    if (!String.IsNullOrWhiteSpace(data.TREATMENT_TYPE_IDS) && gridCheckMark2 != null)
                    {
                        ProcessSelectTreatmentTypeIDS(data, gridCheckMark2);
                    }
                    txtDataStoreCode.Text = data.DATA_STORE_CODE;
                    txtDataStoreName.Text = data.DATA_STORE_NAME;
                    cboParent.EditValue = data.PARENT_ID;
                    cboDepartment.EditValue = data.DEPARTMENT_ID;
                    cboStoreDepartment.EditValue = data.STORED_DEPARTMENT_ID;
                    cboStoreRoom.EditValue = data.STORED_ROOM_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectTreatmentType(HisDataStoreADO data, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_TREATMENT_END_TYPE> ds = cboTreatmentType.Properties.DataSource as List<HIS_TREATMENT_END_TYPE>;
                string[] arrays = data.TREATMENT_END_TYPE_IDS.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_TREATMENT_END_TYPE> selects = new List<HIS_TREATMENT_END_TYPE>();
                    List<long> ids = new List<long>();
                    foreach (var item in arrays)
                    {
                        long id = Convert.ToInt64(item ?? "0");
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == id) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSelectTreatmentTypeIDS(HisDataStoreADO data, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_TREATMENT_TYPE> ds = cboTreatmentTypeIDS.Properties.DataSource as List<HIS_TREATMENT_TYPE>;
                string[] arrays = data.TREATMENT_TYPE_IDS.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_TREATMENT_TYPE> selects = new List<HIS_TREATMENT_TYPE>();
                    List<long> ids = new List<long>();
                    foreach (var item in arrays)
                    {
                        long id = Convert.ToInt64(item ?? "0");
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == id) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                txtDataStoreCode.Focus();
                txtDataStoreCode.SelectAll();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_DATA_STORE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDataStoreFilter filter = new HisDataStoreFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DATA_STORE>>(HisRequestUriStore.MOSHIS_DATA_STORE_GET_VIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                btnAdd.Enabled = txtDataStoreCode.Enabled = cboDepartment.Enabled = (action == GlobalVariables.ActionAdd);
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
                treeList1.CollapseAll();
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
                treeList1.CollapseAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        var rowData = (MOS.EFMODEL.DataModels.HIS_DATA_STORE)treeList1.GetFocusedRow();
        //        if (rowData != null)
        //        {
        //            bool success = false;
        //            CommonParam param = new CommonParam();
        //            success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_DATA_STORE_DELETE,
        //              ApiConsumers.MosConsumer, rowData, param);
        //            if (success)
        //            {
        //                FillDataToGridControl();
        //                BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_DATA_STORE>();

        //                this.currentData = null;
        //            }
        //            MessageManager.Show(this, param, success);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                currentData = null;
                cboTreatmentType.ResetText();
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                }
                cboTreatmentType.Text = "";
                cboTreatmentType.ToolTip = "";

                cboTreatmentTypeIDS.ResetText();
                GridCheckMarksSelection gridCheckMark2 = cboTreatmentTypeIDS.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark2 != null)
                {
                    gridCheckMark2.ClearSelection(cboTreatmentTypeIDS.Properties.View);
                }
                cboTreatmentTypeIDS.Text = "";
                cboTreatmentTypeIDS.ToolTip = "";
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
            HisDataStoreSDO sdo = new HisDataStoreSDO();
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
                MOS.EFMODEL.DataModels.HIS_DATA_STORE updateDTO = new MOS.EFMODEL.DataModels.HIS_DATA_STORE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                sdo.HisDataStore = updateDTO;
                HIS_ROOM room = new HIS_ROOM();
                room.DEPARTMENT_ID = (long)cboDepartment.EditValue;
                room.ROOM_TYPE_ID = 5;
                sdo.HisRoom = room;
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    resultData = new BackendAdapter(param).
                      Post<MOS.EFMODEL.DataModels.HIS_DATA_STORE>
                      (HisRequestUriStore.MOSHIS_DATA_STORE_CREATE,
                      ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                        RefeshDataAfterSave();
                        BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_DATA_STORE>();
                        LoadComboParent();
                    }
                }
                else
                {
                    sdo.HisRoom.ID = currentData.ROOM_ID;
                    resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DATA_STORE>
                      (HisRequestUriStore.MOSHIS_DATA_STORE_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {
                        success = true;
                        btnRefesh_Click(null, null);
                        RefeshDataAfterSave();
                        BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_DATA_STORE>();
                        LoadComboParent();
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



        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_DATA_STORE currentDTO)
        {
            try
            {
                currentDTO.DATA_STORE_CODE = txtDataStoreCode.Text.Trim();
                currentDTO.DATA_STORE_NAME = txtDataStoreName.Text.Trim();
                //currentDTO.ROOM_ID = (long)cboDepartment.EditValue;
                if (cboParent.EditValue != null)
                {
                    currentDTO.PARENT_ID = (long)cboParent.EditValue;
                }
                else
                {
                    currentDTO.PARENT_ID = null;
                }
                if (cboStoreDepartment.EditValue != null)
                {
                    currentDTO.STORED_DEPARTMENT_ID = (long)cboStoreDepartment.EditValue;
                }
                else
                {
                    currentDTO.STORED_DEPARTMENT_ID = null;
                }
                if (cboStoreRoom.EditValue != null)
                {
                    currentDTO.STORED_ROOM_ID = (long)cboStoreRoom.EditValue;
                }
                else
                {
                    currentDTO.STORED_ROOM_ID = null;
                }
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    List<long> ids = new List<long>();
                    foreach (HIS_TREATMENT_END_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv != null && !ids.Contains(rv.ID))
                            ids.Add(rv.ID);
                    }
                    currentDTO.TREATMENT_END_TYPE_IDS = String.Join(",", ids);
                }
                else
                {
                    currentDTO.TREATMENT_END_TYPE_IDS = "";
                }
                cboTreatmentType.ResetText();

                GridCheckMarksSelection gridCheckMark2 = cboTreatmentTypeIDS.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark2 != null && gridCheckMark2.SelectedCount > 0)
                {
                    List<long> ids2 = new List<long>();
                    foreach (HIS_TREATMENT_TYPE rv in gridCheckMark2.Selection)
                    {
                        if (rv != null && !ids2.Contains(rv.ID))
                            ids2.Add(rv.ID);
                    }
                    currentDTO.TREATMENT_TYPE_IDS = String.Join(",", ids2);
                }
                else
                {
                    currentDTO.TREATMENT_TYPE_IDS = "";
                }
                cboTreatmentTypeIDS.ResetText();
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
                ValidationSingleControlWithMaxLength(txtDataStoreCode, 10, true);
                ValidationSingleControlWithMaxLength(txtDataStoreName, 100, true);
                ValidationSingleControl(cboDepartment);

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

        private void ValidationSingleControlWithMaxLength(BaseEdit control, int maxlength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.maxLength = maxlength;
                validRule.IsRequired = IsRequired;
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

        private void ValidationSingleControl1(SpinEdit control)
        {
            try
            {
                ValidateSpin1 validate = new ValidateSpin1();
                validate.spin = control;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validate);
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
                //Init combo TreatmentType
                InitCheck(cboTreatmentType, SelectionGrid__TreatmentType);
                InitCombo(cboTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>(), "TREATMENT_END_TYPE_NAME", "ID");

                //Init combo TreatmentTypeIDS
                InitCheck(cboTreatmentTypeIDS, SelectionGrid__TreatmentTypeIDS);
                InitCombo(cboTreatmentTypeIDS, BackendDataWorker.Get<HIS_TREATMENT_TYPE>(), "TREATMENT_TYPE_NAME", "ID");
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


        //private void ButtonEditLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    LockUnlock();
        //}

        //private void ButtonEditUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    LockUnlock();
        //}

        // khóa, bỏ khóa
        private void LockUnlock()
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var updateDTO = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                var result = new BackendAdapter(param).Post<HIS_PTTT_GROUP>(HisRequestUriStore.MOSHIS_DATA_STORE_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, updateDTO, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                    SetFocusEditor();
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_DATA_STORE>();
                    LoadComboParent();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__TreatmentType(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                var gridCheckMark = (sender as GridCheckMarksSelection);
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_TREATMENT_END_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.TREATMENT_END_TYPE_NAME + ",";
                    }
                }
                cboTreatmentType.Text = typeName;
                cboTreatmentType.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__TreatmentTypeIDS(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                var gridCheckMark = (sender as GridCheckMarksSelection);
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.TREATMENT_TYPE_NAME + ",";
                    }
                }
                cboTreatmentTypeIDS.Text = typeName;
                cboTreatmentTypeIDS.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is HisDataStoreADO)
                {
                    HisDataStoreADO rowData = data as HisDataStoreADO;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnLock : btnUnlock);

                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btnDelete;
                            else
                                e.RepositoryItem = btnDisableDelete;
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

        private void treeList1_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                //InitComboParentID();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                HisDataStoreADO rowData = data as HisDataStoreADO;
                if (rowData != null)
                {
                    
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    HisDataStoreADO rowData = data as HisDataStoreADO;
                    CommonParam param = new CommonParam();
                    HisDataStoreFilter filter = new HisDataStoreFilter();
                    filter.ID = rowData.ID;
                    var data2 = new BackendAdapter(param).Get<System.Collections.Generic.List<HIS_DATA_STORE>>("api/HisDataStore/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    if (rowData != null && data != null)
                    {
                        bool success = false;
                        //CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_DATA_STORE_DELETE, ApiConsumers.MosConsumer, data2, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_DATA_STORE>();
                            BackendDataWorker.Reset<V_HIS_DATA_STORE>();
                            FillDataToGridControl();
                            LoadComboParent();
                        }
                        else
                        {
                            MessageBox.Show("Dữ liệu đã được sử dụng", "Thông báo");
                            return;
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

        private void cboParent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboParent.EditValue = null;
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                LockUnlock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            try
            {
                LockUnlock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        

        private void cboStoreDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                cboStoreDepartment.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStoreRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                cboStoreRoom.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStoreDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadComboStoreRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue == null)
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.ID == (long)cboDepartment.EditValue).ToList();
                        if (department != null)
                        {
                            cboParent.Focus();
                            cboParent.ShowPopup();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartment.EditValue == null)
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.ID == (long)cboDepartment.EditValue).ToList();
                        if (department != null)
                        {
                            cboParent.Focus();
                            cboParent.ShowPopup();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboParent.EditValue == null)
                    {
                        cboParent.Focus();
                        //cboParent.ShowPopup();
                    }
                    else
                    {
                        var department = this.dataStoreList.Where(o => o.IS_ACTIVE == 1 && o.ID == (long)cboParent.EditValue).ToList();
                        if (department != null)
                        {
                            cboStoreDepartment.Focus();
                            cboStoreDepartment.ShowPopup();
                        }
                        else
                        {
                            cboParent.Focus();
                            cboParent.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (cboParent.EditValue == null)
                    //{
                    //    //cboParent.Focus();
                    //    //cboParent.ShowPopup();
                    //}
                    //else
                    //{
                    //    var department = this.dataStoreList.Where(o => o.IS_ACTIVE == 1 && o.ID == (long)cboParent.EditValue).ToList();
                    //    if (department != null)
                    //    {
                    //        cboStoreDepartment.Focus();
                    //        cboStoreDepartment.ShowPopup();
                    //    }
                    //    else
                    //    {
                    //        cboParent.Focus();
                    //        cboParent.ShowPopup();
                    //    }
                    //}

                    cboStoreDepartment.Focus();
                    cboStoreDepartment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStoreDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboStoreDepartment.EditValue == null)
                    {
                        cboStoreDepartment.Focus();
                        //cboStoreDepartment.ShowPopup();
                    }
                    else
                    {
                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.ID == (long)cboStoreDepartment.EditValue).ToList();
                        if (department != null)
                        {
                            cboStoreRoom.Focus();
                            cboStoreRoom.ShowPopup();
                        }
                        else
                        {
                            cboStoreDepartment.Focus();
                            cboStoreDepartment.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStoreDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (cboStoreDepartment.EditValue == null)
                    //{
                    //    //cboStoreDepartment.Focus();
                    //    //cboStoreDepartment.ShowPopup();
                    //}
                    //else
                    //{
                    //    var department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.ID == (long)cboStoreDepartment.EditValue).ToList();
                    //    if (department != null)
                    //    {
                    //        cboStoreRoom.Focus();
                    //        cboStoreRoom.ShowPopup();
                    //    }
                    //    else
                    //    {
                    //        cboStoreDepartment.Focus();
                    //        cboStoreDepartment.ShowPopup();
                    //    }
                    //}

                    cboStoreRoom.Focus();
                    cboStoreRoom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStoreRoom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboStoreRoom.EditValue == null)
                    {
                        cboStoreRoom.Focus();
                        //cboStoreRoom.ShowPopup();
                    }
                    else
                    {
                        cboTreatmentType.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStoreRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                string typeName = "";
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_TREATMENT_END_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.TREATMENT_END_TYPE_NAME + ",";
                    }
                }
                e.DisplayText = typeName;
                cboTreatmentType.ToolTip = typeName;
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
                    cboTreatmentType.ResetText();
                    GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                    }
                    cboTreatmentType.Text = "";
                    cboTreatmentType.ToolTip = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentTypeIDS.Focus();
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
                    if (cboTreatmentType.EditValue == null)
                    {
                        cboTreatmentType.Focus();
                        
                    }
                    else
                    {
                        cboTreatmentTypeIDS.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentTypeIDS_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentTypeIDS_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                string typeName = "";
                GridCheckMarksSelection gridCheckMark = cboTreatmentTypeIDS.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.TREATMENT_TYPE_NAME + ",";
                    }
                }
                e.DisplayText = typeName;
                cboTreatmentTypeIDS.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentTypeIDS_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTreatmentTypeIDS.ResetText();
                    GridCheckMarksSelection gridCheckMark = cboTreatmentTypeIDS.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTreatmentTypeIDS.Properties.View);
                    }
                    cboTreatmentTypeIDS.Text = "";
                    cboTreatmentTypeIDS.ToolTip = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
