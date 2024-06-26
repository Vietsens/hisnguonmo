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
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Config;
using HIS.UC.SereServTree;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne : HIS.Desktop.Utility.FormBase
    {
        private void CheckAllNode()
        {
            try
            {
                foreach (TreeListNode node in treeListSereServ.Nodes)
                {
                    var nodeData = (VHisSereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null)
                    {
                        if (nodeData.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "1" && this.treatment.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                        {
                            node.UncheckAll();
                            CheckNode(node);
                        }
                        else if (nodeData.IsLeaf.HasValue  && HisConfig.MustFinishTreatmentForBill == "2" && this.treatment.IS_PAUSE != 1)
                        {
                            node.UncheckAll();
                            CheckNode(node);
                        }
                        else if ((nodeData.IsLeaf.HasValue && (nodeData.RecieptPrice == null || nodeData.RecieptPrice == 0) && checkNotInvoice.Checked))
                        {
                            node.UncheckAll();
                            CheckNode(node);
                        }
                        else if ((nodeData.IsLeaf.HasValue && (nodeData.InvoicePrice == null || nodeData.InvoicePrice == 0) && checkNotReciept.Checked))
                        {
                            node.UncheckAll();
                            CheckNode(node);
                        }
                        else
                        {
                            if (radioSGAll.Checked)
                            {
                                node.CheckAll();
                                CheckNode(node);
                            }
                            else if (clsPtServiceTypeIds.Contains(nodeData.TDL_SERVICE_TYPE_ID) && radioSGCLS.Checked)
                            {
                                node.CheckAll();
                                CheckNode(node);
                            }
                            else if (nodeData.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && radioSGExam.Checked)
                            {
                                node.CheckAll();
                                CheckNode(node);
                            }
                            else if (nodeData.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && radioSGMedicine.Checked)
                            {
                                node.CheckAll();
                                CheckNode(node);
                            }
                            else
                            {
                                node.UncheckAll();
                                CheckNode(node);
                            }
                            if (node.HasChildren)
                            {
                                this.ProcessChildNode(node);
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

        private void CheckNode(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        var nodeData = (VHisSereServADO)node.TreeList.GetDataRecordByNode(childNode);
                        if (nodeData != null)
                        {
                            if (nodeData.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "1" && this.treatment.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                            {
                                childNode.CheckState = CheckState.Unchecked;
                            }
                            else if (nodeData.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "2" && this.treatment.IS_PAUSE != 1)
                            {
                                childNode.CheckState = CheckState.Unchecked;
                            }
                            else if ((nodeData.IsLeaf.HasValue && (nodeData.RecieptPrice == null || nodeData.RecieptPrice == 0) && checkNotInvoice.Checked))
                            {
                                childNode.CheckState = CheckState.Unchecked;
                            }
                            else if ((nodeData.IsLeaf.HasValue && (nodeData.InvoicePrice == null || nodeData.InvoicePrice == 0) && checkNotReciept.Checked))
                            {
                                childNode.CheckState = CheckState.Unchecked;
                            }
                            else
                            {
                                if (radioSGAll.Checked)
                                {
                                    childNode.CheckAll();
                                    CheckNode(childNode);
                                }
                                else if (clsPtServiceTypeIds.Contains(nodeData.TDL_SERVICE_TYPE_ID) && radioSGCLS.Checked)
                                {
                                    childNode.CheckAll();
                                    CheckNode(childNode);
                                }
                                else if (nodeData.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && radioSGExam.Checked)
                                {
                                    childNode.CheckAll();
                                    CheckNode(childNode);
                                }
                                else if (nodeData.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && radioSGMedicine.Checked)
                                {
                                    childNode.CheckAll();
                                    CheckNode(childNode);
                                }
                                else
                                {
                                    childNode.UncheckAll();
                                    CheckNode(childNode);
                                }
                                if (childNode.HasChildren)
                                {
                                    this.ProcessChildNode(childNode);
                                }
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

        private void ProcessChildNode(TreeListNode parentNode)
        {
            try
            {
                if (parentNode.Nodes != null)
                {
                    if (parentNode.Nodes.Any(o => o.CheckState == CheckState.Indeterminate))
                    {
                        parentNode.CheckState = CheckState.Indeterminate;
                    }
                    else if (parentNode.Nodes.Any(o => !o.Checked))
                    {
                        if (parentNode.Nodes.Any(o => o.Checked))
                        {
                            parentNode.CheckState = CheckState.Indeterminate;
                        }
                        else
                        {
                            parentNode.CheckState = CheckState.Unchecked;
                        }
                    }
                    else
                    {
                        parentNode.CheckState = CheckState.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServ_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                var data = (VHisSereServADO)treeListSereServ.GetDataRecordByNode(e.Node);
                if (data != null && data.IsLeaf.HasValue)
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
                else if (data != null && e.Node.HasChildren)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
                    if (e.Node.ParentNode != null)
                    {
                        e.Appearance.BackColor = Color.Khaki;
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.Pink;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServ_CustomDrawNodeCheckBox(object sender, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    var data = (VHisSereServADO)treeListSereServ.GetDataRecordByNode(e.Node);
                    if (data != null)
                    {
                        if (!data.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "1"
                            && this.treatment.IS_PAUSE != 1
                            && data.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                        {
                            e.Handled = true;
                        }
                        else if (!data.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "2"
                            && this.treatment.IS_PAUSE != 1)
                        {
                            e.Handled = true;
                        }
                        else if (data.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "1" && this.treatment.IS_PAUSE != 1 && data.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                        {
                            e.Handled = true;
                        }
                        else if (data.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "2" && this.treatment.IS_PAUSE != 1)
                        {
                            e.Handled = true;
                        }
                        else if ((data.IsLeaf.HasValue && (data.RecieptPrice == null || data.RecieptPrice == 0) && checkNotInvoice.Checked))
                        {
                            e.Handled = true;
                        }
                        else if ((data.IsLeaf.HasValue && (data.InvoicePrice == null || data.InvoicePrice == 0) && checkNotReciept.Checked))
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServ_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                var node = e.Node;

                if (node != null)
                {
                    var nodeData = (VHisSereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null && !nodeData.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "1" && this.treatment.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
                    else if (nodeData != null && !nodeData.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "2" && this.treatment.IS_PAUSE != 1)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
                    else if (nodeData != null && nodeData.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "1" && this.treatment.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                    {
                        e.CanCheck = false;
                        return;
                    }
                    else if (nodeData != null && nodeData.IsLeaf.HasValue && HisConfig.MustFinishTreatmentForBill == "2" && this.treatment.IS_PAUSE != 1)
                    {
                        e.CanCheck = false;
                        return;
                    }
                    else if ((nodeData.IsLeaf.HasValue && (nodeData.RecieptPrice == null || nodeData.RecieptPrice == 0) && checkNotInvoice.Checked))
                    {
                        e.CanCheck = false;
                        return;
                    }
                    else if ((nodeData.IsLeaf.HasValue && (nodeData.InvoicePrice == null || nodeData.InvoicePrice == 0) && checkNotReciept.Checked))
                    {
                        e.CanCheck = false;
                        return;
                    }
                    if (node.Checked)
                    {
                        node.UncheckAll();
                    }
                    else
                    {
                        node.CheckState = CheckState.Checked;
                        if (node.HasChildren)
                            CheckNode(node);
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

        private void treeListSereServ_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                this.ProcessAfterCheckNode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void ProcessAfterCheckNode()
        {
            try
            {
                totalReciept = 0;
                totalInvoice = 0;
                var listData = this.GetListCheck();
                listRecieptData = new List<VHisSereServADO>();
                listInvoiceData = new List<VHisSereServADO>();
                if (listData != null && listData.Count > 0)
                {
                    foreach (var item in listData)
                    {
                        if ((!checkNotInvoice.Checked) && item.InvoicePrice > 0 && !item.IsInvoiced)
                        {
                            if (HisConfig.MustFinishTreatmentForBill == "1" && this.treatment.IS_PAUSE != 1 && item.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                            {
                                continue;
                            }
                            if (HisConfig.MustFinishTreatmentForBill == "2" && this.treatment.IS_PAUSE != 1)
                            {
                                continue;
                            }
                            totalInvoice += item.InvoicePrice ?? 0;
                            listInvoiceData.Add(item);
                        }
                        if ((!checkNotReciept.Checked) && item.RecieptPrice > 0 && !item.IsReciepted)
                        {
                            if (HisConfig.MustFinishTreatmentForBill == "1" && this.treatment.IS_PAUSE != 1 && item.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                            {
                                continue;
                            }
                            if (HisConfig.MustFinishTreatmentForBill == "2" && this.treatment.IS_PAUSE != 1)
                            {
                                continue;
                            }
                            totalReciept += item.RecieptPrice ?? 0;
                            listRecieptData.Add(item);
                        }


                    }
                    this.totalPatientPrice = totalInvoice + totalReciept;
                }
                else
                {
                    this.totalPatientPrice = 0;
                }
                //spinRecieptAmount.Value = totalReciept;
                lblRecieptAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalReciept, ConfigApplications.NumberSeperator);
                //spinInvoiceAmount.Value = totalInvoice;
                lblInvoiceAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalInvoice, ConfigApplications.NumberSeperator);
                FillDataToTienHoaDon();
                CalcuCanThu(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<VHisSereServADO> GetListCheck()
        {
            List<VHisSereServADO> result = new List<VHisSereServADO>();
            try
            {
                foreach (TreeListNode node in treeListSereServ.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<VHisSereServADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<VHisSereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((VHisSereServADO)treeListSereServ.GetDataRecordByNode(node));
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

        private void treeListSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    VHisSereServADO data = e.Row as VHisSereServADO;
                    if (data == null) return;
                    if (data != null)
                    {
                        if (!e.Node.HasChildren)
                        {
                            if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                               
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "RecieptPriceDisplay")
                            {
                                e.Value = ConvertNumberToString(data.RecieptPrice ?? 0);
                            }
                            else if (e.Column.FieldName == "InvoicePriceDisplay")
                            {
                                e.Value = ConvertNumberToString(data.InvoicePrice ?? 0);
                            }
                        }
                        else
                        {
                            if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_HEIN_PRICE_DISPLAY");
                                e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (e.Column.FieldName == "RecieptPriceDisplay")
                            {
                                this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "RecieptPriceDisplay");
                                e.Value = ConvertNumberToString(data.RecieptPrice ?? 0);
                            }
                            else if (e.Column.FieldName == "InvoicePriceDisplay")
                            {
                                this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "InvoicePriceDisplay");
                                e.Value = ConvertNumberToString(data.InvoicePrice ?? 0);
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

        private void GetTotalPriceOfChildChoice(VHisSereServADO data, TreeListNodes childs, string fieldName)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {
                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (VHisSereServADO)item.TreeList.GetDataRecordByNode(item);
                        if (nodeData == null) continue;
                        if (!item.HasChildren && item.Checked)
                        {
                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "RecieptPriceDisplay")
                            {
                                totalChoicePrice += (nodeData.RecieptPrice ?? 0);
                            }
                            else if (fieldName == "RecieptPriceDisplay")
                            {
                                totalChoicePrice += (nodeData.RecieptPrice ?? 0);
                            }
                        }
                        else if (item.HasChildren)
                        {
                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "RecieptPriceDisplay")
                            {
                                totalChoicePrice += (nodeData.RecieptPrice ?? 0);
                            }
                            else if (fieldName == "InvoicePriceDisplay")
                            {
                                totalChoicePrice += (nodeData.InvoicePrice ?? 0);
                            }
                        }
                    }
                }
                if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                {
                    data.VIR_TOTAL_PRICE = totalChoicePrice;
                }
                else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                {
                    data.VIR_TOTAL_HEIN_PRICE = totalChoicePrice;
                }
                else if (fieldName == "RecieptPriceDisplay")
                {
                    data.RecieptPrice = totalChoicePrice;
                }
                else if (fieldName == "InvoicePriceDisplay")
                {
                    data.InvoicePrice = totalChoicePrice;
                }
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
