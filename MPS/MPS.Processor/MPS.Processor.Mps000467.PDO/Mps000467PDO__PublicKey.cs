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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000467.PDO
{
    /// <summary>
    /// Chia se key
    /// </summary>
    public partial class Mps000467PDO : RDOBase
    {
        public List<Mps000467_ListSereServ> SereServs { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        //public HIS_SERVICE_REQ testServiceReq { get; set; }
        public HIS_TREATMENT CurrentHisTreatment { get; set; }  // được phép ghi đè
        public V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }    // k ghi đè đc
        public Mps000467ADO Mps000467ADO { get; set; }
        public V_HIS_BED_LOG BedLog { get; set; }
        public V_HIS_SERVICE ServiceParent { get; set; }
        public HIS_DHST _HIS_DHST { get; set; }
        public HIS_WORK_PLACE _HIS_WORK_PLACE { get; set; }
        public List<V_HIS_SERVICE> ListService { get; set; }

        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public LIS.EFMODEL.DataModels.V_LIS_SAMPLE LisSample { get; set; }
    }

    public class Mps000467ADO
    {
        public string bebRoomName { get; set; }
        public string firstExamRoomName { get; set; }
        public decimal ratio { get; set; }
        public long PatientTypeId__Bhyt { get; set; }
        public string PARENT_NAME { get; set; }
        public long? CURRENT_EXECUTE_ROOM_NUM_ORDER { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }

    public class Mps000467_ListSereServ : V_HIS_SERE_SERV
    {
        //public string ACTIVE_INGR_BHYT_CODE { get; set; }
        //public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string HEIN_ORDER { get; set; }
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }
        public long? SERVICE_ORDER { get; set; }
        public long? SERVICE_PARENT_ORDER { get; set; }

        public string CONCLUDE { get; set; }
        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string NOTE { get; set; }
        //public string SERVICE_REQ_CODE { get; set; }
        //public string TREATMENT_CODE { get; set; }
        public decimal? ESTIMATE_DURATION { get; set; }

        public string SERVICE_CONDITION_CODE { get; set; }
        public string SERVICE_CONDITION_NAME { get; set; }

        public Mps000467_ListSereServ() { }

        public Mps000467_ListSereServ(V_HIS_SERE_SERV data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_SERE_SERV>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(data));
                    }

                    //this.ACTIVE_INGR_BHYT_CODE = data.TDL_ACTIVE_INGR_BHYT_CODE;
                    //this.ACTIVE_INGR_BHYT_NAME = data.TDL_ACTIVE_INGR_BHYT_NAME;
                    this.HEIN_ORDER = data.TDL_HEIN_ORDER;
                    this.HEIN_SERVICE_BHYT_CODE = data.TDL_HEIN_SERVICE_BHYT_CODE;
                    this.HEIN_SERVICE_BHYT_NAME = data.TDL_HEIN_SERVICE_BHYT_NAME;
                    this.REQUEST_LOGINNAME = data.TDL_REQUEST_LOGINNAME;
                    this.REQUEST_USERNAME = data.TDL_REQUEST_USERNAME;
                    this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                    this.SERVICE_NAME = data.TDL_SERVICE_NAME;
                    //this.SERVICE_REQ_CODE = data.TDL_SERVICE_REQ_CODE;
                    //this.TREATMENT_CODE = data.TDL_TREATMENT_CODE;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000467_ListSereServ(V_HIS_SERE_SERV_11 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(this, data);

                    //this.ACTIVE_INGR_BHYT_CODE = data.TDL_ACTIVE_INGR_BHYT_CODE;
                    //this.ACTIVE_INGR_BHYT_NAME = data.TDL_ACTIVE_INGR_BHYT_NAME;
                    this.HEIN_ORDER = data.TDL_HEIN_ORDER;
                    this.HEIN_SERVICE_BHYT_CODE = data.TDL_HEIN_SERVICE_BHYT_CODE;
                    this.HEIN_SERVICE_BHYT_NAME = data.TDL_HEIN_SERVICE_BHYT_NAME;
                    this.REQUEST_LOGINNAME = data.TDL_REQUEST_LOGINNAME;
                    this.REQUEST_USERNAME = data.TDL_REQUEST_USERNAME;
                    this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                    this.SERVICE_NAME = data.TDL_SERVICE_NAME;
                    //this.SERVICE_REQ_CODE = data.TDL_SERVICE_REQ_CODE;
                    //this.TREATMENT_CODE = data.TDL_TREATMENT_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}