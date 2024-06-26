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
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000124.PDO;
using MPS.Processor.Mps000124.PDO.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000124
{
    public class Mps000124Behavior : MpsDataBase, ILoad
    {

        public Mps000124Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, List<HIS_SERE_SERV_DEPOSIT> _sereServDeposits, List<HIS_SESE_DEPO_REPAY> _sereDepoRepays, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _userNameReturnResult)
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
            this.SereServDeposits = _sereServDeposits;
            this.SeseDepoRepays = _sereDepoRepays;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Start HisTransaction ");
                HisTransactionFilter tranFilter = new HisTransactionFilter();
                tranFilter.TREATMENT_ID = this.Treatment.ID;
                List<HIS_TRANSACTION> transactions = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tranFilter, param);

                Inventec.Common.Logging.LogSystem.Info("End HisTransaction ");


                List<HIS_TRANSACTION> bills = transactions != null ? transactions.Where(o => o.IS_CANCEL == null && !o.SALE_TYPE_ID.HasValue).OrderByDescending(o => o.TRANSACTION_TIME).ToList() : null;

                SingleKeyValue singleKeyValue = new SingleKeyValue();
                singleKeyValue.departmentName = DepartmentName;
                singleKeyValue.statusTreatmentOut = StatusTreatmentOut;
                singleKeyValue.today = TotalDayTreatment;
                singleKeyValue.userNameReturnResult = UserNameReturnResult;


                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                HeinServiceTypeCFG heinServiceType = new HeinServiceTypeCFG();
                heinServiceType.HEIN_SERVICE_TYPE__HIGHTECH_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                heinServiceType.HEIN_SERVICE_TYPE__EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;
                heinServiceType.HEIN_SERVICE_TYPE__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                heinServiceType.HEIN_SERVICE_TYPE__SURG_MISU_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT;

                PatientTypeCFG patientTypeCFG = new PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                TransactionTypeCFG transactionTypeCFG = new TransactionTypeCFG();
                transactionTypeCFG.TRANSACTION_TYPE_ID__BILL = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
                transactionTypeCFG.TRANSACTION_TYPE_ID__DEPOSIT = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU;
                transactionTypeCFG.TRANSACTION_TYPE_ID__REPAY = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU;

                HIS_SERE_SERV sereServ = this.SereServs.Where(o => o.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).OrderByDescending(o => o.CREATE_TIME).FirstOrDefault(o => o.JSON_PATIENT_TYPE_ALTER != null);
                HIS_PATIENT_TYPE_ALTER patyBhyt = null;
                if (sereServ != null)
                    patyBhyt = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(sereServ.JSON_PATIENT_TYPE_ALTER);

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                string ratio_text = "";
                if (patyBhyt != null && this.CurrentPatientTypeAlter != null)
                    ratio_text = SereServProcessor.GetDefaultHeinRatioForView(patyBhyt.HEIN_CARD_NUMBER, this.CurrentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patyBhyt.RIGHT_ROUTE_CODE);
                singleKeyValue.ratio = ratio_text;

                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Debug(LogUtil.TraceData("input: SereServDeposit", this.SereServDeposits)); //o day null
                Inventec.Common.Logging.LogSystem.Debug(LogUtil.TraceData("input: SeseDepoRepay", this.SeseDepoRepays));

                Inventec.Common.Logging.LogSystem.Info("Start mps process ");

                MPS.Processor.Mps000124.PDO.Mps000124PDO rdo = new MPS.Processor.Mps000124.PDO.Mps000124PDO(patyBhyt, this.DepartmentTrans, this.TreatmentFees, departments, heinServiceType, patientTypeCFG, this.SereServs, this.SereServDeposits, this.SeseDepoRepays, bills, transactionTypeCFG, this.Treatment, this.HeinServiceTypes, this.Rooms, Services, materialTypes, singleKeyValue);

                PrintCustomShow<Mps000124PDO> printShow = new PrintCustomShow<Mps000124PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
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
