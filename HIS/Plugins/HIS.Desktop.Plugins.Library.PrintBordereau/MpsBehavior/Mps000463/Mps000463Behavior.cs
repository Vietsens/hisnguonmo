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
using HIS.Desktop.Plugins.Library.PrintBordereau.ChooseServiceType;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000463.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.MpsBehavior.Mps000463
{
    class Mps000463Behavior : MpsDataBase, ILoad
    {
        public Mps000463Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment,
            V_HIS_PATIENT _patient, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _roomName, string _userNameReturnResult,
            List<HIS_SERE_SERV_BILL> listSsBill, List<HIS_SERE_SERV_DEPOSIT> listSsDepsit, List<HIS_SESE_DEPO_REPAY> listSsRepay)
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
            this.SereServBills = listSsBill;
            this.SereServDeposits = listSsDepsit;
            this.SeseDepoRepays = listSsRepay;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            try
            {
                PatientTypeCFG patientTypeCFG = new PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                SingleKeyValue singleValue = new SingleKeyValue();
                singleValue.departmentName = DepartmentName;
                singleValue.statusTreatmentOut = StatusTreatmentOut;
                singleValue.today = TotalDayTreatment;
                singleValue.roomName = RoomName;
                singleValue.userNameReturnResult = UserNameReturnResult;

                bool isPriceWithDifference = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_PRICE_WITH_DIFFERENCE)) == 1 ? true : false;
                bool IsNotSameDepartment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION)) == 1 ? true : false;
                bool surgPriceOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.CALC_ARISING_SURG_PRICE_OPTION) == "1" ? true : false;
                bool IsNotIncludeIsExpend = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfigKey.ConfigKey_NotIncludeIsExpend) == 1 ? true : false;
                bool IsMergeServiceNotHein = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.MERGE_SERVICE_NOT_HEIN)) == 1 ? true : false;
                HisConfigValue hisConfigValue = new HisConfigValue();
                hisConfigValue.IsPriceWithDifference = isPriceWithDifference;
                hisConfigValue.IsNotSameDepartment = IsNotSameDepartment;
                hisConfigValue.IsSurgPriceOption_1 = surgPriceOption;
                hisConfigValue.IsNotIncludeIsExpend = IsNotIncludeIsExpend;
                hisConfigValue.IsMergeServiceNotHein = IsMergeServiceNotHein;

                //mở cửa sổ chọn loại dịch vụ

                FormChooseServiceType frmChoose = new FormChooseServiceType(this.SereServs, SereServChoosed);
                frmChoose.ShowDialog();
                WaitingManager.Show();

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                List<HIS_TREATMENT_TYPE> treatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>();

                CommonParam param = new CommonParam();
                HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
                sereServExtFilter.SERE_SERV_IDs = this.SereServs.Select(o => o.ID).ToList();
                List<HIS_SERE_SERV_EXT> sereServExts = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);

                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                List<HIS_MEDI_ORG> mediOrg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_ORG>();
                List<HIS_OTHER_PAY_SOURCE> otherPaySource = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                List<HIS_SERVICE_UNIT> servuceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                List<HIS_DIIM_TYPE> diimTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DIIM_TYPE>();

                HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                patientTypeAlterFilter.TREATMENT_ID = this.Treatment.ID;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "ASC";
                List<HIS_PATIENT_TYPE_ALTER> patientTypeAlters = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                long isShowMedicineLine = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_SHOW_MEDICINE_LINE));

                List<HIS_MEDICINE_TYPE> medicineTypes = null;
                List<HIS_MEDICINE_LINE> medicineLines = null;
                List<HIS_SERVICE_REQ> serviceReqs = null;
                MPS.Processor.Mps000463.PDO.Mps000463PDO rdo = null;
                if (isShowMedicineLine == 1)
                {
                    medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                    medicineLines = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_LINE>();

                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.TREATMENT_ID = this.Treatment.ID;
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                    serviceReqs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);
                }

                rdo = new MPS.Processor.Mps000463.PDO.Mps000463PDO(this.CurrentPatientTypeAlter, patientTypeAlters, DepartmentTrans, TreatmentFees,
                    patientTypeCFG, this.SereServs, sereServExts, Treatment, this.Patient, HeinServiceTypes, Rooms, Services, treatmentTypes, branch,
                    materialTypes, departments, singleValue, hisConfigValue, servuceUnit, diimTypes, mediOrg, medicineTypes, medicineLines, serviceReqs,
                    otherPaySource, this.SereServBills, this.SereServDeposits, this.SeseDepoRepays);

                #region Run Print
               
                WaitingManager.Hide();
                PrintCustomShow<Mps000463PDO> printShow = new PrintCustomShow<Mps000463PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SereServChoosed(List<HIS_SERE_SERV> data)
        {
            this.SereServs = data;
        }
    }
}
