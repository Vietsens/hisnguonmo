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
using MPS.Processor.Mps000125.PDO;
using MPS.Processor.Mps000125.PDO.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000124
{
    public class Mps000125Behavior : MpsDataBase, ILoad
    {
        private string documentName;
        public Mps000125Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _userNameReturnResult, string documentname)
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
            this.CurrentPatientTypeAlter = currentPatientTypeAlter;
            this.documentName = documentname;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            try
            {
                HeinServiceTypeCFG heinServiceType = new HeinServiceTypeCFG();
                heinServiceType.HEIN_SERVICE_TYPE__HIGHTECH_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                heinServiceType.HEIN_SERVICE_TYPE__EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;
                heinServiceType.HEIN_SERVICE_TYPE__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                heinServiceType.HEIN_SERVICE_TYPE__SURG_MISU_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT;
                PatientTypeCFG patientTypeCFG = new PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;
                SingleKeyValue singleKeyValue = new SingleKeyValue();
                singleKeyValue.departmentName = DepartmentName;
                singleKeyValue.statusTreatmentOut = StatusTreatmentOut;
                singleKeyValue.today = TotalDayTreatment;
                singleKeyValue.userNameReturnResult = UserNameReturnResult;

                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();


                HisTransactionFilter tranFilter = new HisTransactionFilter();
                tranFilter.TREATMENT_ID = this.Treatment.ID;
                tranFilter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
                List<HIS_TRANSACTION> transactions = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tranFilter, param);
                List<HIS_TRANSACTION> bills = transactions != null ? transactions.Where(o => o.IS_CANCEL == null && !o.SALE_TYPE_ID.HasValue).OrderByDescending(o => o.TRANSACTION_TIME).ToList() : null;
                Dictionary<string, List<HIS_SERE_SERV>> dicSereServ = new Dictionary<string, List<HIS_SERE_SERV>>();
                dicSereServ = SereServProcessor.GroupSereServByPatyAlterBhyt(this.SereServs);
                foreach (var sereServ in dicSereServ)
                {
                    HIS_PATIENT_TYPE_ALTER patyBhyt = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(sereServ.Value.FirstOrDefault().JSON_PATIENT_TYPE_ALTER);
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                    HIS_TREATMENT_TYPE treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.CurrentPatientTypeAlter.TREATMENT_TYPE_ID);

                    Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patyBhyt), patyBhyt));
                    string ratio_text = SereServProcessor.GetDefaultHeinRatioForView(patyBhyt.HEIN_CARD_NUMBER, treatmentType.HEIN_TREATMENT_TYPE_CODE, levelCode, patyBhyt.RIGHT_ROUTE_CODE);
                    Inventec.Common.Logging.LogSystem.Error("9");
                    singleKeyValue.ratio = ratio_text;
                    MPS.Processor.Mps000125.PDO.Mps000125PDO rdo = new MPS.Processor.Mps000125.PDO.Mps000125PDO(patyBhyt, this.DepartmentTrans, this.TreatmentFees, departments, bills, heinServiceType, patientTypeCFG, sereServ.Value, this.Treatment, this.HeinServiceTypes, this.Rooms, Services, materialTypes, singleKeyValue);

                    PrintCustomShow<Mps000125PDO> printShow = new PrintCustomShow<Mps000125PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                    result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId, documentName);
                }
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
