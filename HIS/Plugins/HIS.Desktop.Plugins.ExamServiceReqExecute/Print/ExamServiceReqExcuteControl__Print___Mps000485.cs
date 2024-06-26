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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl
    {

        private void PrintMps000485()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__NGUYEN_NHAN_TU_VONG, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintMps000485(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000485");
                if (this.treatmentId > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = treatmentId;
                    HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    List<HIS_EVENTS_CAUSES_DEATH> lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    HIS_DEPARTMENT_TRAN departmentTran = new HIS_DEPARTMENT_TRAN();
                    HIS_DEPARTMENT department = new HIS_DEPARTMENT();
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    CommonParam param = new CommonParam();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = treatmentId;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        SevereIllnessInfo = dtSevere.FirstOrDefault(o => o.IS_DEATH == 1);
                        if (SevereIllnessInfo != null)
                        {
                            MOS.Filter.HisDepartmentTranFilter filterDepartmentTran = new HisDepartmentTranFilter();
                            filterDepartmentTran.TREATMENT_ID = this.treatmentId;
                            filterDepartmentTran.DEPARTMENT_ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datas = new BackendAdapter(null).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDepartmentTran, null);
                            if (datas != null && datas.Count > 0)
                                departmentTran = datas.Last();
                            MOS.Filter.HisDepartmentFilter filterDepartment = new HisDepartmentFilter();
                            filterDepartment.ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datasDepatment = new BackendAdapter(null).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filterDepartment, null);
                            if (datasDepatment != null && datasDepatment.Count > 0)
                                department = datasDepatment.First();
                            HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
                            filterChild.SEVERE_ILLNESS_INFO_ID = SevereIllnessInfo.ID;
                            lstEvents = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
                        }
                    }
                    HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                    patientTypeAlterFilter.TREATMENT_ID = treatmentId;
                    var patientTypeAlterData = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                    if (patientTypeAlterData != null && patientTypeAlterData.Count > 0)
                    {
                        patientTypeAlter = patientTypeAlterData[0];
                    }
                    HIS_PATIENT patient = GetPatientByID(treatment.PATIENT_ID);
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (SevereIllnessInfo == null)
                        SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    if (lstEvents == null)
                        lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    if (patientTypeAlter == null)
                        patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    if (departmentTran == null)
                        departmentTran = new HIS_DEPARTMENT_TRAN();
                    if (department == null)
                        department = new HIS_DEPARTMENT();

                    WaitingManager.Hide();
                    MPS.Processor.Mps000485.PDO.Mps000485PDO pdo = new MPS.Processor.Mps000485.PDO.Mps000485PDO
                        (
                        SevereIllnessInfo,
                        lstEvents,
                        treatment,
                        patientTypeAlter,
                        patient,
                        departmentTran,
                        department,
                        branch,
                        currentIcds,
                        BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_RESULT>().ToList()
                        );

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });

                    //if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    //}
                    //else
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                    //}
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
