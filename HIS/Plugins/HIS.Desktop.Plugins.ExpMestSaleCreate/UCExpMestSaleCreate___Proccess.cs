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
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validtion;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Config;
using HIS.UC.Icd.ADO;
using WCF;
using Newtonsoft.Json;
using WCF.Client;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ADO;
using IcdInputADO = HIS.UC.Icd.ADO.IcdInputADO;
using MediMateTypeADO = HIS.Desktop.Plugins.ExpMestSaleCreate.ADO.MediMateTypeADO;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        private void ReleaseAll()
        {
            try
            {
                if (dicMediMateAdo != null && dicMediMateAdo.Count > 0)
                {
                    var dt = ((BindingList<MediMateTypeADO>)treeListMediMate.DataSource).ToList();
                    if (dt != null && dt.Count > 0)
                    {
                        var groupByClientSessinKey = dt.GroupBy(o => o.ClientSessionKey).ToList();
                        foreach (var clientSessionKey in groupByClientSessinKey)
                        {
                            CommonParam param = new CommonParam();
                            bool releaseMedicineAll = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_MEDICINE_BEAN__RELEASEBEANALL, ApiConsumers.MosConsumer, clientSessionKey.Key, param);
                            if (!releaseMedicineAll)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Release Medicine All False ____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientSessionKey), clientSessionKey));
                            }

                            bool releaseMaterialAll = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_MATERIAL_BEAN__RELEASEBEANALL, ApiConsumers.MosConsumer, clientSessionKey.Key, param);
                            if (!releaseMaterialAll)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Release Material All False ____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientSessionKey), clientSessionKey));
                            }
                        }
                    }
                }
                this.clientSessionKey = Guid.NewGuid().ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool sucPOS = true;
        void ProcessSave(ref bool success, ref CommonParam param)
        {
            try
            {
                bool sucPOS = true;
                success = false;
                if (dicMediMateAdo != null)
                {
                    if (ExistServiceNotInStock() || ExistServiceExceedsAvailable() || !CheckValiDiscount() || !CheckPatientDob())
                        return;
                }

                ddBtnPrint.Focus();
                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));

                if (payForm == null)
                    return;
                if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                {
                    if (spinTransferAmount.Value > this.totalPayPrice)
                    {
                        XtraMessageBox.Show(String.Format("Số tiền chuyển khoản [{0}] lớn hơn số tiền cần thanh toán của phiếu xuất [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, 2), Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.totalPayPrice ?? 0, 2)));
                        spinTransferAmount.Focus();
                        spinTransferAmount.SelectAll();
                        return;
                    }
                }
                isTwoPatient = false;
                string uriRequest = null;
                var dt = ((BindingList<MediMateTypeADO>)treeListMediMate.DataSource).ToList();
                if (dt.Where(o => o.KEY_PRICE_PARENT).ToList().Count > 1)
                {
                    isTwoPatient = true;
                }
                disCountRatio = spinDiscountRatio.Value != null ? spinDiscountRatio.Value : 0;
                HisExpMestSaleListSDO saleSDO = new HisExpMestSaleListSDO();
                List<HisExpMestSaleListSDO> listSaleSDO = new List<HisExpMestSaleListSDO>();

                if (moduleAction != GlobalDataStore.ModuleAction.EDIT)
                {
                    if (!isTwoPatient)
                    {
                        uriRequest = "api/HisExpMest/SaleCreateListSdo";
                        InitDataToSaleCreate(ref saleSDO);
                    }
                    else
                    {
                        uriRequest = "api/HisExpMest/SaleCreateBillList";
                        InitListDataToSaleCreate(ref listSaleSDO);
                    }
                }
                else if (moduleAction == GlobalDataStore.ModuleAction.EDIT)
                {
                    isTwoPatient = false;
                    uriRequest = "api/HisExpMest/SaleUpdateListSdo";
                    InitDataToSaleCreate(ref saleSDO);
                }

                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug("bat dau goi api api/HisExpMest/SaleCreateListSdo hoac api/HisExpMest/SaleUpdateListSdo " + disCountRatio);

                #region Call POS
                if ((payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE
                      || payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                      && this.totalPayPrice > 0)
                {
                    if (spinTransferAmount.Value > this.totalPayPrice)
                    {
                        XtraMessageBox.Show(String.Format("Số tiền quẹt thẻ [{0}] lớn hơn số tiền cần thanh toán của phiếu xuất [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, 2), Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.totalPayPrice ?? 0, 2)));
                        spinTransferAmount.Focus();
                        spinTransferAmount.SelectAll();
                        return;
                    }
                    if (ChkKetNoiPOS.Checked == true && chkCreateBill.Checked == true)
                    {
                        success = true;
                        OpenAppPOS();

                        WcfRequest wc = new WcfRequest(); // Khởi tạo data
                        if ((payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT || payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE) && spinTransferAmount.Value > 0)
                        {
                            wc.AMOUNT = (long)spinTransferAmount.Value; // Số tiền

                        }
                        wc.billId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                        wc.creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        var json = Inventec.WCF.JsonConvert.JsonConvert.Serialize<WcfRequest>(wc);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => json), json));
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                        try
                        {
                            if (cll == null)
                            {
                                cll = new WcfClient();
                            }
                        }
                        catch (Exception ex)
                        {
                            sucPOS = false;
                            success = false;
                            ChkKetNoiPOS.Checked = false;
                            XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                        }
                        var result = cll.Sale(System.Convert.ToBase64String(plainTextBytes));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                        if (result != null && result.RESPONSE_CODE == "00")
                        {
                            sucPOS = true;
                            success = true;
                            HisTransactionBillSDO data = new HisTransactionBillSDO();
                            if (!isTwoPatient)
                            {
                                
                                saleSDO.PosPan = result.PAN;
                                saleSDO.PosCardHoder = result.NAME;
                                saleSDO.PosInvoice = result.INVOICE.ToString();
                                saleSDO.PosResultJson = Inventec.WCF.JsonConvert.JsonConvert.Serialize<WcfRequest>(result);
                            }
                            else
                            {
                                foreach (var item in listSaleSDO)
                                {
                                    //item.BuyerEmail = txtEmail.Text;
                                    item.PosPan = result.PAN;
                                    item.PosCardHoder = result.NAME;
                                    item.PosInvoice = result.INVOICE.ToString();
                                    item.PosResultJson = Inventec.WCF.JsonConvert.JsonConvert.Serialize<WcfRequest>(result);
                                }
                            }
                        }
                        else if (result != null)
                        {
                            sucPOS = false;
                            success = false;
                            WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.
                           Show(" Giao dịch không thành công (Mã lỗi: " + result.ERROR + ")", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK) ;

                        }
                    }
                }
                #endregion

                if (sucPOS)
                {

                    if (!isTwoPatient)
                    {
                        #region 1 Bệnh nhân
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saleSDO), saleSDO));
                        HisExpMestSaleListResultSDO rs = new BackendAdapter(param)
                      .Post<HisExpMestSaleListResultSDO>(uriRequest, ApiConsumers.MosConsumer, saleSDO, param);
                        Inventec.Common.Logging.LogSystem.Debug("Ket thuc goi api api/HisExpMest/SaleCreateListSdo hoac api/HisExpMest/SaleUpdateListSdo");
                        if (rs != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                            this.resultSDO = rs;

                            if (btnQr.Visible && this.resultSDO.Transaction != null && this.resultSDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                                btnQr.Enabled = true;
                            if (resultSDO.ExpMestSdos != null && resultSDO.ExpMestSdos.Count > 0)
                            {
                                InitMenuPrint(resultSDO.ExpMestSdos.FirstOrDefault().ExpMest);
                            }
                            success = true;
                            moduleAction = GlobalDataStore.ModuleAction.EDIT;
                            this.SetLabelSave(GlobalDataStore.ModuleAction.EDIT);
                            ReloadDataDicBeforSave();
                            FillDataToGridExpMest();
                            if (chkCreateBill.Checked && this.resultSDO.Transaction != null)
                            {
                                UpdateDictionaryNumOrderAccountBook(this.resultSDO.Transaction.ACCOUNT_BOOK_ID, this.resultSDO.Transaction.NUM_ORDER);
                                lblTransactionCode.Text = this.resultSDO.Transaction.TRANSACTION_CODE;
                                if (this.resultSDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                                {
                                    List<object> listArgs = new List<object>();
                                    TransReqQRADO adoqr = new TransReqQRADO();
                                    adoqr.TransReqId = CreateReqType.Transaction;    
                                    if (mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON))
                                    {
                                        try
                                        {
                                            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(mediStock.QR_CONFIG_JSON);
                                            if (json != null)
                                                adoqr.ConfigValue = new HIS_CONFIG() { VALUE = json.VALUE, KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", json.BANK) };

                                            adoqr.TreatmentId = 0;
                                            adoqr.Transaction = resultSDO.Transaction;
                                            listArgs.Add(adoqr);
                                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                                        }
                                        catch (Exception ex)
                                        {
                                            Inventec.Common.Logging.LogSystem.Error(ex);
                                            XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ");
                                        }
                                    }
                                    else if (listConfig != null && listConfig.Count > 0)
                                    {
                                        if (listConfig.Count > 1)
                                        {
                                            XtraMessageBox.Show("Vui lòng sử dụng nút tạo QR để thực hiện thanh toán");
                                        }
                                        else
                                        {
                                            adoqr.ConfigValue = listConfig[0];

                                            adoqr.TreatmentId = 0;
                                            adoqr.Transaction = resultSDO.Transaction;
                                            listArgs.Add(adoqr);
                                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                                        }
                                    }

                                }
                            }
                            if (resultSDO.ExpMestSdos != null && resultSDO.ExpMestSdos.Count > 0)
                            {
                                SetControlByExpMest(resultSDO.ExpMestSdos.FirstOrDefault().ExpMest.EXP_MEST_CODE);

                                if (String.IsNullOrWhiteSpace(this.txtPatientCode.Text))
                                {
                                    this.txtPatientCode.Text = resultSDO.ExpMestSdos.FirstOrDefault().ExpMest.TDL_PATIENT_CODE;
                                }

                                btnCancelExport.Enabled = (resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE));
                                btnSave.Enabled = !(resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                                    || resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE));
                                btnSavePrint.Enabled = !(resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                                    || resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE));
                            }
                            this.SetEnableButtonDebt(moduleAction == GlobalDataStore.ModuleAction.EDIT);


                        }
                        else
                        {
                            success = false;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Nhiều bệnh nhân
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSaleSDO), listSaleSDO));
                        List<HisExpMestSaleListResultSDO> ListRs = new BackendAdapter(param)
                            .Post<List<HisExpMestSaleListResultSDO>>(uriRequest, ApiConsumers.MosConsumer, listSaleSDO, param);
                        Inventec.Common.Logging.LogSystem.Debug("Ket thuc goi api api/HisExpMest/SaleCreateBillList");
                        if (ListRs != null && ListRs.Count > 0)
                        {
                            bool isSuccess = false;
                            foreach (var item in ListRs)
                            {
                                if (item != null)
                                {
                                    isSuccess = true;
                                }
                            }
                            if (isSuccess)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListRs), ListRs));
                                this.ListResultSDO = ListRs;
                               
                                foreach (var item in ListResultSDO)
                                {
                                    if (item != null && item.ExpMestSdos != null && item.ExpMestSdos.Count > 0)
                                    {
                                        InitMenuPrint(item.ExpMestSdos.FirstOrDefault().ExpMest);
                                    }
                                }
                                success = true;
                                moduleAction = GlobalDataStore.ModuleAction.EDIT;
                                this.SetLabelSave(GlobalDataStore.ModuleAction.EDIT);
                                ReloadListDataDicBeforSave();
                                FillDataToGridExpMest();
                                bool IsShowMessQr = false;
                                foreach (var item in ListResultSDO)
                                {
                                    this.resultSDO = item; 
                                    if (btnQr.Visible && this.resultSDO.Transaction != null && this.resultSDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                                        btnQr.Enabled = true;
                                    if (this.resultSDO != null)
                                    {
                                        if (chkCreateBill.Checked && this.resultSDO.Transaction != null)
                                        {
                                            UpdateDictionaryNumOrderAccountBook(this.resultSDO.Transaction.ACCOUNT_BOOK_ID, this.resultSDO.Transaction.NUM_ORDER);
                                            if (this.resultSDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                                            {
                                                List<object> listArgs = new List<object>();
                                                TransReqQRADO adoqr = new TransReqQRADO();
                                                adoqr.TransReqId = CreateReqType.Transaction;
                                                if (mediStock != null && !string.IsNullOrEmpty(mediStock.QR_CONFIG_JSON))
                                                {
                                                    try
                                                    {
                                                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(mediStock.QR_CONFIG_JSON);
                                                        if (json != null)
                                                            adoqr.ConfigValue = new HIS_CONFIG() { VALUE = json.VALUE, KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", json.BANK) };

                                                        adoqr.TreatmentId = 0;
                                                        adoqr.Transaction = resultSDO.Transaction;
                                                        listArgs.Add(adoqr);
                                                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Inventec.Common.Logging.LogSystem.Error(ex);
                                                        XtraMessageBox.Show("Định dạng Qr thiết lập trong kho phòng không hợp lệ");
                                                    }
                                                }
                                                else if (listConfig != null && listConfig.Count > 0 && !IsShowMessQr)
                                                {
                                                    if (listConfig.Count > 1)
                                                    {
                                                        XtraMessageBox.Show("Vui lòng sử dụng nút tạo QR để thực hiện thanh toán");
                                                        IsShowMessQr = true;
                                                    }
                                                    else
                                                    {
                                                        adoqr.ConfigValue = listConfig[0];

                                                        adoqr.TreatmentId = 0;
                                                        adoqr.Transaction = resultSDO.Transaction;
                                                        listArgs.Add(adoqr);
                                                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                                                    }
                                                }

                                            }
                                        }

                                        if (resultSDO.ExpMestSdos != null && resultSDO.ExpMestSdos.Count > 0)
                                        {
                                            SetControlByExpMest(resultSDO.ExpMestSdos.FirstOrDefault().ExpMest.EXP_MEST_CODE);

                                            if (String.IsNullOrWhiteSpace(this.txtPatientCode.Text))
                                            {
                                                this.txtPatientCode.Text = resultSDO.ExpMestSdos.FirstOrDefault().ExpMest.TDL_PATIENT_CODE;
                                            }

                                            btnCancelExport.Enabled = (resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE));
                                            btnSave.Enabled = !(resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                                                || resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE));
                                            btnSavePrint.Enabled = !(resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                                                || resultSDO.ExpMestSdos.Any(a => a.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE));
                                        }
                                    }
                                }


                                //if (chkCreateBill.Checked && this.resultSDO.Transaction != null)
                                //{
                                //    lblTransactionCode.Text = this.ListResultSDO.Transaction.TRANSACTION_CODE;
                                //}


                                this.SetEnableButtonDebt(moduleAction == GlobalDataStore.ModuleAction.EDIT);
                            }

                        }
                        else
                        {
                            success = false;
                        }
                        #endregion
                    }
                    WaitingManager.Hide();

                }

                if (this.savePrint)
                {
                    if (!chkCreateBill.Checked)
                        onClickInPhieuXuatBan(null, null);
                    else
                        onClickInHoaDonBienLaiXuatBan(null, null);
                }
                else
                {
                    btnCancelExport.Enabled = false;
                }
                this.savePrint = false;

                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);

                string keyy = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ExpMestSaleCreate__Show_MedicineSaleBill");
                if (keyy == "1")
                {
                    btnSaleBill_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                this.savePrint = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void ReloadListDataDicBeforSave()
        {
            try
            {
                var dicMediMateAdoTmp = dicMediMateAdo;
                dicMediMateAdo = new Dictionary<long, List<MediMateTypeADO>>();
                foreach (var sdo in ListResultSDO)
                {
                    this.resultSDO = sdo;
                    if (this.resultSDO != null && this.resultSDO.ExpMestSdos != null && this.resultSDO.ExpMestSdos.Count > 0)
                    {
                        List<MediMateTypeADO> mediMateTypeADOs = new List<MediMateTypeADO>();
                        foreach (var item in this.resultSDO.ExpMestSdos)
                        {
                            if (item.ExpMedicines != null && item.ExpMedicines.Count > 0)
                            {
                                CommonParam param = new CommonParam();
                                HisMedicineBeanFilter medicineBeanFilter = new HisMedicineBeanFilter();
                                medicineBeanFilter.EXP_MEST_MEDICINE_IDs = item.ExpMedicines.Select(o => o.ID).ToList();
                                var medicineBeans = new BackendAdapter(param)
                              .Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_BEAN>>(RequestUriStore.HIS_MEDICINE_BEAN__GET, ApiConsumers.MosConsumer, medicineBeanFilter, param);
                                mediMateTypeADOs.AddRange(from r in item.ExpMedicines select new MediMateTypeADO(r, item.ExpMest, medicineBeans, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>()));
                            }
                            if (item.ExpMaterials != null && item.ExpMaterials.Count > 0)
                            {
                                CommonParam param = new CommonParam();
                                HisMaterialBeanFilter materialBeanFilter = new HisMaterialBeanFilter();
                                materialBeanFilter.EXP_MEST_MATERIAL_IDs = item.ExpMaterials.Select(o => o.ID).ToList();
                                var materialBeans = new BackendAdapter(param)
                              .Get<List<MOS.EFMODEL.DataModels.HIS_MATERIAL_BEAN>>(RequestUriStore.HIS_MATERIAL_BEAN__GET, ApiConsumers.MosConsumer, materialBeanFilter, param);
                                mediMateTypeADOs.AddRange(from r in item.ExpMaterials select new MediMateTypeADO(r, item.ExpMest, materialBeans, BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>()));
                            }
                        }

                        if (mediMateTypeADOs != null && mediMateTypeADOs.Count > 0)
                        {
                            foreach (var mediMateTypeADO in mediMateTypeADOs)
                            {
                                mediMateTypeADO.AVAILABLE_AMOUNT = (mediMateTypeADO.OLD_AMOUNT ?? 0);
                                if (mediMateTypeADO.IsMedicine)
                                {
                                    HisMedicineTypeInStockSDO mediInStockSDO = this.mediInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID);
                                    if (mediInStockSDO != null)
                                    {
                                        mediMateTypeADO.AVAILABLE_AMOUNT += (mediInStockSDO.AvailableAmount ?? 0);
                                        if (mediMateTypeADO.EXP_AMOUNT > mediMateTypeADO.AVAILABLE_AMOUNT)// if (spinAmount.Value > mediInStockSDO.AvailableAmount)
                                        {
                                            mediMateTypeADO.IsExceedsAvailable = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                            mediMateTypeADO.IsNotInStock = true;
                                    }
                                }
                                if (mediMateTypeADO.IsMaterial)
                                {
                                    HisMaterialTypeInStockSDO mateInStockSDO = this.mateInStocks.FirstOrDefault(o => o.Id == mediMateTypeADO.MEDI_MATE_TYPE_ID && !mediMateTypeADO.OLD_AMOUNT.HasValue);
                                    if (mateInStockSDO != null)
                                    {
                                        mediMateTypeADO.AVAILABLE_AMOUNT += (mateInStockSDO.AvailableAmount ?? 0);
                                        if (mediMateTypeADO.EXP_AMOUNT > mediMateTypeADO.AVAILABLE_AMOUNT)
                                        {
                                            mediMateTypeADO.IsExceedsAvailable = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!mediMateTypeADO.OLD_AMOUNT.HasValue)
                                            mediMateTypeADO.IsNotInStock = true;
                                    }
                                }
                                if (dicMediMateAdo.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                                    dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                                else
                                {
                                    dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID] = new List<MediMateTypeADO>();
                                    if (dicMediMateAdoTmp != null && dicMediMateAdoTmp.Count > 0 && dicMediMateAdoTmp.ContainsKey(mediMateTypeADO.MEDI_MATE_TYPE_ID))
                                        mediMateTypeADO.ClientSessionKey = dicMediMateAdoTmp[mediMateTypeADO.MEDI_MATE_TYPE_ID][0].ClientSessionKey;
                                    else
                                        mediMateTypeADO.ClientSessionKey = clientSessionKey;
                                    dicMediMateAdo[mediMateTypeADO.MEDI_MATE_TYPE_ID].Add(mediMateTypeADO);
                                }
                            }

                        }
                    }
                    LoadDataToGridExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetLabelSave(GlobalDataStore.ModuleAction actionModule)
        {
            try
            {
                if (actionModule == GlobalDataStore.ModuleAction.ADD)
                {
                    if (this.IsUsingFunctionKeyInsteadOfCtrlKey)
                    {
                        btnSavePrint.Text = "Lưu in (F9)";
                        btnSave.Text = "Lưu (F5)";
                    }
                    else
                    {
                        btnSavePrint.Text = "Lưu in (Ctrl I)";
                        btnSave.Text = "Lưu (Ctrl S)";
                    }
                }
                else if (actionModule == GlobalDataStore.ModuleAction.EDIT)
                {
                    if (this.IsUsingFunctionKeyInsteadOfCtrlKey)
                    {
                        btnSavePrint.Text = "Lưu in (F9)";
                        btnSave.Text = "Sửa (F5)";
                    }
                    else
                    {
                        btnSavePrint.Text = "Lưu in (Ctrl I)";
                        btnSave.Text = "Sửa (Ctrl S)";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetControlByExpMest(string expMestCode)
        {
            try
            {
                if (!String.IsNullOrEmpty(expMestCode))
                {
                    //moduleAction = GlobalDataStore.ModuleAction.EDIT;
                    //lblExpMestCode.Text = expMestCode;//xuandv bo
                }
                else
                {
                    moduleAction = GlobalDataStore.ModuleAction.ADD;
                    lblExpMestCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ExistServiceNotPaty()
        {
            bool result = false;
            try
            {
                List<MediMateTypeADO> mediMatyFails = dicMediMateAdo.Select(o => o.Value).SelectMany(p => p).Where(o => o.IsNotHasServicePaty).ToList();
                if (mediMatyFails != null && mediMatyFails.Count > 0)
                {
                    MessageBox.Show("Tồn tại thuốc hoặc vật tư không có chính sách giá", "Thông báo");
                    result = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ExistServiceNotInStock()
        {
            bool result = false;
            try
            {
                List<MediMateTypeADO> mediMatyFails = dicMediMateAdo.Select(o => o.Value).SelectMany(p => p).Where(o => o.IsNotInStock).ToList();
                if (mediMatyFails != null && mediMatyFails.Count > 0)
                {
                    MessageBox.Show("Tồn tại thuốc hoặc vật tư không có trong kho", "Thông báo");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ExistServiceExceedsAvailable()
        {
            bool result = false;
            try
            {
                List<MediMateTypeADO> mediMatyFails = dicMediMateAdo.Select(o => o.Value).SelectMany(p => p).Where(o => o.IsExceedsAvailable).ToList();
                if (mediMatyFails != null && mediMatyFails.Count > 0)
                {
                    MessageBox.Show("Tồn tại thuốc hoặc vật tư vượt quá khả dụng", "Thông báo");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckValiDiscount()
        {
            bool rs = true;
            try
            {
                if (spinDiscountRatio.EditValue != null && (spinDiscountRatio.Value >= 100 || spinDiscountRatio.Value < 0))
                {
                    MessageBox.Show("Chiết khấu không được âm và phải nhỏ hơn 100%", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rs = false;
                }
            }
            catch (Exception ex)
            {
                rs = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private bool CheckPatientDob()
        {
            bool rs = true;
            try
            {
                if (!String.IsNullOrWhiteSpace(txtPatientDob.Text))
                {
                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                    if (dateValidObject != null && !String.IsNullOrWhiteSpace(dateValidObject.Message))
                    {
                        MessageBox.Show(dateValidObject.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rs = false;
                    }
                }
            }
            catch (Exception ex)
            {
                rs = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private void FillDataToGridExpMest()
        {
            try
            {
                if (dicMediMateAdo != null)
                {
                    List<MediMateTypeADO> mediMateTypeList = new List<MediMateTypeADO>();
                    var lstMedicine = dicMediMateAdo.Select(o => o.Value).SelectMany(p => p).Where(o => o.IsMedicine == true).ToList();
                    if (lstMedicine != null && lstMedicine.Count > 0)
                    {
                        lstMedicine = lstMedicine.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MEDI_MATE_TYPE_CODE).ToList();

                        // add parent medicine
                        MediMateTypeADO parent = new MediMateTypeADO();
                        parent.MEDI_MATE_TYPE_NAME = "Thuốc";
                        parent.PARENT_ID = "";
                        parent.CHILD_ID = "TH";
                        mediMateTypeList.Add(parent);

                        foreach (var item in lstMedicine)
                        {
                            item.CHILD_ID = item.MEDI_MATE_TYPE_ID + item.SERVICE_REQ_CODE + "TH";
                            item.PARENT_ID = "TH";
                        }
                        mediMateTypeList.AddRange(lstMedicine);

                    }

                    var lstMaterial = dicMediMateAdo.Select(o => o.Value).SelectMany(p => p).Where(o => o.IsMaterial == true).ToList();
                    if (lstMaterial != null && lstMaterial.Count > 0)
                    {
                        lstMaterial = lstMaterial.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MEDI_MATE_TYPE_CODE).ToList();
                        // add parent medicine
                        MediMateTypeADO parent = new MediMateTypeADO();
                        parent.MEDI_MATE_TYPE_NAME = "Vật tư";
                        parent.PARENT_ID = "";
                        parent.CHILD_ID = "VT";
                        mediMateTypeList.Add(parent);

                        foreach (var item in lstMaterial)
                        {
                            item.PARENT_ID = "VT";
                            item.CHILD_ID = item.MEDI_MATE_TYPE_ID + item.SERVICE_REQ_CODE + "VT";
                        }
                        mediMateTypeList.AddRange(lstMaterial);
                    }

                    // treeList
                    var records = new BindingList<MediMateTypeADO>(mediMateTypeList);
                    this.treeListResult.RefreshDataSource();
                    this.treeListResult.DataSource = null;
                    this.treeListResult.DataSource = records;
                    this.treeListResult.KeyFieldName = "CHILD_ID";
                    this.treeListResult.ParentFieldName = "PARENT_ID";
                    this.treeListResult.ExpandAll();
                }
                else
                {
                    this.treeListResult.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataToSaleCreate(ref HisExpMestSaleListSDO saleSDO)
        {
            try
            {
                var Icd = BackendDataWorker.Get<HIS_ICD>();
                decimal? tranferAmountSum = null;
                if (spinTransferAmount.EditValue != null)
                    tranferAmountSum = spinTransferAmount.Value;
                else
                    tranferAmountSum = null;

                List<HisExpMestSaleSDO> listSale = new List<HisExpMestSaleSDO>();

                HisExpMestSaleSDO ado = new HisExpMestSaleSDO();
                var serviceReqCode = ((BindingList<MediMateTypeADO>)treeListMediMate.DataSource).ToList();
                if (serviceReqCode != null && serviceReqCode.Count > 0)
                    ado.ClientSessionKey = serviceReqCode.FirstOrDefault().ClientSessionKey;
                ado.MediStockId = this.mediStock.ID;
                ado.PatientName = txtVirPatientName.Text;
                if (cboGender.EditValue != null)
                    ado.PatientGenderId = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                //if (cboPayForm.EditValue != null)
                //    ado.PayFormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString());

                ado.PatientAddress = txtAddress.Text;
                saleSDO.BuyerEmail = txtEmail.Text;
                //Inventec.Common.Logging.LogSystem.Debug("cboIcds.EditValue: " + cboIcds.EditValue);
                //if (cboIcds.EditValue != null && !String.IsNullOrEmpty(cboIcds.EditValue.ToString()))
                //{
                //    if (Icd != null && Icd.Count > 0)
                //    {
                //        ado.IcdCode = Icd.Where(o => o.ID == (long)cboIcds.EditValue).FirstOrDefault().ICD_CODE;
                //        ado.IcdName = Icd.Where(o => o.ID == (long)cboIcds.EditValue).FirstOrDefault().ICD_NAME;
                //        Inventec.Common.Logging.LogSystem.Debug("cboIcds end: " + cboIcds.EditValue);
                //    }
                //}
                IcdInputADO OjecIcd = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                ado.IcdName = OjecIcd != null ? OjecIcd.ICD_NAME : "";
                ado.IcdCode = OjecIcd != null ? OjecIcd.ICD_CODE : "";
                if (!String.IsNullOrEmpty(txtSubIcdCode.Text))
                {
                    ado.IcdSubCode = txtSubIcdCode.Text;
                }
                if (!String.IsNullOrEmpty(txtIcd.Text))
                {
                    ado.IcdText = txtIcd.Text;
                }
                //if (this.serviceReq != null && !checkIsVisitor.Checked)
                //{
                //    ado.PrescriptionId = this.serviceReq.ID;
                //    ado.TreatmentId = this.serviceReq.TREATMENT_ID;
                //}
                ado.ReqRoomId = this.roomId;
                ado.Description = txtDescription.Text;
                if (spinDiscountRatio.EditValue != null)
                    ado.Discount = spinDiscount.Value;
                if (dtIntructionTime.EditValue != null)
                    ado.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTime.DateTime);

                if (this.moduleAction == GlobalDataStore.ModuleAction.EDIT
                    && this.expMestId.HasValue
                    && this.expMestId.Value > 0)
                {
                    ado.ExpMestId = this.expMestId;
                }

                if (!String.IsNullOrEmpty(txtLoginName.Text))
                {
                    ado.PrescriptionReqLoginname = txtLoginName.Text;
                }
                if (!String.IsNullOrEmpty(txtPresUser.Text))
                {
                    ado.PrescriptionReqUsername = txtPresUser.Text;
                }
                if (cboBillCashierRoom.EditValue != null)
                {
                    long cashierRoomId = Convert.ToInt64(cboBillCashierRoom.EditValue);
                    ado.CashierRoomId = cashierRoomId;
                }

                if (this.Patient != null)
                {
                    ado.PatientId = this.Patient.ID;
                }

                if (!String.IsNullOrWhiteSpace(this.txtPatientPhone.Text))
                {
                    ado.PatientPhone = this.txtPatientPhone.Text.Trim();
                }

                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null && !String.IsNullOrWhiteSpace(dateValidObject.OutDate))
                {
                    this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                    this.dtPatientDob.Update();
                    ado.PatientDob = Convert.ToInt64(this.dtPatientDob.DateTime.ToString("yyyyMMdd") + "000000");
                    if (dateValidObject.HasNotDayDob)
                    {
                        ado.IsHasNotDayDob = true;
                    }
                }
                if (cboPatientType.EditValue != null)
                    ado.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                if (dicMediMateAdo != null)
                {
                    var dataTrees = dicMediMateAdo.Select(o => o.Value).ToList();
                    var dataCoverts = dataTrees.SelectMany(p => p).Distinct().OrderByDescending(o => o.TOTAL_PRICE).ToList();
                    var dataGroups = dataCoverts.GroupBy(p => p.SERVICE_REQ_CODE).Select(p => p.ToList()).ToList();

                    foreach (var items in dataGroups)
                    {
                        //trong trường hợp chọn nhiều hơn bệnh nhân sẽ set lại thông tin bệnh nhân theo y lệnh
                        if (!checkIsVisitor.Checked && this.serviceReq != null && this.serviceReq.Count > 0)
                        {
                            var req = serviceReq.FirstOrDefault(o => o.SERVICE_REQ_CODE == items.First().SERVICE_REQ_CODE);
                            if (req != null)
                            {
                                ado.PrescriptionId = req.ID;
                                ado.TreatmentId = req.TREATMENT_ID;

                                if (serviceReq.Select(s => s.TDL_PATIENT_ID > 0).Distinct().Count() > 1)
                                {
                                    ado.PatientName = req.TDL_PATIENT_NAME;
                                    ado.PatientGenderId = req.TDL_PATIENT_GENDER_ID;
                                    ado.PatientId = req.TDL_PATIENT_ID;
                                    ado.PatientPhone = req.TDL_PATIENT_PHONE;
                                    ado.PrescriptionReqLoginname = req.REQUEST_LOGINNAME;
                                    ado.PrescriptionReqUsername = req.REQUEST_USERNAME;
                                }
                            }
                        }

                        HisExpMestSaleSDO adoNew = new HisExpMestSaleSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisExpMestSaleSDO>(adoNew, ado);
                        if (items[0].EXP_MEST_ID > 0)
                            adoNew.ExpMestId = items[0].EXP_MEST_ID;
                        else
                            adoNew.ExpMestId = null;
                        if (items[0].SERVICE_REQ_ID > 0 && !checkIsVisitor.Checked)
                            adoNew.PrescriptionId = items[0].SERVICE_REQ_ID;
                        else
                            adoNew.PrescriptionId = null;

                        #region MedicineBean
                        adoNew.MedicineBeanIds = null;
                        adoNew.Medicines = null;
                        List<MediMateTypeADO> expMedicines = items.Where(o => o.IsMedicine == true).ToList();
                        if (expMedicines != null && expMedicines.Count > 0)
                        {
                            List<long> beanIds = new List<long>();
                            List<ExpMedicineTypeSDO> medicines = new List<ExpMedicineTypeSDO>();
                            foreach (var expMedicine in expMedicines)
                            {
                                if (expMedicine.BeanIds != null)
                                    beanIds.AddRange(expMedicine.BeanIds);
                                ExpMedicineTypeSDO medicine = new ExpMedicineTypeSDO();
                                medicine.Amount = expMedicine.EXP_AMOUNT ?? 0;
                                medicine.Description = expMedicine.NOTE;
                                medicine.Tutorial = expMedicine.TUTORIAL;
                                medicine.MedicineTypeId = expMedicine.MEDI_MATE_TYPE_ID;
                                medicine.NumOfDays = expMedicine.DayNum;
                                medicine.NumOrder = expMedicine.NUM_ORDER;

                                if (expMedicine.IsCheckExpPrice)
                                {
                                    medicine.Price = expMedicine.EXP_PRICE * (1 + (expMedicine.Profit ?? 0));
                                    medicine.VatRatio = expMedicine.EXP_VAT_RATIO;
                                }

                                if (expMedicine.DISCOUNT_RATIO.HasValue)
                                {
                                    medicine.DiscountRatio = expMedicine.DISCOUNT_RATIO;
                                }

                                medicines.Add(medicine);
                            }
                            adoNew.MedicineBeanIds = beanIds;
                            adoNew.Medicines = medicines;
                        }
                        #endregion

                        #region MaterialBean
                        adoNew.MaterialBeanIds = null;
                        adoNew.Materials = null;
                        List<MediMateTypeADO> expMaterials = items.Where(o => o.IsMaterial == true).ToList();
                        if (expMaterials != null && expMaterials.Count > 0)
                        {
                            List<long> beanIds = new List<long>();
                            List<ExpMaterialTypeSDO> materials = new List<ExpMaterialTypeSDO>();
                            foreach (var expMaterial in expMaterials)
                            {
                                if (expMaterial.BeanIds != null)
                                    beanIds.AddRange(expMaterial.BeanIds);
                                ExpMaterialTypeSDO material = new ExpMaterialTypeSDO();
                                material.Amount = expMaterial.EXP_AMOUNT ?? 0;
                                material.Description = expMaterial.NOTE;
                                material.MaterialTypeId = expMaterial.MEDI_MATE_TYPE_ID;
                                material.NumOrder = expMaterial.NUM_ORDER;

                                if (expMaterial.IsCheckExpPrice)
                                {
                                    material.Price = expMaterial.EXP_PRICE * (1 + (expMaterial.Profit ?? 0));
                                    material.VatRatio = expMaterial.EXP_VAT_RATIO;
                                }

                                if (expMaterial.DISCOUNT_RATIO.HasValue)
                                {
                                    material.DiscountRatio = expMaterial.DISCOUNT_RATIO;
                                }

                                materials.Add(material);
                            }
                            adoNew.MaterialBeanIds = beanIds;
                            adoNew.Materials = materials;
                        }
                        #endregion

                        listSale.Add(adoNew);
                    }
                }
                saleSDO.SaleData = listSale;
                if (chkCreateBill.Checked)
                {
                    if (cboBillCashierRoom.EditValue != null)
                        saleSDO.CashierRoomId = Convert.ToInt64(cboBillCashierRoom.EditValue);


                    saleSDO.AccountBookId = Convert.ToInt64(cboBillAccountBook.EditValue);
                    saleSDO.CreateBill = true;
                    if (chkRoundPrice.Checked)
                    {
                        saleSDO.RoundedPriceBase = spinBaseValue.Value;
                        saleSDO.RoundedTotalPrice = totalReceivable;
                    }
                    if (spinBillNumOrder.Enabled)
                    {
                        saleSDO.TransactionNumOrder = (long)spinBillNumOrder.Value;
                    }
                    if (tranferAmountSum.HasValue && tranferAmountSum.Value > 0)
                    {
                        var payform_ = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == (long)cboPayForm.EditValue);
                        if (payform_ != null && (payform_.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK || payform_.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK))
                        {
                            saleSDO.TransferAmount = tranferAmountSum;
                        }

                    }
                }
                if (cboPayForm.EditValue != null)
                    saleSDO.PayFormId = Convert.ToInt64(cboPayForm.EditValue);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitListDataToSaleCreate(ref List<HisExpMestSaleListSDO> listSaleSDO)
        {
            try
            {
                var Icd = BackendDataWorker.Get<HIS_ICD>();
                decimal? tranferAmountSum = null;
                if (spinTransferAmount.EditValue != null)
                    tranferAmountSum = spinTransferAmount.Value;
                else
                    tranferAmountSum = null;

                if (dicMediMateAdo != null)
                {
                    var dataTrees = dicMediMateAdo.Select(o => o.Value).ToList();
                    var dataCoverts = dataTrees.SelectMany(p => p).Distinct().OrderByDescending(o => o.TOTAL_PRICE).ToList();
                    var dataGroups = dataCoverts.GroupBy(p => p.SERVICE_REQ_CODE).Select(p => p.ToList()).ToList();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo), dicMediMateAdo));

                    int count = 0;
                    foreach (var item in dataGroups)
                    {
                        List<HisExpMestSaleSDO> listSale = new List<HisExpMestSaleSDO>();
                        HisExpMestSaleListSDO saleSDO = new HisExpMestSaleListSDO();
                        HisExpMestSaleSDO ado = new HisExpMestSaleSDO();

                        ado.ClientSessionKey = item.First().ClientSessionKey;
                        ado.MediStockId = this.mediStock.ID;
                        ado.PatientName = item[0].TDL_PATIENT_NAME;
                        ado.PatientGenderId = item[0].TDL_PATIENT_GENDER_ID;
                        ado.PatientAddress = item[0].TDL_PATIENT_ADDRESS;
                        saleSDO.BuyerEmail = txtEmail.Text;
                        IcdInputADO OjecIcd = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                        ado.IcdName = OjecIcd != null ? OjecIcd.ICD_NAME : "";
                        ado.IcdCode = OjecIcd != null ? OjecIcd.ICD_CODE : "";
                        if (!String.IsNullOrEmpty(txtSubIcdCode.Text))
                        {
                            ado.IcdSubCode = txtSubIcdCode.Text;
                        }
                        if (!String.IsNullOrEmpty(txtIcd.Text))
                        {
                            ado.IcdText = txtIcd.Text;
                        }
                        ado.ReqRoomId = this.roomId;
                        ado.Description = txtDescription.Text;
                        if (dtIntructionTime.EditValue != null)
                            ado.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTime.DateTime);

                        if (this.moduleAction == GlobalDataStore.ModuleAction.EDIT
                            && this.expMestId.HasValue
                            && this.expMestId.Value > 0)
                        {
                            ado.ExpMestId = this.expMestId;
                        }
                        ado.PrescriptionReqLoginname = txtLoginName.Text;
                        ado.PrescriptionReqUsername = txtPresUser.Text;
                        if (cboBillCashierRoom.EditValue != null)
                        {
                            long cashierRoomId = Convert.ToInt64(cboBillCashierRoom.EditValue);
                            ado.CashierRoomId = cashierRoomId;
                        }
                        ado.PatientId = item[0].TDL_PATIENT_ID;
                        ado.PatientPhone = item[0].TDL_PATIENT_PHONE;
                        ado.PatientDob = item[0].TDL_PATIENT_DOB;
                        ado.IsHasNotDayDob = item[0].TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? true : false;
                        if (cboPatientType.EditValue != null)
                            ado.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                        if (spinDiscountRatio.EditValue != null)
                            ado.Discount = item.Sum(o => o.TOTAL_PRICE * spinDiscountRatio.Value / 100) ?? 0;
                        if (!checkIsVisitor.Checked && this.serviceReq != null && this.serviceReq.Count > 0)
                        {
                            var req = serviceReq.FirstOrDefault(o => o.SERVICE_REQ_CODE == item.First().SERVICE_REQ_CODE);
                            if (req != null)
                            {
                                ado.PrescriptionId = req.ID;
                                ado.TreatmentId = req.TREATMENT_ID;

                                if (serviceReq.Select(s => s.TDL_PATIENT_ID > 0).Distinct().Count() > 1)
                                {
                                    ado.PatientName = req.TDL_PATIENT_NAME;
                                    ado.PatientGenderId = req.TDL_PATIENT_GENDER_ID;
                                    ado.PatientId = req.TDL_PATIENT_ID;
                                    ado.PatientPhone = req.TDL_PATIENT_PHONE;
                                    ado.PrescriptionReqLoginname = req.REQUEST_LOGINNAME;
                                    ado.PrescriptionReqUsername = req.REQUEST_USERNAME;
                                }
                            }
                        }

                        HisExpMestSaleSDO adoNew = new HisExpMestSaleSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisExpMestSaleSDO>(adoNew, ado);
                        if (item[0].EXP_MEST_ID > 0)
                            adoNew.ExpMestId = item[0].EXP_MEST_ID;
                        else
                            adoNew.ExpMestId = null;
                        if (item[0].SERVICE_REQ_ID > 0 && !checkIsVisitor.Checked)
                            adoNew.PrescriptionId = item[0].SERVICE_REQ_ID;
                        else
                            adoNew.PrescriptionId = null;

                        #region MedicineBean
                        adoNew.MedicineBeanIds = null;
                        adoNew.Medicines = null;

                        List<MediMateTypeADO> expMedicines = item.Where(o => o.IsMedicine == true).ToList();
                        if (expMedicines != null && expMedicines.Count > 0)
                        {
                            List<long> beanIds = new List<long>();
                            List<ExpMedicineTypeSDO> medicines = new List<ExpMedicineTypeSDO>();
                            foreach (var expMedicine in expMedicines)
                            {
                                if (expMedicine.BeanIds != null)
                                    beanIds.AddRange(expMedicine.BeanIds);
                                ExpMedicineTypeSDO medicine = new ExpMedicineTypeSDO();
                                medicine.Amount = expMedicine.EXP_AMOUNT ?? 0;
                                medicine.Description = expMedicine.NOTE;
                                medicine.Tutorial = expMedicine.TUTORIAL;
                                medicine.MedicineTypeId = expMedicine.MEDI_MATE_TYPE_ID;
                                medicine.NumOfDays = expMedicine.DayNum;
                                medicine.NumOrder = expMedicine.NUM_ORDER;

                                if (expMedicine.IsCheckExpPrice)
                                {
                                    medicine.Price = expMedicine.EXP_PRICE * (1 + (expMedicine.Profit ?? 0));
                                    medicine.VatRatio = expMedicine.EXP_VAT_RATIO;
                                }

                                if (expMedicine.DISCOUNT_RATIO.HasValue)
                                {
                                    medicine.DiscountRatio = expMedicine.DISCOUNT_RATIO;
                                }

                                medicines.Add(medicine);
                            }
                            adoNew.MedicineBeanIds = beanIds;
                            adoNew.Medicines = medicines;
                        }
                        #endregion

                        #region MaterialBean
                        adoNew.MaterialBeanIds = null;
                        adoNew.Materials = null;
                        List<MediMateTypeADO> expMaterials = item.Where(o => o.IsMaterial == true).ToList();
                        if (expMaterials != null && expMaterials.Count > 0)
                        {
                            List<long> beanIds = new List<long>();
                            List<ExpMaterialTypeSDO> materials = new List<ExpMaterialTypeSDO>();
                            foreach (var expMaterial in expMaterials)
                            {
                                if (expMaterial.BeanIds != null)
                                    beanIds.AddRange(expMaterial.BeanIds);
                                ExpMaterialTypeSDO material = new ExpMaterialTypeSDO();
                                material.Amount = expMaterial.EXP_AMOUNT ?? 0;
                                material.Description = expMaterial.NOTE;
                                material.MaterialTypeId = expMaterial.MEDI_MATE_TYPE_ID;
                                material.NumOrder = expMaterial.NUM_ORDER;

                                if (expMaterial.IsCheckExpPrice)
                                {
                                    material.Price = expMaterial.EXP_PRICE * (1 + (expMaterial.Profit ?? 0));
                                    material.VatRatio = expMaterial.EXP_VAT_RATIO;
                                }

                                if (expMaterial.DISCOUNT_RATIO.HasValue)
                                {
                                    material.DiscountRatio = expMaterial.DISCOUNT_RATIO;
                                }

                                materials.Add(material);
                            }
                            adoNew.MaterialBeanIds = beanIds;
                            adoNew.Materials = materials;
                        }
                        #endregion

                        listSale.Add(adoNew);
                        saleSDO.SaleData = listSale;
                        if (chkCreateBill.Checked)
                        {
                            if (cboBillCashierRoom.EditValue != null)
                                saleSDO.CashierRoomId = Convert.ToInt64(cboBillCashierRoom.EditValue);


                            saleSDO.AccountBookId = Convert.ToInt64(cboBillAccountBook.EditValue);
                            saleSDO.CreateBill = true;
                            if (chkRoundPrice.Checked)
                            {
                                saleSDO.RoundedPriceBase = spinBaseValue.Value;

                                saleSDO.RoundedTotalPrice = CalculReceivable(item.Sum(o => (o.TOTAL_PRICE - o.TOTAL_PRICE * spinDiscountRatio.Value / 100)));
                            }
                            if (spinBillNumOrder.Enabled)
                            {
                                saleSDO.TransactionNumOrder = (long)spinBillNumOrder.Value;
                            }
                            if (tranferAmountSum.HasValue && tranferAmountSum.Value > 0)
                            {
                                var payform_ = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == (long)cboPayForm.EditValue);
                                if (payform_ != null && (payform_.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK || payform_.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK))
                                {
                                    saleSDO.TransferAmount = tranferAmountSum;
                                }

                            }
                        }
                        if (cboPayForm.EditValue != null)
                            saleSDO.PayFormId = Convert.ToInt64(cboPayForm.EditValue);
                        listSaleSDO.Add(saleSDO);
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DiscountDisplayProcess(bool discountFocus, bool discountRatioFocus, SpinEdit spinDiscount, SpinEdit spinDiscountRatio, decimal totalPrice)
        {
            try
            {
                if (discountFocus && !discountRatioFocus && spinDiscount.EditValue != null && totalPrice > 0)
                {
                    spinDiscountRatio.Value = (spinDiscount.Value / totalPrice) * 100;
                }
                if (discountRatioFocus && !discountFocus && spinDiscountRatio.EditValue != null && totalPrice > 0)
                {
                    spinDiscount.Value = (spinDiscountRatio.Value / 100) * totalPrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DiscountByAmountAndPriceChange()
        {
            try
            {
                if (this.spinExpPrice.EditValue != null && this.spinExpPrice.Value > 0 && this.spinAmount.EditValue != null && this.spinAmount.Value > 0)
                {
                    if (this.discountDetailFocus)
                    {
                        spinDiscountDetailRatio.Value = (this.spinDiscountDetail.Value / (this.spinAmount.Value * this.spinExpPrice.Value) * 100);
                    }
                    if (this.discountDetailRatioFocus)
                    {
                        spinDiscountDetail.Value = (this.spinDiscountDetailRatio.Value / 100) * (this.spinAmount.Value * this.spinExpPrice.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void DeleteRowExpMestDetail(MediMateTypeADO mediMate)
        {
            try
            {
                //MediMateTypeADO mediMate = null;// gridViewExpMestDetail.GetFocusedRow() as MediMateTypeADO;
                if (mediMate != null)
                {
                    bool release = false;
                    CommonParam param = new CommonParam();
                    ReleaseBeanSDO releaseBean = new ReleaseBeanSDO();
                    releaseBean.BeanIds = mediMate.BeanIds;
                    releaseBean.ClientSessionKey = mediMate.ClientSessionKey;
                    releaseBean.MediStockId = this.mediStock.ID;
                    releaseBean.TypeId = mediMate.MEDI_MATE_TYPE_ID;

                    if (mediMate.IsMedicine)
                    {
                        release = new BackendAdapter(param)
                   .Post<bool>(RequestUriStore.HIS_MEDICINE_BEAN__RELEASE, ApiConsumers.MosConsumer, releaseBean, param);
                    }
                    else if (mediMate.IsMaterial)
                    {
                        release = new BackendAdapter(param)
                       .Post<bool>(RequestUriStore.HIS_MATERIAL_BEAN__RELEASE, ApiConsumers.MosConsumer, releaseBean, param);
                    }

                    if (release)
                    {
                        //dicMediMateAdo.Remove(mediMate.MEDI_MATE_TYPE_ID);
                        dicMediMateAdo.FirstOrDefault(p => p.Key == mediMate.MEDI_MATE_TYPE_ID).Value.Remove(mediMate);
                        SetTotalPriceExpMestDetail();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void EditRowExpMestDetail(MediMateTypeADO dataEdit)
        {
            try
            {
                if (dataEdit != null)
                {
                    this.Action = GlobalDataStore.ActionEdit;
                    btnAdd.Enabled = true;
                    txtTutorial.Text = dataEdit.TUTORIAL;
                    txtNote.Text = dataEdit.NOTE;
                    spinAmount.Value = dataEdit.EXP_AMOUNT ?? 0;
                    checkImpExpPrice.Enabled = true;
                    checkImpExpPrice.CheckState = dataEdit.IsCheckExpPrice ?
                        CheckState.Checked : CheckState.Unchecked;
                    if (dataEdit.Profit != null)
                    {
                        spinExpPrice.EditValue = null;
                        spinProfit.Value = (dataEdit.Profit ?? 0) * 100;
                    }
                    else
                    {
                        spinExpPrice.Value = dataEdit.EXP_PRICE ?? 0;
                        spinProfit.EditValue = null;
                    }

                    if (dataEdit.EXP_VAT_RATIO.HasValue && dataEdit.EXP_VAT_RATIO.Value > 0)
                        spinExpVatRatio.Value = (dataEdit.EXP_VAT_RATIO.Value * 100);
                    else
                        spinExpVatRatio.EditValue = null;
                    if (dataEdit.DISCOUNT_RATIO.HasValue && dataEdit.DISCOUNT_RATIO.Value > 0)
                    {
                        spinDiscountDetailRatio.Value = dataEdit.DISCOUNT_RATIO.Value * 100;
                        spinDiscountDetail.Value = dataEdit.DISCOUNT_RATIO.Value * (dataEdit.TOTAL_PRICE ?? 0);
                        this.discountDetailRatioFocus = true;
                    }
                    else
                    {
                        spinDiscountDetailRatio.EditValue = null;
                        spinDiscountDetail.EditValue = null;
                    }
                    if (dataEdit.IsMedicine)
                    {
                        txtTutorial.Enabled = true;
                        if (dataEdit.DayNum.HasValue)
                        {
                            spinDayNum.Value = dataEdit.DayNum.Value;
                            oldDayNum = dataEdit.DayNum.Value;
                        }
                        else
                        {
                            spinDayNum.EditValue = null;
                            oldDayNum = 1;
                        }
                        spinDayNum.Enabled = true;
                    }
                    else
                    {
                        txtTutorial.Enabled = false;
                        spinDayNum.Enabled = false;
                        spinDayNum.EditValue = null;
                    }

                    // spinAmount.Focus();
                    spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {
                UpdateSpinTransferAmountControl(false);
                if (payForm != null && (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK || payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK))
                {
                    ValidControlTransferAmount(true);
                    //UpdateSpinTransferAmountControl(true);
                    dxValidationProvider_Save.RemoveControlError(spinTransferAmount);
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;
                    lciTranferAmount.Text = "Số tiền chuyển khoản:";
                }
                else if (payForm != null && (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE || payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT))
                {
                    //UpdateSpinTransferAmountControl(true);
                    dxValidationProvider_Save.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;
                    lciTranferAmount.Text = "Số tiền quẹt thẻ:";
                }
                else
                {
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSpinTransferAmountControl(bool isTMCK)
        {
            try
            {
                dxValidationProvider_Save.RemoveControlError(spinTransferAmount);
                ValidControlTransferAmount(isTMCK);
                if (isTMCK)
                {
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;
                }
                else
                {
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UpdateSpinTransferAmountControlQT(bool IsQT)
        {
            try
            {
                //dxValidationProvider_Save.RemoveControlError(spinQuetThe);
                //ValidControlTransferAmount(IsQT);
                //if (IsQT)
                //{
                //    lcQuetthe.AppearanceItemCaption.ForeColor = Color.Maroon;
                //    lcQuetthe.Enabled = true;
                //}
                //else
                //{
                //    lcQuetthe.AppearanceItemCaption.ForeColor = Color.Black;
                //    lcQuetthe.Enabled = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidControlTransferAmount(bool IsRequiredField)
        {
            try
            {
                SpinTranferAmountValidationRule TranferAmountValidate = new SpinTranferAmountValidationRule();
                TranferAmountValidate.spinTranferAmount = spinTransferAmount;
                TranferAmountValidate.isRequiredPin = IsRequiredField;
                dxValidationProvider_Save.SetValidationRule(spinTransferAmount, TranferAmountValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                UCExpMestSaleCreate.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                UCExpMestSaleCreate.currentControlStateRDO = UCExpMestSaleCreate.controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in UCExpMestSaleCreate.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_PRINT_NOW)
                        {
                            chkPrintNow.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAutoShow.Name)
                        {
                            chkAutoShow.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHK_CREATE_BILL)
                        {
                            chkCreateBill.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHK_ROUND_PRICE)
                        {
                            chkRoundPrice.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.SPIN_BASE_VALUE)
                        {
                            spinBaseValue.Value = Convert.ToInt64(item.VALUE);
                        }
                        else if (item.KEY == ControlStateConstant.CHK_KetNoiPos)
                        {
                            ChkKetNoiPOS.Checked = item.VALUE == "1";
                        }
                    }
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessorSearchPatient()
        {
            try
            {
                if (!String.IsNullOrEmpty(txtPatientCode.Text))
                {
                    CommonParam param = new CommonParam();
                    HisPatientFilter filter = new HisPatientFilter();

                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }

                    txtPatientCode.Text = code;
                    filter.PATIENT_CODE = code;
                    var listPatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listPatient != null && listPatient.Count > 0)
                    {
                        this.Patient = listPatient.FirstOrDefault();
                        FillDataPatient(this.Patient);
                        this.txtMediMatyForPrescription.Focus();
                        this.txtMediMatyForPrescription.SelectAll();
                    }
                    else
                    {
                        XtraMessageBox.Show(ResourceMessageLang.MaBenhNhanKhongTonTai);
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void FillDataPatient(HIS_PATIENT patient)
        {
            try
            {
                if (patient != null)
                {
                    txtEmail.Text = patient.EMAIL;   
                    txtPatientCode.Text = patient.PATIENT_CODE;
                    txtVirPatientName.Text = patient.VIR_PATIENT_NAME;
                    cboGender.EditValue = patient.GENDER_ID;
                    if (patient.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDob.Text = patient.DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                    }

                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                    if (!String.IsNullOrWhiteSpace(patient.DISTRICT_CODE))
                    {
                        commune = commune.Where(o => o.DISTRICT_CODE == patient.DISTRICT_CODE).ToList();
                    }

                    if (!String.IsNullOrWhiteSpace(patient.PROVINCE_CODE))
                    {
                        commune = commune.Where(o => o.PROVINCE_CODE == Patient.PROVINCE_CODE).ToList();
                    }

                    if (!String.IsNullOrWhiteSpace(patient.COMMUNE_CODE))
                    {
                        commune = commune.Where(o => o.COMMUNE_CODE == patient.COMMUNE_CODE).ToList();
                    }

                    //txtAddress.Text = patient.ADDRESS;
                    if (commune != null && commune.Count == 1)
                    {
                        this.txtMaTHX.Text = commune.First().SEARCH_CODE_COMMUNE;
                        this.cboTHX.EditValue = commune.First().ID_RAW;
                    }
                    else
                    {
                        this.txtMaTHX.Text = null;
                        this.cboTHX.EditValue = null;
                    }

                    txtAddress.Text = patient.VIR_ADDRESS;
                    txtPatientPhone.Text = patient.PHONE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSearchPatientByInfo()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtVirPatientName.Text) && cboGender.EditValue != null && !String.IsNullOrWhiteSpace(txtPatientDob.Text) && HisConfigCFG.IS_MANAGE_PATIENT_IN_SALE && String.IsNullOrWhiteSpace(txtPatientCode.Text))
                {
                    CommonParam param = new CommonParam();
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.PATIENT_NAME = txtVirPatientName.Text.Trim();
                    filter.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());

                    if (this.txtPatientDob.Text.Length == 4)
                    {
                        filter.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(this.txtPatientDob.Text + "0101000000");
                    }
                    else
                    {
                        DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
                        if (dt.HasValue)
                        {
                            filter.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dt.Value.ToString("yyyyMMdd") + "000000");
                        }
                    }
                    var listPatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listPatient != null && listPatient.Count > 0)
                    {
                        FormChoosePatient.FormPatients choose = new FormChoosePatient.FormPatients(listPatient, ChoosePatient);
                        choose.ShowDialog();

                        if (this.Patient != null)
                        {
                            this.txtMediMatyForPrescription.Focus();
                            this.txtMediMatyForPrescription.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChoosePatient(HIS_PATIENT patient)
        {
            try
            {
                if (patient != null)
                {
                    this.Patient = patient;
                    FillDataPatient(this.Patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectMultiPrescription()
        {
            try
            {
                FormChoosePrescription.FormPrescription choose = new FormChoosePrescription.FormPrescription(this.currentModuleBase, ChoosePrescription);
                choose.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChoosePrescription(List<V_HIS_SERVICE_REQ_11> listData)
        {
            try
            {
                if (listData != null && listData.Count > 0)
                {
                    this.serviceReq = listData;
                    ProcessorSearch(false, listData.Select(s => s.ID).Distinct().ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT ProcessGetPatientById(long patientId)
        {
            HIS_PATIENT result = null;
            try
            {
                if (patientId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = patientId;
                    var listPatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listPatient != null && listPatient.Count > 0)
                    {
                        result = listPatient.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
