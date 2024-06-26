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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignServiceEdit.Base
{
    class GlobalStore
    {
        public const string HIS_SERE_SERV_GETVIEW_1 = "api/HisSereServ/GetView1";
        public const string HIS_SERE_SERV_GET = "api/HisSereServ/Get";
        public static short HIS_SERE_SERV_IS_EXPEND = 1;
        public static int SERE_SERV_TYPE = 3;
        public static long SERVICE_ROOM_TYPE = 4;

        public static Dictionary<long, List<long>> MAPPING = new Dictionary<long, List<long>>()
        {
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN, new List<long>{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT}},
           {IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN, new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN}}
        };

        private static List<V_HIS_PATIENT_TYPE_ALLOW> vpatientTypeAllows;
        public static List<V_HIS_PATIENT_TYPE_ALLOW> HisVPatientTypeAllows
        {
            get
            {
                return BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>().OrderByDescending(o => o.CREATE_TIME).ToList();
            }
        }

        private static List<HIS_PATIENT_TYPE> patientTypes;
        public static List<HIS_PATIENT_TYPE> HisPatientTypes
        {
            get
            {
                return BackendDataWorker.Get<HIS_PATIENT_TYPE>().OrderByDescending(o => o.CREATE_TIME).ToList();
            }
        }

        private static List<V_HIS_SERVICE_PATY> vservicePatys;
        public static List<V_HIS_SERVICE_PATY> HisVServicePatys
        {
            get
            {
                return BackendDataWorker.Get<V_HIS_SERVICE_PATY>().OrderByDescending(o => o.CREATE_TIME).ToList();
            }
        }
    }
}
