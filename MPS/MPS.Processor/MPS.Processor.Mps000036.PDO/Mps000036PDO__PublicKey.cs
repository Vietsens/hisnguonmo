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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000036.PDO
{
    public partial class Mps000036PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER PatyAlter { get; set; }
        public V_HIS_SERVICE_REQ ServiceReq { get; set; }
        public List<V_HIS_SERE_SERV> SereServs { get; set; }
        public HIS_TREATMENT Treatment { get; set; }
        public List<Mps000036_ListSereServ> sereServs11 { get; set; }
        public SingleKeyValue SingleKeyValue { get; set; }
        public V_HIS_BED_LOG BedLog { get; set; }
        public HIS_DHST _HIS_DHST { get; set; }
        public HIS_WORK_PLACE _HIS_WORK_PLACE { get; set; }
    }

    public class PatientADO
    {
        public string VIR_ADDRESS { get; set; }
        public string VIR_PATIENT_NAME { get; set; }
        public long DOB { get; set; }
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }
    }

    public class PatyAlterBhytADO : V_HIS_PATIENT_TYPE_ALTER
    {
        public string HEIN_CARD_NUMBER_SEPARATE { get; set; }
        public string IS_HEIN { get; set; }
        public string IS_VIENPHI { get; set; }
        public string STR_HEIN_CARD_FROM_TIME { get; set; }
        public string STR_HEIN_CARD_TO_TIME { get; set; }
        public string RATIO { get; set; }
        public string HEIN_CARD_NUMBER_1 { get; set; }
        public string HEIN_CARD_NUMBER_2 { get; set; }
        public string HEIN_CARD_NUMBER_3 { get; set; }
        public string HEIN_CARD_NUMBER_4 { get; set; }
        public string HEIN_CARD_NUMBER_5 { get; set; }
        public string HEIN_CARD_NUMBER_6 { get; set; }
        public long TIME_IN_TREATMENT { get; set; }
    }

    public class Mps000036_ListSereServ : V_HIS_SERE_SERV
    {
        public string HEIN_ORDER { get; set; }
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }

        public string CONCLUDE { get; set; }
        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string NOTE { get; set; }
        public decimal? ESTIMATE_DURATION { get; set; }
        public long? PTTT_GROUP_ID { get; set; }

        public Mps000036_ListSereServ() { }

        public Mps000036_ListSereServ(V_HIS_SERE_SERV data)
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

                    this.HEIN_ORDER = data.TDL_HEIN_ORDER;
                    this.HEIN_SERVICE_BHYT_CODE = data.TDL_HEIN_SERVICE_BHYT_CODE;
                    this.HEIN_SERVICE_BHYT_NAME = data.TDL_HEIN_SERVICE_BHYT_NAME;
                    this.REQUEST_LOGINNAME = data.TDL_REQUEST_LOGINNAME;
                    this.REQUEST_USERNAME = data.TDL_REQUEST_USERNAME;
                    this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                    this.SERVICE_NAME = data.TDL_SERVICE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000036_ListSereServ(V_HIS_SERE_SERV_11 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(this, data);

                    this.HEIN_ORDER = data.TDL_HEIN_ORDER;
                    this.HEIN_SERVICE_BHYT_CODE = data.TDL_HEIN_SERVICE_BHYT_CODE;
                    this.HEIN_SERVICE_BHYT_NAME = data.TDL_HEIN_SERVICE_BHYT_NAME;
                    this.REQUEST_LOGINNAME = data.TDL_REQUEST_LOGINNAME;
                    this.REQUEST_USERNAME = data.TDL_REQUEST_USERNAME;
                    this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                    this.SERVICE_NAME = data.TDL_SERVICE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
