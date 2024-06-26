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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AggrApprove.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrApprove
{
    public partial class frmAggrApprove : HIS.Desktop.Utility.FormBase
    {
        private void treeListAggrApprove_CustomDrawNodeCheckBox(object sender, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                var dataTree = (ExpMestApproveADO)treeListAggrApprove.GetDataRecordByNode(e.Node);
                if (dataTree != null && dataTree.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                {
                    e.Handled = true;
                    //if (!e.Node.HasChildren)
                    //{
                    //    e.Handled = true;
                    //}
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListAggrApprove_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (e.Node.HasChildren)
                {
                    e.Appearance.ForeColor = Color.Black;
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

        private void treeListAggrApprove_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                treeListAggrApprove.FocusedNode = e.Node;
                if (e.Node.Checked)
                {
                    e.Node.UncheckAll();
                    if (e.Node.ParentNode == null)
                    {
                        gridControlMediMate.DataSource = null;
                        return;
                    }
                    else
                    {
                        LoadDataMetyMatyReq();
                    }
                }
                else
                {
                    e.Node.CheckAll();
                    LoadDataMetyMatyReq();
                }
                TreeListNode node = e.Node;
                CheckNodesParent(node);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNodesParent(TreeListNode node)
        {
            if (node != null)
            {
                //LoadDataMetyMatyReq();
                while (node.ParentNode != null)
                {
                    node = node.ParentNode;
                    bool hasCheck = false;
                    bool allCheck = true;
                    foreach (TreeListNode item in node.Nodes)
                    {
                        if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                        {
                            hasCheck = true;
                        }
                        if (item.CheckState == CheckState.Unchecked || item.CheckState == CheckState.Indeterminate)
                        {
                            allCheck = false;
                        }
                    }
                    if (allCheck)
                    {
                        node.CheckState = CheckState.Checked;
                    }
                    else if (hasCheck)
                    {
                        node.CheckState = CheckState.Indeterminate;
                    }
                    else
                    {
                        node.CheckState = CheckState.Unchecked;
                    }
                }
            }
        }

        private void treeSereServ_CheckAllNode(TreeListNodes treeListNodes)
        {
            try
            {
                if (treeListNodes != null)
                {
                    foreach (TreeListNode node in treeListNodes)
                    {
                        node.CheckAll();
                        CheckNode(node);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNode(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        CheckNode(childNode);
                    }
                    CheckNodesParent(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<ExpMestApproveADO> GetListCheck()
        {
            List<ExpMestApproveADO> result = new List<ExpMestApproveADO>();
            try
            {
                foreach (TreeListNode node in treeListAggrApprove.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<ExpMestApproveADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<ExpMestApproveADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((ExpMestApproveADO)treeListAggrApprove.GetDataRecordByNode(node));
                    }
                }
                else
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheck(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListAggrApprove_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                var dataTree = (ExpMestApproveADO)treeListAggrApprove.GetDataRecordByNode(e.Node);
                if (dataTree != null && dataTree.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && dataTree.CONCRETE_ID__IN_SETY != null && dataTree.PARENT_ID__IN_SETY != null && dataTree.ID == 0)
                {
                    e.NodeImageIndex = 0;
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
