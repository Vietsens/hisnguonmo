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
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000194.PDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000194
{
    public class Mps000194Behavior : MpsDataBase, ILoad
    {
        private string documentName;
        public Mps000194Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, V_HIS_PATIENT _patient, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _userNameReturnResult, string documentname)
            : base(roomId, _treatment)
        {
            this.SereServs = _sereServs;
            this.DepartmentTrans = _departmentTrans;
            this.TreatmentFees = _treamentFees;
            this.Treatment = _treatment;
            this.Rooms = _rooms;
            this.Services = _services;
            this.HeinServiceTypes = _heinServiceTypes;
            this.TotalDayTreatment = _totalDayTreatment;
            this.StatusTreatmentOut = _statusTreatmentOut;
            this.DepartmentName = _departmentName;
            this.UserNameReturnResult = _userNameReturnResult;
            this.Patient = _patient;
            this.CurrentPatientTypeAlter = currentPatientTypeAlter;
            this.documentName = documentname;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            try
            {

                MPS.Processor.Mps000194.PDO.HeinServiceTypeCFG heinServiceType = new MPS.Processor.Mps000194.PDO.HeinServiceTypeCFG();
                heinServiceType.EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;

                var heinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();

                MPS.Processor.Mps000194.PDO.BordereauSingleValue bordereauSingleValue = new MPS.Processor.Mps000194.PDO.BordereauSingleValue();
                bordereauSingleValue.departmentName = DepartmentName;
                bordereauSingleValue.statusTreatmentOut = StatusTreatmentOut;
                bordereauSingleValue.userNameReturnResult = UserNameReturnResult;
                bordereauSingleValue.today = TotalDayTreatment;

                MPS.Processor.Mps000194.PDO.PatientTypeCFG patientTypeCFG = new MPS.Processor.Mps000194.PDO.PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                List<HIS_TREATMENT_TYPE> treatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                List<HIS_PATIENT_TYPE_ALTER> patyBhyts = new List<HIS_PATIENT_TYPE_ALTER>();


                WaitingManager.Hide();
                Dictionary<string, List<HIS_SERE_SERV>> dicSereServ = new Dictionary<string, List<HIS_SERE_SERV>>();
                dicSereServ = SereServProcessor.GroupSereServByPatyAlterBhyt(SereServs);
                foreach (var sereServ in dicSereServ)
                {
                    if (sereServ.Value == null || sereServ.Value.Count == 0)
                    {
                        continue;
                    }

                    HIS_PATIENT_TYPE_ALTER patyBhyt = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(sereServ.Value.FirstOrDefault().JSON_PATIENT_TYPE_ALTER);
                    patyBhyts.Add(patyBhyt);
                }

                List<long> serviceReqIds = this.SereServs.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                HisExpMestViewFilter preFilter = new HisExpMestViewFilter();
                preFilter.SERVICE_REQ_IDs = serviceReqIds;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param)
                .Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, preFilter, param);

                HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
                sereServExtFilter.SERE_SERV_IDs = this.SereServs.Select(o => o.ID).ToList();
                List<HIS_SERE_SERV_EXT> sereServExts = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);

                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();

                MPS.Processor.Mps000194.PDO.Mps000194PDO rdo = new MPS.Processor.Mps000194.PDO.Mps000194PDO(this.CurrentPatientTypeAlter, Patient, patyBhyts, DepartmentTrans, TreatmentFees, heinServiceType, patientTypeCFG, this.SereServs, sereServExts, expMests, HeinServiceTypes, Rooms, Services, Treatment, treatmentTypes, branch, materialTypes, bordereauSingleValue);

                PrintCustomShow<Mps000194PDO> printShow = new PrintCustomShow<Mps000194PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId, documentName);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
