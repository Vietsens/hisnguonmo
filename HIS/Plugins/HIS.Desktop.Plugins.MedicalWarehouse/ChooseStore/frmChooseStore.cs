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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.MedicalWarehouse.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalWarehouse.ChooseStore
{
    public partial class frmChooseStore : Form
    {
        internal long dataStoreId;
        internal V_HIS_MEDI_RECORD currentMediRecord { get; set; }
        HIS.Desktop.Common.RefeshReference refeshData;
        internal List<HisMediRecordADO> lstMediRecords { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmChooseStore()
        {
            InitializeComponent();
        }

        public frmChooseStore(Inventec.Desktop.Common.Modules.Module currentModule,V_HIS_MEDI_RECORD currentMediRecord, RefeshReference refeshData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentMediRecord = currentMediRecord;
                this.refeshData = refeshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmChooseStore(Inventec.Desktop.Common.Modules.Module currentModule,List<HisMediRecordADO> lstMediRecords, RefeshReference refeshData)
        {
            InitializeComponent();
            try
            {
                this.lstMediRecords = lstMediRecords;
                this.refeshData = refeshData;
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
                LoaddataToTreeList(this);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicalWarehouse.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicalWarehouse.ChooseStore.frmChooseStore).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveData.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.btnSaveData.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmChooseStore.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmChooseStore.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void LoaddataToTreeList(frmChooseStore control)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDataStoreViewFilter dataStoreFilter = new MOS.Filter.HisDataStoreViewFilter();
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_DATA_STORE>>(HisRequestUriStore.HIS_DATA_STORE_GETVIEW, ApiConsumers.MosConsumer, dataStoreFilter, param).Where(p => p.ROOM_ID == this.currentModule.RoomId || p.ROOM_TYPE_ID == this.currentModule.RoomTypeId).ToList();
                var lstParent = currentDataStore.Where(o => o.PARENT_ID == null).ToList();
                if (lstParent != null)
                {
                    treeListDataStore.Nodes.Clear();
                    TreeListNode parentForRootNodes = null;
                    foreach (var item in lstParent)
                    {
                        TreeListNode rootNode = treeListDataStore.AppendNode(
                     new object[] { item.DATA_STORE_NAME },
                     parentForRootNodes, item);
                        CreateChildNode(rootNode, item, currentDataStore, control);
                    }
                    control.treeListDataStore.ExpandToLevel(0);
                    control.treeListDataStore.OptionsBehavior.ImmediateEditor = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void CreateChildNode(TreeListNode rootNode, V_HIS_DATA_STORE hisDataStore, List<V_HIS_DATA_STORE> hisDataStores, frmChooseStore control)
        {
            try
            {
                var listChilds = hisDataStores.Where(o => o.PARENT_ID == hisDataStore.ID).ToList();
                if (listChilds != null && listChilds.Count > 0)
                {
                    foreach (var itemChild in listChilds)
                    {
                        TreeListNode childNode = control.treeListDataStore.AppendNode(
                        new object[] { itemChild.DATA_STORE_NAME },
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

        private void treeListDataStore_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (e.Node.Checked)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    e.Appearance.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDataStore_CustomDrawNodeCheckBox(object sender, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                CheckObjectInfoArgs arg = new CheckObjectInfoArgs(DevExpress.Utils.AppearanceObject.ControlAppearance);
                arg.CheckState = e.Node.CheckState;
                arg.Graphics = e.Graphics;
                arg.Bounds = e.Bounds;
                e.Painter.CalcObjectBounds(arg);

                arg.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;

                arg.State = ObjectState.Hot;  //e.ObjectArgs.State;
                e.Painter.DrawObject(arg);
                e.Handled = true;
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
                var hitInfo = treeListDataStore.CalcHitInfo(e.Location);
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Empty)
                    return;
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                    return;
                treeListDataStore.UncheckAll();
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Cell)
                {
                    if (hitInfo.Node.Level == 0)
                    {
                        if (hitInfo.Node.Checked == false)
                        {
                            for (int i = 0; i < hitInfo.Node.Nodes.Count; i++)
                            {
                                hitInfo.Node.Nodes[i].Checked = false;
                            }
                            hitInfo.Node.Checked = true;
                            var data = (V_HIS_DATA_STORE)hitInfo.Node.Tag;
                            if (data != null)
                            {
                                dataStoreId = data.ID;
                            }
                        }
                        else
                        {
                            hitInfo.Node.Checked = false;
                        }
                    }
                    else
                    {
                        hitInfo.Node.ParentNode.Checked = false;
                        SetRadioCheckedState(hitInfo.Node);
                    }
                }
                else if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.NodeCheckBox)
                {
                    if (hitInfo.Node.Level == 0)
                    {
                        if (hitInfo.Node.Checked == false)
                        {
                            for (int i = 0; i < hitInfo.Node.Nodes.Count; i++)
                            {
                                hitInfo.Node.Nodes[i].Checked = false;
                            }
                            hitInfo.Node.Selected = true;
                            var data = (V_HIS_DATA_STORE)hitInfo.Node.Tag;
                            if (data != null)
                            {
                                dataStoreId = data.ID;
                            }
                        }
                        else
                        {
                            hitInfo.Node.Checked = false;
                        }
                    }
                    else
                    {
                        hitInfo.Node.ParentNode.Checked = false;
                        SetRadioCheckedState2(hitInfo.Node);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetRadioCheckedState(TreeListNode node)
        {
            try
            {
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    if (node.ParentNode.Nodes[i] == node)
                    {
                        node.Checked = true;
                        var data = (V_HIS_DATA_STORE)node.Tag;
                        if (data != null)
                        {
                            dataStoreId = data.ID;
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

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataStoreId == 0 || dataStoreId == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho dữ liệu", "Thông báo");
                    return;
                }
                bool result = false;
                CommonParam param = new CommonParam();

                if (lstMediRecords != null && lstMediRecords.Count > 0)
                {
                    List<HIS_MEDI_RECORD> lstMediRecordUpdate = new List<HIS_MEDI_RECORD>();
                    foreach (var item in lstMediRecords)
                    {
                        HIS_MEDI_RECORD hisMediRecord = new HIS_MEDI_RECORD();
                        Mapper.CreateMap<HisMediRecordADO, HIS_MEDI_RECORD>();
                        hisMediRecord = Mapper.Map<HisMediRecordADO, HIS_MEDI_RECORD>(item);
                        hisMediRecord.DATA_STORE_ID = dataStoreId;
                        lstMediRecordUpdate.Add(hisMediRecord);
                    }
                    var current = new BackendAdapter(param).Post<List<HIS_MEDI_RECORD>>(HisRequestUriStore.HIS_MEDI_RECORD_UPDATE_LIST_DATA_STORE_ID, ApiConsumers.MosConsumer, lstMediRecordUpdate, param);
                    if (current != null)
                    {
                        result = true;
                        this.refeshData();
                    }
                }
                else
                {
                    HIS_MEDI_RECORD hisMediRecord = new HIS_MEDI_RECORD();
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, HIS_MEDI_RECORD>();
                    hisMediRecord = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, HIS_MEDI_RECORD>(currentMediRecord);
                    hisMediRecord.DATA_STORE_ID = dataStoreId;
                    var current = new BackendAdapter(param).Post<HIS_MEDI_RECORD>(HisRequestUriStore.HIS_MEDI_RECORD_UPDATE_DATA_STORE_ID, ApiConsumers.MosConsumer, hisMediRecord, param);
                    if (current != null)
                    {
                        result = true;
                        this.refeshData();
                    }
                }

                #region Show message
                MessageManager.Show(param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
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

        private void treeListDataStore_Click(object sender, EventArgs e)
        {
            try
            {

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
                            dataStoreId = data.ID;
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
    }
}
