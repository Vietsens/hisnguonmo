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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterV2.Process
{
    class DepositServicePrintProcess
    {
        private static Inventec.Desktop.Common.Modules.Module module;

        public static void LoadPhieuThuPhiDichVu(string printTypeCode, string fileName, bool isExpand, List<V_HIS_SERE_SERV_12> SereServAlls, HisServiceReqExamRegisterResultSDO resultSDO, bool isPrintNow, Inventec.Desktop.Common.Modules.Module moduleData)
        {
            bool result = false;
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
            MPS.Processor.Mps000102.PDO.PatientADO patient = new MPS.Processor.Mps000102.PDO.PatientADO();
            V_HIS_TREATMENT_FEE treatmentFee = null;
            try
            {
                module = moduleData;
                //Thông tin bệnh nhân
                if (resultSDO.HisPatientProfile.HisPatient == null)
                {
                    HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                    patientViewFilter.ID = resultSDO.HisPatientProfile.HisTreatment.PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        patient = new MPS.Processor.Mps000102.PDO.PatientADO(patients.FirstOrDefault());
                    }
                }
                else
                {
                    Mapper.CreateMap<HIS_PATIENT, V_HIS_PATIENT>();
                    V_HIS_PATIENT p = Mapper.Map<V_HIS_PATIENT>(resultSDO.HisPatientProfile.HisPatient);
                    patient = new MPS.Processor.Mps000102.PDO.PatientADO(p);
                }

                //TreatmentFee
                HisTreatmentFeeViewFilter treatFeeFilter = new HisTreatmentFeeViewFilter();
                treatFeeFilter.ID = resultSDO.HisPatientProfile.HisTreatment.ID;
                List<V_HIS_TREATMENT_FEE> fees = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatFeeFilter, null);
                if (fees != null)
                {
                    treatmentFee = fees.FirstOrDefault();
                }
                //Thông tin bảo hiểm y tế
                HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                patientTypeFilter.TREATMENT_ID = resultSDO.HisPatientProfile.HisTreatment.ID;
                patientTypeFilter.ID = resultSDO.HisPatientProfile.HisPatientTypeAlter.ID;
                var patientTypeAlters = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, null);
                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }

                List<V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT, ApiConsumers.MosConsumer, resultSDO.HisPatientProfile.HisTreatment.ID, null);

                long? totalDay = null;

                if (resultSDO.HisPatientProfile.HisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(resultSDO.HisPatientProfile.HisTreatment.IN_TIME, resultSDO.HisPatientProfile.HisTreatment.OUT_TIME, resultSDO.HisPatientProfile.HisTreatment.TREATMENT_END_TYPE_ID, resultSDO.HisPatientProfile.HisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(resultSDO.HisPatientProfile.HisTreatment.IN_TIME, resultSDO.HisPatientProfile.HisTreatment.OUT_TIME, resultSDO.HisPatientProfile.HisTreatment.TREATMENT_END_TYPE_ID, resultSDO.HisPatientProfile.HisTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
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
                    sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                    sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServHitech.VIR_TOTAL_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);

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
                var serviceReports = new BackendAdapter(new CommonParam()).Get<List<HIS_HEIN_SERVICE_TYPE>>(HisRequestUriStore.HIS_HEIN_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, HeinServiceTypefilter, null);

                // tinh muc huong
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.LEVEL_CODE, patientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
               
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultSDO.HisPatientProfile.HisTreatment != null ? resultSDO.HisPatientProfile.HisTreatment.TREATMENT_CODE : ""), printTypeCode, moduleData.RoomId);

                var deposit = resultSDO.Transactions.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).ToList();
                if (deposit != null && deposit.Count > 0)
                {
                    foreach (var item in deposit)
                    {
                        var sereServDeposit = resultSDO.SereServDeposits.Where(o => o.DEPOSIT_ID == item.ID).ToList();
                        MPS.Processor.Mps000102.PDO.Mps000102PDO pdo = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                                patient,
                                patientTypeAlter,
                                departmentName,

                                sereServNotHitechADOs,
                                sereServHitechADOs,
                                sereServVTTTADOs,

                                departmentTrans,
                                treatmentFee,

                                serviceReports,

                                item,
                                sereServDeposit,

                                totalDay,
                                ratio_text,
                                resultSDO.ServiceReqs.FirstOrDefault()
                                );

                        MPS.ProcessorBase.Core.PrintData PrintData = null; 
                        if (isPrintNow)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ?  GlobalVariables.dicPrinter[printTypeCode] : "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !string.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]) ? GlobalVariables.dicPrinter[printTypeCode] : "", 1, false, true) { EmrInputADO = inputADO };
                        }
                       
                        PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                        MPS.MpsPrinter.Run(PrintData);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
        }

        private static List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV_12> sereServs)
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

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", module.RoomId, module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
