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
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000127.PDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000124
{
    public class Mps000127Behavior : MpsDataBase, ILoad
    {
        internal HIS_SERE_SERV sereServKTC;
        internal List<HIS_SERE_SERV> sereServKTCPrints { get; set; }
        internal List<HIS_SERE_SERV> sereServPrints { get; set; }
        internal List<HIS_SERE_SERV_EXT> sereServExts { get; set; }

        public Mps000127Behavior(long? roomId, V_HIS_PATIENT _patient, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _userNameReturnResult, long? _currentDepartmentId)
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
            this.CurrentDepartmentId = _currentDepartmentId;
            this.Patient = _patient;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            try
            {
                ServiceTypeCFG serviceTypeCFG = new ServiceTypeCFG();
                serviceTypeCFG.SERVICE_TYPE_ID__BED = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G;
                serviceTypeCFG.SERVICE_TYPE_ID__BLOOD = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                serviceTypeCFG.SERVICE_TYPE_ID__DIIM = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA;
                serviceTypeCFG.SERVICE_TYPE_ID__ENDO = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS;
                serviceTypeCFG.SERVICE_TYPE_ID__EXAM = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH;
                serviceTypeCFG.SERVICE_TYPE_ID__FUEX = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN;
                serviceTypeCFG.SERVICE_TYPE_ID__MATE = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                serviceTypeCFG.SERVICE_TYPE_ID__MEDI = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                serviceTypeCFG.SERVICE_TYPE_ID__MISU = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT;
                serviceTypeCFG.SERVICE_TYPE_ID__PAAN = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL;
                serviceTypeCFG.SERVICE_TYPE_ID__REHA = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN;
                serviceTypeCFG.SERVICE_TYPE_ID__SUIM = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA;
                serviceTypeCFG.SERVICE_TYPE_ID__SURG = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT;
                serviceTypeCFG.SERVICE_TYPE_ID__TEST = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;

                HIS_SERE_SERV sereServ = this.SereServs.Where(o => o.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && !String.IsNullOrEmpty(o.JSON_PATIENT_TYPE_ALTER)).OrderByDescending(o => o.TDL_INTRUCTION_TIME).FirstOrDefault();
                HIS_PATIENT_TYPE_ALTER patyBhyt = null;
                if (sereServ != null)
                    patyBhyt = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(sereServ.JSON_PATIENT_TYPE_ALTER);

                //var parentIds = this.SereServs.Select(o => o.PARENT_ID).ToList();
                var parentIds = this.SereServs
                    .Where(o => o.PARENT_ID != null)   
                    .Select(o => o.PARENT_ID)          
                    .Distinct()                      
                    .ToList();     
                var sereServKTCs = this.SereServs.Where(o => (o.TDL_SERVICE_TYPE_ID == serviceTypeCFG.SERVICE_TYPE_ID__SURG
                    || parentIds.Contains(o.ID)
                    && (o.TDL_SERVICE_TYPE_ID == serviceTypeCFG.SERVICE_TYPE_ID__DIIM))
                    && o.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.IS_EXPEND != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                   ).ToList();

                sereServKTC = null;
                sereServKTCPrints = new List<HIS_SERE_SERV>();
                frmBordereauChooseDepartment frm = new frmBordereauChooseDepartment(this.SereServs, sereServKTCs, this.CurrentDepartmentId ?? 0, delegateLoadData);
                frm.ShowDialog();

                int type = 0;
                if (sereServKTC == null)
                {
                    var sereServKTCsNew = this.SereServs.Where(o => (o.TDL_SERVICE_TYPE_ID == serviceTypeCFG.SERVICE_TYPE_ID__SURG)).ToList();
                    sereServKTCPrints = sereServKTCsNew;
                    type = 2;
                }
                else
                {
                    sereServKTCPrints.Add(sereServKTC);
                    type = 1;
                }

                //Lay danh sach service report,execute group
                var heinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                var executeGroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_GROUP>();
                var rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                SingleKeyValue singleKeyValue = new SingleKeyValue();
                singleKeyValue.DepartmentId = CurrentDepartmentId ?? 0;
                singleKeyValue.DepartmentName = DepartmentName;
                singleKeyValue.StatusTreatmentOut = StatusTreatmentOut;
                singleKeyValue.Today = TotalDayTreatment;
                singleKeyValue.Type = type;
                singleKeyValue.UserNameReturnResult = UserNameReturnResult;

                List<V_HIS_EKIP_USER> EkipUserViews = new List<V_HIS_EKIP_USER>();
                if (sereServKTC != null && this.sereServKTC.EKIP_ID.HasValue)
                {
                    HisEkipUserViewFilter EkipUserViewFilter = new HisEkipUserViewFilter();
                    EkipUserViewFilter.EKIP_ID = this.sereServKTC.EKIP_ID;
                    EkipUserViews = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, EkipUserViewFilter, param);
                }
                List<V_HIS_MATERIAL_TYPE> MaterialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().ToList();

                MPS.Processor.Mps000127.PDO.Mps000127PDO rdo = new MPS.Processor.Mps000127.PDO.Mps000127PDO(Patient, patyBhyt, sereServKTCPrints, sereServPrints, DepartmentTrans, Treatment, executeGroups, serviceTypeCFG, heinServiceTypes, rooms, Services, singleKeyValue, EkipUserViews, this.sereServExts, MaterialTypes);

                PrintCustomShow<Mps000127PDO> printShow = new PrintCustomShow<Mps000127PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void delegateLoadData(List<HIS_SERE_SERV> _sereServs, HIS_SERE_SERV _servServKTC, HIS_DEPARTMENT departmentResult, List<HIS_SERE_SERV_EXT> _sereServExts)
        {
            try
            {
                this.sereServKTC = _servServKTC;
                sereServPrints = _sereServs;
                this.sereServExts = _sereServExts;
                this.DepartmentName = departmentResult != null ? departmentResult.DEPARTMENT_NAME : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
