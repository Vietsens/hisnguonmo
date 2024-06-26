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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.VaccinationExam.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.VaccinationExam
{
    public partial class UCVaccinationExam : UserControlBase
    {
        private bool PrintNow;
        private const string Mps442 = "Mps000442";
        private const string Mps443 = "Mps000443";
        private const string Mps474 = "Mps000474";
        private const string Mps475 = "Mps000475";

        private void ProcessPrintAssign(bool printNow)
        {
            try
            {
                this.PrintNow = printNow;
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(Mps442, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintAppointment(bool printNow)
        {
            try
            {
                this.PrintNow = printNow;
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(Mps443, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintPhieuKham_MPS474(bool printNow)
        {
            try
            {
                this.PrintNow = printNow;
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(Mps474, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintPhieuKham_MPS475(bool printNow)
        {
            try
            {
                this.PrintNow = printNow;
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(Mps475, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case Mps442:
                        ProcessPrintMps442(printCode, fileName, ref result);
                        break;
                    case Mps443:
                        ProcessPrintMps443(printCode, fileName, ref result);
                        break;
                    case Mps474:
                        ProcessPrintMps474(printCode, fileName, ref result);
                        break;
                    case Mps475:
                        ProcessPrintMps475(printCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessPrintMps443(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.VaccAppointmentResult != null && this.VaccAppointmentResult.Count > 0)
                {
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    MPS.Processor.Mps000443.PDO.Mps000443PDO pdo = new MPS.Processor.Mps000443.PDO.Mps000443PDO(this.vaccinationExam, this.VaccAppointmentResult);

                    if (this.PrintNow
                        || ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW) == "1"
                        || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintMps442(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.VaccinationResult != null && this.VaccinationResult.Vaccinations != null && this.VaccinationResult.Vaccinations.Count > 0)
                {
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    foreach (var vaccination in this.VaccinationResult.Vaccinations)
                    {
                        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicine = this.VaccinationResult.Medicines != null ? this.VaccinationResult.Medicines.Where(o => o.TDL_VACCINATION_ID == vaccination.ID).ToList() : null;
                        MPS.Processor.Mps000442.PDO.Mps000442PDO pdo = new MPS.Processor.Mps000442.PDO.Mps000442PDO(vaccination, expMestMedicine);

                        if (this.PrintNow
                            || ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW) == "1"
                            || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintMps474(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps474()");
                if (this.vaccinationExam != null)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    if (this.vaccExamResults == null || this.vaccExamResults.Count == 0)
                    {
                        LoadDataVaccExamResults();
                    }
                    else
                    {
                        foreach (var item in this.vaccExamResults)
                        {
                            item.HIS_VAEX_VAER = null;
                        }
                    }
                    List<HIS_VACC_EXAM_RESULT> listVaccExamResult = this.vaccExamResults.Where(o=>o.IS_BABY == 1).ToList() ?? new List<HIS_VACC_EXAM_RESULT>();
                    List<HIS_VAEX_VAER> listVaexVaer = GetListVaexVaer_ByVaccinationExamID(this.vaccinationExam.ID);
                    if (listVaexVaer != null && listVaexVaer.Count > 0)
                    {
                        foreach (var item in listVaexVaer)
                        {
                            var index = listVaccExamResult.FindIndex(o => o.ID == item.VACC_EXAM_RESULT_ID);
                            var data = listVaccExamResult.Find(o => o.ID == item.VACC_EXAM_RESULT_ID);
                            List<HIS_VAEX_VAER> addItem = new List<HIS_VAEX_VAER>();
                            addItem.Add(item);
                            if (data != null)
                                data.HIS_VAEX_VAER = addItem;
                            if (index != -1 && index > 0)
                                listVaccExamResult[index] = data;
                        }
                    }

                    List<HIS_VACCINATION> listVaccination = GetListVaccination_ByVaccinationExamID(this.vaccinationExam.ID);

                    List<long> listVaccinationId = listVaccination != null ? listVaccination.Select(o => o.ID).ToList() : null;
                    List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = GetListExpMestMedicine_ByVaccinationIDs(listVaccinationId);
                    CommonParam param = new CommonParam();
                    HisDhstFilter filter = new HisDhstFilter();
                    filter.VACCINATION_EXAM_ID = vaccinationExam.ID;
                    var dtDhst =new BackendAdapter(param)
                    .Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listVaccExamResult", listVaccExamResult));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("vaccinationExam", vaccinationExam));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listExpMestMedicine", listExpMestMedicine));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Dhst", dtDhst));
                    WaitingManager.Hide();
                    MPS.Processor.Mps000474.PDO.Mps000474PDO pdo = new MPS.Processor.Mps000474.PDO.Mps000474PDO(listVaccExamResult, this.vaccinationExam, listExpMestMedicine, (dtDhst != null && dtDhst.Count > 0) ? dtDhst.First() : null);

                    if (this.PrintNow
                        || ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW) == "1"
                        || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintMps475(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps475()");
                if (this.vaccinationExam != null)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    if (this.vaccExamResults == null || this.vaccExamResults.Count == 0)
                    {
                        LoadDataVaccExamResults();
                    }
                    else
                    {
                        foreach (var item in this.vaccExamResults)
                        {
                            item.HIS_VAEX_VAER = null;
                        }
                    }
                    List<HIS_VACC_EXAM_RESULT> listVaccExamResult = this.vaccExamResults.Where(o => o.IS_BABY == null).ToList() ?? new List<HIS_VACC_EXAM_RESULT>();
                    List<HIS_VAEX_VAER> listVaexVaer = GetListVaexVaer_ByVaccinationExamID(this.vaccinationExam.ID);
                    if (listVaexVaer != null && listVaexVaer.Count > 0)
                    {
                        foreach (var item in listVaexVaer)
                        {
                            var index = listVaccExamResult.FindIndex(o => o.ID == item.VACC_EXAM_RESULT_ID);
                            var data = listVaccExamResult.Find(o => o.ID == item.VACC_EXAM_RESULT_ID);
                            List<HIS_VAEX_VAER> addItem = new List<HIS_VAEX_VAER>();
                            addItem.Add(item);
                            if (data != null)
                                data.HIS_VAEX_VAER = addItem;
                            if (index != -1 && index > 0)
                                listVaccExamResult[index] = data;
                        }
                    }

                    List<HIS_VACCINATION> listVaccination = GetListVaccination_ByVaccinationExamID(this.vaccinationExam.ID);

                    List<long> listVaccinationId = listVaccination != null ? listVaccination.Select(o => o.ID).ToList() : null;
                    List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = GetListExpMestMedicine_ByVaccinationIDs(listVaccinationId);

                    CommonParam param = new CommonParam();
                    HisDhstFilter filter = new HisDhstFilter();
                    filter.VACCINATION_EXAM_ID = vaccinationExam.ID;
                    var dtDhst = new BackendAdapter(param)
                    .Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listVaccExamResult", listVaccExamResult));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("vaccinationExam", vaccinationExam));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listExpMestMedicine", listExpMestMedicine));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Dhst", dtDhst));
                    WaitingManager.Hide();

                    MPS.Processor.Mps000475.PDO.Mps000475PDO pdo = new MPS.Processor.Mps000475.PDO.Mps000475PDO(listVaccExamResult, this.vaccinationExam, listExpMestMedicine, (dtDhst !=null && dtDhst.Count > 0) ? dtDhst.First() : null );

                    if (this.PrintNow
                        || ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW) == "1"
                        || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_VAEX_VAER> GetListVaexVaer_ByVaccinationExamID(long id)
        {
            List<HIS_VAEX_VAER> result = null;
            try
            {
                if (id <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisVaexVaerFilter filter = new HisVaexVaerFilter();
                filter.VACCINATION_EXAM_ID = id;
                result = new BackendAdapter(param)
                    .Get<List<HIS_VAEX_VAER>>("api/HisVaexVaer/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_VACCINATION> GetListVaccination_ByVaccinationExamID(long id)
        {
            List<HIS_VACCINATION> result = null;
            try
            {
                if (id <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisVaccinationFilter filter = new HisVaccinationFilter();
                filter.VACCINATION_EXAM_ID = id;
                result = new BackendAdapter(param)
                    .Get<List<HIS_VACCINATION>>("api/HisVaccination/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_EXP_MEST_MEDICINE> GetListExpMestMedicine_ByVaccinationIDs(List<long> listId)
        {
            List<V_HIS_EXP_MEST_MEDICINE> result = null;
            try
            {
                if (listId == null || listId.Count == 0)
                    return null;
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_VACCINATION_IDs = listId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, filter, param);
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
