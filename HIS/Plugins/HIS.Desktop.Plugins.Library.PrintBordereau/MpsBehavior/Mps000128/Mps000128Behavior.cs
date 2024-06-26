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
using HIS.Desktop.Plugins.Library.PrintBordereau.ChooseDepartment;
using HIS.Desktop.Plugins.Library.PrintBordereau.ChooseService;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000128.PDO;
using MPS.Processor.Mps000128.PDO.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000128
{
    public class Mps000128Behavior : MpsDataBase, ILoad
    {

        HIS_SERE_SERV sereServKTC { get; set; }

        public Mps000128Behavior(long? roomId, V_HIS_PATIENT _patient, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _userNameReturnResult, long? _currentDepartmentId)
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
            this.CurrentDepartmentId = _currentDepartmentId;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            try
            {
                HIS_SERE_SERV sereServ = this.SereServs.Where(o => o.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && o.JSON_PATIENT_TYPE_ALTER != null).OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                HIS_PATIENT_TYPE_ALTER patyBhyt = null;
                if (sereServ != null)
                    patyBhyt = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(sereServ.JSON_PATIENT_TYPE_ALTER);
                WaitingManager.Hide();

                string packagePolicyCode3Day7Day = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.POLICY_CODE_3DAY7DAY);

                HIS_PACKAGE package = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.PACKAGE_CODE == packagePolicyCode3Day7Day);
                if (package == null)
                {
                    MessageManager.Show("Không tìm thấy gói 37 theo cầu hình ");
                    return false;
                }

                var sereServHitechs = this.SereServs.Where(o => o.PACKAGE_ID == package.ID && o.IS_NO_EXECUTE != 1).ToList();
                if (sereServHitechs == null || sereServHitechs.Count == 0)
                {
                    MessageManager.Show("Không có dịch vụ nào có gói 3/7");
                    return false;
                }

                frmChonDichVuPhauThuat frmChoose = new frmChonDichVuPhauThuat(sereServHitechs, SereServKTCChoosed);
                frmChoose.ShowDialog();
                WaitingManager.Show();

                if (sereServKTC == null)
                    throw new Exception("sereServKTC is null ");


                HeinServiceTypeCFG heinServiceType = new HeinServiceTypeCFG();
                heinServiceType.HEIN_SERVICE_TYPE__HIGHTECH_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                heinServiceType.HEIN_SERVICE_TYPE__EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;
                heinServiceType.HEIN_SERVICE_TYPE__SURG_MISU_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT;

                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();

                SingleKeyValue singleKeyValue = new SingleKeyValue();
                singleKeyValue.departmentName = DepartmentName;
                singleKeyValue.statusTreatmentOut = StatusTreatmentOut;
                singleKeyValue.today = TotalDayTreatment;
                singleKeyValue.userNameReturnResult = UserNameReturnResult;
                singleKeyValue.ratio = TotalDayTreatment.ToString();
                WaitingManager.Hide();

                MPS.Processor.Mps000128.PDO.Mps000128PDO rdo = new MPS.Processor.Mps000128.PDO.Mps000128PDO(patyBhyt, DepartmentTrans, TreatmentFees, heinServiceType, sereServKTC, SereServs, Treatment, HeinServiceTypes, Rooms, Services, departments, singleKeyValue, CurrentDepartmentId ?? 0);

                PrintCustomShow<Mps000128PDO> printShow = new PrintCustomShow<Mps000128PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SereServKTCChoosed(object data)
        {
            if (data != null)
                sereServKTC = data as HIS_SERE_SERV;
        }
    }
}
