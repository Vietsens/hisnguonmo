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
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraBars;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using MPS.Processor.Mps000001.PDO;
using HIS.Desktop.Plugins.RegisterV2.Process;
using MOS.SDO;
using MPS.Processor.Mps000309.PDO;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {

        DevExpress.XtraBars.BarManager barManager = new DevExpress.XtraBars.BarManager();
        PopupMenu menu;
        private enum PrintType
        {
            InDvKham,
            InTheBenhNhan,
            InPhieuYeuCauKham,
            BangKiemTruocTiemChung,
            InBienLaiHoaDon
        }

        private void Print(bool isPrintExam = false)
        {
            try
            {
                if (isPrintExam)
                {
                    if (this.currentHisExamServiceReqResultSDO == null
                    || this.currentHisExamServiceReqResultSDO.ServiceReqs == null
                    || this.currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0
                    || this.actionType == GlobalVariables.ActionAdd)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                        return;
                    }

                    this.currentHisExamServiceReqResultSDO.ServiceReqs = this.currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => this.serviceReqPrintIds.Contains(o.ID)).ToList();

                    this.PrintProcess(this.currentHisExamServiceReqResultSDO);
                }
                else
                {
                    this.InitMenuPrint();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintProcess(MOS.SDO.HisServiceReqExamRegisterResultSDO data)
        {
            try
            {
                if (this.currentHisExamServiceReqResultSDO == null)
                    throw new ArgumentNullException("DelegateRunPrinter => currentHisExamServiceReqResultSDO is null");
                if (this.currentHisExamServiceReqResultSDO.ServiceReqs == null || this.currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0)
                    throw new ArgumentNullException("DelegateRunPrinter => ServiceReqs is null");

                this.currentHisExamServiceReqResultSDO.ServiceReqs = this.currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => serviceReqPrintIds.Contains(o.ID)).ToList();
                HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                HisServiceReqSDO.SereServs = this.currentHisExamServiceReqResultSDO.SereServs;
                HisServiceReqSDO.ServiceReqs = this.currentHisExamServiceReqResultSDO.ServiceReqs;

                List<HIS_SERE_SERV_DEPOSIT> ssDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                List<HIS_SERE_SERV_BILL> ssBill = new List<HIS_SERE_SERV_BILL>();

                if (this.currentHisExamServiceReqResultSDO.CollectedTransactions != null && this.currentHisExamServiceReqResultSDO.CollectedTransactions.Count > 0)
                {
                    if (this.currentHisExamServiceReqResultSDO.SereServBills != null && this.currentHisExamServiceReqResultSDO.SereServBills.Count > 0)
                    {
                        ssBill = this.currentHisExamServiceReqResultSDO.SereServBills.Where(o => this.currentHisExamServiceReqResultSDO.CollectedTransactions.Exists(s => s.ID == o.BILL_ID)).ToList();
                    }

                    if (this.currentHisExamServiceReqResultSDO.SereServDeposits != null && this.currentHisExamServiceReqResultSDO.SereServDeposits.Count > 0)
                    {
                        ssDeposit = this.currentHisExamServiceReqResultSDO.SereServDeposits.Where(o => this.currentHisExamServiceReqResultSDO.CollectedTransactions.Exists(s => s.ID == o.DEPOSIT_ID)).ToList();
                    }
                }

                HisServiceReqSDO.SereServBills = ssBill;
                HisServiceReqSDO.SereServDeposits = ssDeposit;
                HisServiceReqSDO.Transactions = this.currentHisExamServiceReqResultSDO.Transactions;
                HisServiceReqSDO.DepositedSereServs = this.currentHisExamServiceReqResultSDO.DepositedSereServs;

                HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = resultHisPatientProfileSDO.HisTreatment.ID;
                filter.INTRUCTION_TIME = null;
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (hisTreatments != null && hisTreatments.Count > 0)
                {
                    HisTreatment = hisTreatments.FirstOrDefault();
                }
                if (HisTreatment.TDL_PATIENT_TYPE_ID != null && string.IsNullOrEmpty(HisTreatment.PATIENT_TYPE_CODE))
                {

                    HisTreatment.PATIENT_TYPE_CODE = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == HisTreatment.TDL_PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                    HisTreatment.HEIN_CARD_FROM_TIME = HisTreatment.TDL_HEIN_CARD_FROM_TIME ?? 0;
                    HisTreatment.HEIN_CARD_NUMBER = HisTreatment.TDL_HEIN_CARD_NUMBER;
                    HisTreatment.HEIN_CARD_TO_TIME = HisTreatment.TDL_HEIN_CARD_TO_TIME ?? 0;
                    HisTreatment.HEIN_MEDI_ORG_CODE = HisTreatment.TDL_HEIN_MEDI_ORG_CODE;
                    HisTreatment.LEVEL_CODE = resultHisPatientProfileSDO.HisPatientTypeAlter.LEVEL_CODE;
                    HisTreatment.RIGHT_ROUTE_CODE = resultHisPatientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE;
                    HisTreatment.RIGHT_ROUTE_TYPE_CODE = resultHisPatientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                    HisTreatment.TREATMENT_TYPE_CODE = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == HisTreatment.TDL_TREATMENT_TYPE_ID).TREATMENT_TYPE_CODE;
                    HisTreatment.HEIN_CARD_ADDRESS = resultHisPatientProfileSDO.HisPatientTypeAlter.ADDRESS;
                }

                MPS.ProcessorBase.PrintConfig.PreviewType previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                if (!IsActionSavePrint)
                    previewType = MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;
                else
                {
                    if (chkPrintExam.Checked && !chkSignExam.Checked)
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                    }
                    else if (!chkPrintExam.Checked && chkSignExam.Checked)
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else if (chkPrintExam.Checked && chkSignExam.Checked)
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                    }
                }
                var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, null, currentModule != null ? currentModule.RoomId : 0, !string.IsNullOrEmpty(txtGateNumber.Text.Trim()) ? (txtGateNumber.Text.Contains(":") ? txtGateNumber.Text.Trim().Split(':')[0] : txtGateNumber.Text.Trim()) : null, previewType);
                PrintServiceReqProcessor.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER, isPrintNow);

                this.isPrintNow = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintBienLai()
        {
            try
            {
                isPrintNowBL = true;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000420", DelegateRunPrinterInGiaoDichThanhToanPrintNow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintTamThu()
        {
            try
            {
                var deposit = this.currentHisExamServiceReqResultSDO.Transactions.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).ToList();
                if (deposit == null || deposit.Count <= 0) return;

                isPrintNowBL = true;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000102", DelegateRunPrinterInGiaoDichTamThuPrintNow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPatientCard()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__THE_BENH_NHAN__MPS000178, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMenuPrint()
        {
            try
            {
                this.barManager.Form = this;
                if (this.menu == null)
                    this.menu = new PopupMenu(this.barManager);
                this.menu.ItemLinks.Clear();

                BarButtonItem itemInDvKham = new BarButtonItem(barManager, ResourceMessage.Title_InDichVuKham, 1);
                itemInDvKham.Tag = PrintType.InDvKham;
                itemInDvKham.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemInDvKham);

                BarButtonItem itemPrintDangKyKham = new BarButtonItem(barManager, ResourceMessage.Title_InPhieuYeuCauKham, 1);
                itemPrintDangKyKham.Tag = PrintType.InPhieuYeuCauKham;
                itemPrintDangKyKham.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemPrintDangKyKham);

                BarButtonItem itemInTheBenhNhan = new BarButtonItem(barManager, ResourceMessage.Title_InTheBenhNhan, 2);
                itemInTheBenhNhan.Tag = PrintType.InTheBenhNhan;
                itemInTheBenhNhan.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemInTheBenhNhan);

                BarButtonItem itemInBangKiemTruocTiemChung = new BarButtonItem(barManager, ResourceMessage.Title_InBangKiemTruocTiemChung, 1);
                itemInBangKiemTruocTiemChung.Tag = PrintType.BangKiemTruocTiemChung;
                itemInBangKiemTruocTiemChung.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemInBangKiemTruocTiemChung);

                if (currentHisExamServiceReqResultSDO!=null && this.currentHisExamServiceReqResultSDO.Transactions != null && this.currentHisExamServiceReqResultSDO.Transactions.Count() > 0)
                {
                    BarButtonItem itemBienLai = new BarButtonItem(barManager, ResourceMessage.Title_InBienLaiHoaDon, 1);
                    itemBienLai.Tag = PrintType.InBienLaiHoaDon;
                    itemBienLai.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                    this.menu.AddItem(itemBienLai);
                }

                this.menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool isPrintNowBL = false;

        private void onClick__Pluss(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintType type = (PrintType)(e.Item.Tag);

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    switch (type)
                    {
                        case PrintType.InDvKham:
                            if (currentHisExamServiceReqResultSDO == null
                                || currentHisExamServiceReqResultSDO.ServiceReqs == null
                                || currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0
                                || actionType == GlobalVariables.ActionAdd)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                                return;
                            }

                            currentHisExamServiceReqResultSDO.ServiceReqs = currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => serviceReqPrintIds.Contains(o.ID)).ToList();
                            isPrintNow = false;
                            PrintProcess(currentHisExamServiceReqResultSDO);
                            break;

                        case PrintType.InTheBenhNhan:
                            richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinterInTheBenhNhan);
                            break;
                        case PrintType.InPhieuYeuCauKham:
                            richEditorMain.RunPrintTemplate("Mps000309", DelegateRunPrinterInPhieuYeuCauKham);
                            break;
                        case PrintType.BangKiemTruocTiemChung:
                            //TODO
                            richEditorMain.RunPrintTemplate("Mps000358", DelegateRunPrinterInBangKiemTruocTiemChung);
                            break;
                        case PrintType.InBienLaiHoaDon:
                            //TODO
                            isPrintNowBL = false;
                            richEditorMain.RunPrintTemplate("Mps000420", DelegateRunPrinterInGiaoDichThanhToanPrintNow);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterInGiaoDichThanhToan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                //if (resultHisPatientProfileSDO == null
                //                   || resultHisPatientProfileSDO.HisPatient == null)
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuRong, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                //    return result;
                //}
                WaitingManager.Show();
                if (currentHisExamServiceReqResultSDO == null || this.currentHisExamServiceReqResultSDO.Transactions == null || this.currentHisExamServiceReqResultSDO.Transactions.Count() == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Transaction null Print Mps000111 false");
                    return result;
                }

                List<HIS_BILL_FUND> listBillFund = new List<HIS_BILL_FUND>();

                foreach (var Transaction in this.currentHisExamServiceReqResultSDO.Transactions)
                {
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.BILL_ID = Transaction.ID;
                    List<HIS_SERE_SERV_BILL> hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (hisSSBills == null || hisSSBills.Count <= 0)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong lay duoc SereServBill theo BillId: " + Transaction.ID);
                        return result;
                    }

                    List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                    foreach (var item in this.currentHisExamServiceReqResultSDO.SereServs)
                    {
                        HIS_SERE_SERV ss = new HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(ss, item);
                        listSereServ.Add(ss);
                    }

                    //HIS_PATY_ALTER_BHYT patyAlterBhyt = null;

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    if (this.resultHisPatientProfileSDO.HisPatientTypeAlter != null)
                    {
                        patientTypeAlter = this.GetPatientTypeAlterByPatient(this.resultHisPatientProfileSDO.HisPatientTypeAlter);
                    }

                    HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                    departLastFilter.TREATMENT_ID = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                    departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                    V_HIS_PATIENT Vpatient = null;
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient, Vpatient);
                    string treatmentCode = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.TREATMENT_CODE;
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(
                        Transaction,
                        Vpatient,
                        listBillFund,
                        listSereServ,
                        departmentTran,
                        patientTypeAlter,
                        HisConfigCFG.PatientTypeId__BHYT
                        );

                    WaitingManager.Hide();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                    }
                    PrintData.EmrInputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultHisPatientProfileSDO.HisTreatment.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<V_HIS_SERVICE_REQ> ServiceReqList = new List<V_HIS_SERVICE_REQ>();

        private bool DelegateRunPrinterInGiaoDichThanhToanPrintNow(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                if (currentHisExamServiceReqResultSDO == null || this.currentHisExamServiceReqResultSDO.ServiceReqs == null || this.currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("ServiceReqs null Print Mps000420 false");
                    return result;
                }

                if (currentHisExamServiceReqResultSDO.HisPatientProfile == null
                    || currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter == null
                    || currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter.PATIENT_TYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                {
                    Inventec.Common.Logging.LogSystem.Error("doi tuong thanh toan khac BHYT Print Mps000420 false");
                    return result;
                }

                foreach (var ServiceReq in ServiceReqList)
                {
                    if (currentHisExamServiceReqResultSDO.SereServs != null && currentHisExamServiceReqResultSDO.SereServs.Count > 0)
                    {
                        var sereServByServiceReqs = currentHisExamServiceReqResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == ServiceReq.ID).ToList();
                        if (sereServByServiceReqs != null && sereServByServiceReqs.Count > 0 && currentHisExamServiceReqResultSDO.SereServDeposits != null && currentHisExamServiceReqResultSDO.SereServDeposits.Count > 0)
                        {
                            List<HIS_SERE_SERV_DEPOSIT> hisSSDeposits = currentHisExamServiceReqResultSDO.SereServDeposits.Where(o => sereServByServiceReqs.Exists(e => e.ID == o.SERE_SERV_ID)).ToList();
                            if (hisSSDeposits != null && hisSSDeposits.Count > 0 && currentHisExamServiceReqResultSDO.Transactions != null && currentHisExamServiceReqResultSDO.Transactions.Count > 0)
                            {
                                var transactionList = currentHisExamServiceReqResultSDO.Transactions.Where(o => hisSSDeposits.Exists(e => e.DEPOSIT_ID == o.ID)).ToList();
                                if (transactionList != null && transactionList.Count > 0)
                                {
                                    foreach (var transaction in transactionList)
                                    {
                                        string treatmentCode = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.TREATMENT_CODE;
                                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                                        MPS.Processor.Mps000420.PDO.Mps000420PDO pdo = new MPS.Processor.Mps000420.PDO.Mps000420PDO(
                                            transaction,
                                            sereServByServiceReqs,
                                            ServiceReq
                                            );

                                        WaitingManager.Hide();
                                        string printerName = "";
                                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                                        {
                                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                                        }

                                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                                        if (isPrintNowBL || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                                        {
                                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                        }
                                        else
                                        {
                                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                        }
                                        PrintData.EmrInputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultHisPatientProfileSDO.HisTreatment.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                                        result = MPS.MpsPrinter.Run(PrintData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterInGiaoDichTamThuPrintNow(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                if (currentHisExamServiceReqResultSDO == null || this.currentHisExamServiceReqResultSDO.Transactions == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Transactions null Print Mps000102 false");
                    return result;
                }

                var deposit = this.currentHisExamServiceReqResultSDO.Transactions.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).ToList();
                if (deposit == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("deposit null Print Mps000102 false");
                    return result;
                }

                CommonParam param = new CommonParam();

                MOS.Filter.HisSereServView12Filter sereServFilter = new MOS.Filter.HisSereServView12Filter();
                sereServFilter.TREATMENT_ID = this.currentHisExamServiceReqResultSDO.Transactions.First().TREATMENT_ID.Value;
                sereServFilter.IDs = this.currentHisExamServiceReqResultSDO.SereServDeposits.Select(s => s.SERE_SERV_ID).ToList();
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_12>>("api/HisSereServ/GetView12", ApiConsumers.MosConsumer, sereServFilter, param);
                foreach (var item in sereServs)
                {
                    var itemCheck = this.currentHisExamServiceReqResultSDO.SereServDeposits.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                    if (itemCheck != null)
                    {
                        item.VIR_TOTAL_PATIENT_PRICE = itemCheck.AMOUNT;
                    }
                }
                isPrintNow = false;
                if (isPrintNowBL || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    isPrintNow = true;
                }

                DepositServicePrintProcess.LoadPhieuThuPhiDichVu(printTypeCode, fileName, true, sereServs, this.currentHisExamServiceReqResultSDO, isPrintNow, this.currentModule);

                result = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterInBangKiemTruocTiemChung(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (resultHisPatientProfileSDO == null
                                   || resultHisPatientProfileSDO.HisPatient == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuRong, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                    return result;
                }
                WaitingManager.Show();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                if (resultHisPatientProfileSDO.HisPatient != null)
                {
                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = resultHisPatientProfileSDO.HisPatient.ID;
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (rs != null && rs.Count > 0)
                    {
                        currentPatient = rs.FirstOrDefault();
                    }
                }

                MPS.Processor.Mps000358.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000358.PDO.SingleKeyValue();
                //{
                //    LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                //    Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()
                //};


                MPS.Processor.Mps000358.PDO.Mps000358PDO mps000358PDO = new MPS.Processor.Mps000358.PDO.Mps000358PDO(
                    currentPatient,
                    singleKeyValue
                    );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultHisPatientProfileSDO.HisTreatment.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000358PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000358PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "", 1, false, true) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterInPhieuYeuCauKham(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (resultHisPatientProfileSDO == null
                                   || resultHisPatientProfileSDO.HisPatient == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuRong, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                    return result;
                }
                WaitingManager.Show();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                if (resultHisPatientProfileSDO.HisPatient != null)
                {
                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = resultHisPatientProfileSDO.HisPatient.ID;
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (rs != null && rs.Count > 0)
                    {
                        currentPatient = rs.FirstOrDefault();
                    }
                }

                MPS.Processor.Mps000309.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000309.PDO.SingleKeyValue()
                {
                    LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()
                };

                HIS_DHST dhst = new HIS_DHST();

                CommonParam param = new CommonParam();
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = resultHisPatientProfileSDO.HisTreatment.ID;
                dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                dhstFilter.ORDER_DIRECTION = "DESC";
                dhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();


                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                if (this.resultHisPatientProfileSDO.HisPatientTypeAlter != null)
                {
                    patientTypeAlter = this.GetPatientTypeAlterByPatient(this.resultHisPatientProfileSDO.HisPatientTypeAlter);
                }
                //Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>();
                //patientTypeAlter = Mapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(resultHisPatientProfileSDO.HisPatientTypeAlter);

                MPS.Processor.Mps000309.PDO.Mps000309PDO mps000309PDO = new MPS.Processor.Mps000309.PDO.Mps000309PDO(
                    currentPatient,
                    patientTypeAlter,
                    dhst,
                    resultHisPatientProfileSDO.HisTreatment,
                    singleKeyValue
                    );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultHisPatientProfileSDO.HisTreatment.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000309PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000309PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "", 1, false, true) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterInTheBenhNhan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (resultHisPatientProfileSDO == null
                                || resultHisPatientProfileSDO.HisPatient == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuRong, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                    return result;
                }
                WaitingManager.Show();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                if (resultHisPatientProfileSDO.HisPatient != null)
                {
                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = resultHisPatientProfileSDO.HisPatient.ID;
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (rs != null && rs.Count > 0)
                    {
                        currentPatient = rs.FirstOrDefault();
                    }
                }

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlterByPatient = new V_HIS_PATIENT_TYPE_ALTER();
                if (this.resultHisPatientProfileSDO.HisPatientTypeAlter != null)
                {
                    patientTypeAlterByPatient = this.GetPatientTypeAlterByPatient(this.resultHisPatientProfileSDO.HisPatientTypeAlter);
                }

                V_HIS_TREATMENT_4 treatment4 = new V_HIS_TREATMENT_4();
                V_HIS_DEPARTMENT_TRAN departmentTran = new V_HIS_DEPARTMENT_TRAN();
                if (this.resultHisPatientProfileSDO.HisTreatment != null)
                {
                    MOS.Filter.HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                    filter.ID = this.resultHisPatientProfileSDO.HisTreatment.ID;
                    var treatments = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (treatments != null && treatments.Count > 0)
                    {
                        treatment4 = treatments.First();
                    }

                    MOS.Filter.HisDepartmentTranViewFilter defilter = new HisDepartmentTranViewFilter();
                    defilter.ID = this.resultHisPatientProfileSDO.HisTreatment.ID;
                    var departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumer.ApiConsumers.MosConsumer, defilter, null);
                    if (departmentTrans != null && departmentTrans.Count > 0)
                    {
                        departmentTrans = departmentTrans.OrderByDescending(o => o.DEPARTMENT_IN_TIME ?? Int64.MaxValue).ThenByDescending(o => o.ID).ToList();
                        departmentTran = departmentTrans.First();
                    }
                }

                MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                    currentPatient,
                    patientTypeAlterByPatient,
                    treatment4,
                    departmentTran
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment4.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "", 1, false, true) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterInTheBenhNhanPrintNow(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (resultHisPatientProfileSDO == null
                                || resultHisPatientProfileSDO.HisPatient == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuRong, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                    return result;
                }
                WaitingManager.Show();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                if (resultHisPatientProfileSDO.HisPatient != null)
                {
                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = resultHisPatientProfileSDO.HisPatient.ID;
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (rs != null && rs.Count > 0)
                    {
                        currentPatient = rs.FirstOrDefault();
                    }
                }

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlterByPatient = new V_HIS_PATIENT_TYPE_ALTER();
                if (this.resultHisPatientProfileSDO.HisPatientTypeAlter != null)
                {
                    patientTypeAlterByPatient = this.GetPatientTypeAlterByPatient(this.resultHisPatientProfileSDO.HisPatientTypeAlter);
                }

                V_HIS_TREATMENT_4 treatment4 = new V_HIS_TREATMENT_4();
                V_HIS_DEPARTMENT_TRAN departmentTran = new V_HIS_DEPARTMENT_TRAN();
                if (this.resultHisPatientProfileSDO.HisTreatment != null)
                {
                    MOS.Filter.HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                    filter.ID = this.resultHisPatientProfileSDO.HisTreatment.ID;
                    var treatments = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (treatments != null && treatments.Count > 0)
                    {
                        treatment4 = treatments.First();
                    }

                    MOS.Filter.HisDepartmentTranViewFilter defilter = new HisDepartmentTranViewFilter();
                    defilter.TREATMENT_ID = this.resultHisPatientProfileSDO.HisTreatment.ID;
                    var departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumer.ApiConsumers.MosConsumer, defilter, null);
                    if (departmentTrans != null && departmentTrans.Count > 0)
                    {
                        departmentTrans = departmentTrans.OrderByDescending(o => o.DEPARTMENT_IN_TIME ?? Int64.MaxValue).ThenByDescending(o => o.ID).ToList();
                        departmentTran = departmentTrans.First();
                    }
                }

                MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                    currentPatient,
                    patientTypeAlterByPatient,
                    treatment4,
                    departmentTran
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__THE_BENH_NHAN__MPS000178:
                        result = DelegateRunPrinterInTheBenhNhanPrintNow(printTypeCode, fileName);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private V_HIS_PATIENT_TYPE_ALTER GetPatientTypeAlterByPatient(HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            V_HIS_PATIENT_TYPE_ALTER result = new V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT_TYPE_ALTER>(result, patientTypeAlter);
                var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeAlter.PATIENT_TYPE_ID);
                if (patientType != null)
                {
                    result.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                    result.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }
                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeAlter.TREATMENT_TYPE_ID);
                if (treatmentType != null)
                {
                    result.TREATMENT_TYPE_CODE = treatmentType.TREATMENT_TYPE_CODE;
                    result.TREATMENT_TYPE_NAME = treatmentType.TREATMENT_TYPE_NAME;
                }

                if (result == null) Inventec.Common.Logging.LogSystem.Debug("GetPatientTypeAlterByPatient => null");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private V_HIS_ROOM GetRoomById(long id)
        {
            V_HIS_ROOM result = null;
            try
            {
                result = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new V_HIS_ROOM());
        }

        private HIS_TRAN_PATI_REASON GetTranPatiReasonById(long id)
        {
            HIS_TRAN_PATI_REASON result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new HIS_TRAN_PATI_REASON());
        }
    }
}
