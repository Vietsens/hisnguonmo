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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RepayService.RepayService
{
    public partial class frmRepayService : HIS.Desktop.Utility.FormBase
    {
        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menuMedicine = new DXPopupMenu();
                //TODO da ngon ngu
                DXMenuItem itemPhieuHoanUng = new DXMenuItem("In phiếu hoàn ứng", new EventHandler(OnClickInPhieuHoanUng));
                itemPhieuHoanUng.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110;
                menuMedicine.Items.Add(itemPhieuHoanUng);
                ddbPrint.DropDownControl = menuMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuHoanUng(object sender, EventArgs e)
        {
            try
            {
                this.isPrintNow = false;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                var btnItem = sender as DXMenuItem;
                string type = (string)(btnItem.Tag);
                switch (type)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110:
                        InPhieuHoanUng(printTypeCode, fileName, ref result, this.isPrintNow);
                        break;
                    case MPS.Processor.Mps000430.PDO.Mps000430PDO.printTypeCode:
                        DelegatePrint430(printTypeCode, fileName, ref result, this.isPrintNow);
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

        public PatientADO getPatient(long treatmentId)
        {
            Inventec.Common.Logging.LogSystem.Info("Begin get PatientADO");
            PatientADO currentPatientADO = new PatientADO();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = new V_HIS_PATIENT();
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentHisTreatment = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = treatmentId;

                CommonParam param = new CommonParam();
                var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentHisTreatment = treatments.FirstOrDefault();

                    MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                    patientViewFilter.ID = currentHisTreatment.PATIENT_ID;
                    patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientViewFilter, param).SingleOrDefault();

                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_PATIENT, PatientADO>();
                    currentPatientADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_PATIENT, PatientADO>(patient); currentPatientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.DOB);
                    currentPatientADO.AGE = AgeHelper.CalculateAgeFromYear(currentPatientADO.DOB);
                    if (currentPatientADO != null && currentPatientADO.DOB > 0)
                    {
                        currentPatientADO.DOB_YEAR = currentPatientADO.DOB.ToString().Substring(0, 4);
                    }
                    if (currentPatientADO.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        currentPatientADO.GENDER_MALE = "";
                        currentPatientADO.GENDER_FEMALE = "X";
                    }
                    else
                    {
                        currentPatientADO.GENDER_MALE = "X";
                        currentPatientADO.GENDER_FEMALE = "";
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("End get PatientADO");
            }
            catch (Exception ex)
            {
                currentPatientADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentPatientADO;
        }

        private void InPhieuHoanUng(string printTypeCode, string fileName, ref bool result, bool isPrintNow)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                if (this.hisTransaction == null)
                {
                    MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong));
                    return;
                }

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;

                var patientADO = getPatient(this.HisTreatment.ID);

                MOS.Filter.HisSeseDepoRepayFilter hisSeseDepoRepayFilter = new MOS.Filter.HisSeseDepoRepayFilter();
                hisSeseDepoRepayFilter.TDL_TREATMENT_ID = this.treatment.ID;
                hisSeseDepoRepayFilter.REPAY_ID = this.hisTransaction.ID;
                var dereDetails = new BackendAdapter(param).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, hisSeseDepoRepayFilter, param).ToList();

                List<HIS_SERE_SERV_DEPOSIT> lstSsDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                List<V_HIS_TRANSACTION> lstTran = new List<V_HIS_TRANSACTION>();
                if (dereDetails != null && dereDetails.Count > 0)
                {
                    ProcessGetDepositBySsdIds(dereDetails.Select(s => s.SERE_SERV_DEPOSIT_ID).Distinct().ToList(), ref lstSsDeposit, lstTran);
                }

                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT, ApiConsumers.MosConsumer, HisTreatment.ID, param);

                long? totalDay = null;
                if (this.HisTreatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(this.HisTreatment.IN_TIME, this.HisTreatment.OUT_TIME, this.HisTreatment.TREATMENT_END_TYPE_ID, this.HisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(this.HisTreatment.IN_TIME, this.HisTreatment.OUT_TIME, this.HisTreatment.TREATMENT_END_TYPE_ID, this.HisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                }
                string departmentName = "";

                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterViewFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterViewFilter.TREATMENT_ID = HisTreatment.ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterViewFilter, param);

                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }

                MPS.Processor.Mps000110.PDO.PatientADO mpsPatientAdo = new MPS.Processor.Mps000110.PDO.PatientADO();
                if (patientADO != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000110.PDO.PatientADO>(mpsPatientAdo, patientADO);
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.hisTransaction.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                MPS.Processor.Mps000110.PDO.Mps000110PDO pdo = new MPS.Processor.Mps000110.PDO.Mps000110PDO(
                   mpsPatientAdo,
                   patientTypeAlter,
                   departmentName,
                   dereDetails,
                   departmentTrans,
                   this.HisTreatment,
                   this.hisTransaction,
                   totalDay,
                   departmentTran,
                   lstSsDeposit,
                   lstTran
                   );

                MPS.ProcessorBase.Core.PrintData printData = null;
                WaitingManager.Hide();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.HisTreatment != null ? this.HisTreatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? moduleData.RoomId : 0);
                if (isPrintNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    MPS000430();
                    if (result && chkAutoClose.CheckState == CheckState.Checked)
                        this.Close();
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    MPS000430();
                    if (result && chkAutoClose.CheckState == CheckState.Checked)
                        this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessGetDepositBySsdIds(List<long> listIds, ref List<HIS_SERE_SERV_DEPOSIT> lstSsDeposit, List<V_HIS_TRANSACTION> lstTran)
        {
            try
            {
                if (listIds != null && listIds.Count > 0)
                {
                    if (lstSsDeposit == null)
                        lstSsDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                    if (lstTran == null)
                        lstTran = new List<V_HIS_TRANSACTION>();

                    int skip = 0;
                    while (listIds.Count - skip > 0)
                    {
                        var ids = listIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        CommonParam param = new CommonParam();
                        HisSereServDepositFilter deFilter = new HisSereServDepositFilter();
                        deFilter.IDs = ids;
                        var ssdApiresult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, deFilter, param);
                        if (ssdApiresult != null && ssdApiresult.Count > 0)
                        {
                            lstSsDeposit.AddRange(ssdApiresult);
                        }
                    }

                    List<long> transDeposit = lstSsDeposit.Select(s => s.DEPOSIT_ID).Distinct().ToList();

                    skip = 0;
                    while (transDeposit.Count - skip > 0)
                    {
                        var ids = transDeposit.Skip(skip).Take(100).ToList();
                        skip += 100;
                        CommonParam param = new CommonParam();
                        HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                        tranFilter.IDs = ids;
                        var tranApiresult = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                        if (tranApiresult != null && tranApiresult.Count > 0)
                        {
                            lstTran.AddRange(tranApiresult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegatePrint430(string printTypeCode, string fileName, ref bool result, bool isPrintNow)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_1, V_HIS_TREATMENT>();
                var Treatment = AutoMapper.Mapper.Map<V_HIS_TREATMENT_1, V_HIS_TREATMENT>(this.HisTreatment);

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;

                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterViewFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterViewFilter.TREATMENT_ID = HisTreatment.ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterViewFilter, param);

                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }

                MOS.Filter.HisSeseDepoRepayFilter hisSeseDepoRepayFilter = new MOS.Filter.HisSeseDepoRepayFilter();
                hisSeseDepoRepayFilter.TDL_TREATMENT_ID = Treatment.ID;
                hisSeseDepoRepayFilter.IS_CANCEL = false;
                var dereDetails = new BackendAdapter(param).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, hisSeseDepoRepayFilter, param).ToList();

                MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                sereServDepositFilter.TDL_TREATMENT_ID = Treatment.ID;
                sereServDepositFilter.IS_CANCEL = false;
                var sereServDeposits = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);

                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.TREATMENT_ID = Treatment.ID;
                tranFilter.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> listTransaction = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, tranFilter, param);

                var repayDepo = sereServDeposits.Where(o => dereDetails.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID)).ToList();

                List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                List<long> sereServIds = sereServDeposits.Select(s => s.SERE_SERV_ID).Distinct().ToList();
                int skip = 0;
                while (sereServIds.Count - skip > 0)
                {
                    var listIds = sereServIds.Skip(skip).Take(100).ToList();
                    skip += 100;
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.IDs = listIds;
                    var ssResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);
                    if (ssResult != null && ssResult.Count > 0)
                    {
                        listSereServ.AddRange(ssResult);
                    }
                }

                var groupDeposit = repayDepo.GroupBy(o => o.DEPOSIT_ID).ToList();
                foreach (var item in groupDeposit)
                {
                    //lấy các giao dịch hoàn ứng tạo trước giao dịch hiện tại
                    List<V_HIS_TRANSACTION> listTranRepay = listTransaction.Where(o => o.TRANSACTION_TYPE_ID == this.hisTransaction.TRANSACTION_TYPE_ID && o.TRANSACTION_TIME <= this.hisTransaction.TRANSACTION_TIME).ToList();
                    List<HIS_SESE_DEPO_REPAY> ssRepay = dereDetails.Where(o => listTranRepay.Exists(e => e.ID == o.REPAY_ID)).ToList();

                    var ssDepositCheck = sereServDeposits.Where(o => !ssRepay.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID) && o.DEPOSIT_ID == item.Key).ToList();
                    var ssDeposit = sereServDeposits.Where(o => o.DEPOSIT_ID == item.Key).ToList();
                    var tranDeposit = listTransaction.FirstOrDefault(o => o.ID == item.Key) ?? new V_HIS_TRANSACTION();

                    if (ssDepositCheck == null || ssDepositCheck.Count <= 0)
                    {
                        continue;
                    }

                    MPS.Processor.Mps000430.PDO.Mps000430PDO pdo430 = new MPS.Processor.Mps000430.PDO.Mps000430PDO(
                        Treatment,
                        patientTypeAlter,
                        ssDeposit,
                        ssRepay,
                        null,
                        new List<V_HIS_TRANSACTION> { this.hisTransaction, tranDeposit },
                        listSereServ,
                        BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_ROOM>()
                        );

                    WaitingManager.Hide();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? moduleData.RoomId : 0);

                    if (isPrintNow)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo430, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo430, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MPS000430()
        {
            var printType = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == MPS.Processor.Mps000430.PDO.Mps000430PDO.printTypeCode);
            if (printType != null && printType.IS_ACTIVE == 1)
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(MPS.Processor.Mps000430.PDO.Mps000430PDO.printTypeCode, DelegateRunPrinter);
            }
        }
    }
}
