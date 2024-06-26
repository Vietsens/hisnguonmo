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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MedicalStore.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStore.ChooseStore
{
    public partial class frmChooseStore : Form
    {
        //internal long dataStoreId;
        internal List<TreatmentADO> currentTreatment { get; set; }
        HIS.Desktop.Common.RefeshReference refeshData;
        //internal List<V_HIS_TREATMENT_9> lstTreatment { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<DataStoreADO> DataStoreADOs = new List<DataStoreADO>();

        public frmChooseStore(Inventec.Desktop.Common.Modules.Module currentModule, List<TreatmentADO> currentMediRecord, RefeshReference refeshData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentTreatment = currentMediRecord;
                this.refeshData = refeshData;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmChooseStore(Inventec.Desktop.Common.Modules.Module currentModule, List<V_HIS_TREATMENT_9> lstTreatments, RefeshReference refeshData)
        {
            //InitializeComponent();
            //try
            //{
            //    //this.lstTreatment = lstTreatments;
            //    this.currentModule = currentModule;
            //    this.refeshData = refeshData;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDataStore_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoaddataToTreeList();
                this.txtKeyword.SetParamUpdateInputAfterRegconize(UpdateInputAfterRegconizeText, WitRecognizeCFG.AccessToken, WitRecognizeCFG.TimeReplay);
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateInputAfterRegconizeText(string s)
        {
            txtKeyword.Text += s;
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicalStore.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicalStore.ChooseStore.frmChooseStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveData.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.btnSaveData.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem_Save.Caption = Inventec.Common.Resource.Get.Value("frmChooseStore.barButtonItem_Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Print.Caption = Inventec.Common.Resource.Get.Value("frmChooseStore.barButtonItem__Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoaddataToTreeList()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDataStoreViewFilter dataStoreFilter = new MOS.Filter.HisDataStoreViewFilter();
                dataStoreFilter.IS_ACTIVE = 1;
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_DATA_STORE>>(HisRequestUriStore.HIS_DATA_STORE_GETVIEW, ApiConsumers.MosConsumer, dataStoreFilter, param);
                if (currentDataStore != null && currentDataStore.Count > 0)
                {
                    currentDataStore = currentDataStore.Where(p => p.ROOM_ID == this.currentModule.RoomId || p.ROOM_TYPE_ID == this.currentModule.RoomTypeId).ToList();
                    foreach (var item in currentDataStore)
                    {
                        DataStoreADO data = new DataStoreADO(item);
                        this.DataStoreADOs.Add(data);
                    }
                }
                gridControlDataStore.BeginUpdate();
                gridControlDataStore.DataSource = null;
                gridControlDataStore.DataSource = this.DataStoreADOs;
                gridControlDataStore.EndUpdate();

                //var lstParent = currentDataStore.Where(o => o.PARENT_ID == null).ToList();
                //if (lstParent != null)
                //{
                //    treeListDataStore.Nodes.Clear();
                //    TreeListNode parentForRootNodes = null;
                //    foreach (var item in lstParent)
                //    {
                //        TreeListNode rootNode = treeListDataStore.AppendNode(
                //     new object[] { item.DATA_STORE_NAME },
                //     parentForRootNodes, item);
                //        CreateChildNode(rootNode, item, currentDataStore, this);
                //    }
                //    this.treeListDataStore.ExpandToLevel(0);
                //    this.treeListDataStore.OptionsBehavior.ImmediateEditor = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewDataStore.PostEditor();

                var dataSources = (List<DataStoreADO>)gridControlDataStore.DataSource;
                if (dataSources == null || dataSources.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Kho dữ liệu rỗng", "Thông báo");
                    return;
                }

                var dataChecks = dataSources.Where(o => o.CheckStore).ToList();

                if (dataChecks == null || dataChecks.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho dữ liệu", "Thông báo");
                    return;
                }


                bool result = false;
                CommonParam param = new CommonParam();
                string MessageSuccess = "";
                List<HIS_TREATMENT> treatmentResults = null;
                if (currentTreatment != null && currentTreatment.Count > 0)
                {
                    List<HIS_TREATMENT> lstMediRecordUpdate = new List<HIS_TREATMENT>();
                    foreach (var item in currentTreatment)
                    {
                        HIS_TREATMENT hisMediRecord = new HIS_TREATMENT();
                        Mapper.CreateMap<V_HIS_TREATMENT_9, HIS_TREATMENT>();
                        hisMediRecord = Mapper.Map<V_HIS_TREATMENT_9, HIS_TREATMENT>(item);
                        hisMediRecord.DATA_STORE_ID = dataChecks.FirstOrDefault().ID;
                        lstMediRecordUpdate.Add(hisMediRecord);
                    }
                    treatmentResults = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/UpdateListDataStoreId", ApiConsumers.MosConsumer, lstMediRecordUpdate, param);
                    if (treatmentResults != null && treatmentResults.Count > 0)
                    {
                        result = true;
                        foreach (var item in treatmentResults)
                        {
                            MessageSuccess += item.STORE_CODE + ", ";
                        }
                        this.refeshData();

                    }
                }

                #region Show message
                if (treatmentResults != null && treatmentResults.Count > 0)
                {
                    MessageManager.Show("Xử lý thành công. Số lưu trữ của hồ sơ là: " + MessageSuccess);
                }
                else
                {
                    MessageManager.Show(this.ParentForm, param, result);
                }
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (result)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcessByMediRecordCode(PrintTypeMediRecord.IN_BARCODE_MEDI_RECORD_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetRadioCheckedState2(TreeListNode node)
        {
            try
            {
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    if (node.ParentNode.Nodes[i] == node)
                    {
                        node.Selected = true;
                        var data = (V_HIS_DATA_STORE)node.Tag;
                        if (data != null)
                        {
                            //dataStoreId = data.ID;
                        }
                    }
                    else
                    {
                        node.ParentNode.Nodes[i].Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveData_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.gridViewDataStore.FocusedRowHandle = 0;
                    this.gridViewDataStore.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewDataStore.FocusedRowHandle = 0;
                    this.gridViewDataStore.Focus();
                }

                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    var focus = (DataStoreADO)gridViewDataStore.GetFocusedRow();
                    var dataSource = (List<DataStoreADO>)gridControlDataStore.DataSource;
                    UpdateStatus(focus, dataSource);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UpdateStatus(DataStoreADO focus, List<DataStoreADO> dataSource)
        {
            try
            {
                if (focus != null && dataSource != null && dataSource.Count > 0)
                {
                    Parallel.ForEach(dataSource.Where(f => f.ID == focus.ID), l => l.CheckStore = true);
                }

                gridViewDataStore.BeginDataUpdate();
                gridControlDataStore.DataSource = dataSource;
                gridViewDataStore.EndDataUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(this.txtKeyword.Text))
                {
                    string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                    var DataStoreTemp = this.DataStoreADOs.Where(o => o.DATA_STORE_NAME_HIDEN.ToLower().Contains(keyword)).ToList();

                    bool isAutoFocus = false;
                    if (DataStoreTemp.Count == 1)
                    {
                        foreach (var ur in DataStoreTemp)
                        {
                            ur.CheckStore = true;
                        }
                        isAutoFocus = true;
                    }

                    this.gridControlDataStore.DataSource = DataStoreTemp;
                    this.btnSaveData.Enabled = (DataStoreTemp != null && DataStoreTemp.Count > 0);
                    if (isAutoFocus)
                    {
                        this.gridViewDataStore.FocusedRowHandle = 0;
                        btnSaveData.Focus();
                    }
                }
                else
                {
                    this.btnSaveData.Enabled = true;
                    gridControlDataStore.BeginUpdate();
                    Parallel.ForEach(this.DataStoreADOs.Where(f => f.ID > 0), l => l.CheckStore = false);
                    gridControlDataStore.DataSource = this.DataStoreADOs;
                    gridControlDataStore.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

        }

        private void gridViewDataStore_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }

                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }

                    if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "CheckStore")
                        {
                            var focus = (DataStoreADO)view.GetRow(hi.RowHandle);
                            var dataSource = (List<DataStoreADO>)gridControlDataStore.DataSource;
                            List<DataStoreADO> newDataSource = dataSource;
                            Parallel.ForEach(newDataSource.Where(f => f.ID > 0), l => l.CheckStore = false);
                            foreach (var item in newDataSource)
                            {
                                if (item.ID == focus.ID)
                                {
                                    item.CheckStore = true;
                                }
                                //else
                                //{
                                //    item.CheckStore = false;
                                //}
                            }
                            gridControlDataStore.BeginUpdate();
                            gridControlDataStore.DataSource = null;
                            gridControlDataStore.DataSource = newDataSource;
                            gridControlDataStore.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewDataStore_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var row = (DataStoreADO)gridViewDataStore.GetRow(e.RowHandle);
                if (row != null && row.CheckStore)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.FontStyleDelta = FontStyle.Bold;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
