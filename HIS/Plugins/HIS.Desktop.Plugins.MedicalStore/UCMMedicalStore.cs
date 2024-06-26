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
using Inventec.Desktop.Plugins.MedicalStore.CheckBoxProcessor;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.MedicalStore.ADO;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.Skins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.Plugins.MedicalStore.ChooseStore;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.MedicalStore
{
    public partial class UCMedicalStore : UserControl
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        int rowCount = 0;
        int dataTotal = 0;
        internal List<V_HIS_TREATMENT> lstMediRecord { get; set; }
        internal List<long> lstDataStoreId;
        internal List<HisMediRecordADO> hisMediRecords;
        internal List<HisMediRecordADO> hisMediRecordsCheckProcessing;
        bool isCheckAll = true;

        public UCMedicalStore(Inventec.Desktop.Common.Modules.Module currentModule)
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

        private void UCMedicalStore_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                txtKeyWord.Text = "";
                LoaddataToTreeList(this);
                FillDataToGridControlMediRecord();
                cboFill.SelectedIndex = 1;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicalStore.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicalStore.UCMedicalStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicalStore.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMedicalStore.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCMedicalStore.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                lstMediRecord = new List<V_HIS_TREATMENT>();
                lstMediRecord = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event Tree DATA_STORE
        private void LoaddataToTreeList(UCMedicalStore control)
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

        internal void CreateChildNode(TreeListNode rootNode, V_HIS_DATA_STORE hisDataStore, List<V_HIS_DATA_STORE> hisDataStores, UCMedicalStore control)
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
                ChangeCheckedNodesChoices__TabService(e.Node, e.Node.CheckState);
                btnSearch_Click(null, null);
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
                List<V_HIS_TREATMENT> lstMediRecordByDataStoreId = new List<V_HIS_TREATMENT>();
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
                ucPaging1.Init(MediRecordPaging, param, pageSize);
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
                hisMediRecords = new List<HisMediRecordADO>();
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>> apiResult = null;

                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();

                SetFilter(ref filter);

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {                      
                        foreach (var item in data)
                        {
                            ADO.HisMediRecordADO hisMediRecord = new ADO.HisMediRecordADO();
                            Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, ADO.HisMediRecordADO>();
                            hisMediRecord = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, ADO.HisMediRecordADO>(item);

                            if (item.DATA_STORE_ID > 0)
                                hisMediRecord.check = true;
                            else
                                hisMediRecord.check = false;

                            hisMediRecords.Add(hisMediRecord);
                        }
                        
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    gridControlMediRecord.DataSource = null;
                    gridControlMediRecord.DataSource = hisMediRecords;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void SetFilter(ref HisTreatmentViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (cboFill.SelectedIndex == 0)
                {
                    filter.IS_PAUSE = false;
                }
                if (cboFill.SelectedIndex == 1)
                {
                    filter.IS_PAUSE = true;
                }
                if (chkSave.Checked == true)
                {
                    filter.HAS_DATA_STORE = true;
                }
                else if (chkSave.Checked == false)
                {
                    filter.HAS_DATA_STORE = false;
                }

                if (lstDataStoreId != null && lstDataStoreId.Count > 0)
                {
                    filter.DATA_STORE_IDs = lstDataStoreId; //Tìm theo kho dữ liệu
                }
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "DATA_STORE_ID";

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
                    MOS.EFMODEL.DataModels.V_HIS_TREATMENT data = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                            e.Value = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(data.TDL_PATIENT_DOB) + "  tuổi";
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
                        e.RepositoryItem = (dataStoreID > 0) ? repositoryItemButton__UnSave__Enable : repositoryItemButton__UnSave__Disable;
                    }
                    else if (e.Column.FieldName == "check")
                    {
                        e.RepositoryItem = repositoryItemCheck__Enable;
                    }
                    else if (e.Column.FieldName == "PRINT_DISPLAY")
                    {
                        e.RepositoryItem = (dataStoreID > 0) ? repositoryItemButton__Print_Enable : repositoryItemButton__Print_Disable;
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
                var row = (V_HIS_TREATMENT)gridViewMediRecord.GetFocusedRow();
                if (row != null)
                {
                    bool result = false;
                    CommonParam param = new CommonParam();
                    row.DATA_STORE_ID = null;
                    var current = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateDataStoreId", ApiConsumers.MosConsumer, row, param);
                    if (current != null)
                    {
                        result = true;
                        FillDataToGridControlMediRecord();
                    }
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, result);
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
                        HIS.Desktop.Plugins.MedicalStore.ChooseStore.frmChooseStore frmChooseStore = new ChooseStore.frmChooseStore(this.currentModule, hisMediRecordsCheckProcessing, FillDataToGridControlMediRecord);
                        frmChooseStore.ShowDialog();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn hồ sơ cần lưu trữ", "Thông báo");
                        return;
                    }
                }
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
                btnSave.Focus();
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
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
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
                //if (chkSave.Checked)
                //{
                //    treeListDataStore.UncheckAll();
                //    lstDataStoreId = new List<long>();
                //}
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
                    V_HIS_TREATMENT VHisMediRecord = new V_HIS_TREATMENT();
                    Mapper.CreateMap<HisMediRecordADO, V_HIS_TREATMENT>();
                    VHisMediRecord = Mapper.Map<HisMediRecordADO, V_HIS_TREATMENT>(row);
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

        private void repositoryItemCheck__Enable_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
