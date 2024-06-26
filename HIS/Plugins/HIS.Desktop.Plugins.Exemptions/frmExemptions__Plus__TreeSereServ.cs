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
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Exemptions
{
    public partial class frmExemptions : HIS.Desktop.Utility.FormBase
    {
        List<SereServADO> _ListChecks = new List<SereServADO>();
        List<SereServADO> SereServADOs = new List<SereServADO>();
        System.ComponentModel.BindingList<SereServADO> records = null;
        private void BindTreePlus(List<V_HIS_SERE_SERV_5> _sereServs)
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                if (_sereServs != null)
                {
                    var sereServs = (from r in _sereServs select new SereServADO(r)).ToList();
                    List<SereServADO> listSereServExpend = new List<SereServADO>();
                    if (true)
                    {
                        listSereServExpend = sereServs.Where(o => o.IsExpend.HasValue && o.IsExpend.Value).ToList();
                        sereServs = sereServs.Where(o => o.IsExpend != true).ToList();
                    }
                    if (listSereServExpend != null && listSereServExpend.Count > 0)
                    {
                        SereServADO ssRootExpend = new SereServADO();
                        ssRootExpend.TDL_SERVICE_NAME = "Hao phí";
                        ssRootExpend.CONCRETE_ID__IN_SETY = "Expend";
                        SereServADOs.Add(ssRootExpend);
                        var listGroupBySety = listSereServExpend.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var group in listGroupBySety)
                        {
                            var listSub = group.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.TDL_SERVICE_NAME = listSub.First().TDL_SERVICE_NAME;
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY + "_" + listSub.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY;
                            SereServADOs.Add(ssRootSety);
                            foreach (var item in listSub)
                            {
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                SereServADOs.Add(item);
                            }
                        }
                    }
                    var listRoot = sereServs.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var rootPaty in listRoot)
                    {
                        var listByPaty = rootPaty.ToList<SereServADO>();
                        SereServADO ssRootPaty = new SereServADO();
                        ssRootPaty.CONCRETE_ID__IN_SETY = listByPaty.First().PATIENT_TYPE_ID + "";
                        //ssRootPaty.SERVICE_CODE = listByPaty.First().PATIENT_TYPE_CODE;
                        ssRootPaty.TDL_SERVICE_NAME = listByPaty.First().PATIENT_TYPE_NAME;
                        ssRootPaty.PATIENT_TYPE_ID = listByPaty.First().PATIENT_TYPE_ID;
                        SereServADOs.Add(ssRootPaty);
                        var listRootSety = listByPaty.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY + "_" + listBySety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY;
                            ssRootSety.PATIENT_TYPE_ID = ssRootPaty.PATIENT_TYPE_ID;
                            ssRootSety.TDL_SERVICE_NAME = listBySety.First().SERVICE_TYPE_NAME;
                            SereServADOs.Add(ssRootSety);
                            foreach (var item in listBySety)
                            {
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                SereServADOs.Add(item);
                            }
                        }
                    }
                    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenByDescending(o => o.TDL_SERVICE_CODE).ToList();
                }
                records = new System.ComponentModel.BindingList<SereServADO>(SereServADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                //  if (this.sereServTree_CheckAllNode != null)
                if (this._currentServSer != null && this._currentServSer.ID > 0)
                {
                    if (trvService.Nodes != null)
                    {
                        foreach (TreeListNode item in trvService.Nodes)
                        {
                            CheckNodeBySereServ(item);
                        }
                    }

                }
                else
                    this.treeSereServ_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.VIR_TOTAL_PATIENT_PRICE_NO_DC.HasValue && data.VIR_TOTAL_PATIENT_PRICE_NO_DC.Value >= 0)
                    {
                        if (e.Node.Checked)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                    trvService.RefreshDataSource();
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
                        childNode.CheckAll();
                        CheckNode(childNode);
                        if (!childNode.HasChildren)
                        {
                            var data = (SereServADO)childNode.TreeList.GetDataRecordByNode(childNode);
                            if (childNode.Checked)
                            {
                                if (isReloadTree && data.DISCOUNT.HasValue) //http://redmine.vietsens.vn:8080/redmine/issues/45346
                                {
                                    data.VIR_TOTAL_DISCOUNT = data.DISCOUNT;
                                }
                                else
                                {
                                    data.VIR_TOTAL_DISCOUNT = data.VIR_TOTAL_PATIENT_PRICE_NO_DC / 100 * spinEditTyLe.Value;
                                }
                            }
                            else
                            {
                                data.VIR_TOTAL_DISCOUNT = data.DISCOUNT;
                            }
                        }
                    }
                    CheckNodesParent(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTyLe(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    if (!node.HasChildren)
                    {
                        var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                        if (nodeData.IsLeaf.Value)
                        {
                            if (node.Checked) // http://redmine.vietsens.vn:8080/redmine/issues/45346
                            {
                                if (isReloadTree && nodeData.DISCOUNT.HasValue)
                                {
                                    nodeData.VIR_TOTAL_DISCOUNT = nodeData.DISCOUNT;

                                }
                                else
                                {
                                    nodeData.VIR_TOTAL_DISCOUNT = nodeData.VIR_TOTAL_PATIENT_PRICE_NO_DC / 100 * spinEditTyLe.Value;
                                }
                            }
                            else
                            {
                                nodeData.VIR_TOTAL_DISCOUNT = nodeData.DISCOUNT;
                            }
                            trvService.RefreshNode(node);
                        }
                    }
                    else
                    {
                        foreach (TreeListNode childNode in node.Nodes)
                        {
                            ProcessTyLe(childNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNodeBySereServ(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        // childNode.CheckAll();
                        CheckNodeBySereServ(childNode);
                        if (!childNode.HasChildren)
                        {
                            var data = (SereServADO)childNode.TreeList.GetDataRecordByNode(childNode);
                            if (data.ID == this._currentServSer.ID)
                            {
                                childNode.Checked = true;
                            }
                            if (childNode.Checked)
                            {
                                if (isReloadTree && data.DISCOUNT.HasValue) //http://redmine.vietsens.vn:8080/redmine/issues/45346
                                {
                                    data.VIR_TOTAL_DISCOUNT = data.DISCOUNT;
                                }
                                else
                                {
                                    data.VIR_TOTAL_DISCOUNT = data.VIR_TOTAL_PATIENT_PRICE_NO_DC / 100 * spinEditTyLe.Value;
                                }
                            }
                            else
                            {
                                data.VIR_TOTAL_DISCOUNT = data.DISCOUNT;
                            }
                        }
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

        //private void CheckNode(TreeListNode node)
        //{
        //    try
        //    {
        //        if (node != null)
        //        {
        //            foreach (TreeListNode childNode in node.Nodes)
        //            {
        //                var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
        //                if (nodeData != null)
        //                {
        //                    //if (this.currentTreatment.IS_PAUSE != 1)
        //                    //{
        //                    //    childNode.UncheckAll();
        //                    //    CheckNode(childNode);
        //                    //}
        //                    //else
        //                    //{
        //                    childNode.CheckAll();
        //                    CheckNode(childNode);
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void treeSereServ_BeforeCheckNode(TreeListNode node, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (node != null)
                {
                    var nodeData = node.TreeList.GetDataRecordByNode(node);
                    //if (nodeData != null && this.currentTreatment.IS_PAUSE != 1)
                    //{
                    //    e.CanCheck = false;
                    //    node.UncheckAll();
                    //    return;
                    //}
                    e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                    if (node.Checked)
                    {
                        node.UncheckAll();
                    }
                    else
                    {
                        node.CheckAll();
                    }
                    while (node.ParentNode != null)
                    {
                        node = node.ParentNode;
                        bool valid = false;
                        foreach (DevExpress.XtraTreeList.Nodes.TreeListNode item in node.Nodes)
                        {
                            if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                            {
                                valid = true;
                                break;
                            }
                        }
                        if (valid)
                        {
                            node.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            node.CheckState = CheckState.Unchecked;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_AfterCheckNode(TreeListNode node, SereServADO data)
        {
            try
            {
                if (!node.HasChildren)
                {
                    if (node.Checked)
                    {
                        if (isReloadTree && data.DISCOUNT.HasValue)//http://redmine.vietsens.vn:8080/redmine/issues/45346
                        {
                            data.VIR_TOTAL_DISCOUNT = data.DISCOUNT;
                        }
                        else
                        {
                            data.VIR_TOTAL_DISCOUNT = data.VIR_TOTAL_PATIENT_PRICE_NO_DC / 100 * spinEditTyLe.Value;
                        }
                    }
                    else
                    {
                        data.VIR_TOTAL_DISCOUNT = data.DISCOUNT;
                    }
                    trvService.RefreshDataSource();
                }
                else
                {
                    ProcessTyLe(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<SereServADO> GetListCheck()
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                foreach (TreeListNode node in trvService.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<SereServADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<SereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((SereServADO)trvService.GetDataRecordByNode(node));
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

        private void treeSereServ_CustomDrawNodeCheckBox(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                //if (data != null && this.currentTreatment.IS_PAUSE != 1)
                //{
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                //if (data != null && !e.Node.HasChildren)
                //{
                //    if (e.Column.FieldName == "AMOUNT_DISPLAY")
                //    {
                //        e.Value = ConvertNumberToString(data.AMOUNT);
                //    }
                //    else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                //    {
                //        e.Value = ConvertNumberToString(data.VIR_PRICE ?? 0);
                //    }
                //    else if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                //    {
                //        e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                //    }
                //    else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                //    {
                //        e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                //    }
                //    else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                //    {
                //        e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE_NO_DC ?? 0);
                //    }
                //    else if (e.Column.FieldName == "VIR_TOTAL_DISCOUNT")
                //    {
                //        e.Value = ConvertNumberToString(data.VIR_TOTAL_DISCOUNT ?? 0);
                //    }
                //    else if (e.Column.FieldName == "VAT_DISPLAY")
                //    {
                //        e.Value = ConvertNumberToString(data.VAT);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertNumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Number.Convert.NumberToString(number, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
