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
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000348.PDO;
using MPS.Processor.Mps000348.PDO.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.MpsBehavior.Mps000348
{
    class Mps000348Behavior : MpsDataBase, ILoad
    {
        public Mps000348Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, V_HIS_PATIENT _patient, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _roomName, string _userNameReturnResult)
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
            this.RoomName = _roomName;
            this.Patient = _patient;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000348.PDO.Config.PatientTypeCFG patientTypeCFG = new MPS.Processor.Mps000348.PDO.Config.PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                Dictionary<string, List<HIS_SERE_SERV>> dicSereServ = new Dictionary<string, List<HIS_SERE_SERV>>();
                dicSereServ = SereServProcessor.GroupSereServByPatyAlterBhyt(this.SereServs);
                SingleKeyValue singleValue = new SingleKeyValue();
                singleValue.departmentName = DepartmentName;
                singleValue.statusTreatmentOut = StatusTreatmentOut;
                singleValue.today = TotalDayTreatment;
                singleValue.roomName = RoomName;
                singleValue.userNameReturnResult = UserNameReturnResult;

                bool isPriceWithDifference = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_PRICE_WITH_DIFFERENCE)) == 1 ? true : false;
                bool IsNotSameDepartment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION)) == 1 ? true : false;
                bool surgPriceOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.CALC_ARISING_SURG_PRICE_OPTION) == "1" ? true : false;

                HisConfigValue hisConfigValue = new HisConfigValue();
                hisConfigValue.IsPriceWithDifference = isPriceWithDifference;
                hisConfigValue.IsNotSameDepartment = IsNotSameDepartment;
                hisConfigValue.IsSurgPriceOption = surgPriceOption;

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                List<HIS_TREATMENT_TYPE> treatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>();

                CommonParam param = new CommonParam();
                HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
                sereServExtFilter.SERE_SERV_IDs = this.SereServs.Select(o => o.ID).ToList();
                List<HIS_SERE_SERV_EXT> sereServExts = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);

                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();

                HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                patientTypeAlterFilter.TREATMENT_ID = this.Treatment.ID;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "ASC";
                List<HIS_PATIENT_TYPE_ALTER> patientTypeAlters = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                long isShowMedicineLine = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_SHOW_MEDICINE_LINE));
                List<HIS_SERVICE_UNIT> servuceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                List<HIS_MEDI_ORG> mediOrg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_ORG>();

                MPS.Processor.Mps000348.PDO.Mps000348PDO rdo = null;
                if (isShowMedicineLine == 1)
                {
                    List<HIS_MEDICINE_TYPE> medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                    List<HIS_MEDICINE_LINE> medicineLines = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_LINE>();

                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.TREATMENT_ID = this.Treatment.ID;
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                    List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                    rdo = new MPS.Processor.Mps000348.PDO.Mps000348PDO(this.CurrentPatientTypeAlter, patientTypeAlters, DepartmentTrans, TreatmentFees, patientTypeCFG, this.SereServs, sereServExts, Treatment, this.Patient, HeinServiceTypes, Rooms, Services, treatmentTypes, branch, medicineTypes, materialTypes, medicineLines, serviceReqs, departments, singleValue, hisConfigValue, servuceUnit, mediOrg);
                }
                else
                {
                    rdo = new MPS.Processor.Mps000348.PDO.Mps000348PDO(this.CurrentPatientTypeAlter, patientTypeAlters, DepartmentTrans, TreatmentFees, patientTypeCFG, this.SereServs, sereServExts, Treatment, this.Patient, HeinServiceTypes, Rooms, Services, treatmentTypes, branch, materialTypes, departments, singleValue, hisConfigValue, servuceUnit, mediOrg);
                }

                #region Run Print
                PrintCustomShow<Mps000348PDO> printShow = new PrintCustomShow<Mps000348PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
                #endregion
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
