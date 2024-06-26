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
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Repay.ADO;
using HIS.Desktop.Utilities.Extentions;
//using HFS.APP.Model;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Repay.Repay
{
    internal class RepayProcess
    {
        //xemlai...
        internal static void UpdateDataFormTransactionDepositToDTO(HisRepaySDO transactionData, MOS.EFMODEL.DataModels.V_HIS_TREATMENT treatment, frmRepay control)
        {
            try
            {
                if (transactionData == null)
                {
                    transactionData = new HisRepaySDO();
                    transactionData.Repay = new HIS_REPAY();
                    transactionData.Transaction = new HIS_TRANSACTION();
                }

                transactionData.DereDetailIds = new List<long>();

                transactionData.Transaction.AMOUNT = control.spinAmount.Value;
                if (control.cboAccountBook.EditValue != null)
                {
                    transactionData.Transaction.ACCOUNT_BOOK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((control.cboAccountBook.EditValue ?? "").ToString());
                }
                if (control.cboPayForm.EditValue != null)
                {
                    transactionData.Transaction.PAY_FORM_ID = (Inventec.Common.TypeConvert.Parse.ToInt64((control.cboPayForm.EditValue ?? "").ToString()));
                }
                if (treatment != null)
                {
                    transactionData.Transaction.TREATMENT_ID = treatment.ID;
                }
                transactionData.Repay.DESCRIPTION = control.txtDescription.Text;
                foreach (TreeListNode node in control.treeSereServ.Nodes)
                {
                    SetSereServToDataTransfer(node, transactionData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void SetSereServToDataTransfer(TreeListNode nodes, HisRepaySDO transactionData)
        {
            try
            {
                foreach (TreeListNode node in nodes.Nodes)
                {
                    if (node.Level == 2)
                    {
                        var item = (MOS.EFMODEL.DataModels.V_HIS_DERE_DETAIL)node.Tag;
                        if (item != null && node.Checked)
                        {
                            transactionData.DereDetailIds.Add(item.ID);
                        }
                    }
                    SetSereServToDataTransfer(node, transactionData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //xemlai...
        internal static void FillDataToControl(MOS.EFMODEL.DataModels.V_HIS_TREATMENT treatment, MOS.EFMODEL.DataModels.V_HIS_REPAY hisRepay, frmRepay control)
        {
            try
            {
                if (hisRepay != null && treatment != null)
                {
                    control.cboAccountBook.EditValue = hisRepay.ACCOUNT_BOOK_ID;
                    control.txtAccountBookCode.Text = hisRepay.ACCOUNT_BOOK_CODE;
                    control.spinNumberOrder.Value = hisRepay.NUM_ORDER;
                    control.spinAmount.Value = hisRepay.AMOUNT;
                    var pf = control.ListPayForm.FirstOrDefault(o => o.ID == hisRepay.PAY_FORM_ID);
                    if (pf != null)
                    {
                        control.cboPayForm.EditValue = pf.ID;
                        control.txtPayFormCode.Text = pf.PAY_FORM_CODE;
                    }
                    var dataAccountBook = control.ListAccountBookFormBill.FirstOrDefault(o => o.ID == hisRepay.ACCOUNT_BOOK_ID);
                    if (dataAccountBook != null)
                    {
                        //control.txtTotalFromNumberOder.Text = dataAccountBook.TOTAL + "/" + dataAccountBook.FROM_NUM_ORDER + "/" + dataAccountBook.CURRENT_NUM_ORDER;
                    }
                    control.txtCashier.Text = hisRepay.CASHIER_LOGINNAME;
                    if (hisRepay.CREATE_TIME != null)
                        control.dtCreateTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((hisRepay.CREATE_TIME ?? 0));
                    control.txtDescription.Text = hisRepay.DESCRIPTION;
                    control.txtTransactionCode.Text = hisRepay.TRANSACTION_CODE;
                }
                else
                {
                    control.cboAccountBook.EditValue = null;
                    control.txtAccountBookCode.Text = "";
                    control.spinNumberOrder.Value = 0;
                    control.spinAmount.Value = 0;
                    var pf = control.ListPayForm.FirstOrDefault(o => o.PAY_FORM_CODE == control.HIS_PAY_FORM_CODE_DEFAULT);
                    if (pf != null)
                    {
                        control.cboPayForm.EditValue = pf.ID;
                        control.txtPayFormCode.Text = pf.PAY_FORM_CODE;
                    }
                    control.dtCreateTime.EditValue = DateTime.Now;
                    control.txtDescription.Text = "";
                    control.txtTotalFromNumberOder.Text = "";
                    control.txtTransactionCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void EnableButton(int action, frmRepay control)
        {
            try
            {
                control.btnSave.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                control.ddbPrint.Enabled = (action == GlobalVariables.ActionView);
                control.btnAdd.Enabled = true;

                UpdateItemsReadOnly(control);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void UpdateItemsReadOnly(frmRepay control)
        {
            bool isEditing = !(control.ActionType == GlobalVariables.ActionView);
            if (!control.layoutControl1.IsInitialized) return;
            control.layoutControl1.BeginUpdate();
            try
            {
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in control.layoutControl1.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null)
                    {
                        DevExpress.XtraEditors.BaseEdit be = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        if (be != null)
                        {
                            //Fix cung theo form
                            if (be.Name == control.txtCashier.Name
                               || be.Name == control.dtCreateTime.Name
                                || be.Name == control.spinAmount.Name
                               || be.Name == control.txtTotalFromNumberOder.Name
                               || be.Name == control.txtTransactionCode.Name)
                            {
                                be.Properties.ReadOnly = true;
                            }
                            else
                            {
                                be.Properties.ReadOnly = !isEditing;
                            }
                        }
                    }
                }
                control.treeSereServ.Enabled = (control.ActionType != GlobalVariables.ActionView);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                control.layoutControl1.EndUpdate();
            }
        }

        internal static void FillDataToSereServTree(MOS.EFMODEL.DataModels.V_HIS_TREATMENT hisTreatment, frmRepay control)
        {
            try
            {
                if (hisTreatment != null)
                {
                    control.treeSereServ.ClearNodes();
                    TreeListNode parentForRootNodes = null;

                    var listDereDetail = control.ListDereDetail.Where(o => o.DEPOSIT_ID != 0 && o.REPAY_ID == null).ToList();
                    if (listDereDetail != null && listDereDetail.Count > 0)
                    {
                        List<long> listRootPatientTypeId = control.ListDereDetail.Where(o =>
                        o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE
                        && o.DEPOSIT_ID != 0 && o.REPAY_ID == null
                        ).Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();
                        foreach (var rootHeinServiceTypeId in listRootPatientTypeId)
                        {
                            var patientTypeObj = control.ListHisPatientType.FirstOrDefault(o => o.ID == rootHeinServiceTypeId);
                            if (patientTypeObj != null)
                            {
                                TreeListNode rootPatientType = control.treeSereServ.AppendNode(
                            new object[] { patientTypeObj.PATIENT_TYPE_NAME, null, null, null, null, null, null, null, null, null, null },
                            parentForRootNodes, null);
                                CreateChildNodeServiceTypeRepay(rootPatientType, rootHeinServiceTypeId, control);
                            }
                        }
                    }
                    control.treeSereServ.ExpandAll();
                    control.hideCheckBoxHelper = new HideCheckBoxHelper(control.treeSereServ);
                }
                else
                {
                    control.treeSereServ.ClearNodes();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeServiceTypeRepay(TreeListNode rootNode, long patientTypeId, frmRepay control)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_DERE_DETAIL> listChild = null;
                List<long> listServiceTypeId = null;
                listChild = control.ListDereDetail.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null && o.DEPOSIT_ID != 0).ToList();
                listServiceTypeId = control.ListDereDetail.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null && o.DEPOSIT_ID != 0).Select(o => o.SERVICE_TYPE_ID).Distinct().ToList();

                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                     new object[] { serviceTypeObj.SERVICE_TYPE_NAME, null, null, null, null, null, null, null, null, null, null },
                     rootNode, null);
                            var listChildDeposit = control.ListDereDetail.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.SERVICE_TYPE_ID == serviceTypeId && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null && o.DEPOSIT_ID != 0);
                            CreateChildNodeService(childNode, patientTypeId, serviceTypeId, control, listChildDeposit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeServiceTypeUnDeposit(TreeListNode rootNode, long patientTypeId, frmRepay control)
        {
            try
            {
                List<V_HIS_DERE_DETAIL> listChild = null;
                List<long> listServiceTypeId = null;
                listChild = control.ListDereDetail.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null && o.DEPOSIT_ID != null);
                listServiceTypeId = control.ListDereDetail.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null && o.DEPOSIT_ID != null).Select(o => o.SERVICE_TYPE_ID).Distinct().ToList();

                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                     new object[] { serviceTypeObj.SERVICE_TYPE_NAME, null, null, null, null, null, null, null, null, null, null },
                     rootNode, null);
                            var listChildUnDeposit = control.ListDereDetail.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.SERVICE_TYPE_ID == serviceTypeId && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null && o.DEPOSIT_ID != null);
                            CreateChildNodeService(childNode, patientTypeId, serviceTypeId, control, listChildUnDeposit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeService(TreeListNode childNode, long patientTypeId, long serviceTypeId, frmRepay control, List<V_HIS_DERE_DETAIL> listChild)
        {
            try
            {
                if (listChild != null && listChild.Count > 0)
                {
                    foreach (var item in listChild)
                    {
                        string expen = "";
                        if (item.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE)
                        {
                            expen = "Hao phí";
                        }
                        TreeListNode childChildNode = control.treeSereServ.AppendNode(
                    new object[] { item.SERVICE_NAME, item.AMOUNT, null, item.VIR_TOTAL_PRICE, item.VIR_TOTAL_HEIN_PRICE, item.VIR_TOTAL_PATIENT_PRICE, null, item.SERVICE_CODE, null, null, expen, null },
                    childNode, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeServiceTypeExpend(TreeListNode rootNode, frmRepay control)
        {
            try
            {
                var listChild = control.ListDereDetail.FindAll(o => o.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null);
                var listServiceTypeId = control.ListDereDetail.FindAll(o => o.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null).Select(o => o.SERVICE_TYPE_ID).Distinct().ToList();
                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                    new object[] { serviceTypeObj.SERVICE_TYPE_NAME, null, null, null, null, null, null, null, null, null, null },
                    rootNode, null);
                            CreateChildNodeServiceExpend(childNode, serviceTypeId, control);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeServiceExpend(TreeListNode childNode, long serviceTypeId, frmRepay control)
        {
            try
            {
                var listChild = control.ListDereDetail.FindAll(o => o.SERVICE_TYPE_ID == serviceTypeId && o.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && o.REPAY_ID == null);
                if (listChild != null && listChild.Count > 0)
                {
                    string expen = "Hao phí";
                    foreach (var item in listChild)
                    {
                        TreeListNode childChildNode = control.treeSereServ.AppendNode(
                  new object[] { item.SERVICE_NAME, item.AMOUNT, item.SERVICE_UNIT_NAME, item.VIR_TOTAL_PRICE, item.VIR_TOTAL_HEIN_PRICE, item.VIR_TOTAL_PATIENT_PRICE, item.HEIN_SERVICE_BHYT_NAME, item.SERVICE_CODE, item.SERVICE_TYPE_NAME, item.HEIN_SERVICE_TYPE_NAME, expen, null },
                  childNode, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        internal static void LoadAccountBookCombo(string searchCode, bool isExpand, frmRepay control)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    control.cboAccountBook.EditValue = null;
                    control.cboAccountBook.Focus();
                    control.cboAccountBook.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboAccountBook);
                }
                else
                {
                    var data = control.ListAccountBookFormBill.Where(o => o.ACCOUNT_BOOK_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            control.cboAccountBook.EditValue = data[0].ID;
                            control.txtAccountBookCode.Text = data[0].ACCOUNT_BOOK_CODE;
                            control.txtTotalFromNumberOder.Text = data[0].TOTAL + "/" + data[0].FROM_NUM_ORDER + "/" + (int)(data[0].CURRENT_NUM_ORDER ?? 0);
                            control.txtPayFormCode.Focus();
                            control.txtPayFormCode.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            control.cboAccountBook.EditValue = null;
                            control.cboAccountBook.Focus();
                            control.cboAccountBook.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboAccountBook);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadPayFormCombo(string searchCode, bool isExpand, frmRepay control)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    control.cboPayForm.EditValue = null;
                    control.cboPayForm.Focus();
                    control.cboPayForm.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboPayForm);
                }
                else
                {
                    var data = control.ListPayForm.Where(o => o.PAY_FORM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            control.cboPayForm.EditValue = data[0].ID;
                            control.txtPayFormCode.Text = data[0].PAY_FORM_CODE;
                            control.spinAmount.SelectAll();
                            control.spinAmount.Focus();
                        }
                        else if (data.Count > 1)
                        {
                            control.cboPayForm.EditValue = null;
                            control.cboPayForm.Focus();
                            control.cboPayForm.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboPayForm);
                        }
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
