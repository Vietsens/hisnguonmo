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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Plugins.Library.PrintPrescription.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    class PrintMps000296
    {
        private V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter;
        private HIS_SERVICE_REQ HisServiceReq_Exam;
        private HIS_DHST hisDhst;
        private HIS_SERVICE_REQ HisPrescriptionSDOPrintPlus;
        private string mediStockName;
        private string expMestCode;
        private V_HIS_ROOM executeRoom;
        private V_HIS_ROOM reqRoom;
        private V_HIS_PATIENT vHisPatient;
        private HIS_TREATMENT hisTreatment;
        string treatmentCode = "";
        private Inventec.Desktop.Common.Modules.Module currentModule;
        MPS.ProcessorBase.PrintConfig.PreviewType? previewType;

        public PrintMps000296(string printTypeCode, string fileName, ref bool result,
            MOS.SDO.OutPatientPresResultSDO currentOutPresSDO,
            bool printNow, bool hasMediMate, Inventec.Desktop.Common.Modules.Module module,
            MPS.ProcessorBase.PrintConfig.PreviewType? previewType,
            Action<int> countData,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint)
        {
            try
            {
                this.currentModule = module;
                this.previewType = previewType;
                if (currentOutPresSDO != null)
                {
                    vHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    HisServiceReq_Exam = new HIS_SERVICE_REQ();
                    hisDhst = new HIS_DHST();
                    mediStockName = "";
                    HisPrescriptionSDOPrintPlus = new HIS_SERVICE_REQ();
                    executeRoom = new V_HIS_ROOM();
                    reqRoom = new V_HIS_ROOM();

                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> ExpMests = null;
                    ThreadMedicineADO mediMateIn = null;
                    ThreadMedicineADO mediMateOut = null;

                    if (currentOutPresSDO.ExpMests != null &&
                        currentOutPresSDO.ExpMests.Count > 0)
                    {
                        ExpMests = currentOutPresSDO.ExpMests;
                        mediMateIn = new ThreadMedicineADO(currentOutPresSDO, hasMediMate);
                        mediMateOut = new ThreadMedicineADO(currentOutPresSDO, hasMediMate);
                        HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault();
                    }

                    if (ExpMests == null || ExpMests.Count <= 0 || HisPrescriptionSDOPrintPlus == null)
                    {
                        result = false;
                        return;
                    }

                    CreateThreadGetCurrentData(HisPrescriptionSDOPrintPlus);

                    new ThreadLoadDataMediMate(mediMateIn, mediMateOut,hisTreatment);

                    if (mediMateIn != null && mediMateIn.DicLstMediMateExpMestTypeADO != null && countData != null)
                    {
                        countData(mediMateIn.DicLstMediMateExpMestTypeADO.Values.SelectMany(s => s).Count());
                    }

                    if (mediMateOut != null && mediMateOut.DicLstMediMateExpMestTypeADO != null && countData != null)
                    {
                        countData(mediMateOut.DicLstMediMateExpMestTypeADO.Values.SelectMany(s => s).Count());
                    }

                    List<MPS.Processor.Mps000296.PDO.ExpMestMedicineSDO> ExpMestMedicineSDO = new List<MPS.Processor.Mps000296.PDO.ExpMestMedicineSDO>();
                    MPS.Processor.Mps000296.PDO.Mps000296ADO mps000296ADO = new MPS.Processor.Mps000296.PDO.Mps000296ADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000296.PDO.Mps000296ADO>(mps000296ADO, vHisPatient);
                    int countMediMate = 0;

                    foreach (var item in ExpMests)
                    {
                        expMestCode = item.EXP_MEST_CODE;
                        var room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                        mediStockName = room != null ? room.MEDI_STOCK_NAME : "";

                        //Thong tin thuoc / vat tu
                        List<ExpMestMedicineSDO> lstMedicineExpmestTypeADO = new List<ExpMestMedicineSDO>();

                        if (mediMateIn.DicLstMediMateExpMestTypeADO.ContainsKey(item.ID) &&
                            mediMateIn.DicLstMediMateExpMestTypeADO[item.ID] != null &&
                            mediMateIn.DicLstMediMateExpMestTypeADO[item.ID].Count > 0)
                        {
                            lstMedicineExpmestTypeADO.AddRange(mediMateIn.DicLstMediMateExpMestTypeADO[item.ID]);
                        }

                        if (currentOutPresSDO.ServiceReqs != null &&
                            currentOutPresSDO.ServiceReqs.Count > 0)
                        {
                            HisPrescriptionSDOPrintPlus = currentOutPresSDO.ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                        }

                        if (HisPrescriptionSDOPrintPlus == null) continue;
                        if (HisPrescriptionSDOPrintPlus.USE_TIME == null)
                        {
                            HisPrescriptionSDOPrintPlus.USE_TIME = HisPrescriptionSDOPrintPlus.INTRUCTION_TIME;
                        }

                        executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.EXECUTE_ROOM_ID);
                        reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisPrescriptionSDOPrintPlus.REQUEST_ROOM_ID);

                        #region in thuong
                        if (lstMedicineExpmestTypeADO != null && lstMedicineExpmestTypeADO.Count > 0)
                        {
                            countMediMate += lstMedicineExpmestTypeADO.Count;
                            var group = lstMedicineExpmestTypeADO.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME });
                            foreach (var aitem in group)
                            {
                                MPS.Processor.Mps000296.PDO.ExpMestMedicineSDO ado = new MPS.Processor.Mps000296.PDO.ExpMestMedicineSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000296.PDO.ExpMestMedicineSDO>(ado, aitem.First());
                                ado.AMOUNT = aitem.Sum(o => o.AMOUNT);
                                ado.EXP_MEST_CODE = item.EXP_MEST_CODE;

                                ado.CREATOR_NAME = item.REQ_USERNAME;
                                ExpMestMedicineSDO.Add(ado);
                            }
                            //Lọc thuốc theo thứ tự
                            ExpMestMedicineSDO = ExpMestMedicineSDO.OrderBy(o => o.NUM_ORDER ?? 99999).ToList();

                            //MPS.Processor.Mps000296.PDO.Mps000296ADO mps000296ADO = new MPS.Processor.Mps000296.PDO.Mps000296ADO();
                            mps000296ADO.EXECUTE_DEPARTMENT_NAME = executeRoom != null ? executeRoom.DEPARTMENT_NAME : "";
                            mps000296ADO.EXECUTE_ROOM_NAME = executeRoom != null ? executeRoom.ROOM_NAME : "";
                            mps000296ADO.EXP_MEST_CODE = expMestCode;
                            mps000296ADO.REQUEST_DEPARTMENT_NAME = reqRoom != null ? reqRoom.DEPARTMENT_NAME : "";
                            mps000296ADO.REQUEST_ROOM_NAME = reqRoom != null ? reqRoom.ROOM_NAME : "";
                            mps000296ADO.MEDI_STOCK_NAME = mediStockName;
                            mps000296ADO.TITLE = "";
                        }
                        #endregion
                    }

                    MPS.Processor.Mps000296.PDO.Mps000296PDO mps000296RDO = new MPS.Processor.Mps000296.PDO.Mps000296PDO(
                        vHisPatientTypeAlter,
                        hisDhst,
                        HisPrescriptionSDOPrintPlus,
                        ExpMestMedicineSDO,
                        HisServiceReq_Exam,
                        hisTreatment,
                        mps000296ADO);

                    Print.PrintData(printTypeCode, fileName, mps000296RDO, printNow, treatmentCode, ref result, this.currentModule != null ? currentModule.RoomId : 0, previewType, countMediMate, savedData);
                    //PrintData(printTypeCode, fileName, mps000296RDO, printNow, ref result);
                }
            }
            catch (Exception ex)
            {
                cancelPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Thread
        private void CreateThreadGetCurrentData(HIS_SERVICE_REQ HisPrescriptionResultSDOs)
        {
            if (HisPrescriptionResultSDOs != null)
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.KEY_IsPrintPrescriptionNoThread) == "1")
                {
                    GetDataPatientTypeDhst(HisPrescriptionResultSDOs);
                    GetDataExam(HisPrescriptionResultSDOs);
                    GetPatient(HisPrescriptionResultSDOs);
                    GetTreatment(HisPrescriptionResultSDOs);
                }
                else
                {
                    Thread patientType = new Thread(GetDataPatientTypeDhst);
                    Thread exam = new Thread(GetDataExam);
                    Thread patient = new Thread(GetPatient);
                    Thread treatment = new Thread(GetTreatment);
                    try
                    {
                        patientType.Start(HisPrescriptionResultSDOs);
                        exam.Start(HisPrescriptionResultSDOs);
                        patient.Start(HisPrescriptionResultSDOs);
                        treatment.Start(HisPrescriptionResultSDOs);

                        patientType.Join();
                        exam.Join();
                        patient.Join();
                        treatment.Join();
                    }
                    catch (Exception ex)
                    {
                        patientType.Abort();
                        exam.Abort();
                        patient.Abort();
                        treatment.Abort();
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
        }

        private void GetTreatment(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;

                    CommonParam paramCommon = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter() { ID = data.TREATMENT_ID };
                    var lstTreatment = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (lstTreatment != null && lstTreatment.Count > 0)
                    {
                        hisTreatment = lstTreatment.FirstOrDefault();
                        treatmentCode = hisTreatment.TREATMENT_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatient(object obj)
        {
            try
            {
                HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                if (data != null)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisPatientViewFilter filter = new HisPatientViewFilter() { PATIENT_CODE = data.TDL_PATIENT_CODE };
                    var lstpatient = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_PATIENT>>(RequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (lstpatient != null && lstpatient.Count > 0)
                    {
                        vHisPatient = lstpatient.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataExam(object obj)
        {
            try
            {
                GetExamWithPres((HIS_SERVICE_REQ)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExamWithPres(HIS_SERVICE_REQ hIS_SERVICE_REQ)
        {
            try
            {
                HisServiceReqFilter serviceReqfilter = new HisServiceReqFilter();
                serviceReqfilter.TREATMENT_ID = hIS_SERVICE_REQ.TREATMENT_ID;
                serviceReqfilter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH };
                var lstexamServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqfilter, null);

                if (lstexamServiceReq != null && lstexamServiceReq.Count > 0)
                {
                    if (lstexamServiceReq.Count == 1)
                    {
                        HisServiceReq_Exam = lstexamServiceReq.First();
                    }
                    else
                    {
                        List<HIS_SERVICE_REQ> lstExit = new List<HIS_SERVICE_REQ>();
                        foreach (var item in lstexamServiceReq)
                        {
                            if (item.EXECUTE_ROOM_ID == hIS_SERVICE_REQ.REQUEST_ROOM_ID && (!String.IsNullOrEmpty(item.FULL_EXAM) || !String.IsNullOrEmpty(item.PATHOLOGICAL_PROCESS)))
                            {
                                lstExit.Add(item);
                            }
                        }

                        if (lstExit.Count > 0)
                        {
                            if (lstExit.Count == 1)
                            {
                                HisServiceReq_Exam = lstExit.First();
                            }
                            else
                            {
                                HisServiceReq_Exam = lstExit.OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                            }
                        }
                        else
                        {
                            HisServiceReq_Exam = lstexamServiceReq.OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataPatientTypeDhst(object obj)
        {
            try
            {
                GetPatientDhst((HIS_SERVICE_REQ)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatientDhst(HIS_SERVICE_REQ hIS_SERVICE_REQ)
        {
            try
            {
                CommonParam param = new CommonParam();

                //Lấy thông tin thẻ BHYT
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = hIS_SERVICE_REQ.TREATMENT_ID;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                vHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(RequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                //Dấu hiệu sinh tồn
                MOS.Filter.HisDhstFilter hisDhstFilter = new HisDhstFilter();
                hisDhstFilter.TREATMENT_ID = hIS_SERVICE_REQ.TREATMENT_ID;
                var dhsts = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(RequestUriStore.HIS_DHST_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDhstFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dhsts != null && dhsts.Count > 0)
                {
                    //hisDhst = dhsts.FirstOrDefault(o => o.EXECUTE_TIME <= hIS_SERVICE_REQ.INTRUCTION_TIME);
                    //in luôn lúc chỉ định không có thời gian tạo
                    long dateTimeNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 99999999999999;
                    //lấy dhst do ng kê đơn thực hiện. nếu ko có lấy dhst gần với thời gian kê đơn nhất
                    var dhstExecute = dhsts.Where(o => o.CREATE_TIME <= (hIS_SERVICE_REQ.CREATE_TIME ?? dateTimeNow) && o.EXECUTE_LOGINNAME == hIS_SERVICE_REQ.REQUEST_LOGINNAME).ToList();
                    if (dhstExecute != null && dhstExecute.Count > 0)
                    {
                        hisDhst = dhstExecute.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                    }
                    else
                        hisDhst = dhsts.OrderByDescending(o => o.ID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
