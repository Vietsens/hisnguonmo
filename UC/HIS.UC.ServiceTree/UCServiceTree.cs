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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;

namespace HIS.UC.ServiceTree
{
    public partial class UCServiceTree : UserControl
    {
        #region Declare
        ServiceTreeADO serviceTreeADO;
        BindingList<ServiceADO> records;
        List<ServiceADO> serviceADOs = new List<ServiceADO>();
        ServiceTree_CustomUnboundColumnData serviceTree_CustomUnboundColumnData;
        List<ColumnButtonEditADO> columnButtonEdits;
        ServiceTree_NodeCellStyle serviceNodeCellStyle;
        ServiceHandler serviceTreeClick;

        ServiceTree_GetStateImage ServiceTree_GetStateImage;
        ServiceTree_GetSelectImage ServiceTree_GetSelectImage;
        ServiceHandler ServiceTree_StateImageClick;
        ServiceHandler ServiceTree_SelectImageClick;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        ServiceHandler updateSingleRow;
        MenuItems menuItems;
        ServiceTree_CustomNodeCellEdit serviceTree_CustomNodeCellEdit;

        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        #endregion

        #region Construct
        public UCServiceTree(ServiceTreeADO servicetreeADO)
        {
            InitializeComponent();
            try
            {
                serviceTreeADO = servicetreeADO;
                serviceNodeCellStyle = serviceTreeADO.ServiceNodeCellStyle;
                serviceTreeClick = serviceTreeADO.ServiceTreeClick;


                this.ServiceTree_GetStateImage = servicetreeADO.ServiceTree_GetStateImage;
                this.ServiceTree_GetSelectImage = servicetreeADO.ServiceTree_GetSelectImage;
                this.ServiceTree_StateImageClick = servicetreeADO.ServiceTree_StateImageClick;
                this.ServiceTree_SelectImageClick = servicetreeADO.ServiceTree_SelectImageClick;
                this.columnButtonEdits = servicetreeADO.ColumnButtonEdits;
                this.selectImageCollection = servicetreeADO.SelectImageCollection;
                this.stateImageCollection = servicetreeADO.StateImageCollection;
                this.updateSingleRow = servicetreeADO.UpdateSingleRow;
                this.serviceTree_CustomUnboundColumnData = servicetreeADO.ServiceTree_CustomUnboundColumnData;
                this.menuItems = servicetreeADO.MenuItems;
                this.serviceTree_CustomNodeCellEdit = servicetreeADO.ServiceTree_CustomNodeCellEdit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void UCServiceTree_Load(object sender, EventArgs e)
        {
            try
            {
                if (serviceTreeADO != null)
                {
                    BindTree();
                    SetVisibleColumn();
                    SetVisibleSearchPanel();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BindTree()
        {
            try
            {
                //if (!String.IsNullOrEmpty(serviceTreeADO.KeyFieldName))
                //{
                //    trvService.KeyFieldName = serviceTreeADO.KeyFieldName;
                //}
                //if (!String.IsNullOrEmpty(serviceTreeADO.ParentFieldName))
                //{
                //    trvService.ParentFieldName = serviceTreeADO.ParentFieldName;
                //}

                serviceADOs = (from m in serviceTreeADO.Services select new ServiceADO(m)).ToList();
                records = new BindingList<ServiceADO>(serviceADOs);
                trvService.DataSource = records;

                if (this.stateImageCollection != null)
                {
                    trvService.StateImageList = this.stateImageCollection;
                }
                if (this.selectImageCollection != null)
                {
                    trvService.SelectImageList = this.selectImageCollection;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetVisibleColumn()
        {
            try
            {
                if (serviceTreeADO.ServiceTreeColumns != null && serviceTreeADO.ServiceTreeColumns.Count > 0)
                {
                    foreach (var svtr in serviceTreeADO.ServiceTreeColumns)
                    {
                        trvService.Columns[svtr.FieldName].Visible = svtr.Visible;
                        trvService.Columns[svtr.FieldName].VisibleIndex = svtr.VisibleIndex;
                        trvService.Columns[svtr.FieldName].Width = svtr.ColumnWidth;
                        trvService.Columns[svtr.FieldName].FieldName = svtr.FieldName;
                        trvService.Columns[svtr.FieldName].OptionsColumn.AllowEdit = svtr.AllowEdit;
                        trvService.Columns[svtr.FieldName].Caption = svtr.Caption;
                        if (svtr.Format != null)
                        {
                            trvService.Columns[svtr.FieldName].Format.FormatString = svtr.Format.FormatString;
                            trvService.Columns[svtr.FieldName].Format.FormatType = svtr.Format.FormatType;
                        }
                    }
                }
                if (serviceTreeADO.ColumnButtonEdits != null && serviceTreeADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in serviceTreeADO.ColumnButtonEdits)
                    {
                        RepositoryItemButtonEdit buttonEdit = new RepositoryItemButtonEdit();
                        buttonEdit.AutoHeight = false;
                        buttonEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                        DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
                        superToolTip.Items.Add(new DevExpress.Utils.ToolTipItem());
                        buttonEdit.Tag = svtr.ActionHandler;
                        buttonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph,svtr.Image,superToolTip)});
                        buttonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(RepositoryItemButtonClick);
                        DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();

                        treeListColumn.ColumnEdit = buttonEdit;
                        treeListColumn.Width = 20;
                        treeListColumn.ToolTip = svtr.Tooltip;
                        treeListColumn.VisibleIndex = svtr.VisibleIndex;
                        treeListColumn.Caption = svtr.Caption;
                        treeListColumn.ImageAlignment = StringAlignment.Center;
                        trvService.Columns.Add(treeListColumn);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RepositoryItemButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (sender != null && sender is DevExpress.XtraEditors.ButtonEdit)
                {
                    var btn = sender as DevExpress.XtraEditors.ButtonEdit;
                    if (btn.Tag != null)
                    {
                        ServiceHandler clickhandler = btn.Tag as ServiceHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is ServiceADO)
                            {
                                clickhandler((ServiceADO)data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetVisibleSearchPanel()
        {
            try
            {
                if (serviceTreeADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (serviceTreeADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Search();
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
                Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<ServiceADO> GetChild(BindingList<ServiceADO> listData, ServiceADO parent)
        {
            List<ServiceADO> result = new List<ServiceADO>();
            var childData = listData.Where(o => o.PARENT_ID__IN_SETY == parent.CONCRETE_ID__IN_SETY).ToList();
            if (childData != null && childData.Count > 0)
            {
                result.AddRange(childData);
                foreach (var item in childData)
                {
                    var childChild = GetChild(listData, item);
                    if (childChild != null && childChild.Count > 0)
                    {
                        result.AddRange(childChild);
                    }
                }
            }
            return result;
        }
        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                BindingList<ServiceADO> listResult = null;
                if (!String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                {
                    List<ServiceADO> rearchResult = new List<ServiceADO>();

                    var listParentSearch = serviceADOs.Where(o =>
                                                    (o.SERVICE_NAME.ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                                                    || o.SERVICE_CODE.ToLower().Contains(txtKeyword.Text.Trim().ToLower()))
                                                    ).ToList();


                    if (listParentSearch != null)
                    {
                        rearchResult.AddRange(listParentSearch);
                        foreach (var item in listParentSearch)
                        {
                            var childs = GetChild(records, item);
                            if (childs != null && childs.Count > 0)
                            {
                                rearchResult.AddRange(childs);
                            }
                        }
                    }

                    rearchResult = rearchResult.Distinct().ToList();
                    listResult = new BindingList<ServiceADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<ServiceADO>(records);
                }
                trvService.DataSource = listResult;
                trvService.CollapseAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (serviceNodeCellStyle != null)
                {
                    serviceNodeCellStyle(data, e.Appearance);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var node = trvService.FocusedNode;
                    var data = trvService.GetDataRecordByNode(node);
                    if (node != null && node.HasChildren && data != null && data is ServiceADO)
                    {
                        node.Expanded = !node.Expanded;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is ServiceADO)
                {
                    e.Node.Checked = !e.Node.Checked;
                    serviceTreeClick((ServiceADO)row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    ServiceADO currentRow = e.Row as ServiceADO;
                    if (currentRow == null) return;
                    this.serviceTree_CustomUnboundColumnData(currentRow, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_SelectImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is ServiceADO)
                {
                    if (this.ServiceTree_SelectImageClick != null)
                        this.ServiceTree_SelectImageClick((ServiceADO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is ServiceADO)
                {
                    if (this.ServiceTree_GetSelectImage != null)
                        this.ServiceTree_GetSelectImage((ServiceADO)data, e);
                    else
                        e.NodeImageIndex = -1;
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is ServiceADO)
                {
                    if (this.ServiceTree_GetStateImage != null)
                        this.ServiceTree_GetStateImage((ServiceADO)data, e);
                    else
                        e.NodeImageIndex = -1;
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_StateImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is ServiceADO)
                {
                    if (this.ServiceTree_StateImageClick != null)
                        this.ServiceTree_StateImageClick((ServiceADO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (this.menuItems != null)
                {
                    TreeList tree = sender as TreeList;
                    if (tree != null)
                    {
                        Point pt = tree.PointToClient(MousePosition);
                        TreeListHitInfo hitInfo = tree.CalcHitInfo(e.Point);
                        if (hitInfo != null && (hitInfo.HitInfoType == HitInfoType.Row
                            || hitInfo.HitInfoType == HitInfoType.Cell))
                        {
                            e.Menu.Items.Clear();
                            tree.FocusedNode = hitInfo.Node;
                            var data = trvService.GetDataRecordByNode(hitInfo.Node);
                            if (data != null && data is ServiceADO)
                            {
                                foreach (var menu in this.menuItems((ServiceADO)data))
                                {
                                    e.Menu.Items.Add(menu);
                                }
                                e.Menu.Show(pt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is ServiceADO)
                {
                    var rowData = data as ServiceADO;
                    if (rowData != null && this.serviceTree_CustomNodeCellEdit != null)
                    {
                        this.serviceTree_CustomNodeCellEdit(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
