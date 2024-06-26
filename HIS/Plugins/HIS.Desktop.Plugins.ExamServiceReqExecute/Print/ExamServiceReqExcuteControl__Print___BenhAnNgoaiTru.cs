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
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void LoadBieuMauPhieuYCBenhAnNgoaiTru(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                WaitingManager.Show();

                List<Action> methods = new List<Action>();
                methods.Add(LoadPatient);
                methods.Add(LoadPatientTypeAlter);
                methods.Add(LoadDepartmentTran);
                //methods.Add(LoadTranPati);
                methods.Add(LoadExpMest);
                methods.Add(LoadDHST);
                //methods.Add(LoadSereServ);
                ThreadCustomManager.MultipleThreadWithJoin(methods);

                List<long> expMestIds = expMests != null ? expMests.Select(o => o.ID).ToList() : null;
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;//TODO
                CommonParam param = new CommonParam();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param);

                List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>();
                //AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXAM_SERVICE_REQ_1, V_HIS_EXAM_SERVICE_REQ>();
                //V_HIS_EXAM_SERVICE_REQ examServiceReq = AutoMapper.Mapper.Map<V_HIS_EXAM_SERVICE_REQ_1, V_HIS_EXAM_SERVICE_REQ>(this.SereServExt);
                string requestDepartmentName = "";

                requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.HisServiceReqView.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;

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
                sereServFilter.TREATMENT_ID = this.HisServiceReqView.TREATMENT_ID;
                sereServFilter.TDL_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                    , IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT };

                var sereServList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>(ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, null);

                if (sereServList != null && sereServList.Count > 0 && executeRoomIsExam != null && executeRoomIsExam.Count > 0)
                {
                    sereServList = sereServList.Where(o => executeRoomIsExam.Select(p => p.ROOM_ID).Contains(o.TDL_REQUEST_ROOM_ID)).ToList();
                }

                WaitingManager.Hide();
                MPS.Processor.Mps000174.PDO.Mps000174PDO rdo = new MPS.Processor.Mps000174.PDO.Mps000174PDO(
                    patient,
                    departmentTrans,
                    patientTypeAlter,
                    this.HisServiceReqView,
                    dhst,
                    treatment,
                    icds,
                    expMests,
                    expMestMedicines,
                    requestDepartmentName,
                    ado,
                    sereServList
                    );


                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);


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
