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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RepayService.ADO;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
//using HFS.APP.Model;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.RepayService.RepayService
{
    internal class RepayServiceProcess
    {
        //xemlai...
        internal static void UpdateDataFormTransactionDepositToDTO(HisTransactionRepaySDO transactionData, MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1 treatment, frmRepayService control)
        {
            try
            {
                if (transactionData == null)
                {
                    transactionData = new HisTransactionRepaySDO();
                    transactionData.Transaction = new HIS_TRANSACTION();
                }

                transactionData.SereServDepositIds = new List<long>();
                if (control.moduleData != null)
                {
                    transactionData.RequestRoomId = control.moduleData.RoomId;
                }

                transactionData.Transaction.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU;
                if (control.cboAccountBook.EditValue != null)
                {
                    var accountBook = control.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(control.cboAccountBook.EditValue.ToString()));
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        transactionData.Transaction.NUM_ORDER = (long)(control.spinTongTuDen.Value);
                    }
                }
                transactionData.Transaction.AMOUNT = Convert.ToDecimal(control.spinAmount.Tag);
                if (control.cboAccountBook.EditValue != null)
                {
                    transactionData.Transaction.ACCOUNT_BOOK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((control.cboAccountBook.EditValue ?? "").ToString());
                }
                if (control.cboRepayReason.EditValue != null)
                {
                    transactionData.Transaction.REPAY_REASON_ID = Inventec.Common.TypeConvert.Parse.ToInt64((control.cboRepayReason.EditValue ?? "").ToString());
                }
                if (control.cboPayForm.EditValue != null)
                {
                    transactionData.Transaction.PAY_FORM_ID = (Inventec.Common.TypeConvert.Parse.ToInt64((control.cboPayForm.EditValue ?? "").ToString()));
                }
                if (treatment != null)
                {
                    transactionData.Transaction.TREATMENT_ID = treatment.ID;
                }

                if (transactionData.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && control.spinTransferAmount.EditValue != null)
                {
                    transactionData.Transaction.TRANSFER_AMOUNT = control.spinTransferAmount.Value;
                }
                else if (transactionData.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && control.spinTransferAmount.EditValue != null)
                {
                    transactionData.Transaction.SWIPE_AMOUNT = control.spinTransferAmount.Value;
                }

                transactionData.Transaction.CASHIER_ROOM_ID = control.cashierRoomId;
                transactionData.Transaction.DESCRIPTION = control.txtDescription.Text;
                if (control.dtTransactionTime.EditValue != null && control.dtTransactionTime.DateTime != DateTime.MinValue)
                    transactionData.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(control.dtTransactionTime.EditValue).ToString("yyyyMMddHHmm") + "00");
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

        private static void SetSereServToDataTransfer(TreeListNode nodes, HisTransactionRepaySDO transactionData)
        {
            try
            {
                foreach (TreeListNode node in nodes.Nodes)
                {
                    if (node.Level == 2)
                    {
                        var item = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT)node.Tag;
                        if (item != null && node.Checked)
                        {
                            transactionData.SereServDepositIds.Add(item.ID);
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
        internal static void FillDataToControl(MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1 treatment, MOS.EFMODEL.DataModels.V_HIS_TRANSACTION hisRepay, frmRepayService control)
        {
            try
            {
                if (hisRepay != null && treatment != null)
                {
                    control.cboAccountBook.EditValue = hisRepay.ACCOUNT_BOOK_ID;
                    control.spinAmount.Tag = hisRepay.AMOUNT;
                    control.spinAmount.Text = Inventec.Common.Number.Convert.NumberToString(hisRepay.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    var pf = control.ListPayForm.FirstOrDefault(o => o.ID == hisRepay.PAY_FORM_ID);
                    if (pf != null)
                    {
                        control.cboPayForm.EditValue = pf.ID;
                    }

                    if (hisRepay.CREATE_TIME != null)
                        control.dtCreateTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((hisRepay.CREATE_TIME ?? 0));
                    control.txtDescription.Text = hisRepay.DESCRIPTION;
                    control.txtTransactionCode.Text = hisRepay.TRANSACTION_CODE;
                }
                else
                {
                    //control.cboAccountBook.EditValue = null;
                    //control.txtAccountBookCode.Text = "";
                    control.spinAmount.Tag = 0;
                    control.spinAmount.Text = "0";
                    //var pf = control.ListPayForm.FirstOrDefault(o => o.PAY_FORM_CODE == control.HIS_PAY_FORM_CODE_DEFAULT);
                    //if (pf != null)
                    //{
                    //    control.cboPayForm.EditValue = pf.ID;
                    //    control.txtPayFormCode.Text = pf.PAY_FORM_CODE;
                    //}
                    control.dtCreateTime.EditValue = DateTime.Now;
                    control.txtDescription.Text = "";
                    control.txtTransactionCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void FillDataToSereServTree(MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1 hisTreatment, frmRepayService control)
        {
            try
            {
                if (hisTreatment != null)
                {
                    control.treeSereServ.ClearNodes();
                    TreeListNode parentForRootNodes = null;

                    var listDereDetail = control.ListSereServDeposit.Where(o => o.DEPOSIT_ID != 0).ToList();
                    if (listDereDetail != null && listDereDetail.Count > 0)
                    {
                        List<long> listRootPatientTypeId = control.ListSereServDeposit.Where(o => o.DEPOSIT_ID != 0).Select(o => o.TDL_PATIENT_TYPE_ID).Distinct().ToList();
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

        internal static void CreateChildNodeServiceTypeRepay(TreeListNode rootNode, long patientTypeId, frmRepayService control)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT> listChild = null;
                List<long> listServiceTypeId = null;

                listChild = control.ListSereServDeposit.FindAll(o => o.TDL_PATIENT_TYPE_ID == patientTypeId && o.DEPOSIT_ID != 0).ToList();
                listServiceTypeId = control.ListSereServDeposit.FindAll(o => o.TDL_PATIENT_TYPE_ID == patientTypeId && o.TDL_IS_EXPEND != 1 && o.DEPOSIT_ID != 0).Select(o => o.TDL_SERVICE_TYPE_ID).Distinct().ToList();

                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                     new object[] { serviceTypeObj.SERVICE_TYPE_NAME, null, null, null, null, null, null, null, null, null },
                     rootNode, null);
                            var listChildDeposit = control.ListSereServDeposit.FindAll(o => o.TDL_PATIENT_TYPE_ID == patientTypeId && o.TDL_SERVICE_TYPE_ID == serviceTypeId && o.DEPOSIT_ID != 0);
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

        internal static void CreateChildNodeServiceTypeUnDeposit(TreeListNode rootNode, long patientTypeId, frmRepayService control)
        {
            try
            {
                List<V_HIS_SERE_SERV_DEPOSIT> listChild = null;
                List<long> listServiceTypeId = null;

                listChild = control.ListSereServDeposit.FindAll(o => o.TDL_PATIENT_TYPE_ID == patientTypeId && o.TDL_IS_EXPEND != 1 && o.DEPOSIT_ID != null);
                listServiceTypeId = control.ListSereServDeposit.FindAll(o => o.TDL_PATIENT_TYPE_ID == patientTypeId && o.TDL_IS_EXPEND != 1 && o.DEPOSIT_ID != null).Select(o => o.TDL_SERVICE_TYPE_ID).Distinct().ToList();

                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                     new object[] { serviceTypeObj.CREATOR, null, null, null, null, null, null, null, null, null, null, null, null },//SERVICE_TYPE_NAME
                     rootNode, null);
                            var listChildUnDeposit = control.ListSereServDeposit.FindAll(o => o.TDL_PATIENT_TYPE_ID == patientTypeId && o.TDL_SERVICE_TYPE_ID == serviceTypeId && o.TDL_IS_EXPEND != 1 && o.DEPOSIT_ID != null);
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

        internal static void CreateChildNodeService(TreeListNode childNode, long patientTypeId, long serviceTypeId, frmRepayService control, List<V_HIS_SERE_SERV_DEPOSIT> listChild)
        {
            try
            {
                if (listChild != null && listChild.Count > 0)
                {
                    foreach (var item in listChild)
                    {
                        string expen = "";
                        if (item.TDL_IS_EXPEND == 1)
                        {
                            expen = "Hao phí";
                        }

                        Boolean IsNoExecute = false;
                        string departmentRoom = "";
                        if (control.ListSereServ != null && control.ListSereServ.Count > 0)
                        {
                            var HisSereServ1 = control.ListSereServ.FirstOrDefault(o => o.ID == item.SERE_SERV_ID);
                            if (HisSereServ1 != null)
                            {
                                departmentRoom = HisSereServ1.REQUEST_ROOM_NAME;
                                if (HisSereServ1 != null && HisSereServ1.IS_NO_EXECUTE == 1)
                                {
                                    IsNoExecute = true;
                                }
                                else
                                {
                                    IsNoExecute = false;
                                }
                            }

                        }

                        string IntructionTime = "";
                        if (item.INTRUCTION_TIME != null)
                        {
                            IntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)item.INTRUCTION_TIME);
                        }

                        TreeListNode childChildNode = control.treeSereServ.AppendNode(
                    new object[] { item.TDL_SERVICE_NAME, item.AMOUNT, item.TDL_VIR_TOTAL_PRICE, item.TDL_VIR_TOTAL_HEIN_PRICE, item.TDL_VIR_TOTAL_PATIENT_PRICE, IntructionTime, item.SERVICE_REQ_CODE, departmentRoom, item.TDL_SERVICE_CODE, expen, IsNoExecute },
                    childNode, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeServiceTypeExpend(TreeListNode rootNode, frmRepayService control)
        {
            try
            {
                var listChild = control.ListSereServDeposit.FindAll(o => o.TDL_IS_EXPEND == 1);
                var listServiceTypeId = control.ListSereServDeposit.FindAll(o => o.TDL_IS_EXPEND == 1).Select(o => o.TDL_SERVICE_TYPE_ID).Distinct().ToList();
                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                    new object[] { serviceTypeObj.SERVICE_TYPE_NAME, null, null, null, null, null, null, null, null, null },
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

        internal static void CreateChildNodeServiceExpend(TreeListNode childNode, long serviceTypeId, frmRepayService control)
        {
            try
            {
                var listChild = control.ListSereServDeposit.FindAll(o => o.TDL_SERVICE_TYPE_ID == serviceTypeId && o.TDL_IS_EXPEND == 1);
                if (listChild != null && listChild.Count > 0)
                {
                    string expen = "Hao phí";
                    foreach (var item in listChild)
                    {
                        Boolean IsNoExecute = false;
                        if (item.SERE_SERV_ID != null && control.ListSereServ != null && control.ListSereServ.Count > 0)
                        {
                            var HisSereServ1 = control.ListSereServ.FirstOrDefault(o => o.ID == item.SERE_SERV_ID);
                            if (HisSereServ1 != null && HisSereServ1.IS_NO_EXECUTE == 1)
                            {
                                IsNoExecute = true;
                            }
                            else
                            {
                                IsNoExecute = false;
                            }
                        }

                        string IntructionTime = "";
                        if (item.INTRUCTION_TIME != null)
                        {
                            IntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)item.INTRUCTION_TIME);
                        }

                        TreeListNode childChildNode = control.treeSereServ.AppendNode(
                    new object[] { item.TDL_SERVICE_NAME, item.AMOUNT, item.TDL_VIR_TOTAL_PRICE, item.TDL_VIR_TOTAL_HEIN_PRICE, item.TDL_VIR_TOTAL_PATIENT_PRICE, IntructionTime, item.SERVICE_REQ_CODE, item.TDL_SERVICE_CODE, expen, IsNoExecute },
                    childNode, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        internal static void LoadAccountBookCombo(string searchCode, bool isExpand, frmRepayService control)
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
                    var data = control.ListAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            control.cboAccountBook.EditValue = data[0].ID;
                        }
                        else if (data.Count > 1)
                        {
                            control.cboAccountBook.EditValue = null;
                            control.cboAccountBook.Focus();
                            control.cboAccountBook.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadPayFormCombo(string searchCode, bool isExpand, frmRepayService control)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    control.cboPayForm.EditValue = null;
                    control.cboPayForm.Focus();
                    control.cboPayForm.ShowPopup();
                }
                else
                {
                    var data = control.ListPayForm.Where(o => o.PAY_FORM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            control.cboPayForm.EditValue = data[0].ID;
                            control.dtTransactionTime.Focus();
                            control.dtTransactionTime.ShowPopup();
                        }
                        else if (data.Count > 1)
                        {
                            control.cboPayForm.EditValue = null;
                            control.cboPayForm.Focus();
                            control.cboPayForm.ShowPopup();
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
