using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.AdjustmentTransaction.config;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AdjustmentTransaction.AdjustmentTransaction
{
    public partial class frmAdjustmentTransaction : FormBase
    {
        private void treeSereServ_BeforeCheckNode(TreeListNode node, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {

                if (node != null)
                {
                    var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null && config.HisConfigCFG.MustFinishTreatmentForBill == "1" && this.treatmentFee.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
                    if (nodeData != null && config.HisConfigCFG.MustFinishTreatmentForBill == "2" && this.treatmentFee.IS_PAUSE != 1)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
                    if (nodeData != null && nodeData.IS_NO_EXECUTE.HasValue && nodeData.IS_NO_EXECUTE.Value == 1)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
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

        //private void treeSereServ_AfterCheckNode(TreeListNode node, SereServADO data)
        //{
        //    try
        //    {

        //        CalcuTotalPrice();
        //        this.ProcessFundForHCM();
        //        CalcuCanThu();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void treeSereServ_CustomDrawNodeCheckBox(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (data != null && config.HisConfigCFG.MustFinishTreatmentForBill == "1" && this.treatmentFee.IS_PAUSE != 1 && data.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    e.Handled = true;
                }
                else if (data != null && config.HisConfigCFG.MustFinishTreatmentForBill == "2" && this.treatmentFee.IS_PAUSE != 1)
                {
                    e.Handled = true;
                }
                else if (data != null && data.IS_NO_EXECUTE.HasValue && data.IS_NO_EXECUTE.Value == 1)
                {
                    e.Handled = true;
                }
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
                if (data != null)
                {

                    if (!e.Node.HasChildren)
                    {

                        if (e.Column.FieldName == "AMOUNT_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.AMOUNT);
                        }
                        else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.DISCOUNT ?? 0);
                        }
                        else if (e.Column.FieldName == "VAT_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VAT);
                        }
                        else if (e.Column.FieldName == "TOTAL_BILL_AMOUNT")
                        {
                            e.Value = ConvertNumberToString(data.TOTAL_BILL_AMOUNT ?? 0);
                        }
                        else if (e.Column.FieldName == "EDIT_AMOUNT")
                        {
                            TreeList tree = e.Column.TreeList;

                            if (e.Node.HasChildren)
                            {
                                // 🔹 Tính tổng đệ quy cho tất cả node con (mọi cấp)
                                decimal totalChildAdjustment = CalculateTotalAdjustmentRecursive(tree, e.Node);
                                e.Value = totalChildAdjustment;
                            }
                            else
                            {
                                // 🔹 Node con bình thường
                                if (data.ID != null && adjustmentValues.ContainsKey(data.ID))
                                {
                                    e.Value = adjustmentValues[data.ID];
                                }
                                else
                                {
                                    e.Value = 0m;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "INCREASE_BTN")
                        {
                            e.Value = "↑";
                        }
                        else if (e.Column.FieldName == "DECREASE_BTN")
                        {
                            e.Value = "↓";
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
                        else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                        {
                            this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_PATIENT_PRICE_DISPLAY");
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "EDIT_AMOUNT")
                        {
                            // Tính tổng điều chỉnh của các node con
                            decimal totalAdjustment = 0;
                            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode childNode in e.Node.Nodes)
                            {
                                var childData = childNode.TreeList.GetDataRecordByNode(childNode) as SereServADO;
                                if (childData != null)
                                {
                                    object childKey = childData.ID;
                                    if (adjustmentValues.ContainsKey(childKey))
                                    {
                                        totalAdjustment += adjustmentValues[childKey];
                                    }
                                }
                            }
                            e.Value = ConvertNumberToString(totalAdjustment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private decimal CalculateTotalAdjustmentRecursive(TreeList tree, TreeListNode node)
        {
            decimal total = 0;

            foreach (TreeListNode child in node.Nodes)
            {
                var childData = tree.GetDataRecordByNode(child) as SereServADO;
                if (childData == null) continue;

                object childKey = childData.ID;

                // Nếu node con có con nữa → tính đệ quy tiếp
                if (child.HasChildren)
                {
                    total += CalculateTotalAdjustmentRecursive(tree, child);
                }
                else if (adjustmentValues.ContainsKey(childKey))
                {
                    total += adjustmentValues[childKey];
                }
            }

            return total;
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

        private void GetTotalPriceOfChildChoice(SereServADO data, TreeListNodes childs, string fieldName)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {

                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (SereServADO)item.TreeList.GetDataRecordByNode(item);
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
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
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
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
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
                else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                {
                    data.VIR_TOTAL_PATIENT_PRICE = totalChoicePrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CheckAllNode(TreeListNodes nodes)
        {
            try
            {
                if (nodes == null)
                    return;

                foreach (TreeListNode node in nodes)
                {
                    var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null)
                    {
                        // ✅ Check node hiện tại
                        node.CheckAll();

                        // ✅ Nếu có node con thì check luôn tất cả con
                        if (node.HasChildren)
                        {
                            treeSereServ_CheckAllNode(node.Nodes);
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
                        var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(childNode);
                        if (nodeData != null)
                        {
                            if (currentTransaction != null && currentTransaction.IS_CANCEL == 1 && !lstSereServId.Exists(o => o == nodeData.ID))
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else if (config.HisConfigCFG.MustFinishTreatmentForBill == "1" && this.treatmentFee.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else if (config.HisConfigCFG.MustFinishTreatmentForBill == "2" && this.treatmentFee.IS_PAUSE != 1)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else if (nodeData.IS_NO_EXECUTE.HasValue && nodeData.IS_NO_EXECUTE.Value == 1)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);

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

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.VIR_TOTAL_PATIENT_PRICE.HasValue && data.VIR_TOTAL_PATIENT_PRICE.Value > 0 && (!data.IS_NO_EXECUTE.HasValue || data.IS_NO_EXECUTE.Value != 1))
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

                if (!e.Node.HasChildren && (e.Column.FieldName == "INCREASE_BTN" || e.Column.FieldName == "DECREASE_BTN"))
                {
                    e.Handled = true;

                    Image icon = null;
                    if (e.Column.FieldName == "INCREASE_BTN")
                        icon = ByteArrayToImage(Properties.Resources.up_arrow);
                    else if (e.Column.FieldName == "DECREASE_BTN")
                        icon = ByteArrayToImage(Properties.Resources.down);

                    if (icon != null)
                    {
                        int iconSize = Math.Min(e.Bounds.Height - 4, 16);
                        int x = e.Bounds.X + (e.Bounds.Width - iconSize) / 2;
                        int y = e.Bounds.Y + (e.Bounds.Height - iconSize) / 2;
                        e.Graphics.DrawImage(icon, new Rectangle(x, y, iconSize, iconSize));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Image ByteArrayToImage(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
