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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.TreeSereServ7V2;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class UCExecuteRoom : UserControlBase
    {
        private void LoadBieuMauPhieuYCBenhAnNgoaiTru(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.serviceReqRightClick.TREATMENT_ID;
                HIS_TREATMENT treatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = treatment.ID;
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.ID).FirstOrDefault();

                //Lấy thông tin chuyển khoa
                HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                departmentTranFilter.TREATMENT_ID = this.serviceReqRightClick.TREATMENT_ID;
                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param);

                //thuoc

                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.TDL_TREATMENT_ID = this.serviceReqRightClick.TREATMENT_ID;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
                if (expMests != null && expMests.Count > 0)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                    medicineFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                    expMestMedicines = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param);
                }

                string requestDepartmentName = "";
                if (this.serviceReqRightClick != null)
                    requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.serviceReqRightClick.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;
                WaitingManager.Hide();
               V_HIS_SERVICE_REQ serviceReq = GetDynamicData(this.serviceReqRightClick);
                List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>();

                HIS_DHST dhst = null;
                if (serviceReq.DHST_ID.HasValue)
                {
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.ID = serviceReq.DHST_ID.Value;
                    dhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();
                }
                MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO ado = new MPS.Processor.Mps000174.PDO.Mps000174PDO.Mps000174ADO();
                if (treatment.TREATMENT_RESULT_ID.HasValue)
                {
                    var treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == treatment.TREATMENT_RESULT_ID.Value);
                    ado.TREATMENT_RESULT_CODE = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_CODE : "";
                    ado.TREATMENT_RESULT_NAME = treatmentResult != null ? treatmentResult.TREATMENT_RESULT_NAME : "";
                }

                // get sereServ
                var executeRoomIsExam = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == 1).ToList();
                MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                sereServFilter.TREATMENT_ID = treatment.ID;
                sereServFilter.TDL_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                    , IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT };

                var sereServList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>(ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, null);

                if (sereServList != null && sereServList.Count > 0 && executeRoomIsExam != null && executeRoomIsExam.Count > 0)
                {
                    sereServList = sereServList.Where(o => executeRoomIsExam.Select(p => p.ROOM_ID).Contains(o.TDL_REQUEST_ROOM_ID)).ToList();
                }

                MPS.Processor.Mps000174.PDO.Mps000174PDO rdo = new MPS.Processor.Mps000174.PDO.Mps000174PDO(patient, departmentTrans, patientTypeAlter, serviceReq, dhst, treatment, icds, expMests, expMestMedicines, requestDepartmentName, ado, sereServList);
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode, this.roomId);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
    }
}
