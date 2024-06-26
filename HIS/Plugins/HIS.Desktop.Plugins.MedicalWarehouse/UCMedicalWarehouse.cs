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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Nodes;
using MOS.EFMODEL.DataModels;
using Inventec.UC.Paging;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Plugins.MedicalWarehouse.CheckBoxProcessor;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.MedicalWarehouse.ADO;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.Skins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.Plugins.MedicalWarehouse.ChooseStore;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.MedicalWarehouse
{
    public partial class UCMedicalWarehouse : UserControl
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        int rowCount = 0;
        int dataTotal = 0;
        public PagingGrid pagingGrid;
        internal List<V_HIS_MEDI_RECORD> lstMediRecord { get; set; }
        internal List<long> lstDataStoreId;
        internal List<HisMediRecordADO> hisMediRecords;
        internal List<HisMediRecordADO> hisMediRecordsCheckProcessing;
        bool isCheckAll = true;

        public UCMedicalWarehouse()
        {
            InitializeComponent();
        }

        public UCMedicalWarehouse(Inventec.Desktop.Common.Modules.Module currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCMedicalWarehouse_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                txtKeyWord.Text = "";
                LoaddataToTreeList(this);
                FillDataToGridControlMediRecord();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicalWarehouse.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicalWarehouse.UCMedicalWarehouse).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCMedicalWarehouse.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void GetDataAllMediRecord()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisMediRecordViewFilter filter = new HisMediRecordViewFilter();
                lstMediRecord = new List<V_HIS_MEDI_RECORD>();
                lstMediRecord = new BackendAdapter(param).Get<List<V_HIS_MEDI_RECORD>>(HisRequestUriStore.HIS_MEDI_RECORD_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event Tree DATA_STORE
        private void LoaddataToTreeList(UCMedicalWarehouse control)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDataStoreViewFilter dataStoreFilter = new MOS.Filter.HisDataStoreViewFilter();
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_DATA_STORE>>(HisRequestUriStore.HIS_DATA_STORE_GETVIEW, ApiConsumers.MosConsumer, dataStoreFilter, param).Where(p => p.ROOM_ID == this.currentModule.RoomId || p.ROOM_TYPE_ID == this.currentModule.RoomTypeId).ToList();
                if (currentDataStore != null && currentDataStore.Count > 0)
                {
                    var lstParent = currentDataStore.Where(o => o.PARENT_ID == null).ToList();
                    lstDataStoreId = new List<long>();
                    if (lstParent != null)
                    {
                        treeListDataStore.Nodes.Clear();
                        TreeListNode parentForRootNodes = null;
                        foreach (var item in lstParent)
                        {
                            TreeListNode rootNode = treeListDataStore.AppendNode(
                         new object[] { item.DATA_STORE_NAME, null, null, null, null, null, null },
                         parentForRootNodes, item);
                            CreateChildNode(rootNode, item, currentDataStore, control);
                        }
                        control.treeListDataStore.ExpandToLevel(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void CreateChildNode(TreeListNode rootNode, V_HIS_DATA_STORE hisDataStore, List<V_HIS_DATA_STORE> hisDataStores, UCMedicalWarehouse control)
        {
            try
            {
                var listChilds = hisDataStores.Where(o => o.PARENT_ID == hisDataStore.ID).ToList();
                if (listChilds != null && listChilds.Count > 0)
                {
                    foreach (var itemChild in listChilds)
                    {
                        TreeListNode childNode = control.treeListDataStore.AppendNode(
                        new object[] { itemChild.DATA_STORE_NAME, null, null },
                        rootNode, itemChild);
                        CreateChildNode(childNode, itemChild, hisDataStores, control);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDataStore_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                //SetCheckChildNode(e.Node, e.Node.CheckState);
                //SetCheckParentNode(e.Node, e.Node.CheckState);
                ChangeCheckedNodesChoices__TabService(e.Node, e.Node.CheckState);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckChildNode(TreeListNode node, CheckState check)
        {
            try
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    node.Nodes[i].CheckState = check;
                    SetCheckChildNode(node.Nodes[i], check);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckParentNode(TreeListNode node, CheckState check)
        {
            try
            {
                if (node.ParentNode != null)
                {
                    bool b = false;
                    CheckState state;
                    for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                    {
                        state = node.ParentNode.Nodes[i].CheckState;
                        if (!check.Equals(state))
                        {
                            b = !b;
                            break;
                        }
                    }
                    node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                    SetCheckParentNode(node.ParentNode, check);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeCheckedNodesChoices__TabService(TreeListNode node, CheckState check)
        {
            try
            {
                // var data = (V_HIS_DATA_STORE)node.Tag;
                List<V_HIS_MEDI_RECORD> lstMediRecordByDataStoreId = new List<V_HIS_MEDI_RECORD>();
                lstDataStoreId = new List<long>();
                CheckBoxProcessor op = new CheckBoxProcessor();
                treeListDataStore.NodesIterator.DoOperation(op);
                //Get the number of checked nodes:
                int count = op.CheckedNodes.Count;
                var lst = op.CheckedNodes.ToList();
                if (lst != null)
                {
                    foreach (var item in lst)
                    {
                        var datastore = (V_HIS_DATA_STORE)item.Tag;
                        lstDataStoreId.Add(datastore.ID);
                    }
                    chkSave.Checked = false;
                }
                else
                {
                    lstDataStoreId = new List<long>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDataStore_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDataStore_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var info = treeListDataStore.CalcHitInfo(e.Location);
                if (info.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Cell)
                {
                    if (info.Node.Checked)
                    {
                        info.Node.Checked = false;
                        ChangeCheckedNodesChoices__TabService(info.Node, info.Node.CheckState);
                    }
                    else
                    {
                        info.Node.Checked = true;
                        ChangeCheckedNodesChoices__TabService(info.Node, info.Node.CheckState);
                    }
                }
                else if (info.HitInfoType == DevExpress.XtraTreeList.HitInfoType.NodeCheckBox)
                {
                    if (info.Node.Checked)
                    {
                        info.Node.Selected = false;
                        ChangeCheckedNodesChoices__TabService(info.Node, info.Node.CheckState);
                    }
                    else
                    {
                        info.Node.Selected = true;
                        ChangeCheckedNodesChoices__TabService(info.Node, info.Node.CheckState);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDataStore_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var noteData = (MOS.EFMODEL.DataModels.V_HIS_DATA_STORE)e.Node.Tag;
                if (noteData == null)
                {
                    if (e.Node.Nodes.Count > 0)
                    {
                        e.Node.ExpandAll();
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        int startPage = 0;
        private void FillDataToGridControlMediRecord()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                MediRecordPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(MediRecordPaging, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MediRecordPaging(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD>> apiResult = new ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD>>();

                HisMediRecordViewFilter filter = new HisMediRecordViewFilter();

                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (chkSave.Checked == true)
                {
                    filter.HAS_DATA_STORE = false;
                }
                if (lstDataStoreId != null && lstDataStoreId.Count > 0)
                {
                    filter.DATA_STORE_IDs = lstDataStoreId; //Tìm theo kho dữ liệu
                }
                gridControlMediRecord.DataSource = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_MEDI_RECORD>>(HisRequestUriStore.HIS_MEDI_RECORD_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        hisMediRecords = new List<HisMediRecordADO>();
                        foreach (var item in data)
                        {
                            ADO.HisMediRecordADO hisMediRecord = new ADO.HisMediRecordADO();
                            Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, ADO.HisMediRecordADO>();
                            hisMediRecord = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, ADO.HisMediRecordADO>(item);
                            hisMediRecords.Add(hisMediRecord);
                        }
                        gridControlMediRecord.DataSource = hisMediRecords;
                        // gridControlMediRecord.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControlMediRecord();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediRecord_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD data = (MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.DOB) + "  tuổi";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediRecord_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var dataStoreID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewMediRecord.GetRowCellValue(e.RowHandle, "DATA_STORE_ID") ?? "").ToString());
                    if (e.Column.FieldName == "UnSaveDataStore")
                    {
                        if (dataStoreID == 0)
                        {
                            e.RepositoryItem = repositoryItemButton__UnSave__Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__UnSave__Enable;
                        }
                    }
                    else if (e.Column.FieldName == "check")
                    {
                        if (dataStoreID == 0)
                        {
                            e.RepositoryItem = repositoryItemCheck__Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheck__Disable;
                        }
                    }
                    else if (e.Column.FieldName == "PRINT_DISPLAY")
                    {
                        if (dataStoreID == 0)
                        {
                            e.RepositoryItem = repositoryItemButton__Print_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Print_Enable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__UnSave__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_MEDI_RECORD)gridViewMediRecord.GetFocusedRow();
                if (row != null)
                {
                    bool result = false;
                    CommonParam param = new CommonParam();

                    HIS_MEDI_RECORD hisMediRecord = new HIS_MEDI_RECORD();
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, HIS_MEDI_RECORD>();
                    hisMediRecord = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, HIS_MEDI_RECORD>(row);
                    hisMediRecord.DATA_STORE_ID = null;
                    var current = new BackendAdapter(param).Post<HIS_MEDI_RECORD>(HisRequestUriStore.HIS_MEDI_RECORD_UPDATE_DATA_STORE_ID, ApiConsumers.MosConsumer, hisMediRecord, param);
                    if (current != null)
                    {
                        result = true;
                        FillDataToGridControlMediRecord();
                    }
                    #region Show message
                    MessageManager.Show(param, result);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                hisMediRecordsCheckProcessing = new List<HisMediRecordADO>();
                if (hisMediRecords != null && hisMediRecords.Count > 0)
                {
                    hisMediRecordsCheckProcessing = hisMediRecords.Where(o => o.check == true).ToList();
                    if (hisMediRecordsCheckProcessing != null && hisMediRecordsCheckProcessing.Count > 0)
                    {
                        HIS.Desktop.Plugins.MedicalWarehouse.ChooseStore.frmChooseStore frmChooseStore = new ChooseStore.frmChooseStore(this.currentModule,hisMediRecordsCheckProcessing, FillDataToGridControlMediRecord);
                        frmChooseStore.ShowDialog();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn hồ sơ cần lưu trữ", "Thông báo");
                        return;
                    }
                }

                //List<V_HIS_MEDI_RECORD> lstMediRecordChecks = new List<V_HIS_MEDI_RECORD>();
                //if (gridViewMediRecord.RowCount > 0)
                //{
                //    for (int i = 0; i < gridViewMediRecord.SelectedRowsCount; i++)
                //    {
                //        if (gridViewMediRecord.GetSelectedRows()[i] >= 0)
                //        {
                //            lstMediRecordChecks.Add((V_HIS_MEDI_RECORD)gridViewMediRecord.GetRow(gridViewMediRecord.GetSelectedRows()[i]));
                //        }
                //    }
                //}
                //if (lstMediRecordChecks != null && lstMediRecordChecks.Count > 0)
                //{
                //    HIS.Desktop.Plugins.MedicalWarehouse.ChooseStore.frmChooseStore frmChooseStore = new ChooseStore.frmChooseStore(lstMediRecordChecks, FillDataToGridControlMediRecord);
                //    frmChooseStore.ShowDialog();
                //}
                //else
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn hồ sơ cần lưu trữ", "Thông báo");
                //    return;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSAVE()
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

        public void bbtnSEARCH()
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

        public void keyF2Focused()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediRecord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //List<V_HIS_MEDI_RECORD> lstBedLogChecks = new List<V_HIS_MEDI_RECORD>();
                //if (gridViewMediRecord.RowCount > 0)
                //{
                //    for (int i = 0; i < gridViewMediRecord.SelectedRowsCount; i++)
                //    {
                //        if (gridViewMediRecord.GetSelectedRows()[i] >= 0)
                //        {
                //            lstBedLogChecks.Add((V_HIS_MEDI_RECORD)gridViewMediRecord.GetRow(gridViewMediRecord.GetSelectedRows()[i]));
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSave_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                btnSearch.Focus();
                if (chkSave.Checked)
                {
                    treeListDataStore.UncheckAll();
                    lstDataStoreId = new List<long>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediRecord_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            try
            {
                var lstCheckAll = hisMediRecords;
                List<HisMediRecordADO> lstChecks = new List<HisMediRecordADO>();

                if (lstCheckAll != null && lstCheckAll.Count > 0)
                {
                    if (isCheckAll)
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.DATA_STORE_ID == null)
                            {
                                item.check = true;
                                lstChecks.Add(item);
                            }
                            else
                            {
                                lstChecks.Add(item);
                            }
                        }
                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.DATA_STORE_ID == null)
                            {
                                item.check = false;
                                lstChecks.Add(item);
                            }
                            else
                            {
                                lstChecks.Add(item);
                            }
                        }
                        isCheckAll = true;
                    }
                    //gridControlMediRecord.DataSource = null;
                    gridControlMediRecord.BeginUpdate();
                    gridControlMediRecord.DataSource = lstChecks;
                    gridControlMediRecord.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Print_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisMediRecordADO)gridViewMediRecord.GetFocusedRow();
                if (row != null)
                {
                    V_HIS_MEDI_RECORD VHisMediRecord = new V_HIS_MEDI_RECORD();
                    Mapper.CreateMap<HisMediRecordADO, V_HIS_MEDI_RECORD>();
                    VHisMediRecord = Mapper.Map<HisMediRecordADO, V_HIS_MEDI_RECORD>(row);
                    PrintMediRecord(VHisMediRecord);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediRecord_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check")
                        {
                            var lstCheckAll = hisMediRecords;
                            List<HisMediRecordADO> lstChecks = new List<HisMediRecordADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.DATA_STORE_ID == null)
                                        {
                                            item.check = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.DATA_STORE_ID == null)
                                        {
                                            item.check = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }
                                gridControlMediRecord.BeginUpdate();
                                gridControlMediRecord.DataSource = lstChecks;
                                gridControlMediRecord.EndUpdate();
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
    }
}
