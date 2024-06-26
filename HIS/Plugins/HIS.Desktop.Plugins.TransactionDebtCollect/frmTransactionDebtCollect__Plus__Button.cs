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
using HIS.Desktop.Plugins.TransactionDebtCollect.ADO;
using HIS.Desktop.Plugins.TransactionDebtCollect.Base;
using HIS.Desktop.Plugins.TransactionDebtCollect.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.DocumentViewer;
//using Inventec.Common.LocalStorage.SdaConfig;
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

namespace HIS.Desktop.Plugins.TransactionDebtCollect
{
    public partial class frmTransactionDebtCollect : HIS.Desktop.Utility.FormBase
    {
        private string Print106Type = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail");
        bool isPrintNow = false;

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!btnSavePrint.Enabled)
                    return;
                SetEnableButtonSave(false);
                if (!dxValidationProvider1.Validate())// || this.treatmentId == null)
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
                    this.onClickPhieuXacNhanCongNo(null, null);
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
                this.positionHandleControl = -1;
                if (!btnSave.Enabled && !lciBtnSave.Enabled)
                    return;
                SetEnableButtonSave(false);
                if (!dxValidationProvider1.Validate())// || this.treatmentId == null)
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
                    if (this.refeshReference != null)
                    {
                        this.refeshReference();
                    }
                    Inventec.Common.Logging.LogSystem.Debug("stopSave bill");
                    MessageManager.Show(this, param, success);
                }
                else if (success == false)
                {
                    SetEnableButtonSave(true);
                    Inventec.Common.Logging.LogSystem.Debug("can not stopSave bill");
                    MessageManager.Show(param, success);
                }
                GeneratePopupMenu();
                SessionManager.ProcessTokenLost(param);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool? ProcessSave(ref CommonParam param, [Optional] bool isLuuKy)
        {
            this.isPrintNow = false;
            bool? success = false;
            try
            {
                gridViewSereServDebt.PostEditor();
                gridViewSereServDebt.UpdateCurrentRow();

                var dataAll = (List<TransactionADO>)gridControlSereServDebt.DataSource;
                if (dataAll == null || dataAll.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuChonDichVuDeThanhToan);
                    return success;
                }

                listData = new List<TransactionADO>();
                listData = dataAll.FindAll(o => o.Check).ToList();

                if (listData == null || listData.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuChonDichVuDeThanhToan);
                    return success;
                }
                if (cboAccountBook.EditValue == null)
                {
                    param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    return success;
                }

                if (cboPayForm.EditValue == null)
                {
                    param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    return success;
                }

                long payFormId = 0;
                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                if (payForm == null)
                    return success;

                payFormId = payForm.ID;

                MOS.SDO.HisTransactionDebtCollecSDO data = new MOS.SDO.HisTransactionDebtCollecSDO();
                data.TreatmentId = this.treatmentId;
                data.RequestRoomId = this.currentModule.RoomId;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    data.AccountBookId = accountBook.ID;
                }
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    data.NumOrder = (long)spinTongTuDen.Value;
                }
                data.Exemption = Math.Round(totalDiscount, 4);
                data.PayAmount = totalPatientPrice - this.totalDiscount;
                data.PayFormId = payFormId;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    data.TransactionTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTransactionTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                data.Description = txtDescription.Text;

                if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                {

                    if (spinTransferAmount.Value > totalPatientPrice)
                    {
                        param.Messages.Add(String.Format("Số tiền chuyển khoản [{0}] lớn hơn số tiền cần thanh toán của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, 2), Inventec.Common.Number.Convert.NumberToStringRoundAuto((totalPatientPrice), 2)));
                        return false;
                    }
                    else
                    {
                        data.TransferAmount = spinTransferAmount.Value;
                    }
                }
                else if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.EditValue != null)
                {

                    if (spinTransferAmount.Value > totalPatientPrice)
                    {
                        param.Messages.Add(String.Format("Số tiền quẹt thẻ [{0}] lớn hơn số tiền cần thanh toán của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, 2), Inventec.Common.Number.Convert.NumberToStringRoundAuto((totalPatientPrice), 2)));
                        return false;
                    }
                    else
                    {
                        data.SwipeAmount = spinTransferAmount.Value;
                    }
                }

                data.DebtIds = listData.Select(o => o.ID).ToList();

                Inventec.Common.Logging.LogSystem.Debug("Dau vao khi goi api: HisTransaction/DebtCollect: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>(UriStores.HIS_TRANSACTION_CREATE_DEBT, ApiConsumers.MosConsumer, data, param);

                Inventec.Common.Logging.LogSystem.Debug("Ket qua tra ve khi goi api: HisTransaction/DebtCollect: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                if (rs != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("HisTransaction/DebtCollect rs != null");
                    success = true;
                    AddLastAccountToLocal();
                    this.resultTranBill = rs;
                    SetBillSuccessControl();
                    ddBtnPrint.Enabled = true;
                    btnSavePrint.Enabled = false;
                    btnSave.Enabled = false;
                    lciBtnSave.Enabled = false;
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
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

                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                string FindTreatmentCode = txtFindTreatmentCode.Text;
                txtFindTreatmentCode.Text = "";
                txtMaCongNo.Text = "";
                btnSearch_Click(null, null);
                this._ListTransactionId = new List<long>();
                txtFindTreatmentCode.Text = FindTreatmentCode;
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
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
                btnNew.Focus();
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
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBillSuccessControl()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    txtTransactionCode.Text = this.resultTranBill.TRANSACTION_CODE;
                    cboAccountBook.EditValue = this.resultTranBill.ACCOUNT_BOOK_ID;
                    txtTotalAmount.Value = this.resultTranBill.AMOUNT;
                }
                else
                {
                    throw new NullReferenceException("this.resultTranBill is null");
                }
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
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_DROP_DOWN__ITEM_PHIEU_THU_THANH_TOAN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuXacNhanCongNo)));
                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuXacNhanCongNo(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000370", DeletegatePrintTemplate);
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
                    case "Mps000370":
                        InPhieuXacNhanCongNo(printCode, fileName, ref result);
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

        private void InPhieuXacNhanCongNo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultTranBill == null)
                    return;
                WaitingManager.Show();
                if (listData == null || listData.Count == 0)
                {
                    throw new Exception("listData: null");
                }

                List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                HisSereServDebtViewFilter ssDebtFilter = new HisSereServDebtViewFilter();
                ssDebtFilter.DEBT_IDs = listData.Select(o => o.ID).Distinct().ToList();
                var hisSSDebts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/GetView", ApiConsumers.MosConsumer, ssDebtFilter, null);
                if (hisSSDebts != null || hisSSDebts.Count > 0)
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.IDs = hisSSDebts.Select(s => s.SERE_SERV_ID).ToList();
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                }

                List<HIS_DEBT_GOODS> listDebtGood = new List<HIS_DEBT_GOODS>();

                HisDebtGoodsFilter debtGoodFilter = new HisDebtGoodsFilter();
                debtGoodFilter.DEBT_IDs = listData.Select(o => o.ID).Distinct().ToList();
                listDebtGood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DEBT_GOODS>>("api/HisDebtGoods/Get", ApiConsumers.MosConsumer, debtGoodFilter, null);

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

                MPS.Processor.Mps000370.PDO.Mps000370PDO pdo = new MPS.Processor.Mps000370.PDO.Mps000370PDO(
                    resultTranBill,
                    patient,
                    listSereServ,
                    departmentTran,
                    patientTypeAlter,
                    hisSSDebts,
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                    listDebtGood
                    );

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
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
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
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
                    if (apiresult != null && apiresult.Count > 0)
                    {
                        result = apiresult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
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
                    HisTransactionFilter filter = new HisTransactionFilter();
                    filter.TREATMENT_ID = treatmentId;
                    filter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, null);
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
                btnSavePrint.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

}

