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
using HIS.Desktop.Plugins.TransactionBillKiosk.ADO;
using HIS.Desktop.Plugins.TransactionBillKiosk.Base;
using HIS.Desktop.Plugins.TransactionBillKiosk.Config;
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

namespace HIS.Desktop.Plugins.TransactionBillKiosk
{
    public partial class frmTransactionBillKiosk : HIS.Desktop.Utility.FormBase
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
                //WaitingManager.Show();
                labelCardRequire.Visible = true;
                CommonParam param = new CommonParam();
                bool? success = false;
                success = ProcessSave(ref param);
                labelCardRequire.Visible = false;
                //WaitingManager.Hide();
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
                labelCardRequire.Visible = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // kiểm tra hình thức thanh toán có phải là thanh toán qua thẻ không, nếu là thanh toán qua thẻ thì update màu sắc và validate textBox PIN
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (this.accountBook == null || this.accountBook.ID == 0)
                {
                     MessageBox.Show("Không chọn được số thu chi. Vui lòng liên hệ với nhân viên bệnh viện");
                     return;
                }
                this.positionHandleControl = -1;
                if (!btnSave.Enabled && !lciBtnSave.Enabled)
                    return;
                SetEnableButtonSave(false);
                if (!dxValidationProvider1.Validate() || this.treatmentId == null)
                {
                    SetEnableButtonSave(true);
                    return;
                }
                //WaitingManager.Show();
                labelCardRequire.Visible = true;
                //MessageBox.Show("XIN MỜI QUẸT THẺ ĐỂ THANH TOÁN");
                CommonParam param = new CommonParam();
                bool? success = false;
                success = ProcessSave(ref param);
               // WaitingManager.Hide();
                labelCardRequire.Visible = false;
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
                labelCardRequire.Visible = false;
               // WaitingManager.Hide();
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

        private bool? ProcessSave(ref CommonParam param, [Optional] bool isLuuKy)
        {
            
            this.isPrintNow = false;
            bool? success = false;
            try
            {
                gridViewSereServ.PostEditor();
                gridViewSereServ.UpdateCurrentRow();
                CARD.WCF.DCO.WcfSaleDCO saleDCO = null;

                var dataAll = (List<HIS_SERE_SERV>)gridControlSereServ.DataSource;
                if (dataAll == null || dataAll.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuChonDichVuDeThanhToan);
                    return success;
                }

                HisTransactionBillSDO data = new HisTransactionBillSDO();
                data.Transaction = new HIS_TRANSACTION();
                data.Transaction.TREATMENT_ID = this.treatmentId;
                data.Transaction.CASHIER_ROOM_ID = this.cashierRoom.ID;
                data.RequestRoomId = this.cashierRoom.ROOM_ID;
                if (accountBook != null)
                {
                    data.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                }
                else
                {
                    MessageBox.Show("Không chọn được số thu chi. Vui lòng liên hệ với nhân viên bệnh viện");
                    return success;

                }
                data.Transaction.AMOUNT = totalPatientPrice;
                data.PayAmount = totalPatientPrice;
                data.Transaction.PAY_FORM_ID = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE;
                data.Transaction.TRANSACTION_TIME = Inventec.Common.DateTime.Get.Now() ?? 0;
                List<HIS_SERE_SERV_BILL> hisSSBills = new List<HIS_SERE_SERV_BILL>();
                foreach (var item in listData)
                {
                    HIS_SERE_SERV_BILL ssBill = new HIS_SERE_SERV_BILL();
                    ssBill.SERE_SERV_ID = item.ID;
                    ssBill.PRICE = item.VIR_TOTAL_PATIENT_PRICE ?? 0;
                    hisSSBills.Add(ssBill);
                }
                data.SereServBills = hisSSBills;
                // thanh toán qua thẻ

                stopThread = true;
                //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_TRANSACTION_CHECK_BILL, ApiConsumers.MosConsumer, data, param);
                if (!check)
                {
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
                            labelCardRequire.Visible = false;
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
                            return success;
                        }
                        else
                        {
                            data.Transaction.TIG_TRANSACTION_CODE = saleDCO.TransactionCode;
                            data.Transaction.TIG_TRANSACTION_TIME = saleDCO.TransactionTime;
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show("Số tiền phải thu bệnh nhân <= 0. Bạn không thể chọn hình thức thanh toán qua thẻ. ");
                        return null;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("Dau vao khi goi api: HisTransaction/CreateBill: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisTransactionBillResultSDO>(UriStores.HIS_TRANSACTION_CREATE_BILL, ApiConsumers.MosConsumer, data, param);
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Debug("Ket qua tra ve khi goi api: HisTransaction/CreateBill: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("HisTransaction/CreateBill rs != null");
                    success = true;
                    AddLastAccountToLocal();
                    this.resultTranBill = rs.TransactionBill;
                    ddBtnPrint.Enabled = true;
                    btnSave.Enabled = false;
                    lciBtnSave.Enabled = false;
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
                WaitingManager.Hide();
                labelCardRequire.Visible = false;
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                WaitingManager.Hide();
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
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_BILL == 1 && o.ID != accountBook.ID).ToList();// && o.BILL_TYPE_ID == accountBook.BILL_TYPE_ID).ToList();
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
                store.RunPrintTemplate("Mps000376", DeletegatePrintTemplate);
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
                    case "Mps000376":
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
                if (this.resultTranBill == null)
                    return;
                WaitingManager.Show();
                stopThread = true;
                HisBillFundFilter billFundFilter = new HisBillFundFilter();
                billFundFilter.BILL_ID = this.resultTranBill.ID;
                var listBillFund = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_FUND>>("api/HisBillFund/Get", ApiConsumers.MosConsumer, billFundFilter, null);

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.resultTranBill.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultTranBill.ID);
                }

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                //HIS_PATY_ALTER_BHYT patyAlterBhyt = null;

                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                patyAlterAppliedFilter.TreatmentId = this.resultTranBill.TREATMENT_ID.Value;
                var patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                if (patientTypeAlter == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultTranBill.TREATMENT_CODE), this.resultTranBill.TREATMENT_CODE));
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.resultTranBill.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                HIS_PATIENT patient = new HIS_PATIENT();
                if (this.resultTranBill.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = this.resultTranBill.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.First();
                    }
                }
                stopThread = false;
                ResetLoopCount();

                MPS.Processor.Mps000376.PDO.Mps000376PDO pdo = new MPS.Processor.Mps000376.PDO.Mps000376PDO(
                    resultTranBill,
                    patient,
                    listBillFund,
                    listSereServ,
                    departmentTran,
                    patientTypeAlter,
                    HisConfigCFG.PatientTypeId__BHYT);

                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultTranBill != null ? resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

