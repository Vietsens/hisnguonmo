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
using MPS.Processor.Mps000196.PDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000196
{
    public class Mps000196Behavior : MpsDataBase, ILoad
    {
        private string documentName;
        public Mps000196Behavior(long? roomId, V_HIS_PATIENT _patient, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _userNameReturnResult, string documentname)
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
            this.documentName = documentname;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            try
            {
                MPS.Processor.Mps000196.PDO.HeinServiceTypeCFG heinServiceType = new MPS.Processor.Mps000196.PDO.HeinServiceTypeCFG();
                heinServiceType.SURG_MISU_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT;
                heinServiceType.HIGHTECH_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                heinServiceType.EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;

                MPS.Processor.Mps000196.PDO.PatientTypeCFG patientType = new MPS.Processor.Mps000196.PDO.PatientTypeCFG();
                patientType.PATIENT_TYPE_BHYT_ID = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientType.PATIENT_TYPE_FEE_ID = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                var heinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();

                List<long> serviceReqIds = SereServs.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.SERVICE_REQ_IDs = serviceReqIds;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param)
                .Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                MPS.Processor.Mps000196.PDO.BordereauSingleValue bordereauSingleValue = new MPS.Processor.Mps000196.PDO.BordereauSingleValue();
                bordereauSingleValue.departmentName = DepartmentName;
                bordereauSingleValue.statusTreatmentOut = StatusTreatmentOut;
                bordereauSingleValue.userNameReturnResult = UserNameReturnResult;
                bordereauSingleValue.today = TotalDayTreatment;

                MPS.Processor.Mps000196.PDO.Mps000196PDO rdo = new MPS.Processor.Mps000196.PDO.Mps000196PDO(Patient, SereServs, Treatment, expMests, heinServiceTypes, TreatmentFees, DepartmentTrans, Rooms, Services, heinServiceType, patientType, bordereauSingleValue);

                PrintCustomShow<Mps000196PDO> printShow = new PrintCustomShow<Mps000196PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
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
