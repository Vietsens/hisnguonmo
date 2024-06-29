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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Print;
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

        private async Task PrintMps000178()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private V_HIS_TREATMENT_4 GetTreatment4_ByID(long treatmentId)
        {
            V_HIS_TREATMENT_4 result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                filter.ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessPrintMps000178(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000178");
                if (this.treatmentId > 0)
                {
                    V_HIS_TREATMENT_4 treatment4 = GetTreatment4_ByID(treatmentId);

                    //Loai Patient_type_name
                    V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                    long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    if (instructionTime < treatment4.IN_TIME)
                    {
                        instructionTime = treatment4.IN_TIME;
                    }
                    PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment4.ID, instructionTime, ref currentHispatientTypeAlter);
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = treatment4.PATIENT_ID;
                    var currentPatient = new BackendAdapter(param)
                              .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                    V_HIS_DEPARTMENT_TRAN departmentTran = new V_HIS_DEPARTMENT_TRAN();
                    MOS.Filter.HisDepartmentTranViewFilter defilter = new HisDepartmentTranViewFilter();
                    defilter.TREATMENT_ID = treatment4.ID;
                    var departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumer.ApiConsumers.MosConsumer, defilter, null);
                    if (departmentTrans != null && departmentTrans.Count > 0)
                    {
                        departmentTrans = departmentTrans.OrderByDescending(o => o.DEPARTMENT_IN_TIME ?? Int64.MaxValue).ThenByDescending(o => o.ID).ToList();
                        departmentTran = departmentTrans.First();
                    }

                    WaitingManager.Hide();

                    MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                       currentPatient,
                       currentHispatientTypeAlter,
                       treatment4,
                       departmentTran
                       );

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment4 != null ? treatment4.TREATMENT_CODE : ""), printTypeCode, this.moduleData.RoomId);
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
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
