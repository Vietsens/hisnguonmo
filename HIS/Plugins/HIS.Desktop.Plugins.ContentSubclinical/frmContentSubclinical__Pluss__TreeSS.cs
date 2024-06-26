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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ContentSubclinical.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ContentSubclinical
{
    public partial class frmContentSubclinical : HIS.Desktop.Utility.FormBase
    {
        private void treeListServiceReq_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                //SetCheckChildNode(e.Node, e.Node.CheckState);
                // SetCheckParentNode(e.Node, e.Node.CheckState);
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

        private void treeListServiceReq_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                //if (e.Node.HasChildren)
                //{
                ////e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                if (e.Node.Checked)
                {
                    e.Node.UncheckAll();
                }
                else
                {
                    e.Node.CheckAll();
                }
                TreeListNode node = e.Node;
                CheckNodesParent(node);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_CustomDrawNodeCheckBox(object sender, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                //var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(e.Node);
                //if (data != null && data.IsLeaf)
                //{
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<TreeSereServADO> GetListCheck()
        {
            List<TreeSereServADO> result = new List<TreeSereServADO>();
            try
            {
                foreach (TreeListNode node in treeListServiceReq.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<TreeSereServADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<TreeSereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((TreeSereServADO)treeListServiceReq.GetDataRecordByNode(node));
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

        public List<TreeSereServADO> GetListCheckAndIndeterminate()
        {
            List<TreeSereServADO> result = new List<TreeSereServADO>();
            try
            {
                foreach (TreeListNode node in treeListServiceReq.Nodes)
                {
                    GetListNodeCheckAndIndeterminate(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<TreeSereServADO>();
            }
            return result;
        }

        private void GetListNodeCheckAndIndeterminate(ref List<TreeSereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Checked || node.CheckState == CheckState.Indeterminate)
                {
                    result.Add((TreeSereServADO)treeListServiceReq.GetDataRecordByNode(node));
                }
                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheckAndIndeterminate(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<TreeSereServADO> GetListCheck_SereServForPrint()
        {
            List<TreeSereServADO> result = new List<TreeSereServADO>();
            try
            {
                foreach (TreeListNode node in treeListServiceReq.Nodes)
                {
                    GetListNodeCheck_SereServForPrint(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<TreeSereServADO>();
            }
            return result;
        }

        private void GetListNodeCheck_SereServForPrint(ref List<TreeSereServADO> result, TreeListNode node)
        {
            try
            {
                var nodeData = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(node);
                if (nodeData.IS_SERE_SERV_DATA)
                {
                    if (node.Checked)
                    {
                        result.Add(nodeData);
                    }
                }

                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheck_SereServForPrint(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Node.HasChildren)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (data.IsLeaf)
                    {
                        if (data.IS_LOWER == true && data.IS_HIGHER == true)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else if (data.IS_LOWER == true)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else if (data.IS_HIGHER == true)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //typeCheck: 1 tất cả, 2 chỉ số xét nghiệm, 3 - phẫu thuật thủ thuật
        private void treeSereServ_CheckAllNode(TreeListNodes treeListNodes, bool checkConfig, int typeCheck)
        {
            try
            {
                if (treeListNodes != null)
                {
                    foreach (TreeListNode node in treeListNodes)
                    {
                        TreeSereServADO ado = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(node);
                        if (checkConfig
                            && (typeCheck == 3 || typeCheck == 1)
                            && ado != null
                            && (ado.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                            || ado.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT))
                        {
                            if (chkNotSelectSurg.Checked)
                            {
                                node.UncheckAll();
                            }
                            else
                            {
                                node.CheckAll();
                            }
                        }
                        else if (checkConfig && ado != null
                            && (typeCheck == 2 || typeCheck == 1)
                            && ado.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                            && ado.IsLeaf
                            && !ado.IS_IMPORTANT)
                        {
                            if (chkJustSelectIndexImportant.Checked)
                            {
                                node.UncheckAll();
                            }
                            else
                            {
                                node.CheckAll();
                            }
                        }
                        else if (typeCheck == 1)
                        {
                            node.CheckAll();
                        }
                        CheckNode(node, checkConfig, typeCheck);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNode(TreeListNode node, bool checkConfig, int typeCheck)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        TreeSereServADO ado = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(childNode);
                        if (checkConfig
                            && (typeCheck == 3 || typeCheck == 1)
                            && ado != null
                            && (ado.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                            || ado.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT))
                        {
                            if (chkNotSelectSurg.Checked)
                            {
                                childNode.UncheckAll();
                            }
                            else
                            {
                                childNode.CheckAll();
                            }
                        }
                        else if (checkConfig
                            && (typeCheck == 2 || typeCheck == 1)
                            && ado != null
                            && ado.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                            && ado.IsLeaf
                            && !ado.IS_IMPORTANT)
                        {
                            if (chkJustSelectIndexImportant.Checked)
                            {
                                childNode.UncheckAll();
                            }
                            else
                            {
                                childNode.CheckAll();
                            }
                        }
                        else if (typeCheck == 1)
                        {
                            childNode.CheckAll();
                        }
                        CheckNode(childNode, checkConfig, typeCheck);
                    }
                    CheckNodesParent(node);
                }
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
    }
}
