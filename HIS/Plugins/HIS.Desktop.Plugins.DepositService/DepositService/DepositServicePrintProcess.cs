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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
//using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    public static class DepositServicePrintProcess
    {
        static Inventec.Desktop.Common.Modules.Module ModuleData;
        static int iPatientTypeIsNotBHYT = -1;
        //public static List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12> HisSereServ_Bordereaus;

        internal static List<V_HIS_SERE_SERV_12> ProcessGetSereServ(long patientTypeId, bool? isExpend, long treatmentId, List<long> sereServIds)
        {
            List<V_HIS_SERE_SERV_12> SereServAlls = new List<V_HIS_SERE_SERV_12>();
            CommonParam param = new CommonParam();
            try
            {
                var sereServ_Bordereaus = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12>();
                List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12> HisSereServ_Bordereaus = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12>();
                //Lay tat ca du lieu can xu ly
                MOS.Filter.HisSereServViewFilter hisSereServFilter = new HisSereServViewFilter();
                hisSereServFilter.TREATMENT_ID = treatmentId;
                hisSereServFilter.IDs = sereServIds;
                if (patientTypeId != iPatientTypeIsNotBHYT && patientTypeId != 0)
                {
                    hisSereServFilter.PATIENT_TYPE_ID = patientTypeId;
                }

                sereServ_Bordereaus = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_12>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, hisSereServFilter, param);

                if (sereServ_Bordereaus != null && sereServ_Bordereaus.Count > 0)
                {
                    //Kiem tra va xu ly du lieu ton tai
                    ProcessExistsData(sereServ_Bordereaus, patientTypeId, ref HisSereServ_Bordereaus);
                    var query = HisSereServ_Bordereaus.AsQueryable();
                    if (patientTypeId == 0)//Loc du lieu theo doi tuong
                    {
                        query = query.Where(o => o.AMOUNT > 0);
                    }
                    else
                    {

                        var patientTypeCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                        var patientTypeBhyt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == patientTypeCFG);
                        query = query.Where(o => o.AMOUNT > 0
                             && (patientTypeId != iPatientTypeIsNotBHYT || o.PATIENT_TYPE_ID != patientTypeBhyt.ID));
                    }

                    //Check hao phí
                    if (isExpend == false)
                    {
                        query = query.Where(o => o.IS_EXPEND == null);
                    }

                    HisSereServ_Bordereaus = query.ToList();

                    SereServAlls.AddRange(HisSereServ_Bordereaus);
                }
                return SereServAlls;
            }
            catch (Exception)
            {
                return new List<V_HIS_SERE_SERV_12>();
                throw;
            }
        }

        public static void LoadPhieuThuPhiDichVu(string printTypeCode, string fileName, bool isExpand, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12> SereServAlls, MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE currentHisTreatment, List<long> sereServIds, MOS.EFMODEL.DataModels.V_HIS_TRANSACTION deposit, List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT> dereDetails, V_HIS_SERVICE_REQ hisServiceReq, bool isPrintNow, bool isSign, Inventec.Desktop.Common.Modules.Module moduleData)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                WaitingManager.Show();
                ModuleData = moduleData;
                //Thông tin bệnh nhân
                MPS.Processor.Mps000102.PDO.PatientADO patient = new MPS.Processor.Mps000102.PDO.PatientADO();
                MOS.Filter.HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                patientViewFilter.ID = currentHisTreatment.PATIENT_ID;
                var patients = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = new MPS.Processor.Mps000102.PDO.PatientADO(patients.FirstOrDefault());
                }
                //Thông tin bảo hiểm y tế
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                patientTypeFilter.TREATMENT_ID = currentHisTreatment.ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, param);
                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT, ApiConsumers.MosConsumer, currentHisTreatment.ID, param);

                long? totalDay = null;

                if (currentHisTreatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(currentHisTreatment.IN_TIME, currentHisTreatment.OUT_TIME, currentHisTreatment.TREATMENT_END_TYPE_ID, currentHisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(currentHisTreatment.IN_TIME, currentHisTreatment.OUT_TIME, currentHisTreatment.TREATMENT_END_TYPE_ID, currentHisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                }

                string departmentName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentName();
                var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                var sereServHitechs = SereServAlls.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);

                var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                var sereServVTTTs = SereServAlls.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);

                var sereServNotHitechs = SereServAlls.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();
                //Cộng các sereServ trong gói vào dv ktc
                foreach (var sereServHitech in sereServHitechADOs)
                {
                    List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    var sereServVTTTInKtcs = SereServAlls.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                    sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                    sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                    sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                    decimal totalHeinPrice = 0;
                    foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                    {
                        totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                    }
                    sereServHitech.PRICE_BHYT += totalHeinPrice;
                    //sereServHitech.PRICE_BHYT = sereServHitech.PRICE_BHYT * sereServHitech.AMOUNT + sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                    sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServHitech.VIR_TOTAL_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);

                    //TODO
                    //if (sereServVTTTInKtcs.Count > 0)
                    //{
                    //    sereServNotHitechs = sereServNotHitechs.Except(sereServVTTTInKtcs).ToList();
                    //}
                }

                //Lọc các sereServ nằm không nằm trong dịch vụ ktc và vật tư thay thế
                //
                var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                foreach (var sereServVTTTADO in sereServVTTTADOs)
                {
                    var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                    if (sereServADODelete.Count == 0)
                    {
                        sereServDeleteADOs.Add(sereServVTTTADO);
                    }
                }

                foreach (var sereServDelete in sereServDeleteADOs)
                {
                    sereServVTTTADOs.Remove(sereServDelete);
                }
                var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);

                MOS.Filter.HisHeinServiceTypeFilter HeinServiceTypefilter = new MOS.Filter.HisHeinServiceTypeFilter();
                HeinServiceTypefilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var serviceReports = new BackendAdapter(param).Get<List<HIS_HEIN_SERVICE_TYPE>>(HisRequestUriStore.HIS_HEIN_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, HeinServiceTypefilter, param);
                WaitingManager.Hide();

                // tinh muc huong
                V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(currentHisTreatment.ID, 0, ref currentHisPatientTypeAlter);
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentHisPatientTypeAlter.HEIN_CARD_NUMBER, currentHisPatientTypeAlter.LEVEL_CODE, currentHisPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentHisTreatment != null ? currentHisTreatment.TREATMENT_CODE : ""), printTypeCode, moduleData.RoomId);

                MPS.Processor.Mps000102.PDO.Mps000102PDO pdo = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                        patient,
                        patientTypeAlter,
                        departmentName,

                        sereServNotHitechADOs,
                        sereServHitechADOs,
                        sereServVTTTADOs,

                        departmentTrans,
                        currentHisTreatment,

                        serviceReports,

                        deposit,
                        dereDetails,  

                        totalDay,
                        ratio_text,
                        hisServiceReq
                        );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (isPrintNow)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "") { EmrInputADO = inputADO };
                }
                else if (isSign)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "", 1, false, true) { EmrInputADO = inputADO };
                }
                
                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                result = MPS.MpsPrinter.Run(PrintData);
                //if (isPrintNow)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "", 1, false, true);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
        }

        private static void ProcessExistsData(List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12> sereServ_Bordereaus, long patientTypeId, ref List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12> sereServ_BordereausResult)
        {
            try
            {
                var sereServGroups = sereServ_Bordereaus.GroupBy(o => new { o.SERVICE_ID, o.VIR_PRICE, o.PARENT_ID, o.IS_OUT_PARENT_FEE, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.TDL_REQUEST_DEPARTMENT_ID });
                foreach (var sereServGroup in sereServGroups)
                {
                    V_HIS_SERE_SERV_12 sereServ = sereServGroup.FirstOrDefault();
                    sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                    sereServ_BordereausResult.Add(sereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV_12> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();

            try
            {
                foreach (var item in sereServs)
                {

                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12, MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    sereServADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12, MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(item);
                    var patientTypeCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                    if (sereServADO.PATIENT_TYPE_CODE != patientTypeCFG)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

        private static void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", ModuleData.RoomId, ModuleData.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
