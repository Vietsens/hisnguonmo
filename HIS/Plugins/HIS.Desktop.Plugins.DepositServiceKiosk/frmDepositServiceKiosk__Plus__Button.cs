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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.DepositServiceKiosk.ADO;
using HIS.Desktop.Plugins.DepositServiceKiosk.Base;
using HIS.Desktop.Plugins.DepositServiceKiosk.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.DocumentViewer;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepositServiceKiosk
{
    public partial class frmDepositServiceKiosk : HIS.Desktop.Utility.FormBase
    {
        private string Print106Type = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail");
        bool isPrintNow = false;

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                SetEnableButtonSave(false);
                if (!dxValidationProvider1.Validate() || this.treatmentId == null)
                {
                    SetEnableButtonSave(true);
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool? success = false;
                success = ProcessSave(ref param);
                WaitingManager.Hide();
                if (success == true)
                {
                    this.onClickPhieuThuThanhToanKiosk(null, null);
                }
                if (success == false)
                {
                    SetEnableButtonSave(true);
                    MessageManager.Show(param, success);
                }
                // SessionManager.ProcessTokenLost(param);
                GeneratePopupMenu();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // kiểm tra hình thức thanh toán có phải là thanh toán qua thẻ không, nếu là thanh toán qua thẻ thì update màu sắc và validate textBox PIN
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (accountBook == null || accountBook.ID == 0)
                {
                    MessageBox.Show("Không chọn được số thu chi. Vui lòng liên hệ với nhân viên bệnh viện");
                }
                this.positionHandleControl = -1;
                if (!btnSave.Enabled && !lciBtnSave.Enabled)
                    return;
                SetEnableButtonSave(false);
                if (!dxValidationProvider1.Validate())
                {
                    SetEnableButtonSave(true);
                    return;
                }
                CommonParam param = new CommonParam();
                bool? success = false;
                success = ProcessSave(ref param);
                if (success == true)
                {
                    if (this.refeshReference != null)
                    {
                        this.refeshReference();
                    }
                    Inventec.Common.Logging.LogSystem.Debug("stopSave bill");
                    MessageManager.Show(this, param, success);
                    onClickPhieuThuThanhToanKiosk(null, null);
                }
                else if (success == false)
                {
                    SetEnableButtonSave(true);
                    Inventec.Common.Logging.LogSystem.Debug("can not stopSave bill");
                    MessageManager.Show(param, success);
                }
                //GeneratePopupMenu();
                SessionManager.ProcessTokenLost(param);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // gọi sang WCF thanh toán qua thẻ
        CARD.WCF.DCO.WcfSaleDCO SaleCard(ref CARD.WCF.DCO.WcfSaleDCO SaleDCO, CommonParam param)
        {
            CARD.WCF.DCO.WcfSaleDCO result = null;
            try
            {
                stopThread = true;
                // gọi api HisCard/Get để lấy về serviceCodes
                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = this.currentTreatment.PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                stopThread = false;
                ResetLoopCount();
                if (HisCards != null && HisCards.Count > 0)
                {
                    SaleDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();
                }
                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                result = transactionClientManager.Sale(SaleDCO);
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void UpdateDataFormTransactionDepositToDTO(ref HisTransactionDepositSDO transactionData, MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE treatment)
        {
            try
            {
                if (transactionData == null)
                {
                    transactionData = new HisTransactionDepositSDO();
                    transactionData.Transaction = new HIS_TRANSACTION();
                }

                transactionData.SereServDeposits = new List<HIS_SERE_SERV_DEPOSIT>();

                transactionData.RequestRoomId = this.cashierRoom.ROOM_ID;

                //transactionData.Transaction.AMOUNT = (txtAmount.Tag != null && !string.IsNullOrEmpty(txtAmount.Tag.ToString())) ? Convert.ToDecimal(txtAmount.Tag.ToString()) : 0;
                transactionData.Transaction.AMOUNT = this.totalPatientPrice;
                transactionData.Transaction.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU;
                transactionData.Transaction.ACCOUNT_BOOK_ID = this.accountBook.ID;
                transactionData.Transaction.PAY_FORM_ID = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE;

                transactionData.Transaction.CASHIER_ROOM_ID = this.cashierRoom.ID;
                transactionData.Transaction.TRANSACTION_TIME = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (treatment != null)
                {
                    transactionData.Transaction.TREATMENT_ID = treatment.ID;
                    transactionData.Transaction.TDL_TREATMENT_CODE = treatment.TREATMENT_CODE;
                }
                SetSereServToDataTransfer(this.sereServByTreatment, transactionData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSereServToDataTransfer(List<V_HIS_SERE_SERV_5> sereServADOs, HisTransactionDepositSDO transactionData)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("Giá trị của key MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")), HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")));

                Inventec.Common.Logging.LogSystem.Warn("Giá trị của sereServADOs is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOs), sereServADOs));

                foreach (var sereServAdo in sereServADOs)
                {
                    HIS_SERE_SERV_DEPOSIT sereServDeposit = new HIS_SERE_SERV_DEPOSIT();
                    sereServDeposit.SERE_SERV_ID = sereServAdo.ID;
                    if (sereServAdo.PATIENT_TYPE_ID == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"))
                    {
                        V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
                        sereServ = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>(sereServAdo);
                        if (SetDefaultDepositPrice == 1)
                        {
                            sereServDeposit.AMOUNT = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(sereServ) ?? 0;
                        }
                        else if (SetDefaultDepositPrice == 2)
                        {
                            sereServDeposit.AMOUNT = sereServAdo.VIR_TOTAL_PRICE ?? 0;
                        }
                        else
                        {
                            sereServDeposit.AMOUNT = sereServAdo.VIR_TOTAL_PATIENT_PRICE ?? 0;
                        }
                        Inventec.Common.Logging.LogSystem.Warn("PATIENT_TYPE_ID!=BHYT Giá trị của sereServDeposit.AMOUNT is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServDeposit.AMOUNT), sereServDeposit.AMOUNT));
                    }
                    else
                    {
                        sereServDeposit.AMOUNT = sereServAdo.VIR_TOTAL_PATIENT_PRICE ?? 0;
                        Inventec.Common.Logging.LogSystem.Warn("PATIENT_TYPE_ID==BHYT Giá trị của sereServDeposit.AMOUNT is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServDeposit.AMOUNT), sereServDeposit.AMOUNT));
                    }

                    transactionData.SereServDeposits.Add(sereServDeposit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool? ProcessSave(ref CommonParam param, [Optional] bool isLuuKy)
        {
            this.isPrintNow = false;
            bool? success = false;
            try
            {
                gridViewSereServ.PostEditor();
                gridViewSereServ.UpdateCurrentRow();
                CARD.WCF.DCO.WcfSaleDCO saleDCO = null;

                var dataAll = (List<V_HIS_SERE_SERV_5>)gridControlSereServ.DataSource;
                if (dataAll == null || dataAll.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuChonDichVuDeThanhToan);
                    return success;
                }
                WaitingManager.Show();
                MOS.SDO.HisTransactionDepositSDO hisDepositSDO = new HisTransactionDepositSDO();
                hisDepositSDO.Transaction = new HIS_TRANSACTION();
                hisDepositSDO.SereServDeposits = new List<HIS_SERE_SERV_DEPOSIT>();

                UpdateDataFormTransactionDepositToDTO(ref hisDepositSDO, this.currentTreatment);

                stopThread = true;
                //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_TRANSACTION_CHECK_DEPOSIT, ApiConsumers.MosConsumer, hisDepositSDO, param);
                stopThread = false;
                ResetLoopCount();
                if (!check)
                {
                    WaitingManager.Hide();
                    return success;
                }
                else
                {
                    // gọi WCF tab thẻ (POS)

                    if (totalPatientPrice > 0)
                    {
                        CARD.WCF.DCO.WcfSaleDCO SaleDCO = new CARD.WCF.DCO.WcfSaleDCO();
                        SaleDCO.Amount = totalPatientPrice;
                        saleDCO = SaleCard(ref SaleDCO, param);
                        // nếu gọi sang POS trả về false thì kết thúc
                        Inventec.Common.Logging.LogSystem.Debug("saleDCO output: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saleDCO), saleDCO));
                        if (saleDCO == null || (saleDCO.ResultCode == null || !saleDCO.ResultCode.Equals("00")))
                        {
                            MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                            success = false;
                            Inventec.Common.Logging.LogSystem.Info("saleDCO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saleDCO), saleDCO));
                            //param.Messages.Add(ResourceMessageLang.ThanhToanTheThatBai);
                            if (saleDCO != null
                                && !String.IsNullOrWhiteSpace(saleDCO.ResultCode)
                                && mappingErrorTHE.dicMapping != null
                                && mappingErrorTHE.dicMapping.Count > 0
                                && mappingErrorTHE.dicMapping.ContainsKey(saleDCO.ResultCode))
                            {
                                param.Messages.Add(mappingErrorTHE.dicMapping[saleDCO.ResultCode]);
                            }
                            else if (saleDCO != null && String.IsNullOrWhiteSpace(saleDCO.ResultCode))
                            {
                                param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                            }
                            else if (saleDCO != null
                                && !String.IsNullOrWhiteSpace(saleDCO.ResultCode)
                                && mappingErrorTHE.dicMapping != null
                                && mappingErrorTHE.dicMapping.Count > 0
                                && !mappingErrorTHE.dicMapping.ContainsKey(saleDCO.ResultCode)
                                )
                            {
                                param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                            }
                            WaitingManager.Hide();
                            return success;
                        }
                        else
                        {
                            hisDepositSDO.Transaction.TIG_TRANSACTION_CODE = saleDCO.TransactionCode;
                            hisDepositSDO.Transaction.TIG_TRANSACTION_TIME = saleDCO.TransactionTime;
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show("Số tiền phải thu bệnh nhân <= 0. Bạn không thể chọn hình thức thanh toán qua thẻ. ");
                        return null;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("Dau vao khi goi api: HisTransaction/CreateDeposit: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisDepositSDO), hisDepositSDO));
                stopThread = true;
                this.hisDeposit = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>(UriStores.HIS_TRANSACTION_CREATE_DEPOSIT, ApiConsumers.MosConsumer, hisDepositSDO, param);
                stopThread = false;
                ResetLoopCount();
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Debug("Ket qua tra ve khi goi api: HisTransaction/CreateDeposit: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.hisDeposit), this.hisDeposit));
                if (this.hisDeposit != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("HisTransaction/CreateDeposit rs != null");
                    success = true;
                    AddLastAccountToLocal();
                    this.resultTranBill = this.hisDeposit;
                    ddBtnPrint.Enabled = true;
                    btnSave.Enabled = false;
                    lciBtnSave.Enabled = false;
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void ProcessAddLastAccount()
        {
            System.Threading.Thread add = new System.Threading.Thread(AddLastAccountToLocal);
            try
            {
                add.Start();
            }
            catch (Exception ex)
            {
                add.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddLastAccountToLocal()
        {
            try
            {
                if (GlobalVariables.LastAccountBook == null) GlobalVariables.LastAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                if (accountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == accountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_DEPOSIT == 1 && o.ID != accountBook.ID).ToList();// && o.BILL_TYPE_ID == accountBook.BILL_TYPE_ID).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(accountBook);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GeneratePopupMenu()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_DROP_DOWN__ITEM_PHIEU_THU_THANH_TOAN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuThanhToanKiosk)));
                //ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuThuThanhToanKiosk(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000375", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000375":
                        InPhieuThuThanhToanKiosk(printCode, fileName, ref result);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuThuThanhToanKiosk(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                stopThread = true;
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServDepositFilter dereDetailFiter = new MOS.Filter.HisSereServDepositFilter();
                dereDetailFiter.DEPOSIT_ID = this.hisDeposit.ID;
                var dereDetails = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, dereDetailFiter, param);
                var sereServIds = dereDetails.Select(o => o.SERE_SERV_ID).ToList();

                MOS.Filter.HisSereServView12Filter sereServFilter = new MOS.Filter.HisSereServView12Filter();
                sereServFilter.TREATMENT_ID = this.treatmentId;
                sereServFilter.IDs = sereServIds;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_12>>("api/HisSereServ/GetView12", ApiConsumers.MosConsumer, sereServFilter, param);
                foreach (var item in sereServs)
                {
                    var itemCheck = dereDetails.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                    if (itemCheck != null)
                    {
                        item.VIR_TOTAL_PATIENT_PRICE = itemCheck.AMOUNT;
                    }
                }
                DepositServicePrintProcess.LoadPhieuThuPhiDichVu("Mps000375", fileName, true, sereServs, this.currentTreatment, sereServIds, this.hisDeposit, dereDetails, FirstExamRoom(), isPrintNow, this.currentModule);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public V_HIS_SERVICE_REQ FirstExamRoom()
        {

            V_HIS_SERVICE_REQ result = null;
            try
            {
                stopThread = true;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();

                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;

                serviceReqFilter.TREATMENT_ID = this.treatmentId;

                var listServiceReq = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, paramCommon);
                stopThread = false;
                ResetLoopCount();
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    result = listServiceReq.OrderBy(o => o.CREATE_TIME).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TREATMENT GetTreatment(long? treatmentId)
        {
            HIS_TREATMENT result = new HIS_TREATMENT();
            try
            {
                if (treatmentId.HasValue)
                {
                    stopThread = true;
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
                    stopThread = false;
                    ResetLoopCount();
                    if (apiresult != null && apiresult.Count > 0)
                    {
                        result = apiresult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                result = new HIS_TREATMENT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private decimal GetDepositAmount(long? treatmentId)
        {
            decimal result = 0;
            try
            {
                if (treatmentId.HasValue)
                {
                    stopThread = true;
                    HisTransactionFilter filter = new HisTransactionFilter();
                    filter.TREATMENT_ID = treatmentId;
                    filter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, null);
                    stopThread = false;
                    ResetLoopCount();
                    if (apiresult != null && apiresult.Count > 0)
                    {
                        foreach (var item in apiresult)
                        {
                            if (item.IS_CANCEL != 1)
                            {
                                result += item.AMOUNT;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #region InVaCloseForm


        byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        #endregion

        private void SetEnableButtonSave(bool enable)
        {
            try
            {
                btnSave.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

}

